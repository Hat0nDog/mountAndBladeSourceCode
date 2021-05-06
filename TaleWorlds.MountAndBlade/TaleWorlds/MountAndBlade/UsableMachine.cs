// Decompiled with JetBrains decompiler
// Type: TaleWorlds.MountAndBlade.UsableMachine
// Assembly: TaleWorlds.MountAndBlade, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3C0E3F42-756F-4407-B817-06BA412A78B0
// Assembly location: E:\SteamLibrary\steamapps\common\Mount & Blade II Bannerlord\bin\Win64_Shipping_Client\TaleWorlds.MountAndBlade.dll

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using TaleWorlds.Core;
using TaleWorlds.DotNet;
using TaleWorlds.Engine;
using TaleWorlds.Library;
using TaleWorlds.Localization;

namespace TaleWorlds.MountAndBlade
{
  public abstract class UsableMachine : SynchedMissionObject, IFocusable, IOrderable, IDetachment
  {
    public const string UsableMachineParentTag = "machine_parent";
    public string PilotStandingPointTag = "Pilot";
    public string AmmoPickUpTag = "ammopickup";
    public string WaitStandingPointTag = "Wait";
    protected GameEntity ActiveWaitStandingPoint;
    private readonly List<UsableMissionObjectComponent> _components;
    private DestructableComponent _destructionComponent;
    protected bool _areUsableStandingPointsVacant = true;
    protected List<(int, StandingPoint)> _usableStandingPoints;
    protected bool _isDetachmentRecentlyEvaluated;
    private int _reevaluatedCount;
    protected float EnemyRangeToStopUsing;
    protected bool MakeVisibilityCheck = true;
    private UsableMachineAIBase _ai;
    private StandingPoint _currentlyUsedAmmoPickUpPoint;
    private QueryData<bool> _isDisabledForAttackerAI;
    private QueryData<bool> _isDisabledForDefenderAI;
    protected bool _isDisabledForAI;
    private bool _isMachineDeactivated;

    public List<StandingPoint> StandingPoints { get; private set; }

    public StandingPoint PilotStandingPoint { get; private set; }

    protected internal List<StandingPoint> AmmoPickUpPoints { get; private set; }

    protected List<GameEntity> WaitStandingPoints { get; private set; }

    public DestructableComponent DestructionComponent => this._destructionComponent;

    public bool IsDestructible => this.DestructionComponent != null;

    public bool IsDestroyed => this.DestructionComponent != null && this.DestructionComponent.IsDestroyed;

    public Agent PilotAgent => this.PilotStandingPoint?.UserAgent;

    public bool IsLoose => false;

    internal UsableMachineAIBase Ai
    {
      get
      {
        if (this._ai == null)
          this._ai = this.CreateAIBehaviourObject();
        return this._ai;
      }
    }

    public virtual FocusableObjectType FocusableObjectType => FocusableObjectType.Item;

    public StandingPoint CurrentlyUsedAmmoPickUpPoint
    {
      get => this._currentlyUsedAmmoPickUpPoint;
      set
      {
        this._currentlyUsedAmmoPickUpPoint = value;
        this.SetScriptComponentToTick(this.GetTickRequirement());
      }
    }

    public bool HasAIPickingUpAmmo => this.CurrentlyUsedAmmoPickUpPoint != null;

    protected UsableMachine() => this._components = new List<UsableMissionObjectComponent>();

    public void AddComponent(UsableMissionObjectComponent component)
    {
      this._components.Add(component);
      component.OnAdded(this.Scene);
      this.SetScriptComponentToTick(this.GetTickRequirement());
    }

    public void RemoveComponent(UsableMissionObjectComponent component)
    {
      component.OnRemoved();
      this._components.Remove(component);
      this.SetScriptComponentToTick(this.GetTickRequirement());
    }

    public T GetComponent<T>() where T : UsableMissionObjectComponent => this._components.Find((Predicate<UsableMissionObjectComponent>) (c => c is T)) as T;

