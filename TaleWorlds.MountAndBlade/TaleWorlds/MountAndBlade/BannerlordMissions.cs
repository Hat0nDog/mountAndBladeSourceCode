// Decompiled with JetBrains decompiler
// Type: TaleWorlds.MountAndBlade.BannerlordMissions
// Assembly: TaleWorlds.MountAndBlade, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3C0E3F42-756F-4407-B817-06BA412A78B0
// Assembly location: E:\SteamLibrary\steamapps\common\Mount & Blade II Bannerlord\bin\Win64_Shipping_Client\TaleWorlds.MountAndBlade.dll

using System;
using System.Collections.Generic;
using System.Linq;
using TaleWorlds.Core;
using TaleWorlds.Library;
using TaleWorlds.MountAndBlade.Missions.Handlers;
using TaleWorlds.MountAndBlade.MissionSpawnHandlers;
using TaleWorlds.MountAndBlade.Source.Missions;
using TaleWorlds.MountAndBlade.Source.Missions.Handlers.Logic;

namespace TaleWorlds.MountAndBlade
{
  [MissionManager]
  public static class BannerlordMissions
  {
    private const string Level1Tag = "level_1";
    private const string Level2Tag = "level_2";
    private const string Level3Tag = "level_3";
    private const string SiegeTag = "siege";
    private const string SallyOutTag = "sally";

    private static Type GetSiegeWeaponType(SiegeEngineType siegeWeaponType)
    {
      if (siegeWeaponType == DefaultSiegeEngineTypes.Ladder)
        return typeof (SiegeLadder);
      if (siegeWeaponType == DefaultSiegeEngineTypes.Ballista)
        return typeof (Ballista);
      if (siegeWeaponType == DefaultSiegeEngineTypes.FireBallista)
        return typeof (FireBallista);
      if (siegeWeaponType == DefaultSiegeEngineTypes.Ram || siegeWeaponType == DefaultSiegeEngineTypes.ImprovedRam)
        return typeof (BatteringRam);
      if (siegeWeaponType == DefaultSiegeEngineTypes.SiegeTower)
        return typeof (SiegeTower);
      if (siegeWeaponType == DefaultSiegeEngineTypes.Onager || siegeWeaponType == DefaultSiegeEngineTypes.Catapult)
        return typeof (Mangonel);
      if (siegeWeaponType == DefaultSiegeEngineTypes.FireOnager || siegeWeaponType == DefaultSiegeEngineTypes.FireCatapult)
        return typeof (FireMangonel);
      return siegeWeaponType == DefaultSiegeEngineTypes.Trebuchet || siegeWeaponType == DefaultSiegeEngineTypes.Bricole ? typeof (Trebuchet) : (Type) null;
    }

    private static Dictionary<Type, int> GetSiegeWeaponTypes(
      Dictionary<SiegeEngineType, int> values)
    {
      Dictionary<Type, int> dictionary = new Dictionary<Type, int>();
      foreach (KeyValuePair<SiegeEngineType, int> keyValuePair in values)
        dictionary.Add(BannerlordMissions.GetSiegeWeaponType(keyValuePair.Key), keyValuePair.Value);
      return dictionary;
    }

    private static AtmosphereInfo CreateAtmosphereInfoForMission(
      string seasonId,
      int timeOfDay)
    {
      Dictionary<string, int> dictionary1 = new Dictionary<string, int>();
      dictionary1.Add("spring", 0);
      dictionary1.Add("summer", 1);
      dictionary1.Add("fall", 2);
      dictionary1.Add("winter", 3);
      int num = 0;
      dictionary1.TryGetValue(seasonId, out num);
      Dictionary<int, string> dictionary2 = new Dictionary<int, string>();
      dictionary2.Add(6, "TOD_06_00_SemiCloudy");
      dictionary2.Add(12, "TOD_12_00_SemiCloudy");
      dictionary2.Add(15, "TOD_04_00_SemiCloudy");
      dictionary2.Add(18, "TOD_03_00_SemiCloudy");
      dictionary2.Add(22, "TOD_01_00_SemiCloudy");
      string str = "field_battle";
      dictionary2.TryGetValue(timeOfDay, out str);
      return new AtmosphereInfo()
      {
        AtmosphereName = str,
        TimeInfo = new TimeInformation() { Season = num }
      };
    }

