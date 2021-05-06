// Decompiled with JetBrains decompiler
// Type: TaleWorlds.MountAndBlade.MissionNetworkComponent
// Assembly: TaleWorlds.MountAndBlade, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3C0E3F42-756F-4407-B817-06BA412A78B0
// Assembly location: E:\SteamLibrary\steamapps\common\Mount & Blade II Bannerlord\bin\Win64_Shipping_Client\TaleWorlds.MountAndBlade.dll

using NetworkMessages.FromClient;
using NetworkMessages.FromServer;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using TaleWorlds.Core;
using TaleWorlds.DotNet;
using TaleWorlds.Engine;
using TaleWorlds.Library;
using TaleWorlds.MountAndBlade.Network.Messages;
using TaleWorlds.ObjectSystem;

namespace TaleWorlds.MountAndBlade
{
  public sealed class MissionNetworkComponent : MissionNetwork
  {
    private float _accumulatedTimeSinceLastTimerSync;
    private const float TimerSyncPeriod = 2f;

    protected override void AddRemoveMessageHandlers(
      GameNetwork.NetworkMessageHandlerRegistererContainer registerer)
    {
      if (GameNetwork.IsClientOrReplay)
      {
        registerer.Register<CreateFreeMountAgent>(new GameNetworkMessage.ServerMessageHandlerDelegate<CreateFreeMountAgent>(this.HandleServerEventCreateFreeMountAgentEvent));
        registerer.Register<CreateAgent>(new GameNetworkMessage.ServerMessageHandlerDelegate<CreateAgent>(this.HandleServerEventCreateAgent));
        registerer.Register<CreateAgentVisuals>(new GameNetworkMessage.ServerMessageHandlerDelegate<CreateAgentVisuals>(this.HandleServerEventCreateAgentVisuals));
        registerer.Register<RemoveAgentVisualsForPeer>(new GameNetworkMessage.ServerMessageHandlerDelegate<RemoveAgentVisualsForPeer>(this.HandleServerEventRemoveAgentVisualsForPeer));
        registerer.Register<RemoveAgentVisualsFromIndexForPeer>(new GameNetworkMessage.ServerMessageHandlerDelegate<RemoveAgentVisualsFromIndexForPeer>(this.HandleServerEventRemoveAgentVisualsFromIndexForPeer));
        registerer.Register<ReplaceBotWithPlayer>(new GameNetworkMessage.ServerMessageHandlerDelegate<ReplaceBotWithPlayer>(this.HandleServerEventReplaceBotWithPlayer));
        registerer.Register<SetWieldedItemIndex>(new GameNetworkMessage.ServerMessageHandlerDelegate<SetWieldedItemIndex>(this.HandleServerEventSetWieldedItemIndex));
        registerer.Register<SetWeaponNetworkData>(new GameNetworkMessage.ServerMessageHandlerDelegate<SetWeaponNetworkData>(this.HandleServerEventSetWeaponNetworkData));
        registerer.Register<SetWeaponAmmoData>(new GameNetworkMessage.ServerMessageHandlerDelegate<SetWeaponAmmoData>(this.HandleServerEventSetWeaponAmmoData));
        registerer.Register<SetWeaponReloadPhase>(new GameNetworkMessage.ServerMessageHandlerDelegate<SetWeaponReloadPhase>(this.HandleServerEventSetWeaponReloadPhase));
        registerer.Register<WeaponUsageIndexChangeMessage>(new GameNetworkMessage.ServerMessageHandlerDelegate<WeaponUsageIndexChangeMessage>(this.HandleServerEventWeaponUsageIndexChangeMessage));
        registerer.Register<StartSwitchingWeaponUsageIndex>(new GameNetworkMessage.ServerMessageHandlerDelegate<StartSwitchingWeaponUsageIndex>(this.HandleServerEventStartSwitchingWeaponUsageIndex));
        registerer.Register<InitializeFormation>(new GameNetworkMessage.ServerMessageHandlerDelegate<InitializeFormation>(this.HandleServerEventInitializeFormation));
        registerer.Register<SetSpawnedFormationCount>(new GameNetworkMessage.ServerMessageHandlerDelegate<SetSpawnedFormationCount>(this.HandleServerEventSetSpawnedFormationCount));
        registerer.Register<AddTeam>(new GameNetworkMessage.ServerMessageHandlerDelegate<AddTeam>(this.HandleServerEventAddTeam));
        registerer.Register<TeamSetIsEnemyOf>(new GameNetworkMessage.ServerMessageHandlerDelegate<TeamSetIsEnemyOf>(this.HandleServerEventTeamSetIsEnemyOf));
        registerer.Register<AssignFormationToPlayer>(new GameNetworkMessage.ServerMessageHandlerDelegate<AssignFormationToPlayer>(this.HandleServerEventAssignFormationToPlayer));
        registerer.Register<ExistingObjectsBegin>(new GameNetworkMessage.ServerMessageHandlerDelegate<ExistingObjectsBegin>(this.HandleServerEventExistingObjectsBegin));
        registerer.Register<ExistingObjectsEnd>(new GameNetworkMessage.ServerMessageHandlerDelegate<ExistingObjectsEnd>(this.HandleServerEventExistingObjectsEnd));
        registerer.Register<ClearMission>(new GameNetworkMessage.ServerMessageHandlerDelegate<ClearMission>(this.HandleServerEventClearMission));
        registerer.Register<CreateMissionObject>(new GameNetworkMessage.ServerMessageHandlerDelegate<CreateMissionObject>(this.HandleServerEventCreateMissionObject));
        registerer.Register<RemoveMissionObject>(new GameNetworkMessage.ServerMessageHandlerDelegate<RemoveMissionObject>(this.HandleServerEventRemoveMissionObject));
        registerer.Register<StopPhysicsAndSetFrameOfMissionObject>(new GameNetworkMessage.ServerMessageHandlerDelegate<StopPhysicsAndSetFrameOfMissionObject>(this.HandleServerEventStopPhysicsAndSetFrameOfMissionObject));
        registerer.Register<BurstMissionObjectParticles>(new GameNetworkMessage.ServerMessageHandlerDelegate<BurstMissionObjectParticles>(this.HandleServerEventBurstMissionObjectParticles));
        registerer.Register<SetMissionObjectVisibility>(new GameNetworkMessage.ServerMessageHandlerDelegate<SetMissionObjectVisibility>(this.HandleServerEventSetMissionObjectVisibility));
        registerer.Register<SetMissionObjectDisabled>(new GameNetworkMessage.ServerMessageHandlerDelegate<SetMissionObjectDisabled>(this.HandleServerEventSetMissionObjectDisabled));
        registerer.Register<SetMissionObjectColors>(new GameNetworkMessage.ServerMessageHandlerDelegate<SetMissionObjectColors>(this.HandleServerEventSetMissionObjectColors));
        registerer.Register<SetMissionObjectFrame>(new GameNetworkMessage.ServerMessageHandlerDelegate<SetMissionObjectFrame>(this.HandleServerEventSetMissionObjectFrame));
        registerer.Register<SetMissionObjectGlobalFrame>(new GameNetworkMessage.ServerMessageHandlerDelegate<SetMissionObjectGlobalFrame>(this.HandleServerEventSetMissionObjectGlobalFrame));
        registerer.Register<SetMissionObjectFrameOverTime>(new GameNetworkMessage.ServerMessageHandlerDelegate<SetMissionObjectFrameOverTime>(this.HandleServerEventSetMissionObjectFrameOverTime));
        registerer.Register<SetMissionObjectGlobalFrameOverTime>(new GameNetworkMessage.ServerMessageHandlerDelegate<SetMissionObjectGlobalFrameOverTime>(this.HandleServerEventSetMissionObjectGlobalFrameOverTime));
        registerer.Register<SetMissionObjectAnimationAtChannel>(new GameNetworkMessage.ServerMessageHandlerDelegate<SetMissionObjectAnimationAtChannel>(this.HandleServerEventSetMissionObjectAnimationAtChannel));
        registerer.Register<SetMissionObjectAnimationChannelParameter>(new GameNetworkMessage.ServerMessageHandlerDelegate<SetMissionObjectAnimationChannelParameter>(this.HandleServerEventSetMissionObjectAnimationChannelParameter));
        registerer.Register<SetMissionObjectAnimationPaused>(new GameNetworkMessage.ServerMessageHandlerDelegate<SetMissionObjectAnimationPaused>(this.HandleServerEventSetMissionObjectAnimationPaused));
        registerer.Register<SetMissionObjectVertexAnimation>(new GameNetworkMessage.ServerMessageHandlerDelegate<SetMissionObjectVertexAnimation>(this.HandleServerEventSetMissionObjectVertexAnimation));
        registerer.Register<SetMissionObjectVertexAnimationProgress>(new GameNetworkMessage.ServerMessageHandlerDelegate<SetMissionObjectVertexAnimationProgress>(this.HandleServerEventSetMissionObjectVertexAnimationProgress));
        registerer.Register<SetMissionObjectImpulse>(new GameNetworkMessage.ServerMessageHandlerDelegate<SetMissionObjectImpulse>(this.HandleServerEventSetMissionObjectImpulse));
        registerer.Register<AddMissionObjectBodyFlags>(new GameNetworkMessage.ServerMessageHandlerDelegate<AddMissionObjectBodyFlags>(this.HandleServerEventAddMissionObjectBodyFlags));
        registerer.Register<RemoveMissionObjectBodyFlags>(new GameNetworkMessage.ServerMessageHandlerDelegate<RemoveMissionObjectBodyFlags>(this.HandleServerEventRemoveMissionObjectBodyFlags));
        registerer.Register<SetMachineTargetRotation>(new GameNetworkMessage.ServerMessageHandlerDelegate<SetMachineTargetRotation>(this.HandleServerEventSetMachineTargetRotation));
        registerer.Register<SetUsableMissionObjectIsDeactivated>(new GameNetworkMessage.ServerMessageHandlerDelegate<SetUsableMissionObjectIsDeactivated>(this.HandleServerEventSetUsableGameObjectIsDeactivated));
        registerer.Register<SetUsableMissionObjectIsDisabledForPlayers>(new GameNetworkMessage.ServerMessageHandlerDelegate<SetUsableMissionObjectIsDisabledForPlayers>(this.HandleServerEventSetUsableGameObjectIsDisabledForPlayers));
        registerer.Register<SetRangedSiegeWeaponState>(new GameNetworkMessage.ServerMessageHandlerDelegate<SetRangedSiegeWeaponState>(this.HandleServerEventSetRangedSiegeWeaponState));
        registerer.Register<SetRangedSiegeWeaponAmmo>(new GameNetworkMessage.ServerMessageHandlerDelegate<SetRangedSiegeWeaponAmmo>(this.HandleServerEventSetRangedSiegeWeaponAmmo));
        registerer.Register<RangedSiegeWeaponChangeProjectile>(new GameNetworkMessage.ServerMessageHandlerDelegate<RangedSiegeWeaponChangeProjectile>(this.HandleServerEventRangedSiegeWeaponChangeProjectile));
        registerer.Register<SetStonePileAmmo>(new GameNetworkMessage.ServerMessageHandlerDelegate<SetStonePileAmmo>(this.HandleServerEventSetStonePileAmmo));
        registerer.Register<SetSiegeMachineMovementDistance>(new GameNetworkMessage.ServerMessageHandlerDelegate<SetSiegeMachineMovementDistance>(this.HandleServerEventSetSiegeMachineMovementDistance));
        registerer.Register<SetSiegeLadderState>(new GameNetworkMessage.ServerMessageHandlerDelegate<SetSiegeLadderState>(this.HandleServerEventSetSiegeLadderState));
        registerer.Register<SetAgentTargetPositionAndDirection>(new GameNetworkMessage.ServerMessageHandlerDelegate<SetAgentTargetPositionAndDirection>(this.HandleServerEventSetAgentTargetPositionAndDirection));
        registerer.Register<SetAgentTargetPosition>(new GameNetworkMessage.ServerMessageHandlerDelegate<SetAgentTargetPosition>(this.HandleServerEventSetAgentTargetPosition));
        registerer.Register<ClearAgentTargetFrame>(new GameNetworkMessage.ServerMessageHandlerDelegate<ClearAgentTargetFrame>(this.HandleServerEventClearAgentTargetFrame));
        registerer.Register<AgentTeleportToFrame>(new GameNetworkMessage.ServerMessageHandlerDelegate<AgentTeleportToFrame>(this.HandleServerEventAgentTeleportToFrame));
        registerer.Register<SetSiegeTowerGateState>(new GameNetworkMessage.ServerMessageHandlerDelegate<SetSiegeTowerGateState>(this.HandleServerEventSetSiegeTowerGateState));
        registerer.Register<SetSiegeTowerHasArrivedAtTarget>(new GameNetworkMessage.ServerMessageHandlerDelegate<SetSiegeTowerHasArrivedAtTarget>(this.HandleServerEventSetSiegeTowerHasArrivedAtTarget));
        registerer.Register<SetBatteringRamHasArrivedAtTarget>(new GameNetworkMessage.ServerMessageHandlerDelegate<SetBatteringRamHasArrivedAtTarget>(this.HandleServerEventSetBatteringRamHasArrivedAtTarget));
        registerer.Register<SetPeerTeam>(new GameNetworkMessage.ServerMessageHandlerDelegate<SetPeerTeam>(this.HandleServerEventSetPeerTeam));
        registerer.Register<SynchronizeMissionTimeTracker>(new GameNetworkMessage.ServerMessageHandlerDelegate<SynchronizeMissionTimeTracker>(this.HandleServerEventSyncMissionTimer));
        registerer.Register<SetAgentPeer>(new GameNetworkMessage.ServerMessageHandlerDelegate<SetAgentPeer>(this.HandleServerEventSetAgentPeer));
        registerer.Register<SetAgentIsPlayer>(new GameNetworkMessage.ServerMessageHandlerDelegate<SetAgentIsPlayer>(this.HandleServerEventSetAgentIsPlayer));
        registerer.Register<SetAgentHealth>(new GameNetworkMessage.ServerMessageHandlerDelegate<SetAgentHealth>(this.HandleServerEventSetAgentHealth));
        registerer.Register<AgentSetTeam>(new GameNetworkMessage.ServerMessageHandlerDelegate<AgentSetTeam>(this.HandleServerEventAgentSetTeam));
        registerer.Register<SetAgentActionSet>(new GameNetworkMessage.ServerMessageHandlerDelegate<SetAgentActionSet>(this.HandleServerEventSetAgentActionSet));
        registerer.Register<MakeAgentDead>(new GameNetworkMessage.ServerMessageHandlerDelegate<MakeAgentDead>(this.HandleServerEventMakeAgentDead));
        registerer.Register<AgentSetFormation>(new GameNetworkMessage.ServerMessageHandlerDelegate<AgentSetFormation>(this.HandleServerEventAgentSetFormation));
        registerer.Register<AddPrefabComponentToAgentBone>(new GameNetworkMessage.ServerMessageHandlerDelegate<AddPrefabComponentToAgentBone>(this.HandleServerEventAddPrefabComponentToAgentBone));
        registerer.Register<SetAgentPrefabComponentVisibility>(new GameNetworkMessage.ServerMessageHandlerDelegate<SetAgentPrefabComponentVisibility>(this.HandleServerEventSetAgentPrefabComponentVisibility));
        registerer.Register<UseObject>(new GameNetworkMessage.ServerMessageHandlerDelegate<UseObject>(this.HandleServerEventUseObject));
        registerer.Register<StopUsingObject>(new GameNetworkMessage.ServerMessageHandlerDelegate<StopUsingObject>(this.HandleServerEventStopUsingObject));
        registerer.Register<SyncObjectHitpoints>(new GameNetworkMessage.ServerMessageHandlerDelegate<SyncObjectHitpoints>(this.HandleServerEventHitSynchronizeObjectHitpoints));
        registerer.Register<SyncObjectDestructionLevel>(new GameNetworkMessage.ServerMessageHandlerDelegate<SyncObjectDestructionLevel>(this.HandleServerEventHitSynchronizeObjectDestructionLevel));
        registerer.Register<BurstAllHeavyHitParticles>(new GameNetworkMessage.ServerMessageHandlerDelegate<BurstAllHeavyHitParticles>(this.HandleServerEventHitBurstAllHeavyHitParticles));
        registerer.Register<SynchronizeMissionObject>(new GameNetworkMessage.ServerMessageHandlerDelegate<SynchronizeMissionObject>(this.HandleServerEventSynchronizeMissionObject));
        registerer.Register<SpawnWeaponWithNewEntity>(new GameNetworkMessage.ServerMessageHandlerDelegate<SpawnWeaponWithNewEntity>(this.HandleServerEventSpawnWeaponWithNewEntity));
        registerer.Register<AttachWeaponToSpawnedWeapon>(new GameNetworkMessage.ServerMessageHandlerDelegate<AttachWeaponToSpawnedWeapon>(this.HandleServerEventAttachWeaponToSpawnedWeapon));
        registerer.Register<AttachWeaponToAgent>(new GameNetworkMessage.ServerMessageHandlerDelegate<AttachWeaponToAgent>(this.HandleServerEventAttachWeaponToAgent));
        registerer.Register<SpawnWeaponAsDropFromAgent>(new GameNetworkMessage.ServerMessageHandlerDelegate<SpawnWeaponAsDropFromAgent>(this.HandleServerEventSpawnWeaponAsDropFromAgent));
        registerer.Register<SpawnAttachedWeaponOnSpawnedWeapon>(new GameNetworkMessage.ServerMessageHandlerDelegate<SpawnAttachedWeaponOnSpawnedWeapon>(this.HandleServerEventSpawnAttachedWeaponOnSpawnedWeapon));
        registerer.Register<SpawnAttachedWeaponOnCorpse>(new GameNetworkMessage.ServerMessageHandlerDelegate<SpawnAttachedWeaponOnCorpse>(this.HandleServerEventSpawnAttachedWeaponOnCorpse));
        registerer.Register<HandleMissileCollisionReaction>(new GameNetworkMessage.ServerMessageHandlerDelegate<HandleMissileCollisionReaction>(this.HandleServerEventHandleMissileCollisionReaction));
        registerer.Register<RemoveEquippedWeapon>(new GameNetworkMessage.ServerMessageHandlerDelegate<RemoveEquippedWeapon>(this.HandleServerEventRemoveEquippedWeapon));
        registerer.Register<EquipWeaponWithNewEntity>(new GameNetworkMessage.ServerMessageHandlerDelegate<EquipWeaponWithNewEntity>(this.HandleServerEventEquipWeaponWithNewEntity));
        registerer.Register<AttachWeaponToWeaponInAgentEquipmentSlot>(new GameNetworkMessage.ServerMessageHandlerDelegate<AttachWeaponToWeaponInAgentEquipmentSlot>(this.HandleServerEventAttachWeaponToWeaponInAgentEquipmentSlot));
        registerer.Register<EquipWeaponFromSpawnedItemEntity>(new GameNetworkMessage.ServerMessageHandlerDelegate<EquipWeaponFromSpawnedItemEntity>(this.HandleServerEventEquipWeaponFromSpawnedItemEntity));
        registerer.Register<CreateMissile>(new GameNetworkMessage.ServerMessageHandlerDelegate<CreateMissile>(this.HandleServerEventCreateMissile));
        registerer.Register<CombatLogNetworkMessage>(new GameNetworkMessage.ServerMessageHandlerDelegate<CombatLogNetworkMessage>(this.HandleServerEventAgentHit));
        registerer.Register<ConsumeWeaponAmount>(new GameNetworkMessage.ServerMessageHandlerDelegate<ConsumeWeaponAmount>(this.HandleServerEventConsumeWeaponAmount));
      }
      else
      {
        if (!GameNetwork.IsServer)
          return;
        registerer.Register<SetFollowedAgent>(new GameNetworkMessage.ClientMessageHandlerDelegate<SetFollowedAgent>(this.HandleClientEventSetFollowedAgent));
        registerer.Register<SetMachineRotation>(new GameNetworkMessage.ClientMessageHandlerDelegate<SetMachineRotation>(this.HandleClientEventSetMachineRotation));
        registerer.Register<RequestUseObject>(new GameNetworkMessage.ClientMessageHandlerDelegate<RequestUseObject>(this.HandleClientEventRequestUseObject));
        registerer.Register<RequestStopUsingObject>(new GameNetworkMessage.ClientMessageHandlerDelegate<RequestStopUsingObject>(this.HandleClientEventRequestStopUsingObject));
        registerer.Register<ApplyOrder>(new GameNetworkMessage.ClientMessageHandlerDelegate<ApplyOrder>(this.HandleClientEventApplyOrder));
        registerer.Register<ApplySiegeWeaponOrder>(new GameNetworkMessage.ClientMessageHandlerDelegate<ApplySiegeWeaponOrder>(this.HandleClientEventApplySiegeWeaponOrder));
        registerer.Register<ApplyOrderWithPosition>(new GameNetworkMessage.ClientMessageHandlerDelegate<ApplyOrderWithPosition>(this.HandleClientEventApplyOrderWithPosition));
        registerer.Register<ApplyOrderWithFormation>(new GameNetworkMessage.ClientMessageHandlerDelegate<ApplyOrderWithFormation>(this.HandleClientEventApplyOrderWithFormation));
        registerer.Register<ApplyOrderWithFormationAndPercentage>(new GameNetworkMessage.ClientMessageHandlerDelegate<ApplyOrderWithFormationAndPercentage>(this.HandleClientEventApplyOrderWithFormationAndPercentage));
        registerer.Register<ApplyOrderWithFormationAndNumber>(new GameNetworkMessage.ClientMessageHandlerDelegate<ApplyOrderWithFormationAndNumber>(this.HandleClientEventApplyOrderWithFormationAndNumber));
        registerer.Register<ApplyOrderWithTwoPositions>(new GameNetworkMessage.ClientMessageHandlerDelegate<ApplyOrderWithTwoPositions>(this.HandleClientEventApplyOrderWithTwoPositions));
        registerer.Register<ApplyOrderWithMissionObject>(new GameNetworkMessage.ClientMessageHandlerDelegate<ApplyOrderWithMissionObject>(this.HandleClientEventApplyOrderWithGameEntity));
        registerer.Register<ApplyOrderWithAgent>(new GameNetworkMessage.ClientMessageHandlerDelegate<ApplyOrderWithAgent>(this.HandleClientEventApplyOrderWithAgent));
        registerer.Register<SelectAllFormations>(new GameNetworkMessage.ClientMessageHandlerDelegate<SelectAllFormations>(this.HandleClientEventSelectAllFormations));
        registerer.Register<SelectAllSiegeWeapons>(new GameNetworkMessage.ClientMessageHandlerDelegate<SelectAllSiegeWeapons>(this.HandleClientEventSelectAllSiegeWeapons));
        registerer.Register<ClearSelectedFormations>(new GameNetworkMessage.ClientMessageHandlerDelegate<ClearSelectedFormations>(this.HandleClientEventClearSelectedFormations));
        registerer.Register<SelectFormation>(new GameNetworkMessage.ClientMessageHandlerDelegate<SelectFormation>(this.HandleClientEventSelectFormation));
        registerer.Register<SelectSiegeWeapon>(new GameNetworkMessage.ClientMessageHandlerDelegate<SelectSiegeWeapon>(this.HandleClientEventSelectSiegeWeapon));
        registerer.Register<UnselectFormation>(new GameNetworkMessage.ClientMessageHandlerDelegate<UnselectFormation>(this.HandleClientEventUnselectFormation));
        registerer.Register<UnselectSiegeWeapon>(new GameNetworkMessage.ClientMessageHandlerDelegate<UnselectSiegeWeapon>(this.HandleClientEventUnselectSiegeWeapon));
        registerer.Register<DropWeapon>(new GameNetworkMessage.ClientMessageHandlerDelegate<DropWeapon>(this.HandleClientEventDropWeapon));
        registerer.Register<AgentVisualsBreakInvulnerability>(new GameNetworkMessage.ClientMessageHandlerDelegate<AgentVisualsBreakInvulnerability>(this.HandleClientEventBreakAgentVisualsInvulnerability));
        registerer.Register<RequestToSpawnAsBot>(new GameNetworkMessage.ClientMessageHandlerDelegate<RequestToSpawnAsBot>(this.HandleClientEventRequestToSpawnAsBot));
      }
    }

