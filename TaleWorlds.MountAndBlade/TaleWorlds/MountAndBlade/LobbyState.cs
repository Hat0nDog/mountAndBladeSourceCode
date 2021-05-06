// Decompiled with JetBrains decompiler
// Type: TaleWorlds.MountAndBlade.LobbyState
// Assembly: TaleWorlds.MountAndBlade, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3C0E3F42-756F-4407-B817-06BA412A78B0
// Assembly location: E:\SteamLibrary\steamapps\common\Mount & Blade II Bannerlord\bin\Win64_Shipping_Client\TaleWorlds.MountAndBlade.dll

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using TaleWorlds.Core;
using TaleWorlds.Diamond;
using TaleWorlds.Diamond.AccessProvider.Test;
using TaleWorlds.Engine;
using TaleWorlds.Library;
using TaleWorlds.Localization;
using TaleWorlds.MountAndBlade.Diamond;
using TaleWorlds.PlatformService;
using TaleWorlds.PlayerServices;

namespace TaleWorlds.MountAndBlade
{
  public class LobbyState : GameState
  {
    private const string _newsSourceURLBase = "https://taleworldswebsiteassets.blob.core.windows.net/upload/bannerlordnews/NewsFeed_";
    private BannerlordFriendListService _bannerlordFriendListService;
    private RecentPlayersFriendListService _recentPlayersFriendListService;
    private CustomBattleId _customGameInvitation;
    private bool _playerInJoinableSession;
    private int _joinableSessionCapacity;
    private ILobbyStateHandler _handler;
    private LobbyGameClientHandler _lobbyGameClientManager;

    private bool AutoConnect => TestCommonBase.BaseInstance == null || !TestCommonBase.BaseInstance.IsTestEnabled;

    public override bool IsMenuState => true;

    public override bool IsMusicMenuState => false;

    public ILobbyStateHandler Handler
    {
      get => this._handler;
      set => this._handler = value;
    }

    public LobbyClient LobbyClient => NetworkMain.GameClient;

    public TaleWorlds.Library.NewsManager.NewsManager NewsManager { get; private set; }

    public void InitializeLogic(ILobbyStateHandler lobbyStateHandler) => this.Handler = lobbyStateHandler;

    protected override async void OnInitialize()
    {
      LobbyState lobbyState = this;
      // ISSUE: reference to a compiler-generated method
      lobbyState.\u003C\u003En__0();
      PlatformServices.Instance.OnSignInStateUpdated += new Action<bool>(lobbyState.OnPlatformSignInStateUpdated);
      foreach (IFriendListService friendListService in PlatformServices.Instance.GetFriendListServices())
      {
        System.Type type = friendListService.GetType();
        if (type == typeof (BannerlordFriendListService))
          lobbyState._bannerlordFriendListService = (BannerlordFriendListService) friendListService;
        else if (type == typeof (RecentPlayersFriendListService))
          lobbyState._recentPlayersFriendListService = (RecentPlayersFriendListService) friendListService;
      }
      PlatformServices.OnSessionInvitationAccepted += new Action<SessionInvitationType>(lobbyState.OnSessionInvitationAccepted);
      lobbyState.NewsManager = new TaleWorlds.Library.NewsManager.NewsManager();
      lobbyState.NewsManager.SetNewsSourceURL(lobbyState.GetApplicableNewsSourceURL());
      RecentPlayersManager.Initialize();
      lobbyState._lobbyGameClientManager = new LobbyGameClientHandler();
      lobbyState._lobbyGameClientManager.LobbyState = lobbyState;
      lobbyState.NewsManager.UpdateNewsItems(false);
      if (PlatformServices.SessionInvitationType == SessionInvitationType.ConnectionString)
        lobbyState.OnSessionInvitationByConnectionStringAccepted();
      if (lobbyState.AutoConnect || PlatformServices.SessionInvitationType == SessionInvitationType.Multiplayer)
      {
        if (PlatformServices.SessionInvitationType == SessionInvitationType.Multiplayer)
          PlatformServices.OnSessionInvitationHandled();
        await lobbyState.TryLogin();
      }
      else
      {
        lobbyState.SetConnectionState(false);
        lobbyState.OnResume();
      }
    }

