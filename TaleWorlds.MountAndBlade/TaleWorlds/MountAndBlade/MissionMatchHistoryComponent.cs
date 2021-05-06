// Decompiled with JetBrains decompiler
// Type: TaleWorlds.MountAndBlade.MissionMatchHistoryComponent
// Assembly: TaleWorlds.MountAndBlade, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3C0E3F42-756F-4407-B817-06BA412A78B0
// Assembly location: E:\SteamLibrary\steamapps\common\Mount & Blade II Bannerlord\bin\Win64_Shipping_Client\TaleWorlds.MountAndBlade.dll

using NetworkMessages.FromServer;
using System;
using TaleWorlds.Core;
using TaleWorlds.MountAndBlade.Network.Messages;

namespace TaleWorlds.MountAndBlade
{
  public class MissionMatchHistoryComponent : MissionNetwork
  {
    private bool _recordedHistory;
    private MatchInfo _matchInfo;

    public MissionMatchHistoryComponent()
    {
      this._recordedHistory = false;
      this.LoadMatchhHistory();
      MatchInfo matchInfo;
      if (MatchHistory.TryGetMatchInfo(NetworkMain.GameClient.CurrentMatchId, out matchInfo))
      {
        this._matchInfo = matchInfo;
      }
      else
      {
        this._matchInfo = new MatchInfo();
        this._matchInfo.MatchId = NetworkMain.GameClient.CurrentMatchId;
      }
      this._matchInfo.MatchDate = DateTime.Now;
    }

    private async void LoadMatchhHistory() => await MatchHistory.LoadMatchHistory();

    public override void OnBehaviourInitialize()
    {
      MissionMultiplayerGameModeBaseClient missionBehaviour = Mission.Current.GetMissionBehaviour<MissionMultiplayerGameModeBaseClient>();
      this._matchInfo.GameType = (missionBehaviour != null ? missionBehaviour.GameType : MissionLobbyComponent.MultiplayerGameType.FreeForAll).ToString();
    }

    public override void AfterStart()
    {
      base.AfterStart();
      string strValue1 = MultiplayerOptions.OptionType.CultureTeam1.GetStrValue();
      string strValue2 = MultiplayerOptions.OptionType.CultureTeam2.GetStrValue();
      this._matchInfo.Faction1 = strValue1;
      this._matchInfo.Faction2 = strValue2;
      MissionPeer.OnTeamChanged += new MissionPeer.OnTeamChangedDelegate(this.TeamChange);
      this.Mission.GetMissionBehaviour<MissionLobbyComponent>();
      this._matchInfo.MatchType = BannerlordNetwork.LobbyMissionType.ToString();
    }

    protected override void AddRemoveMessageHandlers(
      GameNetwork.NetworkMessageHandlerRegistererContainer registerer)
    {
      registerer.Register<MissionStateChange>(new GameNetworkMessage.ServerMessageHandlerDelegate<MissionStateChange>(this.HandleServerEventMissionStateChange));
    }

    private void TeamChange(NetworkCommunicator player, Team oldTeam, Team nextTeam) => this._matchInfo.AddOrUpdatePlayer(player.VirtualPlayer.Id.ToString(), player.VirtualPlayer.UserName, player.ForcedAvatarIndex, nextTeam.TeamIndex - 1);

    private void HandleServerEventMissionStateChange(MissionStateChange message)
    {
      if (message.CurrentState != MissionLobbyComponent.MultiplayerGameState.Ending || this._recordedHistory)
        return;
      MissionScoreboardComponent missionBehaviour = this.Mission.GetMissionBehaviour<MissionScoreboardComponent>();
      if (missionBehaviour != null)
      {
        int roundScore1 = missionBehaviour.GetRoundScore(BattleSideEnum.Attacker);
        int roundScore2 = missionBehaviour.GetRoundScore(BattleSideEnum.Defender);
        this._matchInfo.WinnerTeam = roundScore1 > roundScore2 ? 0 : (roundScore1 == roundScore2 ? -1 : 1);
        this._matchInfo.AttackerScore = roundScore1;
        this._matchInfo.DefenderScore = roundScore2;
      }
      MatchHistory.AddMatch(this._matchInfo);
      MatchHistory.Serialize();
      this._recordedHistory = true;
    }

    public override void OnRemoveBehaviour()
    {
      if (!this._recordedHistory)
      {
        this._matchInfo.WinnerTeam = -1;
        MissionScoreboardComponent missionBehaviour = this.Mission.GetMissionBehaviour<MissionScoreboardComponent>();
        if (missionBehaviour != null)
        {
          int roundScore1 = missionBehaviour.GetRoundScore(BattleSideEnum.Attacker);
          int roundScore2 = missionBehaviour.GetRoundScore(BattleSideEnum.Defender);
          this._matchInfo.AttackerScore = roundScore1;
          this._matchInfo.DefenderScore = roundScore2;
        }
        MatchHistory.AddMatch(this._matchInfo);
        MatchHistory.Serialize();
        this._recordedHistory = true;
      }
      MissionPeer.OnTeamChanged -= new MissionPeer.OnTeamChangedDelegate(this.TeamChange);
      base.OnRemoveBehaviour();
    }
  }
}
