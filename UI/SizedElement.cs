using System.Windows;
using System.Windows.Controls;

namespace ManchkinQuest.UI
{
  class SizedElement : Panel
  {
    public new static readonly DependencyProperty WidthProperty = DependencyProperty.Register("Width", typeof(double), typeof(SizedElement),
                                                                                              new PropertyMetadata(
                                                                                                delegate(DependencyObject o, DependencyPropertyChangedEventArgs args) { ((SizedElement)o).Width = (double)args.NewValue; }));

    public new static readonly DependencyProperty HeightProperty = DependencyProperty.Register("Height", typeof(double), typeof(SizedElement),
                                                                                               new PropertyMetadata(
                                                                                                 delegate(DependencyObject o, DependencyPropertyChangedEventArgs args) { ((SizedElement)o).Height = (double)args.NewValue; }));

    private readonly UIElement myElement;
    private double myWidth;
    private double myHeight;

    public SizedElement(UIElement element, Size size)
    {
      myElement = element;
      myWidth = size.Width;
      myHeight = size.Height;
      Children.Add(element);
    }

    public new double Width
    {
      get { return myWidth; }
      set
      {
        myWidth = value;
        InvalidateMeasure();
      }
    }

    public new double Height
    {
      get { return myHeight; }
      set
      {
        myHeight = value;
        InvalidateMeasure();
      }
    }

    protected override Size MeasureOverride(Size availableSize)
    {
      var size = new Size(Width, Height);
      myElement.Measure(size);
      return size;
    }

    protected override Size ArrangeOverride(Size finalSize)
    {
      myElement.Arrange(new Rect(0, 0, finalSize.Width, finalSize.Height));
      return finalSize;
    }
  }
}