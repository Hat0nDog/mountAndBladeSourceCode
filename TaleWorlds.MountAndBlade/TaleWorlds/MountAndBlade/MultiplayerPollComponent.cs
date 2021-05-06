// Decompiled with JetBrains decompiler
// Type: TaleWorlds.MountAndBlade.MultiplayerPollComponent
// Assembly: TaleWorlds.MountAndBlade, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3C0E3F42-756F-4407-B817-06BA412A78B0
// Assembly location: E:\SteamLibrary\steamapps\common\Mount & Blade II Bannerlord\bin\Win64_Shipping_Client\TaleWorlds.MountAndBlade.dll

using NetworkMessages.FromClient;
using NetworkMessages.FromServer;
using System;
using System.Collections.Generic;
using TaleWorlds.Localization;
using TaleWorlds.MountAndBlade.Diamond;
using TaleWorlds.MountAndBlade.Network.Messages;

namespace TaleWorlds.MountAndBlade
{
  public class MultiplayerPollComponent : MissionNetwork
  {
    private MultiplayerPollComponent.PollType _pollType;
    public Action OnSelectPollType;
    public Action OnSelectPlayerForKicking;
    public Action OnSelectPlayerForBanning;
    public Action OnSelectGame;
    public Action<string> OnShowPoll;
    public Action<int, int> OnUpdatePollProgress;
    public Action<string> OnPollNotStarted;
    private MissionLobbyComponent _missionLobbyComponent;
    private object _runningPoll;
    private int? _runningPollStartTime;
    private int _runningPollAcceptedCount;
    private int _runningPollRejectedCount;
    private List<NetworkCommunicator> _runningPollVoted = new List<NetworkCommunicator>();
    private const int PollTimeout = 30;

    public override void OnMissionTick(float dt)
    {
      base.OnMissionTick(dt);
      if (!GameNetwork.IsServer || this._runningPoll == null)
        return;
      int? runningPollStartTime = this._runningPollStartTime;
      int num = Environment.TickCount - 30000;
      if (runningPollStartTime.GetValueOrDefault() < num & runningPollStartTime.HasValue)
        this.EndPoll();
      if (this._runningPollAcceptedCount <= (this._runningPoll is MultiplayerPollComponent.KickPlayer || this._runningPoll is MultiplayerPollComponent.BanPlayer ? (int) Math.Ceiling(((double) GameNetwork.NetworkPeerCount - 1.0) / 2.0) : (int) Math.Ceiling((double) GameNetwork.NetworkPeerCount / 2.0)))
        return;
      this.EndPoll();
    }

