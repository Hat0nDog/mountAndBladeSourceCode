// Decompiled with JetBrains decompiler
// Type: TaleWorlds.Engine.Options.NativeOptions
// Assembly: TaleWorlds.Engine, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: DD7DC4E8-6EB6-4CB8-8DE1-FC78E45F7FE8
// Assembly location: E:\SteamLibrary\steamapps\common\Mount & Blade II Bannerlord\bin\Win64_Shipping_Client\TaleWorlds.Engine.dll

using System.Collections.Generic;
using TaleWorlds.Library;

namespace TaleWorlds.Engine.Options
{
  public class NativeOptions
  {
    public static NativeOptions.OnNativeOptionChangedDelegate OnNativeOptionChanged;
    private static List<NativeOptionData> _videoOptions;
    private static List<NativeOptionData> _graphicsOptions;

    public static string GetGFXPresetName(NativeOptions.ConfigQuality presetIndex)
    {
      switch (presetIndex)
      {
        case NativeOptions.ConfigQuality.GFXVeryLow:
          return "1";
        case NativeOptions.ConfigQuality.GFXLow:
          return "2";
        case NativeOptions.ConfigQuality.GFXMedium:
          return "3";
        case NativeOptions.ConfigQuality.GFXHigh:
          return "4";
        case NativeOptions.ConfigQuality.GFXVeryHigh:
          return "5";
        case NativeOptions.ConfigQuality.GFXCustom:
          return "Custom";
        default:
          return "Unknown";
      }
    }

    public static bool IsGFXOptionChangeable(NativeOptions.ConfigQuality config) => config < NativeOptions.ConfigQuality.GFXCustom;

    private static void CorrectSelection(List<NativeOptionData> audioOptions)
    {
      foreach (NativeOptionData audioOption in audioOptions)
      {
        if (audioOption.Type == NativeOptions.NativeOptionsType.SoundDevice)
        {
          int num = 0;
          for (int i = 0; i < NativeOptions.GetSoundDeviceCount(); ++i)
          {
            if (NativeOptions.GetSoundDeviceName(i) != "")
              num = i;
          }
          if ((double) audioOption.GetValue(false) > (double) num)
          {
            NativeOptions.SetConfig(NativeOptions.NativeOptionsType.SoundDevice, 0.0f);
            audioOption.SetValue(0.0f);
          }
        }
      }
    }

    public static List<NativeOptionData> VideoOptions
    {
      get
      {
        if (NativeOptions._videoOptions == null)
        {
          NativeOptions._videoOptions = new List<NativeOptionData>();
          for (NativeOptions.NativeOptionsType type = NativeOptions.NativeOptionsType.None; type < NativeOptions.NativeOptionsType.TotalOptions; ++type)
          {
            switch (type - 13)
            {
              case NativeOptions.NativeOptionsType.MasterVolume:
              case NativeOptions.NativeOptionsType.SoundVolume:
              case NativeOptions.NativeOptionsType.MusicVolume:
              case NativeOptions.NativeOptionsType.SoundDevice:
              case NativeOptions.NativeOptionsType.MaxSimultaneousSoundEventCount:
              case NativeOptions.NativeOptionsType.SoundOcclusion:
                NativeOptions._videoOptions.Add((NativeOptionData) new NativeSelectionOptionData(type));
                break;
              case NativeOptions.NativeOptionsType.SoundOutput:
              case NativeOptions.NativeOptionsType.SoundInBackground:
                NativeOptions._videoOptions.Add((NativeOptionData) new NativeNumericOptionData(type));
                break;
              default:
                if (type == NativeOptions.NativeOptionsType.SharpenAmount)
                  goto case NativeOptions.NativeOptionsType.SoundOutput;
                else
                  break;
            }
          }
        }
        return NativeOptions._videoOptions;
      }
    }