    public override void OnBehaviourInitialize()
    {
      base.OnBehaviourInitialize();
      VirtualPlayer.OnPeerComponentPreRemoved += new Action<VirtualPlayer, PeerComponent>(this.OnPeerComponentPreRemoved);
    }

    public override void OnRemoveBehaviour()
    {
      if (GameNetwork.IsServer)
      {
        foreach (MissionPeer peer in VirtualPlayer.Peers<MissionPeer>())
          peer.ControlledAgent = (Agent) null;
        foreach (Agent allAgent in (IEnumerable<Agent>) this.Mission.AllAgents)
          allAgent.MissionPeer = (MissionPeer) null;
        foreach (NetworkCommunicator networkPeer in GameNetwork.NetworkPeers)
          ;
      }
      if (GameNetwork.MyPeer != null)
        GameNetwork.MyPeer.RemoveComponent<MissionPeer>();
      VirtualPlayer.OnPeerComponentPreRemoved -= new Action<VirtualPlayer, PeerComponent>(this.OnPeerComponentPreRemoved);
      GameNetwork.ResetMissionData();
      base.OnRemoveBehaviour();
    }

    private Team GetTeamOfPeer(NetworkCommunicator networkPeer)
    {
      MissionPeer component = networkPeer.GetComponent<MissionPeer>();
      if (component.ControlledAgent == null)
      {
        MBDebug.Print("peer.ControlledAgent == null");
        return (Team) null;
      }
      Team team = component.ControlledAgent.Team;
      if (team != null)
        return team;
      MBDebug.Print("peersTeam == null");
      return team;
    }

