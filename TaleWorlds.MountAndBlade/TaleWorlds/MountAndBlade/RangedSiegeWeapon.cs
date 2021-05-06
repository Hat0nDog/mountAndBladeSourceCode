// Decompiled with JetBrains decompiler
// Type: TaleWorlds.MountAndBlade.RangedSiegeWeapon
// Assembly: TaleWorlds.MountAndBlade, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3C0E3F42-756F-4407-B817-06BA412A78B0
// Assembly location: E:\SteamLibrary\steamapps\common\Mount & Blade II Bannerlord\bin\Win64_Shipping_Client\TaleWorlds.MountAndBlade.dll

using NetworkMessages.FromClient;
using NetworkMessages.FromServer;
using System;
using System.Collections.Generic;
using System.Linq;
using TaleWorlds.Core;
using TaleWorlds.DotNet;
using TaleWorlds.Engine;
using TaleWorlds.Library;
using TaleWorlds.MountAndBlade.Network.Messages;

namespace TaleWorlds.MountAndBlade
{
  public abstract class RangedSiegeWeapon : SiegeWeapon
  {
    public const string MultipleProjectileId = "grapeshot_fire_stack";
    public const string MultipleProjectileFlyingId = "grapeshot_fire_projectile";
    public const int MultipleProjectileCount = 5;
    public string ReloadTag = "reload";
    public string AmmoLoadTag = "ammoload";
    public string CameraHolderTag = nameof (cameraHolder);
    public string ProjectileTag = "projectile";
    public string MissileItemID;
    protected bool UsesMouseForAiming;
    private RangedSiegeWeapon.WeaponState _state;
    public RangedSiegeWeapon.FiringFocus Focus;
    private int _projectileIndex;
    private GameEntity _missileStartingPositionEntityForSimulation;
    protected SynchedMissionObject[] SkeletonOwnerObjects;
    protected string[] skeletonNames;
    protected string[] fireAnimations;
    protected string[] setUpAnimations;
    protected SynchedMissionObject RotationObject;
    private MatrixFrame _rotationObjectInitialFrame;
    protected SoundEvent MoveSound;
    protected SoundEvent ReloadSound;
    protected int MoveSoundIndex = -1;
    protected int ReloadSoundIndex = -1;
    protected ItemObject OriginalMissileItem;
    protected ItemObject LoadedMissileItem;
    protected List<StandingPoint> ReloadStandingPoints;
    protected List<StandingPointWithWeaponRequirement> AmmoPickUpStandingPoints;
    protected StandingPointWithWeaponRequirement LoadAmmoStandingPoint;
    protected bool AttackClickWillReload;
    protected bool WeaponNeedsClickToReload;
    public int startingAmmoCount = 20;
    protected int CurrentAmmo = 1;
    private bool _hasAmmo = true;
    protected float targetDirection;
    protected float targetReleaseAngle;
    protected float cameraDirection;
    protected float cameraReleaseAngle;
    protected float reloadTargetReleaseAngle;
    protected float maxRotateSpeed;
    protected float dontMoveTimer;
    private MatrixFrame cameraHolderInitialFrame;
    private RangedSiegeWeapon.CameraState cameraState;
    private bool _inputGiven;
    private float _inputX;
    private float _inputY;
    private bool _exactInputGiven;
    private float _inputTargetX;
    private float _inputTargetY;
    private Vec3 _ammoPickupCenter;
    protected float currentDirection;
    private Vec3 _originalDirection;
    protected float currentReleaseAngle;
    private float _lastSyncedDirection;
    private float _lastSyncedReleaseAngle;
    private float _syncTimer;
    public float TopReleaseAngleRestriction = 1.570796f;
    public float BottomReleaseAngleRestriction = -1.570796f;
    protected float ReleaseAngleRestrictionCenter;
    protected float ReleaseAngleRestrictionAngle;
    private float animationTimeElapsed;
    protected float timeGapBetweenShootingEndAndReloadingStart = 0.6f;
    protected float timeGapBetweenShootActionAndProjectileLeaving;
    private int _currentReloaderCount;
    private Agent _lastShooterAgent;
    protected BattleSideEnum _defaultSide;
    private bool hasFrameChangedInPreviousFrame;
    public float VisualizeReleaseTrajectoryAngle;
    private TrajectoryVisualizer _editorVisualizer;

    public RangedSiegeWeapon.WeaponState State
    {
      get => this._state;
      set
      {
        if (this._state == value)
          return;
        if (GameNetwork.IsServerOrRecorder)
        {
          GameNetwork.BeginBroadcastModuleEvent();
          GameNetwork.WriteMessage((GameNetworkMessage) new SetRangedSiegeWeaponState(this, value));
          GameNetwork.EndBroadcastModuleEvent(GameNetwork.EventBroadcastFlags.AddToMissionRecord);
        }
        this._state = value;
        this.OnRangedSiegeWeaponStateChange();
      }
    }

    public GameEntity cameraHolder { get; private set; }

    protected SynchedMissionObject Projectile { get; private set; }

    protected Vec3 MissleStartingPositionForSimulation
    {
      get
      {
        if ((NativeObject) this._missileStartingPositionEntityForSimulation != (NativeObject) null)
          return this._missileStartingPositionEntityForSimulation.GlobalPosition;
        SynchedMissionObject projectile = this.Projectile;
        return projectile == null ? Vec3.Zero : projectile.GameEntity.GlobalPosition;
      }
    }

    protected SynchedMissionObject SkeletonOwnerObject
    {
      set => this.SkeletonOwnerObjects = new SynchedMissionObject[1]
      {
        value
      };
    }

    protected string skeletonName
    {
      set => this.skeletonNames = new string[1]{ value };
    }

    protected string fireAnimation
    {
      set => this.fireAnimations = new string[1]{ value };
    }

    protected string setUpAnimation
    {
      set => this.setUpAnimations = new string[1]{ value };
    }

    public event RangedSiegeWeapon.OnSiegeWeaponReloadDone OnReloadDone;

    public int AmmoCount
    {
      get => this.CurrentAmmo;
      protected set => this.CurrentAmmo = value;
    }

    protected virtual bool HasAmmo
    {
      get => this._hasAmmo;
      set => this._hasAmmo = value;
    }

    protected virtual void ConsumeAmmo()
    {
      --this.AmmoCount;
      if (GameNetwork.IsServerOrRecorder)
      {
        GameNetwork.BeginBroadcastModuleEvent();
        GameNetwork.WriteMessage((GameNetworkMessage) new SetRangedSiegeWeaponAmmo(this, this.AmmoCount));
        GameNetwork.EndBroadcastModuleEvent(GameNetwork.EventBroadcastFlags.AddToMissionRecord);
      }
      this.UpdateAmmoMesh();
      this.CheckAmmo();
    }

    public virtual void SetAmmo(int ammoLeft)
    {
      if (this.AmmoCount == ammoLeft)
        return;
      this.AmmoCount = ammoLeft;
      this.UpdateAmmoMesh();
      this.CheckAmmo();
    }

    protected virtual void CheckAmmo()
    {
      if (this.AmmoCount > 0)
        return;
      this.HasAmmo = false;
      this.ForcedUse = false;
      foreach (UsableMissionObject pickUpStandingPoint in this.AmmoPickUpStandingPoints)
        pickUpStandingPoint.IsDeactivated = true;
    }

    public virtual float DirectionRestriction => 2.094395f;

    public Vec3 OriginalDirection => this._originalDirection;

