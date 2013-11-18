using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.ServiceModel;
using System.ServiceModel.Channels;
using ManchkinQuest.Common;
using ManchkinQuest.Core;
using ManchkinQuest.UI.DuplexService;
using System.Linq;

namespace ManchkinQuest.UI
{
  public class ServerCommunicator
  {
    public delegate void OperationCompletedHandler(bool success, string errorMessage);
    public delegate void EnterGameCompletedHandler(bool success, string errorMessage, ICollection<PlayerInfo> players);

    private readonly DuplexServiceClient myProxy;

    private string myPassword = null;
    private OperationCompletedHandler myLoginCompletedHandler = null;
    private string myGameToCreateName;
    private OperationCompletedHandler myCreateNewGameCompletedHandler = null;
    private string myGamePassword;
    private EnterGameCompletedHandler myEnterGameCompletedHandler = null;

    private int myRecievedEventsCounter;
    private bool myIsReady = false;

    private IGame myGame;
    private Action myStartGameHandler;
    private bool myStartGameOnGameSet = false;

    public ServerCommunicator()
    {
      //TODO: exceptions handling!

      var address = new EndpointAddress("http://localhost:4379/DuplexService.svc");
      var binding = new PollingDuplexHttpBinding(PollingDuplexMode.MultipleMessagesPerPoll);
      myProxy = new DuplexServiceClient(binding, address);

      myProxy.StartLoginCompleted += StartLoginCompleted;
      myProxy.AuthorizeLoginCompleted += AuthorizeLoginCompleted;
      myProxy.CreateNewGameCompleted += CreateNewGameCompleted;
      myProxy.StartAuthorizePlayerCompleted += StartAuthorizePlayerCompleted;
      myProxy.AuthorizePlayerCompleted += AuthorizePlayerCompleted;

      myProxy.ReceiveGameEventsReceived += ReceiveGameEventsReceived;
      myProxy.ReadyStatusChangeReceived += ReadyStatusChangeReceived;
      myProxy.StartGameReceived += (sender, args) =>
                                     {
                                       if (Game != null)
                                         myStartGameHandler();
                                       else
                                         myStartGameOnGameSet = true;
                                     };
    }

    private void StartLoginCompleted(object sender, StartLoginCompletedEventArgs args)
    {
      if (myPassword != null)
      {
        if (args.Error != null)
        {
          OperationCompletedHandler handler = myLoginCompletedHandler;
          if (handler != null)
          {
            myLoginCompletedHandler = null;
            handler(false, args.Error.Message);
          }
          return;
        }
        string encryptedText = EncryptionUtil.EncryptStringToString(args.Result, myPassword);
        myProxy.AuthorizeLoginAsync(encryptedText);
      }
    }

    private void AuthorizeLoginCompleted(object sender, AuthorizeLoginCompletedEventArgs args)
    {
      OperationCompletedHandler handler = myLoginCompletedHandler;
      if (handler != null)
      {
        myLoginCompletedHandler = null;
        if (args.Error != null)
        {
          handler(false, args.Error.Message);
        }
        else
        {
          switch(args.Result)
          {
            case AuthorizeLoginResult.Ok:
              handler(true, null);
              break;
            case AuthorizeLoginResult.WrongNameOrPassword:
              handler(false, "Неверный логин/пароль");
              break;
            case AuthorizeLoginResult.NoLoginStarted:
              handler(false, "Ошибка связи");
              break;
            default:
              throw new ArgumentOutOfRangeException();
          }
        }
      }
    }

    private void CreateNewGameCompleted(object sender, CreateNewGameCompletedEventArgs args)
    {
      OperationCompletedHandler handler = myCreateNewGameCompletedHandler;
      if (handler != null)
      {
        myCreateNewGameCompletedHandler = null;
        if (args.Error != null)
        {
          handler(false, args.Error.Message);
        }
        else
        {
          switch(args.Result)
          {
            case CreateNewGameResult.Ok:
              handler(true, null);
              break;
            case CreateNewGameResult.LoginRequired:
              handler(false, "Ошибка коммуникации");
              break;
            case CreateNewGameResult.YouHaveActiveGame:
              handler(false, "У вас уже есть активная игра");
              break;
            case CreateNewGameResult.GameNameInUse:
              handler(false, string.Format("Имя игры '{0}' занято", myGameToCreateName));
              break;
            default:
              throw new ArgumentOutOfRangeException();
          }
        }
      }
    }

