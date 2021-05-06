// Decompiled with JetBrains decompiler
// Type: TaleWorlds.Core.SaveableCoreTypeDefiner
// Assembly: TaleWorlds.Core, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 43F7B45C-9AE1-4715-A37B-2364840F62AF
// Assembly location: E:\SteamLibrary\steamapps\common\Mount & Blade II Bannerlord\bin\Win64_Shipping_Client\TaleWorlds.Core.dll

using System.Collections.Generic;
using TaleWorlds.ObjectSystem;
using TaleWorlds.SaveSystem;

namespace TaleWorlds.Core
{
  public class SaveableCoreTypeDefiner : SaveableTypeDefiner
  {
    public SaveableCoreTypeDefiner()
      : base(10000)
    {
    }

    protected override void DefineBasicTypes() => base.DefineBasicTypes();

    protected override void DefineClassTypes()
    {
      this.AddClassDefinition(typeof (AgentData), 1);
      this.AddClassDefinition(typeof (ArmorComponent), 2);
      this.AddClassDefinition(typeof (Banner), 3);
      this.AddClassDefinition(typeof (BannerData), 4);
      this.AddClassDefinition(typeof (BasicCharacterObject), 5);
      this.AddClassDefinition(typeof (CharacterAttribute), 6);
      this.AddClassDefinition(typeof (CharacterAttributes), 7);
      this.AddClassDefinition(typeof (CharacterSkills), 8);
      this.AddClassDefinition(typeof (WeaponDesign), 9);
      this.AddClassDefinition(typeof (CraftingPiece), 10);
      this.AddClassDefinition(typeof (CraftingTemplate), 11);
      this.AddClassDefinition(typeof (DefaultItemCategories), 12);
      this.AddClassDefinition(typeof (DefaultSiegeEngineTypes), 13);
      this.AddClassDefinition(typeof (DefaultSkills), 14);
      this.AddClassDefinition(typeof (EntitySystem<>), 15);
      this.AddClassDefinition(typeof (Equipment), 16);
      this.AddClassDefinition(typeof (EventManager<>), 17);
      this.AddClassDefinition(typeof (TradeItemComponent), 18);
      this.AddClassDefinition(typeof (GameLogs), 22);
      this.AddClassDefinition(typeof (GameText), 24);
      this.AddClassDefinition(typeof (GameTextManager), 25);
      this.AddClassDefinition(typeof (GameType), 26);
      this.AddClassDefinition(typeof (HorseComponent), 27);
      this.AddClassDefinition(typeof (ItemCategory), 28);
      this.AddClassDefinition(typeof (ItemComponent), 29);
      this.AddClassDefinition(typeof (ItemModifier), 30);
      this.AddClassDefinition(typeof (ItemModifierGroup), 31);
      this.AddClassDefinition(typeof (ItemObject), 32);
      this.AddClassDefinition(typeof (ItemObjectProperties), 33);
      this.AddClassDefinition(typeof (MissionResult), 36);
      this.AddClassDefinition(typeof (PlayerGameState), 37);
      this.AddClassDefinition(typeof (PropertyObject), 38);
      this.AddClassDefinition(typeof (SkillObject), 39);
      this.AddClassDefinition(typeof (PropertyOwner<>), 40);
      this.AddClassDefinition(typeof (PropertyOwnerF<>), 41);
      this.AddClassDefinition(typeof (SiegeEngineType), 42);
      this.AddClassDefinition(typeof (Timer), 43);
      this.AddClassDefinition(typeof (WeaponDesignElement), 44);
      this.AddClassDefinition(typeof (WeaponComponent), 45);
      this.AddClassDefinition(typeof (WeaponComponentData), 46);
      this.AddClassDefinition(typeof (EventManager<>.EventRegistry), 47);
      this.AddClassDefinition(typeof (DeterministicRandom), 49);
      this.AddClassDefinition(typeof (InformationData), 50);
      this.AddClassDefinition(typeof (Crafting.OverrideData), 51);
    }

    protected override void DefineStructTypes()
    {
      this.AddStructDefinition(typeof (BodyProperties), 1001);
      this.AddStructDefinition(typeof (PieceData), 1002);
      this.AddStructDefinition(typeof (WeaponUsageData), 1003);
      this.AddStructDefinition(typeof (ItemRosterElement), 1004);
      this.AddStructDefinition(typeof (UniqueTroopDescriptor), 1006);
      this.AddStructDefinition(typeof (GameText.GameTextVariation), 1007);
      this.AddStructDefinition(typeof (HorseComponent.MaterialProperty), 1008);
      this.AddStructDefinition(typeof (StaticBodyProperties), 1009);
      this.AddStructDefinition(typeof (DynamicBodyProperties), 1010);
      this.AddStructDefinition(typeof (EquipmentElement), 1011);
    }

    protected override void DefineEnumTypes()
    {
      this.AddEnumDefinition(typeof (BattleSideEnum), 2001);
      this.AddEnumDefinition(typeof (BattleAllegianceEnum), 2002);
      this.AddEnumDefinition(typeof (SkillObject.SkillTypeEnum), 2004);
      this.AddEnumDefinition(typeof (Equipment.EquipmentType), 2006);
      this.AddEnumDefinition(typeof (WeaponFlags), 2007);
      this.AddEnumDefinition(typeof (FormationClass), 2008);
      this.AddEnumDefinition(typeof (BattleState), 2009);
    }

    protected override void DefineInterfaceTypes()
    {
    }

    protected override void DefineRootClassTypes() => this.AddRootClassDefinition(typeof (Game), 4001);

