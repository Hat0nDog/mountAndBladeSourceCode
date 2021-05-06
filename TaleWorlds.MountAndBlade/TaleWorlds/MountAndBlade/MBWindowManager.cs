// Decompiled with JetBrains decompiler
// Type: TaleWorlds.MountAndBlade.MBWindowManager
// Assembly: TaleWorlds.MountAndBlade, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3C0E3F42-756F-4407-B817-06BA412A78B0
// Assembly location: E:\SteamLibrary\steamapps\common\Mount & Blade II Bannerlord\bin\Win64_Shipping_Client\TaleWorlds.MountAndBlade.dll

using TaleWorlds.Engine;
using TaleWorlds.Engine.Screens;
using TaleWorlds.Library;

namespace TaleWorlds.MountAndBlade
{
  public class MBWindowManager
  {
    public static float WorldToScreen(
      Camera camera,
      Vec3 worldSpacePosition,
      ref float screenX,
      ref float screenY,
      ref float w)
    {
      return MBAPI.IMBWindowManager.WorldToScreen(camera.Pointer, worldSpacePosition, ref screenX, ref screenY, ref w);
    }

    public static float WorldToScreenInsideUsableArea(
      Camera camera,
      Vec3 worldSpacePosition,
      ref float screenX,
      ref float screenY,
      ref float w)
    {
      double screen = (double) MBAPI.IMBWindowManager.WorldToScreen(camera.Pointer, worldSpacePosition, ref screenX, ref screenY, ref w);
      screenX -= (float) (((double) Screen.RealScreenResolutionWidth - (double) ScreenManager.UsableArea.X * (double) Screen.RealScreenResolutionWidth) / 2.0);
      screenY -= (float) (((double) Screen.RealScreenResolutionHeight - (double) ScreenManager.UsableArea.Y * (double) Screen.RealScreenResolutionHeight) / 2.0);
      return (float) screen;
    }

    public static float WorldToScreenWithFixedZ(
      Camera camera,
      Vec3 cameraPosition,
      Vec3 worldSpacePosition,
      ref float screenX,
      ref float screenY,
      ref float w)
    {
      return MBAPI.IMBWindowManager.WorldToScreenWithFixedZ(camera.Pointer, cameraPosition, worldSpacePosition, ref screenX, ref screenY, ref w);
    }

    public static void ScreenToWorld(
      Camera camera,
      float screenX,
      float screenY,
      float w,
      ref Vec3 worldSpacePosition)
    {
      MBAPI.IMBWindowManager.ScreenToWorld(camera.Pointer, screenX, screenY, w, ref worldSpacePosition);
    }

    public static void PreDisplay() => MBAPI.IMBWindowManager.PreDisplay();

    public static void DontChangeCursorPos() => MBAPI.IMBWindowManager.DontChangeCursorPos();
  }
}
