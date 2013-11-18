using System;
using System.Collections.Generic;
using System.Linq;

namespace ManchkinQuest.Core.Impl
{
  class Maze : IMaze
  {
    private readonly Dictionary<Coords, RoomInfo> myRooms = new Dictionary<Coords, RoomInfo>();
    private readonly Dictionary<Coords, IRoomLink> myLeftRoomLinks = new Dictionary<Coords, IRoomLink>();
    private readonly Dictionary<Coords, IRoomLink> myUpRoomLinks = new Dictionary<Coords, IRoomLink>();

    public Maze()
    {
      StuffManager stuffManager = StuffManager.Instance;
      myRooms.Add(new Coords(0, 0), new RoomInfo(this, stuffManager.EnterRoom, 0, 0));
      myLeftRoomLinks.Add(new Coords(0, 0), stuffManager.OpenPassageway);
      myLeftRoomLinks.Add(new Coords(1, 0), stuffManager.OpenPassageway);
      myUpRoomLinks.Add(new Coords(0, 0), stuffManager.OpenPassageway);
      myUpRoomLinks.Add(new Coords(0, 1), stuffManager.OpenPassageway);
    }

    public IMazeRoomInfo Room(int x, int y)
    {
      RoomInfo info;
      return myRooms.TryGetValue(new Coords(x, y), out info) ? info : null;
    }

    public IMazeRoomInfo Enter
    {
      get { return Room(0, 0); }
    }

    public IEnumerable<IMazeRoomInfo> AllRooms
    {
      get { return myRooms.Values.Cast<IMazeRoomInfo>(); }
    }

    public IMazeRoomInfo AddRoom(int x, int y, IRoom room)
    {
      var roomInfo = new RoomInfo(this, room, x, y);
      myRooms.Add(new Coords(x, y), roomInfo);
      return roomInfo;
    }

    private class RoomInfo : IMazeRoomInfo
    {
      public Maze Maze { get; private set; }
      public IRoom Room { get; private set; }
      public int X { get; private set; }
      public int Y { get; private set; }

      public RoomInfo(Maze maze, IRoom room, int x, int y)
      {
        Maze = maze;
        Room = room;
        X = x;
        Y = y;
      }

      public IRoomLink Link(Direction direction)
      {
        switch(direction)
        {
          case Direction.Left:
            return Maze.myLeftRoomLinks[new Coords(X, Y)];
          case Direction.Right:
            return Maze.myLeftRoomLinks[new Coords(X + 1, Y)];
          case Direction.Up:
            return Maze.myUpRoomLinks[new Coords(X, Y)];
          case Direction.Down:
            return Maze.myUpRoomLinks[new Coords(X, Y + 1)];
          default:
            throw new ArgumentOutOfRangeException("direction");
        }
      }

      public void SetLink(Direction direction, IRoomLink link)
      {
        switch (direction)
        {
          case Direction.Left:
            if (Y == 0 && (X == 0 || X == 1))
              throw new InvalidOperationException("Cannot change links of enter room");
            Maze.myLeftRoomLinks[new Coords(X, Y)] = link;
            break;
          case Direction.Right:
            if (Y == 0 && (X == 0 || X == -1))
              throw new InvalidOperationException("Cannot change links of enter room");
            Maze.myLeftRoomLinks[new Coords(X + 1, Y)] = link;
            break;
          case Direction.Up:
            if (X == 0 && (Y == 0 || Y == 1))
              throw new InvalidOperationException("Cannot change links of enter room");
            Maze.myUpRoomLinks[new Coords(X, Y)] = link;
            break;
          case Direction.Down:
            if (X == 0 && (Y == 0 || Y == -1))
              throw new InvalidOperationException("Cannot change links of enter room");
            Maze.myUpRoomLinks[new Coords(X, Y + 1)] = link;
            break;
          default:
            throw new ArgumentOutOfRangeException("direction");
        }
      }

      public bool Equals(RoomInfo other)
      {
        if (ReferenceEquals(null, other)) return false;
        if (ReferenceEquals(this, other)) return true;
        return other.X == X && other.Y == Y;
      }

      public override bool Equals(object obj)
      {
        if (ReferenceEquals(null, obj)) return false;
        if (ReferenceEquals(this, obj)) return true;
        if (obj.GetType() != typeof(RoomInfo)) return false;
        return Equals((RoomInfo)obj);
      }

      public override int GetHashCode()
      {
        unchecked
        {
          return (X*397) ^ Y;
        }
      }
    }

    private struct Coords
    {
      public readonly int X;
      public readonly int Y;

      public Coords(int x, int y)
      {
        X = x;
        Y = y;
      }

      public bool Equals(Coords other)
      {
        return other.X == X && other.Y == Y;
      }

      public override bool Equals(object obj)
      {
        if (ReferenceEquals(null, obj)) return false;
        if (obj.GetType() != typeof(Coords)) return false;
        return Equals((Coords)obj);
      }

      public override int GetHashCode()
      {
        unchecked
        {
          return (X*397) ^ Y;
        }
      }
    }
  }
}