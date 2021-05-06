// Decompiled with JetBrains decompiler
// Type: TaleWorlds.MountAndBlade.BehaviorReserve
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
  public class BehaviorReserve : BehaviorComponent
  {
    public BehaviorReserve(Formation formation)
      : base(formation)
    {
      this.behaviorSide = formation.AI.Side;
      this.CalculateCurrentOrder();
    }

    protected override void CalculateCurrentOrder()
    {
      Formation formation = this.formation.Team.Formations.FirstOrDefault<Formation>((Func<Formation, bool>) (f => f != this.formation && f.AI.IsMainFormation));
      WorldPosition medianPosition;
      if (formation != null)
      {
        medianPosition = formation.QuerySystem.MedianPosition;
        Vec2 vec2 = (this.formation.QuerySystem.Team.AverageEnemyPosition - formation.QuerySystem.MedianPosition.AsVec2).Normalized();
        medianPosition.SetVec2(medianPosition.AsVec2 - vec2 * (40f + this.formation.Depth));
      }
      else
      {
        IEnumerable<Formation> source = this.formation.Team.FormationsIncludingSpecial.Where<Formation>((Func<Formation, bool>) (f => f != this.formation));
        if (source.IsEmpty<Formation>())
        {
          this.CurrentOrder = MovementOrder.MovementOrderStop;
          return;
        }
        Vec2 vec2_1 = new Vec2(source.Average<Formation>((Func<Formation, float>) (oa => oa.QuerySystem.AveragePosition.x)), source.Average<Formation>((Func<Formation, float>) (oa => oa.QuerySystem.AveragePosition.y)));
        Vec2 vec2_2 = (this.formation.QuerySystem.Team.AverageEnemyPosition - vec2_1).Normalized();
        medianPosition = this.formation.QuerySystem.MedianPosition;
        medianPosition.SetVec2(vec2_1 - vec2_2 * (20f + this.formation.Depth));
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
      this.formation.FormOrder = FormOrder.FormOrderWider;
      this.formation.WeaponUsageOrder = WeaponUsageOrder.WeaponUsageOrderUseAny;
    }

    protected override float GetAiWeight()
    {
      if (!this.formation.AI.IsMainFormation)
      {
        foreach (Formation formation1 in this.formation.Team.FormationsIncludingSpecialAndEmpty)
        {
          if (this.formation != formation1 && formation1.CountOfUnits > 0)
          {
            using (IEnumerator<Team> enumerator = Mission.Current.Teams.GetEnumerator())
            {
              while (enumerator.MoveNext())
              {
                Team current = enumerator.Current;
                if (current.IsEnemyOf(this.formation.Team))
                {
                  foreach (Formation formation2 in current.FormationsIncludingSpecialAndEmpty)
                  {
                    if (formation2.CountOfUnits > 0)
                      return 0.04f;
                  }
                }
              }
              break;
            }
          }
        }
      }
      return 0.0f;
    }
  }
}