    protected override void OnFinalize()
    {
      base.OnFinalize();
      PlatformServices.OnSessionInvitationAccepted -= new Action<SessionInvitationType>(this.OnSessionInvitationAccepted);
      PlatformServices.Instance.OnSignInStateUpdated -= new Action<bool>(this.OnPlatformSignInStateUpdated);
      RecentPlayersManager.Serialize();
    }

    protected override void OnTick(float dt)
    {
      base.OnTick(dt);
      if (!(this._customGameInvitation.Guid != Guid.Empty) || this.LobbyClient.IsInParty || !this.LobbyClient.AtLobby)
        return;
      CustomBattleId customGameInvitation = this._customGameInvitation;
      this._customGameInvitation = new CustomBattleId(Guid.Empty);
      this.JoinCustomGameOnInvite(customGameInvitation);
    }

    private string GetApplicableNewsSourceURL()
    {
      int num = this.NewsManager.LocalizationID == "zh" ? 1 : 0;
      bool isInPreviewMode = this.NewsManager.IsInPreviewMode;
      string str = num != 0 ? "zh" : "en";
      return !isInPreviewMode ? "https://taleworldswebsiteassets.blob.core.windows.net/upload/bannerlordnews/NewsFeed_" + str + ".json" : "https://taleworldswebsiteassets.blob.core.windows.net/upload/bannerlordnews/NewsFeed_" + str + "_preview.json";
    }

    public async Task TryLogin()
    {
      LobbyClient gameClient = this.LobbyClient;
      if (!gameClient.IsIdle)
        return;
      if (await gameClient.CanLogin())
      {
        ILoginAccessProvider clientLoginProvider = PlatformServices.Instance.CreateLobbyClientLoginProvider();
        string userName = clientLoginProvider.GetUserName();
        bool startedWithAntiCheat = Module.CurrentModule.StartupInfo.IsStartedWithAntiCheat;
        LobbyClientConnectResult clientConnectResult = await gameClient.Connect((ILobbyClientSessionHandler) this._lobbyGameClientManager, clientLoginProvider, userName, startedWithAntiCheat, PlatformServices.Instance.GetInitParams());
        if (clientConnectResult.Connected)
        {
          this.OnResume();
        }
        else
        {
          this.ShowFeedback(new TextObject("{=lVfmVHbz}Login Failed").ToString(), clientConnectResult.Error.ToString());
          this.SetConnectionState(false);
          this.OnResume();
        }
      }
      else
        this.ShowFeedback(new TextObject("{=lVfmVHbz}Login Failed").ToString(), new TextObject("{=pgw7LMRo}Server over capacity.").ToString());
    }

    public async Task TryLogin(string userName, string password, bool forceAntiCheatEnabled = false)
    {
      bool isRunningWithAntiCheat = forceAntiCheatEnabled || Module.CurrentModule.StartupInfo.IsStartedWithAntiCheat;
      LobbyClientConnectResult clientConnectResult = await NetworkMain.GameClient.Connect((ILobbyClientSessionHandler) this._lobbyGameClientManager, (ILoginAccessProvider) new TestLoginAccessProvider(), userName, isRunningWithAntiCheat, PlatformServices.Instance.GetInitParams());
      if (clientConnectResult.Connected)
        return;
      this.ShowFeedback(new TextObject("{=lVfmVHbz}Login Failed").ToString(), clientConnectResult.Error.ToString());
    }

