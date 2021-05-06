// Decompiled with JetBrains decompiler
// Type: TaleWorlds.MountAndBlade.SiegeWeaponController
// Assembly: TaleWorlds.MountAndBlade, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3C0E3F42-756F-4407-B817-06BA412A78B0
// Assembly location: E:\SteamLibrary\steamapps\common\Mount & Blade II Bannerlord\bin\Win64_Shipping_Client\TaleWorlds.MountAndBlade.dll

using NetworkMessages.FromClient;
using System;
using System.Collections.Generic;
using System.Linq;
using TaleWorlds.Core;
using TaleWorlds.Library;
using TaleWorlds.MountAndBlade.Network.Messages;

namespace TaleWorlds.MountAndBlade
{
  public class SiegeWeaponController
  {
    private readonly Mission _mission;
    private readonly Team _team;
    private List<SiegeWeapon> _availableWeapons;
    private List<SiegeWeapon> _selectedWeapons;

    public MBReadOnlyList<SiegeWeapon> SelectedWeapons { get; }

    public event Action<SiegeWeaponOrderType, IEnumerable<SiegeWeapon>> OnOrderIssued;

    public event Action OnSelectedSiegeWeaponsChanged;

    public SiegeWeaponController(Mission mission, Team team)
    {
      this._mission = mission;
      this._team = team;
      this._selectedWeapons = new List<SiegeWeapon>();
      this.SelectedWeapons = new MBReadOnlyList<SiegeWeapon>(this._selectedWeapons);
      this.InitializeWeaponsForDeployment();
    }

    private void InitializeWeaponsForDeployment() => this._availableWeapons = this._mission.ActiveMissionObjects.FindAllWithType<DeploymentPoint>().Where<DeploymentPoint>((Func<DeploymentPoint, bool>) (dp => dp.Side == this._team.Side)).SelectMany<DeploymentPoint, SynchedMissionObject>((Func<DeploymentPoint, IEnumerable<SynchedMissionObject>>) (dp => dp.DeployableWeapons)).Select<SynchedMissionObject, SiegeWeapon>((Func<SynchedMissionObject, SiegeWeapon>) (w => w as SiegeWeapon)).ToList<SiegeWeapon>();

    private void InitializeWeapons()
    {
      this._availableWeapons = new List<SiegeWeapon>();
      this._availableWeapons.AddRange((IEnumerable<SiegeWeapon>) this._mission.ActiveMissionObjects.FindAllWithType<RangedSiegeWeapon>().Where<RangedSiegeWeapon>((Func<RangedSiegeWeapon, bool>) (w => w.Side == this._team.Side)));
      if (this._team.Side == BattleSideEnum.Attacker)
        this._availableWeapons.AddRange(this._mission.ActiveMissionObjects.FindAllWithType<SiegeWeapon>().Where<SiegeWeapon>((Func<SiegeWeapon, bool>) (w => w is IPrimarySiegeWeapon && !(w is RangedSiegeWeapon))));
      this._availableWeapons.Sort((Comparison<SiegeWeapon>) ((w1, w2) => this.GetShortcutIndexOf(w1).CompareTo(this.GetShortcutIndexOf(w2))));
    }

    public void Select(SiegeWeapon weapon)
    {
      if (this.SelectedWeapons.Contains(weapon) || !SiegeWeaponController.IsWeaponSelectable(weapon))
        return;
      if (GameNetwork.IsClient)
      {
        GameNetwork.BeginModuleEventAsClient();
        GameNetwork.WriteMessage((GameNetworkMessage) new SelectSiegeWeapon(weapon));
        GameNetwork.EndModuleEventAsClient();
      }
      this._selectedWeapons.Add(weapon);
      Action siegeWeaponsChanged = this.OnSelectedSiegeWeaponsChanged;
      if (siegeWeaponsChanged == null)
        return;
      siegeWeaponsChanged();
    }

