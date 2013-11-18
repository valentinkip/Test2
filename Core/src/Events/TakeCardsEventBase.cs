using System.IO;

namespace ManchkinQuest.Core.Events
{
  public abstract class TakeCardsEventBase : IGameEvent
  {
    private string myPlayerName;
    private int myCardCount;

    protected TakeCardsEventBase()
    {
    }

    protected TakeCardsEventBase(IPlayer player, int cardCount)
    {
      myPlayerName = player.Name;
      myCardCount = cardCount;
    }

    protected abstract ICard TakeCard(IGame game);

    public void Apply(IGame game)
    {
      IPlayer player = game.PlayerByName(myPlayerName);
      for(int i = 0; i < myCardCount; i++)
        player.HandCards.Add(TakeCard(game));
    }

    public void WriteTo(BinaryWriter writer)
    {
      writer.Write(myPlayerName);
      writer.Write(myCardCount);
    }

    public void ReadFrom(BinaryReader reader)
    {
      myPlayerName = reader.ReadString();
      myCardCount = reader.ReadInt32();
    }
  }
}