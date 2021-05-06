// Decompiled with JetBrains decompiler
// Type: TaleWorlds.MountAndBlade.Mangonel
// Assembly: TaleWorlds.MountAndBlade, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3C0E3F42-756F-4407-B817-06BA412A78B0
// Assembly location: E:\SteamLibrary\steamapps\common\Mount & Blade II Bannerlord\bin\Win64_Shipping_Client\TaleWorlds.MountAndBlade.dll

using System;
using System.Collections.Generic;
using System.Linq;
using TaleWorlds.Core;
using TaleWorlds.DotNet;
using TaleWorlds.Engine;
using TaleWorlds.Library;
using TaleWorlds.Localization;
using TaleWorlds.MountAndBlade.Objects.Siege;

namespace TaleWorlds.MountAndBlade
{
  public class Mangonel : RangedSiegeWeapon, ISpawnable
  {
    private static readonly ActionIndexCache act_usage_mangonel_idle = ActionIndexCache.Create(nameof (act_usage_mangonel_idle));
    private static readonly ActionIndexCache act_usage_mangonel_load_ammo_begin = ActionIndexCache.Create(nameof (act_usage_mangonel_load_ammo_begin));
    private static readonly ActionIndexCache act_usage_mangonel_load_ammo_end = ActionIndexCache.Create(nameof (act_usage_mangonel_load_ammo_end));
    private static readonly ActionIndexCache act_pickup_boulder_begin = ActionIndexCache.Create(nameof (act_pickup_boulder_begin));
    private static readonly ActionIndexCache act_pickup_boulder_end = ActionIndexCache.Create(nameof (act_pickup_boulder_end));
    private static readonly ActionIndexCache act_usage_mangonel_reload = ActionIndexCache.Create(nameof (act_usage_mangonel_reload));
    private static readonly ActionIndexCache act_usage_mangonel_reload_2 = ActionIndexCache.Create(nameof (act_usage_mangonel_reload_2));
    private static readonly ActionIndexCache act_usage_mangonel_reload_2_idle = ActionIndexCache.Create(nameof (act_usage_mangonel_reload_2_idle));
    private static readonly ActionIndexCache act_usage_mangonel_rotate_left = ActionIndexCache.Create(nameof (act_usage_mangonel_rotate_left));
    private static readonly ActionIndexCache act_usage_mangonel_rotate_right = ActionIndexCache.Create(nameof (act_usage_mangonel_rotate_right));
    private static readonly ActionIndexCache act_usage_mangonel_shoot = ActionIndexCache.Create(nameof (act_usage_mangonel_shoot));
    private static readonly ActionIndexCache act_usage_mangonel_big_idle = ActionIndexCache.Create(nameof (act_usage_mangonel_big_idle));
    private static readonly ActionIndexCache act_usage_mangonel_big_shoot = ActionIndexCache.Create(nameof (act_usage_mangonel_big_shoot));
    private static readonly ActionIndexCache act_usage_mangonel_big_reload = ActionIndexCache.Create(nameof (act_usage_mangonel_big_reload));
    private static readonly ActionIndexCache act_usage_mangonel_big_load_ammo_begin = ActionIndexCache.Create(nameof (act_usage_mangonel_big_load_ammo_begin));
    private static readonly ActionIndexCache act_usage_mangonel_big_load_ammo_end = ActionIndexCache.Create(nameof (act_usage_mangonel_big_load_ammo_end));
    private static readonly ActionIndexCache act_strike_bent_over = ActionIndexCache.Create(nameof (act_strike_bent_over));
    private string MissileBoneName = "end_throwarm";
    private const string BodyTag = "body";
    private const string RopeTag = "rope";
    private const string RotateTag = "rotate";
    private const string LeftTag = "left";
    private const string VerticalAdjusterTag = "vertical_adjuster";
    private List<StandingPoint> _rotateStandingPoints;
    private SynchedMissionObject _body;
    private SynchedMissionObject _rope;
    private GameEntity _verticalAdjuster;
    private MatrixFrame _verticalAdjusterStartingLocalFrame;
    private float _timeElapsedAfterLoading;
    private MatrixFrame[] _standingPointLocalIKFrames;
    private StandingPoint _reloadWithoutPilot;
    public string MangonelBodySkeleton = "mangonel_skeleton";
    public string MangonelBodyFire = "mangonel_fire";
    public string MangonelBodyReload = "mangonel_set_up";
    public string MangonelRopeFire = "mangonel_holder_fire";
    public string MangonelRopeReload = "mangonel_holder_set_up";
    public string MangonelAimAnimation = "mangonel_a_anglearm_state";
    public string ProjectileBoneName = "end_throwarm";
    public string IdleActionName;
    public string ShootActionName;
    public string Reload1ActionName;
    public string Reload2ActionName;
    public string RotateLeftActionName;
    public string RotateRightActionName;
    public string LoadAmmoBeginActionName;
    public string LoadAmmoEndActionName;
    public string Reload2IdleActionName;
    public float ProjectileSpeed = 40f;
    private ActionIndexCache _idleAnimationActionIndex;
    private ActionIndexCache _shootAnimationActionIndex;
    private ActionIndexCache _reload1AnimationActionIndex;
    private ActionIndexCache _reload2AnimationActionIndex;
    private ActionIndexCache _rotateLeftAnimationActionIndex;
    private ActionIndexCache _rotateRightAnimationActionIndex;
    private ActionIndexCache _loadAmmoBeginAnimationActionIndex;
    private ActionIndexCache _loadAmmoEndAnimationActionIndex;
    private ActionIndexCache _reload2IdleActionIndex;
    private byte _missileBoneIndex;

