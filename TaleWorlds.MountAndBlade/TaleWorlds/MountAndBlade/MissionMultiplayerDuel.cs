// Decompiled with JetBrains decompiler
// Type: TaleWorlds.MountAndBlade.MissionMultiplayerDuel
// Assembly: TaleWorlds.MountAndBlade, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3C0E3F42-756F-4407-B817-06BA412A78B0
// Assembly location: E:\SteamLibrary\steamapps\common\Mount & Blade II Bannerlord\bin\Win64_Shipping_Client\TaleWorlds.MountAndBlade.dll

using NetworkMessages.FromClient;
using System;
using System.Collections.Generic;
using System.Linq;
using TaleWorlds.Core;
using TaleWorlds.MountAndBlade.MissionRepresentatives;
using TaleWorlds.MountAndBlade.Network.Messages;
using TaleWorlds.ObjectSystem;

namespace TaleWorlds.MountAndBlade
{
  public class MissionMultiplayerDuel : MissionMultiplayerGameModeBase
  {
    public const float DuelRequestTimeOutInSeconds = 3f;
    private const float DuelRequestTimeOutServerToleranceInSeconds = 0.5f;
    private MissionNetworkComponent _missionNetworkComponent;
    private List<MissionMultiplayerDuel.DuelInfo> _duelRequests = new List<MissionMultiplayerDuel.DuelInfo>();
    private List<MissionMultiplayerDuel.DuelInfo> _activeDuels = new List<MissionMultiplayerDuel.DuelInfo>();
    private readonly Queue<Team> _deactiveDuelTeams = new Queue<Team>();

    public override bool IsGameModeHidingAllAgentVisuals => true;

    public override MissionLobbyComponent.MultiplayerGameType GetMissionType() => MissionLobbyComponent.MultiplayerGameType.Duel;

    public override void AfterStart()
    {
      base.AfterStart();
      if (!GameNetwork.IsServer)
        return;
      BasicCultureObject basicCultureObject = MBObjectManager.Instance.GetObject<BasicCultureObject>(MultiplayerOptions.OptionType.CultureTeam1.GetStrValue());
      Banner banner = new Banner(basicCultureObject.BannerKey, basicCultureObject.BackgroundColor1, basicCultureObject.ForegroundColor1);
      this.Mission.Teams.Add(BattleSideEnum.Attacker, basicCultureObject.BackgroundColor1, basicCultureObject.ForegroundColor1, banner, false);
    }

    public override void OnBehaviourInitialize()
    {
      base.OnBehaviourInitialize();
      this._missionNetworkComponent = this.Mission.GetMissionBehaviour<MissionNetworkComponent>();
    }

    protected override void AddRemoveMessageHandlers(
      GameNetwork.NetworkMessageHandlerRegistererContainer registerer)
    {
      registerer.Register<DuelRequest>(new GameNetworkMessage.ClientMessageHandlerDelegate<DuelRequest>(this.HandleClientEventDuelRequest));
      registerer.Register<DuelResponse>(new GameNetworkMessage.ClientMessageHandlerDelegate<DuelResponse>(this.HandleClientEventDuelRequestAccepted));
    }

    protected override void HandleEarlyNewClientAfterLoadingFinished(NetworkCommunicator networkPeer) => networkPeer.AddComponent<DuelMissionRepresentative>();

    protected override void HandleNewClientAfterSynchronized(NetworkCommunicator networkPeer)
    {
      MissionPeer component = networkPeer.GetComponent<MissionPeer>();
      component.Team = this.Mission.AttackerTeam;
      if (networkPeer.IsServerPeer)
        return;
      this._missionNetworkComponent?.OnPeerSelectedTeam(component);
    }

    private bool HandleClientEventDuelRequest(NetworkCommunicator peer, DuelRequest message)
    {
      if (peer == null || peer.GetComponent<MissionPeer>() == null || (peer.GetComponent<MissionPeer>().ControlledAgent == null || message.RequestedAgent == null))
        return false;
      this.DuelRequestReceived(peer.GetComponent<DuelMissionRepresentative>().ControlledAgent, message.RequestedAgent);
      return true;
    }

