// Decompiled with JetBrains decompiler
// Type: TaleWorlds.Network.CoroutineManager
// Assembly: TaleWorlds.Network, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: EDF757B2-6540-4679-910A-3C5D58B2EF82
// Assembly location: E:\SteamLibrary\steamapps\common\Mount & Blade II Bannerlord\bin\Win64_Shipping_Client\TaleWorlds.Network.dll

using System.Collections;
using System.Collections.Generic;

namespace TaleWorlds.Network
{
  public class CoroutineManager
  {
    private List<Coroutine> _coroutines;

    public int CurrentTick { get; private set; }

    public int CoroutineCount => this._coroutines.Count;

    public CoroutineManager()
    {
      this._coroutines = new List<Coroutine>();
      this.CurrentTick = 0;
    }

    public void AddCoroutine(CoroutineDelegate coroutineMethod) => this._coroutines.Add(new Coroutine()
    {
      CoroutineMethod = coroutineMethod,
      IsStarted = false
    });

    public void Tick()
    {
      for (int index = 0; index < this._coroutines.Count; ++index)
      {
        Coroutine coroutine = this._coroutines[index];
        bool flag = false;
        if (!coroutine.IsStarted)
        {
          coroutine.IsStarted = true;
          flag = true;
          coroutine.Enumerator = (IEnumerator) coroutine.CoroutineMethod();
        }
        if (flag || coroutine.CurrentState.IsFinished)
        {
          if (!coroutine.Enumerator.MoveNext())
          {
            this._coroutines.Remove(coroutine);
            --index;
          }
          else
          {
            coroutine.CurrentState = coroutine.Enumerator.Current as CoroutineState;
            coroutine.CurrentState.Initialize(this);
          }
        }
      }
      ++this.CurrentTick;
    }
  }
}
