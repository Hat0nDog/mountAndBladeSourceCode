// Decompiled with JetBrains decompiler
// Type: TaleWorlds.MountAndBlade.Ballista
// Assembly: TaleWorlds.MountAndBlade, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3C0E3F42-756F-4407-B817-06BA412A78B0
// Assembly location: E:\SteamLibrary\steamapps\common\Mount & Blade II Bannerlord\bin\Win64_Shipping_Client\TaleWorlds.MountAndBlade.dll

using System.Collections.Generic;
using TaleWorlds.Core;
using TaleWorlds.Engine;
using TaleWorlds.Library;
using TaleWorlds.Localization;
using TaleWorlds.MountAndBlade.Objects.Siege;

namespace TaleWorlds.MountAndBlade
{
  public class Ballista : RangedSiegeWeapon, ISpawnable
  {
    private static readonly ActionIndexCache act_usage_ballista_ammo_pick_up_end = ActionIndexCache.Create(nameof (act_usage_ballista_ammo_pick_up_end));
    private static readonly ActionIndexCache act_usage_ballista_ammo_pick_up_start = ActionIndexCache.Create(nameof (act_usage_ballista_ammo_pick_up_start));
    private static readonly ActionIndexCache act_usage_ballista_ammo_place_end = ActionIndexCache.Create(nameof (act_usage_ballista_ammo_place_end));
    private static readonly ActionIndexCache act_usage_ballista_ammo_place_start = ActionIndexCache.Create(nameof (act_usage_ballista_ammo_place_start));
    private static readonly ActionIndexCache act_usage_ballista_idle = ActionIndexCache.Create(nameof (act_usage_ballista_idle));
    private static readonly ActionIndexCache act_usage_ballista_reload = ActionIndexCache.Create(nameof (act_usage_ballista_reload));
    private static readonly ActionIndexCache act_strike_bent_over = ActionIndexCache.Create(nameof (act_strike_bent_over));
    public string NavelTag = "BallistaNavel";
    public string BodyTag = "BallistaBody";
    public float AnimationHeightDifference;
    private MatrixFrame _ballistaBodyInitialLocalFrame;
    private MatrixFrame _ballistaNavelInitialFrame;
    private MatrixFrame _pilotInitialLocalFrame;
    private MatrixFrame _pilotInitialLocalIKFrame;
    private MatrixFrame _missileInitialLocalFrame;
    [EditableScriptComponentVariable(true)]
    protected string IdleActionName = "act_usage_ballista_idle_attacker";
    [EditableScriptComponentVariable(true)]
    protected string ReloadActionName = "act_usage_ballista_reload_attacker";
    [EditableScriptComponentVariable(true)]
    protected string PlaceAmmoStartActionName = "act_usage_ballista_ammo_place_start_attacker";
    [EditableScriptComponentVariable(true)]
    protected string PlaceAmmoEndActionName = "act_usage_ballista_ammo_place_end_attacker";
    [EditableScriptComponentVariable(true)]
    protected string PickUpAmmoStartActionName = "act_usage_ballista_ammo_pick_up_start_attacker";
    [EditableScriptComponentVariable(true)]
    protected string PickUpAmmoEndActionName = "act_usage_ballista_ammo_pick_up_end_attacker";
    private ActionIndexCache _idleAnimationActionIndex;
    private ActionIndexCache _reloadAnimationActionIndex;
    private ActionIndexCache _placeAmmoStartAnimationActionIndex;
    private ActionIndexCache _placeAmmoEndAnimationActionIndex;
    private ActionIndexCache _pickUpAmmoStartAnimationActionIndex;
    private ActionIndexCache _pickUpAmmoEndAnimationActionIndex;

    protected SynchedMissionObject ballistaBody { get; private set; }

    protected SynchedMissionObject ballistaNavel { get; private set; }

    public override float DirectionRestriction => 1.570796f;

