// Decompiled with JetBrains decompiler
// Type: TaleWorlds.Engine.ManagedScriptComponent
// Assembly: TaleWorlds.Engine, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: DD7DC4E8-6EB6-4CB8-8DE1-FC78E45F7FE8
// Assembly location: E:\SteamLibrary\steamapps\common\Mount & Blade II Bannerlord\bin\Win64_Shipping_Client\TaleWorlds.Engine.dll

using System;
using TaleWorlds.DotNet;

namespace TaleWorlds.Engine
{
  [EngineClass("rglManaged_script_component")]
  public sealed class ManagedScriptComponent : ScriptComponent
  {
    public ScriptComponentBehaviour ScriptComponentBehaviour => EngineApplicationInterface.IScriptComponent.GetScriptComponentBehaviour(this.Pointer);

    public void SetVariableEditorWidgetStatus(string field, bool enabled) => EngineApplicationInterface.IScriptComponent.SetVariableEditorWidgetStatus(this.Pointer, field, enabled);

    internal ManagedScriptComponent(UIntPtr pointer)
      : base(pointer)
    {
    }
  }
}
