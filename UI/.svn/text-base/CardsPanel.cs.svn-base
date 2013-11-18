using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Linq;
using ManchkinQuest.Core;

namespace ManchkinQuest.UI
{
  class CardsPanel : Panel
  {
    private const double SpaceX = 0.2;
    private const double SpaceY = 0.1;

    private readonly IList<ICard> myCards;
    private readonly Border myBorder;
    private readonly CardZoomInteractor myAutoZoomInteractor;

    public CardsPanel(IList<ICard> cards)
    {
      myCards = cards;

      Background = new SolidColorBrush(Colors.White);

      Children.Add(myBorder = new Border { BorderBrush = new SolidColorBrush(Colors.Black), BorderThickness = new Thickness(3) });

      foreach(ICard card in myCards)
      {
        var cardElement = new CardElement(card);
        Children.Add(cardElement);
        cardElement.MouseEnter += (sender, e) => myAutoZoomInteractor.OnElementMouseEnter(cardElement);
      }

      myAutoZoomInteractor = new CardZoomInteractor(this, false) {ZoomDirection = ZoomDirection.Up};
    }

    protected override Size MeasureOverride(Size availableSize)
    {
      double startX;
      double startY;
      Size requiredSize;
      Size cardSize = CalcCardSize(availableSize, out requiredSize, out startX, out startY);
      foreach(CardElement cardElement in Children.OfType<CardElement>())
        cardElement.Measure(cardSize);
      return requiredSize;
    }

    protected override Size ArrangeOverride(Size finalSize)
    {
      double x;
      double y;
      Size requiredSize;
      Size cardSize = CalcCardSize(finalSize, out requiredSize, out x, out y);
      foreach (CardElement cardElement in Children.OfType<CardElement>())
      {
        cardElement.Arrange(new Rect(new Point(x, y), cardSize));
        x += cardSize.Width*(1 + SpaceX);
      }
      myBorder.Arrange(new Rect(0, 0, finalSize.Width, finalSize.Height));
      return finalSize;
    }

    private Size CalcCardSize(Size panelSize, out Size requiredSize, out double startX, out double startY)
    {
      double nx = myCards.Count * (1 + SpaceX) + SpaceX;
      const double ny = 1 / (1 - 2 * SpaceY);
      double maxW = Math.Min(panelSize.Width/nx, 100);
      double maxH = panelSize.Height / ny;
      var cardSize = maxW/maxH > StuffManager.CardImageWidthToHeight
                       ? new Size(maxH*StuffManager.CardImageWidthToHeight, maxH)
                       : new Size(maxW, maxW/StuffManager.CardImageWidthToHeight);
      requiredSize = new Size(cardSize.Width * nx, cardSize.Height * ny);
      startX = (panelSize.Width - requiredSize.Width) / 2 + cardSize.Width * SpaceX;
      startY = (panelSize.Height - requiredSize.Height) / 2 + cardSize.Height * SpaceY;
      return cardSize;
    }
  }
}