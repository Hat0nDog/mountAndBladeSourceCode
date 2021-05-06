// Decompiled with JetBrains decompiler
// Type: TaleWorlds.DotNet.NativeArray
// Assembly: TaleWorlds.DotNet, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 207BAA99-20DA-4442-9622-4DB0CDEF3C0E
// Assembly location: E:\SteamLibrary\steamapps\common\Mount & Blade II Bannerlord\bin\Win64_Shipping_Client\TaleWorlds.DotNet.dll

using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace TaleWorlds.DotNet
{
  [EngineClass("ftdnNative_array")]
  public sealed class NativeArray : NativeObject
  {
    private static readonly IntPtr _temporaryData = Marshal.AllocHGlobal(16384);
    private const int TemporaryDataSize = 16384;
    private static readonly int DataPointerOffset = LibraryApplicationInterface.INativeArray.GetDataPointerOffset();

    internal NativeArray(UIntPtr pointer)
      : base(pointer)
    {
    }

    public static NativeArray Create() => LibraryApplicationInterface.INativeArray.Create();

    public int DataSize => LibraryApplicationInterface.INativeArray.GetDataSize(this.Pointer);

    private UIntPtr GetDataPointer() => LibraryApplicationInterface.INativeArray.GetDataPointer(this.Pointer);

    public int GetLength<T>() where T : struct => this.DataSize / Marshal.SizeOf<T>();

    public T GetElementAt<T>(int index) where T : struct
    {
      IntPtr structure = Marshal.PtrToStructure<IntPtr>(new IntPtr((long) this.Pointer.ToUInt64() + (long) NativeArray.DataPointerOffset));
      int num = Marshal.SizeOf<T>();
      return Marshal.PtrToStructure<T>(new IntPtr(structure.ToInt64() + (long) (index * num)));
    }

    public IEnumerable<T> GetEnumerator<T>() where T : struct
    {
      NativeArray nativeArray = this;
      int length = nativeArray.GetLength<T>();
      IntPtr dataPointer = Marshal.PtrToStructure<IntPtr>(new IntPtr((long) nativeArray.Pointer.ToUInt64() + (long) NativeArray.DataPointerOffset));
      int elementSize = Marshal.SizeOf<T>();
      for (int i = 0; i < length; ++i)
        yield return Marshal.PtrToStructure<T>(new IntPtr(dataPointer.ToInt64() + (long) (i * elementSize)));
    }

    public T[] ToArray<T>() where T : struct
    {
      T[] objArray = new T[this.GetLength<T>()];
      IEnumerable<T> enumerator = this.GetEnumerator<T>();
      int index = 0;
      foreach (T obj in enumerator)
      {
        objArray[index] = obj;
        ++index;
      }
      return objArray;
    }

    public void AddElement(int value) => LibraryApplicationInterface.INativeArray.AddIntegerElement(this.Pointer, value);

    public void AddElement(float value) => LibraryApplicationInterface.INativeArray.AddFloatElement(this.Pointer, value);

    public void AddElement<T>(T value) where T : struct
    {
      int elementSize = Marshal.SizeOf<T>();
      Marshal.StructureToPtr<T>(value, NativeArray._temporaryData, false);
      LibraryApplicationInterface.INativeArray.AddElement(this.Pointer, NativeArray._temporaryData, elementSize);
    }

    public void Clear() => LibraryApplicationInterface.INativeArray.Clear(this.Pointer);
  }
}
