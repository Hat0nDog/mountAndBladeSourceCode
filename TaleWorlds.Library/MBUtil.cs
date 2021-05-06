// Decompiled with JetBrains decompiler
// Type: TaleWorlds.Library.MBUtil
// Assembly: TaleWorlds.Library, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 271D0DB8-3C5D-4850-A565-542B4CF7D347
// Assembly location: E:\SteamLibrary\steamapps\common\Mount & Blade II Bannerlord\bin\Win64_Shipping_Client\TaleWorlds.Library.dll

using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace TaleWorlds.Library
{
  public static class MBUtil
  {
    public static void DirectoryCopy(string sourceDirName, string destDirName, bool copySubDirs)
    {
      DirectoryInfo directoryInfo1 = new DirectoryInfo(sourceDirName);
      if (!directoryInfo1.Exists)
        return;
      DirectoryInfo[] directories = directoryInfo1.GetDirectories();
      if (!Directory.Exists(destDirName))
        Directory.CreateDirectory(destDirName);
      foreach (FileInfo file in directoryInfo1.GetFiles())
      {
        string destFileName = Path.Combine(destDirName, file.Name);
        file.CopyTo(destFileName, false);
      }
      if (!copySubDirs)
        return;
      foreach (DirectoryInfo directoryInfo2 in directories)
      {
        string destDirName1 = Path.Combine(destDirName, directoryInfo2.Name);
        MBUtil.DirectoryCopy(directoryInfo2.FullName, destDirName1, copySubDirs);
      }
    }

    public static T[] ArrayAdd<T>(T[] tArray, T t)
    {
      List<T> list = ((IEnumerable<T>) tArray).ToList<T>();
      list.Add(t);
      return list.ToArray();
    }

    public static T[] ArrayRemove<T>(T[] tArray, T t)
    {
      List<T> list = ((IEnumerable<T>) tArray).ToList<T>();
      return !list.Remove(t) ? tArray : list.ToArray();
    }
  }
}
