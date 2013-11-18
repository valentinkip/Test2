using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Animation;

namespace ManchkinQuest.UI
{
  public static class UIUtil
  {
    public static Rect GetRect(this UIElement element, UIElement relativeTo)
    {
      GeneralTransform transform = element.TransformToVisual(relativeTo);
      return new Rect(transform.Transform(new Point(0, 0)), element.RenderSize);
    }

    public static Storyboard StartAnimation(double duration, params AnimationInfo[] animations)
    {
      return StartAnimation(duration, (IEnumerable<AnimationInfo>)animations);
    }

    public static Storyboard StartAnimation(double duration, IEnumerable<AnimationInfo> animations)
    {
      var storyboard = new Storyboard();
      var durationSpan = TimeSpan.FromSeconds(duration);
      foreach (AnimationInfo animationInfo in animations)
      {
        var animation = new DoubleAnimation { Duration = durationSpan, From = animationInfo.StartValue, To = animationInfo.EndValue };
        if (animationInfo.EasingFunction != null)
          animation.EasingFunction = animationInfo.EasingFunction;
        Storyboard.SetTarget(animation, animationInfo.Target);
        Storyboard.SetTargetProperty(animation, animationInfo.PropertyPath);
        storyboard.Children.Add(animation);
      }
      storyboard.Begin();
      return storyboard;
    }
  }
}