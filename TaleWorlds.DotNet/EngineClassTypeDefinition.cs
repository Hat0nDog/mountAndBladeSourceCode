// Decompiled with JetBrains decompiler
// Type: TaleWorlds.DotNet.EngineClassTypeDefinition
// Assembly: TaleWorlds.DotNet, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 207BAA99-20DA-4442-9622-4DB0CDEF3C0E
// Assembly location: E:\SteamLibrary\steamapps\common\Mount & Blade II Bannerlord\bin\Win64_Shipping_Client\TaleWorlds.DotNet.dll

using System.Runtime.InteropServices;

namespace TaleWorlds.DotNet
{
  [EngineStruct("ftlObject_type_definition")]
  internal struct EngineClassTypeDefinition
  {
    public int TypeId;
    [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 128)]
    public string TypeName;
  }
}
