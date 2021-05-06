// Decompiled with JetBrains decompiler
// Type: TaleWorlds.Library.MBReadOnlyList`1
// Assembly: TaleWorlds.Library, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 271D0DB8-3C5D-4850-A565-542B4CF7D347
// Assembly location: E:\SteamLibrary\steamapps\common\Mount & Blade II Bannerlord\bin\Win64_Shipping_Client\TaleWorlds.Library.dll

using System;
using System.Collections;
using System.Collections.Generic;

namespace TaleWorlds.Library
{
  public class MBReadOnlyList<T> : 
    ICollection,
    IEnumerable,
    IReadOnlyList<T>,
    IEnumerable<T>,
    IReadOnlyCollection<T>
  {
    private List<T> _list;

    public MBReadOnlyList(List<T> list) => this._list = list;

    public int Count => this._list.Count;

    public bool IsSynchronized => false;

    public object SyncRoot => (object) null;

    public T this[int index] => this._list[index];

    public bool Contains(T item) => this._list.Contains(item);

    public List<T>.Enumerator GetEnumerator() => this._list.GetEnumerator();

    IEnumerator<T> IEnumerable<T>.GetEnumerator() => (IEnumerator<T>) this._list.GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator() => (IEnumerator) this._list.GetEnumerator();

    public int IndexOf(T item) => this._list.IndexOf(item);

    public void CopyTo(Array array, int index)
    {
      if (array is T[] array1)
      {
        this._list.CopyTo(array1, index);
      }
      else
      {
        array.GetType().GetElementType();
        object[] objArray = array as object[];
        int count = this._list.Count;
        try
        {
          for (int index1 = 0; index1 < count; ++index1)
            objArray[index++] = (object) this._list[index1];
        }
        catch (ArrayTypeMismatchException ex)
        {
        }
      }
    }
  }
}
