// Decompiled with JetBrains decompiler
// Type: TaleWorlds.MountAndBlade.BehaviorFireFromInfantryCover
// Assembly: TaleWorlds.MountAndBlade, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3C0E3F42-756F-4407-B817-06BA412A78B0
// Assembly location: E:\SteamLibrary\steamapps\common\Mount & Blade II Bannerlord\bin\Win64_Shipping_Client\TaleWorlds.MountAndBlade.dll

using System;
using System.Linq;
using TaleWorlds.Engine;
using TaleWorlds.Library;

namespace TaleWorlds.MountAndBlade
{
  public class BehaviorFireFromInfantryCover : BehaviorComponent
  {
    public Formation _mainFormation;
    private bool _isFireAtWill = true;

    public BehaviorFireFromInfantryCover(Formation formation)
      : base(formation)
    {
      this._mainFormation = formation.Team.Formations.FirstOrDefault<Formation>((Func<Formation, bool>) (f => f.AI.IsMainFormation));
      this.CalculateCurrentOrder();
    }

    protected override void CalculateCurrentOrder()
    {
      WorldPosition medianPosition = this.formation.QuerySystem.MedianPosition;
      if (this._mainFormation == null)
      {
        medianPosition.SetVec2(this.formation.QuerySystem.AveragePosition);
      }
      else
      {
        Vec2 asVec2 = this._mainFormation.MovementOrder.GetPosition(this._mainFormation).AsVec2;
        if (asVec2.IsValid)
        {
          Vec2 vec2_1 = (asVec2 - this._mainFormation.QuerySystem.AveragePosition).Normalized();
          Vec2 vec2_2 = asVec2 - vec2_1 * this._mainFormation.Depth * 2f;
          medianPosition.SetVec2(vec2_2);
        }
        else
          medianPosition.SetVec2(this.formation.QuerySystem.AveragePosition);
      }
      this.CurrentOrder = MovementOrder.MovementOrderMove(medianPosition);
    }

    protected internal override void TickOccasionally()
    {
      this.CalculateCurrentOrder();
      this.formation.MovementOrder = this.CurrentOrder;
      this.formation.FacingOrder = this.CurrentFacingOrder;
      if ((double) this.formation.QuerySystem.AveragePosition.DistanceSquared(this.CurrentOrder.GetPosition(this.formation).AsVec2) < 100.0)
        this.formation.ArrangementOrder = ArrangementOrder.ArrangementOrderSquare;
      WorldPosition position = this.CurrentOrder.GetPosition(this.formation);
      bool flag = this.formation.QuerySystem.ClosestEnemyFormation == null || (double) this._mainFormation.QuerySystem.AveragePosition.DistanceSquared(this.formation.QuerySystem.AveragePosition) <= (double) this.formation.Depth * (double) this.formation.Width || (double) this.formation.QuerySystem.AveragePosition.DistanceSquared(position.AsVec2) <= ((double) this._mainFormation.Depth + (double) this.formation.Depth) * ((double) this._mainFormation.Depth + (double) this.formation.Depth) * 0.25;
      if (flag == this._isFireAtWill)
        return;
      this._isFireAtWill = flag;
      this.formation.FiringOrder = this._isFireAtWill ? FiringOrder.FiringOrderFireAtWill : FiringOrder.FiringOrderHoldYourFire;
    }

    protected override void OnBehaviorActivatedAux()
    {
      this.CalculateCurrentOrder();
      this.formation.MovementOrder = this.CurrentOrder;
      this.formation.FacingOrder = this.CurrentFacingOrder;
      this.formation.ArrangementOrder = ArrangementOrder.ArrangementOrderLine;
      this.formation.FiringOrder = FiringOrder.FiringOrderFireAtWill;
      int num = (int) Math.Sqrt((double) this.formation.CountOfUnits);
      this.formation.FormOrder = FormOrder.FormOrderCustom((float) ((double) num * (double) this.formation.UnitDiameter + (double) (num - 1) * (double) this.formation.Interval));
      this.formation.WeaponUsageOrder = WeaponUsageOrder.WeaponUsageOrderUseAny;
    }

    protected override float GetAiWeight()
    {
      if (this._mainFormation == null || !this._mainFormation.AI.IsMainFormation)
        this._mainFormation = this.formation.Team.Formations.FirstOrDefault<Formation>((Func<Formation, bool>) (f => f.AI.IsMainFormation));
      return this._mainFormation == null || this.formation.AI.IsMainFormation || (this.formation.QuerySystem.ClosestEnemyFormation == null || !this.formation.QuerySystem.IsRangedFormation) ? 0.0f : 2f;
    }
  }
}
