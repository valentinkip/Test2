using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using ManchkinQuest.Common;

namespace ManchkinQuest.Server
{
  public class DuplexService : IDuplexService
  {
    private static readonly Dictionary<string, string> ourLoginAndPasswords = new Dictionary<string, string>();

    static DuplexService()
    {
      ourLoginAndPasswords.Add("valentin", "Egypt");
    }

    private static readonly Random myRandom = new Random();

    private static readonly Dictionary<IDuplexClient, StartLoginInfo> ourAuthorizingLoginClients = new Dictionary<IDuplexClient, StartLoginInfo>();
    private static readonly Dictionary<IDuplexClient, string> ourLoggedInClients = new Dictionary<IDuplexClient, string>(); // client to login

    private static readonly Dictionary<IDuplexClient, StartAuthorizePlayerInfo> ourAuthorizingPlayerClients = new Dictionary<IDuplexClient, StartAuthorizePlayerInfo>();
    private static readonly Dictionary<IDuplexClient, Player> ourPlayerClients = new Dictionary<IDuplexClient, Player>();
    private static readonly Dictionary<string, Game> ourGames = new Dictionary<string, Game>(); // game name to Game
    private static readonly Dictionary<string, Game> ourGamesByOwner = new Dictionary<string, Game>(); // owner login to Game
    private static readonly object ourGamesAndPlayersLock = new object();

    public string StartLogin(string login)
    {
      var client = OperationContext.Current.GetCallbackChannel<IDuplexClient>();
      string randomText = GenerateRandomText();
      lock (ourAuthorizingLoginClients)
        ourAuthorizingLoginClients[client] = new StartLoginInfo(login, randomText);
      return randomText;
    }

    public AuthorizeLoginResult AuthorizeLogin(string textEncoded)
    {
      var client = OperationContext.Current.GetCallbackChannel<IDuplexClient>();

      StartLoginInfo info;
      lock (ourAuthorizingLoginClients)
      {
        if (!ourAuthorizingLoginClients.TryGetValue(client, out info))
          return AuthorizeLoginResult.NoLoginStarted;
        ourAuthorizingLoginClients.Remove(client);
      }

      string password;
      lock (ourLoginAndPasswords)
      {
        if (!ourLoginAndPasswords.TryGetValue(info.Login, out password))
          return AuthorizeLoginResult.WrongNameOrPassword;
      }
      if (!EncryptionUtil.EncryptStringToString(info.TextToEncode, password).Equals(textEncoded))
        return AuthorizeLoginResult.WrongNameOrPassword;

      lock (ourLoggedInClients)
        ourLoggedInClients[client] = info.Login;

      return AuthorizeLoginResult.Ok;
    }

    public string StartAuthorizePlayer(string gameName, string playerName)
    {
      var client = OperationContext.Current.GetCallbackChannel<IDuplexClient>();
      string randomText = GenerateRandomText();
      lock (ourAuthorizingPlayerClients)
        ourAuthorizingPlayerClients[client] = new StartAuthorizePlayerInfo(gameName, playerName, randomText);
      return randomText;
    }

