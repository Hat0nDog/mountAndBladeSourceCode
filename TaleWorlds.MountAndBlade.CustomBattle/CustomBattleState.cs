// Decompiled with JetBrains decompiler
// Type: TaleWorlds.MountAndBlade.CustomBattle.CustomBattleState
// Assembly: TaleWorlds.MountAndBlade.CustomBattle, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 8065FEE3-AE77-4E53-B13C-3890101BA431
// Assembly location: E:\SteamLibrary\steamapps\common\Mount & Blade II Bannerlord\Modules\CustomBattle\bin\Win64_Shipping_Client\TaleWorlds.MountAndBlade.CustomBattle.dll

using System;
using System.Collections.Generic;
using System.Linq;
using TaleWorlds.Core;
using TaleWorlds.Library;
using TaleWorlds.Localization;
using TaleWorlds.ObjectSystem;

namespace TaleWorlds.MountAndBlade.CustomBattle
{
  public class CustomBattleState : GameState
  {
    public override bool IsMusicMenuState => true;

    public void StartBattleForCombatPerformanceTests(int scene_index, int troop_count = 50)
    {
      int[] playerNumbers = new int[4];
      int[] enemyNumbers = new int[4];
      if (troop_count == -1)
      {
        for (int index = 0; index < 4; ++index)
        {
          playerNumbers[index] = MBRandom.RandomInt() % 50 + 25;
          enemyNumbers[index] = MBRandom.RandomInt() % 50 + 25;
        }
      }
      else
      {
        for (int index = 0; index < 4; ++index)
        {
          playerNumbers[index] = troop_count;
          enemyNumbers[index] = troop_count;
        }
      }
      BasicCultureObject playerFaction = Game.Current.ObjectManager.GetObject<BasicCultureObject>("empire");
      BasicCultureObject enemyFaction = Game.Current.ObjectManager.GetObject<BasicCultureObject>("vlandia");
      BasicCharacterObject basicCharacterObject = Game.Current.ObjectManager.GetObject<BasicCharacterObject>("aserai_skirmisher");
      BasicCharacterObject enemyCharacter = Game.Current.ObjectManager.GetObject<BasicCharacterObject>("aserai_skirmisher");
      CustomBattleCombatant[] customBattleParties = this.GetCustomBattleParties(basicCharacterObject, (BasicCharacterObject) null, enemyCharacter, playerFaction, playerNumbers, (List<BasicCharacterObject>[]) null, enemyFaction, enemyNumbers, (List<BasicCharacterObject>[]) null, true);
      BannerlordMissions.OpenCustomBattleMission(new string[6]
      {
        "battle_terrain_a",
        "battle_terrain_b",
        "battle_terrain_c",
        "battle_terrain_e",
        "battle_terrain_k",
        "battle_terrain_s"
      }[scene_index], basicCharacterObject, customBattleParties[0], customBattleParties[1], true, (BasicCharacterObject) null);
    }

    public CustomBattleCombatant[] GetCustomBattleParties(
      BasicCharacterObject playerCharacter,
      BasicCharacterObject playerSideGeneralCharacter,
      BasicCharacterObject enemyCharacter,
      BasicCultureObject playerFaction,
      int[] playerNumbers,
      List<BasicCharacterObject>[] playerTroopSelections,
      BasicCultureObject enemyFaction,
      int[] enemyNumbers,
      List<BasicCharacterObject>[] enemyTroopSelections,
      bool isPlayerAttacker)
    {
      CustomBattleCombatant[] customBattleCombatantArray = new CustomBattleCombatant[2]
      {
        new CustomBattleCombatant(new TextObject("{=sSJSTe5p}Player Party"), playerFaction, Banner.CreateRandomBanner()),
        new CustomBattleCombatant(new TextObject("{=0xC75dN6}Enemy Party"), enemyFaction, Banner.CreateRandomBanner())
      };
      customBattleCombatantArray[0].Side = isPlayerAttacker ? BattleSideEnum.Attacker : BattleSideEnum.Defender;
      customBattleCombatantArray[0].AddCharacter(playerCharacter, 1);
      if (playerSideGeneralCharacter != null)
        customBattleCombatantArray[0].AddCharacter(playerSideGeneralCharacter, 1);
      customBattleCombatantArray[1].Side = customBattleCombatantArray[0].Side.GetOppositeSide();
      customBattleCombatantArray[1].AddCharacter(enemyCharacter, 1);
      for (int index = 0; index < customBattleCombatantArray.Length; ++index)
        this.PopulateListsWithDefaults(ref customBattleCombatantArray[index], index == 0 ? playerNumbers : enemyNumbers, index == 0 ? playerTroopSelections : enemyTroopSelections);
      return customBattleCombatantArray;
    }