    public static List<NativeOptionData> GraphicsOptions
    {
      get
      {
        if (NativeOptions._graphicsOptions == null)
        {
          NativeOptions._graphicsOptions = new List<NativeOptionData>();
          for (NativeOptions.NativeOptionsType type = NativeOptions.NativeOptionsType.None; type < NativeOptions.NativeOptionsType.TotalOptions; ++type)
          {
            switch (type - 4)
            {
              case NativeOptions.NativeOptionsType.MasterVolume:
              case NativeOptions.NativeOptionsType.ResolutionScale:
              case NativeOptions.NativeOptionsType.FrameLimiter:
              case NativeOptions.NativeOptionsType.VSync:
              case NativeOptions.NativeOptionsType.Brightness:
              case NativeOptions.NativeOptionsType.OverAll:
              case NativeOptions.NativeOptionsType.ShaderQuality:
              case NativeOptions.NativeOptionsType.TextureBudget:
              case NativeOptions.NativeOptionsType.TextureQuality:
              case NativeOptions.NativeOptionsType.ShadowmapResolution:
              case NativeOptions.NativeOptionsType.ShadowmapType:
              case NativeOptions.NativeOptionsType.ShadowmapFiltering:
              case NativeOptions.NativeOptionsType.ParticleDetail:
              case NativeOptions.NativeOptionsType.ParticleQuality:
              case NativeOptions.NativeOptionsType.FoliageQuality:
              case NativeOptions.NativeOptionsType.CharacterDetail:
              case NativeOptions.NativeOptionsType.EnvironmentDetail:
              case NativeOptions.NativeOptionsType.TerrainQuality:
              case NativeOptions.NativeOptionsType.NumberOfRagDolls:
              case NativeOptions.NativeOptionsType.TextureFiltering:
              case NativeOptions.NativeOptionsType.WaterQuality:
                NativeOptions._graphicsOptions.Add((NativeOptionData) new NativeSelectionOptionData(type));
                break;
              case NativeOptions.NativeOptionsType.Occlusion:
                if (NativeOptions.GetIsDLSSAvailable())
                {
                  NativeOptions._graphicsOptions.Add((NativeOptionData) new NativeSelectionOptionData(type));
                  break;
                }
                break;
              case NativeOptions.NativeOptionsType.Antialiasing:
              case NativeOptions.NativeOptionsType.DLSS:
              case NativeOptions.NativeOptionsType.LightingQuality:
              case NativeOptions.NativeOptionsType.DecalQuality:
              case NativeOptions.NativeOptionsType.DepthOfField:
              case NativeOptions.NativeOptionsType.SSR:
              case NativeOptions.NativeOptionsType.ClothSimulation:
              case NativeOptions.NativeOptionsType.InteractiveGrass:
              case NativeOptions.NativeOptionsType.SunShafts:
              case NativeOptions.NativeOptionsType.SSSSS:
                NativeOptions._graphicsOptions.Add((NativeOptionData) new NativeBooleanOptionData(type));
                break;
              case NativeOptions.NativeOptionsType.Bloom:
                if (EngineApplicationInterface.IConfig.CheckGFXSupportStatus(54))
                {
                  NativeOptions._graphicsOptions.Add((NativeOptionData) new NativeBooleanOptionData(type));
                  break;
                }
                break;
              case NativeOptions.NativeOptionsType.FilmGrain:
                if (EngineApplicationInterface.IConfig.CheckGFXSupportStatus(55))
                {
                  NativeOptions._graphicsOptions.Add((NativeOptionData) new NativeBooleanOptionData(type));
                  break;
                }
                break;
              case NativeOptions.NativeOptionsType.MotionBlur:
                if (EngineApplicationInterface.IConfig.CheckGFXSupportStatus(56))
                {
                  NativeOptions._graphicsOptions.Add((NativeOptionData) new NativeBooleanOptionData(type));
                  break;
                }
                break;
              case NativeOptions.NativeOptionsType.SharpenAmount:
                if (EngineApplicationInterface.IConfig.CheckGFXSupportStatus(57))
                {
                  NativeOptions._graphicsOptions.Add((NativeOptionData) new NativeBooleanOptionData(type));
                  break;
                }
                break;
              case NativeOptions.NativeOptionsType.PostFXLensFlare:
                if (EngineApplicationInterface.IConfig.CheckGFXSupportStatus(58))
                {
                  NativeOptions._graphicsOptions.Add((NativeOptionData) new NativeBooleanOptionData(type));
                  break;
                }
                break;
            }
          }
        }
        return NativeOptions._graphicsOptions;
      }
    }

