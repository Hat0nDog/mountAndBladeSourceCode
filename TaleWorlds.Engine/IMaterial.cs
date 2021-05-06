// Decompiled with JetBrains decompiler
// Type: TaleWorlds.Engine.IMaterial
// Assembly: TaleWorlds.Engine, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: DD7DC4E8-6EB6-4CB8-8DE1-FC78E45F7FE8
// Assembly location: E:\SteamLibrary\steamapps\common\Mount & Blade II Bannerlord\bin\Win64_Shipping_Client\TaleWorlds.Engine.dll

using System;
using TaleWorlds.Library;

namespace TaleWorlds.Engine
{
  [ApplicationInterfaceBase]
  internal interface IMaterial
  {
    [EngineMethod("create_copy", false)]
    Material CreateCopy(UIntPtr materialPointer);

    [EngineMethod("get_from_resource", false)]
    Material GetFromResource(string materialName);

    [EngineMethod("get_default_material", false)]
    Material GetDefaultMaterial();

    [EngineMethod("get_outline_material", false)]
    Material GetOutlineMaterial(UIntPtr materialPointer);

    [EngineMethod("get_name", false)]
    string GetName(UIntPtr materialPointer);

    [EngineMethod("set_name", false)]
    void SetName(UIntPtr materialPointer, string name);

    [EngineMethod("get_alpha_blend_mode", false)]
    int GetAlphaBlendMode(UIntPtr materialPointer);

    [EngineMethod("set_alpha_blend_mode", false)]
    void SetAlphaBlendMode(UIntPtr materialPointer, int alphaBlendMode);

    [EngineMethod("release", false)]
    void Release(UIntPtr materialPointer);

    [EngineMethod("set_shader", false)]
    void SetShader(UIntPtr materialPointer, UIntPtr shaderPointer);

    [EngineMethod("get_shader", false)]
    Shader GetShader(UIntPtr materialPointer);

    [EngineMethod("get_shader_flags", false)]
    ulong GetShaderFlags(UIntPtr materialPointer);

    [EngineMethod("set_shader_flags", false)]
    void SetShaderFlags(UIntPtr materialPointer, ulong shaderFlags);

    [EngineMethod("set_mesh_vector_argument", false)]
    void SetMeshVectorArgument(UIntPtr materialPointer, float x, float y, float z, float w);

    [EngineMethod("set_texture", false)]
    void SetTexture(UIntPtr materialPointer, int textureType, UIntPtr texturePointer);

    [EngineMethod("set_texture_at_slot", false)]
    void SetTextureAtSlot(UIntPtr materialPointer, int textureSlotIndex, UIntPtr texturePointer);

    [EngineMethod("get_texture", false)]
    Texture GetTexture(UIntPtr materialPointer, int textureType);

    [EngineMethod("set_alpha_test_value", false)]
    void SetAlphaTestValue(UIntPtr materialPointer, float alphaTestValue);

    [EngineMethod("get_alpha_test_value", false)]
    float GetAlphaTestValue(UIntPtr materialPointer);

    [EngineMethod("get_flags", false)]
    uint GetFlags(UIntPtr materialPointer);

    [EngineMethod("set_flags", false)]
    void SetFlags(UIntPtr materialPointer, uint flags);

    [EngineMethod("add_material_shader_flag", false)]
    void AddMaterialShaderFlag(UIntPtr materialPointer, string flagName, bool showErrors);

    [EngineMethod("set_area_map_scale", false)]
    void SetAreaMapScale(UIntPtr materialPointer, float scale);

    [EngineMethod("set_enable_skinning", false)]
    void SetEnableSkinning(UIntPtr materialPointer, bool enable);

    [EngineMethod("using_skinning", false)]
    bool UsingSkinning(UIntPtr materialPointer);
  }
}