    private void PopulateListsWithDefaults(
      ref CustomBattleCombatant customBattleParties,
      int[] numbers,
      List<BasicCharacterObject>[] troopList)
    {
      BasicCultureObject basicCulture = customBattleParties.BasicCulture;
      if (troopList == null)
        troopList = new List<BasicCharacterObject>[4]
        {
          new List<BasicCharacterObject>(),
          new List<BasicCharacterObject>(),
          new List<BasicCharacterObject>(),
          new List<BasicCharacterObject>()
        };
      if (troopList[0].Count == 0)
        troopList[0] = new List<BasicCharacterObject>()
        {
          CustomBattleState.Helper.GetDefaultTroopOfFormationForFaction(basicCulture, FormationClass.Infantry)
        };
      if (troopList[1].Count == 0)
        troopList[1] = new List<BasicCharacterObject>()
        {
          CustomBattleState.Helper.GetDefaultTroopOfFormationForFaction(basicCulture, FormationClass.Ranged)
        };
      if (troopList[2].Count == 0)
        troopList[2] = new List<BasicCharacterObject>()
        {
          CustomBattleState.Helper.GetDefaultTroopOfFormationForFaction(basicCulture, FormationClass.Cavalry)
        };
      if (troopList[3].Count == 0)
        troopList[3] = new List<BasicCharacterObject>()
        {
          CustomBattleState.Helper.GetDefaultTroopOfFormationForFaction(basicCulture, FormationClass.HorseArcher)
        };
      if (!troopList[3].Any<BasicCharacterObject>() || troopList[3].All<BasicCharacterObject>((Func<BasicCharacterObject, bool>) (troop => troop == null)))
      {
        numbers[2] += numbers[3] / 3;
        numbers[1] += numbers[3] / 3;
        numbers[0] += numbers[3] / 3;
        numbers[0] += numbers[3] - numbers[3] / 3 * 3;
        numbers[3] = 0;
      }
      for (int index1 = 0; index1 < 4; ++index1)
      {
        int count = troopList[index1].Count;
        int number1 = numbers[index1];
        if (number1 > 0)
        {
          for (int index2 = 0; index2 < count; ++index2)
          {
            int number2 = number1 / count;
            customBattleParties.AddCharacter(troopList[index1][index2], number2);
            numbers[index1] -= number2;
            if (index2 == count - 1 && numbers[index1] > 0)
            {
              customBattleParties.AddCharacter(troopList[index1][index2], numbers[index1]);
              numbers[index1] = 0;
            }
          }
        }
      }
    }

    protected override void OnInitialize()
    {
      base.OnInitialize();
      CustomBattleState.Helper.AssertMissingTroopsForDebug();
    }

    [CommandLineFunctionality.CommandLineArgumentFunction("enable_custom_record", "replay_mission")]
    public static string EnableRecordMission(List<string> strings)
    {
      if (!(GameStateManager.Current.ActiveState is CustomBattleState))
        return "Mission recording for custom battle can only be enabled while in custom battle screen.";
      MissionState.RecordMission = true;
      return "Mission recording activated.";
    }

