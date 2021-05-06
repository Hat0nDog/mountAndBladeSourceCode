// Decompiled with JetBrains decompiler
// Type: TaleWorlds.Core.Crafting
// Assembly: TaleWorlds.Core, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 43F7B45C-9AE1-4715-A37B-2364840F62AF
// Assembly location: E:\SteamLibrary\steamapps\common\Mount & Blade II Bannerlord\bin\Win64_Shipping_Client\TaleWorlds.Core.dll

using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml;
using TaleWorlds.Library;
using TaleWorlds.Localization;
using TaleWorlds.ObjectSystem;
using TaleWorlds.SaveSystem;

namespace TaleWorlds.Core
{
  public class Crafting
  {
    public const int WeightOfCrudeIron = 1;
    public const int WeightOfIron = 2;
    public const int WeightOfCompositeIron = 3;
    public const int WeightOfSteel = 4;
    public const int WeightOfRefinedSteel = 5;
    public const int WeightOfCalradianSteel = 6;
    private string _craftedWeaponName;
    private List<WeaponDesign> _history;
    private int _currentHistoryIndex;
    private ItemObject _craftedItemObject;

    public Crafting(CraftingTemplate craftingTemplate, BasicCultureObject culture)
    {
      this.CurrentCraftingTemplate = craftingTemplate;
      this.CurrentCulture = culture;
    }

    public BasicCultureObject CurrentCulture { get; }

    public CraftingTemplate CurrentCraftingTemplate { get; }

    public WeaponDesign CurrentWeaponDesign { get; private set; }

    public ItemModifierGroup CurrentItemModifierGroup { get; private set; }

    public string CraftedWeaponName
    {
      get
      {
        if (this._craftedWeaponName != null)
          return this._craftedWeaponName;
        TextObject textObject = new TextObject("{=uZhHh7pm}Crafted {CURR_TEMPLATE_NAME}");
        textObject.SetTextVariable("CURR_TEMPLATE_NAME", this.CurrentCraftingTemplate.TemplateName);
        return textObject.ToString();
      }
      set => this._craftedWeaponName = string.IsNullOrEmpty(value) ? (string) null : value;
    }

    public void Init()
    {
      this._history = new List<WeaponDesign>();
      this.UsablePiecesList = new List<WeaponDesignElement>[4];
      foreach (CraftingPiece piece in this.CurrentCraftingTemplate.Pieces)
      {
        CraftingPiece craftingPiece = piece;
        if (!this.CurrentCraftingTemplate.BuildOrders.All<PieceData>((Func<PieceData, bool>) (x => x.PieceType != craftingPiece.PieceType)))
        {
          int pieceType = (int) craftingPiece.PieceType;
          if (this.UsablePiecesList[pieceType] == null)
            this.UsablePiecesList[pieceType] = new List<WeaponDesignElement>();
          this.UsablePiecesList[pieceType].Add(WeaponDesignElement.CreateUsablePiece(craftingPiece));
        }
      }
      WeaponDesignElement[] usedPieces = new WeaponDesignElement[4];
      for (int index = 0; index < usedPieces.Length; ++index)
        usedPieces[index] = this.UsablePiecesList[index] == null ? WeaponDesignElement.GetInvalidPieceForType((CraftingPiece.PieceTypes) index) : this.UsablePiecesList[index].First<WeaponDesignElement>((Func<WeaponDesignElement, bool>) (p => !p.CraftingPiece.IsHiddenOnDesigner));
      this.CurrentWeaponDesign = new WeaponDesign(this.CurrentCraftingTemplate, (string) null, usedPieces);
      this._history.Add(this.CurrentWeaponDesign);
    }

    public List<WeaponDesignElement>[] UsablePiecesList { get; private set; }

    public WeaponDesignElement[] SelectedPieces => this.CurrentWeaponDesign.UsedPieces;

    public WeaponDesignElement GetRandomPieceOfType(
      CraftingPiece.PieceTypes pieceType,
      bool randomScale)
    {
      if (!this.CurrentCraftingTemplate.IsPieceTypeUsable(pieceType))
        return WeaponDesignElement.GetInvalidPieceForType(pieceType);
      WeaponDesignElement copy = this.UsablePiecesList[(int) pieceType][MBRandom.RandomInt(this.UsablePiecesList[(int) pieceType].Count)].GetCopy();
      if (randomScale)
        copy.SetScale((int) (90.0 + (double) MBRandom.RandomFloat * 20.0));
      return copy;
    }

    public void SwitchToCraftedItem(ItemObject item)
    {
      WeaponDesignElement[] usedPieces1 = item.WeaponDesign.UsedPieces;
      WeaponDesignElement[] usedPieces2 = new WeaponDesignElement[4];
      for (int index = 0; index < usedPieces2.Length; ++index)
        usedPieces2[index] = usedPieces1[index].GetCopy();
      this.CurrentWeaponDesign = new WeaponDesign(this.CurrentWeaponDesign.Template, this.CurrentWeaponDesign.WeaponName, usedPieces2);
      this.ReIndex();
    }

    public void Randomize()
    {
      WeaponDesignElement[] usedPieces = new WeaponDesignElement[4];
      for (int index = 0; index < usedPieces.Length; ++index)
        usedPieces[index] = this.GetRandomPieceOfType((CraftingPiece.PieceTypes) index, true);
      this.CurrentWeaponDesign = new WeaponDesign(this.CurrentWeaponDesign.Template, this.CurrentWeaponDesign.WeaponName, usedPieces);
      this.ReIndex();
    }

    public void SwitchToPiece(WeaponDesignElement piece)
    {
      this.SelectedPieces[(int) piece.CraftingPiece.PieceType].SetScale(100);
      CraftingPiece.PieceTypes pieceType = piece.CraftingPiece.PieceType;
      WeaponDesignElement[] usedPieces = new WeaponDesignElement[4];
      for (int index = 0; index < usedPieces.Length; ++index)
      {
        if (pieceType == (CraftingPiece.PieceTypes) index)
        {
          usedPieces[index] = piece.GetCopy();
        }
        else
        {
          usedPieces[index] = this.CurrentWeaponDesign.UsedPieces[index].GetCopy();
          if (usedPieces[index].IsValid)
            usedPieces[index].SetScale(this.CurrentWeaponDesign.UsedPieces[index].ScalePercentage);
        }
      }
      this.CurrentWeaponDesign = new WeaponDesign(this.CurrentWeaponDesign.Template, this.CurrentWeaponDesign.WeaponName, usedPieces);
      this.ReIndex();
    }

