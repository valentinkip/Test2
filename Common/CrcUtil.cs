namespace ManchkinQuest.Common
{
  public static class CrcUtil
  {
    public static long CalcCRC(string s)
    {
      return PutString(0, s);
    }

    private static long PutByte(long hash, byte b)
    {
      hash += b + 123;
      hash += (hash << 21);
      hash ^= (hash >> 11);
      return hash;
    }

    private static long PutChar(long hash, char c)
    {
      hash = PutByte(hash, (byte)c);
      hash = PutByte(hash, (byte)(c >> 8));
      return hash;
    }

    private static long PutString(long hash, string s)
    {
      hash = PutInt(hash, s.Length);
      foreach(char c in s)
        hash = PutChar(hash, c);
      return hash;
    }

    private static long PutInt(long hash, int i)
    {
      hash = PutByte(hash, (byte)i);
      hash = PutByte(hash, (byte)(i >> 8));
      hash = PutByte(hash, (byte)(i >> 16));
      hash = PutByte(hash, (byte)(i >> 24));
      return hash;
    }
  }
}