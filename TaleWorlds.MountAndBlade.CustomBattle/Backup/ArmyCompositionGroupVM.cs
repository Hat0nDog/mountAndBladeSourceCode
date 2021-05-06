// Decompiled with JetBrains decompiler
// Type: TaleWorlds.MountAndBlade.CustomBattle.ArmyCompositionGroupVM
// Assembly: TaleWorlds.MountAndBlade.CustomBattle, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 8065FEE3-AE77-4E53-B13C-3890101BA431
// Assembly location: E:\SteamLibrary\steamapps\common\Mount & Blade II Bannerlord\Modules\CustomBattle\bin\Win64_Shipping_Client\TaleWorlds.MountAndBlade.CustomBattle.dll

using System;
using System.Collections.Generic;
using System.Linq;
using TaleWorlds.Core;
using TaleWorlds.Library;
using TaleWorlds.Localization;
using TaleWorlds.MountAndBlade.CustomBattle.CustomBattle;

namespace TaleWorlds.MountAndBlade.CustomBattle
{
  public class ArmyCompositionGroupVM : ViewModel
  {
    private readonly bool _isPlayerSide;
    private bool updatingSliders;
    private BasicCultureObject _selectedCulture;
    private List<BasicCharacterObject> _allCharacterObjects = new List<BasicCharacterObject>();
    private List<BasicCharacterObject> _meleeInfantryTypes = new List<BasicCharacterObject>();
    private List<BasicCharacterObject> _rangedInfantryTypes = new List<BasicCharacterObject>();
    private List<BasicCharacterObject> _meleeCavalryTypes = new List<BasicCharacterObject>();
    private List<BasicCharacterObject> _rangedCavalryTypes = new List<BasicCharacterObject>();
    private int[] _values;
    private int _armySize;
    private int _maxArmySize;
    private int _minArmySize;
    private string _armySizeText;
    private string _armyComposition1Text;
    private string _armyComposition2Text;
    private string _armyComposition3Text;
    private string _armyComposition4Text;
    private bool _isArmyComposition1Enabled;
    private bool _isArmyComposition2Enabled;
    private bool _isArmyComposition3Enabled;
    private bool _isArmyComposition4Enabled;
    private string _name;
    private string _armySizeTitle;

    public List<BasicCharacterObject> SelectedMeleeInfantryTypes { get; private set; }

    public List<BasicCharacterObject> SelectedRangedInfantryTypes { get; private set; }

    public List<BasicCharacterObject> SelectedMeleeCavalryTypes { get; private set; }

    public List<BasicCharacterObject> SelectedRangedCavalryTypes { get; private set; }

    public ArmyCompositionGroupVM(string name, bool isPlayerSide)
    {
      this._isPlayerSide = isPlayerSide;
      this.MinArmySize = 1;
      this.MaxArmySize = (int) Math.Round((double) BannerlordConfig.GetRealBattleSize() / 2.0);
      this._armySizeText = this.MinArmySize.ToString() + " men";
      Game.Current.ObjectManager.GetAllInstancesOfObjectType<BasicCharacterObject>(ref this._allCharacterObjects);
      this._allCharacterObjects = this._allCharacterObjects.Where<BasicCharacterObject>((Func<BasicCharacterObject, bool>) (c => c.IsSoldier)).ToList<BasicCharacterObject>();
      this.SelectedMeleeInfantryTypes = new List<BasicCharacterObject>();
      this.SelectedRangedInfantryTypes = new List<BasicCharacterObject>();
      this.SelectedMeleeCavalryTypes = new List<BasicCharacterObject>();
      this.SelectedRangedCavalryTypes = new List<BasicCharacterObject>();
      this._armySize = 1;
      this._name = name;
      this._values = new int[4];
      this._values[0] = 25;
      this._values[1] = 25;
      this._values[2] = 25;
      this._values[3] = 25;
      this.ArmyComposition1Text = "%" + this._values[0].ToString();
      this.ArmyComposition2Text = "%" + this._values[1].ToString();
      this.ArmyComposition3Text = "%" + this._values[2].ToString();
      this.ArmyComposition4Text = "%" + this._values[3].ToString();
      this.RefreshValues();
    }