    protected override void RegisterAnimationParameters()
    {
      this.SkeletonOwnerObjects = new SynchedMissionObject[2];
      this.skeletonNames = new string[1];
      this.fireAnimations = new string[2];
      this.setUpAnimations = new string[2];
      this.SkeletonOwnerObjects[0] = this._body;
      this.skeletonNames[0] = this.MangonelBodySkeleton;
      this.fireAnimations[0] = this.MangonelBodyFire;
      this.setUpAnimations[0] = this.MangonelBodyReload;
      this.SkeletonOwnerObjects[1] = this._rope;
      this.fireAnimations[1] = this.MangonelRopeFire;
      this.setUpAnimations[1] = this.MangonelRopeReload;
      this.MissileBoneName = this.ProjectileBoneName;
      this._idleAnimationActionIndex = ActionIndexCache.Create(this.IdleActionName);
      this._shootAnimationActionIndex = ActionIndexCache.Create(this.ShootActionName);
      this._reload1AnimationActionIndex = ActionIndexCache.Create(this.Reload1ActionName);
      this._reload2AnimationActionIndex = ActionIndexCache.Create(this.Reload2ActionName);
      this._rotateLeftAnimationActionIndex = ActionIndexCache.Create(this.RotateLeftActionName);
      this._rotateRightAnimationActionIndex = ActionIndexCache.Create(this.RotateRightActionName);
      this._loadAmmoBeginAnimationActionIndex = ActionIndexCache.Create(this.LoadAmmoBeginActionName);
      this._loadAmmoEndAnimationActionIndex = ActionIndexCache.Create(this.LoadAmmoEndActionName);
      this._reload2IdleActionIndex = ActionIndexCache.Create(this.Reload2IdleActionName);
    }

    public override UsableMachineAIBase CreateAIBehaviourObject() => (UsableMachineAIBase) new MangonelAI(this);

    public override void AfterMissionStart()
    {
      if (this.AmmoPickUpStandingPoints != null)
      {
        foreach (UsableMissionObject pickUpStandingPoint in this.AmmoPickUpStandingPoints)
          pickUpStandingPoint.LockUserFrames = true;
      }
      this.UpdateProjectilePosition();
    }

    public override SiegeEngineType GetSiegeEngineType() => this._defaultSide != BattleSideEnum.Attacker ? DefaultSiegeEngineTypes.Catapult : DefaultSiegeEngineTypes.Onager;

