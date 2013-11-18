using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using ManchkinQuest.Common;
using ManchkinQuest.Core;
using ManchkinQuest.Core.Events;
using ManchkinQuest.Core.Impl;
using ManchkinQuest.UI.DuplexService;

namespace ManchkinQuest.UI
{
  public partial class App : Application
  {

    public App()
    {
      this.Startup += this.Application_Startup;
      this.Exit += this.Application_Exit;
      this.UnhandledException += this.Application_UnhandledException;

      InitializeComponent();
    }

    private void Application_Startup(object sender, StartupEventArgs e)
    {
      var rootVisual = new UserControl();
      RootVisual = rootVisual;
      var gameTablePanel = new GameTablePanel();
      rootVisual.Content = gameTablePanel;
      new UIManagerImpl(gameTablePanel);

      gameTablePanel.SetGame(CreateSampleGame());
      gameTablePanel.Update();

/*
      mainPage.StatusBar.Text = "Связываемся с сервером...";

      var serverCommunicator = new ServerCommunicator();
      serverCommunicator.Login("valentin", "Egypt",
        delegate(bool success, string errorMessage)
          {
            if (success)
            {
              var playerInfo1 = new PlayerInfo {Namek__BackingField = "Player 1", Colork__BackingField = PlayerColor.Blue, Sexk__BackingField = Sex.Man};
              var playerInfo2 = new PlayerInfo {Namek__BackingField = "Player 2", Colork__BackingField = PlayerColor.Red, Sexk__BackingField = Sex.Woman};
              const string gameName = "TestGame";
              const string gamePassword = "TestGamePassword";
              serverCommunicator.CreateNewGame(gameName, gamePassword, new[]{playerInfo1, playerInfo2}, 
                delegate(bool success1, string errorMessage1)
                  {
                    if (success1)
                      mainPage.StatusBar.Text = errorMessage1;
                    string activePlayerName = success1 ? "Player 1" : "Player 2";
                    ServerCommunicator.EnterGameCompletedHandler handler = delegate(bool success2, string errorMessage2, ICollection<PlayerInfo> players)
                    {
                      if (success)
                      {
                        mainPage.StatusBar.Text = "ОК";
                        IGame game = CreateGame(players, activePlayerName);
                        mainPage.SetGame(game);
                        mainPage.Update();
                        serverCommunicator.GameEventsApplied += mainPage.Update;
                        serverCommunicator.Game = game;
                      }
                      else
                      {
                        mainPage.StatusBar.Text = errorMessage2;
                      }
                    };
                    serverCommunicator.EnterGame(gameName, activePlayerName, gamePassword, handler, () => StartGame(serverCommunicator));
                  });
            }
            else
            {
              mainPage.StatusBar.Text = errorMessage;
            }
          }
        );
*/
    }

    private static void StartGame(ServerCommunicator serverCommunicator)
    {
      IGame game = serverCommunicator.Game;
      var events = new List<IGameEvent> { new InitializeDecksEvent(new Random().Next()) };
      foreach (IPlayer player in game.Players)
      {
        events.Add(new TakeDxMCardsEvent(player, 3));
        events.Add(new TakeTreasureCardsEvent(player, 3));
      }
      serverCommunicator.SendAndApplyEvents(events);
    }

    private static IGame CreateGame(IEnumerable<PlayerInfo> playerInfos, string activePlayerName)
    {
      IList<IPlayer> players = playerInfos.Select(info => new Player(info.Namek__BackingField, info.Colork__BackingField, info.Sexk__BackingField)).Cast<IPlayer>().ToList();
      IPlayer activePlayer = players.Where(player => player.Name == activePlayerName).First();
      return new Game(players, activePlayer);
    }

