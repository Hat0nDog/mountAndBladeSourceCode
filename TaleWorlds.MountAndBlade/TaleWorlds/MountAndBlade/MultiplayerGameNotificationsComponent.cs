// Decompiled with JetBrains decompiler
// Type: TaleWorlds.MountAndBlade.MultiplayerGameNotificationsComponent
// Assembly: TaleWorlds.MountAndBlade, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3C0E3F42-756F-4407-B817-06BA412A78B0
// Assembly location: E:\SteamLibrary\steamapps\common\Mount & Blade II Bannerlord\bin\Win64_Shipping_Client\TaleWorlds.MountAndBlade.dll

using NetworkMessages.FromServer;
using System;
using System.Collections.Generic;
using System.Linq;
using TaleWorlds.Core;
using TaleWorlds.Localization;
using TaleWorlds.MountAndBlade.Network.Messages;
using TaleWorlds.MountAndBlade.Objects;

namespace TaleWorlds.MountAndBlade
{
  public class MultiplayerGameNotificationsComponent : MissionNetwork
  {
    public static int NotificationCount => 12;

    public void WarmupEnding() => this.HandleNewNotification(MultiplayerGameNotificationsComponent.MultiplayerNotificationEnum.BattleWarmupEnding, 30);

    public void GameOver(Team winnerTeam)
    {
      if (winnerTeam == null)
      {
        this.HandleNewNotification(MultiplayerGameNotificationsComponent.MultiplayerNotificationEnum.GameOverDraw);
      }
      else
      {
        Team syncToTeam = winnerTeam.Side == BattleSideEnum.Attacker ? this.Mission.Teams.Defender : this.Mission.Teams.Attacker;
        this.HandleNewNotification(MultiplayerGameNotificationsComponent.MultiplayerNotificationEnum.GameOverVictory, syncToTeam: winnerTeam);
        this.HandleNewNotification(MultiplayerGameNotificationsComponent.MultiplayerNotificationEnum.GameOverDefeat, syncToTeam: syncToTeam);
      }
    }

    public void PreparationStarted() => this.HandleNewNotification(MultiplayerGameNotificationsComponent.MultiplayerNotificationEnum.BattlePreparationStart);

    public void FlagsXRemoved(FlagCapturePoint removedFlag) => this.HandleNewNotification(MultiplayerGameNotificationsComponent.MultiplayerNotificationEnum.FlagXRemoved, removedFlag.FlagChar);

    public void FlagXRemaining(FlagCapturePoint remainingFlag) => this.HandleNewNotification(MultiplayerGameNotificationsComponent.MultiplayerNotificationEnum.FlagXRemaining, remainingFlag.FlagChar);

    public void FlagsWillBeRemovedInXSeconds(int timeLeft) => this.ShowNotification(MultiplayerGameNotificationsComponent.MultiplayerNotificationEnum.FlagsWillBeRemoved, timeLeft);

    public void FlagXCapturedByTeamX(SynchedMissionObject flag, Team capturingTeam)
    {
      int num = flag is FlagCapturePoint flagCapturePoint ? flagCapturePoint.FlagChar : 65;
      Team syncToTeam = capturingTeam.Side == BattleSideEnum.Attacker ? this.Mission.Teams.Defender : this.Mission.Teams.Attacker;
      this.HandleNewNotification(MultiplayerGameNotificationsComponent.MultiplayerNotificationEnum.FlagXCapturedByYourTeam, num, syncToTeam: capturingTeam);
      this.HandleNewNotification(MultiplayerGameNotificationsComponent.MultiplayerNotificationEnum.FlagXCapturedByOtherTeam, num, syncToTeam: syncToTeam);
    }

    public void GoldCarriedFromPreviousRound(int carriedGoldAmount, NetworkCommunicator syncToPeer) => this.HandleNewNotification(MultiplayerGameNotificationsComponent.MultiplayerNotificationEnum.GoldCarriedFromPreviousRound, carriedGoldAmount, syncToPeer: syncToPeer);

    public void PlayerIsInactive(NetworkCommunicator peer) => this.HandleNewNotification(MultiplayerGameNotificationsComponent.MultiplayerNotificationEnum.PlayerIsInactive, syncToPeer: peer);

    private void HandleNewNotification(
      MultiplayerGameNotificationsComponent.MultiplayerNotificationEnum notification,
      int param1 = -1,
      int param2 = -1,
      Team syncToTeam = null,
      NetworkCommunicator syncToPeer = null)
    {
      if (syncToPeer != null)
        this.SendNotificationToPeer(syncToPeer, notification, param1, param2);
      else if (syncToTeam != null)
        this.SendNotificationToTeam(syncToTeam, notification, param1, param2);
      else
        this.SendNotificationToEveryone(notification, param1, param2);
    }

