// Decompiled with JetBrains decompiler
// Type: TaleWorlds.MountAndBlade.CustomBattle.CustomBattle.MapSelectionGroupVM
// Assembly: TaleWorlds.MountAndBlade.CustomBattle, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 8065FEE3-AE77-4E53-B13C-3890101BA431
// Assembly location: E:\SteamLibrary\steamapps\common\Mount & Blade II Bannerlord\Modules\CustomBattle\bin\Win64_Shipping_Client\TaleWorlds.MountAndBlade.CustomBattle.dll

using System;
using System.Collections.Generic;
using System.Linq;
using TaleWorlds.Core;
using TaleWorlds.Core.ViewModelCollection;
using TaleWorlds.Library;
using TaleWorlds.Localization;
using TaleWorlds.MountAndBlade.CustomBattle.CustomBattle.SelectionItem;

namespace TaleWorlds.MountAndBlade.CustomBattle.CustomBattle
{
  public class MapSelectionGroupVM : ViewModel
  {
    private bool _isCurrentMapSiege;
    private bool _isSallyOutSelected;
    private string _searchText;
    private string _searchDefaultText;
    private SelectorVM<SceneLevelItemVM> _sceneLevelSelection;
    private SelectorVM<WallHitpointItemVM> _wallHitpointSelection;
    private SelectorVM<SeasonItemVM> _seasonSelection;
    private SelectorVM<TimeOfDayItemVM> _timeOfDaySelection;
    private MBBindingList<MapItemVM> _mapSearchResults;
    private string _titleText;
    private string _seasonText;
    private string _timeOfDayText;
    private string _sceneLevelText;
    private string _wallHitpointsText;
    private string _attackerSiegeMachinesText;
    private string _defenderSiegeMachinesText;
    private string _salloutText;

    public int SelectedWallBreachedCount { get; private set; }

    public int SelectedSceneLevel { get; private set; }

    public int SelectedTimeOfDay { get; private set; }

    public string SelectedSeasonId { get; private set; }

    public string SelectedMapId
    {
      get
      {
        MapItemVM mapItemVm = this._availableMaps.Find((Predicate<MapItemVM>) (m => string.Equals(m.MapName, this.SearchText, StringComparison.OrdinalIgnoreCase)));
        return mapItemVm != null ? mapItemVm.MapId : this.SelectedMap.MapId;
      }
    }

    public MapItemVM SelectedMap { get; private set; }

    private List<MapItemVM> _battleMaps { get; set; }

    private List<MapItemVM> _villageMaps { get; set; }

    private List<MapItemVM> _siegeMaps { get; set; }

    private List<MapItemVM> _availableMaps { get; set; }

    public MapSelectionGroupVM()
    {
      this.MapSearchResults = new MBBindingList<MapItemVM>();
      this._battleMaps = new List<MapItemVM>();
      this._villageMaps = new List<MapItemVM>();
      this._siegeMaps = new List<MapItemVM>();
      this.WallHitpointSelection = new SelectorVM<WallHitpointItemVM>(0, new Action<SelectorVM<WallHitpointItemVM>>(this.OnWallHitpointSelection));
      this.SceneLevelSelection = new SelectorVM<SceneLevelItemVM>(0, new Action<SelectorVM<SceneLevelItemVM>>(this.OnSceneLevelSelection));
      this.SeasonSelection = new SelectorVM<SeasonItemVM>(0, new Action<SelectorVM<SeasonItemVM>>(this.OnSeasonSelection));
      this.TimeOfDaySelection = new SelectorVM<TimeOfDayItemVM>(0, new Action<SelectorVM<TimeOfDayItemVM>>(this.OnTimeOfDaySelection));
      this.RefreshValues();
    }