    protected override void RegisterAnimationParameters()
    {
      this.SkeletonOwnerObject = this.ballistaBody;
      this.skeletonName = "ballista_skeleton";
      this.fireAnimation = "ballista_fire";
      this.setUpAnimation = "ballista_set_up";
      this._idleAnimationActionIndex = ActionIndexCache.Create(this.IdleActionName);
      this._reloadAnimationActionIndex = ActionIndexCache.Create(this.ReloadActionName);
      this._placeAmmoStartAnimationActionIndex = ActionIndexCache.Create(this.PlaceAmmoStartActionName);
      this._placeAmmoEndAnimationActionIndex = ActionIndexCache.Create(this.PlaceAmmoEndActionName);
      this._pickUpAmmoStartAnimationActionIndex = ActionIndexCache.Create(this.PickUpAmmoStartActionName);
      this._pickUpAmmoEndAnimationActionIndex = ActionIndexCache.Create(this.PickUpAmmoEndActionName);
    }

    public override SiegeEngineType GetSiegeEngineType() => DefaultSiegeEngineTypes.Ballista;

    protected internal override void OnInit()
    {
      this.ballistaBody = this.GameEntity.CollectObjectsWithTag<SynchedMissionObject>(this.BodyTag)[0];
      this.ballistaNavel = this.GameEntity.CollectObjectsWithTag<SynchedMissionObject>(this.NavelTag)[0];
      this.RotationObject = (SynchedMissionObject) this;
      base.OnInit();
      this.UsesMouseForAiming = true;
      this.GetSoundEventIndices();
      this._ballistaNavelInitialFrame = this.ballistaNavel.GameEntity.GetFrame();
      MatrixFrame globalFrame1 = this.ballistaBody.GameEntity.GetGlobalFrame();
      this._ballistaBodyInitialLocalFrame = this.ballistaBody.GameEntity.GetFrame();
      MatrixFrame globalFrame2 = this.PilotStandingPoint.GameEntity.GetGlobalFrame();
      this._pilotInitialLocalFrame = this.PilotStandingPoint.GameEntity.GetFrame();
      this._pilotInitialLocalIKFrame = globalFrame2.TransformToLocal(globalFrame1);
      this._missileInitialLocalFrame = this.Projectile.GameEntity.GetFrame();
      this.PilotStandingPoint.AddComponent((UsableMissionObjectComponent) new ClearHandInverseKinematicsOnStopUsageComponent());
      this.EnemyRangeToStopUsing = 5f;
      this.AttackClickWillReload = true;
      this.WeaponNeedsClickToReload = true;
      this.SetScriptComponentToTick(this.GetTickRequirement());
    }

    protected override bool CanRotate() => this.State != RangedSiegeWeapon.WeaponState.Shooting;

    public override UsableMachineAIBase CreateAIBehaviourObject() => (UsableMachineAIBase) new BallistaAI(this);

    protected override void OnRangedSiegeWeaponStateChange()
    {
      base.OnRangedSiegeWeaponStateChange();
      switch (this.State)
      {
        case RangedSiegeWeapon.WeaponState.Idle:
          if (this.AmmoCount <= 0)
            break;
          if (!GameNetwork.IsClientOrReplay)
          {
            this.ConsumeAmmo();
            break;
          }
          this.SetAmmo(this.AmmoCount - 1);
          break;
        case RangedSiegeWeapon.WeaponState.WaitingBeforeProjectileLeaving:
          if (!GameNetwork.IsClientOrReplay)
          {
            this.Projectile.SetVisibleSynched(false);
            break;
          }
          this.Projectile.GameEntity.SetVisibilityExcludeParents(false);
          break;
      }
    }

    protected override float MaximumBallisticError => 0.5f;

    protected override float HorizontalAimSensitivity => 1f;

    protected override float VerticalAimSensitivity => 1f;

    protected override Vec3 VisualizationShootingDirection
    {
      get
      {
        Mat3 rotation = this.GameEntity.GetGlobalFrame().rotation;
        rotation.RotateAboutSide(-this.VisualizeReleaseTrajectoryAngle);
        return rotation.TransformToParent(new Vec3(y: -1f));
      }
    }

