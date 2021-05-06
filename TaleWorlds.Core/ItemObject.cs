﻿// Decompiled with JetBrains decompiler
// Type: TaleWorlds.Core.ItemObject
// Assembly: TaleWorlds.Core, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 43F7B45C-9AE1-4715-A37B-2364840F62AF
// Assembly location: E:\SteamLibrary\steamapps\common\Mount & Blade II Bannerlord\bin\Win64_Shipping_Client\TaleWorlds.Core.dll

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Xml;
using TaleWorlds.Library;
using TaleWorlds.Localization;
using TaleWorlds.ObjectSystem;
using TaleWorlds.SaveSystem;

namespace TaleWorlds.Core
{
  [SaveableClass(10024)]
  public sealed class ItemObject : MBObjectBase
  {
    public const float DefaultAppearanceValue = 0.5f;
    public const int MaxHolsterSlotCount = 4;
    public ItemObject.ItemTypeEnum Type;

    internal static void AutoGeneratedStaticCollectObjectsItemObject(
      object o,
      List<object> collectedObjects)
    {
      ((MBObjectBase) o).AutoGeneratedInstanceCollectObjects(collectedObjects);
    }

    protected override void AutoGeneratedInstanceCollectObjects(List<object> collectedObjects) => base.AutoGeneratedInstanceCollectObjects(collectedObjects);

    public ItemComponent ItemComponent { get; private set; }

    public string MultiMeshName { get; private set; }

    public string HolsterMeshName { get; private set; }

    public string HolsterWithWeaponMeshName { get; private set; }

    public string[] ItemHolsters { get; private set; } = new string[4];

    public Vec3 HolsterPositionShift { get; private set; }

    public bool HasLowerHolsterPriority { get; private set; }

    public string FlyingMeshName { get; private set; }

    public string BodyName { get; private set; }

    public string HolsterBodyName { get; private set; }

    public string CollisionBodyName { get; private set; }

    public bool RecalculateBody { get; private set; }

    public string PrefabName { get; private set; }

    public TextObject Name { get; private set; }

    public ItemFlags ItemFlags { get; private set; }

    public ItemCategory ItemCategory { get; private set; }

    public int Value { get; internal set; }

    public float Effectiveness { get; private set; }

    public float Weight { get; private set; }

    public int Difficulty { get; private set; }

    public float Appearance { get; private set; }

    public bool IsUsingTableau { get; private set; }

    public bool IsUsingTeamColor => this.ItemFlags.HasAnyFlag<ItemFlags>(ItemFlags.UseTeamColor);

    public bool DoesNotHideChest => this.ItemFlags.HasAnyFlag<ItemFlags>(ItemFlags.DoesNotHideChest);

    public bool IsCivilian => this.ItemFlags.HasAnyFlag<ItemFlags>(ItemFlags.Civilian);

    public bool UsingFacegenScaling => this.Type == ItemObject.ItemTypeEnum.HeadArmor;

    public string ArmBandMeshName { get; private set; }

    public bool IsFood { get; private set; }

    public bool IsUniqueItem { get; private set; }

    public float ScaleFactor { get; private set; }

    public BasicCultureObject Culture { get; private set; }

    public bool MultiplayerItem { get; private set; }

    public bool NotMerchandise { get; private set; }

    public bool IsCraftedByPlayer { get; private set; }

    public int LodAtlasIndex { get; private set; }

    public bool IsCraftedWeapon => this.WeaponDesign != (WeaponDesign) null;

    public WeaponDesign WeaponDesign { get; private set; }

    public WeaponComponentData PrimaryWeapon => this.WeaponComponent?.PrimaryWeapon;

    public WeaponComponent WeaponComponent => this.ItemComponent as WeaponComponent;

    public HorseComponent HorseComponent => this.ItemComponent as HorseComponent;

    public bool HasHorseComponent => this.HorseComponent != null;

    public ArmorComponent ArmorComponent => this.ItemComponent as ArmorComponent;

    public bool HasArmorComponent => this.ArmorComponent != null;

    public SaddleComponent SaddleComponent => this.ItemComponent as SaddleComponent;

    public bool HasSaddleComponent => this.SaddleComponent != null;

    public TradeItemComponent FoodComponent => this.ItemComponent as TradeItemComponent;

    public bool HasFoodComponent => this.FoodComponent != null;

    public ItemObject()
    {
    }

    public ItemObject(string stringId)
      : base(stringId)
    {
    }

