// Decompiled with JetBrains decompiler
// Type: TaleWorlds.MountAndBlade.CastleGate
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
using TaleWorlds.MountAndBlade.Source.Objects.Siege;

namespace TaleWorlds.MountAndBlade
{
  public class CastleGate : UsableMachine, IPointDefendable, ICastleKeyPosition, ITargetable
  {
    private static int _batteringRamHitSoundId = -1;
    private const float _extraColliderScaleFactor = 1.1f;
    private const string LeftDoorBodyTag = "collider_l";
    private TacticalPosition _middlePosition;
    private const string RightDoorBodyTag = "collider_r";
    private const string RightDoorAgentOnlyBodyTag = "collider_agent_r";
    private const string OpenTag = "open";
    private const string CloseTag = "close";
    private const string middlePositionTag = "middle_pos";
    private const string waitPositionTag = "wait_pos";
    private TacticalPosition _waitPosition;
    private const string LeftDoorAgentOnlyBodyTag = "collider_agent_l";
    private const int HeavyBlowDamageLimit = 200;
    internal const string OuterGateTag = "outer_gate";
    internal const string InnerGateTag = "inner_gate";
    public CastleGate.DoorOwnership OwningTeam;
    public string OpeningAnimationName = "castle_gate_a_opening";
    public string ClosingAnimationName = "castle_gate_a_closing";
    public string HitAnimationName = "castle_gate_a_hit";
    public string PlankHitAnimationName = "castle_gate_a_plank_hit";
    public string HitMeleeAnimationName = "castle_gate_a_hit_melee";
    public string DestroyAnimationName = "castle_gate_a_break";
    public int NavigationMeshId = 1000;
    public int NavigationMeshIdToDisableOnOpen = -1;
    public string LeftDoorBoneName = "bn_bottom_l";
    public string RightDoorBoneName = "bn_bottom_r";
    public string ExtraCollisionObjectTagRight = "extra_collider_r";
    public string ExtraCollisionObjectTagLeft = "extra_collider_l";
    private bool _leftExtraColliderDisabled;
    private bool _rightExtraColliderDisabled;
    private bool _civillianMission;
    public bool ActivateExtraColliders = true;
    public string SideTag;
    private FormationAI.BehaviorSide _defenseSide;
    private CastleGate.GateState _state;
    private bool _openNavMeshIdDisabled;
    private SynchedMissionObject _door;
    private GameEntity _extraColliderRight;
    private GameEntity _extraColliderLeft;
    private readonly List<GameEntity> _attackOnlyDoorColliders;
    private GameEntity _agentColliderRight;
    private GameEntity _agentColliderLeft;
    private LadderQueueManager _queueManager;
    private bool _afterMissionStartTriggered;
    private byte _rightDoorBoneIndex;
    private byte _leftDoorBoneIndex;
    private AgentPathNavMeshChecker _pathChecker;
    public bool AutoOpen;
    private SynchedMissionObject _plank;
    private WorldFrame _middleFrame;
    private WorldFrame _defenseWaitFrame;
    private Action DestructibleComponentOnMissionReset;

    private static int _batteringramHitSoundIdCache
    {
      get
      {
        if (CastleGate._batteringRamHitSoundId == -1)
          CastleGate._batteringRamHitSoundId = SoundEvent.GetEventIdFromString("event:/mission/siege/door/hit");
        return CastleGate._batteringRamHitSoundId;
      }
    }

    public TacticalPosition MiddlePosition => this._middlePosition;

    public TacticalPosition WaitPosition => this._waitPosition;

    public override FocusableObjectType FocusableObjectType => FocusableObjectType.Gate;

    public CastleGate.GateState State
    {
      get => this._state;
      private set
      {
        this._state = value;
        LadderQueueManager queueManager = this._queueManager;
      }
    }

    public bool IsGateOpen => this._state == CastleGate.GateState.Open || this.IsDestroyed;

    public IPrimarySiegeWeapon AttackerSiegeWeapon { get; set; }

    public IEnumerable<DefencePoint> DefencePoints { get; protected set; }

    public CastleGate() => this._attackOnlyDoorColliders = new List<GameEntity>();

    public Vec3 GetPosition() => this.GameEntity.GlobalPosition;

    public override OrderType GetOrder(BattleSideEnum side) => side == BattleSideEnum.Attacker ? OrderType.AttackEntity : OrderType.Use;

    public FormationAI.BehaviorSide DefenseSide => this._defenseSide;

    public WorldFrame MiddleFrame => this._middleFrame;

    public WorldFrame DefenseWaitFrame => this._defenseWaitFrame;

