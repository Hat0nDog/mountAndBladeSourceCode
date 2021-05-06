// Decompiled with JetBrains decompiler
// Type: TaleWorlds.MountAndBlade.BehaviorDefendKeyPosition
// Assembly: TaleWorlds.MountAndBlade, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3C0E3F42-756F-4407-B817-06BA412A78B0
// Assembly location: E:\SteamLibrary\steamapps\common\Mount & Blade II Bannerlord\bin\Win64_Shipping_Client\TaleWorlds.MountAndBlade.dll

using System;
using TaleWorlds.Engine;
using TaleWorlds.Library;

namespace TaleWorlds.MountAndBlade
{
  public class BehaviorDefendKeyPosition : BehaviorComponent
  {
    private WorldPosition _defensePosition = WorldPosition.Invalid;
    public WorldPosition EnemyClusterPosition = WorldPosition.Invalid;
    private readonly QueryData<WorldPosition> _behaviorPosition;

    public WorldPosition DefensePosition
    {
      get => this._behaviorPosition.Value;
      set => this._defensePosition = value;
    }

    public BehaviorDefendKeyPosition(Formation formation)
      : base(formation)
    {
      this._behaviorPosition = new QueryData<WorldPosition>((Func<WorldPosition>) (() => Mission.Current.FindBestDefendingPosition(this.EnemyClusterPosition, this._defensePosition)), 5f);
      this.CalculateCurrentOrder();
    }

    protected override void CalculateCurrentOrder()
    {
      Vec2 direction = this.formation.QuerySystem.ClosestEnemyFormation != null ? ((double) this.formation.Direction.DotProduct((this.formation.QuerySystem.ClosestEnemyFormation.MedianPosition.AsVec2 - this.formation.QuerySystem.AveragePosition).Normalized()) < 0.5 ? this.formation.QuerySystem.ClosestEnemyFormation.MedianPosition.AsVec2 - this.formation.QuerySystem.AveragePosition : this.formation.Direction).Normalized() : this.formation.Direction;
      if (this.DefensePosition.IsValid)
      {
        this.CurrentOrder = MovementOrder.MovementOrderMove(this.DefensePosition);
      }
      else
      {
        WorldPosition medianPosition = this.formation.QuerySystem.MedianPosition;
        medianPosition.SetVec2(this.formation.QuerySystem.AveragePosition);
        this.CurrentOrder = MovementOrder.MovementOrderMove(medianPosition);
      }
      this.CurrentFacingOrder = FacingOrder.FacingOrderLookAtDirection(direction);
    }

    protected internal override void TickOccasionally()
    {
      this.CalculateCurrentOrder();
      this.formation.MovementOrder = this.CurrentOrder;
      this.formation.FacingOrder = this.CurrentFacingOrder;
      if (this.formation.QuerySystem.HasShield && (double) this.formation.QuerySystem.AveragePosition.DistanceSquared(this.CurrentOrder.GetPosition(this.formation).AsVec2) < (double) this.formation.Depth * (double) this.formation.Depth * 4.0)
        this.formation.ArrangementOrder = ArrangementOrder.ArrangementOrderShieldWall;
      else
        this.formation.ArrangementOrder = ArrangementOrder.ArrangementOrderLoose;
    }

    protected override void OnBehaviorActivatedAux()
    {
      this.formation.ArrangementOrder = ArrangementOrder.ArrangementOrderLoose;
      this.formation.FiringOrder = FiringOrder.FiringOrderFireAtWill;
      this.formation.FormOrder = FormOrder.FormOrderWide;
      this.formation.WeaponUsageOrder = WeaponUsageOrder.WeaponUsageOrderUseAny;
    }

    protected override float GetAiWeight() => 10f;
  }
}
