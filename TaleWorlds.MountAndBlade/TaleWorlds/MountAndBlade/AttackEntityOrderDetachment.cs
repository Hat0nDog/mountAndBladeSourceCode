// Decompiled with JetBrains decompiler
// Type: TaleWorlds.MountAndBlade.AttackEntityOrderDetachment
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
  public class AttackEntityOrderDetachment : IDetachment
  {
    private const int Capacity = 8;
    private readonly List<Agent> _agents;
    private WorldFrame _frame;
    private readonly GameEntity _targetEntity;
    private readonly DestructableComponent _targetEntityDestructableComponent;
    private readonly bool _surroundEntity;

    public IEnumerable<Agent> Agents => (IEnumerable<Agent>) this._agents;

    public bool IsLoose => true;

    internal bool IsActive => this._targetEntityDestructableComponent != null && !this._targetEntityDestructableComponent.IsDestroyed;

    public AttackEntityOrderDetachment(GameEntity targetEntity)
    {
      this._targetEntity = targetEntity;
      this._targetEntityDestructableComponent = this._targetEntity.GetFirstScriptOfType<DestructableComponent>();
      this._surroundEntity = this._targetEntity.GetFirstScriptOfType<CastleGate>() == null;
      this._agents = new List<Agent>();
      MatrixFrame globalFrame = this._targetEntity.GetGlobalFrame();
      this._frame = new WorldFrame(globalFrame.rotation, new WorldPosition(Mission.Current.Scene, UIntPtr.Zero, globalFrame.origin, false));
      this._frame.Rotation.Orthonormalize();
    }

    public void AddAgent(Agent agent, int slotIndex)
    {
      this._agents.Add(agent);
      agent.SetScriptedTargetEntityAndPosition(this._targetEntity, new WorldPosition(this._targetEntity.Scene, UIntPtr.Zero, this._targetEntity.GlobalPosition, false), this._surroundEntity ? Agent.AISpecialCombatModeFlags.SurroundAttackEntity : Agent.AISpecialCombatModeFlags.None);
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
      for (int count = this._agents.Count; count < 8; ++count)
        valueTupleList.Add((count, AttackEntityOrderDetachment.CalculateWeight(count)));
      return valueTupleList;
    }

    bool IDetachment.IsSlotAtIndexAvailableForAgent(int slotIndex, Agent agent) => agent.CanBeAssignedForScriptedMovement() && slotIndex < 8 && slotIndex >= this._agents.Count;

    bool IDetachment.IsAgentEligible(Agent agent) => true;

    void IDetachment.UnmarkDetachment()
    {
    }

    bool IDetachment.IsDetachmentRecentlyEvaluated() => false;

    void IDetachment.MarkSlotAtIndex(int slotIndex)
    {
    }

    public bool IsStandingPointAvailableForAgent(Agent agent) => this._agents.Count < 8;

    public float[] GetTemplateCostsOfAgent(Agent candidate, float[] oldValue)
    {
      WorldPosition worldPosition = candidate.GetWorldPosition();
      WorldPosition origin = this._frame.Origin;
      float pathDistance;
      float num = Mission.Current.Scene.GetPathDistanceBetweenPositions(ref worldPosition, ref origin, candidate.Monster.BodyCapsuleRadius, out pathDistance) ? pathDistance : float.MaxValue;
      float[] numArray = oldValue == null || oldValue.Length != 8 ? new float[8] : oldValue;
      for (int index = 0; index < 8; ++index)
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
      if (this._agents.Count < 8)
      {
        Vec3 position = this._targetEntity.GlobalPosition;
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
      if (this._agents.Count < 8)
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
      Vec3 position = this._targetEntity.GlobalPosition;
      match = newAgents.MinBy<Agent, float>((Func<Agent, float>) (a => a.Position.DistanceSquared(position)));
      return new float?((float) (weightOfOccupiedSlot * 0.5));
    }

    public void RemoveAgent(Agent agent)
    {
      this._agents.Remove(agent);
      agent.DisableScriptedMovement();
      agent.DisableScriptedCombatMovement();
    }

    public WorldFrame? GetAgentFrame(Agent agent) => new WorldFrame?();

    private static float CalculateWeight(int index) => (float) (1.0 + (1.0 - (double) index / 8.0));

    public float? GetWeightOfNextSlot(BattleSideEnum side) => this._agents.Count < 8 ? new float?(AttackEntityOrderDetachment.CalculateWeight(this._agents.Count)) : new float?();

    public float GetWeightOfOccupiedSlot(Agent agent) => AttackEntityOrderDetachment.CalculateWeight(this._agents.IndexOf(agent));

    float IDetachment.GetDetachmentWeight(BattleSideEnum side) => this._agents.Count < 8 ? (float) ((double) (8 - this._agents.Count) * 1.0 / 8.0) : float.MinValue;
  }
}
