// Decompiled with JetBrains decompiler
// Type: TaleWorlds.MountAndBlade.TacticPerimeterDefense
// Assembly: TaleWorlds.MountAndBlade, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3C0E3F42-756F-4407-B817-06BA412A78B0
// Assembly location: E:\SteamLibrary\steamapps\common\Mount & Blade II Bannerlord\bin\Win64_Shipping_Client\TaleWorlds.MountAndBlade.dll

using System;
using System.Collections.Generic;
using System.Linq;
using TaleWorlds.Core;
using TaleWorlds.Engine;
using TaleWorlds.Library;

namespace TaleWorlds.MountAndBlade
{
  public class TacticPerimeterDefense : TacticComponent
  {
    private WorldPosition _defendPosition;
    private readonly List<TacticPerimeterDefense.EnemyCluster> _enemyClusters;
    private readonly List<TacticPerimeterDefense.DefenseFront> _defenseFronts;
    private const float RetreatThresholdValue = 2f;
    private List<Formation> _meleeFormations;
    private List<Formation> _rangedFormations;
    private bool _isRetreatingToKeep;

    public TacticPerimeterDefense(Team team)
      : base(team)
    {
      Scene scene = Mission.Current.Scene;
      FleePosition fleePosition = Mission.Current.GetFleePositionsForSide(BattleSideEnum.Defender).FirstOrDefault<FleePosition>((Func<FleePosition, bool>) (fp => fp.GetSide() == BattleSideEnum.Defender));
      this._defendPosition = fleePosition == null ? WorldPosition.Invalid : fleePosition.GameEntity.GlobalPosition.ToWorldPosition();
      this._enemyClusters = new List<TacticPerimeterDefense.EnemyCluster>();
      this._defenseFronts = new List<TacticPerimeterDefense.DefenseFront>();
    }

    private void DetermineEnemyClusters()
    {
      List<Formation> list1 = this.team.QuerySystem.EnemyTeams.SelectMany<TeamQuerySystem, Formation>((Func<TeamQuerySystem, IEnumerable<Formation>>) (et => et.Team.Formations)).ToList<Formation>();
      List<Formation> formationList = new List<Formation>();
      foreach (Formation formation in list1)
      {
        Formation enemyFormation = formation;
        if ((double) enemyFormation.QuerySystem.FormationPower < (double) Math.Min(this.team.QuerySystem.TeamPower, this.team.QuerySystem.EnemyTeams.Sum<TeamQuerySystem>((Func<TeamQuerySystem, float>) (et => et.TeamPower))) / 4.0)
        {
          if (!this._enemyClusters.Any<TacticPerimeterDefense.EnemyCluster>((Func<TacticPerimeterDefense.EnemyCluster, bool>) (ec => ec.enemyFormations.Contains(enemyFormation))))
            formationList.Add(enemyFormation);
        }
        else
        {
          TacticPerimeterDefense.EnemyCluster enemyCluster1 = this._enemyClusters.FirstOrDefault<TacticPerimeterDefense.EnemyCluster>((Func<TacticPerimeterDefense.EnemyCluster, bool>) (ec => ec.enemyFormations.Any<Formation>((Func<Formation, bool>) (ef => ef == enemyFormation))));
          if (enemyCluster1 != null)
          {
            if ((double) (this._defendPosition.AsVec2 - enemyCluster1.AggregatePosition).DotProduct(this._defendPosition.AsVec2 - enemyFormation.QuerySystem.AveragePosition) < 0.70710678118)
              enemyCluster1.RemoveFromCluster(enemyFormation);
            else
              continue;
          }
          List<TacticPerimeterDefense.EnemyCluster> list2 = this._enemyClusters.Where<TacticPerimeterDefense.EnemyCluster>((Func<TacticPerimeterDefense.EnemyCluster, bool>) (c => (double) (this._defendPosition.AsVec2 - c.AggregatePosition).DotProduct(this._defendPosition.AsVec2 - enemyFormation.QuerySystem.MedianPosition.AsVec2) >= 0.70710678118)).ToList<TacticPerimeterDefense.EnemyCluster>();
          if (list2.Any<TacticPerimeterDefense.EnemyCluster>())
          {
            list2.MaxBy<TacticPerimeterDefense.EnemyCluster, float>((Func<TacticPerimeterDefense.EnemyCluster, float>) (ec => (this._defendPosition.AsVec2 - ec.AggregatePosition).DotProduct(this._defendPosition.AsVec2 - enemyFormation.QuerySystem.MedianPosition.AsVec2))).AddToCluster(enemyFormation);
          }
          else
          {
            TacticPerimeterDefense.EnemyCluster enemyCluster2 = new TacticPerimeterDefense.EnemyCluster();
            enemyCluster2.AddToCluster(enemyFormation);
            this._enemyClusters.Add(enemyCluster2);
          }
        }
      }
      if (!this._enemyClusters.Any<TacticPerimeterDefense.EnemyCluster>())
        return;
      foreach (Formation formation in formationList)
      {
        Formation skippedFormation = formation;
        this._enemyClusters.MaxBy<TacticPerimeterDefense.EnemyCluster, float>((Func<TacticPerimeterDefense.EnemyCluster, float>) (ec => (this._defendPosition.AsVec2 - ec.AggregatePosition).DotProduct(this._defendPosition.AsVec2 - skippedFormation.QuerySystem.MedianPosition.AsVec2))).AddToCluster(skippedFormation);
      }
    }

