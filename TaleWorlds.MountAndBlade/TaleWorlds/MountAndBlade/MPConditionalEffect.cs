// Decompiled with JetBrains decompiler
// Type: TaleWorlds.MountAndBlade.MPConditionalEffect
// Assembly: TaleWorlds.MountAndBlade, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3C0E3F42-756F-4407-B817-06BA412A78B0
// Assembly location: E:\SteamLibrary\steamapps\common\Mount & Blade II Bannerlord\bin\Win64_Shipping_Client\TaleWorlds.MountAndBlade.dll

using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Xml;

namespace TaleWorlds.MountAndBlade
{
  public class MPConditionalEffect
  {
    public IReadOnlyList<MPPerkCondition> Conditions { get; }

    public IReadOnlyList<MPPerkEffectBase> Effects { get; }

    public MPPerkCondition.PerkEventFlags EventFlags
    {
      get
      {
        MPPerkCondition.PerkEventFlags perkEventFlags = MPPerkCondition.PerkEventFlags.None;
        foreach (MPPerkCondition condition in (IEnumerable<MPPerkCondition>) this.Conditions)
          perkEventFlags |= condition.EventFlags;
        return perkEventFlags;
      }
    }

    public bool IsTickRequired
    {
      get
      {
        foreach (MPPerkEffectBase effect in (IEnumerable<MPPerkEffectBase>) this.Effects)
        {
          if (effect.IsTickRequired)
            return true;
        }
        return false;
      }
    }

    public MPConditionalEffect(List<string> gameModes, XmlNode node)
    {
      List<MPPerkCondition> mpPerkConditionList = new List<MPPerkCondition>();
      List<MPPerkEffectBase> mpPerkEffectBaseList = new List<MPPerkEffectBase>();
      foreach (XmlNode childNode1 in node.ChildNodes)
      {
        if (childNode1.Name == nameof (Conditions))
        {
          foreach (XmlNode childNode2 in childNode1.ChildNodes)
          {
            if (childNode2.NodeType != XmlNodeType.Comment && childNode2.NodeType != XmlNodeType.SignificantWhitespace)
              mpPerkConditionList.Add(MPPerkCondition.CreateFrom(gameModes, childNode2));
          }
        }
        else if (childNode1.Name == nameof (Effects))
        {
          foreach (XmlNode childNode2 in childNode1.ChildNodes)
          {
            if (childNode2.NodeType != XmlNodeType.Comment && childNode2.NodeType != XmlNodeType.SignificantWhitespace)
            {
              MPPerkEffect from = MPPerkEffect.CreateFrom(childNode2);
              mpPerkEffectBaseList.Add((MPPerkEffectBase) from);
            }
          }
        }
      }
      this.Conditions = (IReadOnlyList<MPPerkCondition>) mpPerkConditionList;
      this.Effects = (IReadOnlyList<MPPerkEffectBase>) mpPerkEffectBaseList;
    }

    public bool Check(MissionPeer peer)
    {
      foreach (MPPerkCondition condition in (IEnumerable<MPPerkCondition>) this.Conditions)
      {
        if (!condition.Check(peer))
          return false;
      }
      return true;
    }

    public bool Check(Agent agent)
    {
      foreach (MPPerkCondition condition in (IEnumerable<MPPerkCondition>) this.Conditions)
      {
        if (!condition.Check(agent))
          return false;
      }
      return true;
    }

