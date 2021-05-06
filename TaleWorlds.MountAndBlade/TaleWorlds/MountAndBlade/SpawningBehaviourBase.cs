// Decompiled with JetBrains decompiler
// Type: TaleWorlds.MountAndBlade.SpawningBehaviourBase
// Assembly: TaleWorlds.MountAndBlade, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3C0E3F42-756F-4407-B817-06BA412A78B0
// Assembly location: E:\SteamLibrary\steamapps\common\Mount & Blade II Bannerlord\bin\Win64_Shipping_Client\TaleWorlds.MountAndBlade.dll

using System;
using System.Collections.Generic;
using System.Linq;
using TaleWorlds.Core;
using TaleWorlds.Engine;
using TaleWorlds.Library;
using TaleWorlds.MountAndBlade.Diamond;
using TaleWorlds.ObjectSystem;

namespace TaleWorlds.MountAndBlade
{
  public abstract class SpawningBehaviourBase
  {
    protected MissionMultiplayerGameModeBase GameMode;
    protected SpawnComponent SpawnComponent;
    private bool _equipmentUpdatingExpired;
    protected bool IsSpawningEnabled;
    protected float SpawningEndDelay = 1f;
    protected float SpawningDelayTimer;
    private bool _hasCalledSpawningEnded;
    protected MissionLobbyComponent MissionLobbyComponent;
    protected MissionLobbyEquipmentNetworkComponent MissionLobbyEquipmentNetworkComponent;
    public static readonly ActionIndexCache PoseActionInfantry = ActionIndexCache.Create("act_walk_idle_unarmed");
    public static readonly ActionIndexCache PoseActionCavalry = ActionIndexCache.Create("act_horse_stand_1");

    public MultiplayerMissionAgentVisualSpawnComponent AgentVisualSpawnComponent { get; private set; }

    protected Mission Mission => this.SpawnComponent.Mission;

    protected event Action<MissionPeer> OnAllAgentsFromPeerSpawnedFromVisuals;

    protected event Action<MissionPeer> OnPeerSpawnedFromVisuals;

    public event SpawningBehaviourBase.OnSpawningEndedEventDelegate OnSpawningEnded;

    public virtual void Initialize(SpawnComponent spawnComponent)
    {
      this.SpawnComponent = spawnComponent;
      this.AgentVisualSpawnComponent = this.Mission.GetMissionBehaviour<MultiplayerMissionAgentVisualSpawnComponent>();
      this.GameMode = this.Mission.GetMissionBehaviour<MissionMultiplayerGameModeBase>();
      this.MissionLobbyComponent = this.Mission.GetMissionBehaviour<MissionLobbyComponent>();
      this.MissionLobbyEquipmentNetworkComponent = this.Mission.GetMissionBehaviour<MissionLobbyEquipmentNetworkComponent>();
      this.MissionLobbyEquipmentNetworkComponent.OnEquipmentRefreshed += new MissionLobbyEquipmentNetworkComponent.OnRefreshEquipmentEventDelegate(this.OnPeerEquipmentUpdated);
    }

    public virtual void Clear() => this.MissionLobbyEquipmentNetworkComponent.OnEquipmentRefreshed -= new MissionLobbyEquipmentNetworkComponent.OnRefreshEquipmentEventDelegate(this.OnPeerEquipmentUpdated);

