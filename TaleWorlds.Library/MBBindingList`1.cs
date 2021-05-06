// Decompiled with JetBrains decompiler
// Type: TaleWorlds.Library.MBBindingList`1
// Assembly: TaleWorlds.Library, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 271D0DB8-3C5D-4850-A565-542B4CF7D347
// Assembly location: E:\SteamLibrary\steamapps\common\Mount & Blade II Bannerlord\bin\Win64_Shipping_Client\TaleWorlds.Library.dll

using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace TaleWorlds.Library
{
  public class MBBindingList<T> : Collection<T>, IMBBindingList, IList, ICollection, IEnumerable
  {
    private readonly List<T> _list;
    private List<ListChangedEventHandler> _eventHandlers;

    public MBBindingList()
      : base((IList<T>) new List<T>(64))
    {
      this._list = (List<T>) this.Items;
    }

    public event ListChangedEventHandler ListChanged
    {
      add
      {
        if (this._eventHandlers == null)
          this._eventHandlers = new List<ListChangedEventHandler>();
        this._eventHandlers.Add(value);
      }
      remove
      {
        if (this._eventHandlers == null)
          return;
        this._eventHandlers.Remove(value);
      }
    }

    protected override void ClearItems()
    {
      base.ClearItems();
      this.FireListChanged(ListChangedType.Reset, -1);
    }

    protected override void InsertItem(int index, T item)
    {
      base.InsertItem(index, item);
      this.FireListChanged(ListChangedType.ItemAdded, index);
    }

    protected override void RemoveItem(int index)
    {
      base.RemoveItem(index);
      this.FireListChanged(ListChangedType.ItemDeleted, index);
    }

    protected override void SetItem(int index, T item)
    {
      base.SetItem(index, item);
      this.FireListChanged(ListChangedType.ItemChanged, index);
    }

    private void FireListChanged(ListChangedType type, int index) => this.OnListChanged(new ListChangedEventArgs(type, index));

    protected virtual void OnListChanged(ListChangedEventArgs e)
    {
      if (this._eventHandlers == null)
        return;
      for (int index = 0; index < this._eventHandlers.Count; ++index)
        this._eventHandlers[index]((object) this, e);
    }

    public void Sort()
    {
      this._list.Sort();
      this.FireListChanged(ListChangedType.Sorted, -1);
    }

    public void Sort(IComparer<T> comparer)
    {
      if (this.IsOrdered(comparer))
        return;
      this._list.Sort(comparer);
      this.FireListChanged(ListChangedType.Sorted, -1);
    }

    public bool IsOrdered(IComparer<T> comparer)
    {
      for (int index = 1; index < this._list.Count; ++index)
      {
        if (comparer.Compare(this._list[index - 1], this._list[index]) == 1)
          return false;
      }
      return true;
    }

    public void ApplyActionOnAllItems(Action<T> action)
    {
      foreach (T obj in this._list)
        action(obj);
    }
  }
}
