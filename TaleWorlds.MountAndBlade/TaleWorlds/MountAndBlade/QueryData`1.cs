// Decompiled with JetBrains decompiler
// Type: TaleWorlds.MountAndBlade.QueryData`1
// Assembly: TaleWorlds.MountAndBlade, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3C0E3F42-756F-4407-B817-06BA412A78B0
// Assembly location: E:\SteamLibrary\steamapps\common\Mount & Blade II Bannerlord\bin\Win64_Shipping_Client\TaleWorlds.MountAndBlade.dll

using System;
using System.Collections.Generic;

namespace TaleWorlds.MountAndBlade
{
  public class QueryData<T> : IQueryData
  {
    private T _cachedValue;
    private float _expireTime;
    private readonly float _lifetime;
    private readonly Func<T> _valueFunc;
    private IEnumerable<IQueryData> _syncGroup;

    public QueryData(Func<T> valueFunc, float lifetime)
    {
      this._cachedValue = default (T);
      this._expireTime = 0.0f;
      this._lifetime = lifetime;
      this._valueFunc = valueFunc;
      this._syncGroup = (IEnumerable<IQueryData>) null;
    }

    public void Evaluate(float currentTime) => this.SetValue(this._valueFunc(), currentTime);

    public void SetValue(T value, float currentTime)
    {
      this._cachedValue = value;
      this._expireTime = currentTime + this._lifetime;
    }

    public T GetCachedValue() => this._cachedValue;

    public T GetCachedValueWithMaxAge(float age)
    {
      if ((double) MBCommon.GetTime(MBCommon.TimeType.Mission) <= (double) this._expireTime - (double) this._lifetime + (double) Math.Min(this._lifetime, age))
        return this._cachedValue;
      this.Expire();
      return this.Value;
    }

    public T Value
    {
      get
      {
        float time = MBCommon.GetTime(MBCommon.TimeType.Mission);
        if ((double) time >= (double) this._expireTime)
        {
          if (this._syncGroup != null)
          {
            foreach (IQueryData queryData in this._syncGroup)
              queryData.Evaluate(time);
          }
          this.Evaluate(time);
        }
        return this._cachedValue;
      }
    }

    public void Expire() => this._expireTime = 0.0f;

    public static void SetupSyncGroup(params IQueryData[] groupItems)
    {
      List<IQueryData> queryDataList = new List<IQueryData>((IEnumerable<IQueryData>) groupItems);
      foreach (IQueryData groupItem in groupItems)
        groupItem.SetSyncGroup((IEnumerable<IQueryData>) queryDataList);
    }

    public void SetSyncGroup(IEnumerable<IQueryData> syncGroup) => this._syncGroup = syncGroup;
  }
}
