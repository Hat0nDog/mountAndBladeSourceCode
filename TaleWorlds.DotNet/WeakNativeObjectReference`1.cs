// Decompiled with JetBrains decompiler
// Type: TaleWorlds.DotNet.WeakNativeObjectReference`1
// Assembly: TaleWorlds.DotNet, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 207BAA99-20DA-4442-9622-4DB0CDEF3C0E
// Assembly location: E:\SteamLibrary\steamapps\common\Mount & Blade II Bannerlord\bin\Win64_Shipping_Client\TaleWorlds.DotNet.dll

using System;

namespace TaleWorlds.DotNet
{
  public sealed class WeakNativeObjectReference<T> where T : NativeObject
  {
    private readonly UIntPtr _pointer;
    private WeakReference<T> _weakReferenceCache;

    public WeakNativeObjectReference(T nativeObject)
    {
      if (!((NativeObject) nativeObject != (NativeObject) null))
        return;
      this._pointer = nativeObject.Pointer;
      this._weakReferenceCache = new WeakReference<T>(nativeObject);
    }

    public void ManualInvalidate()
    {
      T target;
      if (!this._weakReferenceCache.TryGetTarget(out target) || !((NativeObject) target != (NativeObject) null))
        return;
      target.ManualInvalidate();
    }

    public NativeObject GetNativeObject()
    {
      if (!(this._pointer != UIntPtr.Zero))
        return (NativeObject) null;
      T target;
      if (this._weakReferenceCache.TryGetTarget(out target) && (NativeObject) target != (NativeObject) null)
        return (NativeObject) target;
      T instance = (T) Activator.CreateInstance(typeof (T), (object) this._pointer);
      this._weakReferenceCache.SetTarget(instance);
      return (NativeObject) instance;
    }
  }
}
