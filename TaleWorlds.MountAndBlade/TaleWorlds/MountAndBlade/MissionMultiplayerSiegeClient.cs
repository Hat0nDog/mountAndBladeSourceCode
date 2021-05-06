// Decompiled with JetBrains decompiler
// Type: TaleWorlds.MountAndBlade.MissionMultiplayerSiegeClient
// Assembly: TaleWorlds.MountAndBlade, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3C0E3F42-756F-4407-B817-06BA412A78B0
// Assembly location: E:\SteamLibrary\steamapps\common\Mount & Blade II Bannerlord\bin\Win64_Shipping_Client\TaleWorlds.MountAndBlade.dll

using NetworkMessages.FromServer;
using System;
using System.Collections.Generic;
using TaleWorlds.Core;
using TaleWorlds.Engine;
using TaleWorlds.Library;
using TaleWorlds.MountAndBlade.MissionRepresentatives;
using TaleWorlds.MountAndBlade.Network.Messages;
using TaleWorlds.MountAndBlade.Objects;

namespace TaleWorlds.MountAndBlade
{
  public class MissionMultiplayerSiegeClient : 
    MissionMultiplayerGameModeBaseClient,
    ICommanderInfo,
    IMissionBehavior
  {
    private const float DefenderMoraleDropThresholdIncrement = 0.2f;
    private const float DefenderMoraleDropThresholdLow = 0.4f;
    private const float DefenderMoraleDropThresholdMedium = 0.6f;
    private const float DefenderMoraleDropThresholdHigh = 0.8f;
    private const float DefenderMoraleDropMediumDuration = 8f;
    private const float DefenderMoraleDropHighDuration = 4f;
    private const float BattleWinLoseAlertThreshold = 0.25f;
    private const float BattleWinLoseLateAlertThreshold = 0.15f;
    private const string BattleWinningSoundEventString = "event:/alerts/report/battle_winning";
    private const string BattleLosingSoundEventString = "event:/alerts/report/battle_losing";
    private const float IndefiniteDurationThreshold = 8f;
    private Team[] _capturePointOwners;
    private FlagCapturePoint _masterFlag;
    private SiegeMissionRepresentative _myRepresentative;
    private SoundEvent _bellSoundEvent;
    private float _remainingTimeForBellSoundToStop = float.MinValue;
    private float _lastBellSoundPercentage = 1f;
    private bool _battleEndingNotificationGiven;
    private bool _battleEndingLateNotificationGiven;
    private Vec3 _retreatHornPosition;

    public override bool IsGameModeUsingGold => true;

    public override bool IsGameModeTactical => true;

    public override bool IsGameModeUsingRoundCountdown => true;

    public override MissionLobbyComponent.MultiplayerGameType GameType => MissionLobbyComponent.MultiplayerGameType.Siege;

    public event Action<BattleSideEnum, float> OnMoraleChangedEvent;

    public event Action OnFlagNumberChangedEvent;

    public event Action<FlagCapturePoint, Team> OnCapturePointOwnerChangedEvent;

    public event Action<TDMGoldGain> OnGoldGainEvent;

    public event Action<int[]> OnCapturePointRemainingMoraleGainsChangedEvent;

    public bool AreMoralesIndependent => true;

    public IEnumerable<FlagCapturePoint> AllCapturePoints { get; private set; }

    protected override void AddRemoveMessageHandlers(
      GameNetwork.NetworkMessageHandlerRegistererContainer registerer)
    {
      if (!GameNetwork.IsClient)
        return;
      registerer.Register<SiegeMoraleChangeMessage>(new GameNetworkMessage.ServerMessageHandlerDelegate<SiegeMoraleChangeMessage>(this.HandleMoraleChangedMessage));
      registerer.Register<SyncGoldsForSkirmish>(new GameNetworkMessage.ServerMessageHandlerDelegate<SyncGoldsForSkirmish>(this.HandleServerEventUpdateGold));
      registerer.Register<FlagDominationFlagsRemovedMessage>(new GameNetworkMessage.ServerMessageHandlerDelegate<FlagDominationFlagsRemovedMessage>(this.HandleFlagsRemovedMessage));
      registerer.Register<FlagDominationCapturePointMessage>(new GameNetworkMessage.ServerMessageHandlerDelegate<FlagDominationCapturePointMessage>(this.HandleServerEventPointCapturedMessage));
      registerer.Register<TDMGoldGain>(new GameNetworkMessage.ServerMessageHandlerDelegate<TDMGoldGain>(this.HandleServerEventTDMGoldGain));
    }

    public override void OnBehaviourInitialize()
    {
      base.OnBehaviourInitialize();
      NetworkCommunicator.OnPeerComponentAdded += new Action<PeerComponent>(this.OnPeerComponentAdded);
      this._capturePointOwners = new Team[7];
      this.AllCapturePoints = Mission.Current.MissionObjects.FindAllWithType<FlagCapturePoint>();
    }

    public override void AfterStart()
    {
      this.Mission.SetMissionMode(MissionMode.Battle, true);
      foreach (FlagCapturePoint allCapturePoint in this.AllCapturePoints)
      {
        if (allCapturePoint.GameEntity.HasTag("keep_capture_point"))
          this._masterFlag = allCapturePoint;
        else if (allCapturePoint.FlagIndex == 0)
        {
          MatrixFrame globalFrame = allCapturePoint.GameEntity.GetGlobalFrame();
          this._retreatHornPosition = globalFrame.origin + globalFrame.rotation.u * 3f;
        }
      }
    }

    private void OnPeerComponentAdded(PeerComponent component)
    {
      if (!component.IsMine || !(component is MissionRepresentativeBase))
        return;
      this._myRepresentative = component as SiegeMissionRepresentative;
    }

    public override int GetGoldAmount() => this._myRepresentative.Gold;

    public override void OnGoldAmountChangedForRepresentative(
      MissionRepresentativeBase representative,
      int goldAmount)
    {
      if (representative == null || this.MissionLobbyComponent.CurrentMultiplayerState == MissionLobbyComponent.MultiplayerGameState.Ending)
        return;
      representative.UpdateGold(goldAmount);
      this.ScoreboardComponent.PlayerPropertiesChanged(representative.MissionPeer);
    }

    public void OnNumberOfFlagsChanged()
    {
      Action numberChangedEvent = this.OnFlagNumberChangedEvent;
      if (numberChangedEvent != null)
        numberChangedEvent();
      SiegeMissionRepresentative representative = this._myRepresentative;
      int num;
      if (representative == null)
      {
        num = 0;
      }
      else
      {
        BattleSideEnum? side = representative.MissionPeer.Team?.Side;
        BattleSideEnum battleSideEnum = BattleSideEnum.Attacker;
        num = side.GetValueOrDefault() == battleSideEnum & side.HasValue ? 1 : 0;
      }
      if (num == 0)
        return;
      Action<TDMGoldGain> onGoldGainEvent = this.OnGoldGainEvent;
      if (onGoldGainEvent == null)
        return;
      onGoldGainEvent(new TDMGoldGain(new List<KeyValuePair<ushort, int>>()
      {
        new KeyValuePair<ushort, int>((ushort) 512, 35)
      }));
    }

    public void OnCapturePointOwnerChanged(FlagCapturePoint flagCapturePoint, Team ownerTeam)
    {
      this._capturePointOwners[flagCapturePoint.FlagIndex] = ownerTeam;
      Action<FlagCapturePoint, Team> ownerChangedEvent = this.OnCapturePointOwnerChangedEvent;
      if (ownerChangedEvent != null)
        ownerChangedEvent(flagCapturePoint, ownerTeam);
      if (ownerTeam != null && ownerTeam.Side == BattleSideEnum.Defender && ((double) this._remainingTimeForBellSoundToStop > 8.0 && flagCapturePoint == this._masterFlag))
      {
        this._bellSoundEvent.Stop();
        this._bellSoundEvent = (SoundEvent) null;
        this._remainingTimeForBellSoundToStop = float.MinValue;
        this._lastBellSoundPercentage += 0.2f;
      }
      if (this._myRepresentative == null || this._myRepresentative.MissionPeer.Team == null)
        return;
      MatrixFrame cameraFrame = Mission.Current.GetCameraFrame();
      Vec3 position = cameraFrame.origin + cameraFrame.rotation.u;
      if (this._myRepresentative.MissionPeer.Team == ownerTeam)
        MBSoundEvent.PlaySound(SoundEvent.GetEventIdFromString("event:/alerts/report/flag_captured"), position);
      else
        MBSoundEvent.PlaySound(SoundEvent.GetEventIdFromString("event:/alerts/report/flag_lost"), position);
    }

    public void OnMoraleChanged(
      int attackerMorale,
      int defenderMorale,
      int[] capturePointRemainingMoraleGains)
    {
      float num1 = (float) attackerMorale / 360f;
      float num2 = (float) defenderMorale / 360f;
      if (this._myRepresentative?.MissionPeer.Team != null && this._myRepresentative.MissionPeer.Team.Side != BattleSideEnum.None)
      {
        if ((this._capturePointOwners[this._masterFlag.FlagIndex] == null || this._capturePointOwners[this._masterFlag.FlagIndex].Side != BattleSideEnum.Defender) && (double) this._remainingTimeForBellSoundToStop < 0.0)
        {
          if ((double) num2 > (double) this._lastBellSoundPercentage)
            this._lastBellSoundPercentage += 0.2f;
          if ((double) num2 <= 0.400000005960464)
          {
            if ((double) this._lastBellSoundPercentage > 0.400000005960464)
            {
              this._remainingTimeForBellSoundToStop = float.MaxValue;
              this._lastBellSoundPercentage = 0.4f;
            }
          }
          else if ((double) num2 <= 0.600000023841858)
          {
            if ((double) this._lastBellSoundPercentage > 0.600000023841858)
            {
              this._remainingTimeForBellSoundToStop = 8f;
              this._lastBellSoundPercentage = 0.6f;
            }
          }
          else if ((double) num2 <= 0.800000011920929 && (double) this._lastBellSoundPercentage > 0.800000011920929)
          {
            this._remainingTimeForBellSoundToStop = 4f;
            this._lastBellSoundPercentage = 0.8f;
          }
          if ((double) this._remainingTimeForBellSoundToStop > 0.0)
          {
            switch (this._myRepresentative.MissionPeer.Team.Side)
            {
              case BattleSideEnum.Defender:
                this._bellSoundEvent = SoundEvent.CreateEventFromString("event:/multiplayer/warning_bells_defender", this.Mission.Scene);
                break;
              case BattleSideEnum.Attacker:
                this._bellSoundEvent = SoundEvent.CreateEventFromString("event:/multiplayer/warning_bells_attacker", this.Mission.Scene);
                break;
            }
            MatrixFrame globalFrame = this._masterFlag.GameEntity.GetGlobalFrame();
            this._bellSoundEvent.PlayInPosition(globalFrame.origin + globalFrame.rotation.u * 3f);
          }
        }
        if (!this._battleEndingNotificationGiven || !this._battleEndingLateNotificationGiven)
        {
          float num3 = !this._battleEndingNotificationGiven ? 0.25f : 0.15f;
          MatrixFrame cameraFrame = Mission.Current.GetCameraFrame();
          Vec3 position = cameraFrame.origin + cameraFrame.rotation.u;
          if ((double) num1 <= (double) num3 && (double) num2 > (double) num3)
          {
            MBSoundEvent.PlaySound(SoundEvent.GetEventIdFromString(this._myRepresentative.MissionPeer.Team.Side == BattleSideEnum.Attacker ? "event:/alerts/report/battle_losing" : "event:/alerts/report/battle_winning"), position);
            if (this._myRepresentative.MissionPeer.Team.Side == BattleSideEnum.Attacker)
              MBSoundEvent.PlaySound(SoundEvent.GetEventIdFromString("event:/multiplayer/retreat_horn_attacker"), this._retreatHornPosition);
            else if (this._myRepresentative.MissionPeer.Team.Side == BattleSideEnum.Defender)
              MBSoundEvent.PlaySound(SoundEvent.GetEventIdFromString("event:/multiplayer/retreat_horn_defender"), this._retreatHornPosition);
            if (this._battleEndingNotificationGiven)
              this._battleEndingLateNotificationGiven = true;
            this._battleEndingNotificationGiven = true;
          }
          if ((double) num2 <= (double) num3 && (double) num1 > (double) num3)
          {
            MBSoundEvent.PlaySound(SoundEvent.GetEventIdFromString(this._myRepresentative.MissionPeer.Team.Side == BattleSideEnum.Defender ? "event:/alerts/report/battle_losing" : "event:/alerts/report/battle_winning"), position);
            if (this._battleEndingNotificationGiven)
              this._battleEndingLateNotificationGiven = true;
            this._battleEndingNotificationGiven = true;
          }
        }
      }
      Action<BattleSideEnum, float> moraleChangedEvent1 = this.OnMoraleChangedEvent;
      if (moraleChangedEvent1 != null)
        moraleChangedEvent1(BattleSideEnum.Attacker, num1);
      Action<BattleSideEnum, float> moraleChangedEvent2 = this.OnMoraleChangedEvent;
      if (moraleChangedEvent2 != null)
        moraleChangedEvent2(BattleSideEnum.Defender, num2);
      Action<int[]> gainsChangedEvent = this.OnCapturePointRemainingMoraleGainsChangedEvent;
      if (gainsChangedEvent == null)
        return;
      gainsChangedEvent(capturePointRemainingMoraleGains);
    }

    public Team GetFlagOwner(FlagCapturePoint flag) => this._capturePointOwners[flag.FlagIndex];

    public override void OnRemoveBehaviour()
    {
      NetworkCommunicator.OnPeerComponentAdded -= new Action<PeerComponent>(this.OnPeerComponentAdded);
      base.OnRemoveBehaviour();
    }

    public override void OnMissionTick(float dt)
    {
      base.OnMissionTick(dt);
      if ((double) this._remainingTimeForBellSoundToStop <= 0.0)
        return;
      this._remainingTimeForBellSoundToStop -= dt;
      if ((double) this._remainingTimeForBellSoundToStop > 0.0 && this.MissionLobbyComponent.CurrentMultiplayerState == MissionLobbyComponent.MultiplayerGameState.Playing)
        return;
      this._remainingTimeForBellSoundToStop = float.MinValue;
      this._bellSoundEvent.Stop();
      this._bellSoundEvent = (SoundEvent) null;
    }

    private void HandleMoraleChangedMessage(SiegeMoraleChangeMessage message) => this.OnMoraleChanged(message.AttackerMorale, message.DefenderMorale, message.CapturePointRemainingMoraleGains);

    private void HandleServerEventUpdateGold(SyncGoldsForSkirmish message) => this.OnGoldAmountChangedForRepresentative((MissionRepresentativeBase) message.VirtualPlayer.GetComponent<SiegeMissionRepresentative>(), message.GoldAmount);

    private void HandleFlagsRemovedMessage(FlagDominationFlagsRemovedMessage message) => this.OnNumberOfFlagsChanged();

    private void HandleServerEventPointCapturedMessage(FlagDominationCapturePointMessage message)
    {
      foreach (FlagCapturePoint allCapturePoint in this.AllCapturePoints)
      {
        if (allCapturePoint.FlagIndex == message.FlagIndex)
        {
          this.OnCapturePointOwnerChanged(allCapturePoint, message.OwnerTeam);
          break;
        }
      }
    }

    private void HandleServerEventTDMGoldGain(TDMGoldGain message)
    {
      Action<TDMGoldGain> onGoldGainEvent = this.OnGoldGainEvent;
      if (onGoldGainEvent == null)
        return;
      onGoldGainEvent(message);
    }
  }
}