    private OrderController GetOrderControllerOfPeer(NetworkCommunicator networkPeer)
    {
      Team teamOfPeer = this.GetTeamOfPeer(networkPeer);
      if (teamOfPeer != null)
        return teamOfPeer.GetOrderControllerOf(networkPeer.ControlledAgent);
      MBDebug.Print("peersTeam == null");
      return (OrderController) null;
    }

    private void HandleServerEventSyncMissionTimer(SynchronizeMissionTimeTracker message) => this.Mission.MissionTimeTracker.UpdateSync(message.CurrentTime);

    private void HandleServerEventSetPeerTeam(SetPeerTeam message)
    {
      message.Peer.GetComponent<MissionPeer>().Team = message.Team;
      if (!message.Peer.IsMine)
        return;
      this.Mission.PlayerTeam = message.Team;
    }

    private void HandleServerEventCreateFreeMountAgentEvent(CreateFreeMountAgent message)
    {
      Mat3 identity = Mat3.Identity;
      identity.f = message.Direction.ToVec3();
      identity.OrthonormalizeAccordingToForwardAndKeepUpAsZAxis();
      this.Mission.SpawnMonster(message.HorseItem, message.HorseHarnessItem, new MatrixFrame(identity, message.Position), message.AgentIndex);
    }

    private void HandleServerEventCreateAgent(CreateAgent message)
    {
      BasicCharacterObject character = message.Character;
      NetworkCommunicator peer = message.Peer;
      MissionPeer missionPeer = peer != null ? peer.GetComponent<MissionPeer>() : (MissionPeer) null;
      AgentBuildData agentBuildData = new AgentBuildData(character);
      if (message.IsPlayerAgent)
        agentBuildData.MissionPeer(missionPeer);
      agentBuildData.Monster(message.Monster);
      agentBuildData.Equipment(message.SpawnEquipment);
      agentBuildData.MissionEquipment(message.SpawnMissionEquipment);
      agentBuildData.Team(message.Team);
      Formation formation = (Formation) null;
      if (message.Team != null && message.FormationIndex >= 0 && !GameNetwork.IsReplay)
      {
        formation = message.Team.GetFormation((FormationClass) message.FormationIndex);
        agentBuildData.Formation(formation);
      }
      agentBuildData.ClothingColor1(message.ClothingColor1);
      agentBuildData.ClothingColor2(message.ClothingColor2);
      Mat3 identity = Mat3.Identity;
      identity.f = message.Direction.ToVec3();
      identity.OrthonormalizeAccordingToForwardAndKeepUpAsZAxis();
      agentBuildData.InitialFrame(new MatrixFrame(identity, message.Position));
      agentBuildData.IsFemale(message.IsFemale);
      agentBuildData.TroopOrigin((IAgentOriginBase) new BasicBattleAgentOrigin(character));
      agentBuildData.EquipmentSeed(message.BodyPropertiesSeed);
      if (message.IsPlayerAgent)
        agentBuildData.BodyProperties(missionPeer.Peer.BodyProperties);
      else
        agentBuildData.BodyProperties(BodyProperties.GetRandomBodyProperties(agentBuildData.AgentIsFemale, character.GetBodyPropertiesMin(), character.GetBodyPropertiesMax(), (int) agentBuildData.AgentOverridenSpawnEquipment.HairCoverType, agentBuildData.AgentEquipmentSeed, character.HairTags, character.BeardTags, character.TattooTags));
      agentBuildData.Index(message.AgentIndex);
      agentBuildData.MountIndex(message.MountAgentIndex);
      Banner banner = (Banner) null;
      if (formation != null)
      {
        if (!formation.BannerCode.IsStringNoneOrEmpty())
        {
          if (formation.Banner == null)
          {
            banner = new Banner(formation.BannerCode, message.Team.Color, message.Team.Color2);
            formation.Banner = banner;
          }
          else
            banner = formation.Banner;
        }
      }
      else if (missionPeer != null)
        banner = new Banner(missionPeer.Peer.BannerCode, message.Team.Color, message.Team.Color2);
      agentBuildData.Banner(banner);
      Agent mountAgent = this.Mission.SpawnAgent(agentBuildData).MountAgent;
    }

    private void HandleServerEventCreateAgentVisuals(CreateAgentVisuals message)
    {
      MissionPeer component = message.Peer.GetComponent<MissionPeer>();
      BattleSideEnum side = component.Team.Side;
      BasicCultureObject basicCultureObject = MBObjectManager.Instance.GetObject<BasicCultureObject>(side == BattleSideEnum.Attacker ? MultiplayerOptions.OptionType.CultureTeam1.GetStrValue() : MultiplayerOptions.OptionType.CultureTeam2.GetStrValue());
      BasicCharacterObject character = message.Character;
      AgentBuildData buildData = new AgentBuildData(character);
      buildData.VisualsIndex(message.VisualsIndex);
      buildData.Equipment(message.Equipment);
      buildData.EquipmentSeed(message.BodyPropertiesSeed);
      if (message.VisualsIndex == 0)
        buildData.BodyProperties(component.Peer.BodyProperties);
      else
        buildData.BodyProperties(BodyProperties.GetRandomBodyProperties(buildData.AgentIsFemale, character.GetBodyPropertiesMin(), character.GetBodyPropertiesMax(), (int) buildData.AgentOverridenSpawnEquipment.HairCoverType, message.BodyPropertiesSeed, character.HairTags, character.BeardTags, character.TattooTags));
      buildData.IsFemale(message.IsFemale);
      buildData.ClothingColor1(side == BattleSideEnum.Attacker ? basicCultureObject.Color : basicCultureObject.ClothAlternativeColor);
      buildData.ClothingColor2(side == BattleSideEnum.Attacker ? basicCultureObject.Color2 : basicCultureObject.ClothAlternativeColor2);
      this.Mission.GetMissionBehaviour<MultiplayerMissionAgentVisualSpawnComponent>().SpawnAgentVisualsForPeer(component, buildData, message.SelectedEquipmentSetIndex, totalTroopCount: message.TroopCountInFormation);
    }

    private void HandleServerEventRemoveAgentVisualsForPeer(RemoveAgentVisualsForPeer message)
    {
      MissionPeer component = message.Peer.GetComponent<MissionPeer>();
      this.Mission.GetMissionBehaviour<MultiplayerMissionAgentVisualSpawnComponent>().RemoveAgentVisuals(component);
    }

    private void HandleServerEventRemoveAgentVisualsFromIndexForPeer(
      RemoveAgentVisualsFromIndexForPeer message)
    {
      MissionPeer component = message.Peer.GetComponent<MissionPeer>();
      this.Mission.GetMissionBehaviour<MultiplayerMissionAgentVisualSpawnComponent>().RemoveAgentVisualsWithVisualIndex(component, message.VisualsIndex);
    }

    private void HandleServerEventReplaceBotWithPlayer(ReplaceBotWithPlayer message)
    {
      Agent botAgent = message.BotAgent;
      if (botAgent.Formation != null)
        botAgent.Formation.PlayerOwner = botAgent;
      MissionPeer component = message.Peer.GetComponent<MissionPeer>();
      botAgent.MissionPeer = message.Peer.GetComponent<MissionPeer>();
      botAgent.Formation = component.ControlledFormation;
      botAgent.Health = (float) message.Health;
      if (botAgent.MountAgent != null)
        botAgent.MountAgent.Health = (float) message.MountHealth;
      if (botAgent.Formation == null)
        return;
      botAgent.Team.AssignPlayerAsSergeantOfFormation(component, component.ControlledFormation.FormationIndex);
    }

    private void HandleServerEventSetWieldedItemIndex(SetWieldedItemIndex message)
    {
      if (message.Agent == null)
        return;
      message.Agent.SetWieldedItemIndexAsClient(message.IsLeftHand ? Agent.HandIndex.OffHand : Agent.HandIndex.MainHand, message.WieldedItemIndex, message.IsWieldedInstantly, message.IsWieldedOnSpawn, message.MainHandCurrentUsageIndex);
      message.Agent.UpdateAgentStats();
    }

    private void HandleServerEventSetWeaponNetworkData(SetWeaponNetworkData message)
    {
      WeaponComponentData primaryWeapon = message.Agent.Equipment[message.WeaponEquipmentIndex].Item?.PrimaryWeapon;
      if (primaryWeapon == null)
        return;
      if (primaryWeapon.WeaponFlags.HasAnyFlag<WeaponFlags>(WeaponFlags.HasHitPoints))
      {
        message.Agent.ChangeWeaponHitPoints(message.WeaponEquipmentIndex, message.DataValue);
      }
      else
      {
        if (!primaryWeapon.IsConsumable)
          return;
        message.Agent.SetWeaponAmountInSlot(message.WeaponEquipmentIndex, message.DataValue, true);
      }
    }

    private void HandleServerEventSetWeaponAmmoData(SetWeaponAmmoData message)
    {
      if (!message.Agent.Equipment[message.WeaponEquipmentIndex].CurrentUsageItem.IsRangedWeapon)
        return;
      message.Agent.SetWeaponAmmoAsClient(message.WeaponEquipmentIndex, message.AmmoEquipmentIndex, message.Ammo);
    }

    private void HandleServerEventSetWeaponReloadPhase(SetWeaponReloadPhase message) => message.Agent.SetWeaponReloadPhaseAsClient(message.EquipmentIndex, message.ReloadPhase);

    private void HandleServerEventWeaponUsageIndexChangeMessage(
      WeaponUsageIndexChangeMessage message)
    {
      message.Agent.SetUsageIndexOfWeaponInSlotAsClient(message.SlotIndex, message.UsageIndex);
    }

    private void HandleServerEventStartSwitchingWeaponUsageIndex(
      StartSwitchingWeaponUsageIndex message)
    {
      message.Agent.StartSwitchingWeaponUsageIndexAsClient(message.EquipmentIndex, message.UsageIndex, message.CurrentMovementFlagUsageDirection);
    }

    private void HandleServerEventInitializeFormation(InitializeFormation message) => message.Team.GetFormation((FormationClass) message.FormationIndex).BannerCode = message.BannerCode;

    private void HandleServerEventSetSpawnedFormationCount(SetSpawnedFormationCount message)
    {
      this.Mission.NumOfFormationsSpawnedTeamOne = message.NumOfFormationsTeamOne;
      this.Mission.NumOfFormationsSpawnedTeamTwo = message.NumOfFormationsTeamTwo;
    }