    private void StartAuthorizePlayerCompleted(object sender, StartAuthorizePlayerCompletedEventArgs args)
    {
      if (myPassword != null)
      {
        if (args.Error != null)
        {
          EnterGameCompletedHandler handler = myEnterGameCompletedHandler;
          if (handler != null)
          {
            myEnterGameCompletedHandler = null;
            handler(false, args.Error.Message, null);
          }
          return;
        }
        string encryptedText = EncryptionUtil.EncryptStringToString(args.Result, myGamePassword);
        myProxy.AuthorizePlayerAsync(encryptedText, 0);
      }
    }

    private void AuthorizePlayerCompleted(object sender, AuthorizePlayerCompletedEventArgs args)
    {
      EnterGameCompletedHandler handler = myEnterGameCompletedHandler;
      if (handler != null)
      {
        myEnterGameCompletedHandler = null;
        if (args.Error != null)
        {
          handler(false, args.Error.Message, null);
        }
        else
        {
          switch(args.Result)
          {
            case AuthorizePlayerResult.Ok:
              handler(true, null, args.players);
              break;
            case AuthorizePlayerResult.NoAuthorizationStarted:
              handler(false, "Ошибка коммуникации", null);
              break;
            case AuthorizePlayerResult.NoSuchGame:
              handler(false, "Нет такой игры", null);
              break;
            case AuthorizePlayerResult.NoSuchPlayer:
              handler(false, "Нет такого игрока", null);
              break;
            case AuthorizePlayerResult.WrongPassword:
              handler(false, "Пароль неверен", null);
              break;
            default:
              throw new ArgumentOutOfRangeException();
          }
        }
      }
    }

    public void Login(string login, string password, OperationCompletedHandler completedHandler)
    {
      myPassword = password;
      myLoginCompletedHandler = completedHandler;
      myProxy.StartLoginAsync(login);
    }

    public void CreateNewGame(string gameName, string gamePassword, IEnumerable<PlayerInfo> players, OperationCompletedHandler completedHandler)
    {
      myCreateNewGameCompletedHandler = completedHandler;
      myGameToCreateName = gameName;
      string gamePasswordEncoded = EncryptionUtil.EncryptStringToString(gamePassword, myPassword);
      myProxy.CreateNewGameAsync(myGameToCreateName, gamePasswordEncoded, new ObservableCollection<PlayerInfo>(players));
    }

    public void EnterGame(string gameName, string playerName, string gamePassword, EnterGameCompletedHandler completedHandler, Action startGameHandler)
    {
      myStartGameHandler = startGameHandler;
      myGamePassword = gamePassword;
      myEnterGameCompletedHandler = completedHandler;
      myProxy.StartAuthorizePlayerAsync(gameName, playerName);
    }

    public IGame Game
    {
      get { return myGame; }
      set
      {
        myGame = value;
        if (myGame != null && myStartGameOnGameSet)
        {
          myStartGameOnGameSet = false;
          myStartGameHandler();
        }
      }
    }

    public void SendAndApplyEvents(IEnumerable<IGameEvent> events)
    {
      foreach(IGameEvent @event in events)
        @event.Apply(Game);
      if (GameEventsApplied != null)
        GameEventsApplied();

      IEnumerable<string> encrypted = events.Select(CoreUtil.SerializeEvent).Select(bytes => EncryptionUtil.EncryptBytesToString(bytes, myGamePassword));
      myProxy.SendGameEventsAsync(new ObservableCollection<string>(encrypted));
    }

    public bool IsReady
    {
      get { return myIsReady; }
      private set
      {
        if (myIsReady != value)
        {
          myIsReady = value;
          if (IsReadyChanged != null)
            IsReadyChanged();
        }
      }
    }

    public event Action GameEventsApplied;
    public event Action IsReadyChanged;

    private void ReceiveGameEventsReceived(object sender, ReceiveGameEventsReceivedEventArgs args)
    {
      Debug.Assert(Game != null);
      myRecievedEventsCounter += args.events.Count;
      myProxy.GameEventsRecievedAsync(myRecievedEventsCounter);
      foreach(string encryptedEvent in args.events)
      {
        byte[] bytes = EncryptionUtil.DecryptBytesFromString(encryptedEvent, myGamePassword);
        IGameEvent @event = CoreUtil.DeserializeEvent(bytes);
        @event.Apply(Game);
        if (GameEventsApplied != null)
          GameEventsApplied();
      }
    }

    private void ReadyStatusChangeReceived(object sender, ReadyStatusChangeReceivedEventArgs args)
    {
      IsReady = args.isReady;
    }
  }
}