    protected virtual float HorizontalAimSensitivity => 0.2f;

    protected virtual float VerticalAimSensitivity => 0.2f;

    protected abstract void RegisterAnimationParameters();

    protected abstract void GetSoundEventIndices();

    public event Action<RangedSiegeWeapon, Agent> OnAgentLoadsMachine;

    protected void ChangeProjectileEntityServer(Agent loadingAgent, string missileItemID)
    {
      List<SynchedMissionObject> synchedMissionObjectList = this.GameEntity.CollectObjectsWithTag<SynchedMissionObject>(this.ProjectileTag);
      for (int index = 0; index < synchedMissionObjectList.Count; ++index)
      {
        if (synchedMissionObjectList[index].GameEntity.HasTag(missileItemID))
        {
          this.Projectile = synchedMissionObjectList[index];
          this._projectileIndex = index;
          break;
        }
      }
      this.LoadedMissileItem = Game.Current.ObjectManager.GetObject<ItemObject>(missileItemID);
      if (GameNetwork.IsServerOrRecorder)
      {
        GameNetwork.BeginBroadcastModuleEvent();
        GameNetwork.WriteMessage((GameNetworkMessage) new RangedSiegeWeaponChangeProjectile(this, this._projectileIndex));
        GameNetwork.EndBroadcastModuleEvent(GameNetwork.EventBroadcastFlags.AddToMissionRecord);
      }
      Action<RangedSiegeWeapon, Agent> agentLoadsMachine = this.OnAgentLoadsMachine;
      if (agentLoadsMachine == null)
        return;
      agentLoadsMachine(this, loadingAgent);
    }

    public void ChangeProjectileEntityClient(int index)
    {
      this.Projectile = this.GameEntity.CollectObjectsWithTag<SynchedMissionObject>(this.ProjectileTag)[index];
      this._projectileIndex = index;
    }

    protected internal override void OnInit()
    {
      base.OnInit();
      DestructableComponent destructableComponent = this.GameEntity.GetScriptComponents<DestructableComponent>().FirstOrDefault<DestructableComponent>();
      if (destructableComponent != null)
        this._defaultSide = destructableComponent.BattleSide;
      this.ReleaseAngleRestrictionCenter = (float) (((double) this.TopReleaseAngleRestriction + (double) this.BottomReleaseAngleRestriction) * 0.5);
      this.ReleaseAngleRestrictionAngle = this.TopReleaseAngleRestriction - this.BottomReleaseAngleRestriction;
      this.currentReleaseAngle = this._lastSyncedReleaseAngle = this.ReleaseAngleRestrictionCenter;
      this.OriginalMissileItem = Game.Current.ObjectManager.GetObject<ItemObject>(this.MissileItemID);
      this.LoadedMissileItem = this.OriginalMissileItem;
      if (this.RotationObject == null)
        this.RotationObject = (SynchedMissionObject) this;
      this._rotationObjectInitialFrame = this.RotationObject.GameEntity.GetFrame();
      this._originalDirection = this.RotationObject.GameEntity.GetGlobalFrame().rotation.f;
      this._originalDirection.RotateAboutZ(3.141593f);
      this.currentDirection = this._lastSyncedDirection = 0.0f;
      this._syncTimer = 0.0f;
      List<GameEntity> gameEntityList = this.GameEntity.CollectChildrenEntitiesWithTag(this.CameraHolderTag);
      if (gameEntityList.Count > 0)
      {
        this.cameraHolder = gameEntityList[0];
        this.cameraHolderInitialFrame = this.cameraHolder.GetFrame();
        if (GameNetwork.IsClientOrReplay)
          this.MakeVisibilityCheck = false;
      }
      List<SynchedMissionObject> source = this.GameEntity.CollectObjectsWithTag<SynchedMissionObject>(this.ProjectileTag);
      foreach (ScriptComponentBehaviour componentBehaviour in source)
        componentBehaviour.GameEntity.SetVisibilityExcludeParents(false);
      this.Projectile = source.FirstOrDefault<SynchedMissionObject>((Func<SynchedMissionObject, bool>) (x => x.GameEntity.HasTag(this.MissileItemID)));
      this.Projectile.GameEntity.SetVisibilityExcludeParents(true);
      GameEntity gameEntity = this.GameEntity.GetChildren().FirstOrDefault<GameEntity>((Func<GameEntity, bool>) (x => x.Name == "clean"));
      this._missileStartingPositionEntityForSimulation = gameEntity != null ? gameEntity.GetChildren().FirstOrDefault<GameEntity>((Func<GameEntity, bool>) (x => x.Name == "projectile_leaving_position")) : (GameEntity) null;
      this.targetDirection = this.currentDirection;
      this.targetReleaseAngle = this.currentReleaseAngle;
      this.ReloadStandingPoints = new List<StandingPoint>();
      this.AmmoPickUpStandingPoints = new List<StandingPointWithWeaponRequirement>();
      if (this.StandingPoints != null)
      {
        foreach (StandingPoint standingPoint in this.StandingPoints)
        {
          standingPoint.AddComponent((UsableMissionObjectComponent) new ResetAnimationOnStopUsageComponent(ActionIndexCache.act_none));
          if (standingPoint.GameEntity.HasTag(this.ReloadTag))
            this.ReloadStandingPoints.Add(standingPoint);
        }
      }
      foreach (StandingPointWithWeaponRequirement weaponRequirement in this.StandingPoints.OfType<StandingPointWithWeaponRequirement>().ToList<StandingPointWithWeaponRequirement>())
      {
        if (weaponRequirement.GameEntity.HasTag(this.AmmoPickUpTag))
        {
          this.AmmoPickUpStandingPoints.Add(weaponRequirement);
          weaponRequirement.InitGivenWeapon(this.OriginalMissileItem);
        }
        else
        {
          this.LoadAmmoStandingPoint = weaponRequirement;
          this.LoadAmmoStandingPoint.InitRequiredWeaponClasses(this.OriginalMissileItem.PrimaryWeapon.WeaponClass);
        }
      }
      if (this.AmmoPickUpStandingPoints.Count > 1)
      {
        this._ammoPickupCenter = new Vec3();
        foreach (StandingPointWithWeaponRequirement pickUpStandingPoint in this.AmmoPickUpStandingPoints)
        {
          pickUpStandingPoint.SetHasAlternative(true);
          this._ammoPickupCenter += pickUpStandingPoint.GameEntity.GlobalPosition;
        }
        this._ammoPickupCenter /= (float) this.AmmoPickUpStandingPoints.Count;
      }
      else
        this._ammoPickupCenter = this.GameEntity.GlobalPosition;
      this.AmmoCount = this.startingAmmoCount;
      this.UpdateAmmoMesh();
      this.RegisterAnimationParameters();
      this.GetSoundEventIndices();
      this.InitAnimations();
      this.SetScriptComponentToTick(this.GetTickRequirement());
    }

    protected internal override void OnEditorInit()
    {
      List<SynchedMissionObject> synchedMissionObjectList = this.GameEntity.CollectObjectsWithTag<SynchedMissionObject>(this.ProjectileTag);
      if (synchedMissionObjectList.Count > 0)
        this.Projectile = synchedMissionObjectList[0];
      this.VisualizeReleaseTrajectoryAngle = this.TopReleaseAngleRestriction;
    }