    private void HandleServerEventAddTeam(AddTeam message)
    {
      Banner banner = message.BannerCode.IsStringNoneOrEmpty() ? (Banner) null : new Banner(message.BannerCode, message.Color, message.Color2);
      this.Mission.Teams.Add(message.Side, message.Color, message.Color2, banner, message.IsPlayerGeneral, message.IsPlayerSergeant);
    }

    private void HandleServerEventTeamSetIsEnemyOf(TeamSetIsEnemyOf message) => message.Team1.SetIsEnemyOf(message.Team2, message.IsEnemyOf);

    private void HandleServerEventAssignFormationToPlayer(AssignFormationToPlayer message)
    {
      MissionPeer component = message.Peer.GetComponent<MissionPeer>();
      component.Team.AssignPlayerAsSergeantOfFormation(component, message.FormationClass);
    }

    private void HandleServerEventExistingObjectsBegin(ExistingObjectsBegin message)
    {
    }

    private void HandleServerEventExistingObjectsEnd(ExistingObjectsEnd message)
    {
    }

    private void HandleServerEventClearMission(ClearMission message) => this.Mission.ResetMission();

    private void HandleServerEventCreateMissionObject(CreateMissionObject message)
    {
      GameEntity gameEntity = GameEntity.Instantiate(this.Mission.Scene, message.Prefab, message.Frame);
      MissionObject firstScriptOfType1 = gameEntity.GetFirstScriptOfType<MissionObject>();
      if (firstScriptOfType1 == null)
        return;
      firstScriptOfType1.Id = message.ObjectId;
      int num = 0;
      foreach (GameEntity child in gameEntity.GetChildren())
      {
        MissionObject firstScriptOfType2;
        if ((firstScriptOfType2 = child.GetFirstScriptOfType<MissionObject>()) != null)
          firstScriptOfType2.Id = message.ChildObjectIds[num++];
      }
    }

    private void HandleServerEventRemoveMissionObject(RemoveMissionObject message) => this.Mission.MissionObjects.FirstOrDefault<MissionObject>((Func<MissionObject, bool>) (mo => mo.Id == message.ObjectId))?.GameEntity.Remove(82);

    private void HandleServerEventStopPhysicsAndSetFrameOfMissionObject(
      StopPhysicsAndSetFrameOfMissionObject message)
    {
      ((SpawnedItemEntity) this.Mission.MissionObjects.FirstOrDefault<MissionObject>((Func<MissionObject, bool>) (mo => mo.Id == message.ObjectId)))?.StopPhysicsAndSetFrameForClient(message.Frame, message.Parent?.GameEntity);
    }

    private void HandleServerEventBurstMissionObjectParticles(BurstMissionObjectParticles message) => (message.MissionObject as SynchedMissionObject).BurstParticlesSynched(message.DoChildren);

    private void HandleServerEventSetMissionObjectVisibility(SetMissionObjectVisibility message) => message.MissionObject.GameEntity.SetVisibilityExcludeParents(message.Visible);

    private void HandleServerEventSetMissionObjectDisabled(SetMissionObjectDisabled message) => message.MissionObject.SetDisabledAndMakeInvisible();

    private void HandleServerEventSetMissionObjectColors(SetMissionObjectColors message)
    {
      if (!(message.MissionObject is SynchedMissionObject missionObject))
        return;
      missionObject.SetTeamColors(message.Color, message.Color2);
    }

    private void HandleServerEventSetMissionObjectFrame(SetMissionObjectFrame message)
    {
      SynchedMissionObject missionObject = message.MissionObject as SynchedMissionObject;
      MatrixFrame frame = message.Frame;
      ref MatrixFrame local = ref frame;
      missionObject.SetFrameSynched(ref local, true);
    }

    private void HandleServerEventSetMissionObjectGlobalFrame(SetMissionObjectGlobalFrame message)
    {
      SynchedMissionObject missionObject = message.MissionObject as SynchedMissionObject;
      MatrixFrame frame = message.Frame;
      ref MatrixFrame local = ref frame;
      missionObject.SetGlobalFrameSynched(ref local, true);
    }

    private void HandleServerEventSetMissionObjectFrameOverTime(
      SetMissionObjectFrameOverTime message)
    {
      SynchedMissionObject missionObject = message.MissionObject as SynchedMissionObject;
      MatrixFrame frame = message.Frame;
      ref MatrixFrame local = ref frame;
      double duration = (double) message.Duration;
      missionObject.SetFrameSynchedOverTime(ref local, (float) duration, true);
    }

    private void HandleServerEventSetMissionObjectGlobalFrameOverTime(
      SetMissionObjectGlobalFrameOverTime message)
    {
      SynchedMissionObject missionObject = message.MissionObject as SynchedMissionObject;
      MatrixFrame frame = message.Frame;
      ref MatrixFrame local = ref frame;
      double duration = (double) message.Duration;
      missionObject.SetGlobalFrameSynchedOverTime(ref local, (float) duration, true);
    }

    private void HandleServerEventSetMissionObjectAnimationAtChannel(
      SetMissionObjectAnimationAtChannel message)
    {
      message.MissionObject.GameEntity.Skeleton.SetAnimationAtChannel(message.AnimationIndex, message.ChannelNo, message.AnimationSpeed);
    }

    private void HandleServerEventSetRangedSiegeWeaponAmmo(SetRangedSiegeWeaponAmmo message) => message.RangedSiegeWeapon.SetAmmo(message.AmmoCount);

    private void HandleServerEventRangedSiegeWeaponChangeProjectile(
      RangedSiegeWeaponChangeProjectile message)
    {
      message.RangedSiegeWeapon.ChangeProjectileEntityClient(message.Index);
    }

    private void HandleServerEventSetStonePileAmmo(SetStonePileAmmo message) => message.StonePile.SetAmmo(message.AmmoCount);

    private void HandleServerEventSetRangedSiegeWeaponState(SetRangedSiegeWeaponState message) => message.RangedSiegeWeapon.State = message.State;

    private void HandleServerEventSetSiegeLadderState(SetSiegeLadderState message) => message.SiegeLadder.State = message.State;

    private void HandleServerEventSetSiegeTowerGateState(SetSiegeTowerGateState message) => message.SiegeTower.State = message.State;

    private void HandleServerEventSetSiegeTowerHasArrivedAtTarget(
      SetSiegeTowerHasArrivedAtTarget message)
    {
      message.SiegeTower.HasArrivedAtTarget = true;
    }

    private void HandleServerEventSetBatteringRamHasArrivedAtTarget(
      SetBatteringRamHasArrivedAtTarget message)
    {
      message.BatteringRam.HasArrivedAtTarget = true;
    }

    private void HandleServerEventSetSiegeMachineMovementDistance(
      SetSiegeMachineMovementDistance message)
    {
      if (message.UsableMachine == null)
        return;
      if (message.UsableMachine is SiegeTower)
        ((SiegeTower) message.UsableMachine).MovementComponent.SetDistanceTravelledAsClient(message.Distance);
      else
        ((BatteringRam) message.UsableMachine).MovementComponent.SetDistanceTravelledAsClient(message.Distance);
    }

    private void HandleServerEventSetMissionObjectAnimationChannelParameter(
      SetMissionObjectAnimationChannelParameter message)
    {
      if (message.MissionObject == null)
        return;
      message.MissionObject.GameEntity.Skeleton.SetAnimationParameterAtChannel(message.ChannelNo, message.Parameter);
    }

    private void HandleServerEventSetMissionObjectVertexAnimation(
      SetMissionObjectVertexAnimation message)
    {
      if (message.MissionObject == null)
        return;
      (message.MissionObject as VertexAnimator).SetAnimationSynched(message.BeginKey, message.EndKey, message.Speed);
    }

    private void HandleServerEventSetMissionObjectVertexAnimationProgress(
      SetMissionObjectVertexAnimationProgress message)
    {
      if (message.MissionObject == null)
        return;
      (message.MissionObject as VertexAnimator).SetProgressSynched(message.Progress);
    }

    private void HandleServerEventSetMissionObjectAnimationPaused(
      SetMissionObjectAnimationPaused message)
    {
      if (message.MissionObject == null)
        return;
      if (message.IsPaused)
        message.MissionObject.GameEntity.PauseSkeletonAnimation();
      else
        message.MissionObject.GameEntity.ResumeSkeletonAnimation();
    }

    private void HandleServerEventAddMissionObjectBodyFlags(AddMissionObjectBodyFlags message)
    {
      if (message.MissionObject == null)
        return;
      message.MissionObject.GameEntity.AddBodyFlags(message.BodyFlags, message.ApplyToChildren);
    }

    private void HandleServerEventRemoveMissionObjectBodyFlags(RemoveMissionObjectBodyFlags message)
    {
      if (message.MissionObject == null)
        return;
      message.MissionObject.GameEntity.RemoveBodyFlags(message.BodyFlags, message.ApplyToChildren);
    }

    private void HandleServerEventSetMachineTargetRotation(SetMachineTargetRotation message)
    {
      if (message.UsableMachine == null)
        return;
      ((RangedSiegeWeapon) message.UsableMachine).AimAtRotation(message.HorizontalRotation, message.VerticalRotation);
    }

    private void HandleServerEventSetUsableGameObjectIsDeactivated(
      SetUsableMissionObjectIsDeactivated message)
    {
      if (message.UsableGameObject == null)
        return;
      message.UsableGameObject.IsDeactivated = message.IsDeactivated;
    }

    private void HandleServerEventSetUsableGameObjectIsDisabledForPlayers(
      SetUsableMissionObjectIsDisabledForPlayers message)
    {
      if (message.UsableGameObject == null)
        return;
      message.UsableGameObject.IsDisabledForPlayers = message.IsDisabledForPlayers;
    }

    private void HandleServerEventSetMissionObjectImpulse(SetMissionObjectImpulse message)
    {
      if (message.MissionObject == null)
        return;
      Vec3 position = message.LocalSpace ? message.MissionObject.GameEntity.GlobalPosition + message.Position : message.Position;
      message.MissionObject.GameEntity.ApplyImpulseToDynamicBody(position, message.Impulse);
    }

    private void HandleServerEventSetAgentTargetPositionAndDirection(
      SetAgentTargetPositionAndDirection message)
    {
      Vec2 position = message.Position;
      Vec3 direction = message.Direction;
      message.Agent.SetTargetPositionAndDirectionSynched(ref position, ref direction);
    }

    private void HandleServerEventSetAgentTargetPosition(SetAgentTargetPosition message)
    {
      Vec2 position = message.Position;
      message.Agent.SetTargetPositionSynched(ref position);
    }

    private void HandleServerEventClearAgentTargetFrame(ClearAgentTargetFrame message) => message.Agent.ClearTargetFrame();

    private void HandleServerEventAgentTeleportToFrame(AgentTeleportToFrame message)
    {
      message.Agent.TeleportToPosition(message.Position);
      Vec3 vec3 = message.Direction.ToVec3();
      double num = (double) vec3.Normalize();
      message.Agent.SetMovementDirection(ref vec3);
      message.Agent.LookDirection = vec3;
    }

    private void HandleServerEventSetAgentPeer(SetAgentPeer message)
    {
      if (message.Agent == null)
        return;
      NetworkCommunicator peer = message.Peer;
      MissionPeer missionPeer = peer != null ? peer.GetComponent<MissionPeer>() : (MissionPeer) null;
      message.Agent.MissionPeer = missionPeer;
    }

    private void HandleServerEventSetAgentIsPlayer(SetAgentIsPlayer message)
    {
      if (message.Agent.Controller == Agent.ControllerType.Player == message.IsPlayer)
        return;
      if (!message.Agent.IsMine)
        message.Agent.Controller = Agent.ControllerType.None;
      else
        message.Agent.Controller = Agent.ControllerType.Player;
    }

    private void HandleServerEventSetAgentHealth(SetAgentHealth message) => message.Agent.Health = (float) message.Health;

    private void HandleServerEventAgentSetTeam(AgentSetTeam message) => message.Agent.SetTeam(this.Mission.Teams[message.Team], false);