    public virtual OrderType GetOrder(BattleSideEnum side) => OrderType.Use;

    public virtual UsableMachineAIBase CreateAIBehaviourObject() => (UsableMachineAIBase) null;

    public GameEntity GetValidStandingPointForAgent(Agent agent)
    {
      float num = float.MaxValue;
      StandingPoint standingPoint1 = (StandingPoint) null;
      foreach (StandingPoint standingPoint2 in this.StandingPoints)
      {
        if (!standingPoint2.IsDisabledForAgent(agent) && (!standingPoint2.HasUser || standingPoint2.HasAIUser))
        {
          float distanceSq = standingPoint2.GetUserFrameForAgent(agent).Origin.AsVec2.DistanceSquared(agent.Position.AsVec2);
          if (agent.CanReachAndUseObject((UsableMissionObject) standingPoint2, distanceSq) && (double) distanceSq < (double) num && (double) Math.Abs(standingPoint2.GetUserFrameForAgent(agent).Origin.GetGroundVec3().z - agent.Position.z) < 1.5)
          {
            num = distanceSq;
            standingPoint1 = standingPoint2;
          }
        }
      }
      return standingPoint1?.GameEntity;
    }

    public GameEntity GetValidStandingPointForAgentWithoutDistanceCheck(Agent agent)
    {
      float num1 = float.MaxValue;
      StandingPoint standingPoint1 = (StandingPoint) null;
      foreach (StandingPoint standingPoint2 in this.StandingPoints)
      {
        if (!standingPoint2.IsDisabledForAgent(agent) && (!standingPoint2.HasUser || standingPoint2.HasAIUser))
        {
          float num2 = standingPoint2.GetUserFrameForAgent(agent).Origin.AsVec2.DistanceSquared(agent.Position.AsVec2);
          if ((double) num2 < (double) num1 && (double) Math.Abs(standingPoint2.GetUserFrameForAgent(agent).Origin.GetGroundVec3().z - agent.Position.z) < 1.5)
          {
            num1 = num2;
            standingPoint1 = standingPoint2;
          }
        }
      }
      return standingPoint1?.GameEntity;
    }

    public StandingPoint GetVacantStandingPointForAI(Agent agent)
    {
      if (this.PilotStandingPoint != null && !this.PilotStandingPoint.IsDisabledForAgent(agent) && !this.AmmoPickUpPoints.Contains(this.PilotStandingPoint))
        return this.PilotStandingPoint;
      float num = 1E+08f;
      StandingPoint standingPoint1 = (StandingPoint) null;
      foreach (StandingPoint standingPoint2 in this.StandingPoints)
      {
        bool flag = true;
        if (this.AmmoPickUpPoints.Contains(standingPoint2))
        {
          foreach (StandingPoint standingPoint3 in this.StandingPoints)
          {
            if (standingPoint3 is StandingPointWithWeaponRequirement && !this.AmmoPickUpPoints.Contains(standingPoint3) && (standingPoint3.IsDeactivated || standingPoint3.HasUser || standingPoint3.HasAIMovingTo))
            {
              flag = false;
              break;
            }
          }
        }
        if (flag && !standingPoint2.IsDisabledForAgent(agent))
        {
          float lengthSquared = (agent.Position - standingPoint2.GetUserFrameForAgent(agent).Origin.GetGroundVec3()).LengthSquared;
          if (!standingPoint2.IsDisabledForPlayers)
            lengthSquared -= 100000f;
          if ((double) lengthSquared < (double) num)
          {
            num = lengthSquared;
            standingPoint1 = standingPoint2;
          }
        }
      }
      return standingPoint1;
    }

    public StandingPoint GetTargetStandingPointOfAIAgent(Agent agent)
    {
      foreach (StandingPoint standingPoint in this.StandingPoints)
      {
        if (standingPoint.IsAIMovingTo(agent))
          return standingPoint;
      }
      return (StandingPoint) null;
    }