    public virtual void OnTick(float dt)
    {
      foreach (MissionPeer peer in VirtualPlayer.Peers<MissionPeer>())
      {
        if (peer.GetNetworkPeer().IsSynchronized && peer.ControlledAgent == null && (peer.HasSpawnedAgentVisuals && !this.CanUpdateSpawnEquipment(peer)))
        {
          BasicCultureObject basicCultureObject1 = MBObjectManager.Instance.GetObject<BasicCultureObject>(MultiplayerOptions.OptionType.CultureTeam1.GetStrValue());
          BasicCultureObject basicCultureObject2 = MBObjectManager.Instance.GetObject<BasicCultureObject>(MultiplayerOptions.OptionType.CultureTeam2.GetStrValue());
          MultiplayerClassDivisions.MPHeroClass heroClassForPeer = MultiplayerClassDivisions.GetMPHeroClassForPeer(peer);
          MPPerkObject.MPOnSpawnPerkHandler spawnPerkHandler = MPPerkObject.GetOnSpawnPerkHandler(peer);
          int num1 = 0;
          bool flag1 = false;
          if (MultiplayerOptions.OptionType.NumberOfBotsPerFormation.GetIntValue() > 0 && (this.GameMode.WarmupComponent == null || !this.GameMode.WarmupComponent.IsInWarmup))
          {
            num1 = MPPerkObject.GetTroopCount(heroClassForPeer, spawnPerkHandler);
            foreach (MPPerkObject selectedPerk in (IEnumerable<MPPerkObject>) peer.SelectedPerks)
            {
              if (selectedPerk.HasBannerBearer)
              {
                flag1 = true;
                break;
              }
            }
          }
          if (num1 > 0)
            num1 = (int) ((double) num1 * (double) this.GameMode.GetTroopNumberMultiplierForMissingPlayer(peer));
          int num2 = num1 + (flag1 ? 2 : 1);
          IEnumerable<(EquipmentIndex, EquipmentElement)> alternativeEquipments = spawnPerkHandler?.GetAlternativeEquipments(false);
          for (int index = 0; index < num2; ++index)
          {
            bool isPlayer = index == 0;
            BasicCharacterObject basicCharacterObject = isPlayer ? heroClassForPeer.HeroCharacter : (!flag1 || index != 1 ? heroClassForPeer.TroopCharacter : heroClassForPeer.BannerBearerCharacter);
            AgentBuildData agentBuildData = new AgentBuildData(basicCharacterObject);
            if (isPlayer)
              agentBuildData.MissionPeer(peer);
            else
              agentBuildData.OwningMissionPeer(peer);
            agentBuildData.VisualsIndex(index);
            Equipment equipment = isPlayer ? basicCharacterObject.Equipment.Clone() : Equipment.GetRandomEquipmentElements(basicCharacterObject, false, seed: MBRandom.RandomInt());
            IEnumerable<(EquipmentIndex, EquipmentElement)> valueTuples = isPlayer ? spawnPerkHandler?.GetAlternativeEquipments(true) : alternativeEquipments;
            if (valueTuples != null)
            {
              foreach ((EquipmentIndex, EquipmentElement) valueTuple in valueTuples)
                equipment[valueTuple.Item1] = valueTuple.Item2;
            }
            agentBuildData.Equipment(equipment);
            agentBuildData.Team(peer.Team);
            agentBuildData.Formation(peer.ControlledFormation);
            agentBuildData.IsFemale(isPlayer ? peer.Peer.IsFemale : basicCharacterObject.IsFemale);
            agentBuildData.TroopOrigin((IAgentOriginBase) new BasicBattleAgentOrigin(basicCharacterObject));
            BasicCultureObject basicCultureObject3 = peer.Team == this.Mission.AttackerTeam ? basicCultureObject1 : basicCultureObject2;
            if (isPlayer)
            {
              agentBuildData.BodyProperties(this.GetBodyProperties(peer, peer.Team == this.Mission.AttackerTeam ? basicCultureObject1 : basicCultureObject2));
            }
            else
            {
              agentBuildData.EquipmentSeed(this.MissionLobbyComponent.GetRandomFaceSeedForCharacter(basicCharacterObject, agentBuildData.AgentVisualsIndex));
              agentBuildData.BodyProperties(BodyProperties.GetRandomBodyProperties(agentBuildData.AgentIsFemale, basicCharacterObject.GetBodyPropertiesMin(), basicCharacterObject.GetBodyPropertiesMax(), (int) agentBuildData.AgentOverridenSpawnEquipment.HairCoverType, agentBuildData.AgentEquipmentSeed, basicCharacterObject.HairTags, basicCharacterObject.BeardTags, basicCharacterObject.TattooTags));
            }
            agentBuildData.ClothingColor1(peer.Team == this.Mission.AttackerTeam ? basicCultureObject3.Color : basicCultureObject3.ClothAlternativeColor);
            agentBuildData.ClothingColor2(peer.Team == this.Mission.AttackerTeam ? basicCultureObject3.Color2 : basicCultureObject3.ClothAlternativeColor2);
            Banner banner = new Banner(peer.Peer.BannerCode, peer.Team.Color, peer.Team.Color2);
            agentBuildData.Banner(banner);
            if (peer.ControlledFormation != null && peer.ControlledFormation.Banner == null)
              peer.ControlledFormation.Banner = banner;
            SpawnComponent spawnComponent = this.SpawnComponent;
            Team team = peer.Team;
            EquipmentElement equipmentElement = equipment[EquipmentIndex.ArmorItemEndSlot];
            int num3 = equipmentElement.Item != null ? 1 : 0;
            int num4 = peer.SpawnCountThisRound == 0 ? 1 : 0;
            MatrixFrame spawnFrame = spawnComponent.GetSpawnFrame(team, num3 != 0, num4 != 0);
            if (!spawnFrame.IsIdentity)
            {
              MatrixFrame matrixFrame = spawnFrame;
              MatrixFrame? agentInitialFrame = agentBuildData.AgentInitialFrame;
              if ((agentInitialFrame.HasValue ? (matrixFrame != agentInitialFrame.GetValueOrDefault() ? 1 : 0) : 1) != 0)
                agentBuildData.InitialFrame(spawnFrame);
            }
            if (peer.ControlledAgent != null && !isPlayer)
            {
              MatrixFrame frame = peer.ControlledAgent.Frame;
              frame.rotation.OrthonormalizeAccordingToForwardAndKeepUpAsZAxis();
              MatrixFrame matrixFrame = frame;
              matrixFrame.origin -= matrixFrame.rotation.f.NormalizedCopy() * 3.5f;
              Mat3 rotation = matrixFrame.rotation;
              rotation.MakeUnit();
              equipmentElement = basicCharacterObject.Equipment[EquipmentIndex.ArmorItemEndSlot];
              bool flag2 = !equipmentElement.IsEmpty;
              int num5 = Math.Min(num2, 10);
              List<WorldFrame> formationCreation = Formation.GetFormationFramesForBeforeFormationCreation((float) ((double) num5 * (double) Formation.GetDefaultUnitDiameter(flag2) + (double) (num5 - 1) * (double) Formation.GetDefaultMinimumInterval(flag2)), num2, flag2, new WorldPosition(Mission.Current.Scene, matrixFrame.origin), rotation);
              agentBuildData.InitialFrame(formationCreation[index - 1].ToGroundMatrixFrame());
            }
            Agent agent = this.Mission.SpawnAgent(agentBuildData, true);
            agent.AddComponent((AgentComponent) new MPPerksAgentComponent(agent));
            agent.MountAgent?.UpdateAgentProperties();
            float num6 = spawnPerkHandler != null ? spawnPerkHandler.GetHitpoints(isPlayer) : 0.0f;
            agent.HealthLimit += num6;
            agent.Health = agent.HealthLimit;
            agent.AddComponent((AgentComponent) new AgentAIStateFlagComponent(agent));
            if (!isPlayer)
              agent.SetWatchState(AgentAIStateFlagComponent.WatchState.Alarmed);
            agent.WieldInitialWeapons();
            if (isPlayer)
            {
              Action<MissionPeer> spawnedFromVisuals = this.OnPeerSpawnedFromVisuals;
              if (spawnedFromVisuals != null)
                spawnedFromVisuals(peer);
            }
          }
          ++peer.SpawnCountThisRound;
          Action<MissionPeer> spawnedFromVisuals1 = this.OnAllAgentsFromPeerSpawnedFromVisuals;
          if (spawnedFromVisuals1 != null)
            spawnedFromVisuals1(peer);
          this.AgentVisualSpawnComponent.RemoveAgentVisuals(peer, true);
          MPPerkObject.GetPerkHandler(peer)?.OnEvent(MPPerkCondition.PerkEventFlags.SpawnEnd);
        }
      }
      if (this.IsSpawningEnabled || !this.IsRoundInProgress())
        return;
      if ((double) this.SpawningDelayTimer >= (double) this.SpawningEndDelay && !this._hasCalledSpawningEnded)
      {
        Mission.Current.AllowAiTicking = true;
        if (this.OnSpawningEnded != null)
          this.OnSpawningEnded();
        this._hasCalledSpawningEnded = true;
      }
      this.SpawningDelayTimer += dt;
    }

