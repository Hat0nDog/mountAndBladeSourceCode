// Decompiled with JetBrains decompiler
// Type: TaleWorlds.MountAndBlade.CustomBattle.CustomBattleMenuVM
// Assembly: TaleWorlds.MountAndBlade.CustomBattle, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 8065FEE3-AE77-4E53-B13C-3890101BA431
// Assembly location: E:\SteamLibrary\steamapps\common\Mount & Blade II Bannerlord\Modules\CustomBattle\bin\Win64_Shipping_Client\TaleWorlds.MountAndBlade.CustomBattle.dll

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using TaleWorlds.Core;
using TaleWorlds.Core.ViewModelCollection;
using TaleWorlds.InputSystem;
using TaleWorlds.Library;
using TaleWorlds.Localization;
using TaleWorlds.MountAndBlade.CustomBattle.CustomBattle;
using TaleWorlds.MountAndBlade.ViewModelCollection.Input;

namespace TaleWorlds.MountAndBlade.CustomBattle
{
  public class CustomBattleMenuVM : ViewModel
  {
    private CustomBattleState _customBattleState;
    private CustomBattleMenuSideVM _enemySide;
    private CustomBattleMenuSideVM _playerSide;
    private bool _isAttackerCustomMachineSelectionEnabled;
    private bool _isDefenderCustomMachineSelectionEnabled;
    private GameTypeSelectionGroupVM _gameTypeSelectionGroup;
    private MapSelectionGroupVM _mapSelectionGroup;
    private string _randomizeButtonText;
    private string _backButtonText;
    private string _startButtonText;
    private string _titleText;
    private MBBindingList<CustomBattleSiegeMachineVM> _attackerMeleeMachines;
    private MBBindingList<CustomBattleSiegeMachineVM> _attackerRangedMachines;
    private MBBindingList<CustomBattleSiegeMachineVM> _defenderMachines;
    private InputKeyItemVM _startInputKey;
    private InputKeyItemVM _cancelInputKey;
    private InputKeyItemVM _randomizeInputKey;

    private static SiegeEngineType GetSiegeWeaponType(SiegeEngineType siegeWeaponType)
    {
      if (siegeWeaponType == DefaultSiegeEngineTypes.Ladder)
        return DefaultSiegeEngineTypes.Ladder;
      if (siegeWeaponType == DefaultSiegeEngineTypes.Ballista)
        return DefaultSiegeEngineTypes.Ballista;
      if (siegeWeaponType == DefaultSiegeEngineTypes.FireBallista)
        return DefaultSiegeEngineTypes.FireBallista;
      if (siegeWeaponType == DefaultSiegeEngineTypes.Ram || siegeWeaponType == DefaultSiegeEngineTypes.ImprovedRam)
        return DefaultSiegeEngineTypes.Ram;
      if (siegeWeaponType == DefaultSiegeEngineTypes.SiegeTower)
        return DefaultSiegeEngineTypes.SiegeTower;
      if (siegeWeaponType == DefaultSiegeEngineTypes.Onager || siegeWeaponType == DefaultSiegeEngineTypes.Catapult)
        return DefaultSiegeEngineTypes.Onager;
      if (siegeWeaponType == DefaultSiegeEngineTypes.FireOnager || siegeWeaponType == DefaultSiegeEngineTypes.FireCatapult)
        return DefaultSiegeEngineTypes.FireOnager;
      return siegeWeaponType == DefaultSiegeEngineTypes.Trebuchet || siegeWeaponType == DefaultSiegeEngineTypes.Bricole ? DefaultSiegeEngineTypes.Trebuchet : siegeWeaponType;
    }