    public override void SetVisibleSynched(bool value, bool forceChildrenVisible = false) => base.SetVisibleSynched(value, forceChildrenVisible);

    public override void SetPhysicsStateSynched(bool value, bool setChildren = true)
    {
      base.SetPhysicsStateSynched(value, setChildren);
      this.SetAbilityOfFaces(value);
      foreach (StandingPoint standingPoint in this.StandingPoints)
        standingPoint.OnParentMachinePhysicsStateChanged();
    }

    public IEnumerable<Agent> Users => this.StandingPoints.Select<StandingPoint, Agent>((Func<StandingPoint, Agent>) (sp => sp.UserAgent)).Where<Agent>((Func<Agent, bool>) (ua => ua != null));

    public int UserCount => this.StandingPoints.Count<StandingPoint>((Func<StandingPoint, bool>) (sp => sp.HasUser));

    public virtual int MaxUserCount => this.StandingPoints.Count;

    protected internal override void OnInit()
    {
      base.OnInit();
      this._isDisabledForAttackerAI = new QueryData<bool>((Func<bool>) (() =>
      {
        Vec3 globalPosition = this.GameEntity.GlobalPosition;
        Agent closestEnemyAgent = Mission.Current.GetClosestEnemyAgent(Mission.Current.Teams.Attacker, globalPosition, this.EnemyRangeToStopUsing);
        return closestEnemyAgent != null && (double) closestEnemyAgent.Position.z > (double) globalPosition.z - 2.0 && (double) closestEnemyAgent.Position.z < (double) globalPosition.z + 4.0;
      }), 1f);
      this._isDisabledForDefenderAI = new QueryData<bool>((Func<bool>) (() =>
      {
        Vec3 globalPosition = this.GameEntity.GlobalPosition;
        Agent closestEnemyAgent = Mission.Current.GetClosestEnemyAgent(Mission.Current.Teams.Defender, globalPosition, this.EnemyRangeToStopUsing);
        return closestEnemyAgent != null && (double) closestEnemyAgent.Position.z > (double) globalPosition.z - 2.0 && (double) closestEnemyAgent.Position.z < (double) globalPosition.z + 4.0;
      }), 1f);
      GameEntity parent = this.GameEntity.Parent;
      this.StandingPoints = (parent != null ? (parent.HasTag("machine_parent") ? 1 : 0) : 0) == 0 ? this.GameEntity.CollectObjects<StandingPoint>() : this.GameEntity.Parent.CollectObjects<StandingPoint>();
      this.AmmoPickUpPoints = new List<StandingPoint>();
      this._destructionComponent = this.GameEntity.GetFirstScriptOfType<DestructableComponent>();
      this.PilotStandingPoint = (StandingPoint) null;
      foreach (StandingPoint standingPoint in this.StandingPoints)
      {
        if (standingPoint.GameEntity.HasTag(this.PilotStandingPointTag))
          this.PilotStandingPoint = standingPoint;
        if (standingPoint.GameEntity.HasTag(this.AmmoPickUpTag))
          this.AmmoPickUpPoints.Add(standingPoint);
      }
      this.WaitStandingPoints = this.GameEntity.CollectChildrenEntitiesWithTag(this.WaitStandingPointTag);
      if (this.WaitStandingPoints.Count > 0)
        this.ActiveWaitStandingPoint = this.WaitStandingPoints[0];
      this._usableStandingPoints = new List<(int, StandingPoint)>();
      this.SetScriptComponentToTick(this.GetTickRequirement());
    }

    protected override ScriptComponentBehaviour.TickRequirement GetTickRequirement()
    {
      bool flag = false;
      foreach (UsableMissionObjectComponent component in this._components)
      {
        if (component.IsOnTickRequired())
        {
          flag = true;
          break;
        }
      }
      return this.GameEntity.IsVisibleIncludeParents() && (flag || !GameNetwork.IsClientOrReplay && this.HasAIPickingUpAmmo) ? ScriptComponentBehaviour.TickRequirement.Tick : base.GetTickRequirement();
    }

