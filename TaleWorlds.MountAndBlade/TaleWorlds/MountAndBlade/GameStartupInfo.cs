// Decompiled with JetBrains decompiler
// Type: TaleWorlds.MountAndBlade.GameStartupInfo
// Assembly: TaleWorlds.MountAndBlade, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3C0E3F42-756F-4407-B817-06BA412A78B0
// Assembly location: E:\SteamLibrary\steamapps\common\Mount & Blade II Bannerlord\bin\Win64_Shipping_Client\TaleWorlds.MountAndBlade.dll

namespace TaleWorlds.MountAndBlade
{
  public class GameStartupInfo
  {
    public GameStartupType StartupType { get; internal set; }

    public DedicatedServerType DedicatedServerType { get; internal set; }

    public int ServerPort { get; internal set; }

    public string ServerRegion { get; internal set; }

    public sbyte ServerPriority { get; internal set; }

    public string ServerGameMode { get; internal set; }

    public string CustomGameServerConfigFile { get; internal set; }

    public string OverridenUserName { get; internal set; }

    public bool IsStartedWithAntiCheat { get; internal set; }

    public int Permission { get; internal set; }

    public string EpicExchangeCode { get; internal set; }
  }
}
