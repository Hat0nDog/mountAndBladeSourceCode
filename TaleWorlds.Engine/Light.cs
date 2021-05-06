// Decompiled with JetBrains decompiler
// Type: TaleWorlds.Engine.Light
// Assembly: TaleWorlds.Engine, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: DD7DC4E8-6EB6-4CB8-8DE1-FC78E45F7FE8
// Assembly location: E:\SteamLibrary\steamapps\common\Mount & Blade II Bannerlord\bin\Win64_Shipping_Client\TaleWorlds.Engine.dll

using System;
using TaleWorlds.DotNet;
using TaleWorlds.Library;

namespace TaleWorlds.Engine
{
  [EngineClass("rglLight")]
  public sealed class Light : GameEntityComponent
  {
    public bool IsValid => this.Pointer != UIntPtr.Zero;

    internal Light(UIntPtr pointer)
      : base(pointer)
    {
    }

    public static Light CreatePointLight(float lightRadius) => EngineApplicationInterface.ILight.CreatePointLight(lightRadius);

    public MatrixFrame Frame
    {
      get
      {
        MatrixFrame result;
        EngineApplicationInterface.ILight.GetFrame(this.Pointer, out result);
        return result;
      }
      set => EngineApplicationInterface.ILight.SetFrame(this.Pointer, ref value);
    }

    public Vec3 LightColor
    {
      get => EngineApplicationInterface.ILight.GetLightColor(this.Pointer);
      set => EngineApplicationInterface.ILight.SetLightColor(this.Pointer, value);
    }

    public float Intensity
    {
      get => EngineApplicationInterface.ILight.GetIntensity(this.Pointer);
      set => EngineApplicationInterface.ILight.SetIntensity(this.Pointer, value);
    }

    public float Radius
    {
      get => EngineApplicationInterface.ILight.GetRadius(this.Pointer);
      set => EngineApplicationInterface.ILight.SetRadius(this.Pointer, value);
    }

    public void SetShadowType(Light.ShadowType type) => EngineApplicationInterface.ILight.SetShadows(this.Pointer, (int) type);

    public bool ShadowEnabled
    {
      get => EngineApplicationInterface.ILight.IsShadowEnabled(this.Pointer);
      set => EngineApplicationInterface.ILight.EnableShadow(this.Pointer, value);
    }

    public void SetLightFlicker(float magnitude, float interval) => EngineApplicationInterface.ILight.SetLightFlicker(this.Pointer, magnitude, interval);

    public void SetVolumetricProperties(bool volumetricLightEnabled, float volumeParameters) => EngineApplicationInterface.ILight.SetVolumetricProperties(this.Pointer, volumetricLightEnabled, volumeParameters);

    public void Dispose()
    {
      if (!this.IsValid)
        return;
      this.Release();
      GC.SuppressFinalize((object) this);
    }

    public void SetVisibility(bool value) => EngineApplicationInterface.ILight.SetVisibility(this.Pointer, value);

    private void Release() => EngineApplicationInterface.ILight.Release(this.Pointer);

    ~Light() => this.Dispose();

    public enum ShadowType
    {
      NoShadow,
      StaticShadow,
      DynamicShadow,
      Count,
    }
  }
}
