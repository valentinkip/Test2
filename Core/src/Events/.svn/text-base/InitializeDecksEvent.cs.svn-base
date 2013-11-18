using System.IO;
using JetBrains.Annotations;

namespace ManchkinQuest.Core.Events
{
  public class InitializeDecksEvent : IGameEvent
  {
    private int mySeed;

    [UsedImplicitly]
    private InitializeDecksEvent()
    {
    }

    public InitializeDecksEvent(int seed)
    {
      mySeed = seed;
    }

    public void Apply(IGame game)
    {
      game.InitializeDecks(mySeed);
    }

    public void WriteTo(BinaryWriter writer)
    {
      writer.Write(mySeed);
    }

    public void ReadFrom(BinaryReader reader)
    {
      mySeed = reader.ReadInt32();
    }
  }
}