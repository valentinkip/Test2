using System;
using System.Windows;
using System.Windows.Controls;

namespace ManchkinQuest.UI
{
  class CardZoomInteractor : AutoZoomInteractor<CardElement>
  {
    private PlayerPanelPlacement? myPanelPlacement;

    public CardZoomInteractor(Panel panel, bool suspendWhenPopupShown)
      : base(panel, CreateZoomedElement, ZoomedElementWidth, OnZoomed, OnUnzoomed, suspendWhenPopupShown)
    {
    }

    private static UIElement CreateZoomedElement(CardElement cardElement)
    {
      return new CardElement(cardElement.Card);
    }

    private static double ZoomedElementWidth(CardElement cardElement)
    {
      return 200;
    }

    private static void OnZoomed(CardElement cardElement)
    {
      cardElement.ShowBorder = true;
    }

    private static void OnUnzoomed(CardElement cardElement)
    {
      cardElement.ShowBorder = false;
    }

    public PlayerPanelPlacement? PanelPlacement
    {
      get { return myPanelPlacement; }
      set
      {
        myPanelPlacement = value;
        PlayerPanelPlacement placement = myPanelPlacement != null ? (PlayerPanelPlacement)myPanelPlacement : PlayerPanelPlacement.Bottom;
        switch(placement)
        {
          case PlayerPanelPlacement.Bottom:
            ZoomDirection = ZoomDirection.Up;
            RotateZoomed = false;
            break;

          case PlayerPanelPlacement.Top:
            ZoomDirection = ZoomDirection.Down;
            RotateZoomed = false;
            break;

          case PlayerPanelPlacement.Left:
            ZoomDirection = ZoomDirection.Right;
            RotateZoomed = true;
            break;

          case PlayerPanelPlacement.Right:
            ZoomDirection = ZoomDirection.Left;
            RotateZoomed = true;
            break;

          default:
            throw new ArgumentOutOfRangeException();
        }
      }
    }
  }
}