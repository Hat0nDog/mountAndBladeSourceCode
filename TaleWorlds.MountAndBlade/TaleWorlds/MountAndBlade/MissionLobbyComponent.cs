// Decompiled with JetBrains decompiler
// Type: TaleWorlds.MountAndBlade.MissionLobbyComponent
// Assembly: TaleWorlds.MountAndBlade, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3C0E3F42-756F-4407-B817-06BA412A78B0
// Assembly location: E:\SteamLibrary\steamapps\common\Mount & Blade II Bannerlord\bin\Win64_Shipping_Client\TaleWorlds.MountAndBlade.dll

using NetworkMessages.FromServer;
using System;
using System.Collections.Generic;
using System.Linq;
using TaleWorlds.Core;
using TaleWorlds.Engine;
using TaleWorlds.MountAndBlade.Network.Messages;

namespace TaleWorlds.MountAndBlade
{
  public abstract class MissionLobbyComponent : MissionNetwork
  {
    private static readonly float InactivityThreshold = 2f;
    private MissionScoreboardComponent _missionScoreboardComponent;
    private MissionMultiplayerGameModeBase _gameMode;
    private MultiplayerTimerComponent _timerComponent;
    private IRoundComponent _roundComponent;
    private Timer _inactivityTimer;
    private MultiplayerWarmupComponent _warmupComponent;
    private static readonly Dictionary<Tuple<LobbyMissionType, bool>, System.Type> _lobbyComponentTypes = new Dictionary<Tuple<LobbyMissionType, bool>, System.Type>();
    private bool _usingFixedBanners;
    private MissionLobbyComponent.MultiplayerGameState _currentMultiplayerState;

    public event Action OnPostMatchEnded;

    public bool IsInWarmup => this._warmupComponent != null && this._warmupComponent.IsInWarmup;

    static MissionLobbyComponent()
    {
      MissionLobbyComponent.AddLobbyComponentType(typeof (MissionBattleSchedulerClientComponent), LobbyMissionType.Matchmaker, false);
      MissionLobbyComponent.AddLobbyComponentType(typeof (MissionCustomGameClientComponent), LobbyMissionType.Custom, false);
    }

    public static void AddLobbyComponentType(
      System.Type type,
      LobbyMissionType missionType,
      bool isSeverComponent)
    {
      MissionLobbyComponent._lobbyComponentTypes.Add(new Tuple<LobbyMissionType, bool>(missionType, isSeverComponent), type);
    }

    public override void OnBehaviourInitialize()
    {
      base.OnBehaviourInitialize();
      this.CurrentMultiplayerState = MissionLobbyComponent.MultiplayerGameState.WaitingFirstPlayers;
      if (GameNetwork.IsServerOrRecorder)
      {
        MissionMultiplayerGameModeBase missionBehaviour = Mission.Current.GetMissionBehaviour<MissionMultiplayerGameModeBase>();
        if (missionBehaviour == null || missionBehaviour.AllowCustomPlayerBanners())
          return;
        this._usingFixedBanners = true;
        MissionPeer.OnPreTeamChanged += new MissionPeer.OnTeamChangedDelegate(MissionLobbyComponent.CreateFixedBannerForPeer);
      }
      else
        this._inactivityTimer = new Timer(MBCommon.GetTime(MBCommon.TimeType.Mission), MissionLobbyComponent.InactivityThreshold);
    }

    protected override void AddRemoveMessageHandlers(
      GameNetwork.NetworkMessageHandlerRegistererContainer registerer)
    {
      if (GameNetwork.IsClient)
      {
        registerer.Register<KillDeathCountChange>(new GameNetworkMessage.ServerMessageHandlerDelegate<KillDeathCountChange>(this.HandleServerEventKillDeathCountChangeEvent));
        registerer.Register<MissionStateChange>(new GameNetworkMessage.ServerMessageHandlerDelegate<MissionStateChange>(this.HandleServerEventMissionStateChange));
        registerer.Register<NetworkMessages.FromServer.CreateBanner>(new GameNetworkMessage.ServerMessageHandlerDelegate<NetworkMessages.FromServer.CreateBanner>(this.HandleServerEventCreateBannerForPeer));
      }
      else
      {
        if (!GameNetwork.IsServer)
          return;
        registerer.Register<NetworkMessages.FromClient.CreateBanner>(new GameNetworkMessage.ClientMessageHandlerDelegate<NetworkMessages.FromClient.CreateBanner>(this.HandleClientEventCreateBannerForPeer));
      }
    }

