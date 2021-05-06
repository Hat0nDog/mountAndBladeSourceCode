// Decompiled with JetBrains decompiler
// Type: TaleWorlds.MountAndBlade.KillingBlow
// Assembly: TaleWorlds.MountAndBlade, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3C0E3F42-756F-4407-B817-06BA412A78B0
// Assembly location: E:\SteamLibrary\steamapps\common\Mount & Blade II Bannerlord\bin\Win64_Shipping_Client\TaleWorlds.MountAndBlade.dll

using System.Runtime.InteropServices;
using TaleWorlds.Core;
using TaleWorlds.DotNet;
using TaleWorlds.Library;

namespace TaleWorlds.MountAndBlade
{
  [EngineStruct("Killing_blow")]
  public struct KillingBlow
  {
    public Vec3 RagdollImpulseLocalPoint;
    public Vec3 RagdollImpulseAmount;
    public int DeathAction;
    public DamageTypes DamageType;
    public AgentAttackType AttackType;
    public int OwnerId;
    public sbyte BoneIndex;
    public int WeaponClass;
    public Agent.KillInfo OverrideKillInfo;
    public Vec3 BlowPosition;
    public WeaponFlags WeaponRecordWeaponFlags;
    public int WeaponItemKind;
    public int InflictedDamage;
    [MarshalAs(UnmanagedType.I1)]
    public bool IsMissile;
    [MarshalAs(UnmanagedType.I1)]
    public bool IsValid;

    public KillingBlow(
      Blow b,
      Vec3 ragdollImpulsePoint,
      Vec3 ragdollImpulseAmount,
      int deathAction,
      int weaponItemKind,
      Agent.KillInfo overrideKillInfo = Agent.KillInfo.Invalid)
    {
      this.RagdollImpulseLocalPoint = ragdollImpulsePoint;
      this.RagdollImpulseAmount = ragdollImpulseAmount;
      this.DeathAction = deathAction;
      this.OverrideKillInfo = overrideKillInfo;
      this.DamageType = b.DamageType;
      this.AttackType = b.AttackType;
      this.OwnerId = b.OwnerId;
      this.BoneIndex = b.BoneIndex;
      this.WeaponClass = (int) b.WeaponRecord.WeaponClass;
      this.BlowPosition = b.Position;
      this.WeaponRecordWeaponFlags = b.WeaponRecord.WeaponFlags;
      this.WeaponItemKind = weaponItemKind;
      this.InflictedDamage = b.InflictedDamage;
      this.IsMissile = b.IsMissile;
      this.IsValid = true;
    }
  }
}