    public ItemObject(ItemObject itemToCopy)
      : base((MBObjectBase) itemToCopy)
    {
      this.ItemComponent = itemToCopy.ItemComponent;
      this.MultiMeshName = itemToCopy.MultiMeshName;
      this.HolsterMeshName = itemToCopy.HolsterMeshName;
      this.HolsterWithWeaponMeshName = itemToCopy.HolsterWithWeaponMeshName;
      this.ItemHolsters = itemToCopy.ItemHolsters;
      this.HolsterPositionShift = itemToCopy.HolsterPositionShift;
      this.FlyingMeshName = itemToCopy.FlyingMeshName;
      this.BodyName = itemToCopy.BodyName;
      this.HolsterBodyName = itemToCopy.HolsterBodyName;
      this.CollisionBodyName = itemToCopy.CollisionBodyName;
      this.RecalculateBody = itemToCopy.RecalculateBody;
      this.PrefabName = itemToCopy.PrefabName;
      this.Name = itemToCopy.Name;
      this.ItemFlags = itemToCopy.ItemFlags;
      this.Value = itemToCopy.Value;
      this.Weight = itemToCopy.Weight;
      this.Difficulty = itemToCopy.Difficulty;
      this.ArmBandMeshName = itemToCopy.ArmBandMeshName;
      this.IsFood = itemToCopy.IsFood;
      this.Type = itemToCopy.Type;
      this.ScaleFactor = itemToCopy.ScaleFactor;
      this.IsUniqueItem = false;
    }

    internal static ItemObject InitializeTradeGood(
      ItemObject item,
      TextObject name,
      string meshName,
      ItemCategory category,
      int value,
      float weight,
      ItemObject.ItemTypeEnum itemType,
      bool isFood)
    {
      item.Initialize();
      item.Name = name;
      item.MultiMeshName = meshName;
      item.ItemCategory = category;
      item.Value = value;
      item.Weight = weight;
      item.ItemType = itemType;
      item.IsFood = isFood;
      item.ItemComponent = (ItemComponent) new TradeItemComponent();
      item.AfterInitialized();
      item.ItemFlags |= ItemFlags.Civilian;
      return item;
    }

    public static void InitAsPlayerCraftedItem(ref ItemObject itemObject) => itemObject.IsCraftedByPlayer = true;

    internal static void InitCraftedItemObject(
      ref ItemObject itemObject,
      TextObject name,
      BasicCultureObject culture,
      ItemFlags itemProperties,
      float weight,
      float appearance,
      WeaponDesign craftedData)
    {
      BladeData bladeData = craftedData.UsedPieces[0].CraftingPiece.BladeData;
      itemObject.Weight = weight;
      itemObject.Name = name;
      itemObject.MultiMeshName = "";
      itemObject.HolsterMeshName = "";
      itemObject.HolsterWithWeaponMeshName = "";
      itemObject.ItemHolsters = (string[]) craftedData.Template.ItemHolsters.Clone();
      itemObject.HolsterPositionShift = craftedData.HolsterShiftAmount;
      itemObject.FlyingMeshName = "";
      itemObject.BodyName = bladeData?.BodyName;
      itemObject.HolsterBodyName = bladeData?.HolsterBodyName ?? bladeData?.BodyName;
      itemObject.CollisionBodyName = "";
      itemObject.RecalculateBody = true;
      itemObject.Culture = culture;
      itemObject.Difficulty = 0;
      itemObject.ScaleFactor = 1f;
      itemObject.Type = WeaponComponentData.GetItemTypeFromWeaponClass(craftedData.Template.WeaponUsageDatas.First<WeaponUsageData>().WeaponClass);
      itemObject.ItemFlags = itemProperties;
      itemObject.Appearance = appearance;
      itemObject.WeaponDesign = craftedData;
    }

    public override int GetHashCode() => (int) this.Id.SubId;

    public float Tierf => Game.Current.BasicModels.ItemValueModel.CalculateTier(this);

    public ItemObject.ItemTiers Tier => this.ItemComponent == null ? ItemObject.ItemTiers.Tier1 : (ItemObject.ItemTiers) MBMath.ClampInt(MathF.Round(this.Tierf), 0, 5);

    public void DetermineItemCategoryForItem()
    {
      if (Game.Current.BasicModels.ItemCategorySelector == null || this.ItemCategory != null)
        return;
      this.ItemCategory = Game.Current.BasicModels.ItemCategorySelector.GetItemCategoryForItem(this);
    }

    public static ItemObject GetCraftedItemObjectFromHashedCode(string hashedCode)
    {
      foreach (ItemObject objectType in MBObjectManager.Instance.GetObjectTypeList<ItemObject>())
      {
        if (objectType.IsCraftedWeapon && objectType.WeaponDesign.HashedCode == hashedCode)
          return objectType;
      }
      return (ItemObject) null;
    }