    protected internal override void OnInit()
    {
      base.OnInit();
      DestructableComponent destructableComponent = this.GameEntity.GetScriptComponents<DestructableComponent>().FirstOrDefault<DestructableComponent>();
      if (destructableComponent != null)
      {
        destructableComponent.OnNextDestructionState += new Action(this.OnNextDestructionState);
        this.DestructibleComponentOnMissionReset = new Action(((MissionObject) destructableComponent).OnMissionReset);
        if (!GameNetwork.IsClientOrReplay)
        {
          destructableComponent.OnDestroyed += new DestructableComponent.OnHitTakenAndDestroyedDelegate(this.OnDestroyed);
          destructableComponent.OnHitTaken += new DestructableComponent.OnHitTakenAndDestroyedDelegate(this.OnHitTaken);
          destructableComponent.OnCalculateDestructionStateIndex += new Func<int, int, int, int>(this.OnCalculateDestructionStateIndex);
        }
        destructableComponent.BattleSide = BattleSideEnum.Defender;
      }
      this.CollectGameEntities(true);
      this.GameEntity.SetAnimationSoundActivation(true);
      if (GameNetwork.IsClientOrReplay)
        return;
      this._queueManager = this.GameEntity.GetScriptComponents<LadderQueueManager>().FirstOrDefault<LadderQueueManager>();
      if (this._queueManager == null)
      {
        GameEntity gameEntity = this.GameEntity.GetChildren().FirstOrDefault<GameEntity>((Func<GameEntity, bool>) (ce => ce.GetScriptComponents<LadderQueueManager>().Any<LadderQueueManager>()));
        if ((NativeObject) gameEntity != (NativeObject) null)
          this._queueManager = gameEntity.GetFirstScriptOfType<LadderQueueManager>();
      }
      if (this._queueManager != null)
      {
        MatrixFrame identity = MatrixFrame.Identity;
        identity.origin.y -= 2f;
        identity.rotation.RotateAboutSide(-1.570796f);
        identity.rotation.RotateAboutForward(3.141593f);
        this._queueManager.Initialize(this._queueManager.ManagedNavigationFaceId, identity, -identity.rotation.u, BattleSideEnum.Defender, 15, 0.6283185f, 3f, 2.2f, 0.0f, 0.0f, false, 1f, 0.0f, 5f);
        this._queueManager.IsDeactivated = false;
      }
      string sideTag = this.SideTag;
      this._defenseSide = sideTag == "left" ? FormationAI.BehaviorSide.Left : (sideTag == "middle" ? FormationAI.BehaviorSide.Middle : (sideTag == "right" ? FormationAI.BehaviorSide.Right : FormationAI.BehaviorSide.BehaviorSideNotSet));
      List<GameEntity> source1 = this.GameEntity.CollectChildrenEntitiesWithTag("middle_pos");
      if (source1.Any<GameEntity>())
      {
        GameEntity gameEntity = source1.FirstOrDefault<GameEntity>();
        this._middlePosition = gameEntity.GetFirstScriptOfType<TacticalPosition>();
        MatrixFrame globalFrame = gameEntity.GetGlobalFrame();
        this._middleFrame = new WorldFrame(globalFrame.rotation, globalFrame.origin.ToWorldPosition());
        this._middleFrame.Origin.GetGroundVec3();
      }
      else
      {
        MatrixFrame globalFrame = this.GameEntity.GetGlobalFrame();
        this._middleFrame = new WorldFrame(globalFrame.rotation, globalFrame.origin.ToWorldPosition());
      }
      List<GameEntity> source2 = this.GameEntity.CollectChildrenEntitiesWithTag("wait_pos");
      if (source2.Any<GameEntity>())
      {
        GameEntity gameEntity = source2.FirstOrDefault<GameEntity>();
        this._waitPosition = gameEntity.GetFirstScriptOfType<TacticalPosition>();
        MatrixFrame globalFrame = gameEntity.GetGlobalFrame();
        this._defenseWaitFrame = new WorldFrame(globalFrame.rotation, globalFrame.origin.ToWorldPosition());
        this._defenseWaitFrame.Origin.GetGroundVec3();
      }
      else
        this._defenseWaitFrame = this._middleFrame;
      this.SetScriptComponentToTick(this.GetTickRequirement());
    }

    public void SetUsableTeam(Team team)
    {
      foreach (StandingPoint standingPoint in this.StandingPoints)
      {
        if (standingPoint is StandingPointWithTeamLimit pointWithTeamLimit1)
          pointWithTeamLimit1.UsableTeam = team;
      }
    }

