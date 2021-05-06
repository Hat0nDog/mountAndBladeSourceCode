// Decompiled with JetBrains decompiler
// Type: TaleWorlds.MountAndBlade.MissionRepresentatives.DuelMissionRepresentative
// Assembly: TaleWorlds.MountAndBlade, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3C0E3F42-756F-4407-B817-06BA412A78B0
// Assembly location: E:\SteamLibrary\steamapps\common\Mount & Blade II Bannerlord\bin\Win64_Shipping_Client\TaleWorlds.MountAndBlade.dll

using NetworkMessages.FromClient;
using NetworkMessages.FromServer;
using System;
using System.Collections.Generic;
using System.Linq;
using TaleWorlds.Core;
using TaleWorlds.MountAndBlade.Network.Messages;

namespace TaleWorlds.MountAndBlade.MissionRepresentatives
{
  public class DuelMissionRepresentative : MissionRepresentativeBase
  {
    public Action OnDuelRequestedEvent;
    public Action<Agent, int> OnDuelPrepStartedEvent;
    public Action OnDuelStartedEvent;
    private List<Tuple<Agent, MissionTime>> _requesters;
    private MissionMultiplayerDuel _missionMultiplayerDuel;
    public const int DuelPrepTime = 3;

    public override void Initialize()
    {
      this._requesters = new List<Tuple<Agent, MissionTime>>();
      if (GameNetwork.IsServerOrRecorder)
        this._missionMultiplayerDuel = Mission.Current.GetMissionBehaviour<MissionMultiplayerDuel>();
      else if (this.IsMine)
        this.AddRemoveMessageHandlers(GameNetwork.NetworkMessageHandlerRegisterer.RegisterMode.Add);
      Mission.Current.SetMissionMode(MissionMode.Duel, true);
    }

    private void AddRemoveMessageHandlers(
      GameNetwork.NetworkMessageHandlerRegisterer.RegisterMode mode)
    {
      if (!GameNetwork.IsClient)
        return;
      GameNetwork.NetworkMessageHandlerRegisterer handlerRegisterer = new GameNetwork.NetworkMessageHandlerRegisterer(mode);
      handlerRegisterer.Register<NetworkMessages.FromServer.DuelRequest>(new GameNetworkMessage.ServerMessageHandlerDelegate<NetworkMessages.FromServer.DuelRequest>(this.HandleServerEventDuelRequest));
      handlerRegisterer.Register<DuelSessionStarted>(new GameNetworkMessage.ServerMessageHandlerDelegate<DuelSessionStarted>(this.HandleServerEventDuelSessionStarted));
      handlerRegisterer.Register<DuelStarted>(new GameNetworkMessage.ServerMessageHandlerDelegate<DuelStarted>(this.HandleServerEventDuelStarted));
    }

    public override bool IsThereAgentAction(Agent targetAgent) => !targetAgent.IsMount && !this.ControlledAgent.Team.IsDefender && (!targetAgent.Team.IsDefender && this.ControlledAgent.IsActive()) && targetAgent.IsActive();

    public override void OnAgentInteraction(Agent targetAgent)
    {
      if (this._requesters.Any<Tuple<Agent, MissionTime>>((Func<Tuple<Agent, MissionTime>, bool>) (req => req.Item1 == targetAgent)))
      {
        for (int index = 0; index < this._requesters.Count; ++index)
        {
          if (this._requesters[index].Item1 == this.ControlledAgent)
          {
            this._requesters.Remove(this._requesters[index]);
            break;
          }
        }
        switch (this.PlayerType)
        {
          case MissionRepresentativeBase.PlayerTypes.Client:
            GameNetwork.BeginModuleEventAsClient();
            GameNetwork.WriteMessage((GameNetworkMessage) new DuelResponse(targetAgent.MissionRepresentative.Peer.Communicator as NetworkCommunicator, true));
            GameNetwork.EndModuleEventAsClient();
            break;
          case MissionRepresentativeBase.PlayerTypes.Server:
            this._missionMultiplayerDuel.DuelRequestAccepted(targetAgent, this.ControlledAgent);
            break;
        }
      }
      else
      {
        switch (this.PlayerType)
        {
          case MissionRepresentativeBase.PlayerTypes.Client:
            GameNetwork.BeginModuleEventAsClient();
            GameNetwork.WriteMessage((GameNetworkMessage) new NetworkMessages.FromClient.DuelRequest(targetAgent));
            GameNetwork.EndModuleEventAsClient();
            break;
          case MissionRepresentativeBase.PlayerTypes.Server:
            this._missionMultiplayerDuel.DuelRequestReceived(this.ControlledAgent, targetAgent);
            break;
        }
      }
    }