    public ItemObject PrerequisiteItem { get; private set; }

    public IReadOnlyList<WeaponComponentData> Weapons => this.WeaponComponent == null ? (IReadOnlyList<WeaponComponentData>) null : this.WeaponComponent.Weapons;

    public void AddWeapon(WeaponComponentData weapon, ItemModifierGroup itemModifierGroup)
    {
      if (this.WeaponComponent == null)
        this.ItemComponent = (ItemComponent) new WeaponComponent(this);
      this.WeaponComponent.AddWeapon(weapon, itemModifierGroup);
    }

    public override void Deserialize(MBObjectManager objectManager, XmlNode node)
    {
      base.Deserialize(objectManager, node);
      if (node.Name == "CraftedItem")
      {
        XmlNode attribute1 = (XmlNode) node.Attributes["multiplayer_item"];
        if (attribute1 != null && !string.IsNullOrEmpty(attribute1.InnerText))
          this.MultiplayerItem = attribute1.InnerText == "true";
        XmlNode attribute2 = (XmlNode) node.Attributes["is_merchandise"];
        if (attribute2 != null && !string.IsNullOrEmpty(attribute2.InnerText))
          this.NotMerchandise = attribute2.InnerText != "true";
        TextObject weaponName = new TextObject(node.Attributes["name"].InnerText);
        string innerText1 = node.Attributes["crafting_template"].InnerText;
        int num = node.Attributes["has_modifier"] == null ? 1 : (node.Attributes["has_modifier"].InnerText != "false" ? 1 : 0);
        string objectName = node.Attributes["item_modifier_group"]?.Value;
        ItemModifierGroup itemModifierGroup = (ItemModifierGroup) null;
        if (num != 0)
          itemModifierGroup = objectName != null ? Game.Current.ObjectManager.GetObject<ItemModifierGroup>(objectName) : CraftingTemplate.GetTemplateFromId(innerText1).ItemModifierGroup;
        WeaponDesignElement[] usedPieces = new WeaponDesignElement[4];
        XmlNode xmlNode = (XmlNode) null;
        for (int i = 0; i < node.ChildNodes.Count; ++i)
        {
          if (node.ChildNodes[i].Name == "Pieces")
          {
            xmlNode = node.ChildNodes[i];
            break;
          }
        }
        foreach (XmlNode childNode in xmlNode.ChildNodes)
        {
          if (childNode.Name == "Piece")
          {
            XmlAttribute attribute3 = childNode.Attributes["id"];
            XmlAttribute attribute4 = childNode.Attributes["Type"];
            XmlAttribute attribute5 = childNode.Attributes["scale_factor"];
            string innerText2 = attribute3.InnerText;
            CraftingPiece.PieceTypes pieceTypes = (CraftingPiece.PieceTypes) Enum.Parse(typeof (CraftingPiece.PieceTypes), attribute4.InnerText);
            CraftingPiece craftingPiece = MBObjectManager.Instance.GetObject<CraftingPiece>(innerText2);
            usedPieces[(int) pieceTypes] = WeaponDesignElement.CreateUsablePiece(craftingPiece);
            if (attribute5 != null)
              usedPieces[(int) pieceTypes].SetScale(int.Parse(attribute5.Value));
          }
        }
        float weightOverriden = node.Attributes["weight"] != null ? float.Parse(node.Attributes["weight"].Value) : 0.0f;
        int swingSpeedOverriden = node.Attributes["swing_speed"] != null ? int.Parse(node.Attributes["swing_speed"].Value) : 0;
        int thrustSpeedOverriden = node.Attributes["thrust_speed"] != null ? int.Parse(node.Attributes["thrust_speed"].Value) : 0;
        int swingDamageOverriden = node.Attributes["swing_damage"] != null ? int.Parse(node.Attributes["swing_damage"].Value) : 0;
        int thrustDamageOverriden = node.Attributes["thrust_damage"] != null ? int.Parse(node.Attributes["thrust_damage"].Value) : 0;
        ItemObject preCraftedWeapon = Crafting.CreatePreCraftedWeapon(this, usedPieces, innerText1, weaponName, new Crafting.OverrideData(weightOverriden, swingSpeedOverriden, thrustSpeedOverriden, swingDamageOverriden, thrustDamageOverriden), itemModifierGroup);
        if (DefaultItems.Instance != null && preCraftedWeapon == DefaultItems.Trash)
        {
          MBObjectManager.Instance.UnregisterObject((MBObjectBase) this);
          return;
        }
        this.Effectiveness = this.CalculateEffectiveness();
        this.Value = node.Attributes["value"] != null ? int.Parse(node.Attributes["value"].Value) : this.CalculateValue();
        if (node.Attributes["culture"] != null)
          this.Culture = (BasicCultureObject) objectManager.ReadObjectReferenceFromXml("culture", typeof (BasicCultureObject), node);
        this.PrerequisiteItem = node.Attributes["prerequisite_item"] != null ? (ItemObject) objectManager.ReadObjectReferenceFromXml("prerequisite_item", typeof (ItemObject), node) : (ItemObject) null;
      }
      else
      {
        this.Name = new TextObject(node.Attributes["name"].InnerText);
        XmlNode attribute1 = (XmlNode) node.Attributes["multiplayer_item"];
        if (attribute1 != null && !string.IsNullOrEmpty(attribute1.InnerText))
          this.MultiplayerItem = attribute1.InnerText == "true";
        XmlNode attribute2 = (XmlNode) node.Attributes["is_merchandise"];
        if (attribute2 != null && !string.IsNullOrEmpty(attribute2.InnerText))
          this.NotMerchandise = attribute2.InnerText != "true";
        this.PrerequisiteItem = node.Attributes["prerequisite_item"] != null ? (ItemObject) objectManager.ReadObjectReferenceFromXml("prerequisite_item", typeof (ItemObject), node) : (ItemObject) null;
        XmlNode attribute3 = (XmlNode) node.Attributes["mesh"];
        if (attribute3 != null && !string.IsNullOrEmpty(attribute3.InnerText))
          this.MultiMeshName = attribute3.InnerText;
        this.HolsterMeshName = node.Attributes["holster_mesh"] != null ? node.Attributes["holster_mesh"].Value : (string) null;
        this.HolsterWithWeaponMeshName = node.Attributes["holster_mesh_with_weapon"] != null ? node.Attributes["holster_mesh_with_weapon"].Value : (string) null;
        this.FlyingMeshName = node.Attributes["flying_mesh"] != null ? node.Attributes["flying_mesh"].Value : (string) null;
        this.HasLowerHolsterPriority = false;
        if (node.Attributes["item_holsters"] != null)
        {
          this.ItemHolsters = node.Attributes["item_holsters"].Value.Split(':');
          if (node.Attributes["has_lower_holster_priority"] != null)
            this.HasLowerHolsterPriority = bool.Parse(node.Attributes["has_lower_holster_priority"].Value);
        }
        this.HolsterPositionShift = node.Attributes["holster_position_shift"] != null ? Vec3.Parse(node.Attributes["holster_position_shift"].Value) : Vec3.Zero;
        this.BodyName = node.Attributes["body_name"] != null ? node.Attributes["body_name"].Value : (string) null;
        this.HolsterBodyName = node.Attributes["holster_body_name"] != null ? node.Attributes["holster_body_name"].Value : (string) null;
        this.CollisionBodyName = node.Attributes["shield_body_name"] != null ? node.Attributes["shield_body_name"].Value : (string) null;
        this.RecalculateBody = node.Attributes["recalculate_body"] != null && bool.Parse(node.Attributes["recalculate_body"].Value);
        XmlNode attribute4 = (XmlNode) node.Attributes["prefab"];
        this.PrefabName = attribute4 == null || string.IsNullOrEmpty(attribute4.InnerText) ? "" : attribute4.InnerText;
        this.Culture = (BasicCultureObject) objectManager.ReadObjectReferenceFromXml("culture", typeof (BasicCultureObject), node);
        string objectName = node.Attributes["item_category"] != null ? node.Attributes["item_category"].Value : (string) null;
        if (!string.IsNullOrEmpty(objectName))
          this.ItemCategory = Game.Current.ObjectManager.GetObject<ItemCategory>(objectName);
        this.Weight = node.Attributes["weight"] != null ? float.Parse(node.Attributes["weight"].Value) : 1f;
        this.LodAtlasIndex = node.Attributes["lod_atlas_index"] != null ? int.Parse(node.Attributes["lod_atlas_index"].Value) : -1;
        XmlAttribute attribute5 = node.Attributes["difficulty"];
        if (attribute5 != null)
          this.Difficulty = int.Parse(attribute5.Value);
        XmlAttribute attribute6 = node.Attributes["appearance"];
        this.Appearance = attribute6 != null ? float.Parse(attribute6.Value) : 0.5f;
        XmlAttribute attribute7 = node.Attributes["IsFood"];
        if (attribute7 != null)
          this.IsFood = Convert.ToBoolean(attribute7.InnerText);
        this.IsUsingTableau = node.Attributes["using_tableau"] != null && Convert.ToBoolean(node.Attributes["using_tableau"].InnerText);
        XmlNode attribute8 = (XmlNode) node.Attributes["using_arm_band"];
        if (attribute8 != null)
          this.ArmBandMeshName = Convert.ToString(attribute8.InnerText);
        this.ScaleFactor = node.Attributes["scale_factor"] != null ? float.Parse(node.Attributes["scale_factor"].Value) : 1f;
        this.ItemFlags = (ItemFlags) 0;
        foreach (XmlNode childNode1 in node.ChildNodes)
        {
          if (childNode1.Name == "ItemComponent")
          {
            foreach (XmlNode childNode2 in childNode1.ChildNodes)
            {
              if (childNode2.NodeType != XmlNodeType.Comment)
              {
                string name = childNode2.Name;
                ItemComponent itemComponent;
                if (!(name == "Armor"))
                {
                  if (!(name == "Weapon"))
                  {
                    if (!(name == "Horse"))
                    {
                      if (!(name == "Trade"))
                      {
                        if (!(name == "Food"))
                          throw new Exception("Wrong ItemComponent type.");
                        itemComponent = (ItemComponent) null;
                      }
                      else
                        itemComponent = (ItemComponent) new TradeItemComponent();
                    }
                    else
                      itemComponent = (ItemComponent) new HorseComponent();
                  }
                  else
                    itemComponent = (ItemComponent) new WeaponComponent(this);
                }
                else
                  itemComponent = (ItemComponent) new ArmorComponent(this);
                if (itemComponent != null)
                {
                  itemComponent.Deserialize(objectManager, childNode2);
                  this.ItemComponent = itemComponent;
                }
              }
            }
          }
          else if (childNode1.Name == "Flags")
          {
            foreach (ItemFlags itemFlags in Enum.GetValues(typeof (ItemFlags)))
            {
              XmlAttribute attribute9 = childNode1.Attributes[itemFlags.ToString()];
              if (attribute9 != null && attribute9.Value.ToLowerInvariant() != "false")
                this.ItemFlags |= itemFlags;
            }
          }
        }
        XmlAttribute attribute10 = node.Attributes["Type"];
        if (attribute10 != null)
        {
          this.Type = (ItemObject.ItemTypeEnum) Enum.Parse(typeof (ItemObject.ItemTypeEnum), attribute10.Value, true);
          if (this.WeaponComponent != null)
          {
            ItemObject.ItemTypeEnum itemType = this.WeaponComponent.GetItemType();
            if (this.Type != itemType)
              TaleWorlds.Library.Debug.Print("ItemType for \"" + this.StringId + "\" has been overridden by WeaponClass from \"" + (object) this.Type + "\" to \"" + (object) itemType + "\"", color: TaleWorlds.Library.Debug.DebugColor.Red, debugFilter: 64UL);
            this.Type = itemType;
          }
        }
        XmlAttribute attribute11 = node.Attributes["AmmoOffset"];
        if (attribute11 != null)
        {
          string[] strArray = attribute11.Value.Split(',');
          this.WeaponComponent.PrimaryWeapon.SetAmmoOffset(new Vec3());
          if (strArray.Length == 3)
          {
            try
            {
              this.WeaponComponent.PrimaryWeapon.SetAmmoOffset(new Vec3(float.Parse(strArray[0], (IFormatProvider) CultureInfo.InvariantCulture), float.Parse(strArray[1], (IFormatProvider) CultureInfo.InvariantCulture), float.Parse(strArray[2], (IFormatProvider) CultureInfo.InvariantCulture)));
            }
            catch (Exception ex)
            {
            }
          }
        }
        this.Effectiveness = this.CalculateEffectiveness();
        this.Value = node.Attributes["value"] != null ? int.Parse(node.Attributes["value"].Value) : this.CalculateValue();
        if (this.PrimaryWeapon != null)
        {
          if (this.PrimaryWeapon.IsMeleeWeapon || this.PrimaryWeapon.IsRangedWeapon)
          {
            if (!string.IsNullOrEmpty(this.BodyName))
              ;
          }
          else if (this.PrimaryWeapon.IsConsumable)
          {
            string.IsNullOrEmpty(this.HolsterBodyName);
            if (!string.IsNullOrEmpty(this.BodyName))
              ;
          }
          else if (this.PrimaryWeapon.IsShield)
          {
            if (!string.IsNullOrEmpty(this.BodyName))
            {
              int num = this.RecalculateBody ? 1 : 0;
            }
            string.IsNullOrEmpty(this.CollisionBodyName);
          }
        }
      }
      this.DetermineItemCategoryForItem();
    }