    protected internal override void OnInit()
    {
      List<SynchedMissionObject> synchedMissionObjectList = this.GameEntity.CollectObjectsWithTag<SynchedMissionObject>("rope");
      if (synchedMissionObjectList.Count > 0)
        this._rope = synchedMissionObjectList[0];
      this._body = this.GameEntity.CollectObjectsWithTag<SynchedMissionObject>("body")[0];
      this.RotationObject = this._body;
      this._verticalAdjuster = this.GameEntity.CollectChildrenEntitiesWithTag("vertical_adjuster")[0];
      if ((NativeObject) this._verticalAdjuster.Skeleton != (NativeObject) null)
        this._verticalAdjuster.Skeleton.SetAnimationAtChannel(this.MangonelAimAnimation, 0);
      this._verticalAdjusterStartingLocalFrame = this._verticalAdjuster.GetFrame();
      this._verticalAdjusterStartingLocalFrame = this._body.GameEntity.GetBoneEntitialFrameWithIndex((byte) 0).TransformToLocal(this._verticalAdjusterStartingLocalFrame);
      base.OnInit();
      this.timeGapBetweenShootActionAndProjectileLeaving = 0.23f;
      this.timeGapBetweenShootingEndAndReloadingStart = 0.0f;
      this._rotateStandingPoints = new List<StandingPoint>();
      if (this.StandingPoints != null)
      {
        foreach (StandingPoint standingPoint in this.StandingPoints)
        {
          if (standingPoint.GameEntity.HasTag("rotate"))
          {
            if (standingPoint.GameEntity.HasTag("left") && this._rotateStandingPoints.Count > 0)
              this._rotateStandingPoints.Insert(0, standingPoint);
            else
              this._rotateStandingPoints.Add(standingPoint);
          }
        }
        MatrixFrame globalFrame = this._body.GameEntity.GetGlobalFrame();
        this._standingPointLocalIKFrames = new MatrixFrame[this.StandingPoints.Count];
        for (int index = 0; index < this.StandingPoints.Count; ++index)
        {
          this._standingPointLocalIKFrames[index] = this.StandingPoints[index].GameEntity.GetGlobalFrame().TransformToLocal(globalFrame);
          this.StandingPoints[index].AddComponent((UsableMissionObjectComponent) new ClearHandInverseKinematicsOnStopUsageComponent());
        }
      }
      this._missileBoneIndex = (byte) Skeleton.GetBoneIndexFromName(this.SkeletonOwnerObjects[0].GameEntity.Skeleton.GetName(), this.MissileBoneName);
      this.ApplyAimChange();
      foreach (StandingPoint reloadStandingPoint in this.ReloadStandingPoints)
      {
        if (reloadStandingPoint != this.PilotStandingPoint)
          this._reloadWithoutPilot = reloadStandingPoint;
      }
      this.EnemyRangeToStopUsing = 7f;
      this.SetScriptComponentToTick(this.GetTickRequirement());
    }

    protected internal override void OnEditorInit()
    {
    }

    protected override bool CanRotate() => this.State == RangedSiegeWeapon.WeaponState.Idle || this.State == RangedSiegeWeapon.WeaponState.LoadingAmmo || this.State == RangedSiegeWeapon.WeaponState.WaitingBeforeIdle;

    protected override ScriptComponentBehaviour.TickRequirement GetTickRequirement() => this.GameEntity.IsVisibleIncludeParents() ? ScriptComponentBehaviour.TickRequirement.Tick : base.GetTickRequirement();