    protected override void OnUdpNetworkHandlerClose()
    {
      if (!GameNetwork.IsServerOrRecorder && !this._usingFixedBanners)
        return;
      MissionPeer.OnPreTeamChanged -= new MissionPeer.OnTeamChangedDelegate(MissionLobbyComponent.CreateFixedBannerForPeer);
      this._usingFixedBanners = false;
    }

    private static void CreateFixedBannerForPeer(
      NetworkCommunicator peer,
      Team currTeam,
      Team nextTeam)
    {
      if (!GameNetwork.IsServerOrRecorder)
        return;
      Mission.Current.GetMissionBehaviour<MissionMultiplayerGameModeBase>();
      MissionPeer component = peer.GetComponent<MissionPeer>();
      if (nextTeam.Side != BattleSideEnum.Attacker && nextTeam.Side != BattleSideEnum.Defender)
        return;
      List<string> stringList = new List<string>();
      stringList.Add("11.8.1.4345.4345.770.774.1.0.0.133.7.5.512.512.784.769.1.0.0");
      stringList.Add("11.8.1.4345.4345.770.774.1.0.0.156.7.5.512.512.784.769.1.0.0");
      stringList.Add("11.8.1.4345.4345.770.774.1.0.0.155.7.5.512.512.784.769.1.0.0");
      stringList.Add("11.8.1.4345.4345.770.774.1.0.0.158.7.5.512.512.784.769.1.0.0");
      stringList.Add("11.8.1.4345.4345.770.774.1.0.0.118.7.5.512.512.784.769.1.0.0");
      stringList.Add("11.8.1.4345.4345.770.774.1.0.0.149.7.5.512.512.784.769.1.0.0");
      List<string> list = GameNetwork.NetworkPeers.Where<NetworkCommunicator>((Func<NetworkCommunicator, bool>) (x => x.GetComponent<MissionPeer>()?.Team == nextTeam)).Select<NetworkCommunicator, string>((Func<NetworkCommunicator, string>) (x => x.GetComponent<MissionPeer>().Peer.BannerCode)).ToList<string>();
      foreach (string str in stringList)
      {
        if (!list.Contains(str))
        {
          component.Peer.BannerCode = str;
          break;
        }
      }
      GameNetwork.BeginBroadcastModuleEvent();
      GameNetwork.WriteMessage((GameNetworkMessage) new NetworkMessages.FromServer.CreateBanner(peer, component.Peer.BannerCode));
      GameNetwork.EndBroadcastModuleEvent(GameNetwork.EventBroadcastFlags.None);
    }

    public static MissionLobbyComponent CreateBehaviour() => (MissionLobbyComponent) Activator.CreateInstance(MissionLobbyComponent._lobbyComponentTypes[new Tuple<LobbyMissionType, bool>(BannerlordNetwork.LobbyMissionType, GameNetwork.IsDedicatedServer)]);

    public virtual void QuitMission()
    {
    }

    public override void AfterStart()
    {
      this.Mission.MakeDeploymentPlan();
      this._missionScoreboardComponent = this.Mission.GetMissionBehaviour<MissionScoreboardComponent>();
      this._gameMode = this.Mission.GetMissionBehaviour<MissionMultiplayerGameModeBase>();
      this._timerComponent = this.Mission.GetMissionBehaviour<MultiplayerTimerComponent>();
      this._roundComponent = this.Mission.GetMissionBehaviour<IRoundComponent>();
      this._warmupComponent = this.Mission.GetMissionBehaviour<MultiplayerWarmupComponent>();
    }

    public override void EarlyStart()
    {
      if (!GameNetwork.IsServer)
        return;
      this.Mission.SpectatorTeam = this.Mission.Teams.Add(BattleSideEnum.None, uint.MaxValue, uint.MaxValue, (Banner) null, true, false, true);
    }

