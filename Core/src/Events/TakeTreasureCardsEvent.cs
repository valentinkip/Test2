using JetBrains.Annotations;

namespace ManchkinQuest.Core.Events
{
  public class TakeTreasureCardsEvent : TakeCardsEventBase
  {
    [UsedImplicitly]
    private TakeTreasureCardsEvent()
    {
    }

    public TakeTreasureCardsEvent(IPlayer player, int cardCount) : base(player, cardCount)
    {
    }

    protected override ICard TakeCard(IGame game)
    {
      return game.TreasureCardDeck.TakeCard();
    }
  }
}