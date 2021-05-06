// Decompiled with JetBrains decompiler
// Type: TaleWorlds.MountAndBlade.QueryLibrary
// Assembly: TaleWorlds.MountAndBlade, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3C0E3F42-756F-4407-B817-06BA412A78B0
// Assembly location: E:\SteamLibrary\steamapps\common\Mount & Blade II Bannerlord\bin\Win64_Shipping_Client\TaleWorlds.MountAndBlade.dll

namespace TaleWorlds.MountAndBlade
{
  public static class QueryLibrary
  {
    public static bool IsInfantry(Agent a) => !a.HasMount && !a.IsRangedCached;

    public static bool HasShield(Agent a) => a.HasShieldCached;

    public static bool IsRanged(Agent a) => !a.HasMount && a.IsRangedCached;

    public static bool IsCavalry(Agent a) => a.HasMount && !a.IsRangedCached;

    public static bool IsRangedCavalry(Agent a) => a.HasMount && a.IsRangedCached;
  }
}
