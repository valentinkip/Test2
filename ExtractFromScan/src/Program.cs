using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;

namespace ExtractFromScan
{
  static class Program
  {
    private static readonly Color TransparentColor = Color.FromArgb(0, 0, 0, 0);

    private const int ColorDiff = 10;
//    private const int ColorDiffFilterPerimeter = 50;
    private const int MinFragmentSize = 50;
    private const float RotationPrecision = 0.1f;
    private const int MaxRoatationAngle = 30;

    static void Main(string[] args)
    {
      string fileName = args[0];
      Image image = Image.FromFile(fileName);
      var bitmap = new Bitmap(image);
      int width = bitmap.Width;
      int height = bitmap.Height;

      Console.Out.WriteLine("Image size is {0}x{1}", width, height);

      Console.Out.Write("Scanning pixels...");

//      Color backColor = SmartAverageColor(PerimeterColors(bitmap));

/*
      var imageMap = new bool[width, height];
      for(int x = 0; x < width; x++)
        for(int y = 0; y < height; y++)
          imageMap[x, y] = bitmap.GetPixel(x, y).IsDifferentFromBackColor(backColor);
*/

      var colorMap = new Color[width, height];
      for(int x = 0; x < width; x++)
        for(int y = 0; y < height; y++)
          colorMap[x, y] = bitmap.GetPixel(x, y);

      var imageMap = new bool[width, height];
      for(int x = 0; x < width; x++)
        for(int y = 0; y < height; y++)
        {
          if (IsContrastPixel(x, y, colorMap))
            imageMap[x, y] = true;
        }

      Console.Out.WriteLine(" Done");

      Console.Out.Write("Filling in inner areas...");
      FillInnerPixels(imageMap);
      Console.Out.WriteLine(" Done");

      for (int x = 0; x < width; x++)
        for(int y = 0; y < height; y++)
        {
          if (imageMap[x, y])
            bitmap.SetPixel(x, y, Color.Black);
        }
      bitmap.Save(Path.GetFileNameWithoutExtension(fileName) + "-BW.jpg", ImageFormat.Jpeg);

      bitmap = new Bitmap(image);
      for (int x = 0; x < width; x++)
        for(int y = 0; y < height; y++)
        {
          if (!imageMap[x, y])
            bitmap.SetPixel(x, y, TransparentColor);
        }
      image = bitmap;

      int count = 0;
      for(int x = 0; x < width; x++)
        for(int y = 0; y < height; y++)
        {
// ReSharper disable AccessToModifiedClosure
          if (imageMap[x, y])
// ReSharper restore AccessToModifiedClosure
          {
            RectangleF targetRect;
            float angle;
            IEnumerable<Point> pixels;
            if (Grow(new Point(x, y), imageMap, out targetRect, out angle, out pixels))
            {
              Console.Out.WriteLine("Found fragment #{0} of size {1}x{2}", count + 1, targetRect.Width, targetRect.Height);
              var newImage = new Bitmap((int)targetRect.Width, (int)targetRect.Height);
              using (Graphics g = Graphics.FromImage(newImage))
              {
                using(Brush brush = new SolidBrush(TransparentColor))
                  g.FillRectangle(brush, 0, 0, newImage.Width, newImage.Height);

                g.RotateTransform(angle);
                g.TranslateTransform(-targetRect.X, -targetRect.Y, MatrixOrder.Append);
                g.DrawImage(image, 0, 0);
              }
              newImage.Save(Path.GetFileNameWithoutExtension(fileName) + "-Result" + ++count + ".png", ImageFormat.Png);
            }

            // clear pixels from the found fragment
            foreach (Point p in pixels)
              imageMap[p.X, p.Y] = false;
          }
        }
    }

    private static bool IsContrastPixel(int x, int y, Color[,] image)
    {
      Color color = image[x, y];
      bool any = false;
      bool all = true;
      var bounds = new Rectangle(0, 0, image.GetLength(0), image.GetLength(1));
      foreach(Point neighbour in new Point(x, y).Neighbours())
      {
        if (!bounds.Contains(neighbour)) continue;
        if (color.IsDifferentFromColor(image[neighbour.X, neighbour.Y]))
          any = true;
        else
          all = false;
        if (any && !all) return true;
      }
      return false;
    }

    private static bool IsDifferentFromColor(this Color color1, Color color2, int d = ColorDiff)
    {
      int dr = color1.R - color2.R;
      int dg = color1.G - color2.G;
      int db = color1.B - color2.B;
      return dr*dr + dg*dg + db*db > d*d;
    }

