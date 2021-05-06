// Decompiled with JetBrains decompiler
// Type: TaleWorlds.MountAndBlade.LobbyGameClientHandler
// Assembly: TaleWorlds.MountAndBlade, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3C0E3F42-756F-4407-B817-06BA412A78B0
// Assembly location: E:\SteamLibrary\steamapps\common\Mount & Blade II Bannerlord\bin\Win64_Shipping_Client\TaleWorlds.MountAndBlade.dll

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TaleWorlds.Core;
using TaleWorlds.Diamond.ChatSystem.Library;
using TaleWorlds.Engine;
using TaleWorlds.Library;
using TaleWorlds.Localization;
using TaleWorlds.MountAndBlade.Diamond;
using TaleWorlds.PlayerServices;

namespace TaleWorlds.MountAndBlade
{
  public class LobbyGameClientHandler : ILobbyClientSessionHandler
  {
    public IChatHandler ChatHandler;

    void ILobbyClientSessionHandler.OnConnected()
    {
    }

    void ILobbyClientSessionHandler.OnCantConnect()
    {
    }

    void ILobbyClientSessionHandler.OnDisconnected(TextObject feedback)
    {
      if (this.LobbyState == null)
        return;
      this.LobbyState.OnDisconnected(feedback);
    }

    void ILobbyClientSessionHandler.OnPlayerDataReceived(
      PlayerData playerData)
    {
      if (this.LobbyState == null)
        return;
      this.LobbyState.OnPlayerDataReceived(playerData);
    }

    void ILobbyClientSessionHandler.OnBattleResultReceived()
    {
    }

    void ILobbyClientSessionHandler.OnCancelJoiningBattle()
    {
      if (this.LobbyState == null)
        return;
      this.LobbyState.OnCancelFindingGame();
    }

    void ILobbyClientSessionHandler.OnRejoinRequestRejected()
    {
    }

    void ILobbyClientSessionHandler.OnFindGameAnswer(
      bool successful,
      string[] selectedAndEnabledGameTypes,
      bool isRejoin)
    {
      if (!successful || this.LobbyState == null)
        return;
      this.LobbyState.OnUpdateFindingGame(MatchmakingWaitTimeStats.Empty, selectedAndEnabledGameTypes);
    }

    void ILobbyClientSessionHandler.OnEnterBattleWithPartyAnswer(
      string[] selectedGameTypes)
    {
      if (this.LobbyState == null)
        return;
      this.LobbyState.OnUpdateFindingGame(MatchmakingWaitTimeStats.Empty, selectedGameTypes);
    }

    void ILobbyClientSessionHandler.OnWhisperMessageReceived(
      string fromPlayer,
      string toPlayer,
      string message)
    {
      if (this.ChatHandler != null)
        this.ChatHandler.ReceiveChatMessage(ChatChannelType.NaN, fromPlayer, message);
      ChatBox.AddWhisperMessage(fromPlayer, message);
    }

    void ILobbyClientSessionHandler.OnClanMessageReceived(
      string playerName,
      string message)
    {
    }

    void ILobbyClientSessionHandler.OnChannelMessageReceived(
      ChatChannelType channel,
      string playerName,
      string message)
    {
      if (this.ChatHandler != null)
        this.ChatHandler.ReceiveChatMessage(channel, playerName, message);
      ChatBox.AddWhisperMessage(playerName, message);
    }

    void ILobbyClientSessionHandler.OnPartyMessageReceived(
      string playerName,
      string message)
    {
    }

    void ILobbyClientSessionHandler.OnSystemMessageReceived(string message) => InformationManager.DisplayMessage(new InformationMessage(message));

    void ILobbyClientSessionHandler.OnAdminMessageReceived(string message)
    {
      if (this.LobbyState == null)
        return;
      this.LobbyState.OnAdminMessageReceived(message);
    }

