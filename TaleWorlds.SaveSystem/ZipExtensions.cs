// Decompiled with JetBrains decompiler
// Type: TaleWorlds.SaveSystem.ZipExtensions
// Assembly: TaleWorlds.SaveSystem, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 786EF53D-D2EC-43A3-8B8C-4717B7406D76
// Assembly location: E:\SteamLibrary\steamapps\common\Mount & Blade II Bannerlord\bin\Win64_Shipping_Client\TaleWorlds.SaveSystem.dll

using System.IO;
using System.IO.Compression;

namespace TaleWorlds.SaveSystem
{
  internal static class ZipExtensions
  {
    public static void FillFrom(this ZipArchiveEntry entry, byte[] data)
    {
      using (Stream stream = entry.Open())
        stream.Write(data, 0, data.Length);
    }

    public static void FillFrom(this ZipArchiveEntry entry, TaleWorlds.Library.BinaryWriter writer)
    {
      using (Stream stream = entry.Open())
      {
        byte[] data = writer.Data;
        stream.Write(data, 0, data.Length);
      }
    }

    public static TaleWorlds.Library.BinaryReader GetBinaryReader(
      this ZipArchiveEntry entry)
    {
      using (Stream stream = entry.Open())
      {
        using (MemoryStream memoryStream = new MemoryStream())
        {
          stream.CopyTo((Stream) memoryStream);
          return new TaleWorlds.Library.BinaryReader(memoryStream.ToArray());
        }
      }
    }

    public static byte[] GetBinaryData(this ZipArchiveEntry entry)
    {
      using (Stream stream = entry.Open())
      {
        using (MemoryStream memoryStream = new MemoryStream())
        {
          stream.CopyTo((Stream) memoryStream);
          return memoryStream.ToArray();
        }
      }
    }
  }
}