    public void ScaleThePiece(CraftingPiece.PieceTypes scalingPieceType, int percentage)
    {
      WeaponDesignElement[] usedPieces = new WeaponDesignElement[4];
      for (int index = 0; index < usedPieces.Length; ++index)
      {
        usedPieces[index] = this.SelectedPieces[index].GetCopy();
        if (this.SelectedPieces[index].IsPieceScaled)
          usedPieces[index].SetScale(this.SelectedPieces[index].ScalePercentage);
      }
      usedPieces[(int) scalingPieceType].SetScale(percentage);
      this.CurrentWeaponDesign = new WeaponDesign(this.CurrentWeaponDesign.Template, this.CurrentWeaponDesign.WeaponName, usedPieces);
      this.ReIndex();
    }

    public void ReIndex(bool enforceReCreation = false)
    {
      if (!string.IsNullOrEmpty(this.CurrentWeaponDesign.WeaponName))
        this._craftedWeaponName = this.CurrentWeaponDesign.WeaponName;
      if (enforceReCreation)
        this.CurrentWeaponDesign = new WeaponDesign(this.CurrentWeaponDesign.Template, this.CurrentWeaponDesign.WeaponName, ((IEnumerable<WeaponDesignElement>) this.CurrentWeaponDesign.UsedPieces).ToArray<WeaponDesignElement>());
      this.SetItemObject((Crafting.OverrideData) null);
    }

    public bool Undo()
    {
      if (this._currentHistoryIndex <= 0)
        return false;
      --this._currentHistoryIndex;
      this.CurrentWeaponDesign = this._history[this._currentHistoryIndex];
      this.ReIndex();
      return true;
    }

    public bool Redo()
    {
      if (this._currentHistoryIndex + 1 >= this._history.Count)
        return false;
      ++this._currentHistoryIndex;
      this.CurrentWeaponDesign = this._history[this._currentHistoryIndex];
      this.ReIndex();
      return true;
    }

    public void UpdateHistory()
    {
      if (this._currentHistoryIndex < this._history.Count - 1)
        this._history.RemoveRange(this._currentHistoryIndex + 1, this._history.Count - 1 - this._currentHistoryIndex);
      WeaponDesignElement[] usedPieces = new WeaponDesignElement[this.CurrentWeaponDesign.UsedPieces.Length];
      for (int index = 0; index < this.CurrentWeaponDesign.UsedPieces.Length; ++index)
      {
        usedPieces[index] = this.CurrentWeaponDesign.UsedPieces[index].GetCopy();
        if (usedPieces[index].IsValid)
          usedPieces[index].SetScale(this.CurrentWeaponDesign.UsedPieces[index].ScalePercentage);
      }
      this._history.Add(new WeaponDesign(this.CurrentWeaponDesign.Template, this.CurrentWeaponDesign.WeaponName, usedPieces));
      this._currentHistoryIndex = this._history.Count - 1;
    }

    public TextObject GetRandomCraftName() => new TextObject("{=!}RANDOM_NAME");

    public static void GenerateItem(
      WeaponDesign weaponDesign,
      string name,
      BasicCultureObject culture,
      ItemModifierGroup itemModifierGroup,
      ref ItemObject itemObject,
      Crafting.OverrideData overridenData)
    {
      if (itemObject == null)
        itemObject = new ItemObject();
      if (overridenData == null)
        overridenData = new Crafting.OverrideData();
      float num = (float) Math.Round((double) ((IEnumerable<WeaponDesignElement>) weaponDesign.UsedPieces).Sum<WeaponDesignElement>((Func<WeaponDesignElement, float>) (selectedUsablePiece => selectedUsablePiece.ScaledWeight)), 2);
      float appearance = weaponDesign.UsedPieces[3].IsValid ? weaponDesign.UsedPieces[3].CraftingPiece.Appearance : weaponDesign.UsedPieces[0].CraftingPiece.Appearance;
      itemObject.StringId = !string.IsNullOrEmpty(itemObject.StringId) ? itemObject.StringId : weaponDesign.HashedCode;
      ItemObject.InitCraftedItemObject(ref itemObject, new TextObject("{=!}" + name), culture, Crafting.GetItemFlags(weaponDesign), overridenData.WeightOverriden + num, appearance, weaponDesign);
      itemObject = Crafting.CraftedItemGenerationHelper.GenerateCraftedItem(itemObject, weaponDesign, itemModifierGroup, overridenData);
      if (itemObject == null)
      {
        itemObject = DefaultItems.Trash;
        itemObject.IsReady = false;
      }
      else
        itemObject.IsReady = true;
      itemObject.Value = itemObject.CalculateValue();
      itemObject.DetermineItemCategoryForItem();
    }

    private static ItemFlags GetItemFlags(WeaponDesign weaponDesign) => weaponDesign.UsedPieces[0].CraftingPiece.AdditionalItemFlags;

    private void SetItemObject(Crafting.OverrideData overridenData, ItemObject itemObject = null)
    {
      if (itemObject == null)
        itemObject = new ItemObject();
      Crafting.GenerateItem(this.CurrentWeaponDesign, this.CraftedWeaponName, this.CurrentCulture, this.CurrentItemModifierGroup, ref itemObject, overridenData);
      this._craftedItemObject = itemObject;
    }

    public ItemObject GetCurrentCraftedItemObject(
      bool forceReCreate = false,
      Crafting.OverrideData overrideData = null)
    {
      if (forceReCreate)
        this.SetItemObject(overrideData);
      return this._craftedItemObject;
    }

