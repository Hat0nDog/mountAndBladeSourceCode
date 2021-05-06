// Decompiled with JetBrains decompiler
// Type: TaleWorlds.Engine.IShader
// Assembly: TaleWorlds.Engine, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: DD7DC4E8-6EB6-4CB8-8DE1-FC78E45F7FE8
// Assembly location: E:\SteamLibrary\steamapps\common\Mount & Blade II Bannerlord\bin\Win64_Shipping_Client\TaleWorlds.Engine.dll

using System;
using TaleWorlds.Library;

namespace TaleWorlds.Engine
{
  [ApplicationInterfaceBase]
  internal interface IShader
  {
    [EngineMethod("get_from_resource", false)]
    Shader GetFromResource(string shaderName);

    [EngineMethod("get_name", false)]
    string GetName(UIntPtr shaderPointer);

    [EngineMethod("release", false)]
    void Release(UIntPtr shaderPointer);

    [EngineMethod("get_material_shader_flag_mask", false)]
    ulong GetMaterialShaderFlagMask(UIntPtr shaderPointer, string flagName, bool showError);
  }
}
