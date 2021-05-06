// Decompiled with JetBrains decompiler
// Type: TaleWorlds.MountAndBlade.Trebuchet
// Assembly: TaleWorlds.MountAndBlade, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3C0E3F42-756F-4407-B817-06BA412A78B0
// Assembly location: E:\SteamLibrary\steamapps\common\Mount & Blade II Bannerlord\bin\Win64_Shipping_Client\TaleWorlds.MountAndBlade.dll

using System.Collections.Generic;
using System.Linq;
using TaleWorlds.Core;
using TaleWorlds.Engine;
using TaleWorlds.Library;
using TaleWorlds.Localization;
using TaleWorlds.MountAndBlade.Objects.Siege;

namespace TaleWorlds.MountAndBlade
{
  public class Trebuchet : RangedSiegeWeapon, ISpawnable
  {
    private static readonly ActionIndexCache act_usage_trebuchet_idle = ActionIndexCache.Create(nameof (act_usage_trebuchet_idle));
    private static readonly ActionIndexCache act_usage_trebuchet_reload = ActionIndexCache.Create(nameof (act_usage_trebuchet_reload));
    private static readonly ActionIndexCache act_usage_trebuchet_reload_2 = ActionIndexCache.Create(nameof (act_usage_trebuchet_reload_2));
    private static readonly ActionIndexCache act_usage_trebuchet_reload_idle = ActionIndexCache.Create(nameof (act_usage_trebuchet_reload_idle));
    private static readonly ActionIndexCache act_usage_trebuchet_reload_2_idle = ActionIndexCache.Create(nameof (act_usage_trebuchet_reload_2_idle));
    private static readonly ActionIndexCache act_usage_trebuchet_load_ammo = ActionIndexCache.Create(nameof (act_usage_trebuchet_load_ammo));
    private static readonly ActionIndexCache act_usage_trebuchet_shoot = ActionIndexCache.Create(nameof (act_usage_trebuchet_shoot));
    private static readonly ActionIndexCache act_strike_bent_over = ActionIndexCache.Create(nameof (act_strike_bent_over));
    private static readonly ActionIndexCache act_pickup_boulder_begin = ActionIndexCache.Create(nameof (act_pickup_boulder_begin));
    private static readonly ActionIndexCache act_pickup_boulder_end = ActionIndexCache.Create(nameof (act_pickup_boulder_end));
    private const string BodyTag = "body";
    private const string SlideTag = "slide";
    private const string SlingTag = "sling";
    private const string RopeTag = "rope";
    private const string RotateTag = "rotate";
    private const string VerticalAdjusterTag = "vertical_adjuster";
    private const string MissileBoneName = "bn_projectile_holder";
    private const string LeftTag = "left";
    private const string _rotateObjectTag = "rotate_entity";
    public string AIAmmoLoadTag = "ammoload_ai";
    private SynchedMissionObject _body;
    private SynchedMissionObject _sling;
    private SynchedMissionObject _rope;
    public string IdleWithAmmoAnimation;
    public string IdleEmptyAnimation;
    public string BodyFireAnimation;
    public string BodySetUpAnimation;
    public string SlingFireAnimation;
    public string SlingSetUpAnimation;
    public string RopeFireAnimation;
    public string RopeSetUpAnimation;
    public string VerticalAdjusterAnimation;
    public float TimeGapBetweenShootActionAndProjectileLeaving = 1.6f;
    private GameEntity _verticalAdjuster;
    private MatrixFrame _verticalAdjusterStartingLocalFrame;
    private float _timeElapsedAfterLoading;
    private bool _shootAnimPlayed;
    private MatrixFrame[] _standingPointLocalIKFrames;
    private List<StandingPointWithWeaponRequirement> _ammoLoadPoints;
    private byte _missileBoneIndex;
    public float ProjectileSpeed = 45f;

