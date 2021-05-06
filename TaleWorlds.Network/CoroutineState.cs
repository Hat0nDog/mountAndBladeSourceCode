// Decompiled with JetBrains decompiler
// Type: TaleWorlds.Network.CoroutineState
// Assembly: TaleWorlds.Network, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: EDF757B2-6540-4679-910A-3C5D58B2EF82
// Assembly location: E:\SteamLibrary\steamapps\common\Mount & Blade II Bannerlord\bin\Win64_Shipping_Client\TaleWorlds.Network.dll

namespace TaleWorlds.Network
{
  public abstract class CoroutineState
  {
    protected CoroutineManager CoroutineManager { get; private set; }

    protected internal virtual void Initialize(CoroutineManager coroutineManager) => this.CoroutineManager = coroutineManager;

    protected internal abstract bool IsFinished { get; }
  }
}
