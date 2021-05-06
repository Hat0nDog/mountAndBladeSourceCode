// Decompiled with JetBrains decompiler
// Type: TaleWorlds.MountAndBlade.MissionMultiplayerTeamDeathmatchClient
// Assembly: TaleWorlds.MountAndBlade, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3C0E3F42-756F-4407-B817-06BA412A78B0
// Assembly location: E:\SteamLibrary\steamapps\common\Mount & Blade II Bannerlord\bin\Win64_Shipping_Client\TaleWorlds.MountAndBlade.dll

using NetworkMessages.FromServer;
using System;
using TaleWorlds.Core;
using TaleWorlds.Engine;
using TaleWorlds.Library;
using TaleWorlds.MountAndBlade.MissionRepresentatives;
using TaleWorlds.MountAndBlade.Network.Messages;

namespace TaleWorlds.MountAndBlade
{
  public class MissionMultiplayerTeamDeathmatchClient : MissionMultiplayerGameModeBaseClient
  {
    private const string BattleWinningSoundEventString = "event:/alerts/report/battle_winning";
    private const string BattleLosingSoundEventString = "event:/alerts/report/battle_losing";
    private const float BattleWinLoseAlertThreshold = 0.1f;
    private TeamDeathmatchMissionRepresentative _myRepresentative;
    private bool _battleEndingNotificationGiven;

    public event Action<TDMGoldGain> OnGoldGainEvent;

    public override bool IsGameModeUsingGold => true;

    public override bool IsGameModeTactical => false;

    public override bool IsGameModeUsingRoundCountdown => true;

    public override MissionLobbyComponent.MultiplayerGameType GameType => MissionLobbyComponent.MultiplayerGameType.TeamDeathmatch;

    public override void OnBehaviourInitialize()
    {
      base.OnBehaviourInitialize();
      NetworkCommunicator.OnPeerComponentAdded += new Action<PeerComponent>(this.OnPeerComponentAdded);
      this.ScoreboardComponent.OnRoundPropertiesChanged += new Action(this.OnTeamScoresChanged);
    }

    public override void OnGoldAmountChangedForRepresentative(
      MissionRepresentativeBase representative,
      int goldAmount)
    {
      if (representative == null || this.MissionLobbyComponent.CurrentMultiplayerState == MissionLobbyComponent.MultiplayerGameState.Ending)
        return;
      representative.UpdateGold(goldAmount);
      this.ScoreboardComponent.PlayerPropertiesChanged(representative.MissionPeer);
    }

    public override void AfterStart() => this.Mission.SetMissionMode(MissionMode.Battle, true);

    protected override void AddRemoveMessageHandlers(
      GameNetwork.NetworkMessageHandlerRegistererContainer registerer)
    {
      if (!GameNetwork.IsClient)
        return;
      registerer.Register<SyncGoldsForSkirmish>(new GameNetworkMessage.ServerMessageHandlerDelegate<SyncGoldsForSkirmish>(this.HandleServerEventUpdateGold));
      registerer.Register<TDMGoldGain>(new GameNetworkMessage.ServerMessageHandlerDelegate<TDMGoldGain>(this.HandleServerEventTDMGoldGain));
    }

    private void OnPeerComponentAdded(PeerComponent component)
    {
      if (!component.IsMine || !(component is MissionRepresentativeBase))
        return;
      this._myRepresentative = component as TeamDeathmatchMissionRepresentative;
    }

    private void HandleServerEventUpdateGold(SyncGoldsForSkirmish message) => this.OnGoldAmountChangedForRepresentative(message.VirtualPlayer.GetComponent<MissionRepresentativeBase>(), message.GoldAmount);

    private void HandleServerEventTDMGoldGain(TDMGoldGain message)
    {
      Action<TDMGoldGain> onGoldGainEvent = this.OnGoldGainEvent;
      if (onGoldGainEvent == null)
        return;
      onGoldGainEvent(message);
    }

    public override int GetGoldAmount() => this._myRepresentative.Gold;

    public override void OnRemoveBehaviour()
    {
      NetworkCommunicator.OnPeerComponentAdded -= new Action<PeerComponent>(this.OnPeerComponentAdded);
      this.ScoreboardComponent.OnRoundPropertiesChanged -= new Action(this.OnTeamScoresChanged);
      base.OnRemoveBehaviour();
    }

    private void OnTeamScoresChanged()
    {
      if (GameNetwork.IsDedicatedServer || this._battleEndingNotificationGiven || (this._myRepresentative.MissionPeer.Team == null || this._myRepresentative.MissionPeer.Team.Side == BattleSideEnum.None))
        return;
      int intValue = MultiplayerOptions.OptionType.MinScoreToWinMatch.GetIntValue();
      float num1 = (float) (intValue - this.ScoreboardComponent.GetRoundScore(this._myRepresentative.MissionPeer.Team.Side)) / (float) intValue;
      float num2 = (float) (intValue - this.ScoreboardComponent.GetRoundScore(this._myRepresentative.MissionPeer.Team.Side.GetOppositeSide())) / (float) intValue;
      MatrixFrame cameraFrame = Mission.Current.GetCameraFrame();
      Vec3 position = cameraFrame.origin + cameraFrame.rotation.u;
      if ((double) num1 <= 0.100000001490116 && (double) num2 > 0.100000001490116)
      {
        MBSoundEvent.PlaySound(SoundEvent.GetEventIdFromString("event:/alerts/report/battle_winning"), position);
        this._battleEndingNotificationGiven = true;
      }
      if ((double) num2 > 0.100000001490116 || (double) num1 <= 0.100000001490116)
        return;
      MBSoundEvent.PlaySound(SoundEvent.GetEventIdFromString("event:/alerts/report/battle_losing"), position);
      this._battleEndingNotificationGiven = true;
    }
  }
}