    public override TextObject GetActionTextForStandingPoint(
      UsableMissionObject usableGameObject)
    {
      TextObject textObject = !usableGameObject.GameEntity.HasTag(this.AmmoPickUpTag) ? (!usableGameObject.GameEntity.HasTag(this.ReloadTag) ? (!usableGameObject.GameEntity.HasTag("rotate") ? (!usableGameObject.GameEntity.HasTag(this.AmmoLoadTag) ? TextObject.Empty : new TextObject("{=ibC4xPoo}({KEY}) Load Ammo")) : new TextObject("{=5wx4BF5h}({KEY}) Rotate")) : new TextObject(this.PilotStandingPoint == usableGameObject ? "{=fEQAPJ2e}({KEY}) Use" : "{=Na81xuXn}({KEY}) Rearm")) : new TextObject("{=bNYm3K6b}({KEY}) Pick Up");
      textObject.SetTextVariable("KEY", Game.Current.GameTextManager.GetHotKeyGameText("CombatHotKeyCategory", 13));
      return textObject;
    }

    public override string GetDescriptionText(GameEntity gameEntity = null) => gameEntity.HasTag(this.AmmoPickUpTag) ? new TextObject("{=pzfbPbWW}Boulder").ToString() : new TextObject("{=4Skg9QhO}Trebuchet").ToString();

    protected override void RegisterAnimationParameters()
    {
      this.SkeletonOwnerObjects = new SynchedMissionObject[3];
      this.skeletonNames = new string[3];
      this.fireAnimations = new string[3];
      this.setUpAnimations = new string[3];
      this.SkeletonOwnerObjects[0] = this._body;
      this.skeletonNames[0] = "trebuchet_a_skeleton";
      this.fireAnimations[0] = this.BodyFireAnimation;
      this.setUpAnimations[0] = this.BodySetUpAnimation;
      this.SkeletonOwnerObjects[1] = this._sling;
      this.skeletonNames[1] = "trebuchet_a_sling_skeleton";
      this.fireAnimations[1] = this.SlingFireAnimation;
      this.setUpAnimations[1] = this.SlingSetUpAnimation;
      this.SkeletonOwnerObjects[2] = this._rope;
      this.skeletonNames[2] = "trebuchet_a_rope_skeleton";
      this.fireAnimations[2] = this.RopeFireAnimation;
      this.setUpAnimations[2] = this.RopeSetUpAnimation;
    }

    public override SiegeEngineType GetSiegeEngineType() => DefaultSiegeEngineTypes.Trebuchet;

    protected override void GetSoundEventIndices()
    {
      this.MoveSoundIndex = SoundEvent.GetEventIdFromString("event:/mission/siege/mangonel/move");
      this.ReloadSoundIndex = SoundEvent.GetEventIdFromString("event:/mission/siege/mangonel/reload");
    }

    protected override float ShootingSpeed => this.ProjectileSpeed;

    public override UsableMachineAIBase CreateAIBehaviourObject() => (UsableMachineAIBase) new TrebuchetAI(this);

