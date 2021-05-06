// Decompiled with JetBrains decompiler
// Type: TaleWorlds.Library.TWParallel
// Assembly: TaleWorlds.Library, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 271D0DB8-3C5D-4850-A565-542B4CF7D347
// Assembly location: E:\SteamLibrary\steamapps\common\Mount & Blade II Bannerlord\bin\Win64_Shipping_Client\TaleWorlds.Library.dll

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace TaleWorlds.Library
{
  public static class TWParallel
  {
    public static ParallelLoopResult ForEach<TSource>(
      IEnumerable<TSource> source,
      Action<TSource> body)
    {
      return Parallel.ForEach<TSource>((Partitioner<TSource>) Partitioner.Create<TSource>(source), Common.ParallelOptions, body);
    }

    public static ParallelLoopResult For(
      int fromInclusive,
      int toExclusive,
      Action<int> body)
    {
      return Parallel.ForEach<Tuple<int, int>>((Partitioner<Tuple<int, int>>) Partitioner.Create(fromInclusive, toExclusive), Common.ParallelOptions, (Action<Tuple<int, int>, ParallelLoopState>) ((range, loopState) =>
      {
        for (int index = range.Item1; index < range.Item2; ++index)
          body(index);
      }));
    }
  }
}
