// Decompiled with JetBrains decompiler
// Type: TaleWorlds.MountAndBlade.BehaviorSkirmish
// Assembly: TaleWorlds.MountAndBlade, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3C0E3F42-756F-4407-B817-06BA412A78B0
// Assembly location: E:\SteamLibrary\steamapps\common\Mount & Blade II Bannerlord\bin\Win64_Shipping_Client\TaleWorlds.MountAndBlade.dll

using System;
using TaleWorlds.Core;
using TaleWorlds.Engine;
using TaleWorlds.Library;

namespace TaleWorlds.MountAndBlade
{
  public class BehaviorSkirmish : BehaviorComponent
  {
    private bool _cantShoot;
    private float _cantShootDistance = float.MaxValue;
    private BehaviorSkirmish.BehaviorState _behaviorState = BehaviorSkirmish.BehaviorState.Shooting;
    private Timer _cantShootTimer;

    public BehaviorSkirmish(Formation formation)
      : base(formation)
    {
      this.BehaviorCoherence = 0.5f;
      this._cantShootTimer = new Timer(0.0f, 0.0f);
      this.CalculateCurrentOrder();
    }

    protected override void CalculateCurrentOrder()
    {
      WorldPosition medianPosition = this.formation.QuerySystem.MedianPosition;
      bool flag = false;
      Vec2 direction;
      WorldPosition worldPosition;
      if (this.formation.QuerySystem.ClosestSignificantlyLargeEnemyFormation == null)
      {
        direction = this.formation.Direction;
        medianPosition.SetVec2(this.formation.QuerySystem.AveragePosition);
      }
      else
      {
        worldPosition = this.formation.QuerySystem.ClosestSignificantlyLargeEnemyFormation.MedianPosition;
        direction = worldPosition.AsVec2 - this.formation.QuerySystem.AveragePosition;
        float val2 = direction.Normalize();
        float num = MBMath.Lerp(0.1f, 0.33f, (float) (1.0 - (double) MBMath.ClampFloat((float) this.formation.CountOfUnits, 1f, 50f) * 0.0199999995529652)) * this.formation.QuerySystem.RangedUnitRatio;
        switch (this._behaviorState)
        {
          case BehaviorSkirmish.BehaviorState.Approaching:
            if ((double) val2 < (double) this._cantShootDistance * 0.800000011920929)
            {
              this._behaviorState = BehaviorSkirmish.BehaviorState.Shooting;
              this._cantShoot = false;
              flag = true;
              break;
            }
            if ((double) this.formation.QuerySystem.MakingRangedAttackRatio >= (double) num * 1.20000004768372)
            {
              this._behaviorState = BehaviorSkirmish.BehaviorState.Shooting;
              this._cantShoot = false;
              flag = true;
              break;
            }
            break;
          case BehaviorSkirmish.BehaviorState.Shooting:
            if ((double) this.formation.QuerySystem.MakingRangedAttackRatio <= (double) num)
            {
              if ((double) val2 > (double) this.formation.QuerySystem.MissileRange)
              {
                this._behaviorState = BehaviorSkirmish.BehaviorState.Approaching;
                this._cantShootDistance = Math.Min(this._cantShootDistance, this.formation.QuerySystem.MissileRange * 0.9f);
                break;
              }
              if (!this._cantShoot)
              {
                this._cantShoot = true;
                this._cantShootTimer.Reset(Mission.Current.Time, MBMath.Lerp(5f, 10f, (float) (((double) MBMath.ClampFloat((float) this.formation.CountOfUnits, 10f, 60f) - 10.0) * 0.0199999995529652)));
                break;
              }
              if (this._cantShootTimer.Check(Mission.Current.Time))
              {
                this._behaviorState = BehaviorSkirmish.BehaviorState.Approaching;
                this._cantShootDistance = Math.Min(this._cantShootDistance, val2);
                break;
              }
              break;
            }
            this._cantShootDistance = Math.Max(this._cantShootDistance, val2);
            this._cantShoot = false;
            if (this.formation.QuerySystem.IsInfantryFormation && (double) val2 < (double) Math.Min(this.formation.QuerySystem.MissileRange * 0.4f, this._cantShootDistance * 0.666f))
            {
              this._behaviorState = BehaviorSkirmish.BehaviorState.PullingBack;
              break;
            }
            break;
          case BehaviorSkirmish.BehaviorState.PullingBack:
            if ((double) val2 > (double) Math.Min(this._cantShootDistance, this.formation.QuerySystem.MissileRange) * 0.800000011920929)
            {
              this._behaviorState = BehaviorSkirmish.BehaviorState.Shooting;
              this._cantShoot = false;
              flag = true;
              break;
            }
            break;
        }
        switch (this._behaviorState)
        {
          case BehaviorSkirmish.BehaviorState.Approaching:
            medianPosition = this.formation.QuerySystem.ClosestSignificantlyLargeEnemyFormation.MedianPosition;
            medianPosition.SetVec2(this.formation.QuerySystem.ClosestSignificantlyLargeEnemyFormation.AveragePosition);
            break;
          case BehaviorSkirmish.BehaviorState.Shooting:
            medianPosition.SetVec2(this.formation.QuerySystem.AveragePosition);
            break;
          case BehaviorSkirmish.BehaviorState.PullingBack:
            medianPosition = this.formation.QuerySystem.ClosestSignificantlyLargeEnemyFormation.MedianPosition;
            medianPosition.SetVec2(medianPosition.AsVec2 - direction * (float) ((double) this.formation.QuerySystem.MissileRange - (double) this.formation.Depth * 0.5 - 10.0));
            break;
        }
      }
      worldPosition = this.CurrentOrder.GetPosition(this.formation);
      if (((!worldPosition.IsValid ? 1 : (this._behaviorState != BehaviorSkirmish.BehaviorState.Shooting ? 1 : 0)) | (flag ? 1 : 0)) != 0)
        this.CurrentOrder = MovementOrder.MovementOrderMove(medianPosition);
      if (((!this.CurrentFacingOrder.GetDirection(this.formation).IsValid ? 1 : (this._behaviorState != BehaviorSkirmish.BehaviorState.Shooting ? 1 : 0)) | (flag ? 1 : 0)) == 0)
        return;
      this.CurrentFacingOrder = FacingOrder.FacingOrderLookAtDirection(direction);
    }