    public override void AfterMissionStart()
    {
      this._afterMissionStartTriggered = true;
      base.AfterMissionStart();
      this.SetInitialStateOfGate();
      this.InitializeExtraColliderPositions();
      if (!GameNetwork.IsClientOrReplay)
        this.SetAutoOpenState(this.IsSallyOut());
      if (this.OwningTeam == CastleGate.DoorOwnership.Attackers)
        this.SetUsableTeam(Mission.Current.AttackerTeam);
      else if (this.OwningTeam == CastleGate.DoorOwnership.Defenders)
        this.SetUsableTeam(Mission.Current.DefenderTeam);
      this._pathChecker = new AgentPathNavMeshChecker(Mission.Current, this.GameEntity.GetGlobalFrame(), 2f, this.NavigationMeshId, BattleSideEnum.Defender, AgentPathNavMeshChecker.Direction.BothDirections, 14f, 3f);
    }

    protected override void OnRemoved(int removeReason)
    {
      base.OnRemoved(removeReason);
      DestructableComponent destructableComponent = this.GameEntity.GetScriptComponents<DestructableComponent>().FirstOrDefault<DestructableComponent>();
      if (destructableComponent == null)
        return;
      destructableComponent.OnNextDestructionState -= new Action(this.OnNextDestructionState);
      if (GameNetwork.IsClientOrReplay)
        return;
      destructableComponent.OnDestroyed -= new DestructableComponent.OnHitTakenAndDestroyedDelegate(this.OnDestroyed);
      destructableComponent.OnHitTaken -= new DestructableComponent.OnHitTakenAndDestroyedDelegate(this.OnHitTaken);
    }

    private bool IsSallyOut()
    {
      SiegeMissionController missionBehaviour = Mission.Current.GetMissionBehaviour<SiegeMissionController>();
      return missionBehaviour != null && missionBehaviour.IsSallyOut;
    }

    protected internal override void OnEditorInit()
    {
      base.OnEditorInit();
      if (!this.GameEntity.HasTag("outer_gate") || !this.GameEntity.HasTag("inner_gate"))
        return;
      MBDebug.ShowWarning("Castle gate has both the outer gate tag and the inner gate tag.");
    }

    protected internal override void OnMissionReset()
    {
      Action componentOnMissionReset = this.DestructibleComponentOnMissionReset;
      if (componentOnMissionReset != null)
        componentOnMissionReset();
      this.CollectGameEntities(false);
      base.OnMissionReset();
      this.SetInitialStateOfGate();
    }

    private void SetInitialStateOfGate()
    {
      if (!GameNetwork.IsClientOrReplay && this.NavigationMeshIdToDisableOnOpen != -1)
      {
        this._openNavMeshIdDisabled = false;
        this.Scene.SetAbilityOfFacesWithId(this.NavigationMeshIdToDisableOnOpen, true);
      }
      if (!this._civillianMission)
      {
        this._door.GameEntity.Skeleton.SetAnimationAtChannel(this.ClosingAnimationName, 0);
        this._door.GameEntity.Skeleton.SetAnimationParameterAtChannel(0, 0.99f);
        this._door.GameEntity.ResumeSkeletonAnimation();
        this.State = CastleGate.GateState.Closed;
      }
      else
      {
        this.OpenDoor();
        if ((NativeObject) this._door.GameEntity.Skeleton != (NativeObject) null)
          this._door.SetAnimationChannelParameterSynched(0, 1f);
        this.SetGateNavMeshState(true);
        this.SetDisabled(true);
        this.GameEntity.GetFirstScriptOfType<DestructableComponent>()?.SetDisabled();
      }
    }

    public override string GetDescriptionText(GameEntity gameEntity = null) => new TextObject("{=6wZUG0ev}Gate").ToString();

    public override TextObject GetActionTextForStandingPoint(
      UsableMissionObject usableGameObject)
    {
      TextObject textObject = new TextObject(usableGameObject.GameEntity.HasTag("open") ? "{=5oozsaIb}({KEY}) Open" : "{=TJj71hPO}({KEY}) Close");
      textObject.SetTextVariable("KEY", Game.Current.GameTextManager.GetHotKeyGameText("CombatHotKeyCategory", 13));
      return textObject;
    }

    public override UsableMachineAIBase CreateAIBehaviourObject() => (UsableMachineAIBase) new CastleGateAI(this);

    public void OpenDoorAndDisableGateForCivilianMission() => this._civillianMission = true;

    public void OpenDoor()
    {
      if (this.IsDisabled)
        return;
      this.State = CastleGate.GateState.Open;
      if (!this.AutoOpen)
        this.SetGateNavMeshState(true);
      else
        this.SetGateNavMeshStateForEnemies(true);
      string animationAtChannel = this._door.GameEntity.Skeleton.GetAnimationAtChannel(0);
      float parameterAtChannel = this._door.GameEntity.Skeleton.GetAnimationParameterAtChannel(0);
      if ((NativeObject) this._door.GameEntity.Skeleton != (NativeObject) null)
      {
        this._door.SetAnimationAtChannelSynched(this.OpeningAnimationName, 0);
        if (animationAtChannel == this.ClosingAnimationName)
          this._door.SetAnimationChannelParameterSynched(0, 1f - parameterAtChannel);
      }
      if (this._plank == null)
        return;
      this._plank.SetVisibleSynched(false);
    }

