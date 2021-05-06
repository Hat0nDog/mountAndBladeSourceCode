// Decompiled with JetBrains decompiler
// Type: TaleWorlds.MountAndBlade.CustomBattle.CustomBattleSkillList
// Assembly: TaleWorlds.MountAndBlade.CustomBattle, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 8065FEE3-AE77-4E53-B13C-3890101BA431
// Assembly location: E:\SteamLibrary\steamapps\common\Mount & Blade II Bannerlord\Modules\CustomBattle\bin\Win64_Shipping_Client\TaleWorlds.MountAndBlade.CustomBattle.dll

using System.Collections.Generic;
using TaleWorlds.Core;

namespace TaleWorlds.MountAndBlade.CustomBattle
{
  internal class CustomBattleSkillList : SkillList
  {
    internal CustomBattleSkillList()
    {
    }

    public override IEnumerable<SkillObject> GetSkillList()
    {
      yield return DefaultSkills.OneHanded;
      yield return DefaultSkills.TwoHanded;
      yield return DefaultSkills.Polearm;
      yield return DefaultSkills.Bow;
      yield return DefaultSkills.Crossbow;
      yield return DefaultSkills.Throwing;
      yield return DefaultSkills.Riding;
      yield return DefaultSkills.Athletics;
      yield return DefaultSkills.Tactics;
      yield return DefaultSkills.Scouting;
      yield return DefaultSkills.Roguery;
      yield return DefaultSkills.Crafting;
      yield return DefaultSkills.Charm;
      yield return DefaultSkills.Trade;
      yield return DefaultSkills.Leadership;
      yield return DefaultSkills.Steward;
      yield return DefaultSkills.Medicine;
      yield return DefaultSkills.Engineering;
    }
  }
}