    private bool MustRetreatToCastle() => (double) this.team.QuerySystem.PowerRatioIncludingCasualties / (double) this.team.QuerySystem.OverallPowerRatio > 2.0;

    private void StartRetreatToKeep()
    {
      foreach (Formation formation in this.Formations)
      {
        formation.AI.ResetBehaviorWeights();
        TacticComponent.SetDefaultBehaviorWeights(formation);
        formation.AI.SetBehaviorWeight<BehaviorRetreatToKeep>(1f);
      }
    }

    private void CheckAndChangeState()
    {
      this.Formations.Where<Formation>((Func<Formation, bool>) (f => TeamAISiegeComponent.IsFormationInsideCastle(f, true)));
      if (!this.MustRetreatToCastle() || this._isRetreatingToKeep)
        return;
      this._isRetreatingToKeep = true;
      this.StartRetreatToKeep();
    }

    private void ArrangeDefenseFronts()
    {
      this._meleeFormations = this.Formations.Where<Formation>((Func<Formation, bool>) (f => f.QuerySystem.IsInfantryFormation || f.QuerySystem.IsCavalryFormation)).ToList<Formation>();
      this._rangedFormations = this.Formations.Where<Formation>((Func<Formation, bool>) (f => f.QuerySystem.IsRangedFormation || f.QuerySystem.IsRangedCavalryFormation)).ToList<Formation>();
      int count1 = Math.Min(8 - this._rangedFormations.Count, this._enemyClusters.Count);
      if (this._meleeFormations.Count != count1)
      {
        this.SplitFormationClassIntoGivenNumber((Func<Formation, bool>) (f => f.QuerySystem.IsInfantryFormation || f.QuerySystem.IsCavalryFormation), count1);
        this._meleeFormations = this.Formations.Where<Formation>((Func<Formation, bool>) (f => f.QuerySystem.IsInfantryFormation || f.QuerySystem.IsCavalryFormation)).ToList<Formation>();
      }
      int count2 = Math.Min(8 - count1, this._enemyClusters.Count);
      if (this._rangedFormations.Count != count2)
      {
        this.SplitFormationClassIntoGivenNumber((Func<Formation, bool>) (f => f.QuerySystem.IsRangedFormation || f.QuerySystem.IsRangedCavalryFormation), count2);
        this._rangedFormations = this.Formations.Where<Formation>((Func<Formation, bool>) (f => f.QuerySystem.IsRangedFormation || f.QuerySystem.IsRangedCavalryFormation)).ToList<Formation>();
      }
      foreach (TacticPerimeterDefense.DefenseFront defenseFront in this._defenseFronts)
      {
        defenseFront.MatchedEnemyCluster.UpdateClusterData();
        BehaviorDefendKeyPosition defendKeyPosition = defenseFront.MeleeFormation.AI.SetBehaviorWeight<BehaviorDefendKeyPosition>(1f);
        defendKeyPosition.EnemyClusterPosition = defenseFront.MatchedEnemyCluster.MedianAggregatePosition;
        defendKeyPosition.EnemyClusterPosition.SetVec2(defenseFront.MatchedEnemyCluster.AggregatePosition);
      }
      IEnumerable<TacticPerimeterDefense.EnemyCluster> enemyClusters = this._enemyClusters.Where<TacticPerimeterDefense.EnemyCluster>((Func<TacticPerimeterDefense.EnemyCluster, bool>) (ec => this._defenseFronts.All<TacticPerimeterDefense.DefenseFront>((Func<TacticPerimeterDefense.DefenseFront, bool>) (df => df.MatchedEnemyCluster != ec))));
      List<Formation> list1 = this._meleeFormations.Where<Formation>((Func<Formation, bool>) (mf => this._defenseFronts.All<TacticPerimeterDefense.DefenseFront>((Func<TacticPerimeterDefense.DefenseFront, bool>) (df => df.MeleeFormation != mf)))).ToList<Formation>();
      List<Formation> list2 = this._rangedFormations.Where<Formation>((Func<Formation, bool>) (rf => this._defenseFronts.All<TacticPerimeterDefense.DefenseFront>((Func<TacticPerimeterDefense.DefenseFront, bool>) (df => df.RangedFormation != rf)))).ToList<Formation>();
      foreach (TacticPerimeterDefense.EnemyCluster matchedEnemyCluster in enemyClusters)
      {
        if (list1.IsEmpty<Formation>())
          break;
        Formation formation = list1.Last<Formation>();
        TacticPerimeterDefense.DefenseFront defenseFront = new TacticPerimeterDefense.DefenseFront(matchedEnemyCluster, formation);
        formation.AI.ResetBehaviorWeights();
        TacticComponent.SetDefaultBehaviorWeights(formation);
        BehaviorDefendKeyPosition defendKeyPosition = formation.AI.SetBehaviorWeight<BehaviorDefendKeyPosition>(1f);
        defendKeyPosition.DefensePosition = this._defendPosition;
        defendKeyPosition.EnemyClusterPosition = matchedEnemyCluster.MedianAggregatePosition;
        defendKeyPosition.EnemyClusterPosition.SetVec2(matchedEnemyCluster.AggregatePosition);
        list1.Remove(formation);
        if (!list2.IsEmpty<Formation>())
        {
          Formation f = list2.Last<Formation>();
          f.AI.ResetBehaviorWeights();
          TacticComponent.SetDefaultBehaviorWeights(f);
          f.AI.SetBehaviorWeight<BehaviorSkirmishBehindFormation>(1f).ReferenceFormation = formation;
          defenseFront.RangedFormation = f;
          list2.Remove(f);
          this._defenseFronts.Add(defenseFront);
        }
      }
    }

