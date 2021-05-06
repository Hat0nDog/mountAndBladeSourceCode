// Decompiled with JetBrains decompiler
// Type: TaleWorlds.MountAndBlade.WeaponStatsData
// Assembly: TaleWorlds.MountAndBlade, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3C0E3F42-756F-4407-B817-06BA412A78B0
// Assembly location: E:\SteamLibrary\steamapps\common\Mount & Blade II Bannerlord\bin\Win64_Shipping_Client\TaleWorlds.MountAndBlade.dll

using TaleWorlds.DotNet;
using TaleWorlds.Library;

namespace TaleWorlds.MountAndBlade
{
  [EngineStruct("Weapon_stats_data")]
  public struct WeaponStatsData
  {
    public uint Properties;
    public ulong WeaponFlags;
    public int WeaponClass;
    public int AmmoClass;
    public int ItemUsageIndex;
    public int ThrustSpeed;
    public int SwingSpeed;
    public int MissileSpeed;
    public int ShieldArmor;
    public int Accuracy;
    public int WeaponLength;
    public float WeaponBalance;
    public int ThrustDamage;
    public int ThrustDamageType;
    public int SwingDamage;
    public int SwingDamageType;
    public int DefendSpeed;
    public float SweetSpot;
    public short MaxDataValue;
    public MatrixFrame WeaponFrame;
    public Vec3 RotationSpeed;
  }
}