    void ILobbyClientSessionHandler.OnPartyInvitationReceived(
      string inviterPlayerName,
      PlayerId inviterPlayerId)
    {
      if (this.LobbyState == null)
        return;
      this.LobbyState.OnPartyInvitationReceived(inviterPlayerName, inviterPlayerId);
    }

    void ILobbyClientSessionHandler.OnPartyInvitationInvalidated()
    {
      if (this.LobbyState == null)
        return;
      this.LobbyState.OnPartyInvitationInvalidated();
    }

    void ILobbyClientSessionHandler.OnPlayerInvitedToParty(
      PlayerId playerId,
      string playerName)
    {
      if (this.LobbyState == null)
        return;
      this.LobbyState.OnPlayerInvitedToParty(playerId, playerName);
    }

    void ILobbyClientSessionHandler.OnPlayersAddedToParty(
      List<(PlayerId PlayerId, string PlayerName, bool IsPartyLeader)> addedPlayers,
      List<(PlayerId PlayerId, string PlayerName)> invitedPlayers)
    {
      if (this.LobbyState == null)
        return;
      this.LobbyState.OnPlayersAddedToParty(addedPlayers, invitedPlayers);
    }

    void ILobbyClientSessionHandler.OnPlayerRemovedFromParty(
      PlayerId playerId)
    {
      if (this.LobbyState == null)
        return;
      this.LobbyState.OnPlayerRemovedFromParty(playerId);
    }

    void ILobbyClientSessionHandler.OnPlayerAssignedPartyLeader(
      PlayerId partyLeaderId)
    {
      if (this.LobbyState == null)
        return;
      this.LobbyState.OnPlayerAssignedPartyLeader(partyLeaderId);
    }

    void ILobbyClientSessionHandler.OnPlayerSuggestedToParty(
      PlayerId playerId,
      string playerName,
      PlayerId suggestingPlayerId,
      string suggestingPlayerName)
    {
      if (this.LobbyState == null)
        return;
      this.LobbyState.OnPlayerSuggestedToParty(playerId, playerName, suggestingPlayerId, suggestingPlayerName);
    }

    void ILobbyClientSessionHandler.OnServerStatusReceived(
      ServerStatus serverStatus)
    {
      if (this.LobbyState == null)
        return;
      this.LobbyState.OnServerStatusReceived(serverStatus);
    }

    void ILobbyClientSessionHandler.OnFriendListReceived(
      FriendInfo[] friends)
    {
      if (this.LobbyState == null)
        return;
      this.LobbyState.OnFriendListReceived(friends);
    }

    void ILobbyClientSessionHandler.OnRecentPlayerStatusesReceived(
      FriendInfo[] friends)
    {
      if (this.LobbyState == null)
        return;
      this.LobbyState.OnRecentPlayerStatusesReceived(friends);
    }

    void ILobbyClientSessionHandler.OnClanInvitationReceived(
      string clanName,
      string clanTag,
      bool isCreation)
    {
      if (this.LobbyState == null)
        return;
      this.LobbyState.OnClanInvitationReceived(clanName, clanTag, isCreation);
    }

    void ILobbyClientSessionHandler.OnClanInvitationAnswered(
      PlayerId playerId,
      ClanCreationAnswer answer)
    {
      if (this.LobbyState == null)
        return;
      this.LobbyState.OnClanInvitationAnswered(playerId, answer);
    }

    void ILobbyClientSessionHandler.OnClanCreationSuccessful()
    {
      if (this.LobbyState == null)
        return;
      this.LobbyState.OnClanCreationSuccessful();
    }

    void ILobbyClientSessionHandler.OnClanCreationFailed()
    {
      if (this.LobbyState == null)
        return;
      this.LobbyState.OnClanCreationFailed();
    }

    void ILobbyClientSessionHandler.OnClanCreationStarted()
    {
      if (this.LobbyState == null)
        return;
      this.LobbyState.OnClanCreationStarted();
    }

    void ILobbyClientSessionHandler.OnClanInfoChanged()
    {
      if (this.LobbyState == null)
        return;
      this.LobbyState.OnClanInfoChanged();
    }