    private bool HandleClientEventDuelRequestAccepted(
      NetworkCommunicator peer,
      DuelResponse message)
    {
      if (peer == null || peer.GetComponent<MissionPeer>() == null || (peer.GetComponent<MissionPeer>().ControlledAgent == null || message.Peer == null) || (message.Peer.GetComponent<MissionPeer>() == null || message.Peer.GetComponent<MissionPeer>().ControlledAgent == null))
        return false;
      this.DuelRequestAccepted(message.Peer.GetComponent<DuelMissionRepresentative>().ControlledAgent, peer.GetComponent<DuelMissionRepresentative>().ControlledAgent);
      return true;
    }

    public void DuelRequestReceived(Agent requesterAgent, Agent requesteeAgent)
    {
      if (this.IsThereARequestBetweenPlayers(requesterAgent, requesteeAgent) || this.IsHavingDuel(requesterAgent) || this.IsHavingDuel(requesteeAgent))
        return;
      this._duelRequests.Add(new MissionMultiplayerDuel.DuelInfo(requesterAgent, requesteeAgent));
      (requesteeAgent.MissionRepresentative as DuelMissionRepresentative).OnDuelRequested(requesterAgent);
    }

    public void DuelRequestAccepted(Agent requesterAgent, Agent requesteeAgent)
    {
      MissionMultiplayerDuel.DuelInfo duel = this._duelRequests.FirstOrDefault<MissionMultiplayerDuel.DuelInfo>((Func<MissionMultiplayerDuel.DuelInfo, bool>) (dr => dr.RequesterAgent == requesterAgent && dr.RequesteeAgent == requesteeAgent));
      if (duel == null)
        return;
      this.PrepareDuel(duel);
    }

    public override void OnMissionTick(float dt)
    {
      this.CheckDuelPreparations();
      this.CheckDuelRequestTimeouts();
    }

    public override void OnAgentRemoved(
      Agent affectedAgent,
      Agent affectorAgent,
      AgentState agentState,
      KillingBlow blow)
    {
      if (!affectedAgent.IsHuman)
        return;
      MissionMultiplayerDuel.DuelInfo duel = this._activeDuels.Where<MissionMultiplayerDuel.DuelInfo>((Func<MissionMultiplayerDuel.DuelInfo, bool>) (d => d.RequesteeAgent == affectedAgent || d.RequesterAgent == affectedAgent || d.RequesteeAgent == affectorAgent || d.RequesterAgent == affectorAgent)).FirstOrDefault<MissionMultiplayerDuel.DuelInfo>();
      if (duel == null)
        return;
      this.EndDuel(duel);
    }

    private Team ActivateAndGetDuelTeam() => !this._deactiveDuelTeams.Any<Team>() ? this.Mission.Teams.Add(BattleSideEnum.Defender, isSettingRelations: false) : this._deactiveDuelTeams.Dequeue();

    private void DeactivateDuelTeam(Team team) => this._deactiveDuelTeams.Enqueue(team);

    private bool IsHavingDuel(Agent agent) => this._activeDuels.Any<MissionMultiplayerDuel.DuelInfo>((Func<MissionMultiplayerDuel.DuelInfo, bool>) (d => d.RequesteeAgent == agent || d.RequesterAgent == agent));

    private bool IsThereARequestBetweenPlayers(Agent requesterAgent, Agent requesteeAgent)
    {
      foreach (MissionMultiplayerDuel.DuelInfo duelRequest in this._duelRequests)
      {
        if (duelRequest.RequesterAgent == requesterAgent && duelRequest.RequesteeAgent == requesteeAgent || duelRequest.RequesterAgent == requesteeAgent && duelRequest.RequesteeAgent == requesterAgent)
          return true;
      }
      return false;
    }

