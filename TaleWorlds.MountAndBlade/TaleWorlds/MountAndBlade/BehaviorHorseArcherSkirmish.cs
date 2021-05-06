// Decompiled with JetBrains decompiler
// Type: TaleWorlds.MountAndBlade.BehaviorHorseArcherSkirmish
// Assembly: TaleWorlds.MountAndBlade, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3C0E3F42-756F-4407-B817-06BA412A78B0
// Assembly location: E:\SteamLibrary\steamapps\common\Mount & Blade II Bannerlord\bin\Win64_Shipping_Client\TaleWorlds.MountAndBlade.dll

using System;
using System.Collections.Generic;
using System.Linq;
using TaleWorlds.Engine;
using TaleWorlds.Library;

namespace TaleWorlds.MountAndBlade
{
  public class BehaviorHorseArcherSkirmish : BehaviorComponent
  {
    private bool _rushMode;

    public BehaviorHorseArcherSkirmish(Formation formation)
      : base(formation)
    {
      this.CalculateCurrentOrder();
      this.BehaviorCoherence = 0.5f;
    }

    protected override float GetAiWeight() => 0.9f;

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

    protected override void CalculateCurrentOrder()
    {
      Vec2 averagePosition = this.formation.QuerySystem.AveragePosition;
      if (this.formation.QuerySystem.ClosestSignificantlyLargeEnemyFormation == null)
      {
        WorldPosition medianPosition = this.formation.QuerySystem.MedianPosition;
        medianPosition.SetVec2(averagePosition);
        this.CurrentOrder = MovementOrder.MovementOrderMove(medianPosition);
      }
      else
      {
        WorldPosition medianPosition = this.formation.QuerySystem.ClosestSignificantlyLargeEnemyFormation.MedianPosition;
        IEnumerable<Formation> source = this.formation.Team.FormationsIncludingSpecial.Where<Formation>((Func<Formation, bool>) (f => f != this.formation));
        Vec2 vec2_1 = source.Any<Formation>() ? new Vec2(source.Average<Formation>((Func<Formation, float>) (oa => oa.QuerySystem.MedianPosition.AsVec2.x)), source.Average<Formation>((Func<Formation, float>) (oa => oa.QuerySystem.MedianPosition.AsVec2.y))) : averagePosition;
        WorldPosition formationPosition = this.formation.QuerySystem.Team.MedianTargetFormationPosition;
        Vec2 vec2_2 = (formationPosition.AsVec2 - vec2_1).Normalized();
        float missileRange = this.formation.QuerySystem.MissileRange;
        WorldPosition position;
        if (this._rushMode)
        {
          float num = averagePosition.DistanceSquared(medianPosition.AsVec2);
          if ((double) num > (double) this.formation.QuerySystem.MissileRange * (double) this.formation.QuerySystem.MissileRange)
          {
            position = formationPosition;
            position.SetVec2(position.AsVec2 - vec2_2 * (missileRange - (float) (10.0 + (double) this.formation.Depth * 0.5)));
          }
          else if (this.formation.QuerySystem.ClosestEnemyFormation.IsCavalryFormation || (double) num <= 400.0 || (double) this.formation.QuerySystem.UnderRangedAttackRatio >= 0.400000005960464)
          {
            position = this.formation.QuerySystem.Team.MedianPosition;
            position.SetVec2(vec2_1 - ((source.Any<Formation>() ? 30f : 80f) + this.formation.Depth) * vec2_2);
            this._rushMode = false;
          }
          else
          {
            position = this.formation.QuerySystem.Team.MedianPosition;
            Vec2 vec2_3 = (medianPosition.AsVec2 - averagePosition).Normalized();
            position.SetVec2(medianPosition.AsVec2 - vec2_3 * (missileRange - (float) (10.0 + (double) this.formation.Depth * 0.5)));
          }
        }
        else
        {
          if (source.Any<Formation>())
          {
            position = this.formation.QuerySystem.Team.MedianPosition;
            position.SetVec2(vec2_1 - (30f + this.formation.Depth) * vec2_2);
          }
          else
          {
            position = this.formation.QuerySystem.ClosestEnemyFormation.MedianPosition;
            position.SetVec2(position.AsVec2 - 80f * vec2_2);
          }
          if ((double) position.AsVec2.DistanceSquared(averagePosition) <= 400.0)
          {
            position = formationPosition;
            position.SetVec2(position.AsVec2 - vec2_2 * (missileRange - (float) (10.0 + (double) this.formation.Depth * 0.5)));
            this._rushMode = true;
          }
        }
        this.CurrentOrder = MovementOrder.MovementOrderMove(position);
      }
    }

    protected internal override void TickOccasionally()
    {
      this.CalculateCurrentOrder();
      this.formation.MovementOrder = this.CurrentOrder;
    }
  }
}