    public ItemObject.ItemTypeEnum ItemType
    {
      get => this.Type;
      private set => this.Type = value;
    }

    public bool IsMountable => this.HasHorseComponent && this.HorseComponent.IsRideable;

    public bool IsTradeGood => this.ItemType == ItemObject.ItemTypeEnum.Goods;

    public bool IsAnimal => this.HasHorseComponent && !this.HorseComponent.IsRideable;

    public override string ToString() => this.StringId;

    public static MBReadOnlyList<ItemObject> All => Game.Current.ObjectManager.GetObjectTypeList<ItemObject>();

    public static IEnumerable<ItemObject> AllTradeGoods
    {
      get
      {
        foreach (ItemObject objectType in Game.Current.ObjectManager.GetObjectTypeList<ItemObject>())
        {
          if (objectType.IsTradeGood)
            yield return objectType;
        }
      }
    }

    public SkillObject RelevantSkill
    {
      get
      {
        SkillObject skillObject = (SkillObject) null;
        if (this.PrimaryWeapon != null)
          skillObject = this.PrimaryWeapon.RelevantSkill;
        else if (this.HasHorseComponent)
          skillObject = DefaultSkills.Riding;
        return skillObject;
      }
    }

    public static ItemObject GetItemFromWeaponKind(int weaponKind) => weaponKind < 0 ? (ItemObject) null : MBObjectManager.Instance.GetObject(new MBGUID((uint) weaponKind)) as ItemObject;