    public AuthorizePlayerResult AuthorizePlayer(string textEncoded, int gameEventsCounter, out PlayerInfo[] players)
    {
      players = null;

      var client = OperationContext.Current.GetCallbackChannel<IDuplexClient>();

      StartAuthorizePlayerInfo info;
      lock (ourAuthorizingPlayerClients)
      {
        if (!ourAuthorizingPlayerClients.TryGetValue(client, out info))
          return AuthorizePlayerResult.NoAuthorizationStarted;
        ourAuthorizingLoginClients.Remove(client);
      }

      Game game;
      Player player;
      lock (ourGamesAndPlayersLock)
      {
        if (!ourGames.TryGetValue(info.GameName, out game))
          return AuthorizePlayerResult.NoSuchGame;

        player = game.PlayerByName(info.PlayerName);
        if (player == null) return AuthorizePlayerResult.NoSuchPlayer;

        if (!EncryptionUtil.EncryptStringToString(info.TextToEncode, player.Password).Equals(textEncoded))
          return AuthorizePlayerResult.WrongPassword;

        //TODO: notify others, start game if ready
        if (player.Client != null)
          ourPlayerClients.Remove(player.Client);
        Player oldPlayer;
        if (ourPlayerClients.TryGetValue(client, out oldPlayer))
          oldPlayer.Client = null;
        player.Client = client;
        ourPlayerClients.Add(client, player);
      }

      lock(game)
        player.EventsReceived = gameEventsCounter;

      players = new PlayerInfo[game.Players.Count];
      for(int i = 0; i < players.Length; i++)
      {
        Player p = game.Players[i];
        players[i] = new PlayerInfo(p.Name, p.Color, p.Sex);
      }

      //TODO: do it later!
      lock (ourGamesAndPlayersLock)
      {
        if (game.Events.Count == 0 && game.Players.All(p => p.Client != null))
          client.StartGame();
      }

      return AuthorizePlayerResult.Ok;
    }

    public CreateNewGameResult CreateNewGame(string gameName, string gamePasswordEncoded, PlayerInfo[] players)
    {
      if (players.Length < 2 || players.Length > 4)
        throw new ArgumentException();

      var client = OperationContext.Current.GetCallbackChannel<IDuplexClient>();

      string login;
      lock(ourLoggedInClients)
      {
        if (!ourLoggedInClients.TryGetValue(client, out login))
          return CreateNewGameResult.LoginRequired;        
      }

      string password;
      if (!ourLoginAndPasswords.TryGetValue(login, out password))
        return CreateNewGameResult.LoginRequired; //?

      string gamePassword = EncryptionUtil.DecryptStringFromString(gamePasswordEncoded, password);

      lock(ourGamesAndPlayersLock)
      {
        if (ourGames.ContainsKey(gameName))
          return CreateNewGameResult.GameNameInUse;

        if (ourGamesByOwner.ContainsKey(login))
          return CreateNewGameResult.YouHaveActiveGame;

        var game = new Game(gameName, login);
        foreach(PlayerInfo playerInfo in players)
          game.AddPlayer(new Player(game, playerInfo.Name, playerInfo.Color, playerInfo.Sex, gamePassword));
        ourGames.Add(gameName, game);
        ourGamesByOwner.Add(login, game);
      }


      return CreateNewGameResult.Ok;
    }

    public EndGameResult EndGame()
    {
      var client = OperationContext.Current.GetCallbackChannel<IDuplexClient>();

      string login;
      lock (ourLoggedInClients)
      {
        if (!ourLoggedInClients.TryGetValue(client, out login))
          return EndGameResult.LoginRequired;
      }

      lock(ourGamesAndPlayersLock)
      {
        Game game;
        if (!ourGamesByOwner.TryGetValue(login, out game))
          return EndGameResult.YouHaveNoActiveGame;

        ourGames.Remove(game.Name);
        ourGamesByOwner.Remove(login);
        foreach(Player player in game.Players)
        {
          if (player.Client != null)
            ourPlayerClients.Remove(player.Client);
        }
      }

      return EndGameResult.Ok;
    }

    public ChangePlayerPasswordResult ChangePlayerPassword(string newPasswordEncoded)
    {
      var client = OperationContext.Current.GetCallbackChannel<IDuplexClient>();

      Player player;
      if (!ourPlayerClients.TryGetValue(client, out player))
        return ChangePlayerPasswordResult.NotRegistered;

      player.Password = EncryptionUtil.DecryptStringFromString(newPasswordEncoded, player.Password);

      return ChangePlayerPasswordResult.Ok;
    }