    void ILobbyClientSessionHandler.OnPremadeGameEligibilityStatusReceived(
      bool isEligible)
    {
      if (this.LobbyState == null)
        return;
      this.LobbyState.OnPremadeGameEligibilityStatusReceived(isEligible);
    }

    void ILobbyClientSessionHandler.OnPremadeGameCreated()
    {
      if (this.LobbyState == null)
        return;
      this.LobbyState.OnPremadeGameCreated();
    }

    void ILobbyClientSessionHandler.OnPremadeGameListReceived()
    {
      if (this.LobbyState == null)
        return;
      this.LobbyState.OnPremadeGameListReceived();
    }

    void ILobbyClientSessionHandler.OnPremadeGameCreationCancelled()
    {
      if (this.LobbyState == null)
        return;
      this.LobbyState.OnPremadeGameCreationCancelled();
    }

    void ILobbyClientSessionHandler.OnJoinPremadeGameRequested(
      string clanName,
      string clanSigilCode,
      Guid partyId)
    {
      if (this.LobbyState == null)
        return;
      this.LobbyState.OnJoinPremadeGameRequested(clanName, clanSigilCode, partyId);
    }

    void ILobbyClientSessionHandler.OnJoinPremadeGameRequestSuccessful()
    {
      if (this.LobbyState == null)
        return;
      this.LobbyState.OnJoinPremadeGameRequestSuccessful();
    }

    void ILobbyClientSessionHandler.OnGameClientStateChange(
      LobbyClient.State oldState)
    {
      this.HandleGameClientStateChange(oldState);
    }

    private async void HandleGameClientStateChange(LobbyClient.State oldState)
    {
      LobbyClient gameClient = NetworkMain.GameClient;
      Debug.Print("[][] New MBGameClient State: " + (object) gameClient.CurrentState + " old state:" + (object) oldState);
      switch (gameClient.CurrentState)
      {
        case LobbyClient.State.Idle:
          if (oldState == LobbyClient.State.AtBattle || oldState == LobbyClient.State.HostingCustomGame || oldState == LobbyClient.State.InCustomGame)
          {
            if (Mission.Current != null && !(Game.Current.GameStateManager.ActiveState is MissionState))
              Game.Current.GameStateManager.PopState();
            if (Game.Current.GameStateManager.ActiveState is MissionState)
            {
              MissionState missionSystem = (MissionState) Game.Current.GameStateManager.ActiveState;
              while (missionSystem.CurrentMission.CurrentState == Mission.State.NewlyCreated || missionSystem.CurrentMission.CurrentState == Mission.State.Initializing)
                await Task.Delay(1);
              for (int i = 0; i < 3; ++i)
                await Task.Delay(1);
              missionSystem.CurrentMission.EndMission();
              missionSystem = (MissionState) null;
            }
            while (Mission.Current != null)
              await Task.Delay(1);
            this.LobbyState.SetConnectionState(false);
            break;
          }
          if (oldState == LobbyClient.State.AtLobby || oldState == LobbyClient.State.SearchingBattle)
          {
            this.LobbyState.SetConnectionState(false);
            break;
          }
          switch (oldState)
          {
            case LobbyClient.State.Working:
              this.LobbyState.SetConnectionState(false);
              break;
            case LobbyClient.State.Connected:
              this.LobbyState.SetConnectionState(false);
              break;
            case LobbyClient.State.SessionRequested:
              this.LobbyState.SetConnectionState(false);
              break;
            case LobbyClient.State.WaitingToJoinCustomGame:
              this.LobbyState.SetConnectionState(false);
              break;
          }
          break;
        case LobbyClient.State.AtLobby:
          this.LobbyState.SetConnectionState(true);
          break;
        case LobbyClient.State.RequestingToSearchBattle:
          this.LobbyState.OnRequestedToSearchBattle();
          break;
        case LobbyClient.State.RequestingToCancelSearchBattle:
          this.LobbyState.OnRequestedToCancelSearchBattle();
          break;
      }
      this.LobbyState.OnGameClientStateChange(gameClient.CurrentState);
    }

