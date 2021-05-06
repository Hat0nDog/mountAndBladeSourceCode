// Decompiled with JetBrains decompiler
// Type: TaleWorlds.MountAndBlade.MultiplayerMessageFilter
// Assembly: TaleWorlds.MountAndBlade, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3C0E3F42-756F-4407-B817-06BA412A78B0
// Assembly location: E:\SteamLibrary\steamapps\common\Mount & Blade II Bannerlord\bin\Win64_Shipping_Client\TaleWorlds.MountAndBlade.dll

using System;

namespace TaleWorlds.MountAndBlade
{
  [Flags]
  public enum MultiplayerMessageFilter : ulong
  {
    None = 0,
    Peers = 1,
    Messaging = 2,
    Items = 4,
    General = 8,
    Equipment = 16, // 0x0000000000000010
    EquipmentDetailed = 32, // 0x0000000000000020
    Formations = 64, // 0x0000000000000040
    Agents = 128, // 0x0000000000000080
    AgentsDetailed = 256, // 0x0000000000000100
    Mission = 512, // 0x0000000000000200
    MissionDetailed = 1024, // 0x0000000000000400
    AgentAnimations = 2048, // 0x0000000000000800
    SiegeWeapons = 4096, // 0x0000000000001000
    MissionObjects = 8192, // 0x0000000000002000
    MissionObjectsDetailed = 16384, // 0x0000000000004000
    SiegeWeaponsDetailed = 32768, // 0x0000000000008000
    Orders = 65536, // 0x0000000000010000
    GameMode = 131072, // 0x0000000000020000
    Administration = 262144, // 0x0000000000040000
    Particles = 524288, // 0x0000000000080000
    RPC = 1048576, // 0x0000000000100000
    All = 4294967295, // 0x00000000FFFFFFFF
    LightLogging = GameMode | MissionObjects | Mission | Agents | General | Peers, // 0x0000000000022289
    NormalLogging = LightLogging | RPC | Particles | Administration | SiegeWeapons | Equipment | Items, // 0x00000000001E329D
    AllWithoutDetails = NormalLogging | Orders | Formations | Messaging, // 0x00000000001F32DF
  }
}