    public void ClearSelectedWeapons()
    {
      int num = GameNetwork.IsClient ? 1 : 0;
      this._selectedWeapons.Clear();
      Action siegeWeaponsChanged = this.OnSelectedSiegeWeaponsChanged;
      if (siegeWeaponsChanged == null)
        return;
      siegeWeaponsChanged();
    }

    public void Deselect(SiegeWeapon weapon)
    {
      if (!this.SelectedWeapons.Contains(weapon))
        return;
      if (GameNetwork.IsClient)
      {
        GameNetwork.BeginModuleEventAsClient();
        GameNetwork.WriteMessage((GameNetworkMessage) new UnselectSiegeWeapon(weapon));
        GameNetwork.EndModuleEventAsClient();
      }
      this._selectedWeapons.Remove(weapon);
      Action siegeWeaponsChanged = this.OnSelectedSiegeWeaponsChanged;
      if (siegeWeaponsChanged == null)
        return;
      siegeWeaponsChanged();
    }

    public void SelectAll()
    {
      if (GameNetwork.IsClient)
      {
        GameNetwork.BeginModuleEventAsClient();
        GameNetwork.WriteMessage((GameNetworkMessage) new SelectAllSiegeWeapons());
        GameNetwork.EndModuleEventAsClient();
      }
      this._selectedWeapons.Clear();
      foreach (SiegeWeapon availableWeapon in this._availableWeapons)
        this._selectedWeapons.Add(availableWeapon);
      Action siegeWeaponsChanged = this.OnSelectedSiegeWeaponsChanged;
      if (siegeWeaponsChanged == null)
        return;
      siegeWeaponsChanged();
    }

    public static bool IsWeaponSelectable(SiegeWeapon weapon) => !weapon.IsDeactivated;

    public static SiegeWeaponOrderType GetActiveOrderOf(SiegeWeapon weapon)
    {
      if (!weapon.ForcedUse)
        return SiegeWeaponOrderType.Stop;
      if (!(weapon is RangedSiegeWeapon))
        return SiegeWeaponOrderType.Attack;
      switch (((RangedSiegeWeapon) weapon).Focus)
      {
        case RangedSiegeWeapon.FiringFocus.Troops:
          return SiegeWeaponOrderType.FireAtTroops;
        case RangedSiegeWeapon.FiringFocus.Walls:
          return SiegeWeaponOrderType.FireAtWalls;
        case RangedSiegeWeapon.FiringFocus.RangedSiegeWeapons:
          return SiegeWeaponOrderType.FireAtRangedSiegeWeapons;
        case RangedSiegeWeapon.FiringFocus.PrimarySiegeWeapons:
          return SiegeWeaponOrderType.FireAtPrimarySiegeWeapons;
        default:
          return SiegeWeaponOrderType.FireAtTroops;
      }
    }

    public static SiegeWeaponOrderType GetActiveMovementOrderOf(
      SiegeWeapon weapon)
    {
      return !weapon.ForcedUse ? SiegeWeaponOrderType.Stop : SiegeWeaponOrderType.Attack;
    }

    public static SiegeWeaponOrderType GetActiveFacingOrderOf(SiegeWeapon weapon)
    {
      if (!(weapon is RangedSiegeWeapon))
        return SiegeWeaponOrderType.FireAtWalls;
      switch (((RangedSiegeWeapon) weapon).Focus)
      {
        case RangedSiegeWeapon.FiringFocus.Troops:
          return SiegeWeaponOrderType.FireAtTroops;
        case RangedSiegeWeapon.FiringFocus.Walls:
          return SiegeWeaponOrderType.FireAtWalls;
        case RangedSiegeWeapon.FiringFocus.RangedSiegeWeapons:
          return SiegeWeaponOrderType.FireAtRangedSiegeWeapons;
        case RangedSiegeWeapon.FiringFocus.PrimarySiegeWeapons:
          return SiegeWeaponOrderType.FireAtPrimarySiegeWeapons;
        default:
          return SiegeWeaponOrderType.FireAtTroops;
      }
    }