    public override void RefreshValues()
    {
      base.RefreshValues();
      this.PrepareMapLists();
      this.TitleText = new TextObject("{=w9m11T1y}Map").ToString();
      this.SeasonText = new TextObject("{=xTzDM5XE}Season").ToString();
      this.TimeOfDayText = new TextObject("{=DszSWnc3}Time of Day").ToString();
      this.SceneLevelText = new TextObject("{=0s52GQJt}Scene Level").ToString();
      this.WallHitpointsText = new TextObject("{=4IuXGSdc}Wall Hitpoints").ToString();
      this.AttackerSiegeMachinesText = new TextObject("{=AmfIfeIc}Choose Attacker Siege Machines").ToString();
      this.DefenderSiegeMachinesText = new TextObject("{=UoiSWe87}Choose Defender Siege Machines").ToString();
      this.SalloutText = new TextObject("{=EcKMGoFv}Sallyout").ToString();
      this.WallHitpointSelection.ItemList.Clear();
      this.SceneLevelSelection.ItemList.Clear();
      this.SeasonSelection.ItemList.Clear();
      this.TimeOfDaySelection.ItemList.Clear();
      foreach (Tuple<string, int> wallHitpoint in CustomBattleData.WallHitpoints)
        this.WallHitpointSelection.AddItem(new WallHitpointItemVM(wallHitpoint.Item1, wallHitpoint.Item2));
      foreach (int sceneLevel in CustomBattleData.SceneLevels)
        this.SceneLevelSelection.AddItem(new SceneLevelItemVM(sceneLevel));
      foreach (Tuple<string, string> season in CustomBattleData.Seasons)
        this.SeasonSelection.AddItem(new SeasonItemVM(season.Item1, season.Item2));
      foreach (Tuple<string, CustomBattleTimeOfDay> tuple in CustomBattleData.TimesOfDay)
        this.TimeOfDaySelection.AddItem(new TimeOfDayItemVM(tuple.Item1, (int) tuple.Item2));
      this.WallHitpointSelection.SelectedIndex = 0;
      this.SceneLevelSelection.SelectedIndex = 0;
      this.SeasonSelection.SelectedIndex = 0;
      this.TimeOfDaySelection.SelectedIndex = 0;
    }

    public void ExecuteSallyOutChange() => this.IsSallyOutSelected = !this.IsSallyOutSelected;

    private void PrepareMapLists()
    {
      this._battleMaps.Clear();
      this._villageMaps.Clear();
      this._siegeMaps.Clear();
      foreach (CustomBattleSceneData customBattleSceneData in CustomGame.Current.CustomBattleScenes.ToList<CustomBattleSceneData>())
      {
        MapItemVM mapItemVm = new MapItemVM(customBattleSceneData.Name.ToString(), customBattleSceneData.SceneID, new Action<MapItemVM>(this.OnMapSelection));
        if (customBattleSceneData.IsVillageMap)
          this._villageMaps.Add(mapItemVm);
        else if (customBattleSceneData.IsSiegeMap)
          this._siegeMaps.Add(mapItemVm);
        else
          this._battleMaps.Add(mapItemVm);
      }
      Comparer<MapItemVM> comparer = Comparer<MapItemVM>.Create((Comparison<MapItemVM>) ((x, y) => -x.MapName.CompareTo(y.MapName)));
      this._battleMaps.Sort((IComparer<MapItemVM>) comparer);
      this._villageMaps.Sort((IComparer<MapItemVM>) comparer);
      this._siegeMaps.Sort((IComparer<MapItemVM>) comparer);
    }

    private void OnMapSelection(MapItemVM item)
    {
      this.SelectedMap = item;
      this.SearchText = item.MapName;
    }

    private void OnWallHitpointSelection(SelectorVM<WallHitpointItemVM> selector) => this.SelectedWallBreachedCount = selector.SelectedItem.BreachedWallCount;

    private void OnSceneLevelSelection(SelectorVM<SceneLevelItemVM> selector) => this.SelectedSceneLevel = selector.SelectedItem.Level;

    private void OnSeasonSelection(SelectorVM<SeasonItemVM> selector) => this.SelectedSeasonId = selector.SelectedItem.SeasonId;

    private void OnTimeOfDaySelection(SelectorVM<TimeOfDayItemVM> selector) => this.SelectedTimeOfDay = selector.SelectedItem.TimeOfDay;

