// Decompiled with JetBrains decompiler
// Type: TaleWorlds.SaveSystem.SaveableBasicTypeDefiner
// Assembly: TaleWorlds.SaveSystem, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 786EF53D-D2EC-43A3-8B8C-4717B7406D76
// Assembly location: E:\SteamLibrary\steamapps\common\Mount & Blade II Bannerlord\bin\Win64_Shipping_Client\TaleWorlds.SaveSystem.dll

using System;
using System.Collections.Generic;
using TaleWorlds.Library;
using TaleWorlds.SaveSystem.Definition;

namespace TaleWorlds.SaveSystem
{
  public class SaveableBasicTypeDefiner : SaveableTypeDefiner
  {
    public SaveableBasicTypeDefiner()
      : base(30000)
    {
    }

    protected internal override void DefineBasicTypes()
    {
      this.AddBasicTypeDefinition(typeof (int), 1, (IBasicTypeSerializer) new IntBasicTypeSerializer());
      this.AddBasicTypeDefinition(typeof (uint), 2, (IBasicTypeSerializer) new UintBasicTypeSerializer());
      this.AddBasicTypeDefinition(typeof (short), 3, (IBasicTypeSerializer) new ShortBasicTypeSerializer());
      this.AddBasicTypeDefinition(typeof (ushort), 4, (IBasicTypeSerializer) new UshortBasicTypeSerializer());
      this.AddBasicTypeDefinition(typeof (byte), 5, (IBasicTypeSerializer) new ByteBasicTypeSerializer());
      this.AddBasicTypeDefinition(typeof (sbyte), 6, (IBasicTypeSerializer) new SbyteBasicTypeSerializer());
      this.AddBasicTypeDefinition(typeof (float), 7, (IBasicTypeSerializer) new FloatBasicTypeSerializer());
      this.AddBasicTypeDefinition(typeof (double), 8, (IBasicTypeSerializer) new DoubleBasicTypeSerializer());
      this.AddBasicTypeDefinition(typeof (long), 9, (IBasicTypeSerializer) new LongBasicTypeSerializer());
      this.AddBasicTypeDefinition(typeof (ulong), 10, (IBasicTypeSerializer) new UlongBasicTypeSerializer());
      this.AddBasicTypeDefinition(typeof (Vec2), 11, (IBasicTypeSerializer) new Vec2BasicTypeSerializer());
      this.AddBasicTypeDefinition(typeof (Vec2i), 12, (IBasicTypeSerializer) new Vec2iBasicTypeSerializer());
      this.AddBasicTypeDefinition(typeof (Vec3), 13, (IBasicTypeSerializer) new Vec3BasicTypeSerializer());
      this.AddBasicTypeDefinition(typeof (Vec3i), 14, (IBasicTypeSerializer) new Vec3iBasicTypeSerializer());
      this.AddBasicTypeDefinition(typeof (Mat2), 15, (IBasicTypeSerializer) new Mat2BasicTypeSerializer());
      this.AddBasicTypeDefinition(typeof (Mat3), 16, (IBasicTypeSerializer) new Mat3BasicTypeSerializer());
      this.AddBasicTypeDefinition(typeof (MatrixFrame), 17, (IBasicTypeSerializer) new MatrixFrameBasicTypeSerializer());
      this.AddBasicTypeDefinition(typeof (Quaternion), 18, (IBasicTypeSerializer) new QuaternionBasicTypeSerializer());
      this.AddBasicTypeDefinition(typeof (Color), 19, (IBasicTypeSerializer) new ColorBasicTypeSerializer());
      this.AddBasicTypeDefinition(typeof (bool), 20, (IBasicTypeSerializer) new BoolBasicTypeSerializer());
      this.AddBasicTypeDefinition(typeof (string), 21, (IBasicTypeSerializer) new StringSerializer());
    }

    protected internal override void DefineClassTypes()
    {
      this.AddClassDefinition(typeof (object), 0);
      this.AddClassDefinitionWithCustomFields(typeof (Tuple<,>), 100, (IEnumerable<Tuple<string, short>>) new Tuple<string, short>[2]
      {
        new Tuple<string, short>("m_Item1", (short) 1),
        new Tuple<string, short>("m_Item2", (short) 2)
      });
      this.AddClassDefinitionWithCustomFields(typeof (PriorityQueue<,>), 103, (IEnumerable<Tuple<string, short>>) new Tuple<string, short>[1]
      {
        new Tuple<string, short>("_baseHeap", (short) 1)
      });
      this.AddClassDefinitionWithCustomFields(typeof (MBReadOnlyList<>), 104, (IEnumerable<Tuple<string, short>>) new Tuple<string, short>[1]
      {
        new Tuple<string, short>("_list", (short) 1)
      });
      this.AddClassDefinitionWithCustomFields(typeof (MBReadOnlyDictionary<,>), 105, (IEnumerable<Tuple<string, short>>) new Tuple<string, short>[1]
      {
        new Tuple<string, short>("_dictionary", (short) 1)
      });
      this.AddClassDefinition(typeof (GenericComparer<>), 106);
    }

    protected internal override void DefineStructTypes()
    {
      this.AddStructDefinition(typeof (Nullable<>), 101);
      this.AddStructDefinition(typeof (KeyValuePair<,>), 102);
      this.AddStructDefinitionWithCustomFields(typeof (ValueTuple<,>), 107, (IEnumerable<Tuple<string, short>>) new Tuple<string, short>[2]
      {
        new Tuple<string, short>("Item1", (short) 1),
        new Tuple<string, short>("Item2", (short) 2)
      });
    }

