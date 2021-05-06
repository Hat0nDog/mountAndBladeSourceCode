// Decompiled with JetBrains decompiler
// Type: TaleWorlds.MountAndBlade.BehaviorPullBack
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
  public class BehaviorPullBack : BehaviorComponent
  {
    public BehaviorPullBack(Formation formation)
      : base(formation)
    {
      this.CalculateCurrentOrder();
      this.BehaviorCoherence = 0.2f;
    }

    protected override void CalculateCurrentOrder()
    {
      WorldPosition medianPosition = this.formation.QuerySystem.MedianPosition;
      if (this.formation.QuerySystem.ClosestEnemyFormation == null)
      {
        medianPosition.SetVec2(this.formation.QuerySystem.AveragePosition);
        this.CurrentOrder = MovementOrder.MovementOrderMove(medianPosition);
      }
      else
      {
        Vec2 vec2 = (this.formation.QuerySystem.AveragePosition - this.formation.QuerySystem.ClosestEnemyFormation.MedianPosition.AsVec2).Normalized();
        medianPosition.SetVec2(this.formation.QuerySystem.AveragePosition + 50f * vec2);
        this.CurrentOrder = MovementOrder.MovementOrderMove(medianPosition);
      }
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
      this.formation.ArrangementOrder = ArrangementOrder.ArrangementOrderLoose;
      this.formation.FacingOrder = FacingOrder.FacingOrderLookAtEnemy;
      this.formation.FiringOrder = FiringOrder.FiringOrderFireAtWill;
      this.formation.FormOrder = FormOrder.FormOrderWide;
      this.formation.WeaponUsageOrder = WeaponUsageOrder.WeaponUsageOrderUseAny;
    }

    protected override float GetAiWeight()
    {
      if (this.formation.Team.TeamAI is TeamAISiegeComponent)
        return this.GetSiegeAIWeight();
      FormationQuerySystem q = this.formation.QuerySystem;
      if (q.ClosestEnemyFormation == null || q.ClosestEnemyFormation.ClosestEnemyFormation != q || (double) q.ClosestEnemyFormation.MovementSpeedMaximum - (double) q.MovementSpeedMaximum > 2.0)
        return 0.0f;
      Vec2 vec2 = q.AveragePosition;
      ref Vec2 local1 = ref vec2;
      WorldPosition medianPosition = q.ClosestEnemyFormation.MedianPosition;
      Vec2 asVec2_1 = medianPosition.AsVec2;
      float num1 = local1.Distance(asVec2_1) / q.ClosestEnemyFormation.MovementSpeedMaximum;
      float num2 = MBMath.Lerp(0.1f, 1f, (float) (1.0 - ((double) MBMath.ClampFloat(num1, 4f, 10f) - 4.0) / 6.0));
      float num3 = 0.0f;
      foreach (Formation formation in Mission.Current.Teams.GetEnemiesOf(this.formation.Team).SelectMany<Team, Formation>((Func<Team, IEnumerable<Formation>>) (t => t.FormationsIncludingSpecial)).Where<Formation>((Func<Formation, bool>) (f => f != q.ClosestEnemyFormation.Formation)))
      {
        Formation otherEnemyFormation = formation;
        medianPosition = otherEnemyFormation.QuerySystem.MedianPosition;
        vec2 = medianPosition.AsVec2;
        ref Vec2 local2 = ref vec2;
        medianPosition = q.ClosestEnemyFormation.MedianPosition;
        Vec2 asVec2_2 = medianPosition.AsVec2;
        float reliefTime = local2.Distance(asVec2_2) / otherEnemyFormation.QuerySystem.MovementSpeedMaximum;
        if ((double) reliefTime <= (double) num1 + 4.0 && ((double) num1 > 8.0 || otherEnemyFormation.QuerySystem.ClosestEnemyFormation == this.formation.QuerySystem) && ((double) num1 > 8.0 || !this.formation.QuerySystem.Team.AllyTeams.SelectMany<TeamQuerySystem, Formation>((Func<TeamQuerySystem, IEnumerable<Formation>>) (t => t.Team.FormationsIncludingSpecial)).Any<Formation>((Func<Formation, bool>) (f => f.QuerySystem.ClosestEnemyFormation == otherEnemyFormation.QuerySystem && (double) f.QuerySystem.MedianPosition.AsVec2.Distance(q.AveragePosition) / (double) f.QuerySystem.MovementSpeedMaximum < (double) reliefTime + 4.0))))
          num3 += otherEnemyFormation.QuerySystem.FormationMeleeFightingPower * otherEnemyFormation.QuerySystem.GetClassWeightedFactor(1f, 1f, 1f, 1f);
      }
      float num4 = 0.0f;
      foreach (Formation formation in Mission.Current.Teams.GetAlliesOf(this.formation.Team).SelectMany<Team, Formation>((Func<Team, IEnumerable<Formation>>) (t => t.FormationsIncludingSpecial)).Where<Formation>((Func<Formation, bool>) (f => f != this.formation && f.QuerySystem.ClosestEnemyFormation == q.ClosestEnemyFormation)))
      {
        if ((double) formation.QuerySystem.MedianPosition.AsVec2.Distance(formation.QuerySystem.ClosestEnemyFormation.MedianPosition.AsVec2) / (double) formation.QuerySystem.MovementSpeedMaximum < 4.0)
          num4 += formation.QuerySystem.FormationMeleeFightingPower * formation.QuerySystem.GetClassWeightedFactor(1f, 1f, 1f, 1f);
      }
      return MBMath.ClampFloat((float) ((1.0 + (double) num3 + (double) q.ClosestEnemyFormation.Formation.QuerySystem.FormationMeleeFightingPower * (double) q.ClosestEnemyFormation.GetClassWeightedFactor(1f, 1f, 1f, 1f)) / ((double) this.formation.GetFormationMeleeFightingPower() * (double) q.GetClassWeightedFactor(1f, 1f, 1f, 1f) + (double) num4 + 1.0) * (double) q.Team.OverallPowerRatio / 3.0), 0.1f, 1.2f) * num2;
    }

    private float GetSiegeAIWeight() => 0.0f;
  }
}
