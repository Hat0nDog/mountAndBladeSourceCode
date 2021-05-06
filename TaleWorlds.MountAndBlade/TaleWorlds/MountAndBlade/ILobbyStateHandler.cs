// Decompiled with JetBrains decompiler
// Type: TaleWorlds.MountAndBlade.ILobbyStateHandler
// Assembly: TaleWorlds.MountAndBlade, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3C0E3F42-756F-4407-B817-06BA412A78B0
// Assembly location: E:\SteamLibrary\steamapps\common\Mount & Blade II Bannerlord\bin\Win64_Shipping_Client\TaleWorlds.MountAndBlade.dll

using System;
using TaleWorlds.Core;
using TaleWorlds.MountAndBlade.Diamond;
using TaleWorlds.PlayerServices;

namespace TaleWorlds.MountAndBlade
{
  public interface ILobbyStateHandler
  {
    void SetConnectionState(bool isAuthenticated);

    string ShowFeedback(string title, string feedbackText);

    string ShowFeedback(InquiryData inquiryData);

    void DismissFeedback(string id);

    void OnPause();

    void OnResume();

    void OnDisconnected();

    void OnRequestedToSearchBattle();

    void OnUpdateFindingGame(
      MatchmakingWaitTimeStats matchmakingWaitTimeStats,
      string[] gameTypeInfo);

    void OnRequestedToCancelSearchBattle();

    void OnSearchBattleCanceled();

    void OnPlayerDataReceived(PlayerData playerData);

    void OnEnterBattleWithParty(string[] selectedGameTypes);

    void OnPartyInvitationReceived(string inviterPlayerName, PlayerId inviterPlayerId);

    void OnPartyInvitationInvalidated();

    void OnPlayerInvitedToParty(PlayerId playerId, string playerName);

    void OnPlayerAddedToParty(PlayerId playerId, string playerName, bool isPartyLeader);

    void OnPlayerRemovedFromParty(PlayerId playerId);

    void OnSessionInvitationAccepted(PlayerId playerId);

    void OnGameClientStateChange(LobbyClient.State state);

    void OnAdminMessageReceived(string message);

    void OnActivateHome();

    void OnActivateCustomServer();

    void OnActivateMatchmaking();

    void OnActivateArmory();

    void OnActivateOptions();

    void OnDeactivateOptions();

    void OnCustomGameServerListReceived(AvailableCustomGames customGameServerList);

    void OnMatchmakerGameOver(int oldExperience, int newExperience);

    void OnBattleServerLost();

    void OnRemovedFromMatchmakerGame(DisconnectType disconnectType);

    void OnRemovedFromCustomGame(DisconnectType disconnectType);

    void OnPlayerAssignedPartyLeader(PlayerId partyLeaderId);

    void OnPlayerSuggestedToParty(
      PlayerId playerId,
      string playerName,
      PlayerId suggestingPlayerId,
      string suggestingPlayerName);

    void OnJoinCustomGameFailureResponse(CustomGameJoinResponse response);

    void OnServerStatusReceived(ServerStatus serverStatus);

    void OnBattleServerInformationReceived(
      BattleServerInformationForClient battleServerInformation);

    void OnActivateProfile();

    void OnActivateClan();

    void OnClanInvitationReceived(string clanName, string clanTag, bool isCreation);

    void OnClanInvitationAnswered(PlayerId playerId, ClanCreationAnswer answer);

    void OnClanCreationSuccessful();

    void OnClanCreationFailed();

    void OnClanCreationStarted();

    void OnClanInfoChanged();

    void OnPremadeGameEligibilityStatusReceived(bool isEligible);

    void OnPremadeGameCreated();

    void OnPremadeGameListReceived();

    void OnPremadeGameCreationCancelled();

    void OnJoinPremadeGameRequested(string clanName, string clanSigilCode, Guid partyId);

    void OnJoinPremadeGameRequestSuccessful();
  }
}