    protected internal override void OnTick(float dt)
    {
      base.OnTick(dt);
      if (!this.GameEntity.IsVisibleIncludeParents())
        return;
      if (this.State == RangedSiegeWeapon.WeaponState.WaitingBeforeProjectileLeaving)
        this.UpdateProjectilePosition();
      if ((NativeObject) this._verticalAdjuster.Skeleton != (NativeObject) null)
        this._verticalAdjuster.Skeleton.SetAnimationParameterAtChannel(0, (float) (((double) this.currentReleaseAngle - (double) this.BottomReleaseAngleRestriction) / ((double) this.TopReleaseAngleRestriction - (double) this.BottomReleaseAngleRestriction)));
      MatrixFrame parent = this.SkeletonOwnerObjects[0].GameEntity.GetBoneEntitialFrameWithIndex((byte) 0).TransformToParent(this._verticalAdjusterStartingLocalFrame);
      this._verticalAdjuster.SetFrame(ref parent);
      MatrixFrame globalFrame = this._body.GameEntity.GetGlobalFrame();
      for (int index = 0; index < this.StandingPoints.Count; ++index)
      {
        if (this.StandingPoints[index].HasUser)
        {
          if (MBMath.IsBetween((int) this.StandingPoints[index].UserAgent.GetCurrentActionType(0), 44, 48))
            this.StandingPoints[index].UserAgent.ClearHandInverseKinematics();
          else if (this.StandingPoints[index] != this.PilotStandingPoint)
          {
            if (this.StandingPoints[index].UserAgent.GetCurrentAction(1) != this._reload2IdleActionIndex)
              this.StandingPoints[index].UserAgent.SetHandInverseKinematicsFrameFromActionChannel(1, this._standingPointLocalIKFrames[index], globalFrame);
            else
              this.StandingPoints[index].UserAgent.ClearHandInverseKinematics();
          }
          else
            this.StandingPoints[index].UserAgent.SetHandInverseKinematicsFrameFromActionChannel(1, this._standingPointLocalIKFrames[index], globalFrame);
        }
      }
      if (!GameNetwork.IsClientOrReplay)
      {
        for (int index = 0; index < this._rotateStandingPoints.Count; ++index)
        {
          StandingPoint rotateStandingPoint = this._rotateStandingPoints[index];
          if (rotateStandingPoint.HasUser && !rotateStandingPoint.UserAgent.SetActionChannel(1, index == 0 ? this._rotateLeftAnimationActionIndex : this._rotateRightAnimationActionIndex))
            rotateStandingPoint.UserAgent.StopUsingGameObject();
        }
        if (this.PilotAgent != null)
        {
          ActionIndexCache currentAction = this.PilotAgent.GetCurrentAction(1);
          if (this.State == RangedSiegeWeapon.WeaponState.WaitingBeforeProjectileLeaving)
          {
            if (MBMath.IsBetween((int) this.PilotAgent.GetCurrentActionType(0), 44, 48))
            {
              if (this.PilotAgent.GetCurrentAction(0) != Mangonel.act_strike_bent_over)
                this.PilotAgent.SetActionChannel(0, Mangonel.act_strike_bent_over);
            }
            else if (!this.PilotAgent.SetActionChannel(1, this._shootAnimationActionIndex))
              this.PilotAgent.StopUsingGameObject();
          }
          else if (!this.PilotAgent.SetActionChannel(1, this._idleAnimationActionIndex) && currentAction != this._reload1AnimationActionIndex && currentAction != this._shootAnimationActionIndex)
            this.PilotAgent.StopUsingGameObject();
        }
        if (this._reloadWithoutPilot.HasUser)
        {
          Agent userAgent = this._reloadWithoutPilot.UserAgent;
          if (!userAgent.SetActionChannel(1, this._reload2IdleActionIndex) && userAgent.GetCurrentAction(1) != this._reload2AnimationActionIndex)
            userAgent.StopUsingGameObject();
        }
        foreach (StandingPointWithWeaponRequirement pickUpStandingPoint in this.AmmoPickUpStandingPoints)
        {
          if (pickUpStandingPoint.HasUser)
          {
            Agent userAgent = pickUpStandingPoint.UserAgent;
            ActionIndexCache currentAction = userAgent.GetCurrentAction(1);
            if (!(currentAction == Mangonel.act_pickup_boulder_begin))
            {
              if (currentAction == Mangonel.act_pickup_boulder_end)
              {
                MissionWeapon weapon = new MissionWeapon(this.OriginalMissileItem, (ItemModifier) null, (Banner) null, (short) 1);
                userAgent.EquipWeaponToExtraSlotAndWield(ref weapon);
                userAgent.StopUsingGameObject();
                this.ConsumeAmmo();
                if (!this.IsDeactivated && !this.LoadAmmoStandingPoint.HasUser && (!this.LoadAmmoStandingPoint.HasAIMovingTo && this.HasAIPickingUpAmmo) && this.CurrentlyUsedAmmoPickUpPoint == pickUpStandingPoint)
                {
                  this.LoadAmmoStandingPoint.SetIsDeactivatedSynched(false);
                  if (userAgent.IsAIControlled)
                  {
                    StandingPoint standingPointFor = this.GetSuitableStandingPointFor(this.Side, userAgent);
                    if (standingPointFor != null)
                    {
                      ((IDetachment) this).AddAgent(userAgent);
                      if (userAgent.Formation != null)
                      {
                        userAgent.Formation.DetachUnit(userAgent, this.IsLoose);
                        userAgent.Detachment = (IDetachment) this;
                        userAgent.DetachmentWeight = this.GetWeightOfStandingPoint(standingPointFor);
                      }
                    }
                  }
                }
              }
              else if (!userAgent.SetActionChannel(1, Mangonel.act_pickup_boulder_begin))
                userAgent.StopUsingGameObject();
            }
          }
        }
      }
      switch (this.State)
      {
        case RangedSiegeWeapon.WeaponState.LoadingAmmo:
          if (GameNetwork.IsClientOrReplay || !this.LoadAmmoStandingPoint.HasUser)
            break;
          Agent userAgent1 = this.LoadAmmoStandingPoint.UserAgent;
          if (userAgent1.GetCurrentAction(1) == this._loadAmmoEndAnimationActionIndex)
          {
            EquipmentIndex wieldedItemIndex = userAgent1.GetWieldedItemIndex(Agent.HandIndex.MainHand);
            if (wieldedItemIndex != EquipmentIndex.None && userAgent1.Equipment[wieldedItemIndex].CurrentUsageItem.WeaponClass == this.OriginalMissileItem.PrimaryWeapon.WeaponClass)
            {
              this.ChangeProjectileEntityServer(userAgent1, userAgent1.Equipment[wieldedItemIndex].Item.StringId);
              userAgent1.RemoveEquippedWeapon(wieldedItemIndex);
              this._timeElapsedAfterLoading = 0.0f;
              this.Projectile.SetVisibleSynched(true);
              this.State = RangedSiegeWeapon.WeaponState.WaitingBeforeIdle;
              break;
            }
            this.LoadAmmoStandingPoint.UserAgent.StopUsingGameObject();
            break;
          }
          if (!(userAgent1.GetCurrentAction(1) != this._loadAmmoBeginAnimationActionIndex) || this.LoadAmmoStandingPoint.UserAgent.SetActionChannel(1, this._loadAmmoBeginAnimationActionIndex))
            break;
          for (EquipmentIndex equipmentIndex = EquipmentIndex.WeaponItemBeginSlot; equipmentIndex < EquipmentIndex.NumAllWeaponSlots; ++equipmentIndex)
          {
            if (!userAgent1.Equipment[equipmentIndex].IsEmpty && userAgent1.Equipment[equipmentIndex].CurrentUsageItem.WeaponClass == this.OriginalMissileItem.PrimaryWeapon.WeaponClass)
              userAgent1.RemoveEquippedWeapon(equipmentIndex);
          }
          this.LoadAmmoStandingPoint.UserAgent.StopUsingGameObject();
          break;
        case RangedSiegeWeapon.WeaponState.WaitingBeforeIdle:
          this._timeElapsedAfterLoading += dt;
          if ((double) this._timeElapsedAfterLoading <= 1.0)
            break;
          this.State = RangedSiegeWeapon.WeaponState.Idle;
          break;
        case RangedSiegeWeapon.WeaponState.Reloading:
          using (List<StandingPoint>.Enumerator enumerator = this.ReloadStandingPoints.GetEnumerator())
          {
            while (enumerator.MoveNext())
            {
              StandingPoint current = enumerator.Current;
              if (current.HasUser)
              {
                ActionIndexCache currentAction = current.UserAgent.GetCurrentAction(1);
                if (currentAction == this._reload1AnimationActionIndex || currentAction == this._reload2AnimationActionIndex)
                  current.UserAgent.SetCurrentActionProgress(1, this._body.GameEntity.Skeleton.GetAnimationParameterAtChannel(0));
                else if (!GameNetwork.IsClientOrReplay)
                {
                  ActionIndexCache actionIndexCache = current == this.PilotStandingPoint ? this._reload1AnimationActionIndex : this._reload2AnimationActionIndex;
                  if (!current.UserAgent.SetActionChannel(1, actionIndexCache, startProgress: this._body.GameEntity.Skeleton.GetAnimationParameterAtChannel(0)))
                    current.UserAgent.StopUsingGameObject();
                }
              }
            }
            break;
          }
      }
    }