    private void InitAnimations()
    {
      for (int index = 0; index < this.SkeletonOwnerObjects.Length; ++index)
      {
        this.SkeletonOwnerObjects[index].GameEntity.Skeleton.SetAnimationAtChannel(this.setUpAnimations[index], 0, blendInPeriod: 0.0f);
        this.SkeletonOwnerObjects[index].GameEntity.Skeleton.SetAnimationParameterAtChannel(0, 1f);
      }
    }

    protected internal override void OnMissionReset()
    {
      base.OnMissionReset();
      this.Projectile.GameEntity.SetVisibilityExcludeParents(true);
      this._state = RangedSiegeWeapon.WeaponState.Idle;
      this.currentDirection = this._lastSyncedDirection = 0.0f;
      this._syncTimer = 0.0f;
      this.currentReleaseAngle = this._lastSyncedReleaseAngle = this.ReleaseAngleRestrictionCenter;
      this.targetDirection = this.currentDirection;
      this.targetReleaseAngle = this.currentReleaseAngle;
      this.ApplyAimChange();
      this.AmmoCount = this.startingAmmoCount;
      this.UpdateAmmoMesh();
      if (this.MoveSound != null)
      {
        this.MoveSound.Stop();
        this.MoveSound = (SoundEvent) null;
      }
      this.hasFrameChangedInPreviousFrame = false;
      for (int index = 0; index < this.SkeletonOwnerObjects.Length; ++index)
        this.SkeletonOwnerObjects[index].GameEntity.ResumeSkeletonAnimation();
      foreach (UsableMissionObject standingPoint in this.StandingPoints)
        standingPoint.IsDeactivated = false;
      foreach (UsableMissionObject pickUpStandingPoint in this.AmmoPickUpStandingPoints)
        pickUpStandingPoint.IsDeactivated = false;
      this.InitAnimations();
    }

    public override bool ReadFromNetwork()
    {
      bool bufferReadValid = base.ReadFromNetwork();
      int num1 = GameNetworkMessage.ReadIntFromPacket(CompressionMission.RangedSiegeWeaponStateCompressionInfo, ref bufferReadValid);
      float num2 = GameNetworkMessage.ReadFloatFromPacket(CompressionBasic.RadianCompressionInfo, ref bufferReadValid);
      float num3 = GameNetworkMessage.ReadFloatFromPacket(CompressionBasic.RadianCompressionInfo, ref bufferReadValid);
      int num4 = GameNetworkMessage.ReadIntFromPacket(CompressionMission.RangedSiegeWeaponAmmoCompressionInfo, ref bufferReadValid);
      int index = GameNetworkMessage.ReadIntFromPacket(CompressionMission.RangedSiegeWeaponAmmoIndexCompressionInfo, ref bufferReadValid);
      if (bufferReadValid)
      {
        this._state = (RangedSiegeWeapon.WeaponState) num1;
        this.targetDirection = num2;
        this.targetReleaseAngle = MBMath.ClampFloat(num3, this.BottomReleaseAngleRestriction, this.TopReleaseAngleRestriction);
        this.AmmoCount = num4;
        this.currentDirection = this.targetDirection;
        this.currentReleaseAngle = this.targetReleaseAngle;
        this.currentDirection = this.targetDirection;
        this.currentReleaseAngle = this.targetReleaseAngle;
        this.ApplyCurrentDirectionToEntity();
        this.CheckAmmo();
        this.UpdateAmmoMesh();
        this.ChangeProjectileEntityClient(index);
      }
      return bufferReadValid;
    }

    public override void WriteToNetwork()
    {
      base.WriteToNetwork();
      GameNetworkMessage.WriteIntToPacket((int) this.State, CompressionMission.RangedSiegeWeaponStateCompressionInfo);
      GameNetworkMessage.WriteFloatToPacket(this.targetDirection, CompressionBasic.RadianCompressionInfo);
      GameNetworkMessage.WriteFloatToPacket(this.targetReleaseAngle, CompressionBasic.RadianCompressionInfo);
      GameNetworkMessage.WriteIntToPacket(this.AmmoCount, CompressionMission.RangedSiegeWeaponAmmoCompressionInfo);
      GameNetworkMessage.WriteIntToPacket(this._projectileIndex, CompressionMission.RangedSiegeWeaponAmmoIndexCompressionInfo);
    }

    public override bool IsInRangeToCheckAlternativePoints(Agent agent)
    {
      float num = this.AmmoPickUpStandingPoints.Count > 0 ? agent.GetInteractionDistanceToUsable((IUsable) this.AmmoPickUpStandingPoints[0]) + 2f : 2f;
      return (double) this._ammoPickupCenter.DistanceSquared(agent.Position) < (double) num * (double) num;
    }

    public override StandingPoint GetBestPointAlternativeTo(
      StandingPoint standingPoint,
      Agent agent)
    {
      if (!((IEnumerable<StandingPoint>) this.AmmoPickUpStandingPoints).Contains<StandingPoint>(standingPoint))
        return standingPoint;
      IEnumerable<StandingPointWithWeaponRequirement> weaponRequirements = this.AmmoPickUpStandingPoints.Where<StandingPointWithWeaponRequirement>((Func<StandingPointWithWeaponRequirement, bool>) (sp => !sp.IsDeactivated && (sp.IsInstantUse || !sp.HasUser && !sp.HasAIMovingTo) && !sp.IsDisabledForAgent(agent)));
      Vec3 globalPosition = standingPoint.GameEntity.GlobalPosition;
      float num1 = globalPosition.DistanceSquared(agent.Position);
      StandingPoint standingPoint1 = standingPoint;
      foreach (StandingPointWithWeaponRequirement weaponRequirement in weaponRequirements)
      {
        globalPosition = weaponRequirement.GameEntity.GlobalPosition;
        float num2 = globalPosition.DistanceSquared(agent.Position);
        if ((double) num2 < (double) num1)
        {
          num1 = num2;
          standingPoint1 = (StandingPoint) weaponRequirement;
        }
      }
      return standingPoint1;
    }

