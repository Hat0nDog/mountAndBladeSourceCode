// Decompiled with JetBrains decompiler
// Type: TaleWorlds.MountAndBlade.BehaviorDefensiveRing
// Assembly: TaleWorlds.MountAndBlade, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3C0E3F42-756F-4407-B817-06BA412A78B0
// Assembly location: E:\SteamLibrary\steamapps\common\Mount & Blade II Bannerlord\bin\Win64_Shipping_Client\TaleWorlds.MountAndBlade.dll

using System;
using System.Linq;
using TaleWorlds.Engine;
using TaleWorlds.Library;

namespace TaleWorlds.MountAndBlade
{
  public class BehaviorDefensiveRing : BehaviorComponent
  {
    internal TacticalPosition TacticalDefendPosition;

    public BehaviorDefensiveRing(Formation formation)
      : base(formation)
    {
      this.CalculateCurrentOrder();
    }

    protected override void CalculateCurrentOrder()
    {
      Vec2 direction = this.TacticalDefendPosition == null ? (this.formation.QuerySystem.ClosestEnemyFormation != null ? ((double) this.formation.Direction.DotProduct((this.formation.QuerySystem.ClosestEnemyFormation.MedianPosition.AsVec2 - this.formation.QuerySystem.AveragePosition).Normalized()) < 0.5 ? this.formation.QuerySystem.ClosestEnemyFormation.MedianPosition.AsVec2 - this.formation.QuerySystem.AveragePosition : this.formation.Direction).Normalized() : this.formation.Direction) : this.TacticalDefendPosition.Direction;
      if (this.TacticalDefendPosition != null)
      {
        this.CurrentOrder = MovementOrder.MovementOrderMove(this.TacticalDefendPosition.Position);
        this.CurrentFacingOrder = FacingOrder.FacingOrderLookAtDirection(direction);
      }
      else
      {
        WorldPosition medianPosition = this.formation.QuerySystem.MedianPosition;
        medianPosition.SetVec2(this.formation.QuerySystem.AveragePosition);
        this.CurrentOrder = MovementOrder.MovementOrderMove(medianPosition);
        this.CurrentFacingOrder = FacingOrder.FacingOrderLookAtDirection(direction);
      }
    }

    protected internal override void TickOccasionally()
    {
      this.CalculateCurrentOrder();
      this.formation.MovementOrder = this.CurrentOrder;
      this.formation.FacingOrder = this.CurrentFacingOrder;
      if ((double) this.formation.QuerySystem.AveragePosition.Distance(this.CurrentOrder.GetPosition(this.formation).AsVec2) - (double) this.formation.Width < 10.0)
      {
        this.formation.ArrangementOrder = ArrangementOrder.ArrangementOrderCircle;
        if (!this.formation.Team.Formations.Any<Formation>((Func<Formation, bool>) (f => f.QuerySystem.IsRangedFormation)))
          return;
        Formation formation = this.formation.Team.Formations.Where<Formation>((Func<Formation, bool>) (f => f.QuerySystem.IsRangedFormation)).MaxBy<Formation, int>((Func<Formation, int>) (f => f.CountOfUnits));
        int num = (int) Math.Sqrt((double) formation.CountOfUnits);
        this.formation.FormOrder = FormOrder.FormOrderCustom(((float) (((double) num * (double) formation.UnitDiameter + (double) (num - 1) * (double) formation.Interval) * 0.5 * 1.41421294212341) + this.formation.Depth) * 2f);
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
      this.formation.FormOrder = FormOrder.FormOrderWider;
      this.formation.WeaponUsageOrder = WeaponUsageOrder.WeaponUsageOrderUseAny;
    }

    internal override void ResetBehavior()
    {
      base.ResetBehavior();
      this.TacticalDefendPosition = (TacticalPosition) null;
    }

    protected override float GetAiWeight() => this.TacticalDefendPosition == null ? 0.0f : 1f;
  }
}
