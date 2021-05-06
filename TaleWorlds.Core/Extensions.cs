// Decompiled with JetBrains decompiler
// Type: TaleWorlds.Core.Extensions
// Assembly: TaleWorlds.Core, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 43F7B45C-9AE1-4715-A37B-2364840F62AF
// Assembly location: E:\SteamLibrary\steamapps\common\Mount & Blade II Bannerlord\bin\Win64_Shipping_Client\TaleWorlds.Core.dll

using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace TaleWorlds.Core
{
  public static class Extensions
  {
    public static string ToHexadecimalString(this uint number) => string.Format("{0:X}", (object) number);

    public static string Description(this Enum value)
    {
      object[] customAttributes = value.GetType().GetField(value.ToString()).GetCustomAttributes(typeof (DescriptionAttribute), false);
      return customAttributes.Length != 0 ? ((DescriptionAttribute) customAttributes[0]).Description : value.ToString();
    }

    public static float NextFloat(this Random random) => (float) random.NextDouble();

    public static TSource MaxBy<TSource, TKey>(
      this IEnumerable<TSource> source,
      Func<TSource, TKey> selector)
    {
      return source.MaxBy<TSource, TKey>(selector, (IComparer<TKey>) Comparer<TKey>.Default, out TKey _);
    }

    public static TSource MaxBy<TSource, TKey>(
      this IEnumerable<TSource> source,
      Func<TSource, TKey> selector,
      out TKey maxKey)
    {
      return source.MaxBy<TSource, TKey>(selector, (IComparer<TKey>) Comparer<TKey>.Default, out maxKey);
    }

    public static TSource MaxBy<TSource, TKey>(
      this IEnumerable<TSource> source,
      Func<TSource, TKey> selector,
      IComparer<TKey> comparer,
      out TKey maxKey)
    {
      if (source == null)
        throw new ArgumentNullException(nameof (source));
      if (selector == null)
        throw new ArgumentNullException(nameof (selector));
      if (comparer == null)
        throw new ArgumentNullException(nameof (comparer));
      using (IEnumerator<TSource> enumerator = source.GetEnumerator())
      {
        TSource source1 = enumerator.MoveNext() ? enumerator.Current : throw new InvalidOperationException("Sequence contains no elements");
        maxKey = selector(source1);
        while (enumerator.MoveNext())
        {
          TSource current = enumerator.Current;
          TKey x = selector(current);
          if (comparer.Compare(x, maxKey) > 0)
          {
            source1 = current;
            maxKey = x;
          }
        }
        return source1;
      }
    }

    public static TSource MinBy<TSource, TKey>(
      this IEnumerable<TSource> source,
      Func<TSource, TKey> selector)
    {
      return source.MinBy<TSource, TKey>(selector, (IComparer<TKey>) Comparer<TKey>.Default);
    }

    public static TSource MinBy<TSource, TKey>(
      this IEnumerable<TSource> source,
      Func<TSource, TKey> selector,
      IComparer<TKey> comparer)
    {
      if (source == null)
        throw new ArgumentNullException(nameof (source));
      if (selector == null)
        throw new ArgumentNullException(nameof (selector));
      if (comparer == null)
        throw new ArgumentNullException(nameof (comparer));
      using (IEnumerator<TSource> enumerator = source.GetEnumerator())
      {
        TSource source1 = enumerator.MoveNext() ? enumerator.Current : throw new InvalidOperationException("Sequence was empty");
        TKey y = selector(source1);
        while (enumerator.MoveNext())
        {
          TSource current = enumerator.Current;
          TKey x = selector(current);
          if (comparer.Compare(x, y) < 0)
          {
            source1 = current;
            y = x;
          }
        }
        return source1;
      }
    }

    public static IEnumerable<TSource> DistinctBy<TSource, TKey>(
      this IEnumerable<TSource> source,
      Func<TSource, TKey> keySelector)
    {
      return source.DistinctBy<TSource, TKey>(keySelector, (IEqualityComparer<TKey>) null);
    }

    public static IEnumerable<TSource> DistinctBy<TSource, TKey>(
      this IEnumerable<TSource> source,
      Func<TSource, TKey> keySelector,
      IEqualityComparer<TKey> comparer)
    {
      if (source == null)
        throw new ArgumentNullException(nameof (source));
      if (keySelector == null)
        throw new ArgumentNullException(nameof (keySelector));
      return Extensions.DistinctByImpl<TSource, TKey>(source, keySelector, comparer);
    }

    private static IEnumerable<TSource> DistinctByImpl<TSource, TKey>(
      IEnumerable<TSource> source,
      Func<TSource, TKey> keySelector,
      IEqualityComparer<TKey> comparer)
    {
      return source.GroupBy<TSource, TKey>(keySelector, comparer).Select<IGrouping<TKey, TSource>, TSource>((Func<IGrouping<TKey, TSource>, TSource>) (g => g.First<TSource>()));
    }

    public static string Add(this string str, string appendant, bool newLine = true)
    {
      if (str == null)
        str = "";
      str += appendant;
      if (newLine)
        str += "\n";
      return str;
    }

    public static IEnumerable<string> Split(this string str, int maxChunkSize)
    {
      for (int i = 0; i < str.Length; i += maxChunkSize)
        yield return str.Substring(i, Math.Min(maxChunkSize, str.Length - i));
    }

    public static BattleSideEnum GetOppositeSide(this BattleSideEnum side)
    {
      if (side == BattleSideEnum.Attacker)
        return BattleSideEnum.Defender;
      return side != BattleSideEnum.Defender ? side : BattleSideEnum.Attacker;
    }

    public static IEnumerable<IEnumerable<T>> Split<T>(
      this IEnumerable<T> source,
      int splitItemCount)
    {
      if (splitItemCount <= 0)
        throw new ArgumentException();
      int i = 0;
      return (IEnumerable<IEnumerable<T>>) source.GroupBy<T, int>((Func<T, int>) (x => i++ % splitItemCount));
    }

    public static bool IsEmpty<T>(this IEnumerable<T> source)
    {
      switch (source)
      {
        case ICollection<T> objs:
          return objs.Count == 0;
        case ICollection collection:
          return collection.Count == 0;
        default:
          return !source.Any<T>();
      }
    }

    public static void Shuffle<T>(this IList<T> list)
    {
      int count = list.Count;
      while (count > 1)
      {
        --count;
        int index = MBRandom.RandomInt(count + 1);
        T obj = list[index];
        list[index] = list[count];
        list[count] = obj;
      }
    }

    public static T GetRandomElement<T>(this IReadOnlyList<T> e) => e.Count == 0 ? default (T) : e[MBRandom.RandomInt(e.Count)];

    public static T GetRandomElement<T>(this T[] e) => e.Length == 0 ? default (T) : e[MBRandom.RandomInt(e.Length)];

    public static T GetRandomElementInefficiently<T>(this IEnumerable<T> e) => e.IsEmpty<T>() ? default (T) : e.ElementAt<T>(MBRandom.RandomInt(e.Count<T>()));

    public static T GetRandomElementWithPredicate<T>(
      this IReadOnlyList<T> e,
      Func<T, bool> predicate)
    {
      if (e.Count == 0)
        return default (T);
      int maxValue = 0;
      for (int index = 0; index < e.Count; ++index)
      {
        if (predicate(e[index]))
          ++maxValue;
      }
      if (maxValue == 0)
        return default (T);
      int num = MBRandom.RandomInt(maxValue);
      for (int index = 0; index < e.Count; ++index)
      {
        if (predicate(e[index]))
        {
          --num;
          if (num < 0)
            return e[index];
        }
      }
      return default (T);
    }

    public static T GetRandomElementWithPredicate<T>(
      this IReadOnlyList<T> e,
      Func<T, bool> predicate,
      int randomSeed)
    {
      if (e.Count == 0)
        return default (T);
      int maxValue = 0;
      for (int index = 0; index < e.Count; ++index)
      {
        if (predicate(e[index]))
          ++maxValue;
      }
      int num = new Random(randomSeed).Next(maxValue);
      for (int index = 0; index < e.Count; ++index)
      {
        if (predicate(e[index]))
        {
          --num;
          if (num < 0)
            return e[index];
        }
      }
      return default (T);
    }

    public static T GetRandomElementWeighted<T>(
      this IReadOnlyList<T> e,
      Func<T, float> randomWeights)
    {
      return e.Count != 0 ? MBRandom.ChooseWeighted<T>((IEnumerable<T>) e, randomWeights) : throw new ArgumentException();
    }

    public static List<Tuple<T1, T2>> CombineWith<T1, T2>(
      this IEnumerable<T1> list1,
      IEnumerable<T2> list2)
    {
      List<Tuple<T1, T2>> tupleList = new List<Tuple<T1, T2>>();
      foreach (T1 obj1 in list1)
      {
        foreach (T2 obj2 in list2)
          tupleList.Add(new Tuple<T1, T2>(obj1, obj2));
      }
      return tupleList;
    }
  }
}
