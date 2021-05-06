// Decompiled with JetBrains decompiler
// Type: TaleWorlds.MountAndBlade.BattleObserverMissionLogic
// Assembly: TaleWorlds.MountAndBlade, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3C0E3F42-756F-4407-B817-06BA412A78B0
// Assembly location: E:\SteamLibrary\steamapps\common\Mount & Blade II Bannerlord\bin\Win64_Shipping_Client\TaleWorlds.MountAndBlade.dll

using System;
using TaleWorlds.Core;

namespace TaleWorlds.MountAndBlade
{
  public class BattleObserverMissionLogic : MissionLogic
  {
    private int[] _builtAgentCountForSides;
    private int[] _removedAgentCountForSides;

    public IBattleObserver BattleObserver { get; private set; }

    public void SetObserver(IBattleObserver observer) => this.BattleObserver = observer;

    public override void AfterStart()
    {
      this._builtAgentCountForSides = new int[2];
      this._removedAgentCountForSides = new int[2];
    }

    public override void OnAgentBuild(Agent agent, Banner banner)
    {
      if (!agent.IsHuman)
        return;
      this.BattleObserver.TroopNumberChanged(agent.Team.Side, agent.Origin.BattleCombatant, agent.Character, 1);
      ++this._builtAgentCountForSides[(int) agent.Team.Side];
    }

    public override void OnAgentRemoved(
      Agent affectedAgent,
      Agent affectorAgent,
      AgentState agentState,
      KillingBlow blow)
    {
      if (affectedAgent.IsHuman)
      {
        switch (agentState)
        {
          case AgentState.Routed:
            this.BattleObserver.TroopNumberChanged(affectedAgent.Team.Side, affectedAgent.Origin.BattleCombatant, affectedAgent.Character, -1, numberRouted: 1);
            break;
          case AgentState.Unconscious:
            this.BattleObserver.TroopNumberChanged(affectedAgent.Team.Side, affectedAgent.Origin.BattleCombatant, affectedAgent.Character, -1, numberWounded: 1);
            break;
          case AgentState.Killed:
            this.BattleObserver.TroopNumberChanged(affectedAgent.Team.Side, affectedAgent.Origin.BattleCombatant, affectedAgent.Character, -1, 1);
            break;
          default:
            throw new ArgumentOutOfRangeException(nameof (agentState), (object) agentState, (string) null);
        }
        ++this._removedAgentCountForSides[(int) affectedAgent.Team.Side];
      }
      if (affectorAgent == null || !affectorAgent.IsHuman || !affectedAgent.IsHuman || agentState != AgentState.Unconscious && agentState != AgentState.Killed)
        return;
      this.BattleObserver.TroopNumberChanged(affectorAgent.Team.Side, affectorAgent.Origin.BattleCombatant, affectorAgent.Character, killCount: 1);
    }

    public override void OnMissionResultReady(MissionResult missionResult)
    {
      if (!missionResult.PlayerVictory)
        return;
      this.BattleObserver.BattleResultsReady();
    }

    public float GetDeathToBuiltAgentRatioForSide(BattleSideEnum side) => (float) this._removedAgentCountForSides[(int) side] / (float) this._builtAgentCountForSides[(int) side];
  }
}
