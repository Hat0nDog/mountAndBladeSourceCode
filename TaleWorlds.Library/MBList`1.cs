// Decompiled with JetBrains decompiler
// Type: TaleWorlds.Library.MBList`1
// Assembly: TaleWorlds.Library, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 271D0DB8-3C5D-4850-A565-542B4CF7D347
// Assembly location: E:\SteamLibrary\steamapps\common\Mount & Blade II Bannerlord\bin\Win64_Shipping_Client\TaleWorlds.Library.dll

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace TaleWorlds.Library
{
  public class MBList<T> : IMBCollection, ICollection, IEnumerable, IEnumerable<T>
  {
    private T[] _data;

    public int Count { get; private set; }

    public int Capacity => this._data.Length;

    public MBList()
    {
      this._data = new T[1];
      this.Count = 0;
    }

    public MBList(List<T> list)
    {
      this._data = list.ToArray();
      this.Count = this._data.Length;
    }

    public MBList(IEnumerable<T> list)
    {
      this._data = list.ToArray<T>();
      this.Count = this._data.Length;
    }

    public T[] RawArray => this._data;

    public bool IsSynchronized => false;

    public object SyncRoot => (object) null;

    public T this[int index]
    {
      get => this._data[index];
      set => this._data[index] = value;
    }

    public int IndexOf(T item)
    {
      int num = -1;
      EqualityComparer<T> equalityComparer = EqualityComparer<T>.Default;
      for (int index = 0; index < this.Count; ++index)
      {
        if (equalityComparer.Equals(this._data[index], item))
        {
          num = index;
          break;
        }
      }
      return num;
    }

    public bool Contains(T item)
    {
      EqualityComparer<T> equalityComparer = EqualityComparer<T>.Default;
      for (int index = 0; index < this.Count; ++index)
      {
        if (equalityComparer.Equals(this._data[index], item))
          return true;
      }
      return false;
    }

    public IEnumerator<T> GetEnumerator()
    {
      for (int i = 0; i < this.Count; ++i)
        yield return this._data[i];
    }

    IEnumerator IEnumerable.GetEnumerator() => (IEnumerator) this.GetEnumerator();

    public void Clear()
    {
      for (int index = 0; index < this.Count; ++index)
        this._data[index] = default (T);
      this.Count = 0;
    }

    private void EnsureCapacity(int newMinimumCapacity)
    {
      if (newMinimumCapacity <= this.Capacity)
        return;
      T[] objArray = new T[Math.Max(this.Capacity * 2, newMinimumCapacity)];
      this.CopyTo((Array) objArray, 0);
      this._data = objArray;
    }

    public void Add(T item)
    {
      this.EnsureCapacity(this.Count + 1);
      this._data[this.Count] = item;
      ++this.Count;
    }

    public void AddRange(IEnumerable<T> list)
    {
      foreach (T obj in list)
      {
        this.EnsureCapacity(this.Count + 1);
        this._data[this.Count] = obj;
        ++this.Count;
      }
    }

    public bool Remove(T item)
    {
      int index1 = this.IndexOf(item);
      if (index1 < 0)
        return false;
      for (int index2 = index1; index2 < this.Count - 1; ++index2)
        this._data[index1] = this._data[index1 + 1];
      --this.Count;
      this._data[this.Count] = default (T);
      return true;
    }

    public void CopyTo(Array array, int index)
    {
      if (array is T[] objArray1)
      {
        for (int index1 = 0; index1 < this.Count; ++index1)
          objArray1[index1 + index] = this._data[index1];
      }
      else
      {
        array.GetType().GetElementType();
        object[] objArray = array as object[];
        try
        {
          for (int index1 = 0; index1 < this.Count; ++index1)
            objArray[index++] = (object) this._data[index1];
        }
        catch (ArrayTypeMismatchException ex)
        {
        }
      }
    }
  }
}
