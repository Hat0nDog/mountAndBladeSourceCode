// Decompiled with JetBrains decompiler
// Type: TaleWorlds.MountAndBlade.IncrementalTimer
// Assembly: TaleWorlds.MountAndBlade, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3C0E3F42-756F-4407-B817-06BA412A78B0
// Assembly location: E:\SteamLibrary\steamapps\common\Mount & Blade II Bannerlord\bin\Win64_Shipping_Client\TaleWorlds.MountAndBlade.dll

using System;
using TaleWorlds.Core;

namespace TaleWorlds.MountAndBlade
{
  public class IncrementalTimer
  {
    private readonly float _totalDuration;
    private readonly float _tickInterval;
    private readonly Timer _timer;

    public float TimerCounter { get; private set; }

    public IncrementalTimer(float totalDuration, float tickInterval)
    {
      this._tickInterval = Math.Max(tickInterval, 0.01f);
      this._totalDuration = Math.Max(totalDuration, 0.01f);
      this.TimerCounter = 0.0f;
      this._timer = new Timer(MBCommon.GetTime(MBCommon.TimeType.Mission), this._tickInterval);
    }

    public bool Check()
    {
      if (!this._timer.Check(MBCommon.GetTime(MBCommon.TimeType.Mission)))
        return false;
      this.TimerCounter += this._tickInterval / this._totalDuration;
      return true;
    }

    public bool HasEnded() => (double) this.TimerCounter >= 1.0;
  }
}
