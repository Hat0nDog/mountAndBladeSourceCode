// Decompiled with JetBrains decompiler
// Type: TaleWorlds.MountAndBlade.BlowWeaponRecord
// Assembly: TaleWorlds.MountAndBlade, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3C0E3F42-756F-4407-B817-06BA412A78B0
// Assembly location: E:\SteamLibrary\steamapps\common\Mount & Blade II Bannerlord\bin\Win64_Shipping_Client\TaleWorlds.MountAndBlade.dll

using System.Runtime.InteropServices;
using TaleWorlds.Core;
using TaleWorlds.DotNet;
using TaleWorlds.Library;

namespace TaleWorlds.MountAndBlade
{
  [EngineStruct("Blow_weapon_record")]
  public struct BlowWeaponRecord
  {
    public Vec3 StartingPosition;
    public Vec3 CurrentPosition;
    public Vec3 Velocity;
    public ItemFlags ItemFlags;
    public WeaponFlags WeaponFlags;
    public WeaponClass WeaponClass;
    public sbyte BoneNoToAttach;
    public int AffectorWeaponSlotOrMissileIndex;
    public float Weight;
    [MarshalAs(UnmanagedType.I1)]
    private bool _isMissile;
    [MarshalAs(UnmanagedType.I1)]
    private bool _isMaterialMetal;

    public void FillAsMeleeBlow(
      ItemObject item,
      WeaponComponentData weaponComponentData,
      int affectorWeaponSlot,
      sbyte weaponAttachBoneIndex)
    {
      this._isMissile = false;
      if (weaponComponentData != null)
      {
        this.ItemFlags = item.ItemFlags;
        this.WeaponFlags = weaponComponentData.WeaponFlags;
        this.WeaponClass = weaponComponentData.WeaponClass;
        this.BoneNoToAttach = weaponAttachBoneIndex;
        this.AffectorWeaponSlotOrMissileIndex = affectorWeaponSlot;
        this.Weight = item.Weight;
        this._isMaterialMetal = weaponComponentData.PhysicsMaterial.Contains("metal");
      }
      else
      {
        this._isMaterialMetal = false;
        this.AffectorWeaponSlotOrMissileIndex = -1;
      }
    }

    public void FillAsMissileBlow(
      ItemObject item,
      WeaponComponentData weaponComponentData,
      int missileIndex,
      sbyte weaponAttachBoneIndex,
      Vec3 startingPosition,
      Vec3 currentPosition,
      Vec3 velocity)
    {
      this._isMissile = true;
      this.StartingPosition = startingPosition;
      this.CurrentPosition = currentPosition;
      this.Velocity = velocity;
      this.ItemFlags = item.ItemFlags;
      this.WeaponFlags = weaponComponentData.WeaponFlags;
      this.WeaponClass = weaponComponentData.WeaponClass;
      this.BoneNoToAttach = weaponAttachBoneIndex;
      this.AffectorWeaponSlotOrMissileIndex = missileIndex;
      this.Weight = item.Weight;
      this._isMaterialMetal = weaponComponentData.PhysicsMaterial.Contains("metal");
    }

    public bool HasWeapon() => this.AffectorWeaponSlotOrMissileIndex >= 0;

    public bool IsMissile => this._isMissile;

    public bool IsShield => !this.WeaponFlags.HasAnyFlag<WeaponFlags>(WeaponFlags.WeaponMask) && this.WeaponFlags.HasAllFlags<WeaponFlags>(WeaponFlags.HasHitPoints | WeaponFlags.CanBlockRanged);

    public bool IsRanged => this.WeaponFlags.HasAnyFlag<WeaponFlags>(WeaponFlags.RangedWeapon);

    public bool IsAmmo => !this.WeaponFlags.HasAnyFlag<WeaponFlags>(WeaponFlags.WeaponMask) && this.WeaponFlags.HasAnyFlag<WeaponFlags>(WeaponFlags.Consumable);

