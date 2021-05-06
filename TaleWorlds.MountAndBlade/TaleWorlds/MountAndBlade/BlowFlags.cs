// Decompiled with JetBrains decompiler
// Type: TaleWorlds.MountAndBlade.BlowFlags
// Assembly: TaleWorlds.MountAndBlade, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3C0E3F42-756F-4407-B817-06BA412A78B0
// Assembly location: E:\SteamLibrary\steamapps\common\Mount & Blade II Bannerlord\bin\Win64_Shipping_Client\TaleWorlds.MountAndBlade.dll

using System;

namespace TaleWorlds.MountAndBlade
{
  [Flags]
  public enum BlowFlags
  {
    None = 0,
    KnockBack = 16, // 0x00000010
    KnockDown = 32, // 0x00000020
    NoSound = 64, // 0x00000040
    CrushThrough = 128, // 0x00000080
    ShrugOff = 256, // 0x00000100
    MakesRear = 512, // 0x00000200
    NonTipThrust = 1024, // 0x00000400
    CanDismount = 2048, // 0x00000800
  }
}
