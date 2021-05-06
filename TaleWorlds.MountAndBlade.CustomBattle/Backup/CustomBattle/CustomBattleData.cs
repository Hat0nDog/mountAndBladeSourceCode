// Decompiled with JetBrains decompiler
// Type: TaleWorlds.MountAndBlade.CustomBattle.CustomBattle.CustomBattleData
// Assembly: TaleWorlds.MountAndBlade.CustomBattle, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 8065FEE3-AE77-4E53-B13C-3890101BA431
// Assembly location: E:\SteamLibrary\steamapps\common\Mount & Blade II Bannerlord\Modules\CustomBattle\bin\Win64_Shipping_Client\TaleWorlds.MountAndBlade.CustomBattle.dll

using System;
using System.Collections.Generic;
using TaleWorlds.Core;
using TaleWorlds.Localization;

namespace TaleWorlds.MountAndBlade.CustomBattle.CustomBattle
{
  public struct CustomBattleData
  {
    public const int NumberOfAttackerMeleeMachines = 3;
    public const int NumberOfAttackerRangedMachines = 4;
    public const int NumberOfDefenderRangedMachines = 4;
    public CustomBattleGameType GameType;
    public string SceneId;
    public string SeasonId;
    public BasicCharacterObject PlayerCharacter;
    public BasicCharacterObject PlayerSideGeneralCharacter;
    public CustomBattleCombatant PlayerParty;
    public CustomBattleCombatant EnemyParty;
    public float TimeOfDay;
    public bool IsPlayerGeneral;
    public string SceneLevel;
    public Dictionary<SiegeEngineType, int> AttackerMachines;
    public Dictionary<SiegeEngineType, int> DefenderMachines;
    public float[] WallHitpointPercentages;
    public bool HasAnySiegeTower;
    public bool IsPlayerAttacker;
    public bool IsReliefAttack;
    public bool IsSallyOut;
    public int SceneUpgradeLevel;

    public static IEnumerable<SiegeEngineType> GetAllAttackerMeleeMachines()
    {
      yield return DefaultSiegeEngineTypes.Ram;
      yield return DefaultSiegeEngineTypes.SiegeTower;
    }

    public static IEnumerable<SiegeEngineType> GetAllDefenderRangedMachines()
    {
      yield return DefaultSiegeEngineTypes.Ballista;
      yield return DefaultSiegeEngineTypes.FireBallista;
      yield return DefaultSiegeEngineTypes.Catapult;
      yield return DefaultSiegeEngineTypes.FireCatapult;
    }

    public static IEnumerable<SiegeEngineType> GetAllAttackerRangedMachines()
    {
      yield return DefaultSiegeEngineTypes.Ballista;
      yield return DefaultSiegeEngineTypes.FireBallista;
      yield return DefaultSiegeEngineTypes.Onager;
      yield return DefaultSiegeEngineTypes.FireOnager;
      yield return DefaultSiegeEngineTypes.Trebuchet;
    }

    public static IEnumerable<Tuple<string, CustomBattleGameType>> GameTypes
    {
      get
      {
        yield return new Tuple<string, CustomBattleGameType>(GameTexts.FindText("str_battle").ToString(), CustomBattleGameType.Battle);
        yield return new Tuple<string, CustomBattleGameType>(new TextObject("{=Ua6CNLBZ}Village").ToString(), CustomBattleGameType.Village);
        yield return new Tuple<string, CustomBattleGameType>(GameTexts.FindText("str_siege").ToString(), CustomBattleGameType.Siege);
      }
    }

    public static IEnumerable<Tuple<string, CustomBattlePlayerType>> PlayerTypes
    {
      get
      {
        yield return new Tuple<string, CustomBattlePlayerType>(GameTexts.FindText("str_team_commander").ToString(), CustomBattlePlayerType.Commander);
        yield return new Tuple<string, CustomBattlePlayerType>(new TextObject("{=g9VIbA9s}Sergeant").ToString(), CustomBattlePlayerType.Sergeant);
      }
    }

    public static IEnumerable<Tuple<string, CustomBattlePlayerSide>> PlayerSides
    {
      get
      {
        yield return new Tuple<string, CustomBattlePlayerSide>(new TextObject("{=XEVFUaFj}Defender").ToString(), CustomBattlePlayerSide.Defender);
        yield return new Tuple<string, CustomBattlePlayerSide>(new TextObject("{=KASD0tnO}Attacker").ToString(), CustomBattlePlayerSide.Attacker);
      }
    }

