// Decompiled with JetBrains decompiler
// Type: TaleWorlds.MountAndBlade.MoraleAgentComponent
// Assembly: TaleWorlds.MountAndBlade, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3C0E3F42-756F-4407-B817-06BA412A78B0
// Assembly location: E:\SteamLibrary\steamapps\common\Mount & Blade II Bannerlord\bin\Win64_Shipping_Client\TaleWorlds.MountAndBlade.dll

using System;
using System.Linq;
using TaleWorlds.Core;
using TaleWorlds.Library;

namespace TaleWorlds.MountAndBlade
{
  public class MoraleAgentComponent : AgentComponent
  {
    private const float MoraleThresholdForPanicking = 0.01f;
    private RetreatAgentComponent retreatComponent;
    private FadeOutAgentComponent fadeOutComponent;
    private float _morale = 50f;

    public bool IsPanicked { get; private set; }

    public bool IsRetreating { get; private set; }

    public float Morale
    {
      get => this._morale;
      set => this._morale = MBMath.ClampFloat(value, 0.0f, 100f);
    }

    public MoraleAgentComponent(Agent agent)
      : base(agent)
    {
    }

    protected internal override void Initialize()
    {
      base.Initialize();
      this.retreatComponent = this.Agent.GetComponent<RetreatAgentComponent>();
      this.fadeOutComponent = this.Agent.GetComponent<FadeOutAgentComponent>();
      this.InitializeMorale();
    }

    private void InitializeMorale() => this.Morale = MBMath.ClampFloat(MissionGameModels.Current.BattleMoraleModel.GetEffectiveInitialMorale(this.Agent, (float) (35.0 + (double) MBRandom.RandomInt(30)) + this.Agent.Components.Sum<AgentComponent>((Func<AgentComponent, float>) (c => c.GetMoraleAddition()))), 15f, 100f);

    protected internal override void OnTickAsAI(float dt)
    {
      base.OnTickAsAI(dt);
      if (!this.IsRetreating && (double) this._morale < 0.00999999977648258)
      {
        if (MissionGameModels.Current.BattleMoraleModel.CanPanicDueToMorale(this.Agent))
          this.Panic();
        else
          this.Morale = 0.01f;
      }
      if (this.IsPanicked && this.retreatComponent != null)
        this.retreatComponent.ReevaluatePosition();
      if ((this.IsRetreating || this.Agent.IsRunningAway) && (this.Agent.RiderAgent == null && this.fadeOutComponent != null))
        this.fadeOutComponent.CheckFadeOut();
      if (this.Agent.Mission == null || !this.Agent.Mission.MissionEnded())
        return;
      MissionResult missionResult = this.Agent.Mission.MissionResult;
      if (this.Agent.Team == null || missionResult == null || (!missionResult.PlayerVictory || !this.Agent.Team.IsPlayerTeam && !this.Agent.Team.IsPlayerAlly) && (!missionResult.PlayerDefeated || this.Agent.Team.IsPlayerTeam || this.Agent.Team.IsPlayerAlly) || (this.Agent == Agent.Main || !this.Agent.IsActive() || !this.IsPanicked))
        return;
      this.StopRetreating();
    }

    public void Panic()
    {
      if (this.IsPanicked)
        return;
      this.IsPanicked = true;
      this.Agent.Mission.OnAgentPanicked(this.Agent);
    }

    public void Retreat()
    {
      if (this.IsRetreating)
        return;
      this.IsRetreating = true;
      if (this.retreatComponent == null)
        return;
      this.retreatComponent.Retreat();
    }

    public void StopRetreating()
    {
      if (!this.IsRetreating)
        return;
      this.IsRetreating = false;
      this.IsPanicked = false;
      this.Agent.SetMorale(0.02f);
      if (this.retreatComponent == null)
        return;
      this.retreatComponent.StopRetreating();
    }

    protected internal override void OnHit(
      Agent affectorAgent,
      int damage,
      in MissionWeapon affectorWeapon)
    {
      base.OnHit(affectorAgent, damage, in affectorWeapon);
      if (damage < 1 || !this.Agent.IsMount || (!this.Agent.IsAIControlled || this.Agent.RiderAgent != null))
        return;
      this.Panic();
    }
  }
}