    protected internal override void TickOccasionally()
    {
      if (!this.AreFormationsCreated)
        return;
      this.CheckAndChangeState();
      if (this._isRetreatingToKeep)
        return;
      this.DetermineEnemyClusters();
      this.ArrangeDefenseFronts();
    }

    internal override float GetTacticWeight() => this._defendPosition.IsValid ? 10f : 0.0f;

    private class DefenseFront
    {
      public Formation MeleeFormation;
      public Formation RangedFormation;
      public TacticPerimeterDefense.EnemyCluster MatchedEnemyCluster;

      public DefenseFront(
        TacticPerimeterDefense.EnemyCluster matchedEnemyCluster,
        Formation meleeFormation)
      {
        this.MatchedEnemyCluster = matchedEnemyCluster;
        this.MeleeFormation = meleeFormation;
        this.RangedFormation = (Formation) null;
      }
    }

    private class EnemyCluster
    {
      public List<Formation> enemyFormations;
      public float totalPower;

      public Vec2 AggregatePosition { get; private set; }

      public WorldPosition MedianAggregatePosition { get; private set; }

      public EnemyCluster() => this.enemyFormations = new List<Formation>();

      public void UpdateClusterData()
      {
        this.totalPower = this.enemyFormations.Sum<Formation>((Func<Formation, float>) (ef => ef.QuerySystem.FormationPower));
        this.AggregatePosition = Vec2.Zero;
        foreach (Formation enemyFormation in this.enemyFormations)
          this.AggregatePosition += enemyFormation.QuerySystem.AveragePosition * (enemyFormation.QuerySystem.FormationPower / this.totalPower);
        this.UpdateMedianPosition();
      }

      public void AddToCluster(Formation formation)
      {
        this.enemyFormations.Add(formation);
        float totalPower = this.totalPower;
        this.totalPower += formation.QuerySystem.FormationPower;
        this.AggregatePosition = this.AggregatePosition * (totalPower / this.totalPower) + formation.QuerySystem.AveragePosition * (formation.QuerySystem.FormationPower / this.totalPower);
        this.UpdateMedianPosition();
      }

      public void RemoveFromCluster(Formation formation)
      {
        this.enemyFormations.Remove(formation);
        float totalPower = this.totalPower;
        this.totalPower -= formation.QuerySystem.FormationPower;
        this.AggregatePosition -= formation.QuerySystem.AveragePosition * (formation.QuerySystem.FormationPower / totalPower);
        this.AggregatePosition *= totalPower / this.totalPower;
        this.UpdateMedianPosition();
      }

      private void UpdateMedianPosition()
      {
        float num1 = float.MaxValue;
        foreach (Formation enemyFormation in this.enemyFormations)
        {
          float num2 = enemyFormation.QuerySystem.MedianPosition.AsVec2.DistanceSquared(this.AggregatePosition);
          if ((double) num2 < (double) num1)
          {
            num1 = num2;
            this.MedianAggregatePosition = enemyFormation.QuerySystem.MedianPosition;
          }
        }
      }
    }
  }
}