    public void OnEvent(
      MissionPeer peer,
      MPConditionalEffect.ConditionalEffectContainer container)
    {
      if (MultiplayerOptions.OptionType.NumberOfBotsPerFormation.GetIntValue() <= 0)
      {
        if (peer == null)
          return;
        bool? nullable = peer.ControlledAgent?.IsActive();
        bool flag = true;
        if (!(nullable.GetValueOrDefault() == flag & nullable.HasValue))
          return;
      }
      bool flag1 = true;
      foreach (MPPerkCondition condition in (IEnumerable<MPPerkCondition>) this.Conditions)
      {
        if (condition.IsPeerCondition && !condition.Check(peer))
        {
          flag1 = false;
          break;
        }
      }
      if (!flag1)
      {
        if (MultiplayerOptions.OptionType.NumberOfBotsPerFormation.GetIntValue() > 0)
        {
          List<IFormationUnit> allUnits = peer?.ControlledFormation?.arrangement.GetAllUnits();
          if (allUnits == null)
            return;
          foreach (IFormationUnit formationUnit in allUnits)
          {
            if (formationUnit is Agent agent7 && agent7.IsActive())
              this.UpdateAgentState(container, agent7, false);
          }
        }
        else
          this.UpdateAgentState(container, peer.ControlledAgent, false);
      }
      else if (MultiplayerOptions.OptionType.NumberOfBotsPerFormation.GetIntValue() > 0)
      {
        List<IFormationUnit> allUnits = peer?.ControlledFormation?.arrangement.GetAllUnits();
        if (allUnits == null)
          return;
        foreach (IFormationUnit formationUnit in allUnits)
        {
          if (formationUnit is Agent agent8 && agent8.IsActive())
          {
            bool state = true;
            foreach (MPPerkCondition condition in (IEnumerable<MPPerkCondition>) this.Conditions)
            {
              if (!condition.IsPeerCondition && !condition.Check(agent8))
              {
                state = false;
                break;
              }
            }
            this.UpdateAgentState(container, agent8, state);
          }
        }
      }
      else
      {
        bool state = true;
        foreach (MPPerkCondition condition in (IEnumerable<MPPerkCondition>) this.Conditions)
        {
          if (!condition.IsPeerCondition && !condition.Check(peer.ControlledAgent))
          {
            state = false;
            break;
          }
        }
        this.UpdateAgentState(container, peer.ControlledAgent, state);
      }
    }

    public void OnEvent(
      Agent agent,
      MPConditionalEffect.ConditionalEffectContainer container)
    {
      if (agent == null)
        return;
      bool state = true;
      foreach (MPPerkCondition condition in (IEnumerable<MPPerkCondition>) this.Conditions)
      {
        if (!condition.Check(agent))
        {
          state = false;
          break;
        }
      }
      this.UpdateAgentState(container, agent, state);
    }

    public void OnTick(MissionPeer peer, int tickCount)
    {
      if (MultiplayerOptions.OptionType.NumberOfBotsPerFormation.GetIntValue() <= 0)
      {
        if (peer == null)
          return;
        bool? nullable = peer.ControlledAgent?.IsActive();
        bool flag = true;
        if (!(nullable.GetValueOrDefault() == flag & nullable.HasValue))
          return;
      }
      bool flag1 = true;
      foreach (MPPerkCondition condition in (IEnumerable<MPPerkCondition>) this.Conditions)
      {
        if (condition.IsPeerCondition && !condition.Check(peer))
        {
          flag1 = false;
          break;
        }
      }
      if (!flag1)
        return;
      if (MultiplayerOptions.OptionType.NumberOfBotsPerFormation.GetIntValue() > 0)
      {
        List<IFormationUnit> allUnits = peer?.ControlledFormation?.arrangement.GetAllUnits();
        if (allUnits == null)
          return;
        foreach (IFormationUnit formationUnit in allUnits)
        {
          if (formationUnit is Agent agent3 && agent3.IsActive())
          {
            bool flag2 = true;
            foreach (MPPerkCondition condition in (IEnumerable<MPPerkCondition>) this.Conditions)
            {
              if (!condition.IsPeerCondition && !condition.Check(agent3))
              {
                flag2 = false;
                break;
              }
            }
            if (flag2)
            {
              foreach (MPPerkEffectBase effect in (IEnumerable<MPPerkEffectBase>) this.Effects)
              {
                if (effect.IsTickRequired)
                  effect.OnTick(agent3, tickCount);
              }
            }
          }
        }
      }
      else
      {
        bool flag2 = true;
        foreach (MPPerkCondition condition in (IEnumerable<MPPerkCondition>) this.Conditions)
        {
          if (!condition.IsPeerCondition && !condition.Check(peer.ControlledAgent))
          {
            flag2 = false;
            break;
          }
        }
        if (!flag2)
          return;
        foreach (MPPerkEffectBase effect in (IEnumerable<MPPerkEffectBase>) this.Effects)
        {
          if (effect.IsTickRequired)
            effect.OnTick(peer.ControlledAgent, tickCount);
        }
      }
    }

