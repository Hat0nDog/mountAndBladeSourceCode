// Decompiled with JetBrains decompiler
// Type: TaleWorlds.MountAndBlade.Blow
// Assembly: TaleWorlds.MountAndBlade, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3C0E3F42-756F-4407-B817-06BA412A78B0
// Assembly location: E:\SteamLibrary\steamapps\common\Mount & Blade II Bannerlord\bin\Win64_Shipping_Client\TaleWorlds.MountAndBlade.dll

using System.Runtime.InteropServices;
using TaleWorlds.Core;
using TaleWorlds.DotNet;
using TaleWorlds.Library;

namespace TaleWorlds.MountAndBlade
{
  [EngineStruct("Blow")]
  public struct Blow
  {
    public BlowWeaponRecord WeaponRecord;
    public Vec3 Position;
    public Vec3 Direction;
    public Vec3 SwingDirection;
    public int InflictedDamage;
    public int SelfInflictedDamage;
    public float BaseMagnitude;
    public float DefenderStunPeriod;
    public float AttackerStunPeriod;
    public float AbsorbedByArmor;
    public float MovementSpeedDamageModifier;
    public StrikeType StrikeType;
    public AgentAttackType AttackType;
    public BlowFlags BlowFlag;
    public int OwnerId;
    public sbyte BoneIndex;
    public BoneBodyPartType VictimBodyPart;
    public DamageTypes DamageType;
    [MarshalAs(UnmanagedType.I1)]
    public bool NoIgnore;
    [MarshalAs(UnmanagedType.I1)]
    public bool DamageCalculated;
    public float DamagedPercentage;

    public Blow(int ownerId)
      : this()
    {
      this.OwnerId = ownerId;
    }

    public bool IsMissile => this.WeaponRecord.IsMissile;

    public bool IsBlowCrit(int maxHitPointsOfVictim) => (double) this.InflictedDamage > (double) maxHitPointsOfVictim * 0.5;

    public bool IsBlowLow(int maxHitPointsOfVictim) => (double) this.InflictedDamage <= (double) maxHitPointsOfVictim * 0.100000001490116;
  }
}