    public static SiegeWeaponOrderType GetActiveFiringOrderOf(SiegeWeapon weapon) => !weapon.ForcedUse ? SiegeWeaponOrderType.Stop : SiegeWeaponOrderType.Attack;

    public static SiegeWeaponOrderType GetActiveAIControlOrderOf(
      SiegeWeapon weapon)
    {
      return weapon.ForcedUse ? SiegeWeaponOrderType.AIControlOn : SiegeWeaponOrderType.AIControlOff;
    }

    private void SetOrderAux(SiegeWeaponOrderType order, SiegeWeapon weapon)
    {
      switch (order)
      {
        case SiegeWeaponOrderType.Stop:
        case SiegeWeaponOrderType.AIControlOff:
          weapon.ForcedUse = false;
          break;
        case SiegeWeaponOrderType.Attack:
        case SiegeWeaponOrderType.AIControlOn:
          weapon.ForcedUse = true;
          break;
        case SiegeWeaponOrderType.FireAtWalls:
          weapon.ForcedUse = true;
          if (!(weapon is RangedSiegeWeapon rangedSiegeWeapon5))
            break;
          rangedSiegeWeapon5.Focus = RangedSiegeWeapon.FiringFocus.Walls;
          break;
        case SiegeWeaponOrderType.FireAtTroops:
          weapon.ForcedUse = true;
          if (!(weapon is RangedSiegeWeapon rangedSiegeWeapon6))
            break;
          rangedSiegeWeapon6.Focus = RangedSiegeWeapon.FiringFocus.Troops;
          break;
        case SiegeWeaponOrderType.FireAtRangedSiegeWeapons:
          weapon.ForcedUse = true;
          if (!(weapon is RangedSiegeWeapon rangedSiegeWeapon7))
            break;
          rangedSiegeWeapon7.Focus = RangedSiegeWeapon.FiringFocus.RangedSiegeWeapons;
          break;
        case SiegeWeaponOrderType.FireAtPrimarySiegeWeapons:
          weapon.ForcedUse = true;
          if (!(weapon is RangedSiegeWeapon rangedSiegeWeapon8))
            break;
          rangedSiegeWeapon8.Focus = RangedSiegeWeapon.FiringFocus.PrimarySiegeWeapons;
          break;
      }
    }

    public void SetOrder(SiegeWeaponOrderType order)
    {
      if (GameNetwork.IsClient)
      {
        GameNetwork.BeginModuleEventAsClient();
        GameNetwork.WriteMessage((GameNetworkMessage) new ApplySiegeWeaponOrder(order));
        GameNetwork.EndModuleEventAsClient();
      }
      foreach (SiegeWeapon selectedWeapon in this.SelectedWeapons)
        this.SetOrderAux(order, selectedWeapon);
      Action<SiegeWeaponOrderType, IEnumerable<SiegeWeapon>> onOrderIssued = this.OnOrderIssued;
      if (onOrderIssued == null)
        return;
      onOrderIssued(order, (IEnumerable<SiegeWeapon>) this.SelectedWeapons);
    }

    public int GetShortcutIndexOf(SiegeWeapon weapon)
    {
      int num1;
      switch (SiegeWeaponController.GetSideOf(weapon))
      {
        case FormationAI.BehaviorSide.Left:
          num1 = 1;
          break;
        case FormationAI.BehaviorSide.Right:
          num1 = 2;
          break;
        default:
          num1 = 0;
          break;
      }
      int num2 = num1;
      if (!(weapon is IPrimarySiegeWeapon))
        num2 += 3;
      return num2;
    }

    private static FormationAI.BehaviorSide GetSideOf(SiegeWeapon weapon)
    {
      if (weapon is IPrimarySiegeWeapon primarySiegeWeapon)
        return primarySiegeWeapon.WeaponSide;
      RangedSiegeWeapon rangedSiegeWeapon = weapon as RangedSiegeWeapon;
      return FormationAI.BehaviorSide.Middle;
    }
  }
}
