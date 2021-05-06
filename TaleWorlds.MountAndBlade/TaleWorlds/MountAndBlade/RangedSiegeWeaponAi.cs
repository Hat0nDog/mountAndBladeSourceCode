// Decompiled with JetBrains decompiler
// Type: TaleWorlds.MountAndBlade.RangedSiegeWeaponAi
// Assembly: TaleWorlds.MountAndBlade, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3C0E3F42-756F-4407-B817-06BA412A78B0
// Assembly location: E:\SteamLibrary\steamapps\common\Mount & Blade II Bannerlord\bin\Win64_Shipping_Client\TaleWorlds.MountAndBlade.dll

using System;
using TaleWorlds.Core;

namespace TaleWorlds.MountAndBlade
{
  public abstract class RangedSiegeWeaponAi : UsableMachineAIBase
  {
    private const float TargetEvaluationDelay = 0.5f;
    private const int MaxTargetEvaluationCount = 4;
    private ThreatSeeker threatSeeker;
    private Threat _target;
    private float _delayTimer;
    private float _delayDuration = 1f;
    private int _cannotShootCounter;
    private Timer _targetEvaluationTimer;

    public RangedSiegeWeaponAi(RangedSiegeWeapon rangedSiegeWeapon)
      : base((UsableMachine) rangedSiegeWeapon)
    {
      this.threatSeeker = new ThreatSeeker(rangedSiegeWeapon);
      (this.UsableMachine as RangedSiegeWeapon).OnReloadDone += new RangedSiegeWeapon.OnSiegeWeaponReloadDone(this.FindNextTarget);
      this._delayTimer = this._delayDuration;
      this._targetEvaluationTimer = new Timer(MBCommon.GetTime(MBCommon.TimeType.Mission), 0.5f);
    }

    protected override void OnTick(
      Func<Agent, bool> isAgentManagedByThisMachineAI,
      Team potentialUsersTeam,
      float dt)
    {
      base.OnTick(isAgentManagedByThisMachineAI, potentialUsersTeam, dt);
      if (this.UsableMachine.PilotAgent != null && this.UsableMachine.PilotAgent.IsAIControlled)
      {
        RangedSiegeWeapon usableMachine = this.UsableMachine as RangedSiegeWeapon;
        if (usableMachine.State == RangedSiegeWeapon.WeaponState.WaitingAfterShooting && usableMachine.PilotAgent != null && usableMachine.PilotAgent.IsAIControlled)
          usableMachine.ManualReload();
        if ((double) dt > 0.0 && this._target == null && usableMachine.State == RangedSiegeWeapon.WeaponState.Idle)
        {
          if ((double) this._delayTimer <= 0.0)
            this.FindNextTarget();
          this._delayTimer -= dt;
        }
        if (this._target != null)
        {
          if (this._target.Agent != null && !this._target.Agent.IsActive())
          {
            this._target = (Threat) null;
            return;
          }
          if (usableMachine.State == RangedSiegeWeapon.WeaponState.Idle && usableMachine.UserCount > 0)
          {
            if (DebugSiegeBehaviour.ToggleTargetDebug)
            {
              Agent pilotAgent = this.UsableMachine.PilotAgent;
            }
            if (this._targetEvaluationTimer.Check(MBCommon.GetTime(MBCommon.TimeType.Mission)) && !(this.UsableMachine as RangedSiegeWeapon).CanShootAtBox(this._target.BoundingBoxMin, this._target.BoundingBoxMax))
              ++this._cannotShootCounter;
            if (this._cannotShootCounter < 4)
            {
              if (usableMachine.AimAtTarget(this._target.Position) && usableMachine.PilotAgent != null)
              {
                this._delayTimer -= dt;
                if ((double) this._delayTimer <= 0.0)
                {
                  usableMachine.Shoot();
                  this._target = (Threat) null;
                  this.SetTargetingTimer();
                  this._cannotShootCounter = 0;
                  this._targetEvaluationTimer.Reset(MBCommon.GetTime(MBCommon.TimeType.Mission));
                }
              }
            }
            else
            {
              this._target = (Threat) null;
              this.SetTargetingTimer();
              this._cannotShootCounter = 0;
            }
          }
          else
            this._targetEvaluationTimer.Reset(MBCommon.GetTime(MBCommon.TimeType.Mission));
        }
      }
      this.AfterTick(isAgentManagedByThisMachineAI, potentialUsersTeam, dt);
    }

    public void FindNextTarget()
    {
      if (this.UsableMachine.PilotAgent == null || !this.UsableMachine.PilotAgent.IsAIControlled)
        return;
      this._target = this.threatSeeker.GetTarget();
      this.SetTargetingTimer();
    }

    private void AfterTick(
      Func<Agent, bool> isAgentManagedByThisMachineAI,
      Team potentialUsersTeam,
      float dt)
    {
      if ((double) dt > 0.0 && isAgentManagedByThisMachineAI(this.UsableMachine.PilotAgent) || this.UsableMachine.PilotAgent != null)
        return;
      this.threatSeeker.Release();
      this._target = (Threat) null;
    }

    private void SetTargetingTimer() => this._delayTimer = this._delayDuration + MBRandom.RandomFloat * 0.5f;
  }
}
