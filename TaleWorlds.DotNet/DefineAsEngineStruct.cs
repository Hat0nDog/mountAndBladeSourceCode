// Decompiled with JetBrains decompiler
// Type: TaleWorlds.DotNet.DefineAsEngineStruct
// Assembly: TaleWorlds.DotNet, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 207BAA99-20DA-4442-9622-4DB0CDEF3C0E
// Assembly location: E:\SteamLibrary\steamapps\common\Mount & Blade II Bannerlord\bin\Win64_Shipping_Client\TaleWorlds.DotNet.dll

using System;

namespace TaleWorlds.DotNet
{
  [AttributeUsage(AttributeTargets.Assembly, AllowMultiple = true)]
  public class DefineAsEngineStruct : Attribute
  {
    public Type Type { get; set; }

    public string EngineType { get; set; }

    public DefineAsEngineStruct(Type type, string engineType)
    {
      this.Type = type;
      this.EngineType = engineType;
    }
  }
}
