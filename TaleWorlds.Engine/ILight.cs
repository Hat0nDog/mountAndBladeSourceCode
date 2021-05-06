// Decompiled with JetBrains decompiler
// Type: TaleWorlds.Engine.ILight
// Assembly: TaleWorlds.Engine, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: DD7DC4E8-6EB6-4CB8-8DE1-FC78E45F7FE8
// Assembly location: E:\SteamLibrary\steamapps\common\Mount & Blade II Bannerlord\bin\Win64_Shipping_Client\TaleWorlds.Engine.dll

using System;
using TaleWorlds.Library;

namespace TaleWorlds.Engine
{
  [ApplicationInterfaceBase]
  internal interface ILight
  {
    [EngineMethod("create_point_light", false)]
    Light CreatePointLight(float lightRadius);

    [EngineMethod("set_radius", false)]
    void SetRadius(UIntPtr lightpointer, float radius);

    [EngineMethod("set_light_flicker", false)]
    void SetLightFlicker(UIntPtr lightpointer, float magnitude, float interval);

    [EngineMethod("enable_shadow", false)]
    void EnableShadow(UIntPtr lightpointer, bool shadowEnabled);

    [EngineMethod("is_shadow_enabled", false)]
    bool IsShadowEnabled(UIntPtr lightpointer);

    [EngineMethod("set_volumetric_properties", false)]
    void SetVolumetricProperties(
      UIntPtr lightpointer,
      bool volumelightenabled,
      float volumeparameter);

    [EngineMethod("set_visibility", false)]
    void SetVisibility(UIntPtr lightpointer, bool value);

    [EngineMethod("get_radius", false)]
    float GetRadius(UIntPtr lightpointer);

    [EngineMethod("set_shadows", false)]
    void SetShadows(UIntPtr lightPointer, int shadowType);

    [EngineMethod("set_light_color", false)]
    void SetLightColor(UIntPtr lightpointer, Vec3 color);

    [EngineMethod("get_light_color", false)]
    Vec3 GetLightColor(UIntPtr lightpointer);

    [EngineMethod("set_intensity", false)]
    void SetIntensity(UIntPtr lightPointer, float value);

    [EngineMethod("get_intensity", false)]
    float GetIntensity(UIntPtr lightPointer);

    [EngineMethod("release", false)]
    void Release(UIntPtr lightpointer);

    [EngineMethod("set_frame", false)]
    void SetFrame(UIntPtr lightPointer, ref MatrixFrame frame);

    [EngineMethod("get_frame", false)]
    void GetFrame(UIntPtr lightPointer, out MatrixFrame result);
  }
}
