using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using ManchkinQuest.Core;

namespace ManchkinQuest.UI
{
  class HandCardsElement : Panel
  {
    public static readonly DependencyProperty CardWidthProperty = DependencyProperty.Register("CardWidth", typeof(double), typeof(HandCardsElement),
                                                                                              new PropertyMetadata(
                                                                                                delegate(DependencyObject o, DependencyPropertyChangedEventArgs args) { ((HandCardsElement)o).CardWidth = (double)args.NewValue; }));

    public static readonly DependencyProperty CardHeightProperty = DependencyProperty.Register("CardHeight", typeof(double), typeof(HandCardsElement),
                                                                                               new PropertyMetadata(
                                                                                                 delegate(DependencyObject o, DependencyPropertyChangedEventArgs args) { ((HandCardsElement)o).CardHeight = (double)args.NewValue; }));

    public static readonly DependencyProperty XStepProperty = DependencyProperty.Register("XStep", typeof(double), typeof(HandCardsElement),
                                                                                          new PropertyMetadata(
                                                                                            delegate(DependencyObject o, DependencyPropertyChangedEventArgs args) { ((HandCardsElement)o).XStep = (double)args.NewValue; }));

    public static readonly DependencyProperty MaxAngleProperty = DependencyProperty.Register("MaxAngle", typeof(double), typeof(HandCardsElement),
                                                                                             new PropertyMetadata(
                                                                                               delegate(DependencyObject o, DependencyPropertyChangedEventArgs args) { ((HandCardsElement)o).MaxAngle = (double)args.NewValue; }));

    public static readonly DependencyProperty AngleStepProperty = DependencyProperty.Register("AngleStep", typeof(double), typeof(HandCardsElement),
                                                                                              new PropertyMetadata(
                                                                                                delegate(DependencyObject o, DependencyPropertyChangedEventArgs args) { ((HandCardsElement)o).AngleStep = (double)args.NewValue; }));

    private readonly double myRotationAngle;
    private readonly List<Image> myImages;
    private readonly TextBlock myText;
//    private readonly Border myBorder;

    private double myCardWidth;
    private double myCardHeight;
    private double myXStep;
    private double myMaxAngle;
    private double myAngleStep;

    private Action myNavigationAction = null; 

    public HandCardsElement(IEnumerable<ICard> cards, bool showFaces, double rotationAngle, bool displayCardCount = true)
    {
//      Background = new SolidColorBrush(Colors.Blue);

      XStep = 0.2;
      MaxAngle = 45;
      AngleStep = 10;

      myRotationAngle = rotationAngle;
      if (myRotationAngle != 0 && myRotationAngle != 90 && myRotationAngle != -90)
        throw new ArgumentException();

      myImages = new List<Image>();
      foreach(ICard card in cards)
      {
        var image = new Image { Source = showFaces ? card.FaceImage : card.BackImage, Stretch = Stretch.Fill };
        myImages.Add(image);
        Children.Add(image);
      }
      if (displayCardCount && myImages.Count > 0)
      {
        Children.Add(myText = new TextBlock
        {
          Text = myImages.Count.ToString(),
          Foreground = new SolidColorBrush(Colors.Black),
          FontWeight = FontWeights.Bold,
          TextAlignment = TextAlignment.Center
        });
      }
//      Children.Add(myBorder = new Border { BorderBrush = new SolidColorBrush(Colors.Blue), BorderThickness = new Thickness(2)/*, Visibility = Visibility.Collapsed*/});

      MouseEnter += delegate
      {
        if (myNavigationAction != null)
        {
          Cursor = Cursors.Hand;
          RenderTransform = new ScaleTransform { ScaleX = 1.1, ScaleY = 1.1, CenterX = ActualWidth / 2, CenterY = ActualHeight / 2 };
        }
      };
      MouseLeave += delegate
      {
        Cursor = Cursors.Arrow;
        RenderTransform = null;
      };
      MouseLeftButtonUp += delegate { if (myNavigationAction != null) myNavigationAction(); };
    }

    public double CardWidth
    {
      get { return myCardWidth; }
      set
      {
        myCardWidth = value;
        InvalidateArrange();
      }
    }

    public double CardHeight
    {
      get { return myCardHeight; }
      set
      {
        myCardHeight = value;
        InvalidateArrange();
      }
    }

    public double XStep
    {
      get { return myXStep; }
      set
      {
        myXStep = value;
        InvalidateArrange();
      }
    }

    public double MaxAngle
    {
      get { return myMaxAngle; }
      set
      {
        myMaxAngle = value;
        InvalidateArrange();
      }
    }

    public double AngleStep
    {
      get { return myAngleStep; }
      set
      {
        myAngleStep = value;
        InvalidateArrange();
      }
    }

    public Action NavigationAction
    {
      get { return myNavigationAction; }
      set { myNavigationAction = value; }
    }

    protected override Size MeasureOverride(Size availableSize)
    {
      double endAngle = CalcEndAngle() / 180 * Math.PI;
      var size = new Size(CardWidth * myImages.Count * XStep + (CardHeight + CardWidth / 2) * Math.Sin(endAngle) * 2, CardHeight * 2);
      return myRotationAngle == 0 ? size : new Size(size.Height, size.Width);
    }

    protected override Size ArrangeOverride(Size finalSize)
    {      
      var transform = new RotateTransform { Angle = myRotationAngle, CenterX = finalSize.Width / 2, CenterY = finalSize.Height / 2 };

      double endAngle = CalcEndAngle();
      double startAngle = -endAngle;
      double textH = CardHeight * 0.7; //??
      double xStep = CardWidth * XStep;
      double startX = (finalSize.Width - CardWidth) / 2;
      if (myImages.Count > 1)
        startX -= xStep * (myImages.Count - 1) / 2;
      for (int i = 0; i < myImages.Count; i++)
      {
        Image image = myImages[i];
        double angle = myImages.Count > 1 ? startAngle + (endAngle - startAngle) * i / (myImages.Count - 1) : 0;
        var cardTransform = new RotateTransform { Angle = angle, CenterX = CardWidth / 2, CenterY = CardHeight };
        var location = new Point(startX + xStep * i, (finalSize.Height - CardHeight) / 2 - textH * 0.25);
        var rotationPoint = new Point(location.X + cardTransform.CenterX, location.Y + cardTransform.CenterY);
        rotationPoint = transform.Transform(rotationPoint);
        location = new Point(rotationPoint.X - cardTransform.CenterX, rotationPoint.Y - cardTransform.CenterY);
        image.RenderTransform = new RotateTransform {Angle = cardTransform.Angle + transform.Angle, CenterX = cardTransform.CenterX, CenterY = cardTransform.CenterY};
        image.Arrange(new Rect(location, new Size(CardWidth, CardHeight)));
      }

      if (myText != null)
      {
        myText.FontSize = CardHeight / 2;
        if (myRotationAngle == 0)
        {
          myText.Arrange(new Rect(0, finalSize.Height - textH, finalSize.Width, textH));
        }
        else if (myRotationAngle == 90)
        {
          myText.Arrange(new Rect(0, finalSize.Height / 2 - textH / 2, textH, finalSize.Height));
        }
        else if (myRotationAngle == -90)
        {
          myText.Arrange(new Rect(finalSize.Width - textH, finalSize.Height / 2 - textH / 2, textH, finalSize.Height));
        }
      }

//      myBorder.Arrange(new Rect(0, 0, finalSize.Width, finalSize.Height));

      return finalSize;
    }

    private double CalcEndAngle()
    {
      return Math.Min(MaxAngle, (myImages.Count - 1) * AngleStep);
    }
  }
}