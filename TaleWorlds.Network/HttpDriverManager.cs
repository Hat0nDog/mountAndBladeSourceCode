// Decompiled with JetBrains decompiler
// Type: TaleWorlds.Network.HttpDriverManager
// Assembly: TaleWorlds.Network, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: EDF757B2-6540-4679-910A-3C5D58B2EF82
// Assembly location: E:\SteamLibrary\steamapps\common\Mount & Blade II Bannerlord\bin\Win64_Shipping_Client\TaleWorlds.Network.dll

using System.Collections.Generic;

namespace TaleWorlds.Network
{
  public static class HttpDriverManager
  {
    private static Dictionary<string, IHttpDriver> _httpDrivers = new Dictionary<string, IHttpDriver>();
    private static string _defaultHttpDriver;

    static HttpDriverManager() => HttpDriverManager.AddHttpDriver("DotNet", (IHttpDriver) new DotNetHttpDriver());

    public static void AddHttpDriver(string name, IHttpDriver driver)
    {
      if (HttpDriverManager._httpDrivers.Count == 0)
        HttpDriverManager._defaultHttpDriver = name;
      HttpDriverManager._httpDrivers.Add(name, driver);
    }

    public static void SetDefault(string name)
    {
      if (HttpDriverManager.GetHttpDriver(name) == null)
        return;
      HttpDriverManager._defaultHttpDriver = name;
    }

    public static IHttpDriver GetHttpDriver(string name)
    {
      IHttpDriver httpDriver;
      HttpDriverManager._httpDrivers.TryGetValue(name, out httpDriver);
      return httpDriver;
    }

    public static IHttpDriver GetDefaultHttpDriver() => HttpDriverManager.GetHttpDriver(HttpDriverManager._defaultHttpDriver);
  }
}
