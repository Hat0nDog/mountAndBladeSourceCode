// Decompiled with JetBrains decompiler
// Type: TaleWorlds.Engine.Resource
// Assembly: TaleWorlds.Engine, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: DD7DC4E8-6EB6-4CB8-8DE1-FC78E45F7FE8
// Assembly location: E:\SteamLibrary\steamapps\common\Mount & Blade II Bannerlord\bin\Win64_Shipping_Client\TaleWorlds.Engine.dll

using System;
using System.Diagnostics;
using TaleWorlds.DotNet;

namespace TaleWorlds.Engine
{
  [EngineClass("rglResource")]
  public abstract class Resource : NativeObject
  {
    public bool IsValid => this.Pointer != UIntPtr.Zero;

    internal Resource(UIntPtr ptr)
      : base(ptr)
    {
    }

    [Conditional("TRACE")]
    protected void CheckResourceParameter(Resource param, string paramName = "")
    {
      if ((NativeObject) param == (NativeObject) null)
        throw new NullReferenceException(paramName);
      if (!param.IsValid)
        throw new ArgumentException(paramName);
    }
  }
}
