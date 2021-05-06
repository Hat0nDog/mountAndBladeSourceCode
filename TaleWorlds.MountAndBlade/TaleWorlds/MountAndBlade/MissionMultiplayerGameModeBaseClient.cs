// Decompiled with JetBrains decompiler
// Type: TaleWorlds.MountAndBlade.MissionMultiplayerGameModeBaseClient
// Assembly: TaleWorlds.MountAndBlade, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3C0E3F42-756F-4407-B817-06BA412A78B0
// Assembly location: E:\SteamLibrary\steamapps\common\Mount & Blade II Bannerlord\bin\Win64_Shipping_Client\TaleWorlds.MountAndBlade.dll

using TaleWorlds.Core;
using TaleWorlds.Library;

namespace TaleWorlds.MountAndBlade
{
  public abstract class MissionMultiplayerGameModeBaseClient : MissionNetwork, ICameraModeLogic
  {
    public MissionLobbyComponent MissionLobbyComponent { get; private set; }

    public MissionScoreboardComponent ScoreboardComponent { get; private set; }

    public MultiplayerGameNotificationsComponent NotificationsComponent { get; private set; }

    public MultiplayerWarmupComponent WarmupComponent { get; private set; }

    public IRoundComponent RoundComponent { get; private set; }

    public MultiplayerTimerComponent TimerComponent { get; private set; }

    public abstract bool IsGameModeUsingGold { get; }

    public abstract bool IsGameModeTactical { get; }

    public abstract bool IsGameModeUsingRoundCountdown { get; }

    public abstract MissionLobbyComponent.MultiplayerGameType GameType { get; }

    public abstract int GetGoldAmount();

    public virtual SpectatorCameraTypes GetMissionCameraLockMode(
      bool lockedToMainPlayer)
    {
      return SpectatorCameraTypes.Invalid;
    }

    public bool IsRoundInProgress
    {
      get
      {
        IRoundComponent roundComponent = this.RoundComponent;
        return roundComponent != null && roundComponent.CurrentRoundState == MultiplayerRoundState.InProgress;
      }
    }

    public bool IsInWarmup => this.MissionLobbyComponent.IsInWarmup;

    public float RemainingTime => this.TimerComponent.GetRemainingTime(GameNetwork.IsClientOrReplay);

    public override void OnBehaviourInitialize()
    {
      base.OnBehaviourInitialize();
      this.MissionLobbyComponent = this.Mission.GetMissionBehaviour<MissionLobbyComponent>();
      this.ScoreboardComponent = this.Mission.GetMissionBehaviour<MissionScoreboardComponent>();
      this.NotificationsComponent = this.Mission.GetMissionBehaviour<MultiplayerGameNotificationsComponent>();
      this.WarmupComponent = this.Mission.GetMissionBehaviour<MultiplayerWarmupComponent>();
      this.RoundComponent = this.Mission.GetMissionBehaviour<IRoundComponent>();
      this.TimerComponent = this.Mission.GetMissionBehaviour<MultiplayerTimerComponent>();
    }

    public override void EarlyStart() => this.MissionLobbyComponent.MissionType = this.GameType;

    public bool CheckTimer(out int remainingTime, out int remainingWarningTime, bool forceUpdate = false)
    {
      bool flag = false;
      float f = 0.0f;
      if (this.WarmupComponent != null && this.MissionLobbyComponent.CurrentMultiplayerState == MissionLobbyComponent.MultiplayerGameState.WaitingFirstPlayers)
        flag = !this.WarmupComponent.IsInWarmup;
      else if (this.RoundComponent != null)
      {
        flag = !this.RoundComponent.CurrentRoundState.StateHasVisualTimer();
        f = this.RoundComponent.LastRoundEndRemainingTime;
      }
      if (forceUpdate || !flag)
      {
        remainingTime = !flag ? MBMath.Ceiling(this.RemainingTime) : MBMath.Ceiling(f);
        remainingWarningTime = MBMath.Ceiling((float) this.GetWarningTimer());
        return true;
      }
      remainingTime = 0;
      remainingWarningTime = 0;
      return false;
    }

    protected virtual int GetWarningTimer() => 0;

    public abstract void OnGoldAmountChangedForRepresentative(
      MissionRepresentativeBase representative,
      int goldAmount);
  }
}