    public static float GetConfig(NativeOptions.NativeOptionsType type) => EngineApplicationInterface.IConfig.GetRGLConfig((int) type);

    public static float GetDefaultConfig(NativeOptions.NativeOptionsType type) => EngineApplicationInterface.IConfig.GetDefaultRGLConfig((int) type);

    public static float GetDefaultConfigForOverallSettings(
      NativeOptions.NativeOptionsType type,
      int config)
    {
      return EngineApplicationInterface.IConfig.GetRGLConfigForDefaultSettings((int) type, config);
    }

    public static int GetGameKeys(int keyType, int i) => 0;

    public static string GetSoundDeviceName(int i) => EngineApplicationInterface.IConfig.GetSoundDeviceName(i);

    public static string GetMonitorDeviceName(int i) => EngineApplicationInterface.IConfig.GetMonitorDeviceName(i);

    public static string GetVideoDeviceName(int i) => EngineApplicationInterface.IConfig.GetVideoDeviceName(i);

    public static int GetSoundDeviceCount() => EngineApplicationInterface.IConfig.GetSoundDeviceCount();

    public static int GetMonitorDeviceCount() => EngineApplicationInterface.IConfig.GetMonitorDeviceCount();

    public static int GetVideoDeviceCount() => EngineApplicationInterface.IConfig.GetVideoDeviceCount();

    public static int GetResolutionCount() => EngineApplicationInterface.IConfig.GetResolutionCount();

    public static void RefreshOptionsData() => EngineApplicationInterface.IConfig.RefreshOptionsData();

    public static int GetRefreshRateCount() => EngineApplicationInterface.IConfig.GetRefreshRateCount();

    public static int GetRefreshRateAtIndex(int index) => EngineApplicationInterface.IConfig.GetRefreshRateAtIndex(index);

    public static void SetCustomResolution(int width, int height) => EngineApplicationInterface.IConfig.SetCustomResolution(width, height);

    public static void GetResolution(ref int width, ref int height) => EngineApplicationInterface.IConfig.GetDesktopResolution(ref width, ref height);

    public static void GetDesktopResolution(ref int width, ref int height) => EngineApplicationInterface.IConfig.GetDesktopResolution(ref width, ref height);

    public static Vec2 GetResolutionAtIndex(int index) => EngineApplicationInterface.IConfig.GetResolutionAtIndex(index);

    public static int GetDLSSTechnique() => EngineApplicationInterface.IConfig.GetDlssTechnique();

    public static int GetDLSSOptionCount() => EngineApplicationInterface.IConfig.GetDlssOptionCount();

    public static bool GetIsDLSSAvailable() => EngineApplicationInterface.IConfig.IsDlssAvailable();

    public static bool CheckGFXSupportStatus(int enumType) => EngineApplicationInterface.IConfig.CheckGFXSupportStatus(enumType);

    public static void SetConfig(NativeOptions.NativeOptionsType type, float value)
    {
      EngineApplicationInterface.IConfig.SetRGLConfig((int) type, value);
      if (NativeOptions.OnNativeOptionChanged == null)
        return;
      NativeOptions.OnNativeOptionChanged(type);
    }

    public static void ApplyConfigChanges(bool resizeWindow) => EngineApplicationInterface.IConfig.ApplyConfigChanges(resizeWindow);

    public static void SetGameKeys(int keyType, int index, int key)
    {
    }

    public static void Apply(
      int texture_budget,
      int sharpen_amount,
      int hdr,
      int dof_mode,
      int motion_blur,
      int ssr,
      int size,
      int texture_filtering,
      int trail_amount)
    {
      EngineApplicationInterface.IConfig.Apply(texture_budget, sharpen_amount, hdr, dof_mode, motion_blur, ssr, size, texture_filtering, trail_amount);
    }

