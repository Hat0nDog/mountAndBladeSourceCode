// Decompiled with JetBrains decompiler
// Type: TaleWorlds.Core.AgentData
// Assembly: TaleWorlds.Core, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 43F7B45C-9AE1-4715-A37B-2364840F62AF
// Assembly location: E:\SteamLibrary\steamapps\common\Mount & Blade II Bannerlord\bin\Win64_Shipping_Client\TaleWorlds.Core.dll

using System.Collections.Generic;
using TaleWorlds.ObjectSystem;
using TaleWorlds.SaveSystem;

namespace TaleWorlds.Core
{
  [SaveableClass(10000)]
  public class AgentData
  {
    private TaleWorlds.Core.Monster _agentMonster;
    private string _agentMonsterId;

    public BasicCharacterObject AgentCharacter { get; private set; }

    public TaleWorlds.Core.Monster AgentMonster
    {
      get => this._agentMonster ?? (this._agentMonster = MBObjectManager.Instance.GetObject<TaleWorlds.Core.Monster>(this._agentMonsterId));
      private set
      {
        this._agentMonster = (TaleWorlds.Core.Monster) null;
        this._agentMonsterId = value.StringId;
      }
    }

    public IBattleCombatant AgentOwnerParty { get; private set; }

    public TaleWorlds.Core.Equipment AgentOverridenEquipment { get; private set; }

    public int AgentEquipmentSeed { get; private set; }

    public bool AgentNoHorses { get; private set; }

    public string AgentMountKey { get; private set; }

    public bool AgentNoWeapons { get; private set; }

    public bool AgentNoArmor { get; private set; }

    public bool AgentFixedEquipment { get; private set; }

    public bool AgentCivilianEquipment { get; private set; }

    public uint AgentClothingColor1 { get; private set; }

    public uint AgentClothingColor2 { get; private set; }

    public bool BodyPropertiesOverriden { get; private set; }

    public TaleWorlds.Core.BodyProperties AgentBodyProperties { get; private set; }

    public bool AgeOverriden { get; private set; }

    public int AgentAge { get; private set; }

    public bool GenderOverriden { get; private set; }

    public bool AgentIsFemale { get; private set; }

    public IAgentOriginBase AgentOrigin { get; private set; }

    public AgentData(IAgentOriginBase agentOrigin)
      : this(agentOrigin.Troop)
    {
      this.AgentOrigin = agentOrigin;
      this.AgentCharacter = agentOrigin.Troop;
      this.AgentEquipmentSeed = agentOrigin.Seed;
    }

    public AgentData(BasicCharacterObject characterObject)
    {
      this.AgentCharacter = characterObject;
      this.AgentMonster = Game.Current.HumanMonster;
      this.AgentOwnerParty = (IBattleCombatant) null;
      this.AgentOverridenEquipment = (TaleWorlds.Core.Equipment) null;
      this.AgentEquipmentSeed = 0;
      this.AgentNoHorses = false;
      this.AgentNoWeapons = false;
      this.AgentNoArmor = false;
      this.AgentFixedEquipment = false;
      this.AgentCivilianEquipment = false;
      this.AgentClothingColor1 = uint.MaxValue;
      this.AgentClothingColor2 = uint.MaxValue;
      this.BodyPropertiesOverriden = false;
      this.GenderOverriden = false;
    }

    public AgentData Character(BasicCharacterObject characterObject)
    {
      this.AgentCharacter = characterObject;
      return this;
    }

    public AgentData Monster(TaleWorlds.Core.Monster monster)
    {
      this.AgentMonster = monster;
      return this;
    }

    public AgentData OwnerParty(IBattleCombatant owner)
    {
      this.AgentOwnerParty = owner;
      return this;
    }

    public AgentData Equipment(TaleWorlds.Core.Equipment equipment)
    {
      this.AgentOverridenEquipment = equipment;
      return this;
    }

    public AgentData EquipmentSeed(int seed)
    {
      this.AgentEquipmentSeed = seed;
      return this;
    }

    public AgentData NoHorses(bool noHorses)
    {
      this.AgentNoHorses = noHorses;
      return this;
    }

    public AgentData NoWeapons(bool noWeapons)
    {
      this.AgentNoWeapons = noWeapons;
      return this;
    }

    public AgentData NoArmor(bool noArmor)
    {
      this.AgentNoArmor = noArmor;
      return this;
    }

    public AgentData FixedEquipment(bool fixedEquipment)
    {
      this.AgentFixedEquipment = fixedEquipment;
      return this;
    }

    public AgentData CivilianEquipment(bool civilianEquipment)
    {
      this.AgentCivilianEquipment = civilianEquipment;
      return this;
    }

    public AgentData ClothingColor1(uint color)
    {
      this.AgentClothingColor1 = color;
      return this;
    }

    public AgentData ClothingColor2(uint color)
    {
      this.AgentClothingColor2 = color;
      return this;
    }

    public AgentData BodyProperties(TaleWorlds.Core.BodyProperties bodyProperties)
    {
      this.AgentBodyProperties = bodyProperties;
      this.BodyPropertiesOverriden = true;
      return this;
    }

    public AgentData Age(int age)
    {
      this.AgentAge = age;
      this.AgeOverriden = true;
      return this;
    }

    public AgentData TroopOrigin(IAgentOriginBase troopOrigin)
    {
      this.AgentOrigin = troopOrigin;
      if (troopOrigin?.Troop != null && !troopOrigin.Troop.IsHero)
        this.EquipmentSeed(troopOrigin.Seed);
      return this;
    }

    public AgentData IsFemale(bool isFemale)
    {
      this.AgentIsFemale = isFemale;
      this.GenderOverriden = true;
      return this;
    }

    public AgentData MountKey(string mountKey)
    {
      this.AgentMountKey = mountKey;
      return this;
    }

    internal static void AutoGeneratedStaticCollectObjectsAgentData(
      object o,
      List<object> collectedObjects)
    {
      ((AgentData) o).AutoGeneratedInstanceCollectObjects(collectedObjects);
    }

    protected virtual void AutoGeneratedInstanceCollectObjects(List<object> collectedObjects)
    {
    }
  }
}
