// Decompiled with JetBrains decompiler
// Type: TaleWorlds.Engine.Screen
// Assembly: TaleWorlds.Engine, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: DD7DC4E8-6EB6-4CB8-8DE1-FC78E45F7FE8
// Assembly location: E:\SteamLibrary\steamapps\common\Mount & Blade II Bannerlord\bin\Win64_Shipping_Client\TaleWorlds.Engine.dll

using TaleWorlds.Library;

namespace TaleWorlds.Engine
{
  public static class Screen
  {
    public static float RealScreenResolutionWidth { get; private set; }

    public static float RealScreenResolutionHeight { get; private set; }

    public static Vec2 RealScreenResolution => new Vec2(Screen.RealScreenResolutionWidth, Screen.RealScreenResolutionHeight);

    public static float AspectRatio { get; private set; }

    public static Vec2 DesktopResolution { get; private set; }

    internal static void Update()
    {
      Screen.RealScreenResolutionWidth = EngineApplicationInterface.IScreen.GetRealScreenResolutionWidth();
      Screen.RealScreenResolutionHeight = EngineApplicationInterface.IScreen.GetRealScreenResolutionHeight();
      Screen.AspectRatio = EngineApplicationInterface.IScreen.GetAspectRatio();
      Screen.DesktopResolution = new Vec2(EngineApplicationInterface.IScreen.GetDesktopWidth(), EngineApplicationInterface.IScreen.GetDesktopHeight());
    }
  }
}
