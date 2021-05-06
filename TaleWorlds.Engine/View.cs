// Decompiled with JetBrains decompiler
// Type: TaleWorlds.Engine.View
// Assembly: TaleWorlds.Engine, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: DD7DC4E8-6EB6-4CB8-8DE1-FC78E45F7FE8
// Assembly location: E:\SteamLibrary\steamapps\common\Mount & Blade II Bannerlord\bin\Win64_Shipping_Client\TaleWorlds.Engine.dll

using System;
using TaleWorlds.DotNet;
using TaleWorlds.Library;

namespace TaleWorlds.Engine
{
  [EngineClass("rglView")]
  public abstract class View : NativeObject
  {
    internal View(UIntPtr meshPointer)
      : base(meshPointer)
    {
    }

    public void SetScale(Vec2 scale) => EngineApplicationInterface.IView.SetScale(this.Pointer, scale.x, scale.y);

    public void SetOffset(Vec2 offset) => EngineApplicationInterface.IView.SetOffset(this.Pointer, offset.x, offset.y);

    public void SetRenderOrder(int value) => EngineApplicationInterface.IView.SetRenderOrder(this.Pointer, value);

    public void SetRenderOption(View.ViewRenderOptions optionEnum, bool value) => EngineApplicationInterface.IView.SetRenderOption(this.Pointer, (int) optionEnum, value);

    public void SetRenderTarget(Texture texture) => EngineApplicationInterface.IView.SetRenderTarget(this.Pointer, texture.Pointer);

    public void SetDepthTarget(Texture texture) => EngineApplicationInterface.IView.SetDepthTarget(this.Pointer, texture.Pointer);

    public void DontClearBackground()
    {
      this.SetRenderOption(View.ViewRenderOptions.ClearColor, false);
      this.SetRenderOption(View.ViewRenderOptions.ClearDepth, false);
    }

    public void SetClearColor(uint rgba) => EngineApplicationInterface.IView.SetClearColor(this.Pointer, rgba);

    public void SetEnable(bool value) => EngineApplicationInterface.IView.SetEnable(this.Pointer, value);

    public void SetRenderOnDemand(bool value) => EngineApplicationInterface.IView.SetRenderOnDemand(this.Pointer, value);

    public void SetAutoDepthTargetCreation(bool value) => EngineApplicationInterface.IView.SetAutoDepthTargetCreation(this.Pointer, value);

    public void SetSaveFinalResultToDisk(bool value) => EngineApplicationInterface.IView.SetSaveFinalResultToDisk(this.Pointer, value);

    public void SetFileNameToSaveResult(string name) => EngineApplicationInterface.IView.SetFileNameToSaveResult(this.Pointer, name);

    public void SetFileTypeToSave(View.TextureSaveFormat format) => EngineApplicationInterface.IView.SetFileTypeToSave(this.Pointer, (int) format);

    public void SetFilePathToSaveResult(string name) => EngineApplicationInterface.IView.SetFilePathToSaveResult(this.Pointer, name);

    public enum TextureSaveFormat
    {
      TextureTypeUnknown,
      TextureTypeBmp,
      TextureTypeJpg,
      TextureTypePng,
      TextureTypeDds,
      TextureTypeTif,
      TextureTypePsd,
      TextureTypeRaw,
    }

    public enum PostfxConfig : uint
    {
      pfx_config_bloom = 1,
      pfx_lower_bound = 1,
      pfx_config_sunshafts = 2,
      pfx_config_motionblur = 4,
      pfx_config_dof = 8,
      pfx_config_tsao = 16, // 0x00000010
      pfx_config_fxaa = 64, // 0x00000040
      pfx_config_smaa = 128, // 0x00000080
      pfx_config_temporal_smaa = 256, // 0x00000100
      pfx_config_temporal_resolve = 512, // 0x00000200
      pfx_config_temporal_filter = 1024, // 0x00000400
      pfx_config_contour = 2048, // 0x00000800
      pfx_config_ssr = 4096, // 0x00001000
      pfx_config_sssss = 8192, // 0x00002000
      pfx_config_streaks = 16384, // 0x00004000
      pfx_config_lens_flares = 32768, // 0x00008000
      pfx_config_chromatic_aberration = 65536, // 0x00010000
      pfx_config_vignette = 131072, // 0x00020000
      pfx_config_sharpen = 262144, // 0x00040000
      pfx_config_grain = 524288, // 0x00080000
      pfx_config_temporal_shadow = 1048576, // 0x00100000
      pfx_config_editor_scene = 2097152, // 0x00200000
      pfx_config_custom1 = 16777216, // 0x01000000
      pfx_config_custom2 = 33554432, // 0x02000000
      pfx_config_custom3 = 67108864, // 0x04000000
      pfx_config_custom4 = 134217728, // 0x08000000
      pfx_config_hexagon_vignette = 268435456, // 0x10000000
      pfx_config_screen_rt_injection = 536870912, // 0x20000000
      pfx_upper_bound = 536870912, // 0x20000000
      pfx_config_high_dof = 1073741824, // 0x40000000
    }

    public enum ViewRenderOptions
    {
      ClearColor,
      ClearDepth,
    }
  }
}