    [Conditional("TRACE")]
    private void MakeSureProperFlagsSetForOneAndTwoHandedWeapons()
    {
      if (this.PrimaryWeapon == null)
        return;
      if ((this.Type == ItemObject.ItemTypeEnum.Bow || this.Type == ItemObject.ItemTypeEnum.Crossbow || this.Type == ItemObject.ItemTypeEnum.TwoHandedWeapon) && !this.PrimaryWeapon.WeaponFlags.HasAnyFlag<WeaponFlags>(WeaponFlags.NotUsableWithOneHand))
        this.PrimaryWeapon.WeaponFlags |= WeaponFlags.NotUsableWithOneHand;
      if ((this.Type == ItemObject.ItemTypeEnum.Bow || this.Type == ItemObject.ItemTypeEnum.Crossbow) && !this.PrimaryWeapon.WeaponFlags.HasAnyFlag<WeaponFlags>(WeaponFlags.TwoHandIdleOnMount))
        this.PrimaryWeapon.WeaponFlags |= WeaponFlags.TwoHandIdleOnMount;
      if (this.Type != ItemObject.ItemTypeEnum.OneHandedWeapon && this.Type != ItemObject.ItemTypeEnum.Shield || !this.PrimaryWeapon.WeaponFlags.HasAnyFlag<WeaponFlags>(WeaponFlags.NotUsableWithOneHand))
        return;
      this.PrimaryWeapon.WeaponFlags &= ~WeaponFlags.NotUsableWithOneHand;
    }

