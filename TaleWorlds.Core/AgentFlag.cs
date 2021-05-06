// Decompiled with JetBrains decompiler
// Type: TaleWorlds.Core.AgentFlag
// Assembly: TaleWorlds.Core, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 43F7B45C-9AE1-4715-A37B-2364840F62AF
// Assembly location: E:\SteamLibrary\steamapps\common\Mount & Blade II Bannerlord\bin\Win64_Shipping_Client\TaleWorlds.Core.dll

using System;

namespace TaleWorlds.Core
{
  [Flags]
  public enum AgentFlag : uint
  {
    None = 0,
    Mountable = 1,
    CanJump = 2,
    CanRear = 4,
    CanAttack = 8,
    CanDefend = 16, // 0x00000010
    RunsAwayWhenHit = 32, // 0x00000020
    CanCharge = 64, // 0x00000040
    CanBeCharged = 128, // 0x00000080
    CanClimbLadders = 256, // 0x00000100
    CanBeInGroup = 512, // 0x00000200
    CanSprint = 1024, // 0x00000400
    IsHumanoid = 2048, // 0x00000800
    CanGetScared = 4096, // 0x00001000
    CanRide = 8192, // 0x00002000
    CanWieldWeapon = 16384, // 0x00004000
    CanCrouch = 32768, // 0x00008000
    CanGetAlarmed = 65536, // 0x00010000
    CanWander = 131072, // 0x00020000
    CanKick = 524288, // 0x00080000
    CanRetreat = 1048576, // 0x00100000
    MoveAsHerd = 2097152, // 0x00200000
    MoveForwardOnly = 4194304, // 0x00400000
    IsUnique = 8388608, // 0x00800000
    CanUseAllBowsMounted = 16777216, // 0x01000000
    CanReloadAllXBowsMounted = 33554432, // 0x02000000
    CanDeflectArrowsWith2HSword = 67108864, // 0x04000000
  }
}
