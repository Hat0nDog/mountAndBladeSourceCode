// Decompiled with JetBrains decompiler
// Type: TaleWorlds.MountAndBlade.MultiplayerMissions
// Assembly: TaleWorlds.MountAndBlade, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3C0E3F42-756F-4407-B817-06BA412A78B0
// Assembly location: E:\SteamLibrary\steamapps\common\Mount & Blade II Bannerlord\bin\Win64_Shipping_Client\TaleWorlds.MountAndBlade.dll

using System.Collections.Generic;
using TaleWorlds.Core;
using TaleWorlds.MountAndBlade.Source.Missions;

namespace TaleWorlds.MountAndBlade
{
  [MissionManager]
  public static class MultiplayerMissions
  {
    [MissionMethod]
    public static void OpenFreeForAllMission(string scene) => MissionState.OpenNew("MultiplayerFreeForAll", new MissionInitializerRecord(scene), (InitializeMissionBehvaioursDelegate) (missionController => GameNetwork.IsServer ? (IEnumerable<MissionBehaviour>) new MissionBehaviour[18]
    {
      (MissionBehaviour) MissionLobbyComponent.CreateBehaviour(),
      (MissionBehaviour) new MissionMultiplayerFFA(),
      (MissionBehaviour) new MissionMultiplayerFFAClient(),
      (MissionBehaviour) new MultiplayerTimerComponent(),
      (MissionBehaviour) new MultiplayerMissionAgentVisualSpawnComponent(),
      (MissionBehaviour) new SpawnComponent((SpawnFrameBehaviourBase) new FFASpawnFrameBehaviour(), (SpawningBehaviourBase) new WarmupSpawningBehaviour()),
      (MissionBehaviour) new MissionLobbyEquipmentNetworkComponent(),
      (MissionBehaviour) new MissionBoundaryPlacer(),
      (MissionBehaviour) new MultiplayerTeamSelectComponent(),
      (MissionBehaviour) new MissionHardBorderPlacer(),
      (MissionBehaviour) new MissionBoundaryCrossingHandler(),
      (MissionBehaviour) new MultiplayerPollComponent(),
      (MissionBehaviour) new MultiplayerAdminComponent(),
      (MissionBehaviour) new MultiplayerGameNotificationsComponent(),
      (MissionBehaviour) new MissionOptionsComponent(),
      (MissionBehaviour) new MissionScoreboardComponent("FreeForAll"),
      (MissionBehaviour) new MissionAgentPanicHandler(),
      (MissionBehaviour) new AgentBattleAILogic()
    } : (IEnumerable<MissionBehaviour>) new MissionBehaviour[15]
    {
      (MissionBehaviour) MissionLobbyComponent.CreateBehaviour(),
      (MissionBehaviour) new MissionMultiplayerFFAClient(),
      (MissionBehaviour) new MultiplayerTimerComponent(),
      (MissionBehaviour) new MultiplayerMissionAgentVisualSpawnComponent(),
      (MissionBehaviour) new MissionLobbyEquipmentNetworkComponent(),
      (MissionBehaviour) new MissionHardBorderPlacer(),
      (MissionBehaviour) new MultiplayerTeamSelectComponent(),
      (MissionBehaviour) new MissionBoundaryPlacer(),
      (MissionBehaviour) new MissionBoundaryCrossingHandler(),
      (MissionBehaviour) new MultiplayerPollComponent(),
      (MissionBehaviour) new MultiplayerGameNotificationsComponent(),
      (MissionBehaviour) new MissionOptionsComponent(),
      (MissionBehaviour) new MissionScoreboardComponent("FreeForAll"),
      (MissionBehaviour) new MissionMatchHistoryComponent(),
      (MissionBehaviour) new MissionRecentPlayersComponent()
    }));

