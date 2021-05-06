// Decompiled with JetBrains decompiler
// Type: TaleWorlds.MountAndBlade.CustomBattle.CustomBattleMenuSideVM
// Assembly: TaleWorlds.MountAndBlade.CustomBattle, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 8065FEE3-AE77-4E53-B13C-3890101BA431
// Assembly location: E:\SteamLibrary\steamapps\common\Mount & Blade II Bannerlord\Modules\CustomBattle\bin\Win64_Shipping_Client\TaleWorlds.MountAndBlade.CustomBattle.dll

using System;
using System.Collections.ObjectModel;
using TaleWorlds.Core;
using TaleWorlds.Core.ViewModelCollection;
using TaleWorlds.Library;
using TaleWorlds.Localization;
using TaleWorlds.MountAndBlade.CustomBattle.CustomBattle;
using TaleWorlds.MountAndBlade.CustomBattle.CustomBattle.SelectionItem;

namespace TaleWorlds.MountAndBlade.CustomBattle
{
  public class CustomBattleMenuSideVM : ViewModel
  {
    private readonly TextObject _sideName;
    private readonly bool _isPlayerSide;
    public CustomBattleMenuSideVM OppositeSide;
    private ArmyCompositionGroupVM _compositionGroup;
    private SelectorVM<FactionItemVM> _factionSelectionGroup;
    private SelectorVM<CharacterItemVM> _characterSelectionGroup;
    private CharacterViewModel _currentSelectedCharacter;
    private MBBindingList<CharacterEquipmentItemVM> _armorsList;
    private MBBindingList<CharacterEquipmentItemVM> _weaponsList;
    private string _currentSelectedCultureID;
    private string _name;
    private string _factionText;
    private string _titleText;

    public BasicCharacterObject SelectedCharacter { get; private set; }

    public BasicCultureObject SelectedFaction { get; private set; }

    public CustomBattleMenuSideVM(TextObject sideName, bool isPlayerSide)
    {
      this._sideName = sideName;
      this._isPlayerSide = isPlayerSide;
      this.CompositionGroup = new ArmyCompositionGroupVM(sideName.ToString() + "_COMPOSITION", this._isPlayerSide);
      this.FactionSelectionGroup = new SelectorVM<FactionItemVM>(0, new Action<SelectorVM<FactionItemVM>>(this.OnFactionSelection));
      this.CharacterSelectionGroup = new SelectorVM<CharacterItemVM>(0, new Action<SelectorVM<CharacterItemVM>>(this.OnCharacterSelection));
      this.ArmorsList = new MBBindingList<CharacterEquipmentItemVM>();
      this.WeaponsList = new MBBindingList<CharacterEquipmentItemVM>();
      this.RefreshValues();
    }

    public override void RefreshValues()
    {
      base.RefreshValues();
      this.Name = this._sideName.ToString();
      this.FactionText = GameTexts.FindText("str_faction").ToString();
      this.TitleText = !this._isPlayerSide ? new TextObject("{=QAYngoNQ}Enemy Character").ToString() : new TextObject("{=bLXleed8}Player Character").ToString();
      this.FactionSelectionGroup.ItemList.Clear();
      this.CharacterSelectionGroup.ItemList.Clear();
      foreach (BasicCultureObject faction in CustomBattleData.Factions)
        this.FactionSelectionGroup.AddItem(new FactionItemVM(faction));
      foreach (BasicCharacterObject character in CustomBattleData.Characters)
        this.CharacterSelectionGroup.AddItem(new CharacterItemVM(character));
      this.FactionSelectionGroup.SelectedIndex = 0;
      this.CharacterSelectionGroup.SelectedIndex = this._isPlayerSide ? 0 : 1;
      this.UpdateCharacterVisual();
      this.CompositionGroup.RefreshValues();
      ((ViewModel) this.CharacterSelectionGroup).RefreshValues();
      ((ViewModel) this.FactionSelectionGroup).RefreshValues();
    }

