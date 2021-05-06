// Decompiled with JetBrains decompiler
// Type: TaleWorlds.Library.EventSystem.DictionaryByType
// Assembly: TaleWorlds.Library, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 271D0DB8-3C5D-4850-A565-542B4CF7D347
// Assembly location: E:\SteamLibrary\steamapps\common\Mount & Blade II Bannerlord\bin\Win64_Shipping_Client\TaleWorlds.Library.dll

using System;
using System.Collections.Generic;

namespace TaleWorlds.Library.EventSystem
{
  public class DictionaryByType
  {
    private readonly IDictionary<Type, object> _eventsByType = (IDictionary<Type, object>) new Dictionary<Type, object>();

    public void Add<T>(Action<T> value)
    {
      object obj;
      if (!this._eventsByType.TryGetValue(typeof (T), out obj))
      {
        obj = (object) new List<Action<T>>();
        this._eventsByType[typeof (T)] = obj;
      }
      ((List<Action<T>>) obj).Add(value);
    }

    public void Remove<T>(Action<T> value)
    {
      object obj;
      if (!this._eventsByType.TryGetValue(typeof (T), out obj))
        return;
      List<Action<T>> actionList = (List<Action<T>>) obj;
      actionList.Remove(value);
      this._eventsByType[typeof (T)] = (object) actionList;
    }

    public void InvokeActions<T>(T item)
    {
      object obj;
      if (!this._eventsByType.TryGetValue(typeof (T), out obj))
        return;
      foreach (Action<T> action in (List<Action<T>>) obj)
        action(item);
    }

    public List<Action<T>> Get<T>() => (List<Action<T>>) this._eventsByType[typeof (T)];

    public bool TryGet<T>(out List<Action<T>> value)
    {
      object obj;
      if (this._eventsByType.TryGetValue(typeof (T), out obj))
      {
        value = (List<Action<T>>) obj;
        return true;
      }
      value = (List<Action<T>>) null;
      return false;
    }

    public IDictionary<Type, object> GetClone() => (IDictionary<Type, object>) new Dictionary<Type, object>(this._eventsByType);

    public void Clear() => this._eventsByType.Clear();
  }
}