    public void HostGame()
    {
      if (string.IsNullOrEmpty(MultiplayerOptions.OptionType.ServerName.GetStrValue()))
        MultiplayerOptions.OptionType.ServerName.SetValue(NetworkMain.GameClient.Name);
      string strValue1 = MultiplayerOptions.OptionType.GamePassword.GetStrValue();
      string strValue2 = MultiplayerOptions.OptionType.AdminPassword.GetStrValue();
      string str1 = !string.IsNullOrEmpty(strValue1) ? Common.CalculateMD5Hash(strValue1) : (string) null;
      string str2 = !string.IsNullOrEmpty(strValue2) ? Common.CalculateMD5Hash(strValue2) : (string) null;
      MultiplayerOptions.OptionType.GamePassword.SetValue(str1);
      MultiplayerOptions.OptionType.AdminPassword.SetValue(str2);
      string strValue3 = MultiplayerOptions.OptionType.GameType.GetStrValue();
      NetworkMain.GameClient.RegisterCustomGame(MultiplayerGameTypes.GetGameTypeInfo(strValue3).GameModule, strValue3, MultiplayerOptions.OptionType.ServerName.GetStrValue(), MultiplayerOptions.OptionType.MaxNumberOfPlayers.GetIntValue(), MultiplayerOptions.OptionType.Map.GetStrValue(), MultiplayerOptions.OptionType.GamePassword.GetStrValue(), MultiplayerOptions.OptionType.AdminPassword.GetStrValue(), 9999);
      MultiplayerOptions.Instance.InitializeAllOptionsFromCurrent();
    }

    public void CreateClanGame()
    {
      string strValue1 = MultiplayerOptions.OptionType.ServerName.GetStrValue();
      string strValue2 = MultiplayerOptions.OptionType.ClanMatchType.GetStrValue();
      string strValue3 = MultiplayerOptions.OptionType.Map.GetStrValue();
      string strValue4 = MultiplayerOptions.OptionType.CultureTeam1.GetStrValue();
      string strValue5 = MultiplayerOptions.OptionType.CultureTeam2.GetStrValue();
      string strValue6 = MultiplayerOptions.OptionType.GamePassword.GetStrValue();
      if (strValue1 != null && !strValue1.IsEmpty<char>())
        NetworkMain.GameClient.CreatePremadeGame(strValue1, strValue2, strValue3, strValue4, strValue5, strValue6);
      else
        this.ShowFeedback(new TextObject("{=oZrVNUOk}Error").ToString(), new TextObject("{=EgTUzWUz}Name Can't Be Empty!").ToString());
    }

    private async void JoinCustomGameOnInvite(CustomBattleId customBattleId)
    {
      if (await NetworkMain.GameClient.RequestJoinCustomGame(customBattleId, ""))
        return;
      InformationManager.ShowInquiry(new InquiryData("", GameTexts.FindText("str_couldnt_join_server").ToString(), true, false, GameTexts.FindText("str_ok").ToString(), "", (Action) null, (Action) null));
    }

    private void UpdatePlayerJoinableSession()
    {
      if (this.LobbyClient.CurrentState == LobbyClient.State.InCustomGame)
      {
        int num = this._joinableSessionCapacity - MBNetwork.NetworkPeers.Count;
        if (!this._playerInJoinableSession && num > 0)
        {
          this._playerInJoinableSession = true;
          PlatformServices.SessionService?.OnJoinJoinableSession("CustomGame:" + this.LobbyClient.CurrentMatchId);
        }
        else
        {
          if (!this._playerInJoinableSession || num != 0)
            return;
          this._playerInJoinableSession = false;
          PlatformServices.SessionService?.OnLeaveJoinableSession();
        }
      }
      else
      {
        if (!this._playerInJoinableSession)
          return;
        this._playerInJoinableSession = false;
        PlatformServices.SessionService?.OnLeaveJoinableSession();
      }
    }

    public string ShowFeedback(string title, string message) => this.Handler != null ? this.Handler.ShowFeedback(title, message) : (string) null;

    public string ShowFeedback(InquiryData inquiryData) => this.Handler != null ? this.Handler.ShowFeedback(inquiryData) : (string) null;

    public void DismissFeedback(string messageId)
    {
      if (this.Handler == null)
        return;
      this.Handler.DismissFeedback(messageId);
    }

    public void OnPause()
    {
      if (this.Handler == null)
        return;
      this.Handler.OnPause();
    }