    private void HandleServerEventSetAgentActionSet(SetAgentActionSet message)
    {
      AnimationSystemData animationSystemData = new AnimationSystemData()
      {
        ActionSet = message.ActionSet,
        NumPaces = message.NumPaces,
        MonsterUsageSetIndex = message.MonsterUsageSetIndex,
        WalkingSpeedLimit = message.WalkingSpeedLimit,
        CrouchWalkingSpeedLimit = message.CrouchWalkingSpeedLimit,
        StepSize = message.StepSize,
        HasClippingPlane = false
      };
      AgentVisualsNativeData agentVisualsNativeData = new AgentVisualsNativeData()
      {
        MainHandItemBoneIndex = message.Agent.Monster.MainHandItemBoneIndex,
        OffHandItemBoneIndex = message.Agent.Monster.OffHandItemBoneIndex,
        MainHandItemSecondaryBoneIndex = message.Agent.Monster.MainHandItemSecondaryBoneIndex,
        OffHandItemSecondaryBoneIndex = message.Agent.Monster.OffHandItemSecondaryBoneIndex,
        RiderSitBoneIndex = message.Agent.Monster.RiderSitBoneIndex,
        ReinHandleBoneIndex = message.Agent.Monster.ReinHandleBoneIndex,
        ReinCollision1BoneIndex = message.Agent.Monster.ReinCollision1BoneIndex,
        ReinCollision2BoneIndex = message.Agent.Monster.ReinCollision2BoneIndex,
        HeadLookDirectionBoneIndex = message.Agent.Monster.HeadLookDirectionBoneIndex,
        ThoraxLookDirectionBoneIndex = message.Agent.Monster.ThoraxLookDirectionBoneIndex,
        MainHandNumBonesForIk = message.Agent.Monster.MainHandNumBonesForIk,
        OffHandNumBonesForIk = message.Agent.Monster.OffHandNumBonesForIk,
        ReinHandleLeftLocalPosition = message.Agent.Monster.ReinHandleLeftLocalPosition,
        ReinHandleRightLocalPosition = message.Agent.Monster.ReinHandleRightLocalPosition
      };
      message.Agent.SetActionSet(ref agentVisualsNativeData, ref animationSystemData);
    }

    private void HandleServerEventMakeAgentDead(MakeAgentDead message) => message.Agent.MakeDead(message.IsKilled, message.ActionCodeIndex);

    private void HandleServerEventAddPrefabComponentToAgentBone(
      AddPrefabComponentToAgentBone message)
    {
      message.Agent.AddSynchedPrefabComponentToBone(message.PrefabName, message.BoneIndex);
    }

    private void HandleServerEventSetAgentPrefabComponentVisibility(
      SetAgentPrefabComponentVisibility message)
    {
      message.Agent.SetSynchedPrefabComponentVisibility(message.ComponentIndex, message.Visibility);
    }

    private void HandleServerEventAgentSetFormation(AgentSetFormation message)
    {
      Agent agent = message.Agent;
      Team team = agent.Team;
      Formation formation = (Formation) null;
      if (team != null)
        formation = message.FormationIndex >= 0 ? team.GetFormation((FormationClass) message.FormationIndex) : (Formation) null;
      agent.Formation = formation;
    }

    private void HandleServerEventUseObject(UseObject message) => message.UsableGameObject?.SetUserForClient(message.Agent);

    private void HandleServerEventStopUsingObject(StopUsingObject message) => message.Agent?.StopUsingGameObject(message.IsSuccessful);

    private void HandleServerEventHitSynchronizeObjectHitpoints(SyncObjectHitpoints message)
    {
      if (message.MissionObject == null)
        return;
      message.MissionObject.GameEntity.GetFirstScriptOfType<DestructableComponent>().HitPoint = message.Hitpoints;
    }

    private void HandleServerEventHitSynchronizeObjectDestructionLevel(
      SyncObjectDestructionLevel message)
    {
      message.MissionObject?.GameEntity.GetFirstScriptOfType<DestructableComponent>().SetDestructionLevel(message.DestructionLevel, message.ForcedIndex, message.BlowMagnitude, message.BlowPosition, message.BlowDirection);
    }

    private void HandleServerEventHitBurstAllHeavyHitParticles(BurstAllHeavyHitParticles message) => message.MissionObject?.GameEntity.GetFirstScriptOfType<DestructableComponent>().BurstHeavyHitParticles();

    private void HandleServerEventSynchronizeMissionObject(SynchronizeMissionObject message)
    {
    }

    private void HandleServerEventSpawnWeaponWithNewEntity(SpawnWeaponWithNewEntity message)
    {
      GameEntity gameEntity = this.Mission.SpawnWeaponWithNewEntityAux(message.Weapon, message.WeaponSpawnFlags, message.Frame, message.ForcedIndex, message.ParentMissionObject, message.HasLifeTime);
      if (message.IsVisible)
        return;
      gameEntity.SetVisibilityExcludeParents(false);
    }

    private void HandleServerEventAttachWeaponToSpawnedWeapon(AttachWeaponToSpawnedWeapon message) => this.Mission.AttachWeaponWithNewEntityToSpawnedWeapon(message.Weapon, message.MissionObject as SpawnedItemEntity, message.AttachLocalFrame);

    private void HandleServerEventAttachWeaponToAgent(AttachWeaponToAgent message)
    {
      MatrixFrame attachLocalFrame = message.AttachLocalFrame;
      message.Agent.AttachWeaponToBone(message.Weapon, (GameEntity) null, message.BoneIndex, ref attachLocalFrame);
    }

    private void HandleServerEventHandleMissileCollisionReaction(
      HandleMissileCollisionReaction message)
    {
      this.Mission.HandleMissileCollisionReaction(message.MissileIndex, message.CollisionReaction, message.AttachLocalFrame, message.AttackerAgent, message.AttachedAgent, message.AttachedToShield, message.AttachedBoneIndex, message.AttachedMissionObject, message.BounceBackVelocity, message.BounceBackAngularVelocity, message.ForcedSpawnIndex);
    }

    private void HandleServerEventSpawnWeaponAsDropFromAgent(SpawnWeaponAsDropFromAgent message)
    {
      Vec3 velocity = message.Velocity;
      Vec3 angularVelocity = message.AngularVelocity;
      this.Mission.SpawnWeaponAsDropFromAgentAux(message.Agent, message.EquipmentIndex, ref velocity, ref angularVelocity, message.WeaponSpawnFlags, message.ForcedIndex);
    }

    private void HandleServerEventSpawnAttachedWeaponOnSpawnedWeapon(
      SpawnAttachedWeaponOnSpawnedWeapon message)
    {
      this.Mission.SpawnAttachedWeaponOnSpawnedWeapon(message.SpawnedWeapon, message.AttachmentIndex, message.ForcedIndex);
    }

    private void HandleServerEventSpawnAttachedWeaponOnCorpse(SpawnAttachedWeaponOnCorpse message) => this.Mission.SpawnAttachedWeaponOnCorpse(message.Agent, message.AttachedIndex, message.ForcedIndex);

    private void HandleServerEventRemoveEquippedWeapon(RemoveEquippedWeapon message) => message.Agent.RemoveEquippedWeapon(message.SlotIndex);

    private void HandleServerEventEquipWeaponWithNewEntity(EquipWeaponWithNewEntity message)
    {
      if (message.Agent == null)
        return;
      MissionWeapon weapon = message.Weapon;
      message.Agent.EquipWeaponWithNewEntity(message.SlotIndex, ref weapon);
    }

    private void HandleServerEventAttachWeaponToWeaponInAgentEquipmentSlot(
      AttachWeaponToWeaponInAgentEquipmentSlot message)
    {
      MatrixFrame attachLocalFrame = message.AttachLocalFrame;
      message.Agent.AttachWeaponToWeapon(message.SlotIndex, message.Weapon, (GameEntity) null, ref attachLocalFrame);
    }

    private void HandleServerEventEquipWeaponFromSpawnedItemEntity(
      EquipWeaponFromSpawnedItemEntity message)
    {
      message.Agent.EquipWeaponFromSpawnedItemEntity(message.SlotIndex, message.SpawnedItemEntity, message.RemoveWeapon);
    }

    private void HandleServerEventCreateMissile(CreateMissile message)
    {
      if (message.WeaponIndex != EquipmentIndex.None)
      {
        Vec3 velocity = message.Direction * message.Speed;
        this.Mission.OnAgentShootMissile(message.Agent, message.WeaponIndex, message.Position, velocity, message.Orientation, message.HasRigidBody, message.IsPrimaryWeaponShot, message.MissileIndex);
      }
      else
        this.Mission.AddCustomMissile(message.Agent, message.Weapon, message.Position, message.Direction, message.Orientation, message.Speed, message.Speed, message.HasRigidBody, message.MissionObjectToIgnore, message.MissileIndex);
    }

    private void HandleServerEventAgentHit(CombatLogNetworkMessage networkMessage) => CombatLogManager.GenerateCombatLog(networkMessage.GetData());

    private void HandleServerEventConsumeWeaponAmount(ConsumeWeaponAmount message) => (message.SpawnedItemEntity as SpawnedItemEntity).ConsumeWeaponAmount(message.ConsumedAmount);

    private bool HandleClientEventSetFollowedAgent(
      NetworkCommunicator networkPeer,
      SetFollowedAgent message)
    {
      networkPeer.GetComponent<MissionPeer>().FollowedAgent = message.Agent;
      return true;
    }

    private bool HandleClientEventSetMachineRotation(
      NetworkCommunicator networkPeer,
      SetMachineRotation message)
    {
      MissionPeer component = networkPeer.GetComponent<MissionPeer>();
      if (component.IsControlledAgentActive && message.UsableMachine is RangedSiegeWeapon)
      {
        RangedSiegeWeapon usableMachine = message.UsableMachine as RangedSiegeWeapon;
        if (component.ControlledAgent == usableMachine.PilotAgent)
          usableMachine.AimAtRotation(message.HorizontalRotation, message.VerticalRotation);
      }
      return true;
    }

    private bool HandleClientEventRequestUseObject(
      NetworkCommunicator networkPeer,
      RequestUseObject message)
    {
      MissionPeer component = networkPeer.GetComponent<MissionPeer>();
      if (message.UsableGameObject != null && component.ControlledAgent != null && component.ControlledAgent.IsActive())
      {
        Vec3 position = component.ControlledAgent.Position;
        Vec3 globalPosition = message.UsableGameObject.InteractionEntity.GlobalPosition;
        float num1 = Math.Max(globalPosition.Distance(message.UsableGameObject.InteractionEntity.GetBoundingBoxMin()), globalPosition.Distance(message.UsableGameObject.InteractionEntity.GetBoundingBoxMax()));
        float num2 = Math.Max(globalPosition.Distance(new Vec3(position.x, position.y, component.ControlledAgent.GetEyeGlobalHeight())) - num1, 0.0f);
        if (component.ControlledAgent.CurrentlyUsedGameObject != message.UsableGameObject && component.ControlledAgent.CanReachAndUseObject(message.UsableGameObject, (float) ((double) num2 * (double) num2 * 0.899999976158142 * 0.899999976158142)) && component.ControlledAgent.ObjectHasVacantPosition(message.UsableGameObject))
          component.ControlledAgent.UseGameObject(message.UsableGameObject, message.UsedObjectPreferenceIndex);
      }
      return true;
    }

    private bool HandleClientEventRequestStopUsingObject(
      NetworkCommunicator networkPeer,
      RequestStopUsingObject message)
    {
      MissionPeer component = networkPeer.GetComponent<MissionPeer>();
      if (component.ControlledAgent?.CurrentlyUsedGameObject != null)
        component.ControlledAgent.StopUsingGameObject(false);
      return true;
    }

    private bool HandleClientEventApplyOrder(NetworkCommunicator networkPeer, ApplyOrder message)
    {
      this.GetOrderControllerOfPeer(networkPeer)?.SetOrder(message.OrderType);
      return true;
    }

    private bool HandleClientEventApplySiegeWeaponOrder(
      NetworkCommunicator networkPeer,
      ApplySiegeWeaponOrder message)
    {
      this.GetOrderControllerOfPeer(networkPeer)?.SiegeWeaponController.SetOrder(message.OrderType);
      return true;
    }

    private bool HandleClientEventApplyOrderWithPosition(
      NetworkCommunicator networkPeer,
      ApplyOrderWithPosition message)
    {
      OrderController controllerOfPeer = this.GetOrderControllerOfPeer(networkPeer);
      if (controllerOfPeer != null)
      {
        WorldPosition orderPosition = new WorldPosition(this.Mission.Scene, UIntPtr.Zero, message.Position, false);
        controllerOfPeer.SetOrderWithPosition(message.OrderType, orderPosition);
      }
      return true;
    }