    protected override void HandleUserAiming(float dt)
    {
      if (this.PilotAgent == null)
        this.targetReleaseAngle = 0.0f;
      base.HandleUserAiming(dt);
    }

    protected override void ApplyAimChange()
    {
      MatrixFrame navelInitialFrame = this._ballistaNavelInitialFrame;
      navelInitialFrame.rotation.RotateAboutUp(this.currentDirection);
      this.ballistaNavel.GameEntity.SetFrame(ref navelInitialFrame);
      MatrixFrame local = this._ballistaNavelInitialFrame.TransformToLocal(this._pilotInitialLocalFrame);
      MatrixFrame parent = navelInitialFrame.TransformToParent(local);
      this.PilotStandingPoint.GameEntity.SetFrame(ref parent);
      MatrixFrame initialLocalFrame = this._ballistaBodyInitialLocalFrame;
      initialLocalFrame.rotation.RotateAboutSide(-this.currentReleaseAngle);
      this.ballistaBody.GameEntity.SetFrame(ref initialLocalFrame);
    }

    protected override float ShootingSpeed => 120f;

    protected override void GetSoundEventIndices()
    {
      this.MoveSoundIndex = SoundEvent.GetEventIdFromString("event:/mission/siege/ballista/move");
      this.ReloadSoundIndex = SoundEvent.GetEventIdFromString("event:/mission/siege/ballista/reload");
    }

    internal override bool IsTargetValid(ITargetable target) => !(target is ICastleKeyPosition);

    protected override ScriptComponentBehaviour.TickRequirement GetTickRequirement() => this.GameEntity.IsVisibleIncludeParents() ? ScriptComponentBehaviour.TickRequirement.Tick : base.GetTickRequirement();

    protected internal override void OnTick(float dt)
    {
      base.OnTick(dt);
      if (!this.GameEntity.IsVisibleIncludeParents())
        return;
      if (this.PilotAgent != null)
      {
        this.PilotAgent.SetHandInverseKinematicsFrameFromActionChannel(1, this._pilotInitialLocalIKFrame, this.ballistaBody.GameEntity.GetGlobalFrame(), this.AnimationHeightDifference);
        ActionIndexCache currentAction = this.PilotAgent.GetCurrentAction(1);
        if (currentAction == this._pickUpAmmoEndAnimationActionIndex || currentAction == this._placeAmmoStartAnimationActionIndex)
        {
          MatrixFrame boneEntitialFrame = this.PilotAgent.AgentVisuals.GetSkeleton().GetBoneEntitialFrame((int) this.PilotAgent.Monster.MainHandItemBoneIndex);
          this.Projectile.GameEntity.SetGlobalFrame(this.PilotAgent.AgentVisuals.GetGlobalFrame().TransformToParent(boneEntitialFrame));
        }
        else
          this.Projectile.GameEntity.SetFrame(ref this._missileInitialLocalFrame);
      }
      if (GameNetwork.IsClientOrReplay)
        return;
      if (this.State == RangedSiegeWeapon.WeaponState.Reloading)
      {
        if (this.PilotAgent == null || this.PilotAgent.SetActionChannel(1, this._reloadAnimationActionIndex))
          return;
        this.PilotAgent.StopUsingGameObject();
      }
      else if (this.State == RangedSiegeWeapon.WeaponState.LoadingAmmo)
      {
        bool flag = false;
        if (this.PilotAgent != null)
        {
          ActionIndexCache currentAction = this.PilotAgent.GetCurrentAction(1);
          if (currentAction != this._pickUpAmmoStartAnimationActionIndex && currentAction != this._pickUpAmmoEndAnimationActionIndex && (currentAction != this._placeAmmoStartAnimationActionIndex && currentAction != this._placeAmmoEndAnimationActionIndex) && !this.PilotAgent.SetActionChannel(1, this._pickUpAmmoStartAnimationActionIndex))
            this.PilotAgent.StopUsingGameObject();
          else if (currentAction == this._pickUpAmmoEndAnimationActionIndex || currentAction == this._placeAmmoStartAnimationActionIndex)
            flag = true;
          else if (currentAction == this._placeAmmoEndAnimationActionIndex)
          {
            flag = true;
            this.State = RangedSiegeWeapon.WeaponState.WaitingBeforeIdle;
          }
        }
        this.Projectile.SetVisibleSynched(flag);
      }
      else if (this.State == RangedSiegeWeapon.WeaponState.WaitingBeforeIdle)
      {
        if (this.PilotAgent == null)
        {
          this.State = RangedSiegeWeapon.WeaponState.Idle;
        }
        else
        {
          ActionIndexCache currentAction = this.PilotAgent.GetCurrentAction(1);
          float currentActionProgress = this.PilotAgent.GetCurrentActionProgress(1);
          ActionIndexCache animationActionIndex = this._placeAmmoEndAnimationActionIndex;
          if (currentAction != animationActionIndex)
          {
            this.PilotAgent.StopUsingGameObject();
          }
          else
          {
            if ((double) currentActionProgress <= 0.999899983406067)
              return;
            this.State = RangedSiegeWeapon.WeaponState.Idle;
            if (this.PilotAgent == null || this.PilotAgent.SetActionChannel(1, this._idleAnimationActionIndex))
              return;
            this.PilotAgent.StopUsingGameObject();
          }
        }
      }
      else
      {
        if (this.PilotAgent == null)
          return;
        if (MBMath.IsBetween((int) this.PilotAgent.GetCurrentActionType(0), 44, 48))
        {
          if (!(this.PilotAgent.GetCurrentAction(0) != Ballista.act_strike_bent_over))
            return;
          this.PilotAgent.SetActionChannel(0, Ballista.act_strike_bent_over);
        }
        else
        {
          if (this.PilotAgent.SetActionChannel(1, this._idleAnimationActionIndex))
            return;
          this.PilotAgent.StopUsingGameObject();
        }
      }
    }

