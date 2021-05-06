// Decompiled with JetBrains decompiler
// Type: TaleWorlds.MountAndBlade.MultiplayerWarmupComponent
// Assembly: TaleWorlds.MountAndBlade, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3C0E3F42-756F-4407-B817-06BA412A78B0
// Assembly location: E:\SteamLibrary\steamapps\common\Mount & Blade II Bannerlord\bin\Win64_Shipping_Client\TaleWorlds.MountAndBlade.dll

using NetworkMessages.FromServer;
using System;
using TaleWorlds.Core;
using TaleWorlds.Engine;
using TaleWorlds.Library;
using TaleWorlds.MountAndBlade.Network.Messages;

namespace TaleWorlds.MountAndBlade
{
  public class MultiplayerWarmupComponent : MissionNetwork
  {
    public const int RespawnPeriodInWarmup = 3;
    public const int WarmupEndWaitTime = 30;
    private MissionMultiplayerGameModeBase _gameMode;
    private MultiplayerTimerComponent _timerComponent;
    private MissionTime _currentStateStartTime;
    private MultiplayerWarmupComponent.WarmupStates _warmupState;

    public static float TotalWarmupDuration => (float) (MultiplayerOptions.OptionType.WarmupTimeLimit.GetIntValue() * 60);

    public event Action OnWarmupEnding;

    public event Action OnWarmupEnded;

    public bool IsInWarmup => this.WarmupState != MultiplayerWarmupComponent.WarmupStates.Ended;

    private MultiplayerWarmupComponent.WarmupStates WarmupState
    {
      get => this._warmupState;
      set
      {
        this._warmupState = value;
        if (!GameNetwork.IsServer)
          return;
        this._currentStateStartTime = MissionTime.Now;
        GameNetwork.BeginBroadcastModuleEvent();
        GameNetwork.WriteMessage((GameNetworkMessage) new WarmupStateChange(this._warmupState, this._currentStateStartTime.NumberOfTicks));
        GameNetwork.EndBroadcastModuleEvent(GameNetwork.EventBroadcastFlags.None);
      }
    }

    public override void OnBehaviourInitialize()
    {
      base.OnBehaviourInitialize();
      this._gameMode = this.Mission.GetMissionBehaviour<MissionMultiplayerGameModeBase>();
      this._timerComponent = this.Mission.GetMissionBehaviour<MultiplayerTimerComponent>();
    }

    public override void AfterStart()
    {
      base.AfterStart();
      GameNetwork.AddNetworkHandler((IUdpNetworkHandler) this);
      this.AddRemoveMessageHandlers(GameNetwork.NetworkMessageHandlerRegisterer.RegisterMode.Add);
    }

    protected override void OnUdpNetworkHandlerClose() => this.AddRemoveMessageHandlers(GameNetwork.NetworkMessageHandlerRegisterer.RegisterMode.Remove);

    private void AddRemoveMessageHandlers(
      GameNetwork.NetworkMessageHandlerRegisterer.RegisterMode mode)
    {
      GameNetwork.NetworkMessageHandlerRegisterer handlerRegisterer = new GameNetwork.NetworkMessageHandlerRegisterer(mode);
      if (!GameNetwork.IsClient)
        return;
      handlerRegisterer.Register<WarmupStateChange>(new GameNetworkMessage.ServerMessageHandlerDelegate<WarmupStateChange>(this.HandleServerEventWarmupStateChange));
    }

    public bool CheckForWarmupProgressEnd() => this._gameMode.CheckForWarmupEnd() || (double) this._timerComponent.GetRemainingTime(false) <= 30.0;

    public override void OnPreDisplayMissionTick(float dt)
    {
      if (!GameNetwork.IsServer)
        return;
      switch (this.WarmupState)
      {
        case MultiplayerWarmupComponent.WarmupStates.WaitingForPlayers:
          this.BeginWarmup();
          break;
        case MultiplayerWarmupComponent.WarmupStates.InProgress:
          if (!this.CheckForWarmupProgressEnd())
            break;
          this.EndWarmupProgress();
          break;
        case MultiplayerWarmupComponent.WarmupStates.Ending:
          if (!this._timerComponent.CheckIfTimerPassed())
            break;
          this.EndWarmup();
          break;
        case MultiplayerWarmupComponent.WarmupStates.Ended:
          if (!this._timerComponent.CheckIfTimerPassed())
            break;
          this.Mission.RemoveMissionBehaviour((MissionBehaviour) this);
          break;
        default:
          throw new ArgumentOutOfRangeException();
      }
    }