    private bool HandleClientEventApplyOrderWithFormation(
      NetworkCommunicator networkPeer,
      ApplyOrderWithFormation message)
    {
      Team teamOfPeer = this.GetTeamOfPeer(networkPeer);
      OrderController orderControllerOf = teamOfPeer?.GetOrderControllerOf(networkPeer.ControlledAgent);
      Formation orderFormation = teamOfPeer != null ? teamOfPeer.Formations.SingleOrDefault<Formation>((Func<Formation, bool>) (f => f.Index == message.FormationIndex)) : (Formation) null;
      if (teamOfPeer != null && orderControllerOf != null && orderFormation != null)
        orderControllerOf.SetOrderWithFormation(message.OrderType, orderFormation);
      return true;
    }

    private bool HandleClientEventApplyOrderWithFormationAndPercentage(
      NetworkCommunicator networkPeer,
      ApplyOrderWithFormationAndPercentage message)
    {
      Team teamOfPeer = this.GetTeamOfPeer(networkPeer);
      OrderController orderControllerOf = teamOfPeer?.GetOrderControllerOf(networkPeer.ControlledAgent);
      Formation orderFormation = teamOfPeer != null ? teamOfPeer.Formations.SingleOrDefault<Formation>((Func<Formation, bool>) (f => f.Index == message.FormationIndex)) : (Formation) null;
      float percentage = (float) message.Percentage * 0.01f;
      if (teamOfPeer != null && orderControllerOf != null && orderFormation != null)
        orderControllerOf.SetOrderWithFormationAndPercentage(message.OrderType, orderFormation, percentage);
      return true;
    }

    private bool HandleClientEventApplyOrderWithFormationAndNumber(
      NetworkCommunicator networkPeer,
      ApplyOrderWithFormationAndNumber message)
    {
      Team teamOfPeer = this.GetTeamOfPeer(networkPeer);
      OrderController orderControllerOf = teamOfPeer?.GetOrderControllerOf(networkPeer.ControlledAgent);
      Formation orderFormation = teamOfPeer == null ? (Formation) null : teamOfPeer.Formations.SingleOrDefault<Formation>((Func<Formation, bool>) (f => f.Index == message.FormationIndex));
      int number = message.Number;
      if (teamOfPeer != null && orderControllerOf != null && orderFormation != null)
        orderControllerOf.SetOrderWithFormationAndNumber(message.OrderType, orderFormation, number);
      return true;
    }

    private bool HandleClientEventApplyOrderWithTwoPositions(
      NetworkCommunicator networkPeer,
      ApplyOrderWithTwoPositions message)
    {
      OrderController controllerOfPeer = this.GetOrderControllerOfPeer(networkPeer);
      if (controllerOfPeer != null)
      {
        WorldPosition position1 = new WorldPosition(this.Mission.Scene, UIntPtr.Zero, message.Position1, false);
        WorldPosition position2 = new WorldPosition(this.Mission.Scene, UIntPtr.Zero, message.Position2, false);
        controllerOfPeer.SetOrderWithTwoPositions(message.OrderType, position1, position2);
      }
      return true;
    }

    private bool HandleClientEventApplyOrderWithGameEntity(
      NetworkCommunicator networkPeer,
      ApplyOrderWithMissionObject message)
    {
      IOrderable missionObject = message.MissionObject as IOrderable;
      this.GetOrderControllerOfPeer(networkPeer)?.SetOrderWithOrderableObject(missionObject);
      return true;
    }

    private bool HandleClientEventApplyOrderWithAgent(
      NetworkCommunicator networkPeer,
      ApplyOrderWithAgent message)
    {
      this.GetOrderControllerOfPeer(networkPeer)?.SetOrderWithAgent(message.OrderType, message.Agent);
      return true;
    }

    private bool HandleClientEventSelectAllFormations(
      NetworkCommunicator networkPeer,
      SelectAllFormations message)
    {
      this.GetOrderControllerOfPeer(networkPeer)?.SelectAllFormations();
      return true;
    }

    private bool HandleClientEventSelectAllSiegeWeapons(
      NetworkCommunicator networkPeer,
      SelectAllSiegeWeapons message)
    {
      this.GetOrderControllerOfPeer(networkPeer)?.SiegeWeaponController.SelectAll();
      return true;
    }

    private bool HandleClientEventClearSelectedFormations(
      NetworkCommunicator networkPeer,
      ClearSelectedFormations message)
    {
      this.GetOrderControllerOfPeer(networkPeer)?.ClearSelectedFormations();
      return true;
    }

    private bool HandleClientEventSelectFormation(
      NetworkCommunicator networkPeer,
      SelectFormation message)
    {
      Team teamOfPeer = this.GetTeamOfPeer(networkPeer);
      OrderController orderControllerOf = teamOfPeer?.GetOrderControllerOf(networkPeer.ControlledAgent);
      Formation formation = teamOfPeer == null ? (Formation) null : teamOfPeer.Formations.SingleOrDefault<Formation>((Func<Formation, bool>) (f => f.Index == message.FormationIndex));
      if (teamOfPeer != null && orderControllerOf != null && formation != null)
        orderControllerOf.SelectFormation(formation);
      return true;
    }

    private bool HandleClientEventSelectSiegeWeapon(
      NetworkCommunicator networkPeer,
      SelectSiegeWeapon message)
    {
      Team teamOfPeer = this.GetTeamOfPeer(networkPeer);
      SiegeWeaponController weaponController = teamOfPeer?.GetOrderControllerOf(networkPeer.ControlledAgent).SiegeWeaponController;
      SiegeWeapon siegeWeapon = message.SiegeWeapon;
      if (teamOfPeer != null && weaponController != null && siegeWeapon != null)
        weaponController.Select(siegeWeapon);
      return true;
    }

    private bool HandleClientEventUnselectFormation(
      NetworkCommunicator networkPeer,
      UnselectFormation message)
    {
      Team teamOfPeer = this.GetTeamOfPeer(networkPeer);
      OrderController orderControllerOf = teamOfPeer?.GetOrderControllerOf(networkPeer.ControlledAgent);
      Formation formation = teamOfPeer == null ? (Formation) null : teamOfPeer.Formations.SingleOrDefault<Formation>((Func<Formation, bool>) (f => f.Index == message.FormationIndex));
      if (teamOfPeer != null && orderControllerOf != null && formation != null)
        orderControllerOf.DeselectFormation(formation);
      return true;
    }

    private bool HandleClientEventUnselectSiegeWeapon(
      NetworkCommunicator networkPeer,
      UnselectSiegeWeapon message)
    {
      Team teamOfPeer = this.GetTeamOfPeer(networkPeer);
      SiegeWeaponController weaponController = teamOfPeer?.GetOrderControllerOf(networkPeer.ControlledAgent).SiegeWeaponController;
      SiegeWeapon siegeWeapon = message.SiegeWeapon;
      if (teamOfPeer != null && weaponController != null && siegeWeapon != null)
        weaponController.Deselect(siegeWeapon);
      return true;
    }

    private bool HandleClientEventDropWeapon(NetworkCommunicator networkPeer, DropWeapon message)
    {
      networkPeer.GetComponent<MissionPeer>()?.ControlledAgent?.HandleDropWeapon(message.IsDefendPressed, message.ForcedSlotIndexToDropWeaponFrom);
      return true;
    }

    private bool HandleClientEventBreakAgentVisualsInvulnerability(
      NetworkCommunicator networkPeer,
      AgentVisualsBreakInvulnerability message)
    {
      if (this.Mission == null || this.Mission.GetMissionBehaviour<SpawnComponent>() == null || networkPeer.GetComponent<MissionPeer>() == null)
        return false;
      this.Mission.GetMissionBehaviour<SpawnComponent>().SetEarlyAgentVisualsDespawning(networkPeer.GetComponent<MissionPeer>());
      return true;
    }

    private bool HandleClientEventRequestToSpawnAsBot(
      NetworkCommunicator networkPeer,
      RequestToSpawnAsBot message)
    {
      MissionPeer component = networkPeer.GetComponent<MissionPeer>();
      if (component == null)
        return false;
      if (component.HasSpawnTimerExpired)
        component.WantsToSpawnAsBot = true;
      return true;
    }

    private void SendExistingObjectsToPeer(NetworkCommunicator networkPeer)
    {
      MBDebug.Print("Sending all existing objects to peer: " + networkPeer.UserName + " with index: " + (object) networkPeer.Index, debugFilter: 17179869184UL);
      GameNetwork.BeginModuleEventAsServer(networkPeer);
      GameNetwork.WriteMessage((GameNetworkMessage) new ExistingObjectsBegin());
      GameNetwork.EndModuleEventAsServer();
      GameNetwork.BeginModuleEventAsServer(networkPeer);
      GameNetwork.WriteMessage((GameNetworkMessage) new SynchronizeMissionTimeTracker((float) MissionTime.Now.ToSeconds));
      GameNetwork.EndModuleEventAsServer();
      this.SendTeamsToPeer(networkPeer);
      this.SendTeamRelationsToPeer(networkPeer);
      foreach (MissionPeer peerComponent in GameNetwork.NetworkPeers.Where<NetworkCommunicator>((Func<NetworkCommunicator, bool>) (x => x.IsSynchronized)).Select<NetworkCommunicator, MissionPeer>((Func<NetworkCommunicator, MissionPeer>) (x => x.GetComponent<MissionPeer>())).ToList<MissionPeer>())
      {
        if (peerComponent.Team != null)
        {
          GameNetwork.BeginModuleEventAsServer(networkPeer);
          GameNetwork.WriteMessage((GameNetworkMessage) new SetPeerTeam(peerComponent.GetNetworkPeer(), peerComponent.Team));
          GameNetwork.EndModuleEventAsServer();
        }
      }
      this.SendFormationInformation(networkPeer);
      this.SendAgentsToPeer(networkPeer);
      this.SendSpawnedMissionObjectsToPeer(networkPeer);
      this.SynchronizeMissionObjectsToPeer(networkPeer);
      this.SendMissilesToPeer(networkPeer);
      this.SendTroopSelectionInformation(networkPeer);
      networkPeer.SendExistingObjects(this.Mission);
      GameNetwork.BeginModuleEventAsServer(networkPeer);
      GameNetwork.WriteMessage((GameNetworkMessage) new ExistingObjectsEnd());
      GameNetwork.EndModuleEventAsServer();
    }

    private void SendTroopSelectionInformation(NetworkCommunicator networkPeer)
    {
      foreach (NetworkCommunicator networkPeer1 in GameNetwork.NetworkPeers)
      {
        MissionPeer component = networkPeer1.GetComponent<MissionPeer>();
        if (component != null)
        {
          for (int index1 = 0; index1 < component.Perks.Count; ++index1)
          {
            for (int index2 = 0; index2 < 3; ++index2)
            {
              GameNetwork.BeginModuleEventAsServer(networkPeer);
              GameNetwork.WriteMessage((GameNetworkMessage) new SyncPerk(networkPeer1, index2, component.Perks[index1][index2], index1));
              GameNetwork.EndModuleEventAsServer();
            }
          }
          GameNetwork.BeginModuleEventAsServer(networkPeer);
          GameNetwork.WriteMessage((GameNetworkMessage) new UpdateSelectedTroopIndex(networkPeer1, component.SelectedTroopIndex));
          GameNetwork.EndModuleEventAsServer();
        }
      }
    }

    private void SendTeamsToPeer(NetworkCommunicator networkPeer)
    {
      foreach (Team team in (ReadOnlyCollection<Team>) this.Mission.Teams)
      {
        MBDebug.Print("Syncing a team to peer: " + networkPeer.UserName + " with index: " + (object) networkPeer.Index, debugFilter: 17179869184UL);
        GameNetwork.BeginModuleEventAsServer(networkPeer);
        GameNetwork.WriteMessage((GameNetworkMessage) new AddTeam(team));
        GameNetwork.EndModuleEventAsServer();
      }
    }