    public override TextObject GetActionTextForStandingPoint(
      UsableMissionObject usableGameObject)
    {
      TextObject textObject = new TextObject("{=fEQAPJ2e}({KEY}) Use");
      textObject.SetTextVariable("KEY", Game.Current.GameTextManager.GetHotKeyGameText("CombatHotKeyCategory", 13));
      return textObject;
    }

    public override string GetDescriptionText(GameEntity gameEntity = null) => new TextObject("{=abbALYlp}Ballista").ToString();

    protected override void UpdateAmmoMesh()
    {
      int num = 8 - this.AmmoCount;
      foreach (GameEntity child in this.GameEntity.GetChildren())
      {
        for (int metaMeshIndex = 0; metaMeshIndex < child.MultiMeshComponentCount; ++metaMeshIndex)
        {
          MetaMesh metaMesh = child.GetMetaMesh(metaMeshIndex);
          for (int meshIndex = 0; meshIndex < metaMesh.MeshCount; ++meshIndex)
            metaMesh.GetMeshAtIndex(meshIndex).SetVectorArgument(0.0f, (float) num, 0.0f, 0.0f);
        }
      }
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
      TargetFlags targetFlags = (TargetFlags) (0 | 2 | 8 | 16 | 32);
      if (this.IsDestroyed || this.IsDeactivated)
        targetFlags |= TargetFlags.NotAThreat;
      if (this.Side == BattleSideEnum.Attacker && DebugSiegeBehaviour.DebugDefendState == DebugSiegeBehaviour.DebugStateDefender.DebugDefendersToBallistae)
        targetFlags |= TargetFlags.DebugThreat;
      if (this.Side == BattleSideEnum.Defender && DebugSiegeBehaviour.DebugAttackState == DebugSiegeBehaviour.DebugStateAttacker.DebugAttackersToBallistae)
        targetFlags |= TargetFlags.DebugThreat;
      return targetFlags;
    }

    public override float GetTargetValue(List<Vec3> weaponPos) => 30f * this.GetUserMultiplierOfWeapon() * this.GetDistanceMultiplierOfWeapon(weaponPos[0]) * this.GetHitpointMultiplierofWeapon();

    public void SetSpawnedFromSpawner() => this._spawnedFromSpawner = true;
  }
}
