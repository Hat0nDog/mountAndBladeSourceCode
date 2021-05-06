// Decompiled with JetBrains decompiler
// Type: TaleWorlds.Engine.IThumbnailCreatorView
// Assembly: TaleWorlds.Engine, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: DD7DC4E8-6EB6-4CB8-8DE1-FC78E45F7FE8
// Assembly location: E:\SteamLibrary\steamapps\common\Mount & Blade II Bannerlord\bin\Win64_Shipping_Client\TaleWorlds.Engine.dll

using System;
using TaleWorlds.Library;

namespace TaleWorlds.Engine
{
  [ApplicationInterfaceBase]
  internal interface IThumbnailCreatorView
  {
    [EngineMethod("create_thumbnail_creator_view", false)]
    ThumbnailCreatorView CreateThumbnailCreatorView();

    [EngineMethod("register_scene", false)]
    void RegisterScene(UIntPtr pointer, UIntPtr scene_ptr, bool use_postfx);

    [EngineMethod("register_entity", false)]
    void RegisterEntity(
      UIntPtr pointer,
      UIntPtr scene_ptr,
      UIntPtr cam_ptr,
      UIntPtr texture_ptr,
      UIntPtr entity_ptr,
      string render_id,
      int allocationGroupIndex);

    [EngineMethod("register_entity_without_texture", false)]
    void RegisterEntityWithoutTexture(
      UIntPtr pointer,
      UIntPtr scene_ptr,
      UIntPtr cam_ptr,
      UIntPtr entity_ptr,
      int width,
      int height,
      string render_id,
      string debug_name,
      int allocationGroupIndex);
  }
}