    [Conditional("DEBUG")]
    private void DebugMakeSurePhysicsMaterialCorrectlySet()
    {
    }

    [Conditional("DEBUG")]
    private void MakeSureWeaponLengthAndMissileSpeedCorrect()
    {
      if (this.WeaponComponent == null)
        return;
      foreach (WeaponComponentData weapon in (IEnumerable<WeaponComponentData>) this.WeaponComponent.Weapons)
      {
        int weaponLength = weapon.WeaponLength;
        if (this.Type == ItemObject.ItemTypeEnum.Arrows || this.Type == ItemObject.ItemTypeEnum.Bolts || (this.Type == ItemObject.ItemTypeEnum.Bullets || this.Type == ItemObject.ItemTypeEnum.Thrown))
        {
          int missileSpeed = weapon.MissileSpeed;
        }
      }
    }

    public static ItemObject.ItemTypeEnum GetAmmoTypeForItemType(
      ItemObject.ItemTypeEnum itemType)
    {
      switch (itemType)
      {
        case ItemObject.ItemTypeEnum.Bow:
          return ItemObject.ItemTypeEnum.Arrows;
        case ItemObject.ItemTypeEnum.Crossbow:
          return ItemObject.ItemTypeEnum.Bolts;
        case ItemObject.ItemTypeEnum.Thrown:
          return ItemObject.ItemTypeEnum.Thrown;
        case ItemObject.ItemTypeEnum.Pistol:
          return ItemObject.ItemTypeEnum.Bullets;
        default:
          return ItemObject.ItemTypeEnum.Invalid;
      }
    }

