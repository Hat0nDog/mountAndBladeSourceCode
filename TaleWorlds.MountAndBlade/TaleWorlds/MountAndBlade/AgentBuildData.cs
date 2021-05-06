// Decompiled with JetBrains decompiler
// Type: TaleWorlds.MountAndBlade.AgentBuildData
// Assembly: TaleWorlds.MountAndBlade, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3C0E3F42-756F-4407-B817-06BA412A78B0
// Assembly location: E:\SteamLibrary\steamapps\common\Mount & Blade II Bannerlord\bin\Win64_Shipping_Client\TaleWorlds.MountAndBlade.dll

using TaleWorlds.Core;
using TaleWorlds.Engine;
using TaleWorlds.Library;

namespace TaleWorlds.MountAndBlade
{
  public class AgentBuildData
  {
    public AgentData AgentData { get; private set; }

    public BasicCharacterObject AgentCharacter => this.AgentData.AgentCharacter;

    public TaleWorlds.Core.Monster AgentMonster => this.AgentData.AgentMonster;

    public TaleWorlds.Core.Equipment AgentOverridenSpawnEquipment => this.AgentData.AgentOverridenEquipment;

    public TaleWorlds.MountAndBlade.MissionEquipment AgentOverridenSpawnMissionEquipment { get; private set; }

    public int AgentEquipmentSeed => this.AgentData.AgentEquipmentSeed;

    public bool AgentNoHorses => this.AgentData.AgentNoHorses;

    public string AgentMountKey => this.AgentData.AgentMountKey;

    public bool AgentNoWeapons => this.AgentData.AgentNoWeapons;

    public bool AgentNoArmor => this.AgentData.AgentNoArmor;

    public bool AgentFixedEquipment => this.AgentData.AgentFixedEquipment;

    public bool AgentCivilianEquipment => this.AgentData.AgentCivilianEquipment;

    public uint AgentClothingColor1 => this.AgentData.AgentClothingColor1;

    public uint AgentClothingColor2 => this.AgentData.AgentClothingColor2;

    public bool BodyPropertiesOverriden => this.AgentData.BodyPropertiesOverriden;

    public TaleWorlds.Core.BodyProperties AgentBodyProperties => this.AgentData.AgentBodyProperties;

    public bool AgeOverriden => this.AgentData.AgeOverriden;

    public int AgentAge => this.AgentData.AgentAge;

    public bool GenderOverriden => this.AgentData.GenderOverriden;

    public bool AgentIsFemale => this.AgentData.AgentIsFemale;

    public IAgentOriginBase AgentOrigin => this.AgentData.AgentOrigin;

    public Agent.ControllerType AgentController { get; private set; }

    public TaleWorlds.MountAndBlade.Team AgentTeam { get; private set; }

    public bool AgentIsReinforcement { get; private set; }

    public bool EnforceSpawningOnInitialPoint { get; private set; }

    public float MakeUnitStandOutDistance { get; private set; }

    public MatrixFrame? AgentInitialFrame { get; private set; }

    public TaleWorlds.MountAndBlade.Formation AgentFormation { get; private set; }

    public int AgentFormationTroopCount { get; private set; }

    public int AgentFormationTroopIndex { get; private set; }

    public TaleWorlds.MountAndBlade.MissionPeer AgentMissionPeer { get; private set; }

    public TaleWorlds.MountAndBlade.MissionPeer OwningAgentMissionPeer { get; private set; }

    public bool AgentIndexOverriden { get; private set; }

    public int AgentIndex { get; private set; }

    public bool AgentMountIndexOverriden { get; private set; }

    public int AgentMountIndex { get; private set; }

    public int AgentVisualsIndex { get; private set; }

    public TaleWorlds.Core.Banner AgentBanner { get; private set; }

    public bool RandomizeColors => this.AgentCharacter != null && !this.AgentCharacter.IsHero && this.AgentMissionPeer == null;

    private AgentBuildData()
    {
      this.AgentController = Agent.ControllerType.AI;
      this.AgentTeam = TaleWorlds.MountAndBlade.Team.Invalid;
      this.AgentFormation = (TaleWorlds.MountAndBlade.Formation) null;
      this.AgentMissionPeer = (TaleWorlds.MountAndBlade.MissionPeer) null;
      this.AgentFormationTroopIndex = -1;
    }

    public AgentBuildData(AgentData agentData)
      : this()
    {
      this.AgentData = agentData;
    }

    public AgentBuildData(IAgentOriginBase agentOrigin)
      : this()
    {
      this.AgentData = new AgentData(agentOrigin);
    }

    public AgentBuildData(BasicCharacterObject characterObject)
      : this()
    {
      this.AgentData = new AgentData(characterObject);
    }

    public AgentBuildData Character(BasicCharacterObject characterObject)
    {
      this.AgentData.Character(characterObject);
      return this;
    }

    public AgentBuildData Controller(Agent.ControllerType controller)
    {
      this.AgentController = controller;
      return this;
    }

    public AgentBuildData Team(TaleWorlds.MountAndBlade.Team team)
    {
      this.AgentTeam = team;
      return this;
    }