    private void UpdateProjectilePosition()
    {
      MatrixFrame entitialFrameWithIndex = this.SkeletonOwnerObjects[0].GameEntity.GetBoneEntitialFrameWithIndex(this._missileBoneIndex);
      this.Projectile.GameEntity.SetFrame(ref entitialFrameWithIndex);
    }

    protected override void OnRangedSiegeWeaponStateChange()
    {
      base.OnRangedSiegeWeaponStateChange();
      switch (this.State)
      {
        case RangedSiegeWeapon.WeaponState.Idle:
          if (!GameNetwork.IsClientOrReplay)
          {
            this.Projectile.SetVisibleSynched(true);
            break;
          }
          this.Projectile.GameEntity.SetVisibilityExcludeParents(true);
          break;
        case RangedSiegeWeapon.WeaponState.Shooting:
          if (!GameNetwork.IsClientOrReplay)
          {
            this.Projectile.SetVisibleSynched(false);
            break;
          }
          this.Projectile.GameEntity.SetVisibilityExcludeParents(false);
          break;
        case RangedSiegeWeapon.WeaponState.WaitingBeforeIdle:
          this.UpdateProjectilePosition();
          break;
      }
    }

    protected override float MaximumBallisticError => 1.5f;

    protected override float ShootingSpeed => this.ProjectileSpeed;

