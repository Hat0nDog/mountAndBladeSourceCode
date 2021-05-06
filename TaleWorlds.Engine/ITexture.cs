// Decompiled with JetBrains decompiler
// Type: TaleWorlds.Engine.ITexture
// Assembly: TaleWorlds.Engine, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: DD7DC4E8-6EB6-4CB8-8DE1-FC78E45F7FE8
// Assembly location: E:\SteamLibrary\steamapps\common\Mount & Blade II Bannerlord\bin\Win64_Shipping_Client\TaleWorlds.Engine.dll

using System;
using TaleWorlds.Library;

namespace TaleWorlds.Engine
{
  [ApplicationInterfaceBase]
  internal interface ITexture
  {
    [EngineMethod("get_cur_object", false)]
    void GetCurObject(UIntPtr texturePointer);

    [EngineMethod("get_from_resource", false)]
    Texture GetFromResource(string textureName);

    [EngineMethod("load_texture_from_path", false)]
    Texture LoadTextureFromPath(string fileName, string folder);

    [EngineMethod("check_and_get_from_resource", false)]
    Texture CheckAndGetFromResource(string textureName);

    [EngineMethod("get_name", false)]
    string GetName(UIntPtr texturePointer);

    [EngineMethod("set_name", false)]
    void SetName(UIntPtr texturePointer, string name);

    [EngineMethod("get_width", false)]
    int GetWidth(UIntPtr texturePointer);

    [EngineMethod("get_height", false)]
    int GetHeight(UIntPtr texturePointer);

    [EngineMethod("get_memory_size", false)]
    int GetMemorySize(UIntPtr texturePointer);

    [EngineMethod("is_render_target", false)]
    bool IsRenderTarget(UIntPtr texturePointer);

    [EngineMethod("release", false)]
    void Release(UIntPtr texturePointer);

    [EngineMethod("create_render_target", false)]
    Texture CreateRenderTarget(
      string name,
      int width,
      int height,
      bool autoMipmaps,
      bool isTableau,
      bool createUninitialized,
      bool always_valid);

    [EngineMethod("create_depth_target", false)]
    Texture CreateDepthTarget(string name, int width, int height);

    [EngineMethod("create_from_byte_array", false)]
    Texture CreateFromByteArray(byte[] data, int width, int height);

    [EngineMethod("create_from_memory", false)]
    Texture CreateFromMemory(byte[] data);

    [EngineMethod("save_to_file", false)]
    void SaveToFile(UIntPtr texturePointer, string fileName);

    [EngineMethod("release_gpu_memories", false)]
    void ReleaseGpuMemories();

    [EngineMethod("transform_render_target_to_resource_texture", false)]
    void TransformRenderTargetToResourceTexture(UIntPtr texturePointer, string name);

    [EngineMethod("remove_continous_tableau_texture", false)]
    void RemoveContinousTableauTexture(UIntPtr texturePointer);

    [EngineMethod("set_tableau_view", false)]
    void SetTableauView(UIntPtr texturePointer, UIntPtr tableauView);

    [EngineMethod("create_texture_from_path", false)]
    Texture CreateTextureFromPath(string path, string fileName);

    [EngineMethod("get_render_target_component", false)]
    RenderTargetComponent GetRenderTargetComponent(UIntPtr texturePointer);

    [EngineMethod("get_tableau_view", false)]
    TableauView GetTableauView(UIntPtr texturePointer);
  }
}