    public void OnPlayerTypeChange(CustomBattlePlayerType playerType) => this.CompositionGroup.OnPlayerTypeChange(playerType);

    private void OnFactionSelection(SelectorVM<FactionItemVM> selector)
    {
      BasicCultureObject faction = selector.SelectedItem.Faction;
      this.SelectedFaction = faction;
      this.CompositionGroup.SetCurrentSelectedCulture(faction);
      this.CurrentSelectedCultureID = faction.StringId;
    }

    private void OnCharacterSelection(SelectorVM<CharacterItemVM> selector)
    {
      this.SelectedCharacter = selector.SelectedItem.Character;
      this.UpdateCharacterVisual();
      if (this.OppositeSide == null)
        return;
      int index = 0;
      foreach (CharacterItemVM characterItemVm1 in (Collection<CharacterItemVM>) selector.ItemList)
      {
        CharacterItemVM characterItemVm2 = this.OppositeSide.CharacterSelectionGroup.ItemList[index];
        if (index == selector.SelectedIndex)
          characterItemVm2.CanBeSelected = false;
        else
          characterItemVm2.CanBeSelected = true;
        ++index;
      }
    }

    public void UpdateCharacterVisual()
    {
      this.CurrentSelectedCharacter = new CharacterViewModel((CharacterViewModel.StanceTypes) 1);
      this.CurrentSelectedCharacter.FillFrom(this.SelectedCharacter, -1);
      this.ArmorsList.Clear();
      this.ArmorsList.Add(new CharacterEquipmentItemVM(this.SelectedCharacter.Equipment[EquipmentIndex.NumAllWeaponSlots].Item));
      this.ArmorsList.Add(new CharacterEquipmentItemVM(this.SelectedCharacter.Equipment[EquipmentIndex.Cape].Item));
      this.ArmorsList.Add(new CharacterEquipmentItemVM(this.SelectedCharacter.Equipment[EquipmentIndex.Body].Item));
      this.ArmorsList.Add(new CharacterEquipmentItemVM(this.SelectedCharacter.Equipment[EquipmentIndex.Gloves].Item));
      this.ArmorsList.Add(new CharacterEquipmentItemVM(this.SelectedCharacter.Equipment[EquipmentIndex.Leg].Item));
      this.WeaponsList.Clear();
      this.WeaponsList.Add(new CharacterEquipmentItemVM(this.SelectedCharacter.Equipment[EquipmentIndex.WeaponItemBeginSlot].Item));
      this.WeaponsList.Add(new CharacterEquipmentItemVM(this.SelectedCharacter.Equipment[EquipmentIndex.Weapon1].Item));
      this.WeaponsList.Add(new CharacterEquipmentItemVM(this.SelectedCharacter.Equipment[EquipmentIndex.Weapon2].Item));
      this.WeaponsList.Add(new CharacterEquipmentItemVM(this.SelectedCharacter.Equipment[EquipmentIndex.Weapon3].Item));
      this.WeaponsList.Add(new CharacterEquipmentItemVM(this.SelectedCharacter.Equipment[EquipmentIndex.Weapon4].Item));
    }

    public void Randomize()
    {
      this.FactionSelectionGroup.ExecuteRandomize();
      this.CharacterSelectionGroup.ExecuteRandomize();
      this.CompositionGroup.RandomizeArmySize();
      int num1 = MBRandom.RandomInt(100);
      int num2 = MBRandom.RandomInt(100);
      int num3 = MBRandom.RandomInt(100);
      int num4 = MBRandom.RandomInt(100);
      int num5 = num1 + num2 + num3 + num4;
      int num6 = (int) Math.Round(100.0 * ((double) num1 / (double) num5));
      int num7 = (int) Math.Round(100.0 * ((double) num2 / (double) num5));
      int num8 = (int) Math.Round(100.0 * ((double) num3 / (double) num5));
      int num9 = 100 - (num6 + num7 + num8);
      this.CompositionGroup.IsArmyComposition1Enabled = false;
      this.CompositionGroup.IsArmyComposition2Enabled = false;
      this.CompositionGroup.IsArmyComposition3Enabled = false;
      this.CompositionGroup.IsArmyComposition4Enabled = false;
      this.CompositionGroup.ArmyComposition1Value = num6;
      this.CompositionGroup.ArmyComposition2Value = num7;
      this.CompositionGroup.ArmyComposition3Value = num8;
      this.CompositionGroup.ArmyComposition4Value = num9;
    }