    public void CloseDoor()
    {
      if (this.IsDisabled)
        return;
      this.State = CastleGate.GateState.Closed;
      if (!this.AutoOpen)
        this.SetGateNavMeshState(false);
      else
        this.SetGateNavMeshStateForEnemies(false);
      string animationAtChannel = this._door.GameEntity.Skeleton.GetAnimationAtChannel(0);
      float parameterAtChannel = this._door.GameEntity.Skeleton.GetAnimationParameterAtChannel(0);
      this._door.SetAnimationAtChannelSynched(this.ClosingAnimationName, 0);
      string openingAnimationName = this.OpeningAnimationName;
      if (!(animationAtChannel == openingAnimationName))
        return;
      this._door.SetAnimationChannelParameterSynched(0, 1f - parameterAtChannel);
    }

    private void UpdateDoorBodies()
    {
      if (this._attackOnlyDoorColliders.Count == 2)
      {
        Skeleton skeleton = this._door.GameEntity.Skeleton;
        MatrixFrame frame1 = skeleton.GetBoneEntitialFrameWithIndex(this._leftDoorBoneIndex);
        MatrixFrame frame2 = skeleton.GetBoneEntitialFrameWithIndex(this._rightDoorBoneIndex);
        this._attackOnlyDoorColliders[0].SetFrame(ref frame2);
        this._attackOnlyDoorColliders[1].SetFrame(ref frame1);
        this._agentColliderLeft?.SetFrame(ref frame1);
        this._agentColliderRight?.SetFrame(ref frame2);
        if (!((NativeObject) this._extraColliderLeft != (NativeObject) null) || !((NativeObject) this._extraColliderRight != (NativeObject) null))
          return;
        if (this.State == CastleGate.GateState.Closed)
        {
          if (!this._leftExtraColliderDisabled)
          {
            this._extraColliderLeft.SetBodyFlags(this._extraColliderLeft.BodyFlag | BodyFlags.Disabled);
            this._leftExtraColliderDisabled = true;
          }
          if (this._rightExtraColliderDisabled)
            return;
          this._extraColliderRight.SetBodyFlags(this._extraColliderRight.BodyFlag | BodyFlags.Disabled);
          this._rightExtraColliderDisabled = true;
        }
        else
        {
          float num1 = (frame2.origin - frame1.origin).Length * 0.5f;
          float num2 = Vec3.DotProduct(frame2.rotation.s, Vec3.Side) / (frame2.rotation.s.Length * 1f);
          float num3 = MathF.Sqrt((float) (1.0 - (double) num2 * (double) num2));
          float num4 = num1 * 1.1f;
          this._extraColliderLeft.SetLocalPosition(frame1.origin - new Vec3(num4 - num1, num1 * num3));
          this._extraColliderRight.SetLocalPosition(frame2.origin - new Vec3((float) -((double) num4 - (double) num1), num1 * num3));
          float num5 = (double) num2 >= 0.0 ? num1 - num1 * num2 : num1 + num1 * -num2;
          float x1 = (num4 - num5) / num1;
          if ((double) x1 <= 9.99999974737875E-05)
          {
            if (!this._leftExtraColliderDisabled)
            {
              this._extraColliderLeft.SetBodyFlags(this._extraColliderLeft.BodyFlag | BodyFlags.Disabled);
              this._leftExtraColliderDisabled = true;
            }
          }
          else
          {
            if (this._leftExtraColliderDisabled)
            {
              this._extraColliderLeft.SetBodyFlags(this._extraColliderLeft.BodyFlag & ~BodyFlags.Disabled);
              this._leftExtraColliderDisabled = false;
            }
            frame1 = this._extraColliderLeft.GetFrame();
            frame1.rotation.Orthonormalize();
            frame1.Scale(new Vec3(x1, 1f, 1f));
            this._extraColliderLeft.SetFrame(ref frame1);
          }
          frame2 = this._extraColliderRight.GetFrame();
          frame2.rotation.Orthonormalize();
          float num6 = (double) num2 >= 0.0 ? num1 - num1 * num2 : num1 + num1 * -num2;
          float x2 = (num4 - num6) / num1;
          if ((double) x2 <= 9.99999974737875E-05)
          {
            if (this._rightExtraColliderDisabled)
              return;
            this._extraColliderRight.SetBodyFlags(this._extraColliderRight.BodyFlag | BodyFlags.Disabled);
            this._rightExtraColliderDisabled = true;
          }
          else
          {
            if (this._rightExtraColliderDisabled)
            {
              this._extraColliderRight.SetBodyFlags(this._extraColliderRight.BodyFlag & ~BodyFlags.Disabled);
              this._rightExtraColliderDisabled = false;
            }
            frame2.Scale(new Vec3(x2, 1f, 1f));
            this._extraColliderRight.SetFrame(ref frame2);
          }
        }
      }
      else
      {
        if (this._attackOnlyDoorColliders.Count != 1)
          return;
        MatrixFrame entitialFrameWithName = this._door.GameEntity.Skeleton.GetBoneEntitialFrameWithName(this.RightDoorBoneName);
        this._attackOnlyDoorColliders[0].SetFrame(ref entitialFrameWithName);
        this._agentColliderRight?.SetFrame(ref entitialFrameWithName);
      }
    }

