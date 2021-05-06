// Decompiled with JetBrains decompiler
// Type: TaleWorlds.MountAndBlade.BehaviorCautiousAdvance
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
  public sealed class BehaviorCautiousAdvance : BehaviorComponent
  {
    private bool _isInShieldWallDistance;
    private bool _switchedToShieldWallRecently;
    private Timer _switchedToShieldWallTimer;
    private Vec2 _reformPosition = Vec2.Invalid;
    private Formation _archerFormation;
    private bool _cantShoot;
    private float _cantShootDistance = float.MaxValue;
    private BehaviorCautiousAdvance.BehaviorState _behaviorState = BehaviorCautiousAdvance.BehaviorState.Shooting;
    private Timer _cantShootTimer;
    private Vec2 _shootPosition = Vec2.Invalid;

    public BehaviorCautiousAdvance()
    {
      this.BehaviorCoherence = 1f;
      this._cantShootTimer = new Timer(0.0f, 0.0f);
      this._switchedToShieldWallTimer = new Timer(0.0f, 0.0f);
    }

    public BehaviorCautiousAdvance(Formation formation)
      : base(formation)
    {
      this.BehaviorCoherence = 1f;
      this._cantShootTimer = new Timer(0.0f, 0.0f);
      this._switchedToShieldWallTimer = new Timer(0.0f, 0.0f);
      this.CalculateCurrentOrder();
    }

    protected override void CalculateCurrentOrder()
    {
      WorldPosition medianPosition = this.formation.QuerySystem.MedianPosition;
      bool flag = false;
      Vec2 vec2_1;
      WorldPosition worldPosition;
      Vec2 vec2_2;
      if (this.formation.QuerySystem.ClosestSignificantlyLargeEnemyFormation == null || this._archerFormation == null)
      {
        vec2_1 = this.formation.Direction;
        medianPosition.SetVec2(this.formation.QuerySystem.AveragePosition);
      }
      else
      {
        worldPosition = this.formation.QuerySystem.ClosestSignificantlyLargeEnemyFormation.MedianPosition;
        vec2_1 = worldPosition.AsVec2 - this.formation.QuerySystem.AveragePosition;
        float val2 = vec2_1.Normalize();
        float num = this._archerFormation.QuerySystem.RangedUnitRatio * 0.5f / (float) this._archerFormation.arrangement.RankCount;
        switch (this._behaviorState)
        {
          case BehaviorCautiousAdvance.BehaviorState.Approaching:
            if ((double) val2 < (double) this._cantShootDistance * 0.800000011920929)
            {
              this._behaviorState = BehaviorCautiousAdvance.BehaviorState.Shooting;
              this._cantShoot = false;
              flag = true;
            }
            else if ((double) this._archerFormation.QuerySystem.MakingRangedAttackRatio >= (double) num * 1.20000004768372)
            {
              this._behaviorState = BehaviorCautiousAdvance.BehaviorState.Shooting;
              this._cantShoot = false;
              flag = true;
            }
            if (this._behaviorState == BehaviorCautiousAdvance.BehaviorState.Shooting)
            {
              this._shootPosition = this.formation.QuerySystem.AveragePosition + vec2_1 * 5f;
              break;
            }
            break;
          case BehaviorCautiousAdvance.BehaviorState.Shooting:
            if ((double) this._archerFormation.QuerySystem.MakingRangedAttackRatio <= (double) num)
            {
              if ((double) val2 > (double) this._archerFormation.QuerySystem.MissileRange)
              {
                this._behaviorState = BehaviorCautiousAdvance.BehaviorState.Approaching;
                this._cantShootDistance = Math.Min(this._cantShootDistance, this._archerFormation.QuerySystem.MissileRange * 0.9f);
                this._shootPosition = Vec2.Invalid;
                break;
              }
              if (!this._cantShoot)
              {
                this._cantShoot = true;
                this._cantShootTimer.Reset(Mission.Current.Time, this._archerFormation == null ? 10f : MBMath.Lerp(10f, 15f, (float) (((double) MBMath.ClampFloat((float) this._archerFormation.CountOfUnits, 10f, 60f) - 10.0) * 0.0199999995529652)));
                break;
              }
              if (this._cantShootTimer.Check(Mission.Current.Time))
              {
                this._behaviorState = BehaviorCautiousAdvance.BehaviorState.Approaching;
                this._cantShootDistance = Math.Min(this._cantShootDistance, val2);
                this._shootPosition = Vec2.Invalid;
                break;
              }
              break;
            }
            this._cantShootDistance = Math.Max(this._cantShootDistance, val2);
            this._cantShoot = false;
            if ((!this.formation.QuerySystem.ClosestSignificantlyLargeEnemyFormation.IsRangedFormation && !this.formation.QuerySystem.ClosestSignificantlyLargeEnemyFormation.IsRangedCavalryFormation || (double) val2 < (double) this.formation.QuerySystem.ClosestSignificantlyLargeEnemyFormation.MissileRange && (double) this.formation.QuerySystem.ClosestSignificantlyLargeEnemyFormation.MakingRangedAttackRatio < 0.100000001490116) && (double) val2 < (double) Math.Min(this._archerFormation.QuerySystem.MissileRange * 0.4f, this._cantShootDistance * 0.667f))
            {
              this._behaviorState = BehaviorCautiousAdvance.BehaviorState.PullingBack;
              this._shootPosition = Vec2.Invalid;
              break;
            }
            break;
          case BehaviorCautiousAdvance.BehaviorState.PullingBack:
            if ((double) val2 > (double) Math.Min(this._cantShootDistance, this._archerFormation.QuerySystem.MissileRange) * 0.800000011920929)
            {
              this._behaviorState = BehaviorCautiousAdvance.BehaviorState.Shooting;
              this._cantShoot = false;
              this._shootPosition = this.formation.QuerySystem.AveragePosition + vec2_1 * 5f;
              flag = true;
              break;
            }
            break;
        }
        switch (this._behaviorState)
        {
          case BehaviorCautiousAdvance.BehaviorState.Approaching:
            medianPosition = this.formation.QuerySystem.ClosestEnemyFormation.MedianPosition;
            if (this._switchedToShieldWallRecently && !this._switchedToShieldWallTimer.Check(Mission.Current.Time) && (double) this.formation.QuerySystem.FormationDispersedness > 2.0)
            {
              if (this._reformPosition.IsValid)
              {
                medianPosition.SetVec2(this._reformPosition);
                break;
              }
              worldPosition = this.formation.QuerySystem.Team.MedianTargetFormationPosition;
              vec2_2 = worldPosition.AsVec2 - this.formation.QuerySystem.AveragePosition;
              vec2_1 = vec2_2.Normalized();
              this._reformPosition = this.formation.QuerySystem.AveragePosition + vec2_1 * 5f;
              medianPosition.SetVec2(this._reformPosition);
              break;
            }
            this._switchedToShieldWallRecently = false;
            this._reformPosition = Vec2.Invalid;
            medianPosition.SetVec2(this.formation.QuerySystem.ClosestEnemyFormation.AveragePosition);
            break;
          case BehaviorCautiousAdvance.BehaviorState.Shooting:
            if (this._shootPosition.IsValid)
            {
              medianPosition.SetVec2(this._shootPosition);
              break;
            }
            medianPosition.SetVec2(this.formation.QuerySystem.AveragePosition);
            break;
          case BehaviorCautiousAdvance.BehaviorState.PullingBack:
            medianPosition = this.formation.QuerySystem.MedianPosition;
            medianPosition.SetVec2(this.formation.QuerySystem.AveragePosition);
            break;
        }
      }
      worldPosition = this.CurrentOrder.GetPosition(this.formation);
      if (((!worldPosition.IsValid ? 1 : (this._behaviorState != BehaviorCautiousAdvance.BehaviorState.Shooting ? 1 : 0)) | (flag ? 1 : 0)) == 0)
      {
        worldPosition = this.CurrentOrder.GetPosition(this.formation);
        if ((double) worldPosition.GetNavMeshVec3().DistanceSquared(medianPosition.GetNavMeshVec3()) < (double) this.formation.Depth * (double) this.formation.Depth)
          goto label_34;
      }
      this.CurrentOrder = MovementOrder.MovementOrderMove(medianPosition);
label_34:
      vec2_2 = this.CurrentFacingOrder.GetDirection(this.formation);
      if (vec2_2.IsValid && !(this._behaviorState != BehaviorCautiousAdvance.BehaviorState.Shooting | flag))
      {
        vec2_2 = this.CurrentFacingOrder.GetDirection(this.formation);
        if ((double) vec2_2.DotProduct(vec2_1) > (double) MBMath.Lerp(0.5f, 1f, (float) (1.0 - (double) MBMath.ClampFloat(this.formation.Width, 1f, 20f) * 0.0500000007450581)))
          return;
      }
      this.CurrentFacingOrder = FacingOrder.FacingOrderLookAtDirection(vec2_1);
    }

    protected override void OnBehaviorActivatedAux()
    {
      IEnumerable<Formation> source = this.formation.Team.Formations.Where<Formation>((Func<Formation, bool>) (f => f != this.formation && f.QuerySystem.IsRangedFormation));
      if (source.Any<Formation>())
        this._archerFormation = source.MaxBy<Formation, float>((Func<Formation, float>) (f => f.QuerySystem.FormationPower));
      this._cantShoot = false;
      this._cantShootDistance = float.MaxValue;
      this._behaviorState = BehaviorCautiousAdvance.BehaviorState.Shooting;
      this._cantShootTimer.Reset(Mission.Current.Time, this._archerFormation == null ? 10f : MBMath.Lerp(10f, 15f, (float) (((double) MBMath.ClampFloat((float) this._archerFormation.CountOfUnits, 10f, 60f) - 10.0) * 0.0199999995529652)));
      this.CalculateCurrentOrder();
      this.formation.MovementOrder = this.CurrentOrder;
      this.formation.FacingOrder = this.CurrentFacingOrder;
      this._isInShieldWallDistance = true;
      this.formation.ArrangementOrder = ArrangementOrder.ArrangementOrderLine;
      this.formation.FiringOrder = FiringOrder.FiringOrderFireAtWill;
      this.formation.FormOrder = FormOrder.FormOrderWide;
      this.formation.WeaponUsageOrder = WeaponUsageOrder.WeaponUsageOrderUseAny;
    }

    public override void OnBehaviorCanceled()
    {
    }

    protected internal override void TickOccasionally()
    {
      if (this.formation.IsInfantry())
      {
        bool flag = this.formation.QuerySystem.ClosestEnemyFormation != null && (this.formation.QuerySystem.IsUnderRangedAttack || (double) this.formation.QuerySystem.AveragePosition.DistanceSquared(this.CurrentOrder.GetPosition(this.formation).AsVec2) < 25.0 + (this._isInShieldWallDistance ? 75.0 : 0.0)) && (double) this.formation.QuerySystem.AveragePosition.DistanceSquared(this.formation.QuerySystem.ClosestEnemyFormation.MedianPosition.AsVec2) > 100.0 - (this._isInShieldWallDistance ? 75.0 : 0.0);
        if (flag != this._isInShieldWallDistance)
        {
          this._isInShieldWallDistance = flag;
          if (this._isInShieldWallDistance)
          {
            ArrangementOrder arrangementOrder = this.formation.QuerySystem.HasShield ? ArrangementOrder.ArrangementOrderShieldWall : ArrangementOrder.ArrangementOrderLoose;
            if (this.formation.ArrangementOrder != arrangementOrder)
            {
              this.formation.ArrangementOrder = arrangementOrder;
              this._switchedToShieldWallRecently = true;
              this._switchedToShieldWallTimer.Reset(Mission.Current.Time, 5f);
            }
          }
          else if (!(this.formation.ArrangementOrder == ArrangementOrder.ArrangementOrderLine))
            this.formation.ArrangementOrder = ArrangementOrder.ArrangementOrderLine;
        }
      }
      this.CalculateCurrentOrder();
      this.formation.MovementOrder = this.CurrentOrder;
      this.formation.FacingOrder = this.CurrentFacingOrder;
    }

    protected override float GetAiWeight() => 1f;

    private enum BehaviorState
    {
      Approaching,
      Shooting,
      PullingBack,
    }
  }
}