    public void OnResume()
    {
      if (this.Handler == null)
        return;
      this.Handler.OnResume();
    }

    public void OnRequestedToSearchBattle()
    {
      if (this.Handler == null)
        return;
      this.Handler.OnRequestedToSearchBattle();
    }

    public void OnUpdateFindingGame(
      MatchmakingWaitTimeStats matchmakingWaitTimeStats,
      string[] gameTypeInfo = null)
    {
      if (this.Handler == null)
        return;
      this.Handler.OnUpdateFindingGame(matchmakingWaitTimeStats, gameTypeInfo);
    }

    public void OnRequestedToCancelSearchBattle()
    {
      if (this.Handler == null)
        return;
      this.Handler.OnRequestedToCancelSearchBattle();
    }

    public void OnCancelFindingGame()
    {
      if (this.Handler == null)
        return;
      this.Handler.OnSearchBattleCanceled();
    }

    public void OnDisconnected(TextObject feedback)
    {
      if (this.Handler != null)
        this.Handler.OnDisconnected();
      if (feedback == null)
        return;
      this.ShowFeedback(new TextObject("{=MbXatV1Q}Disconnected").ToString(), feedback.ToString());
    }

    public void OnPlayerDataReceived(PlayerData playerData)
    {
      if (this.Handler == null)
        return;
      this.Handler.OnPlayerDataReceived(playerData);
    }

    public void OnEnterBattleWithParty(string[] selectedGameTypes)
    {
      if (this.Handler == null)
        return;
      this.Handler.OnEnterBattleWithParty(selectedGameTypes);
    }

    public void OnPartyInvitationReceived(string inviterPlayerName, PlayerId inviterPlayerId)
    {
      IPlatformInvitationServices invitationServices = PlatformServices.InvitationServices;
      if ((invitationServices != null ? (invitationServices.OnPartyInvitationReceived(inviterPlayerId) ? 1 : 0) : 0) != 0)
      {
        this.LobbyClient.AcceptPartyInvitation();
      }
      else
      {
        if (this.Handler == null)
          return;
        this.Handler.OnPartyInvitationReceived(inviterPlayerName, inviterPlayerId);
      }
    }

    public void OnAdminMessageReceived(string message)
    {
      if (this.Handler == null)
        return;
      this.Handler.OnAdminMessageReceived(message);
    }

    public void OnPartyInvitationInvalidated()
    {
      if (this.Handler == null)
        return;
      this.Handler.OnPartyInvitationInvalidated();
    }

    public void OnPlayerInvitedToParty(PlayerId playerId, string playerName)
    {
      if (this.LobbyClient.IsPartyLeader)
        PlatformServices.InvitationServices?.OnInviteToParty(playerId);
      if (this.Handler == null)
        return;
      this.Handler.OnPlayerInvitedToParty(playerId, playerName);
    }

    public void OnPlayerRemovedFromParty(PlayerId playerId)
    {
      if (playerId.Equals((object) this.LobbyClient.PlayerID))
        PlatformServices.InvitationServices?.OnLeftParty();
      if (this.Handler == null)
        return;
      this.Handler.OnPlayerRemovedFromParty(playerId);
    }

    public void OnPlayersAddedToParty(
      List<(PlayerId PlayerId, string PlayerName, bool IsPartyLeader)> addedPlayers,
      List<(PlayerId PlayerId, string PlayerName)> invitedPlayers)
    {
      foreach ((PlayerId PlayerId, string PlayerName, bool IsPartyLeader) addedPlayer in addedPlayers)
      {
        if (addedPlayer.PlayerId.Equals((object) this.LobbyClient.PlayerID))
          PlatformServices.InvitationServices?.CheckPartySession(this.LobbyClient.PlayersInParty.Select<PartyPlayerInLobbyClient, PlayerId>((Func<PartyPlayerInLobbyClient, PlayerId>) (p => p.PlayerId)), this.LobbyClient.IsPartyLeader);
      }
      if (this.Handler == null)
        return;
      foreach ((PlayerId PlayerId, string PlayerName, bool IsPartyLeader) addedPlayer in addedPlayers)
        this.Handler.OnPlayerAddedToParty(addedPlayer.PlayerId, addedPlayer.PlayerName, addedPlayer.IsPartyLeader);
      foreach ((PlayerId PlayerId, string PlayerName) invitedPlayer in invitedPlayers)
        this.Handler.OnPlayerInvitedToParty(invitedPlayer.PlayerId, invitedPlayer.PlayerName);
    }

