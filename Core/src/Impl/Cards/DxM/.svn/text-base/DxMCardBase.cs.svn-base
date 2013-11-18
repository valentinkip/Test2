using System.Windows.Media.Imaging;

namespace ManchkinQuest.Core.Impl.Cards.DxM
{
  abstract class DxMCardBase : IDxMCard
  {
    protected DxMCardBase(BitmapSource faceImage)
    {
      FaceImage = faceImage;
    }

    public BitmapSource FaceImage { get; private set; }

    public BitmapSource BackImage
    {
      get { return Images.DxMCardBack; }
    }
  }
}