    public override void OnMissionTick(float dt)
    {
      if (GameNetwork.IsClient && this._inactivityTimer.Check(MBCommon.GetTime(MBCommon.TimeType.Mission)))
        NetworkMain.GameClient.IsInCriticalState = MBAPI.IMBNetwork.ElapsedTimeSinceLastUdpPacketArrived() > (double) MissionLobbyComponent.InactivityThreshold;
      if (this.CurrentMultiplayerState == MissionLobbyComponent.MultiplayerGameState.WaitingFirstPlayers)
      {
        if (!GameNetwork.IsServer || this._warmupComponent != null && (this._warmupComponent.IsInWarmup || !this._timerComponent.CheckIfTimerPassed()))
          return;
        int num1 = GameNetwork.NetworkPeers.Count<NetworkCommunicator>((Func<NetworkCommunicator, bool>) (x => x.IsSynchronized));
        int num2 = MultiplayerOptions.OptionType.NumberOfBotsTeam1.GetIntValue() + MultiplayerOptions.OptionType.NumberOfBotsTeam2.GetIntValue();
        int intValue = MultiplayerOptions.OptionType.MinNumberOfPlayersForMatchStart.GetIntValue();
        int num3 = num2;
        if (num1 + num3 < intValue && MBCommon.CurrentGameType != MBCommon.GameType.MultiClientServer)
          return;
        this.SetStatePlayingAsServer();
      }
      else
      {
        if (this.CurrentMultiplayerState != MissionLobbyComponent.MultiplayerGameState.Playing)
          return;
        bool flag = this._timerComponent.CheckIfTimerPassed();
        if (!GameNetwork.IsServerOrRecorder || this._gameMode.RoundController != null || !flag && !this._gameMode.CheckForMatchEnd())
          return;
        this._gameMode.GetWinnerTeam();
        this._gameMode.SpawnComponent.SpawningBehaviour.RequestStopSpawnSession();
        this.SetStateEndingAsServer();
      }
    }

    protected override void OnUdpNetworkHandlerTick()
    {
      if (this.CurrentMultiplayerState != MissionLobbyComponent.MultiplayerGameState.Ending || !this._timerComponent.CheckIfTimerPassed() || !GameNetwork.IsServer)
        return;
      this.EndGameAsServer();
    }

    protected override void HandlePlayerDisconnect(NetworkCommunicator networkPeer) => networkPeer.RemoveComponent<MissionPeer>();

    public override void OnRemoveBehaviour()
    {
      NetworkCommunicator myPeer = GameNetwork.MyPeer;
      this.QuitMission();
      base.OnRemoveBehaviour();
    }

    private void HandleServerEventMissionStateChange(MissionStateChange message)
    {
      this.CurrentMultiplayerState = message.CurrentState;
      if (this.CurrentMultiplayerState != MissionLobbyComponent.MultiplayerGameState.WaitingFirstPlayers)
      {
        if (this.CurrentMultiplayerState == MissionLobbyComponent.MultiplayerGameState.Playing && this._warmupComponent != null)
        {
          this.Mission.RemoveMissionBehaviour((MissionBehaviour) this._warmupComponent);
          this._warmupComponent = (MultiplayerWarmupComponent) null;
        }
        float duration = this.CurrentMultiplayerState == MissionLobbyComponent.MultiplayerGameState.Playing ? (float) (MultiplayerOptions.OptionType.MapTimeLimit.GetIntValue() * 60) : 5f;
        this._timerComponent.StartTimerAsClient(message.StateStartTimeInSeconds, duration);
      }
      if (this.CurrentMultiplayerState != MissionLobbyComponent.MultiplayerGameState.Ending)
        return;
      this.SetStateEndingAsClient();
    }

    private void HandleServerEventKillDeathCountChangeEvent(KillDeathCountChange message)
    {
      if (message.VictimPeer == null)
        return;
      MissionPeer component = message.VictimPeer.GetComponent<MissionPeer>();
      NetworkCommunicator attackerPeer = message.AttackerPeer;
      MissionPeer killedPeer = attackerPeer != null ? attackerPeer.GetComponent<MissionPeer>() : (MissionPeer) null;
      if (component != null)
      {
        component.KillCount = message.KillCount;
        component.AssistCount = message.AssistCount;
        component.DeathCount = message.DeathCount;
        component.Score = message.Score;
        component.OnKillAnotherPeer(killedPeer);
        if (message.KillCount == 0 && message.AssistCount == 0 && (message.DeathCount == 0 && message.Score == 0))
          component.ResetKillRegistry();
      }
      if (this._missionScoreboardComponent == null)
        return;
      this._missionScoreboardComponent.PlayerPropertiesChanged(message.VictimPeer);
    }

