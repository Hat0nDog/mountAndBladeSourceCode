// Decompiled with JetBrains decompiler
// Type: TaleWorlds.MountAndBlade.BehaviorCharge
// Assembly: TaleWorlds.MountAndBlade, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3C0E3F42-756F-4407-B817-06BA412A78B0
// Assembly location: E:\SteamLibrary\steamapps\common\Mount & Blade II Bannerlord\bin\Win64_Shipping_Client\TaleWorlds.MountAndBlade.dll

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using TaleWorlds.Engine;
using TaleWorlds.Library;

namespace TaleWorlds.MountAndBlade
{
  public class BehaviorCharge : BehaviorComponent
  {
    public BehaviorCharge(Formation formation)
      : base(formation)
    {
      this.CalculateCurrentOrder();
      this.BehaviorCoherence = 0.5f;
    }

    protected override void CalculateCurrentOrder()
    {
      if (this.formation.QuerySystem.ClosestEnemyFormation == null)
        this.CurrentOrder = MovementOrder.MovementOrderCharge;
      else
        this.CurrentOrder = MovementOrder.MovementOrderChargeToTarget(this.formation.QuerySystem.ClosestEnemyFormation.Formation);
    }

    protected internal override void TickOccasionally()
    {
      base.TickOccasionally();
      this.CalculateCurrentOrder();
      this.formation.MovementOrder = this.CurrentOrder;
    }

    protected internal override float NavmeshlessTargetPositionPenalty => 1f;

    protected override void OnBehaviorActivatedAux()
    {
      this.formation.ArrangementOrder = ArrangementOrder.ArrangementOrderLoose;
      this.formation.FacingOrder = FacingOrder.FacingOrderLookAtEnemy;
      this.formation.FiringOrder = FiringOrder.FiringOrderFireAtWill;
      this.formation.FormOrder = FormOrder.FormOrderWide;
      this.formation.WeaponUsageOrder = WeaponUsageOrder.WeaponUsageOrderUseAny;
    }