    public CustomBattleMenuVM(CustomBattleState battleState)
    {
      this._customBattleState = battleState;
      this.IsAttackerCustomMachineSelectionEnabled = false;
      this.PlayerSide = new CustomBattleMenuSideVM(new TextObject("{=BC7n6qxk}PLAYER"), true);
      this.EnemySide = new CustomBattleMenuSideVM(new TextObject("{=35IHscBa}ENEMY"), false);
      this.PlayerSide.OppositeSide = this.EnemySide;
      this.EnemySide.OppositeSide = this.PlayerSide;
      this.MapSelectionGroup = new MapSelectionGroupVM();
      this.GameTypeSelectionGroup = new GameTypeSelectionGroupVM(new Action<CustomBattlePlayerType>(this.OnPlayerTypeChange), new Action<CustomBattleGameType>(this.OnGameTypeChange));
      this.AttackerMeleeMachines = new MBBindingList<CustomBattleSiegeMachineVM>();
      for (int index = 0; index < 3; ++index)
        this.AttackerMeleeMachines.Add(new CustomBattleSiegeMachineVM((SiegeEngineType) null, new Action<CustomBattleSiegeMachineVM>(this.OnMeleeMachineSelection)));
      this.AttackerRangedMachines = new MBBindingList<CustomBattleSiegeMachineVM>();
      for (int index = 0; index < 4; ++index)
        this.AttackerRangedMachines.Add(new CustomBattleSiegeMachineVM((SiegeEngineType) null, new Action<CustomBattleSiegeMachineVM>(this.OnAttackerRangedMachineSelection)));
      this.DefenderMachines = new MBBindingList<CustomBattleSiegeMachineVM>();
      for (int index = 0; index < 4; ++index)
        this.DefenderMachines.Add(new CustomBattleSiegeMachineVM((SiegeEngineType) null, new Action<CustomBattleSiegeMachineVM>(this.OnDefenderRangedMachineSelection)));
      this.RefreshValues();
      this.SetDefaultSiegeMachines();
    }

    private void SetDefaultSiegeMachines()
    {
      this.AttackerMeleeMachines[0].SetMachineType(SiegeEngineType.All.FirstOrDefault<SiegeEngineType>((Func<SiegeEngineType, bool>) (e => e.StringId == "siege_tower_level1")));
      this.AttackerMeleeMachines[1].SetMachineType(SiegeEngineType.All.FirstOrDefault<SiegeEngineType>((Func<SiegeEngineType, bool>) (e => e.StringId == "ram")));
      this.AttackerMeleeMachines[2].SetMachineType(SiegeEngineType.All.FirstOrDefault<SiegeEngineType>((Func<SiegeEngineType, bool>) (e => e.StringId == "siege_tower_level1")));
      this.AttackerRangedMachines[0].SetMachineType(SiegeEngineType.All.FirstOrDefault<SiegeEngineType>((Func<SiegeEngineType, bool>) (e => e.StringId == "trebuchet")));
      this.AttackerRangedMachines[1].SetMachineType(SiegeEngineType.All.FirstOrDefault<SiegeEngineType>((Func<SiegeEngineType, bool>) (e => e.StringId == "onager")));
      this.AttackerRangedMachines[2].SetMachineType(SiegeEngineType.All.FirstOrDefault<SiegeEngineType>((Func<SiegeEngineType, bool>) (e => e.StringId == "onager")));
      this.AttackerRangedMachines[3].SetMachineType(SiegeEngineType.All.FirstOrDefault<SiegeEngineType>((Func<SiegeEngineType, bool>) (e => e.StringId == "fire_ballista")));
      this.DefenderMachines[0].SetMachineType(SiegeEngineType.All.FirstOrDefault<SiegeEngineType>((Func<SiegeEngineType, bool>) (e => e.StringId == "fire_catapult")));
      this.DefenderMachines[1].SetMachineType(SiegeEngineType.All.FirstOrDefault<SiegeEngineType>((Func<SiegeEngineType, bool>) (e => e.StringId == "fire_catapult")));
      this.DefenderMachines[2].SetMachineType(SiegeEngineType.All.FirstOrDefault<SiegeEngineType>((Func<SiegeEngineType, bool>) (e => e.StringId == "catapult")));
      this.DefenderMachines[3].SetMachineType(SiegeEngineType.All.FirstOrDefault<SiegeEngineType>((Func<SiegeEngineType, bool>) (e => e.StringId == "fire_ballista")));
    }

    internal void SetActiveState(bool isActive)
    {
      if (isActive)
      {
        this.EnemySide.UpdateCharacterVisual();
        this.PlayerSide.UpdateCharacterVisual();
      }
      else
      {
        this.EnemySide.CurrentSelectedCharacter = (CharacterViewModel) null;
        this.PlayerSide.CurrentSelectedCharacter = (CharacterViewModel) null;
      }
    }