    public static float GetAirFrictionConstant(WeaponClass weaponClass)
    {
      switch (weaponClass)
      {
        case WeaponClass.Arrow:
          return ManagedParameters.Instance.GetManagedParameter(ManagedParametersEnum.AirFrictionArrow);
        case WeaponClass.Bolt:
          return ManagedParameters.Instance.GetManagedParameter(ManagedParametersEnum.AirFrictionArrow);
        case WeaponClass.Cartridge:
          return ManagedParameters.Instance.GetManagedParameter(ManagedParametersEnum.AirFrictionBullet);
        case WeaponClass.Bow:
          return ManagedParameters.Instance.GetManagedParameter(ManagedParametersEnum.AirFrictionArrow);
        case WeaponClass.Crossbow:
          return ManagedParameters.Instance.GetManagedParameter(ManagedParametersEnum.AirFrictionArrow);
        case WeaponClass.Stone:
          return ManagedParameters.Instance.GetManagedParameter(ManagedParametersEnum.AirFrictionKnife);
        case WeaponClass.Boulder:
          return ManagedParameters.Instance.GetManagedParameter(ManagedParametersEnum.AirFrictionArrow);
        case WeaponClass.ThrowingAxe:
          return ManagedParameters.Instance.GetManagedParameter(ManagedParametersEnum.AirFrictionAxe);
        case WeaponClass.ThrowingKnife:
          return ManagedParameters.Instance.GetManagedParameter(ManagedParametersEnum.AirFrictionKnife);
        case WeaponClass.Javelin:
          return ManagedParameters.Instance.GetManagedParameter(ManagedParametersEnum.AirFrictionJavelin);
        case WeaponClass.Pistol:
          return ManagedParameters.Instance.GetManagedParameter(ManagedParametersEnum.AirFrictionBullet);
        case WeaponClass.Musket:
          return ManagedParameters.Instance.GetManagedParameter(ManagedParametersEnum.AirFrictionBullet);
        default:
          return ManagedParameters.Instance.GetManagedParameter(ManagedParametersEnum.AirFrictionArrow);
      }
    }

