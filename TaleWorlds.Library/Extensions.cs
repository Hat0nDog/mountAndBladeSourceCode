// Decompiled with JetBrains decompiler
// Type: TaleWorlds.Library.Extensions
// Assembly: TaleWorlds.Library, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 271D0DB8-3C5D-4850-A565-542B4CF7D347
// Assembly location: E:\SteamLibrary\steamapps\common\Mount & Blade II Bannerlord\bin\Win64_Shipping_Client\TaleWorlds.Library.dll

using System;
using System.Collections.Generic;

namespace TaleWorlds.Library
{
  public static class Extensions
  {
    public static void AppendList<T>(this List<T> list1, List<T> list2)
    {
      if (list1.Count + list2.Count > list1.Capacity)
        list1.Capacity = list1.Count + list2.Count;
      for (int index = 0; index < list2.Count; ++index)
        list1.Add(list2[index]);
    }

    public static MBReadOnlyList<T> GetReadOnlyList<T>(this List<T> list) => new MBReadOnlyList<T>(list);

    public static MBReadOnlyDictionary<TKey, TValue> GetReadOnlyDictionary<TKey, TValue>(
      this Dictionary<TKey, TValue> dictionary)
    {
      return new MBReadOnlyDictionary<TKey, TValue>(dictionary);
    }

    public static bool HasAnyFlag<T>(this T p1, T p2) where T : struct => EnumHelper<T>.HasAnyFlag(p1, p2);

    public static bool HasAllFlags<T>(this T p1, T p2) where T : struct => EnumHelper<T>.HasAllFlags(p1, p2);

    public static bool IsStringNoneOrEmpty(this string str) => str == "none" || string.IsNullOrEmpty(str);

    public static int GetDeterministicHashCode(this string text)
    {
      int num = 5381;
      for (int index = 0; index < text.Length; ++index)
        num = (num << 5) + num + (int) text[index];
      return num;
    }

    public static int IndexOfMin<TSource>(this IReadOnlyList<TSource> self, Func<TSource, int> func)
    {
      int num1 = int.MaxValue;
      int num2 = -1;
      for (int index = 0; index < self.Count; ++index)
      {
        int num3 = func(self[index]);
        if (num3 < num1)
        {
          num1 = num3;
          num2 = index;
        }
      }
      return num2;
    }

    public static int IndexOfMax<TSource>(this IReadOnlyList<TSource> self, Func<TSource, int> func)
    {
      int num1 = int.MinValue;
      int num2 = -1;
      for (int index = 0; index < self.Count; ++index)
      {
        int num3 = func(self[index]);
        if (num3 > num1)
        {
          num1 = num3;
          num2 = index;
        }
      }
      return num2;
    }

    public static int FindIndex<TValue>(
      this IReadOnlyList<TValue> source,
      Func<TValue, bool> predicate)
    {
      for (int index = 0; index < source.Count; ++index)
      {
        if (predicate(source[index]))
          return index;
      }
      return -1;
    }

    public static int IndexOf<TValue>(this IReadOnlyList<TValue> source, TValue obj) where TValue : class
    {
      for (int index = 0; index < source.Count; ++index)
      {
        if (obj.Equals((object) source[index]))
          return index;
      }
      return -1;
    }

    public static int FindLastIndex<TValue>(
      this IReadOnlyList<TValue> source,
      Func<TValue, bool> predicate)
    {
      for (int index = source.Count - 1; index >= 0; --index)
      {
        if (predicate(source[index]))
          return index;
      }
      return -1;
    }

    public static void Randomize<T>(this IList<T> array)
    {
      Random random = new Random();
      int count = array.Count;
      while (count > 1)
      {
        --count;
        int index = random.Next(0, count + 1);
        T obj = array[index];
        array[index] = array[count];
        array[count] = obj;
      }
    }
  }
}
