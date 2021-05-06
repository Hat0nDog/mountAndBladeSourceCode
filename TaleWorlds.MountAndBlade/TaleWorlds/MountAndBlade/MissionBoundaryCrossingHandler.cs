// Decompiled with JetBrains decompiler
// Type: TaleWorlds.MountAndBlade.MissionBoundaryCrossingHandler
// Assembly: TaleWorlds.MountAndBlade, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3C0E3F42-756F-4407-B817-06BA412A78B0
// Assembly location: E:\SteamLibrary\steamapps\common\Mount & Blade II Bannerlord\bin\Win64_Shipping_Client\TaleWorlds.MountAndBlade.dll

using NetworkMessages.FromServer;
using System;
using System.Collections.Generic;
using System.Linq;
using TaleWorlds.Core;
using TaleWorlds.Engine;
using TaleWorlds.Library;
using TaleWorlds.MountAndBlade.Network.Messages;

namespace TaleWorlds.MountAndBlade
{
  public class MissionBoundaryCrossingHandler : MissionLogic
  {
    private const float LeewayTime = 10f;
    private Dictionary<Agent, MissionTimer> _agentTimers;
    private MissionTimer _mainAgentLeaveTimer;

    public event Action<float, float> StartTime;

    public event Action StopTime;

    public event Action<float> TimeCount;

    public override void OnBehaviourInitialize()
    {
      base.OnBehaviourInitialize();
      if (GameNetwork.IsSessionActive)
        this.AddRemoveMessageHandlers(GameNetwork.NetworkMessageHandlerRegisterer.RegisterMode.Add);
      if (!GameNetwork.IsServer)
        return;
      this._agentTimers = new Dictionary<Agent, MissionTimer>();
    }

    public override void OnRemoveBehaviour()
    {
      if (GameNetwork.IsSessionActive)
        this.AddRemoveMessageHandlers(GameNetwork.NetworkMessageHandlerRegisterer.RegisterMode.Remove);
      base.OnRemoveBehaviour();
    }

    private void AddRemoveMessageHandlers(
      GameNetwork.NetworkMessageHandlerRegisterer.RegisterMode mode)
    {
      GameNetwork.NetworkMessageHandlerRegisterer handlerRegisterer = new GameNetwork.NetworkMessageHandlerRegisterer(mode);
      if (!GameNetwork.IsClient)
        return;
      handlerRegisterer.Register<SetBoundariesState>(new GameNetworkMessage.ServerMessageHandlerDelegate<SetBoundariesState>(this.HandleServerEventSetPeerBoundariesState));
    }

    private void OnAgentWentOut(Agent agent, float startTimeInSeconds)
    {
      MissionTimer missionTimer = GameNetwork.IsClient ? MissionTimer.CreateSynchedTimerClient(startTimeInSeconds, 10f) : new MissionTimer(10f);
      if (GameNetwork.IsServer)
      {
        this._agentTimers.Add(agent, missionTimer);
        MissionPeer missionPeer = agent.MissionPeer;
        NetworkCommunicator communicator = missionPeer != null ? missionPeer.GetNetworkPeer() : (NetworkCommunicator) null;
        if (communicator != null && !communicator.IsServerPeer)
        {
          GameNetwork.BeginModuleEventAsServer(communicator);
          GameNetwork.WriteMessage((GameNetworkMessage) new SetBoundariesState(true, missionTimer.GetStartTime().NumberOfTicks));
          GameNetwork.EndModuleEventAsServer();
        }
      }
      if (this.Mission.MainAgent != agent)
        return;
      this._mainAgentLeaveTimer = missionTimer;
      Action<float, float> startTime = this.StartTime;
      if (startTime != null)
        startTime(10f, 0.0f);
      MatrixFrame cameraFrame = Mission.Current.GetCameraFrame();
      Vec3 position = cameraFrame.origin + cameraFrame.rotation.u;
      if (Mission.Current.Mode != MissionMode.Battle)
        return;
      MBSoundEvent.PlaySound(SoundEvent.GetEventIdFromString("event:/alerts/report/out_of_map"), position);
    }

