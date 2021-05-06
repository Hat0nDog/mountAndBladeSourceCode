// Decompiled with JetBrains decompiler
// Type: TaleWorlds.Network.TickManager
// Assembly: TaleWorlds.Network, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: EDF757B2-6540-4679-910A-3C5D58B2EF82
// Assembly location: E:\SteamLibrary\steamapps\common\Mount & Blade II Bannerlord\bin\Win64_Shipping_Client\TaleWorlds.Network.dll

using System.Diagnostics;
using System.Threading;

namespace TaleWorlds.Network
{
  public class TickManager
  {
    private Stopwatch _stopwatch;
    private int _tickRate = 5000;
    private TickManager.TickDelegate _tickMethod;
    private double _residualWaitTime;
    private double _numberOfTicksPerMilisecond;
    private double _inverseNumberOfTicksPerMilisecond;
    private double _maxTickMilisecond;

    public TickManager(int tickRate, TickManager.TickDelegate tickMethod)
    {
      this._tickRate = tickRate;
      this._tickMethod = tickMethod;
      this._numberOfTicksPerMilisecond = (double) Stopwatch.Frequency / 1000.0;
      this._inverseNumberOfTicksPerMilisecond = 1000.0 / (double) Stopwatch.Frequency;
      this._maxTickMilisecond = 1000.0 / (double) this._tickRate;
      this._stopwatch = new Stopwatch();
      this._stopwatch.Start();
    }

    public void Tick()
    {
      long elapsedTicks = this._stopwatch.ElapsedTicks;
      this._tickMethod();
      double num1 = this._inverseNumberOfTicksPerMilisecond * (double) (this._stopwatch.ElapsedTicks - elapsedTicks);
      if (num1 >= this._maxTickMilisecond)
        return;
      double num2 = this._maxTickMilisecond - num1 + this._residualWaitTime;
      int millisecondsTimeout = (int) num2;
      this._residualWaitTime = num2 - (double) millisecondsTimeout;
      if (millisecondsTimeout <= 0)
        return;
      Thread.Sleep(millisecondsTimeout);
    }

    public delegate void TickDelegate();
  }
}
