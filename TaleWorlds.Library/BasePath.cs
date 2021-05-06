// Decompiled with JetBrains decompiler
// Type: TaleWorlds.Library.BasePath
// Assembly: TaleWorlds.Library, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 271D0DB8-3C5D-4850-A565-542B4CF7D347
// Assembly location: E:\SteamLibrary\steamapps\common\Mount & Blade II Bannerlord\bin\Win64_Shipping_Client\TaleWorlds.Library.dll

using System.IO;
using System.Reflection;

namespace TaleWorlds.Library
{
  public static class BasePath
  {
    public static string Name
    {
      get
      {
        switch (ApplicationPlatform.CurrentPlatform)
        {
          case Platform.Orbis:
            return "/app0/";
          case Platform.Durango:
            return "/";
          case Platform.Web:
            return Path.GetDirectoryName(Assembly.GetEntryAssembly().Location) + "/../../";
          default:
            return "../../";
        }
      }
    }
  }
}