    protected internal override void OnTick(float dt)
    {
      base.OnTick(dt);
      if (this.MakeVisibilityCheck && !this.GameEntity.IsVisibleIncludeParents())
        return;
      if (!GameNetwork.IsClientOrReplay && this.HasAIPickingUpAmmo && (!this.CurrentlyUsedAmmoPickUpPoint.HasAIMovingTo && !this.CurrentlyUsedAmmoPickUpPoint.HasAIUser))
        this.CurrentlyUsedAmmoPickUpPoint = (StandingPoint) null;
      foreach (UsableMissionObjectComponent component in this._components)
        component.OnTick(dt);
      int num = GameNetwork.IsClientOrReplay ? 1 : 0;
    }

    private static string DebugGetMemberNameOf<T>(object instance, T sp) where T : class
    {
      System.Type type = instance.GetType();
      foreach (PropertyInfo property in type.GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic))
      {
        if (!(property.GetMethod == (MethodInfo) null))
        {
          if (property.GetValue(instance) == (object) sp)
            return property.Name;
          if (property.GetType().IsGenericType && property.GetType().GetGenericTypeDefinition() == typeof (List<>) && property.GetValue(instance) is List<StandingPoint> standingPointList3)
          {
            for (int index = 0; index < standingPointList3.Count; ++index)
            {
              StandingPoint standingPoint = standingPointList3[index];
              if ((object) sp == standingPoint)
                return property.Name + "[" + (object) index + "]";
            }
          }
        }
      }
      foreach (FieldInfo field in type.GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic))
      {
        if (field.GetValue(instance) == (object) sp)
          return field.Name;
        if (field.FieldType.IsGenericType && field.FieldType.GetGenericTypeDefinition() == typeof (List<>) && field.GetValue(instance) is List<StandingPoint> standingPointList4)
        {
          for (int index = 0; index < standingPointList4.Count; ++index)
          {
            StandingPoint standingPoint = standingPointList4[index];
            if ((object) sp == standingPoint)
              return field.Name + "[" + (object) index + "]";
          }
        }
      }
      return (string) null;
    }

    [Conditional("TRACE")]
    protected virtual void DebugTick(float dt)
    {
      if (!MBDebug.IsDisplayingHighLevelAI)
        return;
      foreach (StandingPoint standingPoint in this.StandingPoints)
      {
        Vec3 globalPosition = standingPoint.GameEntity.GlobalPosition;
        Vec3 vec3 = Vec3.One / 3f;
        int num = standingPoint.IsDeactivated ? 1 : 0;
      }
    }

    protected internal override void OnEditorTick(float dt)
    {
      base.OnEditorTick(dt);
      foreach (UsableMissionObjectComponent component in this._components)
        component.OnEditorTick(dt);
    }

    protected internal override void OnEditorValidate()
    {
      base.OnEditorValidate();
      foreach (UsableMissionObjectComponent component in this._components)
        component.OnEditorValidate();
    }

    public virtual void OnFocusGain(Agent userAgent)
    {
      foreach (UsableMissionObjectComponent component in this._components)
        component.OnFocusGain(userAgent);
    }

    public virtual void OnFocusLose(Agent userAgent)
    {
      foreach (UsableMissionObjectComponent component in this._components)
        component.OnFocusLose(userAgent);
    }

    public virtual TextObject GetInfoTextForBeingNotInteractable(Agent userAgent) => TextObject.Empty;

    public virtual bool HasWaitFrame => (NativeObject) this.ActiveWaitStandingPoint != (NativeObject) null;

    public MatrixFrame WaitFrame => (NativeObject) this.ActiveWaitStandingPoint != (NativeObject) null ? this.ActiveWaitStandingPoint.GetGlobalFrame() : new MatrixFrame();

    public GameEntity WaitEntity => this.ActiveWaitStandingPoint;

    protected internal override void OnMissionReset()
    {
      base.OnMissionReset();
      foreach (UsableMissionObjectComponent component in this._components)
        component.OnMissionReset();
    }

    public virtual bool IsDeactivated => this._isMachineDeactivated || this.IsDestroyed;

    public void Deactivate()
    {
      this._isMachineDeactivated = true;
      foreach (UsableMissionObject standingPoint in this.StandingPoints)
        standingPoint.IsDeactivated = true;
    }

    public void Activate()
    {
      this._isMachineDeactivated = false;
      foreach (UsableMissionObject standingPoint in this.StandingPoints)
        standingPoint.IsDeactivated = false;
    }

    public virtual bool IsDisabledForBattleSide(BattleSideEnum sideEnum) => this.IsDeactivated;

    public virtual bool IsDisabledForBattleSideAI(BattleSideEnum sideEnum)
    {
      if (this._isDisabledForAI)
        return true;
      if ((double) this.EnemyRangeToStopUsing <= 0.0)
        return false;
      if (sideEnum == BattleSideEnum.Attacker)
        return this._isDisabledForAttackerAI.Value;
      return sideEnum == BattleSideEnum.Defender && this._isDisabledForDefenderAI.Value;
    }

    public virtual void Disable()
    {
      foreach (Team team in Mission.Current.Teams.Where<Team>((Func<Team, bool>) (t => t.DetachmentManager.Detachments.Contains((IDetachment) this))))
        team.DetachmentManager.DestroyDetachment((IDetachment) this);
      foreach (StandingPoint standingPoint in this.StandingPoints)
      {
        if (!standingPoint.GameEntity.HasTag(this.AmmoPickUpTag))
        {
          if (standingPoint.HasUser)
            standingPoint.UserAgent.StopUsingGameObject();
          standingPoint.SetIsDeactivatedSynched(true);
        }
      }
    }

    protected override void OnRemoved(int removeReason)
    {
      base.OnRemoved(removeReason);
      foreach (UsableMissionObjectComponent component in this._components)
        component.OnRemoved();
    }

    public override bool ReadFromNetwork()
    {
      bool flag = base.ReadFromNetwork();
      foreach (UsableMissionObjectComponent component in this._components)
        flag = flag && component.ReadFromNetwork();
      return flag;
    }

    public override void WriteToNetwork()
    {
      base.WriteToNetwork();
      foreach (UsableMissionObjectComponent component in this._components)
        component.WriteToNetwork();
    }

    public override string ToString()
    {
      string str = this.GetType().ToString() + " with Components:";
      foreach (UsableMissionObjectComponent component in this._components)
        str = str + "[" + (object) component + "]";
      return str;
    }

    public abstract TextObject GetActionTextForStandingPoint(
      UsableMissionObject usableGameObject);

    public virtual StandingPoint GetBestPointAlternativeTo(
      StandingPoint standingPoint,
      Agent agent)
    {
      return standingPoint;
    }

    public virtual bool IsInRangeToCheckAlternativePoints(Agent agent)
    {
      float num = this.StandingPoints.Count > 0 ? agent.GetInteractionDistanceToUsable((IUsable) this.StandingPoints[0]) + 1f : 2f;
      return (double) this.GameEntity.GlobalPosition.DistanceSquared(agent.Position) < (double) num * (double) num;
    }

    IEnumerable<Agent> IDetachment.Agents => this.Users.Concat<Agent>(this.StandingPoints.SelectMany<StandingPoint, Agent>((Func<StandingPoint, IEnumerable<Agent>>) (sp => (IEnumerable<Agent>) sp.MovingAgents.Keys)));

    protected virtual float GetWeightOfStandingPoint(StandingPoint sp) => !sp.HasAIMovingTo ? 0.6f : 0.2f;

    float IDetachment.GetDetachmentWeight(BattleSideEnum side) => this.GetDetachmentWeightAux(side);

    protected virtual float GetDetachmentWeightAux(BattleSideEnum side)
    {
      if (this.IsDisabledForBattleSideAI(side))
        return float.MinValue;
      this._usableStandingPoints.Clear();
      bool flag1 = false;
      bool flag2 = false;
      for (int index = 0; index < this.StandingPoints.Count; ++index)
      {
        StandingPoint standingPoint = this.StandingPoints[index];
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

    List<(int, float)> IDetachment.GetSlotIndexWeightTuples()
    {
      List<(int, float)> valueTupleList = new List<(int, float)>();
      foreach ((int, StandingPoint) usableStandingPoint in this._usableStandingPoints)
      {
        StandingPoint sp = usableStandingPoint.Item2;
        valueTupleList.Add((usableStandingPoint.Item1, this.GetWeightOfStandingPoint(sp) * (this._areUsableStandingPointsVacant || !sp.HasRecentlyBeenRechecked ? 1f : 0.1f)));
      }
      return valueTupleList;
    }

    bool IDetachment.IsSlotAtIndexAvailableForAgent(int slotIndex, Agent agent) => agent.CanBeAssignedForScriptedMovement() && !this.StandingPoints[slotIndex].IsDisabledForAgent(agent);

    bool IDetachment.IsAgentEligible(Agent agent) => true;

    public void AddAgentAtSlotIndex(Agent agent, int slotIndex)
    {
      StandingPoint standingPoint = this.StandingPoints[slotIndex];
      if (standingPoint.HasAIMovingTo)
      {
        foreach (KeyValuePair<Agent, UsableMissionObject.MoveInfo> keyValuePair in standingPoint.MovingAgents.ToList<KeyValuePair<Agent, UsableMissionObject.MoveInfo>>())
        {
          Agent key = keyValuePair.Key;
          ((IDetachment) this).RemoveAgent(key);
          key.Formation?.AttachUnit(key);
        }
      }
      ((IDetachment) this).AddAgent(agent, slotIndex);
      agent.Formation?.DetachUnit(agent, false);
      agent.DetachmentWeight = 1f;
      agent.Detachment = (IDetachment) this;
    }

    Agent IDetachment.GetMovingAgentAtSlotIndex(int slotIndex) => this.StandingPoints[slotIndex].MovingAgents.FirstOrDefault<KeyValuePair<Agent, UsableMissionObject.MoveInfo>>().Key;

    bool IDetachment.IsDetachmentRecentlyEvaluated() => this._isDetachmentRecentlyEvaluated;

    void IDetachment.UnmarkDetachment() => this._isDetachmentRecentlyEvaluated = false;

    void IDetachment.MarkSlotAtIndex(int slotIndex)
    {
      if (++this._reevaluatedCount >= this._usableStandingPoints.Count)
      {
        foreach ((int, StandingPoint) usableStandingPoint in this._usableStandingPoints)
          usableStandingPoint.Item2.HasRecentlyBeenRechecked = false;
        this._isDetachmentRecentlyEvaluated = true;
        this._reevaluatedCount = 0;
      }
      else
        this.StandingPoints[slotIndex].HasRecentlyBeenRechecked = true;
    }

    float? IDetachment.GetWeightOfNextSlot(BattleSideEnum side)
    {
      if (this.IsDisabledForBattleSideAI(side))
        return new float?();
      StandingPoint standingPointFor = this.GetSuitableStandingPointFor(side);
      return standingPointFor != null ? new float?(this.GetWeightOfStandingPoint(standingPointFor)) : new float?();
    }

    float IDetachment.GetExactCostOfAgentAtSlot(Agent candidate, int slotIndex)
    {
      StandingPoint standingPoint = this.StandingPoints[slotIndex];
      Vec3 globalPosition = standingPoint.GameEntity.GlobalPosition;
      WorldPosition point0 = new WorldPosition(standingPoint.GameEntity.Scene, globalPosition);
      WorldPosition worldPosition = candidate.GetWorldPosition();
      float pathDistance;
      if (!standingPoint.Scene.GetPathDistanceBetweenPositions(ref point0, ref worldPosition, candidate.Monster.BodyCapsuleRadius, out pathDistance))
        pathDistance = float.MaxValue;
      return pathDistance;
    }

    float[] IDetachment.GetTemplateCostsOfAgent(Agent candidate, float[] oldValue)
    {
      float[] numArray = oldValue == null || oldValue.Length != this.StandingPoints.Count ? new float[this.StandingPoints.Count] : oldValue;
      for (int index = 0; index < this.StandingPoints.Count; ++index)
        numArray[index] = float.MaxValue;
      foreach ((int, StandingPoint) usableStandingPoint in this._usableStandingPoints)
      {
        float num = usableStandingPoint.Item2.GameEntity.GlobalPosition.Distance(candidate.Position);
        numArray[usableStandingPoint.Item1] = num;
      }
      return numArray;
    }

    float IDetachment.GetTemplateWeightOfAgent(Agent candidate)
    {
      Scene scene = Mission.Current.Scene;
      Vec3 globalPosition = this.GameEntity.GlobalPosition;
      WorldPosition worldPosition = candidate.GetWorldPosition();
      WorldPosition point0 = new WorldPosition(scene, UIntPtr.Zero, globalPosition, true);
      float pathDistance;
      if (!scene.GetPathDistanceBetweenPositions(ref point0, ref worldPosition, candidate.Monster.BodyCapsuleRadius, out pathDistance))
        pathDistance = float.MaxValue;
      return pathDistance;
    }

    float IDetachment.GetWeightOfOccupiedSlot(Agent agent) => this.GetWeightOfStandingPoint(this.StandingPoints.FirstOrDefault<StandingPoint>((Func<StandingPoint, bool>) (sp => sp.UserAgent == agent || sp.MovingAgents.ContainsKey(agent))));

    WorldFrame? IDetachment.GetAgentFrame(Agent agent) => new WorldFrame?();

    void IDetachment.RemoveAgent(Agent agent) => agent.StopUsingGameObject(autoAttach: false);

    public bool IsStandingPointAvailableForAgent(Agent agent)
    {
      int side = (int) agent.Team.Side;
      bool flag = false;
      foreach (StandingPoint standingPoint in this.StandingPoints)
      {
        if (!standingPoint.IsDeactivated && (standingPoint.IsInstantUse || (!standingPoint.HasUser || standingPoint.UserAgent == agent) && (!standingPoint.HasAIMovingTo || standingPoint.MovingAgents.ContainsKey(agent))) && (!standingPoint.IsDisabledForAgent(agent) && !this.IsStandingPointNotUsedOnAccountOfBeingAmmoLoad(standingPoint)))
        {
          flag = true;
          break;
        }
      }
      return flag;
    }

    float? IDetachment.GetWeightOfAgentAtNextSlot(
      IEnumerable<Agent> candidates,
      out Agent match)
    {
      BattleSideEnum side = candidates.First<Agent>().Team.Side;
      StandingPoint standingPointFor = this.GetSuitableStandingPointFor(side, agents: candidates);
      if (standingPointFor != null)
      {
        match = UsableMachineAIBase.GetSuitableAgentForStandingPoint(this, standingPointFor, candidates, new List<Agent>());
        if (match == null)
          return new float?();
        float? weightOfNextSlot = ((IDetachment) this).GetWeightOfNextSlot(side);
        float num = 1f;
        return !weightOfNextSlot.HasValue ? new float?() : new float?(weightOfNextSlot.GetValueOrDefault() * num);
      }
      match = (Agent) null;
      return new float?();
    }

    float? IDetachment.GetWeightOfAgentAtNextSlot(
      IEnumerable<AgentValuePair<float>> candidates,
      out Agent match)
    {
      BattleSideEnum side = candidates.First<AgentValuePair<float>>().Agent.Team.Side;
      StandingPoint standingPointFor = this.GetSuitableStandingPointFor(side, agentValuePairs: candidates);
      if (standingPointFor != null)
      {
        float? weightOfNextSlot = ((IDetachment) this).GetWeightOfNextSlot(side);
        match = UsableMachineAIBase.GetSuitableAgentForStandingPoint(this, standingPointFor, candidates, new List<Agent>(), weightOfNextSlot.Value);
        if (match == null)
          return new float?();
        float? nullable = weightOfNextSlot;
        float num = 1f;
        return !nullable.HasValue ? new float?() : new float?(nullable.GetValueOrDefault() * num);
      }
      match = (Agent) null;
      return new float?();
    }

    float? IDetachment.GetWeightOfAgentAtOccupiedSlot(
      Agent detachedAgent,
      IEnumerable<Agent> candidates,
      out Agent match)
    {
      BattleSideEnum side = candidates.First<Agent>().Team.Side;
      StandingPoint standingPoint = this.StandingPoints.FirstOrDefault<StandingPoint>((Func<StandingPoint, bool>) (sp => sp.MovingAgents.Keys.Contains<Agent>(detachedAgent) || sp.UserAgent == detachedAgent));
      match = UsableMachineAIBase.GetSuitableAgentForStandingPoint(this, standingPoint, candidates, new List<Agent>());
      if (match == null)
        return new float?();
      float? weightOfNextSlot = ((IDetachment) this).GetWeightOfNextSlot(side);
      float num = 1f;
      return !weightOfNextSlot.HasValue ? new float?() : new float?((float) ((double) weightOfNextSlot.GetValueOrDefault() * (double) num * 0.5));
    }

    void IDetachment.AddAgent(Agent agent, int slotIndex)
    {
      StandingPoint standingPoint = slotIndex == -1 ? this.GetSuitableStandingPointFor(agent.Team.Side, agent) : this.StandingPoints[slotIndex];
      if (standingPoint == null)
        return;
      if (standingPoint.HasAIMovingTo && !standingPoint.IsInstantUse)
        standingPoint.MovingAgents.FirstOrDefault<KeyValuePair<Agent, UsableMissionObject.MoveInfo>>().Key.StopUsingGameObject();
      agent.AIMoveToGameObjectEnable((UsableMissionObject) standingPoint, this.Ai.GetScriptedFrameFlags(agent));
      if (!standingPoint.GameEntity.HasTag(this.AmmoPickUpTag))
        return;
      this.CurrentlyUsedAmmoPickUpPoint = standingPoint;
    }

    internal virtual bool IsStandingPointNotUsedOnAccountOfBeingAmmoLoad(StandingPoint standingPoint) => this.AmmoPickUpPoints.Contains(standingPoint) && (this.StandingPoints.Any<StandingPoint>((Func<StandingPoint, bool>) (standingPoint2 => (standingPoint2.IsDeactivated || standingPoint2.HasUser || standingPoint2.HasAIMovingTo) && !standingPoint2.GameEntity.HasTag(this.AmmoPickUpTag) && standingPoint2 is StandingPointWithWeaponRequirement)) || this.HasAIPickingUpAmmo);

    protected virtual StandingPoint GetSuitableStandingPointFor(
      BattleSideEnum side,
      Agent agent = null,
      IEnumerable<Agent> agents = null,
      IEnumerable<AgentValuePair<float>> agentValuePairs = null)
    {
      return this.StandingPoints.FirstOrDefault<StandingPoint>((Func<StandingPoint, bool>) (sp => !sp.IsDeactivated && (sp.IsInstantUse || !sp.HasUser && !sp.HasAIMovingTo) && ((agent == null || !sp.IsDisabledForAgent(agent)) && (agents == null || agents.Any<Agent>((Func<Agent, bool>) (a => !sp.IsDisabledForAgent(a)))) && (agentValuePairs == null || agentValuePairs.Any<AgentValuePair<float>>((Func<AgentValuePair<float>, bool>) (avp => !sp.IsDisabledForAgent(avp.Agent))))) && !this.IsStandingPointNotUsedOnAccountOfBeingAmmoLoad(sp)));
    }

    public abstract string GetDescriptionText(GameEntity gameEntity = null);
  }
}
