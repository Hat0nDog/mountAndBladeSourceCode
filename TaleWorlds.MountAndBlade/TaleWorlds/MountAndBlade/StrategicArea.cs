// Decompiled with JetBrains decompiler
// Type: TaleWorlds.MountAndBlade.StrategicArea
// Assembly: TaleWorlds.MountAndBlade, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3C0E3F42-756F-4407-B817-06BA412A78B0
// Assembly location: E:\SteamLibrary\steamapps\common\Mount & Blade II Bannerlord\bin\Win64_Shipping_Client\TaleWorlds.MountAndBlade.dll

using System;
using System.Collections.Generic;
using System.Linq;
using TaleWorlds.Core;
using TaleWorlds.Engine;
using TaleWorlds.Library;

namespace TaleWorlds.MountAndBlade
{
  public class StrategicArea : MissionObject, IDetachment
  {
    private List<Agent> _agents;
    private WorldFrame _frame;
    [EditableScriptComponentVariable(true)]
    private float _width;
    private int _unitSpacing;
    private int _capacity;
    private Dictionary<Formation, Formation> _simulationFormations;
    [EditableScriptComponentVariable(true)]
    private BattleSideEnum _side;
    [EditableScriptComponentVariable(true)]
    private float _depth = 1f;
    [EditableScriptComponentVariable(true)]
    private float _distanceToCheck = 10f;
    [EditableScriptComponentVariable(true)]
    private bool _ignoreHeight = true;
    private List<DestructableComponent> _nearbyDestructibleObjects = new List<DestructableComponent>();
    private bool _isActive;
    private float _lastShimmyTime;
    private float _lastShootTime;
    private StrategicArea.ShimmyDirection _shimmyDirection;
    private bool _doesFrameNeedUpdate = true;
    private readonly StrategicArea.StrategicAreaMutableTuple[] _strategicAreaSidesScoreTally = new StrategicArea.StrategicAreaMutableTuple[5];
    private WorldFrame? _centerFrame;
    private WorldFrame _cachedWorldFrame;

    public IEnumerable<Agent> Agents => (IEnumerable<Agent>) this._agents;

    public bool IsLoose => true;

    internal float DistanceToCheck => this._distanceToCheck;

    internal bool IgnoreHeight => this._ignoreHeight;

    internal bool IsActive
    {
      get => this._isActive;
      set
      {
        if (value == this._isActive)
          return;
        List<Team> list = Mission.Current.Teams.Where<Team>((Func<Team, bool>) (t => this.IsUsableBy(t.Side))).ToList<Team>();
        this._isActive = value;
        foreach (Team team in list)
        {
          if (team.TeamAI != null)
          {
            if (this._isActive)
              team.TeamAI.AddStrategicArea(this);
            else
              team.TeamAI.RemoveStrategicArea(this);
          }
        }
      }
    }

    protected internal override void OnInit()
    {
      base.OnInit();
      this._agents = new List<Agent>();
      MatrixFrame globalFrame = this.GameEntity.GetGlobalFrame();
      this._frame = new WorldFrame(globalFrame.rotation, new WorldPosition(Mission.Current.Scene, UIntPtr.Zero, globalFrame.origin, false));
      this._frame.Rotation.Orthonormalize();
      this._unitSpacing = ArrangementOrder.GetUnitSpacingOf(ArrangementOrder.ArrangementOrderEnum.Line);
      this._capacity = Math.Max(1, (int) Math.Ceiling((double) Math.Max(1f, this._width) * (double) Math.Max(1f, this._depth)));
      this._simulationFormations = new Dictionary<Formation, Formation>();
      this._isActive = true;
      for (int index = 0; index < 5; ++index)
        this._strategicAreaSidesScoreTally[index] = new StrategicArea.StrategicAreaMutableTuple(0, 0);
    }

