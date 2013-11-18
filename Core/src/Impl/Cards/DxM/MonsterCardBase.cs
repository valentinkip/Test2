using System.Windows.Media.Imaging;

namespace ManchkinQuest.Core.Impl.Cards.DxM
{
  abstract class MonsterCardBase : IMonsterCard
  {
    protected MonsterCardBase(BitmapSource faceImage)
    {
      FaceImage = faceImage;
    }

    public BitmapSource FaceImage { get; private set; }

    public BitmapSource BackImage
    {
      get { return Images.MonsterCardBack; }
    }
  }
}