    private void SetGateNavMeshState(bool isEnabled)
    {
      if (GameNetwork.IsClientOrReplay)
        return;
      this.Scene.SetAbilityOfFacesWithId(this.NavigationMeshId, isEnabled);
      if (this._queueManager == null)
        return;
      this._queueManager.IsDeactivated = false;
      this.Scene.SetAbilityOfFacesWithId(this._queueManager.ManagedNavigationFaceId, isEnabled);
    }

    private void SetGateNavMeshStateForEnemies(bool isEnabled)
    {
      Team attackerTeam = Mission.Current.AttackerTeam;
      if (attackerTeam == null)
        return;
      foreach (Agent activeAgent in attackerTeam.ActiveAgents)
      {
        if (activeAgent.IsAIControlled)
          activeAgent.SetAgentExcludeStateForFaceGroupId(this.NavigationMeshId, !isEnabled);
      }
    }

    public void SetAutoOpenState(bool isEnabled)
    {
      this.AutoOpen = isEnabled;
      if (this.AutoOpen)
      {
        this.SetGateNavMeshState(true);
        if (this.State == CastleGate.GateState.Open)
          this.SetGateNavMeshStateForEnemies(true);
        else
          this.SetGateNavMeshStateForEnemies(false);
      }
      else
      {
        if (this.State == CastleGate.GateState.Open)
          this.CloseDoor();
        else
          this.SetGateNavMeshState(false);
        this.SetGateNavMeshStateForEnemies(true);
      }
    }

    protected override ScriptComponentBehaviour.TickRequirement GetTickRequirement() => this.GameEntity.IsVisibleIncludeParents() ? ScriptComponentBehaviour.TickRequirement.Tick : base.GetTickRequirement();

    protected internal override void OnTick(float dt)
    {
      base.OnTick(dt);
      if (!this.GameEntity.IsVisibleIncludeParents())
        return;
      if (!GameNetwork.IsClientOrReplay && this.NavigationMeshIdToDisableOnOpen != -1)
      {
        if ((this.State == CastleGate.GateState.Open || this.IsDestroyed) && !this._openNavMeshIdDisabled)
        {
          this.Scene.SetAbilityOfFacesWithId(this.NavigationMeshIdToDisableOnOpen, false);
          this._openNavMeshIdDisabled = true;
        }
        else if (this.State == CastleGate.GateState.Closed && !this.IsDestroyed && this._openNavMeshIdDisabled)
        {
          string animationAtChannel = this._door.GameEntity.Skeleton.GetAnimationAtChannel(0);
          float parameterAtChannel = this._door.GameEntity.Skeleton.GetAnimationParameterAtChannel(0);
          string closingAnimationName = this.ClosingAnimationName;
          if (animationAtChannel != closingAnimationName || (double) parameterAtChannel > 0.400000005960464)
          {
            this.Scene.SetAbilityOfFacesWithId(this.NavigationMeshIdToDisableOnOpen, true);
            this._openNavMeshIdDisabled = false;
          }
        }
      }
      if (this._afterMissionStartTriggered)
        this.UpdateDoorBodies();
      if (!GameNetwork.IsClientOrReplay)
        this.ServerTick(dt);
      if (!this.Ai.HasActionCompleted || !Mission.Current.Teams.SelectMany<Team, Formation>((Func<Team, IEnumerable<Formation>>) (t => t.Formations)).All<Formation>((Func<Formation, bool>) (f => !f.IsUsingMachine((UsableMachine) this))))
        return;
      (this.Ai as CastleGateAI).ResetInitialGateState(this._state);
    }