    internal void DetermineAssociatedDestructibleComponents(
      IEnumerable<DestructableComponent> destructibleComponents)
    {
      this._nearbyDestructibleObjects = new List<DestructableComponent>();
      foreach (DestructableComponent destructibleComponent in destructibleComponents)
      {
        if ((double) destructibleComponent.GameEntity.GetGlobalFrame().TransformToParent((destructibleComponent.GameEntity.GetBoundingBoxMax() + destructibleComponent.GameEntity.GetBoundingBoxMin()) * 0.5f).DistanceSquared(this.GameEntity.GlobalPosition) <= 9.0)
          this._nearbyDestructibleObjects.Add(destructibleComponent);
      }
      foreach (DestructableComponent destructibleObject in this._nearbyDestructibleObjects)
        destructibleObject.OnDestroyed += new DestructableComponent.OnHitTakenAndDestroyedDelegate(this.OnCoveringDestructibleObjectDestroyed);
    }

    public void OnParentGameEntityVisibilityChanged(bool isVisible) => this.IsActive = isVisible;

    private void OnCoveringDestructibleObjectDestroyed(
      DestructableComponent destroyedComponent,
      Agent destroyerAgent,
      in MissionWeapon weapon,
      ScriptComponentBehaviour attackerScriptComponentBehaviour,
      int inflictedDamage)
    {
      this.IsActive = false;
    }

    protected override void OnRemoved(int removeReason)
    {
      base.OnRemoved(removeReason);
      foreach (DestructableComponent destructibleObject in this._nearbyDestructibleObjects)
        destructibleObject.OnDestroyed -= new DestructableComponent.OnHitTakenAndDestroyedDelegate(this.OnCoveringDestructibleObjectDestroyed);
    }

    public void InitializeAutogenerated(float width, int capacity, BattleSideEnum side)
    {
      this._width = width;
      this._capacity = capacity;
      this._side = side;
    }

    public void AddAgent(Agent agent, int slotIndex)
    {
      this._agents.Add(agent);
      if (this._capacity != 1 || this._centerFrame.HasValue)
        return;
      Formation formation = agent.Formation;
      Formation simulationFormation = this.GetSimulationFormation(formation);
      this._centerFrame = formation.GetUnitPositionWithIndexAccordingToNewOrder(simulationFormation, 0, ref this._frame, this._width, this._unitSpacing, this._agents.Count);
      int num = this._centerFrame.HasValue ? 1 : 0;
      this._cachedWorldFrame = this._centerFrame ?? agent.GetWorldFrame();
    }

    public void AddAgentAtSlotIndex(Agent agent, int slotIndex)
    {
      this.AddAgent(agent, slotIndex);
      if (agent.Formation != null)
        agent.Formation.DetachUnit(agent, true);
      agent.DetachmentWeight = 1f;
      agent.Detachment = (IDetachment) this;
    }

    Agent IDetachment.GetMovingAgentAtSlotIndex(int slotIndex) => (Agent) null;

    List<(int, float)> IDetachment.GetSlotIndexWeightTuples()
    {
      List<(int, float)> valueTupleList = new List<(int, float)>();
      for (int count = this._agents.Count; count < this._capacity; ++count)
        valueTupleList.Add((count, StrategicArea.CalculateWeight(this._capacity, count)));
      return valueTupleList;
    }

    bool IDetachment.IsSlotAtIndexAvailableForAgent(int slotIndex, Agent agent) => agent.CanBeAssignedForScriptedMovement() && slotIndex < this._capacity && slotIndex >= this._agents.Count && this.IsAgentEligible(agent);

    public bool IsAgentEligible(Agent agent) => agent.IsRangedCached;

    void IDetachment.UnmarkDetachment()
    {
    }

    bool IDetachment.IsDetachmentRecentlyEvaluated() => false;

    void IDetachment.MarkSlotAtIndex(int slotIndex)
    {
    }

    public bool IsStandingPointAvailableForAgent(Agent agent) => this._agents.Count < this._capacity;