    [DataSourceProperty]
    public CharacterViewModel CurrentSelectedCharacter
    {
      get => this._currentSelectedCharacter;
      set
      {
        if (value == this._currentSelectedCharacter)
          return;
        this._currentSelectedCharacter = value;
        this.OnPropertyChangedWithValue((object) value, nameof (CurrentSelectedCharacter));
      }
    }

    [DataSourceProperty]
    public MBBindingList<CharacterEquipmentItemVM> ArmorsList
    {
      get => this._armorsList;
      set
      {
        if (value == this._armorsList)
          return;
        this._armorsList = value;
        this.OnPropertyChangedWithValue((object) value, nameof (ArmorsList));
      }
    }

    [DataSourceProperty]
    public MBBindingList<CharacterEquipmentItemVM> WeaponsList
    {
      get => this._weaponsList;
      set
      {
        if (value == this._weaponsList)
          return;
        this._weaponsList = value;
        this.OnPropertyChangedWithValue((object) value, nameof (WeaponsList));
      }
    }

    [DataSourceProperty]
    public string CurrentSelectedCultureID
    {
      get => this._currentSelectedCultureID;
      set
      {
        if (!(value != this._currentSelectedCultureID))
          return;
        this._currentSelectedCultureID = value;
        this.OnPropertyChangedWithValue((object) value, nameof (CurrentSelectedCultureID));
      }
    }

    [DataSourceProperty]
    public string FactionText
    {
      get => this._factionText;
      set
      {
        if (!(value != this._factionText))
          return;
        this._factionText = value;
        this.OnPropertyChangedWithValue((object) value, nameof (FactionText));
      }
    }

    [DataSourceProperty]
    public string TitleText
    {
      get => this._titleText;
      set
      {
        if (!(value != this._titleText))
          return;
        this._titleText = value;
        this.OnPropertyChangedWithValue((object) value, nameof (TitleText));
      }
    }

    [DataSourceProperty]
    public string Name
    {
      get => this._name;
      set
      {
        if (!(value != this._name))
          return;
        this._name = value;
        this.OnPropertyChangedWithValue((object) value, nameof (Name));
      }
    }

    [DataSourceProperty]
    public SelectorVM<CharacterItemVM> CharacterSelectionGroup
    {
      get => this._characterSelectionGroup;
      set
      {
        if (value == this._characterSelectionGroup)
          return;
        this._characterSelectionGroup = value;
        this.OnPropertyChangedWithValue((object) value, nameof (CharacterSelectionGroup));
      }
    }

    [DataSourceProperty]
    public ArmyCompositionGroupVM CompositionGroup
    {
      get => this._compositionGroup;
      set
      {
        if (value == this._compositionGroup)
          return;
        this._compositionGroup = value;
        this.OnPropertyChangedWithValue((object) value, nameof (CompositionGroup));
      }
    }

    [DataSourceProperty]
    public SelectorVM<FactionItemVM> FactionSelectionGroup
    {
      get => this._factionSelectionGroup;
      set
      {
        if (value == this._factionSelectionGroup)
          return;
        this._factionSelectionGroup = value;
        this.OnPropertyChangedWithValue((object) value, nameof (FactionSelectionGroup));
      }
    }
  }
}
