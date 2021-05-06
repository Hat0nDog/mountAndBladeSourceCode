// Decompiled with JetBrains decompiler
// Type: TaleWorlds.Library.PinnedArrayData`1
// Assembly: TaleWorlds.Library, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 271D0DB8-3C5D-4850-A565-542B4CF7D347
// Assembly location: E:\SteamLibrary\steamapps\common\Mount & Blade II Bannerlord\bin\Win64_Shipping_Client\TaleWorlds.Library.dll

using System;
using System.Runtime.InteropServices;

namespace TaleWorlds.Library
{
  public struct PinnedArrayData<T>
  {
    private static IntPtr _unmanagedCache = Marshal.AllocHGlobal(16384);

    public bool Pinned { get; private set; }

    public IntPtr Pointer { get; private set; }

    public T[] Array { get; private set; }

    public T[,] Array2D { get; private set; }

    public GCHandle Handle { get; private set; }

    public PinnedArrayData(T[] array, bool manualPinning = false)
    {
      this.Array = array;
      this.Array2D = (T[,]) null;
      this.Pinned = false;
      this.Pointer = IntPtr.Zero;
      if (array == null)
        return;
      if (!manualPinning)
      {
        try
        {
          this.Handle = GCHandle.Alloc((object) array, GCHandleType.Pinned);
          this.Pointer = this.Handle.AddrOfPinnedObject();
          this.Pinned = true;
        }
        catch (ArgumentException ex)
        {
          manualPinning = true;
        }
      }
      if (!manualPinning)
        return;
      this.Pinned = false;
      int num = Marshal.SizeOf<T>();
      for (int index = 0; index < array.Length; ++index)
        Marshal.StructureToPtr<T>(array[index], PinnedArrayData<T>._unmanagedCache + num * index, false);
      this.Pointer = PinnedArrayData<T>._unmanagedCache;
    }

    public PinnedArrayData(T[,] array, bool manualPinning = false)
    {
      this.Array = (T[]) null;
      this.Array2D = array;
      this.Pinned = false;
      this.Pointer = IntPtr.Zero;
      if (array == null)
        return;
      if (!manualPinning)
      {
        try
        {
          this.Handle = GCHandle.Alloc((object) array, GCHandleType.Pinned);
          this.Pointer = this.Handle.AddrOfPinnedObject();
          this.Pinned = true;
        }
        catch (ArgumentException ex)
        {
          manualPinning = true;
        }
      }
      if (!manualPinning)
        return;
      this.Pinned = false;
      int num = Marshal.SizeOf<T>();
      for (int index1 = 0; index1 < array.GetLength(0); ++index1)
      {
        for (int index2 = 0; index2 < array.GetLength(1); ++index2)
          Marshal.StructureToPtr<T>(array[index1, index2], PinnedArrayData<T>._unmanagedCache + num * (index1 * array.GetLength(1) + index2), false);
      }
      this.Pointer = PinnedArrayData<T>._unmanagedCache;
    }

    public static bool CheckIfTypeRequiresManualPinning(Type type)
    {
      bool flag = false;
      System.Array instance = System.Array.CreateInstance(type, 10);
      GCHandle gcHandle;
      try
      {
        gcHandle = GCHandle.Alloc((object) instance, GCHandleType.Pinned);
        gcHandle.AddrOfPinnedObject();
      }
      catch (ArgumentException ex)
      {
        flag = true;
      }
      if (gcHandle.IsAllocated)
        gcHandle.Free();
      return flag;
    }

    public void Dispose()
    {
      if (!this.Pinned)
        return;
      if (this.Array != null)
      {
        this.Handle.Free();
        this.Array = (T[]) null;
        this.Pointer = IntPtr.Zero;
      }
      else
      {
        if (this.Array2D == null)
          return;
        this.Handle.Free();
        this.Array2D = (T[,]) null;
        this.Pointer = IntPtr.Zero;
      }
    }
  }
}
