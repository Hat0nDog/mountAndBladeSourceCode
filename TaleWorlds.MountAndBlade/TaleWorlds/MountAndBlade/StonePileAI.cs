// Decompiled with JetBrains decompiler
// Type: TaleWorlds.MountAndBlade.StonePileAI
// Assembly: TaleWorlds.MountAndBlade, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3C0E3F42-756F-4407-B817-06BA412A78B0
// Assembly location: E:\SteamLibrary\steamapps\common\Mount & Blade II Bannerlord\bin\Win64_Shipping_Client\TaleWorlds.MountAndBlade.dll

using System.Collections.Generic;

namespace TaleWorlds.MountAndBlade
{
  public class StonePileAI : UsableMachineAIBase
  {
    private const float HeightDifference = 4f;
    private const float RadialDistance = 10f;

    public StonePileAI(StonePile stonePile)
      : base((UsableMachine) stonePile)
    {
    }

    public override bool HasActionCompleted => (this.UsableMachine as StonePile).IsWaiting;

    public static Agent GetSuitableAgentForStandingPoint(
      StonePile usableMachine,
      StandingPoint standingPoint,
      IEnumerable<Agent> agents,
      List<Agent> usedAgents)
    {
      float num = float.MinValue;
      Agent agent1 = (Agent) null;
      foreach (Agent agent2 in agents)
      {
        if (StonePileAI.IsAgentAssignable(agent2) && !standingPoint.IsDisabledForAgent(agent2) && (double) standingPoint.GetUsageScoreForAgent(agent2) > (double) num)
        {
          num = standingPoint.GetUsageScoreForAgent(agent2);
          agent1 = agent2;
        }
      }
      return agent1;
    }

    public static Agent GetSuitableAgentForStandingPoint(
      StonePile stonePile,
      StandingPoint standingPoint,
      IEnumerable<AgentValuePair<float>> agents,
      List<Agent> usedAgents,
      float weight)
    {
      float num = float.MinValue;
      Agent agent1 = (Agent) null;
      foreach (AgentValuePair<float> agent2 in agents)
      {
        Agent agent3 = agent2.Agent;
        if (StonePileAI.IsAgentAssignable(agent3) && !standingPoint.IsDisabledForAgent(agent3) && (double) standingPoint.GetUsageScoreForAgent(agent3) > (double) num)
        {
          num = standingPoint.GetUsageScoreForAgent(agent3);
          agent1 = agent3;
        }
      }
      return agent1;
    }

    public static bool IsAgentAssignable(Agent agent)
    {
      if (agent == null || !agent.IsAIControlled || (!agent.IsActive() || agent.IsRunningAway) || (agent.AIUseGameObjectIsEnabled() || agent.AIMoveToGameObjectIsEnabled()))
        return false;
      return agent.Formation == null || !agent.Formation.IsUnitDetached(agent);
    }

    protected override void HandleAgentStopUsingStandingPoint(
      Agent agent,
      StandingPoint standingPoint)
    {
      if (standingPoint is StandingPointWithVolumeBox && agent.IsUsingGameObject)
        agent.DisableScriptedCombatMovement();
      base.HandleAgentStopUsingStandingPoint(agent, standingPoint);
    }
  }
}