    private void SendTeamRelationsToPeer(NetworkCommunicator networkPeer)
    {
      int count = this.Mission.Teams.Count;
      for (int index1 = 0; index1 < count; ++index1)
      {
        for (int index2 = index1; index2 < count; ++index2)
        {
          Team team1 = this.Mission.Teams[index1];
          Team team2 = this.Mission.Teams[index2];
          if (team1.IsEnemyOf(team2))
          {
            GameNetwork.BeginModuleEventAsServer(networkPeer);
            GameNetwork.WriteMessage((GameNetworkMessage) new TeamSetIsEnemyOf(team1, team2, true));
            GameNetwork.EndModuleEventAsServer();
          }
        }
      }
    }

    private void SendFormationInformation(NetworkCommunicator networkPeer)
    {
      MBDebug.Print("formations sending begin-", debugFilter: 17179869184UL);
      foreach (Team team in (ReadOnlyCollection<Team>) this.Mission.Teams)
      {
        if (team.IsValid && team.Side != BattleSideEnum.None)
        {
          foreach (Formation formation in team.FormationsIncludingEmpty)
          {
            if (!formation.BannerCode.IsStringNoneOrEmpty())
            {
              GameNetwork.BeginModuleEventAsServer(networkPeer);
              GameNetwork.WriteMessage((GameNetworkMessage) new InitializeFormation(formation, team, formation.BannerCode));
              GameNetwork.EndModuleEventAsServer();
            }
          }
        }
      }
      if (!networkPeer.IsServerPeer)
      {
        GameNetwork.BeginModuleEventAsServer(networkPeer);
        GameNetwork.WriteMessage((GameNetworkMessage) new SetSpawnedFormationCount(this.Mission.NumOfFormationsSpawnedTeamOne, this.Mission.NumOfFormationsSpawnedTeamTwo));
        GameNetwork.EndModuleEventAsServer();
      }
      MBDebug.Print("formations sending end-", debugFilter: 17179869184UL);
    }

    private void SendAgentVisualsToPeer(NetworkCommunicator networkPeer, Team peerTeam)
    {
      MBDebug.Print("agentvisuals sending begin-", debugFilter: 17179869184UL);
      uint num = 0;
      foreach (MissionPeer missionPeer in GameNetwork.NetworkPeers.Select<NetworkCommunicator, MissionPeer>((Func<NetworkCommunicator, MissionPeer>) (p => p.GetComponent<MissionPeer>())).Where<MissionPeer>((Func<MissionPeer, bool>) (pr => pr != null)))
      {
        if (missionPeer.Team == peerTeam)
        {
          int agentVisualsForPeer = missionPeer.GetAmountOfAgentVisualsForPeer();
          for (int visualIndex = 0; visualIndex < agentVisualsForPeer; ++visualIndex)
          {
            PeerVisualsHolder visuals = missionPeer.GetVisuals(visualIndex);
            IAgentVisual agentVisuals = visuals.AgentVisuals;
            AgentBuildData agentBuildData = new AgentBuildData(MBObjectManager.Instance.GetObject<BasicCharacterObject>(agentVisuals.GetCharacterObjectID()));
            if (num == 0U)
              agentBuildData.MissionPeer(missionPeer);
            MatrixFrame frame = agentVisuals.GetFrame();
            frame.rotation.MakeUnit();
            agentBuildData.Equipment(agentVisuals.GetEquipment());
            agentBuildData.VisualsIndex(visuals.VisualsIndex);
            agentBuildData.Team(missionPeer.Team);
            agentBuildData.IsFemale(agentVisuals.GetIsFemale());
            agentBuildData.BodyProperties(agentVisuals.GetBodyProperties());
            agentBuildData.InitialFrame(frame);
            GameNetwork.BeginModuleEventAsServer(networkPeer);
            GameNetwork.WriteMessage((GameNetworkMessage) new CreateAgentVisuals(missionPeer.GetNetworkPeer(), agentBuildData, missionPeer.SelectedTroopIndex));
            GameNetwork.EndModuleEventAsServer();
          }
        }
      }
      MBDebug.Print("agentvisuals sending end-", debugFilter: 17179869184UL);
    }

    private void SendAgentsToPeer(NetworkCommunicator networkPeer)
    {
      MBDebug.Print("agents sending begin-", debugFilter: 17179869184UL);
      foreach (Agent allAgent in (IEnumerable<Agent>) this.Mission.AllAgents)
      {
        Agent agent = allAgent;
        bool isMount = agent.IsMount;
        AgentState state = agent.State;
        if (state == AgentState.Active || (state == AgentState.Killed || state == AgentState.Unconscious) && (agent.GetAttachedWeaponsCount() > 0 || !isMount && (agent.GetWieldedItemIndex(Agent.HandIndex.MainHand) >= EquipmentIndex.WeaponItemBeginSlot || agent.GetWieldedItemIndex(Agent.HandIndex.OffHand) >= EquipmentIndex.WeaponItemBeginSlot)) || state != AgentState.Active && this.Mission.Missiles.Any<Mission.Missile>((Func<Mission.Missile, bool>) (m => m.ShooterAgent == agent)))
        {
          if (isMount && agent.RiderAgent == null)
          {
            MBDebug.Print("mount sending " + (object) agent.Index, debugFilter: 17179869184UL);
            GameNetwork.BeginModuleEventAsServer(networkPeer);
            GameNetwork.WriteMessage((GameNetworkMessage) new CreateFreeMountAgent(agent, agent.Position, agent.GetMovementDirection().AsVec2));
            GameNetwork.EndModuleEventAsServer();
            agent.LockAgentReplicationTableDataWithCurrentReliableSequenceNo(networkPeer);
            int attachedWeaponsCount = agent.GetAttachedWeaponsCount();
            for (int index = 0; index < attachedWeaponsCount; ++index)
            {
              GameNetwork.BeginModuleEventAsServer(networkPeer);
              GameNetwork.WriteMessage((GameNetworkMessage) new AttachWeaponToAgent(agent.GetAttachedWeapon(index), agent, agent.GetAttachedWeaponBoneIndex(index), agent.GetAttachedWeaponFrame(index)));
              GameNetwork.EndModuleEventAsServer();
            }
            if (!agent.IsActive())
            {
              GameNetwork.BeginModuleEventAsServer(networkPeer);
              GameNetwork.WriteMessage((GameNetworkMessage) new MakeAgentDead(agent, state == AgentState.Killed, agent.GetCurrentAction(0)));
              GameNetwork.EndModuleEventAsServer();
            }
          }
          else if (!isMount)
          {
            MBDebug.Print("human sending " + (object) agent.Index, debugFilter: 17179869184UL);
            Agent agent1 = agent.MountAgent;
            if (agent1 != null && agent1.RiderAgent == null)
              agent1 = (Agent) null;
            GameNetwork.BeginModuleEventAsServer(networkPeer);
            Agent agent2 = agent;
            int num1 = agent.MissionPeer == null ? 0 : (agent.OwningAgentMissionPeer == null ? 1 : 0);
            Vec3 position = agent.Position;
            Vec2 asVec2 = agent.GetMovementDirection().AsVec2;
            MissionPeer missionPeer = agent.MissionPeer;
            NetworkCommunicator peer = missionPeer != null ? missionPeer.GetNetworkPeer() : (NetworkCommunicator) null;
            if (peer == null)
            {
              MissionPeer agentMissionPeer = agent.OwningAgentMissionPeer;
              peer = agentMissionPeer != null ? agentMissionPeer.GetNetworkPeer() : (NetworkCommunicator) null;
            }
            GameNetwork.WriteMessage((GameNetworkMessage) new CreateAgent(agent2, num1 != 0, position, asVec2, peer));
            GameNetwork.EndModuleEventAsServer();
            agent.LockAgentReplicationTableDataWithCurrentReliableSequenceNo(networkPeer);
            agent1?.LockAgentReplicationTableDataWithCurrentReliableSequenceNo(networkPeer);
            for (EquipmentIndex index = EquipmentIndex.WeaponItemBeginSlot; index < EquipmentIndex.NumAllWeaponSlots; ++index)
            {
              int attachmentIndex = 0;
              while (true)
              {
                int num2 = attachmentIndex;
                MissionWeapon missionWeapon = agent.Equipment[index];
                int attachedWeaponsCount = missionWeapon.GetAttachedWeaponsCount();
                if (num2 < attachedWeaponsCount)
                {
                  GameNetwork.BeginModuleEventAsServer(networkPeer);
                  missionWeapon = agent.Equipment[index];
                  MissionWeapon attachedWeapon = missionWeapon.GetAttachedWeapon(attachmentIndex);
                  Agent agent3 = agent;
                  int num3 = (int) index;
                  missionWeapon = agent.Equipment[index];
                  MatrixFrame attachedWeaponFrame = missionWeapon.GetAttachedWeaponFrame(attachmentIndex);
                  GameNetwork.WriteMessage((GameNetworkMessage) new AttachWeaponToWeaponInAgentEquipmentSlot(attachedWeapon, agent3, (EquipmentIndex) num3, attachedWeaponFrame));
                  GameNetwork.EndModuleEventAsServer();
                  ++attachmentIndex;
                }
                else
                  break;
              }
            }
            int attachedWeaponsCount1 = agent.GetAttachedWeaponsCount();
            for (int index = 0; index < attachedWeaponsCount1; ++index)
            {
              GameNetwork.BeginModuleEventAsServer(networkPeer);
              GameNetwork.WriteMessage((GameNetworkMessage) new AttachWeaponToAgent(agent.GetAttachedWeapon(index), agent, agent.GetAttachedWeaponBoneIndex(index), agent.GetAttachedWeaponFrame(index)));
              GameNetwork.EndModuleEventAsServer();
            }
            if (agent1 != null)
            {
              int attachedWeaponsCount2 = agent1.GetAttachedWeaponsCount();
              for (int index = 0; index < attachedWeaponsCount2; ++index)
              {
                GameNetwork.BeginModuleEventAsServer(networkPeer);
                GameNetwork.WriteMessage((GameNetworkMessage) new AttachWeaponToAgent(agent1.GetAttachedWeapon(index), agent1, agent1.GetAttachedWeaponBoneIndex(index), agent1.GetAttachedWeaponFrame(index)));
                GameNetwork.EndModuleEventAsServer();
              }
            }
            EquipmentIndex wieldedItemIndex = agent.GetWieldedItemIndex(Agent.HandIndex.MainHand);
            int mainHandCurUsageIndex = wieldedItemIndex != EquipmentIndex.None ? agent.Equipment[wieldedItemIndex].CurrentUsageIndex : 0;
            GameNetwork.BeginModuleEventAsServer(networkPeer);
            GameNetwork.WriteMessage((GameNetworkMessage) new SetWieldedItemIndex(agent, false, true, true, wieldedItemIndex, mainHandCurUsageIndex));
            GameNetwork.EndModuleEventAsServer();
            GameNetwork.BeginModuleEventAsServer(networkPeer);
            GameNetwork.WriteMessage((GameNetworkMessage) new SetWieldedItemIndex(agent, true, true, true, agent.GetWieldedItemIndex(Agent.HandIndex.OffHand), mainHandCurUsageIndex));
            GameNetwork.EndModuleEventAsServer();
            MBActionSet actionSet = agent.ActionSet;
            if (actionSet.IsValid)
            {
              AnimationSystemData animationSystemData = agent.Monster.FillAnimationSystemData(actionSet, agent.Character.GetStepSize(), false);
              GameNetwork.BeginModuleEventAsServer(networkPeer);
              GameNetwork.WriteMessage((GameNetworkMessage) new SetAgentActionSet(agent, animationSystemData));
              GameNetwork.EndModuleEventAsServer();
              if (!agent.IsActive())
              {
                GameNetwork.BeginModuleEventAsServer(networkPeer);
                GameNetwork.WriteMessage((GameNetworkMessage) new MakeAgentDead(agent, state == AgentState.Killed, agent.GetCurrentAction(0)));
                GameNetwork.EndModuleEventAsServer();
              }
            }
            else if (!agent.IsActive())
            {
              GameNetwork.BeginModuleEventAsServer(networkPeer);
              GameNetwork.WriteMessage((GameNetworkMessage) new MakeAgentDead(agent, state == AgentState.Killed, ActionIndexCache.act_none));
              GameNetwork.EndModuleEventAsServer();
            }
          }
          else
            MBDebug.Print("agent not sending " + (object) agent.Index, debugFilter: 17179869184UL);
        }
      }
      MBDebug.Print("agents sending end-", debugFilter: 17179869184UL);
    }