    private float CalculateAIWeight(bool isSiege, bool isInsideCastle)
    {
      FormationQuerySystem formationQuerySystem = this.formation.QuerySystem;
      float num1 = formationQuerySystem.AveragePosition.Distance(formationQuerySystem.ClosestEnemyFormation.MedianPosition.AsVec2) / formationQuerySystem.MovementSpeedMaximum;
      float num2 = formationQuerySystem.IsCavalryFormation || formationQuerySystem.IsRangedCavalryFormation ? ((double) num1 > 4.0 ? MBMath.Lerp(0.1f, 1.4f, (float) (1.0 - ((double) MBMath.ClampFloat(num1, 4f, 10f) - 4.0) / 6.0)) : MBMath.Lerp(0.1f, 1.4f, MBMath.ClampFloat(num1, 0.0f, 4f) / 4f)) : MBMath.Lerp(0.1f, 1f, (float) (1.0 - ((double) MBMath.ClampFloat(num1, 4f, 10f) - 4.0) / 6.0));
      float num3 = 0.0f;
      foreach (Team team in (ReadOnlyCollection<Team>) Mission.Current.Teams)
      {
        if (team.IsEnemyOf(this.formation.Team))
        {
          foreach (Formation formation in team.FormationsIncludingSpecialAndEmpty)
          {
            Formation enemyFormation = formation;
            if (enemyFormation.CountOfUnits > 0 && formationQuerySystem.ClosestEnemyFormation.Formation != enemyFormation && (!isSiege || TeamAISiegeComponent.IsFormationInsideCastle(enemyFormation, true) == isInsideCastle))
            {
              WorldPosition medianPosition = enemyFormation.QuerySystem.MedianPosition;
              Vec2 asVec2_1 = medianPosition.AsVec2;
              ref Vec2 local = ref asVec2_1;
              medianPosition = formationQuerySystem.ClosestEnemyFormation.MedianPosition;
              Vec2 asVec2_2 = medianPosition.AsVec2;
              float reliefTime = local.Distance(asVec2_2) / enemyFormation.QuerySystem.MovementSpeedMaximum;
              if ((double) reliefTime <= (double) num1 + 4.0 && ((double) num1 > 8.0 || enemyFormation.QuerySystem.ClosestEnemyFormation == this.formation.QuerySystem) && ((double) num1 > 8.0 || !this.formation.QuerySystem.Team.AllyTeams.SelectMany<TeamQuerySystem, Formation>((Func<TeamQuerySystem, IEnumerable<Formation>>) (t => t.Team.FormationsIncludingSpecial)).Any<Formation>((Func<Formation, bool>) (f => f.QuerySystem.ClosestEnemyFormation == enemyFormation.QuerySystem && (double) f.QuerySystem.MedianPosition.AsVec2.Distance(formationQuerySystem.AveragePosition) / (double) f.QuerySystem.MovementSpeedMaximum < (double) reliefTime + 4.0))))
                num3 += enemyFormation.QuerySystem.FormationMeleeFightingPower * enemyFormation.QuerySystem.GetClassWeightedFactor(1f, 1f, 1f, 1f);
            }
          }
        }
      }
      float num4 = 0.0f;
      foreach (Team team in (ReadOnlyCollection<Team>) Mission.Current.Teams)
      {
        if (team.IsFriendOf(this.formation.Team))
        {
          foreach (Formation formation in team.FormationsIncludingSpecialAndEmpty)
          {
            if (formation != this.formation && formation.CountOfUnits > 0 && formationQuerySystem.ClosestEnemyFormation == formation.QuerySystem.ClosestEnemyFormation && (!isSiege || TeamAISiegeComponent.IsFormationInsideCastle(formation, true) == isInsideCastle) && (double) formation.QuerySystem.MedianPosition.AsVec2.Distance(formation.QuerySystem.ClosestEnemyFormation.MedianPosition.AsVec2) / (double) formation.QuerySystem.MovementSpeedMaximum < 4.0)
              num4 += formation.QuerySystem.FormationMeleeFightingPower * formation.QuerySystem.GetClassWeightedFactor(1f, 1f, 1f, 1f);
          }
        }
      }
      float num5 = (float) (((double) this.formation.QuerySystem.FormationMeleeFightingPower * (double) formationQuerySystem.GetClassWeightedFactor(1f, 1f, 1f, 1f) + (double) num4 + 1.0) / (1.0 + (double) num3 + (double) formationQuerySystem.ClosestEnemyFormation.Formation.QuerySystem.FormationMeleeFightingPower * (double) formationQuerySystem.ClosestEnemyFormation.GetClassWeightedFactor(1f, 1f, 1f, 1f)) / (!isSiege ? (double) MBMath.ClampFloat(formationQuerySystem.Team.OverallPowerRatio, 0.2f, 3f) : (double) MBMath.ClampFloat(formationQuerySystem.Team.OverallPowerRatio, 0.5f, 3f)));
      if ((double) num5 > 1.0)
        num5 = (float) (((double) num5 - 1.0) / 3.0) + 1f;
      float num6 = MBMath.ClampFloat(num5, 0.1f, 1.25f);
      float num7 = 1f;
      if ((double) num1 <= 4.0)
      {
        float length = (formationQuerySystem.AveragePosition - formationQuerySystem.ClosestEnemyFormation.MedianPosition.AsVec2).Length;
        if ((double) length != 0.0)
        {
          WorldPosition medianPosition = formationQuerySystem.MedianPosition;
          medianPosition.SetVec2(formationQuerySystem.AveragePosition);
          float navMeshZ = medianPosition.GetNavMeshZ();
          if (!float.IsNaN(navMeshZ))
            num7 = MBMath.Lerp(0.9f, 1.1f, (float) (((double) MBMath.ClampFloat((navMeshZ - formationQuerySystem.ClosestEnemyFormation.MedianPosition.GetNavMeshZ()) / length, -0.58f, 0.58f) + 0.579999983310699) / 1.1599999666214));
        }
      }
      float num8 = 1f;
      if ((double) num1 <= 4.0 && (double) num1 >= 1.5)
        num8 = 1.2f;
      float num9 = 1f;
      if ((double) num1 <= 4.0 && formationQuerySystem.ClosestEnemyFormation.ClosestEnemyFormation != formationQuerySystem)
        num9 = 1.2f;
      float num10 = !isSiege ? formationQuerySystem.GetClassWeightedFactor(1f, 1f, 1.5f, 1.5f) * formationQuerySystem.ClosestEnemyFormation.GetClassWeightedFactor(1f, 1f, 0.5f, 0.5f) : formationQuerySystem.GetClassWeightedFactor(1f, 1f, 1.2f, 1.2f) * formationQuerySystem.ClosestEnemyFormation.GetClassWeightedFactor(1f, 1f, 0.3f, 0.3f);
      return num2 * num6 * num7 * num8 * num9 * num10;
    }

    protected override float GetAiWeight()
    {
      bool isSiege = this.formation.Team.TeamAI is TeamAISiegeComponent;
      float num = 0.0f;
      if (this.formation.QuerySystem.ClosestEnemyFormation == null)
      {
        if (this.formation.Team.HasAnyEnemyTeamsWithAgents(false))
          num = 0.2f;
      }
      else
      {
        bool isInsideCastle = false;
        bool flag;
        if (!isSiege)
          flag = true;
        else if ((this.formation.Team.TeamAI as TeamAISiegeComponent).CalculateIsChargePastWallsApplicable(this.formation.AI.Side))
        {
          flag = true;
        }
        else
        {
          isInsideCastle = TeamAISiegeComponent.IsFormationInsideCastle(this.formation.QuerySystem.ClosestEnemyFormation.Formation, true, 0.51f);
          flag = isInsideCastle == TeamAISiegeComponent.IsFormationInsideCastle(this.formation, true, isInsideCastle ? 0.9f : 0.1f);
        }
        if (flag)
          num = this.CalculateAIWeight(isSiege, isInsideCastle);
      }
      return num;
    }
  }
}