    protected internal override void OnInit()
    {
      this._body = this.GameEntity.CollectObjectsWithTag<SynchedMissionObject>("body")[0];
      this._sling = this.GameEntity.CollectObjectsWithTag<SynchedMissionObject>("sling")[0];
      this._rope = this.GameEntity.CollectObjectsWithTag<SynchedMissionObject>("rope")[0];
      this._verticalAdjuster = this.GameEntity.CollectChildrenEntitiesWithTag("vertical_adjuster")[0];
      this._verticalAdjusterStartingLocalFrame = this._verticalAdjuster.GetFrame();
      this._verticalAdjusterStartingLocalFrame = this._body.GameEntity.GetBoneEntitialFrameWithIndex((byte) 0).TransformToLocal(this._verticalAdjusterStartingLocalFrame);
      this.RotationObject = this.GameEntity.CollectObjectsWithTag<SynchedMissionObject>("rotate_entity")[0];
      base.OnInit();
      this.timeGapBetweenShootActionAndProjectileLeaving = this.TimeGapBetweenShootActionAndProjectileLeaving;
      this.timeGapBetweenShootingEndAndReloadingStart = 0.0f;
      this._ammoLoadPoints = new List<StandingPointWithWeaponRequirement>();
      if (this.StandingPoints != null)
      {
        int index1 = -1;
        for (int index2 = 0; index2 < this.StandingPoints.Count; ++index2)
        {
          if (this.StandingPoints[index2].GameEntity.HasTag(this.AIAmmoLoadTag))
          {
            if (index1 >= 0)
              this._ammoLoadPoints.Add(this.StandingPoints[index2] as StandingPointWithWeaponRequirement);
            else
              index1 = index2;
          }
          else if (this.StandingPoints[index2].GameEntity.HasTag(this.AmmoLoadTag))
            this._ammoLoadPoints.Add(this.StandingPoints[index2] as StandingPointWithWeaponRequirement);
        }
        if (index1 >= 0)
        {
          this.LoadAmmoStandingPoint = this.StandingPoints[index1] as StandingPointWithWeaponRequirement;
          this._ammoLoadPoints.Add(this.LoadAmmoStandingPoint);
        }
        else
          this.LoadAmmoStandingPoint = this._ammoLoadPoints[this._ammoLoadPoints.Count - 1];
        MatrixFrame globalFrame = this._body.GameEntity.GetGlobalFrame();
        this._standingPointLocalIKFrames = new MatrixFrame[this.StandingPoints.Count];
        for (int index2 = 0; index2 < this.StandingPoints.Count; ++index2)
        {
          this._standingPointLocalIKFrames[index2] = this.StandingPoints[index2].GameEntity.GetGlobalFrame().TransformToLocal(globalFrame);
          this.StandingPoints[index2].AddComponent((UsableMissionObjectComponent) new ClearHandInverseKinematicsOnStopUsageComponent());
        }
      }
      this.ApplyAimChange();
      if (!GameNetwork.IsClientOrReplay)
      {
        this.EnemyRangeToStopUsing = 7f;
        if (this.State == RangedSiegeWeapon.WeaponState.Idle)
          this._sling.SetAnimationAtChannelSynched(this.IdleWithAmmoAnimation, 0);
        else
          this._sling.SetAnimationAtChannelSynched(this.IdleEmptyAnimation, 0);
      }
      this._missileBoneIndex = (byte) Skeleton.GetBoneIndexFromName(this._sling.GameEntity.Skeleton.GetName(), "bn_projectile_holder");
      this._shootAnimPlayed = false;
      this.UpdateAmmoMesh();
      this.SetScriptComponentToTick(this.GetTickRequirement());
      this.UpdateProjectilePosition();
    }

    public override void AfterMissionStart()
    {
      if (this.AmmoPickUpStandingPoints != null)
      {
        foreach (UsableMissionObject pickUpStandingPoint in this.AmmoPickUpStandingPoints)
          pickUpStandingPoint.LockUserFrames = true;
      }
      if (this._ammoLoadPoints == null)
        return;
      foreach (StandingPointWithWeaponRequirement ammoLoadPoint in this._ammoLoadPoints)
      {
        ammoLoadPoint.LockUserFrames = true;
        if (!GameNetwork.IsClientOrReplay)
          ammoLoadPoint.SetIsDeactivatedSynched(true);
      }
    }

    protected override void OnRangedSiegeWeaponStateChange()
    {
      base.OnRangedSiegeWeaponStateChange();
      if (this.State == RangedSiegeWeapon.WeaponState.WaitingBeforeIdle)
        this.UpdateProjectilePosition();
      if (GameNetwork.IsClientOrReplay)
        return;
      switch (this.State)
      {
        case RangedSiegeWeapon.WeaponState.Idle:
          this.Projectile.SetVisibleSynched(true);
          break;
        case RangedSiegeWeapon.WeaponState.Shooting:
          this.Projectile.SetVisibleSynched(false);
          Agent pilotAgent = this.PilotAgent;
          if (this.PilotAgent == null)
            break;
          int num1 = pilotAgent.IsAIControlled ? 1 : 0;
          break;
        case RangedSiegeWeapon.WeaponState.LoadingAmmo:
          this._sling.SetAnimationAtChannelSynched(this.IdleEmptyAnimation, 0);
          break;
        case RangedSiegeWeapon.WeaponState.WaitingBeforeIdle:
          foreach (UsableMissionObject ammoLoadPoint in this._ammoLoadPoints)
            ammoLoadPoint.SetIsDeactivatedSynched(true);
          Agent userAgent = this.LoadAmmoStandingPoint.UserAgent;
          if (!this.LoadAmmoStandingPoint.HasAIUser || !userAgent.IsAIControlled)
            break;
          int num2 = this.PilotStandingPoint.HasAIUser ? 1 : 0;
          break;
        case RangedSiegeWeapon.WeaponState.Reloading:
          this._shootAnimPlayed = false;
          break;
      }
    }

