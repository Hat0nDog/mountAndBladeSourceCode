// Decompiled with JetBrains decompiler
// Type: TaleWorlds.MountAndBlade.FireBallista
// Assembly: TaleWorlds.MountAndBlade, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3C0E3F42-756F-4407-B817-06BA412A78B0
// Assembly location: E:\SteamLibrary\steamapps\common\Mount & Blade II Bannerlord\bin\Win64_Shipping_Client\TaleWorlds.MountAndBlade.dll

using TaleWorlds.Core;

namespace TaleWorlds.MountAndBlade
{
  public class FireBallista : Ballista
  {
    public override SiegeEngineType GetSiegeEngineType() => DefaultSiegeEngineTypes.FireBallista;

    public override float ProcessTargetValue(float baseValue, TargetFlags flags)
    {
      if (flags.HasAnyFlag<TargetFlags>(TargetFlags.NotAThreat))
        return -1000f;
      if (flags.HasAnyFlag<TargetFlags>(TargetFlags.None))
        baseValue *= 1.5f;
      if (flags.HasAnyFlag<TargetFlags>(TargetFlags.IsSiegeEngine))
        baseValue *= 2.5f;
      if (flags.HasAnyFlag<TargetFlags>(TargetFlags.IsStructure))
        baseValue *= 0.1f;
      if (flags.HasAnyFlag<TargetFlags>(TargetFlags.IsFlammable))
        baseValue *= 2f;
      if (flags.HasAnyFlag<TargetFlags>(TargetFlags.DebugThreat))
        baseValue *= 1000f;
      return baseValue;
    }
  }
}