    private void HandleServerEventDuelRequest(NetworkMessages.FromServer.DuelRequest message) => this.OnDuelRequested(message.RequesterAgent);

    private void HandleServerEventDuelSessionStarted(DuelSessionStarted message) => this.OnDuelPreparation(message.RequesterAgent, message.RequestedAgent);

    private void HandleServerEventDuelStarted(DuelStarted message)
    {
    }

    public void OnDuelRequested(Agent requesterAgent)
    {
      this._requesters.Add(new Tuple<Agent, MissionTime>(requesterAgent, MissionTime.Now + MissionTime.Seconds(3f)));
      switch (this.PlayerType)
      {
        case MissionRepresentativeBase.PlayerTypes.Bot:
          this._missionMultiplayerDuel.DuelRequestAccepted(requesterAgent, this.ControlledAgent);
          break;
        case MissionRepresentativeBase.PlayerTypes.Client:
          if (this.IsMine)
          {
            Action duelRequestedEvent = this.OnDuelRequestedEvent;
            if (duelRequestedEvent == null)
              break;
            duelRequestedEvent();
            break;
          }
          GameNetwork.BeginModuleEventAsServer(this.Peer);
          GameNetwork.WriteMessage((GameNetworkMessage) new NetworkMessages.FromServer.DuelRequest(requesterAgent, this.ControlledAgent));
          GameNetwork.EndModuleEventAsServer();
          break;
        case MissionRepresentativeBase.PlayerTypes.Server:
          Action duelRequestedEvent1 = this.OnDuelRequestedEvent;
          if (duelRequestedEvent1 == null)
            break;
          duelRequestedEvent1();
          break;
        default:
          throw new ArgumentOutOfRangeException();
      }
    }

    public bool HasRequestFrom(Agent requestOwner)
    {
      Tuple<Agent, MissionTime> tuple = this._requesters.FirstOrDefault<Tuple<Agent, MissionTime>>((Func<Tuple<Agent, MissionTime>, bool>) (req => req.Item1 == requestOwner));
      if (tuple == null)
        return false;
      if (!tuple.Item2.IsPast)
        return true;
      this._requesters.Remove(tuple);
      return false;
    }

    public override void Tick(float dt)
    {
      if (!this.IsMine || !GameNetwork.IsClient)
        return;
      int num = this._requesters.Count - 1;
      while (num >= 0)
        --num;
    }

    public void OnDuelPreparation(Agent requesterAgent, Agent requesteeAgent)
    {
      switch (this.PlayerType)
      {
        case MissionRepresentativeBase.PlayerTypes.Client:
          if (this.IsMine)
          {
            Action<Agent, int> prepStartedEvent = this.OnDuelPrepStartedEvent;
            if (prepStartedEvent != null)
            {
              prepStartedEvent(this.ControlledAgent == requesterAgent ? requesteeAgent : requesterAgent, 3);
              break;
            }
            break;
          }
          GameNetwork.BeginModuleEventAsServer(this.Peer);
          GameNetwork.WriteMessage((GameNetworkMessage) new DuelSessionStarted(requesterAgent, requesteeAgent));
          GameNetwork.EndModuleEventAsServer();
          break;
        case MissionRepresentativeBase.PlayerTypes.Server:
          Action<Agent, int> prepStartedEvent1 = this.OnDuelPrepStartedEvent;
          if (prepStartedEvent1 != null)
          {
            prepStartedEvent1(this.ControlledAgent == requesterAgent ? requesteeAgent : requesterAgent, 3);
            break;
          }
          break;
      }
      Tuple<Agent, MissionTime> tuple = this._requesters.FirstOrDefault<Tuple<Agent, MissionTime>>((Func<Tuple<Agent, MissionTime>, bool>) (req => req.Item1 == requesterAgent));
      if (tuple == null)
        return;
      this._requesters.Remove(tuple);
    }

    public void OnDuelStarted(Agent requesterAgent, Agent requesteeAgent)
    {
      switch (this.PlayerType)
      {
        case MissionRepresentativeBase.PlayerTypes.Client:
          if (this.IsMine)
          {
            Action duelStartedEvent = this.OnDuelStartedEvent;
            if (duelStartedEvent == null)
              break;
            duelStartedEvent();
            break;
          }
          GameNetwork.BeginModuleEventAsServer(this.Peer);
          GameNetwork.WriteMessage((GameNetworkMessage) new DuelStarted(requesterAgent, requesteeAgent));
          GameNetwork.EndModuleEventAsServer();
          break;
        case MissionRepresentativeBase.PlayerTypes.Server:
          Action duelStartedEvent1 = this.OnDuelStartedEvent;
          if (duelStartedEvent1 == null)
            break;
          duelStartedEvent1();
          break;
      }
    }
  }
}