    public AgentBuildData IsReinforcement(bool isReinforcement)
    {
      this.AgentIsReinforcement = isReinforcement;
      return this;
    }

    public AgentBuildData SpawnOnInitialPoint(bool spawnOnInitialPoint)
    {
      this.EnforceSpawningOnInitialPoint = spawnOnInitialPoint;
      return this;
    }

    public AgentBuildData MakeUnitStandOutOfFormationDistance(
      float makeUnitStandOutDistance)
    {
      this.MakeUnitStandOutDistance = makeUnitStandOutDistance;
      return this;
    }

    public AgentBuildData InitialFrame(MatrixFrame frame)
    {
      this.AgentInitialFrame = new MatrixFrame?(frame);
      return this;
    }

    public AgentBuildData InitialFrameFromSpawnPointEntity(GameEntity entity)
    {
      MatrixFrame globalFrame = entity.GetGlobalFrame();
      globalFrame.rotation.OrthonormalizeAccordingToForwardAndKeepUpAsZAxis();
      this.AgentInitialFrame = new MatrixFrame?(globalFrame);
      return this;
    }

    public AgentBuildData Formation(TaleWorlds.MountAndBlade.Formation formation)
    {
      this.AgentFormation = formation;
      return this;
    }

    public AgentBuildData Monster(TaleWorlds.Core.Monster monster)
    {
      this.AgentData.Monster(monster);
      return this;
    }

    public AgentBuildData VisualsIndex(int index)
    {
      this.AgentVisualsIndex = index;
      return this;
    }

    public AgentBuildData Equipment(TaleWorlds.Core.Equipment equipment)
    {
      this.AgentData.Equipment(equipment);
      return this;
    }

    public AgentBuildData MissionEquipment(TaleWorlds.MountAndBlade.MissionEquipment missionEquipment)
    {
      this.AgentOverridenSpawnMissionEquipment = missionEquipment;
      return this;
    }

    public AgentBuildData EquipmentSeed(int seed)
    {
      this.AgentData.EquipmentSeed(seed);
      return this;
    }

    public AgentBuildData NoHorses(bool noHorses)
    {
      this.AgentData.NoHorses(noHorses);
      return this;
    }

    public AgentBuildData NoWeapons(bool noWeapons)
    {
      this.AgentData.NoWeapons(noWeapons);
      return this;
    }

    public AgentBuildData NoArmor(bool noArmor)
    {
      this.AgentData.NoArmor(noArmor);
      return this;
    }

    public AgentBuildData FixedEquipment(bool fixedEquipment)
    {
      this.AgentData.FixedEquipment(fixedEquipment);
      return this;
    }

    public AgentBuildData CivilianEquipment(bool civilianEquipment)
    {
      this.AgentData.CivilianEquipment(civilianEquipment);
      return this;
    }

    public AgentBuildData ClothingColor1(uint color)
    {
      this.AgentData.ClothingColor1(color);
      return this;
    }

    public AgentBuildData ClothingColor2(uint color)
    {
      this.AgentData.ClothingColor2(color);
      return this;
    }

    public AgentBuildData MissionPeer(TaleWorlds.MountAndBlade.MissionPeer missionPeer)
    {
      this.AgentMissionPeer = missionPeer;
      return this;
    }

    public AgentBuildData OwningMissionPeer(TaleWorlds.MountAndBlade.MissionPeer missionPeer)
    {
      this.OwningAgentMissionPeer = missionPeer;
      return this;
    }

    public AgentBuildData BodyProperties(TaleWorlds.Core.BodyProperties bodyProperties)
    {
      this.AgentData.BodyProperties(bodyProperties);
      return this;
    }

    public AgentBuildData Age(int age)
    {
      this.AgentData.Age(age);
      return this;
    }

    public AgentBuildData TroopOrigin(IAgentOriginBase troopOrigin)
    {
      this.AgentData.TroopOrigin(troopOrigin);
      return this;
    }

    public AgentBuildData IsFemale(bool isFemale)
    {
      this.AgentData.IsFemale(isFemale);
      return this;
    }

    public AgentBuildData MountKey(string mountKey)
    {
      this.AgentData.MountKey(mountKey);
      return this;
    }

    public AgentBuildData Index(int index)
    {
      this.AgentIndex = index;
      this.AgentIndexOverriden = true;
      return this;
    }

    public AgentBuildData MountIndex(int mountIndex)
    {
      this.AgentMountIndex = mountIndex;
      this.AgentMountIndexOverriden = true;
      return this;
    }

    public AgentBuildData Banner(TaleWorlds.Core.Banner banner)
    {
      this.AgentBanner = banner;
      return this;
    }

    public AgentBuildData FormationTroopCount(int formationTroopCount)
    {
      this.AgentFormationTroopCount = formationTroopCount;
      return this;
    }

    public AgentBuildData FormationTroopIndex(int formationTroopIndex)
    {
      this.AgentFormationTroopIndex = formationTroopIndex;
      return this;
    }
  }
}
