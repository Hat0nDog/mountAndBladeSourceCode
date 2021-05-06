// Decompiled with JetBrains decompiler
// Type: TaleWorlds.Engine.IScriptComponent
// Assembly: TaleWorlds.Engine, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: DD7DC4E8-6EB6-4CB8-8DE1-FC78E45F7FE8
// Assembly location: E:\SteamLibrary\steamapps\common\Mount & Blade II Bannerlord\bin\Win64_Shipping_Client\TaleWorlds.Engine.dll

using System;
using TaleWorlds.Library;

namespace TaleWorlds.Engine
{
  [ApplicationInterfaceBase]
  internal interface IScriptComponent
  {
    [EngineMethod("get_script_component_behaviour", false)]
    ScriptComponentBehaviour GetScriptComponentBehaviour(UIntPtr pointer);

    [EngineMethod("set_variable_editor_widget_status", false)]
    void SetVariableEditorWidgetStatus(UIntPtr pointer, string field, bool enabled);
  }
}
