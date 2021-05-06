// Decompiled with JetBrains decompiler
// Type: TaleWorlds.MountAndBlade.CustomBattle.CustomBattle.CustomBattleSiegeMachineVM
// Assembly: TaleWorlds.MountAndBlade.CustomBattle, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 8065FEE3-AE77-4E53-B13C-3890101BA431
// Assembly location: E:\SteamLibrary\steamapps\common\Mount & Blade II Bannerlord\Modules\CustomBattle\bin\Win64_Shipping_Client\TaleWorlds.MountAndBlade.CustomBattle.dll

using System;
using TaleWorlds.Core;
using TaleWorlds.Library;

namespace TaleWorlds.MountAndBlade.CustomBattle.CustomBattle
{
  public class CustomBattleSiegeMachineVM : ViewModel
  {
    private Action<CustomBattleSiegeMachineVM> _onSelection;
    private string _name;
    private bool _isRanged;
    private string _machineID;

    public SiegeEngineType SiegeEngineType { get; private set; }

    public CustomBattleSiegeMachineVM(
      SiegeEngineType machineType,
      Action<CustomBattleSiegeMachineVM> onSelection)
    {
      this._onSelection = onSelection;
      this.SetMachineType(machineType);
    }

    public void SetMachineType(SiegeEngineType machine)
    {
      this.SiegeEngineType = machine;
      this.Name = machine != null ? machine.StringId : "";
      this.IsRanged = machine != null && machine.IsRanged;
      this.MachineID = machine != null ? machine.StringId : "";
    }

    private void OnSelection() => this._onSelection(this);

    [DataSourceProperty]
    public bool IsRanged
    {
      get => this._isRanged;
      set
      {
        if (value == this._isRanged)
          return;
        this._isRanged = value;
        this.OnPropertyChangedWithValue((object) value, nameof (IsRanged));
      }
    }

    [DataSourceProperty]
    public string MachineID
    {
      get => this._machineID;
      set
      {
        if (!(value != this._machineID))
          return;
        this._machineID = value;
        this.OnPropertyChangedWithValue((object) value, nameof (MachineID));
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
  }
}
