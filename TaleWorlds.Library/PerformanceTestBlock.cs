// Decompiled with JetBrains decompiler
// Type: TaleWorlds.Library.PerformanceTestBlock
// Assembly: TaleWorlds.Library, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 271D0DB8-3C5D-4850-A565-542B4CF7D347
// Assembly location: E:\SteamLibrary\steamapps\common\Mount & Blade II Bannerlord\bin\Win64_Shipping_Client\TaleWorlds.Library.dll

using System;
using System.Diagnostics;

namespace TaleWorlds.Library
{
  public class PerformanceTestBlock : IDisposable
  {
    private readonly string _name;
    private readonly Stopwatch _stopwatch;

    public PerformanceTestBlock(string name)
    {
      this._name = name;
      this._stopwatch = new Stopwatch();
      this._stopwatch.Start();
    }

    void IDisposable.Dispose() => Debug.Print(this._name + " took " + (object) this._stopwatch.ElapsedMilliseconds + " ms.");
  }
}
