using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using ManchkinQuest.Core;

namespace ManchkinQuest.UI
{
  public class MazePanel : MazePanelBase
  {
    protected override IEnumerable<IMazeRoomInfo> Rooms
    {
      get { return Game.Maze.AllRooms; }
    }

    protected override void RegisterRoomElement(RoomElement roomElement)
    {
      roomElement.MouseEnter += delegate
      {
        roomElement.Cursor = Cursors.Hand;
        roomElement.RenderTransform = new ScaleTransform { ScaleX = 1.1, ScaleY = 1.1, CenterX = roomElement.ActualWidth / 2, CenterY = roomElement.ActualHeight / 2 };
      };
      roomElement.MouseLeave += delegate
      {
        roomElement.Cursor = Cursors.Arrow;
        roomElement.RenderTransform = null;
      };
      roomElement.MouseLeftButtonUp += delegate
      {
        roomElement.RenderTransform = null;

        var zoomedElement = new MazeFragmentElement(Game, roomElement.RoomInfo);
        UIElement rootVisual = Application.Current.RootVisual;
        Size mainSize = rootVisual.RenderSize;
        Size size = roomElement.RenderSize;
        double scaleW = mainSize.Width / size.Width;
        double scaleH = mainSize.Height / size.Height;
        double scale = Math.Min(scaleW, scaleH);
        double endWidth = size.Width * scale;
        double endHeight = size.Height * scale;
        Rect startRect = roomElement.GetRect(rootVisual);
        var endRect = new Rect((mainSize.Width - endWidth) / 2, (mainSize.Height - endHeight) / 2, endWidth, endHeight);
//        roomElement.Visibility = Visibility.Collapsed;
        UIManager.Instance.ShowInPopup(zoomedElement, startRect, endRect/*, delegate { roomElement.Visibility = Visibility.Visible; }*/);
      };
    }
  }
}