    private void ServerTick(float dt)
    {
      if (this.IsDeactivated)
        return;
      foreach (ScriptComponentBehaviour componentBehaviour in this.StandingPoints.Where<StandingPoint>((Func<StandingPoint, bool>) (standingPoint => standingPoint.HasUser)))
      {
        if (componentBehaviour.GameEntity.HasTag("open"))
        {
          this.OpenDoor();
          if (this.AutoOpen)
            this.SetAutoOpenState(false);
        }
        else
        {
          this.CloseDoor();
          if (this.IsSallyOut())
            this.SetAutoOpenState(true);
        }
      }
      if (this.AutoOpen && this._pathChecker != null)
      {
        this._pathChecker.Tick(dt);
        if (this._pathChecker.HasAgentsUsingPath())
        {
          if (this.State != CastleGate.GateState.Open)
            this.OpenDoor();
        }
        else if (this.State != CastleGate.GateState.Closed)
          this.CloseDoor();
      }
      if (!((NativeObject) this._door.GameEntity.Skeleton != (NativeObject) null) || this.IsDestroyed)
        return;
      float parameterAtChannel = this._door.GameEntity.Skeleton.GetAnimationParameterAtChannel(0);
      foreach (StandingPoint standingPoint in this.StandingPoints)
      {
        bool flag = (double) parameterAtChannel < 1.0 || standingPoint.GameEntity.HasTag(this.State == CastleGate.GateState.Open ? "open" : "close");
        standingPoint.SetIsDeactivatedSynched(flag);
      }
      if (this._plank == null || this.State != CastleGate.GateState.Closed || (double) parameterAtChannel <= 0.899999976158142)
        return;
      this._plank.SetVisibleSynched(true);
    }

    public TargetFlags GetTargetFlags()
    {
      TargetFlags targetFlags = (TargetFlags) (0 | 4);
      if (this.IsDestroyed)
        targetFlags |= TargetFlags.NotAThreat;
      if (DebugSiegeBehaviour.DebugAttackState == DebugSiegeBehaviour.DebugStateAttacker.DebugAttackersToBattlements)
        targetFlags |= TargetFlags.DebugThreat;
      return targetFlags;
    }

    public float GetTargetValue(List<Vec3> weaponPos) => 10f;

    public GameEntity GetTargetEntity() => this.GameEntity;

    public BattleSideEnum GetSide() => BattleSideEnum.None;

    public GameEntity Entity() => this.GameEntity;

    protected void CollectGameEntities(bool calledFromOnInit)
    {
      this.CollectDynamicGameEntities(calledFromOnInit);
      if (GameNetwork.IsClientOrReplay)
        return;
      List<GameEntity> source = this.GameEntity.CollectChildrenEntitiesWithTag("plank");
      if (!source.Any<GameEntity>())
        return;
      this._plank = source.FirstOrDefault<GameEntity>().GetFirstScriptOfType<SynchedMissionObject>();
    }

    protected void OnNextDestructionState()
    {
      this.CollectDynamicGameEntities(false);
      this.UpdateDoorBodies();
    }

