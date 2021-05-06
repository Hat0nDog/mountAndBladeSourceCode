// Decompiled with JetBrains decompiler
// Type: TaleWorlds.MountAndBlade.AgentVictoryLogic
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
  public class AgentVictoryLogic : MissionLogic
  {
    private const float HighCheerThreshold = 0.25f;
    private const float MidCheerThreshold = 0.75f;
    private bool _reactionsStarted;
    private readonly ActionIndexCache[] _lowCheerActions = new ActionIndexCache[10]
    {
      ActionIndexCache.Create("act_cheering_low_01"),
      ActionIndexCache.Create("act_cheering_low_02"),
      ActionIndexCache.Create("act_cheering_low_03"),
      ActionIndexCache.Create("act_cheering_low_04"),
      ActionIndexCache.Create("act_cheering_low_05"),
      ActionIndexCache.Create("act_cheering_low_06"),
      ActionIndexCache.Create("act_cheering_low_07"),
      ActionIndexCache.Create("act_cheering_low_08"),
      ActionIndexCache.Create("act_cheering_low_09"),
      ActionIndexCache.Create("act_cheering_low_10")
    };
    private readonly ActionIndexCache[] _midCheerActions = new ActionIndexCache[4]
    {
      ActionIndexCache.Create("act_cheer_1"),
      ActionIndexCache.Create("act_cheer_2"),
      ActionIndexCache.Create("act_cheer_3"),
      ActionIndexCache.Create("act_cheer_4")
    };
    private readonly ActionIndexCache[] _highCheerActions = new ActionIndexCache[8]
    {
      ActionIndexCache.Create("act_cheering_high_01"),
      ActionIndexCache.Create("act_cheering_high_02"),
      ActionIndexCache.Create("act_cheering_high_03"),
      ActionIndexCache.Create("act_cheering_high_04"),
      ActionIndexCache.Create("act_cheering_high_05"),
      ActionIndexCache.Create("act_cheering_high_06"),
      ActionIndexCache.Create("act_cheering_high_07"),
      ActionIndexCache.Create("act_cheering_high_08")
    };
    private ActionIndexCache[] _selectedCheerActions;

    public override void AfterStart()
    {
      this.Mission.MissionCloseTimeAfterFinish = 60f;
      this._reactionsStarted = false;
    }

    public override void OnClearScene() => this._reactionsStarted = false;

    public override void OnAgentRemoved(
      Agent affectedAgent,
      Agent affectorAgent,
      AgentState agentState,
      KillingBlow killingBlow)
    {
      VictoryComponent component = affectedAgent.GetComponent<VictoryComponent>();
      if (component == null)
        return;
      affectedAgent.RemoveComponent((AgentComponent) component);
    }

    public override void OnMissionTick(float dt)
    {
      if (!this._reactionsStarted)
        return;
      this.CheckAnimationAndVoice();
    }

    private void CheckAnimationAndVoice()
    {
      for (int index = 0; index < this.Mission.Agents.Count; ++index)
      {
        Agent agent = this.Mission.Agents[index];
        VictoryComponent component = agent.GetComponent<VictoryComponent>();
        if (component != null && component.CheckTimer())
        {
          bool resetTimer;
          this.ChooseWeaponToCheerWithCheerAndUpdateTimer(agent, out resetTimer);
          if (resetTimer)
            component.ChangeTimerDuration(6f, 12f);
        }
      }
    }

    private void SelectVictoryCondition(BattleSideEnum side)
    {
      BattleObserverMissionLogic missionBehaviour = Mission.Current.GetMissionBehaviour<BattleObserverMissionLogic>();
      if (missionBehaviour != null)
      {
        float agentRatioForSide = missionBehaviour.GetDeathToBuiltAgentRatioForSide(side);
        if ((double) agentRatioForSide < 0.25)
          this._selectedCheerActions = this._highCheerActions;
        else if ((double) agentRatioForSide < 0.75)
          this._selectedCheerActions = this._midCheerActions;
        else
          this._selectedCheerActions = this._lowCheerActions;
      }
      else
        this._selectedCheerActions = this._midCheerActions;
    }

    public void SetTimersOfVictoryReactions(BattleSideEnum side)
    {
      this._reactionsStarted = true;
      this.SelectVictoryCondition(side);
      foreach (Formation formation in this.Mission.Teams.Where<Team>((Func<Team, bool>) (t => t.Side == side)).SelectMany<Team, Formation>((Func<Team, IEnumerable<Formation>>) (t => t.FormationsIncludingSpecial)))
        formation.MovementOrder = MovementOrder.MovementOrderStop;
      foreach (Agent agent in (IEnumerable<Agent>) this.Mission.Agents)
      {
        if (agent.IsHuman && agent.IsAIControlled && (agent.Team != null && side == agent.Team.Side) && (agent.IsAlarmed() && agent.GetComponent<VictoryComponent>() == null))
          agent.AddComponent((AgentComponent) new VictoryComponent(agent, new RandomTimer(this.Mission.Time, 1f, 8f)));
      }
    }

    public void SetTimersOfVictoryReactionsForRetreating(BattleSideEnum side)
    {
      this._reactionsStarted = true;
      this.SelectVictoryCondition(side);
      List<Agent> list = this.Mission.Agents.Where<Agent>((Func<Agent, bool>) (agent => agent.IsHuman && agent.IsAIControlled && agent.Team.Side == side)).ToList<Agent>();
      int num1 = (int) ((double) list.Count * 0.5);
      List<Agent> agentList = new List<Agent>();
      for (int index = 0; index < list.Count && agentList.Count != num1; ++index)
      {
        int num2 = list.Count - index - (num1 - agentList.Count);
        if ((double) MBRandom.RandomFloat <= 0.5 + 0.5 * (double) MBMath.ClampFloat((float) (num1 - num2) / (float) num1, 0.0f, 1f))
          agentList.Add(list[index]);
      }
      foreach (Agent agent in agentList)
      {
        MatrixFrame frame = agent.Frame;
        Vec2 asVec2 = frame.origin.AsVec2;
        Vec3 f = frame.rotation.f;
        agent.SetTargetPositionAndDirectionSynched(ref asVec2, ref f);
        this.SetTimersOfVictoryReactions(agent, 1f, 8f);
      }
    }

    public void SetTimersOfVictoryReactions(Agent agent, float minStartTime, float maxStartTime)
    {
      this._reactionsStarted = true;
      if (!agent.IsActive() || !agent.IsHuman || !agent.IsAIControlled)
        return;
      agent.AddComponent((AgentComponent) new VictoryComponent(agent, new RandomTimer(this.Mission.Time, minStartTime, maxStartTime)));
    }

    private void ChooseWeaponToCheerWithCheerAndUpdateTimer(Agent cheerAgent, out bool resetTimer)
    {
      resetTimer = false;
      if (cheerAgent.GetCurrentActionType(1) == Agent.ActionCodeType.EquipUnequip)
        return;
      EquipmentIndex wieldedItemIndex = cheerAgent.GetWieldedItemIndex(Agent.HandIndex.MainHand);
      bool flag = wieldedItemIndex != EquipmentIndex.None && !cheerAgent.Equipment[wieldedItemIndex].Item.ItemFlags.HasAnyFlag<ItemFlags>(ItemFlags.DropOnAnyAction);
      if (!flag)
      {
        EquipmentIndex slotIndex = EquipmentIndex.None;
        for (EquipmentIndex index = EquipmentIndex.WeaponItemBeginSlot; index < EquipmentIndex.Weapon4; ++index)
        {
          if ((cheerAgent.Equipment[index].IsEmpty ? 0 : (!cheerAgent.Equipment[index].Item.ItemFlags.HasAnyFlag<ItemFlags>(ItemFlags.DropOnAnyAction) ? 1 : 0)) != 0)
          {
            slotIndex = index;
            break;
          }
        }
        if (slotIndex == EquipmentIndex.None)
        {
          if (wieldedItemIndex != EquipmentIndex.None)
            cheerAgent.TryToSheathWeaponInHand(Agent.HandIndex.MainHand, Agent.WeaponWieldActionType.WithAnimation);
          else
            flag = true;
        }
        else
          cheerAgent.TryToWieldWeaponInSlot(slotIndex, Agent.WeaponWieldActionType.WithAnimation, false);
      }
      if (!flag)
        return;
      if (this._selectedCheerActions == null)
        this._selectedCheerActions = this._midCheerActions;
      cheerAgent.SetActionChannel(1, this._selectedCheerActions[MBRandom.RandomInt(this._selectedCheerActions.Length)]);
      cheerAgent.MakeVoice(SkinVoiceManager.VoiceType.Victory, SkinVoiceManager.CombatVoiceNetworkPredictionType.NoPrediction);
      resetTimer = true;
    }
  }
}