    protected virtual void OnRangedSiegeWeaponStateChange()
    {
      switch (this.State)
      {
        case RangedSiegeWeapon.WeaponState.Idle:
        case RangedSiegeWeapon.WeaponState.WaitingBeforeIdle:
          this.cameraState = this.cameraState != RangedSiegeWeapon.CameraState.FreeMove ? RangedSiegeWeapon.CameraState.StickToWeapon : RangedSiegeWeapon.CameraState.ApproachToCamera;
          break;
        case RangedSiegeWeapon.WeaponState.WaitingBeforeProjectileLeaving:
          this.AttackClickWillReload = this.WeaponNeedsClickToReload;
          break;
        case RangedSiegeWeapon.WeaponState.Shooting:
          if ((NativeObject) this.cameraHolder != (NativeObject) null)
          {
            this.cameraState = RangedSiegeWeapon.CameraState.DontMove;
            this.dontMoveTimer = 0.35f;
            break;
          }
          break;
        case RangedSiegeWeapon.WeaponState.WaitingAfterShooting:
          this.AttackClickWillReload = this.WeaponNeedsClickToReload;
          this.CheckAmmo();
          break;
        case RangedSiegeWeapon.WeaponState.WaitingBeforeReloading:
          this.AttackClickWillReload = false;
          if ((NativeObject) this.cameraHolder != (NativeObject) null)
            this.cameraState = RangedSiegeWeapon.CameraState.MoveDownToReload;
          this.CheckAmmo();
          break;
        case RangedSiegeWeapon.WeaponState.LoadingAmmo:
          if (this.ReloadSound != null && this.ReloadSound.IsValid)
            this.ReloadSound.Stop();
          this.ReloadSound = (SoundEvent) null;
          break;
        case RangedSiegeWeapon.WeaponState.Reloading:
          if (this.ReloadSound != null && this.ReloadSound.IsValid)
          {
            if (this.ReloadSound.IsPaused())
            {
              this.ReloadSound.Resume();
              break;
            }
            this.ReloadSound.PlayInPosition(this.GameEntity.GetGlobalFrame().origin);
            break;
          }
          this.ReloadSound = SoundEvent.CreateEvent(this.ReloadSoundIndex, this.Scene);
          this.ReloadSound.PlayInPosition(this.GameEntity.GetGlobalFrame().origin);
          break;
        case RangedSiegeWeapon.WeaponState.ReloadingPaused:
          if (this.ReloadSound != null && this.ReloadSound.IsValid)
          {
            this.ReloadSound.Pause();
            break;
          }
          break;
      }
      if (GameNetwork.IsClientOrReplay)
        return;
      switch (this.State)
      {
        case RangedSiegeWeapon.WeaponState.WaitingBeforeProjectileLeaving:
          for (int index = 0; index < this.SkeletonOwnerObjects.Length; ++index)
            this.SkeletonOwnerObjects[index].SetAnimationAtChannelSynched(this.fireAnimations[index], 0);
          break;
        case RangedSiegeWeapon.WeaponState.Shooting:
          this.ShootProjectile();
          break;
        case RangedSiegeWeapon.WeaponState.Reloading:
          for (int index = 0; index < this.SkeletonOwnerObjects.Length; ++index)
          {
            if (this.SkeletonOwnerObjects[index].GameEntity.IsSkeletonAnimationPaused())
              this.SkeletonOwnerObjects[index].ResumeSkeletonAnimationSynched();
            else
              this.SkeletonOwnerObjects[index].SetAnimationAtChannelSynched(this.setUpAnimations[index], 0);
          }
          this._currentReloaderCount = 1;
          break;
        case RangedSiegeWeapon.WeaponState.ReloadingPaused:
          for (int index = 0; index < this.SkeletonOwnerObjects.Length; ++index)
            this.SkeletonOwnerObjects[index].PauseSkeletonAnimationSynched();
          break;
      }
    }

    protected override ScriptComponentBehaviour.TickRequirement GetTickRequirement() => this.GameEntity.IsVisibleIncludeParents() ? ScriptComponentBehaviour.TickRequirement.Tick : base.GetTickRequirement();

    protected internal override void OnTick(float dt)
    {
      base.OnTick(dt);
      if (!this.GameEntity.IsVisibleIncludeParents())
        return;
      if (!GameNetwork.IsClientOrReplay)
      {
        this.UpdateState(dt);
        if (this.PilotAgent != null && this.PilotAgent.MovementFlags.HasAnyFlag<Agent.MovementControlFlag>(Agent.MovementControlFlag.AttackMask))
        {
          if (this.State == RangedSiegeWeapon.WeaponState.Idle)
            this.Shoot();
          else if (this.State == RangedSiegeWeapon.WeaponState.WaitingAfterShooting && this.AttackClickWillReload)
            this.ManualReload();
        }
      }
      this.HandleUserAiming(dt);
    }

    public void ToggleTrajectoryVisibility(bool isVisible)
    {
      if (isVisible)
      {
        this.VisualizeReleaseTrajectoryAngle = this.TopReleaseAngleRestriction;
        if (this._editorVisualizer == null)
        {
          this.VisualizeReleaseTrajectoryAngle = this.TopReleaseAngleRestriction;
          this._editorVisualizer = new TrajectoryVisualizer(this.GameEntity.Scene);
          this._editorVisualizer.Init(this.ProjectileEntityCurrentGlobalPosition, this.GetBallisticDirectionForVisualization() * this.ShootingSpeed, 6f, 200f);
        }
        this._editorVisualizer.SetVisible(true);
        this._editorVisualizer.Update(this.ProjectileEntityCurrentGlobalPosition, this.GetBallisticDirectionForVisualization() * this.ShootingSpeed, 6f, 200f, (ItemObject) null);
      }
      else
      {
        if (this._editorVisualizer == null)
          return;
        this._editorVisualizer.SetVisible(false);
        this._editorVisualizer = (TrajectoryVisualizer) null;
      }
    }

    protected internal override void OnEditorTick(float dt)
    {
      if (MBEditor.IsEntitySelected(this.GameEntity))
      {
        if (this._editorVisualizer == null && this.RotationObject != null)
        {
          this._editorVisualizer = new TrajectoryVisualizer(this.GameEntity.Scene);
          this._editorVisualizer.Init(this.ProjectileEntityCurrentGlobalPosition, this.GetBallisticDirectionForVisualization() * this.ShootingSpeed, 6f, 200f);
        }
        if (this._editorVisualizer == null)
          return;
        this._editorVisualizer.SetVisible(true);
        this._editorVisualizer.Update(this.ProjectileEntityCurrentGlobalPosition, this.GetBallisticDirectionForVisualization() * this.ShootingSpeed, 6f, 200f, (ItemObject) null);
      }
      else
      {
        if (this._editorVisualizer == null)
          return;
        this._editorVisualizer.SetVisible(false);
      }
    }

    protected override void OnRemoved(int removeReason)
    {
      base.OnRemoved(removeReason);
      if (!MBEditor.IsEditModeOn || this._editorVisualizer == null)
        return;
      this._editorVisualizer.Clear();
      this._editorVisualizer = (TrajectoryVisualizer) null;
    }

    protected virtual float CalculateShootingRange(float heightDifference) => Mission.GetMissileRange(this.ShootingSpeed, heightDifference);

    protected static bool ApproachToAngle(
      ref float angle,
      float angleToApproach,
      bool isMouse,
      float speed_limit,
      float dt,
      float sensitivity)
    {
      if ((double) angle == (double) angleToApproach)
        return false;
      float val1 = sensitivity * dt;
      float num = Math.Abs(angle - angleToApproach);
      if (isMouse)
        val1 *= Math.Max(num * 8f, 0.15f);
      if ((double) speed_limit > 0.0)
        val1 = Math.Min(val1, speed_limit * dt);
      if ((double) num <= (double) val1)
        angle = angleToApproach;
      else
        angle += val1 * (float) Math.Sign(angleToApproach - angle);
      return true;
    }

