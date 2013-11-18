using System.Globalization;
using System.IO;
using System.Text;
using JetBrains.Annotations;

namespace ManchkinQuest.Common
{
  public static class EncryptionUtil
  {
    [NotNull]
    public static char[] EncryptString([NotNull]string s, string password)
    {
      return EncryptChars(s.ToCharArray(), password);
    }

    [NotNull]
    public static string DecryptString([NotNull]char[] chars, string password)
    {
      return new string(EncryptChars(chars, password));
    }

    private static char[] EncryptChars(char[] chars, string password)
    {
      var chars1 = new char[chars.Length];
      for (int i = 0; i < chars.Length; i++)
      {
        char c = chars[i];
        char c1 = password[i % password.Length];
        chars1[i] = (char)(c ^ c1);
      }
      return chars1;
    }

    [NotNull]
    public static string EncryptStringToString([NotNull]string s, string password)
    {
      byte[] bytes;
      using(var stream = new MemoryStream())
      using(var writer = new BinaryWriter(stream))
      {
        writer.Write(s);
        bytes = stream.ToArray();
      }
      return EncryptBytesToString(bytes, password);
    }

    [NotNull]
    public static string DecryptStringFromString([NotNull]string s, string password)
    {
      byte[] bytes = DecryptBytesFromString(s, password);
      using(var stream = new MemoryStream(bytes))
      using(var reader = new BinaryReader(stream))
        return reader.ReadString();
    }

    [NotNull]
    public static byte[] EncryptBytes([NotNull]byte[] bytes, [NotNull] string password)
    {
      var newBytes = new byte[bytes.Length];
      for(int i = 0; i < bytes.Length; i++)
      {
        byte b = bytes[i];
        char c = password[i % password.Length];
        newBytes[i] = (byte)(b ^ c);
      }
      return newBytes;
    }

    [NotNull]
    public static byte[] DecryptBytes([NotNull]byte[] bytes, string password)
    {
      return EncryptBytes(bytes, password);
    }

    public static string EncryptBytesToString(byte[] bytes, [CanBeNull] string password)
    {
      byte[] encyptedBytes = password != null ? EncryptBytes(bytes, password) : bytes;
      var builder = new StringBuilder();
      foreach(byte b in encyptedBytes)
        builder.AppendFormat("{0:X2}", b);
      return builder.ToString();
    }

    public static byte[] DecryptBytesFromString(string s, [CanBeNull] string password)
    {
      var bytes = new byte[s.Length / 2];
      for (int i = 0; i < bytes.Length; i++)
        bytes[i] = byte.Parse(s.Substring(i * 2, 2), NumberStyles.HexNumber);
      return password != null ? DecryptBytes(bytes, password) : bytes;
    }
  }
}