    public IEnumerable<CraftingStatData> GetStatDatas(int usageIndex)
    {
      WeaponComponentData weapon = this._craftedItemObject.GetWeaponWithUsageIndex(usageIndex);
      foreach (KeyValuePair<CraftingTemplate.CraftingStatTypes, float> statData in this.CurrentCraftingTemplate.GetStatDatas(usageIndex, weapon.ThrustDamageType, weapon.SwingDamageType))
      {
        TextObject text = GameTexts.FindText("str_crafting_stat", statData.Key.ToString());
        switch (statData.Key)
        {
          case CraftingTemplate.CraftingStatTypes.ThrustDamage:
            text.SetTextVariable("THRUST_DAMAGE_TYPE", GameTexts.FindText("str_inventory_dmg_type", ((int) weapon.ThrustDamageType).ToString()));
            break;
          case CraftingTemplate.CraftingStatTypes.SwingDamage:
            text.SetTextVariable("SWING_DAMAGE_TYPE", GameTexts.FindText("str_inventory_dmg_type", ((int) weapon.SwingDamageType).ToString()));
            break;
          case CraftingTemplate.CraftingStatTypes.MissileDamage:
            if (weapon.ThrustDamageType != DamageTypes.Invalid)
            {
              text.SetTextVariable("THRUST_DAMAGE_TYPE", GameTexts.FindText("str_inventory_dmg_type", ((int) weapon.ThrustDamageType).ToString()));
              break;
            }
            if (weapon.SwingDamageType != DamageTypes.Invalid)
            {
              text.SetTextVariable("SWING_DAMAGE_TYPE", GameTexts.FindText("str_inventory_dmg_type", ((int) weapon.SwingDamageType).ToString()));
              break;
            }
            break;
        }
        float weaponOfUsageIndex = this.GetValueForCraftingStatForWeaponOfUsageIndex(statData.Key, this._craftedItemObject, weapon);
        float maxValue = statData.Value;
        yield return new CraftingStatData(text, weaponOfUsageIndex, maxValue, statData.Key);
      }
    }

    private float GetValueForCraftingStatForWeaponOfUsageIndex(
      CraftingTemplate.CraftingStatTypes craftingStatType,
      ItemObject item,
      WeaponComponentData weapon)
    {
      switch (craftingStatType)
      {
        case CraftingTemplate.CraftingStatTypes.Weight:
          return item.Weight;
        case CraftingTemplate.CraftingStatTypes.WeaponReach:
          return (float) weapon.WeaponLength;
        case CraftingTemplate.CraftingStatTypes.ThrustSpeed:
          return (float) weapon.ThrustSpeed;
        case CraftingTemplate.CraftingStatTypes.SwingSpeed:
          return (float) weapon.SwingSpeed;
        case CraftingTemplate.CraftingStatTypes.ThrustDamage:
          return (float) weapon.ThrustDamage;
        case CraftingTemplate.CraftingStatTypes.SwingDamage:
          return (float) weapon.SwingDamage;
        case CraftingTemplate.CraftingStatTypes.Handling:
          return (float) weapon.Handling;
        case CraftingTemplate.CraftingStatTypes.MissileDamage:
          return (float) weapon.MissileDamage;
        case CraftingTemplate.CraftingStatTypes.MissileSpeed:
          return (float) weapon.MissileSpeed;
        case CraftingTemplate.CraftingStatTypes.Accuracy:
          return (float) weapon.Accuracy;
        default:
          throw new ArgumentOutOfRangeException(nameof (craftingStatType), (object) craftingStatType, (string) null);
      }
    }

    public string GetXmlCodeForCurrentItem(ItemObject item)
    {
      string str1 = "" + "<CraftedItem id=\"" + this.CurrentWeaponDesign.HashedCode + "\"\nname=\"" + this.CraftedWeaponName + "\"\ncrafting_template=\"" + this.CurrentCraftingTemplate.StringId + "\">" + "\n" + "<Pieces>" + "\n";
      foreach (WeaponDesignElement selectedPiece in this.SelectedPieces)
      {
        if (selectedPiece.IsValid)
        {
          string str2 = "";
          if (selectedPiece.ScalePercentage != 100)
            str2 = "\nscale_factor=\"" + (object) selectedPiece.ScalePercentage + "\"";
          str1 = str1 + "<Piece id=\"" + selectedPiece.CraftingPiece.StringId + "\"\nType=\"" + (object) selectedPiece.CraftingPiece.PieceType + "\"" + str2 + "/>" + "\n";
        }
      }
      return str1 + "</Pieces>" + "\n" + "<!-- " + "Length: " + (object) MBMath.Floor((float) item.PrimaryWeapon.WeaponLength) + " Weight: " + (object) (float) Math.Round((double) item.Weight, 2) + " -->" + "\n" + "</CraftedItem>";
    }

    public bool TryGetWeaponPropertiesFromXmlCode(
      string xmlCode,
      out CraftingTemplate craftingTemplate,
      out (CraftingPiece, int)[] pieces)
    {
      try
      {
        XmlDocument xmlDocument = new XmlDocument();
        xmlDocument.LoadXml(xmlCode);
        pieces = new (CraftingPiece, int)[4];
        XmlNode xmlNode = xmlDocument.SelectSingleNode("CraftedItem");
        string templateId = xmlNode.Attributes["crafting_template"].Value;
        craftingTemplate = CraftingTemplate.GetTemplateFromId(templateId);
        foreach (XmlNode childNode in xmlNode.SelectSingleNode("Pieces").ChildNodes)
        {
          CraftingPiece.PieceTypes pieceType = CraftingPiece.PieceTypes.Invalid;
          string pieceId = (string) null;
          int num = 100;
          foreach (XmlAttribute attribute in (XmlNamedNodeMap) childNode.Attributes)
          {
            if (attribute.Name == "Type")
              pieceType = (CraftingPiece.PieceTypes) Enum.Parse(typeof (CraftingPiece.PieceTypes), attribute.Value);
            else if (attribute.Name == "id")
              pieceId = attribute.Value;
            else if (attribute.Name == "scale_factor")
              num = int.Parse(attribute.Value);
          }
          if (pieceType != CraftingPiece.PieceTypes.Invalid && !string.IsNullOrEmpty(pieceId) && craftingTemplate.IsPieceTypeUsable(pieceType))
            pieces[(int) pieceType] = (CraftingPiece.All.FirstOrDefault<CraftingPiece>((Func<CraftingPiece, bool>) (p => p.StringId == pieceId)), num);
        }
        return true;
      }
      catch (Exception ex)
      {
        craftingTemplate = (CraftingTemplate) null;
        pieces = ((CraftingPiece, int)[]) null;
        return false;
      }
    }

