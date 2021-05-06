// Decompiled with JetBrains decompiler
// Type: TaleWorlds.Network.WaitForTicks
// Assembly: TaleWorlds.Network, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: EDF757B2-6540-4679-910A-3C5D58B2EF82
// Assembly location: E:\SteamLibrary\steamapps\common\Mount & Blade II Bannerlord\bin\Win64_Shipping_Client\TaleWorlds.Network.dll

namespace TaleWorlds.Network
{
  public class WaitForTicks : CoroutineState
  {
    private int _beginTick;

    internal int TickCount { get; private set; }

    public WaitForTicks(int tickCount) => this.TickCount = tickCount;

    protected internal override void Initialize(CoroutineManager coroutineManager)
    {
      base.Initialize(coroutineManager);
      this._beginTick = coroutineManager.CurrentTick;
    }

    protected internal override bool IsFinished => this._beginTick + this.TickCount >= this.CoroutineManager.CurrentTick;
  }
}