    protected void CollectDynamicGameEntities(bool calledFromOnInit)
    {
      this._attackOnlyDoorColliders.Clear();
      List<GameEntity> list;
      if (calledFromOnInit)
      {
        list = this.GameEntity.CollectChildrenEntitiesWithTag("gate").ToList<GameEntity>();
        this._leftExtraColliderDisabled = false;
        this._rightExtraColliderDisabled = false;
        this._agentColliderLeft = this.GameEntity.GetFirstChildEntityWithTag("collider_agent_l");
        this._agentColliderRight = this.GameEntity.GetFirstChildEntityWithTag("collider_agent_r");
      }
      else
        list = this.GameEntity.CollectChildrenEntitiesWithTag("gate").Where<GameEntity>((Func<GameEntity, bool>) (x => x.IsVisibleIncludeParents())).ToList<GameEntity>();
      if (!list.Any<GameEntity>())
        return;
      if (list.Count > 1)
      {
        int num1 = int.MinValue;
        int num2 = int.MaxValue;
        GameEntity gameEntity1 = (GameEntity) null;
        GameEntity gameEntity2 = (GameEntity) null;
        foreach (GameEntity gameEntity3 in list)
        {
          int num3 = int.Parse(((IEnumerable<string>) ((IEnumerable<string>) gameEntity3.Tags).FirstOrDefault<string>((Func<string, bool>) (x => x.Contains("state_"))).Split('_')).Last<string>());
          if (num3 > num1)
          {
            num1 = num3;
            gameEntity1 = gameEntity3;
          }
          if (num3 < num2)
          {
            num2 = num3;
            gameEntity2 = gameEntity3;
          }
        }
        this._door = calledFromOnInit ? gameEntity2.GetFirstScriptOfType<SynchedMissionObject>() : gameEntity1.GetFirstScriptOfType<SynchedMissionObject>();
      }
      else
        this._door = list.First<GameEntity>().GetFirstScriptOfType<SynchedMissionObject>();
      GameEntity gameEntity4 = this._door.GameEntity.CollectChildrenEntitiesWithTag("collider_r").FirstOrDefault<GameEntity>();
      if ((NativeObject) gameEntity4 != (NativeObject) null)
        this._attackOnlyDoorColliders.Add(gameEntity4);
      GameEntity gameEntity5 = this._door.GameEntity.CollectChildrenEntitiesWithTag("collider_l").FirstOrDefault<GameEntity>();
      if ((NativeObject) gameEntity5 != (NativeObject) null)
        this._attackOnlyDoorColliders.Add(gameEntity5);
      if ((NativeObject) gameEntity4 == (NativeObject) null || (NativeObject) gameEntity5 == (NativeObject) null)
      {
        this._agentColliderLeft?.SetVisibilityExcludeParents(false);
        this._agentColliderRight?.SetVisibilityExcludeParents(false);
      }
      GameEntity gameEntity6 = this._door.GameEntity.CollectChildrenEntitiesWithTag(this.ExtraCollisionObjectTagLeft).FirstOrDefault<GameEntity>();
      if ((NativeObject) gameEntity6 != (NativeObject) null)
      {
        if (!this.ActivateExtraColliders)
        {
          gameEntity6.RemovePhysics();
        }
        else
        {
          if (!calledFromOnInit)
          {
            MatrixFrame frame = !((NativeObject) this._extraColliderLeft != (NativeObject) null) ? this._door.GameEntity.Skeleton.GetBoneEntitialFrameWithName(this.LeftDoorBoneName) : this._extraColliderLeft.GetFrame();
            this._extraColliderLeft = gameEntity6;
            this._extraColliderLeft.SetFrame(ref frame);
          }
          else
            this._extraColliderLeft = gameEntity6;
          if (this._leftExtraColliderDisabled)
            this._extraColliderLeft.SetBodyFlags(this._extraColliderLeft.BodyFlag | BodyFlags.Disabled);
          else
            this._extraColliderLeft.SetBodyFlags(this._extraColliderLeft.BodyFlag & ~BodyFlags.Disabled);
        }
      }
      GameEntity gameEntity7 = this._door.GameEntity.CollectChildrenEntitiesWithTag(this.ExtraCollisionObjectTagRight).FirstOrDefault<GameEntity>();
      if ((NativeObject) gameEntity7 != (NativeObject) null)
      {
        if (!this.ActivateExtraColliders)
        {
          gameEntity7.RemovePhysics();
        }
        else
        {
          if (!calledFromOnInit)
          {
            MatrixFrame frame = !((NativeObject) this._extraColliderRight != (NativeObject) null) ? this._door.GameEntity.Skeleton.GetBoneEntitialFrameWithName(this.RightDoorBoneName) : this._extraColliderRight.GetFrame();
            this._extraColliderRight = gameEntity7;
            this._extraColliderRight.SetFrame(ref frame);
          }
          else
            this._extraColliderRight = gameEntity7;
          if (this._rightExtraColliderDisabled)
            this._extraColliderRight.SetBodyFlags(this._extraColliderRight.BodyFlag | BodyFlags.Disabled);
          else
            this._extraColliderRight.SetBodyFlags(this._extraColliderRight.BodyFlag & ~BodyFlags.Disabled);
        }
      }
      if (this._door == null || !((NativeObject) this._door.GameEntity.Skeleton != (NativeObject) null))
        return;
      this._leftDoorBoneIndex = (byte) Skeleton.GetBoneIndexFromName(this._door.GameEntity.Skeleton.GetName(), this.LeftDoorBoneName);
      this._rightDoorBoneIndex = (byte) Skeleton.GetBoneIndexFromName(this._door.GameEntity.Skeleton.GetName(), this.RightDoorBoneName);
    }

    private void InitializeExtraColliderPositions()
    {
      if ((NativeObject) this._extraColliderLeft != (NativeObject) null)
      {
        MatrixFrame entitialFrameWithName = this._door.GameEntity.Skeleton.GetBoneEntitialFrameWithName(this.LeftDoorBoneName);
        this._extraColliderLeft.SetFrame(ref entitialFrameWithName);
        this._extraColliderLeft.SetVisibilityExcludeParents(true);
      }
      if ((NativeObject) this._extraColliderRight != (NativeObject) null)
      {
        MatrixFrame entitialFrameWithName = this._door.GameEntity.Skeleton.GetBoneEntitialFrameWithName(this.RightDoorBoneName);
        this._extraColliderRight.SetFrame(ref entitialFrameWithName);
        this._extraColliderRight.SetVisibilityExcludeParents(true);
      }
      this.UpdateDoorBodies();
      foreach (GameEntity onlyDoorCollider in this._attackOnlyDoorColliders)
        onlyDoorCollider.SetVisibilityExcludeParents(true);
      if ((NativeObject) this._agentColliderLeft != (NativeObject) null)
        this._agentColliderLeft.SetVisibilityExcludeParents(true);
      if (!((NativeObject) this._agentColliderRight != (NativeObject) null))
        return;
      this._agentColliderRight.SetVisibilityExcludeParents(true);
    }