    public void OnGameClientStateChange(LobbyClient.State state)
    {
      if (this.Handler != null)
        this.Handler.OnGameClientStateChange(state);
      this.UpdatePlayerJoinableSession();
    }

    public void SetConnectionState(bool isAuthenticated)
    {
      if (this.Handler != null)
        this.Handler.SetConnectionState(isAuthenticated);
      PlatformServices.ConnectionStateChanged(isAuthenticated);
    }

    public void OnActivateHome()
    {
      if (this.Handler == null)
        return;
      this.Handler.OnActivateHome();
    }

    public void OnActivateCustomServer()
    {
      if (this.Handler == null)
        return;
      this.Handler.OnActivateCustomServer();
    }

    public void OnActivateMatchmaking()
    {
      if (this.Handler == null)
        return;
      this.Handler.OnActivateMatchmaking();
    }

    public void OnActivateProfile()
    {
      if (this.Handler == null)
        return;
      this.Handler.OnActivateProfile();
    }

    public void OnActivateClan()
    {
      if (this.Handler == null)
        return;
      this.Handler.OnActivateClan();
    }

    public void OnClanInvitationReceived(string clanName, string clanTag, bool isCreation)
    {
      if (this.Handler == null)
        return;
      this.Handler.OnClanInvitationReceived(clanName, clanTag, isCreation);
    }

    public void OnClanInvitationAnswered(PlayerId playerId, ClanCreationAnswer answer)
    {
      if (this.Handler == null)
        return;
      this.Handler.OnClanInvitationAnswered(playerId, answer);
    }

    public void OnClanCreationSuccessful()
    {
      if (this.Handler == null)
        return;
      this.Handler.OnClanCreationSuccessful();
    }

    public void OnClanCreationFailed()
    {
      if (this.Handler == null)
        return;
      this.Handler.OnClanCreationFailed();
    }

    public void OnClanCreationStarted()
    {
      if (this.Handler == null)
        return;
      this.Handler.OnClanCreationStarted();
    }

    public void OnClanInfoChanged()
    {
      if (this.Handler == null)
        return;
      this.Handler.OnClanInfoChanged();
    }

    public void OnPremadeGameEligibilityStatusReceived(bool isEligible)
    {
      if (this.Handler == null)
        return;
      this.Handler.OnPremadeGameEligibilityStatusReceived(isEligible);
    }

    public void OnPremadeGameCreated()
    {
      if (this.Handler == null)
        return;
      this.Handler.OnPremadeGameCreated();
    }

    public void OnPremadeGameListReceived()
    {
      if (this.Handler == null)
        return;
      this.Handler.OnPremadeGameListReceived();
    }

    public void OnPremadeGameCreationCancelled()
    {
      if (this.Handler == null)
        return;
      this.Handler.OnPremadeGameCreationCancelled();
    }

    public void OnJoinPremadeGameRequested(string clanName, string clanSigilCode, Guid partyId)
    {
      if (this.Handler == null)
        return;
      this.Handler.OnJoinPremadeGameRequested(clanName, clanSigilCode, partyId);
    }

    public void OnJoinPremadeGameRequestSuccessful()
    {
      if (this.Handler == null)
        return;
      this.Handler.OnJoinPremadeGameRequestSuccessful();
    }

    public void OnActivateArmory()
    {
      if (this.Handler == null)
        return;
      this.Handler.OnActivateArmory();
    }

    public void OnActivateOptions()
    {
      if (this.Handler == null)
        return;
      this.Handler.OnActivateOptions();
    }