    [MissionMethod]
    public static Mission OpenCustomBattleMission(
      string scene,
      BasicCharacterObject character,
      CustomBattleCombatant playerParty,
      CustomBattleCombatant enemyParty,
      bool isPlayerGeneral,
      BasicCharacterObject playerSideGeneralCharacter,
      string sceneLevels = "",
      string seasonString = "",
      float timeOfDay = 6f)
    {
      BattleSideEnum playerSide = playerParty.Side;
      bool isPlayerAttacker = playerSide == BattleSideEnum.Attacker;
      IMissionTroopSupplier[] troopSuppliers = new IMissionTroopSupplier[2];
      CustomBattleTroopSupplier battleTroopSupplier1 = new CustomBattleTroopSupplier(playerParty, true);
      troopSuppliers[(int) playerParty.Side] = (IMissionTroopSupplier) battleTroopSupplier1;
      CustomBattleTroopSupplier battleTroopSupplier2 = new CustomBattleTroopSupplier(enemyParty, false);
      troopSuppliers[(int) enemyParty.Side] = (IMissionTroopSupplier) battleTroopSupplier2;
      bool isPlayerSergeant = !isPlayerGeneral;
      return MissionState.OpenNew("CustomBattle", new MissionInitializerRecord(scene)
      {
        DoNotUseLoadingScreen = false,
        PlayingInCampaignMode = false,
        AtmosphereOnCampaign = BannerlordMissions.CreateAtmosphereInfoForMission(seasonString, (int) timeOfDay),
        SceneLevels = sceneLevels,
        TimeOfDay = timeOfDay
      }, (InitializeMissionBehvaioursDelegate) (missionController => (IEnumerable<MissionBehaviour>) new MissionBehaviour[20]
      {
        (MissionBehaviour) new MissionOptionsComponent(),
        (MissionBehaviour) new BattleEndLogic(),
        (MissionBehaviour) new MissionCombatantsLogic((IEnumerable<IBattleCombatant>) null, (IBattleCombatant) playerParty, !isPlayerAttacker ? (IBattleCombatant) playerParty : (IBattleCombatant) enemyParty, isPlayerAttacker ? (IBattleCombatant) playerParty : (IBattleCombatant) enemyParty, Mission.MissionTeamAITypeEnum.FieldBattle, isPlayerSergeant),
        (MissionBehaviour) new BattleObserverMissionLogic(),
        (MissionBehaviour) new CustomBattleAgentLogic(),
        (MissionBehaviour) new MissionAgentSpawnLogic(troopSuppliers, playerSide),
        (MissionBehaviour) new CustomBattleMissionSpawnHandler(!isPlayerAttacker ? playerParty : enemyParty, isPlayerAttacker ? playerParty : enemyParty),
        (MissionBehaviour) new AgentBattleAILogic(),
        (MissionBehaviour) new AgentVictoryLogic(),
        (MissionBehaviour) new MissionAgentPanicHandler(),
        (MissionBehaviour) new MissionHardBorderPlacer(),
        (MissionBehaviour) new MissionBoundaryPlacer(),
        (MissionBehaviour) new MissionBoundaryCrossingHandler(),
        (MissionBehaviour) new BattleMissionAgentInteractionLogic(),
        (MissionBehaviour) new AgentFadeOutLogic(),
        (MissionBehaviour) new AgentMoraleInteractionLogic(),
        (MissionBehaviour) new AssignPlayerRoleInTeamMissionController(isPlayerGeneral, isPlayerSergeant, false, isPlayerSergeant ? Enumerable.Repeat<string>(character.StringId, 1).ToList<string>() : new List<string>()),
        (MissionBehaviour) new CreateBodyguardMissionBehavior(isPlayerAttacker & isPlayerGeneral ? character.GetName().ToString() : (isPlayerAttacker & isPlayerSergeant ? playerSideGeneralCharacter.GetName().ToString() : (string) null), !isPlayerAttacker & isPlayerGeneral ? character.GetName().ToString() : (!isPlayerAttacker & isPlayerSergeant ? playerSideGeneralCharacter.GetName().ToString() : (string) null)),
        (MissionBehaviour) new HighlightsController(),
        (MissionBehaviour) new BattleHighlightsController()
      }));
    }

