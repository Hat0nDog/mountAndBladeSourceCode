// Decompiled with JetBrains decompiler
// Type: TaleWorlds.Engine.IView
// Assembly: TaleWorlds.Engine, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: DD7DC4E8-6EB6-4CB8-8DE1-FC78E45F7FE8
// Assembly location: E:\SteamLibrary\steamapps\common\Mount & Blade II Bannerlord\bin\Win64_Shipping_Client\TaleWorlds.Engine.dll

using System;
using TaleWorlds.Library;

namespace TaleWorlds.Engine
{
  [ApplicationInterfaceBase]
  internal interface IView
  {
    [EngineMethod("set_render_option", false)]
    void SetRenderOption(UIntPtr ptr, int optionEnum, bool value);

    [EngineMethod("set_render_order", false)]
    void SetRenderOrder(UIntPtr ptr, int value);

    [EngineMethod("set_render_target", false)]
    void SetRenderTarget(UIntPtr ptr, UIntPtr texture_ptr);

    [EngineMethod("set_depth_target", false)]
    void SetDepthTarget(UIntPtr ptr, UIntPtr texture_ptr);

    [EngineMethod("set_scale", false)]
    void SetScale(UIntPtr ptr, float x, float y);

    [EngineMethod("set_offset", false)]
    void SetOffset(UIntPtr ptr, float x, float y);

    [EngineMethod("set_debug_render_functionality", false)]
    void SetDebugRenderFunctionality(UIntPtr ptr, bool value);

    [EngineMethod("set_clear_color", false)]
    void SetClearColor(UIntPtr ptr, uint rgba);

    [EngineMethod("set_enable", false)]
    void SetEnable(UIntPtr ptr, bool value);

    [EngineMethod("set_render_on_demand", false)]
    void SetRenderOnDemand(UIntPtr ptr, bool value);

    [EngineMethod("set_auto_depth_creation", false)]
    void SetAutoDepthTargetCreation(UIntPtr ptr, bool value);

    [EngineMethod("set_save_final_result_to_disk", false)]
    void SetSaveFinalResultToDisk(UIntPtr ptr, bool value);

    [EngineMethod("set_file_name_to_save_result", false)]
    void SetFileNameToSaveResult(UIntPtr ptr, string name);

    [EngineMethod("set_file_type_to_save", false)]
    void SetFileTypeToSave(UIntPtr ptr, int type);

    [EngineMethod("set_file_path_to_save_result", false)]
    void SetFilePathToSaveResult(UIntPtr ptr, string name);
  }
}