    private void BeginWarmup()
    {
      this.WarmupState = MultiplayerWarmupComponent.WarmupStates.InProgress;
      Mission.Current.ResetMission();
      Mission.Current.BalanceTeams();
      this._timerComponent.StartTimerAsServer(MultiplayerWarmupComponent.TotalWarmupDuration);
      this._gameMode.SpawnComponent.SpawningBehaviour.Clear();
      SpawnComponent.SetWarmupSpawningBehaviour();
    }

    private void EndWarmupProgress()
    {
      this.WarmupState = MultiplayerWarmupComponent.WarmupStates.Ending;
      this._timerComponent.StartTimerAsServer(30f);
      Action onWarmupEnding = this.OnWarmupEnding;
      if (onWarmupEnding == null)
        return;
      onWarmupEnding();
    }

    private void EndWarmup()
    {
      this.WarmupState = MultiplayerWarmupComponent.WarmupStates.Ended;
      this._timerComponent.StartTimerAsServer(3f);
      Action onWarmupEnded = this.OnWarmupEnded;
      if (onWarmupEnded != null)
        onWarmupEnded();
      if (!GameNetwork.IsDedicatedServer)
        this.PlayBattleStartingSound();
      Mission.Current.ResetMission();
      Mission.Current.BalanceTeams();
      this._gameMode.SpawnComponent.SpawningBehaviour.Clear();
      SpawnComponent.SetSpawningBehaviorForCurrentGameType(this._gameMode.GetMissionType());
    }

    public override void OnRemoveBehaviour()
    {
      base.OnRemoveBehaviour();
      this.OnWarmupEnding = (Action) null;
      this.OnWarmupEnded = (Action) null;
      if (!GameNetwork.IsServer || this._gameMode.UseRoundController())
        return;
      this._gameMode.SpawnComponent.SpawningBehaviour.RequestStartSpawnSession();
    }

    protected override void HandleNewClientAfterSynchronized(NetworkCommunicator networkPeer)
    {
      if (!this.IsInWarmup || networkPeer.IsServerPeer)
        return;
      GameNetwork.BeginModuleEventAsServer(networkPeer);
      GameNetwork.WriteMessage((GameNetworkMessage) new WarmupStateChange(this._warmupState, this._currentStateStartTime.NumberOfTicks));
      GameNetwork.EndModuleEventAsServer();
    }

    private void HandleServerEventWarmupStateChange(WarmupStateChange message)
    {
      this.WarmupState = message.WarmupState;
      switch (this.WarmupState)
      {
        case MultiplayerWarmupComponent.WarmupStates.InProgress:
          this._timerComponent.StartTimerAsClient(message.StateStartTimeInSeconds, MultiplayerWarmupComponent.TotalWarmupDuration);
          break;
        case MultiplayerWarmupComponent.WarmupStates.Ending:
          this._timerComponent.StartTimerAsClient(message.StateStartTimeInSeconds, 30f);
          Action onWarmupEnding = this.OnWarmupEnding;
          if (onWarmupEnding == null)
            break;
          onWarmupEnding();
          break;
        case MultiplayerWarmupComponent.WarmupStates.Ended:
          this._timerComponent.StartTimerAsClient(message.StateStartTimeInSeconds, 3f);
          Action onWarmupEnded = this.OnWarmupEnded;
          if (onWarmupEnded != null)
            onWarmupEnded();
          this.PlayBattleStartingSound();
          break;
      }
    }

    private void PlayBattleStartingSound()
    {
      MatrixFrame cameraFrame = Mission.Current.GetCameraFrame();
      Vec3 position = cameraFrame.origin + cameraFrame.rotation.u;
      NetworkCommunicator myPeer = GameNetwork.MyPeer;
      MissionPeer missionPeer = myPeer != null ? myPeer.GetComponent<MissionPeer>() : (MissionPeer) null;
      if (missionPeer?.Team != null)
        MBSoundEvent.PlaySound(SoundEvent.GetEventIdFromString("event:/alerts/rally/" + (missionPeer.Team.Side == BattleSideEnum.Attacker ? MultiplayerOptions.OptionType.CultureTeam1.GetStrValue() : MultiplayerOptions.OptionType.CultureTeam2.GetStrValue()).ToLower()), position);
      else
        MBSoundEvent.PlaySound(SoundEvent.GetEventIdFromString("event:/alerts/rally/generic"), position);
    }

    public enum WarmupStates
    {
      WaitingForPlayers,
      InProgress,
      Ending,
      Ended,
    }
  }
}
