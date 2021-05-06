// Decompiled with JetBrains decompiler
// Type: TaleWorlds.MountAndBlade.BehaviorDefend
// Assembly: TaleWorlds.MountAndBlade, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3C0E3F42-756F-4407-B817-06BA412A78B0
// Assembly location: E:\SteamLibrary\steamapps\common\Mount & Blade II Bannerlord\bin\Win64_Shipping_Client\TaleWorlds.MountAndBlade.dll

using System;
using TaleWorlds.Engine;
using TaleWorlds.Library;

namespace TaleWorlds.MountAndBlade
{
  public class BehaviorDefend : BehaviorComponent
  {
    private WorldPosition _defensePosition = WorldPosition.Invalid;
    internal TacticalPosition TacticalDefendPosition;

    public WorldPosition DefensePosition
    {
      get => this._defensePosition;
      set => this._defensePosition = value;
    }

    public BehaviorDefend(Formation formation)
      : base(formation)
    {
      this.CalculateCurrentOrder();
    }

    protected override void CalculateCurrentOrder()
    {
      Vec2 direction1 = this.formation.Direction;
      Vec2 direction2;
      WorldPosition worldPosition;
      if (this.TacticalDefendPosition != null)
        direction2 = this.TacticalDefendPosition.IsInsurmountable ? (this.formation.Team.QuerySystem.AverageEnemyPosition - this.TacticalDefendPosition.Position.AsVec2).Normalized() : this.TacticalDefendPosition.Direction;
      else if (this.formation.QuerySystem.ClosestEnemyFormation == null)
      {
        direction2 = this.formation.Direction;
      }
      else
      {
        Vec2 vec2;
        if ((double) this.formation.Direction.DotProduct((this.formation.QuerySystem.ClosestEnemyFormation.MedianPosition.AsVec2 - this.formation.QuerySystem.AveragePosition).Normalized()) >= 0.5)
        {
          vec2 = this.formation.Direction;
        }
        else
        {
          worldPosition = this.formation.QuerySystem.ClosestEnemyFormation.MedianPosition;
          vec2 = worldPosition.AsVec2 - this.formation.QuerySystem.AveragePosition;
        }
        direction2 = vec2.Normalized();
      }
      if (this.TacticalDefendPosition != null)
      {
        if (!this.TacticalDefendPosition.IsInsurmountable)
        {
          this.CurrentOrder = MovementOrder.MovementOrderMove(this.TacticalDefendPosition.Position);
        }
        else
        {
          worldPosition = this.TacticalDefendPosition.Position;
          Vec2 vec2 = worldPosition.AsVec2 + this.TacticalDefendPosition.Width * 0.5f * direction2;
          WorldPosition position = this.TacticalDefendPosition.Position;
          position.SetVec2(vec2);
          this.CurrentOrder = MovementOrder.MovementOrderMove(position);
        }
        if (!this.TacticalDefendPosition.IsInsurmountable)
          this.CurrentFacingOrder = FacingOrder.FacingOrderLookAtDirection(direction2);
        else
          this.CurrentFacingOrder = FacingOrder.FacingOrderLookAtEnemy;
      }
      else
      {
        worldPosition = this.DefensePosition;
        if (worldPosition.IsValid)
        {
          this.CurrentOrder = MovementOrder.MovementOrderMove(this.DefensePosition);
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
    }

    protected internal override void TickOccasionally()
    {
      this.CalculateCurrentOrder();
      this.formation.MovementOrder = this.CurrentOrder;
      this.formation.FacingOrder = this.CurrentFacingOrder;
      if ((double) this.formation.QuerySystem.AveragePosition.DistanceSquared(this.CurrentOrder.GetPosition(this.formation).AsVec2) < 100.0)
      {
        if (this.formation.QuerySystem.HasShield)
          this.formation.ArrangementOrder = ArrangementOrder.ArrangementOrderShieldWall;
        else if (this.formation.QuerySystem.ClosestSignificantlyLargeEnemyFormation != null && (double) this.formation.QuerySystem.AveragePosition.DistanceSquared(this.formation.QuerySystem.ClosestSignificantlyLargeEnemyFormation.MedianPosition.AsVec2) > 100.0 && (double) this.formation.QuerySystem.UnderRangedAttackRatio > 0.200000002980232 - (this.formation.ArrangementOrder.OrderEnum == ArrangementOrder.ArrangementOrderEnum.Loose ? 0.100000001490116 : 0.0))
          this.formation.ArrangementOrder = ArrangementOrder.ArrangementOrderLoose;
        if (this.TacticalDefendPosition == null)
          return;
        float customWidth;
        if (this.TacticalDefendPosition.TacticalPositionType == TacticalPosition.TacticalPositionTypeEnum.ChokePoint)
        {
          customWidth = this.TacticalDefendPosition.Width;
        }
        else
        {
          int countOfUnits = this.formation.CountOfUnits;
          customWidth = Math.Min(this.TacticalDefendPosition.Width, (float) ((double) this.formation.Interval * (double) (countOfUnits - 1) + (double) this.formation.UnitDiameter * (double) countOfUnits) / 3f);
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
      this.TacticalDefendPosition = (TacticalPosition) null;
    }

    protected override float GetAiWeight() => 1f;
  }
}