    private void OnPlayerTypeChange(CustomBattlePlayerType playerType) => this.PlayerSide.OnPlayerTypeChange(playerType);

    private void OnGameTypeChange(CustomBattleGameType gameType) => this.MapSelectionGroup.OnGameTypeChange(gameType);

    public override void RefreshValues()
    {
      base.RefreshValues();
      this.RandomizeButtonText = GameTexts.FindText("str_randomize").ToString();
      this.StartButtonText = GameTexts.FindText("str_start").ToString();
      this.BackButtonText = GameTexts.FindText("str_back").ToString();
      this.TitleText = GameTexts.FindText("str_custom_battle").ToString();
      this.EnemySide.RefreshValues();
      this.PlayerSide.RefreshValues();
      this.AttackerMeleeMachines.ApplyActionOnAllItems((Action<CustomBattleSiegeMachineVM>) (x => x.RefreshValues()));
      this.AttackerRangedMachines.ApplyActionOnAllItems((Action<CustomBattleSiegeMachineVM>) (x => x.RefreshValues()));
      this.DefenderMachines.ApplyActionOnAllItems((Action<CustomBattleSiegeMachineVM>) (x => x.RefreshValues()));
      this.MapSelectionGroup.RefreshValues();
    }

    private void OnMeleeMachineSelection(CustomBattleSiegeMachineVM selectedSlot)
    {
      List<InquiryElement> inquiryElements = new List<InquiryElement>();
      inquiryElements.Add(new InquiryElement((object) null, GameTexts.FindText("str_empty").ToString(), (ImageIdentifier) null));
      foreach (SiegeEngineType attackerMeleeMachine in CustomBattleData.GetAllAttackerMeleeMachines())
        inquiryElements.Add(new InquiryElement((object) attackerMeleeMachine, attackerMeleeMachine.Name.ToString(), (ImageIdentifier) null));
      InformationManager.ShowMultiSelectionInquiry(new MultiSelectionInquiryData(new TextObject("{=MVOWsP48}Select a Melee Machine").ToString(), string.Empty, inquiryElements, false, 1, GameTexts.FindText("str_done").ToString(), "", (Action<List<InquiryElement>>) (selectedElements => selectedSlot.SetMachineType(selectedElements.First<InquiryElement>().Identifier as SiegeEngineType)), (Action<List<InquiryElement>>) null));
    }

    private void OnAttackerRangedMachineSelection(CustomBattleSiegeMachineVM selectedSlot)
    {
      List<InquiryElement> inquiryElements = new List<InquiryElement>();
      inquiryElements.Add(new InquiryElement((object) null, GameTexts.FindText("str_empty").ToString(), (ImageIdentifier) null));
      foreach (SiegeEngineType attackerRangedMachine in CustomBattleData.GetAllAttackerRangedMachines())
        inquiryElements.Add(new InquiryElement((object) attackerRangedMachine, attackerRangedMachine.Name.ToString(), (ImageIdentifier) null));
      InformationManager.ShowMultiSelectionInquiry(new MultiSelectionInquiryData(new TextObject("{=SLZzfNPr}Select a Ranged Machine").ToString(), string.Empty, inquiryElements, false, 1, GameTexts.FindText("str_done").ToString(), "", (Action<List<InquiryElement>>) (selectedElements => selectedSlot.SetMachineType(selectedElements[0].Identifier as SiegeEngineType)), (Action<List<InquiryElement>>) null));
    }

    private void OnDefenderRangedMachineSelection(CustomBattleSiegeMachineVM selectedSlot)
    {
      List<InquiryElement> inquiryElements = new List<InquiryElement>();
      inquiryElements.Add(new InquiryElement((object) null, GameTexts.FindText("str_empty").ToString(), (ImageIdentifier) null));
      foreach (SiegeEngineType defenderRangedMachine in CustomBattleData.GetAllDefenderRangedMachines())
        inquiryElements.Add(new InquiryElement((object) defenderRangedMachine, defenderRangedMachine.Name.ToString(), (ImageIdentifier) null));
      InformationManager.ShowMultiSelectionInquiry(new MultiSelectionInquiryData(new TextObject("{=SLZzfNPr}Select a Ranged Machine").ToString(), string.Empty, inquiryElements, false, 1, GameTexts.FindText("str_done").ToString(), "", (Action<List<InquiryElement>>) (selectedElements => selectedSlot.SetMachineType(selectedElements[0].Identifier as SiegeEngineType)), (Action<List<InquiryElement>>) null));
    }