    public static class Helper
    {
      private const string EmpireInfantryTroop = "imperial_veteran_infantryman";
      private const string EmpireRangedTroop = "imperial_archer";
      private const string EmpireCavalryTroop = "imperial_heavy_horseman";
      private const string EmpireHorseArcherTroop = "bucellarii";
      private const string EmpireInfantryBannermanTroop = "imperial_infantry_banner_bearer";
      private const string EmpireCavalryBannermanTroop = "imperial_cavalry_banner_bearer";
      private const string SturgiaInfantryTroop = "sturgian_spearman";
      private const string SturgiaRangedTroop = "sturgian_archer";
      private const string SturgiaCavalryTroop = "sturgian_hardened_brigand";
      private const string SturgiaInfantryBannermanTroop = "sturgian_infantry_banner_bearer";
      private const string SturgiaCavalryBannermanTroop = "sturgian_cavalry_banner_bearer";
      private const string AseraiInfantryTroop = "aserai_infantry";
      private const string AseraiRangedTroop = "aserai_archer";
      private const string AseraiCavalryTroop = "aserai_mameluke_cavalry";
      private const string AseraiHorseArcherTroop = "aserai_faris";
      private const string AseraiInfantryBannermanTroop = "aserai_infantry_banner_bearer";
      private const string AseraiCavalryBannermanTroop = "aserai_cavalry_banner_bearer";
      private const string VlandiaInfantryTroop = "vlandian_swordsman";
      private const string VlandiaRangedTroop = "vlandian_hardened_crossbowman";
      private const string VlandiaCavalryTroop = "vlandian_knight";
      private const string VlandiaInfantryBannermanTroop = "vlandian_infantry_banner_bearer";
      private const string VlandiaCavalryBannermanTroop = "vlandian_cavalry_banner_bearer";
      private const string BattaniaInfantryTroop = "battanian_picked_warrior";
      private const string BattaniaRangedTroop = "battanian_hero";
      private const string BattaniaCavalryTroop = "battanian_scout";
      private const string BattaniaInfantryBannermanTroop = "battanian_woodrunner";
      private const string BattaniaCavalryBannermanTroop = "battanian_cavalry_banner_bearer";
      private const string KhuzaitInfantryTroop = "khuzait_spear_infantry";
      private const string KhuzaitRangedTroop = "khuzait_archer";
      private const string KhuzaitCavalryTroop = "khuzait_lancer";
      private const string KhuzaitHorseArcherTroop = "khuzait_horse_archer";
      private const string KhuzaitInfantryBannermanTroop = "khuzait_infantry_banner_bearer";
      private const string KhuzaitCavalryBannermanTroop = "khuzait_cavalry_banner_bearer";

      public static void AssertMissingTroopsForDebug()
      {
        foreach (BasicCultureObject objectType in MBObjectManager.Instance.GetObjectTypeList<BasicCultureObject>())
        {
          for (int index = 0; index < 4; ++index)
            CustomBattleState.Helper.GetDefaultTroopOfFormationForFaction(objectType, (FormationClass) index);
        }
      }

      public static BasicCharacterObject GetDefaultTroopOfFormationForFaction(
        BasicCultureObject culture,
        FormationClass formation)
      {
        if (culture.StringId.ToLower() == "empire")
        {
          switch (formation)
          {
            case FormationClass.Infantry:
              return CustomBattleState.Helper.GetTroopFromId("imperial_veteran_infantryman");
            case FormationClass.Ranged:
              return CustomBattleState.Helper.GetTroopFromId("imperial_archer");
            case FormationClass.Cavalry:
              return CustomBattleState.Helper.GetTroopFromId("imperial_heavy_horseman");
            case FormationClass.HorseArcher:
              return CustomBattleState.Helper.GetTroopFromId("bucellarii");
          }
        }
        else if (culture.StringId.ToLower() == "sturgia")
        {
          switch (formation)
          {
            case FormationClass.Infantry:
              return CustomBattleState.Helper.GetTroopFromId("sturgian_spearman");
            case FormationClass.Ranged:
              return CustomBattleState.Helper.GetTroopFromId("sturgian_archer");
            case FormationClass.Cavalry:
              return CustomBattleState.Helper.GetTroopFromId("sturgian_hardened_brigand");
          }
        }
        else if (culture.StringId.ToLower() == "aserai")
        {
          switch (formation)
          {
            case FormationClass.Infantry:
              return CustomBattleState.Helper.GetTroopFromId("aserai_infantry");
            case FormationClass.Ranged:
              return CustomBattleState.Helper.GetTroopFromId("aserai_archer");
            case FormationClass.Cavalry:
              return CustomBattleState.Helper.GetTroopFromId("aserai_mameluke_cavalry");
            case FormationClass.HorseArcher:
              return CustomBattleState.Helper.GetTroopFromId("aserai_faris");
          }
        }
        else if (culture.StringId.ToLower() == "vlandia")
        {
          switch (formation)
          {
            case FormationClass.Infantry:
              return CustomBattleState.Helper.GetTroopFromId("vlandian_swordsman");
            case FormationClass.Ranged:
              return CustomBattleState.Helper.GetTroopFromId("vlandian_hardened_crossbowman");
            case FormationClass.Cavalry:
              return CustomBattleState.Helper.GetTroopFromId("vlandian_knight");
          }
        }
        else if (culture.StringId.ToLower() == "battania")
        {
          switch (formation)
          {
            case FormationClass.Infantry:
              return CustomBattleState.Helper.GetTroopFromId("battanian_picked_warrior");
            case FormationClass.Ranged:
              return CustomBattleState.Helper.GetTroopFromId("battanian_hero");
            case FormationClass.Cavalry:
              return CustomBattleState.Helper.GetTroopFromId("battanian_scout");
          }
        }
        else if (culture.StringId.ToLower() == "khuzait")
        {
          switch (formation)
          {
            case FormationClass.Infantry:
              return CustomBattleState.Helper.GetTroopFromId("khuzait_spear_infantry");
            case FormationClass.Ranged:
              return CustomBattleState.Helper.GetTroopFromId("khuzait_archer");
            case FormationClass.Cavalry:
              return CustomBattleState.Helper.GetTroopFromId("khuzait_lancer");
            case FormationClass.HorseArcher:
              return CustomBattleState.Helper.GetTroopFromId("khuzait_horse_archer");
          }
        }
        return (BasicCharacterObject) null;
      }

