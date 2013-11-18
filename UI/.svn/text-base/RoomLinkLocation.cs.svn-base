namespace ManchkinQuest.UI
{
  struct RoomLinkLocation
  {
    public readonly int X;
    public readonly int Y;
    public readonly bool Vertical;

    public RoomLinkLocation(int x, int y, bool vertical)
    {
      X = x;
      Y = y;
      Vertical = vertical;
    }

    public bool Equals(RoomLinkLocation other)
    {
      return other.X == X && other.Y == Y && other.Vertical.Equals(Vertical);
    }

    public override bool Equals(object obj)
    {
      if (ReferenceEquals(null, obj)) return false;
      if (obj.GetType() != typeof(RoomLinkLocation)) return false;
      return Equals((RoomLinkLocation)obj);
    }

    public override int GetHashCode()
    {
      unchecked
      {
        int result = X;
        result = (result * 397) ^ Y;
        result = (result * 397) ^ Vertical.GetHashCode();
        return result;
      }
    }
  }
}