    protected virtual void HandleUserAiming(float dt)
    {
      bool flag1 = false;
      float horizontalAimSensitivity = this.HorizontalAimSensitivity;
      float verticalAimSensitivity = this.VerticalAimSensitivity;
      if (this.cameraState != RangedSiegeWeapon.CameraState.DontMove)
      {
        if (this._inputGiven)
        {
          if (this.CanRotate())
          {
            if ((double) this._inputX != 0.0)
            {
              this.targetDirection += horizontalAimSensitivity * dt * this._inputX;
              this.targetDirection = MBMath.WrapAngle(this.targetDirection);
              this.targetDirection = MBMath.ClampAngle(this.targetDirection, this.currentDirection, 0.7f);
              this.targetDirection = MBMath.ClampAngle(this.targetDirection, 0.0f, this.DirectionRestriction);
            }
            if ((double) this._inputY != 0.0)
            {
              this.targetReleaseAngle += verticalAimSensitivity * dt * this._inputY;
              this.targetReleaseAngle = MBMath.ClampAngle(this.targetReleaseAngle, this.currentReleaseAngle + 0.05f, 0.6f);
              this.targetReleaseAngle = MBMath.ClampAngle(this.targetReleaseAngle, this.ReleaseAngleRestrictionCenter, this.ReleaseAngleRestrictionAngle);
            }
          }
          this._inputGiven = false;
          this._inputX = 0.0f;
          this._inputY = 0.0f;
        }
        else if (this._exactInputGiven)
        {
          bool flag2 = false;
          if (this.CanRotate())
          {
            if ((double) this.targetDirection != (double) this._inputTargetX)
            {
              float num = horizontalAimSensitivity * dt;
              if ((double) MBMath.Absf(this.targetDirection - this._inputTargetX) < (double) num)
                this.targetDirection = this._inputTargetX;
              else if ((double) this.targetDirection < (double) this._inputTargetX)
              {
                this.targetDirection += num;
                flag2 = true;
              }
              else
              {
                this.targetDirection -= num;
                flag2 = true;
              }
              this.targetDirection = MBMath.WrapAngle(this.targetDirection);
              this.targetDirection = MBMath.ClampAngle(this.targetDirection, this.currentDirection, 0.7f);
              this.targetDirection = MBMath.ClampAngle(this.targetDirection, 0.0f, this.DirectionRestriction);
            }
            if ((double) this.targetReleaseAngle != (double) this._inputTargetY)
            {
              float num = verticalAimSensitivity * dt;
              if ((double) MBMath.Absf(this.targetReleaseAngle - this._inputTargetY) < (double) num)
                this.targetReleaseAngle = this._inputTargetY;
              else if ((double) this.targetReleaseAngle < (double) this._inputTargetY)
              {
                this.targetReleaseAngle += num;
                flag2 = true;
              }
              else
              {
                this.targetReleaseAngle -= num;
                flag2 = true;
              }
              this.targetReleaseAngle = MBMath.ClampAngle(this.targetReleaseAngle, this.currentReleaseAngle + 0.05f, 0.6f);
              this.targetReleaseAngle = MBMath.ClampAngle(this.targetReleaseAngle, this.ReleaseAngleRestrictionCenter, this.ReleaseAngleRestrictionAngle);
            }
          }
          else
            flag2 = true;
          if (!flag2)
            this._exactInputGiven = false;
        }
      }
      if (this.cameraState == RangedSiegeWeapon.CameraState.StickToWeapon)
      {
        bool flag2 = RangedSiegeWeapon.ApproachToAngle(ref this.currentDirection, this.targetDirection, this.UsesMouseForAiming, -1f, dt, horizontalAimSensitivity) | flag1;
        flag1 = RangedSiegeWeapon.ApproachToAngle(ref this.currentReleaseAngle, this.targetReleaseAngle, this.UsesMouseForAiming, -1f, dt, verticalAimSensitivity) | flag2;
        this.cameraDirection = this.currentDirection;
        this.cameraReleaseAngle = this.currentReleaseAngle;
      }
      else if (this.cameraState == RangedSiegeWeapon.CameraState.DontMove)
      {
        this.dontMoveTimer -= dt;
        if ((double) this.dontMoveTimer < 0.0)
        {
          if (!this.AttackClickWillReload)
          {
            this.cameraState = RangedSiegeWeapon.CameraState.MoveDownToReload;
            this.maxRotateSpeed = 0.0f;
            this.reloadTargetReleaseAngle = (double) Math.Abs(this.currentReleaseAngle) <= 0.174532920122147 ? this.currentReleaseAngle : 0.0f;
            this.reloadTargetReleaseAngle = MBMath.ClampAngle(this.reloadTargetReleaseAngle, this.currentReleaseAngle - 0.05f, 0.6f);
            this.targetDirection = this.cameraDirection;
            this.cameraReleaseAngle = this.targetReleaseAngle;
          }
          else
            this.cameraState = RangedSiegeWeapon.CameraState.StickToWeapon;
        }
      }
      else if (this.cameraState == RangedSiegeWeapon.CameraState.MoveDownToReload)
      {
        this.maxRotateSpeed += dt * 1.2f;
        this.maxRotateSpeed = Math.Min(this.maxRotateSpeed, 1f);
        bool flag2 = RangedSiegeWeapon.ApproachToAngle(ref this.currentReleaseAngle, this.reloadTargetReleaseAngle, this.UsesMouseForAiming, 0.4f + this.maxRotateSpeed, dt, verticalAimSensitivity) | flag1;
        bool flag3 = RangedSiegeWeapon.ApproachToAngle(ref this.cameraDirection, this.targetDirection, this.UsesMouseForAiming, -1f, dt, horizontalAimSensitivity) | flag2;
        this.targetReleaseAngle = Math.Min(this.targetReleaseAngle, this.reloadTargetReleaseAngle + 0.35f);
        this.targetReleaseAngle = Math.Max(this.targetReleaseAngle, this.reloadTargetReleaseAngle - 0.25f);
        flag1 = RangedSiegeWeapon.ApproachToAngle(ref this.cameraReleaseAngle, this.targetReleaseAngle, this.UsesMouseForAiming, 0.5f + this.maxRotateSpeed, dt, verticalAimSensitivity) | flag3;
        if (!flag1)
        {
          this.cameraState = RangedSiegeWeapon.CameraState.FreeMove;
          RangedSiegeWeapon.OnSiegeWeaponReloadDone onReloadDone = this.OnReloadDone;
          if (onReloadDone != null)
            onReloadDone();
        }
      }
      else if (this.cameraState == RangedSiegeWeapon.CameraState.FreeMove)
      {
        bool flag2 = RangedSiegeWeapon.ApproachToAngle(ref this.cameraDirection, this.targetDirection, this.UsesMouseForAiming, -1f, dt, horizontalAimSensitivity) | flag1;
        flag1 = RangedSiegeWeapon.ApproachToAngle(ref this.cameraReleaseAngle, this.targetReleaseAngle, this.UsesMouseForAiming, -1f, dt, verticalAimSensitivity) | flag2;
        this.maxRotateSpeed = 0.0f;
      }
      else if (this.cameraState == RangedSiegeWeapon.CameraState.ApproachToCamera)
      {
        this.maxRotateSpeed += (float) (0.899999976158142 * (double) dt + (double) this.maxRotateSpeed * 2.0 * (double) dt);
        bool flag2 = RangedSiegeWeapon.ApproachToAngle(ref this.cameraDirection, this.targetDirection, this.UsesMouseForAiming, -1f, dt, horizontalAimSensitivity) | flag1;
        bool flag3 = RangedSiegeWeapon.ApproachToAngle(ref this.cameraReleaseAngle, this.targetReleaseAngle, this.UsesMouseForAiming, -1f, dt, verticalAimSensitivity) | flag2;
        bool flag4 = RangedSiegeWeapon.ApproachToAngle(ref this.currentDirection, this.targetDirection, this.UsesMouseForAiming, this.maxRotateSpeed, dt, horizontalAimSensitivity) | flag3;
        flag1 = RangedSiegeWeapon.ApproachToAngle(ref this.currentReleaseAngle, this.targetReleaseAngle, this.UsesMouseForAiming, this.maxRotateSpeed, dt, verticalAimSensitivity) | flag4;
        if (!flag1)
          this.cameraState = RangedSiegeWeapon.CameraState.StickToWeapon;
      }
      if ((NativeObject) this.cameraHolder != (NativeObject) null)
      {
        MatrixFrame frame = this.cameraHolderInitialFrame;
        frame.rotation.RotateAboutForward(this.cameraDirection - this.currentDirection);
        frame.rotation.RotateAboutSide(this.cameraReleaseAngle - this.currentReleaseAngle);
        this.cameraHolder.SetFrame(ref frame);
        frame = this.cameraHolder.GetGlobalFrame();
        frame.rotation.s.z = 0.0f;
        double num1 = (double) frame.rotation.s.Normalize();
        frame.rotation.u = Vec3.CrossProduct(frame.rotation.s, frame.rotation.f);
        double num2 = (double) frame.rotation.u.Normalize();
        frame.rotation.f = Vec3.CrossProduct(frame.rotation.u, frame.rotation.s);
        double num3 = (double) frame.rotation.f.Normalize();
        this.cameraHolder.SetGlobalFrame(frame);
      }
      if (flag1 && !this.hasFrameChangedInPreviousFrame)
        this.OnRotationStarted();
      else if (!flag1 && this.hasFrameChangedInPreviousFrame)
        this.OnRotationStopped();
      this.hasFrameChangedInPreviousFrame = flag1;
      if (flag1 && GameNetwork.IsClient && this.PilotAgent == Agent.Main || GameNetwork.IsServerOrRecorder)
      {
        float num = !GameNetwork.IsClient || this.PilotAgent != Agent.Main ? 0.02f : 0.0001f;
        if ((double) this._syncTimer > 0.200000002980232 && ((double) MBMath.Absf(this.currentDirection - this._lastSyncedDirection) > (double) num || (double) MBMath.Absf(this.currentReleaseAngle - this._lastSyncedReleaseAngle) > (double) num))
        {
          this._lastSyncedDirection = this.currentDirection;
          this._lastSyncedReleaseAngle = this.currentReleaseAngle;
          if (GameNetwork.IsClient && this.PilotAgent == Agent.Main)
          {
            GameNetwork.BeginModuleEventAsClient();
            GameNetwork.WriteMessage((GameNetworkMessage) new SetMachineRotation((UsableMachine) this, this.currentDirection, this.currentReleaseAngle));
            GameNetwork.EndModuleEventAsClient();
          }
          else if (GameNetwork.IsServerOrRecorder)
          {
            GameNetwork.BeginBroadcastModuleEvent();
            GameNetwork.WriteMessage((GameNetworkMessage) new SetMachineTargetRotation((UsableMachine) this, this.currentDirection, this.currentReleaseAngle));
            Agent pilotAgent = this.PilotAgent;
            NetworkCommunicator targetPlayer;
            if (pilotAgent == null)
            {
              targetPlayer = (NetworkCommunicator) null;
            }
            else
            {
              MissionPeer missionPeer = pilotAgent.MissionPeer;
              targetPlayer = missionPeer != null ? missionPeer.GetNetworkPeer() : (NetworkCommunicator) null;
            }
            GameNetwork.EndBroadcastModuleEvent(GameNetwork.EventBroadcastFlags.ExcludeTargetPlayer | GameNetwork.EventBroadcastFlags.AddToMissionRecord, targetPlayer);
          }
        }
      }
      this._syncTimer += dt;
      if ((double) this._syncTimer >= 1.0)
        --this._syncTimer;
      if (!flag1)
        return;
      this.ApplyAimChange();
    }

