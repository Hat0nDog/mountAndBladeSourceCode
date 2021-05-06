// Decompiled with JetBrains decompiler
// Type: TaleWorlds.Library.MBList2D`1
// Assembly: TaleWorlds.Library, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 271D0DB8-3C5D-4850-A565-542B4CF7D347
// Assembly location: E:\SteamLibrary\steamapps\common\Mount & Blade II Bannerlord\bin\Win64_Shipping_Client\TaleWorlds.Library.dll

using System;

namespace TaleWorlds.Library
{
  public class MBList2D<T> : IMBCollection
  {
    private T[] _data;

    public int Count1 { get; private set; }

    public int Count2 { get; private set; }

    private int Capacity => this._data.Length;

    public MBList2D(int count1, int count2)
    {
      this._data = new T[count1 * count2];
      this.Count1 = count1;
      this.Count2 = count2;
    }

    public T[] RawArray => this._data;

    public T this[int index1, int index2]
    {
      get => this._data[index1 * this.Count2 + index2];
      set => this._data[index1 * this.Count2 + index2] = value;
    }

    public bool Contains(T item)
    {
      for (int index1 = 0; index1 < this.Count1; ++index1)
      {
        for (int index2 = 0; index2 < this.Count2; ++index2)
        {
          if (this._data[index1 * this.Count2 + index2].Equals((object) item))
            return true;
        }
      }
      return false;
    }

    public void Clear()
    {
      for (int index = 0; index < this.Count1 * this.Count2; ++index)
        this._data[index] = default (T);
    }

    public void ResetWithNewCount(int newCount1, int newCount2)
    {
      if (this.Count1 != newCount1 || this.Count2 != newCount2)
      {
        this.Count1 = newCount1;
        this.Count2 = newCount2;
        if (this.Capacity < newCount1 * newCount2)
          this._data = new T[Math.Max(this.Capacity * 2, newCount1 * newCount2)];
        else
          this.Clear();
      }
      else
        this.Clear();
    }

    public void CopyRowTo(
      int sourceIndex1,
      int sourceIndex2,
      MBList2D<T> destination,
      int destinationIndex1,
      int destinationIndex2,
      int copyCount)
    {
      for (int index = 0; index < copyCount; ++index)
        destination[destinationIndex1, destinationIndex2 + index] = this[sourceIndex1, sourceIndex2 + index];
    }
  }
}
