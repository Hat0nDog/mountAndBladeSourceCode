// Decompiled with JetBrains decompiler
// Type: TaleWorlds.MountAndBlade.CustomBattle.CustomBattle.SelectionItem.MapItemVM
// Assembly: TaleWorlds.MountAndBlade.CustomBattle, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 8065FEE3-AE77-4E53-B13C-3890101BA431
// Assembly location: E:\SteamLibrary\steamapps\common\Mount & Blade II Bannerlord\Modules\CustomBattle\bin\Win64_Shipping_Client\TaleWorlds.MountAndBlade.CustomBattle.dll

using System;
using TaleWorlds.Library;

namespace TaleWorlds.MountAndBlade.CustomBattle.CustomBattle.SelectionItem
{
  public class MapItemVM : ViewModel
  {
    private string _searchedText;
    private Action<MapItemVM> _onSelection;
    public string _nameText;

    public string MapName { get; private set; }

    public string MapId { get; private set; }

    public MapItemVM(string mapName, string mapId, Action<MapItemVM> onSelection)
    {
      this.MapName = mapName;
      this.MapId = mapId;
      this.NameText = mapName;
      this._onSelection = onSelection;
    }

    public void UpdateSearchedText(string searchedText)
    {
      this._searchedText = searchedText;
      string oldValue = (string) null;
      if (this.MapName.IndexOf(this._searchedText, StringComparison.OrdinalIgnoreCase) != -1)
        oldValue = this.MapName.Substring(this.MapName.IndexOf(this._searchedText, StringComparison.OrdinalIgnoreCase), this._searchedText.Length);
      if (!string.IsNullOrEmpty(oldValue))
        this.NameText = this.MapName.Replace(oldValue, "<a>" + oldValue + "</a>");
      else
        this.NameText = this.MapName;
    }

    public void ExecuteSelection() => this._onSelection(this);

    [DataSourceProperty]
    public string NameText
    {
      get => this._nameText;
      set
      {
        if (!(this._nameText != value))
          return;
        this._nameText = value;
        this.OnPropertyChangedWithValue((object) value, nameof (NameText));
      }
    }
  }
}
