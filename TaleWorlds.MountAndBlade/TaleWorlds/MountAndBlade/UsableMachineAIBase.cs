// Decompiled with JetBrains decompiler
// Type: TaleWorlds.MountAndBlade.UsableMachineAIBase
// Assembly: TaleWorlds.MountAndBlade, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3C0E3F42-756F-4407-B817-06BA412A78B0
// Assembly location: E:\SteamLibrary\steamapps\common\Mount & Blade II Bannerlord\bin\Win64_Shipping_Client\TaleWorlds.MountAndBlade.dll

using NetworkMessages.FromServer;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using TaleWorlds.Core;
using TaleWorlds.DotNet;
using TaleWorlds.Engine;
using TaleWorlds.InputSystem;
using TaleWorlds.MountAndBlade.Network.Messages;

namespace TaleWorlds.MountAndBlade
{
  public abstract class UsableMachineAIBase
  {
    protected readonly UsableMachine UsableMachine;
    private GameEntity _lastActiveWaitStandingPoint;
    private int _tickCounter;
    private const int MovingAgentReassigningFrequency = 200;
    private const float CurrentMovingAgentBias = 0.9f;

    protected UsableMachineAIBase(UsableMachine usableMachine)
    {
      this.UsableMachine = usableMachine;
      this._lastActiveWaitStandingPoint = this.UsableMachine.WaitEntity;
    }

    public virtual bool HasActionCompleted => false;

    protected internal virtual Agent.AIScriptedFrameFlags GetScriptedFrameFlags(Agent agent) => Agent.AIScriptedFrameFlags.NoAttack;

    public void Tick(
      Func<Agent, bool> isAgentManagedByThisMachineAI,
      Team potentialUsersTeam,
      float dt)
    {
      this.OnTick(isAgentManagedByThisMachineAI, potentialUsersTeam, dt);
    }

    protected virtual void OnTick(
      Func<Agent, bool> isAgentManagedByThisMachineAI,
      Team potentialUsersTeam,
      float dt)
    {
      this._tickCounter = ++this._tickCounter % 200;
      foreach (StandingPoint standingPoint in this.UsableMachine.StandingPoints)
      {
        if (isAgentManagedByThisMachineAI(standingPoint.UserAgent))
        {
          Agent userAgent = standingPoint.UserAgent;
          if (this.HasActionCompleted || potentialUsersTeam != null && this.UsableMachine.IsDisabledForBattleSideAI(potentialUsersTeam.Side) || userAgent.IsRunningAway)
            this.HandleAgentStopUsingStandingPoint(userAgent, standingPoint);
        }
        List<KeyValuePair<Agent, UsableMissionObject.MoveInfo>> list = standingPoint.MovingAgents.ToList<KeyValuePair<Agent, UsableMissionObject.MoveInfo>>();
        List<Agent> agentList = new List<Agent>();
        for (int index = list.Count - 1; index >= 0; --index)
        {
          Agent key = list[index].Key;
          if (isAgentManagedByThisMachineAI(key))
          {
            if (this.HasActionCompleted || potentialUsersTeam != null && this.UsableMachine.IsDisabledForBattleSideAI(potentialUsersTeam.Side) || key.IsRunningAway)
            {
              this.HandleAgentStopUsingStandingPoint(key, standingPoint);
            }
            else
            {
              if (standingPoint.HasAlternative() && this.UsableMachine.IsInRangeToCheckAlternativePoints(key))
              {
                StandingPoint pointAlternativeTo = this.UsableMachine.GetBestPointAlternativeTo(standingPoint, key);
                if (pointAlternativeTo != standingPoint)
                {
                  standingPoint.OnMoveToStopped(key);
                  key.AIMoveToGameObjectEnable((UsableMissionObject) pointAlternativeTo, this.GetScriptedFrameFlags(key));
                  if (standingPoint == this.UsableMachine.CurrentlyUsedAmmoPickUpPoint)
                  {
                    this.UsableMachine.CurrentlyUsedAmmoPickUpPoint = pointAlternativeTo;
                    continue;
                  }
                  continue;
                }
              }
              if (standingPoint.HasUserPositionsChanged(key))
              {
                WorldFrame userFrameForAgent = standingPoint.GetUserFrameForAgent(key);
                string debugString = this.UsableMachine.GameEntity.Name + " Id:" + (object) this.UsableMachine.Id.Id + " " + standingPoint.GameEntity.Name;
                key.SetScriptedPositionAndDirection(ref userFrameForAgent.Origin, userFrameForAgent.Rotation.f.AsVec2.RotationInRadians, false, this.GetScriptedFrameFlags(key), debugString);
              }
              if (!standingPoint.IsDisabled && !standingPoint.HasUser && !key.IsAIPaused() && key.CanReachAndUseObject((UsableMissionObject) standingPoint, standingPoint.GetUserFrameForAgent(key).Origin.GetGroundVec3().DistanceSquared(key.Position)))
              {
                key.UseGameObject((UsableMissionObject) standingPoint);
                key.SetScriptedFlags(key.GetScriptedFlags() & ~standingPoint.DisableScriptedFrameFlags);
                agentList.Add(key);
              }
            }
          }
        }
      }
      if (!((NativeObject) this._lastActiveWaitStandingPoint != (NativeObject) this.UsableMachine.WaitEntity))
        return;
      foreach (Formation formation in potentialUsersTeam.FormationsIncludingSpecial.Where<Formation>((Func<Formation, bool>) (f => f.IsUsingMachine(this.UsableMachine) && f.MovementOrder.OrderEnum == MovementOrder.MovementOrderEnum.FollowEntity && (NativeObject) f.MovementOrder.TargetEntity == (NativeObject) this._lastActiveWaitStandingPoint)))
        formation.MovementOrder = !(this is SiegeTowerAI) ? MovementOrder.MovementOrderFollowEntity(this.UsableMachine.WaitEntity) : this.NextOrder;
      this._lastActiveWaitStandingPoint = this.UsableMachine.WaitEntity;
    }

