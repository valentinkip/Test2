using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using ManchkinQuest.Core;

namespace ManchkinQuest.UI
{
  public class RoomElement : Panel
  {
    private readonly Image myImage;
    private readonly List<Image> myPlayerImages;

    public IMazeRoomInfo RoomInfo { get; private set; }
    public ICollection<IPlayer> Players { get; private set; }

    public RoomElement(IMazeRoomInfo roomInfo, ICollection<IPlayer> players)
    {
      RoomInfo = roomInfo;
      Players = players;

      Children.Add(myImage = new Image { Source = roomInfo.Room.Image, Stretch = Stretch.Fill });

      myPlayerImages = new List<Image>(players.Count);
      foreach(IPlayer player in players)
      {        
        var image = new Image {Source = player.Image, Stretch = Stretch.Fill, Projection = new PlaneProjection{RotationX = 60}};
        myPlayerImages.Add(image);
        Children.Add(image);
      }
    }

    protected override Size ArrangeOverride(Size finalSize)
    {
      myImage.Arrange(new Rect(0, 0, finalSize.Width, finalSize.Height));

      double shiftX = finalSize.Width/8;
      double shiftY = finalSize.Height/8;
      var innerArea = new Rect(shiftX, shiftY, finalSize.Width - 2*shiftX, finalSize.Height - 2*shiftY);
      double width = innerArea.Width * 0.27;
      double height = width / StuffManager.ManchkinImageWidthToHeight;
      double space = width/5;
      double x = 0;
      double y = 0;
      foreach(Image image in myPlayerImages)
      {
        image.Arrange(new Rect(innerArea.X + x, innerArea.Y + y, width, height));
        x += width + space;
        if (x + width > innerArea.Width)
        {
          y += height + space;
          x = 0;
        }
      }

      return finalSize;
    }
  }
}