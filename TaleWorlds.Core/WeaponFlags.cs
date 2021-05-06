// Decompiled with JetBrains decompiler
// Type: TaleWorlds.Core.WeaponFlags
// Assembly: TaleWorlds.Core, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 43F7B45C-9AE1-4715-A37B-2364840F62AF
// Assembly location: E:\SteamLibrary\steamapps\common\Mount & Blade II Bannerlord\bin\Win64_Shipping_Client\TaleWorlds.Core.dll

using System;

namespace TaleWorlds.Core
{
  [Flags]
  public enum WeaponFlags : ulong
  {
    MeleeWeapon = 1,
    RangedWeapon = 2,
    WeaponMask = RangedWeapon | MeleeWeapon, // 0x0000000000000003
    FirearmAmmo = 4,
    NotUsableWithOneHand = 16, // 0x0000000000000010
    NotUsableWithTwoHand = 32, // 0x0000000000000020
    HandUsageMask = NotUsableWithTwoHand | NotUsableWithOneHand, // 0x0000000000000030
    WideGrip = 64, // 0x0000000000000040
    AttachAmmoToVisual = 128, // 0x0000000000000080
    Consumable = 256, // 0x0000000000000100
    HasHitPoints = 512, // 0x0000000000000200
    DataValueMask = HasHitPoints | Consumable, // 0x0000000000000300
    HasString = 1024, // 0x0000000000000400
    StringHeldByHand = 3072, // 0x0000000000000C00
    UnloadWhenSheathed = 4096, // 0x0000000000001000
    AffectsArea = 8192, // 0x0000000000002000
    AffectsAreaBig = 16384, // 0x0000000000004000
    Burning = 32768, // 0x0000000000008000
    BonusAgainstShield = 65536, // 0x0000000000010000
    CanPenetrateShield = 131072, // 0x0000000000020000
    CantReloadOnHorseback = 262144, // 0x0000000000040000
    AutoReload = 524288, // 0x0000000000080000
    TwoHandIdleOnMount = 2097152, // 0x0000000000200000
    NoBlood = 4194304, // 0x0000000000400000
    PenaltyWithShield = 8388608, // 0x0000000000800000
    CanDismount = 16777216, // 0x0000000001000000
    MissileWithPhysics = 33554432, // 0x0000000002000000
    MultiplePenetration = 67108864, // 0x0000000004000000
    CanKnockDown = 134217728, // 0x0000000008000000
    CanBlockRanged = 268435456, // 0x0000000010000000
    LeavesTrail = 536870912, // 0x0000000020000000
    CanCrushThrough = 1073741824, // 0x0000000040000000
    UseHandAsThrowBase = 2147483648, // 0x0000000080000000
    AmmoBreaksOnBounceBack = 4294967296, // 0x0000000100000000
    AmmoCanBreakOnBounceBack = 8589934592, // 0x0000000200000000
    AmmoBreakOnBounceBackMask = AmmoCanBreakOnBounceBack | AmmoBreaksOnBounceBack, // 0x0000000300000000
    AmmoSticksWhenShot = 17179869184, // 0x0000000400000000
  }
}