    public void GiveInput(float inputX, float inputY)
    {
      this._exactInputGiven = false;
      this._inputGiven = true;
      this._inputX = inputX;
      this._inputY = inputY;
      this._inputX = MBMath.ClampFloat(this._inputX, -1f, 1f);
      this._inputY = MBMath.ClampFloat(this._inputY, -1f, 1f);
    }

    public void GiveExactInput(float targetX, float targetY)
    {
      this._exactInputGiven = true;
      this._inputGiven = false;
      this._inputTargetX = MBMath.ClampAngle(targetX, 0.0f, this.DirectionRestriction);
      this._inputTargetY = MBMath.ClampAngle(targetY, this.ReleaseAngleRestrictionCenter, this.ReleaseAngleRestrictionAngle);
    }

    protected virtual bool CanRotate() => this.State == RangedSiegeWeapon.WeaponState.Idle;

    protected virtual void ApplyAimChange()
    {
      if (this.CanRotate())
      {
        this.ApplyCurrentDirectionToEntity();
      }
      else
      {
        this.targetDirection = this.currentDirection;
        this.targetReleaseAngle = this.currentReleaseAngle;
      }
    }

    private void ApplyCurrentDirectionToEntity()
    {
      MatrixFrame objectInitialFrame = this._rotationObjectInitialFrame;
      objectInitialFrame.rotation.RotateAboutUp(this.currentDirection);
      this.RotationObject.GameEntity.SetFrame(ref objectInitialFrame);
    }

    public virtual float GetTargetDirection(Vec3 target)
    {
      MatrixFrame globalFrame = this.GameEntity.GetGlobalFrame();
      globalFrame.rotation.RotateAboutUp(3.141593f);
      return globalFrame.TransformToLocal(target).AsVec2.RotationInRadians;
    }

    public virtual float GetTargetReleaseAngle(Vec3 target)
    {
      WeaponStatsData statsDataForUsage = new MissionWeapon(this.OriginalMissileItem, (ItemModifier) null, (Banner) null).GetWeaponStatsDataForUsage(0);
      return Mission.GetMissileVerticalAimCorrection(target - this.MissleStartingPositionForSimulation, this.ShootingSpeed, ref statsDataForUsage, ItemObject.GetAirFrictionConstant(this.OriginalMissileItem.PrimaryWeapon.WeaponClass));
    }

    private static void RotateTowardsTarget(
      ref float current,
      float target,
      float delta,
      out bool hasRotated)
    {
      float betweenTwoAngles = MBMath.GetSmallestDifferenceBetweenTwoAngles(current, target);
      if ((double) Math.Abs(betweenTwoAngles) > 1.0 / 1000.0)
      {
        hasRotated = true;
        if ((double) betweenTwoAngles > 0.0)
        {
          current += Math.Min(delta, betweenTwoAngles);
          if ((double) current <= 3.14159274101257)
            return;
          current -= 6.283185f;
        }
        else
        {
          current += Math.Max(delta * -1f, betweenTwoAngles);
          if ((double) current >= -3.14159274101257)
            return;
          current += 6.283185f;
        }
      }
      else
        hasRotated = false;
    }

    public virtual bool AimAtTarget(Vec3 target)
    {
      float targetDirection = this.GetTargetDirection(target);
      float targetReleaseAngle = this.GetTargetReleaseAngle(target);
      float targetX = MBMath.ClampAngle(targetDirection, 0.0f, this.DirectionRestriction);
      float targetY = MBMath.ClampAngle(targetReleaseAngle, this.ReleaseAngleRestrictionCenter, this.ReleaseAngleRestrictionAngle);
      if (!this._exactInputGiven || (double) targetX != (double) this._inputTargetX || (double) targetY != (double) this._inputTargetY)
        this.GiveExactInput(targetX, targetY);
      return (double) Math.Abs(this.currentDirection - this._inputTargetX) < 1.0 / 1000.0 && (double) Math.Abs(this.currentReleaseAngle - this._inputTargetY) < 1.0 / 1000.0;
    }