      public static BasicCharacterObject GetBannermanTroopOfFormationForFaction(
        BasicCultureObject culture,
        FormationClass formation)
      {
        if (culture.StringId.ToLower() == "empire")
        {
          if (formation == FormationClass.Infantry)
            return CustomBattleState.Helper.GetTroopFromId("imperial_infantry_banner_bearer");
          if (formation == FormationClass.Cavalry)
            return CustomBattleState.Helper.GetTroopFromId("imperial_cavalry_banner_bearer");
        }
        else if (culture.StringId.ToLower() == "sturgia")
        {
          if (formation == FormationClass.Infantry)
            return CustomBattleState.Helper.GetTroopFromId("sturgian_infantry_banner_bearer");
          if (formation == FormationClass.Cavalry)
            return CustomBattleState.Helper.GetTroopFromId("sturgian_cavalry_banner_bearer");
        }
        else if (culture.StringId.ToLower() == "aserai")
        {
          if (formation == FormationClass.Infantry)
            return CustomBattleState.Helper.GetTroopFromId("aserai_infantry_banner_bearer");
          if (formation == FormationClass.Cavalry)
            return CustomBattleState.Helper.GetTroopFromId("aserai_cavalry_banner_bearer");
        }
        else if (culture.StringId.ToLower() == "vlandia")
        {
          if (formation == FormationClass.Infantry)
            return CustomBattleState.Helper.GetTroopFromId("vlandian_infantry_banner_bearer");
          if (formation == FormationClass.Cavalry)
            return CustomBattleState.Helper.GetTroopFromId("vlandian_cavalry_banner_bearer");
        }
        else if (culture.StringId.ToLower() == "battania")
        {
          if (formation == FormationClass.Infantry)
            return CustomBattleState.Helper.GetTroopFromId("battanian_woodrunner");
          if (formation == FormationClass.Cavalry)
            return CustomBattleState.Helper.GetTroopFromId("battanian_cavalry_banner_bearer");
        }
        else if (culture.StringId.ToLower() == "khuzait")
        {
          if (formation == FormationClass.Infantry)
            return CustomBattleState.Helper.GetTroopFromId("khuzait_infantry_banner_bearer");
          if (formation == FormationClass.Cavalry)
            return CustomBattleState.Helper.GetTroopFromId("khuzait_cavalry_banner_bearer");
        }
        return (BasicCharacterObject) null;
      }

      private static BasicCharacterObject GetTroopFromId(string troopId) => MBObjectManager.Instance.GetObject<BasicCharacterObject>(troopId);
    }
  }
}
