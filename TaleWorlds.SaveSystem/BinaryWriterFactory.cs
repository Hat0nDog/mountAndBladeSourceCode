// Decompiled with JetBrains decompiler
// Type: TaleWorlds.SaveSystem.BinaryWriterFactory
// Assembly: TaleWorlds.SaveSystem, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 786EF53D-D2EC-43A3-8B8C-4717B7406D76
// Assembly location: E:\SteamLibrary\steamapps\common\Mount & Blade II Bannerlord\bin\Win64_Shipping_Client\TaleWorlds.SaveSystem.dll

using System.Collections.Generic;
using System.Threading;
using TaleWorlds.Library;

namespace TaleWorlds.SaveSystem
{
  internal static class BinaryWriterFactory
  {
    private static ThreadLocal<Stack<BinaryWriter>> _binaryWriters;

    public static BinaryWriter GetBinaryWriter()
    {
      if (BinaryWriterFactory._binaryWriters.Value == null)
      {
        BinaryWriterFactory._binaryWriters.Value = new Stack<BinaryWriter>();
        for (int index = 0; index < 5; ++index)
        {
          BinaryWriter binaryWriter = new BinaryWriter(4096);
          BinaryWriterFactory._binaryWriters.Value.Push(binaryWriter);
        }
      }
      Stack<BinaryWriter> binaryWriterStack = BinaryWriterFactory._binaryWriters.Value;
      return binaryWriterStack.Count != 0 ? binaryWriterStack.Pop() : new BinaryWriter(4096);
    }

    public static void ReleaseBinaryWriter(BinaryWriter writer)
    {
      if (BinaryWriterFactory._binaryWriters == null)
        return;
      writer.Clear();
      BinaryWriterFactory._binaryWriters.Value.Push(writer);
    }

    public static void Initialize() => BinaryWriterFactory._binaryWriters = new ThreadLocal<Stack<BinaryWriter>>();

    public static void Release() => BinaryWriterFactory._binaryWriters = (ThreadLocal<Stack<BinaryWriter>>) null;
  }
}