    public float[] GetTemplateCostsOfAgent(Agent candidate, float[] oldValue)
    {
      WorldPosition worldPosition = candidate.GetWorldPosition();
      WorldPosition origin = this._frame.Origin;
      float pathDistance;
      float num = Mission.Current.Scene.GetPathDistanceBetweenPositions(ref worldPosition, ref origin, candidate.Monster.BodyCapsuleRadius, out pathDistance) ? pathDistance : float.MaxValue;
      float[] numArray = oldValue == null || oldValue.Length != this._capacity ? new float[this._capacity] : oldValue;
      for (int index = 0; index < this._capacity; ++index)
        numArray[index] = num;
      return numArray;
    }

    float IDetachment.GetExactCostOfAgentAtSlot(Agent candidate, int slotIndex) => 0.0f;

    public float GetTemplateWeightOfAgent(Agent candidate)
    {
      WorldPosition worldPosition = candidate.GetWorldPosition();
      WorldPosition origin = this._frame.Origin;
      float pathDistance;
      return !Mission.Current.Scene.GetPathDistanceBetweenPositions(ref worldPosition, ref origin, candidate.Monster.BodyCapsuleRadius, out pathDistance) ? float.MaxValue : pathDistance;
    }

    public float? GetWeightOfAgentAtNextSlot(IEnumerable<Agent> newAgents, out Agent match)
    {
      float? weightOfNextSlot = this.GetWeightOfNextSlot(newAgents.First<Agent>().Team.Side);
      if (this._agents.Count < this._capacity)
      {
        Vec3 position = this.GameEntity.GlobalPosition;
        match = newAgents.MinBy<Agent, float>((Func<Agent, float>) (a => a.Position.DistanceSquared(position)));
        return weightOfNextSlot;
      }
      match = (Agent) null;
      return new float?();
    }

    public float? GetWeightOfAgentAtNextSlot(
      IEnumerable<AgentValuePair<float>> agentTemplateScores,
      out Agent match)
    {
      float? weight = this.GetWeightOfNextSlot(agentTemplateScores.First<AgentValuePair<float>>().Agent.Team.Side);
      if (this._agents.Count < this._capacity)
      {
        IEnumerable<AgentValuePair<float>> source = agentTemplateScores.Where<AgentValuePair<float>>((Func<AgentValuePair<float>, bool>) (a =>
        {
          Agent agent = a.Agent;
          if (!agent.IsDetachedFromFormation)
            return true;
          double detachmentWeight = (double) agent.DetachmentWeight;
          float? nullable1 = weight;
          float num = 0.4f;
          float? nullable2 = nullable1.HasValue ? new float?(nullable1.GetValueOrDefault() * num) : new float?();
          double valueOrDefault = (double) nullable2.GetValueOrDefault();
          return detachmentWeight < valueOrDefault & nullable2.HasValue;
        }));
        if (source.Any<AgentValuePair<float>>())
        {
          match = source.MinBy<AgentValuePair<float>, float>((Func<AgentValuePair<float>, float>) (a => a.Value)).Agent;
          return weight;
        }
      }
      match = (Agent) null;
      return new float?();
    }

    public float? GetWeightOfAgentAtOccupiedSlot(
      Agent detachedAgent,
      IEnumerable<Agent> newAgents,
      out Agent match)
    {
      double weightOfOccupiedSlot = (double) this.GetWeightOfOccupiedSlot(detachedAgent);
      Vec3 position = this.GameEntity.GlobalPosition;
      match = newAgents.MinBy<Agent, float>((Func<Agent, float>) (a => a.Position.DistanceSquared(position)));
      return new float?((float) (weightOfOccupiedSlot * 0.5));
    }

    public void RemoveAgent(Agent agent) => this._agents.Remove(agent);

    private Formation GetSimulationFormation(Formation formation)
    {
      if (!this._simulationFormations.ContainsKey(formation))
        this._simulationFormations[formation] = new Formation((Team) null, -1);
      return this._simulationFormations[formation];
    }