    [MissionMethod]
    public static void OpenTeamDeathmatchMission(string scene) => MissionState.OpenNew("MultiplayerTeamDeathmatch", new MissionInitializerRecord(scene), (InitializeMissionBehvaioursDelegate) (missionController => GameNetwork.IsServer ? (IEnumerable<MissionBehaviour>) new MissionBehaviour[19]
    {
      (MissionBehaviour) MissionLobbyComponent.CreateBehaviour(),
      (MissionBehaviour) new MissionMultiplayerTeamDeathmatch(),
      (MissionBehaviour) new MissionMultiplayerTeamDeathmatchClient(),
      (MissionBehaviour) new MultiplayerTimerComponent(),
      (MissionBehaviour) new MultiplayerMissionAgentVisualSpawnComponent(),
      (MissionBehaviour) new SpawnComponent((SpawnFrameBehaviourBase) new TeamDeathmatchSpawnFrameBehavior(), (SpawningBehaviourBase) new TeamDeathmatchSpawningBehaviour()),
      (MissionBehaviour) new MissionLobbyEquipmentNetworkComponent(),
      (MissionBehaviour) new MultiplayerTeamSelectComponent(),
      (MissionBehaviour) new MissionHardBorderPlacer(),
      (MissionBehaviour) new MissionBoundaryPlacer(),
      (MissionBehaviour) new MissionBoundaryCrossingHandler(),
      (MissionBehaviour) new MultiplayerPollComponent(),
      (MissionBehaviour) new MultiplayerAdminComponent(),
      (MissionBehaviour) new MultiplayerGameNotificationsComponent(),
      (MissionBehaviour) new MissionOptionsComponent(),
      (MissionBehaviour) new MissionScoreboardComponent("TeamDeathmatch"),
      (MissionBehaviour) new MissionAgentPanicHandler(),
      (MissionBehaviour) new AgentBattleAILogic(),
      (MissionBehaviour) new AgentFadeOutLogic()
    } : (IEnumerable<MissionBehaviour>) new MissionBehaviour[15]
    {
      (MissionBehaviour) MissionLobbyComponent.CreateBehaviour(),
      (MissionBehaviour) new MissionMultiplayerTeamDeathmatchClient(),
      (MissionBehaviour) new MultiplayerTimerComponent(),
      (MissionBehaviour) new MultiplayerMissionAgentVisualSpawnComponent(),
      (MissionBehaviour) new MissionLobbyEquipmentNetworkComponent(),
      (MissionBehaviour) new MultiplayerTeamSelectComponent(),
      (MissionBehaviour) new MissionHardBorderPlacer(),
      (MissionBehaviour) new MissionBoundaryPlacer(),
      (MissionBehaviour) new MissionBoundaryCrossingHandler(),
      (MissionBehaviour) new MultiplayerPollComponent(),
      (MissionBehaviour) new MultiplayerGameNotificationsComponent(),
      (MissionBehaviour) new MissionOptionsComponent(),
      (MissionBehaviour) new MissionScoreboardComponent("TeamDeathmatch"),
      (MissionBehaviour) new MissionMatchHistoryComponent(),
      (MissionBehaviour) new MissionRecentPlayersComponent()
    }));

