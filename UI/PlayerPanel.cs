using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using ManchkinQuest.Core;

namespace ManchkinQuest.UI
{
  public enum PlayerPanelPlacement { Bottom, Top, Left, Right }
 
  public static class PlayerPanelPlacementExtensions
  {
    public static bool IsHorizontal(this PlayerPanelPlacement placement)
    {
      return placement == PlayerPanelPlacement.Top || placement == PlayerPanelPlacement.Bottom;
    }

    public static bool IsVertical(this PlayerPanelPlacement placement)
    {
      return !IsHorizontal(placement);
    }
  }

  public class PlayerPanel : Panel
  {
    private PlayerPanelPlacement myPlacement;
    private readonly CardZoomInteractor myAutoZoomInteractor;

    private readonly Dictionary<ICard, CardElement> myCardElements = new Dictionary<ICard,CardElement>();
    private HandCardsElement myHandCardsElement;
    private Border myBorder;
    private TextBlock myTitle;
    private bool myNavigated = false;

    public PlayerPanel()
    {
      myAutoZoomInteractor = new CardZoomInteractor(this, true);
    }

    public PlayerPanelPlacement Placement
    {
      get { return myPlacement; }
      set
      {
        myPlacement = value;
        myAutoZoomInteractor.PanelPlacement = myPlacement;
      }
    }

    public IPlayer Player { get; set; }
    public bool IsActivePlayer { get; set; }

    public bool Navigated
    {
      get { return myNavigated; }
      set
      {
        myNavigated = value;
        if (myBorder != null)
          myBorder.BorderThickness = new Thickness(BorderThinkness);
      }
    }

    private int BorderThinkness
    {
      get { return myNavigated ? 6 : 3; }
    }

    public void Update()
    {      
      Children.Clear();
      myCardElements.Clear();
      //myZoomedCard = null; //?
      if (Player == null) return;

      myBorder = new Border
                   {
                     BorderBrush = new SolidColorBrush(Player.Color.ToColor()),
                     BorderThickness = new Thickness(BorderThinkness),
                   };
      Children.Add(myBorder);

      myTitle = new TextBlock
                  {
                    Text = Player.Name,
                    HorizontalAlignment = HorizontalAlignment.Center,
                    Foreground = new SolidColorBrush(Player.Color.ToColor()),
                    FontSize = 16,
                    FontWeight = FontWeights.Bold
                  };
      Children.Add(myTitle);

      AddCardImages(Player.RaceCards.Cast<ICard>());
      AddCardImage(Player.HalfBreededCard);
      AddCardImages(Player.ClassCards.Cast<ICard>());
      AddCardImage(Player.SuperManchkinCard);
      foreach (Tuple<ITreasureCard, ICheatCard> tuple in Player.WornItems)
      {
        AddCardImage(tuple.Item1);
        AddCardImage(tuple.Item2);
      }
      //TODO
      //AddCardImages(myPlayer.InPackItems);

      Action navigationAction = IsActivePlayer ? DisplayHandCards : (Action)null;
      Children.Add(myHandCardsElement = new HandCardsElement(Player.HandCards, IsActivePlayer, RotationAngle) {NavigationAction = navigationAction});

      InvalidateArrange();
    }

    private void DisplayHandCards()
    {
      var element = new HandCardsElement(Player.HandCards, 
        showFaces: true, 
        rotationAngle: 0, 
        displayCardCount: false)
                      {
                        CardWidth = myHandCardsElement.CardWidth,
                        CardHeight = myHandCardsElement.CardHeight,
                      };

      UIElement rootVisual = Application.Current.RootVisual;
      var startRect = myHandCardsElement.GetRect(rootVisual);
      var endRect = new Rect(0, 0, rootVisual.RenderSize.Width, rootVisual.RenderSize.Height);
      const double endXStep = 1.1;
      double endCardWidth = rootVisual.RenderSize.Width / (Player.HandCards.Count * endXStep + 2 * (endXStep - 1));
      double endCardHeight = myHandCardsElement.CardHeight * endCardWidth / myHandCardsElement.CardWidth;
      var projection = new PlaneProjection();
      element.Projection = projection;
      myHandCardsElement.Visibility = Visibility.Collapsed;
      myHandCardsElement.NavigationAction = null;
      Cursor = Cursors.Arrow;
      Action onClosed = delegate
                          {
                            myHandCardsElement.Visibility = Visibility.Visible;
                            Timer timer = null;
                            timer = new Timer(delegate
                                                {
                                                  myHandCardsElement.NavigationAction = DisplayHandCards;
                                                  timer.Dispose();
                                                }, 
                                                null, 500, int.MaxValue);
                          };
      UIManager.Instance.ShowInPopup(element, startRect, endRect, onClosed,
                                     new AnimationInfo(element, new PropertyPath(HandCardsElement.XStepProperty), myHandCardsElement.XStep, endXStep),
                                     new AnimationInfo(element, new PropertyPath(HandCardsElement.MaxAngleProperty), myHandCardsElement.MaxAngle, 0),
                                     new AnimationInfo(element, new PropertyPath(HandCardsElement.AngleStepProperty), myHandCardsElement.AngleStep, 0),
                                     new AnimationInfo(element, new PropertyPath(HandCardsElement.CardWidthProperty), myHandCardsElement.CardWidth, endCardWidth),
                                     new AnimationInfo(element, new PropertyPath(HandCardsElement.CardHeightProperty), myHandCardsElement.CardHeight, endCardHeight),
                                     new AnimationInfo(projection, new PropertyPath(PlaneProjection.RotationXProperty), -45, 0));
    }

    private void AddCardImages(IEnumerable<ICard> cards)
    {
      foreach(ICard card in cards)     
        AddCardImage(card);
    }

