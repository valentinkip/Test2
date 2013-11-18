using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using ManchkinQuest.Core;

namespace ManchkinQuest.UI
{
  class RoomLinkElement : Panel
  {
    public IRoomLink RoomLink { get; private set; }
    public RoomLinkLocation Location { get; private set; }

    private readonly Image myImage;

    public RoomLinkElement(IRoomLink roomLink, RoomLinkLocation location)
    {
//      Background = new SolidColorBrush(Colors.Blue);
      RoomLink = roomLink;
      Location = location;
      myImage = new Image { Source = RoomLink.Image, Stretch = Stretch.Fill};
      if (Location.Vertical)
        myImage.RenderTransform = new RotateTransform { Angle = 90, CenterX = 0, CenterY = 0 };
      Children.Add(myImage);
    }

    protected override Size ArrangeOverride(Size finalSize)
    {
      Rect rect = Location.Vertical
                    ? new Rect(finalSize.Width, 0, finalSize.Height, finalSize.Width)
                    : new Rect(0, 0, finalSize.Width, finalSize.Height);
      myImage.Arrange(rect);
      return finalSize;
    }
  }
}