    public bool AreAgentsSpawning() => this.IsSpawningEnabled;

    protected void ResetSpawnCounts()
    {
      foreach (NetworkCommunicator networkPeer in GameNetwork.NetworkPeers)
      {
        MissionPeer component = networkPeer.GetComponent<MissionPeer>();
        if (component != null)
          component.SpawnCountThisRound = 0;
      }
    }

    protected void ResetSpawnTimers()
    {
      foreach (NetworkCommunicator networkPeer in GameNetwork.NetworkPeers)
        networkPeer.GetComponent<MissionPeer>()?.SpawnTimer.Reset(MBCommon.GetTime(MBCommon.TimeType.Mission), 0.0f);
    }

    public virtual void RequestStartSpawnSession()
    {
      this.IsSpawningEnabled = true;
      this.SpawningDelayTimer = 0.0f;
      this._hasCalledSpawningEnded = false;
      this.ResetSpawnCounts();
    }

    public virtual void RequestStopSpawnSession()
    {
      this.IsSpawningEnabled = false;
      this.SpawningDelayTimer = 0.0f;
      this._hasCalledSpawningEnded = false;
      foreach (MissionPeer peer in VirtualPlayer.Peers<MissionPeer>())
        this.AgentVisualSpawnComponent.RemoveAgentVisuals(peer, true);
    }