    public override void RefreshValues()
    {
      base.RefreshValues();
      this.ArmySizeTitle = GameTexts.FindText("str_army_size").ToString();
    }

    private static int SumOfValues(int[] array, bool[] enabledArray, int excludedIndex = -1)
    {
      int num = 0;
      for (int index = 0; index < array.Length; ++index)
      {
        if (enabledArray[index] && excludedIndex != index)
          num += array[index];
      }
      return num;
    }

    private void ExecuteMeleeInfantryTypeSelection()
    {
      if (!Game.Current.IsDevelopmentMode)
        return;
      List<InquiryElement> inquiryElements = new List<InquiryElement>();
      foreach (BasicCharacterObject meleeInfantryType in this._meleeInfantryTypes)
      {
        ImageIdentifier imageIdentifier = new ImageIdentifier(CharacterCode.CreateFrom(meleeInfantryType));
        inquiryElements.Add(new InquiryElement((object) meleeInfantryType, meleeInfantryType.Name.ToString(), imageIdentifier));
      }
      InformationManager.ShowMultiSelectionInquiry(new MultiSelectionInquiryData("Melee Infantry Troop Types", string.Empty, inquiryElements, true, -1, "Done", "", new Action<List<InquiryElement>>(this.OnMeleeInfantryTypeSelectionOver), new Action<List<InquiryElement>>(this.OnMeleeInfantryTypeSelectionOver)));
    }

    private void OnMeleeInfantryTypeSelectionOver(List<InquiryElement> selectedElements)
    {
      this.SelectedMeleeInfantryTypes.Clear();
      foreach (InquiryElement selectedElement in selectedElements)
        this.SelectedMeleeInfantryTypes.Add(selectedElement.Identifier as BasicCharacterObject);
    }

    private void ExecuteRangedInfantryTypeSelection()
    {
      if (!Game.Current.IsDevelopmentMode)
        return;
      List<InquiryElement> inquiryElements = new List<InquiryElement>();
      foreach (BasicCharacterObject rangedInfantryType in this._rangedInfantryTypes)
      {
        ImageIdentifier imageIdentifier = new ImageIdentifier(CharacterCode.CreateFrom(rangedInfantryType));
        inquiryElements.Add(new InquiryElement((object) rangedInfantryType, rangedInfantryType.Name.ToString(), imageIdentifier));
      }
      InformationManager.ShowMultiSelectionInquiry(new MultiSelectionInquiryData("Ranged Infantry Troop Types", string.Empty, inquiryElements, true, -1, "Done", "", new Action<List<InquiryElement>>(this.OnRangedInfantryTypeSelectionOver), new Action<List<InquiryElement>>(this.OnRangedInfantryTypeSelectionOver)));
    }

    private void OnRangedInfantryTypeSelectionOver(List<InquiryElement> selectedElements)
    {
      this.SelectedRangedInfantryTypes.Clear();
      foreach (InquiryElement selectedElement in selectedElements)
        this.SelectedRangedInfantryTypes.Add(selectedElement.Identifier as BasicCharacterObject);
    }

    private void ExecuteMeleeCavalryTypeSelection()
    {
      if (!Game.Current.IsDevelopmentMode)
        return;
      List<InquiryElement> inquiryElements = new List<InquiryElement>();
      foreach (BasicCharacterObject meleeCavalryType in this._meleeCavalryTypes)
      {
        ImageIdentifier imageIdentifier = new ImageIdentifier(CharacterCode.CreateFrom(meleeCavalryType));
        inquiryElements.Add(new InquiryElement((object) meleeCavalryType, meleeCavalryType.Name.ToString(), imageIdentifier));
      }
      InformationManager.ShowMultiSelectionInquiry(new MultiSelectionInquiryData("Melee Cavalry Troop Types", string.Empty, inquiryElements, true, -1, "Done", "", new Action<List<InquiryElement>>(this.OnMeleeCavalryTypeSelectionOver), new Action<List<InquiryElement>>(this.OnMeleeCavalryTypeSelectionOver)));
    }

    private void OnMeleeCavalryTypeSelectionOver(List<InquiryElement> selectedElements)
    {
      this.SelectedMeleeCavalryTypes.Clear();
      foreach (InquiryElement selectedElement in selectedElements)
        this.SelectedMeleeCavalryTypes.Add(selectedElement.Identifier as BasicCharacterObject);
    }