    public static ItemObject CreatePreCraftedWeapon(
      ItemObject itemObject,
      WeaponDesignElement[] usedPieces,
      string templateId,
      TextObject weaponName,
      Crafting.OverrideData overridenData,
      ItemModifierGroup itemModifierGroup)
    {
      for (int index = 0; index < usedPieces.Length; ++index)
      {
        if (usedPieces[index] == null)
          usedPieces[index] = WeaponDesignElement.GetInvalidPieceForType((CraftingPiece.PieceTypes) index);
      }
      TextObject textObject = !TextObject.IsNullOrEmpty(weaponName) ? weaponName : new TextObject("{=Uz1HHeKg}Crafted Random Weapon");
      WeaponDesign weaponDesign = new WeaponDesign(CraftingTemplate.GetTemplateFromId(templateId), textObject.ToString(), usedPieces);
      Crafting crafting = new Crafting(CraftingTemplate.GetTemplateFromId(templateId), (BasicCultureObject) null);
      crafting._craftedWeaponName = textObject.ToString();
      crafting.CurrentWeaponDesign = weaponDesign;
      crafting.CurrentItemModifierGroup = itemModifierGroup;
      crafting._history = new List<WeaponDesign>()
      {
        weaponDesign
      };
      crafting.SetItemObject(overridenData, itemObject);
      return crafting._craftedItemObject;
    }

    public static ItemObject InitializePreCraftedWeaponOnLoad(
      ItemObject itemObject,
      WeaponDesign craftedData,
      TextObject itemName,
      BasicCultureObject culture,
      Crafting.OverrideData overrideData)
    {
      Crafting crafting = new Crafting(craftedData.Template, culture);
      crafting._craftedWeaponName = itemName.ToString();
      crafting.CurrentWeaponDesign = craftedData;
      crafting._history = new List<WeaponDesign>()
      {
        craftedData
      };
      crafting.SetItemObject(overrideData, itemObject);
      return crafting._craftedItemObject;
    }

    public static ItemObject CreateRandomCraftedItem(BasicCultureObject culture)
    {
      Crafting crafting = new Crafting(CraftingTemplate.All.GetRandomElement<CraftingTemplate>(), culture);
      crafting.Init();
      crafting.Randomize();
      string hashedCode = crafting._craftedItemObject.WeaponDesign.HashedCode;
      crafting._craftedItemObject.StringId = hashedCode;
      return MBObjectManager.Instance.GetObject<ItemObject>(hashedCode) ?? MBObjectManager.Instance.RegisterObject<ItemObject>(crafting._craftedItemObject);
    }

    public class OverrideData
    {
      [SaveableField(10)]
      public float WeightOverriden;
      [SaveableField(20)]
      public int SwingSpeedOverriden;
      [SaveableField(30)]
      public int ThrustSpeedOverriden;
      [SaveableField(40)]
      public int SwingDamageOverriden;
      [SaveableField(50)]
      public int ThrustDamageOverriden;
      [SaveableField(60)]
      public int Handling;

      internal static void AutoGeneratedStaticCollectObjectsOverrideData(
        object o,
        List<object> collectedObjects)
      {
        ((Crafting.OverrideData) o).AutoGeneratedInstanceCollectObjects(collectedObjects);
      }

      protected virtual void AutoGeneratedInstanceCollectObjects(List<object> collectedObjects)
      {
      }

      internal static object AutoGeneratedGetMemberValueWeightOverriden(object o) => (object) ((Crafting.OverrideData) o).WeightOverriden;

      internal static object AutoGeneratedGetMemberValueSwingSpeedOverriden(object o) => (object) ((Crafting.OverrideData) o).SwingSpeedOverriden;

      internal static object AutoGeneratedGetMemberValueThrustSpeedOverriden(object o) => (object) ((Crafting.OverrideData) o).ThrustSpeedOverriden;

      internal static object AutoGeneratedGetMemberValueSwingDamageOverriden(object o) => (object) ((Crafting.OverrideData) o).SwingDamageOverriden;

      internal static object AutoGeneratedGetMemberValueThrustDamageOverriden(object o) => (object) ((Crafting.OverrideData) o).ThrustDamageOverriden;

      internal static object AutoGeneratedGetMemberValueHandling(object o) => (object) ((Crafting.OverrideData) o).Handling;

      public static Crafting.OverrideData Invalid => new Crafting.OverrideData();

      public OverrideData(
        float weightOverriden = 0.0f,
        int swingSpeedOverriden = 0,
        int thrustSpeedOverriden = 0,
        int swingDamageOverriden = 0,
        int thrustDamageOverriden = 0)
      {
        this.WeightOverriden = weightOverriden;
        this.SwingSpeedOverriden = swingSpeedOverriden;
        this.ThrustSpeedOverriden = thrustSpeedOverriden;
        this.SwingDamageOverriden = swingDamageOverriden;
        this.ThrustDamageOverriden = thrustDamageOverriden;
      }
    }

    public class RefiningFormula
    {
      public RefiningFormula(
        CraftingMaterials input1,
        int input1Count,
        CraftingMaterials input2,
        int input2Count,
        CraftingMaterials output,
        int outputCount = 1,
        CraftingMaterials output2 = CraftingMaterials.IronOre,
        int output2Count = 0)
      {
        this.Output = output;
        this.OutputCount = outputCount;
        this.Output2 = output2;
        this.Output2Count = output2Count;
        this.Input1 = input1;
        this.Input1Count = input1Count;
        this.Input2 = input2;
        this.Input2Count = input2Count;
      }

      public CraftingMaterials Output { get; }

      public int OutputCount { get; }

      public CraftingMaterials Output2 { get; }

      public int Output2Count { get; }

      public CraftingMaterials Input1 { get; }

      public int Input1Count { get; }

      public CraftingMaterials Input2 { get; }

      public int Input2Count { get; }
    }

    private static class CraftedItemGenerationHelper
    {
      public static ItemObject GenerateCraftedItem(
        ItemObject item,
        WeaponDesign craftedData,
        ItemModifierGroup itemModifierGroup,
        Crafting.OverrideData overridenData)
      {
        if (item == null)
          item = new ItemObject();
        foreach (WeaponDesignElement usedPiece in craftedData.UsedPieces)
        {
          if (usedPiece.IsValid && !craftedData.Template.Pieces.Contains<CraftingPiece>(usedPiece.CraftingPiece) || usedPiece.CraftingPiece.IsInitialized && !usedPiece.IsValid)
            return (ItemObject) null;
        }
        bool isAlternative = false;
        foreach (WeaponUsageData weaponUsageData1 in craftedData.Template.WeaponUsageDatas)
        {
          WeaponUsageData weaponUsageData = weaponUsageData1;
          if (!((IEnumerable<WeaponDesignElement>) craftedData.UsedPieces).Any<WeaponDesignElement>((Func<WeaponDesignElement, bool>) (piece => piece.IsValid && piece.CraftingPiece.UnavailableUsages.Any<string>((Func<string, bool>) (uc => uc == weaponUsageData.WeaponUsageDataId)))))
          {
            WeaponComponentData weapon = new WeaponComponentData(item, weaponUsageData.WeaponClass, weaponUsageData.WeaponFlags | craftedData.WeaponFlags);
            Crafting.CraftedItemGenerationHelper.CraftingStats.FillWeapon(item, weapon, weaponUsageData, isAlternative, overridenData);
            item.AddWeapon(weapon, itemModifierGroup);
            isAlternative = true;
          }
        }
        return item;
      }