    private void OnHitTaken(
      DestructableComponent hitComponent,
      Agent hitterAgent,
      in MissionWeapon weapon,
      ScriptComponentBehaviour attackerScriptComponentBehaviour,
      int inflictedDamage)
    {
      if (GameNetwork.IsClientOrReplay || inflictedDamage < 200 || this.State != CastleGate.GateState.Closed)
        return;
      this._plank?.SetAnimationAtChannelSynched(this.PlankHitAnimationName, 0);
      this._door.SetAnimationAtChannelSynched(this.HitAnimationName, 0);
      Mission.Current.MakeSound(CastleGate._batteringramHitSoundIdCache, this.GameEntity.GlobalPosition, false, true, -1, -1);
    }

    private void OnDestroyed(
      DestructableComponent destroyedComponent,
      Agent destroyerAgent,
      in MissionWeapon weapon,
      ScriptComponentBehaviour attackerScriptComponentBehaviour,
      int inflictedDamage)
    {
      if (GameNetwork.IsClientOrReplay)
        return;
      this._plank?.SetVisibleSynched(false);
      foreach (UsableMissionObject standingPoint in this.StandingPoints)
        standingPoint.SetIsDeactivatedSynched(true);
      if (attackerScriptComponentBehaviour is BatteringRam)
        this._door.SetAnimationAtChannelSynched(this.DestroyAnimationName, 0);
      this.SetGateNavMeshState(true);
    }

    private int OnCalculateDestructionStateIndex(
      int destructionStateIndex,
      int inflictedDamage,
      int destructionStateCount)
    {
      return inflictedDamage >= 200 ? (destructionStateIndex = Math.Min(destructionStateIndex, destructionStateCount - 1)) : destructionStateIndex;
    }

    protected internal override bool OnCheckForProblems()
    {
      base.OnCheckForProblems();
      bool flag = false;
      if (this.GameEntity.HasTag("outer_gate") && this.GameEntity.HasTag("inner_gate"))
      {
        MBEditor.AddEntityWarning(this.GameEntity, "This castle gate has both outer and inner tag at the same time.");
        flag = true;
      }
      if (this.GameEntity.CollectChildrenEntitiesWithTag("wait_pos").Count != 1)
      {
        MBEditor.AddEntityWarning(this.GameEntity, "There must be one entity with wait position tag under castle gate.");
        flag = true;
      }
      if (this.GameEntity.HasTag("outer_gate"))
      {
        uint visibilityMask = this.GameEntity.GetVisibilityLevelMaskIncludingParents();
        GameEntity gameEntity1 = this.GameEntity.GetChildren().FirstOrDefault<GameEntity>((Func<GameEntity, bool>) (x => x.HasTag("middle_pos") && (int) x.GetVisibilityLevelMaskIncludingParents() == (int) visibilityMask));
        if ((NativeObject) gameEntity1 != (NativeObject) null)
        {
          GameEntity gameEntity2 = this.Scene.FindEntitiesWithTag("inner_gate").FirstOrDefault<GameEntity>((Func<GameEntity, bool>) (x => (int) x.GetVisibilityLevelMaskIncludingParents() == (int) visibilityMask));
          if ((NativeObject) gameEntity2 != (NativeObject) null)
          {
            if (gameEntity2.HasScriptOfType<CastleGate>())
            {
              if ((double) Vec2.DotProduct(gameEntity2.GlobalPosition.AsVec2 - gameEntity1.GlobalPosition.AsVec2, this.GameEntity.GlobalPosition.AsVec2 - gameEntity1.GlobalPosition.AsVec2) <= 0.0)
              {
                MBEditor.AddEntityWarning(this.GameEntity, "Outer gate's middle position must not be between outer and inner gate.");
                flag = true;
              }
            }
            else
            {
              MBEditor.AddEntityWarning(this.GameEntity, gameEntity2.Name + " this entity has inner gate tag but doesn't have castle gate script.");
              flag = true;
            }
          }
          else
          {
            MBEditor.AddEntityWarning(this.GameEntity, "There is no entity with inner gate tag.");
            flag = true;
          }
        }
        else
        {
          MBEditor.AddEntityWarning(this.GameEntity, "Outer gate doesn't have any middle positions");
          flag = true;
        }
      }
      return flag;
    }

    public enum DoorOwnership
    {
      Defenders,
      Attackers,
    }

    public enum GateState
    {
      Open,
      Closed,
    }
  }
}
