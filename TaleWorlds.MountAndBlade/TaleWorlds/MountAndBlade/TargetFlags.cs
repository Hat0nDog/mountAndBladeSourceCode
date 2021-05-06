// Decompiled with JetBrains decompiler
// Type: TaleWorlds.MountAndBlade.TargetFlags
// Assembly: TaleWorlds.MountAndBlade, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3C0E3F42-756F-4407-B817-06BA412A78B0
// Assembly location: E:\SteamLibrary\steamapps\common\Mount & Blade II Bannerlord\bin\Win64_Shipping_Client\TaleWorlds.MountAndBlade.dll

using System;

namespace TaleWorlds.MountAndBlade
{
  [Flags]
  public enum TargetFlags
  {
    None = 0,
    IsMoving = 1,
    IsFlammable = 2,
    IsStructure = 4,
    IsSiegeEngine = 8,
    IsAttacker = 16, // 0x00000010
    IsSmall = 32, // 0x00000020
    NotAThreat = 64, // 0x00000040
    DebugThreat = 128, // 0x00000080
  }
}