    private void ExecuteRandomizeAttackerSiegeEngines()
    {
      List<SiegeEngineType> e = new List<SiegeEngineType>();
      e.AddRange(CustomBattleData.GetAllAttackerMeleeMachines());
      e.Add((SiegeEngineType) null);
      foreach (CustomBattleSiegeMachineVM attackerMeleeMachine in (Collection<CustomBattleSiegeMachineVM>) this._attackerMeleeMachines)
        attackerMeleeMachine.SetMachineType(e.GetRandomElement<SiegeEngineType>());
      e.Clear();
      e.AddRange(CustomBattleData.GetAllAttackerRangedMachines());
      e.Add((SiegeEngineType) null);
      foreach (CustomBattleSiegeMachineVM attackerRangedMachine in (Collection<CustomBattleSiegeMachineVM>) this._attackerRangedMachines)
        attackerRangedMachine.SetMachineType(e.GetRandomElement<SiegeEngineType>());
    }

    private void ExecuteRandomizeDefenderSiegeEngines()
    {
      List<SiegeEngineType> e = new List<SiegeEngineType>();
      e.AddRange(CustomBattleData.GetAllDefenderRangedMachines());
      e.Add((SiegeEngineType) null);
      foreach (CustomBattleSiegeMachineVM defenderMachine in (Collection<CustomBattleSiegeMachineVM>) this._defenderMachines)
        defenderMachine.SetMachineType(e.GetRandomElement<SiegeEngineType>());
    }

    public void ExecuteBack()
    {
      Debug.Print("EXECUTE BACK - PRESSED", color: Debug.DebugColor.Green);
      Game.Current.GameStateManager.PopState();
    }