    private void CheckDuelPreparations()
    {
      for (int index = this._activeDuels.Count - 1; index >= 0; --index)
      {
        MissionMultiplayerDuel.DuelInfo activeDuel = this._activeDuels[index];
        if (!activeDuel.Started && activeDuel.Timer.IsPast)
          this.StartDuel(activeDuel);
      }
    }

    private void CheckDuelRequestTimeouts()
    {
      for (int index = this._duelRequests.Count - 1; index >= 0; --index)
      {
        MissionMultiplayerDuel.DuelInfo duelRequest = this._duelRequests[index];
        if (duelRequest.Timer.IsPast)
          this._duelRequests.Remove(duelRequest);
      }
    }

    private void PrepareDuel(MissionMultiplayerDuel.DuelInfo duel)
    {
      this._duelRequests.Remove(duel);
      if (this.IsHavingDuel(duel.RequesteeAgent) || this.IsHavingDuel(duel.RequesterAgent))
        return;
      this._activeDuels.Add(duel);
      Team duelTeam = this.ActivateAndGetDuelTeam();
      duel.OnDuelPreparation(duelTeam);
      (duel.RequesterAgent.MissionRepresentative as DuelMissionRepresentative).OnDuelPreparation(duel.RequesterAgent, duel.RequesteeAgent);
      (duel.RequesteeAgent.MissionRepresentative as DuelMissionRepresentative).OnDuelPreparation(duel.RequesterAgent, duel.RequesteeAgent);
    }

    private void StartDuel(MissionMultiplayerDuel.DuelInfo duel)
    {
      duel.OnDuelStarted();
      duel.RequesterAgent.Health = duel.RequesterAgent.HealthLimit;
      duel.RequesteeAgent.Health = duel.RequesteeAgent.HealthLimit;
      duel.RequesterAgent.RestoreShieldHitPoints();
      duel.RequesteeAgent.RestoreShieldHitPoints();
      (duel.RequesterAgent.MissionRepresentative as DuelMissionRepresentative).OnDuelStarted(duel.RequesterAgent, duel.RequesteeAgent);
      (duel.RequesteeAgent.MissionRepresentative as DuelMissionRepresentative).OnDuelStarted(duel.RequesterAgent, duel.RequesteeAgent);
    }

    private void EndDuel(MissionMultiplayerDuel.DuelInfo duel)
    {
      this._activeDuels.Remove(duel);
      if (!duel.Started)
        return;
      duel.OnDuelEnded();
      this.DeactivateDuelTeam(duel.DuelTeam);
    }

    private class DuelInfo
    {
      private const float DuelStartCountdown = 3f;
      public readonly Agent RequesterAgent;
      public readonly Agent RequesteeAgent;

      public MissionTime Timer { get; private set; }

      public bool Started { get; private set; }

      public Team DuelTeam { get; private set; }

      public DuelInfo(Agent requesterAgent, Agent requesteeAgent)
      {
        this.RequesterAgent = requesterAgent;
        this.RequesteeAgent = requesteeAgent;
        this.Timer = MissionTime.Now + MissionTime.Seconds(3.5f);
      }

      public void OnDuelPreparation(Team duelTeam)
      {
        this.DuelTeam = duelTeam;
        this.RequesterAgent.SetTeam(this.DuelTeam, true);
        this.RequesteeAgent.SetTeam(this.DuelTeam, true);
        this.Timer = MissionTime.Now + MissionTime.Seconds(3f);
      }

      public void OnDuelStarted()
      {
        this.Started = true;
        this.DuelTeam.SetIsEnemyOf(this.DuelTeam, true);
      }

      public void OnDuelEnded()
      {
        this.DuelTeam.SetIsEnemyOf(this.DuelTeam, false);
        this.RequesterAgent.SetTeam(Mission.Current.AttackerTeam, true);
        this.RequesteeAgent.SetTeam(Mission.Current.AttackerTeam, true);
      }
    }
  }
}