    private void AddCardImage(ICard card)
    {
      if (card == null) return;
      var cardElement = new CardElement(card, RotationAngle);
      Children.Add(cardElement);
      myCardElements.Add(card, cardElement);
      cardElement.MouseEnter += (sender, e) => myAutoZoomInteractor.OnElementMouseEnter(cardElement);
    }

    protected override Size MeasureOverride(Size availableSize)
    {
      Size cardSize = CalcCardSize(availableSize);

      foreach(CardElement cardElement in myCardElements.Values)
        cardElement.Measure(cardSize);

      if (myHandCardsElement != null)
      {
        myHandCardsElement.CardWidth = cardSize.Width;
        myHandCardsElement.CardHeight = cardSize.Height;
        myHandCardsElement.Measure(availableSize);
      }

      if (myTitle != null) 
        myTitle.Measure(availableSize);

      return base.MeasureOverride(availableSize);
    }

    protected override Size ArrangeOverride(Size finalSize)
    {
      if (Player == null) return finalSize;

      Size cardSize = CalcCardSize(finalSize);
      double space = cardSize.Width / 6;
      double startX = space;
      double startY = space;

      myTitle.Arrange(new Rect(space, 0, finalSize.Width, myTitle.DesiredSize.Height));
      switch (Placement)
      {
        case PlayerPanelPlacement.Bottom:
        case PlayerPanelPlacement.Top:
          startY += myTitle.DesiredSize.Height;
          break;
        case PlayerPanelPlacement.Left:
          startX += myTitle.DesiredSize.Height;
          break;
        case PlayerPanelPlacement.Right:
          break;
        default:
          throw new ArgumentOutOfRangeException();
      }

      double x = startX;
      double y = startY;

      PlaceRaceOrClassCards(Player.RaceCards.Cast<ICard>(), Player.HalfBreededCard, ref x, y, cardSize.Width, cardSize.Height, space, finalSize);
      PlaceRaceOrClassCards(Player.ClassCards.Cast<ICard>(), Player.SuperManchkinCard, ref x, y, cardSize.Width, cardSize.Height, space, finalSize);

      double maxX = x;

      y += cardSize.Height + space;
      x = startX;

      foreach (Tuple<ITreasureCard, ICheatCard> tuple in Player.WornItems)
      {
        ICard card = tuple.Item1;
        ICheatCard cheatCard = tuple.Item2;
        PlaceCard(card, new Rect(x, y, cardSize.Width, cardSize.Height), finalSize);
        if (cheatCard != null)
          PlaceCard(cheatCard, new Rect(x, y + cardSize.Height*0.7, cardSize.Width, cardSize.Height), finalSize);
        x += cardSize.Width + space;
      }

      // TODO: InPackItems

      maxX = Math.Max(x, maxX);

      myHandCardsElement.CardWidth = cardSize.Width; //?
      myHandCardsElement.CardHeight = cardSize.Height; //?
      double handCardsW = myHandCardsElement.DesiredSize.Width;
      double handCardsH = myHandCardsElement.DesiredSize.Height;
      double handCardsX = maxX;
      double handCardsY = Placement.IsHorizontal() ? finalSize.Height / 2 - handCardsH / 2 : finalSize.Width / 2 - handCardsW  / 2;
      var handCardsRect = new Rect(handCardsX, handCardsY, 
        Placement.IsHorizontal() ? handCardsW : handCardsH, 
        Placement.IsHorizontal() ? handCardsH : handCardsW);
      PlaceElement(myHandCardsElement, handCardsRect, finalSize);      

      myBorder.Arrange(new Rect(0, 0, finalSize.Width, finalSize.Height));
      return finalSize;
    }

    private Size CalcCardSize(Size panelSize)
    {
      double h = Placement.IsHorizontal() ? panelSize.Height : panelSize.Width;
      double cardHeight = h/3;
      double cardWidth = cardHeight*StuffManager.CardImageWidthToHeight;
      return new Size(cardWidth, cardHeight);
    }

    private void PlaceRaceOrClassCards(IEnumerable<ICard> cards, ICard mixCard, ref double x, double y, double cardWidth, double cardHeight, double space, Size finalSize)
    {
      //TODO: better layout
      foreach (ICard card in cards.Concat(mixCard != null ? new[] { mixCard } : new ICard[0]))
      {
        PlaceCard(card, new Rect(x, y, cardWidth, cardHeight), finalSize);
        x += cardWidth + space;
      }
    }

    private void PlaceCard(ICard card, Rect rect, Size panelSize)
    {
      CardElement cardElement = myCardElements[card];
      PlaceElement(cardElement, rect, panelSize);
    }

    private void PlaceElement(UIElement element, Rect rect, Size panelSize)
    {
      if (Placement == PlayerPanelPlacement.Left)
        rect = new Rect(panelSize.Width - rect.Y - rect.Height, rect.X, rect.Height, rect.Width);
      else if (Placement == PlayerPanelPlacement.Right)
        rect = new Rect(rect.Y, panelSize.Height - rect.X - rect.Width, rect.Height, rect.Width);
      element.Arrange(rect);
    }

    private int RotationAngle
    {
      get
      {
        int rotationAngle;
        switch(Placement)
        {
          case PlayerPanelPlacement.Bottom:
          case PlayerPanelPlacement.Top:
            rotationAngle = 0;
            break;
          case PlayerPanelPlacement.Left:
            rotationAngle = 90;
            break;
          case PlayerPanelPlacement.Right:
            rotationAngle = -90;
            break;
          default:
            throw new ArgumentOutOfRangeException();
        }
        return rotationAngle;
      }
    }
  }
}