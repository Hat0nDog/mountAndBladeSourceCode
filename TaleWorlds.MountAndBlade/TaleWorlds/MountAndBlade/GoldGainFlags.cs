// Decompiled with JetBrains decompiler
// Type: TaleWorlds.MountAndBlade.GoldGainFlags
// Assembly: TaleWorlds.MountAndBlade, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3C0E3F42-756F-4407-B817-06BA412A78B0
// Assembly location: E:\SteamLibrary\steamapps\common\Mount & Blade II Bannerlord\bin\Win64_Shipping_Client\TaleWorlds.MountAndBlade.dll

using System;

namespace TaleWorlds.MountAndBlade
{
  [Flags]
  public enum GoldGainFlags : ushort
  {
    FirstRangedKill = 1,
    FirstMeleeKill = 2,
    FirstAssist = 4,
    SecondAssist = 8,
    ThirdAssist = 16, // 0x0010
    FifthKill = 32, // 0x0020
    TenthKill = 64, // 0x0040
    DefaultKill = 128, // 0x0080
    DefaultAssist = 256, // 0x0100
    ObjectiveCompleted = 512, // 0x0200
    ObjectiveDestroyed = 1024, // 0x0400
    PerkBonus = 2048, // 0x0800
  }
}
