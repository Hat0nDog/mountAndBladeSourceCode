// Decompiled with JetBrains decompiler
// Type: TaleWorlds.MountAndBlade.MultiplayerBattleMoraleModel
// Assembly: TaleWorlds.MountAndBlade, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3C0E3F42-756F-4407-B817-06BA412A78B0
// Assembly location: E:\SteamLibrary\steamapps\common\Mount & Blade II Bannerlord\bin\Win64_Shipping_Client\TaleWorlds.MountAndBlade.dll

using TaleWorlds.Core;
using TaleWorlds.MountAndBlade.ComponentInterfaces;

namespace TaleWorlds.MountAndBlade
{
  public class MultiplayerBattleMoraleModel : BattleMoraleModel
  {
    public override (float killedSideMoraleChange, float killerSideMoraleChange) CalculateMoraleChangeAfterAgentKilled(
      Agent killedAgent,
      Agent killerAgent,
      SkillObject killerWeaponRelevantSkill)
    {
      return (0.0f, 0.0f);
    }

    public override (float panickedSideMoraleChange, float affectorSideMoraleChange) CalculateMoraleChangeAfterAgentPanicked(
      Agent agent)
    {
      return (0.0f, 0.0f);
    }

    public override float CalculateMoraleChangeToCharacter(
      Agent agent,
      float moraleChange,
      float distance)
    {
      return 0.0f;
    }

    public override float GetEffectiveInitialMorale(Agent agent, float baseMorale) => baseMorale;

    public override bool CanPanicDueToMorale(Agent agent) => true;

    public override float GetImportance(Agent agent) => 0.0f;
  }
}
