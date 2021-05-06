// Decompiled with JetBrains decompiler
// Type: TaleWorlds.MountAndBlade.BattleHighlightsController
// Assembly: TaleWorlds.MountAndBlade, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3C0E3F42-756F-4407-B817-06BA412A78B0
// Assembly location: E:\SteamLibrary\steamapps\common\Mount & Blade II Bannerlord\bin\Win64_Shipping_Client\TaleWorlds.MountAndBlade.dll

using System;
using System.Collections.Generic;
using System.Linq;
using TaleWorlds.Core;
using TaleWorlds.Library;

namespace TaleWorlds.MountAndBlade
{
  public class BattleHighlightsController : MissionLogic
  {
    private List<HighlightsController.HighlightType> _highlightTypes = new List<HighlightsController.HighlightType>()
    {
      new HighlightsController.HighlightType("hlid_kill_last_enemy_on_battlefield", "Take No Prisoners", "grpid_incidents", -5000, 3000, 0.0f, float.MaxValue, false),
      new HighlightsController.HighlightType("hlid_win_battle_as_last_man_standing", "Last Man Standing", "grpid_incidents", -5000, 3000, 0.0f, float.MaxValue, false),
      new HighlightsController.HighlightType("hlid_wall_break_kill", "Wall Break Kill", "grpid_incidents", -5000, 3000, 0.25f, 100f, true)
    };
    private HighlightsController _highlightsController;

    public override void AfterStart()
    {
      this._highlightsController = Mission.Current.GetMissionBehaviour<HighlightsController>();
      foreach (HighlightsController.HighlightType highlightType in this._highlightTypes)
        HighlightsController.AddHighlightType(highlightType);
    }

    public override void OnAgentRemoved(
      Agent affectedAgent,
      Agent affectorAgent,
      AgentState agentState,
      KillingBlow killingBlow)
    {
      if (affectorAgent == null || affectedAgent == null || (!affectorAgent.IsHuman || !affectedAgent.IsHuman) || agentState != AgentState.Killed && agentState != AgentState.Unconscious)
        return;
      bool flag1 = affectorAgent != null && affectorAgent.Team.IsPlayerTeam;
      bool flag2 = affectorAgent != null && affectorAgent.IsMainAgent;
      HighlightsController.Highlight highlight = new HighlightsController.Highlight();
      highlight.Start = Mission.Current.Time;
      highlight.End = Mission.Current.Time;
      bool flag3 = false;
      if (flag2 | flag1 && (killingBlow.WeaponRecordWeaponFlags.HasAllFlags<WeaponFlags>(WeaponFlags.Burning) || killingBlow.WeaponRecordWeaponFlags.HasAllFlags<WeaponFlags>(WeaponFlags.AffectsArea)))
      {
        highlight.HighlightType = this._highlightsController.GetHighlightTypeWithId("hlid_wall_break_kill");
        flag3 = true;
      }
      MBReadOnlyList<Agent> teamAgents1 = affectedAgent.Team?.TeamAgents;
      bool flag4 = teamAgents1 == null || teamAgents1.Any<Agent>((Func<Agent, bool>) (agent => agent.State != AgentState.Killed && agent.State != AgentState.Unconscious));
      if (!flag4 && flag2 | flag1)
      {
        highlight.HighlightType = this._highlightsController.GetHighlightTypeWithId("hlid_kill_last_enemy_on_battlefield");
        flag3 = true;
      }
      if (flag2)
      {
        MBReadOnlyList<Agent> teamAgents2 = affectorAgent.Team?.TeamAgents;
        if ((teamAgents2 == null ? 1 : (teamAgents2.Any<Agent>((Func<Agent, bool>) (agent => !agent.IsMainAgent && agent.State != AgentState.Killed && agent.State != AgentState.Unconscious)) ? 1 : 0)) == 0 && !flag4)
        {
          highlight.HighlightType = this._highlightsController.GetHighlightTypeWithId("hlid_win_battle_as_last_man_standing");
          flag3 = true;
        }
      }
      if (!flag3)
        return;
      this._highlightsController.SaveHighlight(highlight, affectedAgent.Position);
    }
  }
}