    protected override float HorizontalAimSensitivity => 0.1f;

    protected override float VerticalAimSensitivity => 0.075f;

    public override float DirectionRestriction => base.DirectionRestriction / 1.5f;

    protected override Vec3 ShootingDirection
    {
      get
      {
        Mat3 rotation = this.RotationObject.GameEntity.GetGlobalFrame().rotation;
        rotation.RotateAboutSide(-this.currentReleaseAngle);
        return rotation.TransformToParent(new Vec3(y: -1f));
      }
    }

    protected override Vec3 VisualizationShootingDirection
    {
      get
      {
        Mat3 rotation = this.RotationObject.GameEntity.GetGlobalFrame().rotation;
        rotation.RotateAboutSide(-this.VisualizeReleaseTrajectoryAngle);
        return rotation.TransformToParent(new Vec3(y: -1f));
      }
    }

    protected override float MaximumBallisticError => base.MaximumBallisticError;

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

    public override float ProcessTargetValue(float baseValue, TargetFlags flags)
    {
      if (flags.HasAnyFlag<TargetFlags>(TargetFlags.NotAThreat))
        return -1000f;
      if (flags.HasAnyFlag<TargetFlags>(TargetFlags.None))
        baseValue *= 1.5f;
      if (flags.HasAnyFlag<TargetFlags>(TargetFlags.IsSiegeEngine))
        baseValue *= 2.5f;
      if (flags.HasAnyFlag<TargetFlags>(TargetFlags.IsStructure))
        baseValue *= 0.1f;
      if (flags.HasAnyFlag<TargetFlags>(TargetFlags.DebugThreat))
        baseValue *= 10000f;
      return baseValue;
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

    protected override bool CanRotate() => this.State == RangedSiegeWeapon.WeaponState.Idle || this.State == RangedSiegeWeapon.WeaponState.LoadingAmmo || this.State == RangedSiegeWeapon.WeaponState.WaitingBeforeIdle;

    protected override ScriptComponentBehaviour.TickRequirement GetTickRequirement() => this.GameEntity.IsVisibleIncludeParents() ? ScriptComponentBehaviour.TickRequirement.Tick : base.GetTickRequirement();

    protected internal override void OnTick(float dt)
    {
      base.OnTick(dt);
      if (!this.GameEntity.IsVisibleIncludeParents())
        return;
      if (this.State == RangedSiegeWeapon.WeaponState.WaitingBeforeProjectileLeaving)
        this.UpdateProjectilePosition();
      this._verticalAdjuster.Skeleton.SetAnimationAtChannel(this.VerticalAdjusterAnimation, 0);
      this._verticalAdjuster.Skeleton.SetAnimationParameterAtChannel(0, MBMath.ClampFloat((float) (((double) this.currentReleaseAngle - (double) this.BottomReleaseAngleRestriction) / ((double) this.TopReleaseAngleRestriction - (double) this.BottomReleaseAngleRestriction)), 0.0f, 1f));
      MatrixFrame parent = this._body.GameEntity.GetBoneEntitialFrameWithIndex((byte) 0).TransformToParent(this._verticalAdjusterStartingLocalFrame);
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
            if (this.StandingPoints[index].UserAgent.GetCurrentAction(1) == Trebuchet.act_usage_trebuchet_reload_2)
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
        if (this.PilotAgent != null)
        {
          ActionIndexCache currentAction = this.PilotAgent.GetCurrentAction(1);
          if (this.State == RangedSiegeWeapon.WeaponState.WaitingBeforeProjectileLeaving || this.State == RangedSiegeWeapon.WeaponState.Shooting || this.State == RangedSiegeWeapon.WeaponState.WaitingBeforeReloading)
          {
            if (MBMath.IsBetween((int) this.PilotAgent.GetCurrentActionType(0), 44, 48))
            {
              if (this.PilotAgent.GetCurrentAction(0) != Trebuchet.act_strike_bent_over)
                this.PilotAgent.SetActionChannel(0, Trebuchet.act_strike_bent_over);
            }
            else if (!this._shootAnimPlayed && currentAction != Trebuchet.act_usage_trebuchet_shoot)
            {
              this.PilotAgent.SetActionChannel(1, Trebuchet.act_usage_trebuchet_shoot);
              this._shootAnimPlayed = true;
            }
            else if (currentAction != Trebuchet.act_usage_trebuchet_shoot && !this.PilotAgent.SetActionChannel(1, Trebuchet.act_usage_trebuchet_reload_idle))
              this.PilotAgent.StopUsingGameObject();
          }
          else if (currentAction != Trebuchet.act_usage_trebuchet_reload && currentAction != Trebuchet.act_usage_trebuchet_shoot && !this.PilotAgent.SetActionChannel(1, Trebuchet.act_usage_trebuchet_idle))
            this.PilotAgent.StopUsingGameObject();
        }
        if (this.State != RangedSiegeWeapon.WeaponState.Reloading)
        {
          foreach (StandingPoint reloadStandingPoint in this.ReloadStandingPoints)
          {
            if (reloadStandingPoint.HasUser && reloadStandingPoint != this.PilotStandingPoint)
            {
              Agent userAgent = reloadStandingPoint.UserAgent;
              if (!userAgent.SetActionChannel(1, Trebuchet.act_usage_trebuchet_reload_2_idle))
                userAgent.StopUsingGameObject();
            }
          }
        }
        foreach (StandingPointWithWeaponRequirement pickUpStandingPoint in this.AmmoPickUpStandingPoints)
        {
          if (pickUpStandingPoint.HasUser)
          {
            Agent userAgent = pickUpStandingPoint.UserAgent;
            ActionIndexCache currentAction = userAgent.GetCurrentAction(1);
            if (!(currentAction == Trebuchet.act_pickup_boulder_begin))
            {
              if (currentAction == Trebuchet.act_pickup_boulder_end)
              {
                MissionWeapon weapon = new MissionWeapon(this.OriginalMissileItem, (ItemModifier) null, (Banner) null);
                userAgent.EquipWeaponToExtraSlotAndWield(ref weapon);
                userAgent.StopUsingGameObject();
                this.ConsumeAmmo();
                if (!this.IsDeactivated && this.HasAIPickingUpAmmo && this.CurrentlyUsedAmmoPickUpPoint == pickUpStandingPoint)
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
              else if (!userAgent.SetActionChannel(1, Trebuchet.act_pickup_boulder_begin))
                userAgent.StopUsingGameObject();
            }
          }
        }
        foreach (StandingPoint standingPoint in this.StandingPoints)
        {
          if (standingPoint.HasUser && this.ReloadStandingPoints.IndexOf(standingPoint) < 0 && (((IReadOnlyList<StandingPoint>) this._ammoLoadPoints).IndexOf<StandingPoint>(standingPoint) < 0 && ((IReadOnlyList<StandingPoint>) this.AmmoPickUpStandingPoints).IndexOf<StandingPoint>(standingPoint) < 0))
          {
            Agent userAgent = standingPoint.UserAgent;
            if (!userAgent.SetActionChannel(1, Trebuchet.act_usage_trebuchet_reload_2_idle))
              userAgent.StopUsingGameObject();
          }
        }
      }
      switch (this.State)
      {
        case RangedSiegeWeapon.WeaponState.LoadingAmmo:
          if (GameNetwork.IsClientOrReplay)
            break;
          bool flag = false;
          using (List<StandingPointWithWeaponRequirement>.Enumerator enumerator = this._ammoLoadPoints.GetEnumerator())
          {
            while (enumerator.MoveNext())
            {
              StandingPointWithWeaponRequirement current = enumerator.Current;
              if (flag)
                current.SetIsDeactivatedSynched(true);
              else if (current.HasUser)
              {
                flag = true;
                Agent userAgent = current.UserAgent;
                ActionIndexCache currentAction = userAgent.GetCurrentAction(1);
                MissionWeapon missionWeapon;
                if (currentAction == Trebuchet.act_usage_trebuchet_load_ammo && (double) userAgent.AgentVisuals.GetSkeleton().GetAnimationParameterAtChannel(1) > 0.560000002384186)
                {
                  EquipmentIndex wieldedItemIndex = userAgent.GetWieldedItemIndex(Agent.HandIndex.MainHand);
                  if (wieldedItemIndex != EquipmentIndex.None)
                  {
                    missionWeapon = userAgent.Equipment[wieldedItemIndex];
                    if (missionWeapon.CurrentUsageItem.WeaponClass == this.OriginalMissileItem.PrimaryWeapon.WeaponClass)
                    {
                      Agent loadingAgent = userAgent;
                      missionWeapon = userAgent.Equipment[wieldedItemIndex];
                      string stringId = missionWeapon.Item.StringId;
                      this.ChangeProjectileEntityServer(loadingAgent, stringId);
                      userAgent.RemoveEquippedWeapon(wieldedItemIndex);
                      this._timeElapsedAfterLoading = 0.0f;
                      this.Projectile.SetVisibleSynched(true);
                      this._sling.SetAnimationAtChannelSynched(this.IdleWithAmmoAnimation, 0);
                      this.State = RangedSiegeWeapon.WeaponState.WaitingBeforeIdle;
                      continue;
                    }
                  }
                  current.UserAgent.StopUsingGameObject();
                }
                else if (currentAction != Trebuchet.act_usage_trebuchet_load_ammo && !current.UserAgent.SetActionChannel(1, Trebuchet.act_usage_trebuchet_load_ammo))
                {
                  for (EquipmentIndex equipmentIndex = EquipmentIndex.WeaponItemBeginSlot; equipmentIndex < EquipmentIndex.NumAllWeaponSlots; ++equipmentIndex)
                  {
                    missionWeapon = userAgent.Equipment[equipmentIndex];
                    if (!missionWeapon.IsEmpty)
                    {
                      missionWeapon = userAgent.Equipment[equipmentIndex];
                      if (missionWeapon.CurrentUsageItem.WeaponClass == this.OriginalMissileItem.PrimaryWeapon.WeaponClass)
                        userAgent.RemoveEquippedWeapon(equipmentIndex);
                    }
                  }
                  current.UserAgent.StopUsingGameObject();
                }
              }
              else
                current.SetIsDeactivatedSynched(false);
            }
            break;
          }
        case RangedSiegeWeapon.WeaponState.WaitingBeforeIdle:
          this._timeElapsedAfterLoading += dt;
          if ((double) this._timeElapsedAfterLoading <= 1.0)
            break;
          this.State = RangedSiegeWeapon.WeaponState.Idle;
          break;
        case RangedSiegeWeapon.WeaponState.Reloading:
          for (int index = 0; index < this.ReloadStandingPoints.Count; ++index)
          {
            if (this.ReloadStandingPoints[index].HasUser)
            {
              ActionIndexCache currentAction = this.ReloadStandingPoints[index].UserAgent.GetCurrentAction(1);
              if (currentAction == Trebuchet.act_usage_trebuchet_reload || currentAction == Trebuchet.act_usage_trebuchet_reload_2)
                this.ReloadStandingPoints[index].UserAgent.SetCurrentActionProgress(1, this._body.GameEntity.Skeleton.GetAnimationParameterAtChannel(0));
              else if (!GameNetwork.IsClientOrReplay)
              {
                ActionIndexCache actionIndexCache = Trebuchet.act_usage_trebuchet_reload;
                if (this.ReloadStandingPoints[index].GameEntity.HasTag("right"))
                  actionIndexCache = Trebuchet.act_usage_trebuchet_reload_2;
                if (!this.ReloadStandingPoints[index].UserAgent.SetActionChannel(1, actionIndexCache, startProgress: this._body.GameEntity.Skeleton.GetAnimationParameterAtChannel(0)))
                  this.ReloadStandingPoints[index].UserAgent.StopUsingGameObject();
              }
            }
          }
          break;
      }
    }

    private void UpdateProjectilePosition()
    {
      MatrixFrame entitialFrameWithIndex = this._sling.GameEntity.GetBoneEntitialFrameWithIndex(this._missileBoneIndex);
      this.Projectile.GameEntity.SetFrame(ref entitialFrameWithIndex);
    }

    internal override bool IsStandingPointNotUsedOnAccountOfBeingAmmoLoad(
      StandingPoint standingPoint)
    {
      return ((IEnumerable<StandingPoint>) this._ammoLoadPoints).Contains<StandingPoint>(standingPoint) && this.LoadAmmoStandingPoint != standingPoint || base.IsStandingPointNotUsedOnAccountOfBeingAmmoLoad(standingPoint);
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
