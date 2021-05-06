// Decompiled with JetBrains decompiler
// Type: TaleWorlds.MountAndBlade.LoadingBackgroundVisual
// Assembly: TaleWorlds.MountAndBlade, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3C0E3F42-756F-4407-B817-06BA412A78B0
// Assembly location: E:\SteamLibrary\steamapps\common\Mount & Blade II Bannerlord\bin\Win64_Shipping_Client\TaleWorlds.MountAndBlade.dll

using System;
using System.IO;
using TaleWorlds.Engine;
using TaleWorlds.ModuleManager;

namespace TaleWorlds.MountAndBlade
{
  public class LoadingBackgroundVisual
  {
    private static Texture GetTexture()
    {
      string path = ModuleHelper.GetModuleFullPath("Native") + "LoadingTextures/";
      try
      {
        string[] files = Directory.GetFiles(path, "*.tif");
        int length = files.Length;
        if (length != 0)
        {
          int index = new Random().Next(length);
          string fileName = files[index];
          int num = files[index].IndexOf("/LoadingTextures/");
          if (num != -1)
          {
            int startIndex = num + 17;
            fileName = files[index].Substring(startIndex);
          }
          return Texture.CreateTextureFromPath(path, fileName);
        }
      }
      catch (Exception ex)
      {
        MBDebug.Print(ex.ToString());
      }
      return (Texture) null;
    }
  }
}