    private void HandleServerEventCreateBannerForPeer(NetworkMessages.FromServer.CreateBanner message)
    {
      MissionPeer component = message.Peer.GetComponent<MissionPeer>();
      if (component == null)
        return;
      component.Peer.BannerCode = message.BannerCode;
    }

    private bool HandleClientEventCreateBannerForPeer(
      NetworkCommunicator peer,
      NetworkMessages.FromClient.CreateBanner message)
    {
      MissionMultiplayerGameModeBase missionBehaviour = Mission.Current.GetMissionBehaviour<MissionMultiplayerGameModeBase>();
      if (missionBehaviour == null || !missionBehaviour.AllowCustomPlayerBanners())
        return false;
      MissionPeer component = peer.GetComponent<MissionPeer>();
      if (component == null)
        return false;
      component.Peer.BannerCode = message.BannerCode;
      MissionLobbyComponent.SyncBannersToAllClients(message.BannerCode, component.GetNetworkPeer());
      return true;
    }

    private static void SyncBannersToAllClients(string bannerCode, NetworkCommunicator ownerPeer)
    {
      GameNetwork.BeginBroadcastModuleEvent();
      GameNetwork.WriteMessage((GameNetworkMessage) new NetworkMessages.FromServer.CreateBanner(ownerPeer, bannerCode));
      GameNetwork.EndBroadcastModuleEvent(GameNetwork.EventBroadcastFlags.ExcludeTargetPlayer, ownerPeer);
    }

    protected override void HandleNewClientConnect(PlayerConnectionInfo clientConnectionInfo) => base.HandleNewClientConnect(clientConnectionInfo);

    protected override void HandleLateNewClientAfterLoadingFinished(NetworkCommunicator networkPeer)
    {
      if (networkPeer.IsServerPeer)
        return;
      this.SendExistingObjectsToPeer(networkPeer);
    }

    private void SendExistingObjectsToPeer(NetworkCommunicator peer)
    {
      long stateStartTimeInTicks = 0;
      if (this.CurrentMultiplayerState != MissionLobbyComponent.MultiplayerGameState.WaitingFirstPlayers)
        stateStartTimeInTicks = this._timerComponent.GetCurrentTimerStartTime().NumberOfTicks;
      GameNetwork.BeginModuleEventAsServer(peer);
      GameNetwork.WriteMessage((GameNetworkMessage) new MissionStateChange(this.CurrentMultiplayerState, stateStartTimeInTicks));
      GameNetwork.EndModuleEventAsServer();
      this.SendPeerInformationsToPeer(peer);
    }

    private void SendPeerInformationsToPeer(NetworkCommunicator peer)
    {
      foreach (NetworkCommunicator networkPeer in GameNetwork.NetworkPeers)
      {
        if (networkPeer.IsSynchronized)
        {
          MissionPeer component = networkPeer.GetComponent<MissionPeer>();
          GameNetwork.BeginModuleEventAsServer(peer);
          GameNetwork.WriteMessage((GameNetworkMessage) new KillDeathCountChange(component.GetNetworkPeer(), (NetworkCommunicator) null, component.KillCount, component.AssistCount, component.DeathCount, component.Score));
          GameNetwork.EndModuleEventAsServer();
          if (component.BotsUnderControlAlive != 0 || component.BotsUnderControlTotal != 0)
          {
            GameNetwork.BeginModuleEventAsServer(peer);
            GameNetwork.WriteMessage((GameNetworkMessage) new BotsControlledChange(component.GetNetworkPeer(), component.BotsUnderControlAlive, component.BotsUnderControlTotal));
            GameNetwork.EndModuleEventAsServer();
          }
        }
      }
    }

    public override void OnScoreHit(
      Agent affectedAgent,
      Agent affectorAgent,
      WeaponComponentData attackerWeapon,
      bool isBlocked,
      float damage,
      float movementSpeedDamageModifier,
      float hitDistance,
      AgentAttackType attackType,
      float shotDifficulty,
      BoneBodyPartType victimHitBodyPart)
    {
      if (!GameNetwork.IsServer || isBlocked || (affectorAgent == affectedAgent || affectorAgent.MissionPeer == null) || (double) damage <= 0.0)
        return;
      affectedAgent.AddHitter(affectorAgent.MissionPeer, damage, affectorAgent.IsFriendOf(affectedAgent));
    }