    private static IGame CreateSampleGame()
    {
      Player player1 = CreatePlayer("Player 1", PlayerColor.Blue, Sex.Woman, 1);
      Player player2 = CreatePlayer("Player 2", PlayerColor.Yellow, Sex.Man, 2);
      Player player3 = CreatePlayer("Player 3", PlayerColor.Green, Sex.Woman, 3);
      Player player4 = CreatePlayer("Player 4", PlayerColor.Red, Sex.Man, 4);

      var game = new Game(new[] { player1, player2, player3, player4 }, player2);
      StuffManager stuffManager = StuffManager.Instance;

      IMazeRoomInfo roomInfo1 = game.Maze.AddRoom(1, 0, stuffManager.RoomCards[0].Side1);
      roomInfo1.SetLink(Direction.Up, stuffManager.OpenPassageway);
      roomInfo1.SetLink(Direction.Down, stuffManager.RegularDoor);
      roomInfo1.SetLink(Direction.Right, stuffManager.LockedDoor);

      IMazeRoomInfo roomInfo2 = game.Maze.AddRoom(2, 0, stuffManager.RoomCards[1].Side1);
      roomInfo2.SetLink(Direction.Up, stuffManager.HiddenDoor);
      roomInfo2.SetLink(Direction.Down, stuffManager.Wall);
      roomInfo2.SetLink(Direction.Right, stuffManager.OpenPassageway);

      IMazeRoomInfo roomInfo3 = game.Maze.AddRoom(1, 1, stuffManager.RoomCards[1].Side2);
      roomInfo3.SetLink(Direction.Left, stuffManager.HiddenDoor);
      roomInfo3.SetLink(Direction.Right, stuffManager.Wall);
      roomInfo3.SetLink(Direction.Down, stuffManager.OpenPassageway);

      IMazeRoomInfo roomInfo4 = game.Maze.AddRoom(-1, 0, stuffManager.RoomCards[0].Side2);
      roomInfo4.SetLink(Direction.Up, stuffManager.HiddenDoor);
      roomInfo4.SetLink(Direction.Down, stuffManager.Wall);
      roomInfo4.SetLink(Direction.Left, stuffManager.OpenPassageway);

      player1.Location = roomInfo1;
      player2.Location = roomInfo1;
      player3.Location = roomInfo1;
      player4.Location = roomInfo1;

      return game;
    }

    private static Player CreatePlayer(string name, PlayerColor color, Sex sex, int n)
    {
      var player = new Player(name, color, sex);
      int i = 0;
      StuffManager stuffManager = StuffManager.Instance;
      foreach (IDxMCard dxMCard in stuffManager.DxMCards)
      {
        if (dxMCard is ICheatCard) continue;
        if (dxMCard is IRaceCard && player.RaceCards.Count < 2)
          player.RaceCards.Add((IRaceCard)dxMCard);
        else if (dxMCard is IClassCard && player.ClassCards.Count < 2)
          player.ClassCards.Add((IClassCard)dxMCard);
        else if (dxMCard is IHalfBreededCard)
          player.HalfBreededCard = (IHalfBreededCard)dxMCard;
        else if (dxMCard is ISuperManchkinCard)
          player.SuperManchkinCard = (ISuperManchkinCard)dxMCard;
        else
          player.HandCards.Add(dxMCard);
      }
      foreach (ITreasureCard treasureCard in stuffManager.TreasureCards)
      {
        ICheatCard cheatCard = i++ == 4 ? stuffManager.DxMCards.OfType<ICheatCard>().Single() : null;
        player.WornItems.Add(Tuple.Create(treasureCard, cheatCard));
      }
      /*
            for (int j = 0; j < n * 2; j++)
              player.HandCards.Add(cardManager.TreasureCards.First());
      */
      return player;
    }

    private void Application_Exit(object sender, EventArgs e)
    {

    }

    private void Application_UnhandledException(object sender, ApplicationUnhandledExceptionEventArgs e)
    {
      // If the app is running outside of the debugger then report the exception using
      // the browser's exception mechanism. On IE this will display it a yellow alert 
      // icon in the status bar and Firefox will display a script error.
      if (!System.Diagnostics.Debugger.IsAttached)
      {

        // NOTE: This will allow the application to continue running after an exception has been thrown
        // but not handled. 
        // For production applications this error handling should be replaced with something that will 
        // report the error to the website and stop the application.
        e.Handled = true;
        Deployment.Current.Dispatcher.BeginInvoke(delegate { ReportErrorToDOM(e); });
      }
    }

    private void ReportErrorToDOM(ApplicationUnhandledExceptionEventArgs e)
    {
      try
      {
        string errorMsg = e.ExceptionObject.Message + e.ExceptionObject.StackTrace;
        errorMsg = errorMsg.Replace('"', '\'').Replace("\r\n", @"\n");

        System.Windows.Browser.HtmlPage.Window.Eval("throw new Error(\"Unhandled Error in Silverlight Application " + errorMsg + "\");");
      }
      catch (Exception)
      {
      }
    }
  }
}
