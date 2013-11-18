using System.Windows.Media.Imaging;

namespace ManchkinQuest.Core.Impl.Cards.Treasure
{
  abstract class TreasureCardBase : ITreasureCard
  {
    protected TreasureCardBase(int price, BitmapSource faceImage)
    {
      Price = price;
      FaceImage = faceImage;
    }

    public int Price { get; private set; }

    public BitmapSource FaceImage { get; private set; }

    public BitmapSource BackImage
    {
      get { return Images.TreasureCardBack; }
    }
  }
}