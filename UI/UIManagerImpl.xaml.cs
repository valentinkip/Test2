using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Media.Animation;
using System.Windows.Media.Effects;

namespace ManchkinQuest.UI
{
  class UIManagerImpl : UIManager
  {
    private readonly GameTablePanel myGameTablePanel;
    private readonly PopupData myPopupData = new PopupData(false);
    private readonly PopupData myFastPopupData = new PopupData(true);

    private Storyboard myStoryboard;
    private Action myOnAnimationCompleted;
    private BackgroundState myCurrentBackgroundState;

    private readonly BlurEffect myBackgroundBlur;

    private class BackgroundState
    {
      public double BlurRadious { get; private set; }
      public double Opacity { get; private set; }

      public BackgroundState(double blurRadious, double opacity)
      {
        BlurRadious = blurRadious;
        Opacity = opacity;
      }
    }

    private class PopupData
    {
      public readonly Popup Popup = new Popup();
      public readonly bool CloseImmediately;
      public UIElement Element = null;
      public Action OnPopupClosed = null;
      public BackgroundState InitialBackgroundState;
      public ICollection<AnimationInfo> AnimationsOnClose;

      public PopupData(bool closeImmediately)
      {
        CloseImmediately = closeImmediately;
      }
    }

    public UIManagerImpl(GameTablePanel gameTablePanel)
    {
      myGameTablePanel = gameTablePanel;
      Application.Current.RootVisual.Effect = myBackgroundBlur = new BlurEffect {Radius = 0};
      myCurrentBackgroundState = new BackgroundState(0, 1);

      Application.Current.RootVisual.MouseLeftButtonDown += delegate(object sender, MouseButtonEventArgs e)
                                                              {
                                                                if (myPopupData.Popup.IsOpen)
                                                                {
                                                                  ClosePopup(myPopupData, myPopupData.Element, false);
                                                                  e.Handled = true;
                                                                }
                                                              };
      Application.Current.RootVisual.KeyDown += delegate(object sender, KeyEventArgs e)
                                                  {
                                                    if (e.Key == Key.Escape && myPopupData.Popup.IsOpen)
                                                    {
                                                      ClosePopup(myPopupData, myPopupData.Element, false);
                                                      e.Handled = true;
                                                    }
                                                  };
    }

    public override GameTablePanel GameTablePanel
    {
      get { return myGameTablePanel; }
    }

    public override void ShowInPopup(UIElement element, Rect startRect, Rect endRect, Action onClosed = null, params AnimationInfo[] animations)
    {
      ShowPopup(myPopupData, element, startRect, endRect, 0.5, 1, 0.5, onClosed, animations);
    }

    public override Popup Popup
    {
      get { return myPopupData.Popup; }
    }

    public override UIElement PopupElement
    {
      get { return myPopupData.Element; }
    }

    public override void ShowInFastPopup(UIElement element, Rect rect, Action onClosed = null, double startOpacity = 0.3, double endOpacity = 0.8)
    {
      ShowPopup(myFastPopupData, element, rect, rect, startOpacity, endOpacity, 1, onClosed);
    }

    public override void CloseFastPopup(UIElement element)
    {
      ClosePopup(myFastPopupData, element, false);
    }

    public override Popup FastPopup
    {
      get { return myFastPopupData.Popup; }
    }

    public override UIElement FastPopupElement
    {
      get { return myFastPopupData.Element; }
    }