    public void EndPoll()
    {
      if (this._runningPollAcceptedCount >= (this._runningPoll is MultiplayerPollComponent.KickPlayer || this._runningPoll is MultiplayerPollComponent.BanPlayer ? (int) Math.Ceiling(((double) GameNetwork.NetworkPeerCount - 1.0) / 2.0) : (int) Math.Ceiling((double) GameNetwork.NetworkPeerCount / 2.0)))
      {
        if (this._runningPoll is MultiplayerPollComponent.KickPlayer)
        {
          MissionPeer component = ((MultiplayerPollComponent.KickPlayer) this._runningPoll).playerPeer.GetComponent<MissionPeer>();
          if (component != null)
          {
            NetworkCommunicator networkPeer = component.GetNetworkPeer();
            DisconnectInfo disconnectInfo = networkPeer.PlayerConnectionInfo.GetParameter<DisconnectInfo>("DisconnectInfo") ?? new DisconnectInfo();
            disconnectInfo.Type = DisconnectType.KickedByPoll;
            networkPeer.PlayerConnectionInfo.AddParameter("DisconnectInfo", (object) disconnectInfo);
            GameNetwork.AddNetworkPeerToDisconnectAsServer(networkPeer);
            if (GameNetwork.IsDedicatedServer)
              throw new NotImplementedException();
            NetworkMain.GameClient.KickPlayer(component.Peer.Id, false);
          }
        }
        else if (this._runningPoll is MultiplayerPollComponent.BanPlayer)
        {
          MissionPeer component = ((MultiplayerPollComponent.BanPlayer) this._runningPoll).playerPeer.GetComponent<MissionPeer>();
          if (component != null)
          {
            NetworkCommunicator networkPeer = component.GetNetworkPeer();
            DisconnectInfo disconnectInfo = networkPeer.PlayerConnectionInfo.GetParameter<DisconnectInfo>("DisconnectInfo") ?? new DisconnectInfo();
            disconnectInfo.Type = DisconnectType.BannedByPoll;
            networkPeer.PlayerConnectionInfo.AddParameter("DisconnectInfo", (object) disconnectInfo);
            GameNetwork.AddNetworkPeerToDisconnectAsServer(networkPeer);
            if (GameNetwork.IsClient)
              BannedPlayerManagerCustomGameClient.AddBannedPlayer(component.Peer.Id, GameNetwork.IsDedicatedServer ? -1 : Environment.TickCount + 600000);
            else if (GameNetwork.IsDedicatedServer)
              BannedPlayerManagerCustomGameServer.AddBannedPlayer(component.Peer.Id, component.GetPeer().UserName, GameNetwork.IsDedicatedServer ? -1 : Environment.TickCount + 600000);
            if (GameNetwork.IsDedicatedServer)
              throw new NotImplementedException();
            NetworkMain.GameClient.KickPlayer(component.Peer.Id, true);
          }
        }
        else if (this._runningPoll is MultiplayerPollComponent.ChangeGame)
        {
          MultiplayerPollComponent.ChangeGame runningPoll = (MultiplayerPollComponent.ChangeGame) this._runningPoll;
          MultiplayerOptions.MultiplayerOptionsAccessMode mode = MultiplayerOptions.MultiplayerOptionsAccessMode.NextMapOptions;
          MultiplayerOptions.OptionType.GameType.SetValue(runningPoll.GameType, mode);
          MultiplayerOptions.Instance.OnGameTypeChanged(mode);
          MultiplayerOptions.OptionType.Map.SetValue(runningPoll.MapName, mode);
          this._missionLobbyComponent.SetStateEndingAsServer();
        }
      }
      this._runningPoll = (object) null;
      this._runningPollStartTime = new int?();
      this._runningPollAcceptedCount = 0;
      this._runningPollRejectedCount = 0;
      this._runningPollVoted.Clear();
    }

    public void CreateAPoll()
    {
      if (this.OnSelectPollType == null)
        return;
      this.OnSelectPollType();
    }

    public void SelectPollType(MultiplayerPollComponent.PollType pollType)
    {
      this._pollType = pollType;
      if (this._pollType == MultiplayerPollComponent.PollType.KickPlayer)
      {
        if (this.OnSelectPlayerForKicking == null)
          return;
        this.OnSelectPlayerForKicking();
      }
      else if (this._pollType == MultiplayerPollComponent.PollType.BanPlayer)
      {
        if (this.OnSelectPlayerForBanning == null)
          return;
        this.OnSelectPlayerForBanning();
      }
      else
      {
        if (this._pollType != MultiplayerPollComponent.PollType.ChangeGame || this.OnSelectGame == null)
          return;
        this.OnSelectGame();
      }
    }

    public void Vote(bool accepted)
    {
      if (GameNetwork.IsServer)
      {
        if (GameNetwork.MyPeer == null)
          return;
        this.ApplyVote(GameNetwork.MyPeer, accepted);
      }
      else
      {
        GameNetwork.BeginModuleEventAsClient();
        GameNetwork.WriteMessage((GameNetworkMessage) new PollResponse(accepted));
        GameNetwork.EndModuleEventAsClient();
      }
    }

    private void ApplyVote(NetworkCommunicator peer, bool accepted)
    {
      if (this._runningPollVoted.Contains(peer))
        return;
      if (accepted)
        ++this._runningPollAcceptedCount;
      else
        ++this._runningPollRejectedCount;
      this._runningPollVoted.Add(peer);
      GameNetwork.BeginBroadcastModuleEvent();
      GameNetwork.WriteMessage((GameNetworkMessage) new PollProgress(this._runningPollAcceptedCount, this._runningPollRejectedCount));
      GameNetwork.EndBroadcastModuleEvent(GameNetwork.EventBroadcastFlags.None);
      this.UpdatePollProgress(this._runningPollAcceptedCount, this._runningPollRejectedCount);
    }