    public void OnGameTypeChange(CustomBattleGameType gameType)
    {
      this.MapSearchResults.Clear();
      switch (gameType)
      {
        case CustomBattleGameType.Battle:
          this.IsCurrentMapSiege = false;
          this._availableMaps = this._battleMaps;
          break;
        case CustomBattleGameType.Village:
          this.IsCurrentMapSiege = false;
          this._availableMaps = this._villageMaps;
          break;
        case CustomBattleGameType.Siege:
          this.IsCurrentMapSiege = true;
          this._availableMaps = this._siegeMaps;
          break;
      }
      foreach (MapItemVM availableMap in this._availableMaps)
        this.MapSearchResults.Add(availableMap);
      this.SelectedMap = this._availableMaps[this._availableMaps.Count - 1];
      this._searchText = new TextObject("{=7i1vmgQ9}Select a Map").ToString();
      this.OnPropertyChanged("SearchText");
    }

    public void RandomizeAll()
    {
      MBBindingList<MapItemVM> mapSearchResults = this.MapSearchResults;
      // ISSUE: explicit non-virtual call
      if ((mapSearchResults != null ? (__nonvirtual (mapSearchResults.Count) > 0 ? 1 : 0) : 0) != 0)
      {
        this.SearchText = "";
        this.MapSearchResults[MBRandom.RandomInt(this.MapSearchResults.Count)].ExecuteSelection();
      }
      this.SceneLevelSelection.ExecuteRandomize();
      this.SeasonSelection.ExecuteRandomize();
      this.WallHitpointSelection.ExecuteRandomize();
      this.TimeOfDaySelection.ExecuteRandomize();
    }

    private void RefreshSearch(bool isAppending)
    {
      if (isAppending)
      {
        foreach (MapItemVM mapItemVm in this.MapSearchResults.ToList<MapItemVM>())
        {
          if (mapItemVm.MapName.IndexOf(this._searchText, StringComparison.OrdinalIgnoreCase) < 0)
            this.MapSearchResults.Remove(mapItemVm);
          else
            mapItemVm.UpdateSearchedText(this._searchText);
        }
      }
      else
      {
        this.MapSearchResults.Clear();
        foreach (MapItemVM availableMap in this._availableMaps)
        {
          MapItemVM map = availableMap;
          if (map.MapName.IndexOf(this._searchText, StringComparison.OrdinalIgnoreCase) >= 0 && !this.MapSearchResults.Any<MapItemVM>((Func<MapItemVM, bool>) (m => m.MapName == map.MapName)))
            this.MapSearchResults.Add(map);
        }
        this._availableMaps.ForEach((Action<MapItemVM>) (m => m.UpdateSearchedText(this._searchText)));
      }
    }

    [DataSourceProperty]
    public MBBindingList<MapItemVM> MapSearchResults
    {
      get => this._mapSearchResults;
      set
      {
        if (value == this._mapSearchResults)
          return;
        this._mapSearchResults = value;
        this.OnPropertyChangedWithValue((object) value, nameof (MapSearchResults));
      }
    }

    [DataSourceProperty]
    public SelectorVM<SceneLevelItemVM> SceneLevelSelection
    {
      get => this._sceneLevelSelection;
      set
      {
        if (value == this._sceneLevelSelection)
          return;
        this._sceneLevelSelection = value;
        this.OnPropertyChangedWithValue((object) value, nameof (SceneLevelSelection));
      }
    }

    [DataSourceProperty]
    public SelectorVM<WallHitpointItemVM> WallHitpointSelection
    {
      get => this._wallHitpointSelection;
      set
      {
        if (value == this._wallHitpointSelection)
          return;
        this._wallHitpointSelection = value;
        this.OnPropertyChangedWithValue((object) value, nameof (WallHitpointSelection));
      }
    }

    [DataSourceProperty]
    public SelectorVM<SeasonItemVM> SeasonSelection
    {
      get => this._seasonSelection;
      set
      {
        if (value == this._seasonSelection)
          return;
        this._seasonSelection = value;
        this.OnPropertyChangedWithValue((object) value, nameof (SeasonSelection));
      }
    }