      private struct CraftingStats
      {
        private WeaponDesign _craftedData;
        private WeaponUsageData _weaponUsageData;
        private float _stoppingTorque;
        private float _armInertia;
        private float _swingDamageFactor;
        private float _thrustDamageFactor;
        private float _currentWeaponWeight;
        private float _currentWeaponReach;
        private float _currentWeaponSweetSpot;
        private float _currentWeaponCenterOfMass;
        private float _currentWeaponInertia;
        private float _currentWeaponInertiaAroundShoulder;
        private float _currentWeaponInertiaAroundGrip;
        private float _currentWeaponSwingSpeed;
        private float _currentWeaponThrustSpeed;
        private float _currentWeaponHandling;
        private float _currentWeaponSwingDamage;
        private float _currentWeaponThrustDamage;
        private WeaponComponentData.WeaponTiers _currentWeaponTier;

        public static void FillWeapon(
          ItemObject item,
          WeaponComponentData weapon,
          WeaponUsageData weaponUsageData,
          bool isAlternative,
          Crafting.OverrideData overridenData)
        {
          Crafting.CraftedItemGenerationHelper.CraftingStats craftingStats = new Crafting.CraftedItemGenerationHelper.CraftingStats()
          {
            _craftedData = item.WeaponDesign,
            _weaponUsageData = weaponUsageData
          };
          craftingStats.CalculateStats(overridenData);
          craftingStats.SetWeaponData(weapon, isAlternative);
        }

        private void CalculateStats(Crafting.OverrideData overridenData)
        {
          WeaponUsageData weaponUsageData = this._weaponUsageData;
          WeaponDesign craftedData = this._craftedData;
          this._stoppingTorque = 10f;
          this._armInertia = 2.9f;
          if (this._weaponUsageData.WeaponFlags.HasAllFlags<WeaponFlags>(WeaponFlags.MeleeWeapon | WeaponFlags.NotUsableWithOneHand))
          {
            this._stoppingTorque *= 1.5f;
            this._armInertia *= 1.4f;
          }
          if (this._weaponUsageData.WeaponFlags.HasAllFlags<WeaponFlags>(WeaponFlags.MeleeWeapon | WeaponFlags.WideGrip))
          {
            this._stoppingTorque *= 1.5f;
            this._armInertia *= 1.4f;
          }
          this._currentWeaponWeight = 0.0f;
          this._currentWeaponReach = 0.0f;
          this._currentWeaponCenterOfMass = 0.0f;
          this._currentWeaponInertia = 0.0f;
          this._currentWeaponInertiaAroundShoulder = 0.0f;
          this._currentWeaponInertiaAroundGrip = 0.0f;
          this._currentWeaponSwingSpeed = 1f;
          this._currentWeaponThrustSpeed = 1f;
          this._currentWeaponSwingDamage = 0.0f;
          this._currentWeaponThrustDamage = 0.0f;
          this._currentWeaponHandling = 1f;
          this._currentWeaponTier = WeaponComponentData.WeaponTiers.Tier1;
          this._currentWeaponWeight = (float) Math.Round((double) ((IEnumerable<WeaponDesignElement>) craftedData.UsedPieces).Sum<WeaponDesignElement>((Func<WeaponDesignElement, float>) (selectedUsablePiece => selectedUsablePiece.ScaledWeight)), 2);
          this._currentWeaponReach = (float) Math.Round((double) this._craftedData.CraftedWeaponLength, 2);
          this._currentWeaponCenterOfMass = this.CalculateCenterOfMass();
          this._currentWeaponInertia = this.CalculateWeaponInertia();
          this._currentWeaponInertiaAroundShoulder = Crafting.CraftedItemGenerationHelper.CraftingStats.ParallelAxis(this._currentWeaponInertia, this._currentWeaponWeight, 0.5f + this._currentWeaponCenterOfMass);
          this._currentWeaponInertiaAroundGrip = Crafting.CraftedItemGenerationHelper.CraftingStats.ParallelAxis(this._currentWeaponInertia, this._currentWeaponWeight, this._currentWeaponCenterOfMass);
          this._currentWeaponSwingSpeed = this.CalculateSwingSpeed();
          this._currentWeaponThrustSpeed = this.CalculateThrustSpeed();
          this._currentWeaponHandling = (float) this.CalculateAgility();
          this._currentWeaponTier = this.CalculateWeaponTier();
          this._swingDamageFactor = this._craftedData.UsedPieces[0].CraftingPiece.BladeData.SwingDamageFactor;
          this._thrustDamageFactor = this._craftedData.UsedPieces[0].CraftingPiece.BladeData.ThrustDamageFactor;
          if (weaponUsageData.WeaponClass == WeaponClass.ThrowingAxe || weaponUsageData.WeaponClass == WeaponClass.ThrowingKnife || weaponUsageData.WeaponClass == WeaponClass.Javelin)
          {
            this._currentWeaponSwingDamage = 0.0f;
            this.CalculateMissileDamage(out this._currentWeaponThrustDamage);
          }
          else
          {
            this.CalculateSwingBaseDamage(out this._currentWeaponSwingDamage);
            this.CalculateThrustBaseDamage(out this._currentWeaponThrustDamage);
          }
          this._currentWeaponSwingSpeed += (float) overridenData.SwingSpeedOverriden / 4.545455f;
          this._currentWeaponThrustSpeed += (float) overridenData.ThrustSpeedOverriden / 11.76471f;
          this._currentWeaponSwingDamage += (float) overridenData.SwingDamageOverriden;
          this._currentWeaponThrustDamage += (float) overridenData.ThrustDamageOverriden;
          this._currentWeaponHandling += (float) overridenData.Handling;
          this._currentWeaponSweetSpot = this.CalculateSweetSpot();
          this._currentWeaponWeight += overridenData.WeightOverriden;
        }