    public void StartPlayerKickPoll(NetworkCommunicator peer, bool banPlayer)
    {
      if (GameNetwork.IsServer)
      {
        if (GameNetwork.MyPeer == null)
          return;
        this.StartPlayerKickPollOnServer(GameNetwork.MyPeer, peer, banPlayer);
      }
      else
      {
        GameNetwork.BeginModuleEventAsClient();
        GameNetwork.WriteMessage((GameNetworkMessage) new NetworkMessages.FromClient.KickPlayerPoll(peer, banPlayer));
        GameNetwork.EndModuleEventAsClient();
      }
    }

    private void RejectPollStartServer(
      NetworkCommunicator pollCreatorPeer,
      MultiplayerPollComponent.PollRejectReason rejectReason)
    {
      if (pollCreatorPeer.IsMine)
      {
        this.RejectPollStartServer(rejectReason.ToString());
      }
      else
      {
        GameNetwork.BeginModuleEventAsServer(pollCreatorPeer);
        GameNetwork.WriteMessage((GameNetworkMessage) new RejectPollStart(rejectReason.ToString()));
        GameNetwork.EndModuleEventAsServer();
      }
    }

    private void RejectPollStartServer(string rejectReason)
    {
      if (this.OnPollNotStarted == null)
        return;
      this.OnPollNotStarted(rejectReason);
    }

    private void StartPlayerKickPollOnServer(
      NetworkCommunicator pollCreatorPeer,
      NetworkCommunicator peer,
      bool banPlayer)
    {
      if (this._runningPoll == null)
      {
        this._runningPoll = (object) new MultiplayerPollComponent.KickPlayer()
        {
          playerPeer = peer
        };
        this._runningPollStartTime = new int?(Environment.TickCount);
        if (!GameNetwork.IsDedicatedServer)
          this.ShowPlayerKickPoll(peer, banPlayer);
        GameNetwork.BeginBroadcastModuleEvent();
        GameNetwork.WriteMessage((GameNetworkMessage) new NetworkMessages.FromServer.KickPlayerPoll(peer, banPlayer));
        GameNetwork.EndBroadcastModuleEvent(GameNetwork.EventBroadcastFlags.None);
      }
      else
        this.RejectPollStartServer(pollCreatorPeer, MultiplayerPollComponent.PollRejectReason.APollIsAlreadyRunning);
    }

    private void StartGameChangePollOnServer(
      NetworkCommunicator pollCreatorPeer,
      string gameType,
      string scene)
    {
      if (this._runningPoll == null)
      {
        this._runningPoll = (object) new MultiplayerPollComponent.ChangeGame()
        {
          GameType = gameType,
          MapName = scene
        };
        this._runningPollStartTime = new int?(Environment.TickCount);
        if (!GameNetwork.IsDedicatedServer)
          this.ShowGameChangePoll(gameType, scene);
        GameNetwork.BeginBroadcastModuleEvent();
        GameNetwork.WriteMessage((GameNetworkMessage) new NetworkMessages.FromServer.ChangeGamePoll(gameType, scene));
        GameNetwork.EndBroadcastModuleEvent(GameNetwork.EventBroadcastFlags.None);
      }
      else
        this.RejectPollStartServer(pollCreatorPeer, MultiplayerPollComponent.PollRejectReason.APollIsAlreadyRunning);
    }

    private void ShowPlayerKickPoll(NetworkCommunicator peer, bool banUser)
    {
      TextObject pollQuestion = new TextObject("{=PNg2Rb4p}{PEER_NAME} will be kicked{IS_BANNED}");
      pollQuestion.SetTextVariable("PEER_NAME", banUser ? " and banned" : "");
      pollQuestion.SetTextVariable("IS_BANNED", peer.UserName);
      this.ShowPoll(pollQuestion);
    }

    private void ShowGameChangePoll(string gameType, string scene)
    {
      TextObject pollQuestion = new TextObject("{=hOSafKZ9}A new {GAME_TYPE} game will be started on {SCENE_NAME}");
      pollQuestion.SetTextVariable("GAME_TYPE", gameType);
      pollQuestion.SetTextVariable("SCENE_NAME", scene);
      this.ShowPoll(pollQuestion);
    }

    private void ShowPoll(TextObject pollQuestion)
    {
      if (this.OnShowPoll == null)
        return;
      this.OnShowPoll(pollQuestion.ToString());
    }