    void ILobbyClientSessionHandler.OnCustomGameServerListReceived(
      AvailableCustomGames customGameServerList)
    {
      this.LobbyState.OnCustomGameServerListReceived(customGameServerList);
    }

    void ILobbyClientSessionHandler.OnMatchmakerGameOver(
      int oldExperience,
      int newExperience)
    {
      GameStateManager gameStateManager = Game.Current.GameStateManager;
      if (!(gameStateManager.ActiveState is LobbyState))
      {
        if (gameStateManager.ActiveState is MissionState)
          BannerlordNetwork.EndMultiplayerLobbyMission();
        else
          gameStateManager.PopState();
      }
      this.LobbyState.OnMatchmakerGameOver(oldExperience, newExperience);
    }

    void ILobbyClientSessionHandler.OnQuitFromMatchmakerGame()
    {
      GameStateManager gameStateManager = Game.Current.GameStateManager;
      if (gameStateManager.ActiveState is LobbyState)
        return;
      if (gameStateManager.ActiveState is MissionState)
        BannerlordNetwork.EndMultiplayerLobbyMission();
      else
        gameStateManager.PopState();
    }

    void ILobbyClientSessionHandler.OnBattleServerInformationReceived(
      BattleServerInformationForClient battleServerInformation)
    {
      if (this.LobbyState != null)
        this.LobbyState.OnBattleServerInformationReceived(battleServerInformation);
      LobbyGameStateMatchmakerClient state = Game.Current.GameStateManager.CreateState<LobbyGameStateMatchmakerClient>();
      state.SetStartingParameters(this, battleServerInformation.PeerIndex, battleServerInformation.SessionKey, battleServerInformation.ServerAddress, (int) battleServerInformation.ServerPort, battleServerInformation.GameType, battleServerInformation.SceneName);
      Game.Current.GameStateManager.PushState((GameState) state);
    }

    void ILobbyClientSessionHandler.OnBattleServerLost()
    {
      GameStateManager gameStateManager = Game.Current.GameStateManager;
      if (!(gameStateManager.ActiveState is LobbyState))
      {
        if (gameStateManager.ActiveState is MissionState)
          BannerlordNetwork.EndMultiplayerLobbyMission();
        else
          gameStateManager.PopState();
      }
      this.LobbyState.OnBattleServerLost();
    }

    void ILobbyClientSessionHandler.OnRemovedFromMatchmakerGame(
      DisconnectType disconnectType)
    {
      GameStateManager gameStateManager = Game.Current.GameStateManager;
      if (!(gameStateManager.ActiveState is LobbyState))
      {
        if (gameStateManager.ActiveState is MissionState)
          BannerlordNetwork.EndMultiplayerLobbyMission();
        else
          gameStateManager.PopState();
      }
      this.LobbyState.OnRemovedFromMatchmakerGame(disconnectType);
    }

    void ILobbyClientSessionHandler.OnRegisterCustomGameServerResponse()
    {
      if (GameNetwork.IsSessionActive)
        return;
      LobbyGameStatePlayerBasedCustomServer state = Game.Current.GameStateManager.CreateState<LobbyGameStatePlayerBasedCustomServer>();
      state.SetStartingParameters(this);
      Game.Current.GameStateManager.PushState((GameState) state);
    }

    void ILobbyClientSessionHandler.OnCustomGameEnd()
    {
      if (Game.Current == null)
        return;
      GameStateManager gameStateManager = Game.Current.GameStateManager;
      if (gameStateManager.ActiveState is LobbyState)
        return;
      if (Game.Current.GameStateManager.ActiveState is MissionState)
        BannerlordNetwork.EndMultiplayerLobbyMission();
      else
        gameStateManager.PopState();
    }