    private void ShowNotification(
      MultiplayerGameNotificationsComponent.MultiplayerNotificationEnum notification,
      params int[] parameters)
    {
      if (GameNetwork.IsDedicatedServer)
        return;
      NotificationProperty attribute = (NotificationProperty) ((IEnumerable<object>) notification.GetType().GetField(notification.ToString()).GetCustomAttributes(typeof (NotificationProperty), false)).Single<object>();
      if (attribute == null)
        return;
      int[] array = ((IEnumerable<int>) parameters).Where<int>((Func<int, bool>) (x => x != -1)).ToArray<int>();
      InformationManager.AddQuickInformation(this.ToNotificationString(notification, attribute, array), soundEventPath: this.ToSoundString(notification, attribute, array));
    }

    private void SendNotificationToEveryone(
      MultiplayerGameNotificationsComponent.MultiplayerNotificationEnum message,
      int param1 = -1,
      int param2 = -1)
    {
      this.ShowNotification(message, param1, param2);
      GameNetwork.BeginBroadcastModuleEvent();
      GameNetwork.WriteMessage((GameNetworkMessage) new NotificationMessage((int) message, param1, param2));
      GameNetwork.EndBroadcastModuleEvent(GameNetwork.EventBroadcastFlags.None);
    }

    private void SendNotificationToPeer(
      NetworkCommunicator peer,
      MultiplayerGameNotificationsComponent.MultiplayerNotificationEnum message,
      int param1 = -1,
      int param2 = -1)
    {
      if (peer.IsServerPeer)
      {
        this.ShowNotification(message, param1, param2);
      }
      else
      {
        GameNetwork.BeginModuleEventAsServer(peer);
        GameNetwork.WriteMessage((GameNetworkMessage) new NotificationMessage((int) message, param1, param2));
        GameNetwork.EndModuleEventAsServer();
      }
    }

    private void SendNotificationToTeam(
      Team team,
      MultiplayerGameNotificationsComponent.MultiplayerNotificationEnum message,
      int param1 = -1,
      int param2 = -1)
    {
      NetworkCommunicator myPeer = GameNetwork.MyPeer;
      MissionPeer missionPeer = myPeer != null ? myPeer.GetComponent<MissionPeer>() : (MissionPeer) null;
      if (!GameNetwork.IsDedicatedServer && (missionPeer?.Team != null && missionPeer.Team.IsEnemyOf(team)))
        this.ShowNotification(message, param1, param2);
      foreach (MissionPeer peer in VirtualPlayer.Peers<MissionPeer>())
      {
        if (peer.Team != null && !peer.IsMine && !peer.Team.IsEnemyOf(team))
        {
          GameNetwork.BeginModuleEventAsServer(peer.Peer);
          GameNetwork.WriteMessage((GameNetworkMessage) new NotificationMessage((int) message, param1, param2));
          GameNetwork.EndModuleEventAsServer();
        }
      }
    }

    private string ToSoundString(
      MultiplayerGameNotificationsComponent.MultiplayerNotificationEnum value,
      NotificationProperty attribute,
      params int[] parameters)
    {
      string str = string.Empty;
      if (attribute.SoundIdTwo.IsStringNoneOrEmpty())
      {
        str = attribute.SoundIdOne;
      }
      else
      {
        switch (value)
        {
          case MultiplayerGameNotificationsComponent.MultiplayerNotificationEnum.BattleYouHaveXTheRound:
            Team team1 = parameters[0] == 0 ? Mission.Current.AttackerTeam : Mission.Current.DefenderTeam;
            Team team2 = GameNetwork.IsMyPeerReady ? GameNetwork.MyPeer.GetComponent<MissionPeer>().Team : (Team) null;
            str = attribute.SoundIdOne;
            if (team2 != null && team2 != team1)
            {
              str = attribute.SoundIdTwo;
              break;
            }
            break;
          case MultiplayerGameNotificationsComponent.MultiplayerNotificationEnum.FlagXCapturedByYourTeam:
            str = attribute.SoundIdOne;
            break;
          case MultiplayerGameNotificationsComponent.MultiplayerNotificationEnum.FlagXCapturedByOtherTeam:
            str = attribute.SoundIdTwo;
            break;
        }
      }
      return str;
    }

    private TextObject ToNotificationString(
      MultiplayerGameNotificationsComponent.MultiplayerNotificationEnum value,
      NotificationProperty attribute,
      params int[] parameters)
    {
      if (parameters.Length != 0)
        this.SetGameTextVariables(value, parameters);
      return GameTexts.FindText(attribute.StringId);
    }

