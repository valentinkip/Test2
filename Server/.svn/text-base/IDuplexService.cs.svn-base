using System;
using System.ServiceModel;
using ManchkinQuest.Common;

namespace ManchkinQuest.Server
{
  [ServiceContract(Namespace = "Silverlight", CallbackContract = typeof(IDuplexClient))]
  public interface IDuplexService
  {
    [OperationContract]
    string StartLogin(string login);

    [OperationContract]
    AuthorizeLoginResult AuthorizeLogin(string textEncoded);

    [OperationContract]
    CreateNewGameResult CreateNewGame(string gameName, string gamePasswordEncoded, PlayerInfo[] players);

    [OperationContract]
    EndGameResult EndGame();

    [OperationContract]
    string StartAuthorizePlayer(string gameName, string playerName);

    [OperationContract]
    AuthorizePlayerResult AuthorizePlayer(string textEncoded, int gameEventsCounter, out PlayerInfo[] players);

    [OperationContract]
    ChangePlayerPasswordResult ChangePlayerPassword(string newPasswordEncoded);

    [OperationContract]
    SendEventsResult SendGameEvents(string[] events);

    [OperationContract]
    void GameEventsRecieved(int newEventsCounter);
  }

  [ServiceContract]
  public interface IDuplexClient
  {
    [OperationContract(IsOneWay = true)]
    void ReceiveGameEvents(string[] events, bool initialization);

    /// <summary>
    /// Is sent to one of players (arbitrary one) for him to send initializing events to all
    /// </summary>
    [OperationContract(IsOneWay = true)]
    void StartGame();

    [OperationContract(IsOneWay = true)]
    void ReadyStatusChange(bool isReady);
  }

  [Serializable]
  public class PlayerInfo
  {
    public string Name { get; private set; }
    public PlayerColor Color { get; private set; }
    public Sex Sex { get; private set; }

    public PlayerInfo(string name, PlayerColor color, Sex sex)
    {
      Name = name;
      Color = color;
      Sex = sex;
    }
  }

  public enum AuthorizeLoginResult
  {
    Ok,
    NoLoginStarted,
    WrongNameOrPassword
  }

  public enum CreateNewGameResult
  {
    Ok,
    LoginRequired,
    YouHaveActiveGame,
    GameNameInUse
  }

  public enum EndGameResult
  {
    Ok,
    LoginRequired,
    YouHaveNoActiveGame
  }

  public enum AuthorizePlayerResult
  {
    Ok,
    NoAuthorizationStarted,
    NoSuchGame,
    NoSuchPlayer,
    WrongPassword
  }

  public enum ChangePlayerPasswordResult
  {
    Ok,
    NotRegistered
  }

  public enum SendEventsResult
  {
    Ok, 
    NotRegistered,
    NotReady
  }
}