    private void ExecuteRangedCavalryTypeSelection()
    {
      if (!Game.Current.IsDevelopmentMode)
        return;
      List<InquiryElement> inquiryElements = new List<InquiryElement>();
      foreach (BasicCharacterObject rangedCavalryType in this._rangedCavalryTypes)
      {
        ImageIdentifier imageIdentifier = new ImageIdentifier(CharacterCode.CreateFrom(rangedCavalryType));
        inquiryElements.Add(new InquiryElement((object) rangedCavalryType, rangedCavalryType.Name.ToString(), imageIdentifier));
      }
      InformationManager.ShowMultiSelectionInquiry(new MultiSelectionInquiryData("Ranged Cavalry Troop Types", string.Empty, inquiryElements, true, -1, "Done", "", new Action<List<InquiryElement>>(this.OnRangedCavalryTypeSelectionOver), new Action<List<InquiryElement>>(this.OnRangedCavalryTypeSelectionOver)));
    }

    private void OnRangedCavalryTypeSelectionOver(List<InquiryElement> selectedElements)
    {
      this.SelectedRangedCavalryTypes.Clear();
      foreach (InquiryElement selectedElement in selectedElements)
        this.SelectedRangedCavalryTypes.Add(selectedElement.Identifier as BasicCharacterObject);
    }

    public void SetCurrentSelectedCulture(BasicCultureObject selectedCulture)
    {
      if (this._selectedCulture == selectedCulture)
        return;
      this.PopulateTroopTypeList(ArmyCompositionGroupVM.TroopType.MeleeInfantry, selectedCulture);
      this.PopulateTroopTypeList(ArmyCompositionGroupVM.TroopType.RangedInfantry, selectedCulture);
      this.PopulateTroopTypeList(ArmyCompositionGroupVM.TroopType.MeleeCavalry, selectedCulture);
      this.PopulateTroopTypeList(ArmyCompositionGroupVM.TroopType.RangedCavalry, selectedCulture);
      this._selectedCulture = selectedCulture;
    }

    private void PopulateTroopTypeList(
      ArmyCompositionGroupVM.TroopType troopType,
      BasicCultureObject cultureOfTroops)
    {
      if (troopType == ArmyCompositionGroupVM.TroopType.MeleeInfantry)
      {
        this._meleeInfantryTypes.Clear();
        this._meleeInfantryTypes.AddRange(this._allCharacterObjects.Where<BasicCharacterObject>((Func<BasicCharacterObject, bool>) (o => this.IsValidUnitItem(o, cultureOfTroops, troopType))));
        this.SelectedMeleeInfantryTypes.Clear();
        this.SelectedMeleeInfantryTypes.Add(CustomBattleState.Helper.GetDefaultTroopOfFormationForFaction(cultureOfTroops, FormationClass.Infantry));
      }
      else if (troopType == ArmyCompositionGroupVM.TroopType.RangedInfantry)
      {
        this._rangedInfantryTypes.Clear();
        this._rangedInfantryTypes.AddRange(this._allCharacterObjects.Where<BasicCharacterObject>((Func<BasicCharacterObject, bool>) (o => this.IsValidUnitItem(o, cultureOfTroops, troopType))));
        this.SelectedRangedInfantryTypes.Clear();
        this.SelectedRangedInfantryTypes.Add(CustomBattleState.Helper.GetDefaultTroopOfFormationForFaction(cultureOfTroops, FormationClass.Ranged));
      }
      else if (troopType == ArmyCompositionGroupVM.TroopType.MeleeCavalry)
      {
        this._meleeCavalryTypes.Clear();
        this._meleeCavalryTypes.AddRange(this._allCharacterObjects.Where<BasicCharacterObject>((Func<BasicCharacterObject, bool>) (o => this.IsValidUnitItem(o, cultureOfTroops, troopType))));
        this.SelectedMeleeCavalryTypes.Clear();
        this.SelectedMeleeCavalryTypes.Add(CustomBattleState.Helper.GetDefaultTroopOfFormationForFaction(cultureOfTroops, FormationClass.Cavalry));
      }
      else
      {
        if (troopType != ArmyCompositionGroupVM.TroopType.RangedCavalry)
          return;
        this._rangedCavalryTypes.Clear();
        this._rangedCavalryTypes.AddRange(this._allCharacterObjects.Where<BasicCharacterObject>((Func<BasicCharacterObject, bool>) (o => this.IsValidUnitItem(o, cultureOfTroops, troopType))));
        this.SelectedRangedCavalryTypes.Clear();
        this.SelectedRangedCavalryTypes.Add(CustomBattleState.Helper.GetDefaultTroopOfFormationForFaction(cultureOfTroops, FormationClass.HorseArcher));
      }
    }