    private void SetGameTextVariables(
      MultiplayerGameNotificationsComponent.MultiplayerNotificationEnum message,
      params int[] parameters)
    {
      if (parameters.Length == 0)
        return;
      switch (message)
      {
        case MultiplayerGameNotificationsComponent.MultiplayerNotificationEnum.BattleWarmupEnding:
          GameTexts.SetVariable("SECONDS_LEFT", parameters[0]);
          break;
        case MultiplayerGameNotificationsComponent.MultiplayerNotificationEnum.BattleYouHaveXTheRound:
          Team team1 = parameters[0] == 0 ? Mission.Current.AttackerTeam : Mission.Current.DefenderTeam;
          Team team2 = GameNetwork.IsMyPeerReady ? GameNetwork.MyPeer.GetComponent<MissionPeer>().Team : (Team) null;
          if (team2 == null)
            break;
          GameTexts.SetVariable("IS_WINNER", team2 == team1 ? 1 : 0);
          break;
        case MultiplayerGameNotificationsComponent.MultiplayerNotificationEnum.FlagXRemoved:
          GameTexts.SetVariable("PARAM1", ((char) parameters[0]).ToString());
          break;
        case MultiplayerGameNotificationsComponent.MultiplayerNotificationEnum.FlagXRemaining:
          GameTexts.SetVariable("PARAM1", ((char) parameters[0]).ToString());
          break;
        case MultiplayerGameNotificationsComponent.MultiplayerNotificationEnum.FlagsWillBeRemoved:
          GameTexts.SetVariable("PARAM1", parameters[0]);
          break;
        case MultiplayerGameNotificationsComponent.MultiplayerNotificationEnum.FlagXCapturedByYourTeam:
        case MultiplayerGameNotificationsComponent.MultiplayerNotificationEnum.FlagXCapturedByOtherTeam:
          GameTexts.SetVariable("PARAM1", ((char) parameters[0]).ToString());
          break;
        case MultiplayerGameNotificationsComponent.MultiplayerNotificationEnum.GoldCarriedFromPreviousRound:
          GameTexts.SetVariable("PARAM1", parameters[0].ToString());
          break;
      }
    }

    protected override void AddRemoveMessageHandlers(
      GameNetwork.NetworkMessageHandlerRegistererContainer registerer)
    {
      if (!GameNetwork.IsClient)
        return;
      registerer.Register<NotificationMessage>(new GameNetworkMessage.ServerMessageHandlerDelegate<NotificationMessage>(this.HandleServerEventServerMessage));
    }

    private void HandleServerEventServerMessage(NotificationMessage message) => this.ShowNotification((MultiplayerGameNotificationsComponent.MultiplayerNotificationEnum) message.Message, message.ParameterOne, message.ParameterTwo);

    protected override void HandleNewClientConnect(PlayerConnectionInfo clientConnectionInfo)
    {
      int num = clientConnectionInfo.NetworkPeer.IsServerPeer ? 1 : 0;
    }

    protected override void HandlePlayerDisconnect(NetworkCommunicator networkPeer)
    {
      int num = GameNetwork.IsServer ? 1 : 0;
    }

    private enum MultiplayerNotificationEnum
    {
      [NotificationProperty("str_battle_warmup_ending_in_x_seconds", "event:/ui/mission/multiplayer/lastmanstanding", "")] BattleWarmupEnding,
      [NotificationProperty("str_battle_preparation_start", "event:/ui/mission/multiplayer/roundstart", "")] BattlePreparationStart,
      [NotificationProperty("str_round_result_win_lose", "event:/ui/mission/multiplayer/victory", "event:/ui/mission/multiplayer/defeat")] BattleYouHaveXTheRound,
      [NotificationProperty("str_mp_mission_game_over_draw", "", "")] GameOverDraw,
      [NotificationProperty("str_mp_mission_game_over_victory", "", "")] GameOverVictory,
      [NotificationProperty("str_mp_mission_game_over_defeat", "", "")] GameOverDefeat,
      [NotificationProperty("str_mp_flag_removed", "event:/ui/mission/multiplayer/pointsremoved", "")] FlagXRemoved,
      [NotificationProperty("str_sergeant_a_one_flag_remaining", "event:/ui/mission/multiplayer/pointsremoved", "")] FlagXRemaining,
      [NotificationProperty("str_sergeant_a_flags_will_be_removed", "event:/ui/mission/multiplayer/pointwarning", "")] FlagsWillBeRemoved,
      [NotificationProperty("str_sergeant_a_flag_captured_by_your_team", "event:/ui/mission/multiplayer/pointcapture", "event:/ui/mission/multiplayer/pointlost")] FlagXCapturedByYourTeam,
      [NotificationProperty("str_sergeant_a_flag_captured_by_other_team", "event:/ui/mission/multiplayer/pointcapture", "event:/ui/mission/multiplayer/pointlost")] FlagXCapturedByOtherTeam,
      [NotificationProperty("str_gold_carried_from_previous_round", "", "")] GoldCarriedFromPreviousRound,
      [NotificationProperty("str_player_is_inactive", "", "")] PlayerIsInactive,
      Count,
    }
  }
}
