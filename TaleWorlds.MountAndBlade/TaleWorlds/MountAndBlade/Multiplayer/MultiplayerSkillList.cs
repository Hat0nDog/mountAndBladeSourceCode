// Decompiled with JetBrains decompiler
// Type: TaleWorlds.MountAndBlade.Multiplayer.MultiplayerSkillList
// Assembly: TaleWorlds.MountAndBlade, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3C0E3F42-756F-4407-B817-06BA412A78B0
// Assembly location: E:\SteamLibrary\steamapps\common\Mount & Blade II Bannerlord\bin\Win64_Shipping_Client\TaleWorlds.MountAndBlade.dll

using System.Collections.Generic;
using TaleWorlds.Core;

namespace TaleWorlds.MountAndBlade.Multiplayer
{
  internal class MultiplayerSkillList : SkillList
  {
    internal MultiplayerSkillList()
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
