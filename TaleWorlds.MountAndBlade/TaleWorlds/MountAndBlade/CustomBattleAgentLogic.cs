// Decompiled with JetBrains decompiler
// Type: TaleWorlds.MountAndBlade.CustomBattleAgentLogic
// Assembly: TaleWorlds.MountAndBlade, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3C0E3F42-756F-4407-B817-06BA412A78B0
// Assembly location: E:\SteamLibrary\steamapps\common\Mount & Blade II Bannerlord\bin\Win64_Shipping_Client\TaleWorlds.MountAndBlade.dll

using TaleWorlds.Core;

namespace TaleWorlds.MountAndBlade
{
  public class CustomBattleAgentLogic : MissionLogic
  {
    public override void OnAgentHit(
      Agent affectedAgent,
      Agent affectorAgent,
      int damage,
      in MissionWeapon affectorWeapon)
    {
      if (affectedAgent.Character == null || (affectorAgent?.Character == null || affectedAgent.State != AgentState.Active))
        return;
      bool isFatal = (double) affectedAgent.Health - (double) damage < 1.0;
      bool isTeamKill = affectedAgent.Team.Side == affectorAgent.Team.Side;
      affectorAgent.Origin.OnScoreHit(affectedAgent.Character, affectorAgent?.Formation?.Captain?.Character, damage, isFatal, isTeamKill, affectorWeapon.CurrentUsageItem);
    }

    public override void OnAgentRemoved(
      Agent affectedAgent,
      Agent affectorAgent,
      AgentState agentState,
      KillingBlow killingBlow)
    {
      if (affectorAgent == null && affectedAgent.IsMount && agentState == AgentState.Routed || affectedAgent.Origin == null)
        return;
      switch (agentState)
      {
        case AgentState.Unconscious:
          affectedAgent.Origin.SetWounded();
          if (affectedAgent != this.Mission.MainAgent)
            break;
          this.BecomeGhost();
          break;
        case AgentState.Killed:
          affectedAgent.Origin.SetKilled();
          break;
        default:
          affectedAgent.Origin.SetRouted();
          break;
      }
    }

    private void BecomeGhost()
    {
      Agent leader = this.Mission.PlayerEnemyTeam.Leader;
      if (leader != null)
        leader.Controller = Agent.ControllerType.AI;
      this.Mission.MainAgent.Controller = Agent.ControllerType.AI;
    }
  }
}