    public override void OnAgentRemoved(
      Agent affectedAgent,
      Agent affectorAgent,
      AgentState agentState,
      KillingBlow killingBlow)
    {
      base.OnAgentRemoved(affectedAgent, affectorAgent, agentState, killingBlow);
      if (!GameNetwork.IsServer || this.CurrentMultiplayerState == MissionLobbyComponent.MultiplayerGameState.Ending || agentState != AgentState.Killed && agentState != AgentState.Unconscious && agentState != AgentState.Routed || (affectedAgent == null || !affectedAgent.IsHuman))
        return;
      MissionPeer assistorPeer = affectorAgent?.MissionPeer != null ? this.RemoveHittersAndGetAssistorPeer(affectorAgent.MissionPeer, affectedAgent) : (MissionPeer) null;
      if (affectedAgent.MissionPeer != null)
        this.OnPlayerDies(affectedAgent.MissionPeer, assistorPeer);
      else
        this.OnBotDies(affectedAgent, assistorPeer);
      if (affectorAgent == null || !affectorAgent.IsHuman)
        return;
      if (affectorAgent != affectedAgent)
      {
        if (affectorAgent.MissionPeer != null)
          this.OnPlayerKills(affectorAgent.MissionPeer, affectedAgent, assistorPeer);
        else
          this.OnBotKills(affectorAgent, affectedAgent);
      }
      else
      {
        if (affectorAgent.MissionPeer == null)
          return;
        affectorAgent.MissionPeer.Score -= this._gameMode.GetScoreForKill(affectedAgent);
        this._missionScoreboardComponent.PlayerPropertiesChanged(affectorAgent.MissionPeer.GetNetworkPeer());
        GameNetwork.BeginBroadcastModuleEvent();
        GameNetwork.WriteMessage((GameNetworkMessage) new KillDeathCountChange(affectorAgent.MissionPeer.GetNetworkPeer(), affectedAgent.MissionPeer.GetNetworkPeer(), affectorAgent.MissionPeer.KillCount, affectorAgent.MissionPeer.AssistCount, affectorAgent.MissionPeer.DeathCount, affectorAgent.MissionPeer.Score));
        GameNetwork.EndBroadcastModuleEvent(GameNetwork.EventBroadcastFlags.None);
      }
    }

    public override void OnAgentBuild(Agent agent, Banner banner)
    {
      if (!GameNetwork.IsServer || agent.IsMount || agent.MissionPeer != null)
        return;
      if (agent.Formation?.PlayerOwner != null)
      {
        MissionPeer missionPeer = agent.Formation.PlayerOwner.MissionPeer;
        if (missionPeer == null)
          return;
        ++missionPeer.BotsUnderControlAlive;
        ++missionPeer.BotsUnderControlTotal;
      }
      else
        ++this._missionScoreboardComponent.Sides[(int) agent.Team.Side].BotScores.AliveCount;
    }

    protected virtual void OnPlayerKills(
      MissionPeer killerPeer,
      Agent killedAgent,
      MissionPeer assistorPeer)
    {
      NetworkCommunicator killedPeer = (NetworkCommunicator) null;
      if (killedAgent.MissionPeer == null)
      {
        NetworkCommunicator networkPeer = GameNetwork.NetworkPeers.SingleOrDefault<NetworkCommunicator>((Func<NetworkCommunicator, bool>) (x => x.GetComponent<MissionPeer>() != null && x.GetComponent<MissionPeer>().ControlledFormation != null && x.GetComponent<MissionPeer>().ControlledFormation == killedAgent.Formation));
        if (networkPeer != null)
        {
          MissionPeer component = networkPeer.GetComponent<MissionPeer>();
          killedPeer = networkPeer;
          killerPeer.OnKillAnotherPeer(component);
        }
      }
      else
      {
        killerPeer.OnKillAnotherPeer(killedAgent.MissionPeer);
        killedPeer = killedAgent.MissionPeer.GetNetworkPeer();
      }
      if (killerPeer.ControlledAgent.Team.IsEnemyOf(killedAgent.Team))
      {
        killerPeer.Score += this._gameMode.GetScoreForKill(killedAgent);
        ++killerPeer.KillCount;
      }
      else
      {
        killerPeer.Score -= this._gameMode.GetScoreForKill(killedAgent);
        --killerPeer.KillCount;
      }
      this._missionScoreboardComponent.PlayerPropertiesChanged(killerPeer.GetNetworkPeer());
      GameNetwork.BeginBroadcastModuleEvent();
      GameNetwork.WriteMessage((GameNetworkMessage) new KillDeathCountChange(killerPeer.GetNetworkPeer(), killedPeer, killerPeer.KillCount, killerPeer.AssistCount, killerPeer.DeathCount, killerPeer.Score));
      GameNetwork.EndBroadcastModuleEvent(GameNetwork.EventBroadcastFlags.None);
    }

