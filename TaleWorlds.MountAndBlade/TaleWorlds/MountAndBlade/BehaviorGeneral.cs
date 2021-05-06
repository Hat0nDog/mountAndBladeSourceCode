// Decompiled with JetBrains decompiler
// Type: TaleWorlds.MountAndBlade.BehaviorGeneral
// Assembly: TaleWorlds.MountAndBlade, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3C0E3F42-756F-4407-B817-06BA412A78B0
// Assembly location: E:\SteamLibrary\steamapps\common\Mount & Blade II Bannerlord\bin\Win64_Shipping_Client\TaleWorlds.MountAndBlade.dll

using System;
using System.Collections.Generic;
using System.Linq;
using TaleWorlds.Engine;

namespace TaleWorlds.MountAndBlade
{
  public class BehaviorGeneral : BehaviorComponent
  {
    private Formation _mainFormation;

    public BehaviorGeneral(Formation formation)
      : base(formation)
    {
      this._mainFormation = formation.Team.Formations.FirstOrDefault<Formation>((Func<Formation, bool>) (f => f.AI.IsMainFormation));
      this.CalculateCurrentOrder();
    }

    protected override void CalculateCurrentOrder()
    {
      WorldPosition medianPosition;
      if (this.formation.Team.QuerySystem.EnemyTeams.SelectMany<TeamQuerySystem, Formation>((Func<TeamQuerySystem, IEnumerable<Formation>>) (t => t.Team.FormationsIncludingSpecial)).Any<Formation>() && this.formation.Team.FormationsIncludingSpecial.Any<Formation>())
      {
        float num1 = !this.formation.IsMounted() || (double) this.formation.Team.QuerySystem.CavalryRatio + (double) this.formation.Team.QuerySystem.RangedCavalryRatio < 33.2999992370605 ? 3f : 40f;
        if (this._mainFormation != null && this._mainFormation.CountOfUnits > 0)
        {
          float num2 = this._mainFormation.Depth + num1;
          medianPosition = this._mainFormation.QuerySystem.MedianPosition;
          medianPosition.SetVec2(medianPosition.AsVec2 - (this.formation.QuerySystem.Team.MedianTargetFormationPosition.AsVec2 - this._mainFormation.QuerySystem.MedianPosition.AsVec2).Normalized() * num2);
        }
        else
        {
          medianPosition = this.formation.QuerySystem.Team.MedianPosition;
          medianPosition.SetVec2(this.formation.QuerySystem.Team.AveragePosition - (this.formation.QuerySystem.Team.MedianTargetFormationPosition.AsVec2 - this.formation.QuerySystem.Team.AveragePosition).Normalized() * num1);
        }
      }
      else
      {
        medianPosition = this.formation.QuerySystem.MedianPosition;
        medianPosition.SetVec2(this.formation.QuerySystem.AveragePosition);
      }
      this.CurrentOrder = MovementOrder.MovementOrderMove(medianPosition);
    }

    protected internal override void TickOccasionally()
    {
      this.CalculateCurrentOrder();
      this.formation.MovementOrder = this.CurrentOrder;
    }

    protected override void OnBehaviorActivatedAux()
    {
      this.CalculateCurrentOrder();
      this.formation.MovementOrder = this.CurrentOrder;
      this.formation.ArrangementOrder = ArrangementOrder.ArrangementOrderLine;
      this.formation.FacingOrder = FacingOrder.FacingOrderLookAtEnemy;
      this.formation.FiringOrder = FiringOrder.FiringOrderFireAtWill;
      this.formation.FormOrder = FormOrder.FormOrderDeep;
      this.formation.WeaponUsageOrder = WeaponUsageOrder.WeaponUsageOrderUseAny;
    }

    protected override float GetAiWeight()
    {
      if (this._mainFormation == null || !this._mainFormation.AI.IsMainFormation)
        this._mainFormation = this.formation.Team.Formations.FirstOrDefault<Formation>((Func<Formation, bool>) (f => f.AI.IsMainFormation));
      return 1.2f;
    }
  }
}
