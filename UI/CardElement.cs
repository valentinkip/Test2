﻿using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Effects;
using ManchkinQuest.Core;

namespace ManchkinQuest.UI
{
  class CardElement : Panel
  {
    private readonly Image myImage;
    private readonly int myRotationAngle;
    private Border myBorder;

    public ICard Card { get; private set; }

    public CardElement(ICard card, int rotationAngle = 0, bool shadow = true)
    {
      myRotationAngle = rotationAngle;
      Card = card;
      myImage = new Image { Source = card.FaceImage, Stretch = Stretch.Fill };
      if (rotationAngle != 0)
      {
        Debug.Assert(rotationAngle == 90 || rotationAngle == -90);
        myImage.RenderTransform = new RotateTransform { Angle = rotationAngle };
      }
      if (shadow)
        myImage.Effect = new DropShadowEffect();
      Children.Add(myImage);
    }

    public bool ShowBorder
    {
      get { return myBorder != null; }
      set
      {
        if (ShowBorder != value)
        {
          if (value)
          {
            myBorder = new Border { BorderBrush = new SolidColorBrush(Colors.Black), BorderThickness = new Thickness(5) };
            Children.Add(myBorder);
            InvalidateArrange();
          }
          else
          {
            Children.Remove(myBorder);
            myBorder = null;
            InvalidateArrange();
          }
        }
      }
    }

    protected override Size MeasureOverride(Size availableSize)
    {
      myImage.Measure(availableSize);
      return base.MeasureOverride(availableSize);
    }

    protected override Size ArrangeOverride(Size finalSize)
    {
      if (myRotationAngle == 90)
        myImage.Arrange(new Rect(finalSize.Width, 0, finalSize.Height, finalSize.Width));
      else if (myRotationAngle == -90)
        myImage.Arrange(new Rect(0, finalSize.Height, finalSize.Height, finalSize.Width));
      else
        myImage.Arrange(new Rect(0, 0, finalSize.Width, finalSize.Height));
      if (myBorder != null)
        myBorder.Arrange(new Rect(0, 0, finalSize.Width, finalSize.Height));
      return finalSize;
    }
  }
}