    private void UpdatePollProgress(int votesAccepted, int votesRejected)
    {
      if (this.OnUpdatePollProgress == null)
        return;
      this.OnUpdatePollProgress(votesAccepted, votesRejected);
    }

    public void StartGameChangePoll(string gameType, string map)
    {
      if (GameNetwork.IsServer)
      {
        if (GameNetwork.MyPeer == null)
          return;
        this.StartGameChangePollOnServer(GameNetwork.MyPeer, gameType, map);
      }
      else
      {
        GameNetwork.BeginModuleEventAsClient();
        GameNetwork.WriteMessage((GameNetworkMessage) new NetworkMessages.FromClient.ChangeGamePoll(gameType, map));
        GameNetwork.EndModuleEventAsClient();
      }
    }

    protected override void AddRemoveMessageHandlers(
      GameNetwork.NetworkMessageHandlerRegistererContainer registerer)
    {
      if (GameNetwork.IsClient)
      {
        registerer.Register<NetworkMessages.FromServer.ChangeGamePoll>(new GameNetworkMessage.ServerMessageHandlerDelegate<NetworkMessages.FromServer.ChangeGamePoll>(this.HandleServerEventChangeGamePoll));
        registerer.Register<NetworkMessages.FromServer.KickPlayerPoll>(new GameNetworkMessage.ServerMessageHandlerDelegate<NetworkMessages.FromServer.KickPlayerPoll>(this.HandleServerEventKickPlayerPoll));
        registerer.Register<PollProgress>(new GameNetworkMessage.ServerMessageHandlerDelegate<PollProgress>(this.HandleServerEventUpdatePollProgress));
      }
      else
      {
        if (!GameNetwork.IsServer)
          return;
        registerer.Register<NetworkMessages.FromClient.ChangeGamePoll>(new GameNetworkMessage.ClientMessageHandlerDelegate<NetworkMessages.FromClient.ChangeGamePoll>(this.HandleClientEventChangeGamePoll));
        registerer.Register<NetworkMessages.FromClient.KickPlayerPoll>(new GameNetworkMessage.ClientMessageHandlerDelegate<NetworkMessages.FromClient.KickPlayerPoll>(this.HandleClientEventKickPlayerPoll));
        registerer.Register<PollResponse>(new GameNetworkMessage.ClientMessageHandlerDelegate<PollResponse>(this.HandleClientEventPollResponse));
      }
    }

    public override void OnBehaviourInitialize()
    {
      base.OnBehaviourInitialize();
      this._missionLobbyComponent = this.Mission.GetMissionBehaviour<MissionLobbyComponent>();
    }

    private bool HandleClientEventChangeGamePoll(NetworkCommunicator peer, NetworkMessages.FromClient.ChangeGamePoll message)
    {
      this.StartGameChangePollOnServer(peer, message.GameType, message.Map);
      return true;
    }

    private bool HandleClientEventKickPlayerPoll(NetworkCommunicator peer, NetworkMessages.FromClient.KickPlayerPoll message)
    {
      this.StartPlayerKickPollOnServer(peer, message.PlayerPeer, message.BanPlayer);
      return true;
    }

    private bool HandleClientEventPollResponse(NetworkCommunicator peer, PollResponse message)
    {
      this.ApplyVote(peer, message.Accepted);
      return true;
    }

    private void HandleServerEventChangeGamePoll(NetworkMessages.FromServer.ChangeGamePoll message) => this.ShowGameChangePoll(message.GameType, message.Map);

    private void HandleServerEventKickPlayerPoll(NetworkMessages.FromServer.KickPlayerPoll message) => this.ShowPlayerKickPoll(message.PlayerPeer, message.BanPlayer);

    private void HandleServerEventUpdatePollProgress(PollProgress message) => this.UpdatePollProgress(message.VotesAccepted, message.VotesRejected);

    public struct ChangeGame
    {
      public string GameType { get; set; }

      public string MapName { get; set; }
    }

    public struct KickPlayer
    {
      public NetworkCommunicator playerPeer { get; set; }
    }

    public struct BanPlayer
    {
      public NetworkCommunicator playerPeer { get; set; }
    }

    public enum PollType
    {
      KickPlayer,
      BanPlayer,
      ChangeGame,
    }

    public enum PollRejectReason
    {
      APollIsAlreadyRunning,
      TooFrequentPollsBySamePlayer,
    }
  }
}