    protected abstract void SpawnAgents();

    protected BodyProperties GetBodyProperties(
      MissionPeer missionPeer,
      BasicCultureObject cultureLimit)
    {
      NetworkCommunicator networkPeer = missionPeer.GetNetworkPeer();
      if (networkPeer != null)
        return networkPeer.PlayerConnectionInfo.GetParameter<PlayerData>("PlayerData").BodyProperties;
      Team team = missionPeer.Team;
      BasicCharacterObject troopCharacter = MultiplayerClassDivisions.GetMPHeroClasses(cultureLimit).ToList<MultiplayerClassDivisions.MPHeroClass>().GetRandomElement<MultiplayerClassDivisions.MPHeroClass>().TroopCharacter;
      MatrixFrame spawnFrame = this.SpawnComponent.GetSpawnFrame(team, troopCharacter.HasMount(), true);
      AgentBuildData agentBuildData = new AgentBuildData(troopCharacter);
      agentBuildData.Team(team);
      agentBuildData.InitialFrame(spawnFrame);
      agentBuildData.TroopOrigin((IAgentOriginBase) new BasicBattleAgentOrigin(troopCharacter));
      agentBuildData.EquipmentSeed(this.MissionLobbyComponent.GetRandomFaceSeedForCharacter(troopCharacter));
      agentBuildData.ClothingColor1(team.Side == BattleSideEnum.Attacker ? cultureLimit.Color : cultureLimit.ClothAlternativeColor);
      agentBuildData.ClothingColor2(team.Side == BattleSideEnum.Attacker ? cultureLimit.Color2 : cultureLimit.ClothAlternativeColor2);
      agentBuildData.IsFemale(troopCharacter.IsFemale);
      agentBuildData.Equipment(Equipment.GetRandomEquipmentElements(troopCharacter, !(Game.Current.GameType is MultiplayerGame), seed: agentBuildData.AgentEquipmentSeed));
      agentBuildData.BodyProperties(BodyProperties.GetRandomBodyProperties(agentBuildData.AgentIsFemale, troopCharacter.GetBodyPropertiesMin(), troopCharacter.GetBodyPropertiesMax(), (int) agentBuildData.AgentOverridenSpawnEquipment.HairCoverType, agentBuildData.AgentEquipmentSeed, troopCharacter.HairTags, troopCharacter.BeardTags, troopCharacter.TattooTags));
      return agentBuildData.AgentBodyProperties;
    }

