// Decompiled with JetBrains decompiler
// Type: TaleWorlds.MountAndBlade.IMBDebugExtensions
// Assembly: TaleWorlds.MountAndBlade, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3C0E3F42-756F-4407-B817-06BA412A78B0
// Assembly location: E:\SteamLibrary\steamapps\common\Mount & Blade II Bannerlord\bin\Win64_Shipping_Client\TaleWorlds.MountAndBlade.dll

using System;
using TaleWorlds.Library;

namespace TaleWorlds.MountAndBlade
{
  [ScriptingInterfaceBase]
  internal interface IMBDebugExtensions
  {
    [EngineMethod("render_debug_circle_on_terrain", false)]
    void RenderDebugCircleOnTerrain(
      UIntPtr scenePointer,
      ref MatrixFrame frame,
      float radius,
      uint color,
      bool depthCheck,
      bool isDotted);

    [EngineMethod("render_debug_arc_on_terrain", false)]
    void RenderDebugArcOnTerrain(
      UIntPtr scenePointer,
      ref MatrixFrame frame,
      float radius,
      float beginAngle,
      float endAngle,
      uint color,
      bool depthCheck,
      bool isDotted);

    [EngineMethod("render_debug_line_on_terrain", false)]
    void RenderDebugLineOnTerrain(
      UIntPtr scenePointer,
      Vec3 position,
      Vec3 direction,
      uint color,
      bool depthCheck,
      float time,
      bool isDotted,
      float pointDensity);

    [EngineMethod("override_native_parameter", false)]
    void OverrideNativeParameter(string paramName, float value);

    [EngineMethod("reload_native_parameters", false)]
    void ReloadNativeParameters();
  }
}
