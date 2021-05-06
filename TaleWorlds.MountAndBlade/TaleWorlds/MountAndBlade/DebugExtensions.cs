// Decompiled with JetBrains decompiler
// Type: TaleWorlds.MountAndBlade.DebugExtensions
// Assembly: TaleWorlds.MountAndBlade, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3C0E3F42-756F-4407-B817-06BA412A78B0
// Assembly location: E:\SteamLibrary\steamapps\common\Mount & Blade II Bannerlord\bin\Win64_Shipping_Client\TaleWorlds.MountAndBlade.dll

using TaleWorlds.Engine;
using TaleWorlds.Library;

namespace TaleWorlds.MountAndBlade
{
  public static class DebugExtensions
  {
    public static void RenderDebugCircleOnTerrain(
      Scene scene,
      MatrixFrame frame,
      float radius,
      uint color,
      bool depthCheck = true,
      bool isDotted = false)
    {
      MBAPI.IMBDebugExtensions.RenderDebugCircleOnTerrain(scene.Pointer, ref frame, radius, color, depthCheck, isDotted);
    }

    public static void RenderDebugArcOnTerrain(
      Scene scene,
      MatrixFrame frame,
      float radius,
      float beginAngle,
      float endAngle,
      uint color,
      bool depthCheck = true,
      bool isDotted = false)
    {
      MBAPI.IMBDebugExtensions.RenderDebugArcOnTerrain(scene.Pointer, ref frame, radius, beginAngle, endAngle, color, depthCheck, isDotted);
    }

    public static void RenderDebugLineOnTerrain(
      Scene scene,
      Vec3 position,
      Vec3 direction,
      uint color,
      bool depthCheck = true,
      float time = 0.0f,
      bool isDotted = false,
      float pointDensity = 1f)
    {
      MBAPI.IMBDebugExtensions.RenderDebugLineOnTerrain(scene.Pointer, position, direction, color, depthCheck, time, isDotted, pointDensity);
    }
  }
}