    [DataSourceProperty]
    public SelectorVM<TimeOfDayItemVM> TimeOfDaySelection
    {
      get => this._timeOfDaySelection;
      set
      {
        if (value == this._timeOfDaySelection)
          return;
        this._timeOfDaySelection = value;
        this.OnPropertyChangedWithValue((object) value, nameof (TimeOfDaySelection));
      }
    }

    [DataSourceProperty]
    public bool IsCurrentMapSiege
    {
      get => this._isCurrentMapSiege;
      set
      {
        if (value == this._isCurrentMapSiege)
          return;
        this._isCurrentMapSiege = value;
        this.OnPropertyChangedWithValue((object) value, nameof (IsCurrentMapSiege));
      }
    }

    [DataSourceProperty]
    public bool IsSallyOutSelected
    {
      get => this._isSallyOutSelected;
      set
      {
        if (value == this._isSallyOutSelected)
          return;
        this._isSallyOutSelected = value;
        this.OnPropertyChangedWithValue((object) value, nameof (IsSallyOutSelected));
      }
    }

    [DataSourceProperty]
    public string SearchText
    {
      get => this._searchText;
      set
      {
        if (!(value != this._searchText))
          return;
        bool isAppending = true;
        if (this._searchText != null && this._searchText != string.Empty)
          isAppending = value.IndexOf(this._searchText, StringComparison.OrdinalIgnoreCase) >= 0;
        this._searchText = value;
        this.RefreshSearch(isAppending);
        this.OnPropertyChangedWithValue((object) value, nameof (SearchText));
      }
    }

    [DataSourceProperty]
    public string SearchDefaultText
    {
      get => this._searchDefaultText;
      set
      {
        if (!(value != this._searchDefaultText))
          return;
        this._searchDefaultText = value;
        this.OnPropertyChanged(nameof (SearchDefaultText));
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
    public string SeasonText
    {
      get => this._seasonText;
      set
      {
        if (!(value != this._seasonText))
          return;
        this._seasonText = value;
        this.OnPropertyChangedWithValue((object) value, nameof (SeasonText));
      }
    }

    [DataSourceProperty]
    public string TimeOfDayText
    {
      get => this._timeOfDayText;
      set
      {
        if (!(value != this._timeOfDayText))
          return;
        this._timeOfDayText = value;
        this.OnPropertyChangedWithValue((object) value, nameof (TimeOfDayText));
      }
    }

    [DataSourceProperty]
    public string SceneLevelText
    {
      get => this._sceneLevelText;
      set
      {
        if (!(value != this._sceneLevelText))
          return;
        this._sceneLevelText = value;
        this.OnPropertyChangedWithValue((object) value, nameof (SceneLevelText));
      }
    }

    [DataSourceProperty]
    public string WallHitpointsText
    {
      get => this._wallHitpointsText;
      set
      {
        if (!(value != this._wallHitpointsText))
          return;
        this._wallHitpointsText = value;
        this.OnPropertyChangedWithValue((object) value, nameof (WallHitpointsText));
      }
    }

    [DataSourceProperty]
    public string AttackerSiegeMachinesText
    {
      get => this._attackerSiegeMachinesText;
      set
      {
        if (!(value != this._attackerSiegeMachinesText))
          return;
        this._attackerSiegeMachinesText = value;
        this.OnPropertyChangedWithValue((object) value, nameof (AttackerSiegeMachinesText));
      }
    }

    [DataSourceProperty]
    public string DefenderSiegeMachinesText
    {
      get => this._defenderSiegeMachinesText;
      set
      {
        if (!(value != this._defenderSiegeMachinesText))
          return;
        this._defenderSiegeMachinesText = value;
        this.OnPropertyChangedWithValue((object) value, nameof (DefenderSiegeMachinesText));
      }
    }

    [DataSourceProperty]
    public string SalloutText
    {
      get => this._salloutText;
      set
      {
        if (!(value != this._salloutText))
          return;
        this._salloutText = value;
        this.OnPropertyChangedWithValue((object) value, nameof (SalloutText));
      }
    }
  }
}