    public static void SaveConfig() => EngineApplicationInterface.IConfig.SaveRGLConfig();

    public static void SetBrightness(float gamma) => EngineApplicationInterface.IConfig.SetBrightness(gamma);

    public static void SetDefaultGameKeys()
    {
    }

    public static void SetDefaultGameConfig() => EngineApplicationInterface.IConfig.SetDefaultGameConfig();

    public enum ConfigQuality
    {
      GFXVeryLow,
      GFXLow,
      GFXMedium,
      GFXHigh,
      GFXVeryHigh,
      GFXCustom,
    }

    public enum NativeOptionsType
    {
      None = -1, // 0xFFFFFFFF
      MasterVolume = 0,
      SoundVolume = 1,
      MusicVolume = 2,
      SoundDevice = 3,
      MaxSimultaneousSoundEventCount = 4,
      SoundOutput = 5,
      SoundInBackground = 6,
      SoundOcclusion = 7,
      MouseSensitivity = 8,
      InvertMouseYAxis = 9,
      MouseYMovementScale = 10, // 0x0000000A
      TrailAmount = 11, // 0x0000000B
      EnableVibration = 12, // 0x0000000C
      DisplayMode = 13, // 0x0000000D
      SelectedMonitor = 14, // 0x0000000E
      SelectedAdapter = 15, // 0x0000000F
      ScreenResolution = 16, // 0x00000010
      RefreshRate = 17, // 0x00000011
      ResolutionScale = 18, // 0x00000012
      FrameLimiter = 19, // 0x00000013
      VSync = 20, // 0x00000014
      Brightness = 21, // 0x00000015
      OverAll = 22, // 0x00000016
      ShaderQuality = 23, // 0x00000017
      TextureBudget = 24, // 0x00000018
      TextureQuality = 25, // 0x00000019
      ShadowmapResolution = 26, // 0x0000001A
      ShadowmapType = 27, // 0x0000001B
      ShadowmapFiltering = 28, // 0x0000001C
      ParticleDetail = 29, // 0x0000001D
      ParticleQuality = 30, // 0x0000001E
      FoliageQuality = 31, // 0x0000001F
      CharacterDetail = 32, // 0x00000020
      EnvironmentDetail = 33, // 0x00000021
      TerrainQuality = 34, // 0x00000022
      NumberOfRagDolls = 35, // 0x00000023
      Occlusion = 36, // 0x00000024
      TextureFiltering = 37, // 0x00000025
      WaterQuality = 38, // 0x00000026
      Antialiasing = 39, // 0x00000027
      DLSS = 40, // 0x00000028
      LightingQuality = 41, // 0x00000029
      DecalQuality = 42, // 0x0000002A
      DepthOfField = 43, // 0x0000002B
      SSR = 44, // 0x0000002C
      ClothSimulation = 45, // 0x0000002D
      InteractiveGrass = 46, // 0x0000002E
      SunShafts = 47, // 0x0000002F
      SSSSS = 48, // 0x00000030
      Tesselation = 49, // 0x00000031
      Bloom = 50, // 0x00000032
      FilmGrain = 51, // 0x00000033
      MotionBlur = 52, // 0x00000034
      SharpenAmount = 53, // 0x00000035
      PostFXLensFlare = 54, // 0x00000036
      PostFXStreaks = 55, // 0x00000037
      PostFXChromaticAberration = 56, // 0x00000038
      PostFXVignette = 57, // 0x00000039
      PostFXHexagonVignette = 58, // 0x0000003A
      BrightnessMin = 59, // 0x0000003B
      BrightnessMax = 60, // 0x0000003C
      BrightnessCalibrated = 61, // 0x0000003D
      ExposureCompensation = 62, // 0x0000003E
      NumOfOptionTypes = 63, // 0x0000003F
      TotalOptions = 64, // 0x00000040
    }

    public delegate void OnNativeOptionChangedDelegate(
      NativeOptions.NativeOptionsType changedNativeOptionsType);
  }
}
