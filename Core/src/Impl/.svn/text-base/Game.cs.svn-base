using System.Collections.Generic;
using System.Linq;

namespace ManchkinQuest.Core.Impl
{
  public class Game : IGame
  {
    public Game(IList<IPlayer> players, IPlayer activePlayer)
    {
      Players = players;
      ActivePlayer = activePlayer;
      Maze = new Maze();

      foreach(IPlayer player in players)
        player.Location = Maze.Enter;
    }

    public IList<IPlayer> Players { get; private set; }
    public IPlayer ActivePlayer { get; private set; }

    public IPlayer PlayerByName(string name)
    {
      return Players.First(player => player.Name == name);
    }

    public IMaze Maze { get; private set; }

    public ICardDeck<IDxMCard> DxMCardDeck { get; private set; }
    public ICardDeck<ITreasureCard> TreasureCardDeck { get; private set; }
    public ICardDeck<IMonsterCard> MonsterCardDeck { get; private set; }

    public void InitializeDecks(int seed)
    {
      StuffManager stuffManager = StuffManager.Instance;
      DxMCardDeck = new CardDeck<IDxMCard>(stuffManager.DxMCards, seed);
      TreasureCardDeck = new CardDeck<ITreasureCard>(stuffManager.TreasureCards, seed);
      MonsterCardDeck = new CardDeck<IMonsterCard>(stuffManager.MonsterCards, seed);
    }
  }
}