    protected override void DefineGenericClassDefinitions()
    {
      this.ConstructGenericClassDefinition(typeof (ObsoleteObjectManager.ObjectTypeRecord<ItemObject>));
      this.ConstructGenericClassDefinition(typeof (ObsoleteObjectManager.ObjectTypeRecord<ItemComponent>));
      this.ConstructGenericClassDefinition(typeof (ObsoleteObjectManager.ObjectTypeRecord<ItemModifier>));
      this.ConstructGenericClassDefinition(typeof (ObsoleteObjectManager.ObjectTypeRecord<ItemModifierGroup>));
      this.ConstructGenericClassDefinition(typeof (ObsoleteObjectManager.ObjectTypeRecord<CharacterAttribute>));
      this.ConstructGenericClassDefinition(typeof (ObsoleteObjectManager.ObjectTypeRecord<SkillObject>));
      this.ConstructGenericClassDefinition(typeof (ObsoleteObjectManager.ObjectTypeRecord<ItemCategory>));
      this.ConstructGenericClassDefinition(typeof (ObsoleteObjectManager.ObjectTypeRecord<CraftingPiece>));
      this.ConstructGenericClassDefinition(typeof (ObsoleteObjectManager.ObjectTypeRecord<CraftingTemplate>));
      this.ConstructGenericClassDefinition(typeof (ObsoleteObjectManager.ObjectTypeRecord<SiegeEngineType>));
      this.ConstructGenericClassDefinition(typeof (ObsoleteObjectManager.ObjectTypeRecord<PropertyObject>));
    }

    protected override void DefineGenericStructDefinitions()
    {
    }

    protected override void DefineContainerDefinitions()
    {
      this.ConstructContainerDefinition(typeof (ItemRosterElement[]));
      this.ConstructContainerDefinition(typeof (EquipmentElement[]));
      this.ConstructContainerDefinition(typeof (Equipment[]));
      this.ConstructContainerDefinition(typeof (WeaponDesignElement[]));
      this.ConstructContainerDefinition(typeof (List<ItemObject>));
      this.ConstructContainerDefinition(typeof (List<ItemComponent>));
      this.ConstructContainerDefinition(typeof (List<ItemModifier>));
      this.ConstructContainerDefinition(typeof (List<ItemModifierGroup>));
      this.ConstructContainerDefinition(typeof (List<CharacterAttribute>));
      this.ConstructContainerDefinition(typeof (List<SkillObject>));
      this.ConstructContainerDefinition(typeof (List<ItemCategory>));
      this.ConstructContainerDefinition(typeof (List<CraftingPiece>));
      this.ConstructContainerDefinition(typeof (List<CraftingTemplate>));
      this.ConstructContainerDefinition(typeof (List<SiegeEngineType>));
      this.ConstructContainerDefinition(typeof (List<PropertyObject>));
      this.ConstructContainerDefinition(typeof (List<UniqueTroopDescriptor>));
      this.ConstructContainerDefinition(typeof (List<Equipment>));
      this.ConstructContainerDefinition(typeof (List<BannerData>));
      this.ConstructContainerDefinition(typeof (List<EquipmentElement>));
      this.ConstructContainerDefinition(typeof (Dictionary<string, ItemCategory>));
      this.ConstructContainerDefinition(typeof (Dictionary<string, CraftingPiece>));
      this.ConstructContainerDefinition(typeof (Dictionary<string, CraftingTemplate>));
      this.ConstructContainerDefinition(typeof (Dictionary<string, SiegeEngineType>));
      this.ConstructContainerDefinition(typeof (Dictionary<string, PropertyObject>));
      this.ConstructContainerDefinition(typeof (Dictionary<string, SkillObject>));
      this.ConstructContainerDefinition(typeof (Dictionary<string, CharacterAttribute>));
      this.ConstructContainerDefinition(typeof (Dictionary<string, ItemModifierGroup>));
      this.ConstructContainerDefinition(typeof (Dictionary<string, ItemComponent>));
      this.ConstructContainerDefinition(typeof (Dictionary<string, ItemObject>));
      this.ConstructContainerDefinition(typeof (Dictionary<string, ItemModifier>));
      this.ConstructContainerDefinition(typeof (Dictionary<MBGUID, ItemCategory>));
      this.ConstructContainerDefinition(typeof (Dictionary<MBGUID, CraftingPiece>));
      this.ConstructContainerDefinition(typeof (Dictionary<MBGUID, CraftingTemplate>));
      this.ConstructContainerDefinition(typeof (Dictionary<MBGUID, SiegeEngineType>));
      this.ConstructContainerDefinition(typeof (Dictionary<MBGUID, PropertyObject>));
      this.ConstructContainerDefinition(typeof (Dictionary<MBGUID, SkillObject>));
      this.ConstructContainerDefinition(typeof (Dictionary<MBGUID, CharacterAttribute>));
      this.ConstructContainerDefinition(typeof (Dictionary<MBGUID, ItemModifierGroup>));
      this.ConstructContainerDefinition(typeof (Dictionary<MBGUID, ItemObject>));
      this.ConstructContainerDefinition(typeof (Dictionary<MBGUID, ItemComponent>));
      this.ConstructContainerDefinition(typeof (Dictionary<MBGUID, ItemModifier>));
      this.ConstructContainerDefinition(typeof (Dictionary<ItemCategory, float>));
      this.ConstructContainerDefinition(typeof (Dictionary<ItemCategory, int>));
      this.ConstructContainerDefinition(typeof (Dictionary<SiegeEngineType, int>));
      this.ConstructContainerDefinition(typeof (Dictionary<SkillObject, int>));
      this.ConstructContainerDefinition(typeof (Dictionary<PropertyObject, int>));
      this.ConstructContainerDefinition(typeof (Dictionary<PropertyObject, float>));
      this.ConstructContainerDefinition(typeof (Dictionary<ItemObject, int>));
    }
  }
}
