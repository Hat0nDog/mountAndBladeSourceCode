// Decompiled with JetBrains decompiler
// Type: TaleWorlds.MountAndBlade.MultiplayerAgentDecideKilledOrUnconsciousModel
// Assembly: TaleWorlds.MountAndBlade, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3C0E3F42-756F-4407-B817-06BA412A78B0
// Assembly location: E:\SteamLibrary\steamapps\common\Mount & Blade II Bannerlord\bin\Win64_Shipping_Client\TaleWorlds.MountAndBlade.dll

using TaleWorlds.Core;
using TaleWorlds.MountAndBlade.ComponentInterfaces;

namespace TaleWorlds.MountAndBlade
{
  public class MultiplayerAgentDecideKilledOrUnconsciousModel : AgentDecideKilledOrUnconsciousModel
  {
    public override float GetAgentStateProbability(
      Agent affectorAgent,
      Agent effectedAgent,
      DamageTypes damageType,
      out float useSurgeryProbability)
    {
      useSurgeryProbability = 0.0f;
      return 1f;
    }
  }
}
