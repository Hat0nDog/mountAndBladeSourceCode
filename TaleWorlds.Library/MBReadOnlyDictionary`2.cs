// Decompiled with JetBrains decompiler
// Type: TaleWorlds.Library.MBReadOnlyDictionary`2
// Assembly: TaleWorlds.Library, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 271D0DB8-3C5D-4850-A565-542B4CF7D347
// Assembly location: E:\SteamLibrary\steamapps\common\Mount & Blade II Bannerlord\bin\Win64_Shipping_Client\TaleWorlds.Library.dll

using System;
using System.Collections;
using System.Collections.Generic;

namespace TaleWorlds.Library
{
  [Serializable]
  public class MBReadOnlyDictionary<TKey, TValue> : 
    ICollection,
    IEnumerable,
    IReadOnlyDictionary<TKey, TValue>,
    IEnumerable<KeyValuePair<TKey, TValue>>,
    IReadOnlyCollection<KeyValuePair<TKey, TValue>>
  {
    private Dictionary<TKey, TValue> _dictionary;

    public MBReadOnlyDictionary(Dictionary<TKey, TValue> dictionary) => this._dictionary = dictionary;

    public int Count => this._dictionary.Count;

    public bool IsSynchronized => false;

    public object SyncRoot => (object) null;

    public Dictionary<TKey, TValue>.Enumerator GetEnumerator() => this._dictionary.GetEnumerator();

    IEnumerator<KeyValuePair<TKey, TValue>> IEnumerable<KeyValuePair<TKey, TValue>>.GetEnumerator() => (IEnumerator<KeyValuePair<TKey, TValue>>) this._dictionary.GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator() => (IEnumerator) this._dictionary.GetEnumerator();

    public bool ContainsKey(TKey key) => this._dictionary.ContainsKey(key);

    public bool TryGetValue(TKey key, out TValue value) => this._dictionary.TryGetValue(key, out value);

    public TValue this[TKey key] => this._dictionary[key];

    public IEnumerable<TKey> Keys => (IEnumerable<TKey>) this._dictionary.Keys;

    public IEnumerable<TValue> Values => (IEnumerable<TValue>) this._dictionary.Values;

    public void CopyTo(Array array, int index)
    {
      switch (array)
      {
        case KeyValuePair<TKey, TValue>[] keyValuePairArray:
          ((ICollection) this._dictionary).CopyTo((Array) keyValuePairArray, index);
          break;
        case DictionaryEntry[] dictionaryEntryArray:
          using (Dictionary<TKey, TValue>.Enumerator enumerator = this._dictionary.GetEnumerator())
          {
            while (enumerator.MoveNext())
            {
              KeyValuePair<TKey, TValue> current = enumerator.Current;
              int index1 = index++;
              DictionaryEntry dictionaryEntry = new DictionaryEntry((object) current.Key, (object) current.Value);
              dictionaryEntryArray[index1] = dictionaryEntry;
            }
            break;
          }
        default:
          object[] objArray = array as object[];
          try
          {
            using (Dictionary<TKey, TValue>.Enumerator enumerator = this._dictionary.GetEnumerator())
            {
              while (enumerator.MoveNext())
              {
                KeyValuePair<TKey, TValue> current = enumerator.Current;
                objArray[index++] = (object) new KeyValuePair<TKey, TValue>(current.Key, current.Value);
              }
              break;
            }
          }
          catch (ArrayTypeMismatchException ex)
          {
            break;
          }
      }
    }
  }
}
