// Decompiled with JetBrains decompiler
// Type: TaleWorlds.Library.PriorityQueue`2
// Assembly: TaleWorlds.Library, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 271D0DB8-3C5D-4850-A565-542B4CF7D347
// Assembly location: E:\SteamLibrary\steamapps\common\Mount & Blade II Bannerlord\bin\Win64_Shipping_Client\TaleWorlds.Library.dll

using System;
using System.Collections;
using System.Collections.Generic;

namespace TaleWorlds.Library
{
  public class PriorityQueue<TPriority, TValue> : 
    ICollection<KeyValuePair<TPriority, TValue>>,
    IEnumerable<KeyValuePair<TPriority, TValue>>,
    IEnumerable
  {
    private readonly List<KeyValuePair<TPriority, TValue>> _baseHeap;
    private readonly IComparer<TPriority> _customComparer;

    private IComparer<TPriority> Comparer => this._customComparer == null ? (IComparer<TPriority>) System.Collections.Generic.Comparer<TPriority>.Default : this._customComparer;

    public PriorityQueue() => this._baseHeap = new List<KeyValuePair<TPriority, TValue>>();

    public PriorityQueue(int capacity) => this._baseHeap = new List<KeyValuePair<TPriority, TValue>>(capacity);

    public PriorityQueue(int capacity, IComparer<TPriority> comparer)
    {
      if (comparer == null)
        throw new ArgumentNullException();
      this._baseHeap = new List<KeyValuePair<TPriority, TValue>>(capacity);
      this._customComparer = comparer;
    }

    public PriorityQueue(IComparer<TPriority> comparer)
    {
      if (comparer == null)
        throw new ArgumentNullException();
      this._baseHeap = new List<KeyValuePair<TPriority, TValue>>();
      this._customComparer = comparer;
    }

    public PriorityQueue(IEnumerable<KeyValuePair<TPriority, TValue>> data)
      : this(data, (IComparer<TPriority>) System.Collections.Generic.Comparer<TPriority>.Default)
    {
    }

    public PriorityQueue(
      IEnumerable<KeyValuePair<TPriority, TValue>> data,
      IComparer<TPriority> comparer)
    {
      this._customComparer = data != null && comparer != null ? comparer : throw new ArgumentNullException();
      this._baseHeap = new List<KeyValuePair<TPriority, TValue>>(data);
      for (int pos = this._baseHeap.Count / 2 - 1; pos >= 0; --pos)
        this.HeapifyFromBeginningToEnd(pos);
    }

    public static PriorityQueue<TPriority, TValue> MergeQueues(
      PriorityQueue<TPriority, TValue> pq1,
      PriorityQueue<TPriority, TValue> pq2)
    {
      if (pq1 == null || pq2 == null)
        throw new ArgumentNullException();
      if (pq1.Comparer != pq2.Comparer)
        throw new InvalidOperationException("Priority queues to be merged must have equal comparers");
      return PriorityQueue<TPriority, TValue>.MergeQueues(pq1, pq2, pq1.Comparer);
    }

    public static PriorityQueue<TPriority, TValue> MergeQueues(
      PriorityQueue<TPriority, TValue> pq1,
      PriorityQueue<TPriority, TValue> pq2,
      IComparer<TPriority> comparer)
    {
      if (pq1 == null || pq2 == null || comparer == null)
        throw new ArgumentNullException();
      PriorityQueue<TPriority, TValue> priorityQueue = new PriorityQueue<TPriority, TValue>(pq1.Count + pq2.Count, comparer);
      priorityQueue._baseHeap.AddRange((IEnumerable<KeyValuePair<TPriority, TValue>>) pq1._baseHeap);
      priorityQueue._baseHeap.AddRange((IEnumerable<KeyValuePair<TPriority, TValue>>) pq2._baseHeap);
      for (int pos = priorityQueue._baseHeap.Count / 2 - 1; pos >= 0; --pos)
        priorityQueue.HeapifyFromBeginningToEnd(pos);
      return priorityQueue;
    }

    public void Enqueue(TPriority priority, TValue value) => this.Insert(priority, value);

    public KeyValuePair<TPriority, TValue> Dequeue()
    {
      if (this.IsEmpty)
        throw new InvalidOperationException("Priority queue is empty");
      KeyValuePair<TPriority, TValue> keyValuePair = this._baseHeap[0];
      this.DeleteRoot();
      return keyValuePair;
    }

    public TValue DequeueValue() => this.Dequeue().Value;

    public KeyValuePair<TPriority, TValue> Peek()
    {
      if (!this.IsEmpty)
        return this._baseHeap[0];
      throw new InvalidOperationException("Priority queue is empty");
    }

    public TValue PeekValue() => this.Peek().Value;

    public bool IsEmpty => this._baseHeap.Count == 0;

    private void ExchangeElements(int pos1, int pos2)
    {
      KeyValuePair<TPriority, TValue> keyValuePair = this._baseHeap[pos1];
      this._baseHeap[pos1] = this._baseHeap[pos2];
      this._baseHeap[pos2] = keyValuePair;
    }

    private void Insert(TPriority priority, TValue value)
    {
      this._baseHeap.Add(new KeyValuePair<TPriority, TValue>(priority, value));
      this.HeapifyFromEndToBeginning(this._baseHeap.Count - 1);
    }

    private int HeapifyFromEndToBeginning(int pos)
    {
      if (pos >= this._baseHeap.Count)
        return -1;
      IComparer<TPriority> comparer = this.Comparer;
      int num;
      for (; pos > 0; pos = num)
      {
        num = (pos - 1) / 2;
        if (comparer.Compare(this._baseHeap[num].Key, this._baseHeap[pos].Key) < 0)
          this.ExchangeElements(num, pos);
        else
          break;
      }
      return pos;
    }

    private void DeleteRoot()
    {
      if (this._baseHeap.Count <= 1)
      {
        this._baseHeap.Clear();
      }
      else
      {
        this._baseHeap[0] = this._baseHeap[this._baseHeap.Count - 1];
        this._baseHeap.RemoveAt(this._baseHeap.Count - 1);
        this.HeapifyFromBeginningToEnd(0);
      }
    }

    private void HeapifyFromBeginningToEnd(int pos)
    {
      if (pos >= this._baseHeap.Count)
        return;
      IComparer<TPriority> comparer = this.Comparer;
      while (true)
      {
        int num = pos;
        int index1 = 2 * pos + 1;
        int index2 = 2 * pos + 2;
        if (index1 < this._baseHeap.Count && comparer.Compare(this._baseHeap[num].Key, this._baseHeap[index1].Key) < 0)
          num = index1;
        if (index2 < this._baseHeap.Count && comparer.Compare(this._baseHeap[num].Key, this._baseHeap[index2].Key) < 0)
          num = index2;
        if (num != pos)
        {
          this.ExchangeElements(num, pos);
          pos = num;
        }
        else
          break;
      }
    }

    public void Add(KeyValuePair<TPriority, TValue> item) => this.Enqueue(item.Key, item.Value);

    public void Clear() => this._baseHeap.Clear();

    public bool Contains(KeyValuePair<TPriority, TValue> item) => this._baseHeap.Contains(item);

    public int Count => this._baseHeap.Count;

    public void CopyTo(KeyValuePair<TPriority, TValue>[] array, int arrayIndex) => this._baseHeap.CopyTo(array, arrayIndex);

    public bool IsReadOnly => false;

    public bool Remove(KeyValuePair<TPriority, TValue> item)
    {
      int num = this._baseHeap.IndexOf(item);
      if (num < 0)
        return false;
      this._baseHeap[num] = this._baseHeap[this._baseHeap.Count - 1];
      this._baseHeap.RemoveAt(this._baseHeap.Count - 1);
      if (this.HeapifyFromEndToBeginning(num) == num)
        this.HeapifyFromBeginningToEnd(num);
      return true;
    }

    public IEnumerator<KeyValuePair<TPriority, TValue>> GetEnumerator() => (IEnumerator<KeyValuePair<TPriority, TValue>>) this._baseHeap.GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator() => (IEnumerator) this.GetEnumerator();
  }
}