    PlayerJoinGameResponseDataFromHost[] ILobbyClientSessionHandler.OnClientWantsToConnectCustomGame(
      PlayerJoinGameData[] playerJoinData,
      string password)
    {
      Debug.Print("Game join request with party received", color: Debug.DebugColor.Green);
      CustomGameJoinResponse gameJoinResponse = CustomGameJoinResponse.UnspecifiedError;
      List<PlayerJoinGameResponseDataFromHost> responseDataFromHostList = new List<PlayerJoinGameResponseDataFromHost>();
      if (Mission.Current != null && Mission.Current.CurrentState == Mission.State.Continuing)
      {
        foreach (PlayerJoinGameData playerJoinGameData in playerJoinData)
        {
          if (BannedPlayerManagerCustomGameClient.IsUserBanned(playerJoinGameData.PlayerId))
            gameJoinResponse = CustomGameJoinResponse.PlayerBanned;
        }
        if (gameJoinResponse != CustomGameJoinResponse.PlayerBanned)
        {
          string strValue1 = MultiplayerOptions.OptionType.AdminPassword.GetStrValue();
          string strValue2 = MultiplayerOptions.OptionType.GamePassword.GetStrValue();
          bool isAdmin = !string.IsNullOrEmpty(strValue1) && strValue1 == password && playerJoinData.Length == 1;
          bool flag1 = isAdmin || string.IsNullOrEmpty(strValue2) || strValue2 == password;
          bool flag2 = MultiplayerOptions.OptionType.MaxNumberOfPlayers.GetIntValue() < GameNetwork.NetworkPeerCount + playerJoinData.Length;
          if (flag1 && !flag2)
          {
            List<PlayerConnectionInfo> playerConnectionInfoList = new List<PlayerConnectionInfo>();
            foreach (PlayerJoinGameData playerJoinGameData in playerJoinData)
            {
              PlayerConnectionInfo playerConnectionInfo = new PlayerConnectionInfo();
              playerConnectionInfo.AddParameter("PlayerData", (object) playerJoinGameData.PlayerData);
              playerConnectionInfo.Name = playerJoinGameData.Name;
              playerConnectionInfoList.Add(playerConnectionInfo);
            }
            GameNetwork.AddPlayersResult addPlayersResult = GameNetwork.HandleNewClientsConnect(playerConnectionInfoList.ToArray(), isAdmin);
            if (addPlayersResult.Success)
            {
              for (int index = 0; index < playerJoinData.Length; ++index)
              {
                PlayerJoinGameData playerJoinGameData = playerJoinData[index];
                NetworkCommunicator networkPeer = addPlayersResult.NetworkPeers[index];
                PlayerJoinGameResponseDataFromHost responseDataFromHost = new PlayerJoinGameResponseDataFromHost()
                {
                  PlayerId = playerJoinGameData.PlayerId,
                  PeerIndex = networkPeer.Index,
                  SessionKey = networkPeer.SessionKey,
                  CustomGameJoinResponse = CustomGameJoinResponse.Success
                };
                responseDataFromHostList.Add(responseDataFromHost);
              }
              gameJoinResponse = CustomGameJoinResponse.Success;
            }
            else
              gameJoinResponse = CustomGameJoinResponse.ErrorOnGameServer;
          }
          else
            gameJoinResponse = flag1 ? (!flag2 ? CustomGameJoinResponse.UnspecifiedError : CustomGameJoinResponse.ServerCapacityIsFull) : CustomGameJoinResponse.IncorrectPassword;
        }
      }
      else
        gameJoinResponse = CustomGameJoinResponse.CustomGameServerNotAvailable;
      if (gameJoinResponse != CustomGameJoinResponse.Success)
      {
        foreach (PlayerJoinGameData playerJoinGameData in playerJoinData)
        {
          PlayerJoinGameResponseDataFromHost responseDataFromHost = new PlayerJoinGameResponseDataFromHost()
          {
            PlayerId = playerJoinGameData.PlayerId,
            PeerIndex = -1,
            SessionKey = -1,
            CustomGameJoinResponse = gameJoinResponse
          };
          responseDataFromHostList.Add(responseDataFromHost);
        }
      }
      Debug.Print("Responding game join request with " + (object) gameJoinResponse);
      return responseDataFromHostList.ToArray();
    }