    private void UpdateAgentState(
      MPConditionalEffect.ConditionalEffectContainer container,
      Agent agent,
      bool state)
    {
      if (container.GetState(this, agent) == state)
        return;
      container.SetState(this, agent, state);
      foreach (MPPerkEffectBase effect in (IEnumerable<MPPerkEffectBase>) this.Effects)
        effect.OnUpdate(agent, state);
    }

    public class ConditionalEffectContainer : List<MPConditionalEffect>
    {
      private Dictionary<MPConditionalEffect, ConditionalWeakTable<Agent, MPConditionalEffect.ConditionalEffectContainer.ConditionState>> _states;

      public ConditionalEffectContainer()
      {
      }

      public ConditionalEffectContainer(
        IEnumerable<MPConditionalEffect> conditionalEffects)
        : base(conditionalEffects)
      {
      }

      public bool GetState(MPConditionalEffect conditionalEffect, Agent agent)
      {
        ConditionalWeakTable<Agent, MPConditionalEffect.ConditionalEffectContainer.ConditionState> conditionalWeakTable;
        MPConditionalEffect.ConditionalEffectContainer.ConditionState conditionState;
        return this._states != null && this._states.TryGetValue(conditionalEffect, out conditionalWeakTable) && conditionalWeakTable.TryGetValue(agent, out conditionState) && conditionState.IsSatisfied;
      }

      public void SetState(MPConditionalEffect conditionalEffect, Agent agent, bool state)
      {
        if (this._states == null)
        {
          this._states = new Dictionary<MPConditionalEffect, ConditionalWeakTable<Agent, MPConditionalEffect.ConditionalEffectContainer.ConditionState>>();
          ConditionalWeakTable<Agent, MPConditionalEffect.ConditionalEffectContainer.ConditionState> conditionalWeakTable = new ConditionalWeakTable<Agent, MPConditionalEffect.ConditionalEffectContainer.ConditionState>();
          conditionalWeakTable.Add(agent, new MPConditionalEffect.ConditionalEffectContainer.ConditionState()
          {
            IsSatisfied = state
          });
          this._states.Add(conditionalEffect, conditionalWeakTable);
        }
        else
        {
          ConditionalWeakTable<Agent, MPConditionalEffect.ConditionalEffectContainer.ConditionState> conditionalWeakTable1;
          if (!this._states.TryGetValue(conditionalEffect, out conditionalWeakTable1))
          {
            ConditionalWeakTable<Agent, MPConditionalEffect.ConditionalEffectContainer.ConditionState> conditionalWeakTable2 = new ConditionalWeakTable<Agent, MPConditionalEffect.ConditionalEffectContainer.ConditionState>();
            conditionalWeakTable2.Add(agent, new MPConditionalEffect.ConditionalEffectContainer.ConditionState()
            {
              IsSatisfied = state
            });
            this._states.Add(conditionalEffect, conditionalWeakTable2);
          }
          else
          {
            MPConditionalEffect.ConditionalEffectContainer.ConditionState conditionState;
            if (!conditionalWeakTable1.TryGetValue(agent, out conditionState))
              conditionalWeakTable1.Add(agent, new MPConditionalEffect.ConditionalEffectContainer.ConditionState()
              {
                IsSatisfied = state
              });
            else
              conditionState.IsSatisfied = state;
          }
        }
      }

      public void ResetStates() => this._states = (Dictionary<MPConditionalEffect, ConditionalWeakTable<Agent, MPConditionalEffect.ConditionalEffectContainer.ConditionState>>) null;

      private class ConditionState
      {
        public bool IsSatisfied { get; set; }
      }
    }
  }
}
