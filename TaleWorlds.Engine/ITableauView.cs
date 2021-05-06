// Decompiled with JetBrains decompiler
// Type: TaleWorlds.Engine.ITableauView
// Assembly: TaleWorlds.Engine, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: DD7DC4E8-6EB6-4CB8-8DE1-FC78E45F7FE8
// Assembly location: E:\SteamLibrary\steamapps\common\Mount & Blade II Bannerlord\bin\Win64_Shipping_Client\TaleWorlds.Engine.dll

using System;
using TaleWorlds.Library;

namespace TaleWorlds.Engine
{
  [ApplicationInterfaceBase]
  internal interface ITableauView
  {
    [EngineMethod("create_tableau_view", false)]
    TableauView CreateTableauView();

    [EngineMethod("set_sort_meshes", false)]
    void SetSortingEnabled(UIntPtr pointer, bool value);

    [EngineMethod("set_continous_rendering", false)]
    void SetContinousRendering(UIntPtr pointer, bool value);

    [EngineMethod("set_do_not_render_this_frame", false)]
    void SetDoNotRenderThisFrame(UIntPtr pointer, bool value);

    [EngineMethod("set_delete_after_rendering", false)]
    void SetDeleteAfterRendering(UIntPtr pointer, bool value);

    [EngineMethod("add_clear_task", false)]
    void AddClearTask(UIntPtr pointer, bool clear_only_scene_view);
  }
}
