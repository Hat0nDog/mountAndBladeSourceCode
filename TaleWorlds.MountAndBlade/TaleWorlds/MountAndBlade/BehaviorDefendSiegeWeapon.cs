// Decompiled with JetBrains decompiler
// Type: TaleWorlds.MountAndBlade.BehaviorDefendSiegeWeapon
// Assembly: TaleWorlds.MountAndBlade, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3C0E3F42-756F-4407-B817-06BA412A78B0
// Assembly location: E:\SteamLibrary\steamapps\common\Mount & Blade II Bannerlord\bin\Win64_Shipping_Client\TaleWorlds.MountAndBlade.dll

using System;
using TaleWorlds.DotNet;
using TaleWorlds.Engine;
using TaleWorlds.Library;

namespace TaleWorlds.MountAndBlade
{
  public class BehaviorDefendSiegeWeapon : BehaviorComponent
  {
    private WorldPosition _defensePosition = WorldPosition.Invalid;
    private TacticalPosition _tacticalDefendPosition;
    private SiegeWeapon _defendedSiegeWeapon;

    public BehaviorDefendSiegeWeapon(Formation formation)
      : base(formation)
    {
      this.CalculateCurrentOrder();
    }

    internal void SetDefensePositionFromTactic(WorldPosition defensePosition) => this._defensePosition = defensePosition;

    internal void SetDefendedSiegeWeaponFromTactic(SiegeWeapon siegeWeapon) => this._defendedSiegeWeapon = siegeWeapon;

    protected override void CalculateCurrentOrder()
    {
      Vec2 direction1 = this.formation.Direction;
      float num = 5f;
      Vec2 direction2;
      if (this._tacticalDefendPosition != null)
        direction2 = this._tacticalDefendPosition.IsInsurmountable ? (this.formation.Team.QuerySystem.AverageEnemyPosition - this._tacticalDefendPosition.Position.AsVec2).Normalized() : this._tacticalDefendPosition.Direction;
      else if (this.formation.QuerySystem.ClosestEnemyFormation == null)
        direction2 = this.formation.Direction;
      else if (this._defendedSiegeWeapon != null)
      {
        direction2 = this.formation.QuerySystem.ClosestEnemyFormation.MedianPosition.AsVec2 - this._defendedSiegeWeapon.GameEntity.GlobalPosition.AsVec2;
        num = Math.Max(Math.Min(direction2.Normalize(), 5f), !((NativeObject) this._defendedSiegeWeapon.WaitEntity != (NativeObject) null) ? 3f : (this._defendedSiegeWeapon.WaitEntity.GlobalPosition - this._defendedSiegeWeapon.GameEntity.GlobalPosition).Length);
      }
      else
        direction2 = ((double) this.formation.Direction.DotProduct((this.formation.QuerySystem.ClosestEnemyFormation.MedianPosition.AsVec2 - this.formation.QuerySystem.AveragePosition).Normalized()) < 0.5 ? this.formation.QuerySystem.ClosestEnemyFormation.MedianPosition.AsVec2 - this.formation.QuerySystem.AveragePosition : this.formation.Direction).Normalized();
      if (this._tacticalDefendPosition != null)
      {
        if (!this._tacticalDefendPosition.IsInsurmountable)
        {
          this.CurrentOrder = MovementOrder.MovementOrderMove(this._tacticalDefendPosition.Position);
        }
        else
        {
          Vec2 vec2 = this._tacticalDefendPosition.Position.AsVec2 + this._tacticalDefendPosition.Width * 0.5f * direction2;
          WorldPosition position = this._tacticalDefendPosition.Position;
          position.SetVec2(vec2);
          this.CurrentOrder = MovementOrder.MovementOrderMove(position);
        }
        if (!this._tacticalDefendPosition.IsInsurmountable)
          this.CurrentFacingOrder = FacingOrder.FacingOrderLookAtDirection(direction2);
        else
          this.CurrentFacingOrder = FacingOrder.FacingOrderLookAtEnemy;
      }
      else if (this._defensePosition.IsValid)
      {
        WorldPosition defensePosition = this._defensePosition;
        defensePosition.SetVec2(this._defensePosition.AsVec2 + direction2 * num);
        this.CurrentOrder = MovementOrder.MovementOrderMove(defensePosition);
        this.CurrentFacingOrder = FacingOrder.FacingOrderLookAtDirection(direction2);
      }
      else
      {
        WorldPosition medianPosition = this.formation.QuerySystem.MedianPosition;
        medianPosition.SetVec2(this.formation.QuerySystem.AveragePosition);
        this.CurrentOrder = MovementOrder.MovementOrderMove(medianPosition);
        this.CurrentFacingOrder = FacingOrder.FacingOrderLookAtDirection(direction2);
      }
    }

    protected internal override void TickOccasionally()
    {
      this.CalculateCurrentOrder();
      this.formation.MovementOrder = this.CurrentOrder;
      this.formation.FacingOrder = this.CurrentFacingOrder;
      if ((double) this.formation.QuerySystem.AveragePosition.DistanceSquared(this.CurrentOrder.GetPosition(this.formation).AsVec2) < 100.0)
      {
        if (this.formation.QuerySystem.HasShield)
          this.formation.ArrangementOrder = ArrangementOrder.ArrangementOrderLine;
        else if (this.formation.QuerySystem.ClosestSignificantlyLargeEnemyFormation != null && (double) this.formation.QuerySystem.AveragePosition.DistanceSquared(this.formation.QuerySystem.ClosestSignificantlyLargeEnemyFormation.MedianPosition.AsVec2) > 100.0 && (double) this.formation.QuerySystem.UnderRangedAttackRatio > 0.200000002980232 - (this.formation.ArrangementOrder.OrderEnum == ArrangementOrder.ArrangementOrderEnum.Loose ? 0.100000001490116 : 0.0))
          this.formation.ArrangementOrder = ArrangementOrder.ArrangementOrderLoose;
        if (this._tacticalDefendPosition == null)
          return;
        float customWidth;
        if (this._tacticalDefendPosition.TacticalPositionType == TacticalPosition.TacticalPositionTypeEnum.ChokePoint)
        {
          customWidth = this._tacticalDefendPosition.Width;
        }
        else
        {
          int countOfUnits = this.formation.CountOfUnits;
          customWidth = Math.Min(this._tacticalDefendPosition.Width, (float) ((double) this.formation.Interval * (double) (countOfUnits - 1) + (double) this.formation.UnitDiameter * (double) countOfUnits) / 3f);
        }
        this.formation.FormOrder = FormOrder.FormOrderCustom(customWidth);
      }
      else
        this.formation.ArrangementOrder = ArrangementOrder.ArrangementOrderLoose;
    }

    protected override void OnBehaviorActivatedAux()
    {
      this.CalculateCurrentOrder();
      this.formation.MovementOrder = this.CurrentOrder;
      this.formation.FacingOrder = this.CurrentFacingOrder;
      this.formation.ArrangementOrder = ArrangementOrder.ArrangementOrderLoose;
      this.formation.FiringOrder = FiringOrder.FiringOrderFireAtWill;
      this.formation.FormOrder = FormOrder.FormOrderWide;
      this.formation.WeaponUsageOrder = WeaponUsageOrder.WeaponUsageOrderUseAny;
    }

    internal override void ResetBehavior()
    {
      base.ResetBehavior();
      this._defensePosition = WorldPosition.Invalid;
      this._tacticalDefendPosition = (TacticalPosition) null;
    }

    protected override float GetAiWeight() => 1f;
  }
}