    protected override void GetSoundEventIndices()
    {
      this.MoveSoundIndex = SoundEvent.GetEventIdFromString("event:/mission/siege/mangonel/move");
      this.ReloadSoundIndex = SoundEvent.GetEventIdFromString("event:/mission/siege/mangonel/reload");
    }

    protected override float HorizontalAimSensitivity => this._defaultSide == BattleSideEnum.Defender ? 0.25f : 0.05f + this._rotateStandingPoints.Where<StandingPoint>((Func<StandingPoint, bool>) (rotateStandingPoint => rotateStandingPoint.HasUser)).Sum<StandingPoint>((Func<StandingPoint, float>) (rotateStandingPoint => 0.1f));

    protected override float VerticalAimSensitivity => 0.1f;

    protected override Vec3 ShootingDirection
    {
      get
      {
        Mat3 rotation = this._body.GameEntity.GetGlobalFrame().rotation;
        rotation.RotateAboutSide(-this.currentReleaseAngle);
        return rotation.TransformToParent(new Vec3(y: -1f));
      }
    }

    protected override Vec3 VisualizationShootingDirection
    {
      get
      {
        Mat3 rotation = this._body.GameEntity.GetGlobalFrame().rotation;
        rotation.RotateAboutSide(-this.VisualizeReleaseTrajectoryAngle);
        return rotation.TransformToParent(new Vec3(y: -1f));
      }
    }

    protected override bool HasAmmo
    {
      get => base.HasAmmo || this.CurrentlyUsedAmmoPickUpPoint != null || this.LoadAmmoStandingPoint.HasUser || this.LoadAmmoStandingPoint.HasAIMovingTo;
      set => base.HasAmmo = value;
    }

    protected override void ApplyAimChange()
    {
      base.ApplyAimChange();
      double num = (double) this.ShootingDirection.Normalize();
    }

    public override string GetDescriptionText(GameEntity gameEntity = null) => gameEntity.HasTag(this.AmmoPickUpTag) ? new TextObject("{=pzfbPbWW}Boulder").ToString() : new TextObject("{=NbpcDXtJ}Mangonel").ToString();

    public override TextObject GetActionTextForStandingPoint(
      UsableMissionObject usableGameObject)
    {
      TextObject textObject = !usableGameObject.GameEntity.HasTag(this.ReloadTag) ? (!usableGameObject.GameEntity.HasTag("rotate") ? (!usableGameObject.GameEntity.HasTag(this.AmmoPickUpTag) ? (!usableGameObject.GameEntity.HasTag(this.AmmoLoadTag) ? new TextObject("{=fEQAPJ2e}({KEY}) Use") : new TextObject("{=ibC4xPoo}({KEY}) Load Ammo")) : new TextObject("{=bNYm3K6b}({KEY}) Pick Up")) : new TextObject("{=5wx4BF5h}({KEY}) Rotate")) : new TextObject(this.PilotStandingPoint == usableGameObject ? "{=fEQAPJ2e}({KEY}) Use" : "{=Na81xuXn}({KEY}) Rearm");
      textObject.SetTextVariable("KEY", Game.Current.GameTextManager.GetHotKeyGameText("CombatHotKeyCategory", 13));
      return textObject;
    }