    protected internal override void TickOccasionally()
    {
      this.CalculateCurrentOrder();
      this.formation.MovementOrder = this.CurrentOrder;
      this.formation.FacingOrder = this.CurrentFacingOrder;
    }

    protected override void OnBehaviorActivatedAux()
    {
      this._cantShoot = false;
      this._cantShootDistance = float.MaxValue;
      this._behaviorState = BehaviorSkirmish.BehaviorState.Shooting;
      this._cantShootTimer.Reset(Mission.Current.Time, MBMath.Lerp(5f, 10f, (float) (((double) MBMath.ClampFloat((float) this.formation.CountOfUnits, 10f, 60f) - 10.0) * 0.0199999995529652)));
      this.CalculateCurrentOrder();
      this.formation.MovementOrder = this.CurrentOrder;
      this.formation.FacingOrder = this.CurrentFacingOrder;
      this.formation.ArrangementOrder = ArrangementOrder.ArrangementOrderLoose;
      this.formation.FiringOrder = FiringOrder.FiringOrderFireAtWill;
      this.formation.FormOrder = FormOrder.FormOrderWide;
      this.formation.WeaponUsageOrder = WeaponUsageOrder.WeaponUsageOrderUseAny;
    }

    protected override float GetAiWeight()
    {
      FormationQuerySystem querySystem = this.formation.QuerySystem;
      return MBMath.Lerp(0.1f, 1f, MBMath.ClampFloat(querySystem.RangedUnitRatio + querySystem.RangedCavalryUnitRatio, 0.0f, 0.5f) * 2f);
    }

    private enum BehaviorState
    {
      Approaching,
      Shooting,
      PullingBack,
    }
  }
}