    private CustomBattleData PrepareBattleData()
    {
      BasicCharacterObject selectedCharacter1 = this.PlayerSide.SelectedCharacter;
      BasicCharacterObject selectedCharacter2 = this.EnemySide.SelectedCharacter;
      int armySize1 = this.PlayerSide.CompositionGroup.ArmySize;
      int armySize2 = this.EnemySide.CompositionGroup.ArmySize;
      bool isPlayerAttacker = this.GameTypeSelectionGroup.SelectedPlayerSide == CustomBattlePlayerSide.Attacker;
      bool flag = this.GameTypeSelectionGroup.SelectedPlayerType == CustomBattlePlayerType.Commander;
      BasicCharacterObject playerSideGeneralCharacter = (BasicCharacterObject) null;
      if (!flag)
      {
        List<BasicCharacterObject> list = CustomBattleData.Characters.ToList<BasicCharacterObject>();
        list.Remove(selectedCharacter1);
        list.Remove(selectedCharacter2);
        playerSideGeneralCharacter = list.GetRandomElement<BasicCharacterObject>();
        --armySize1;
      }
      int[] troopCounts1 = CustomBattleMenuVM.CustomBattleHelper.GetTroopCounts(armySize1, this.PlayerSide.CompositionGroup);
      ArmyCompositionGroupVM compositionGroup = this.EnemySide.CompositionGroup;
      int[] troopCounts2 = CustomBattleMenuVM.CustomBattleHelper.GetTroopCounts(armySize2, compositionGroup);
      List<BasicCharacterObject>[] troopSelections1 = CustomBattleMenuVM.CustomBattleHelper.GetTroopSelections(this.PlayerSide.CompositionGroup);
      List<BasicCharacterObject>[] troopSelections2 = CustomBattleMenuVM.CustomBattleHelper.GetTroopSelections(this.EnemySide.CompositionGroup);
      BasicCultureObject selectedFaction1 = this.PlayerSide.SelectedFaction;
      BasicCultureObject selectedFaction2 = this.EnemySide.SelectedFaction;
      CustomBattleCombatant[] customBattleParties = this._customBattleState.GetCustomBattleParties(selectedCharacter1, playerSideGeneralCharacter, selectedCharacter2, selectedFaction1, troopCounts1, troopSelections1, selectedFaction2, troopCounts2, troopSelections2, isPlayerAttacker);
      CustomBattleData customBattleData = new CustomBattleData();
      customBattleData.GameType = this.GameTypeSelectionGroup.SelectedGameType;
      customBattleData.SceneId = this.MapSelectionGroup.SelectedMapId;
      customBattleData.PlayerCharacter = selectedCharacter1;
      customBattleData.PlayerParty = customBattleParties[0];
      customBattleData.EnemyParty = customBattleParties[1];
      customBattleData.IsPlayerGeneral = flag;
      customBattleData.PlayerSideGeneralCharacter = playerSideGeneralCharacter;
      customBattleData.SeasonId = this.MapSelectionGroup.SelectedSeasonId;
      customBattleData.SceneLevel = "";
      customBattleData.TimeOfDay = (float) this.MapSelectionGroup.SelectedTimeOfDay;
      if (customBattleData.GameType == CustomBattleGameType.Siege)
      {
        Dictionary<SiegeEngineType, int> machineCounts1 = new Dictionary<SiegeEngineType, int>();
        Dictionary<SiegeEngineType, int> machineCounts2 = new Dictionary<SiegeEngineType, int>();
        CustomBattleMenuVM.CustomBattleHelper.FillSiegeMachineCounts(machineCounts1, this._attackerMeleeMachines);
        CustomBattleMenuVM.CustomBattleHelper.FillSiegeMachineCounts(machineCounts1, this._attackerRangedMachines);
        CustomBattleMenuVM.CustomBattleHelper.FillSiegeMachineCounts(machineCounts2, this._defenderMachines);
        float[] hitpointPercentages = CustomBattleMenuVM.CustomBattleHelper.GetWallHitpointPercentages(this.MapSelectionGroup.SelectedWallBreachedCount);
        customBattleData.AttackerMachines = machineCounts1;
        customBattleData.DefenderMachines = machineCounts2;
        customBattleData.WallHitpointPercentages = hitpointPercentages;
        customBattleData.HasAnySiegeTower = this._attackerMeleeMachines.Any<CustomBattleSiegeMachineVM>((Func<CustomBattleSiegeMachineVM, bool>) (mm => mm.SiegeEngineType == DefaultSiegeEngineTypes.SiegeTower));
        customBattleData.IsPlayerAttacker = isPlayerAttacker;
        customBattleData.SceneUpgradeLevel = this.MapSelectionGroup.SelectedSceneLevel;
        customBattleData.IsSallyOut = this.MapSelectionGroup.IsSallyOutSelected;
        customBattleData.IsReliefAttack = false;
      }
      return customBattleData;
    }

    public void ExecuteStart()
    {
      CustomBattleMenuVM.CustomBattleHelper.StartGame(this.PrepareBattleData());
      Debug.Print("EXECUTE START - PRESSED", color: Debug.DebugColor.Green);
    }

    public void ExecuteRandomize()
    {
      this.GameTypeSelectionGroup.RandomizeAll();
      this.MapSelectionGroup.RandomizeAll();
      this.PlayerSide.Randomize();
      this.EnemySide.Randomize();
      this.ExecuteRandomizeAttackerSiegeEngines();
      this.ExecuteRandomizeDefenderSiegeEngines();
      Debug.Print("EXECUTE RANDOMIZE - PRESSED", color: Debug.DebugColor.Green);
    }

    private void ExecuteDoneDefenderCustomMachineSelection() => this.IsDefenderCustomMachineSelectionEnabled = false;

    private void ExecuteDoneAttackerCustomMachineSelection() => this.IsAttackerCustomMachineSelectionEnabled = false;

    public override void OnFinalize()
    {
      base.OnFinalize();
      ((ViewModel) this.StartInputKey).OnFinalize();
      ((ViewModel) this.CancelInputKey).OnFinalize();
      ((ViewModel) this.RandomizeInputKey).OnFinalize();
    }

    [DataSourceProperty]
    public bool IsAttackerCustomMachineSelectionEnabled
    {
      get => this._isAttackerCustomMachineSelectionEnabled;
      set
      {
        if (value == this._isAttackerCustomMachineSelectionEnabled)
          return;
        this._isAttackerCustomMachineSelectionEnabled = value;
        this.OnPropertyChangedWithValue((object) value, nameof (IsAttackerCustomMachineSelectionEnabled));
      }
    }