    private bool IsValidUnitItem(
      BasicCharacterObject o,
      BasicCultureObject culture,
      ArmyCompositionGroupVM.TroopType troopType)
    {
      if (o == null || culture != o.Culture)
        return false;
      switch (troopType)
      {
        case ArmyCompositionGroupVM.TroopType.MeleeInfantry:
          return o.DefaultFormationClass == FormationClass.Infantry || o.DefaultFormationClass == FormationClass.HeavyInfantry;
        case ArmyCompositionGroupVM.TroopType.RangedInfantry:
          return o.DefaultFormationClass == FormationClass.Ranged;
        case ArmyCompositionGroupVM.TroopType.MeleeCavalry:
          return o.DefaultFormationClass == FormationClass.Cavalry || o.DefaultFormationClass == FormationClass.HeavyCavalry || o.DefaultFormationClass == FormationClass.LightCavalry;
        case ArmyCompositionGroupVM.TroopType.RangedCavalry:
          return o.DefaultFormationClass == FormationClass.HorseArcher;
        default:
          return false;
      }
    }

    private static float SumOfValues(float[] array)
    {
      float num = 0.0f;
      for (int index = 0; index < array.Length; ++index)
        num += array[index];
      return num;
    }

    private void UpdateSliders(int value, int changedSliderIndex)
    {
      this.updatingSliders = true;
      bool[] enabledArray = new bool[4]
      {
        !this.IsArmyComposition1Enabled,
        !this.IsArmyComposition2Enabled,
        !this.IsArmyComposition3Enabled,
        !this.IsArmyComposition4Enabled
      };
      int[] array = new int[4]
      {
        this._values[0],
        this._values[1],
        this._values[2],
        this._values[3]
      };
      int[] numArray = new int[4]
      {
        this._values[0],
        this._values[1],
        this._values[2],
        this._values[3]
      };
      int num1 = ((IEnumerable<bool>) enabledArray).Count<bool>((Func<bool, bool>) (s => s));
      if (enabledArray[changedSliderIndex])
        --num1;
      if (num1 > 0)
      {
        int num2 = ArmyCompositionGroupVM.SumOfValues(array, enabledArray);
        if (value >= num2)
          value = num2;
        int num3 = value - array[changedSliderIndex];
        if (num3 != 0)
        {
          int num4 = ArmyCompositionGroupVM.SumOfValues(array, enabledArray, changedSliderIndex);
          int num5 = num4 - num3;
          if (num5 > 0)
          {
            int num6 = 0;
            numArray[changedSliderIndex] = value;
            for (int index = 0; index < enabledArray.Length; ++index)
            {
              if (changedSliderIndex != index && enabledArray[index] && array[index] != 0)
              {
                int num7 = MathF.Round((float) array[index] / (float) num4 * (float) num5);
                num6 += num7;
                numArray[index] = num7;
              }
            }
            int num8 = num5 - num6;
            if (num8 != 0)
            {
              int num7 = 0;
              for (int index = 0; index < enabledArray.Length; ++index)
              {
                if (enabledArray[index] && index != changedSliderIndex && (0 < array[index] + num8 && 100 > array[index] + num8))
                  ++num7;
              }
              for (int index = 0; index < enabledArray.Length; ++index)
              {
                if (enabledArray[index] && index != changedSliderIndex && (0 < array[index] + num8 && 100 > array[index] + num8))
                {
                  int num9 = (int) Math.Round((double) num8 / (double) num7);
                  numArray[index] += num9;
                  num8 -= num9;
                }
              }
              if (num8 != 0)
              {
                for (int index = 0; index < enabledArray.Length; ++index)
                {
                  if (enabledArray[index] && index != changedSliderIndex && (0 <= array[index] + num8 && 100 >= array[index] + num8))
                  {
                    numArray[index] += num8;
                    break;
                  }
                }
              }
            }
          }
          else
          {
            numArray[changedSliderIndex] = value;
            for (int index = 0; index < enabledArray.Length; ++index)
            {
              if (changedSliderIndex != index && enabledArray[index])
                numArray[index] = 0;
            }
          }
        }
      }
      this.SetArmyCompositionValue(0, numArray[0]);
      this.SetArmyCompositionValue(1, numArray[1]);
      this.SetArmyCompositionValue(2, numArray[2]);
      this.SetArmyCompositionValue(3, numArray[3]);
      this.updatingSliders = false;
    }