    void ILobbyClientSessionHandler.OnJoinCustomGameResponse(
      bool success,
      JoinGameData joinGameData,
      CustomGameJoinResponse failureReason)
    {
      if (!success)
        return;
      Module.CurrentModule.GetMultiplayerGameMode(joinGameData.GameServerProperties.GameType).JoinCustomGame(joinGameData);
      Debug.Print("Join game successful", color: Debug.DebugColor.Green);
      this.LobbyState.OnJoinCustomGameResponse(joinGameData);
    }

    void ILobbyClientSessionHandler.OnJoinCustomGameFailureResponse(
      CustomGameJoinResponse response)
    {
      this.LobbyState.OnJoinCustomGameFailureResponse(response);
    }

    void ILobbyClientSessionHandler.OnQuitFromCustomGame()
    {
      GameStateManager gameStateManager = Game.Current.GameStateManager;
      if (gameStateManager.ActiveState is LobbyState)
        return;
      if (gameStateManager.ActiveState is MissionState)
        BannerlordNetwork.EndMultiplayerLobbyMission();
      else
        gameStateManager.PopState();
    }

    void ILobbyClientSessionHandler.OnRemovedFromCustomGame(
      DisconnectType disconnectType)
    {
      GameStateManager gameStateManager = Game.Current.GameStateManager;
      if (!(gameStateManager.ActiveState is LobbyState))
      {
        if (gameStateManager.ActiveState is MissionState)
          BannerlordNetwork.EndMultiplayerLobbyMission();
        else
          gameStateManager.PopState();
      }
      this.LobbyState.OnRemovedFromCustomGame(disconnectType);
    }

    void ILobbyClientSessionHandler.OnEnterCustomBattleWithPartyAnswer()
    {
    }

    void ILobbyClientSessionHandler.OnClientQuitFromCustomGame(
      PlayerId playerId)
    {
      if (Mission.Current == null || Mission.Current.CurrentState != Mission.State.Continuing)
        return;
      NetworkCommunicator networkPeer = GameNetwork.NetworkPeers.FirstOrDefault<NetworkCommunicator>((Func<NetworkCommunicator, bool>) (x => x.VirtualPlayer.Id == playerId));
      if (networkPeer == null || networkPeer.IsServerPeer)
        return;
      if (networkPeer.GetComponent<MissionPeer>() != null)
        networkPeer.QuitFromMission = true;
      GameNetwork.AddNetworkPeerToDisconnectAsServer(networkPeer);
      MBDebug.Print("player with id " + (object) playerId + " quit from game");
    }

    void ILobbyClientSessionHandler.OnChatMessageReceived(
      Guid roomId,
      string roomName,
      string playerName,
      string messageText,
      string textColor,
      MessageType type)
    {
      InformationMessage message;
      if (type != MessageType.System)
        message = new InformationMessage("[" + roomName + "] [" + playerName + "]: " + messageText, Color.ConvertStringToColor(textColor));
      else
        message = new InformationMessage("[" + roomName + "]: " + messageText, Color.ConvertStringToColor(textColor));
      InformationManager.DisplayMessage(message);
    }

    void ILobbyClientSessionHandler.OnInviteToGame(PlayerId playerId) => this.LobbyState.OnInviteToGame(playerId);

    void ILobbyClientSessionHandler.OnInvitedPlayerOnline(
      PlayerId playerId)
    {
      this.LobbyState.OnInvitedPlayerOnline(playerId);
    }

    public LobbyState LobbyState { get; set; }

    public LobbyGameState LobbyGameState { get; set; }

    public LobbyClient GameClient => NetworkMain.GameClient;
  }
}