    private void OnAgentWentInOrRemoved(Agent agent, bool isAgentRemoved)
    {
      if (GameNetwork.IsServer)
      {
        this._agentTimers.Remove(agent);
        if (!isAgentRemoved)
        {
          MissionPeer missionPeer = agent.MissionPeer;
          NetworkCommunicator communicator = missionPeer != null ? missionPeer.GetNetworkPeer() : (NetworkCommunicator) null;
          if (communicator != null && !communicator.IsServerPeer)
          {
            GameNetwork.BeginModuleEventAsServer(communicator);
            GameNetwork.WriteMessage((GameNetworkMessage) new SetBoundariesState(false));
            GameNetwork.EndModuleEventAsServer();
          }
        }
      }
      if (this.Mission.MainAgent != agent)
        return;
      this._mainAgentLeaveTimer = (MissionTimer) null;
      Action stopTime = this.StopTime;
      if (stopTime == null)
        return;
      stopTime();
    }

    private void HandleAgentPunishment(Agent agent)
    {
      if (GameNetwork.IsSessionActive)
      {
        if (!GameNetwork.IsServer)
          return;
        Blow b = new Blow(agent.Index);
        b.WeaponRecord.FillAsMeleeBlow((ItemObject) null, (WeaponComponentData) null, -1, (sbyte) 0);
        b.DamageType = DamageTypes.Blunt;
        b.BaseMagnitude = 10000f;
        b.WeaponRecord.WeaponClass = WeaponClass.Undefined;
        b.Position = agent.Position;
        agent.Die(b);
      }
      else
        this.Mission.RetreatMission();
    }

    public override void OnClearScene()
    {
      if (GameNetwork.IsServer)
      {
        foreach (Agent agent in this._agentTimers.Keys.ToList<Agent>())
          this.OnAgentWentInOrRemoved(agent, true);
      }
      else
      {
        if (this._mainAgentLeaveTimer == null)
          return;
        if (this.Mission.MainAgent != null)
          this.OnAgentWentInOrRemoved(this.Mission.MainAgent, true);
        else
          this._mainAgentLeaveTimer = (MissionTimer) null;
      }
    }

    public override void OnAgentRemoved(
      Agent affectedAgent,
      Agent affectorAgent,
      AgentState agentState,
      KillingBlow blow)
    {
      this.OnAgentWentInOrRemoved(affectedAgent, true);
    }

    public override void OnMissionTick(float dt)
    {
      base.OnMissionTick(dt);
      if (GameNetwork.IsServer)
      {
        for (int index = this.Mission.Agents.Count - 1; index >= 0; --index)
        {
          Agent agent = this.Mission.Agents[index];
          if (agent.MissionPeer != null)
            this.TickForAgentAsServer(agent);
        }
      }
      else if (!GameNetwork.IsSessionActive && Agent.Main != null)
        this.TickForMainAgent();
      if (this._mainAgentLeaveTimer == null)
        return;
      this._mainAgentLeaveTimer.Check();
      float num = (float) (1.0 - (double) this._mainAgentLeaveTimer.GetRemainingTimeInSeconds(true) / 10.0);
      Action<float> timeCount = this.TimeCount;
      if (timeCount == null)
        return;
      timeCount(num);
    }

    private void TickForMainAgent() => this.HandleAgentStateChange(Agent.Main, !this.Mission.IsPositionInsideBoundaries(Agent.Main.Position.AsVec2), this._mainAgentLeaveTimer != null, this._mainAgentLeaveTimer);

    private void TickForAgentAsServer(Agent agent)
    {
      bool isAgentOutside = !this.Mission.IsPositionInsideBoundaries(agent.Position.AsVec2);
      bool isTimerActiveForAgent = this._agentTimers.ContainsKey(agent);
      this.HandleAgentStateChange(agent, isAgentOutside, isTimerActiveForAgent, isTimerActiveForAgent ? this._agentTimers[agent] : (MissionTimer) null);
    }

    private void HandleAgentStateChange(
      Agent agent,
      bool isAgentOutside,
      bool isTimerActiveForAgent,
      MissionTimer timerInstance)
    {
      if (isAgentOutside && !isTimerActiveForAgent)
        this.OnAgentWentOut(agent, 0.0f);
      else if (!isAgentOutside & isTimerActiveForAgent)
      {
        this.OnAgentWentInOrRemoved(agent, false);
      }
      else
      {
        if (!isAgentOutside || !timerInstance.Check())
          return;
        this.HandleAgentPunishment(agent);
      }
    }

    private void HandleServerEventSetPeerBoundariesState(SetBoundariesState message)
    {
      if (message.IsOutside)
        this.OnAgentWentOut(this.Mission.MainAgent, message.StateStartTimeInSeconds);
      else
        this.OnAgentWentInOrRemoved(this.Mission.MainAgent, false);
    }
  }
}