    public int GetHitSound(
      bool isOwnerHumanoid,
      bool isCriticalBlow,
      bool isLowBlow,
      bool isNonTipThrust,
      AgentAttackType attackType,
      DamageTypes damageType)
    {
      int num;
      if (this.HasWeapon())
      {
        if (this.IsRanged || this.IsAmmo)
        {
          switch (this.WeaponClass)
          {
            case WeaponClass.Stone:
              num = !isCriticalBlow ? (!isLowBlow ? CombatSoundContainer.SoundCodeMissionCombatThrowingStoneMed : CombatSoundContainer.SoundCodeMissionCombatThrowingStoneLow) : CombatSoundContainer.SoundCodeMissionCombatThrowingStoneHigh;
              break;
            case WeaponClass.Boulder:
              num = !isCriticalBlow ? (!isLowBlow ? CombatSoundContainer.SoundCodeMissionCombatBoulderMed : CombatSoundContainer.SoundCodeMissionCombatBoulderLow) : CombatSoundContainer.SoundCodeMissionCombatBoulderHigh;
              break;
            case WeaponClass.ThrowingAxe:
              num = !isCriticalBlow ? (!isLowBlow ? CombatSoundContainer.SoundCodeMissionCombatThrowingAxeMed : CombatSoundContainer.SoundCodeMissionCombatThrowingAxeLow) : CombatSoundContainer.SoundCodeMissionCombatThrowingAxeHigh;
              break;
            case WeaponClass.ThrowingKnife:
              num = !isCriticalBlow ? (!isLowBlow ? CombatSoundContainer.SoundCodeMissionCombatThrowingDaggerMed : CombatSoundContainer.SoundCodeMissionCombatThrowingDaggerLow) : CombatSoundContainer.SoundCodeMissionCombatThrowingDaggerHigh;
              break;
            case WeaponClass.Javelin:
              num = !isCriticalBlow ? (!isLowBlow ? CombatSoundContainer.SoundCodeMissionCombatMissileMed : CombatSoundContainer.SoundCodeMissionCombatMissileLow) : CombatSoundContainer.SoundCodeMissionCombatMissileHigh;
              break;
            default:
              num = !isCriticalBlow ? (!isLowBlow ? CombatSoundContainer.SoundCodeMissionCombatMissileMed : CombatSoundContainer.SoundCodeMissionCombatMissileLow) : CombatSoundContainer.SoundCodeMissionCombatMissileHigh;
              break;
          }
        }
        else if (this.IsShield)
          num = !this._isMaterialMetal ? CombatSoundContainer.SoundCodeMissionCombatWoodShieldBash : CombatSoundContainer.SoundCodeMissionCombatMetalShieldBash;
        else if (attackType == AgentAttackType.Bash)
        {
          num = CombatSoundContainer.SoundCodeMissionCombatBluntLow;
        }
        else
        {
          if (isNonTipThrust)
            damageType = DamageTypes.Blunt;
          switch (damageType)
          {
            case DamageTypes.Cut:
              num = !isCriticalBlow ? (!isLowBlow ? CombatSoundContainer.SoundCodeMissionCombatCutMed : CombatSoundContainer.SoundCodeMissionCombatCutLow) : CombatSoundContainer.SoundCodeMissionCombatCutHigh;
              break;
            case DamageTypes.Pierce:
              num = !isCriticalBlow ? (!isLowBlow ? CombatSoundContainer.SoundCodeMissionCombatPierceMed : CombatSoundContainer.SoundCodeMissionCombatPierceLow) : CombatSoundContainer.SoundCodeMissionCombatPierceHigh;
              break;
            case DamageTypes.Blunt:
              num = !isCriticalBlow ? (!isLowBlow ? CombatSoundContainer.SoundCodeMissionCombatBluntMed : CombatSoundContainer.SoundCodeMissionCombatBluntLow) : CombatSoundContainer.SoundCodeMissionCombatBluntHigh;
              break;
            default:
              num = CombatSoundContainer.SoundCodeMissionCombatBluntMed;
              break;
          }
        }
      }
      else
        num = isOwnerHumanoid ? (attackType != AgentAttackType.Kick ? (!isCriticalBlow ? (!isLowBlow ? CombatSoundContainer.SoundCodeMissionCombatPunchMed : CombatSoundContainer.SoundCodeMissionCombatPunchLow) : CombatSoundContainer.SoundCodeMissionCombatPunchHigh) : CombatSoundContainer.SoundCodeMissionCombatKick) : CombatSoundContainer.SoundCodeMissionCombatChargeDamage;
      return num;
    }
  }
}
