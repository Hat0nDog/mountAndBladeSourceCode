// Decompiled with JetBrains decompiler
// Type: TaleWorlds.MountAndBlade.AttackCollisionData
// Assembly: TaleWorlds.MountAndBlade, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3C0E3F42-756F-4407-B817-06BA412A78B0
// Assembly location: E:\SteamLibrary\steamapps\common\Mount & Blade II Bannerlord\bin\Win64_Shipping_Client\TaleWorlds.MountAndBlade.dll

using System.Runtime.InteropServices;
using TaleWorlds.DotNet;
using TaleWorlds.Engine;
using TaleWorlds.Library;

namespace TaleWorlds.MountAndBlade
{
  [EngineStruct("Attack_collision_data")]
  public struct AttackCollisionData
  {
    [MarshalAs(UnmanagedType.I1)]
    private bool _attackBlockedWithShield;
    [MarshalAs(UnmanagedType.I1)]
    private bool _correctSideShieldBlock;
    [MarshalAs(UnmanagedType.I1)]
    private bool _isAlternativeAttack;
    [MarshalAs(UnmanagedType.I1)]
    private bool _isColliderAgent;
    [MarshalAs(UnmanagedType.I1)]
    private bool _collidedWithShieldOnBack;
    [MarshalAs(UnmanagedType.I1)]
    private bool _isMissile;
    [MarshalAs(UnmanagedType.I1)]
    private bool _missileBlockedWithWeapon;
    [MarshalAs(UnmanagedType.I1)]
    private bool _missileHasPhysics;
    [MarshalAs(UnmanagedType.I1)]
    private bool _entityExists;
    [MarshalAs(UnmanagedType.I1)]
    private bool _thrustTipHit;
    [MarshalAs(UnmanagedType.I1)]
    private bool _missileGoneUnderWater;
    private int _collisionResult;
    private Vec3 _weaponBlowDir;
    public float BaseMagnitude;
    public float MovementSpeedDamageModifier;
    public int AbsorbedByArmor;
    public int InflictedDamage;
    public int SelfInflictedDamage;
    [MarshalAs(UnmanagedType.I1)]
    public bool IsShieldBroken;

    public bool AttackBlockedWithShield => this._attackBlockedWithShield;

    public bool CorrectSideShieldBlock => this._correctSideShieldBlock;

    public bool IsAlternativeAttack => this._isAlternativeAttack;

    public bool IsColliderAgent => this._isColliderAgent;

    public bool CollidedWithShieldOnBack => this._collidedWithShieldOnBack;

    public bool IsMissile => this._isMissile;

    public bool MissileBlockedWithWeapon => this._missileBlockedWithWeapon;

    public bool MissileHasPhysics => this._missileHasPhysics;

    public bool EntityExists => this._entityExists;

    public bool ThrustTipHit => this._thrustTipHit;

    public bool MissileGoneUnderWater => this._missileGoneUnderWater;

    public bool IsHorseCharge => (double) this.ChargeVelocity > 0.0;

    public bool IsFallDamage => (double) this.FallSpeed > 0.0;

    public CombatCollisionResult CollisionResult => (CombatCollisionResult) this._collisionResult;

    public int AffectorWeaponSlotOrMissileIndex { get; }

    public int StrikeType { get; }

    public int DamageType { get; }

    public sbyte CollisionBoneIndex { get; private set; }

    public BoneBodyPartType VictimHitBodyPart { get; }

    public sbyte AttackBoneIndex { get; }

    public Agent.UsageDirection AttackDirection { get; }

    public int PhysicsMaterialIndex { get; private set; }

    public CombatHitResultFlags CollisionHitResultFlags { get; private set; }

    public float AttackProgress { get; }

    public float CollisionDistanceOnWeapon { get; }

    public float AttackerStunPeriod { get; set; }

    public float DefenderStunPeriod { get; set; }

    public float MissileTotalDamage { get; }

    public float MissileStartingBaseSpeed { get; }

    public float ChargeVelocity { get; }

    public float FallSpeed { get; private set; }

    public Vec3 WeaponRotUp { get; }

    public Vec3 WeaponBlowDir => this._weaponBlowDir;

    public Vec3 CollisionGlobalPosition { get; }

    public Vec3 MissileVelocity { get; }

    public Vec3 MissileStartingPosition { get; }

    public Vec3 VictimAgentCurVelocity { get; }

    public Vec3 CollisionGlobalNormal { get; }

    public static void UpdateDataForShieldPenetration(ref AttackCollisionData oldData)
    {
      oldData._attackBlockedWithShield = false;
      oldData._isColliderAgent = true;
      oldData.PhysicsMaterialIndex = PhysicsMaterial.GetFromName("flesh").Index;
    }

    public void SetCollisionBoneIndexForAreaDamage(sbyte boneIndex) => this.CollisionBoneIndex = boneIndex;