    public SendEventsResult SendGameEvents(string[] events)
    {
      var client = OperationContext.Current.GetCallbackChannel<IDuplexClient>();

      Player player;
      if (!ourPlayerClients.TryGetValue(client, out player))
        return SendEventsResult.NotRegistered;

      Game game = player.Game;
      lock(game)
      {
        if (game.Players.Any(aPlayer => aPlayer.EventsReceived != game.Events.Count))
          return SendEventsResult.NotReady;

        IEnumerable<byte[]> decryptedEvents = events.Select(s => EncryptionUtil.DecryptBytesFromString(s, player.Password));
        game.Events.AddRange(decryptedEvents);
        player.EventsReceived += events.Length; // no need to deliver these events back to the same player

        foreach (Player aPlayer in game.Players)
        {
          if (aPlayer.Client != null)
          {
            aPlayer.Client.ReadyStatusChange(false);
            if (aPlayer != player)
            {
// ReSharper disable AccessToModifiedClosure
              string[] encryptedEvents = decryptedEvents.Select(bytes => EncryptionUtil.EncryptBytesToString(bytes, aPlayer.Password)).ToArray();
// ReSharper restore AccessToModifiedClosure
              aPlayer.Client.ReceiveGameEvents(encryptedEvents, false);
            }
          }
        }
      }

      return SendEventsResult.Ok;
    }

    public void GameEventsRecieved(int newEventsCounter)
    {
      var client = OperationContext.Current.GetCallbackChannel<IDuplexClient>();

      Player player;
      if (!ourPlayerClients.TryGetValue(client, out player))
        return; //Q: do we need to notify about this?

      Game game = player.Game;
      lock(game)
      {
        player.EventsReceived = newEventsCounter;
        if (game.Players.All(aPlayer => aPlayer.EventsReceived == game.Events.Count && aPlayer.Client != null))
        {
          foreach(Player aPlayer in game.Players)
            aPlayer.Client.ReadyStatusChange(true);
        }
      }
    }

    private static string GenerateRandomText()
    {
      const int length = 10;
      var builder = new StringBuilder(length);
      lock(myRandom)
      {
        for (int i = 0; i < length; i++)
        {
          var c = (char)myRandom.Next(' ', 'z');
          builder.Append(c);
        }        
      }
      return builder.ToString();
    }

    private struct StartLoginInfo
    {
      public readonly string Login;
      public readonly string TextToEncode;

      public StartLoginInfo(string login, string textToEncode)
      {
        Login = login;
        TextToEncode = textToEncode;
      }
    }

    private struct StartAuthorizePlayerInfo
    {
      public readonly string GameName;
      public readonly string PlayerName;
      public readonly string TextToEncode;

      public StartAuthorizePlayerInfo(string gameName, string playerName, string textToEncode)
      {
        GameName = gameName;
        PlayerName = playerName;
        TextToEncode = textToEncode;
      }
    }

    private class Player
    {
      public Game Game { get; private set; }
      public string Name { get; private set; }
      public PlayerColor Color { get; private set; }
      public Sex Sex { get; private set; }
      public string Password { get; set; }
      public IDuplexClient Client { get; set; }
      public int EventsReceived { get; set; }

      public Player(Game game, string name, PlayerColor color, Sex sex, string password)
      {
        Game = game;
        Name = name;
        Color = color;
        Sex = sex;
        Password = password;
      }
    }

    private class Game
    {
      public string Name { get; private set; }
      public string OwnerLogin { get; private set; }

      private readonly List<Player> myPlayers = new List<Player>();
      private readonly List<byte[]> myEvents = new List<byte[]>();

      public Game(string gameName, string ownerLogin)
      {
        Name = gameName;
        OwnerLogin = ownerLogin;
      }

      public IList<Player> Players
      {
        get { return myPlayers; }
      }

      public List<byte[]> Events
      {
        get { return myEvents; }
      }

      public Player PlayerByName(string name)
      {
        return Players.Where(p => p.Name == name).FirstOrDefault();
      }

      public void AddPlayer(Player player)
      {
        if (player.Game != this)
          throw new InvalidOperationException();
        myPlayers.Add(player);
      }
    }
  }
}