    public static IEnumerable<BasicCharacterObject> Characters
    {
      get
      {
        yield return Game.Current.ObjectManager.GetObject<BasicCharacterObject>("commander_1");
        yield return Game.Current.ObjectManager.GetObject<BasicCharacterObject>("commander_2");
        yield return Game.Current.ObjectManager.GetObject<BasicCharacterObject>("commander_3");
        yield return Game.Current.ObjectManager.GetObject<BasicCharacterObject>("commander_4");
        yield return Game.Current.ObjectManager.GetObject<BasicCharacterObject>("commander_5");
        yield return Game.Current.ObjectManager.GetObject<BasicCharacterObject>("commander_6");
        yield return Game.Current.ObjectManager.GetObject<BasicCharacterObject>("commander_7");
        yield return Game.Current.ObjectManager.GetObject<BasicCharacterObject>("commander_8");
        yield return Game.Current.ObjectManager.GetObject<BasicCharacterObject>("commander_9");
        yield return Game.Current.ObjectManager.GetObject<BasicCharacterObject>("commander_10");
        yield return Game.Current.ObjectManager.GetObject<BasicCharacterObject>("commander_11");
        yield return Game.Current.ObjectManager.GetObject<BasicCharacterObject>("commander_12");
      }
    }

    public static IEnumerable<BasicCultureObject> Factions
    {
      get
      {
        yield return Game.Current.ObjectManager.GetObject<BasicCultureObject>("empire");
        yield return Game.Current.ObjectManager.GetObject<BasicCultureObject>("sturgia");
        yield return Game.Current.ObjectManager.GetObject<BasicCultureObject>("aserai");
        yield return Game.Current.ObjectManager.GetObject<BasicCultureObject>("vlandia");
        yield return Game.Current.ObjectManager.GetObject<BasicCultureObject>("battania");
        yield return Game.Current.ObjectManager.GetObject<BasicCultureObject>("khuzait");
      }
    }

    public static IEnumerable<Tuple<string, CustomBattleTimeOfDay>> TimesOfDay
    {
      get
      {
        yield return new Tuple<string, CustomBattleTimeOfDay>(new TextObject("{=X3gcUz7C}Morning").ToString(), CustomBattleTimeOfDay.Morning);
        yield return new Tuple<string, CustomBattleTimeOfDay>(new TextObject("{=CTtjSwRb}Noon").ToString(), CustomBattleTimeOfDay.Noon);
        yield return new Tuple<string, CustomBattleTimeOfDay>(new TextObject("{=J2gvnexb}Afternoon").ToString(), CustomBattleTimeOfDay.Afternoon);
        yield return new Tuple<string, CustomBattleTimeOfDay>(new TextObject("{=gENb9SSW}Evening").ToString(), CustomBattleTimeOfDay.Evening);
        yield return new Tuple<string, CustomBattleTimeOfDay>(new TextObject("{=fAxjyMt5}Night").ToString(), CustomBattleTimeOfDay.Night);
      }
    }

    public static IEnumerable<Tuple<string, string>> Seasons
    {
      get
      {
        yield return new Tuple<string, string>(new TextObject("{=f7vOVQb7}Summer").ToString(), "summer");
        yield return new Tuple<string, string>(new TextObject("{=cZzfNlxd}Fall").ToString(), "fall");
        yield return new Tuple<string, string>(new TextObject("{=nwqUFaU8}Winter").ToString(), "winter");
        yield return new Tuple<string, string>(new TextObject("{=nWbp3o3H}Spring").ToString(), "spring");
      }
    }

    public static IEnumerable<Tuple<string, int>> WallHitpoints
    {
      get
      {
        yield return new Tuple<string, int>(new TextObject("{=dsMeB3vi}Solid").ToString(), 0);
        yield return new Tuple<string, int>(new TextObject("{=Kvxo2jzJ}Single Breached").ToString(), 1);
        yield return new Tuple<string, int>(new TextObject("{=AiNXIt5N}Dual Breached").ToString(), 2);
      }
    }

    public static IEnumerable<int> SceneLevels
    {
      get
      {
        yield return 1;
        yield return 2;
        yield return 3;
      }
    }
  }
}