    public virtual void AimAtRotation(float horizontalRotation, float verticalRotation)
    {
      horizontalRotation = MBMath.ClampFloat(horizontalRotation, -3.141593f, 3.141593f);
      verticalRotation = MBMath.ClampFloat(verticalRotation, -3.141593f, 3.141593f);
      horizontalRotation = MBMath.ClampAngle(horizontalRotation, 0.0f, this.DirectionRestriction);
      verticalRotation = MBMath.ClampAngle(verticalRotation, this.ReleaseAngleRestrictionCenter, this.ReleaseAngleRestrictionAngle);
      if (this._exactInputGiven && (double) horizontalRotation == (double) this._inputTargetX && (double) verticalRotation == (double) this._inputTargetY)
        return;
      this.GiveExactInput(horizontalRotation, verticalRotation);
    }

    private void UpdateState(float dt)
    {
      if (this.LoadAmmoStandingPoint != null)
      {
        this.LoadAmmoStandingPoint.SetIsDeactivatedSynched(this.IsDeactivated || this.State != RangedSiegeWeapon.WeaponState.LoadingAmmo);
        if (this.State != RangedSiegeWeapon.WeaponState.LoadingAmmo)
        {
          foreach (UsableMissionObject reloadStandingPoint in this.ReloadStandingPoints)
            reloadStandingPoint.SetIsDeactivatedSynched(this.IsDeactivated);
        }
        else
        {
          StandingPoint standingPointOfWhichUserRunsForAmmo = !this.ReloadStandingPoints.Any<StandingPoint>((Func<StandingPoint, bool>) (rsp =>
          {
            if (rsp == this.PilotStandingPoint)
              return false;
            return !rsp.HasUser || rsp.UserAgent.IsAIControlled;
          })) ? this.ReloadStandingPoints.FirstOrDefault<StandingPoint>((Func<StandingPoint, bool>) (rsp => !rsp.HasUser || rsp.UserAgent.IsAIControlled)) : this.ReloadStandingPoints.FirstOrDefault<StandingPoint>((Func<StandingPoint, bool>) (rsp =>
          {
            if (rsp == this.PilotStandingPoint)
              return false;
            return !rsp.HasUser || rsp.UserAgent.IsAIControlled;
          }));
          foreach (UsableMissionObject usableMissionObject in this.ReloadStandingPoints.Where<StandingPoint>((Func<StandingPoint, bool>) (rsp => rsp != standingPointOfWhichUserRunsForAmmo)))
            usableMissionObject.SetIsDeactivatedSynched(this.IsDeactivated);
          if (standingPointOfWhichUserRunsForAmmo != null)
          {
            if (this.HasAmmo)
              standingPointOfWhichUserRunsForAmmo.SetIsDeactivatedSynched(true);
            else
              this._isDisabledForAI = true;
          }
        }
      }
      switch (this.State)
      {
        case RangedSiegeWeapon.WeaponState.WaitingBeforeProjectileLeaving:
          this.animationTimeElapsed += dt;
          if ((double) this.animationTimeElapsed < (double) this.timeGapBetweenShootActionAndProjectileLeaving)
            break;
          this.State = RangedSiegeWeapon.WeaponState.Shooting;
          break;
        case RangedSiegeWeapon.WeaponState.Shooting:
          for (int index = 0; index < this.SkeletonOwnerObjects.Length; ++index)
          {
            string animationAtChannel = this.SkeletonOwnerObjects[index].GameEntity.Skeleton.GetAnimationAtChannel(0);
            float parameterAtChannel = this.SkeletonOwnerObjects[index].GameEntity.Skeleton.GetAnimationParameterAtChannel(0);
            string fireAnimation = this.fireAnimations[index];
            if (animationAtChannel == fireAnimation && (double) parameterAtChannel >= 0.999899983406067)
            {
              this.State = !this.AttackClickWillReload ? RangedSiegeWeapon.WeaponState.WaitingBeforeReloading : RangedSiegeWeapon.WeaponState.WaitingAfterShooting;
              this.animationTimeElapsed = 0.0f;
            }
          }
          break;
        case RangedSiegeWeapon.WeaponState.WaitingBeforeReloading:
          this.animationTimeElapsed += dt;
          if ((double) this.animationTimeElapsed < (double) this.timeGapBetweenShootingEndAndReloadingStart || this.cameraState != RangedSiegeWeapon.CameraState.FreeMove && this.cameraState != RangedSiegeWeapon.CameraState.StickToWeapon && !((NativeObject) this.cameraHolder == (NativeObject) null))
            break;
          if (this.ReloadStandingPoints.Count == 0)
          {
            this.State = RangedSiegeWeapon.WeaponState.Reloading;
            break;
          }
          if (!this.ReloadStandingPoints.Any<StandingPoint>((Func<StandingPoint, bool>) (reloadStandingPoint => reloadStandingPoint.HasUser)))
            break;
          this.State = RangedSiegeWeapon.WeaponState.Reloading;
          break;
        case RangedSiegeWeapon.WeaponState.Reloading:
          int num = 0;
          if (this.ReloadStandingPoints.Count == 0)
          {
            if (this.PilotAgent != null)
              num = 1;
          }
          else
          {
            foreach (UsableMissionObject reloadStandingPoint in this.ReloadStandingPoints)
            {
              if (reloadStandingPoint.HasUser)
                ++num;
            }
          }
          if (num == 0)
          {
            this.State = RangedSiegeWeapon.WeaponState.ReloadingPaused;
            break;
          }
          if (this._currentReloaderCount != num)
          {
            this._currentReloaderCount = num;
            float animationSpeed = (float) Math.Sqrt((double) this._currentReloaderCount);
            for (int index = 0; index < this.SkeletonOwnerObjects.Length; ++index)
            {
              float parameterAtChannel = this.SkeletonOwnerObjects[index].GameEntity.Skeleton.GetAnimationParameterAtChannel(0);
              this.SkeletonOwnerObjects[index].SetAnimationAtChannelSynched(this.setUpAnimations[index], 0, animationSpeed);
              if ((double) parameterAtChannel > 0.0)
                this.SkeletonOwnerObjects[index].SetAnimationChannelParameterSynched(0, parameterAtChannel);
            }
          }
          for (int index = 0; index < this.SkeletonOwnerObjects.Length; ++index)
          {
            string animationAtChannel = this.SkeletonOwnerObjects[index].GameEntity.Skeleton.GetAnimationAtChannel(0);
            float parameterAtChannel = this.SkeletonOwnerObjects[index].GameEntity.Skeleton.GetAnimationParameterAtChannel(0);
            string setUpAnimation = this.setUpAnimations[index];
            if (animationAtChannel == setUpAnimation && (double) parameterAtChannel >= 0.999899983406067)
            {
              this.State = RangedSiegeWeapon.WeaponState.LoadingAmmo;
              this.animationTimeElapsed = 0.0f;
            }
          }
          break;
        case RangedSiegeWeapon.WeaponState.ReloadingPaused:
          if (this.ReloadStandingPoints.Count == 0)
          {
            if (this.PilotAgent == null)
              break;
            this.State = RangedSiegeWeapon.WeaponState.Reloading;
            break;
          }
          using (List<StandingPoint>.Enumerator enumerator = this.ReloadStandingPoints.GetEnumerator())
          {
            while (enumerator.MoveNext())
            {
              if (enumerator.Current.HasUser)
              {
                this.State = RangedSiegeWeapon.WeaponState.Reloading;
                break;
              }
            }
            break;
          }
      }
    }