    private float CalculateEffectiveness()
    {
      float num1 = 1f;
      ArmorComponent armorComponent = this.ArmorComponent;
      if (armorComponent != null)
        num1 = this.Type != ItemObject.ItemTypeEnum.HorseHarness ? (float) (((double) armorComponent.HeadArmor * 34.0 + (double) armorComponent.BodyArmor * 42.0 + (double) armorComponent.LegArmor * 12.0 + (double) armorComponent.ArmArmor * 12.0) * 0.0299999993294477) : (float) armorComponent.BodyArmor * 1.67f;
      if (this.WeaponComponent != null)
      {
        WeaponComponentData primaryWeapon = this.WeaponComponent.PrimaryWeapon;
        float num2 = 1f;
        switch (primaryWeapon.WeaponClass)
        {
          case WeaponClass.Dagger:
            num2 = 0.4f;
            break;
          case WeaponClass.OneHandedSword:
            num2 = 0.55f;
            break;
          case WeaponClass.TwoHandedSword:
            num2 = 0.6f;
            break;
          case WeaponClass.OneHandedAxe:
            num2 = 0.5f;
            break;
          case WeaponClass.TwoHandedAxe:
            num2 = 0.55f;
            break;
          case WeaponClass.Mace:
            num2 = 0.5f;
            break;
          case WeaponClass.Pick:
            num2 = 0.4f;
            break;
          case WeaponClass.TwoHandedMace:
            num2 = 0.55f;
            break;
          case WeaponClass.OneHandedPolearm:
            num2 = 0.4f;
            break;
          case WeaponClass.TwoHandedPolearm:
            num2 = 0.4f;
            break;
          case WeaponClass.LowGripPolearm:
            num2 = 0.4f;
            break;
          case WeaponClass.Arrow:
            num2 = 3f;
            break;
          case WeaponClass.Bolt:
            num2 = 3f;
            break;
          case WeaponClass.Cartridge:
            num2 = 3f;
            break;
          case WeaponClass.Bow:
            num2 = 0.55f;
            break;
          case WeaponClass.Crossbow:
            num2 = 0.57f;
            break;
          case WeaponClass.Stone:
            num2 = 0.1f;
            break;
          case WeaponClass.Boulder:
            num2 = 0.1f;
            break;
          case WeaponClass.ThrowingAxe:
            num2 = 0.25f;
            break;
          case WeaponClass.ThrowingKnife:
            num2 = 0.2f;
            break;
          case WeaponClass.Javelin:
            num2 = 0.28f;
            break;
          case WeaponClass.Pistol:
            num2 = 1f;
            break;
          case WeaponClass.Musket:
            num2 = 1f;
            break;
          case WeaponClass.SmallShield:
            num2 = 0.4f;
            break;
          case WeaponClass.LargeShield:
            num2 = 0.5f;
            break;
        }
        if (primaryWeapon.IsRangedWeapon)
          num1 = !primaryWeapon.IsConsumable ? (float) (((double) (primaryWeapon.MissileSpeed * primaryWeapon.MissileDamage) * 1.75 + (double) (primaryWeapon.ThrustSpeed * primaryWeapon.Accuracy) * 0.300000011920929) * 0.00999999977648258) * (float) primaryWeapon.MaxDataValue * num2 : (float) (((double) (primaryWeapon.MissileDamage * primaryWeapon.MissileSpeed) * 1.77499997615814 + (double) (primaryWeapon.Accuracy * (int) primaryWeapon.MaxDataValue) * 25.0 + (double) primaryWeapon.WeaponLength * 4.0) * 0.00694399978965521) * (float) primaryWeapon.MaxDataValue * num2;
        else if (primaryWeapon.IsMeleeWeapon)
        {
          float val2 = (float) (primaryWeapon.ThrustSpeed * primaryWeapon.ThrustDamage) * 0.01f;
          double num3 = (double) (primaryWeapon.SwingSpeed * primaryWeapon.SwingDamage) * 0.00999999977648258;
          float num4 = Math.Max((float) num3, val2);
          float num5 = Math.Min((float) num3, val2);
          num1 = (float) ((((double) num4 + (double) num5 * (double) num5 / (double) num4) * 120.0 + (double) primaryWeapon.Handling * 15.0 + (double) primaryWeapon.WeaponLength * 20.0 + (double) this.Weight * 5.0) * 0.00999999977648258) * num2;
        }
        else if (primaryWeapon.IsConsumable)
          num1 = (float) (((double) primaryWeapon.MissileDamage * 550.0 + (double) primaryWeapon.MissileSpeed * 15.0 + (double) primaryWeapon.MaxDataValue * 60.0) * 0.00999999977648258) * num2;
        else if (primaryWeapon.IsShield)
          num1 = (float) (((double) primaryWeapon.BodyArmor * 60.0 + (double) primaryWeapon.ThrustSpeed * 10.0 + (double) primaryWeapon.MaxDataValue * 40.0 + (double) primaryWeapon.WeaponLength * 20.0) * 0.00999999977648258) * num2;
      }
      if (this.HorseComponent != null)
        num1 = (float) (((double) (this.HorseComponent.ChargeDamage * this.HorseComponent.Speed + this.HorseComponent.Maneuver * this.HorseComponent.Speed) + (double) this.HorseComponent.BodyLength * (double) this.Weight * 0.025000000372529) * (double) (this.HorseComponent.HitPoints + this.HorseComponent.HitPointBonus) * 9.99999974737875E-05);
      return num1;
    }

    internal int CalculateValue()
    {
      ItemValueModel itemValueModel = Game.Current.BasicModels.ItemValueModel;
      return itemValueModel == null ? 1 : itemValueModel.CalculateValue(this);
    }

    public WeaponComponentData GetWeaponWithUsageIndex(int usageIndex) => this.Weapons.ElementAt<WeaponComponentData>(usageIndex);

    public enum ItemUsageSetFlags
    {
      RequiresMount = 1,
      RequiresNoMount = 2,
      RequiresShield = 4,
      RequiresNoShield = 8,
      PassiveUsage = 16, // 0x00000010
    }

    public enum ItemTypeEnum
    {
      Invalid,
      Horse,
      OneHandedWeapon,
      TwoHandedWeapon,
      Polearm,
      Arrows,
      Bolts,
      Shield,
      Bow,
      Crossbow,
      Thrown,
      Goods,
      HeadArmor,
      BodyArmor,
      LegArmor,
      HandArmor,
      Pistol,
      Musket,
      Bullets,
      Animal,
      Book,
      ChestArmor,
      Cape,
      HorseHarness,
      Banner,
    }

    public enum ItemTiers
    {
      Tier1,
      Tier2,
      Tier3,
      Tier4,
      Tier5,
      Tier6,
      NumTiers,
    }
  }
}