    protected virtual void OnPlayerDies(MissionPeer peer, MissionPeer assistorPeer)
    {
      if (assistorPeer != null && assistorPeer.GetNetworkPeer().IsConnectionActive)
      {
        GameNetwork.BeginBroadcastModuleEvent();
        GameNetwork.WriteMessage((GameNetworkMessage) new KillDeathCountChange(assistorPeer.GetNetworkPeer(), (NetworkCommunicator) null, assistorPeer.KillCount, assistorPeer.AssistCount, assistorPeer.DeathCount, assistorPeer.Score));
        GameNetwork.EndBroadcastModuleEvent(GameNetwork.EventBroadcastFlags.None);
      }
      ++peer.DeathCount;
      peer.SpawnTimer.Reset(MBCommon.GetTime(MBCommon.TimeType.Mission), (float) MissionLobbyComponent.GetSpawnPeriodDurationForPeer(peer));
      peer.WantsToSpawnAsBot = false;
      peer.HasSpawnTimerExpired = false;
      this._missionScoreboardComponent.PlayerPropertiesChanged(peer.GetNetworkPeer());
      GameNetwork.BeginBroadcastModuleEvent();
      GameNetwork.WriteMessage((GameNetworkMessage) new KillDeathCountChange(peer.GetNetworkPeer(), (NetworkCommunicator) null, peer.KillCount, peer.AssistCount, peer.DeathCount, peer.Score));
      GameNetwork.EndBroadcastModuleEvent(GameNetwork.EventBroadcastFlags.None);
    }

    protected virtual void OnBotKills(Agent botAgent, Agent killedAgent)
    {
      if (botAgent?.Team == null)
        return;
      if (botAgent.Formation?.PlayerOwner != null)
      {
        NetworkCommunicator networkCommunicator1 = GameNetwork.NetworkPeers.SingleOrDefault<NetworkCommunicator>((Func<NetworkCommunicator, bool>) (x => x.GetComponent<MissionPeer>() != null && x.GetComponent<MissionPeer>().ControlledFormation == botAgent.Formation));
        if (networkCommunicator1 != null)
        {
          MissionPeer component = networkCommunicator1.GetComponent<MissionPeer>();
          MissionPeer missionPeer = killedAgent.MissionPeer;
          NetworkCommunicator networkCommunicator2 = missionPeer != null ? missionPeer.GetNetworkPeer() : (NetworkCommunicator) null;
          if (killedAgent.MissionPeer == null)
          {
            NetworkCommunicator networkCommunicator3 = GameNetwork.NetworkPeers.SingleOrDefault<NetworkCommunicator>((Func<NetworkCommunicator, bool>) (x => x.GetComponent<MissionPeer>() != null && x.GetComponent<MissionPeer>().ControlledFormation == killedAgent.Formation));
            if (networkCommunicator3 != null)
            {
              networkCommunicator2 = networkCommunicator3;
              component.OnKillAnotherPeer(networkCommunicator2.GetComponent<MissionPeer>());
            }
          }
          else
            component.OnKillAnotherPeer(killedAgent.MissionPeer);
          if (botAgent.Team.IsEnemyOf(killedAgent.Team))
          {
            ++component.KillCount;
            component.Score += this._gameMode.GetScoreForKill(killedAgent);
          }
          else
          {
            --component.KillCount;
            component.Score -= this._gameMode.GetScoreForKill(killedAgent);
          }
          this._missionScoreboardComponent.PlayerPropertiesChanged(networkCommunicator1);
          GameNetwork.BeginBroadcastModuleEvent();
          GameNetwork.WriteMessage((GameNetworkMessage) new KillDeathCountChange(networkCommunicator1, networkCommunicator2, component.KillCount, component.AssistCount, component.DeathCount, component.Score));
          GameNetwork.EndBroadcastModuleEvent(GameNetwork.EventBroadcastFlags.None);
        }
      }
      else
      {
        MissionScoreboardComponent.MissionScoreboardSide sideSafe = this._missionScoreboardComponent.GetSideSafe(botAgent.Team.Side);
        BotData botScores = sideSafe.BotScores;
        if (botAgent.Team.IsEnemyOf(killedAgent.Team))
          ++botScores.KillCount;
        else
          --botScores.KillCount;
        this._missionScoreboardComponent.BotPropertiesChanged(sideSafe.Side);
        GameNetwork.BeginBroadcastModuleEvent();
        GameNetwork.WriteMessage((GameNetworkMessage) new NetworkMessages.FromServer.BotData(sideSafe.Side, botScores.KillCount, botScores.AssistCount, botScores.DeathCount, botScores.AliveCount));
        GameNetwork.EndBroadcastModuleEvent(GameNetwork.EventBroadcastFlags.None);
      }
      this._missionScoreboardComponent.BotPropertiesChanged(botAgent.Team.Side);
    }