    [MissionMethod]
    public static void OpenDuelMission(string scene) => MissionState.OpenNew("MultiplayerDuel", new MissionInitializerRecord(scene), (InitializeMissionBehvaioursDelegate) (missionController => GameNetwork.IsServer ? (IEnumerable<MissionBehaviour>) new MissionBehaviour[17]
    {
      (MissionBehaviour) MissionLobbyComponent.CreateBehaviour(),
      (MissionBehaviour) new MissionMultiplayerDuel(),
      (MissionBehaviour) new MissionMultiplayerGameModeDuelClient(),
      (MissionBehaviour) new MultiplayerTimerComponent(),
      (MissionBehaviour) new MultiplayerMissionAgentVisualSpawnComponent(),
      (MissionBehaviour) new SpawnComponent((SpawnFrameBehaviourBase) new FFASpawnFrameBehaviour(), (SpawningBehaviourBase) new WarmupSpawningBehaviour()),
      (MissionBehaviour) new MissionLobbyEquipmentNetworkComponent(),
      (MissionBehaviour) new MissionHardBorderPlacer(),
      (MissionBehaviour) new MissionBoundaryPlacer(),
      (MissionBehaviour) new MissionBoundaryCrossingHandler(),
      (MissionBehaviour) new MultiplayerPollComponent(),
      (MissionBehaviour) new MultiplayerAdminComponent(),
      (MissionBehaviour) new MultiplayerGameNotificationsComponent(),
      (MissionBehaviour) new MissionOptionsComponent(),
      (MissionBehaviour) new MissionScoreboardComponent("Duel"),
      (MissionBehaviour) new MissionAgentPanicHandler(),
      (MissionBehaviour) new AgentBattleAILogic()
    } : (IEnumerable<MissionBehaviour>) new MissionBehaviour[14]
    {
      (MissionBehaviour) MissionLobbyComponent.CreateBehaviour(),
      (MissionBehaviour) new MissionMultiplayerGameModeDuelClient(),
      (MissionBehaviour) new MultiplayerTimerComponent(),
      (MissionBehaviour) new MultiplayerMissionAgentVisualSpawnComponent(),
      (MissionBehaviour) new MissionLobbyEquipmentNetworkComponent(),
      (MissionBehaviour) new MissionHardBorderPlacer(),
      (MissionBehaviour) new MissionBoundaryPlacer(),
      (MissionBehaviour) new MissionBoundaryCrossingHandler(),
      (MissionBehaviour) new MultiplayerPollComponent(),
      (MissionBehaviour) new MultiplayerGameNotificationsComponent(),
      (MissionBehaviour) new MissionOptionsComponent(),
      (MissionBehaviour) new MissionScoreboardComponent("Duel"),
      (MissionBehaviour) new MissionMatchHistoryComponent(),
      (MissionBehaviour) new MissionRecentPlayersComponent()
    }));

    [MissionMethod]
    public static void OpenSiegeMission(string scene) => MissionState.OpenNew("MultiplayerSiege", new MissionInitializerRecord(scene)
    {
      SceneUpgradeLevel = 3,
      SceneLevels = ""
    }, (InitializeMissionBehvaioursDelegate) (missionController => GameNetwork.IsServer ? (IEnumerable<MissionBehaviour>) new MissionBehaviour[19]
    {
      (MissionBehaviour) MissionLobbyComponent.CreateBehaviour(),
      (MissionBehaviour) new MissionMultiplayerSiege(),
      (MissionBehaviour) new MultiplayerWarmupComponent(),
      (MissionBehaviour) new MissionMultiplayerSiegeClient(),
      (MissionBehaviour) new MultiplayerTimerComponent(),
      (MissionBehaviour) new MultiplayerMissionAgentVisualSpawnComponent(),
      (MissionBehaviour) new SpawnComponent((SpawnFrameBehaviourBase) new SiegeSpawnFrameBehaviour(), (SpawningBehaviourBase) new SiegeSpawningBehaviour()),
      (MissionBehaviour) new MissionLobbyEquipmentNetworkComponent(),
      (MissionBehaviour) new MultiplayerTeamSelectComponent(),
      (MissionBehaviour) new MissionHardBorderPlacer(),
      (MissionBehaviour) new MissionBoundaryPlacer(),
      (MissionBehaviour) new MissionBoundaryCrossingHandler(),
      (MissionBehaviour) new MultiplayerPollComponent(),
      (MissionBehaviour) new MultiplayerAdminComponent(),
      (MissionBehaviour) new MultiplayerGameNotificationsComponent(),
      (MissionBehaviour) new MissionOptionsComponent(),
      (MissionBehaviour) new MissionScoreboardComponent("Siege"),
      (MissionBehaviour) new MissionAgentPanicHandler(),
      (MissionBehaviour) new AgentBattleAILogic()
    } : (IEnumerable<MissionBehaviour>) new MissionBehaviour[16]
    {
      (MissionBehaviour) MissionLobbyComponent.CreateBehaviour(),
      (MissionBehaviour) new MultiplayerWarmupComponent(),
      (MissionBehaviour) new MissionMultiplayerSiegeClient(),
      (MissionBehaviour) new MultiplayerTimerComponent(),
      (MissionBehaviour) new MultiplayerMissionAgentVisualSpawnComponent(),
      (MissionBehaviour) new MissionLobbyEquipmentNetworkComponent(),
      (MissionBehaviour) new MultiplayerTeamSelectComponent(),
      (MissionBehaviour) new MissionHardBorderPlacer(),
      (MissionBehaviour) new MissionBoundaryPlacer(),
      (MissionBehaviour) new MissionBoundaryCrossingHandler(),
      (MissionBehaviour) new MultiplayerPollComponent(),
      (MissionBehaviour) new MultiplayerGameNotificationsComponent(),
      (MissionBehaviour) new MissionOptionsComponent(),
      (MissionBehaviour) new MissionScoreboardComponent("Siege"),
      (MissionBehaviour) new MissionMatchHistoryComponent(),
      (MissionBehaviour) new MissionRecentPlayersComponent()
    }));

