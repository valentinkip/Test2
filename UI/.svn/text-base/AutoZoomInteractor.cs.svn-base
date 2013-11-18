using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using JetBrains.Annotations;

namespace ManchkinQuest.UI
{
  enum ZoomDirection
  {
    Center, Up, Down, Left, Right
  }

  class AutoZoomInteractor<TElement> where TElement : UIElement
  {
    private readonly Panel myPanel;
    private readonly Func<TElement, UIElement> myCreateZoomedElement;
    private readonly Func<TElement, double> myZoomedElementWidth;
    private readonly Action<TElement> myOnZoomed;
    private readonly Action<TElement> myOnUnzoomed;
    private UIElement myZoomedElement = null;
    private TElement myOriginalElement = null;

    public AutoZoomInteractor(
      Panel panel,
      [NotNull] Func<TElement, UIElement> createZoomedElement,
      [NotNull] Func<TElement, double> zoomedElementWidth,
      [CanBeNull] Action<TElement> onZoomed,
      [CanBeNull] Action<TElement> onUnzoomed,
      bool suspendWhenPopupShown)
    {
      myPanel = panel;
      myCreateZoomedElement = createZoomedElement;
      myZoomedElementWidth = zoomedElementWidth;
      myOnZoomed = onZoomed;
      myOnUnzoomed = onUnzoomed;
      SuspendWhenPopupShown = suspendWhenPopupShown;
      ZoomDirection = ZoomDirection.Up;
      RotateZoomed = false;
    }

    public bool RotateZoomed { get; set; }
    public bool SuspendWhenPopupShown { get; set; }

    private void OnMouseMove(object sender, MouseEventArgs args)
    {
      if (myZoomedElement != null)
      {
        TElement elementToZoom = ElementToZoom(args);
        if (elementToZoom == null)
          Unzoom();
      }
    }

    public ZoomDirection ZoomDirection { get; set; }

    public void OnElementMouseEnter(TElement element)
    {
      if (myZoomedElement == null || myOriginalElement != element)
        Zoom(element);
    }

    private void Zoom(TElement element)
    {
      Unzoom();

      if (UIManager.Instance.Popup.IsOpen && SuspendWhenPopupShown) return;       

      myZoomedElement = myCreateZoomedElement(element);
      myOriginalElement = element;

      myZoomedElement.MouseMove += delegate(object sender, MouseEventArgs args)
      {
        TElement elementToZoom = ElementToZoom(args);
        if (elementToZoom != null && elementToZoom != element)
          Zoom(elementToZoom);
      };
      Application.Current.RootVisual.MouseMove += OnMouseMove;

      UIManager.Instance.ShowInFastPopup(myZoomedElement, GrowRect(element), OnPopupClosed);

      if (myOnZoomed != null) 
        myOnZoomed(element);
    }

    private Rect GrowRect(TElement element)
    {
      Rect rect = element.GetRect(Application.Current.RootVisual);
      double cx = rect.X + rect.Width / 2;
      double cy = rect.Y + rect.Height / 2;
      double newW = myZoomedElementWidth(element);
      double newH = RotateZoomed ? newW * rect.Width / rect.Height : newW * rect.Height / rect.Width;
      switch (ZoomDirection)
      {
        case ZoomDirection.Center:
          return new Rect(Math.Max(cx - newW / 2, 0), Math.Max(cy - newH / 2, 0), newW, newH);
        case ZoomDirection.Up:
          return new Rect(Math.Max(cx - newW / 2, 0), rect.Top - newH, newW, newH);
        case ZoomDirection.Down:
          return new Rect(Math.Max(cx - newW / 2, 0), rect.Bottom, newW, newH);
        case ZoomDirection.Left:
          return new Rect(rect.Left - newW, Math.Max(cy - newH / 2, 0), newW, newH);
        case ZoomDirection.Right:
          return new Rect(rect.Right, Math.Max(cy - newH / 2, 0), newW, newH);
        default:
          throw new ArgumentOutOfRangeException();
      }
    }

    private void Unzoom()
    {
      if (myZoomedElement != null)
        UIManager.Instance.CloseFastPopup(myZoomedElement);
    }

    private void OnPopupClosed()
    {
      Application.Current.RootVisual.MouseMove -= OnMouseMove;
      if (myOnUnzoomed != null)
        myOnUnzoomed(myOriginalElement);
      myZoomedElement = null;
      myOriginalElement = null;
    }

    private TElement ElementToZoom(MouseEventArgs args)
    {
      Point mousePos = args.GetPosition(myPanel);
      if (mousePos.X != 0 || mousePos.Y != 0)
      {
        TElement element = myPanel.Children
          .OfType<TElement>()
          .Where(e => e.GetRect(myPanel).Contains(mousePos))
          .FirstOrDefault();
        if (element != null) return element;
      }

      if (myZoomedElement != null)
      {
        Point p1 = args.GetPosition(UIManager.Instance.FastPopup);
        if (new Rect(new Point(0, 0), UIManager.Instance.FastPopup.RenderSize).Contains(p1))
          return myOriginalElement;

        Point mousePos1 = args.GetPosition(myZoomedElement);
        if (new Rect(new Point(0, 0), myZoomedElement.RenderSize).Contains(mousePos1))
          return myOriginalElement;
      }

      return null;
    }
  }
}