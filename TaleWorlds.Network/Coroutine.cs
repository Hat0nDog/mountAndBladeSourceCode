// Decompiled with JetBrains decompiler
// Type: TaleWorlds.Network.Coroutine
// Assembly: TaleWorlds.Network, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: EDF757B2-6540-4679-910A-3C5D58B2EF82
// Assembly location: E:\SteamLibrary\steamapps\common\Mount & Blade II Bannerlord\bin\Win64_Shipping_Client\TaleWorlds.Network.dll

using System.Collections;

namespace TaleWorlds.Network
{
  public class Coroutine
  {
    public bool IsStarted { get; internal set; }

    internal CoroutineDelegate CoroutineMethod { get; set; }

    internal IEnumerator Enumerator { get; set; }

    internal CoroutineState CurrentState { get; set; }
  }
}