    private int CalculateNumOfAvailableSliders(
      bool isAvailableToIncrease,
      int indexOfSliderToExclude)
    {
      int num = 0;
      if (isAvailableToIncrease)
      {
        if ((double) this.ArmyComposition1Value < 100.0 && indexOfSliderToExclude != 1)
          ++num;
        if ((double) this.ArmyComposition2Value < 100.0 && indexOfSliderToExclude != 2)
          ++num;
        if ((double) this.ArmyComposition3Value < 100.0 && indexOfSliderToExclude != 3)
          ++num;
        if ((double) this.ArmyComposition4Value < 100.0 && indexOfSliderToExclude != 4)
          ++num;
      }
      else
      {
        if ((double) this.ArmyComposition1Value > 0.0 && indexOfSliderToExclude != 1)
          ++num;
        if ((double) this.ArmyComposition2Value > 0.0 && indexOfSliderToExclude != 2)
          ++num;
        if ((double) this.ArmyComposition3Value > 0.0 && indexOfSliderToExclude != 3)
          ++num;
        if ((double) this.ArmyComposition4Value > 0.0 && indexOfSliderToExclude != 4)
          ++num;
      }
      return num;
    }

    public void RandomizeArmySize() => this.ArmySize = MBRandom.RandomInt(100);

    internal void OnPlayerTypeChange(CustomBattlePlayerType playerType)
    {
      this.MinArmySize = playerType == CustomBattlePlayerType.Commander ? 1 : 2;
      this.ArmySize = this._armySize;
    }

    private void OnArmySizeValueChange(int value)
    {
      TextObject textObject = new TextObject("{=mxbq3InD}{ARMY_SIZE} men");
      textObject.SetTextVariable("ARMY_SIZE", value);
      this.ArmySizeText = textObject.ToString();
    }

    [DataSourceProperty]
    public string ArmySizeTitle
    {
      get => this._armySizeTitle;
      set
      {
        if (!(value != this._armySizeTitle))
          return;
        this._armySizeTitle = value;
        this.OnPropertyChangedWithValue((object) value, nameof (ArmySizeTitle));
      }
    }

    public int ArmySize
    {
      get => this._armySize;
      set
      {
        if (this._armySize == (int) MathF.Clamp((float) value, (float) this.MinArmySize, (float) this.MaxArmySize))
          return;
        this._armySize = (int) MathF.Clamp((float) value, (float) this.MinArmySize, (float) this.MaxArmySize);
        this.OnPropertyChangedWithValue((object) value, nameof (ArmySize));
        this.OnArmySizeValueChange(value);
      }
    }

    public int MaxArmySize
    {
      get => this._maxArmySize;
      set
      {
        if (this._maxArmySize == value)
          return;
        this._maxArmySize = value;
        this.OnPropertyChangedWithValue((object) value, nameof (MaxArmySize));
      }
    }

    public int MinArmySize
    {
      get => this._minArmySize;
      set
      {
        if (this._minArmySize == value)
          return;
        this._minArmySize = value;
        this.OnPropertyChangedWithValue((object) value, nameof (MinArmySize));
      }
    }

    public string ArmySizeText
    {
      get => this._armySizeText;
      set
      {
        if (!(this._armySizeText != value))
          return;
        this._armySizeText = value;
        this.OnPropertyChangedWithValue((object) value, nameof (ArmySizeText));
      }
    }

    private void CheckAndSet(int value, int index)
    {
      if (this._values[index] == value || this.updatingSliders)
        return;
      this.UpdateSliders(value, index);
    }

