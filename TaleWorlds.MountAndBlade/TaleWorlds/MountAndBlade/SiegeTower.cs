// Decompiled with JetBrains decompiler
// Type: TaleWorlds.MountAndBlade.SiegeTower
// Assembly: TaleWorlds.MountAndBlade, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3C0E3F42-756F-4407-B817-06BA412A78B0
// Assembly location: E:\SteamLibrary\steamapps\common\Mount & Blade II Bannerlord\bin\Win64_Shipping_Client\TaleWorlds.MountAndBlade.dll

using NetworkMessages.FromServer;
using System;
using System.Collections.Generic;
using System.Linq;
using TaleWorlds.Core;
using TaleWorlds.DotNet;
using TaleWorlds.Engine;
using TaleWorlds.Library;
using TaleWorlds.Localization;
using TaleWorlds.MountAndBlade.Network.Messages;
using TaleWorlds.MountAndBlade.Objects.Siege;

namespace TaleWorlds.MountAndBlade
{
  public class SiegeTower : 
    SiegeWeapon,
    IPathHolder,
    IPrimarySiegeWeapon,
    IMoveableSiegeWeapon,
    ISpawnable
  {
    private const string BreakableWallTag = "breakable_wall";
    private const string DestroyedWallTag = "destroyed";
    private const string NonDestroyedWallTag = "non_destroyed";
    private const string LadderTag = "ladder";
    private const string BattlementDestroyedParticleTag = "particle_spawnpoint";
    private const string CleanStateTag = "operational";
    private string _pathEntityName;
    public string GateTag = "gate";
    public string GateOpenTag = "gateOpen";
    public string HandleTag = "handle";
    public string GateHandleIdleAnimation = "siegetower_handle_idle";
    public string GateTrembleAnimation = "siegetower_door_stop";
    public string BattlementDestroyedParticle = "psys_adobe_battlement_destroyed";
    private string _targetWallSegmentTag;
    public bool GhostEntityMove = true;
    public float GhostEntitySpeedMultiplier = 1f;
    private string _sideTag;
    public float WheelDiameter = 1.3f;
    public float MinSpeed = 0.5f;
    public float MaxSpeed = 1f;
    public int GateNavMeshId;
    public int NavMeshIdToDisableOnDestination = -1;
    private int _soilNavMeshID1;
    private int _soilNavMeshID2;
    private int _ditchNavMeshID1;
    private int _ditchNavMeshID2;
    private int _groundToSoilNavMeshID1;
    private int _groundToSoilNavMeshID2;
    private int _soilGenericNavMeshID;
    private int _groundGenericNavMeshID;
    public string BarrierTagToRemove = "barrier";
    private List<GameEntity> _aiBarriers;
    private bool _isGhostMovementOn;
    private bool _hasArrivedAtTarget;
    private SiegeTower.GateState _state;
    private SynchedMissionObject _gateObject;
    private SynchedMissionObject _handleObject;
    private SoundEvent _gateOpenSound;
    private int _gateOpenSoundIndex = -1;
    private Mat3 _openStateRotation;
    private Mat3 _closedStateRotation;
    private float _fallAngularSpeed;
    private float _lastDotProductOfAnimationAndTargetRotation;
    private GameEntity _cleanState;
    private GameEntity _destroyedWallEntity;
    private GameEntity _nonDestroyedWallEntity;
    private GameEntity _battlementDestroyedParticle;
    private StandingPoint _gateStandingPoint;
    private MatrixFrame _gateStandingPointLocalIKFrame;
    private SynchedMissionObject _ditchFillDebris;
    private List<LadderQueueManager> _queueManagers;
    private FormationAI.BehaviorSide _weaponSide;
    private WallSegment _targetWallSegment;
    private List<SiegeLadder> _sameSideSiegeLadders;

    public MissionObject TargetCastlePosition => (MissionObject) this._targetWallSegment;

    private GameEntity CleanState
    {
      get => (NativeObject) this._cleanState == (NativeObject) null ? this.GameEntity : this._cleanState;
      set => this._cleanState = value;
    }

    public FormationAI.BehaviorSide WeaponSide => this._weaponSide;

    public string PathEntity => this._pathEntityName;

    public bool EditorGhostEntityMove => this.GhostEntityMove;

    public bool HasCompletedAction() => this.IsDeactivated && !this.IsDestroyed;

    public float SiegeWeaponPriority => 15f;

    public int OverTheWallNavMeshID => this.GetGateNavMeshId();

    public bool HoldLadders => !this.MovementComponent.HasArrivedAtTarget;

    public bool SendLadders => this.MovementComponent.HasArrivedAtTarget;

    public int GetGateNavMeshId()
    {
      if (this.GateNavMeshId != 0)
        return this.GateNavMeshId;
      return this.DynamicNavmeshIdStart == 0 ? 0 : this.DynamicNavmeshIdStart + 3;
    }

    public bool HasArrivedAtTarget
    {
      get => this._hasArrivedAtTarget;
      set
      {
        if (!GameNetwork.IsClientOrReplay)
          this.MovementComponent.SetDestinationNavMeshIdState(!this.HasArrivedAtTarget);
        if (this._hasArrivedAtTarget == value)
          return;
        this._hasArrivedAtTarget = value;
        if (this._hasArrivedAtTarget)
        {
          this.ActiveWaitStandingPoint = this.WaitStandingPoints[1];
          if (!GameNetwork.IsClientOrReplay)
          {
            foreach (LadderQueueManager queueManager in this._queueManagers)
            {
              this.CleanState.Scene.SetAbilityOfFacesWithId(queueManager.ManagedNavigationFaceId, true);
              queueManager.IsDeactivated = false;
            }
          }
        }
        else if (!GameNetwork.IsClientOrReplay && this.GetGateNavMeshId() > 0)
          this.CleanState.Scene.SetAbilityOfFacesWithId(this.GetGateNavMeshId(), false);
        if (GameNetwork.IsServerOrRecorder)
        {
          GameNetwork.BeginBroadcastModuleEvent();
          GameNetwork.WriteMessage((GameNetworkMessage) new SetSiegeTowerHasArrivedAtTarget(this));
          GameNetwork.EndBroadcastModuleEvent(GameNetwork.EventBroadcastFlags.AddToMissionRecord);
        }
        else
        {
          if (!GameNetwork.IsClientOrReplay)
            return;
          this.MovementComponent.MoveToTargetAsClient();
        }
      }
    }

    public SiegeTower.GateState State
    {
      get => this._state;
      set
      {
        if (this._state == value)
          return;
        if (GameNetwork.IsServerOrRecorder)
        {
          GameNetwork.BeginBroadcastModuleEvent();
          GameNetwork.WriteMessage((GameNetworkMessage) new SetSiegeTowerGateState(this, value));
          GameNetwork.EndBroadcastModuleEvent(GameNetwork.EventBroadcastFlags.AddToMissionRecord);
        }
        this._state = value;
        this.OnSiegeTowerGateStateChange();
      }
    }

    public SiegeWeaponMovementComponent MovementComponent { get; private set; }

    public override string GetDescriptionText(GameEntity gameEntity = null) => (NativeObject) gameEntity == (NativeObject) null || !gameEntity.HasScriptOfType<UsableMissionObject>() || gameEntity.HasTag("move") ? new TextObject("{=aXjlMBiE}Siege Tower").ToString() : new TextObject("{=6wZUG0ev}Gate").ToString();

    public override TextObject GetActionTextForStandingPoint(
      UsableMissionObject usableGameObject)
    {
      TextObject textObject = !usableGameObject.GameEntity.HasTag("move") ? new TextObject("{=5oozsaIb}({KEY}) Open") : new TextObject("{=rwZAZSvX}({KEY}) Move");
      textObject.SetTextVariable("KEY", Game.Current.GameTextManager.GetHotKeyGameText("CombatHotKeyCategory", 13));
      return textObject;
    }

    public override void WriteToNetwork()
    {
      base.WriteToNetwork();
      GameNetworkMessage.WriteBoolToPacket(this.HasArrivedAtTarget);
      GameNetworkMessage.WriteIntToPacket((int) this.State, CompressionMission.SiegeTowerGateStateCompressionInfo);
      GameNetworkMessage.WriteFloatToPacket(this._fallAngularSpeed, CompressionMission.SiegeMachineComponentAngularSpeedCompressionInfo);
    }

    public override bool ReadFromNetwork()
    {
      bool bufferReadValid = base.ReadFromNetwork();
      bool flag = GameNetworkMessage.ReadBoolFromPacket(ref bufferReadValid);
      int num1 = GameNetworkMessage.ReadIntFromPacket(CompressionMission.SiegeTowerGateStateCompressionInfo, ref bufferReadValid);
      float num2 = GameNetworkMessage.ReadFloatFromPacket(CompressionMission.SiegeMachineComponentAngularSpeedCompressionInfo, ref bufferReadValid);
      if (bufferReadValid)
      {
        this.HasArrivedAtTarget = flag;
        this._state = (SiegeTower.GateState) num1;
        this._fallAngularSpeed = num2;
        this._lastDotProductOfAnimationAndTargetRotation = -1000f;
        if (this._state == SiegeTower.GateState.Open)
        {
          if ((NativeObject) this._destroyedWallEntity != (NativeObject) null && (NativeObject) this._nonDestroyedWallEntity != (NativeObject) null)
          {
            this._nonDestroyedWallEntity.SetVisibilityExcludeParents(false);
            this._destroyedWallEntity.SetVisibilityExcludeParents(true);
          }
          MatrixFrame frame = this._gateObject.GameEntity.GetFrame();
          frame.rotation = this._openStateRotation;
          this._gateObject.GameEntity.SetFrame(ref frame);
        }
      }
      return bufferReadValid;
    }

    public override OrderType GetOrder(BattleSideEnum side)
    {
      if (side != BattleSideEnum.Attacker)
        return OrderType.AttackEntity;
      return this.HasCompletedAction() ? OrderType.Use : OrderType.FollowEntity;
    }

    public override TargetFlags GetTargetFlags()
    {
      TargetFlags targetFlags1 = TargetFlags.None;
      if (this.UserCount > 0)
        targetFlags1 |= TargetFlags.IsMoving;
      TargetFlags targetFlags2 = targetFlags1 | TargetFlags.IsFlammable | TargetFlags.IsSiegeEngine | TargetFlags.IsAttacker;
      if (this.HasCompletedAction() || this.IsDestroyed || this.IsDeactivated)
        targetFlags2 |= TargetFlags.NotAThreat;
      if (this.Side == BattleSideEnum.Attacker && DebugSiegeBehaviour.DebugDefendState == DebugSiegeBehaviour.DebugStateDefender.DebugDefendersToTower)
        targetFlags2 |= TargetFlags.DebugThreat;
      return targetFlags2;
    }

    public override float GetTargetValue(List<Vec3> weaponPos) => 90f * this.GetUserMultiplierOfWeapon() * this.GetDistanceMultiplierOfWeapon(weaponPos[0]) * this.GetHitpointMultiplierofWeapon();

    public override void Disable()
    {
      base.Disable();
      this.SetAbilityOfFaces(false);
      if (this._queueManagers == null)
        return;
      foreach (LadderQueueManager queueManager in this._queueManagers)
      {
        this.CleanState.Scene.SetAbilityOfFacesWithId(queueManager.ManagedNavigationFaceId, false);
        queueManager.IsDeactivated = true;
      }
    }

    public override SiegeEngineType GetSiegeEngineType() => DefaultSiegeEngineTypes.SiegeTower;

    public override UsableMachineAIBase CreateAIBehaviourObject() => (UsableMachineAIBase) new SiegeTowerAI(this);

    public override bool IsDeactivated => this.MovementComponent.HasArrivedAtTarget && this.State == SiegeTower.GateState.Open || base.IsDeactivated;

    internal override void OnDeploymentStateChanged(bool isDeployed)
    {
      base.OnDeploymentStateChanged(isDeployed);
      if (this._ditchFillDebris != null)
      {
        if (!GameNetwork.IsClientOrReplay)
          this._ditchFillDebris.SetVisibleSynched(isDeployed);
        if (!GameNetwork.IsClientOrReplay)
        {
          if (isDeployed)
          {
            if (this._soilGenericNavMeshID > 0)
              Mission.Current.Scene.SetAbilityOfFacesWithId(this._soilGenericNavMeshID, true);
            if (this._soilNavMeshID1 > 0 && this._groundToSoilNavMeshID1 > 0 && this._ditchNavMeshID1 > 0)
            {
              Mission.Current.Scene.SetAbilityOfFacesWithId(this._soilNavMeshID1, true);
              Mission.Current.Scene.SwapFaceConnectionsWithID(this._groundToSoilNavMeshID1, this._ditchNavMeshID1, this._soilNavMeshID1);
            }
            if (this._soilNavMeshID2 > 0 && this._groundToSoilNavMeshID2 > 0 && this._ditchNavMeshID2 > 0)
            {
              Mission.Current.Scene.SetAbilityOfFacesWithId(this._soilNavMeshID2, true);
              Mission.Current.Scene.SwapFaceConnectionsWithID(this._groundToSoilNavMeshID2, this._ditchNavMeshID2, this._soilNavMeshID2);
            }
            if (this._groundGenericNavMeshID > 0)
              Mission.Current.Scene.SetAbilityOfFacesWithId(this._groundGenericNavMeshID, false);
          }
          else
          {
            if (this._groundGenericNavMeshID > 0)
              Mission.Current.Scene.SetAbilityOfFacesWithId(this._groundGenericNavMeshID, true);
            if (this._soilNavMeshID1 > 0 && this._groundToSoilNavMeshID1 > 0 && this._ditchNavMeshID1 > 0)
            {
              Mission.Current.Scene.SwapFaceConnectionsWithID(this._groundToSoilNavMeshID1, this._soilNavMeshID1, this._ditchNavMeshID1);
              Mission.Current.Scene.SetAbilityOfFacesWithId(this._soilNavMeshID1, false);
            }
            if (this._soilNavMeshID2 > 0 && this._groundToSoilNavMeshID2 > 0 && this._ditchNavMeshID2 > 0)
            {
              Mission.Current.Scene.SwapFaceConnectionsWithID(this._groundToSoilNavMeshID2, this._soilNavMeshID2, this._ditchNavMeshID2);
              Mission.Current.Scene.SetAbilityOfFacesWithId(this._soilNavMeshID2, false);
            }
            if (this._soilGenericNavMeshID > 0)
              Mission.Current.Scene.SetAbilityOfFacesWithId(this._soilGenericNavMeshID, false);
          }
        }
      }
      if (this._sameSideSiegeLadders == null)
        this._sameSideSiegeLadders = Mission.Current.ActiveMissionObjects.FindAllWithType<SiegeLadder>().Where<SiegeLadder>((Func<SiegeLadder, bool>) (sl => sl.WeaponSide == this.WeaponSide)).ToList<SiegeLadder>();
      foreach (ScriptComponentBehaviour sameSideSiegeLadder in this._sameSideSiegeLadders)
        sameSideSiegeLadder.GameEntity.SetVisibilityExcludeParents(!isDeployed);
    }

    protected override void AttachDynamicNavmeshToEntity()
    {
      if (this.NavMeshPrefabName.Length <= 0)
        return;
      this.DynamicNavmeshIdStart = Mission.Current.GetNextDynamicNavMeshIdStart();
      this.CleanState.Scene.ImportNavigationMeshPrefab(this.NavMeshPrefabName, this.DynamicNavmeshIdStart);
      this.GetEntityToAttachNavMeshFaces().AttachNavigationMeshFaces(this.DynamicNavmeshIdStart + 1, false);
      this.GetEntityToAttachNavMeshFaces().AttachNavigationMeshFaces(this.DynamicNavmeshIdStart + 2, true);
      this.GetEntityToAttachNavMeshFaces().AttachNavigationMeshFaces(this.DynamicNavmeshIdStart + 4, false, true);
    }

    protected override GameEntity GetEntityToAttachNavMeshFaces() => this.CleanState;

    protected override void OnRemoved(int removeReason)
    {
      base.OnRemoved(removeReason);
      this.MovementComponent?.OnRemoved();
    }

    public override void SetAbilityOfFaces(bool enabled)
    {
      base.SetAbilityOfFaces(enabled);
      if (this._queueManagers == null)
        return;
      foreach (LadderQueueManager queueManager in this._queueManagers)
      {
        this.CleanState.Scene.SetAbilityOfFacesWithId(queueManager.ManagedNavigationFaceId, enabled);
        queueManager.IsDeactivated = !enabled;
      }
    }

    protected override float GetDistanceMultiplierOfWeapon(Vec3 weaponPos)
    {
      float betweenPositions = this.GetMinimumDistanceBetweenPositions(weaponPos);
      if ((double) betweenPositions < 10.0)
        return 1f;
      return (double) betweenPositions < 25.0 ? 0.8f : 0.6f;
    }

    protected internal override void OnInit()
    {
      this.CleanState = this.GameEntity.GetFirstChildEntityWithTag("body");
      base.OnInit();
      this.DestructionComponent.OnDestroyed += new DestructableComponent.OnHitTakenAndDestroyedDelegate(this.OnDestroyed);
      this.DestructionComponent.BattleSide = BattleSideEnum.Attacker;
      this._aiBarriers = this.Scene.FindEntitiesWithTag(this.BarrierTagToRemove).ToList<GameEntity>();
      if (!GameNetwork.IsClientOrReplay && this._soilGenericNavMeshID > 0)
        this.CleanState.Scene.SetAbilityOfFacesWithId(this._soilGenericNavMeshID, false);
      List<SynchedMissionObject> source1 = this.CleanState.CollectObjectsWithTag<SynchedMissionObject>(this.GateTag);
      if (!source1.IsEmpty<SynchedMissionObject>())
        this._gateObject = source1.First<SynchedMissionObject>();
      this.AddRegularMovementComponent();
      List<GameEntity> list = this.Scene.FindEntitiesWithTag("breakable_wall").ToList<GameEntity>();
      if (!list.IsEmpty<GameEntity>())
      {
        float num = 1E+07f;
        GameEntity entity = (GameEntity) null;
        MatrixFrame targetFrame = this.MovementComponent.GetTargetFrame();
        foreach (GameEntity gameEntity in list)
        {
          float lengthSquared = (gameEntity.GlobalPosition - targetFrame.origin).LengthSquared;
          if ((double) lengthSquared < (double) num)
          {
            num = lengthSquared;
            entity = gameEntity;
          }
        }
        List<GameEntity> source2 = entity.CollectChildrenEntitiesWithTag("destroyed");
        if (!source2.IsEmpty<GameEntity>())
          this._destroyedWallEntity = source2.First<GameEntity>();
        List<GameEntity> source3 = entity.CollectChildrenEntitiesWithTag("non_destroyed");
        if (!source3.IsEmpty<GameEntity>())
          this._nonDestroyedWallEntity = source3.First<GameEntity>();
        List<GameEntity> source4 = entity.CollectChildrenEntitiesWithTag("particle_spawnpoint");
        if (!source4.IsEmpty<GameEntity>())
          this._battlementDestroyedParticle = source4.First<GameEntity>();
      }
      List<SynchedMissionObject> source5 = this.CleanState.CollectObjectsWithTag<SynchedMissionObject>(this.HandleTag);
      this._handleObject = source5.Count >= 1 ? source5.First<SynchedMissionObject>() : (SynchedMissionObject) null;
      this._queueManagers = new List<LadderQueueManager>();
      if (!GameNetwork.IsClientOrReplay)
      {
        List<GameEntity> source2 = this.CleanState.CollectChildrenEntitiesWithTag("ladder");
        if (source2.Count > 0)
        {
          LadderQueueManager ladderQueueManager = source2.ElementAt<GameEntity>(source2.Count / 2).GetScriptComponents<LadderQueueManager>().FirstOrDefault<LadderQueueManager>();
          if (ladderQueueManager != null)
          {
            MatrixFrame identity = MatrixFrame.Identity;
            identity.rotation.RotateAboutSide(1.570796f);
            identity.rotation.RotateAboutForward(0.3926991f);
            ladderQueueManager.Initialize(this.DynamicNavmeshIdStart + 1, identity, new Vec3(z: 1f), BattleSideEnum.Attacker, source2.Count * 5, 0.7853982f, 2f, 1f, 2f, 3f, false, 0.8f, 4f, 5f);
            this._queueManagers.Add(ladderQueueManager);
          }
        }
        else
        {
          LadderQueueManager ladderQueueManager = this.CleanState.GetScriptComponents<LadderQueueManager>().FirstOrDefault<LadderQueueManager>();
          if (ladderQueueManager != null)
          {
            MatrixFrame identity = MatrixFrame.Identity;
            identity.origin.y += 4f;
            identity.rotation.RotateAboutSide(-1.570796f);
            identity.rotation.RotateAboutUp(3.141593f);
            ladderQueueManager.Initialize(this.DynamicNavmeshIdStart + 2, identity, new Vec3(y: -1f), BattleSideEnum.Attacker, 15, 0.7853982f, 2f, 1f, 2f, 1f, false, 0.8f, 0.0f, 5f);
            this._queueManagers.Add(ladderQueueManager);
          }
        }
      }
      this._state = SiegeTower.GateState.Closed;
      this._gateOpenSoundIndex = SoundEvent.GetEventIdFromString("event:/mission/siege/siegetower/dooropen");
      this._closedStateRotation = this._gateObject.GameEntity.GetFrame().rotation;
      foreach (StandingPoint standingPoint in this.StandingPoints)
      {
        standingPoint.AddComponent((UsableMissionObjectComponent) new ResetAnimationOnStopUsageComponent(ActionIndexCache.act_none));
        if (!standingPoint.GameEntity.HasTag("move"))
        {
          this._gateStandingPoint = standingPoint;
          standingPoint.IsDeactivated = true;
          this._gateStandingPointLocalIKFrame = standingPoint.GameEntity.GetGlobalFrame().TransformToLocal(this.CleanState.GetGlobalFrame());
          standingPoint.AddComponent((UsableMissionObjectComponent) new ClearHandInverseKinematicsOnStopUsageComponent());
        }
      }
      if ((double) this.WaitStandingPoints[0].GlobalPosition.z > (double) this.WaitStandingPoints[1].GlobalPosition.z)
      {
        GameEntity waitStandingPoint = this.WaitStandingPoints[0];
        this.WaitStandingPoints[0] = this.WaitStandingPoints[1];
        this.WaitStandingPoints[1] = waitStandingPoint;
        this.ActiveWaitStandingPoint = this.WaitStandingPoints[0];
      }
      IEnumerable<GameEntity> source6 = this.Scene.FindEntitiesWithTag(this._targetWallSegmentTag).ToList<GameEntity>().Where<GameEntity>((Func<GameEntity, bool>) (ewtwst => ewtwst.HasScriptOfType<WallSegment>()));
      if (!source6.IsEmpty<GameEntity>())
      {
        this._targetWallSegment = source6.First<GameEntity>().GetFirstScriptOfType<WallSegment>();
        this._targetWallSegment.AttackerSiegeWeapon = (IPrimarySiegeWeapon) this;
      }
      string sideTag = this._sideTag;
      this._weaponSide = sideTag == "left" ? FormationAI.BehaviorSide.Left : (sideTag == "middle" ? FormationAI.BehaviorSide.Middle : (sideTag == "right" ? FormationAI.BehaviorSide.Right : FormationAI.BehaviorSide.Middle));
      if (!GameNetwork.IsClientOrReplay)
      {
        if (this.GetGateNavMeshId() != 0)
          this.CleanState.Scene.SetAbilityOfFacesWithId(this.GetGateNavMeshId(), false);
        foreach (LadderQueueManager queueManager in this._queueManagers)
        {
          this.CleanState.Scene.SetAbilityOfFacesWithId(queueManager.ManagedNavigationFaceId, false);
          queueManager.IsDeactivated = true;
        }
      }
      GameEntity gameEntity1 = this.Scene.FindEntitiesWithTag("ditch_filler").FirstOrDefault<GameEntity>((Func<GameEntity, bool>) (df => df.HasTag(this._sideTag)));
      if ((NativeObject) gameEntity1 != (NativeObject) null)
        this._ditchFillDebris = gameEntity1.GetFirstScriptOfType<SynchedMissionObject>();
      if (!GameNetwork.IsClientOrReplay)
        this._gateObject.GameEntity.AttachNavigationMeshFaces(this.DynamicNavmeshIdStart + 3, true);
      this.SetScriptComponentToTick(this.GetTickRequirement());
    }

    protected override ScriptComponentBehaviour.TickRequirement GetTickRequirement() => this.GameEntity.IsVisibleIncludeParents() ? ScriptComponentBehaviour.TickRequirement.Tick : base.GetTickRequirement();

    protected internal override void OnTick(float dt)
    {
      base.OnTick(dt);
      if (!this.CleanState.IsVisibleIncludeParents())
        return;
      if (this._gateStandingPoint.HasUser)
        this._gateStandingPoint.UserAgent.SetHandInverseKinematicsFrameFromActionChannel(1, this._gateStandingPointLocalIKFrame, this.CleanState.GetGlobalFrame());
      if (!GameNetwork.IsClientOrReplay)
      {
        foreach (StandingPoint standingPoint in this.StandingPoints)
        {
          if (standingPoint.GameEntity.HasTag("move"))
            standingPoint.SetIsDeactivatedSynched(this.MovementComponent.HasArrivedAtTarget);
          else
            standingPoint.SetIsDeactivatedSynched(!this.MovementComponent.HasArrivedAtTarget || this.State == SiegeTower.GateState.GateFalling || this.State == SiegeTower.GateState.GateFallingWallDestroyed || this.State == SiegeTower.GateState.Open);
        }
      }
      if (!GameNetwork.IsClientOrReplay && this.MovementComponent.HasArrivedAtTarget && !this.HasArrivedAtTarget)
      {
        this.HasArrivedAtTarget = true;
        this.ActiveWaitStandingPoint = this.WaitStandingPoints[1];
      }
      if (!this.HasArrivedAtTarget)
        return;
      int userCount = this.UserCount;
      switch (this.State)
      {
        case SiegeTower.GateState.Closed:
          if (GameNetwork.IsClientOrReplay || userCount <= 0)
            break;
          this.State = SiegeTower.GateState.GateFalling;
          break;
        case SiegeTower.GateState.GateFalling:
          MatrixFrame frame1 = this._gateObject.GameEntity.GetFrame();
          frame1.rotation.RotateAboutSide(this._fallAngularSpeed * dt);
          this._gateObject.GameEntity.SetFrame(ref frame1);
          if ((double) Vec3.DotProduct(frame1.rotation.f, this._openStateRotation.f) > 0.975000023841858)
            this.State = SiegeTower.GateState.GateFallingWallDestroyed;
          this._fallAngularSpeed += dt * 2f * Math.Max(0.3f, 1f - frame1.rotation.u.z);
          break;
        case SiegeTower.GateState.GateFallingWallDestroyed:
          MatrixFrame frame2 = this._gateObject.GameEntity.GetFrame();
          frame2.rotation.RotateAboutSide(this._fallAngularSpeed * dt);
          this._gateObject.GameEntity.SetFrame(ref frame2);
          float num = Vec3.DotProduct(frame2.rotation.f, this._openStateRotation.f);
          if ((double) this._fallAngularSpeed > 0.0 && (double) num > 0.949999988079071 && (double) num < (double) this._lastDotProductOfAnimationAndTargetRotation)
          {
            frame2.rotation = this._openStateRotation;
            this._gateObject.GameEntity.SetFrame(ref frame2);
            this._gateObject.GameEntity.Skeleton.SetAnimationAtChannel(this.GateTrembleAnimation, 0);
            if (this._gateOpenSound != null)
              this._gateOpenSound.Stop();
            if (!GameNetwork.IsClientOrReplay)
              this.State = SiegeTower.GateState.Open;
          }
          this._fallAngularSpeed += dt * 3f * Math.Max(0.3f, 1f - frame2.rotation.u.z);
          this._lastDotProductOfAnimationAndTargetRotation = num;
          break;
      }
    }

    protected internal override void OnMissionReset()
    {
      base.OnMissionReset();
      if (!GameNetwork.IsClientOrReplay && this.GetGateNavMeshId() > 0)
        this.CleanState.Scene.SetAbilityOfFacesWithId(this.GetGateNavMeshId(), false);
      this._state = SiegeTower.GateState.Closed;
      this._hasArrivedAtTarget = false;
      MatrixFrame frame = this._gateObject.GameEntity.GetFrame();
      frame.rotation = this._closedStateRotation;
      if (this._handleObject != null)
        this._handleObject.GameEntity.Skeleton.SetAnimationAtChannel(-1, 0);
      this._gateObject.GameEntity.Skeleton.SetAnimationAtChannel(-1, 0);
      this._gateObject.GameEntity.SetFrame(ref frame);
      if ((NativeObject) this._destroyedWallEntity != (NativeObject) null && (NativeObject) this._nonDestroyedWallEntity != (NativeObject) null)
      {
        this._nonDestroyedWallEntity.SetVisibilityExcludeParents(false);
        this._destroyedWallEntity.SetVisibilityExcludeParents(true);
      }
      foreach (StandingPoint standingPoint in this.StandingPoints)
        standingPoint.IsDeactivated = !standingPoint.GameEntity.HasTag("move");
    }

    public void OnDestroyed(
      DestructableComponent destroyedComponent,
      Agent destroyerAgent,
      in MissionWeapon weapon,
      ScriptComponentBehaviour attackerScriptComponentBehaviour,
      int inflictedDamage)
    {
      bool burnAgents = false;
      if (weapon.CurrentUsageItem != null)
        burnAgents = weapon.CurrentUsageItem.WeaponFlags.HasFlag((Enum) WeaponFlags.Burning) && weapon.CurrentUsageItem.WeaponFlags.HasAnyFlag<WeaponFlags>(WeaponFlags.AffectsArea | WeaponFlags.AffectsAreaBig);
      Mission.Current.KillAgentsOnEntity(destroyedComponent.CurrentState, destroyerAgent, burnAgents);
      foreach (GameEntity aiBarrier in this._aiBarriers)
        aiBarrier.SetVisibilityExcludeParents(true);
    }

    public void HighlightPath() => this.MovementComponent.HighlightPath();

    public void SwitchGhostEntityMovementMode(bool isGhostEnabled)
    {
      if (isGhostEnabled)
      {
        if (!this._isGhostMovementOn)
        {
          this.RemoveComponent((UsableMissionObjectComponent) this.MovementComponent);
          this.GhostEntityMove = true;
          SiegeWeaponMovementComponent component = this.GetComponent<SiegeWeaponMovementComponent>();
          component.GhostEntitySpeedMultiplier *= 3f;
          component.SetGhostVisibility(true);
        }
        this._isGhostMovementOn = true;
      }
      else
      {
        if (this._isGhostMovementOn)
        {
          this.RemoveComponent((UsableMissionObjectComponent) this.MovementComponent);
          this.RemoveComponent((UsableMissionObjectComponent) this.GetComponent<PathLastNodeFixer>());
          this.AddRegularMovementComponent();
          this.MovementComponent.SetGhostVisibility(false);
        }
        this._isGhostMovementOn = false;
      }
    }

    public MatrixFrame GetInitialFrame() => this.MovementComponent != null ? this.MovementComponent.GetInitialFrame() : this.CleanState.GetGlobalFrame();

    private void OnSiegeTowerGateStateChange()
    {
      switch (this.State)
      {
        case SiegeTower.GateState.Closed:
          if (this._handleObject != null)
            this._handleObject.GameEntity.Skeleton.SetAnimationAtChannel(this.GateHandleIdleAnimation, 0);
          if (GameNetwork.IsClientOrReplay || this.GetGateNavMeshId() == 0)
            break;
          this.CleanState.Scene.SetAbilityOfFacesWithId(this.GetGateNavMeshId(), false);
          break;
        case SiegeTower.GateState.Open:
          if (this._gateObject.GameEntity.Skeleton.GetAnimationAtChannel(0) != this.GateTrembleAnimation)
          {
            MatrixFrame frame = this._gateObject.GameEntity.GetFrame();
            frame.rotation = this._openStateRotation;
            this._gateObject.GameEntity.SetFrame(ref frame);
            this._gateObject.GameEntity.Skeleton.SetAnimationAtChannel(this.GateTrembleAnimation, 0);
            if (this._gateOpenSound != null)
              this._gateOpenSound.Stop();
            Mission.Current.MakeSound(MiscSoundContainer.SoundCodeSiegeSiegetowerDoorland, this.CleanState.GlobalPosition, true, false, -1, -1);
            if (!GameNetwork.IsClientOrReplay && this.GetGateNavMeshId() != 0)
              this.CleanState.Scene.SetAbilityOfFacesWithId(this.GetGateNavMeshId(), true);
          }
          if (!GameNetwork.IsClientOrReplay)
            this.CleanState.Scene.SetAbilityOfFacesWithId(this.GetGateNavMeshId(), true);
          using (List<GameEntity>.Enumerator enumerator = this._aiBarriers.GetEnumerator())
          {
            while (enumerator.MoveNext())
              enumerator.Current.SetVisibilityExcludeParents(false);
            break;
          }
        case SiegeTower.GateState.GateFalling:
          this._fallAngularSpeed = 0.0f;
          this._gateOpenSound = SoundEvent.CreateEvent(this._gateOpenSoundIndex, this.Scene);
          this._gateOpenSound.PlayInPosition(this._gateObject.GameEntity.GlobalPosition);
          break;
        case SiegeTower.GateState.GateFallingWallDestroyed:
          if (!((NativeObject) this._destroyedWallEntity != (NativeObject) null) || !((NativeObject) this._nonDestroyedWallEntity != (NativeObject) null))
            break;
          this._fallAngularSpeed *= 0.1f;
          this._lastDotProductOfAnimationAndTargetRotation = -1000f;
          this._nonDestroyedWallEntity.SetVisibilityExcludeParents(false);
          this._destroyedWallEntity.SetVisibilityExcludeParents(true);
          if (!((NativeObject) this._battlementDestroyedParticle != (NativeObject) null))
            break;
          Mission.Current.AddParticleSystemBurstByName(this.BattlementDestroyedParticle, this._battlementDestroyedParticle.GetGlobalFrame(), false);
          break;
      }
    }

    private void AddRegularMovementComponent()
    {
      this.MovementComponent = new SiegeWeaponMovementComponent()
      {
        PathEntityName = this._pathEntityName,
        MinSpeed = this.MinSpeed,
        MaxSpeed = this.MaxSpeed,
        MainObject = (SynchedMissionObject) this,
        WheelDiameter = this.WheelDiameter,
        NavMeshIdToDisableOnDestination = this.NavMeshIdToDisableOnDestination,
        MovementSoundCodeID = SoundEvent.GetEventIdFromString("event:/mission/siege/siegetower/move"),
        GhostEntitySpeedMultiplier = this.GhostEntitySpeedMultiplier
      };
      this.AddComponent((UsableMissionObjectComponent) this.MovementComponent);
    }

    internal void SetUpGhostEntity()
    {
      this.AddComponent((UsableMissionObjectComponent) new PathLastNodeFixer()
      {
        PathHolder = (IPathHolder) this
      });
      this.MovementComponent = new SiegeWeaponMovementComponent()
      {
        PathEntityName = this._pathEntityName,
        MainObject = (SynchedMissionObject) this,
        GhostEntitySpeedMultiplier = this.GhostEntitySpeedMultiplier
      };
      this.AddComponent((UsableMissionObjectComponent) this.MovementComponent);
      this.MovementComponent.SetupGhostEntity();
    }

    private void UpdateGhostEntity()
    {
      List<GameEntity> source = this.CleanState.CollectChildrenEntitiesWithTag("ghost_object");
      if (source.IsEmpty<GameEntity>())
        return;
      GameEntity gameEntity = source.First<GameEntity>();
      if (gameEntity.ChildCount <= 0)
        return;
      this.GetComponent<SiegeWeaponMovementComponent>().GhostEntitySpeedMultiplier = this.GhostEntitySpeedMultiplier;
      GameEntity child = gameEntity.GetChild(0);
      MatrixFrame frame = child.GetFrame();
      child.SetFrame(ref frame);
    }

    public void SetSpawnedFromSpawner() => this._spawnedFromSpawner = true;

    public void AssignParametersFromSpawner(
      string pathEntityName,
      string targetWallSegment,
      string sideTag,
      int soilNavMeshID1,
      int soilNavMeshID2,
      int ditchNavMeshID1,
      int ditchNavMeshID2,
      int groundToSoilNavMeshID1,
      int groundToSoilNavMeshID2,
      int soilGenericNavMeshID,
      int groundGenericNavMeshID,
      Mat3 openStateRotation,
      string barrierTagToRemove)
    {
      this._pathEntityName = pathEntityName;
      this._targetWallSegmentTag = targetWallSegment;
      this._sideTag = sideTag;
      this._soilNavMeshID1 = soilNavMeshID1;
      this._soilNavMeshID2 = soilNavMeshID2;
      this._ditchNavMeshID1 = ditchNavMeshID1;
      this._ditchNavMeshID2 = ditchNavMeshID2;
      this._groundToSoilNavMeshID1 = groundToSoilNavMeshID1;
      this._groundToSoilNavMeshID2 = groundToSoilNavMeshID2;
      this._soilGenericNavMeshID = soilGenericNavMeshID;
      this._groundGenericNavMeshID = groundGenericNavMeshID;
      this._openStateRotation = openStateRotation;
      this.BarrierTagToRemove = barrierTagToRemove;
    }

    public enum GateState
    {
      Closed,
      Open,
      GateFalling,
      GateFallingWallDestroyed,
      NumberOfStates,
    }
  }
}
