// Decompiled with JetBrains decompiler
// Type: TaleWorlds.MountAndBlade.Agent
// Assembly: TaleWorlds.MountAndBlade, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3C0E3F42-756F-4407-B817-06BA412A78B0
// Assembly location: E:\SteamLibrary\steamapps\common\Mount & Blade II Bannerlord\bin\Win64_Shipping_Client\TaleWorlds.MountAndBlade.dll

using NetworkMessages.FromClient;
using NetworkMessages.FromServer;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using TaleWorlds.Core;
using TaleWorlds.DotNet;
using TaleWorlds.Engine;
using TaleWorlds.Library;
using TaleWorlds.Localization;
using TaleWorlds.MountAndBlade.Network.Messages;

namespace TaleWorlds.MountAndBlade
{
  public sealed class Agent : 
    DotNetObject,
    IAgent,
    IFocusable,
    IUsable,
    IFormationUnit,
    ITrackableBase
  {
    public const float BecomeTeenagerAge = 14f;
    public const float MaxFocusDistance = 10f;
    public const float MaxInteractionDistance = 3f;
    public const float MaxMountInteractionDistance = 1.75f;
    public const float DismountVelocityLimit = 0.5f;
    public const float HealthDyingThreshold = 1f;
    internal const float CachedAndFormationValuesUpdateTime = 0.5f;
    private readonly List<AgentComponent> _agentComponentList;
    private readonly Agent.CreationType _creationType;
    private Agent.ControllerType _agentControllerType = Agent.ControllerType.AI;
    private List<sbyte> _attachedWeaponBoneIndices;
    private Agent.AgentBoneMapArray _boneMap;
    private Agent _cachedMountAgent;
    private Agent _cachedRiderAgent;
    private BasicCharacterObject _character;
    private uint? _clothingColor1;
    private uint? _clothingColor2;
    private float _currentDiscipline;
    private float _defensiveness;
    private UIntPtr _positionPointer;
    private UIntPtr _pointer;
    private UIntPtr _flagsPointer;
    private UIntPtr _indexPointer;
    private UIntPtr _statePointer;
    private readonly Dictionary<System.Type, AgentComponent> _agentComponentsDict;
    private Agent _lookAgentCache;
    private readonly List<AgentController> _agentControllers;
    private readonly Timer _cachedAndFormationValuesUpdateTimer;
    private IDetachment _detachment;
    private readonly List<Agent.Hitter> _hitterList;
    private List<MissionWeapon> _attachedWeapons;
    private List<MatrixFrame> _attachedWeaponFrames;
    private float _health;
    private MissionPeer _missionPeer;
    private TextObject _name;
    private float _removalTime;
    private List<CompositeComponent> _synchedBodyComponents;
    public Action OnAgentMountedStateChanged;
    private Formation _formation;
    private bool _checkIfTargetFrameIsChanged;
    public Agent.AgentPropertiesModifiers _propertyModifiers;
    public MissionRepresentativeBase MissionRepresentative;
    private int _usedObjectPreferenceIndex = -1;
    private bool _wantsToYell;
    private float _yellTimer;
    public float LastDetachmentTickAgentTime;
    private Vec3 _lastSynchedTargetDirection;
    private Vec2 _lastSynchedTargetPosition;
    public Agent.OnMainAgentWieldedItemChangeDelegate OnMainAgentWieldedItemChange;
    public MissionPeer OwningAgentMissionPeer;
    private WeakReference<MBAgentVisuals> _visualsWeakRef = new WeakReference<MBAgentVisuals>((MBAgentVisuals) null);
    private ClothSimulatorComponent _capeClothSimulator;
    public Action OnAgentWieldedItemChange;
    internal float DetachmentWeight;

    public static Agent Main => Mission.Current?.MainAgent;

    public bool IsMainAgent => this == Agent.Main;

    public bool IsAIControlled => this.Controller == Agent.ControllerType.AI && !GameNetwork.IsClientOrReplay;

    public bool IsPlayerControlled => this.IsMine || this.MissionPeer != null;

    public bool IsMine => this.Controller == Agent.ControllerType.Player;

    public Monster Monster { get; }

    public BodyProperties BodyPropertiesValue { get; private set; }

    public string HorseCreationKey { get; private set; }

    public int BodyPropertiesSeed { get; internal set; }

    public float Age
    {
      get => this.BodyPropertiesValue.Age;
      internal set
      {
        double num = (double) value;
        BodyProperties bodyPropertiesValue = this.BodyPropertiesValue;
        double weight = (double) bodyPropertiesValue.Weight;
        bodyPropertiesValue = this.BodyPropertiesValue;
        double build = (double) bodyPropertiesValue.Build;
        this.BodyPropertiesValue = new BodyProperties(new DynamicBodyProperties((float) num, (float) weight, (float) build), this.BodyPropertiesValue.StaticProperties);
        this.BodyPropertiesValue = this.BodyPropertiesValue;
      }
    }

    public bool IsFemale { get; internal set; }

    public bool IsItemUseDisabled { get; set; }

    public UsableMissionObject CurrentlyUsedGameObject { get; private set; }

    public bool IsUsingGameObject => this.CurrentlyUsedGameObject != null;

    public Mission Mission { get; private set; }

    public int Index { get; }

    private UIntPtr Pointer => this._pointer;

    private UIntPtr FlagsPointer => this._flagsPointer;

    private UIntPtr PositionPointer => this._positionPointer;

    public Agent.AIStateFlag AIStateFlags
    {
      get => MBAPI.IMBAgent.GetAIStateFlags(this.GetPtr());
      set => MBAPI.IMBAgent.SetAIStateFlags(this.GetPtr(), value);
    }

    public float MaximumForwardUnlimitedSpeed => MBAPI.IMBAgent.GetMaximumForwardUnlimitedSpeed(this.GetPtr());

    public Vec2 MovementVelocity => MBAPI.IMBAgent.GetMovementVelocity(this.GetPtr());

    public float DestinationSpeed
    {
      set => MBAPI.IMBAgent.SetDestinationSpeed(this.GetPtr(), value);
    }

    public MBActionSet ActionSet => new MBActionSet(MBAPI.IMBAgent.GetActionSetNo(this.GetPtr()));

    public Vec3 Position => AgentHelper.GetAgentPosition(this.PositionPointer);

    public MatrixFrame Frame
    {
      get
      {
        MatrixFrame outFrame = new MatrixFrame();
        MBAPI.IMBAgent.GetRotationFrame(this.GetPtr(), ref outFrame);
        return outFrame;
      }
    }

    public Vec3 LookDirection
    {
      get => MBAPI.IMBAgent.GetLookDirection(this.GetPtr());
      set => MBAPI.IMBAgent.SetLookDirection(this.GetPtr(), value);
    }

    public Vec3 AverageVelocity => MBAPI.IMBAgent.GetAverageVelocity(this.GetPtr());

    public Vec3 Velocity => this.Frame.rotation.TransformToParent(new Vec3(MBAPI.IMBAgent.GetMovementVelocity(this.GetPtr())));

    public Agent.MovementControlFlag MovementFlags
    {
      get => (Agent.MovementControlFlag) MBAPI.IMBAgent.GetMovementFlags(this.GetPtr());
      set => MBAPI.IMBAgent.SetMovementFlags(this.GetPtr(), value);
    }

    public Vec2 MovementInputVector
    {
      get => MBAPI.IMBAgent.GetMovementInputVector(this.GetPtr());
      set => MBAPI.IMBAgent.SetMovementInputVector(this.GetPtr(), value);
    }

    public CapsuleData CollisionCapsule
    {
      get
      {
        CapsuleData capsuleData = new CapsuleData();
        MBAPI.IMBAgent.GetCollisionCapsule(this.GetPtr(), ref capsuleData);
        return capsuleData;
      }
    }

    public Vec3 CollisionCapsuleCenter
    {
      get
      {
        CapsuleData collisionCapsule = this.CollisionCapsule;
        return (collisionCapsule.GetBoxMax() + collisionCapsule.GetBoxMin()) * 0.5f;
      }
    }

    public float LastRangedHitTime { get; private set; } = float.MinValue;

    public float LastMeleeHitTime { get; private set; } = float.MinValue;

    public float LastRangedAttackTime { get; private set; } = float.MinValue;

    public float LastMeleeAttackTime { get; private set; } = float.MinValue;

    public Agent.AgentBoneMapArray BoneMappingArray
    {
      get
      {
        if (this._boneMap == null)
          this.UpdateBoneMapCache();
        return this._boneMap;
      }
    }

    public bool IsHuman => (this.GetAgentFlags() & AgentFlag.IsHumanoid) > AgentFlag.None;

    public MBAgentVisuals AgentVisuals
    {
      get
      {
        MBAgentVisuals target;
        if (!this._visualsWeakRef.TryGetTarget(out target))
        {
          target = MBAPI.IMBAgent.GetAgentVisuals(this.GetPtr());
          this._visualsWeakRef.SetTarget(target);
        }
        return target;
      }
    }

    public Agent.EventControlFlag EventControlFlags
    {
      get => (Agent.EventControlFlag) MBAPI.IMBAgent.GetEventControlFlags(this.GetPtr());
      set => MBAPI.IMBAgent.SetEventControlFlags(this.GetPtr(), value);
    }

    public float MovementDirectionAsAngle => MBAPI.IMBAgent.GetMovementDirectionAsAngle(this.GetPtr());

    public bool IsRunningAway { get; private set; }

    public bool IsMount => (this.GetAgentFlags() & AgentFlag.Mountable) > AgentFlag.None;

    public bool IsLookDirectionLocked
    {
      get => MBAPI.IMBAgent.GetIsLookDirectionLocked(this.GetPtr());
      set => MBAPI.IMBAgent.SetIsLookDirectionLocked(this.GetPtr(), value);
    }

    public IReadOnlyList<Agent.Hitter> HitterList => (IReadOnlyList<Agent.Hitter>) this._hitterList;

    public float AgentScale => MBAPI.IMBAgent.GetAgentScale(this.GetPtr());

    public bool CrouchMode => MBAPI.IMBAgent.GetCrouchMode(this.GetPtr());

    public bool WalkMode => MBAPI.IMBAgent.GetWalkMode(this.GetPtr());

    public Vec3 VisualPosition => MBAPI.IMBAgent.GetVisualPosition(this.GetPtr());

    public float LookDirectionAsAngle
    {
      get => MBAPI.IMBAgent.GetLookDirectionAsAngle(this.GetPtr());
      set => MBAPI.IMBAgent.SetLookDirectionAsAngle(this.GetPtr(), value);
    }

    public bool IsLookRotationInSlowMotion => MBAPI.IMBAgent.IsLookRotationInSlowMotion(this.GetPtr());

    public bool HeadCameraMode
    {
      get => MBAPI.IMBAgent.GetHeadCameraMode(this.GetPtr());
      set => MBAPI.IMBAgent.SetHeadCameraMode(this.GetPtr(), value);
    }

    public Agent.GuardMode CurrentGuardMode => MBAPI.IMBAgent.GetCurrentGuardMode(this.GetPtr());

    public Agent ImmediateEnemy => MBAPI.IMBAgent.GetImmediateEnemy(this.GetPtr());

    public bool IsDoingPassiveAttack => MBAPI.IMBAgent.GetIsDoingPassiveAttack(this.GetPtr());

    public bool IsPassiveUsageConditionsAreMet => MBAPI.IMBAgent.GetIsPassiveUsageConditionsAreMet(this.GetPtr());

    public float CurrentAimingError => MBAPI.IMBAgent.GetCurrentAimingError(this.GetPtr());

    public float CurrentAimingTurbulance => MBAPI.IMBAgent.GetCurrentAimingTurbulance(this.GetPtr());

    public Agent.UsageDirection AttackDirection => MBAPI.IMBAgent.GetAttackDirectionUsage(this.GetPtr());

    public float WalkingSpeedLimitOfMountable => MBAPI.IMBAgent.GetWalkSpeedLimitOfMountable(this.GetPtr());

    public Agent.UsageDirection EnforceShieldUsage
    {
      set => MBAPI.IMBAgent.EnforceShieldUsage(this.GetPtr(), value);
    }

    public IEnumerable<AgentComponent> Components => (IEnumerable<AgentComponent>) this._agentComponentList;

    public TextObject AgentRole { get; set; }

    internal bool HasBeenBuilt { get; private set; }

    public Agent MountAgent
    {
      get => this.GetMountAgentAux();
      private set
      {
        this.SetMountAgent(value);
        this.UpdateAgentStats();
      }
    }

    public bool HasMount => this.MountAgent != null;

    internal bool IsDeleted { get; private set; }

    internal bool IsRemoved { get; private set; }

    public MissionEquipment Equipment { get; private set; }

    internal IDetachment Detachment
    {
      get => this._detachment;
      set => this._detachment = value;
    }

    internal bool IsDetachedFromFormation => this._detachment != null;

    public float CurrentDiscipline
    {
      set
      {
        MBAPI.IMBAgent.SetCurrentDiscipline(this.GetPtr(), value);
        this._currentDiscipline = value;
        this._agentComponentList.ForEach((Action<AgentComponent>) (ac => ac.OnDisciplineChanged()));
      }
      get => this._currentDiscipline;
    }

    public bool CanLogCombatFor
    {
      get
      {
        if (this.RiderAgent != null && !this.RiderAgent.IsAIControlled)
          return true;
        return !this.IsMount && !this.IsAIControlled;
      }
    }

    public AgentMovementLockedState MovementLockedState => this.GetMovementLockedState();

    public bool Invulnerable { get; private set; }

    public Agent RiderAgent => this.GetRiderAgentAux();

    public Vec2 GetTargetPosition() => MBAPI.IMBAgent.GetTargetPosition(this.GetPtr());

    public void SetTargetPosition(Vec2 value) => MBAPI.IMBAgent.SetTargetPosition(this.GetPtr(), ref value);

    public Vec3 GetTargetDirection() => MBAPI.IMBAgent.GetTargetDirection(this.GetPtr());

    public TaleWorlds.Core.Equipment SpawnEquipment { get; private set; }

    public float Defensiveness
    {
      get => this._defensiveness;
      set
      {
        if ((double) MathF.Abs(value - this._defensiveness) <= 9.99999974737875E-05)
          return;
        this._defensiveness = value;
        this.UpdateAgentProperties();
      }
    }

    public Formation Formation
    {
      get => this._formation;
      set
      {
        if (this._formation == value)
          return;
        if (GameNetwork.IsServer && this.HasBeenBuilt && this.Mission.GetMissionBehaviour<MissionNetworkComponent>() != null)
        {
          GameNetwork.BeginBroadcastModuleEvent();
          GameNetwork.WriteMessage((GameNetworkMessage) new AgentSetFormation(this, value != null ? value.Index : -1));
          GameNetwork.EndBroadcastModuleEvent(GameNetwork.EventBroadcastFlags.None);
        }
        this.SetNativeFormationNo(value != null ? value.Index : -1);
        IDetachment detachment = (IDetachment) null;
        float num = 0.0f;
        if (this._formation != null)
        {
          this._formation.OnShapeChanged -= new Action(this.Formation_OnShapeChanged);
          if (this.IsDetachedFromFormation)
          {
            detachment = this.Detachment;
            num = this.DetachmentWeight;
          }
          this._formation.RemoveUnit(this);
        }
        this._formation = value;
        if (this._formation != null)
        {
          this._formation.OnShapeChanged += new Action(this.Formation_OnShapeChanged);
          this._formation.AddUnit(this);
          if (detachment != null && this._formation.Detachments.Contains(detachment) && detachment.IsStandingPointAvailableForAgent(this))
          {
            detachment.AddAgent(this);
            this._formation.DetachUnit(this, detachment.IsLoose);
            this.Detachment = detachment;
            this.DetachmentWeight = num;
          }
        }
        this.UpdateCachedAndFormationValues(false, false);
      }
    }

    public float MaximumMissileRange => this.GetMissileRangeWithHeightDifference();

    public void SetAveragePingInMilliseconds(double averagePingInMilliseconds) => MBAPI.IMBAgent.SetAveragePingInMilliseconds(this.GetPtr(), averagePingInMilliseconds);

    public void SetTargetPositionAndDirection(Vec2 targetPosition, Vec3 targetDirection) => MBAPI.IMBAgent.SetTargetPositionAndDirection(this.GetPtr(), ref targetPosition, ref targetDirection);

    internal FormationPositionPreference FormationPositionPreference { get; set; }

    public bool IsHero => this.Character.IsHero;

    public bool RandomizeColors { get; private set; }

    public void SetTargetPositionSynched(ref Vec2 targetPosition)
    {
      if (this.MovementLockedState != AgentMovementLockedState.None && !(this.GetTargetPosition() != targetPosition))
        return;
      if (GameNetwork.IsClientOrReplay)
      {
        this._lastSynchedTargetPosition = targetPosition;
        this._checkIfTargetFrameIsChanged = true;
      }
      else
      {
        this.SetTargetPosition(targetPosition);
        if (!GameNetwork.IsServerOrRecorder)
          return;
        GameNetwork.BeginBroadcastModuleEvent();
        GameNetwork.WriteMessage((GameNetworkMessage) new SetAgentTargetPosition(this, ref targetPosition));
        GameNetwork.EndBroadcastModuleEvent(GameNetwork.EventBroadcastFlags.AddToMissionRecord);
      }
    }

    public void SetTargetPositionAndDirectionSynched(
      ref Vec2 targetPosition,
      ref Vec3 targetDirection)
    {
      if (this.MovementLockedState != AgentMovementLockedState.None && !(this.GetTargetDirection() != targetDirection))
        return;
      if (GameNetwork.IsClientOrReplay)
      {
        this._lastSynchedTargetDirection = targetDirection;
        this._checkIfTargetFrameIsChanged = true;
      }
      else
      {
        this.SetTargetPositionAndDirection(targetPosition, targetDirection);
        if (!GameNetwork.IsServerOrRecorder)
          return;
        GameNetwork.BeginBroadcastModuleEvent();
        GameNetwork.WriteMessage((GameNetworkMessage) new SetAgentTargetPositionAndDirection(this, ref targetPosition, ref targetDirection));
        GameNetwork.EndBroadcastModuleEvent(GameNetwork.EventBroadcastFlags.AddToMissionRecord);
      }
    }

    public int KillCount { get; set; }

    public void ClearTargetFrame()
    {
      this._checkIfTargetFrameIsChanged = false;
      if (this.MovementLockedState == AgentMovementLockedState.None)
        return;
      this.ClearTargetFrameAux();
      if (!GameNetwork.IsServerOrRecorder)
        return;
      GameNetwork.BeginBroadcastModuleEvent();
      GameNetwork.WriteMessage((GameNetworkMessage) new ClearAgentTargetFrame(this));
      GameNetwork.EndBroadcastModuleEvent(GameNetwork.EventBroadcastFlags.AddToMissionRecord);
    }

    public AgentDrivenProperties AgentDrivenProperties { get; private set; }

    public void SetBodyArmorMaterialType(
      ArmorComponent.ArmorMaterialTypes bodyArmorMaterialType)
    {
      MBAPI.IMBAgent.SetBodyArmorMaterialType(this.GetPtr(), bodyArmorMaterialType);
    }

    internal void AddHitter(MissionPeer peer, float damage, bool isFriendlyHit)
    {
      Agent.Hitter hitter = this._hitterList.Find((Predicate<Agent.Hitter>) (h => h.HitterPeer == peer && h.IsFriendlyHit == isFriendlyHit));
      if (hitter == null)
        this._hitterList.Add(new Agent.Hitter(peer, damage, Environment.TickCount, isFriendlyHit));
      else
        hitter.IncreaseDamage(damage);
    }

    public MissionWeapon WieldedWeapon
    {
      get
      {
        EquipmentIndex wieldedItemIndex = this.GetWieldedItemIndex(Agent.HandIndex.MainHand);
        return wieldedItemIndex < EquipmentIndex.WeaponItemBeginSlot ? MissionWeapon.Invalid : this.Equipment[wieldedItemIndex];
      }
    }

    internal bool IsRangedCached { get; private set; }

    internal void RemoveHitter(MissionPeer peer, bool isFriendlyHit)
    {
      Agent.Hitter hitter = this._hitterList.Find((Predicate<Agent.Hitter>) (h => h.HitterPeer == peer && h.IsFriendlyHit == isFriendlyHit));
      if (hitter == null)
        return;
      this._hitterList.Remove(hitter);
    }

    internal float CharPowerCached { get; private set; }

    internal float WalkSpeedCached { get; private set; }

    public float GetInteractionDistanceToUsable(IUsable usable)
    {
      if (usable is Agent agent)
        return !agent.IsMount ? 3f : 1.75f;
      float interactionDistance = MissionGameModels.Current.AgentStatCalculateModel.GetInteractionDistance(this);
      if (!(usable is StandingPoint))
        return interactionDistance;
      return !this.IsAIControlled || !this.WalkMode ? 1f : 0.5f;
    }

    internal float RunSpeedCached { get; private set; }

    public bool CanReachAgent(Agent otherAgent)
    {
      float distanceToUsable = this.GetInteractionDistanceToUsable((IUsable) otherAgent);
      return (double) this.Position.DistanceSquared(otherAgent.Position) < (double) distanceToUsable * (double) distanceToUsable;
    }

    FocusableObjectType IFocusable.FocusableObjectType => !this.IsMount ? FocusableObjectType.Agent : FocusableObjectType.Mount;

    public bool CanInteractWithAgent(Agent otherAgent, float userAgentCameraElevation)
    {
      bool flag1 = false;
      foreach (MissionBehaviour missionBehaviour in Mission.Current.MissionBehaviours)
        flag1 = flag1 || missionBehaviour.IsThereAgentAction(this, otherAgent);
      if (!flag1 && GameNetwork.IsSessionActive && this.MissionRepresentative != null)
        flag1 = this.MissionRepresentative.IsThereAgentAction(otherAgent);
      if (!flag1)
        return false;
      bool flag2 = this.CanReachAgent(otherAgent);
      if (!otherAgent.IsMount)
        return this.IsOnLand() & flag2;
      if (this.MountAgent == null && this.GetCurrentAction(0) != ActionIndexCache.act_none || this.MountAgent != null && !this.IsOnLand())
        return false;
      return otherAgent.RiderAgent == null ? this.MountAgent == null & flag2 && this.CheckSkillForMounting(otherAgent) && otherAgent.GetCurrentActionType(0) != Agent.ActionCodeType.Rear : otherAgent == this.MountAgent && flag2 && (double) userAgentCameraElevation < (double) this.GetLookDownLimit() + 0.400000005960464 && (double) this.GetCurrentVelocity().LengthSquared < 0.25 && otherAgent.GetCurrentActionType(0) != Agent.ActionCodeType.Rear;
    }

    internal void SetUsedGameObjectForClient(UsableMissionObject usedObject)
    {
      this.CurrentlyUsedGameObject = usedObject;
      usedObject.OnUse(this);
      this.Mission.OnObjectUsed(this, usedObject);
    }

    public void UseGameObject(UsableMissionObject usedObject, int preferenceIndex = -1)
    {
      if (usedObject.LockUserFrames)
      {
        WorldFrame userFrameForAgent = usedObject.GetUserFrameForAgent(this);
        this.SetTargetPositionAndDirection(userFrameForAgent.Origin.AsVec2, userFrameForAgent.Rotation.f);
        this.SetScriptedFlags(this.GetScriptedFlags() | Agent.AIScriptedFrameFlags.NoAttack);
      }
      else if (usedObject.LockUserPositions)
      {
        this.SetTargetPosition(usedObject.GetUserFrameForAgent(this).Origin.AsVec2);
        this.SetScriptedFlags(this.GetScriptedFlags() | Agent.AIScriptedFrameFlags.NoAttack);
      }
      if (this.IsActive() && this.IsAIControlled)
        this.AIMoveToGameObjectDisable();
      this.CurrentlyUsedGameObject = usedObject;
      this._usedObjectPreferenceIndex = preferenceIndex;
      if (this.IsAIControlled)
        this.AIUseGameObjectEnable(true);
      usedObject.OnUse(this);
      this.Mission.OnObjectUsed(this, usedObject);
      if (!usedObject.IsInstantUse || GameNetwork.IsClientOrReplay || !this.IsActive())
        return;
      this.StopUsingGameObject();
    }

    public bool CanBeAssignedForScriptedMovement() => this.IsActive() && !this.IsDetachedFromFormation && (!this.IsUsingGameObject && this.IsAIControlled) && (!this.IsRunningAway && (this.GetScriptedFlags() & Agent.AIScriptedFrameFlags.GoToPosition) == Agent.AIScriptedFrameFlags.None) && (this.AIStateFlags & (Agent.AIStateFlag.UseObjectMoving | Agent.AIStateFlag.UseObjectUsing)) == Agent.AIStateFlag.None;

    public bool CanReachAndUseObject(UsableMissionObject gameObject, float distanceSq) => this.CanReachObject(gameObject, distanceSq) && this.CanUseObject(gameObject);

    public bool CanReachObject(UsableMissionObject gameObject, float distanceSq)
    {
      if (this.IsItemUseDisabled || this.IsUsingGameObject)
        return false;
      float distanceToUsable = this.GetInteractionDistanceToUsable((IUsable) gameObject);
      return (double) distanceSq <= (double) distanceToUsable * (double) distanceToUsable && (double) Math.Abs(gameObject.InteractionEntity.GlobalPosition.z - this.Position.z) <= (double) distanceToUsable * 2.0;
    }

    public bool CanUseObject(UsableMissionObject gameObject) => !gameObject.IsDisabledForAgent(this) && gameObject.IsUsableByAgent(this);

    public bool ObjectHasVacantPosition(UsableMissionObject gameObject) => !gameObject.HasUser || gameObject.HasAIUser;

    public void StopUsingGameObject(bool isSuccessful = true, bool autoAttach = true)
    {
      UsableMachine usableMachine = this.Controller != Agent.ControllerType.AI || this.Formation == null ? (UsableMachine) null : this.Formation.GetDetachmentOrDefault(this) as UsableMachine;
      if (usableMachine == null)
        autoAttach = false;
      UsableMissionObject currentlyUsedGameObject = this.CurrentlyUsedGameObject;
      if (this.IsUsingGameObject)
      {
        int num = this.CurrentlyUsedGameObject.LockUserFrames ? 1 : (this.CurrentlyUsedGameObject.LockUserPositions ? 1 : 0);
        if (GameNetwork.IsServerOrRecorder)
        {
          GameNetwork.BeginBroadcastModuleEvent();
          GameNetwork.WriteMessage((GameNetworkMessage) new StopUsingObject(this, isSuccessful));
          GameNetwork.EndBroadcastModuleEvent(GameNetwork.EventBroadcastFlags.AddToMissionRecord);
        }
        this.CurrentlyUsedGameObject.OnUseStopped(this, isSuccessful, this._usedObjectPreferenceIndex);
        this.CurrentlyUsedGameObject = (UsableMissionObject) null;
        this._usedObjectPreferenceIndex = -1;
        if (num != 0)
          this.ClearTargetFrame();
      }
      else
        this.AIMoveToGameObjectDisable();
      if (this.IsAIControlled)
      {
        this.DisableScriptedMovement();
        this.AIUseGameObjectEnable(false);
        if (autoAttach && this.Formation != null)
          this.Formation.AttachUnit(this);
        if (usableMachine != null)
        {
          foreach (StandingPoint standingPoint in usableMachine.StandingPoints)
            standingPoint.FavoredUser = this;
        }
      }
      if (currentlyUsedGameObject != null)
        this.Mission.OnObjectStoppedBeingUsed(this, currentlyUsedGameObject);
      this._agentComponentList.ForEach((Action<AgentComponent>) (ac => ac.OnStopUsingGameObject()));
    }

    public void OnFocusLose(Agent userAgent)
    {
    }

    public TextObject GetInfoTextForBeingNotInteractable(Agent userAgent) => this.IsMount && !userAgent.CheckSkillForMounting(this) ? GameTexts.FindText("str_ui_riding_skill_not_adequate_to_mount") : TextObject.Empty;

    public void HandleStopUsingAction()
    {
      if (GameNetwork.IsClient)
      {
        GameNetwork.BeginModuleEventAsClient();
        GameNetwork.WriteMessage((GameNetworkMessage) new RequestStopUsingObject());
        GameNetwork.EndModuleEventAsClient();
      }
      else
        this.StopUsingGameObject(false);
    }

    public void HandleStartUsingAction(UsableMissionObject targetObject, int preferenceIndex)
    {
      if (GameNetwork.IsClient)
      {
        MBDebug.Print("MissionSiegeNetworkComponent::OnAgentRequestUseGameObject " + (object) targetObject.GameEntity);
        GameNetwork.BeginModuleEventAsClient();
        GameNetwork.WriteMessage((GameNetworkMessage) new RequestUseObject(targetObject, preferenceIndex));
        GameNetwork.EndModuleEventAsClient();
      }
      else
        this.UseGameObject(targetObject, preferenceIndex);
    }

    public void OnItemRemovedFromScene() => this.StopUsingGameObject(false);

    public AgentController AddController(System.Type type)
    {
      AgentController agentController = (AgentController) null;
      if (type.IsSubclassOf(typeof (AgentController)))
        agentController = Activator.CreateInstance(type) as AgentController;
      if (agentController != null)
      {
        agentController.Owner = this;
        agentController.Mission = this.Mission;
        this._agentControllers.Add(agentController);
        agentController.OnInitialize();
      }
      return agentController;
    }

    public T GetController<T>() where T : AgentController
    {
      for (int index = 0; index < this._agentControllers.Count; ++index)
      {
        if (this._agentControllers[index] is T)
          return (T) this._agentControllers[index];
      }
      return default (T);
    }

    int IFormationUnit.FormationFileIndex { get; set; }

    public AgentController RemoveController(System.Type type)
    {
      for (int index = 0; index < this._agentControllers.Count; ++index)
      {
        if (type.IsInstanceOfType((object) this._agentControllers[index]))
        {
          AgentController agentController = this._agentControllers[index];
          this._agentControllers.RemoveAt(index);
          return agentController;
        }
      }
      return (AgentController) null;
    }

    int IFormationUnit.FormationRankIndex { get; set; }

    public void SetTeam(Team team, bool sync)
    {
      if (this.Team == team)
        return;
      this.Team?.RemoveAgentFromTeam(this);
      this.Team = team;
      this.Team?.AddAgentToTeam(this);
      this.SetTeamInternal(team != null ? team.MBTeam : MBTeam.InvalidTeam);
      if (!sync || !GameNetwork.IsServer || !this.Mission.HasMissionBehaviour<MissionNetworkComponent>())
        return;
      GameNetwork.BeginBroadcastModuleEvent();
      GameNetwork.WriteMessage((GameNetworkMessage) new AgentSetTeam(this, team));
      GameNetwork.EndBroadcastModuleEvent(GameNetwork.EventBroadcastFlags.AddToMissionRecord);
    }

    public Team Team { get; private set; }

    public void SetClothingColor1(uint color) => this._clothingColor1 = new uint?(color);

    public IAgentOriginBase Origin { get; set; }

    IFormationUnit IFormationUnit.FollowedUnit
    {
      get
      {
        if (!this.IsActive())
          return (IFormationUnit) null;
        return this.IsAIControlled ? (IFormationUnit) this.GetFollowedUnit() : (IFormationUnit) null;
      }
    }

    public void SetClothingColor2(uint color) => this._clothingColor2 = new uint?(color);

    public bool CheckTracked(BasicCharacterObject basicCharacter) => this.Character == basicCharacter;

    private UIntPtr GetPtr() => this.Pointer;

    public void ClearEquipment() => MBAPI.IMBAgent.ClearEquipment(this.GetPtr());

    public EquipmentIndex GetWieldedItemIndex(Agent.HandIndex index) => MBAPI.IMBAgent.GetWieldedItemIndex(this.GetPtr(), (int) index);

    public void SetWieldedItemIndexAsClient(
      Agent.HandIndex handIndex,
      EquipmentIndex equipmentIndex,
      bool isWieldedInstantly,
      bool isWieldedOnSpawn,
      int mainHandCurrentUsageIndex)
    {
      MBAPI.IMBAgent.SetWieldedItemIndexAsClient(this.GetPtr(), (int) handIndex, (int) equipmentIndex, isWieldedInstantly, isWieldedOnSpawn, mainHandCurrentUsageIndex);
    }

    public void SetAsConversationAgent(bool set)
    {
      if (set)
      {
        this.SetScriptedFlags(this.GetScriptedFlags() | Agent.AIScriptedFrameFlags.InConversation);
        this.DisableLookToPointOfInterest();
      }
      else
        this.SetScriptedFlags(this.GetScriptedFlags() & ~Agent.AIScriptedFrameFlags.InConversation);
    }

    public void SetCrouchMode(bool set)
    {
      if (set)
        this.SetScriptedFlags(this.GetScriptedFlags() | Agent.AIScriptedFrameFlags.Crouch);
      else
        this.SetScriptedFlags(this.GetScriptedFlags() & ~Agent.AIScriptedFrameFlags.Crouch);
    }

    bool IAgent.IsEnemyOf(IAgent agent) => this.IsEnemyOf((Agent) agent);

    public AgentState State
    {
      get => AgentHelper.GetAgentState(this._statePointer);
      set
      {
        if (this.State == value)
          return;
        MBAPI.IMBAgent.SetStateFlags(this.GetPtr(), value);
      }
    }

    public bool IsActive() => this.State == AgentState.Active;

    private void SetWeaponHitPointsInSlot(EquipmentIndex equipmentIndex, short hitPoints) => MBAPI.IMBAgent.SetWeaponHitPointsInSlot(this.GetPtr(), (int) equipmentIndex, hitPoints);

    bool IAgent.IsFriendOf(IAgent agent) => this.IsFriendOf((Agent) agent);

    public void SetWeaponAmountInSlot(
      EquipmentIndex equipmentSlot,
      short amount,
      bool enforcePrimaryItem)
    {
      MBAPI.IMBAgent.SetWeaponAmountInSlot(this.GetPtr(), (int) equipmentSlot, amount, enforcePrimaryItem);
    }

    public void SetWeaponAmmoAsClient(
      EquipmentIndex equipmentIndex,
      EquipmentIndex ammoEquipmentIndex,
      short ammo)
    {
      MBAPI.IMBAgent.SetWeaponAmmoAsClient(this.GetPtr(), (int) equipmentIndex, (int) ammoEquipmentIndex, ammo);
    }

    public MissionWeapon WieldedOffhandWeapon
    {
      get
      {
        EquipmentIndex wieldedItemIndex = this.GetWieldedItemIndex(Agent.HandIndex.OffHand);
        return wieldedItemIndex < EquipmentIndex.WeaponItemBeginSlot ? MissionWeapon.Invalid : this.Equipment[wieldedItemIndex];
      }
    }

    IMissionTeam IAgent.Team => (IMissionTeam) this.Team;

    public void SetWeaponReloadPhaseAsClient(EquipmentIndex equipmentIndex, short reloadState) => MBAPI.IMBAgent.SetWeaponReloadPhaseAsClient(this.GetPtr(), (int) equipmentIndex, reloadState);

    TextObject ITrackableBase.GetName() => this.Character != null ? new TextObject(this.Character.Name.ToString()) : TextObject.Empty;

    public void OnFocusGain(Agent userAgent)
    {
    }

    public float GetTrackDistanceToMainAgent()
    {
      float num = -1f;
      if (Agent.Main != null)
        num = Agent.Main.Position.Distance(this.Position);
      return num;
    }

    Vec3 ITrackableBase.GetPosition() => this.Position;

    public string GetDescriptionText(GameEntity gameEntity = null) => this.Name;

    public void SetReloadAmmoInSlot(
      EquipmentIndex equipmentIndex,
      EquipmentIndex ammoSlotIndex,
      short reloadedAmmo)
    {
      MBAPI.IMBAgent.SetReloadAmmoInSlot(this.GetPtr(), (int) equipmentIndex, (int) ammoSlotIndex, reloadedAmmo);
    }

    public void SetUsageIndexOfWeaponInSlotAsClient(EquipmentIndex slotIndex, int usageIndex) => MBAPI.IMBAgent.SetUsageIndexOfWeaponInSlotAsClient(this.GetPtr(), (int) slotIndex, usageIndex);

    public void StartSwitchingWeaponUsageIndexAsClient(
      EquipmentIndex equipmentIndex,
      int usageIndex,
      Agent.UsageDirection currentMovementFlagUsageDirection)
    {
      MBAPI.IMBAgent.StartSwitchingWeaponUsageIndexAsClient(this.GetPtr(), (int) equipmentIndex, usageIndex, currentMovementFlagUsageDirection);
    }

    public void TryToWieldWeaponInSlot(
      EquipmentIndex slotIndex,
      Agent.WeaponWieldActionType type,
      bool isWieldedOnSpawn)
    {
      MBAPI.IMBAgent.TryToWieldWeaponInSlot(this.GetPtr(), (int) slotIndex, (int) type, isWieldedOnSpawn);
    }

    public GameEntity GetWeaponEntityFromEquipmentSlot(EquipmentIndex slotIndex) => new GameEntity(MBAPI.IMBAgent.GetWeaponEntityFromEquipmentSlot(this.GetPtr(), (int) slotIndex));

    public void SetRandomizeColors(bool shouldRandomize) => this.RandomizeColors = shouldRandomize;

    public void PrepareWeaponForDropInEquipmentSlot(
      EquipmentIndex slotIndex,
      bool showHolsterWithWeapon)
    {
      MBAPI.IMBAgent.PrepareWeaponForDropInEquipmentSlot(this.GetPtr(), (int) slotIndex, showHolsterWithWeapon);
    }

    public void TryToSheathWeaponInHand(Agent.HandIndex handIndex, Agent.WeaponWieldActionType type) => MBAPI.IMBAgent.TryToSheathWeaponInHand(this.GetPtr(), (int) handIndex, (int) type);

    public void UpdateWeapons() => MBAPI.IMBAgent.UpdateWeapons(this.GetPtr());

    public event Agent.OnAgentHealthChangedDelegate OnAgentHealthChanged;

    public void OnUse(Agent userAgent) => this.Mission.OnAgentInteraction(userAgent, this);

    internal Agent(
      Mission mission,
      Mission.AgentCreationResult creationResult,
      Agent.CreationType creationType,
      Monster monster)
    {
      this.AgentRole = TextObject.Empty;
      this.Mission = mission;
      this.Index = creationResult.Index;
      this._pointer = creationResult.AgentPtr;
      this._positionPointer = creationResult.PositionPtr;
      this._flagsPointer = creationResult.FlagsPtr;
      this._indexPointer = creationResult.IndexPtr;
      this._statePointer = creationResult.StatePtr;
      MBAPI.IMBAgent.SetMonoObject(this.GetPtr(), this);
      this.Monster = monster;
      this.KillCount = 0;
      this.HasBeenBuilt = false;
      this._creationType = creationType;
      this._agentControllers = new List<AgentController>();
      this._agentComponentList = new List<AgentComponent>();
      this._agentComponentsDict = new Dictionary<System.Type, AgentComponent>();
      this._hitterList = new List<Agent.Hitter>();
      ((IFormationUnit) this).FormationFileIndex = -1;
      ((IFormationUnit) this).FormationRankIndex = -1;
      this._synchedBodyComponents = (List<CompositeComponent>) null;
      this._cachedAndFormationValuesUpdateTimer = new Timer(MBCommon.GetTime(MBCommon.TimeType.Mission), (float) (0.449999988079071 + (double) MBRandom.RandomFloat * 0.100000001490116));
    }

    public void OnUseStopped(Agent userAgent, bool isSuccessful, int preferenceIndex)
    {
    }

    private void AttachWeaponToBoneAux(
      ref MissionWeapon weapon,
      GameEntity weaponEntity,
      sbyte boneIndex,
      ref MatrixFrame attachLocalFrame)
    {
      WeaponData weaponData = weapon.GetWeaponData(true);
      MBAPI.IMBAgent.AttachWeaponToBone(this.Pointer, in weaponData, weapon.GetWeaponStatsData(), weapon.WeaponsCount, weaponEntity != null ? weaponEntity.Pointer : UIntPtr.Zero, boneIndex, ref attachLocalFrame);
      weaponData.DeinitializeManagedPointers();
    }

    private void AttachWeaponToWeaponAux(
      EquipmentIndex slotIndex,
      ref MissionWeapon weapon,
      GameEntity weaponEntity,
      ref MatrixFrame attachLocalFrame)
    {
      WeaponData weaponData = weapon.GetWeaponData(true);
      MBAPI.IMBAgent.AttachWeaponToWeaponInSlot(this.Pointer, in weaponData, weapon.GetWeaponStatsData(), weapon.WeaponsCount, weaponEntity != null ? weaponEntity.Pointer : UIntPtr.Zero, (int) slotIndex, ref attachLocalFrame);
      weaponData.DeinitializeManagedPointers();
    }

    public GameEntity GetSteppedEntity()
    {
      UIntPtr steppedEntityId = MBAPI.IMBAgent.GetSteppedEntityId(this.GetPtr());
      return !(steppedEntityId != UIntPtr.Zero) ? (GameEntity) null : new GameEntity(steppedEntityId);
    }

    private Agent GetMountAgentAux() => this._cachedMountAgent;

    [MBCallback]
    internal void UpdateMountAgentCache(Agent newMountAgent) => this._cachedMountAgent = newMountAgent;

    private Agent GetRiderAgentAux() => this._cachedRiderAgent;

    [MBCallback]
    internal void UpdateRiderAgentCache(Agent newRiderAgent) => this._cachedRiderAgent = newRiderAgent;

    public void SetAlwaysAttackInMelee(bool attack) => MBAPI.IMBAgent.SetAlwaysAttackInMelee(this.GetPtr(), attack);

    private void SetMountAgent(Agent mountAgent)
    {
      int mountAgentIndex = mountAgent == null ? -1 : mountAgent.Index;
      MBAPI.IMBAgent.SetMountAgent(this.GetPtr(), mountAgentIndex);
    }

    private void SetTeamInternal(MBTeam team) => MBAPI.IMBAgent.SetTeam(this.GetPtr(), team.Index);

    public void SetFormationFrameDisabled() => MBAPI.IMBAgent.SetFormationFrameDisabled(this.GetPtr());

    public void SetFormationFrameEnabled(
      WorldPosition position,
      Vec2 direction,
      float formationDirectionEnforcingFactor)
    {
      if (!(this.Mission.IsTeleportingAgents & MBAPI.IMBAgent.SetFormationFrameEnabled(this.GetPtr(), position, direction, formationDirectionEnforcingFactor)))
        return;
      this.TeleportToPosition(position.GetGroundVec3());
    }

    public void SetShouldCatchUpWithFormation(bool value) => MBAPI.IMBAgent.SetShouldCatchUpWithFormation(this.GetPtr(), value);

    public void SetFormationIntegrityData(
      Vec2 position,
      Vec2 currentFormationDirection,
      Vec2 averageVelocityOfCloseAgents,
      float averageMaxUnlimitedSpeedOfCloseAgents,
      float deviationOfPositions)
    {
      MBAPI.IMBAgent.SetFormationIntegrityData(this.GetPtr(), position, currentFormationDirection, averageVelocityOfCloseAgents, averageMaxUnlimitedSpeedOfCloseAgents, deviationOfPositions);
    }

    public void SetFormationNeighborhoodData(
      int[] neighborAgentIndices,
      Vec2[] neighborAgentLocalDifferences)
    {
      MBAPI.IMBAgent.SetFormationNeighborhoodData(this.GetPtr(), neighborAgentIndices, neighborAgentLocalDifferences);
    }

    public void Retreat(WorldPosition retreatPos) => MBAPI.IMBAgent.SetRetreatMode(this.GetPtr(), retreatPos, true);

    public void StopRetreating()
    {
      MBAPI.IMBAgent.SetRetreatMode(this.GetPtr(), WorldPosition.Invalid, false);
      this.IsRunningAway = false;
    }

    public bool IsRetreating() => MBAPI.IMBAgent.IsRetreating(this.GetPtr());

    public bool IsFadingOut() => MBAPI.IMBAgent.IsFadingOut(this.GetPtr());

    public void StartFadingOut() => MBAPI.IMBAgent.StartFadingOut(this.GetPtr());

    public WorldPosition GetRetreatPos() => MBAPI.IMBAgent.GetRetreatPos(this.GetPtr());

    public void SetGuardedAgent(Agent guardedAgent)
    {
      int guardedAgentIndex = guardedAgent != null ? guardedAgent.Index : -1;
      MBAPI.IMBAgent.SetGuardedAgentIndex(this.GetPtr(), guardedAgentIndex);
    }

    public void SetColumnwiseFollowAgent(Agent followAgent, ref Vec2 followPosition)
    {
      if (!this.IsAIControlled)
        return;
      int followAgentIndex = followAgent != null ? followAgent.Index : -1;
      MBAPI.IMBAgent.SetColumnwiseFollowAgent(this.GetPtr(), followAgentIndex, ref followPosition);
      this.SetFollowedUnit(followAgent);
    }

    public void ResetAI() => MBAPI.IMBAgent.ResetAI(this.GetPtr());

    private Agent.ControllerType GetController() => this._agentControllerType;

    private void SetController(Agent.ControllerType controllerType)
    {
      if (controllerType == this._agentControllerType)
        return;
      this._agentControllerType = controllerType;
      MBAPI.IMBAgent.SetController(this.GetPtr(), controllerType);
    }

    public bool AllowFirstPersonWideRotation() => MBAPI.IMBAgent.AllowFirstPersonWideRotation(this.GetPtr());

    public AnimFlags GetCurrentAnimationFlag(int channelNo) => (AnimFlags) MBAPI.IMBAgent.GetCurrentAnimationFlags(this.GetPtr(), channelNo);

    private Agent GetRiderAgent() => MBAPI.IMBAgent.GetRiderAgent(this.GetPtr());

    public ActionIndexCache GetCurrentAction(int channelNo) => new ActionIndexCache(MBAPI.IMBAgent.GetCurrentAction(this.GetPtr(), channelNo));

    public Agent.ActionCodeType GetCurrentActionType(int channelNo) => (Agent.ActionCodeType) MBAPI.IMBAgent.GetCurrentActionType(this.GetPtr(), channelNo);

    public Agent.ActionStage GetCurrentActionStage(int channelNo) => (Agent.ActionStage) MBAPI.IMBAgent.GetCurrentActionStage(this.GetPtr(), channelNo);

    public Agent.UsageDirection GetCurrentActionDirection(int channelNo) => (Agent.UsageDirection) MBAPI.IMBAgent.GetCurrentActionDirection(this.GetPtr(), channelNo);

    public Vec3 ComputeAnimationDisplacement(float dt) => MBAPI.IMBAgent.ComputeAnimationDisplacement(this.GetPtr(), dt);

    public int GetCurrentActionPriority(int channelNo) => MBAPI.IMBAgent.GetCurrentActionPriority(this.GetPtr(), channelNo);

    public float GetCurrentActionProgress(int channelNo) => MBAPI.IMBAgent.GetCurrentActionProgress(this.GetPtr(), channelNo);

    public void SetCurrentActionProgress(int channelNo, float progress) => MBAPI.IMBAgent.SetCurrentActionProgress(this.GetPtr(), channelNo, progress);

    public void SetCurrentActionSpeed(int channelNo, float speed) => MBAPI.IMBAgent.SetCurrentActionSpeed(this.GetPtr(), channelNo, speed);

    public bool SetActionChannel(
      int channelNo,
      ActionIndexCache actionIndexCache,
      bool ignorePriority = false,
      ulong additionalFlags = 0,
      float blendWithNextActionFactor = 0.0f,
      float actionSpeed = 1f,
      float blendInPeriod = -0.2f,
      float blendOutPeriodToNoAnim = 0.4f,
      float startProgress = 0.0f,
      bool useLinearSmoothing = false,
      float blendOutPeriod = -0.2f,
      int actionShift = 0,
      bool forceFaceMorphRestart = true)
    {
      int index = actionIndexCache.Index;
      return MBAPI.IMBAgent.SetActionChannel(this.GetPtr(), channelNo, index + actionShift, additionalFlags, ignorePriority, blendWithNextActionFactor, actionSpeed, blendInPeriod, blendOutPeriodToNoAnim, startProgress, useLinearSmoothing, blendOutPeriod, forceFaceMorphRestart);
    }

    public void TickActionChannels(float dt) => MBAPI.IMBAgent.TickActionChannels(this.GetPtr(), dt);

    public float GetActionChannelWeight(int channelNo) => MBAPI.IMBAgent.GetActionChannelWeight(this.GetPtr(), channelNo);

    public float GetActionChannelCurrentActionWeight(int channelNo) => MBAPI.IMBAgent.GetActionChannelCurrentActionWeight(this.GetPtr(), channelNo);

    internal void Clear()
    {
      this.Mission = (Mission) null;
      this._pointer = UIntPtr.Zero;
      this._positionPointer = UIntPtr.Zero;
      this._flagsPointer = UIntPtr.Zero;
      this._indexPointer = UIntPtr.Zero;
      this._statePointer = UIntPtr.Zero;
    }

    private void SetNetworkPeer(NetworkCommunicator newPeer) => MBAPI.IMBAgent.SetNetworkPeer(this.GetPtr(), newPeer != null ? newPeer.Index : -1);

    private void SetInitialFrame(ref MatrixFrame initialFrame) => MBAPI.IMBAgent.SetInitialFrame(this.GetPtr(), ref initialFrame);

    private void WeaponEquipped(
      EquipmentIndex equipmentSlot,
      in WeaponData weaponData,
      WeaponStatsData[] weaponStatsData,
      in WeaponData ammoWeaponData,
      WeaponStatsData[] ammoWeaponStatsData,
      GameEntity weaponEntity,
      bool removeOldWeaponFromScene,
      bool isWieldedOnSpawn)
    {
      MBAPI.IMBAgent.WeaponEquipped(this.GetPtr(), (int) equipmentSlot, in weaponData, weaponStatsData, weaponStatsData != null ? weaponStatsData.Length : 0, in ammoWeaponData, ammoWeaponStatsData, ammoWeaponStatsData != null ? ammoWeaponStatsData.Length : 0, weaponEntity != null ? weaponEntity.Pointer : UIntPtr.Zero, removeOldWeaponFromScene, isWieldedOnSpawn);
      this.CheckEquipmentForCapeClothSimulationStateChange();
    }

    public void OnWeaponDrop(EquipmentIndex equipmentSlot)
    {
      MissionWeapon droppedWeapon = this.Equipment[equipmentSlot];
      this.Equipment[equipmentSlot] = MissionWeapon.Invalid;
      this.WeaponEquipped(equipmentSlot, in WeaponData.InvalidWeaponData, (WeaponStatsData[]) null, in WeaponData.InvalidWeaponData, (WeaponStatsData[]) null, (GameEntity) null, false, false);
      foreach (AgentComponent agentComponent in this._agentComponentList)
        agentComponent.OnWeaponDrop(droppedWeapon);
    }

    private void BuildAux() => MBAPI.IMBAgent.Build(this.GetPtr(), this.Monster.EyeOffsetWrtHead);

    public void LockAgentReplicationTableDataWithCurrentReliableSequenceNo(NetworkCommunicator peer)
    {
      MBDebug.Print("peer : " + peer.UserName + " id : " + (object) this.Index + " name : " + this.Name);
      MBAPI.IMBAgent.LockAgentReplicationTableDataWithCurrentReliableSequenceNo(this.GetPtr(), peer.Index);
    }

    private void UpdateDrivenProperties(float[] values) => MBAPI.IMBAgent.UpdateDrivenProperties(this.GetPtr(), values);

    public WorldFrame GetWorldFrame() => new WorldFrame(this.LookRotation, this.GetWorldPosition());

    public void TeleportToPosition(Vec3 position)
    {
      if (this.MountAgent != null)
        MBAPI.IMBAgent.SetPosition(this.MountAgent.GetPtr(), ref position);
      MBAPI.IMBAgent.SetPosition(this.GetPtr(), ref position);
      if (this.RiderAgent == null)
        return;
      MBAPI.IMBAgent.SetPosition(this.RiderAgent.GetPtr(), ref position);
    }

    public float GetLookDownLimit() => MBAPI.IMBAgent.GetLookDownLimit(this.GetPtr());

    public float GetEyeGlobalHeight() => MBAPI.IMBAgent.GetEyeGlobalHeight(this.GetPtr());

    public void SetMinimumSpeed(float speed) => MBAPI.IMBAgent.SetMinimumSpeed(this.GetPtr(), speed);

    public void SetMaximumSpeedLimit(float maximumSpeedLimit, bool isMultiplier) => MBAPI.IMBAgent.SetMaximumSpeedLimit(this.GetPtr(), maximumSpeedLimit, isMultiplier);

    public float GetMaximumSpeedLimit() => MBAPI.IMBAgent.GetMaximumSpeedLimit(this.GetPtr());

    public Vec2 GetCurrentVelocity() => MBAPI.IMBAgent.GetCurrentVelocity(this.GetPtr());

    public float GetTurnSpeed() => MBAPI.IMBAgent.GetTurnSpeed(this.GetPtr());

    public float GetCurrentSpeedLimit() => MBAPI.IMBAgent.GetCurrentSpeedLimit(this.GetPtr());

    public void SetAttackState(int attackState) => MBAPI.IMBAgent.SetAttackState(this.GetPtr(), attackState);

    public void SetAiBehaviorParams(
      AISimpleBehaviorKind behavior,
      float y1,
      float x2,
      float y2,
      float x3,
      float y3)
    {
      MBAPI.IMBAgent.SetAiBehaviorParams(this.GetPtr(), (int) behavior, y1, x2, y2, x3, y3);
    }

    public void SetAllBehaviorParams(BehaviorValues[] behaviorParams) => MBAPI.IMBAgent.SetAllAIBehaviorParams(this.GetPtr(), behaviorParams);

    private void UpdateLastAttackAndHitTimes(Agent attackerAgent, bool isMissile)
    {
      float time = MBCommon.TimeType.Mission.GetTime();
      if (isMissile)
        this.LastRangedHitTime = time;
      else
        this.LastMeleeHitTime = time;
      if (attackerAgent == this || attackerAgent == null)
        return;
      if (isMissile)
        attackerAgent.LastRangedAttackTime = time;
      else
        attackerAgent.LastMeleeAttackTime = time;
    }

    private void UpdateBoneMapCache() => this._boneMap = new Agent.AgentBoneMapArray(this.AgentVisuals.GetSkeleton());

    public Vec3 GetMovementDirection() => MBAPI.IMBAgent.GetMovementDirection(this.GetPtr());

    public void SetMovementDirection(ref Vec3 direction) => MBAPI.IMBAgent.SetMovementDirection(this.GetPtr(), ref direction);

    public Vec3 GetCurWeaponOffset() => MBAPI.IMBAgent.GetCurWeaponOffset(this.GetPtr());

    public bool GetIsLeftStance() => MBAPI.IMBAgent.GetIsLeftStance(this.GetPtr());

    private void SetInitialAgentScaleAux(float initialScale) => MBAPI.IMBAgent.SetAgentScale(this.GetPtr(), initialScale);

    public void FadeOut(bool hideInstantly, bool hideMount)
    {
      MBAPI.IMBAgent.FadeOut(this.GetPtr(), hideInstantly);
      if (!hideMount || !this.HasMount)
        return;
      this.MountAgent.FadeOut(hideMount, false);
    }

    public void FadeIn() => MBAPI.IMBAgent.FadeIn(this.GetPtr());

    public Agent.AIScriptedFrameFlags GetScriptedFlags() => (Agent.AIScriptedFrameFlags) MBAPI.IMBAgent.GetScriptedFlags(this.GetPtr());

    public void SetScriptedFlags(Agent.AIScriptedFrameFlags flags) => MBAPI.IMBAgent.SetScriptedFlags(this.GetPtr(), (int) flags);

    public Agent.AISpecialCombatModeFlags GetScriptedCombatFlags() => (Agent.AISpecialCombatModeFlags) MBAPI.IMBAgent.GetScriptedCombatFlags(this.GetPtr());

    public void SetScriptedCombatFlags(Agent.AISpecialCombatModeFlags flags) => MBAPI.IMBAgent.SetScriptedCombatFlags(this.GetPtr(), (int) flags);

    public void SetScriptedPositionAndDirection(
      ref WorldPosition scriptedPosition,
      float scriptedDirection,
      bool addHumanLikeDelay,
      Agent.AIScriptedFrameFlags additionalFlags = Agent.AIScriptedFrameFlags.None,
      string debugString = "")
    {
      MBAPI.IMBAgent.SetScriptedPositionAndDirection(this.GetPtr(), ref scriptedPosition, scriptedDirection, addHumanLikeDelay, (int) additionalFlags, debugString);
      if (!this.Mission.IsTeleportingAgents || !(scriptedPosition.AsVec2 != this.Position.AsVec2))
        return;
      this.TeleportToPosition(scriptedPosition.GetGroundVec3());
    }

    public void SetScriptedPosition(
      ref WorldPosition position,
      bool addHumanLikeDelay,
      Agent.AIScriptedFrameFlags additionalFlags = Agent.AIScriptedFrameFlags.None,
      string debugString = "")
    {
      MBAPI.IMBAgent.SetScriptedPosition(this.GetPtr(), ref position, addHumanLikeDelay, (int) additionalFlags, debugString);
      if (!this.Mission.IsTeleportingAgents || !(position.AsVec2 != this.Position.AsVec2))
        return;
      this.TeleportToPosition(position.GetGroundVec3());
    }

    public void SetScriptedTargetEntityAndPosition(
      GameEntity target,
      WorldPosition position,
      Agent.AISpecialCombatModeFlags additionalFlags = Agent.AISpecialCombatModeFlags.None)
    {
      MBAPI.IMBAgent.SetScriptedTargetEntity(this.GetPtr(), target.Pointer, ref position, (int) additionalFlags);
    }

    public void DisableScriptedMovement() => MBAPI.IMBAgent.DisableScriptedMovement(this.GetPtr());

    public void DisableScriptedCombatMovement() => MBAPI.IMBAgent.DisableScriptedCombatMovement(this.GetPtr());

    public void ForceAiBehaviourSelection() => MBAPI.IMBAgent.ForceAiBehaviourSelection(this.GetPtr());

    public bool HasPathThroughNavigationFaceIdFromDirection(int navigationFaceId, Vec2 direction) => MBAPI.IMBAgent.HasPathThroughNavigationFaceIdFromDirection(this.GetPtr(), navigationFaceId, ref direction);

    public bool CanMoveDirectlyToPosition(in WorldPosition worldPosition) => MBAPI.IMBAgent.CanMoveDirectlyToPosition(this.GetPtr(), in worldPosition);

    public float GetPathDistanceToPoint(ref Vec3 point) => MBAPI.IMBAgent.GetPathDistanceToPoint(this.GetPtr(), ref point);

    public bool CheckPathToAITargetAgentPassesThroughNavigationFaceIdFromDirection(
      int navigationFaceId,
      Vec3 direction,
      float overridenCostForFaceId)
    {
      return MBAPI.IMBAgent.CheckPathToAITargetAgentPassesThroughNavigationFaceIdFromDirection(this.GetPtr(), navigationFaceId, ref direction, overridenCostForFaceId);
    }

    public int GetCurrentNavigationFaceId() => MBAPI.IMBAgent.GetCurrentNavigationFaceId(this.GetPtr());

    public WorldPosition GetWorldPosition() => MBAPI.IMBAgent.GetWorldPosition(this.GetPtr());

    public void SetAgentExcludeStateForFaceGroupId(int faceGroupId, bool isExcluded) => MBAPI.IMBAgent.SetAgentExcludeStateForFaceGroupId(this.GetPtr(), faceGroupId, isExcluded);

    public void SetLookAgent(Agent agent)
    {
      this._lookAgentCache = agent;
      MBAPI.IMBAgent.SetLookAgent(this.GetPtr(), agent != null ? agent.GetPtr() : UIntPtr.Zero);
    }

    public void SetInteractionAgent(Agent agent) => MBAPI.IMBAgent.SetInteractionAgent(this.GetPtr(), agent != null ? agent.GetPtr() : UIntPtr.Zero);

    public void ResetLookAgent() => this.SetLookAgent((Agent) null);

    public Agent GetLookAgent() => this._lookAgentCache;

    public Agent GetTargetAgent() => MBAPI.IMBAgent.GetTargetAgent(this.GetPtr());

    public void SetLookToPointOfInterest(Vec3 point) => MBAPI.IMBAgent.SetLookToPointOfInterest(this.GetPtr(), point);

    public void DisableLookToPointOfInterest() => MBAPI.IMBAgent.DisableLookToPointOfInterest(this.GetPtr());

    public AgentFlag GetAgentFlags() => AgentHelper.GetAgentFlags(this.FlagsPointer);

    public void SetAgentFlags(AgentFlag agentFlags) => MBAPI.IMBAgent.SetAgentFlags(this.GetPtr(), (uint) agentFlags);

    public void SetAgentFacialAnimation(
      Agent.FacialAnimChannel channel,
      string animationName,
      bool loop)
    {
      MBAPI.IMBAgent.SetAgentFacialAnimation(this.GetPtr(), (int) channel, animationName, loop);
    }

    public string GetAgentFacialAnimation() => MBAPI.IMBAgent.GetAgentFacialAnimation(this.GetPtr());

    public string GetAgentVoiceDefinition() => MBAPI.IMBAgent.GetAgentVoiceDefinition(this.GetPtr());

    public CompositeComponent AddPrefabComponentToBone(
      string prefabName,
      sbyte boneIndex)
    {
      return MBAPI.IMBAgent.AddPrefabToAgentBone(this.GetPtr(), prefabName, boneIndex);
    }

    private AgentMovementLockedState GetMovementLockedState() => MBAPI.IMBAgent.GetMovementLockedState(this.GetPtr());

    private void ClearTargetFrameAux() => MBAPI.IMBAgent.ClearTargetFrame(this.GetPtr());

    public void MakeVoice(
      SkinVoiceManager.SkinVoiceType voiceType,
      SkinVoiceManager.CombatVoiceNetworkPredictionType predictionType)
    {
      MBAPI.IMBAgent.MakeVoice(this.GetPtr(), voiceType.Index, (int) predictionType);
    }

    public Vec3 GetEyeGlobalPosition() => MBAPI.IMBAgent.GetEyeGlobalPosition(this.GetPtr());

    public Vec3 GetChestGlobalPosition() => MBAPI.IMBAgent.GetChestGlobalPosition(this.GetPtr());

    public void WieldNextWeapon(Agent.HandIndex weaponIndex) => MBAPI.IMBAgent.WieldNextWeapon(this.GetPtr(), (int) weaponIndex);

    private void PreloadForRenderingAux() => MBAPI.IMBAgent.PreloadForRendering(this.GetPtr());

    public Agent.MovementControlFlag AttackDirectionToMovementFlag(
      Agent.UsageDirection direction)
    {
      return MBAPI.IMBAgent.AttackDirectionToMovementFlag(this.GetPtr(), direction);
    }

    public Agent.MovementControlFlag DefendDirectionToMovementFlag(
      Agent.UsageDirection direction)
    {
      return MBAPI.IMBAgent.DefendDirectionToMovementFlag(this.GetPtr(), direction);
    }

    public static Agent.UsageDirection MovementFlagToDirection(Agent.MovementControlFlag flag)
    {
      if (flag.HasAnyFlag<Agent.MovementControlFlag>(Agent.MovementControlFlag.AttackDown))
        return Agent.UsageDirection.AttackDown;
      if (flag.HasAnyFlag<Agent.MovementControlFlag>(Agent.MovementControlFlag.AttackUp))
        return Agent.UsageDirection.AttackUp;
      if (flag.HasAnyFlag<Agent.MovementControlFlag>(Agent.MovementControlFlag.AttackLeft))
        return Agent.UsageDirection.AttackLeft;
      if (flag.HasAnyFlag<Agent.MovementControlFlag>(Agent.MovementControlFlag.AttackRight))
        return Agent.UsageDirection.AttackRight;
      if (flag.HasAnyFlag<Agent.MovementControlFlag>(Agent.MovementControlFlag.DefendDown))
        return Agent.UsageDirection.DefendDown;
      if (flag.HasAnyFlag<Agent.MovementControlFlag>(Agent.MovementControlFlag.DefendUp))
        return Agent.UsageDirection.AttackEnd;
      if (flag.HasAnyFlag<Agent.MovementControlFlag>(Agent.MovementControlFlag.DefendLeft))
        return Agent.UsageDirection.DefendLeft;
      return flag.HasAnyFlag<Agent.MovementControlFlag>(Agent.MovementControlFlag.DefendRight) ? Agent.UsageDirection.DefendRight : Agent.UsageDirection.None;
    }

    public bool KickClear() => MBAPI.IMBAgent.KickClear(this.GetPtr());

    public void ResetGuard() => MBAPI.IMBAgent.ResetGuard(this.GetPtr());

    public Agent.MovementControlFlag GetDefendMovementFlag() => MBAPI.IMBAgent.GetDefendMovementFlag(this.GetPtr());

    public Agent.UsageDirection GetAttackDirection(bool doAiCheck) => MBAPI.IMBAgent.GetAttackDirection(this.GetPtr(), doAiCheck);

    public Agent.UsageDirection PlayerAttackDirection() => MBAPI.IMBAgent.PlayerAttackDirection(this.GetPtr());

    public WeaponInfo GetWieldedWeaponInfo(Agent.HandIndex handIndex)
    {
      bool isMeleeWeapon = false;
      bool isRangedWeapon = false;
      return MBAPI.IMBAgent.GetWieldedWeaponInfo(this.GetPtr(), (int) handIndex, ref isMeleeWeapon, ref isRangedWeapon) ? new WeaponInfo(isMeleeWeapon, isRangedWeapon) : (WeaponInfo) null;
    }

    public Vec2 GetBodyRotationConstraint(int channelIndex = 1) => MBAPI.IMBAgent.GetBodyRotationConstraint(this.GetPtr(), channelIndex).AsVec2;

    public static Agent.UsageDirection GetActionDirection(int actionIndex) => MBAPI.IMBAgent.GetActionDirection(actionIndex);

    private void HandleBlowAux(ref Blow b) => MBAPI.IMBAgent.HandleBlowAux(this.GetPtr(), ref b);

    public bool SetHandInverseKinematicsFrame(
      ref MatrixFrame leftGlobalFrame,
      ref MatrixFrame rightGlobalFrame)
    {
      return MBAPI.IMBAgent.SetHandInverseKinematicsFrame(this.GetPtr(), ref leftGlobalFrame, ref rightGlobalFrame);
    }

    public void ClearHandInverseKinematics() => MBAPI.IMBAgent.ClearHandInverseKinematics(this.GetPtr());

    public void CreateBloodBurstAtLimb(sbyte iBone, ref Vec3 impactPosition, float scale) => MBAPI.IMBAgent.CreateBloodBurstAtLimb(this.GetPtr(), iBone, ref impactPosition, scale);

    public bool IsOnLand() => MBAPI.IMBAgent.IsOnLand(this.GetPtr());

    public bool IsSliding() => MBAPI.IMBAgent.IsSliding(this.GetPtr());

    public static int GetMonsterUsageIndex(string monsterUsage) => MBAPI.IMBAgent.GetMonsterUsageIndex(monsterUsage);

    [MBCallback]
    public float GetMissileRangeWithHeightDifferenceAux(float targetZ) => MBAPI.IMBAgent.GetMissileRangeWithHeightDifference(this.GetPtr(), targetZ);

    public void SetNativeFormationNo(int formationNo) => MBAPI.IMBAgent.SetFormationNo(this.GetPtr(), formationNo);

    [Conditional("TRACE")]
    private void CheckUnmanagedAgentValid() => AgentHelper.GetAgentIndex(this._indexPointer);

    public void SetDirectionChangeTendency(float tendency) => MBAPI.IMBAgent.SetDirectionChangeTendency(this.GetPtr(), tendency);

    public void SetFiringOrder(int order) => MBAPI.IMBAgent.SetFiringOrder(this.GetPtr(), order);

    public void SetSoundOcclusion(float value) => MBAPI.IMBAgent.SetSoundOcclusion(this.GetPtr(), value);

    public void SetRidingOrder(int order) => MBAPI.IMBAgent.SetRidingOrder(this.GetPtr(), order);

    public float GetTotalEncumbrance() => this.AgentDrivenProperties.ArmorEncumbrance + this.AgentDrivenProperties.WeaponsEncumbrance;

    public void SetInvulnerable(bool newValue) => this.Invulnerable = newValue;

    private float GetMissileRangeWithHeightDifference() => this.IsMount || !this.IsRangedCached || (this.Formation == null || this.Formation.QuerySystem.ClosestEnemyFormation == null) ? 0.0f : this.GetMissileRangeWithHeightDifferenceAux(this.Formation.QuerySystem.ClosestEnemyFormation.MedianPosition.GetNavMeshZ());

    public void AddComponent(AgentComponent agentComponent)
    {
      this._agentComponentList.Add(agentComponent);
      this._agentComponentsDict[agentComponent.GetType()] = agentComponent;
    }

    public bool RemoveComponent(AgentComponent agentComponent)
    {
      int num = this._agentComponentList.Remove(agentComponent) ? 1 : 0;
      this._agentComponentsDict[agentComponent.GetType()] = (AgentComponent) null;
      return num != 0;
    }

    public T GetComponent<T>() where T : AgentComponent
    {
      AgentComponent agentComponent;
      return this._agentComponentsDict.TryGetValue(typeof (T), out agentComponent) ? (T) agentComponent : default (T);
    }

    internal void InitializeComponents()
    {
      foreach (AgentComponent agentComponent in this._agentComponentList)
        agentComponent.Initialize();
    }

    public float GetAgentDrivenPropertyValue(DrivenProperty type) => this.AgentDrivenProperties.GetStat(type);

    public void SetAgentDrivenPropertyValueFromConsole(DrivenProperty type, float val) => this.AgentDrivenProperties.SetStat(type, val);

    public void InitializeSpawnEquipment(TaleWorlds.Core.Equipment spawnEquipment) => this.SpawnEquipment = spawnEquipment;

    public void InitializeMissionEquipment(MissionEquipment missionEquipment, Banner banner)
    {
      if (missionEquipment != null)
        this.Equipment = missionEquipment;
      else
        this.Equipment = new MissionEquipment(this.SpawnEquipment, banner);
    }

    internal void Build(AgentBuildData agentBuildData)
    {
      this.BuildAux();
      this.HasBeenBuilt = true;
      this.Controller = this.GetAgentFlags().HasAnyFlag<AgentFlag>(AgentFlag.IsHumanoid) ? agentBuildData.AgentController : Agent.ControllerType.AI;
      MissionGameModels.Current?.AgentStatCalculateModel.InitializeMissionEquipment(this);
      this.InitializeAgentProperties(this.SpawnEquipment, agentBuildData);
      if (!GameNetwork.IsServerOrRecorder)
        return;
      foreach (NetworkCommunicator networkPeer in GameNetwork.NetworkPeers)
      {
        if (!networkPeer.IsMine && networkPeer.IsSynchronized)
          this.LockAgentReplicationTableDataWithCurrentReliableSequenceNo(networkPeer);
      }
    }

    [MBCallback]
    internal void OnWieldedItemIndexChange(
      bool isOffHand,
      bool isWieldedInstantly,
      bool isWieldedOnSpawn)
    {
      if (this.IsMainAgent)
      {
        Agent.OnMainAgentWieldedItemChangeDelegate wieldedItemChange = this.OnMainAgentWieldedItemChange;
        if (wieldedItemChange != null)
          wieldedItemChange();
      }
      Action wieldedItemChange1 = this.OnAgentWieldedItemChange;
      if (wieldedItemChange1 != null)
        wieldedItemChange1();
      if (GameNetwork.IsServerOrRecorder)
      {
        int mainHandCurUsageIndex = 0;
        EquipmentIndex wieldedItemIndex = this.GetWieldedItemIndex(Agent.HandIndex.MainHand);
        if (wieldedItemIndex != EquipmentIndex.None)
          mainHandCurUsageIndex = this.Equipment[wieldedItemIndex].CurrentUsageIndex;
        GameNetwork.BeginBroadcastModuleEvent();
        GameNetwork.WriteMessage((GameNetworkMessage) new SetWieldedItemIndex(this, isOffHand, isWieldedInstantly, isWieldedOnSpawn, this.GetWieldedItemIndex(isOffHand ? Agent.HandIndex.OffHand : Agent.HandIndex.MainHand), mainHandCurUsageIndex));
        GameNetwork.EndBroadcastModuleEvent(GameNetwork.EventBroadcastFlags.AddToMissionRecord);
      }
      this.CheckEquipmentForCapeClothSimulationStateChange();
    }

    public void CheckEquipmentForCapeClothSimulationStateChange()
    {
      if (!((NativeObject) this._capeClothSimulator != (NativeObject) null))
        return;
      bool flag = false;
      EquipmentIndex wieldedItemIndex = this.GetWieldedItemIndex(Agent.HandIndex.OffHand);
      for (EquipmentIndex index = EquipmentIndex.WeaponItemBeginSlot; index < EquipmentIndex.Weapon4; ++index)
      {
        MissionWeapon missionWeapon = this.Equipment[index];
        if (!missionWeapon.IsEmpty && missionWeapon.IsShield() && index != wieldedItemIndex)
        {
          flag = true;
          break;
        }
      }
      this._capeClothSimulator.SetMaxDistanceMultiplier(flag ? 0.0f : 1f);
    }

    public void InitializeAgentProperties(TaleWorlds.Core.Equipment spawnEquipment, AgentBuildData agentBuildData)
    {
      this._propertyModifiers = new Agent.AgentPropertiesModifiers();
      this.AgentDrivenProperties = new AgentDrivenProperties();
      this.UpdateDrivenProperties(this.AgentDrivenProperties.InitializeDrivenProperties(this, spawnEquipment, agentBuildData));
    }

    public void ResetAgentProperties() => this.AgentDrivenProperties = (AgentDrivenProperties) null;

    public void UpdateAgentProperties()
    {
      if (this.AgentDrivenProperties == null)
        return;
      this.UpdateDrivenProperties(this.AgentDrivenProperties.UpdateDrivenProperties(this));
    }

    public void UpdateCustomDrivenProperties()
    {
      if (this.AgentDrivenProperties == null)
        return;
      this.UpdateDrivenProperties(this.AgentDrivenProperties.Values);
    }

    public void CheckToDropFlaggedItem()
    {
      if (!this.GetAgentFlags().HasAnyFlag<AgentFlag>(AgentFlag.CanWieldWeapon))
        return;
      for (int index = 0; index < 2; ++index)
      {
        EquipmentIndex wieldedItemIndex = this.GetWieldedItemIndex((Agent.HandIndex) index);
        if (wieldedItemIndex != EquipmentIndex.None && this.Equipment[wieldedItemIndex].Item.ItemFlags.HasAnyFlag<ItemFlags>(ItemFlags.DropOnAnyAction))
          this.DropItem(wieldedItemIndex);
      }
    }

    public void HandleDropWeapon(
      bool isDefendPressed,
      EquipmentIndex forcedSlotIndexToDropWeaponFrom)
    {
      Agent.ActionCodeType currentActionType = this.GetCurrentActionType(1);
      if (this.State != AgentState.Active || currentActionType == Agent.ActionCodeType.ReleaseMelee || (currentActionType == Agent.ActionCodeType.ReleaseRanged || currentActionType == Agent.ActionCodeType.ReleaseThrowing) || currentActionType == Agent.ActionCodeType.WeaponBash)
        return;
      EquipmentIndex itemIndex = forcedSlotIndexToDropWeaponFrom;
      if (itemIndex == EquipmentIndex.None)
      {
        EquipmentIndex wieldedItemIndex1 = this.GetWieldedItemIndex(Agent.HandIndex.MainHand);
        EquipmentIndex wieldedItemIndex2 = this.GetWieldedItemIndex(Agent.HandIndex.OffHand);
        if (wieldedItemIndex2 >= EquipmentIndex.WeaponItemBeginSlot & isDefendPressed)
          itemIndex = wieldedItemIndex2;
        else if (wieldedItemIndex1 >= EquipmentIndex.WeaponItemBeginSlot)
          itemIndex = wieldedItemIndex1;
        else if (wieldedItemIndex2 >= EquipmentIndex.WeaponItemBeginSlot)
        {
          itemIndex = wieldedItemIndex2;
        }
        else
        {
          for (EquipmentIndex index1 = EquipmentIndex.WeaponItemBeginSlot; index1 < EquipmentIndex.Weapon4; ++index1)
          {
            if (!this.Equipment[index1].IsEmpty && this.Equipment[index1].Item.PrimaryWeapon.IsConsumable)
            {
              if (this.Equipment[index1].Item.PrimaryWeapon.IsRangedWeapon)
              {
                if (this.Equipment[index1].Amount == (short) 0)
                {
                  itemIndex = index1;
                  break;
                }
              }
              else
              {
                bool flag = false;
                for (EquipmentIndex index2 = EquipmentIndex.WeaponItemBeginSlot; index2 < EquipmentIndex.Weapon4; ++index2)
                {
                  if (!this.Equipment[index2].IsEmpty && (this.Equipment[index2].HasAnyUsageWithAmmoClass(this.Equipment[index1].Item.PrimaryWeapon.WeaponClass) && this.Equipment[index1].Amount > (short) 0))
                  {
                    flag = true;
                    break;
                  }
                }
                if (!flag)
                {
                  itemIndex = index1;
                  break;
                }
              }
            }
          }
        }
      }
      if (itemIndex == EquipmentIndex.None)
        return;
      this.DropItem(itemIndex);
      this.UpdateAgentProperties();
    }

    public void DropItem(EquipmentIndex itemIndex, WeaponClass pickedUpItemType = WeaponClass.Undefined)
    {
      if (itemIndex != EquipmentIndex.None && this.Equipment[itemIndex].CurrentUsageItem.WeaponFlags.HasAllFlags<WeaponFlags>(WeaponFlags.AffectsArea | WeaponFlags.Burning))
      {
        MatrixFrame entitialFrameWithIndex = this.AgentVisuals.GetSkeleton().GetBoneEntitialFrameWithIndex((byte) this.Monster.MainHandItemBoneIndex);
        MatrixFrame globalFrame = this.AgentVisuals.GetGlobalFrame();
        MatrixFrame parent = globalFrame.TransformToParent(entitialFrameWithIndex);
        Vec3 velocity = globalFrame.origin + globalFrame.rotation.f - parent.origin;
        double num = (double) velocity.Normalize();
        Mat3 identity = Mat3.Identity;
        identity.f = velocity;
        identity.Orthonormalize();
        Mission.Current.OnAgentShootMissile(this, itemIndex, parent.origin, velocity, identity, false, false, -1);
        this.RemoveEquippedWeapon(itemIndex);
      }
      else
        MBAPI.IMBAgent.DropItem(this.GetPtr(), (int) itemIndex, (int) pickedUpItemType);
    }

    public void EquipItemsFromSpawnEquipment()
    {
      this.Mission.OnEquipItemsFromSpawnEquipmentBegin(this, this._creationType);
      switch (this._creationType)
      {
        case Agent.CreationType.FromRoster:
        case Agent.CreationType.FromCharacterObj:
          for (EquipmentIndex equipmentIndex = EquipmentIndex.WeaponItemBeginSlot; equipmentIndex < EquipmentIndex.NumAllWeaponSlots; ++equipmentIndex)
          {
            WeaponData weaponData = WeaponData.InvalidWeaponData;
            WeaponStatsData[] weaponStatsData = (WeaponStatsData[]) null;
            WeaponData ammoWeaponData = WeaponData.InvalidWeaponData;
            WeaponStatsData[] ammoWeaponStatsData = (WeaponStatsData[]) null;
            MissionWeapon missionWeapon = this.Equipment[equipmentIndex];
            if (!missionWeapon.IsEmpty)
            {
              missionWeapon = this.Equipment[equipmentIndex];
              weaponData = missionWeapon.GetWeaponData(true);
              missionWeapon = this.Equipment[equipmentIndex];
              weaponStatsData = missionWeapon.GetWeaponStatsData();
              missionWeapon = this.Equipment[equipmentIndex];
              ammoWeaponData = missionWeapon.GetAmmoWeaponData(true);
              missionWeapon = this.Equipment[equipmentIndex];
              ammoWeaponStatsData = missionWeapon.GetAmmoWeaponStatsData();
            }
            this.WeaponEquipped(equipmentIndex, in weaponData, weaponStatsData, in ammoWeaponData, ammoWeaponStatsData, (GameEntity) null, true, true);
            weaponData.DeinitializeManagedPointers();
            ammoWeaponData.DeinitializeManagedPointers();
            int attachmentIndex = 0;
            while (true)
            {
              int num = attachmentIndex;
              missionWeapon = this.Equipment[equipmentIndex];
              int attachedWeaponsCount = missionWeapon.GetAttachedWeaponsCount();
              if (num < attachedWeaponsCount)
              {
                missionWeapon = this.Equipment[equipmentIndex];
                MatrixFrame attachedWeaponFrame = missionWeapon.GetAttachedWeaponFrame(attachmentIndex);
                missionWeapon = this.Equipment[equipmentIndex];
                MissionWeapon attachedWeapon = missionWeapon.GetAttachedWeapon(attachmentIndex);
                this.AttachWeaponToWeaponAux(equipmentIndex, ref attachedWeapon, (GameEntity) null, ref attachedWeaponFrame);
                ++attachmentIndex;
              }
              else
                break;
            }
          }
          this.AddSkinMeshes();
          break;
      }
      this.UpdateAgentProperties();
      this.Mission.OnEquipItemsFromSpawnEquipment(this, this._creationType);
      this.CheckEquipmentForCapeClothSimulationStateChange();
    }

    private void AddSkinMeshes()
    {
      bool prepareImmediately = this == Agent.Main;
      this.AgentVisuals.AddSkinMeshes(new SkinGenerationParams((int) this.SpawnEquipment.GetSkinMeshesMask(), this.SpawnEquipment.GetUnderwearType(this.IsFemale && (double) this.BodyPropertiesValue.Age >= 14.0), (int) this.SpawnEquipment.BodyMeshType, (int) this.SpawnEquipment.HairCoverType, (int) this.SpawnEquipment.BeardCoverType, (int) this.SpawnEquipment.BodyDeformType, prepareImmediately, this.Character.FaceDirtAmount, this.IsFemale ? 1 : 0, false, false), this.BodyPropertiesValue, this.Character != null && this.Character.FaceMeshCache);
    }

    public void WieldInitialWeapons(Agent.WeaponWieldActionType wieldActionType = Agent.WeaponWieldActionType.InstantAfterPickUp)
    {
      EquipmentIndex mainHandWeaponIndex = this.GetWieldedItemIndex(Agent.HandIndex.MainHand);
      EquipmentIndex offHandWeaponIndex = this.GetWieldedItemIndex(Agent.HandIndex.OffHand);
      this.SpawnEquipment.GetInitialWeaponIndicesToEquip(out mainHandWeaponIndex, out offHandWeaponIndex, out bool _);
      if (offHandWeaponIndex != EquipmentIndex.None)
        this.TryToWieldWeaponInSlot(offHandWeaponIndex, wieldActionType, true);
      if (mainHandWeaponIndex == EquipmentIndex.None)
        return;
      this.TryToWieldWeaponInSlot(mainHandWeaponIndex, wieldActionType, true);
    }

    public void ChangeWeaponHitPoints(EquipmentIndex slotIndex, short hitPoints)
    {
      this.Equipment.SetHitPointsOfSlot(slotIndex, hitPoints);
      this.SetWeaponHitPointsInSlot(slotIndex, hitPoints);
      if (GameNetwork.IsServerOrRecorder)
      {
        GameNetwork.BeginBroadcastModuleEvent();
        GameNetwork.WriteMessage((GameNetworkMessage) new SetWeaponNetworkData(this, slotIndex, hitPoints));
        GameNetwork.EndBroadcastModuleEvent(GameNetwork.EventBroadcastFlags.AddToMissionRecord);
      }
      foreach (AgentComponent agentComponent in this._agentComponentList)
        agentComponent.OnWeaponHPChanged(this.Equipment[slotIndex].Item, (int) hitPoints);
    }

    public bool HasWeapon()
    {
      for (int index = 0; index < 5; ++index)
      {
        WeaponComponentData currentUsageItem = this.Equipment[index].CurrentUsageItem;
        if (currentUsageItem != null && currentUsageItem.WeaponFlags.HasAnyFlag<WeaponFlags>(WeaponFlags.WeaponMask))
          return true;
      }
      return false;
    }

    internal void SetMountAgentBeforeBuild(Agent mount) => this.MountAgent = mount;

    [MBCallback]
    internal void OnRemoveWeapon(EquipmentIndex slotIndex) => this.RemoveEquippedWeapon(slotIndex);

    [MBCallback]
    internal void OnWeaponUsageIndexChange(EquipmentIndex slotIndex, int usageIndex)
    {
      this.Equipment.SetUsageIndexOfSlot(slotIndex, usageIndex);
      this.UpdateAgentProperties();
      if (!GameNetwork.IsServerOrRecorder)
        return;
      GameNetwork.BeginBroadcastModuleEvent();
      GameNetwork.WriteMessage((GameNetworkMessage) new WeaponUsageIndexChangeMessage(this, slotIndex, usageIndex));
      GameNetwork.EndBroadcastModuleEvent(GameNetwork.EventBroadcastFlags.AddToMissionRecord);
    }

    [MBCallback]
    internal void OnWeaponSwitchingToAlternativeStart(EquipmentIndex slotIndex, int usageIndex)
    {
      if (!GameNetwork.IsServerOrRecorder)
        return;
      GameNetwork.BeginBroadcastModuleEvent();
      GameNetwork.WriteMessage((GameNetworkMessage) new StartSwitchingWeaponUsageIndex(this, slotIndex, usageIndex, Agent.MovementFlagToDirection(this.MovementFlags)));
      GameNetwork.EndBroadcastModuleEvent(GameNetwork.EventBroadcastFlags.AddToMissionRecord);
    }

    [MBCallback]
    internal void OnWeaponReloadPhaseChange(EquipmentIndex slotIndex, short reloadPhase)
    {
      this.Equipment.SetReloadPhaseOfSlot(slotIndex, reloadPhase);
      if (!GameNetwork.IsServerOrRecorder)
        return;
      GameNetwork.BeginBroadcastModuleEvent();
      GameNetwork.WriteMessage((GameNetworkMessage) new SetWeaponReloadPhase(this, slotIndex, reloadPhase));
      GameNetwork.EndBroadcastModuleEvent(GameNetwork.EventBroadcastFlags.AddToMissionRecord);
    }

    [MBCallback]
    internal void OnWeaponAmountChange(EquipmentIndex slotIndex, short amount)
    {
      this.Equipment.SetAmountOfSlot(slotIndex, amount);
      this.UpdateAgentProperties();
      if (!GameNetwork.IsServerOrRecorder)
        return;
      GameNetwork.BeginBroadcastModuleEvent();
      GameNetwork.WriteMessage((GameNetworkMessage) new SetWeaponNetworkData(this, slotIndex, amount));
      GameNetwork.EndBroadcastModuleEvent(GameNetwork.EventBroadcastFlags.AddToMissionRecord);
    }

    [MBCallback]
    internal void OnWeaponAmmoReload(
      EquipmentIndex slotIndex,
      EquipmentIndex ammoSlotIndex,
      short totalAmmo)
    {
      if (this.Equipment[slotIndex].CurrentUsageItem.IsRangedWeapon)
      {
        this.Equipment.SetReloadedAmmoOfSlot(slotIndex, ammoSlotIndex, totalAmmo);
        if (GameNetwork.IsServerOrRecorder)
        {
          GameNetwork.BeginBroadcastModuleEvent();
          GameNetwork.WriteMessage((GameNetworkMessage) new SetWeaponAmmoData(this, slotIndex, ammoSlotIndex, totalAmmo));
          GameNetwork.EndBroadcastModuleEvent(GameNetwork.EventBroadcastFlags.AddToMissionRecord);
        }
      }
      this.UpdateAgentProperties();
    }

    [MBCallback]
    internal void OnWeaponAmmoConsume(EquipmentIndex slotIndex, short totalAmmo)
    {
      if (this.Equipment[slotIndex].CurrentUsageItem.IsRangedWeapon)
      {
        this.Equipment.SetConsumedAmmoOfSlot(slotIndex, totalAmmo);
        if (GameNetwork.IsServerOrRecorder)
        {
          GameNetwork.BeginBroadcastModuleEvent();
          GameNetwork.WriteMessage((GameNetworkMessage) new SetWeaponAmmoData(this, slotIndex, EquipmentIndex.None, totalAmmo));
          GameNetwork.EndBroadcastModuleEvent(GameNetwork.EventBroadcastFlags.AddToMissionRecord);
        }
      }
      this.UpdateAgentProperties();
    }

    [MBCallback]
    internal void OnShieldDamaged(EquipmentIndex slotIndex, int inflictedDamage)
    {
      int num = Math.Max(0, (int) this.Equipment[slotIndex].HitPoints - inflictedDamage);
      this.ChangeWeaponHitPoints(slotIndex, (short) num);
      if (num != 0)
        return;
      this.RemoveEquippedWeapon(slotIndex);
    }

    [MBCallback]
    internal void OnWeaponAmmoRemoved(EquipmentIndex slotIndex)
    {
      MissionWeapon ammoWeapon = this.Equipment[slotIndex];
      ammoWeapon = ammoWeapon.AmmoWeapon;
      if (ammoWeapon.IsEmpty)
        return;
      this.Equipment.SetConsumedAmmoOfSlot(slotIndex, (short) 0);
    }

    public void UpdateBodyProperties(BodyProperties bodyProperties) => this.BodyPropertiesValue = bodyProperties;

    public void AttachWeaponToWeapon(
      EquipmentIndex slotIndex,
      MissionWeapon weapon,
      GameEntity weaponEntity,
      ref MatrixFrame attachLocalFrame)
    {
      this.Equipment.AttachWeaponToWeaponInSlot(slotIndex, ref weapon, ref attachLocalFrame);
      this.AttachWeaponToWeaponAux(slotIndex, ref weapon, weaponEntity, ref attachLocalFrame);
    }

    public void AttachWeaponToBone(
      MissionWeapon weapon,
      GameEntity weaponEntity,
      sbyte boneIndex,
      ref MatrixFrame attachLocalFrame)
    {
      if (this._attachedWeapons == null)
      {
        this._attachedWeapons = new List<MissionWeapon>();
        this._attachedWeaponFrames = new List<MatrixFrame>();
        this._attachedWeaponBoneIndices = new List<sbyte>();
      }
      this._attachedWeapons.Add(weapon);
      this._attachedWeaponFrames.Add(attachLocalFrame);
      this._attachedWeaponBoneIndices.Add(boneIndex);
      this.AttachWeaponToBoneAux(ref weapon, weaponEntity, boneIndex, ref attachLocalFrame);
    }

    public int GetAttachedWeaponsCount()
    {
      List<MissionWeapon> attachedWeapons = this._attachedWeapons;
      // ISSUE: explicit non-virtual call
      return attachedWeapons == null ? 0 : __nonvirtual (attachedWeapons.Count);
    }

    public MissionWeapon GetAttachedWeapon(int index) => this._attachedWeapons[index];

    public MatrixFrame GetAttachedWeaponFrame(int index) => this._attachedWeaponFrames[index];

    public Agent.ControllerType Controller
    {
      get => this.GetController();
      set
      {
        Agent.ControllerType controller = this.Controller;
        if (value == controller)
          return;
        this.SetController(value);
        if (value == Agent.ControllerType.Player)
        {
          this.Mission.MainAgent = this;
          if (this.Formation != null)
          {
            Formation formation = this.Formation;
            formation.RemoveUnit(this);
            formation.AddUnit(this);
          }
          this.SetAgentFlags(this.GetAgentFlags() | AgentFlag.CanRide);
          this.SetMaximumSpeedLimit(-1f, false);
        }
        if (value != Agent.ControllerType.AI)
          this.SetMaximumSpeedLimit(-1f, false);
        foreach (MissionBehaviour missionBehaviour in this.Mission.MissionBehaviours)
          missionBehaviour.OnAgentControllerChanged(this);
        if (!GameNetwork.IsServer)
          return;
        MissionPeer missionPeer = this.MissionPeer;
        NetworkCommunicator communicator = missionPeer != null ? missionPeer.GetNetworkPeer() : (NetworkCommunicator) null;
        if (communicator == null || communicator.IsServerPeer)
          return;
        GameNetwork.BeginModuleEventAsServer(communicator);
        GameNetwork.WriteMessage((GameNetworkMessage) new SetAgentIsPlayer(this, this.Controller != Agent.ControllerType.AI));
        GameNetwork.EndModuleEventAsServer();
      }
    }

    public sbyte GetAttachedWeaponBoneIndex(int index) => this._attachedWeaponBoneIndices[index];

    public void ClearAttachedWeapons() => this._attachedWeapons?.Clear();

    public void RestoreShieldHitPoints()
    {
      for (EquipmentIndex index = EquipmentIndex.WeaponItemBeginSlot; index < EquipmentIndex.Weapon4; ++index)
      {
        MissionWeapon missionWeapon = this.Equipment[index];
        if (!missionWeapon.IsEmpty)
        {
          missionWeapon = this.Equipment[index];
          if (missionWeapon.CurrentUsageItem.IsShield)
          {
            int num = (int) index;
            missionWeapon = this.Equipment[index];
            int modifiedMaxHitPoints = (int) missionWeapon.ModifiedMaxHitPoints;
            this.ChangeWeaponHitPoints((EquipmentIndex) num, (short) modifiedMaxHitPoints);
          }
        }
      }
    }

    public void Die(Blow b, Agent.KillInfo overrideKillInfo = Agent.KillInfo.Invalid)
    {
      if (this.Formation != null)
      {
        this.Formation.Team.QuerySystem.RegisterDeath();
        if (b.IsMissile)
          this.Formation.Team.QuerySystem.RegisterDeathByRanged();
      }
      this.Health = 0.0f;
      MBAPI.IMBAgent.Die(this.GetPtr(), ref b, (sbyte) overrideKillInfo);
    }

    public uint ClothingColor1
    {
      get
      {
        if (this._clothingColor1.HasValue)
          return this._clothingColor1.Value;
        return this.Team != null ? this.Team.Color : uint.MaxValue;
      }
    }

    public string Name => this.MissionPeer == null ? this._name.ToString() : this.MissionPeer.Name;

    internal MatrixFrame InitialFrame
    {
      set => this.SetInitialFrame(ref value);
    }

    public uint ClothingColor2 => this._clothingColor2.HasValue ? this._clothingColor2.Value : this.ClothingColor1;

    public float Health
    {
      get => this._health;
      set
      {
        float comparedValue = value.ApproximatelyEqualsTo(0.0f) ? 0.0f : (float) MBMath.Ceiling(value);
        if (this._health.ApproximatelyEqualsTo(comparedValue))
          return;
        float health = this._health;
        this._health = comparedValue;
        Agent.OnAgentHealthChangedDelegate agentHealthChanged = this.OnAgentHealthChanged;
        if (agentHealthChanged == null)
          return;
        agentHealthChanged(this, health, this._health);
      }
    }

    public void MakeDead(bool isKilled, ActionIndexCache actionIndex) => MBAPI.IMBAgent.MakeDead(this.GetPtr(), isKilled, actionIndex.Index);

    public float BaseHealthLimit { get; set; }

    public float HealthLimit { get; set; }

    public MatrixFrame LookFrame => new MatrixFrame()
    {
      origin = this.Position,
      rotation = this.LookRotation
    };

    public MissionPeer MissionPeer
    {
      get => this._missionPeer;
      set
      {
        if (this._missionPeer == value)
          return;
        MissionPeer missionPeer = this._missionPeer;
        this._missionPeer = value;
        if (missionPeer != null && missionPeer.ControlledAgent == this)
          missionPeer.ControlledAgent = (Agent) null;
        if (this._missionPeer != null && this._missionPeer.ControlledAgent != this)
        {
          this._missionPeer.ControlledAgent = this;
          if (GameNetwork.IsServerOrRecorder)
          {
            Agent.OnAgentHealthChangedDelegate agentHealthChanged = this.OnAgentHealthChanged;
            if (agentHealthChanged != null)
              agentHealthChanged(this, this.Health, this.Health);
          }
        }
        if (value != null)
          this.Controller = value.IsMine ? Agent.ControllerType.Player : Agent.ControllerType.None;
        if (!GameNetwork.IsServer || !this.IsHuman || this.IsDeleted)
          return;
        NetworkCommunicator networkCommunicator = value != null ? value.GetNetworkPeer() : (NetworkCommunicator) null;
        this.SetNetworkPeer(networkCommunicator);
        GameNetwork.BeginBroadcastModuleEvent();
        GameNetwork.WriteMessage((GameNetworkMessage) new SetAgentPeer(this, networkCommunicator));
        GameNetwork.EndBroadcastModuleEvent(GameNetwork.EventBroadcastFlags.None);
      }
    }

    public Mat3 LookRotation
    {
      get
      {
        Mat3 mat3;
        mat3.f = this.LookDirection;
        mat3.u = Vec3.Up;
        mat3.s = Vec3.CrossProduct(mat3.f, mat3.u);
        double num = (double) mat3.s.Normalize();
        mat3.u = Vec3.CrossProduct(mat3.s, mat3.f);
        return mat3;
      }
    }

    internal void OnDelete()
    {
      this.IsDeleted = true;
      this.MissionPeer = (MissionPeer) null;
    }

    internal void OnRemove()
    {
      this.IsRemoved = true;
      this._removalTime = MBCommon.GetTime(MBCommon.TimeType.Mission);
      this.Origin?.OnAgentRemoved(this.Health);
      if (this.Formation != null)
      {
        this.Formation.Team.DetachmentManager.OnAgentRemoved(this);
        this.Formation = (Formation) null;
      }
      if (this.IsUsingGameObject && !GameNetwork.IsClientOrReplay && (this.Mission != null && !this.Mission.MissionEnded()))
        this.StopUsingGameObject(false);
      foreach (AgentComponent component in this.Components)
        component.OnAgentRemoved();
    }

    public void RegisterBlow(Blow blow) => this.HandleBlow(ref blow);

    public Blow CreateBlowFromBlowAsReflection(Blow blow)
    {
      blow.InflictedDamage = blow.SelfInflictedDamage;
      blow.Position = this.Position;
      blow.BoneIndex = (sbyte) 0;
      blow.BlowFlag = BlowFlags.None;
      return blow;
    }

    private void HandleBlow(ref Blow b)
    {
      b.BaseMagnitude = Math.Min(b.BaseMagnitude, 1000f);
      Agent agent = b.OwnerId != -1 ? this.Mission.FindAgentWithIndex(b.OwnerId) : this;
      if (!b.BlowFlag.HasAnyFlag<BlowFlags>(BlowFlags.NoSound))
      {
        bool isCriticalBlow = b.IsBlowCrit(this.Monster.HitPoints * 4);
        bool isLowBlow = b.IsBlowLow(this.Monster.HitPoints);
        bool isOwnerHumanoid = agent != null && agent.IsHuman;
        bool isNonTipThrust = b.BlowFlag.HasAnyFlag<BlowFlags>(BlowFlags.NonTipThrust);
        int hitSound = b.WeaponRecord.GetHitSound(isOwnerHumanoid, isCriticalBlow, isLowBlow, isNonTipThrust, b.AttackType, b.DamageType);
        SoundEventParameter parameter = new SoundEventParameter("Armor Type", Agent.GetSoundParameterForArmorType(this.GetProtectorArmorMaterialOfBone(b.BoneIndex)));
        this.Mission.MakeSound(hitSound, b.Position, false, true, b.OwnerId, this.Index, ref parameter);
        if (b.IsMissile && agent != null)
          this.Mission.MakeSoundOnlyOnRelatedPeer(CombatSoundContainer.SoundCodeMissionCombatPlayerhit, b.Position, agent.Index);
        this.Mission.AddSoundAlarmFactorToAgents(b.OwnerId, b.Position, 15f);
      }
      if (b.InflictedDamage <= 0)
        return;
      b.DamagedPercentage = (float) b.InflictedDamage / this.HealthLimit;
      this.UpdateLastAttackAndHitTimes(agent, b.IsMissile);
      float num1 = this.Health - (float) b.InflictedDamage;
      if ((double) num1 < 0.0)
        num1 = 0.0f;
      if (!this.Invulnerable && !Mission.DisableDying)
        this.Health = num1;
      int slotOrMissileIndex = b.WeaponRecord.AffectorWeaponSlotOrMissileIndex;
      float hitDistance = b.IsMissile ? (b.Position - b.WeaponRecord.StartingPosition).Length : 0.0f;
      double num2 = (double) this.Mission.OnAgentHit(this, agent, slotOrMissileIndex, b.IsMissile, false, b.InflictedDamage, b.MovementSpeedDamageModifier, hitDistance, b.AttackType, b.VictimBodyPart);
      if ((double) this.Health < 1.0)
        this.Die(b);
      this.HandleBlowAux(ref b);
      float duration = -1f;
      if (this.IsMainAgent)
        duration = 0.5f;
      else if (this.RiderAgent != null && this.RiderAgent.IsMainAgent)
        duration = 0.2f;
      if ((double) duration <= 0.0)
        return;
      EngineApplicationInterface.IInput.VibrateControllerMotors((float) (0.00499999988824129 * (double) b.InflictedDamage + 0.300000011920929), (float) (0.00499999988824129 * (double) b.InflictedDamage + 0.300000011920929), duration);
    }

    private ArmorComponent.ArmorMaterialTypes GetProtectorArmorMaterialOfBone(
      sbyte boneIndex)
    {
      if (boneIndex >= (sbyte) 0)
      {
        EquipmentIndex index = EquipmentIndex.None;
        switch (this.AgentVisuals.GetBoneTypeData(boneIndex).BodyPartType)
        {
          case BoneBodyPartType.Head:
          case BoneBodyPartType.Neck:
            index = EquipmentIndex.NumAllWeaponSlots;
            break;
          case BoneBodyPartType.Chest:
          case BoneBodyPartType.Abdomen:
          case BoneBodyPartType.ShoulderLeft:
          case BoneBodyPartType.ShoulderRight:
            index = EquipmentIndex.Body;
            break;
          case BoneBodyPartType.BipedalArmLeft:
          case BoneBodyPartType.BipedalArmRight:
          case BoneBodyPartType.QuadrupedalArmLeft:
          case BoneBodyPartType.QuadrupedalArmRight:
            index = EquipmentIndex.Gloves;
            break;
          case BoneBodyPartType.BipedalLegs:
          case BoneBodyPartType.QuadrupedalLegs:
            index = EquipmentIndex.Leg;
            break;
        }
        if (index != EquipmentIndex.None)
        {
          EquipmentElement equipmentElement = this.SpawnEquipment[index];
          if (equipmentElement.Item != null)
          {
            equipmentElement = this.SpawnEquipment[index];
            return equipmentElement.Item.ArmorComponent.MaterialType;
          }
        }
      }
      return ArmorComponent.ArmorMaterialTypes.None;
    }

    private static float GetSoundParameterForArmorType(
      ArmorComponent.ArmorMaterialTypes armorMaterialType)
    {
      return (float) armorMaterialType * 0.1f;
    }

    public BasicCharacterObject Character
    {
      get => this._character;
      set
      {
        this._character = value;
        if (value == null)
          return;
        this.Health = (float) this._character.HitPoints;
        this.BaseHealthLimit = (float) this._character.MaxHitPoints();
        this.HealthLimit = this.BaseHealthLimit;
        this._name = value.Name;
        this.IsFemale = value.IsFemale;
      }
    }

    internal void Tick(float dt)
    {
      if (this.IsActive())
      {
        if (this._checkIfTargetFrameIsChanged)
        {
          Vec2 vecFrom1 = this.MovementLockedState != AgentMovementLockedState.None ? this.GetTargetPosition() : this.LookFrame.origin.AsVec2;
          Vec3 vecFrom2 = this.MovementLockedState != AgentMovementLockedState.None ? this.GetTargetDirection() : this.LookFrame.rotation.f;
          switch (this.MovementLockedState)
          {
            case AgentMovementLockedState.PositionLocked:
              this._checkIfTargetFrameIsChanged = this._lastSynchedTargetPosition != vecFrom1;
              break;
            case AgentMovementLockedState.FrameLocked:
              this._checkIfTargetFrameIsChanged = this._lastSynchedTargetPosition != vecFrom1 || this._lastSynchedTargetDirection != vecFrom2;
              break;
          }
          if (this._checkIfTargetFrameIsChanged)
          {
            if (this.MovementLockedState == AgentMovementLockedState.FrameLocked)
              this.SetTargetPositionAndDirection(MBMath.Lerp(vecFrom1, this._lastSynchedTargetPosition, 5f * dt, 0.005f), MBMath.Lerp(vecFrom2, this._lastSynchedTargetDirection, 5f * dt, 0.005f));
            else
              this.SetTargetPosition(MBMath.Lerp(vecFrom1, this._lastSynchedTargetPosition, 5f * dt, 0.005f));
          }
        }
        if (this.Mission.AllowAiTicking && this.IsAIControlled)
          this.TickAsAI(dt);
        if (!this._wantsToYell)
          return;
        if ((double) this._yellTimer > 0.0)
        {
          this._yellTimer -= dt;
        }
        else
        {
          this.MakeVoice(this.MountAgent != null ? SkinVoiceManager.VoiceType.HorseRally : SkinVoiceManager.VoiceType.Yell, SkinVoiceManager.CombatVoiceNetworkPredictionType.NoPrediction);
          this._wantsToYell = false;
        }
      }
      else
      {
        if (this.MissionPeer?.ControlledAgent != this || this.IsCameraAttachable())
          return;
        this.MissionPeer.ControlledAgent = (Agent) null;
      }
    }

    [Conditional("DEBUG")]
    public void DebugMore() => MBAPI.IMBAgent.DebugMore(this.GetPtr());

    private void TickAsAI(float dt)
    {
      for (int index = 0; index < this._agentComponentList.Count; ++index)
        this._agentComponentList[index].OnTickAsAI(dt);
      if (this.Formation == null || !this._cachedAndFormationValuesUpdateTimer.Check(MBCommon.GetTime(MBCommon.TimeType.Mission)))
        return;
      this.UpdateCachedAndFormationValues(false, true);
    }

    public UsableMachine GetSteppedMachine()
    {
      GameEntity gameEntity = this.GetSteppedEntity();
      while ((NativeObject) gameEntity != (NativeObject) null && !gameEntity.HasScriptOfType<UsableMachine>())
        gameEntity = gameEntity.Parent;
      return (NativeObject) gameEntity != (NativeObject) null ? gameEntity.GetFirstScriptOfType<UsableMachine>() : (UsableMachine) null;
    }

    IFormationArrangement IFormationUnit.Formation => this._formation.arrangement;

    public bool CanBackStab(Agent agent) => false;

    public bool CanThrustAttackStickToBone(sbyte boneIndex)
    {
      if (this.IsHuman)
      {
        Agent.AgentBoneMapArray boneMappingArray = this.BoneMappingArray;
        sbyte[] numArray = new sbyte[11]
        {
          boneMappingArray[HumanBone.Abdomen],
          boneMappingArray[HumanBone.ThighL],
          boneMappingArray[HumanBone.ThighR],
          boneMappingArray[HumanBone.Spine1],
          boneMappingArray[HumanBone.Spine2],
          boneMappingArray[HumanBone.Thorax],
          boneMappingArray[HumanBone.Neck],
          boneMappingArray[HumanBone.ShoulderL],
          boneMappingArray[HumanBone.ShoulderR],
          boneMappingArray[HumanBone.UpperarmL],
          boneMappingArray[HumanBone.UpperarmR]
        };
        foreach (int num in numArray)
        {
          if ((int) boneIndex == num)
            return true;
        }
      }
      return false;
    }

    public bool CheckSkillForMounting(Agent mountAgent)
    {
      int skillValue = this.Character.GetSkillValue(DefaultSkills.Riding);
      return (this.GetAgentFlags() & AgentFlag.CanRide) > AgentFlag.None && (double) skillValue >= (double) mountAgent.GetAgentDrivenPropertyValue(DrivenProperty.MountDifficulty);
    }

    public void Mount(Agent mountAgent)
    {
      bool flag = mountAgent.GetCurrentActionType(0) == Agent.ActionCodeType.Rear;
      if (this.MountAgent == null && mountAgent.RiderAgent == null)
      {
        if (!this.CheckSkillForMounting(mountAgent) || flag || !(this.GetCurrentAction(0) == ActionIndexCache.act_none))
          return;
        this.EventControlFlags |= Agent.EventControlFlag.Mount;
        this.SetInteractionAgent(mountAgent);
      }
      else
      {
        if (this.MountAgent != mountAgent || flag)
          return;
        this.EventControlFlags |= Agent.EventControlFlag.Dismount;
      }
    }

    [MBCallback]
    internal void OnMount(Agent mount)
    {
      if (!GameNetwork.IsClientOrReplay)
      {
        if (mount.IsAIControlled && mount.IsRetreating(false))
          mount.StopRetreatingMoraleComponent();
        this.CheckToDropFlaggedItem();
      }
      if (this.HasBeenBuilt)
      {
        foreach (AgentComponent agentComponent in this._agentComponentList)
          agentComponent.OnMount(mount);
        this.Mission.OnAgentMount(this);
      }
      this.UpdateAgentStats();
      Action mountedStateChanged = this.OnAgentMountedStateChanged;
      if (mountedStateChanged != null)
        mountedStateChanged();
      if (!GameNetwork.IsServerOrRecorder)
        return;
      mount.SyncHealthToClient();
    }

    [MBCallback]
    internal void OnDismount(Agent mount)
    {
      if (!GameNetwork.IsClientOrReplay)
      {
        this.Formation?.OnAgentLostMount(this);
        this.CheckToDropFlaggedItem();
      }
      foreach (AgentComponent agentComponent in this._agentComponentList)
        agentComponent.OnDismount(mount);
      this.Mission.OnAgentDismount(this);
      if (!this.IsActive())
        return;
      this.UpdateAgentStats();
      Action mountedStateChanged = this.OnAgentMountedStateChanged;
      if (mountedStateChanged == null)
        return;
      mountedStateChanged();
    }

    public void OnItemPickup(
      SpawnedItemEntity spawnedItemEntity,
      EquipmentIndex weaponPickUpSlotIndex,
      out bool removeWeapon)
    {
      removeWeapon = true;
      bool flag1 = true;
      MissionWeapon weaponCopy = spawnedItemEntity.WeaponCopy;
      if (weaponPickUpSlotIndex == EquipmentIndex.None)
        weaponPickUpSlotIndex = MissionEquipment.SelectWeaponPickUpSlot(this, weaponCopy, spawnedItemEntity.IsStuckMissile());
      bool flag2 = false;
      MissionWeapon missionWeapon;
      if (weaponPickUpSlotIndex == EquipmentIndex.Weapon4)
      {
        if (!GameNetwork.IsClientOrReplay)
        {
          flag2 = true;
          missionWeapon = this.Equipment[weaponPickUpSlotIndex];
          if (!missionWeapon.IsEmpty)
          {
            int num = (int) weaponPickUpSlotIndex;
            missionWeapon = this.Equipment[weaponPickUpSlotIndex];
            int weaponClass = (int) missionWeapon.Item.PrimaryWeapon.WeaponClass;
            this.DropItem((EquipmentIndex) num, (WeaponClass) weaponClass);
          }
        }
      }
      else if (weaponPickUpSlotIndex != EquipmentIndex.None)
      {
        int val1 = 0;
        if ((spawnedItemEntity.IsStuckMissile() || spawnedItemEntity.WeaponCopy.IsAnyConsumable(out WeaponComponentData _)) && (!this.Equipment[weaponPickUpSlotIndex].IsEmpty && this.Equipment[weaponPickUpSlotIndex].IsSameType(weaponCopy)) && this.Equipment[weaponPickUpSlotIndex].IsAnyConsumable(out WeaponComponentData _))
          val1 = (int) this.Equipment[weaponPickUpSlotIndex].ModifiedMaxAmount - (int) this.Equipment[weaponPickUpSlotIndex].Amount;
        if (val1 > 0)
        {
          short consumedAmount = (short) Math.Min(val1, (int) weaponCopy.Amount);
          if ((int) consumedAmount != (int) weaponCopy.Amount)
          {
            removeWeapon = false;
            if (!GameNetwork.IsClientOrReplay)
            {
              spawnedItemEntity.ConsumeWeaponAmount(consumedAmount);
              if (GameNetwork.IsServer)
              {
                GameNetwork.BeginBroadcastModuleEvent();
                GameNetwork.WriteMessage((GameNetworkMessage) new ConsumeWeaponAmount(spawnedItemEntity, consumedAmount));
                GameNetwork.EndBroadcastModuleEvent(GameNetwork.EventBroadcastFlags.None);
              }
            }
          }
          if (!GameNetwork.IsClientOrReplay)
          {
            int num1 = (int) weaponPickUpSlotIndex;
            missionWeapon = this.Equipment[weaponPickUpSlotIndex];
            int num2 = (int) (short) ((int) missionWeapon.Amount + (int) consumedAmount);
            this.SetWeaponAmountInSlot((EquipmentIndex) num1, (short) num2, true);
            if (this.GetWieldedItemIndex(Agent.HandIndex.MainHand) == EquipmentIndex.None && (weaponCopy.Item.PrimaryWeapon.IsRangedWeapon || weaponCopy.Item.PrimaryWeapon.IsMeleeWeapon))
              flag2 = true;
          }
        }
        else if (!GameNetwork.IsClientOrReplay)
        {
          flag2 = true;
          missionWeapon = this.Equipment[weaponPickUpSlotIndex];
          if (!missionWeapon.IsEmpty)
            this.DropItem(weaponPickUpSlotIndex, weaponCopy.Item.PrimaryWeapon.WeaponClass);
        }
      }
      if (!GameNetwork.IsClientOrReplay)
      {
        flag1 = MissionEquipment.DoesWeaponFitToSlot(weaponPickUpSlotIndex, weaponCopy);
        if (flag1)
        {
          this.EquipWeaponFromSpawnedItemEntity(weaponPickUpSlotIndex, spawnedItemEntity, removeWeapon);
          if (flag2)
          {
            EquipmentIndex slotIndex = weaponPickUpSlotIndex;
            if (weaponCopy.Item.PrimaryWeapon.AmmoClass == weaponCopy.Item.PrimaryWeapon.WeaponClass)
            {
              for (EquipmentIndex index = EquipmentIndex.WeaponItemBeginSlot; index < weaponPickUpSlotIndex; ++index)
              {
                missionWeapon = this.Equipment[index];
                if (!missionWeapon.IsEmpty && weaponCopy.IsEqualTo(this.Equipment[index]))
                {
                  slotIndex = index;
                  break;
                }
              }
            }
            this.TryToWieldWeaponInSlot(slotIndex, Agent.WeaponWieldActionType.InstantAfterPickUp, false);
          }
          for (int index = 0; index < this._agentComponentList.Count; ++index)
            this._agentComponentList[index].OnItemPickup(spawnedItemEntity);
          if (this.Controller == Agent.ControllerType.AI)
            this.GetComponent<ItemPickupAgentComponent>().ItemPickupDone(spawnedItemEntity);
        }
      }
      if (!flag1)
        return;
      foreach (MissionBehaviour missionBehaviour in this.Mission.MissionBehaviours)
        missionBehaviour.OnItemPickup(this, spawnedItemEntity);
    }

    public void EquipWeaponToExtraSlotAndWield(ref MissionWeapon weapon)
    {
      if (!this.Equipment[EquipmentIndex.Weapon4].IsEmpty)
        this.DropItem(EquipmentIndex.Weapon4);
      this.EquipWeaponWithNewEntity(EquipmentIndex.Weapon4, ref weapon);
      this.TryToWieldWeaponInSlot(EquipmentIndex.Weapon4, Agent.WeaponWieldActionType.InstantAfterPickUp, false);
    }

    public void RemoveEquippedWeapon(EquipmentIndex slotIndex)
    {
      if (GameNetwork.IsServerOrRecorder)
      {
        GameNetwork.BeginBroadcastModuleEvent();
        GameNetwork.WriteMessage((GameNetworkMessage) new NetworkMessages.FromServer.RemoveEquippedWeapon(this, slotIndex));
        GameNetwork.EndBroadcastModuleEvent(GameNetwork.EventBroadcastFlags.AddToMissionRecord);
      }
      this.Equipment[slotIndex] = MissionWeapon.Invalid;
      this.WeaponEquipped(slotIndex, in WeaponData.InvalidWeaponData, (WeaponStatsData[]) null, in WeaponData.InvalidWeaponData, (WeaponStatsData[]) null, (GameEntity) null, true, false);
      this.UpdateAgentProperties();
    }

    public void UpdateSpawnEquipmentAndRefreshVisuals(TaleWorlds.Core.Equipment newSpawnEquipment)
    {
      this.SpawnEquipment = newSpawnEquipment;
      this.AgentVisuals.ClearVisualComponents(false);
      this.Mission.OnEquipItemsFromSpawnEquipment(this, Agent.CreationType.FromCharacterObj);
      this.AgentVisuals.ClearAllWeaponMeshes();
      this.Equipment.FillFrom(this.SpawnEquipment, this.Origin?.Banner);
      this.CheckEquipmentForCapeClothSimulationStateChange();
      this.EquipItemsFromSpawnEquipment();
      this.UpdateAgentProperties();
    }

    public void EquipWeaponWithNewEntity(EquipmentIndex slotIndex, ref MissionWeapon weapon)
    {
      if (GameNetwork.IsServerOrRecorder)
      {
        GameNetwork.BeginBroadcastModuleEvent();
        GameNetwork.WriteMessage((GameNetworkMessage) new NetworkMessages.FromServer.EquipWeaponWithNewEntity(this, slotIndex, weapon));
        GameNetwork.EndBroadcastModuleEvent(GameNetwork.EventBroadcastFlags.AddToMissionRecord);
      }
      this.Equipment[slotIndex] = weapon;
      WeaponData weaponData = WeaponData.InvalidWeaponData;
      WeaponStatsData[] weaponStatsData = (WeaponStatsData[]) null;
      WeaponData ammoWeaponData = WeaponData.InvalidWeaponData;
      WeaponStatsData[] ammoWeaponStatsData = (WeaponStatsData[]) null;
      if (!weapon.IsEmpty)
      {
        weaponData = weapon.GetWeaponData(true);
        weaponStatsData = weapon.GetWeaponStatsData();
        ammoWeaponData = weapon.GetAmmoWeaponData(true);
        ammoWeaponStatsData = weapon.GetAmmoWeaponStatsData();
      }
      this.WeaponEquipped(slotIndex, in weaponData, weaponStatsData, in ammoWeaponData, ammoWeaponStatsData, (GameEntity) null, true, true);
      weaponData.DeinitializeManagedPointers();
      ammoWeaponData.DeinitializeManagedPointers();
      for (int attachmentIndex = 0; attachmentIndex < weapon.GetAttachedWeaponsCount(); ++attachmentIndex)
      {
        MissionWeapon attachedWeapon = weapon.GetAttachedWeapon(attachmentIndex);
        MatrixFrame attachedWeaponFrame = weapon.GetAttachedWeaponFrame(attachmentIndex);
        if (GameNetwork.IsServerOrRecorder)
        {
          GameNetwork.BeginBroadcastModuleEvent();
          GameNetwork.WriteMessage((GameNetworkMessage) new AttachWeaponToWeaponInAgentEquipmentSlot(attachedWeapon, this, slotIndex, attachedWeaponFrame));
          GameNetwork.EndBroadcastModuleEvent(GameNetwork.EventBroadcastFlags.AddToMissionRecord);
        }
        this.AttachWeaponToWeaponAux(slotIndex, ref attachedWeapon, (GameEntity) null, ref attachedWeaponFrame);
      }
      this.UpdateAgentProperties();
    }

    public void EquipWeaponFromSpawnedItemEntity(
      EquipmentIndex slotIndex,
      SpawnedItemEntity spawnedItemEntity,
      bool removeWeapon)
    {
      if (GameNetwork.IsServerOrRecorder)
      {
        GameNetwork.BeginBroadcastModuleEvent();
        GameNetwork.WriteMessage((GameNetworkMessage) new NetworkMessages.FromServer.EquipWeaponFromSpawnedItemEntity(this, slotIndex, spawnedItemEntity, removeWeapon));
        GameNetwork.EndBroadcastModuleEvent(GameNetwork.EventBroadcastFlags.AddToMissionRecord);
      }
      MissionWeapon weaponCopy1;
      if ((NativeObject) spawnedItemEntity.GameEntity.Parent != (NativeObject) null && spawnedItemEntity.GameEntity.Parent.HasScriptOfType<SpawnedItemEntity>())
      {
        SpawnedItemEntity firstScriptOfType = spawnedItemEntity.GameEntity.Parent.GetFirstScriptOfType<SpawnedItemEntity>();
        int attachmentIndex = -1;
        for (int index = 0; index < firstScriptOfType.GameEntity.ChildCount; ++index)
        {
          if ((NativeObject) firstScriptOfType.GameEntity.GetChild(index) == (NativeObject) spawnedItemEntity.GameEntity)
          {
            attachmentIndex = index;
            break;
          }
        }
        if (GameNetwork.IsClientOrReplay)
          TaleWorlds.Library.Debug.Print("EquipWeaponFromSpawnedItemEntity: Has spawned item as parent, as childIndex: " + (object) attachmentIndex);
        weaponCopy1 = firstScriptOfType.WeaponCopy;
        weaponCopy1.RemoveAttachedWeapon(attachmentIndex);
      }
      if (!removeWeapon)
        return;
      weaponCopy1 = this.Equipment[slotIndex];
      if (!weaponCopy1.IsEmpty)
      {
        if (GameNetwork.IsClientOrReplay)
          TaleWorlds.Library.Debug.Print("EquipWeaponFromSpawnedItemEntity: Removing weapon, slot is not empty.");
        spawnedItemEntity.GameEntity.Remove(73);
      }
      else
      {
        if (GameNetwork.IsClientOrReplay)
          TaleWorlds.Library.Debug.Print("EquipWeaponFromSpawnedItemEntity: Removing weapon, slot is empty.");
        GameEntity gameEntity = spawnedItemEntity.GameEntity;
        gameEntity.RemovePhysics();
        gameEntity.RemoveScriptComponent(spawnedItemEntity.ScriptComponent.Pointer, 10);
        gameEntity.SetVisibilityExcludeParents(true);
        MissionWeapon weaponCopy2 = spawnedItemEntity.WeaponCopy;
        this.Equipment[slotIndex] = weaponCopy2;
        WeaponData weaponData = weaponCopy2.GetWeaponData(true);
        WeaponStatsData[] weaponStatsData = weaponCopy2.GetWeaponStatsData();
        WeaponData ammoWeaponData = weaponCopy2.GetAmmoWeaponData(true);
        WeaponStatsData[] ammoWeaponStatsData = weaponCopy2.GetAmmoWeaponStatsData();
        this.WeaponEquipped(slotIndex, in weaponData, weaponStatsData, in ammoWeaponData, ammoWeaponStatsData, gameEntity, true, false);
        weaponData.DeinitializeManagedPointers();
        for (int attachmentIndex = 0; attachmentIndex < weaponCopy2.GetAttachedWeaponsCount(); ++attachmentIndex)
        {
          MatrixFrame attachedWeaponFrame = weaponCopy2.GetAttachedWeaponFrame(attachmentIndex);
          MissionWeapon attachedWeapon = weaponCopy2.GetAttachedWeapon(attachmentIndex);
          this.AttachWeaponToWeaponAux(slotIndex, ref attachedWeapon, (GameEntity) null, ref attachedWeaponFrame);
        }
        this.UpdateAgentProperties();
      }
    }

    public void PreloadForRendering() => this.PreloadForRenderingAux();

    internal void SetMountInitialValues(TextObject name, string horseCreationKey)
    {
      this._name = name;
      this.HorseCreationKey = horseCreationKey;
    }

    internal void SetInitialAgentScale(float initialScale) => this.SetInitialAgentScaleAux(initialScale);

    public bool IsCameraAttachable() => !this.IsDeleted && (!this.IsRemoved || (double) this._removalTime + 2.09999990463257 > (double) MBCommon.GetTime(MBCommon.TimeType.Mission)) && this.IsHuman && (NativeObject) this.AgentVisuals?.GetEntity() != (NativeObject) null;

    internal bool GetHasRangedWeapon(bool checkHasAmmo = false)
    {
      for (EquipmentIndex equipmentIndex = EquipmentIndex.WeaponItemBeginSlot; equipmentIndex < EquipmentIndex.NumAllWeaponSlots; ++equipmentIndex)
      {
        if (!this.Equipment[equipmentIndex].IsEmpty && (this.Equipment[equipmentIndex].GetRangedUsageIndex() >= 0 && (!checkHasAmmo || this.Equipment.HasAmmo(equipmentIndex, out int _, out bool _, out bool _))))
          return true;
      }
      return false;
    }

    private void GatherInformationFromEquipment(
      out bool hasMeleeWeapon,
      out bool hasShield,
      out bool hasPolearm,
      out bool hasNonConsumableRangedWeapon,
      out bool hasNonConsumableRangedWeaponWithAmmo,
      out bool hasThrownWeapon)
    {
      hasMeleeWeapon = false;
      hasShield = false;
      hasPolearm = false;
      hasNonConsumableRangedWeapon = false;
      hasNonConsumableRangedWeaponWithAmmo = false;
      hasThrownWeapon = false;
      for (EquipmentIndex index = EquipmentIndex.WeaponItemBeginSlot; index < EquipmentIndex.NumAllWeaponSlots; ++index)
      {
        bool weaponHasMelee;
        bool weaponHasShield;
        bool weaponHasPolearm;
        bool weaponHasNonConsumableRanged;
        bool weaponHasThrown;
        WeaponClass rangedAmmoClass;
        this.Equipment[index].GatherInformationFromWeapon(out weaponHasMelee, out weaponHasShield, out weaponHasPolearm, out weaponHasNonConsumableRanged, out weaponHasThrown, out rangedAmmoClass);
        hasMeleeWeapon |= weaponHasMelee;
        hasShield |= weaponHasShield;
        hasPolearm |= weaponHasPolearm;
        hasNonConsumableRangedWeapon |= weaponHasNonConsumableRanged;
        hasThrownWeapon |= weaponHasThrown;
        if (weaponHasNonConsumableRanged)
          hasNonConsumableRangedWeaponWithAmmo = hasNonConsumableRangedWeaponWithAmmo || this.Equipment.GetAmmoAmount(rangedAmmoClass) > 0;
      }
    }

    public int AddSynchedPrefabComponentToBone(string prefabName, sbyte boneIndex)
    {
      if (this._synchedBodyComponents == null)
        this._synchedBodyComponents = new List<CompositeComponent>();
      if (!GameEntity.PrefabExists(prefabName))
      {
        MBDebug.ShowWarning("Missing prefab for agent logic :" + prefabName);
        prefabName = "rock_001";
      }
      CompositeComponent bone = this.AddPrefabComponentToBone(prefabName, boneIndex);
      int count = this._synchedBodyComponents.Count;
      this._synchedBodyComponents.Add(bone);
      if (!GameNetwork.IsServerOrRecorder)
        return count;
      GameNetwork.BeginBroadcastModuleEvent();
      GameNetwork.WriteMessage((GameNetworkMessage) new AddPrefabComponentToAgentBone(this, prefabName, boneIndex));
      GameNetwork.EndBroadcastModuleEvent(GameNetwork.EventBroadcastFlags.AddToMissionRecord);
      return count;
    }

    public void SetSynchedPrefabComponentVisibility(int componentIndex, bool visibility)
    {
      this._synchedBodyComponents[componentIndex].SetVisible(visibility);
      this.AgentVisuals.LazyUpdateAgentRendererData();
      if (!GameNetwork.IsServerOrRecorder)
        return;
      GameNetwork.BeginBroadcastModuleEvent();
      GameNetwork.WriteMessage((GameNetworkMessage) new SetAgentPrefabComponentVisibility(this, componentIndex, visibility));
      GameNetwork.EndBroadcastModuleEvent(GameNetwork.EventBroadcastFlags.AddToMissionRecord);
    }

    public bool IsSynchedPrefabComponentVisible(int componentIndex) => this._synchedBodyComponents[componentIndex].GetVisible();

    public void SetActionSet(
      ref AgentVisualsNativeData agentVisualsNativeData,
      ref AnimationSystemData animationSystemData)
    {
      MBAPI.IMBAgent.SetActionSet(this.GetPtr(), ref agentVisualsNativeData, ref animationSystemData);
      if (!GameNetwork.IsServerOrRecorder)
        return;
      GameNetwork.BeginBroadcastModuleEvent();
      GameNetwork.WriteMessage((GameNetworkMessage) new SetAgentActionSet(this, animationSystemData));
      GameNetwork.EndBroadcastModuleEvent(GameNetwork.EventBroadcastFlags.AddToMissionRecord);
    }

    internal void GetFormationFileAndRankInfo(
      out int fileIndex,
      out int rankIndex,
      out int fileCount,
      out int rankCount)
    {
      IFormationUnit formationUnit = (IFormationUnit) this;
      fileIndex = formationUnit.FormationFileIndex;
      rankIndex = formationUnit.FormationRankIndex;
      if (formationUnit.Formation is LineFormation formation)
      {
        formation.GetFormationInfo(out fileCount, out rankCount);
      }
      else
      {
        fileCount = -1;
        rankCount = -1;
      }
    }

    [MBCallback]
    internal void OnAgentAlarmedStateChanged(Agent.AIStateFlag flag)
    {
      foreach (MissionBehaviour missionBehaviour in Mission.Current.MissionBehaviours)
        missionBehaviour.OnAgentAlarmedStateChanged(this, flag);
    }

    internal Vec2 GetWallDirectionOfRelativeFormationLocation() => this.Formation.GetWallDirectionOfRelativeFormationLocation(this);

    [MBCallback]
    internal bool IsInSameFormationWith(Agent otherAgent)
    {
      Formation formation = otherAgent.Formation;
      return this.Formation != null && formation != null && this.Formation == formation;
    }

    [MBCallback]
    internal int GetFormationUnitSpacing() => this.Formation.UnitSpacing;

    [MBCallback]
    public string GetSoundAndCollisionInfoClassName() => this.Monster.SoundAndCollisionInfoClassName;

    [MBCallback]
    public void UpdateAgentStats()
    {
      if (!this.IsActive())
        return;
      this.UpdateAgentProperties();
    }

    internal void ResetAiWaitBeforeShootFactor() => this._propertyModifiers.resetAiWaitBeforeShootFactor = true;

    public void SetHandInverseKinematicsFrameFromActionChannel(
      int channelIndex,
      MatrixFrame localIKFrame,
      MatrixFrame boundEntityGlobalFrame,
      float animationHeightDifference = 0.0f)
    {
      if (this.GetCurrentAction(channelIndex) != ActionIndexCache.act_none && (double) this.GetActionChannelWeight(channelIndex) > 0.0)
      {
        MBAgentVisuals agentVisuals = this.AgentVisuals;
        Skeleton skeleton = agentVisuals.GetSkeleton();
        MatrixFrame globalFrame = agentVisuals.GetGlobalFrame();
        MatrixFrame leftGlobalFrame;
        leftGlobalFrame.origin = skeleton.GetBoneEntitialFrameAtChannel(channelIndex, this.Monster.OffHandItemBoneIndex).origin;
        MatrixFrame rightGlobalFrame;
        rightGlobalFrame.origin = skeleton.GetBoneEntitialFrameAtChannel(channelIndex, this.Monster.MainHandItemBoneIndex).origin;
        if ((double) Math.Abs(animationHeightDifference) > 9.99999974737875E-05)
        {
          leftGlobalFrame.origin.z += animationHeightDifference;
          rightGlobalFrame.origin.z += animationHeightDifference;
        }
        leftGlobalFrame.origin = localIKFrame.TransformToLocal(leftGlobalFrame.origin);
        rightGlobalFrame.origin = localIKFrame.TransformToLocal(rightGlobalFrame.origin);
        leftGlobalFrame.origin = boundEntityGlobalFrame.TransformToParent(leftGlobalFrame.origin);
        rightGlobalFrame.origin = boundEntityGlobalFrame.TransformToParent(rightGlobalFrame.origin);
        leftGlobalFrame.rotation = skeleton.GetBoneEntitialFrameAtChannel(channelIndex, (sbyte) 19).rotation;
        leftGlobalFrame.rotation = globalFrame.rotation.TransformToParent(leftGlobalFrame.rotation);
        rightGlobalFrame.rotation = skeleton.GetBoneEntitialFrameAtChannel(channelIndex, (sbyte) 26).rotation;
        rightGlobalFrame.rotation = globalFrame.rotation.TransformToParent(rightGlobalFrame.rotation);
        this.SetHandInverseKinematicsFrame(ref leftGlobalFrame, ref rightGlobalFrame);
      }
      else
        this.ClearHandInverseKinematics();
    }

    public float GetArmLength() => this.Monster.ArmLength * this.AgentScale;

    public float GetArmWeight() => this.Monster.ArmWeight * this.AgentScale;

    internal bool HasMeleeWeaponCached { get; private set; }

    internal bool HasShieldCached { get; private set; }

    internal bool HasSpearCached { get; private set; }

    internal bool HasThrownCached { get; private set; }

    private void Formation_OnShapeChanged()
    {
      if (!this.IsActive() || !this.IsAIControlled)
        return;
      this.FormationCohesionMarkAsDirty();
    }

    public bool IsEnemyOf(Agent otherAgent) => MBAPI.IMBAgent.IsEnemy(this.GetPtr(), otherAgent.GetPtr());

    public bool IsFriendOf(Agent otherAgent) => MBAPI.IMBAgent.IsFriend(this.GetPtr(), otherAgent.GetPtr());

    internal void UpdateCachedAndFormationValues(
      bool updateOnlyMovement,
      bool arrangementChangeAllowed)
    {
      if (!this.IsActive())
        return;
      if (!updateOnlyMovement)
      {
        Func<Agent, float> func = (Func<Agent, float>) (agent =>
        {
          Agent mountAgent = agent.MountAgent;
          return mountAgent == null ? agent.Monster.WalkingSpeedLimit : mountAgent.WalkingSpeedLimitOfMountable;
        });
        bool hasMeleeWeapon;
        bool hasShield;
        bool hasPolearm;
        bool hasNonConsumableRangedWeaponWithAmmo;
        bool hasThrownWeapon;
        this.GatherInformationFromEquipment(out hasMeleeWeapon, out hasShield, out hasPolearm, out bool _, out hasNonConsumableRangedWeaponWithAmmo, out hasThrownWeapon);
        this.HasMeleeWeaponCached = hasMeleeWeapon;
        this.HasShieldCached = hasShield;
        this.HasSpearCached = hasPolearm;
        this.IsRangedCached = hasNonConsumableRangedWeaponWithAmmo;
        this.HasThrownCached = hasThrownWeapon;
        BasicCharacterObject character = this.Character;
        this.CharPowerCached = character != null ? character.GetPower() : 0.0f;
        this.WalkSpeedCached = func(this);
        this.RunSpeedCached = this.MaximumForwardUnlimitedSpeed;
      }
      if (GameNetwork.IsClientOrReplay)
        return;
      if (!updateOnlyMovement && !this.IsDetachedFromFormation)
        this.Formation?.arrangement.OnTickOccasionallyOfUnit((IFormationUnit) this, arrangementChangeAllowed);
      if (this.IsAIControlled)
        this.UpdateFormationMovement();
      if (!updateOnlyMovement && this.Formation != null)
        this.Formation.Team.DetachmentManager.TickAgent(this);
      if (updateOnlyMovement || !this.IsAIControlled)
        return;
      this.UpdateFormationOrders();
      if (this.Formation == null)
        return;
      int fileIndex;
      int rankIndex;
      int fileCount;
      int rankCount;
      this.GetFormationFileAndRankInfo(out fileIndex, out rankIndex, out fileCount, out rankCount);
      Vec2 formationLocation = this.GetWallDirectionOfRelativeFormationLocation();
      MBAPI.IMBAgent.SetFormationInfo(this.GetPtr(), fileIndex, rankIndex, fileCount, rankCount, formationLocation, this.Formation.UnitSpacing);
    }

    public void SetWantsToYell()
    {
      this._wantsToYell = true;
      this._yellTimer = (float) ((double) MBRandom.RandomFloat * 0.300000011920929 + 0.100000001490116);
    }

    [MBCallback]
    internal void SetAgentAIPerformingRetreatBehavior(bool isAgentAIPerformingRetreatBehavior)
    {
      if (GameNetwork.IsClientOrReplay || this.Mission == null)
        return;
      this.IsRunningAway = isAgentAIPerformingRetreatBehavior;
    }

    [MBCallback]
    internal void OnRetreating()
    {
      if (GameNetwork.IsClientOrReplay || this.Mission == null || this.Mission.MissionEnded())
        return;
      if (this.IsUsingGameObject)
        this.StopUsingGameObject();
      foreach (AgentComponent agentComponent in this._agentComponentList)
        agentComponent.OnRetreating();
    }

    public void GetRunningSimulationDataUntilMaximumSpeedReached(
      ref float combatAccelerationTime,
      ref float maxSpeed,
      float[] speedValues)
    {
      MBAPI.IMBAgent.GetRunningSimulationDataUntilMaximumSpeedReached(this.GetPtr(), ref combatAccelerationTime, ref maxSpeed, speedValues);
    }

    public float GetBaseArmorEffectivenessForBodyPart(BoneBodyPartType bodyPart)
    {
      if (!this.IsHuman)
        return this.GetAgentDrivenPropertyValue(DrivenProperty.ArmorTorso);
      switch (bodyPart)
      {
        case BoneBodyPartType.None:
          return 0.0f;
        case BoneBodyPartType.Head:
        case BoneBodyPartType.Neck:
          return this.GetAgentDrivenPropertyValue(DrivenProperty.ArmorHead);
        case BoneBodyPartType.Chest:
        case BoneBodyPartType.ShoulderLeft:
        case BoneBodyPartType.ShoulderRight:
          return this.GetAgentDrivenPropertyValue(DrivenProperty.ArmorTorso);
        case BoneBodyPartType.BipedalArmLeft:
        case BoneBodyPartType.BipedalArmRight:
        case BoneBodyPartType.QuadrupedalArmLeft:
        case BoneBodyPartType.QuadrupedalArmRight:
          return this.GetAgentDrivenPropertyValue(DrivenProperty.ArmorArms);
        case BoneBodyPartType.BipedalLegs:
        case BoneBodyPartType.QuadrupedalLegs:
          return this.GetAgentDrivenPropertyValue(DrivenProperty.ArmorLegs);
        default:
          goto case BoneBodyPartType.Chest;
      }
    }

    public AITargetVisibilityState GetLastTargetVisibilityState() => (AITargetVisibilityState) MBAPI.IMBAgent.GetLastTargetVisibilityState(this.GetPtr());

    public float GetTargetRange() => MBAPI.IMBAgent.GetTargetRange(this.GetPtr());

    public float GetMissileRange() => MBAPI.IMBAgent.GetMissileRange(this.GetPtr());

    public ItemObject GetWeaponToReplaceOnQuickAction(
      SpawnedItemEntity spawnedItem,
      out EquipmentIndex possibleSlotIndex)
    {
      EquipmentIndex index = MissionEquipment.SelectWeaponPickUpSlot(this, spawnedItem.WeaponCopy, spawnedItem.IsStuckMissile());
      possibleSlotIndex = index;
      if (index != EquipmentIndex.None && !this.Equipment[index].IsEmpty)
      {
        MissionWeapon weaponCopy;
        if (spawnedItem.IsStuckMissile() || spawnedItem.WeaponCopy.IsAnyConsumable(out WeaponComponentData _))
        {
          int weaponClass1 = (int) this.Equipment[index].Item.PrimaryWeapon.WeaponClass;
          weaponCopy = spawnedItem.WeaponCopy;
          int weaponClass2 = (int) weaponCopy.Item.PrimaryWeapon.WeaponClass;
          if (weaponClass1 == weaponClass2)
          {
            weaponCopy = this.Equipment[index];
            int amount = (int) weaponCopy.Amount;
            weaponCopy = this.Equipment[index];
            int modifiedMaxAmount = (int) weaponCopy.ModifiedMaxAmount;
            if (amount != modifiedMaxAmount)
              goto label_5;
          }
        }
        weaponCopy = this.Equipment[index];
        return weaponCopy.Item;
      }
label_5:
      return (ItemObject) null;
    }

    public bool CanInteractableWeaponBePickedUp(SpawnedItemEntity spawnedItem)
    {
      EquipmentIndex possibleSlotIndex;
      return this.GetWeaponToReplaceOnQuickAction(spawnedItem, out possibleSlotIndex) != null || possibleSlotIndex == EquipmentIndex.None;
    }

    public bool CanQuickPickUp(SpawnedItemEntity spawnedItem) => MissionEquipment.SelectWeaponPickUpSlot(this, spawnedItem.WeaponCopy, spawnedItem.IsStuckMissile()) != EquipmentIndex.None;

    public bool WillDropWieldedShield(SpawnedItemEntity spawnedItem)
    {
      EquipmentIndex wieldedItemIndex = this.GetWieldedItemIndex(Agent.HandIndex.OffHand);
      if (wieldedItemIndex != EquipmentIndex.None)
      {
        MissionWeapon weaponCopy = spawnedItem.WeaponCopy;
        if (weaponCopy.CurrentUsageItem.WeaponFlags.HasAnyFlag<WeaponFlags>(WeaponFlags.NotUsableWithOneHand))
        {
          weaponCopy = spawnedItem.WeaponCopy;
          if (weaponCopy.HasAllUsagesWithAnyWeaponFlag(WeaponFlags.NotUsableWithOneHand))
          {
            bool flag = false;
            for (EquipmentIndex index = EquipmentIndex.WeaponItemBeginSlot; index < EquipmentIndex.Weapon4; ++index)
            {
              if (index != wieldedItemIndex)
              {
                weaponCopy = this.Equipment[index];
                if (!weaponCopy.IsEmpty)
                {
                  weaponCopy = this.Equipment[index];
                  if (weaponCopy.IsShield())
                  {
                    flag = true;
                    break;
                  }
                }
              }
            }
            if (flag)
              return true;
          }
        }
      }
      return false;
    }

    public bool HadSameTypeOfConsumableOrShieldOnSpawn(WeaponClass weaponClass)
    {
      for (EquipmentIndex index = EquipmentIndex.WeaponItemBeginSlot; index < EquipmentIndex.Weapon4; ++index)
      {
        if (!this.SpawnEquipment[index].IsEmpty)
        {
          foreach (WeaponComponentData weapon in (IEnumerable<WeaponComponentData>) this.SpawnEquipment[index].Item.Weapons)
          {
            if ((weapon.IsConsumable || weapon.IsShield) && weapon.WeaponClass == weaponClass)
              return true;
          }
        }
      }
      return false;
    }

    public Agent.Hitter GetAssistingHitter(MissionPeer killerPeer)
    {
      Agent.Hitter hitter1 = (Agent.Hitter) null;
      foreach (Agent.Hitter hitter2 in (IEnumerable<Agent.Hitter>) this.HitterList)
      {
        if (hitter2.HitterPeer != killerPeer && (hitter1 == null || (double) hitter2.Damage > (double) hitter1.Damage))
          hitter1 = hitter2;
      }
      return hitter1 != null && (double) hitter1.Damage >= 35.0 ? hitter1 : (Agent.Hitter) null;
    }

    internal void UpdateLastRangedAttackTimeDueToAnAttack(float newTime) => this.LastRangedAttackTime = newTime;

    public void SetCapeClothSimulator(GameEntityComponent clothSimulatorComponent) => this._capeClothSimulator = clothSimulatorComponent as ClothSimulatorComponent;

    public delegate void OnAgentHealthChangedDelegate(
      Agent agent,
      float oldHealth,
      float newHealth);

    public class Hitter
    {
      public const float AssistMinDamage = 35f;
      public readonly MissionPeer HitterPeer;
      public readonly bool IsFriendlyHit;
      public readonly int Time;

      public float Damage { get; private set; }

      public Hitter(MissionPeer peer, float damage, int time, bool isFriendlyHit)
      {
        this.HitterPeer = peer;
        this.Damage = damage;
        this.Time = time;
        this.IsFriendlyHit = isFriendlyHit;
      }

      public void IncreaseDamage(float amount) => this.Damage += amount;
    }

    public delegate void OnMainAgentWieldedItemChangeDelegate();

    public enum ActionStage
    {
      None = -1, // 0xFFFFFFFF
      AttackReady = 0,
      AttackQuickReady = 1,
      AttackRelease = 2,
      ReloadPhase1 = 3,
      ReloadPhase2 = 4,
      Defend = 5,
      DefendParry = 6,
      NumActionStages = 7,
    }

    [Flags]
    public enum AIScriptedFrameFlags
    {
      None = 0,
      GoToPosition = 1,
      NoAttack = 2,
      ConsiderRotation = 4,
      NeverSlowDown = 8,
      DoNotRun = 16, // 0x00000010
      GoWithoutMount = 32, // 0x00000020
      RangerCanMoveForClearTarget = 128, // 0x00000080
      InConversation = 256, // 0x00000100
      Crouch = 512, // 0x00000200
    }

    [Flags]
    public enum AISpecialCombatModeFlags
    {
      None = 0,
      AttackEntity = 1,
      SurroundAttackEntity = 2,
      IgnoreAmmoLimitForRangeCalculation = 1024, // 0x00000400
    }

    [Flags]
    public enum AIStateFlag
    {
      None = 0,
      Cautious = 1,
      Alarmed = 2,
      Paused = 4,
      UseObjectMoving = 8,
      UseObjectUsing = 16, // 0x00000010
      UseObjectWaiting = 32, // 0x00000020
      Guard = 64, // 0x00000040
      ColumnwiseFollow = 128, // 0x00000080
    }

    [EngineStruct("Agent_controller_type")]
    public enum ControllerType
    {
      None,
      AI,
      Player,
      Count,
    }

    public enum CreationType
    {
      Invalid,
      FromRoster,
      FromHorseObj,
      FromCharacterObj,
    }

    [Flags]
    public enum EventControlFlag : uint
    {
      Dismount = 1,
      Mount = 2,
      Rear = 4,
      Jump = 8,
      Wield0 = 16, // 0x00000010
      Wield1 = 32, // 0x00000020
      Wield2 = 64, // 0x00000040
      Wield3 = 128, // 0x00000080
      Sheath0 = 256, // 0x00000100
      Sheath1 = 512, // 0x00000200
      ToggleAlternativeWeapon = 1024, // 0x00000400
      ToggleWalk = 2048, // 0x00000800
      ToggleCrouch = 4096, // 0x00001000
      Kick = 8192, // 0x00002000
      DoubleTapToDirectionUp = 65536, // 0x00010000
      DoubleTapToDirectionDown = 131072, // 0x00020000
      DoubleTapToDirectionLeft = DoubleTapToDirectionDown | DoubleTapToDirectionUp, // 0x00030000
      DoubleTapToDirectionRight = 262144, // 0x00040000
      DoubleTapToDirectionMask = DoubleTapToDirectionRight | DoubleTapToDirectionLeft, // 0x00070000
    }

    public enum FacialAnimChannel
    {
      High,
      Mid,
      Low,
      num_facial_anim_channels,
    }

    public struct AgentPropertiesModifiers
    {
      public bool resetAiWaitBeforeShootFactor;
    }

    public enum ActionCodeType
    {
      Other = 0,
      CombatAllBegin = 1,
      DefendAllBegin = 1,
      DefendFist = 1,
      DefendShield = 2,
      DefendForward2h = 3,
      DefendUp2h = 4,
      DefendRight2h = 5,
      DefendLeft2h = 6,
      DefendForward1h = 7,
      DefendUp1h = 8,
      DefendRight1h = 9,
      DefendLeft1h = 10, // 0x0000000A
      DefendForwardStaff = 11, // 0x0000000B
      DefendUpStaff = 12, // 0x0000000C
      DefendRightStaff = 13, // 0x0000000D
      DefendLeftStaff = 14, // 0x0000000E
      DefendAllEnd = 15, // 0x0000000F
      ReadyRanged = 15, // 0x0000000F
      ReleaseRanged = 16, // 0x00000010
      ReleaseThrowing = 17, // 0x00000011
      Reload = 18, // 0x00000012
      AttackMeleeAllBegin = 19, // 0x00000013
      ReadyMelee = 19, // 0x00000013
      ReleaseMelee = 20, // 0x00000014
      ParriedMelee = 21, // 0x00000015
      BlockedMelee = 22, // 0x00000016
      AttackMeleeAllEnd = 23, // 0x00000017
      CombatAllEnd = 23, // 0x00000017
      Fall = 23, // 0x00000017
      JumpAllBegin = 24, // 0x00000018
      JumpStart = 24, // 0x00000018
      Jump = 25, // 0x00000019
      JumpEnd = 26, // 0x0000001A
      JumpEndHard = 27, // 0x0000001B
      JumpAllEnd = 28, // 0x0000001C
      Kick = 28, // 0x0000001C
      WeaponBash = 29, // 0x0000001D
      PassiveUsage = 30, // 0x0000001E
      EquipUnequip = 31, // 0x0000001F
      Idle = 32, // 0x00000020
      Guard = 33, // 0x00000021
      Mount = 34, // 0x00000022
      Dismount = 35, // 0x00000023
      Dash = 36, // 0x00000024
      MountQuickStop = 37, // 0x00000025
      HitObject = 38, // 0x00000026
      Sit = 39, // 0x00000027
      SitOnTheFloor = 40, // 0x00000028
      LadderRaise = 41, // 0x00000029
      LadderRaiseEnd = 42, // 0x0000002A
      Rear = 43, // 0x0000002B
      StrikeBegin = 44, // 0x0000002C
      StrikeLight = 44, // 0x0000002C
      StrikeMedium = 45, // 0x0000002D
      StrikeHeavy = 46, // 0x0000002E
      StrikeKnockBack = 47, // 0x0000002F
      MountStrike = 48, // 0x00000030
      StrikeEnd = 48, // 0x00000030
      Count = 49, // 0x00000031
    }

    [EngineStruct("Agent_guard_mode")]
    public enum GuardMode
    {
      None = -1, // 0xFFFFFFFF
      Up = 0,
      Down = 1,
      Left = 2,
      Right = 3,
    }

    public enum HandIndex
    {
      MainHand,
      OffHand,
    }

    public enum KillInfo : sbyte
    {
      Invalid = -1, // 0xFF
      Headshot = 0,
      CouchedLance = 1,
      Punch = 2,
      MountHit = 3,
      Bow = 4,
      Crossbow = 5,
      ThrowingAxe = 6,
      ThrowingKnife = 7,
      Javelin = 8,
      Stone = 9,
      Pistol = 10, // 0x0A
      Musket = 11, // 0x0B
      OneHandedSword = 12, // 0x0C
      TwoHandedSword = 13, // 0x0D
      OneHandedAxe = 14, // 0x0E
      TwoHandedAxe = 15, // 0x0F
      Mace = 16, // 0x10
      Spear = 17, // 0x11
      Morningstar = 18, // 0x12
      Maul = 19, // 0x13
      Backstabbed = 20, // 0x14
      Gravity = 21, // 0x15
      ShieldBash = 22, // 0x16
      WeaponBash = 23, // 0x17
      Kick = 24, // 0x18
    }

    public enum MovementBehaviourType
    {
      Engaged,
      Idle,
      Flee,
    }

    [Flags]
    public enum MovementControlFlag : uint
    {
      Forward = 1,
      Backward = 2,
      StrafeRight = 4,
      StrafeLeft = 8,
      TurnRight = 16, // 0x00000010
      TurnLeft = 32, // 0x00000020
      AttackLeft = 64, // 0x00000040
      AttackRight = 128, // 0x00000080
      AttackUp = 256, // 0x00000100
      AttackDown = 512, // 0x00000200
      DefendLeft = 1024, // 0x00000400
      DefendRight = 2048, // 0x00000800
      DefendUp = 4096, // 0x00001000
      DefendDown = 8192, // 0x00002000
      DefendAuto = 16384, // 0x00004000
      DefendBlock = 32768, // 0x00008000
      Action = 65536, // 0x00010000
      AttackMask = AttackDown | AttackUp | AttackRight | AttackLeft, // 0x000003C0
      DefendMask = DefendAuto | DefendDown | DefendUp | DefendRight | DefendLeft, // 0x00007C00
      DefendDirMask = DefendDown | DefendUp | DefendRight | DefendLeft, // 0x00003C00
      MoveMask = TurnLeft | TurnRight | StrafeLeft | StrafeRight | Backward | Forward, // 0x0000003F
    }

    public enum UnderAttackType
    {
      NotUnderAttack,
      UnderMeleeAttack,
      UnderRangedAttack,
    }

    [EngineStruct("Usage_direction")]
    public enum UsageDirection
    {
      None = -1, // 0xFFFFFFFF
      AttackBegin = 0,
      AttackUp = 0,
      AttackDown = 1,
      AttackLeft = 2,
      AttackRight = 3,
      AttackEnd = 4,
      DefendBegin = 4,
      DefendUp = 4,
      DefendDown = 5,
      DefendLeft = 6,
      DefendRight = 7,
      DefendAny = 8,
      AttackAny = 9,
      DefendEnd = 9,
    }

    [EngineStruct("Weapon_wield_action_type")]
    public enum WeaponWieldActionType
    {
      WithAnimation,
      Instant,
      InstantAfterPickUp,
      WithAnimationUninterruptible,
    }

    public class AgentBoneMapArray
    {
      private readonly sbyte[] _boneMappingArray;

      public sbyte this[HumanBone i] => this._boneMappingArray[(int) i];

      public sbyte this[HorseBone i] => this._boneMappingArray[(int) i];

      public int Length => this._boneMappingArray.Length;

      public AgentBoneMapArray(Skeleton s)
      {
        byte boneCount = s.GetBoneCount();
        this._boneMappingArray = new sbyte[(int) boneCount];
        for (sbyte boneIndex = 0; (int) boneIndex < (int) boneCount; ++boneIndex)
          this._boneMappingArray[(int) boneIndex] = s.GetSkeletonBoneMapping(boneIndex);
      }
    }
  }
}
