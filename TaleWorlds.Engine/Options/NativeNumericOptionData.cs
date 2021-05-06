// Decompiled with JetBrains decompiler
// Type: TaleWorlds.Engine.Options.NativeNumericOptionData
// Assembly: TaleWorlds.Engine, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: DD7DC4E8-6EB6-4CB8-8DE1-FC78E45F7FE8
// Assembly location: E:\SteamLibrary\steamapps\common\Mount & Blade II Bannerlord\bin\Win64_Shipping_Client\TaleWorlds.Engine.dll

namespace TaleWorlds.Engine.Options
{
  public class NativeNumericOptionData : NativeOptionData, INumericOptionData, IOptionData
  {
    private readonly float _minValue;
    private readonly float _maxValue;

    public NativeNumericOptionData(NativeOptions.NativeOptionsType type)
      : base(type)
    {
      this._minValue = NativeNumericOptionData.GetLimitValue(this.Type, true);
      this._maxValue = NativeNumericOptionData.GetLimitValue(this.Type, false);
    }

    public float GetMinValue() => this._minValue;

    public float GetMaxValue() => this._maxValue;

    private static float GetLimitValue(NativeOptions.NativeOptionsType type, bool isMin)
    {
      switch (type)
      {
        case NativeOptions.NativeOptionsType.MouseYMovementScale:
          return !isMin ? 4f : 0.25f;
        case NativeOptions.NativeOptionsType.TrailAmount:
          return !isMin ? 1f : 0.0f;
        case NativeOptions.NativeOptionsType.ResolutionScale:
          return !isMin ? 100f : 20f;
        case NativeOptions.NativeOptionsType.FrameLimiter:
          return !isMin ? 360f : 30f;
        case NativeOptions.NativeOptionsType.Brightness:
          return !isMin ? 100f : 0.0f;
        case NativeOptions.NativeOptionsType.SharpenAmount:
          return !isMin ? 65f : 0.0f;
        case NativeOptions.NativeOptionsType.BrightnessMin:
          return !isMin ? 0.3f : 0.0f;
        case NativeOptions.NativeOptionsType.BrightnessMax:
          return !isMin ? 1f : 0.7f;
        case NativeOptions.NativeOptionsType.ExposureCompensation:
          return !isMin ? 2f : -2f;
        default:
          return !isMin ? 1f : 0.0f;
      }
    }

    public bool GetIsDiscrete()
    {
      switch (this.Type)
      {
        case NativeOptions.NativeOptionsType.ResolutionScale:
        case NativeOptions.NativeOptionsType.FrameLimiter:
        case NativeOptions.NativeOptionsType.Brightness:
        case NativeOptions.NativeOptionsType.BrightnessMin:
        case NativeOptions.NativeOptionsType.BrightnessMax:
        case NativeOptions.NativeOptionsType.ExposureCompensation:
          return true;
        default:
          return false;
      }
    }

    public bool GetShouldUpdateContinuously() => true;
  }
}
