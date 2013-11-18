using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using ManchkinQuest.Core;

namespace ManchkinQuest.UI
{
  public abstract class MazePanelBase : Panel
  {
    private readonly Dictionary<IMazeRoomInfo, RoomElement> myRoomElements = new Dictionary<IMazeRoomInfo, RoomElement>();
    private readonly Dictionary<RoomLinkLocation, RoomLinkElement> myRoomLinkElements = new Dictionary<RoomLinkLocation, RoomLinkElement>();
    private IGame myGame;

    public IGame Game
    {
      get { return myGame; }
      set
      {
        myGame = value;
        Update();
      }
    }

    protected abstract IEnumerable<IMazeRoomInfo> Rooms { get; }

    protected virtual void RegisterRoomElement(RoomElement roomElement)
    {      
    }

    public void Update()
    {
      Children.Clear();
      myRoomElements.Clear();
      myRoomLinkElements.Clear();

      if (Game == null) return;

      bool containsEnter = Rooms.Any(info => info.Room is IEnterRoom);
      foreach(IMazeRoomInfo roomInfo in Rooms)
      {
// ReSharper disable AccessToModifiedClosure
        var roomElement = new RoomElement(roomInfo, Game.Players.Where(player => player.Location == roomInfo).ToList());
// ReSharper restore AccessToModifiedClosure
        Children.Add(roomElement);
        myRoomElements.Add(roomInfo, roomElement);

        RegisterRoomElement(roomElement);

        foreach(Direction direction in CoreUtil.EnumerateDirections())
        {
          IRoomLink roomLink = roomInfo.Link(direction);
          int x = roomInfo.X;
          int y = roomInfo.Y;
          RoomLinkLocation location;
          switch(direction)
          {
            case Direction.Left:
              location = new RoomLinkLocation(x, y, true);
              break;
            case Direction.Right:
              location = new RoomLinkLocation(x + 1, y, true);
              break;
            case Direction.Up:
              location = new RoomLinkLocation(x, y, false);
              break;
            case Direction.Down:
              location = new RoomLinkLocation(x, y + 1, false);
              break;
            default:
              throw new ArgumentOutOfRangeException();
          }
          if (containsEnter)
          {
            // skip links for enter room
            if (location.Vertical)
            {
              if (location.Y == 0 && (location.X == 0 || location.X == 1)) continue;
            }
            else
            {
              if (location.X == 0 && (location.Y == 0 || location.Y == 1)) continue;
            }
          }
          if (!myRoomLinkElements.ContainsKey(location))
          {
            var linkElement = new RoomLinkElement(roomLink, location);
            Children.Add(linkElement);
            myRoomLinkElements.Add(location, linkElement);
          }
        }
      }

      InvalidateArrange();
    }

    protected override Size ArrangeOverride(Size finalSize)
    {
      if (Game == null) return finalSize;

      StuffManager stuffManager = StuffManager.Instance;

      double linkWidth = stuffManager.RoomLinkWidthToRoomSize;
      double linkHeight = linkWidth/stuffManager.RoomLinkWidthToHeight;
      
      IEnumerable<IMazeRoomInfo> rooms = Rooms;
      int minX = rooms.Select(info => info.X).Min();
      int maxX = rooms.Select(info => info.X).Max();
      int minY = rooms.Select(info => info.Y).Min();
      int maxY = rooms.Select(info => info.Y).Max();

      double step = 1 + linkHeight * (1 - 2 * StuffManager.RoomLinkJunctionToHeight);
      double totalW = (maxX - minX + 1)*step + linkHeight;
      double totalH = (maxY - minY + 1)*step + linkHeight;

      double roomSize;
      if (totalW/totalH > finalSize.Width/finalSize.Height)
        roomSize = finalSize.Width/totalW;
      else
        roomSize = finalSize.Height / totalH;
      linkWidth *= roomSize;
      linkHeight *= roomSize;
      step *= roomSize;
      totalW *= roomSize;
      totalH *= roomSize;

      double shiftX = (roomSize - linkWidth) / 2;
      double shiftY = -linkHeight * (1 - StuffManager.RoomLinkJunctionToHeight);
      double startX = (finalSize.Width - totalW) / 2 - shiftY;
      double startY = (finalSize.Height - totalH) / 2 - shiftY;

      foreach(RoomElement roomElement in myRoomElements.Values)
      {
        IMazeRoomInfo roomInfo = roomElement.RoomInfo;
        double x = startX + (roomInfo.X - minX)*step;
        double y = startY + (roomInfo.Y - minY)*step;
        if (roomInfo.Room is IEnterRoom)
        {
          double size = roomSize + 2 * linkHeight * (1 - StuffManager.RoomLinkJunctionToHeight);
          roomElement.Arrange(new Rect(x - (size - roomSize) / 2, y - (size - roomSize) / 2, size, size));
        }
        else
        {
          roomElement.Arrange(new Rect(x, y, roomSize, roomSize));
        }
      }

      foreach(RoomLinkElement linkElement in myRoomLinkElements.Values)
      {
        RoomLinkLocation location = linkElement.Location;
        double x = startX + (location.X - minX)*step;
        double y = startY + (location.Y - minY)*step;
        var rect = location.Vertical
                     ? new Rect(x + shiftY, y + shiftX, linkHeight, linkWidth)
                     : new Rect(x + shiftX, y + shiftY, linkWidth, linkHeight);
        linkElement.Arrange(rect);
      }

      return finalSize;
    }
  }
}