    private AttackCollisionData(
      bool attackBlockedWithShield,
      bool correctSideShieldBlock,
      bool isAlternativeAttack,
      bool isColliderAgent,
      bool collidedWithShieldOnBack,
      bool isMissile,
      bool missileBlockedWithWeapon,
      bool missileHasPhysics,
      bool entityExists,
      bool thrustTipHit,
      bool missileGoneUnderWater,
      CombatCollisionResult collisionResult,
      int affectorWeaponSlotOrMissileIndex,
      int StrikeType,
      int DamageType,
      sbyte CollisionBoneIndex,
      BoneBodyPartType VictimHitBodyPart,
      sbyte AttackBoneIndex,
      Agent.UsageDirection AttackDirection,
      int PhysicsMaterialIndex,
      CombatHitResultFlags CollisionHitResultFlags,
      float AttackProgress,
      float CollisionDistanceOnWeapon,
      float AttackerStunPeriod,
      float DefenderStunPeriod,
      float MissileTotalDamage,
      float MissileStartingBaseSpeed,
      float ChargeVelocity,
      float FallSpeed,
      Vec3 WeaponRotUp,
      Vec3 weaponBlowDir,
      Vec3 CollisionGlobalPosition,
      Vec3 MissileVelocity,
      Vec3 MissileStartingPosition,
      Vec3 VictimAgentCurVelocity,
      Vec3 GroundNormal)
    {
      this._attackBlockedWithShield = attackBlockedWithShield;
      this._correctSideShieldBlock = correctSideShieldBlock;
      this._isAlternativeAttack = isAlternativeAttack;
      this._isColliderAgent = isColliderAgent;
      this._collidedWithShieldOnBack = collidedWithShieldOnBack;
      this._isMissile = isMissile;
      this._missileBlockedWithWeapon = missileBlockedWithWeapon;
      this._missileHasPhysics = missileHasPhysics;
      this._entityExists = entityExists;
      this._thrustTipHit = thrustTipHit;
      this._missileGoneUnderWater = missileGoneUnderWater;
      this._collisionResult = (int) collisionResult;
      this.AffectorWeaponSlotOrMissileIndex = affectorWeaponSlotOrMissileIndex;
      this.StrikeType = StrikeType;
      this.DamageType = DamageType;
      this.CollisionBoneIndex = CollisionBoneIndex;
      this.VictimHitBodyPart = VictimHitBodyPart;
      this.AttackBoneIndex = AttackBoneIndex;
      this.AttackDirection = AttackDirection;
      this.PhysicsMaterialIndex = PhysicsMaterialIndex;
      this.CollisionHitResultFlags = CollisionHitResultFlags;
      this.AttackProgress = AttackProgress;
      this.CollisionDistanceOnWeapon = CollisionDistanceOnWeapon;
      this.AttackerStunPeriod = AttackerStunPeriod;
      this.DefenderStunPeriod = DefenderStunPeriod;
      this.MissileTotalDamage = MissileTotalDamage;
      this.MissileStartingBaseSpeed = MissileStartingBaseSpeed;
      this.ChargeVelocity = ChargeVelocity;
      this.FallSpeed = FallSpeed;
      this.WeaponRotUp = WeaponRotUp;
      this._weaponBlowDir = weaponBlowDir;
      this.CollisionGlobalPosition = CollisionGlobalPosition;
      this.MissileVelocity = MissileVelocity;
      this.MissileStartingPosition = MissileStartingPosition;
      this.VictimAgentCurVelocity = VictimAgentCurVelocity;
      this.CollisionGlobalNormal = GroundNormal;
      this.BaseMagnitude = 0.0f;
      this.MovementSpeedDamageModifier = 0.0f;
      this.AbsorbedByArmor = 0;
      this.InflictedDamage = 0;
      this.SelfInflictedDamage = 0;
      this.IsShieldBroken = false;
    }

    public static AttackCollisionData GetAttackCollisionDataForDebugPurpose(
      bool _attackBlockedWithShield,
      bool _correctSideShieldBlock,
      bool _isAlternativeAttack,
      bool _isColliderAgent,
      bool _collidedWithShieldOnBack,
      bool _isMissile,
      bool _isMissileBlockedWithWeapon,
      bool _missileHasPhysics,
      bool _entityExists,
      bool _thrustTipHit,
      bool _missileGoneUnderWater,
      CombatCollisionResult collisionResult,
      int affectorWeaponSlotOrMissileIndex,
      int StrikeType,
      int DamageType,
      sbyte CollisionBoneIndex,
      BoneBodyPartType VictimHitBodyPart,
      sbyte AttackBoneIndex,
      Agent.UsageDirection AttackDirection,
      int PhysicsMaterialIndex,
      CombatHitResultFlags CollisionHitResultFlags,
      float AttackProgress,
      float CollisionDistanceOnWeapon,
      float AttackerStunPeriod,
      float DefenderStunPeriod,
      float MissileTotalDamage,
      float MissileInitialSpeed,
      float ChargeVelocity,
      float FallSpeed,
      Vec3 WeaponRotUp,
      Vec3 _weaponBlowDir,
      Vec3 CollisionGlobalPosition,
      Vec3 MissileVelocity,
      Vec3 MissileStartingPosition,
      Vec3 VictimAgentCurVelocity,
      Vec3 GroundNormal)
    {
      return new AttackCollisionData(_attackBlockedWithShield, _correctSideShieldBlock, _isAlternativeAttack, _isColliderAgent, _collidedWithShieldOnBack, _isMissile, _isMissileBlockedWithWeapon, _missileHasPhysics, _entityExists, _thrustTipHit, _missileGoneUnderWater, collisionResult, affectorWeaponSlotOrMissileIndex, StrikeType, DamageType, CollisionBoneIndex, VictimHitBodyPart, AttackBoneIndex, AttackDirection, PhysicsMaterialIndex, CollisionHitResultFlags, AttackProgress, CollisionDistanceOnWeapon, AttackerStunPeriod, DefenderStunPeriod, MissileTotalDamage, MissileInitialSpeed, ChargeVelocity, FallSpeed, WeaponRotUp, _weaponBlowDir, CollisionGlobalPosition, MissileVelocity, MissileStartingPosition, VictimAgentCurVelocity, GroundNormal);
    }
  }
}
