using System;
using System.Windows;
using System.Windows.Media.Imaging;
using System.Windows.Resources;

namespace ManchkinQuest.Core
{
  public static class Images
  {
    public static BitmapSource LoadImage(string imageName)
    {
      StreamResourceInfo resourceInfo = Application.GetResourceStream(new Uri("Core;component/resources/" + imageName, UriKind.Relative));
      var image = new BitmapImage();
      image.SetSource(resourceInfo.Stream);
      return image;
    }

    public static readonly BitmapSource DxMCardBack = LoadImage("Cards/DxMBack.jpg");
    public static readonly BitmapSource TreasureCardBack = LoadImage("Cards/TreasureBack.jpg");
    public static readonly BitmapSource MonsterCardBack = LoadImage("Cards/MonsterBack.jpg");

    public static readonly BitmapSource BlueManchkin = LoadImage("Manchkin-Blue.png");
    public static readonly BitmapSource GreenManchkin = LoadImage("Manchkin-Green.png");
    public static readonly BitmapSource RedManchkin = LoadImage("Manchkin-Red.png");
    public static readonly BitmapSource YellowManchkin = LoadImage("Manchkin-Yellow.png");
  }
}