    public void OnDeactivateOptions()
    {
      if (this.Handler == null)
        return;
      this.Handler.OnDeactivateOptions();
    }

    internal void OnCustomGameServerListReceived(AvailableCustomGames customGameServerList)
    {
      if (this.Handler == null)
        return;
      this.Handler.OnCustomGameServerListReceived(customGameServerList);
    }

    internal void OnMatchmakerGameOver(int oldExp, int newExp)
    {
      if (this.Handler == null)
        return;
      this.Handler.OnMatchmakerGameOver(oldExp, newExp);
    }

    internal void OnBattleServerLost()
    {
      if (this.Handler == null)
        return;
      this.Handler.OnBattleServerLost();
    }

    internal void OnRemovedFromMatchmakerGame(DisconnectType disconnectType)
    {
      if (this.Handler == null)
        return;
      this.Handler.OnRemovedFromMatchmakerGame(disconnectType);
    }

    public void OnJoinCustomGameResponse(JoinGameData joinGameData)
    {
      this._joinableSessionCapacity = joinGameData.GameServerProperties.MaxPlayerCount;
      this.UpdatePlayerJoinableSession();
    }

    internal void OnRemovedFromCustomGame(DisconnectType disconnectType)
    {
      if (this.Handler == null)
        return;
      this.Handler.OnRemovedFromCustomGame(disconnectType);
    }

    internal void OnPlayerAssignedPartyLeader(PlayerId partyLeaderId)
    {
      if (this.Handler == null)
        return;
      this.Handler.OnPlayerAssignedPartyLeader(partyLeaderId);
    }

    internal void OnPlayerSuggestedToParty(
      PlayerId playerId,
      string playerName,
      PlayerId suggestingPlayerId,
      string suggestingPlayerName)
    {
      if (this.Handler == null)
        return;
      this.Handler.OnPlayerSuggestedToParty(playerId, playerName, suggestingPlayerId, suggestingPlayerName);
    }

    internal void OnJoinCustomGameFailureResponse(CustomGameJoinResponse response)
    {
      if (this.Handler == null)
        return;
      this.Handler.OnJoinCustomGameFailureResponse(response);
    }

    internal void OnServerStatusReceived(ServerStatus serverStatus)
    {
      if (this.Handler == null)
        return;
      this.Handler.OnServerStatusReceived(serverStatus);
    }

    internal void OnFriendListReceived(FriendInfo[] friends) => this._bannerlordFriendListService?.OnFriendListReceived(friends);

    internal void OnRecentPlayerStatusesReceived(FriendInfo[] friends) => this._recentPlayersFriendListService?.OnFriendListReceived(friends);

    internal void OnBattleServerInformationReceived(
      BattleServerInformationForClient battleServerInformation)
    {
      if (this.Handler == null)
        return;
      this.Handler.OnBattleServerInformationReceived(battleServerInformation);
    }

    private void OnPlatformSignInStateUpdated(bool isSignedIn)
    {
      if (isSignedIn || !this.LobbyClient.Connected)
        return;
      this.LobbyClient.Logout(new TextObject("{=oPOa77dI}Logged out of platform"));
    }

