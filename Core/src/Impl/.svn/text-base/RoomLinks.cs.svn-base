using System.Windows.Media.Imaging;

namespace ManchkinQuest.Core.Impl
{
  class OpenPassageway : IRoomLink
  {
    private readonly BitmapSource myImage = Images.LoadImage("RoomLinks/OpenPassageway.png");

    public BitmapSource Image
    {
      get { return myImage; }
    }

    public int Steps
    {
      get { return 1; }
    }
  }

  class RegularDoor : IRoomLink
  {
    private readonly BitmapSource myImage = Images.LoadImage("RoomLinks/RegularDoor.png");

    public BitmapSource Image
    {
      get { return myImage; }
    }

    public int Steps
    {
      get { return 1; }
    }
  }

  class LockedDoor : IRoomLink
  {
    private readonly BitmapSource myImage = Images.LoadImage("RoomLinks/LockedDoor.png");

    public BitmapSource Image
    {
      get { return myImage; }
    }

    public int Steps
    {
      get { return 3; }
    }
  }

  class HiddenDoor : IRoomLink
  {
    private readonly BitmapSource myImage = Images.LoadImage("RoomLinks/HiddenDoor.png");

    public BitmapSource Image
    {
      get { return myImage; }
    }

    public int Steps
    {
      get { return 3; }
    }
  }

  class Wall : IRoomLink
  {
    private readonly BitmapSource myImage = Images.LoadImage("RoomLinks/Wall.png");

    public BitmapSource Image
    {
      get { return myImage; }
    }

    public int Steps
    {
      get { return -1; }
    }
  }
}