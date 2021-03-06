// Decompiled with JetBrains decompiler
// Type: TaleWorlds.DotNet.EngineStruct
// Assembly: TaleWorlds.DotNet, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 207BAA99-20DA-4442-9622-4DB0CDEF3C0E
// Assembly location: E:\SteamLibrary\steamapps\common\Mount & Blade II Bannerlord\bin\Win64_Shipping_Client\TaleWorlds.DotNet.dll

using System;

namespace TaleWorlds.DotNet
{
  public class EngineStruct : Attribute
  {
    public string EngineType { get; set; }

    public string AlternateDotNetType { get; set; }

    public EngineStruct(string engineType)
    {
      this.EngineType = engineType;
      this.AlternateDotNetType = (string) null;
    }

    public EngineStruct(string engineType, string alternateDotNetType)
    {
      this.EngineType = engineType;
      this.AlternateDotNetType = alternateDotNetType;
    }
  }
}