    public int ArmyComposition1Value
    {
      get => this._values[0];
      set => this.CheckAndSet(value, 0);
    }

    public int ArmyComposition2Value
    {
      get => this._values[1];
      set => this.CheckAndSet(value, 1);
    }

    public int ArmyComposition3Value
    {
      get => this._values[2];
      set => this.CheckAndSet(value, 2);
    }

    public int ArmyComposition4Value
    {
      get => this._values[3];
      set => this.CheckAndSet(value, 3);
    }

    private void SetArmyCompositionValue(int index, int value)
    {
      switch (index)
      {
        case 0:
          this._values[0] = value;
          this.OnPropertyChanged("ArmyComposition1Value");
          this.ArmyComposition1Text = "%" + value.ToString();
          break;
        case 1:
          this._values[1] = value;
          this.OnPropertyChanged("ArmyComposition2Value");
          this.ArmyComposition2Text = "%" + value.ToString();
          break;
        case 2:
          this._values[2] = value;
          this.OnPropertyChanged("ArmyComposition3Value");
          this.ArmyComposition3Text = "%" + value.ToString();
          break;
        case 3:
          this._values[3] = value;
          this.OnPropertyChanged("ArmyComposition4Value");
          this.ArmyComposition4Text = "%" + value.ToString();
          break;
      }
    }

    public bool IsArmyComposition1Enabled
    {
      get => this._isArmyComposition1Enabled;
      set
      {
        if (this._isArmyComposition1Enabled == value)
          return;
        this._isArmyComposition1Enabled = value;
        this.OnPropertyChangedWithValue((object) value, nameof (IsArmyComposition1Enabled));
      }
    }

    public bool IsArmyComposition2Enabled
    {
      get => this._isArmyComposition2Enabled;
      set
      {
        if (this._isArmyComposition2Enabled == value)
          return;
        this._isArmyComposition2Enabled = value;
        this.OnPropertyChangedWithValue((object) value, nameof (IsArmyComposition2Enabled));
      }
    }

    public bool IsArmyComposition3Enabled
    {
      get => this._isArmyComposition3Enabled;
      set
      {
        if (this._isArmyComposition3Enabled == value)
          return;
        this._isArmyComposition3Enabled = value;
        this.OnPropertyChangedWithValue((object) value, nameof (IsArmyComposition3Enabled));
      }
    }

    public bool IsArmyComposition4Enabled
    {
      get => this._isArmyComposition4Enabled;
      set
      {
        if (this._isArmyComposition4Enabled == value)
          return;
        this._isArmyComposition4Enabled = value;
        this.OnPropertyChangedWithValue((object) value, nameof (IsArmyComposition4Enabled));
      }
    }

    [DataSourceProperty]
    public string ArmyComposition1Text
    {
      get => this._armyComposition1Text;
      set
      {
        if (!(this._armyComposition1Text != value))
          return;
        this._armyComposition1Text = value;
        this.OnPropertyChangedWithValue((object) value, nameof (ArmyComposition1Text));
      }
    }

    [DataSourceProperty]
    public string ArmyComposition2Text
    {
      get => this._armyComposition2Text;
      set
      {
        if (!(this._armyComposition2Text != value))
          return;
        this._armyComposition2Text = value;
        this.OnPropertyChangedWithValue((object) value, nameof (ArmyComposition2Text));
      }
    }

    [DataSourceProperty]
    public string ArmyComposition3Text
    {
      get => this._armyComposition3Text;
      set
      {
        if (!(this._armyComposition3Text != value))
          return;
        this._armyComposition3Text = value;
        this.OnPropertyChangedWithValue((object) value, nameof (ArmyComposition3Text));
      }
    }

    [DataSourceProperty]
    public string ArmyComposition4Text
    {
      get => this._armyComposition4Text;
      set
      {
        if (!(this._armyComposition4Text != value))
          return;
        this._armyComposition4Text = value;
        this.OnPropertyChangedWithValue((object) value, nameof (ArmyComposition4Text));
      }
    }

    private enum TroopType
    {
      MeleeInfantry,
      RangedInfantry,
      MeleeCavalry,
      RangedCavalry,
    }
  }
}
