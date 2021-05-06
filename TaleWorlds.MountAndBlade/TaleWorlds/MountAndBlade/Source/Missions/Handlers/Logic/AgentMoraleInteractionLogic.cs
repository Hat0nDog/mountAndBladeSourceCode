// Decompiled with JetBrains decompiler
// Type: TaleWorlds.MountAndBlade.Source.Missions.Handlers.Logic.AgentMoraleInteractionLogic
// Assembly: TaleWorlds.MountAndBlade, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3C0E3F42-756F-4407-B817-06BA412A78B0
// Assembly location: E:\SteamLibrary\steamapps\common\Mount & Blade II Bannerlord\bin\Win64_Shipping_Client\TaleWorlds.MountAndBlade.dll

using System;
using System.Collections.Generic;
using TaleWorlds.Core;
using TaleWorlds.Engine;

namespace TaleWorlds.MountAndBlade.Source.Missions.Handlers.Logic
{
  public class AgentMoraleInteractionLogic : MissionLogic
  {
    private const float DebacleVoiceChance = 0.7f;

    public override void OnAgentRemoved(
      Agent affectedAgent,
      Agent affectorAgent,
      AgentState agentState,
      KillingBlow killingBlow)
    {
      if (affectedAgent.Character == null || (affectorAgent?.Character == null || agentState != AgentState.Killed && agentState != AgentState.Unconscious || affectedAgent.Team == null))
        return;
      (float num1, float num2) = MissionGameModels.Current.BattleMoraleModel.CalculateMoraleChangeAfterAgentKilled(affectedAgent, affectorAgent, WeaponComponentData.GetRelevantSkillFromWeaponClass((WeaponClass) killingBlow.WeaponClass));
      if ((double) num1 == 0.0 && (double) num2 == 0.0)
        return;
      this.ApplyAoeMoraleEffect(affectedAgent, affectedAgent.GetWorldPosition(), affectorAgent.GetWorldPosition(), affectedAgent.Team, num1, num2, 10f);
    }

    public override void OnAgentFleeing(Agent affectedAgent)
    {
      if (affectedAgent.IsMount)
        return;
      (float num1, float num2) = MissionGameModels.Current.BattleMoraleModel.CalculateMoraleChangeAfterAgentPanicked(affectedAgent);
      if ((double) num1 == 0.0 && (double) num2 == 0.0)
        return;
      this.ApplyAoeMoraleEffect(affectedAgent, affectedAgent.GetWorldPosition(), affectedAgent.GetWorldPosition(), affectedAgent.Team, num1, num2, 10f);
      if ((double) MBRandom.RandomFloat >= 0.699999988079071)
        return;
      affectedAgent.MakeVoice(SkinVoiceManager.VoiceType.Debacle, SkinVoiceManager.CombatVoiceNetworkPredictionType.NoPrediction);
    }

    public void ApplyAoeMoraleEffect(
      Agent affectedAgent,
      WorldPosition affectedAgentPosition,
      WorldPosition affectorAgentPosition,
      Team affectedAgentTeam,
      float moraleChangeAffected,
      float moraleChangeAffector,
      float radius,
      Predicate<Agent> affectedCondition = null,
      Predicate<Agent> affectorCondition = null)
    {
      IEnumerable<Agent> nearbyAgents = this.Mission.GetNearbyAgents(affectedAgentPosition.AsVec2, radius);
      int num1 = 10;
      int val1 = 10;
      foreach (Agent agent in nearbyAgents)
      {
        if (agent.Team != null && agent.Team.IsValid)
        {
          float distance = agent.GetWorldPosition().GetNavMeshVec3().Distance(affectedAgentPosition.GetNavMeshVec3());
          if ((double) distance < (double) radius && agent.IsAIControlled)
          {
            if (agent.Team.IsEnemyOf(affectedAgentTeam))
            {
              if (num1 > 0 && (affectorCondition == null || affectorCondition(agent)))
              {
                float changeToCharacter = MissionGameModels.Current.BattleMoraleModel.CalculateMoraleChangeToCharacter(agent, moraleChangeAffector, distance);
                agent.ChangeMorale(changeToCharacter);
                --num1;
              }
            }
            else if (val1 > 0 && (affectedCondition == null || affectedCondition(agent)))
            {
              float changeToCharacter = MissionGameModels.Current.BattleMoraleModel.CalculateMoraleChangeToCharacter(agent, moraleChangeAffected, distance);
              agent.ChangeMorale(changeToCharacter);
              --val1;
            }
          }
        }
      }
      if (val1 <= 0)
        return;
      List<IFormationUnit> allUnits = affectedAgent.Formation?.arrangement?.GetAllUnits();
      if (allUnits == null)
        return;
      HashSet<int> intSet = new HashSet<int>();
      int count = allUnits.Count;
      int num2 = Math.Min(val1, allUnits.Count);
      for (int index = count - num2; index < count; ++index)
      {
        int num3 = MBRandom.RandomInt(0, index + 1);
        intSet.Add(intSet.Contains(num3) ? index : num3);
      }
      foreach (int index in intSet)
      {
        if (allUnits[index] is Agent agent1 && agent1.IsActive() && agent1.IsAIControlled)
        {
          float distance = agent1.GetWorldPosition().GetNavMeshVec3().Distance(affectedAgentPosition.GetNavMeshVec3());
          float changeToCharacter = MissionGameModels.Current.BattleMoraleModel.CalculateMoraleChangeToCharacter(agent1, moraleChangeAffected, distance);
          agent1.ChangeMorale(changeToCharacter);
        }
      }
    }
  }
}