    private void SendSpawnedMissionObjectsToPeer(NetworkCommunicator networkPeer)
    {
      using (IEnumerator<MissionObject> enumerator = this.Mission.MissionObjects.GetEnumerator())
      {
label_18:
        while (enumerator.MoveNext())
        {
          MissionObject missionObject = enumerator.Current;
          if (missionObject is SpawnedItemEntity spawnedWeapon4)
          {
            GameEntity gameEntity = spawnedWeapon4.GameEntity;
            if ((NativeObject) gameEntity.Parent == (NativeObject) null || !gameEntity.Parent.HasScriptOfType<SpawnedItemEntity>())
            {
              MissionObject parentMissionObject = (MissionObject) null;
              if ((NativeObject) spawnedWeapon4.GameEntity.Parent != (NativeObject) null)
                parentMissionObject = gameEntity.Parent.GetFirstScriptOfType<MissionObject>();
              MatrixFrame frame = gameEntity.GetGlobalFrame();
              if (parentMissionObject != null)
                frame = parentMissionObject.GameEntity.GetGlobalFrame().TransformToLocalNonOrthogonal(ref frame);
              frame.origin.z = Math.Max(frame.origin.z, CompressionBasic.PositionCompressionInfo.GetMinimumValue() + 1f);
              Mission.WeaponSpawnFlags weaponSpawnFlags = spawnedWeapon4.SpawnFlags;
              if (weaponSpawnFlags.HasAnyFlag<Mission.WeaponSpawnFlags>(Mission.WeaponSpawnFlags.WithPhysics) && !gameEntity.GetPhysicsState())
                weaponSpawnFlags = weaponSpawnFlags & ~Mission.WeaponSpawnFlags.WithPhysics | Mission.WeaponSpawnFlags.WithStaticPhysics;
              bool hasLifeTime = true;
              bool isVisible = (NativeObject) gameEntity.Parent == (NativeObject) null || parentMissionObject != null;
              GameNetwork.BeginModuleEventAsServer(networkPeer);
              GameNetwork.WriteMessage((GameNetworkMessage) new SpawnWeaponWithNewEntity(spawnedWeapon4.WeaponCopy, weaponSpawnFlags, spawnedWeapon4.Id.Id, frame, parentMissionObject, isVisible, hasLifeTime));
              GameNetwork.EndModuleEventAsServer();
              int num1 = 0;
              while (true)
              {
                int num2 = num1;
                MissionWeapon missionWeapon = spawnedWeapon4.WeaponCopy;
                int attachedWeaponsCount = missionWeapon.GetAttachedWeaponsCount();
                if (num2 < attachedWeaponsCount)
                {
                  GameNetwork.BeginModuleEventAsServer(networkPeer);
                  missionWeapon = spawnedWeapon4.WeaponCopy;
                  MissionWeapon attachedWeapon = missionWeapon.GetAttachedWeapon(num1);
                  SpawnedItemEntity spawnedItemEntity = spawnedWeapon4;
                  missionWeapon = spawnedWeapon4.WeaponCopy;
                  MatrixFrame attachedWeaponFrame = missionWeapon.GetAttachedWeaponFrame(num1);
                  GameNetwork.WriteMessage((GameNetworkMessage) new AttachWeaponToSpawnedWeapon(attachedWeapon, (MissionObject) spawnedItemEntity, attachedWeaponFrame));
                  GameNetwork.EndModuleEventAsServer();
                  missionWeapon = spawnedWeapon4.WeaponCopy;
                  missionWeapon = missionWeapon.GetAttachedWeapon(num1);
                  if (missionWeapon.Item.ItemFlags.HasAnyFlag<ItemFlags>(ItemFlags.CanBePickedUpFromCorpse))
                  {
                    GameNetwork.BeginModuleEventAsServer(networkPeer);
                    GameNetwork.WriteMessage((GameNetworkMessage) new SpawnAttachedWeaponOnSpawnedWeapon(spawnedWeapon4, num1, gameEntity.GetChild(num1).GetFirstScriptOfType<SpawnedItemEntity>().Id.Id));
                    GameNetwork.EndModuleEventAsServer();
                  }
                  ++num1;
                }
                else
                  goto label_18;
              }
            }
          }
          else if (missionObject.CreatedAtRuntime)
          {
            Mission.DynamicallyCreatedEntity dynamicallyCreatedEntity = this.Mission.AddedEntitiesInfo.SingleOrDefault<Mission.DynamicallyCreatedEntity>((Func<Mission.DynamicallyCreatedEntity, bool>) (x => x.ObjectId == missionObject.Id));
            if (dynamicallyCreatedEntity != null)
            {
              GameNetwork.BeginModuleEventAsServer(networkPeer);
              GameNetwork.WriteMessage((GameNetworkMessage) new CreateMissionObject(dynamicallyCreatedEntity.ObjectId, dynamicallyCreatedEntity.Prefab, dynamicallyCreatedEntity.Frame, dynamicallyCreatedEntity.ChildObjectIds));
              GameNetwork.EndModuleEventAsServer();
            }
          }
        }
      }
    }

    private void SynchronizeMissionObjectsToPeer(NetworkCommunicator networkPeer)
    {
      foreach (MissionObject missionObject1 in (IEnumerable<MissionObject>) this.Mission.MissionObjects)
      {
        if (missionObject1 is SynchedMissionObject missionObject)
        {
          GameNetwork.BeginModuleEventAsServer(networkPeer);
          GameNetwork.WriteMessage((GameNetworkMessage) new SynchronizeMissionObject(missionObject));
          GameNetwork.EndModuleEventAsServer();
        }
      }
    }

    private void SendMissilesToPeer(NetworkCommunicator networkPeer)
    {
      foreach (Mission.Missile missile in this.Mission.Missiles)
      {
        Vec3 velocity = missile.GetVelocity();
        float speed = velocity.Normalize();
        Mat3 identity = Mat3.Identity;
        identity.f = velocity;
        identity.Orthonormalize();
        GameNetwork.BeginModuleEventAsServer(networkPeer);
        GameNetwork.WriteMessage((GameNetworkMessage) new CreateMissile(missile.Index, missile.ShooterAgent, EquipmentIndex.None, missile.Weapon, missile.GetPosition(), velocity, speed, identity, missile.GetHasRigidBody(), missile.MissionObjectToIgnore, false));
        GameNetwork.EndModuleEventAsServer();
      }
    }

    private void OnPeerComponentPreRemoved(VirtualPlayer peer, PeerComponent component)
    {
      if (!(component is MissionPeer missionPeer) || !missionPeer.HasSpawnedAgentVisuals)
        return;
      this.Mission.GetMissionBehaviour<MultiplayerMissionAgentVisualSpawnComponent>().RemoveAgentVisuals(missionPeer);
    }

    protected override void HandleEarlyNewClientAfterLoadingFinished(NetworkCommunicator networkPeer)
    {
      if (!networkPeer.IsServerPeer)
      {
        foreach (NetworkCommunicator networkPeer1 in GameNetwork.NetworkPeers)
        {
          if (networkPeer1.IsSynchronized)
            networkPeer1.VirtualPlayer.SynchronizeComponentsTo(networkPeer.VirtualPlayer);
        }
      }
      networkPeer.GetComponent<MissionPeer>();
      networkPeer.AddComponent<MissionPeer>().JoinTime = DateTime.Now;
    }

    protected override void HandleLateNewClientAfterLoadingFinished(NetworkCommunicator networkPeer)
    {
      if (networkPeer.IsServerPeer)
        return;
      this.SendExistingObjectsToPeer(networkPeer);
    }

    protected override void HandleNewClientAfterSynchronized(NetworkCommunicator networkPeer)
    {
      Team parameter = networkPeer.PlayerConnectionInfo.GetParameter<Team>("Team");
      if (parameter == null)
        return;
      networkPeer.GetComponent<MissionPeer>().Team = parameter;
    }

    protected override void HandleEarlyPlayerDisconnect(NetworkCommunicator networkPeer)
    {
      MissionPeer component = networkPeer.GetComponent<MissionPeer>();
      if (component == null)
        return;
      this.Mission?.GetMissionBehaviour<MultiplayerMissionAgentVisualSpawnComponent>().RemoveAgentVisuals(component, true);
    }

    protected override void HandlePlayerDisconnect(NetworkCommunicator networkPeer)
    {
      MissionPeer component = networkPeer.GetComponent<MissionPeer>();
      if (component == null)
        return;
      if (component.ControlledAgent != null)
      {
        Agent controlledAgent = component.ControlledAgent;
        Blow b = new Blow(controlledAgent.Index)
        {
          WeaponRecord = new BlowWeaponRecord(),
          DamageType = DamageTypes.Invalid,
          BaseMagnitude = 10000f
        };
        b.WeaponRecord.WeaponClass = WeaponClass.Undefined;
        b.Position = controlledAgent.Position;
        controlledAgent.Die(b);
      }
      if (this.Mission.AllAgents != null)
      {
        foreach (Agent allAgent in (IEnumerable<Agent>) this.Mission.AllAgents)
        {
          if (allAgent.MissionPeer == component)
            allAgent.MissionPeer = (MissionPeer) null;
          if (allAgent.OwningAgentMissionPeer == component)
            allAgent.OwningAgentMissionPeer = (MissionPeer) null;
        }
      }
      if (component.ControlledFormation != null)
        component.ControlledFormation.PlayerOwner = (Agent) null;
      networkPeer.RemoveComponent<MissionPeer>();
    }

    public override void OnAddTeam(Team team)
    {
      base.OnAddTeam(team);
      if (GameNetwork.IsServerOrRecorder)
      {
        MBDebug.Print("----------OnAddTeam-");
        MBDebug.Print("Adding a team and sending it to all clients", debugFilter: 17179869184UL);
        GameNetwork.BeginBroadcastModuleEvent();
        GameNetwork.WriteMessage((GameNetworkMessage) new AddTeam(team));
        GameNetwork.EndBroadcastModuleEvent(GameNetwork.EventBroadcastFlags.AddToMissionRecord);
      }
      else
      {
        if (team.Side == BattleSideEnum.Attacker || team.Side == BattleSideEnum.Defender || this.Mission.SpectatorTeam != null)
          return;
        this.Mission.SpectatorTeam = team;
      }
    }

    public override void OnClearScene()
    {
      foreach (MissionPeer peer in VirtualPlayer.Peers<MissionPeer>())
        ;
      if (!GameNetwork.IsServerOrRecorder)
        return;
      MBDebug.Print("I am clearing the scene, and sending this message to all clients", debugFilter: 17179869184UL);
      GameNetwork.BeginBroadcastModuleEvent();
      GameNetwork.WriteMessage((GameNetworkMessage) new ClearMission());
      GameNetwork.EndBroadcastModuleEvent(GameNetwork.EventBroadcastFlags.AddToMissionRecord);
    }

    public override void OnMissionTick(float dt)
    {
      if (GameNetwork.IsServerOrRecorder)
      {
        this._accumulatedTimeSinceLastTimerSync += dt;
        if ((double) this._accumulatedTimeSinceLastTimerSync > 2.0)
        {
          this._accumulatedTimeSinceLastTimerSync -= 2f;
          GameNetwork.BeginBroadcastModuleEvent();
          GameNetwork.WriteMessage((GameNetworkMessage) new SynchronizeMissionTimeTracker((float) MissionTime.Now.ToSeconds));
          GameNetwork.EndBroadcastModuleEvent(GameNetwork.EventBroadcastFlags.None);
        }
      }
      foreach (NetworkCommunicator networkPeer in GameNetwork.NetworkPeers)
      {
        networkPeer.GetComponent<MissionRepresentativeBase>()?.Tick(dt);
        if (GameNetwork.IsServer)
          networkPeer.GetComponent<MissionPeer>()?.TickInactivityStatus();
      }
    }

    public void OnPeerSelectedTeam(MissionPeer missionPeer) => this.SendAgentVisualsToPeer(missionPeer.GetNetworkPeer(), missionPeer.Team);
  }
}