    [MissionMethod]
    public static void OpenCaptainMission(string scene) => MissionState.OpenNew("MultiplayerCaptain", new MissionInitializerRecord(scene), (InitializeMissionBehvaioursDelegate) (missionController => GameNetwork.IsServer ? (IEnumerable<MissionBehaviour>) new MissionBehaviour[21]
    {
      (MissionBehaviour) MissionLobbyComponent.CreateBehaviour(),
      (MissionBehaviour) new MultiplayerRoundController(),
      (MissionBehaviour) new MissionMultiplayerFlagDomination(false),
      (MissionBehaviour) new MultiplayerWarmupComponent(),
      (MissionBehaviour) new MissionMultiplayerGameModeFlagDominationClient(),
      (MissionBehaviour) new MultiplayerTimerComponent(),
      (MissionBehaviour) new MultiplayerMissionAgentVisualSpawnComponent(),
      (MissionBehaviour) new SpawnComponent((SpawnFrameBehaviourBase) new FlagDominationSpawnFrameBehaviour(), (SpawningBehaviourBase) new FlagDominationSpawningBehaviour()),
      (MissionBehaviour) new MissionLobbyEquipmentNetworkComponent(),
      (MissionBehaviour) new MultiplayerTeamSelectComponent(),
      (MissionBehaviour) new AgentVictoryLogic(),
      (MissionBehaviour) new AgentBattleAILogic(),
      (MissionBehaviour) new MissionAgentPanicHandler(),
      (MissionBehaviour) new MissionBoundaryPlacer(),
      (MissionBehaviour) new MissionBoundaryCrossingHandler(),
      (MissionBehaviour) new MultiplayerPollComponent(),
      (MissionBehaviour) new MultiplayerAdminComponent(),
      (MissionBehaviour) new MultiplayerGameNotificationsComponent(),
      (MissionBehaviour) new MissionOptionsComponent(),
      (MissionBehaviour) new MissionScoreboardComponent("Captain"),
      (MissionBehaviour) new AgentFadeOutLogic()
    } : (IEnumerable<MissionBehaviour>) new MissionBehaviour[17]
    {
      (MissionBehaviour) MissionLobbyComponent.CreateBehaviour(),
      (MissionBehaviour) new MultiplayerRoundComponent(),
      (MissionBehaviour) new MultiplayerWarmupComponent(),
      (MissionBehaviour) new MissionMultiplayerGameModeFlagDominationClient(),
      (MissionBehaviour) new MultiplayerTimerComponent(),
      (MissionBehaviour) new MultiplayerMissionAgentVisualSpawnComponent(),
      (MissionBehaviour) new MissionLobbyEquipmentNetworkComponent(),
      (MissionBehaviour) new MultiplayerTeamSelectComponent(),
      (MissionBehaviour) new AgentVictoryLogic(),
      (MissionBehaviour) new MissionBoundaryPlacer(),
      (MissionBehaviour) new MissionBoundaryCrossingHandler(),
      (MissionBehaviour) new MultiplayerPollComponent(),
      (MissionBehaviour) new MultiplayerGameNotificationsComponent(),
      (MissionBehaviour) new MissionOptionsComponent(),
      (MissionBehaviour) new MissionScoreboardComponent("Captain"),
      (MissionBehaviour) new MissionMatchHistoryComponent(),
      (MissionBehaviour) new MissionRecentPlayersComponent()
    }));