        private void SetWeaponData(WeaponComponentData weapon, bool isAlternative)
        {
          BladeData bladeData = this._craftedData.UsedPieces[0].CraftingPiece.BladeData;
          short maxDataValue = 0;
          string passBySoundCode = "";
          int accuracy = 0;
          int missileSpeed = 0;
          MatrixFrame stickingFrame = MatrixFrame.Identity;
          if (this._weaponUsageData.WeaponClass == WeaponClass.Javelin || this._weaponUsageData.WeaponClass == WeaponClass.ThrowingAxe || this._weaponUsageData.WeaponClass == WeaponClass.ThrowingKnife)
          {
            short num = isAlternative ? (short) 1 : bladeData.StackAmount;
            switch (this._weaponUsageData.WeaponClass)
            {
              case WeaponClass.ThrowingAxe:
                maxDataValue = num;
                accuracy = 93;
                passBySoundCode = "event:/mission/combat/throwing/passby";
                break;
              case WeaponClass.ThrowingKnife:
                maxDataValue = num;
                accuracy = 95;
                passBySoundCode = "event:/mission/combat/throwing/passby";
                break;
              case WeaponClass.Javelin:
                maxDataValue = num;
                accuracy = 92;
                passBySoundCode = "event:/mission/combat/missile/passby";
                break;
            }
            missileSpeed = MBMath.Floor(this.CalculateMissileSpeed());
            Mat3 identity = Mat3.Identity;
            switch (this._weaponUsageData.WeaponClass)
            {
              case WeaponClass.ThrowingAxe:
                float bladeWidth = this._craftedData.UsedPieces[0].CraftingPiece.BladeData.BladeWidth;
                float piecePivotDistance = this._craftedData.PiecePivotDistances[0];
                float distanceToNextPiece = this._craftedData.UsedPieces[0].ScaledDistanceToNextPiece;
                identity.RotateAboutUp(1.570796f);
                identity.RotateAboutSide((float) (-(15.0 + (double) distanceToNextPiece * 3.0 / (double) piecePivotDistance * 25.0) * (Math.PI / 180.0)));
                stickingFrame = new MatrixFrame(identity, -identity.u * (piecePivotDistance + distanceToNextPiece * 0.6f) - identity.f * bladeWidth * 0.8f);
                break;
              case WeaponClass.ThrowingKnife:
                identity.RotateAboutForward(-1.570796f);
                stickingFrame = new MatrixFrame(identity, Vec3.Side * this._currentWeaponReach);
                break;
              case WeaponClass.Javelin:
                identity.RotateAboutSide(1.570796f);
                stickingFrame = new MatrixFrame(identity, -Vec3.Up * this._currentWeaponReach);
                break;
            }
          }
          if (this._weaponUsageData.WeaponClass == WeaponClass.Arrow || this._weaponUsageData.WeaponClass == WeaponClass.Bolt)
            stickingFrame.rotation.RotateAboutSide(1.570796f);
          Vec3 rotationSpeed = Vec3.Zero;
          if (this._weaponUsageData.WeaponClass == WeaponClass.ThrowingAxe)
            rotationSpeed = new Vec3(y: 18f);
          else if (this._weaponUsageData.WeaponClass == WeaponClass.ThrowingKnife)
            rotationSpeed = new Vec3(y: 24f);
          weapon.Init(this._weaponUsageData.WeaponUsageDataId, bladeData.PhysicsMaterial, this.GetItemUsage(), bladeData.ThrustDamageType, bladeData.SwingDamageType, this.GetWeaponHandArmorBonus(), (int) ((double) this._currentWeaponReach * 100.0), (float) Math.Round((double) this.GetWeaponBalance(), 2), this._currentWeaponInertia, this._currentWeaponCenterOfMass, MBMath.Floor(this._currentWeaponHandling), (float) Math.Round((double) this._swingDamageFactor, 2), (float) Math.Round((double) this._thrustDamageFactor, 2), maxDataValue, passBySoundCode, accuracy, missileSpeed, stickingFrame, this.GetAmmoClass(), this._currentWeaponSweetSpot, MBMath.Floor(this._currentWeaponSwingSpeed * 4.545455f), MBMath.Round(this._currentWeaponSwingDamage), MBMath.Floor(this._currentWeaponThrustSpeed * 11.76471f), MBMath.Round(this._currentWeaponThrustDamage), rotationSpeed, this._currentWeaponTier);
          Mat3 identity1 = Mat3.Identity;
          Vec3 v = Vec3.Zero;
          if (this._weaponUsageData.RotatedInHand)
            identity1.RotateAboutSide(3.141593f);
          if (this._weaponUsageData.UseCenterOfMassAsHandBase)
            v = -Vec3.Up * this._currentWeaponCenterOfMass;
          weapon.SetFrame(new MatrixFrame(identity1, identity1.TransformToParent(v)));
        }

        private float CalculateSweetSpot()
        {
          float num1 = -1f;
          float num2 = -1f;
          for (int index = 0; index < 100; ++index)
          {
            float impactPointAsPercent = 0.01f * (float) index;
            float magnitudeForSwing = CombatStatCalculator.CalculateStrikeMagnitudeForSwing(this._currentWeaponSwingSpeed, impactPointAsPercent, this._currentWeaponWeight, this._currentWeaponReach, this._currentWeaponInertia, this._currentWeaponCenterOfMass, 0.0f);
            if ((double) num1 < (double) magnitudeForSwing)
            {
              num1 = magnitudeForSwing;
              num2 = impactPointAsPercent;
            }
          }
          return num2;
        }

