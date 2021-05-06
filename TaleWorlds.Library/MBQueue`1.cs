// Decompiled with JetBrains decompiler
// Type: TaleWorlds.Library.MBQueue`1
// Assembly: TaleWorlds.Library, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 271D0DB8-3C5D-4850-A565-542B4CF7D347
// Assembly location: E:\SteamLibrary\steamapps\common\Mount & Blade II Bannerlord\bin\Win64_Shipping_Client\TaleWorlds.Library.dll

using System;
using System.Collections;
using System.Collections.Generic;

namespace TaleWorlds.Library
{
  public class MBQueue<T> : IMBCollection, ICollection, IEnumerable, IEnumerable<T>
  {
    private readonly Queue<T> _data;

    public int Count => this._data.Count;

    public MBQueue() => this._data = new Queue<T>();

    public MBQueue(Queue<T> queue) => this._data = new Queue<T>((IEnumerable<T>) queue);

    public bool IsSynchronized => false;

    public object SyncRoot => (object) null;

    public bool Contains(T item) => this._data.Contains(item);

    public IEnumerator<T> GetEnumerator() => (IEnumerator<T>) this._data.GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator() => (IEnumerator) this.GetEnumerator();

    public void Clear() => this._data.Clear();

    public void Enqueue(T item) => this._data.Enqueue(item);

    public T Dequeue() => this._data.Dequeue();

    public void CopyTo(Array array, int index) => this._data.CopyTo((T[]) array, index);
  }
}
