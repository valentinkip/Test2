using JetBrains.Annotations;

namespace ManchkinQuest.Core.Events
{
  public class TakeDxMCardsEvent : TakeCardsEventBase
  {
    [UsedImplicitly]
    private TakeDxMCardsEvent()
    {
    }

    public TakeDxMCardsEvent(IPlayer player, int cardCount) : base(player, cardCount)
    {
    }

    protected override ICard TakeCard(IGame game)
    {
      return game.DxMCardDeck.TakeCard();
    }
  }
}