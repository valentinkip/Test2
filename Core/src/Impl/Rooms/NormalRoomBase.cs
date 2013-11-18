using System;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace ManchkinQuest.Core.Impl.Rooms
{
  abstract class NormalRoomBase : INormalRoom
  {
    private readonly Color myLeft;
    private readonly Color myRight;
    private readonly Color myUp;
    private readonly Color myDown;

    protected NormalRoomBase(BitmapSource image, Color left, Color right, Color up, Color down)
    {
      Image = image;
      myLeft = left;
      myRight = right;
      myUp = up;
      myDown = down;
    }

    public BitmapSource Image { get; private set; }

    public Color ArrowColor(Direction direction)
    {
      switch(direction)
      {
        case Direction.Left:
          return myLeft;
        case Direction.Right:
          return myRight;
        case Direction.Up:
          return myUp;
        case Direction.Down:
          return myDown;
        default:
          throw new ArgumentOutOfRangeException("direction");
      }
    }
  }
}