    public bool Shoot()
    {
      this._lastShooterAgent = this.PilotAgent;
      if (this.State != RangedSiegeWeapon.WeaponState.Idle)
        return false;
      this.State = RangedSiegeWeapon.WeaponState.WaitingBeforeProjectileLeaving;
      if (!GameNetwork.IsClientOrReplay)
        this.animationTimeElapsed = 0.0f;
      return true;
    }

    public void ManualReload()
    {
      if (!this.AttackClickWillReload)
        return;
      this.State = RangedSiegeWeapon.WeaponState.WaitingBeforeReloading;
    }

    protected abstract float ShootingSpeed { get; }

    protected virtual float MaximumBallisticError => 1f;

    private Vec3 GetBallisticErrorAppliedDirection(float BallisticErrorAmount)
    {
      Mat3 mat3 = new Mat3()
      {
        f = this.ShootingDirection,
        u = Vec3.Up
      };
      mat3.Orthonormalize();
      float a = MBRandom.RandomFloat * 6.283185f;
      mat3.RotateAboutForward(a);
      float f = BallisticErrorAmount * MBRandom.RandomFloat;
      mat3.RotateAboutSide(f.ToRadians());
      return mat3.f;
    }

    private Vec3 GetBallisticDirectionForVisualization()
    {
      Mat3 mat3 = new Mat3()
      {
        f = this.VisualizationShootingDirection,
        u = Vec3.Up
      };
      mat3.Orthonormalize();
      return mat3.f;
    }

    private void ShootProjectile()
    {
      if (this.LoadedMissileItem.StringId == "grapeshot_fire_stack")
      {
        ItemObject missileItem = Game.Current.ObjectManager.GetObject<ItemObject>("grapeshot_fire_projectile");
        for (int index = 0; index < 5; ++index)
          this.ShootProjectileAux(missileItem, true);
      }
      else
        this.ShootProjectileAux(this.LoadedMissileItem, false);
      this._lastShooterAgent = (Agent) null;
    }

    private void ShootProjectileAux(ItemObject missileItem, bool randomizeMissileSpeed)
    {
      Mat3 identity = Mat3.Identity;
      float shootingSpeed = this.ShootingSpeed;
      if (randomizeMissileSpeed)
      {
        shootingSpeed *= MBRandom.RandomFloatRanged(0.9f, 1.1f);
        identity.f = this.GetBallisticErrorAppliedDirection(2.5f);
        identity.Orthonormalize();
      }
      else
      {
        identity.f = this.GetBallisticErrorAppliedDirection(this.MaximumBallisticError);
        identity.Orthonormalize();
      }
      Mission.Current.AddCustomMissile(this._lastShooterAgent, new MissionWeapon(missileItem, (ItemModifier) null, this._lastShooterAgent.Origin?.Banner, (short) 1), this.ProjectileEntityCurrentGlobalPosition, identity.f, identity, (float) this.LoadedMissileItem.PrimaryWeapon.MissileSpeed, shootingSpeed, false, (MissionObject) this);
    }

    protected virtual Vec3 ShootingDirection => this.Projectile.GameEntity.GetGlobalFrame().rotation.u;

    protected virtual Vec3 VisualizationShootingDirection => this.Projectile.GameEntity.GetGlobalFrame().rotation.u;

    public virtual Vec3 ProjectileEntityCurrentGlobalPosition => this.Projectile.GameEntity.GetGlobalFrame().origin;

    protected void OnRotationStarted()
    {
      if (this.MoveSound != null && this.MoveSound.IsValid)
        return;
      this.MoveSound = SoundEvent.CreateEvent(this.MoveSoundIndex, this.Scene);
      this.MoveSound.PlayInPosition(this.RotationObject.GameEntity.GlobalPosition);
    }

    protected void OnRotationStopped()
    {
      this.MoveSound.Stop();
      this.MoveSound = (SoundEvent) null;
    }

    public abstract override SiegeEngineType GetSiegeEngineType();

    public override BattleSideEnum Side => this.PilotAgent != null ? this.PilotAgent.Team.Side : this._defaultSide;

    public bool CanShootAtBox(Vec3 boxMin, Vec3 boxMax, uint attempts = 5)
    {
      Vec3 vec3;
      Vec3 v1 = vec3 = (boxMin + boxMax) / 2f;
      v1.z = boxMin.z;
      Vec3 v2 = vec3;
      v2.z = boxMax.z;
      uint num = attempts;
      while (!this.CanShootAtPoint(Vec3.Lerp(v1, v2, (float) num / (float) attempts)))
      {
        --num;
        if (num <= 0U)
          return false;
      }
      return true;
    }

    public bool CanShootAtPoint(Vec3 target)
    {
      float num1 = 10f;
      float shootingRange = this.CalculateShootingRange(-(target - this.ProjectileEntityCurrentGlobalPosition).z);
      float num2 = this.ProjectileEntityCurrentGlobalPosition.AsVec2.Distance(target.AsVec2);
      if ((double) num2 < (double) num1 || (double) num2 > (double) shootingRange || (double) this.DirectionRestriction / 2.0 - (double) Math.Abs((target.AsVec2 - this.ProjectileEntityCurrentGlobalPosition.AsVec2).Normalized().AngleBetween(this.OriginalDirection.AsVec2.Normalized())) < 0.0)
        return false;
      double num3 = (double) (target - this.ProjectileEntityCurrentGlobalPosition).Normalize();
      return this.Scene.CheckPointCanSeePoint(this.ProjectileEntityCurrentGlobalPosition + new Vec3(z: 1.411f), target);
    }

    internal virtual bool IsTargetValid(ITargetable target) => true;

    public override OrderType GetOrder(BattleSideEnum side) => this.Side == side ? OrderType.Use : OrderType.AttackEntity;

    protected override GameEntity GetEntityToAttachNavMeshFaces() => this.RotationObject.GameEntity;

    public abstract float ProcessTargetValue(float baseValue, TargetFlags flags);

    protected virtual void UpdateAmmoMesh()
    {
      GameEntity gameEntity = this.AmmoPickUpStandingPoints[0].GameEntity;
      int num = 20 - this.AmmoCount;
      for (; (NativeObject) gameEntity.Parent != (NativeObject) null; gameEntity = gameEntity.Parent)
      {
        for (int metaMeshIndex = 0; metaMeshIndex < gameEntity.MultiMeshComponentCount; ++metaMeshIndex)
        {
          MetaMesh metaMesh = gameEntity.GetMetaMesh(metaMeshIndex);
          for (int meshIndex = 0; meshIndex < metaMesh.MeshCount; ++meshIndex)
            metaMesh.GetMeshAtIndex(meshIndex).SetVectorArgument(0.0f, (float) num, 0.0f, 0.0f);
        }
      }
    }

    public enum WeaponState
    {
      Idle,
      WaitingBeforeProjectileLeaving,
      Shooting,
      WaitingAfterShooting,
      WaitingBeforeReloading,
      LoadingAmmo,
      WaitingBeforeIdle,
      Reloading,
      ReloadingPaused,
      NumberOfStates,
    }

    public enum FiringFocus
    {
      Troops,
      Walls,
      RangedSiegeWeapons,
      PrimarySiegeWeapons,
    }

    public delegate void OnSiegeWeaponReloadDone();

    public enum CameraState
    {
      StickToWeapon,
      DontMove,
      MoveDownToReload,
      FreeMove,
      ApproachToCamera,
    }
  }
}