    [MissionMethod]
    public static Mission OpenSiegeMissionWithDeployment(
      string scene,
      BasicCharacterObject character,
      CustomBattleCombatant playerParty,
      CustomBattleCombatant enemyParty,
      bool isPlayerGeneral,
      float[] wallHitPointPercentages,
      bool hasAnySiegeTower,
      Dictionary<SiegeEngineType, int> siegeWeaponsCountOfAttackers,
      Dictionary<SiegeEngineType, int> siegeWeaponsCountOfDefenders,
      bool isPlayerAttacker,
      int sceneUpgradeLevel = 0,
      string seasonString = "",
      bool isSallyOut = false,
      bool isReliefForceAttack = false,
      float timeOfDay = 6f)
    {
      string str1;
      switch (sceneUpgradeLevel)
      {
        case 1:
          str1 = "level_1";
          break;
        case 2:
          str1 = "level_2";
          break;
        default:
          str1 = "level_3";
          break;
      }
      string str2 = str1 + " siege";
      BattleSideEnum playerSide = playerParty.Side;
      IMissionTroopSupplier[] troopSuppliers = new IMissionTroopSupplier[2];
      CustomBattleTroopSupplier battleTroopSupplier1 = new CustomBattleTroopSupplier(playerParty, true);
      troopSuppliers[(int) playerParty.Side] = (IMissionTroopSupplier) battleTroopSupplier1;
      CustomBattleTroopSupplier battleTroopSupplier2 = new CustomBattleTroopSupplier(enemyParty, false);
      troopSuppliers[(int) enemyParty.Side] = (IMissionTroopSupplier) battleTroopSupplier2;
      bool isPlayerSergeant = !isPlayerGeneral;
      return MissionState.OpenNew("CustomSiegeBattle", new MissionInitializerRecord(scene)
      {
        PlayingInCampaignMode = false,
        AtmosphereOnCampaign = BannerlordMissions.CreateAtmosphereInfoForMission(seasonString, (int) timeOfDay),
        SceneLevels = str2,
        TimeOfDay = timeOfDay
      }, (InitializeMissionBehvaioursDelegate) (mission =>
      {
        List<MissionBehaviour> missionBehaviourList = new List<MissionBehaviour>();
        missionBehaviourList.Add((MissionBehaviour) new BattleSpawnLogic(isSallyOut ? "sally_out_set" : (isReliefForceAttack ? "relief_force_attack_set" : "battle_set")));
        missionBehaviourList.Add((MissionBehaviour) new MissionOptionsComponent());
        missionBehaviourList.Add((MissionBehaviour) new BattleEndLogic());
        missionBehaviourList.Add((MissionBehaviour) new MissionCombatantsLogic((IEnumerable<IBattleCombatant>) null, (IBattleCombatant) playerParty, !isPlayerAttacker ? (IBattleCombatant) playerParty : (IBattleCombatant) enemyParty, isPlayerAttacker ? (IBattleCombatant) playerParty : (IBattleCombatant) enemyParty, !isSallyOut ? Mission.MissionTeamAITypeEnum.Siege : Mission.MissionTeamAITypeEnum.SallyOut, isPlayerSergeant));
        missionBehaviourList.Add((MissionBehaviour) new SiegeMissionPreparationHandler(isSallyOut, isReliefForceAttack, wallHitPointPercentages, hasAnySiegeTower));
        missionBehaviourList.Add((MissionBehaviour) new MissionAgentSpawnLogic(troopSuppliers, playerSide));
        if (isSallyOut)
        {
          missionBehaviourList.Add((MissionBehaviour) new CustomSiegeSallyOutMissionSpawnHandler(!isPlayerAttacker ? (IBattleCombatant) playerParty : (IBattleCombatant) enemyParty, isPlayerAttacker ? (IBattleCombatant) playerParty : (IBattleCombatant) enemyParty));
        }
        else
        {
          missionBehaviourList.Add((MissionBehaviour) new CustomSiegeMissionSpawnHandler(!isPlayerAttacker ? (IBattleCombatant) playerParty : (IBattleCombatant) enemyParty, isPlayerAttacker ? (IBattleCombatant) playerParty : (IBattleCombatant) enemyParty));
          missionBehaviourList.Add((MissionBehaviour) new AgentFadeOutLogic());
        }
        missionBehaviourList.Add((MissionBehaviour) new BattleObserverMissionLogic());
        missionBehaviourList.Add((MissionBehaviour) new CustomBattleAgentLogic());
        missionBehaviourList.Add((MissionBehaviour) new AgentBattleAILogic());
        if (!isSallyOut)
          missionBehaviourList.Add((MissionBehaviour) new AmmoSupplyLogic(new List<BattleSideEnum>()
          {
            BattleSideEnum.Defender
          }));
        missionBehaviourList.Add((MissionBehaviour) new AgentVictoryLogic());
        missionBehaviourList.Add((MissionBehaviour) new SiegeMissionController(BannerlordMissions.GetSiegeWeaponTypes(siegeWeaponsCountOfAttackers), BannerlordMissions.GetSiegeWeaponTypes(siegeWeaponsCountOfDefenders), isPlayerAttacker, isSallyOut));
        missionBehaviourList.Add((MissionBehaviour) new AssignPlayerRoleInTeamMissionController(isPlayerGeneral, isPlayerSergeant, false));
        SiegeDeploymentHandler deploymentHandler = new SiegeDeploymentHandler(isPlayerAttacker, isPlayerAttacker ? BannerlordMissions.GetSiegeWeaponTypes(siegeWeaponsCountOfAttackers) : BannerlordMissions.GetSiegeWeaponTypes(siegeWeaponsCountOfDefenders));
        missionBehaviourList.Add((MissionBehaviour) deploymentHandler);
        missionBehaviourList.Add((MissionBehaviour) new MissionAgentPanicHandler());
        missionBehaviourList.Add((MissionBehaviour) new MissionBoundaryPlacer());
        missionBehaviourList.Add((MissionBehaviour) new MissionBoundaryCrossingHandler());
        missionBehaviourList.Add((MissionBehaviour) new AgentMoraleInteractionLogic());
        missionBehaviourList.Add((MissionBehaviour) new HighlightsController());
        missionBehaviourList.Add((MissionBehaviour) new BattleHighlightsController());
        return (IEnumerable<MissionBehaviour>) missionBehaviourList.ToArray();
      }));
    }

    private enum CustomBattleGameTypes
    {
      AttackerGeneral,
      DefenderGeneral,
      AttackerSergeant,
      DefenderSergeant,
    }
  }
}