        private float CalculateCenterOfMass()
        {
          float num1 = 0.0f;
          float num2 = 0.0f;
          float num3 = 0.0f;
          foreach (PieceData buildOrder in this._craftedData.Template.BuildOrders)
          {
            WeaponDesignElement usedPiece = this._craftedData.UsedPieces[(int) buildOrder.PieceType];
            if (usedPiece.IsValid)
            {
              float scaledWeight = usedPiece.ScaledWeight;
              float num4 = 0.0f;
              float num5;
              if (buildOrder.Order < 0)
              {
                num5 = num4 - (num3 + (usedPiece.ScaledLength - usedPiece.ScaledCenterOfMass)) * scaledWeight;
                num3 += usedPiece.ScaledLength;
              }
              else
              {
                num5 = num4 + (num2 + usedPiece.ScaledCenterOfMass) * scaledWeight;
                num2 += usedPiece.ScaledLength;
              }
              num1 += num5;
            }
          }
          return (float) ((double) num1 / (double) this._currentWeaponWeight - ((double) this._craftedData.UsedPieces[2].ScaledDistanceToPreviousPiece - (double) this._craftedData.UsedPieces[2].ScaledPieceOffset));
        }

        private float CalculateWeaponInertia()
        {
          float offset = -this._currentWeaponCenterOfMass;
          float num = 0.0f;
          foreach (PieceData buildOrder in this._craftedData.Template.BuildOrders)
          {
            WeaponDesignElement usedPiece = this._craftedData.UsedPieces[(int) buildOrder.PieceType];
            if (usedPiece.IsValid)
            {
              float weightMultiplier = 1f;
              num += Crafting.CraftedItemGenerationHelper.CraftingStats.ParallelAxis(usedPiece, offset, weightMultiplier);
              offset += usedPiece.ScaledLength;
            }
          }
          return num;
        }

        private float CalculateSwingSpeed()
        {
          float num1 = (float) (1.0 * (double) this._currentWeaponInertiaAroundShoulder + 0.899999976158142);
          double usablePower1 = 200.0;
          double usablePower2 = 170.0;
          double usablePower3 = 90.0;
          double num2 = 27.0;
          double num3 = 15.0;
          double num4 = 7.0;
          if (this._weaponUsageData.IsTwoHandedCloseWeapon)
          {
            ++num1;
            num4 *= 2.40000009536743;
            num3 *= 1.29999995231628;
            usablePower3 *= 1.35;
            usablePower2 *= 1.14999997615814;
          }
          else if (this._weaponUsageData.IsTwoHandedPolearm)
          {
            num1 += 1.5f;
            num4 *= 4.0;
            num3 *= 1.70000004768372;
            usablePower3 *= 1.3;
            usablePower2 *= 1.14999997615814;
          }
          double maxUsableTorque1 = Math.Max(1.0, num2 - (double) num1);
          double maxUsableTorque2 = Math.Max(1.0, num3 - (double) num1);
          double maxUsableTorque3 = Math.Max(1.0, num4 - (double) num1);
          double finalTime1;
          this.SimulateSwingLayer(1.5, usablePower1, maxUsableTorque1, 2.0 + (double) num1, out double _, out finalTime1);
          double finalTime2;
          this.SimulateSwingLayer(1.5, usablePower2, maxUsableTorque2, 1.0 + (double) num1, out double _, out finalTime2);
          double finalTime3;
          this.SimulateSwingLayer(1.5, usablePower3, maxUsableTorque3, 0.5 + (double) num1, out double _, out finalTime3);
          return (float) (20.8 / (0.33 * (finalTime1 + finalTime2 + finalTime3)));
        }

        private float CalculateThrustSpeed()
        {
          float num = (float) (1.79999995231628 + (double) this._currentWeaponWeight + (double) this._currentWeaponInertiaAroundGrip * 0.200000002980232);
          double usablePower1 = 250.0;
          double usablePower2 = 170.0;
          double usablePower3 = 90.0;
          double maxUsableForce1 = 48.0;
          double maxUsableForce2 = 24.0;
          double maxUsableForce3 = 15.0;
          if (this._weaponUsageData.IsTwoHandedCloseWeapon)
          {
            num += 0.6f;
            maxUsableForce3 *= 1.9;
            maxUsableForce2 *= 1.10000002384186;
            usablePower3 *= 1.2;
            usablePower2 *= 1.04999995231628;
          }
          else if (this._weaponUsageData.IsTwoHandedPolearm)
          {
            num += 0.9f;
            maxUsableForce3 *= 2.1;
            maxUsableForce2 *= 1.20000004768372;
            usablePower3 *= 1.2;
            usablePower2 *= 1.04999995231628;
          }
          double finalTime1;
          this.SimulateThrustLayer(0.6, usablePower1, maxUsableForce1, 4.0 + (double) num, out double _, out finalTime1);
          double finalTime2;
          this.SimulateThrustLayer(0.6, usablePower2, maxUsableForce2, 2.0 + (double) num, out double _, out finalTime2);
          double finalTime3;
          this.SimulateThrustLayer(0.6, usablePower3, maxUsableForce3, 0.5 + (double) num, out double _, out finalTime3);
          return (float) (3.85 / (0.33 * (finalTime1 + finalTime2 + finalTime3)));
        }

        private void CalculateSwingBaseDamage(out float damage)
        {
          float num = 0.0f;
          for (float impactPoint = 0.93f; (double) impactPoint > 0.5; impactPoint -= 0.05f)
          {
            float magnitudeForSwing = CombatStatCalculator.CalculateBaseBlowMagnitudeForSwing(this._currentWeaponSwingSpeed, this._currentWeaponReach, this._currentWeaponWeight, this._currentWeaponInertia, this._currentWeaponCenterOfMass, impactPoint, 0.0f);
            if ((double) magnitudeForSwing > (double) num)
              num = magnitudeForSwing;
          }
          damage = num * this._swingDamageFactor;
        }

        private void CalculateThrustBaseDamage(out float damage, bool isThrown = false)
        {
          float magnitudeForThrust = Game.Current.BasicModels.StrikeMagnitudeModel.CalculateStrikeMagnitudeForThrust((BasicCharacterObject) null, (BasicCharacterObject) null, this._currentWeaponThrustSpeed, this._currentWeaponWeight, 0.0f, false, WeaponClass.Undefined, isThrown);
          damage = magnitudeForThrust * this._thrustDamageFactor;
        }

        private void CalculateMissileDamage(out float damage)
        {
          switch (this._weaponUsageData.WeaponClass)
          {
            case WeaponClass.ThrowingAxe:
              this.CalculateSwingBaseDamage(out damage);
              damage *= 2f;
              break;
            case WeaponClass.ThrowingKnife:
              this.CalculateThrustBaseDamage(out damage, true);
              damage *= 3.3f;
              break;
            case WeaponClass.Javelin:
              this.CalculateThrustBaseDamage(out damage, true);
              damage *= 9f;
              break;
            default:
              damage = 0.0f;
              break;
          }
        }