    protected virtual void OnBotDies(Agent botAgent, MissionPeer assistorPeer)
    {
      if (assistorPeer != null)
      {
        GameNetwork.BeginBroadcastModuleEvent();
        GameNetwork.WriteMessage((GameNetworkMessage) new KillDeathCountChange(assistorPeer.GetNetworkPeer(), (NetworkCommunicator) null, assistorPeer.KillCount, assistorPeer.AssistCount, assistorPeer.DeathCount, assistorPeer.Score));
        GameNetwork.EndBroadcastModuleEvent(GameNetwork.EventBroadcastFlags.None);
      }
      if (botAgent == null)
        return;
      if (botAgent.Formation?.PlayerOwner != null)
      {
        NetworkCommunicator networkCommunicator = GameNetwork.NetworkPeers.SingleOrDefault<NetworkCommunicator>((Func<NetworkCommunicator, bool>) (x => x.GetComponent<MissionPeer>() != null && x.GetComponent<MissionPeer>().ControlledFormation == botAgent.Formation));
        if (networkCommunicator != null)
        {
          MissionPeer component = networkCommunicator.GetComponent<MissionPeer>();
          ++component.DeathCount;
          --component.BotsUnderControlAlive;
          this._missionScoreboardComponent.PlayerPropertiesChanged(networkCommunicator);
          GameNetwork.BeginBroadcastModuleEvent();
          GameNetwork.WriteMessage((GameNetworkMessage) new KillDeathCountChange(networkCommunicator, (NetworkCommunicator) null, component.KillCount, component.AssistCount, component.DeathCount, component.Score));
          GameNetwork.EndBroadcastModuleEvent(GameNetwork.EventBroadcastFlags.None);
          GameNetwork.BeginBroadcastModuleEvent();
          GameNetwork.WriteMessage((GameNetworkMessage) new BotsControlledChange(networkCommunicator, component.BotsUnderControlAlive, component.BotsUnderControlTotal));
          GameNetwork.EndBroadcastModuleEvent(GameNetwork.EventBroadcastFlags.None);
        }
      }
      else
      {
        MissionScoreboardComponent.MissionScoreboardSide sideSafe = this._missionScoreboardComponent.GetSideSafe(botAgent.Team.Side);
        BotData botScores = sideSafe.BotScores;
        ++botScores.DeathCount;
        --botScores.AliveCount;
        GameNetwork.BeginBroadcastModuleEvent();
        GameNetwork.WriteMessage((GameNetworkMessage) new NetworkMessages.FromServer.BotData(sideSafe.Side, botScores.KillCount, botScores.AssistCount, botScores.DeathCount, botScores.AliveCount));
        GameNetwork.EndBroadcastModuleEvent(GameNetwork.EventBroadcastFlags.None);
      }
      this._missionScoreboardComponent.BotPropertiesChanged(botAgent.Team.Side);
    }

    public override void OnClearScene()
    {
      foreach (NetworkCommunicator networkPeer in GameNetwork.NetworkPeers)
      {
        MissionPeer component = networkPeer.GetComponent<MissionPeer>();
        if (component != null)
        {
          component.BotsUnderControlAlive = 0;
          component.BotsUnderControlTotal = 0;
          component.ControlledFormation = (Formation) null;
        }
      }
    }

    public static int GetSpawnPeriodDurationForPeer(MissionPeer peer) => Mission.Current.GetMissionBehaviour<SpawnComponent>().GetMaximumReSpawnPeriodForPeer(peer);

