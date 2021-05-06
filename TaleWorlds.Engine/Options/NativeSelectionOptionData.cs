// Decompiled with JetBrains decompiler
// Type: TaleWorlds.Engine.Options.NativeSelectionOptionData
// Assembly: TaleWorlds.Engine, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: DD7DC4E8-6EB6-4CB8-8DE1-FC78E45F7FE8
// Assembly location: E:\SteamLibrary\steamapps\common\Mount & Blade II Bannerlord\bin\Win64_Shipping_Client\TaleWorlds.Engine.dll

using System.Collections.Generic;
using TaleWorlds.Library;

namespace TaleWorlds.Engine.Options
{
  public class NativeSelectionOptionData : NativeOptionData, ISelectionOptionData, IOptionData
  {
    private readonly int _selectableOptionsLimit;
    private readonly IEnumerable<SelectionData> _selectableOptionNames;

    public NativeSelectionOptionData(NativeOptions.NativeOptionsType type)
      : base(type)
    {
      this._selectableOptionsLimit = NativeSelectionOptionData.GetOptionsLimit(type);
      this._selectableOptionNames = NativeSelectionOptionData.GetOptionNames(type);
    }

    public int GetSelectableOptionsLimit() => this._selectableOptionsLimit;

    public IEnumerable<SelectionData> GetSelectableOptionNames() => NativeSelectionOptionData.GetOptionNames(this.Type);

    public static int GetOptionsLimit(NativeOptions.NativeOptionsType optionType)
    {
      switch (optionType)
      {
        case NativeOptions.NativeOptionsType.SoundDevice:
          return NativeOptions.GetSoundDeviceCount();
        case NativeOptions.NativeOptionsType.MaxSimultaneousSoundEventCount:
          return 4;
        case NativeOptions.NativeOptionsType.SoundOutput:
          return 3;
        case NativeOptions.NativeOptionsType.DisplayMode:
          return 3;
        case NativeOptions.NativeOptionsType.SelectedMonitor:
          return NativeOptions.GetMonitorDeviceCount();
        case NativeOptions.NativeOptionsType.SelectedAdapter:
          return NativeOptions.GetVideoDeviceCount();
        case NativeOptions.NativeOptionsType.ScreenResolution:
          return NativeOptions.GetResolutionCount() + 1;
        case NativeOptions.NativeOptionsType.RefreshRate:
          return NativeOptions.GetRefreshRateCount();
        case NativeOptions.NativeOptionsType.VSync:
          return 3;
        case NativeOptions.NativeOptionsType.OverAll:
          return 6;
        case NativeOptions.NativeOptionsType.ShaderQuality:
          return 3;
        case NativeOptions.NativeOptionsType.TextureBudget:
          return 4;
        case NativeOptions.NativeOptionsType.TextureQuality:
          return 3;
        case NativeOptions.NativeOptionsType.ShadowmapResolution:
          return 4;
        case NativeOptions.NativeOptionsType.ShadowmapType:
          return 3;
        case NativeOptions.NativeOptionsType.ShadowmapFiltering:
          return 2;
        case NativeOptions.NativeOptionsType.ParticleDetail:
          return 3;
        case NativeOptions.NativeOptionsType.ParticleQuality:
          return 3;
        case NativeOptions.NativeOptionsType.FoliageQuality:
          return 5;
        case NativeOptions.NativeOptionsType.CharacterDetail:
          return 5;
        case NativeOptions.NativeOptionsType.EnvironmentDetail:
          return 5;
        case NativeOptions.NativeOptionsType.TerrainQuality:
          return 3;
        case NativeOptions.NativeOptionsType.NumberOfRagDolls:
          return 6;
        case NativeOptions.NativeOptionsType.Occlusion:
          return 2;
        case NativeOptions.NativeOptionsType.TextureFiltering:
          return 6;
        case NativeOptions.NativeOptionsType.WaterQuality:
          return 3;
        case NativeOptions.NativeOptionsType.Antialiasing:
          return 6;
        case NativeOptions.NativeOptionsType.DLSS:
          return NativeOptions.GetDLSSOptionCount();
        case NativeOptions.NativeOptionsType.LightingQuality:
          return 3;
        case NativeOptions.NativeOptionsType.DecalQuality:
          return 5;
        default:
          return 0;
      }
    }

    private static IEnumerable<SelectionData> GetOptionNames(
      NativeOptions.NativeOptionsType type)
    {
      int i1;
      int i2;
      switch (type)
      {
        case NativeOptions.NativeOptionsType.SoundDevice:
          for (i1 = 0; i1 < NativeOptions.GetSoundDeviceCount(); ++i1)
          {
            string soundDeviceName = NativeOptions.GetSoundDeviceName(i1);
            if (soundDeviceName != "")
              yield return new SelectionData(false, soundDeviceName);
          }
          break;
        case NativeOptions.NativeOptionsType.SelectedMonitor:
          for (i1 = 0; i1 < NativeOptions.GetMonitorDeviceCount(); ++i1)
            yield return new SelectionData(false, NativeOptions.GetMonitorDeviceName(i1));
          break;
        case NativeOptions.NativeOptionsType.SelectedAdapter:
          for (i1 = 0; i1 < NativeOptions.GetVideoDeviceCount(); ++i1)
            yield return new SelectionData(false, NativeOptions.GetVideoDeviceName(i1));
          break;
        case NativeOptions.NativeOptionsType.ScreenResolution:
          for (i2 = 0; i2 < NativeOptions.GetResolutionCount(); ++i2)
          {
            Vec2 resolutionAtIndex = NativeOptions.GetResolutionAtIndex(i2);
            yield return new SelectionData(false, string.Format("{0}x{1} ({2})", (object) resolutionAtIndex.x, (object) resolutionAtIndex.y, (object) NativeSelectionOptionData.GetAspectRatioOfResolution((int) resolutionAtIndex.x, (int) resolutionAtIndex.y)));
          }
          int width = 0;
          int height1 = 0;
          i1 = 0;
          int height2 = 0;
          NativeOptions.GetDesktopResolution(ref width, ref height1);
          NativeOptions.GetResolution(ref i1, ref height2);
          if (NativeOptions.GetDLSSTechnique() != 4 || width >= 3840)
            yield return new SelectionData(true, "str_options_type_ScreenResolution_Desktop");
          if (NativeOptions.GetDLSSTechnique() == 4 && i1 < 3840)
            break;
          yield return new SelectionData(true, "str_options_type_ScreenResolution_Custom");
          break;
        case NativeOptions.NativeOptionsType.RefreshRate:
          for (i1 = 0; i1 < NativeOptions.GetRefreshRateCount(); ++i1)
            yield return new SelectionData(false, NativeOptions.GetRefreshRateAtIndex(i1).ToString() + " Hz");
          break;
        default:
          i1 = NativeSelectionOptionData.GetOptionsLimit(type);
          string typeName = type.ToString();
          for (i2 = 0; i2 < i1; ++i2)
            yield return new SelectionData(true, "str_options_type_" + typeName + "_" + i2.ToString());
          typeName = (string) null;
          break;
      }
    }

    private static string GetAspectRatioOfResolution(int width, int height) => string.Format("{0}:{1}", (object) (width / MathF.GreatestCommonDivisor(width, height)), (object) (height / MathF.GreatestCommonDivisor(width, height)));
  }
}
