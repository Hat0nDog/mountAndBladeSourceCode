// Decompiled with JetBrains decompiler
// Type: TaleWorlds.Library.EventSystem.EventManager
// Assembly: TaleWorlds.Library, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 271D0DB8-3C5D-4850-A565-542B4CF7D347
// Assembly location: E:\SteamLibrary\steamapps\common\Mount & Blade II Bannerlord\bin\Win64_Shipping_Client\TaleWorlds.Library.dll

using System;
using System.Collections.Generic;

namespace TaleWorlds.Library.EventSystem
{
  public class EventManager
  {
    private readonly DictionaryByType _eventsByType;

    public EventManager() => this._eventsByType = new DictionaryByType();

    public void RegisterEvent<T>(Action<T> eventObjType)
    {
      if (!typeof (T).IsSubclassOf(typeof (EventBase)))
        return;
      this._eventsByType.Add<T>(eventObjType);
    }

    public void UnregisterEvent<T>(Action<T> eventObjType)
    {
      if (!typeof (T).IsSubclassOf(typeof (EventBase)))
        return;
      this._eventsByType.Remove<T>(eventObjType);
    }

    public void TriggerEvent<T>(T eventObj) => this._eventsByType.InvokeActions<T>(eventObj);

    public void Clear() => this._eventsByType.Clear();

    public IDictionary<Type, object> GetCloneOfEventDictionary() => this._eventsByType.GetClone();
  }
}