    [DataSourceProperty]
    public bool IsDefenderCustomMachineSelectionEnabled
    {
      get => this._isDefenderCustomMachineSelectionEnabled;
      set
      {
        if (value == this._isDefenderCustomMachineSelectionEnabled)
          return;
        this._isDefenderCustomMachineSelectionEnabled = value;
        this.OnPropertyChangedWithValue((object) value, nameof (IsDefenderCustomMachineSelectionEnabled));
      }
    }

    [DataSourceProperty]
    public string RandomizeButtonText
    {
      get => this._randomizeButtonText;
      set
      {
        if (!(value != this._randomizeButtonText))
          return;
        this._randomizeButtonText = value;
        this.OnPropertyChangedWithValue((object) value, nameof (RandomizeButtonText));
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
    public string BackButtonText
    {
      get => this._backButtonText;
      set
      {
        if (!(value != this._backButtonText))
          return;
        this._backButtonText = value;
        this.OnPropertyChangedWithValue((object) value, nameof (BackButtonText));
      }
    }

    [DataSourceProperty]
    public string StartButtonText
    {
      get => this._startButtonText;
      set
      {
        if (!(value != this._startButtonText))
          return;
        this._startButtonText = value;
        this.OnPropertyChangedWithValue((object) value, nameof (StartButtonText));
      }
    }

    [DataSourceProperty]
    public CustomBattleMenuSideVM EnemySide
    {
      get => this._enemySide;
      set
      {
        if (value == this._enemySide)
          return;
        this._enemySide = value;
        this.OnPropertyChangedWithValue((object) value, nameof (EnemySide));
      }
    }

    [DataSourceProperty]
    public CustomBattleMenuSideVM PlayerSide
    {
      get => this._playerSide;
      set
      {
        if (value == this._playerSide)
          return;
        this._playerSide = value;
        this.OnPropertyChangedWithValue((object) value, nameof (PlayerSide));
      }
    }

    [DataSourceProperty]
    public GameTypeSelectionGroupVM GameTypeSelectionGroup
    {
      get => this._gameTypeSelectionGroup;
      set
      {
        if (value == this._gameTypeSelectionGroup)
          return;
        this._gameTypeSelectionGroup = value;
        this.OnPropertyChangedWithValue((object) value, nameof (GameTypeSelectionGroup));
      }
    }

    [DataSourceProperty]
    public MapSelectionGroupVM MapSelectionGroup
    {
      get => this._mapSelectionGroup;
      set
      {
        if (value == this._mapSelectionGroup)
          return;
        this._mapSelectionGroup = value;
        this.OnPropertyChangedWithValue((object) value, nameof (MapSelectionGroup));
      }
    }

    [DataSourceProperty]
    public MBBindingList<CustomBattleSiegeMachineVM> AttackerMeleeMachines
    {
      get => this._attackerMeleeMachines;
      set
      {
        if (value == this._attackerMeleeMachines)
          return;
        this._attackerMeleeMachines = value;
        this.OnPropertyChangedWithValue((object) value, nameof (AttackerMeleeMachines));
      }
    }

    [DataSourceProperty]
    public MBBindingList<CustomBattleSiegeMachineVM> AttackerRangedMachines
    {
      get => this._attackerRangedMachines;
      set
      {
        if (value == this._attackerRangedMachines)
          return;
        this._attackerRangedMachines = value;
        this.OnPropertyChangedWithValue((object) value, nameof (AttackerRangedMachines));
      }
    }

    [DataSourceProperty]
    public MBBindingList<CustomBattleSiegeMachineVM> DefenderMachines
    {
      get => this._defenderMachines;
      set
      {
        if (value == this._defenderMachines)
          return;
        this._defenderMachines = value;
        this.OnPropertyChangedWithValue((object) value, nameof (DefenderMachines));
      }
    }

    public void SetStartInputKey(HotKey hotkey) => this.StartInputKey = InputKeyItemVM.CreateFromHotKey(hotkey, true);

    public void SetCancelInputKey(HotKey hotkey) => this.CancelInputKey = InputKeyItemVM.CreateFromHotKey(hotkey, true);

    public void SetRandomizeInputKey(HotKey hotkey) => this.RandomizeInputKey = InputKeyItemVM.CreateFromHotKey(hotkey, true);

    public InputKeyItemVM StartInputKey
    {
      get => this._startInputKey;
      set
      {
        if (value == this._startInputKey)
          return;
        this._startInputKey = value;
        this.OnPropertyChangedWithValue((object) value, nameof (StartInputKey));
      }
    }

    public InputKeyItemVM CancelInputKey
    {
      get => this._cancelInputKey;
      set
      {
        if (value == this._cancelInputKey)
          return;
        this._cancelInputKey = value;
        this.OnPropertyChangedWithValue((object) value, nameof (CancelInputKey));
      }
    }

    public InputKeyItemVM RandomizeInputKey
    {
      get => this._randomizeInputKey;
      set
      {
        if (value == this._randomizeInputKey)
          return;
        this._randomizeInputKey = value;
        this.OnPropertyChangedWithValue((object) value, nameof (RandomizeInputKey));
      }
    }

    private static class CustomBattleHelper
    {
      public static void StartGame(CustomBattleData data)
      {
        Game.Current.PlayerTroop = data.PlayerCharacter;
        if (data.GameType == CustomBattleGameType.Siege)
          BannerlordMissions.OpenSiegeMissionWithDeployment(data.SceneId, data.PlayerCharacter, data.PlayerParty, data.EnemyParty, data.IsPlayerGeneral, data.WallHitpointPercentages, data.HasAnySiegeTower, data.AttackerMachines, data.DefenderMachines, data.IsPlayerAttacker, data.SceneUpgradeLevel, data.SeasonId, data.IsSallyOut, data.IsReliefAttack, data.TimeOfDay);
        else
          BannerlordMissions.OpenCustomBattleMission(data.SceneId, data.PlayerCharacter, data.PlayerParty, data.EnemyParty, data.IsPlayerGeneral, data.PlayerSideGeneralCharacter, data.SceneLevel, data.SeasonId, data.TimeOfDay);
      }

      public static void FillSiegeMachineCounts(
        Dictionary<SiegeEngineType, int> machineCounts,
        MBBindingList<CustomBattleSiegeMachineVM> machines)
      {
        foreach (CustomBattleSiegeMachineVM machine in (Collection<CustomBattleSiegeMachineVM>) machines)
        {
          if (machine.SiegeEngineType != null)
          {
            SiegeEngineType siegeWeaponType = CustomBattleMenuVM.GetSiegeWeaponType(machine.SiegeEngineType);
            if (!machineCounts.ContainsKey(siegeWeaponType))
              machineCounts.Add(siegeWeaponType, 0);
            machineCounts[siegeWeaponType]++;
          }
        }
      }

      public static float[] GetWallHitpointPercentages(int breachedWallCount)
      {
        float[] numArray = new float[2];
        switch (breachedWallCount)
        {
          case 0:
            numArray[0] = 1f;
            numArray[1] = 1f;
            break;
          case 1:
            int index = MBRandom.RandomInt(2);
            numArray[index] = 0.0f;
            numArray[1 - index] = 1f;
            break;
          default:
            numArray[0] = 0.0f;
            numArray[1] = 0.0f;
            break;
        }
        return numArray;
      }

      public static int[] GetTroopCounts(int armySize, ArmyCompositionGroupVM armyComposition)
      {
        int[] numArray = new int[4];
        --armySize;
        numArray[1] = (int) Math.Round((double) armyComposition.ArmyComposition2Value / 100.0 * (double) armySize);
        numArray[2] = (int) Math.Round((double) armyComposition.ArmyComposition3Value / 100.0 * (double) armySize);
        numArray[3] = (int) Math.Round((double) armyComposition.ArmyComposition4Value / 100.0 * (double) armySize);
        numArray[0] = armySize - ((IEnumerable<int>) numArray).Sum();
        return numArray;
      }

      public static List<BasicCharacterObject>[] GetTroopSelections(
        ArmyCompositionGroupVM armyComposition)
      {
        return new List<BasicCharacterObject>[4]
        {
          armyComposition.SelectedMeleeInfantryTypes,
          armyComposition.SelectedRangedInfantryTypes,
          armyComposition.SelectedMeleeCavalryTypes,
          armyComposition.SelectedRangedCavalryTypes
        };
      }
    }
  }
}
