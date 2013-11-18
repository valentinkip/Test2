using System;
using System.Collections.Generic;

namespace ManchkinQuest.Core.Impl
{
  class CardDeck<TCard> : ICardDeck<TCard> where TCard : class, ICard
  {
    private readonly Random myRandom;

    public CardDeck(IEnumerable<TCard> allCards, int seed)
    {
      myRandom = new Random(seed);
      Cards = new List<TCard>(allCards);
      ShuffleCards(Cards);
      Discards = new List<TCard>();
    }

    public IList<TCard> Cards { get; private set; }
    public IList<TCard> Discards { get; private set; }

    public TCard TakeCard()
    {
      if (Cards.Count == 0)
      {
        if (Discards.Count == 0) return null; // TODO?
        Cards = Discards;
        ShuffleCards(Cards);
        Discards = new List<TCard>();
      }
      TCard card = Cards[0];
      Cards.RemoveAt(0);
      return card;
    }

    public void DiscardCard(TCard card)
    {
      Discards.Add(card);
    }

    private void ShuffleCards(IList<TCard> cards)
    {
      for(int i = 0; i < cards.Count; i++)
      {
        int newPos = myRandom.Next(0, cards.Count);
        TCard card1 = cards[newPos];
        TCard card2 = cards[i];
        cards[newPos] = card2;
        cards[i] = card1;
      }
    }
  }
}