    [Conditional("DEBUG")]
    private void PrintCompressionInfoKey()
    {
      try
      {
        List<System.Type> typeList = new List<System.Type>();
        Assembly[] array = ((IEnumerable<Assembly>) AppDomain.CurrentDomain.GetAssemblies()).Where<Assembly>((Func<Assembly, bool>) (assembly => assembly.GetName().Name.StartsWith("TaleWorlds."))).ToArray<Assembly>();
        foreach (Assembly assembly in array)
        {
          System.Type type = ((IEnumerable<System.Type>) assembly.GetTypes()).FirstOrDefault<System.Type>((Func<System.Type, bool>) (ty => ty.Name.Contains("CompressionInfo")));
          if (type != (System.Type) null)
          {
            typeList.AddRange((IEnumerable<System.Type>) type.GetNestedTypes());
            break;
          }
        }
        List<FieldInfo> fieldInfoList = new List<FieldInfo>();
        foreach (Assembly assembly in array)
        {
          foreach (System.Type type in assembly.GetTypes())
          {
            foreach (FieldInfo field in type.GetFields())
            {
              if (typeList.Contains(field.FieldType))
                fieldInfoList.Add(field);
            }
          }
        }
        int num = 0;
        foreach (FieldInfo fieldInfo in fieldInfoList)
        {
          object obj = fieldInfo.GetValue((object) null);
          MethodInfo method = fieldInfo.FieldType.GetMethod("GetHashKey", BindingFlags.Instance | BindingFlags.NonPublic);
          num += (int) method.Invoke(obj, new object[0]);
        }
        TaleWorlds.Library.Debug.Print("CompressionInfoKey: " + (object) num, color: TaleWorlds.Library.Debug.DebugColor.Cyan, debugFilter: 17179869184UL);
      }
      catch
      {
        TaleWorlds.Library.Debug.Print("CompressionInfoKey checking failed.", color: TaleWorlds.Library.Debug.DebugColor.Cyan, debugFilter: 17179869184UL);
      }
    }

    public void OnInviteToGame(PlayerId playerId)
    {
      if (!this.LobbyClient.Connected || this.LobbyClient.IsInParty && !this.LobbyClient.IsPartyLeader || this.LobbyClient.PlayersInParty.Count >= TaleWorlds.MountAndBlade.Diamond.Parameters.MaxPlayerCountInParty)
        return;
      PlatformServices.InvitationServices?.OnInviteToGame(playerId);
    }

    public void OnInvitedPlayerOnline(PlayerId playerId)
    {
      if (!this.LobbyClient.Connected || this.LobbyClient.IsInParty && !this.LobbyClient.IsPartyLeader || this.LobbyClient.PlayersInParty.Count >= TaleWorlds.MountAndBlade.Diamond.Parameters.MaxPlayerCountInParty)
        return;
      IPlatformInvitationServices invitationServices = PlatformServices.InvitationServices;
      if ((invitationServices != null ? (invitationServices.OnInvitedPlayerOnline(playerId) ? 1 : 0) : 0) == 0)
        return;
      this.LobbyClient.InviteToParty(playerId);
    }

    public async void OnSessionInvitationByConnectionStringAccepted()
    {
      if (this.LobbyClient.IsInGame)
        return;
      string[] strArray = PlatformServices.SessionInvitationData["value"].Split(':');
      PlatformServices.OnSessionInvitationHandled();
      Guid result;
      if (!(strArray[0] == "CustomGame") || strArray.Length != 2 || !Guid.TryParse(strArray[1], out result))
        return;
      this._customGameInvitation = new CustomBattleId(result);
      if (this.LobbyClient.IsIdle)
        await this.TryLogin();
      if (!this.LobbyClient.IsInParty)
        return;
      this.LobbyClient.KickPlayerFromParty(this.LobbyClient.PlayerID);
    }

    public async void OnSessionInvitationAccepted(SessionInvitationType targetGameType)
    {
      switch (targetGameType)
      {
        case SessionInvitationType.Multiplayer:
          PlatformServices.OnSessionInvitationHandled();
          if (this.LobbyClient.IsIdle)
          {
            await this.TryLogin();
            break;
          }
          if (this.LobbyClient.IsInParty)
          {
            PlatformServices.InvitationServices?.CheckPartySession(this.LobbyClient.PlayersInParty.Select<PartyPlayerInLobbyClient, PlayerId>((Func<PartyPlayerInLobbyClient, PlayerId>) (p => p.PlayerId)), this.LobbyClient.IsPartyLeader);
            break;
          }
          if (this.Handler == null)
            break;
          IPlatformInvitationServices invitationServices = PlatformServices.InvitationServices;
          this.Handler.OnSessionInvitationAccepted(invitationServices != null ? invitationServices.GetInvitationPlayerId() : PlayerId.Empty);
          break;
        case SessionInvitationType.ConnectionString:
          this.OnSessionInvitationByConnectionStringAccepted();
          break;
      }
    }
  }
}