    [Conditional("DEBUG")]
    private void OnTickDebug()
    {
      if (!Input.DebugInput.IsHotKeyDown("UsableMachineAiBaseHotkeyShowMachineUsers"))
        return;
      foreach (StandingPoint standingPoint in this.UsableMachine.StandingPoints)
      {
        foreach (KeyValuePair<Agent, UsableMissionObject.MoveInfo> movingAgent in standingPoint.MovingAgents)
        {
          Agent key = movingAgent.Key;
        }
        Agent userAgent = standingPoint.UserAgent;
      }
    }

    public static Agent GetSuitableAgentForStandingPoint(
      UsableMachine usableMachine,
      StandingPoint standingPoint,
      IEnumerable<Agent> agents,
      List<Agent> usedAgents)
    {
      if (usableMachine.AmmoPickUpPoints.Contains(standingPoint) && usableMachine.StandingPoints.Any<StandingPoint>((Func<StandingPoint, bool>) (standingPoint2 => (standingPoint2.IsDeactivated || standingPoint2.HasUser || standingPoint2.HasAIMovingTo) && !standingPoint2.GameEntity.HasTag(usableMachine.AmmoPickUpTag) && standingPoint2 is StandingPointWithWeaponRequirement)))
        return (Agent) null;
      IEnumerable<Agent> source = agents.Where<Agent>((Func<Agent, bool>) (a =>
      {
        if (usedAgents.Contains(a) || !a.IsAIControlled || (!a.IsActive() || a.IsRunningAway) || (a.AIUseGameObjectIsEnabled() || a.AIMoveToGameObjectIsEnabled() || standingPoint.IsDisabledForAgent(a)))
          return false;
        return a.Formation == null || !a.Formation.IsUnitDetached(a);
      }));
      return !source.Any<Agent>() ? (Agent) null : source.MaxBy<Agent, float>((Func<Agent, float>) (a => standingPoint.GetUsageScoreForAgent(a)));
    }

    public static Agent GetSuitableAgentForStandingPoint(
      UsableMachine usableMachine,
      StandingPoint standingPoint,
      IEnumerable<AgentValuePair<float>> agents,
      List<Agent> usedAgents,
      float weight)
    {
      if (usableMachine.IsStandingPointNotUsedOnAccountOfBeingAmmoLoad(standingPoint))
        return (Agent) null;
      IEnumerable<AgentValuePair<float>> source = agents.Where<AgentValuePair<float>>((Func<AgentValuePair<float>, bool>) (ap =>
      {
        Agent agent = ap.Agent;
        if (usedAgents.Contains(agent) || !agent.IsAIControlled || (!agent.IsActive() || agent.IsRunningAway) || (agent.AIUseGameObjectIsEnabled() || agent.AIMoveToGameObjectIsEnabled() || standingPoint.IsDisabledForAgent(agent)))
          return false;
        return agent.Formation == null || !agent.Formation.IsUnitDetached(agent) || (double) agent.DetachmentWeight * 0.400000005960464 > (double) weight;
      }));
      return !source.Any<AgentValuePair<float>>() ? (Agent) null : source.MaxBy<AgentValuePair<float>, float>((Func<AgentValuePair<float>, float>) (a => standingPoint.GetUsageScoreForAgent(a))).Agent;
    }

    protected virtual MovementOrder NextOrder => MovementOrder.MovementOrderStop;

    public virtual void TeleportUserAgentsToMachine(List<Agent> agentList)
    {
      int num1 = 0;
      bool flag;
      do
      {
        ++num1;
        flag = false;
        foreach (Agent agent in agentList)
        {
          if (agent.AIMoveToGameObjectIsEnabled())
          {
            flag = true;
            WorldFrame userFrameForAgent = this.UsableMachine.GetTargetStandingPointOfAIAgent(agent).GetUserFrameForAgent(agent);
            userFrameForAgent.Rotation.f.z = 0.0f;
            double num2 = (double) userFrameForAgent.Rotation.f.Normalize();
            if ((double) (agent.Position.AsVec2 - userFrameForAgent.Origin.AsVec2).LengthSquared > 9.99999974737875E-05 || (double) (agent.GetMovementDirection() - userFrameForAgent.Rotation.f).LengthSquared > 9.99999974737875E-05)
            {
              agent.TeleportToPosition(userFrameForAgent.Origin.GetGroundVec3());
              agent.SetMovementDirection(ref userFrameForAgent.Rotation.f);
              if (GameNetwork.IsServerOrRecorder)
              {
                GameNetwork.BeginBroadcastModuleEvent();
                GameNetwork.WriteMessage((GameNetworkMessage) new AgentTeleportToFrame(agent, userFrameForAgent.Origin.GetGroundVec3(), userFrameForAgent.Rotation.f.AsVec2));
                GameNetwork.EndBroadcastModuleEvent(GameNetwork.EventBroadcastFlags.AddToMissionRecord);
              }
            }
          }
        }
      }
      while (flag && num1 < 10);
    }

    protected virtual void HandleAgentStopUsingStandingPoint(
      Agent agent,
      StandingPoint standingPoint)
    {
      agent.StopUsingGameObject();
    }
  }
}