    private void ShowPopup(PopupData popupData, UIElement element, Rect startRect, Rect endRect, double startOpacity, double endOpacity, double animationDuration, Action onClosed, IEnumerable<AnimationInfo> animations = null)
    {
      if (popupData.Element != null)
        ClosePopup(popupData, popupData.Element, true);

      var sizedElement = new SizedElement(element, new Size(startRect.Width, startRect.Height)) { Opacity = startOpacity };
      Popup popup = popupData.Popup;
      popup.Child = sizedElement;
      popup.HorizontalOffset = startRect.X;
      popup.VerticalOffset = startRect.Y;
      popup.Width = startRect.Width;
      popup.Height = startRect.Height;
      popupData.Element = element;
      popupData.OnPopupClosed = onClosed;
      popup.IsOpen = true;
      popupData.InitialBackgroundState = myCurrentBackgroundState;

      if (!endRect.Equals(startRect))
      {
        var xAnimation = new AnimationInfo(popup, new PropertyPath(Popup.HorizontalOffsetProperty), startRect.X, endRect.X);
        var yAnimation = new AnimationInfo(popup, new PropertyPath(Popup.VerticalOffsetProperty), startRect.Y, endRect.Y);
        var widthAnimation = new AnimationInfo(sizedElement, new PropertyPath(SizedElement.WidthProperty), startRect.Width, endRect.Width);
        var heightAnimation = new AnimationInfo(sizedElement, new PropertyPath(SizedElement.HeightProperty), startRect.Height, endRect.Height);
        var additionalAnimations = new[] {xAnimation, yAnimation, widthAnimation, heightAnimation};
        animations = animations != null ? animations.Concat(additionalAnimations) : additionalAnimations;
      }

      popupData.AnimationsOnClose = animations != null ? (ICollection<AnimationInfo>)animations.Select(info => info.Reverse()).ToList() : new AnimationInfo[0];

      AnimatePopup(sizedElement, animationDuration, endOpacity, Math.Min(myCurrentBackgroundState.Opacity, 0.7), 5, animations);
    }

    private void ClosePopup(PopupData popupData, UIElement popupElement, bool forceCloseImmediately)
    {
      if (popupData.Element == popupElement)
      {
        Action onClosed = popupData.OnPopupClosed;
        ICollection<AnimationInfo> animationsOnClose = popupData.AnimationsOnClose;
        Popup popup = popupData.Popup;
        Action close = delegate
                         {
                           popupData.Element = null;
                           popupData.OnPopupClosed = null;
                           popupData.AnimationsOnClose = null;
                           popup.IsOpen = false;
                           if (onClosed != null)
                             onClosed();
                         };
        bool closeImmediately = popupData.CloseImmediately | forceCloseImmediately;
        AnimatePopup(popup.Child, 0.5, 1, popupData.InitialBackgroundState.Opacity, popupData.InitialBackgroundState.BlurRadious, animationsOnClose, closeImmediately ? null : close);
        if (closeImmediately)
          close();
      }
    }

    private void AnimatePopup(
      UIElement element, 
      double duration, 
      double targetElementOpacity, 
      double targetBackgroundOpacity, 
      double targetBackgroundBlur,
      IEnumerable<AnimationInfo> additionalAnimations = null,
      Action onCompleted = null)
    {
      if (myStoryboard != null)
      {
        myStoryboard.Pause();
        if (myOnAnimationCompleted != null)
        {
          myOnAnimationCompleted();
          myOnAnimationCompleted = null;
        }        
      }

      UIElement rootVisual = Application.Current.RootVisual;
      IEnumerable<AnimationInfo> animations = new[]
                                                {
                                                  new AnimationInfo(element, new PropertyPath(UIElement.OpacityProperty), element.Opacity, targetElementOpacity), 
                                                  new AnimationInfo(rootVisual, new PropertyPath(UIElement.OpacityProperty), rootVisual.Opacity, targetBackgroundOpacity), 
                                                  new AnimationInfo(myBackgroundBlur, new PropertyPath(BlurEffect.RadiusProperty), myBackgroundBlur.Radius, targetBackgroundBlur)
                                                };
      if (additionalAnimations != null)
        animations = animations.Concat(additionalAnimations);

      myCurrentBackgroundState = new BackgroundState(targetBackgroundBlur, targetBackgroundOpacity);
      myStoryboard = UIUtil.StartAnimation(duration, animations);
      if (onCompleted != null)
      {
        myOnAnimationCompleted = onCompleted;
        myStoryboard.Completed += delegate
                                    {
                                      onCompleted();
                                      myOnAnimationCompleted = null;
                                    };
      }
    }
  }
}