// Decompiled with JetBrains decompiler
// Type: TaleWorlds.MountAndBlade.FireMangonel
// Assembly: TaleWorlds.MountAndBlade, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3C0E3F42-756F-4407-B817-06BA412A78B0
// Assembly location: E:\SteamLibrary\steamapps\common\Mount & Blade II Bannerlord\bin\Win64_Shipping_Client\TaleWorlds.MountAndBlade.dll

using TaleWorlds.Core;

namespace TaleWorlds.MountAndBlade
{
  public class FireMangonel : Mangonel
  {
    public override SiegeEngineType GetSiegeEngineType() => this._defaultSide != BattleSideEnum.Attacker ? DefaultSiegeEngineTypes.FireCatapult : DefaultSiegeEngineTypes.FireOnager;

    public override float ProcessTargetValue(float baseValue, TargetFlags flags)
    {
      if (flags.HasAnyFlag<TargetFlags>(TargetFlags.NotAThreat))
        return -1000f;
      if (flags.HasAnyFlag<TargetFlags>(TargetFlags.None))
        baseValue *= 1.5f;
      if (flags.HasAnyFlag<TargetFlags>(TargetFlags.IsSiegeEngine))
        baseValue *= 2f;
      if (flags.HasAnyFlag<TargetFlags>(TargetFlags.IsStructure))
        baseValue *= 1.5f;
      if (flags.HasAnyFlag<TargetFlags>(TargetFlags.IsSmall))
        baseValue *= 0.5f;
      if (flags.HasAnyFlag<TargetFlags>(TargetFlags.IsMoving))
        baseValue *= 0.8f;
      if (flags.HasAnyFlag<TargetFlags>(TargetFlags.DebugThreat))
        baseValue *= 10000f;
      return baseValue;
    }
  }
}