    private static void FillInnerPixels(bool[,] imageMap)
    {
      int width = imageMap.GetLength(0);
      int height = imageMap.GetLength(1);
      var newImageMap = (bool[,])imageMap.Clone();
      foreach(Point p in PerimeterPoints(width, height))
      {
        if (!newImageMap[p.X, p.Y])
        {
          List<Point> allPixels;
          List<Point> borderPixels;
          FillComponent(newImageMap, p, true, newImageMap, out allPixels, out borderPixels);
        }
      }
      for(int x = 0; x < width; x++)
        for(int y = 0; y < height; y++)
        {
          if (!newImageMap[x, y])
            imageMap[x, y] = true;
        }
    }

/*
    private static IEnumerable<Color> PerimeterColors(Bitmap bitmap)
    {
      int width = bitmap.Width;
      int height = bitmap.Height;

      for(int x = 0; x < width; x++)
        yield return bitmap.GetPixel(x, 0);

      for(int x = 0; x < width; x++)
        yield return bitmap.GetPixel(x, height - 1);

      for (int y = 1; y < height - 1; y++)
        yield return bitmap.GetPixel(0, y);

      for (int y = 1; y < height - 1; y++)
        yield return bitmap.GetPixel(width - 1, y);
    }
*/

/*
    private static Color AverageColor(IEnumerable<Color> colors)
    {
      int count = 0;
      int rSum = 0;
      int gSum = 0;
      int bSum = 0;
      foreach(Color color in colors)
      {
        count++;
        rSum += color.R;
        gSum += color.G;
        bSum += color.B;
      }
      return Color.FromArgb((rSum + count / 2) / count, (gSum + count / 2) / count, (bSum + count / 2) / count);
    }
*/

/*
    private static Color SmartAverageColor(IEnumerable<Color> colors)
    {
      Color averageColor = AverageColor(colors);
      return AverageColor(colors.Where(color => !color.IsDifferentFromBackColor(averageColor, ColorDiffFilterPerimeter)));
    }
*/

    private static IEnumerable<Point> PerimeterPoints(int width, int height)
    {
      for (int x = 0; x < width; x++)
        yield return new Point(x, 0);

      for (int x = 0; x < width; x++)
        yield return new Point(x, height - 1);

      for (int y = 1; y < height - 1; y++)
        yield return new Point(0, y);

      for (int y = 1; y < height - 1; y++)
        yield return new Point(width - 1, y);
    }

    private static bool Grow(Point startPixel, bool[,] imageMap, out RectangleF targetRect, out float rotationAngle, out IEnumerable<Point> pixels)
    {
      int width = imageMap.GetLength(0);
      int height = imageMap.GetLength(1);
      var pixelsBitSet = new bool[width, height];
      List<Point> borderPixels;
      List<Point> allPixels;
      FillComponent(imageMap, startPixel, true, pixelsBitSet, out allPixels, out borderPixels);
      pixels = allPixels;

      IEnumerable<PointF> points = borderPixels.Select(p => (PointF)p);
      var approxRect = BoundingRectangle(points);
      if (approxRect.Width < MinFragmentSize || approxRect.Height < MinFragmentSize)
      {
        rotationAngle = 0;
        targetRect = Rectangle.Empty;
        return false;
      }

      Func<float, float> funcToMinimize = angle => CalcArea(BoundingRectForAngle(points, angle));
      rotationAngle = MinimumFinder.FindMinimum(funcToMinimize, -MaxRoatationAngle, MaxRoatationAngle, RotationPrecision);
      targetRect = BoundingRectForAngle(points, rotationAngle);
      return true;
    }

    private static void FillComponent<TColor1, TColor2>(
      TColor1[,] originalMap, Point startPixel, TColor2 color, TColor2[,] targetMap, 
      out List<Point> allPixels, out List<Point> borderPixels)
    {
      int width = originalMap.GetLength(0);
      int height = originalMap.GetLength(1);
      allPixels = new List<Point> {startPixel};
      borderPixels = new List<Point>();
      var bounds = new Rectangle(0, 0, width, height);
      int marker = 0;
      TColor1 colorToFill = originalMap[startPixel.X, startPixel.Y];
      while(marker < allPixels.Count)
      {
        Point pixel = allPixels[marker++];
        bool allNeighboursIncluded = true;
        foreach(Point p in pixel.Neighbours())
        {
          int x = p.X;
          int y = p.Y;
          bool isNeighbourIncluded = bounds.Contains(x, y) && Equals(originalMap[x, y], colorToFill);
          allNeighboursIncluded &= isNeighbourIncluded;
          if (isNeighbourIncluded && !Equals(targetMap[x, y], color))
          {
            targetMap[x, y] = color;
            allPixels.Add(p);
          }
        }
        if (!allNeighboursIncluded)
          borderPixels.Add(pixel);
      }
    }

    private static float CalcArea(RectangleF rectangle)
    {
      return rectangle.Width*rectangle.Height;
    }

    private static RectangleF BoundingRectForAngle(IEnumerable<PointF> points, float angle)
    {
      var matrix = new Matrix();
      matrix.Rotate(angle);
      PointF[] pixels = points.ToArray();
      matrix.TransformPoints(pixels);
      return BoundingRectangle(pixels);
    }

    private static RectangleF BoundingRectangle(IEnumerable<PointF> pixels)
    {
      float minX = float.MaxValue;
      float maxX = float.MinValue;
      float minY = float.MaxValue;
      float maxY = float.MinValue;
      foreach(PointF p in pixels)
      {
        if (p.X < minX) minX = p.X;
        if (p.X > maxX) maxX = p.X;
        if (p.Y < minY) minY = p.Y;
        if (p.Y > maxY) maxY = p.Y;
      }
      return new RectangleF(minX, minY, maxX - minX + 1, maxY - minY + 1);
    }

    private static IEnumerable<Point> Neighbours(this Point p)
    {
      yield return new Point(p.X, p.Y - 1);
      yield return new Point(p.X, p.Y + 1);
      yield return new Point(p.X - 1, p.Y);
      yield return new Point(p.X + 1, p.Y);
    }
  }
}
