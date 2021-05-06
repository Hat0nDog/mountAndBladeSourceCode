﻿// Decompiled with JetBrains decompiler
// Type: TaleWorlds.Core.DeterministicRandom
// Assembly: TaleWorlds.Core, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 43F7B45C-9AE1-4715-A37B-2364840F62AF
// Assembly location: E:\SteamLibrary\steamapps\common\Mount & Blade II Bannerlord\bin\Win64_Shipping_Client\TaleWorlds.Core.dll

using System.Collections.Generic;
using TaleWorlds.SaveSystem;

namespace TaleWorlds.Core
{
  public class DeterministicRandom
  {
    public const int MaxDeterministicRandomValue = 100;
    [SaveableField(0)]
    private readonly int _capacity;
    [SaveableField(1)]
    private int[] _randomValues;

    internal static void AutoGeneratedStaticCollectObjectsDeterministicRandom(
      object o,
      List<object> collectedObjects)
    {
      ((DeterministicRandom) o).AutoGeneratedInstanceCollectObjects(collectedObjects);
    }

    protected virtual void AutoGeneratedInstanceCollectObjects(List<object> collectedObjects) => collectedObjects.Add((object) this._randomValues);

    internal static object AutoGeneratedGetMemberValue_capacity(object o) => (object) ((DeterministicRandom) o)._capacity;

    internal static object AutoGeneratedGetMemberValue_randomValues(object o) => (object) ((DeterministicRandom) o)._randomValues;

    public DeterministicRandom(int capacity)
    {
      this._capacity = capacity;
      this._randomValues = new int[this._capacity];
      for (int index = 0; index < this._capacity; ++index)
        this._randomValues[index] = (int) ((double) MBRandom.DeterministicRandom.NextFloat() * 100.0);
    }

    public int GetValue(int index)
    {
      if (this._randomValues == null)
      {
        this._randomValues = new int[this._capacity];
        for (int index1 = 0; index1 < this._capacity; ++index1)
          this._randomValues[index1] = (int) ((double) MBRandom.DeterministicRandom.NextFloat() * 100.0);
      }
      index %= this._capacity;
      return this._randomValues[index];
    }

    public float GetValueNormalized(int index) => (float) this.GetValue(index) / 100f;

    public float GetValue(int index, float max) => this.GetValue(index, 0.0f, max);

    public float GetValue(int index, float min, float max)
    {
      double num = ((double) max - (double) min) / 100.0;
      return (float) this.GetValue(index) * (float) num + min;
    }
  }
}
