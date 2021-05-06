// Decompiled with JetBrains decompiler
// Type: TaleWorlds.MountAndBlade.MultiplayerTimerComponent
// Assembly: TaleWorlds.MountAndBlade, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3C0E3F42-756F-4407-B817-06BA412A78B0
// Assembly location: E:\SteamLibrary\steamapps\common\Mount & Blade II Bannerlord\bin\Win64_Shipping_Client\TaleWorlds.MountAndBlade.dll

using System;

namespace TaleWorlds.MountAndBlade
{
  public class MultiplayerTimerComponent : MissionNetwork
  {
    private MissionTimer _missionTimer;

    public bool IsTimerRunning { get; private set; }

    public void StartTimerAsServer(float duration)
    {
      this._missionTimer = new MissionTimer(duration);
      this.IsTimerRunning = true;
    }

    public void StartTimerAsClient(float startTime, float duration)
    {
      this._missionTimer = MissionTimer.CreateSynchedTimerClient(startTime, duration);
      this.IsTimerRunning = true;
    }

    public float GetRemainingTime(bool isSynched)
    {
      if (!this.IsTimerRunning)
        return 0.0f;
      float remainingTimeInSeconds = this._missionTimer.GetRemainingTimeInSeconds(isSynched);
      return isSynched ? Math.Min(remainingTimeInSeconds, this._missionTimer.GetTimerDuration()) : remainingTimeInSeconds;
    }

    public bool CheckIfTimerPassed() => this.IsTimerRunning && this._missionTimer.Check();

    public MissionTime GetCurrentTimerStartTime() => this._missionTimer.GetStartTime();
  }
}