    public WorldFrame? GetAgentFrame(Agent agent)
    {
      if (this._capacity > 1)
      {
        int unitIndex = this._agents.IndexOf(agent);
        Formation formation = agent.Formation;
        Formation simulationFormation = this.GetSimulationFormation(formation);
        WorldFrame? accordingToNewOrder = formation.GetUnitPositionWithIndexAccordingToNewOrder(simulationFormation, unitIndex, ref this._frame, this._width, this._unitSpacing, this._agents.Count);
        if (accordingToNewOrder.HasValue)
          return accordingToNewOrder;
        if (!this._centerFrame.HasValue)
          MBDebug.ShowWarning("Strategic archer position at position at X=" + (object) this._frame.Origin.Position.x + " Y=" + (object) this._frame.Origin.Position.y + " Z=" + (object) this._frame.Origin.Position.z + "doesn't yield a viable frame. It may be in the air, underground or off the navmesh, please check.Scene: " + this.Scene.GetName());
        return new WorldFrame?(agent.GetWorldFrame());
      }
      float time = MBCommon.TimeType.Mission.GetTime();
      StrategicArea.ShimmyDirection shimmyDirection = this._shimmyDirection;
      int num1 = ((IEnumerable<StrategicArea.StrategicAreaMutableTuple>) this._strategicAreaSidesScoreTally).Count<StrategicArea.StrategicAreaMutableTuple>((Func<StrategicArea.StrategicAreaMutableTuple, bool>) (sasst => sasst != null)) > 1 ? 1 : 0;
      if (num1 != 0 && (double) this._lastShootTime < (double) agent.LastRangedAttackTime)
      {
        this._lastShootTime = agent.LastRangedAttackTime;
        StrategicArea.StrategicAreaMutableTuple areaMutableTuple = this._strategicAreaSidesScoreTally[(int) this._shimmyDirection];
        if (areaMutableTuple != null)
          ++areaMutableTuple.RangedHitScoredCount;
        else
          this._strategicAreaSidesScoreTally[(int) this._shimmyDirection] = new StrategicArea.StrategicAreaMutableTuple(0, 1);
      }
      bool flag1 = false;
      if (num1 != 0 && (double) this._lastShimmyTime < (double) agent.LastRangedHitTime)
      {
        StrategicArea.StrategicAreaMutableTuple areaMutableTuple = this._strategicAreaSidesScoreTally[(int) this._shimmyDirection];
        if (areaMutableTuple != null)
          ++areaMutableTuple.RangedHitReceivedCount;
        else
          this._strategicAreaSidesScoreTally[(int) this._shimmyDirection] = new StrategicArea.StrategicAreaMutableTuple(1, 0);
        flag1 = true;
      }
      bool flag2 = false;
      if (num1 != 0 && !flag1 && (double) time - (double) Math.Max(agent.LastRangedAttackTime, this._lastShimmyTime) > 8.0)
      {
        StrategicArea.StrategicAreaMutableTuple areaMutableTuple = this._strategicAreaSidesScoreTally[(int) this._shimmyDirection];
        if (areaMutableTuple != null)
          --areaMutableTuple.RangedHitScoredCount;
        else
          this._strategicAreaSidesScoreTally[(int) this._shimmyDirection] = new StrategicArea.StrategicAreaMutableTuple(0, -1);
        flag2 = true;
      }
      if (flag1 | flag2)
      {
        int num2 = int.MinValue;
        int num3 = 0;
        for (int index = 0; index < 5; ++index)
        {
          if ((StrategicArea.ShimmyDirection) index != this._shimmyDirection && this._strategicAreaSidesScoreTally[index] != null)
          {
            int num4 = this._strategicAreaSidesScoreTally[index].RangedHitScoredCount - this._strategicAreaSidesScoreTally[index].RangedHitReceivedCount;
            if (num4 > num2)
            {
              num2 = num4;
              num3 = 1;
            }
            else if (num4 == num2)
              ++num3;
          }
        }
        int num5 = MBRandom.RandomInt(num3 - 1);
        for (int index = 0; index < 5; ++index)
        {
          if ((StrategicArea.ShimmyDirection) index != this._shimmyDirection && this._strategicAreaSidesScoreTally[index] != null && (this._strategicAreaSidesScoreTally[index].RangedHitScoredCount - this._strategicAreaSidesScoreTally[index].RangedHitReceivedCount == num2 && --num5 < 0))
            shimmyDirection = (StrategicArea.ShimmyDirection) index;
        }
        this._doesFrameNeedUpdate = true;
      }
      if (!this._doesFrameNeedUpdate)
        return new WorldFrame?(this._cachedWorldFrame);
      WorldFrame? centerFrame = this._centerFrame;
      if (centerFrame.HasValue)
      {
        WorldPosition origin = centerFrame.Value.Origin;
        Mat3 rotation = centerFrame.Value.Rotation;
        Vec2 vec2;
        switch (shimmyDirection)
        {
          case StrategicArea.ShimmyDirection.Center:
            vec2 = Vec2.Zero;
            break;
          case StrategicArea.ShimmyDirection.Left:
            vec2 = rotation.s.AsVec2;
            break;
          case StrategicArea.ShimmyDirection.Forward:
            vec2 = rotation.f.AsVec2;
            break;
          case StrategicArea.ShimmyDirection.Right:
            vec2 = -rotation.s.AsVec2;
            break;
          case StrategicArea.ShimmyDirection.Back:
            vec2 = -rotation.f.AsVec2;
            break;
          default:
            vec2 = Vec2.Zero;
            break;
        }
        WorldPosition worldPosition = origin;
        int num2 = 8;
        bool flag3 = false;
        while (num2-- > 0)
        {
          origin.SetVec2(worldPosition.AsVec2 + (float) (0.600000023841858 + 0.0500000007450581 * (double) num2) * vec2);
          if (origin.GetNavMesh() != UIntPtr.Zero)
          {
            flag3 = true;
            break;
          }
        }
        this._doesFrameNeedUpdate = false;
        if (!flag3)
        {
          this._strategicAreaSidesScoreTally[(int) shimmyDirection] = (StrategicArea.StrategicAreaMutableTuple) null;
        }
        else
        {
          this._shimmyDirection = shimmyDirection;
          this._lastShimmyTime = time;
          this._cachedWorldFrame = new WorldFrame(centerFrame.Value.Rotation, new WorldPosition(agent.Mission.Scene, origin.GetNavMeshVec3()));
        }
        return new WorldFrame?(this._cachedWorldFrame);
      }
      MBDebug.ShowWarning("Strategic archer position at position at X=" + (object) this._frame.Origin.Position.x + " Y=" + (object) this._frame.Origin.Position.y + " Z=" + (object) this._frame.Origin.Position.z + "doesn't yield a viable frame. It may be in the air, underground or off the navmesh, please check.Scene: " + this.Scene.GetName());
      return new WorldFrame?(agent.GetWorldFrame());
    }