        private WeaponComponentData.WeaponTiers CalculateWeaponTier()
        {
          int num1 = 0;
          int num2 = 0;
          foreach (WeaponDesignElement weaponDesignElement in ((IEnumerable<WeaponDesignElement>) this._craftedData.UsedPieces).Where<WeaponDesignElement>((Func<WeaponDesignElement, bool>) (ucp => ucp.IsValid)))
          {
            num1 += weaponDesignElement.CraftingPiece.PieceTier;
            ++num2;
          }
          WeaponComponentData.WeaponTiers result;
          return Enum.TryParse<WeaponComponentData.WeaponTiers>(((int) ((double) num1 / (double) num2)).ToString(), out result) ? result : WeaponComponentData.WeaponTiers.Tier1;
        }

        private string GetItemUsage()
        {
          List<string> list = ((IEnumerable<string>) this._weaponUsageData.ItemUsageFeatures.Split(':')).ToList<string>();
          foreach (WeaponDesignElement weaponDesignElement in ((IEnumerable<WeaponDesignElement>) this._craftedData.UsedPieces).Where<WeaponDesignElement>((Func<WeaponDesignElement, bool>) (ucp => ucp.IsValid)))
          {
            string featuresToExclude = weaponDesignElement.CraftingPiece.ItemUsageFeaturesToExclude;
            char[] chArray = new char[1]{ ':' };
            foreach (string str in featuresToExclude.Split(chArray))
            {
              if (!str.IsStringNoneOrEmpty())
                list.Remove(str);
            }
          }
          string str1 = "";
          for (int index = 0; index < list.Count; ++index)
          {
            str1 += list[index];
            if (index < list.Count - 1)
              str1 += "_";
          }
          return str1;
        }

        private float CalculateMissileSpeed()
        {
          if (this._weaponUsageData.WeaponClass == WeaponClass.ThrowingAxe)
            return this._currentWeaponThrustSpeed * 3.2f;
          if (this._weaponUsageData.WeaponClass == WeaponClass.ThrowingKnife)
            return this._currentWeaponThrustSpeed * 3.9f;
          return this._weaponUsageData.WeaponClass == WeaponClass.Javelin ? this._currentWeaponThrustSpeed * 3.6f : 10f;
        }

        private int CalculateAgility()
        {
          float inertiaAroundGrip = this._currentWeaponInertiaAroundGrip;
          return MathF.Round(100f * ((float) Math.Pow(1.0 / (!this._weaponUsageData.WeaponFlags.HasAllFlags<WeaponFlags>(WeaponFlags.MeleeWeapon | WeaponFlags.NotUsableWithOneHand) ? (!this._weaponUsageData.WeaponFlags.HasAllFlags<WeaponFlags>(WeaponFlags.MeleeWeapon | WeaponFlags.WideGrip) ? (double) (inertiaAroundGrip + 0.7f) : (double) (inertiaAroundGrip * 0.4f + 1f)) : (double) (inertiaAroundGrip * 0.5f + 0.9f)), 0.55) * 1f));
        }

        private float GetWeaponBalance() => MBMath.ClampFloat((float) (((double) this._currentWeaponSwingSpeed * 4.54545450210571 - 70.0) / 30.0), 0.0f, 1f);

        private int GetWeaponHandArmorBonus()
        {
          WeaponDesignElement usedPiece = this._craftedData.UsedPieces[1];
          return usedPiece == null ? 0 : usedPiece.CraftingPiece.ArmorBonus;
        }

        private WeaponClass GetAmmoClass() => this._weaponUsageData.WeaponClass != WeaponClass.ThrowingKnife && this._weaponUsageData.WeaponClass != WeaponClass.ThrowingAxe && this._weaponUsageData.WeaponClass != WeaponClass.Javelin ? WeaponClass.Undefined : this._weaponUsageData.WeaponClass;

        private static float ParallelAxis(
          WeaponDesignElement selectedPiece,
          float offset,
          float weightMultiplier)
        {
          double inertia = (double) selectedPiece.CraftingPiece.Inertia;
          float num1 = offset + selectedPiece.CraftingPiece.CenterOfMass;
          double num2 = (double) (selectedPiece.ScaledWeight * weightMultiplier);
          double num3 = (double) num1;
          return Crafting.CraftedItemGenerationHelper.CraftingStats.ParallelAxis((float) inertia, (float) num2, (float) num3);
        }

        private static float ParallelAxis(float inertiaAroundCm, float mass, float offsetFromCm) => inertiaAroundCm + mass * offsetFromCm * offsetFromCm;

        private void SimulateSwingLayer(
          double angleSpan,
          double usablePower,
          double maxUsableTorque,
          double inertia,
          out double finalSpeed,
          out double finalTime)
        {
          double num1 = 0.0;
          double num2 = 0.01;
          float num3 = 0.0f;
          float num4 = 0.01f;
          float num5 = (float) (3.90000009536743 * (double) this._currentWeaponReach * (this._weaponUsageData.WeaponFlags.HasAllFlags<WeaponFlags>(WeaponFlags.MeleeWeapon | WeaponFlags.WideGrip) ? 1.0 : 0.300000011920929));
          while (num1 < angleSpan)
          {
            double num6 = usablePower / num2;
            if (num6 > maxUsableTorque)
              num6 = maxUsableTorque;
            double num7 = num6 - num2 * (double) num5;
            double num8 = (double) num4 * num7 / inertia;
            num2 += num8;
            num1 += num2 * (double) num4;
            num3 += num4;
          }
          finalSpeed = num2;
          finalTime = (double) num3;
        }

        private void SimulateThrustLayer(
          double distance,
          double usablePower,
          double maxUsableForce,
          double mass,
          out double finalSpeed,
          out double finalTime)
        {
          double num1 = 0.0;
          double num2 = 0.01;
          float num3 = 0.0f;
          float num4 = 0.01f;
          while (num1 < distance)
          {
            double num5 = usablePower / num2;
            if (num5 > maxUsableForce)
              num5 = maxUsableForce;
            double num6 = (double) num4 * num5 / mass;
            num2 += num6;
            num1 += num2 * (double) num4;
            num3 += num4;
          }
          finalSpeed = num2;
          finalTime = (double) num3;
        }
      }
    }
  }
}
