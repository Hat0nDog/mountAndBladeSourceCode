// Decompiled with JetBrains decompiler
// Type: TaleWorlds.MountAndBlade.ComponentInterfaces.BattleMoraleModel
// Assembly: TaleWorlds.MountAndBlade, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3C0E3F42-756F-4407-B817-06BA412A78B0
// Assembly location: E:\SteamLibrary\steamapps\common\Mount & Blade II Bannerlord\bin\Win64_Shipping_Client\TaleWorlds.MountAndBlade.dll

using TaleWorlds.Core;

namespace TaleWorlds.MountAndBlade.ComponentInterfaces
{
  public abstract class BattleMoraleModel : GameModel
  {
    public abstract (float killedSideMoraleChange, float killerSideMoraleChange) CalculateMoraleChangeAfterAgentKilled(
      Agent killedAgent,
      Agent killerAgent,
      SkillObject killerWeaponRelevantSkill);

    public abstract (float panickedSideMoraleChange, float affectorSideMoraleChange) CalculateMoraleChangeAfterAgentPanicked(
      Agent agent);

    public abstract float CalculateMoraleChangeToCharacter(
      Agent agent,
      float moraleChange,
      float distance);

    public abstract float GetEffectiveInitialMorale(Agent agent, float baseMorale);

    public abstract bool CanPanicDueToMorale(Agent agent);

    public abstract float GetImportance(Agent agent);
  }
}
