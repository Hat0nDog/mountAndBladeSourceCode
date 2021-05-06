// Decompiled with JetBrains decompiler
// Type: TaleWorlds.Core.MetaDataExtensions
// Assembly: TaleWorlds.Core, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 43F7B45C-9AE1-4715-A37B-2364840F62AF
// Assembly location: E:\SteamLibrary\steamapps\common\Mount & Blade II Bannerlord\bin\Win64_Shipping_Client\TaleWorlds.Core.dll

using System;
using TaleWorlds.Library;
using TaleWorlds.SaveSystem;

namespace TaleWorlds.Core
{
  public static class MetaDataExtensions
  {
    public static DateTime GetCreationTime(this MetaData metaData)
    {
      DateTime result;
      return DateTime.TryParse(metaData?["CreationTime"], out result) ? result : DateTime.MinValue;
    }

    public static ApplicationVersion GetApplicationVersion(this MetaData metaData)
    {
      string versionAsString = metaData?["ApplicationVersion"];
      return versionAsString == null ? ApplicationVersion.Empty : ApplicationVersion.FromString(versionAsString, ApplicationVersionGameType.Singleplayer);
    }

    public static string[] GetModules(this MetaData metaData)
    {
      string str;
      if (metaData == null || !metaData.TryGetValue("Modules", out str))
        return new string[0];
      return str.Split(';');
    }

    public static ApplicationVersion GetModuleVersion(
      this MetaData metaData,
      string moduleName)
    {
      string key = "Module_" + moduleName;
      if (metaData != null)
      {
        string versionAsString;
        if (metaData.TryGetValue(key, out versionAsString))
        {
          try
          {
            return ApplicationVersion.FromString(versionAsString, ApplicationVersionGameType.Singleplayer);
          }
          catch (Exception ex)
          {
          }
        }
      }
      return ApplicationVersion.Empty;
    }
  }
}
