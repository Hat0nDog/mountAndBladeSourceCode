// Decompiled with JetBrains decompiler
// Type: TaleWorlds.MountAndBlade.CustomBattle.CustomBattle.GameTypeSelectionGroupVM
// Assembly: TaleWorlds.MountAndBlade.CustomBattle, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 8065FEE3-AE77-4E53-B13C-3890101BA431
// Assembly location: E:\SteamLibrary\steamapps\common\Mount & Blade II Bannerlord\Modules\CustomBattle\bin\Win64_Shipping_Client\TaleWorlds.MountAndBlade.CustomBattle.dll

using System;
using TaleWorlds.Core.ViewModelCollection;
using TaleWorlds.Library;
using TaleWorlds.Localization;
using TaleWorlds.MountAndBlade.CustomBattle.CustomBattle.SelectionItem;

namespace TaleWorlds.MountAndBlade.CustomBattle.CustomBattle
{
  public class GameTypeSelectionGroupVM : ViewModel
  {
    private readonly Action<CustomBattlePlayerType> _onPlayerTypeChange;
    private readonly Action<CustomBattleGameType> _onGameTypeChange;
    private SelectorVM<GameTypeItemVM> _gameTypeSelection;
    private SelectorVM<PlayerTypeItemVM> _playerTypeSelection;
    private SelectorVM<PlayerSideItemVM> _playerSideSelection;
    private string _gameTypeText;
    private string _playerTypeText;
    private string _playerSideText;

    public CustomBattleGameType SelectedGameType { get; private set; }

    public CustomBattlePlayerType SelectedPlayerType { get; private set; }

    public CustomBattlePlayerSide SelectedPlayerSide { get; private set; }

    public GameTypeSelectionGroupVM(
      Action<CustomBattlePlayerType> onPlayerTypeChange,
      Action<CustomBattleGameType> onGameTypeChange)
    {
      this._onPlayerTypeChange = onPlayerTypeChange;
      this._onGameTypeChange = onGameTypeChange;
      this.GameTypeSelection = new SelectorVM<GameTypeItemVM>(0, new Action<SelectorVM<GameTypeItemVM>>(this.OnGameTypeSelection));
      this.PlayerTypeSelection = new SelectorVM<PlayerTypeItemVM>(0, new Action<SelectorVM<PlayerTypeItemVM>>(this.OnPlayerTypeSelection));
      this.PlayerSideSelection = new SelectorVM<PlayerSideItemVM>(0, new Action<SelectorVM<PlayerSideItemVM>>(this.OnPlayerSideSelection));
      this.RefreshValues();
    }

    public override void RefreshValues()
    {
      base.RefreshValues();
      this.GameTypeText = new TextObject("{=JPimShCw}Game Type").ToString();
      this.PlayerTypeText = new TextObject("{=bKg8Mmwb}Player Type").ToString();
      this.PlayerSideText = new TextObject("{=P3rMg4uZ}Player Side").ToString();
      this.GameTypeSelection.ItemList.Clear();
      this.PlayerTypeSelection.ItemList.Clear();
      this.PlayerSideSelection.ItemList.Clear();
      foreach (Tuple<string, CustomBattleGameType> gameType in CustomBattleData.GameTypes)
        this.GameTypeSelection.AddItem(new GameTypeItemVM(gameType.Item1, gameType.Item2));
      foreach (Tuple<string, CustomBattlePlayerType> playerType in CustomBattleData.PlayerTypes)
        this.PlayerTypeSelection.AddItem(new PlayerTypeItemVM(playerType.Item1, playerType.Item2));
      foreach (Tuple<string, CustomBattlePlayerSide> playerSide in CustomBattleData.PlayerSides)
        this.PlayerSideSelection.AddItem(new PlayerSideItemVM(playerSide.Item1, playerSide.Item2));
      this.GameTypeSelection.SelectedIndex = 0;
      this.PlayerTypeSelection.SelectedIndex = 0;
      this.PlayerSideSelection.SelectedIndex = 0;
    }

    public void RandomizeAll()
    {
      this.GameTypeSelection.ExecuteRandomize();
      this.PlayerTypeSelection.ExecuteRandomize();
      this.PlayerSideSelection.ExecuteRandomize();
    }

    private void OnGameTypeSelection(SelectorVM<GameTypeItemVM> selector)
    {
      this.SelectedGameType = selector.SelectedItem.GameType;
      this._onGameTypeChange(this.SelectedGameType);
    }

    private void OnPlayerTypeSelection(SelectorVM<PlayerTypeItemVM> selector)
    {
      this.SelectedPlayerType = selector.SelectedItem.PlayerType;
      this._onPlayerTypeChange(this.SelectedPlayerType);
    }

    private void OnPlayerSideSelection(SelectorVM<PlayerSideItemVM> selector) => this.SelectedPlayerSide = selector.SelectedItem.PlayerSide;

    [DataSourceProperty]
    public SelectorVM<GameTypeItemVM> GameTypeSelection
    {
      get => this._gameTypeSelection;
      set
      {
        if (value == this._gameTypeSelection)
          return;
        this._gameTypeSelection = value;
        this.OnPropertyChangedWithValue((object) value, nameof (GameTypeSelection));
      }
    }

    [DataSourceProperty]
    public SelectorVM<PlayerTypeItemVM> PlayerTypeSelection
    {
      get => this._playerTypeSelection;
      set
      {
        if (value == this._playerTypeSelection)
          return;
        this._playerTypeSelection = value;
        this.OnPropertyChangedWithValue((object) value, nameof (PlayerTypeSelection));
      }
    }

    [DataSourceProperty]
    public SelectorVM<PlayerSideItemVM> PlayerSideSelection
    {
      get => this._playerSideSelection;
      set
      {
        if (value == this._playerSideSelection)
          return;
        this._playerSideSelection = value;
        this.OnPropertyChangedWithValue((object) value, nameof (PlayerSideSelection));
      }
    }

    [DataSourceProperty]
    public string GameTypeText
    {
      get => this._gameTypeText;
      set
      {
        if (!(value != this._gameTypeText))
          return;
        this._gameTypeText = value;
        this.OnPropertyChangedWithValue((object) value, nameof (GameTypeText));
      }
    }

    [DataSourceProperty]
    public string PlayerTypeText
    {
      get => this._playerTypeText;
      set
      {
        if (!(value != this._playerTypeText))
          return;
        this._playerTypeText = value;
        this.OnPropertyChangedWithValue((object) value, nameof (PlayerTypeText));
      }
    }

    [DataSourceProperty]
    public string PlayerSideText
    {
      get => this._playerSideText;
      set
      {
        if (!(value != this._playerSideText))
          return;
        this._playerSideText = value;
        this.OnPropertyChangedWithValue((object) value, nameof (PlayerSideText));
      }
    }
  }
}
