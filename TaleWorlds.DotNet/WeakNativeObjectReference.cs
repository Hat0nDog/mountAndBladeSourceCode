// Decompiled with JetBrains decompiler
// Type: TaleWorlds.DotNet.WeakNativeObjectReference
// Assembly: TaleWorlds.DotNet, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 207BAA99-20DA-4442-9622-4DB0CDEF3C0E
// Assembly location: E:\SteamLibrary\steamapps\common\Mount & Blade II Bannerlord\bin\Win64_Shipping_Client\TaleWorlds.DotNet.dll

using System;
using System.Reflection;

namespace TaleWorlds.DotNet
{
  public sealed class WeakNativeObjectReference
  {
    private readonly UIntPtr _pointer;
    private readonly Type _type;
    private readonly ConstructorInfo _constructorInfo;
    private WeakReference _weakReferenceCache;

    public WeakNativeObjectReference(NativeObject nativeObject)
    {
      if (!(nativeObject != (NativeObject) null))
        return;
      this._pointer = nativeObject.Pointer;
      this._type = nativeObject.GetType();
      this._constructorInfo = this._type.GetConstructor(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic, (Binder) null, new Type[1]
      {
        typeof (UIntPtr)
      }, (ParameterModifier[]) null);
      this._weakReferenceCache = new WeakReference((object) nativeObject);
    }

    public void ManualInvalidate()
    {
      NativeObject target = (NativeObject) this._weakReferenceCache.Target;
      if (!(target != (NativeObject) null))
        return;
      target.ManualInvalidate();
    }

    public NativeObject GetNativeObject()
    {
      if (!(this._pointer != UIntPtr.Zero))
        return (NativeObject) null;
      NativeObject target = (NativeObject) this._weakReferenceCache.Target;
      if (target != (NativeObject) null)
        return target;
      NativeObject nativeObject = (NativeObject) this._constructorInfo.Invoke(new object[1]
      {
        (object) this._pointer
      });
      this._weakReferenceCache.Target = (object) nativeObject;
      return nativeObject;
    }
  }
}
