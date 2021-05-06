// Decompiled with JetBrains decompiler
// Type: TaleWorlds.MountAndBlade.CustomBattleApplyWeatherEffectsModel
// Assembly: TaleWorlds.MountAndBlade, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3C0E3F42-756F-4407-B817-06BA412A78B0
// Assembly location: E:\SteamLibrary\steamapps\common\Mount & Blade II Bannerlord\bin\Win64_Shipping_Client\TaleWorlds.MountAndBlade.dll

using TaleWorlds.DotNet;
using TaleWorlds.Engine;
using TaleWorlds.MountAndBlade.ComponentInterfaces;

namespace TaleWorlds.MountAndBlade
{
  public class CustomBattleApplyWeatherEffectsModel : ApplyWeatherEffectsModel
  {
    public override void ApplyWeatherEffects()
    {
      Scene scene = Mission.Current.Scene;
      if (!((NativeObject) scene != (NativeObject) null))
        return;
      bool flag1 = (double) scene.GetRainDensity() > 0.0 | (double) scene.GetSnowDensity() > 0.0;
      bool flag2 = (double) scene.GetFog() > 0.0;
      Mission.Current.SetBowMissileSpeedModifier(flag1 ? 0.9f : 1f);
      Mission.Current.SetCrossbowMissileSpeedModifier(flag1 ? 0.9f : 1f);
      Mission.Current.SetMissileRangeModifier(flag2 ? 0.8f : 1f);
    }
  }
}