    [MissionMethod]
    public static void OpenSkirmishMission(string scene) => MissionState.OpenNew("MultiplayerSkirmish", new MissionInitializerRecord(scene), (InitializeMissionBehvaioursDelegate) (missionController => GameNetwork.IsServer ? (IEnumerable<MissionBehaviour>) new MissionBehaviour[20]
    {
      (MissionBehaviour) MissionLobbyComponent.CreateBehaviour(),
      (MissionBehaviour) new MultiplayerRoundController(),
      (MissionBehaviour) new MissionMultiplayerFlagDomination(true),
      (MissionBehaviour) new MultiplayerWarmupComponent(),
      (MissionBehaviour) new MissionMultiplayerGameModeFlagDominationClient(),
      (MissionBehaviour) new MultiplayerTimerComponent(),
      (MissionBehaviour) new MultiplayerMissionAgentVisualSpawnComponent(),
      (MissionBehaviour) new SpawnComponent((SpawnFrameBehaviourBase) new FlagDominationSpawnFrameBehaviour(), (SpawningBehaviourBase) new FlagDominationSpawningBehaviour()),
      (MissionBehaviour) new MissionLobbyEquipmentNetworkComponent(),
      (MissionBehaviour) new MultiplayerTeamSelectComponent(),
      (MissionBehaviour) new AgentVictoryLogic(),
      (MissionBehaviour) new MissionAgentPanicHandler(),
      (MissionBehaviour) new AgentBattleAILogic(),
      (MissionBehaviour) new MissionBoundaryPlacer(),
      (MissionBehaviour) new MissionBoundaryCrossingHandler(),
      (MissionBehaviour) new MultiplayerPollComponent(),
      (MissionBehaviour) new MultiplayerAdminComponent(),
      (MissionBehaviour) new MultiplayerGameNotificationsComponent(),
      (MissionBehaviour) new MissionOptionsComponent(),
      (MissionBehaviour) new MissionScoreboardComponent("Skirmish")
    } : (IEnumerable<MissionBehaviour>) new MissionBehaviour[17]
    {
      (MissionBehaviour) MissionLobbyComponent.CreateBehaviour(),
      (MissionBehaviour) new MultiplayerRoundComponent(),
      (MissionBehaviour) new MultiplayerWarmupComponent(),
      (MissionBehaviour) new MissionMultiplayerGameModeFlagDominationClient(),
      (MissionBehaviour) new MultiplayerTimerComponent(),
      (MissionBehaviour) new MultiplayerMissionAgentVisualSpawnComponent(),
      (MissionBehaviour) new MissionLobbyEquipmentNetworkComponent(),
      (MissionBehaviour) new MultiplayerTeamSelectComponent(),
      (MissionBehaviour) new AgentVictoryLogic(),
      (MissionBehaviour) new MissionBoundaryPlacer(),
      (MissionBehaviour) new MissionBoundaryCrossingHandler(),
      (MissionBehaviour) new MultiplayerPollComponent(),
      (MissionBehaviour) new MultiplayerGameNotificationsComponent(),
      (MissionBehaviour) new MissionOptionsComponent(),
      (MissionBehaviour) new MissionScoreboardComponent("Skirmish"),
      (MissionBehaviour) new MissionMatchHistoryComponent(),
      (MissionBehaviour) new MissionRecentPlayersComponent()
    }));
  }
}
