// Decompiled with JetBrains decompiler
// Type: TaleWorlds.MountAndBlade.BehaviorDestroySiegeWeapons
// Assembly: TaleWorlds.MountAndBlade, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3C0E3F42-756F-4407-B817-06BA412A78B0
// Assembly location: E:\SteamLibrary\steamapps\common\Mount & Blade II Bannerlord\bin\Win64_Shipping_Client\TaleWorlds.MountAndBlade.dll

using System;
using System.Collections.Generic;
using System.Linq;
using TaleWorlds.Core;

namespace TaleWorlds.MountAndBlade
{
  public class BehaviorDestroySiegeWeapons : BehaviorComponent
  {
    private readonly List<SiegeWeapon> _allWeapons;
    private List<SiegeWeapon> _targetWeapons;
    internal SiegeWeapon LastTargetWeapon;
    private bool _isTargetPrimaryWeapon;

    private void DetermineTargetWeapons()
    {
      this._targetWeapons = this._allWeapons.Where<SiegeWeapon>((Func<SiegeWeapon, bool>) (w => w is IPrimarySiegeWeapon && (w as IPrimarySiegeWeapon).WeaponSide == this.behaviorSide && w.IsDestructible && !w.IsDestroyed)).ToList<SiegeWeapon>();
      if (this._targetWeapons.IsEmpty<SiegeWeapon>())
      {
        this._targetWeapons = this._allWeapons.Where<SiegeWeapon>((Func<SiegeWeapon, bool>) (w => !(w is IPrimarySiegeWeapon) && w.IsDestructible)).ToList<SiegeWeapon>();
        this._isTargetPrimaryWeapon = false;
      }
      else
        this._isTargetPrimaryWeapon = true;
    }

    public BehaviorDestroySiegeWeapons(Formation formation)
      : base(formation)
    {
      this.BehaviorCoherence = 0.2f;
      this.behaviorSide = formation.AI.Side;
      this._allWeapons = Mission.Current.ActiveMissionObjects.FindAllWithType<SiegeWeapon>().Where<SiegeWeapon>((Func<SiegeWeapon, bool>) (sw => sw.Side != formation.Team.Side)).ToList<SiegeWeapon>();
      this.DetermineTargetWeapons();
      this.CurrentOrder = MovementOrder.MovementOrderCharge;
    }

    internal override void OnValidBehaviorSideSet()
    {
      base.OnValidBehaviorSideSet();
      this.DetermineTargetWeapons();
    }

    protected internal override void TickOccasionally()
    {
      base.TickOccasionally();
      this._targetWeapons.RemoveAll((Predicate<SiegeWeapon>) (tw => tw.IsDestroyed));
      if (this._targetWeapons.Count == 0)
        this.DetermineTargetWeapons();
      if (this.formation.AI.ActiveBehavior != this)
        return;
      if (this._targetWeapons.Count == 0)
      {
        if (this.CurrentOrder != (object) MovementOrder.MovementOrderCharge)
          this.CurrentOrder = MovementOrder.MovementOrderCharge;
        this._isTargetPrimaryWeapon = false;
      }
      else
      {
        SiegeWeapon siegeWeapon = this._targetWeapons.MinBy<SiegeWeapon, float>((Func<SiegeWeapon, float>) (tw => this.formation.QuerySystem.AveragePosition.DistanceSquared(tw.GameEntity.GlobalPosition.AsVec2)));
        if (this.CurrentOrder.OrderEnum != MovementOrder.MovementOrderEnum.AttackEntity || this.LastTargetWeapon != siegeWeapon)
        {
          this.LastTargetWeapon = siegeWeapon;
          this.CurrentOrder = MovementOrder.MovementOrderAttackEntity(this.LastTargetWeapon.GameEntity, true);
        }
      }
      this.formation.MovementOrder = this.CurrentOrder;
    }

    protected override void OnBehaviorActivatedAux()
    {
      this.DetermineTargetWeapons();
      this.formation.ArrangementOrder = this.formation.QuerySystem.IsCavalryFormation || this.formation.QuerySystem.IsRangedCavalryFormation ? ArrangementOrder.ArrangementOrderSkein : ArrangementOrder.ArrangementOrderLine;
      this.formation.FacingOrder = FacingOrder.FacingOrderLookAtEnemy;
      this.formation.FiringOrder = FiringOrder.FiringOrderFireAtWill;
      this.formation.FormOrder = FormOrder.FormOrderDeep;
      this.formation.WeaponUsageOrder = WeaponUsageOrder.WeaponUsageOrderUseAny;
    }

    protected internal override float NavmeshlessTargetPositionPenalty => 1f;

    protected override float GetAiWeight()
    {
      if (this._targetWeapons.IsEmpty<SiegeWeapon>())
        return 0.0f;
      return !this._isTargetPrimaryWeapon ? 0.7f : 1.3f;
    }
  }
}
