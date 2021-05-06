// Decompiled with JetBrains decompiler
// Type: TaleWorlds.MountAndBlade.CustomBattleMoraleModel
// Assembly: TaleWorlds.MountAndBlade, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3C0E3F42-756F-4407-B817-06BA412A78B0
// Assembly location: E:\SteamLibrary\steamapps\common\Mount & Blade II Bannerlord\bin\Win64_Shipping_Client\TaleWorlds.MountAndBlade.dll

using System;
using TaleWorlds.Core;
using TaleWorlds.Library;
using TaleWorlds.MountAndBlade.ComponentInterfaces;

namespace TaleWorlds.MountAndBlade
{
  public class CustomBattleMoraleModel : BattleMoraleModel
  {
    public override (float killedSideMoraleChange, float killerSideMoraleChange) CalculateMoraleChangeAfterAgentKilled(
      Agent killedAgent,
      Agent killerAgent,
      SkillObject killerWeaponRelevantSkill)
    {
      float importance = this.GetImportance(killedAgent);
      return (-10f * importance, 6f * importance);
    }

    public override (float panickedSideMoraleChange, float affectorSideMoraleChange) CalculateMoraleChangeAfterAgentPanicked(
      Agent agent)
    {
      float importance = this.GetImportance(agent);
      return (-9f * importance, 7f * importance);
    }

    public override float CalculateMoraleChangeToCharacter(
      Agent agent,
      float moraleChange,
      float distance)
    {
      return moraleChange / MathF.Clamp(agent.Character.GetPower(), 0.7f, 10f);
    }

    public override float GetEffectiveInitialMorale(Agent agent, float baseMorale) => baseMorale;

    public override bool CanPanicDueToMorale(Agent agent) => true;

    public override float GetImportance(Agent agent)
    {
      bool flag = false;
      int num1 = 8;
      if (agent.Team != null)
      {
        flag = agent == agent.Team.GeneralAgent;
        num1 = agent.Team.ActiveAgents.Count;
      }
      BasicCharacterObject character = agent.Character;
      float num2 = MathF.Pow(Math.Max(character.GetPower(), 0.7f), 0.8f) * MathF.Clamp(3f / MathF.Pow((float) (num1 + 1), 0.5f), 0.3f, 1f);
      if (flag)
        num2 *= 2f;
      if (character.IsHero)
        num2 *= 2f;
      return num2;
    }
  }
}