    protected void SpawnBot(Team agentTeam, BasicCultureObject cultureLimit)
    {
      BasicCharacterObject troopCharacter = MultiplayerClassDivisions.GetMPHeroClasses(cultureLimit).ToList<MultiplayerClassDivisions.MPHeroClass>().GetRandomElement<MultiplayerClassDivisions.MPHeroClass>().TroopCharacter;
      MatrixFrame spawnFrame = this.SpawnComponent.GetSpawnFrame(agentTeam, troopCharacter.HasMount(), true);
      AgentBuildData agentBuildData = new AgentBuildData(troopCharacter);
      agentBuildData.Team(agentTeam);
      agentBuildData.InitialFrame(spawnFrame);
      agentBuildData.TroopOrigin((IAgentOriginBase) new BasicBattleAgentOrigin(troopCharacter));
      agentBuildData.EquipmentSeed(this.MissionLobbyComponent.GetRandomFaceSeedForCharacter(troopCharacter));
      agentBuildData.ClothingColor1(agentTeam.Side == BattleSideEnum.Attacker ? cultureLimit.Color : cultureLimit.ClothAlternativeColor);
      agentBuildData.ClothingColor2(agentTeam.Side == BattleSideEnum.Attacker ? cultureLimit.Color2 : cultureLimit.ClothAlternativeColor2);
      agentBuildData.IsFemale(troopCharacter.IsFemale);
      agentBuildData.Equipment(Equipment.GetRandomEquipmentElements(troopCharacter, !(Game.Current.GameType is MultiplayerGame), seed: agentBuildData.AgentEquipmentSeed));
      agentBuildData.BodyProperties(BodyProperties.GetRandomBodyProperties(agentBuildData.AgentIsFemale, troopCharacter.GetBodyPropertiesMin(), troopCharacter.GetBodyPropertiesMax(), (int) agentBuildData.AgentOverridenSpawnEquipment.HairCoverType, agentBuildData.AgentEquipmentSeed, troopCharacter.HairTags, troopCharacter.BeardTags, troopCharacter.TattooTags));
      Agent agent = this.Mission.SpawnAgent(agentBuildData);
      agent.AddComponent((AgentComponent) new AgentAIStateFlagComponent(agent));
      agent.SetWatchState(AgentAIStateFlagComponent.WatchState.Alarmed);
    }

    private void OnPeerEquipmentUpdated(MissionPeer peer)
    {
      if (!this.IsSpawningEnabled || !this.CanUpdateSpawnEquipment(peer))
        return;
      peer.HasSpawnedAgentVisuals = false;
      Debug.Print("HasSpawnedAgentVisuals = false for peer: " + peer.Name + " because he just updated his equipment");
      if (peer.ControlledFormation == null)
        return;
      peer.ControlledFormation.HasBeenPositioned = false;
      peer.ControlledFormation.GroupSpawnIndex = 0;
    }

    public virtual bool CanUpdateSpawnEquipment(MissionPeer missionPeer) => !missionPeer.EquipmentUpdatingExpired && !this._equipmentUpdatingExpired;

    public void ToggleUpdatingSpawnEquipment(bool canUpdate) => this._equipmentUpdatingExpired = !canUpdate;

    public abstract bool AllowEarlyAgentVisualsDespawning(MissionPeer missionPeer);

    public virtual int GetMaximumReSpawnPeriodForPeer(MissionPeer peer) => 3;

    protected abstract bool IsRoundInProgress();

    public virtual void OnClearScene()
    {
    }

    public delegate void OnSpawningEndedEventDelegate();
  }
}
