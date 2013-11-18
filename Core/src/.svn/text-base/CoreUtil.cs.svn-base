using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Windows.Media;
using ManchkinQuest.Common;

namespace ManchkinQuest.Core
{
  public static class CoreUtil
  {
    public static Color ToColor(this PlayerColor playerColor)
    {
      switch(playerColor)
      {
        case PlayerColor.Red: return Colors.Red;
        case PlayerColor.Green: return Colors.Green;
        case PlayerColor.Blue: return Colors.Blue;
        case PlayerColor.Yellow: return Colors.Yellow;
        default:
          throw new ArgumentOutOfRangeException("playerColor");
      }
    }

    public static IEnumerable<Direction> EnumerateDirections()
    {
      yield return Direction.Left;
      yield return Direction.Right;
      yield return Direction.Up;
      yield return Direction.Down;
    }

    public static byte[] SerializeEvent(IGameEvent @event)
    {
      using(var memoryStream = new MemoryStream())
      {
        using(var writer = new BinaryWriter(memoryStream))
        {
          string typeName = @event.GetType().FullName;
          Debug.Assert(typeName != null);
          writer.Write(typeName);
          @event.WriteTo(writer);
        }
        return memoryStream.ToArray();
      }
    }

    public static IGameEvent DeserializeEvent(byte[] bytes)
    {
      using(var stream = new MemoryStream(bytes))
      using(var reader = new BinaryReader(stream))
      {
        string typeName = reader.ReadString();
        Type type = Assembly.GetExecutingAssembly().GetType(typeName);
        var @event = (IGameEvent)Activator.CreateInstance(type);
        @event.ReadFrom(reader);
        return @event;
      }
    }
  }
}