// Decompiled with JetBrains decompiler
// Type: TaleWorlds.Engine.IScreen
// Assembly: TaleWorlds.Engine, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: DD7DC4E8-6EB6-4CB8-8DE1-FC78E45F7FE8
// Assembly location: E:\SteamLibrary\steamapps\common\Mount & Blade II Bannerlord\bin\Win64_Shipping_Client\TaleWorlds.Engine.dll

using TaleWorlds.Library;

namespace TaleWorlds.Engine
{
  [ApplicationInterfaceBase]
  internal interface IScreen
  {
    [EngineMethod("get_real_screen_resolution_width", false)]
    float GetRealScreenResolutionWidth();

    [EngineMethod("get_real_screen_resolution_height", false)]
    float GetRealScreenResolutionHeight();

    [EngineMethod("get_desktop_width", false)]
    float GetDesktopWidth();

    [EngineMethod("get_desktop_height", false)]
    float GetDesktopHeight();

    [EngineMethod("get_aspect_ratio", false)]
    float GetAspectRatio();

    [EngineMethod("get_mouse_visible", false)]
    bool GetMouseVisible();

    [EngineMethod("set_mouse_visible", false)]
    void SetMouseVisible(bool value);

    [EngineMethod("get_usable_area_percentages", false)]
    Vec2 GetUsableAreaPercentages();
  }
}