    private static float CalculateWeight(int capacity, int index) => (float) ((double) (capacity - index) * 1.0 / (double) capacity * 0.5);

    public float? GetWeightOfNextSlot(BattleSideEnum side) => this._agents.Count < this._capacity ? new float?(StrategicArea.CalculateWeight(this._capacity, this._agents.Count)) : new float?();

    public float GetWeightOfOccupiedSlot(Agent agent) => StrategicArea.CalculateWeight(this._capacity, this._agents.IndexOf(agent));

    public bool IsUsableBy(BattleSideEnum side) => this._side == side || this._side == BattleSideEnum.None;

    float IDetachment.GetDetachmentWeight(BattleSideEnum side) => this._agents.Count < this._capacity ? (float) (this._capacity - this._agents.Count) * 1f / (float) this._capacity : float.MinValue;

    private class StrategicAreaMutableTuple
    {
      public int RangedHitReceivedCount;
      public int RangedHitScoredCount;

      internal StrategicAreaMutableTuple(int rangedHitReceivedCount, int rangedHitScoredCount)
      {
        this.RangedHitReceivedCount = rangedHitReceivedCount;
        this.RangedHitScoredCount = rangedHitScoredCount;
      }
    }

    private enum ShimmyDirection
    {
      Center,
      Left,
      Forward,
      Right,
      Back,
      NumDirections,
    }
  }
}