    public virtual void SetStateEndingAsServer()
    {
      this.CurrentMultiplayerState = MissionLobbyComponent.MultiplayerGameState.Ending;
      MBDebug.Print("Multiplayer game mission ending");
      this._timerComponent.StartTimerAsServer(5f);
      GameNetwork.BeginBroadcastModuleEvent();
      GameNetwork.WriteMessage((GameNetworkMessage) new MissionStateChange(this.CurrentMultiplayerState, this._timerComponent.GetCurrentTimerStartTime().NumberOfTicks));
      GameNetwork.EndBroadcastModuleEvent(GameNetwork.EventBroadcastFlags.None);
      Action onPostMatchEnded = this.OnPostMatchEnded;
      if (onPostMatchEnded == null)
        return;
      onPostMatchEnded();
    }

    private void SetStatePlayingAsServer()
    {
      this._warmupComponent = (MultiplayerWarmupComponent) null;
      this.CurrentMultiplayerState = MissionLobbyComponent.MultiplayerGameState.Playing;
      this._timerComponent.StartTimerAsServer((float) (MultiplayerOptions.OptionType.MapTimeLimit.GetIntValue() * 60));
      GameNetwork.BeginBroadcastModuleEvent();
      GameNetwork.WriteMessage((GameNetworkMessage) new MissionStateChange(this.CurrentMultiplayerState, this._timerComponent.GetCurrentTimerStartTime().NumberOfTicks));
      GameNetwork.EndBroadcastModuleEvent(GameNetwork.EventBroadcastFlags.None);
    }

    protected virtual void EndGameAsServer()
    {
    }

    private MissionPeer RemoveHittersAndGetAssistorPeer(
      MissionPeer killerPeer,
      Agent killedAgent)
    {
      Agent.Hitter assistingHitter = killedAgent.GetAssistingHitter(killerPeer);
      foreach (Agent.Hitter hitter in killedAgent.HitterList.ToList<Agent.Hitter>())
      {
        if (hitter.HitterPeer != killerPeer && hitter != assistingHitter)
          killedAgent.RemoveHitter(hitter.HitterPeer, hitter.IsFriendlyHit);
      }
      if (assistingHitter?.HitterPeer != null)
      {
        int scoreForAssist = this._gameMode.GetScoreForAssist(killedAgent);
        if (!assistingHitter.IsFriendlyHit)
        {
          ++assistingHitter.HitterPeer.AssistCount;
          assistingHitter.HitterPeer.Score += scoreForAssist;
        }
        else
        {
          --assistingHitter.HitterPeer.AssistCount;
          assistingHitter.HitterPeer.Score -= scoreForAssist;
        }
      }
      return assistingHitter?.HitterPeer;
    }

    private void SetStateEndingAsClient()
    {
      Action onPostMatchEnded = this.OnPostMatchEnded;
      if (onPostMatchEnded == null)
        return;
      onPostMatchEnded();
    }

    public MissionLobbyComponent.MultiplayerGameType MissionType { get; set; }

    public MissionLobbyComponent.MultiplayerGameState CurrentMultiplayerState
    {
      get => this._currentMultiplayerState;
      private set
      {
        if (this._currentMultiplayerState == value)
          return;
        this._currentMultiplayerState = value;
        Action<MissionLobbyComponent.MultiplayerGameState> multiplayerStateChanged = this.CurrentMultiplayerStateChanged;
        if (multiplayerStateChanged == null)
          return;
        multiplayerStateChanged(value);
      }
    }

    public event Action<MissionLobbyComponent.MultiplayerGameState> CurrentMultiplayerStateChanged;

    public int GetRandomFaceSeedForCharacter(BasicCharacterObject character, int addition = 0)
    {
      BasicCharacterObject basicCharacterObject = character;
      int num1 = addition;
      IRoundComponent roundComponent = this._roundComponent;
      int num2 = roundComponent != null ? roundComponent.RoundCount : 0;
      int rank = num1 + num2;
      return basicCharacterObject.GetDefaultFaceSeed(rank) % 2000;
    }

    public enum MultiplayerGameState
    {
      WaitingFirstPlayers,
      Playing,
      Ending,
    }

    public enum MultiplayerGameType
    {
      FreeForAll,
      TeamDeathmatch,
      Duel,
      Siege,
      Battle,
      Captain,
      Skirmish,
    }
  }
}
