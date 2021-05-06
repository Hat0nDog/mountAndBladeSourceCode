// Decompiled with JetBrains decompiler
// Type: TaleWorlds.Library.ConfigurationManager
// Assembly: TaleWorlds.Library, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 271D0DB8-3C5D-4850-A565-542B4CF7D347
// Assembly location: E:\SteamLibrary\steamapps\common\Mount & Blade II Bannerlord\bin\Win64_Shipping_Client\TaleWorlds.Library.dll

namespace TaleWorlds.Library
{
  public static class ConfigurationManager
  {
    private static IConfigurationManager _configurationManager;

    public static void SetConfigurationManager(IConfigurationManager configurationManager) => ConfigurationManager._configurationManager = configurationManager;

    public static string GetAppSettings(string name) => ConfigurationManager._configurationManager != null ? ConfigurationManager._configurationManager.GetAppSettings(name) : (string) null;
  }
}