    public override TargetFlags GetTargetFlags()
    {
      TargetFlags targetFlags = (TargetFlags) (0 | 2 | 8 | 16);
      if (this.IsDestroyed || this.IsDeactivated)
        targetFlags |= TargetFlags.NotAThreat;
      if (this.Side == BattleSideEnum.Attacker && DebugSiegeBehaviour.DebugDefendState == DebugSiegeBehaviour.DebugStateDefender.DebugDefendersToMangonels)
        targetFlags |= TargetFlags.DebugThreat;
      if (this.Side == BattleSideEnum.Defender && DebugSiegeBehaviour.DebugAttackState == DebugSiegeBehaviour.DebugStateAttacker.DebugAttackersToMangonels)
        targetFlags |= TargetFlags.DebugThreat;
      return targetFlags;
    }

    public override float GetTargetValue(List<Vec3> weaponPos) => 40f * this.GetUserMultiplierOfWeapon() * this.GetDistanceMultiplierOfWeapon(weaponPos[0]) * this.GetHitpointMultiplierofWeapon();

    public override float ProcessTargetValue(float baseValue, TargetFlags flags)
    {
      if (flags.HasAnyFlag<TargetFlags>(TargetFlags.NotAThreat))
        return -1000f;
      if (flags.HasAnyFlag<TargetFlags>(TargetFlags.IsSiegeEngine))
        baseValue *= 1.5f;
      if (flags.HasAnyFlag<TargetFlags>(TargetFlags.IsStructure))
        baseValue *= 2.5f;
      if (flags.HasAnyFlag<TargetFlags>(TargetFlags.IsSmall))
        baseValue *= 0.5f;
      if (flags.HasAnyFlag<TargetFlags>(TargetFlags.IsMoving))
        baseValue *= 0.8f;
      if (flags.HasAnyFlag<TargetFlags>(TargetFlags.DebugThreat))
        baseValue *= 10000f;
      return baseValue;
    }

    protected override float GetDetachmentWeightAux(BattleSideEnum side)
    {
      if (this.IsDisabledForBattleSideAI(side))
        return float.MinValue;
      this._usableStandingPoints.Clear();
      bool flag1 = false;
      bool flag2 = false;
      bool isDeactivated = this.LoadAmmoStandingPoint.IsDeactivated;
      bool flag3 = this.LoadAmmoStandingPoint.HasUser || this.LoadAmmoStandingPoint.HasAIMovingTo;
      bool flag4 = this.CurrentlyUsedAmmoPickUpPoint != null;
      bool flag5 = false;
      for (int index = 0; index < this.StandingPoints.Count; ++index)
      {
        StandingPoint standingPoint = this.StandingPoints[index];
        if (standingPoint.GameEntity.HasTag(this.AmmoPickUpTag))
        {
          if (!flag5 && !(isDeactivated | flag3) && (!flag4 || standingPoint == this.CurrentlyUsedAmmoPickUpPoint))
            flag5 = true;
          else
            continue;
        }
        else if (standingPoint == this.LoadAmmoStandingPoint && isDeactivated | flag4)
          continue;
        if (standingPoint.IsUsableBySide(side))
        {
          if (!standingPoint.HasAIMovingTo)
          {
            if (!flag2)
              this._usableStandingPoints.Clear();
            flag2 = true;
          }
          else if (flag2 || standingPoint.MovingAgents.FirstOrDefault<KeyValuePair<Agent, UsableMissionObject.MoveInfo>>().Key.Formation.Team.Side != side)
            continue;
          flag1 = true;
          this._usableStandingPoints.Add((index, standingPoint));
        }
      }
      this._areUsableStandingPointsVacant = flag2;
      if (!flag1)
        return float.MinValue;
      if (flag2)
        return 1f;
      return !this._isDetachmentRecentlyEvaluated ? 0.1f : 0.01f;
    }

    public void SetSpawnedFromSpawner() => this._spawnedFromSpawner = true;
  }
}
