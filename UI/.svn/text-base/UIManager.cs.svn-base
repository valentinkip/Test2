using System;
using System.Windows;
using System.Windows.Controls.Primitives;
using System.Windows.Media.Animation;

namespace ManchkinQuest.UI
{
  public abstract class UIManager
  {
    public static UIManager Instance = null;

    protected UIManager()
    {
      if (Instance != null)
        throw new InvalidOperationException("2 UIManager's");
      Instance = this;
    }

    public abstract GameTablePanel GameTablePanel { get; }

    public abstract void ShowInPopup(UIElement element, Rect startRect, Rect endRect, Action onClosed = null, params AnimationInfo[] animations);
    public abstract Popup Popup { get; }
    public abstract UIElement PopupElement { get; }

    public abstract void ShowInFastPopup(UIElement element, Rect rect, Action onClosed = null, double startOpacity = 0.3, double endOpacity = 0.8);
    public abstract void CloseFastPopup(UIElement element);
    public abstract Popup FastPopup { get; }
    public abstract UIElement FastPopupElement { get; }
  }

  public class AnimationInfo
  {
    public readonly DependencyObject Target;
    public readonly PropertyPath PropertyPath;
    public readonly double StartValue;
    public readonly double EndValue;
    public readonly IEasingFunction EasingFunction;

    public AnimationInfo(DependencyObject target, PropertyPath propertyPath, double startValue, double endValue, IEasingFunction easingFunction = null)
    {
      Target = target;
      PropertyPath = propertyPath;
      StartValue = startValue;
      EndValue = endValue;
      EasingFunction = easingFunction;
    }

    public AnimationInfo Reverse()
    {
      //TODO: reverse Ease?
      return new AnimationInfo(Target, PropertyPath, EndValue, StartValue, EasingFunction);
    }
  }
}