    protected internal override void DefineGenericStructDefinitions() => this.ConstructGenericStructDefinition(typeof (KeyValuePair<string, string>));

    protected internal override void DefineGenericClassDefinitions()
    {
      this.ConstructGenericClassDefinition(typeof (Tuple<string, int>));
      this.ConstructGenericClassDefinition(typeof (Tuple<bool, float>));
      this.ConstructGenericClassDefinition(typeof (GenericComparer<int>));
      this.ConstructGenericClassDefinition(typeof (GenericComparer<float>));
    }

    protected internal override void DefineContainerDefinitions()
    {
      this.ConstructContainerDefinition(typeof (List<int>));
      this.ConstructContainerDefinition(typeof (List<uint>));
      this.ConstructContainerDefinition(typeof (List<short>));
      this.ConstructContainerDefinition(typeof (List<ushort>));
      this.ConstructContainerDefinition(typeof (List<byte>));
      this.ConstructContainerDefinition(typeof (List<sbyte>));
      this.ConstructContainerDefinition(typeof (List<float>));
      this.ConstructContainerDefinition(typeof (List<double>));
      this.ConstructContainerDefinition(typeof (List<long>));
      this.ConstructContainerDefinition(typeof (List<ulong>));
      this.ConstructContainerDefinition(typeof (List<Vec2>));
      this.ConstructContainerDefinition(typeof (List<Vec2i>));
      this.ConstructContainerDefinition(typeof (List<Vec3>));
      this.ConstructContainerDefinition(typeof (List<Vec3i>));
      this.ConstructContainerDefinition(typeof (List<Mat2>));
      this.ConstructContainerDefinition(typeof (List<Mat3>));
      this.ConstructContainerDefinition(typeof (List<MatrixFrame>));
      this.ConstructContainerDefinition(typeof (List<Quaternion>));
      this.ConstructContainerDefinition(typeof (List<Color>));
      this.ConstructContainerDefinition(typeof (List<bool>));
      this.ConstructContainerDefinition(typeof (List<string>));
      this.ConstructContainerDefinition(typeof (List<KeyValuePair<string, string>>));
      this.ConstructContainerDefinition(typeof (List<Tuple<bool, float>>));
      this.ConstructContainerDefinition(typeof (Queue<int>));
      this.ConstructContainerDefinition(typeof (Queue<uint>));
      this.ConstructContainerDefinition(typeof (Queue<short>));
      this.ConstructContainerDefinition(typeof (Queue<ushort>));
      this.ConstructContainerDefinition(typeof (Queue<byte>));
      this.ConstructContainerDefinition(typeof (Queue<sbyte>));
      this.ConstructContainerDefinition(typeof (Queue<float>));
      this.ConstructContainerDefinition(typeof (Queue<double>));
      this.ConstructContainerDefinition(typeof (Queue<long>));
      this.ConstructContainerDefinition(typeof (Queue<ulong>));
      this.ConstructContainerDefinition(typeof (Queue<Vec2>));
      this.ConstructContainerDefinition(typeof (Queue<Vec2i>));
      this.ConstructContainerDefinition(typeof (Queue<Vec3>));
      this.ConstructContainerDefinition(typeof (Queue<Vec3i>));
      this.ConstructContainerDefinition(typeof (Queue<Mat2>));
      this.ConstructContainerDefinition(typeof (Queue<Mat3>));
      this.ConstructContainerDefinition(typeof (Queue<MatrixFrame>));
      this.ConstructContainerDefinition(typeof (Queue<Quaternion>));
      this.ConstructContainerDefinition(typeof (Queue<Color>));
      this.ConstructContainerDefinition(typeof (Queue<bool>));
      this.ConstructContainerDefinition(typeof (Queue<string>));
      this.ConstructContainerDefinition(typeof (int[]));
      this.ConstructContainerDefinition(typeof (uint[]));
      this.ConstructContainerDefinition(typeof (short[]));
      this.ConstructContainerDefinition(typeof (ushort[]));
      this.ConstructContainerDefinition(typeof (byte[]));
      this.ConstructContainerDefinition(typeof (sbyte[]));
      this.ConstructContainerDefinition(typeof (float[]));
      this.ConstructContainerDefinition(typeof (double[]));
      this.ConstructContainerDefinition(typeof (long[]));
      this.ConstructContainerDefinition(typeof (ulong[]));
      this.ConstructContainerDefinition(typeof (Vec2[]));
      this.ConstructContainerDefinition(typeof (Vec2i[]));
      this.ConstructContainerDefinition(typeof (Vec3[]));
      this.ConstructContainerDefinition(typeof (Vec3i[]));
      this.ConstructContainerDefinition(typeof (Mat2[]));
      this.ConstructContainerDefinition(typeof (Mat3[]));
      this.ConstructContainerDefinition(typeof (MatrixFrame[]));
      this.ConstructContainerDefinition(typeof (Quaternion[]));
      this.ConstructContainerDefinition(typeof (Color[]));
      this.ConstructContainerDefinition(typeof (bool[]));
      this.ConstructContainerDefinition(typeof (string[]));
      this.ConstructContainerDefinition(typeof (Dictionary<int, string>));
      this.ConstructContainerDefinition(typeof (Dictionary<string, int>));
      this.ConstructContainerDefinition(typeof (Dictionary<int, int>));
      this.ConstructContainerDefinition(typeof (Dictionary<string, string>));
      this.ConstructContainerDefinition(typeof (Dictionary<long, int>));
      this.ConstructContainerDefinition(typeof (Dictionary<string, object>));
      this.ConstructContainerDefinition(typeof (Dictionary<string, float>));
    }
  }
}
