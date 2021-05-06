// Decompiled with JetBrains decompiler
// Type: TaleWorlds.MountAndBlade.BehaviorTacticalCharge
// Assembly: TaleWorlds.MountAndBlade, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3C0E3F42-756F-4407-B817-06BA412A78B0
// Assembly location: E:\SteamLibrary\steamapps\common\Mount & Blade II Bannerlord\bin\Win64_Shipping_Client\TaleWorlds.MountAndBlade.dll

using TaleWorlds.Core;
using TaleWorlds.Engine;
using TaleWorlds.Library;
using TaleWorlds.Localization;

namespace TaleWorlds.MountAndBlade
{
  public class BehaviorTacticalCharge : BehaviorComponent
  {
    private BehaviorTacticalCharge.ChargeState _chargeState;
    private FormationQuerySystem _lastTarget;
    private Vec2 _initialChargeDirection;
    private float _desiredChargeStopDistance;
    private WorldPosition _lastReformDestination;
    private Timer _chargingPastTimer;
    private Timer _reformTimer;
    private Vec2 _bracePosition = Vec2.Invalid;

    public BehaviorTacticalCharge(Formation formation)
      : base(formation)
    {
      this._lastTarget = (FormationQuerySystem) null;
      this.CurrentOrder = MovementOrder.MovementOrderCharge;
      this.CurrentFacingOrder = FacingOrder.FacingOrderLookAtEnemy;
      this._chargeState = BehaviorTacticalCharge.ChargeState.Undetermined;
      this.BehaviorCoherence = 0.5f;
      this._desiredChargeStopDistance = 20f;
    }

    protected internal override void TickOccasionally()
    {
      base.TickOccasionally();
      if (this.formation.AI.ActiveBehavior != this)
        return;
      this.CalculateCurrentOrder();
      this.formation.MovementOrder = this.CurrentOrder;
      this.formation.FacingOrder = this.CurrentFacingOrder;
    }

    private BehaviorTacticalCharge.ChargeState CheckAndChangeState()
    {
      BehaviorTacticalCharge.ChargeState chargeState = this._chargeState;
      if (this.formation.QuerySystem.ClosestEnemyFormation == null)
      {
        chargeState = BehaviorTacticalCharge.ChargeState.Undetermined;
      }
      else
      {
        switch (this._chargeState)
        {
          case BehaviorTacticalCharge.ChargeState.Undetermined:
            if (this.formation.QuerySystem.ClosestEnemyFormation != null && (!this.formation.QuerySystem.IsCavalryFormation && !this.formation.QuerySystem.IsRangedCavalryFormation || (double) this.formation.QuerySystem.AveragePosition.Distance(this.formation.QuerySystem.ClosestEnemyFormation.MedianPosition.AsVec2) / (double) this.formation.QuerySystem.MovementSpeedMaximum <= 5.0))
            {
              chargeState = BehaviorTacticalCharge.ChargeState.Charging;
              break;
            }
            break;
          case BehaviorTacticalCharge.ChargeState.Charging:
            if (!this.formation.QuerySystem.IsCavalryFormation && !this.formation.QuerySystem.IsRangedCavalryFormation)
            {
              if (!this.formation.QuerySystem.IsInfantryFormation || !this.formation.QuerySystem.ClosestEnemyFormation.IsCavalryFormation)
              {
                chargeState = BehaviorTacticalCharge.ChargeState.Charging;
                break;
              }
              Vec2 vec2 = this.formation.QuerySystem.AveragePosition - this.formation.QuerySystem.ClosestEnemyFormation.AveragePosition;
              double num1 = (double) vec2.Normalize();
              Vec2 currentVelocity = this.formation.QuerySystem.ClosestEnemyFormation.CurrentVelocity;
              double num2 = (double) currentVelocity.Normalize();
              if (num1 / num2 <= 6.0 && (double) vec2.DotProduct(currentVelocity) > 0.5)
              {
                this._chargeState = BehaviorTacticalCharge.ChargeState.Bracing;
                break;
              }
              break;
            }
            if ((double) this._initialChargeDirection.DotProduct(this.formation.QuerySystem.ClosestEnemyFormation.MedianPosition.AsVec2 - this.formation.QuerySystem.AveragePosition) <= 0.0)
            {
              chargeState = BehaviorTacticalCharge.ChargeState.ChargingPast;
              break;
            }
            break;
          case BehaviorTacticalCharge.ChargeState.ChargingPast:
            if (this._chargingPastTimer.Check(MBCommon.GetTime(MBCommon.TimeType.Mission)) || (double) this.formation.QuerySystem.AveragePosition.Distance(this.formation.QuerySystem.ClosestEnemyFormation.MedianPosition.AsVec2) >= (double) this._desiredChargeStopDistance)
            {
              chargeState = BehaviorTacticalCharge.ChargeState.Reforming;
              break;
            }
            break;
          case BehaviorTacticalCharge.ChargeState.Reforming:
            if (this._reformTimer.Check(MBCommon.GetTime(MBCommon.TimeType.Mission)) || (double) this.formation.QuerySystem.AveragePosition.Distance(this.formation.QuerySystem.ClosestEnemyFormation.MedianPosition.AsVec2) <= 30.0)
            {
              chargeState = BehaviorTacticalCharge.ChargeState.Charging;
              break;
            }
            break;
          case BehaviorTacticalCharge.ChargeState.Bracing:
            bool flag = false;
            if (this.formation.QuerySystem.IsInfantryFormation && this.formation.QuerySystem.ClosestEnemyFormation.IsCavalryFormation)
            {
              Vec2 vec2 = this.formation.QuerySystem.AveragePosition - this.formation.QuerySystem.ClosestEnemyFormation.AveragePosition;
              double num1 = (double) vec2.Normalize();
              Vec2 currentVelocity = this.formation.QuerySystem.ClosestEnemyFormation.CurrentVelocity;
              double num2 = (double) currentVelocity.Normalize();
              if (num1 / num2 <= 8.0 && (double) vec2.DotProduct(currentVelocity) > 0.330000013113022)
                flag = true;
            }
            if (!flag)
            {
              this._bracePosition = Vec2.Invalid;
              this._chargeState = BehaviorTacticalCharge.ChargeState.Charging;
              break;
            }
            break;
        }
      }
      return chargeState;
    }

    protected override void CalculateCurrentOrder()
    {
      if (this.formation.QuerySystem.ClosestEnemyFormation == null)
      {
        this.CurrentOrder = MovementOrder.MovementOrderCharge;
      }
      else
      {
        BehaviorTacticalCharge.ChargeState chargeState = this.CheckAndChangeState();
        if (chargeState != this._chargeState)
        {
          this._chargeState = chargeState;
          switch (this._chargeState)
          {
            case BehaviorTacticalCharge.ChargeState.Undetermined:
              this.CurrentOrder = MovementOrder.MovementOrderCharge;
              break;
            case BehaviorTacticalCharge.ChargeState.Charging:
              this._lastTarget = this.formation.QuerySystem.ClosestEnemyFormation;
              if (this.formation.QuerySystem.IsCavalryFormation || this.formation.QuerySystem.IsRangedCavalryFormation)
              {
                this._initialChargeDirection = this._lastTarget.MedianPosition.AsVec2 - this.formation.QuerySystem.AveragePosition;
                this._desiredChargeStopDistance = MBMath.ClampFloat(this._initialChargeDirection.Normalize(), 20f, 50f);
                break;
              }
              break;
            case BehaviorTacticalCharge.ChargeState.ChargingPast:
              this._chargingPastTimer = new Timer(MBCommon.GetTime(MBCommon.TimeType.Mission), 5f);
              break;
            case BehaviorTacticalCharge.ChargeState.Reforming:
              this._reformTimer = new Timer(MBCommon.GetTime(MBCommon.TimeType.Mission), 2f);
              break;
            case BehaviorTacticalCharge.ChargeState.Bracing:
              this._bracePosition = this.formation.QuerySystem.AveragePosition + (this.formation.QuerySystem.Team.MedianTargetFormationPosition.AsVec2 - this.formation.QuerySystem.AveragePosition).Normalized() * 5f;
              break;
          }
        }
        switch (this._chargeState)
        {
          case BehaviorTacticalCharge.ChargeState.Undetermined:
            if (this.formation.QuerySystem.ClosestEnemyFormation != null && (this.formation.QuerySystem.IsCavalryFormation || this.formation.QuerySystem.IsRangedCavalryFormation))
              this.CurrentOrder = MovementOrder.MovementOrderMove(this.formation.QuerySystem.ClosestEnemyFormation.MedianPosition);
            else
              this.CurrentOrder = MovementOrder.MovementOrderCharge;
            this.CurrentFacingOrder = FacingOrder.FacingOrderLookAtEnemy;
            break;
          case BehaviorTacticalCharge.ChargeState.Charging:
            if (!this.formation.QuerySystem.IsCavalryFormation && !this.formation.QuerySystem.IsRangedCavalryFormation)
            {
              this.CurrentOrder = MovementOrder.MovementOrderMove(this.formation.QuerySystem.ClosestEnemyFormation.MedianPosition);
              this.CurrentFacingOrder = FacingOrder.FacingOrderLookAtEnemy;
              break;
            }
            Vec2 direction1 = (this._lastTarget.MedianPosition.AsVec2 - this.formation.QuerySystem.AveragePosition).Normalized();
            WorldPosition medianPosition1 = this._lastTarget.MedianPosition;
            Vec2 vec2 = medianPosition1.AsVec2 + direction1 * this._desiredChargeStopDistance;
            medianPosition1.SetVec2(vec2);
            this.CurrentOrder = MovementOrder.MovementOrderMove(medianPosition1);
            this.CurrentFacingOrder = FacingOrder.FacingOrderLookAtDirection(direction1);
            break;
          case BehaviorTacticalCharge.ChargeState.ChargingPast:
            Vec2 averagePosition = this.formation.QuerySystem.AveragePosition;
            WorldPosition medianPosition2 = this._lastTarget.MedianPosition;
            Vec2 asVec2 = medianPosition2.AsVec2;
            Vec2 direction2 = averagePosition - asVec2;
            if ((double) direction2.Normalize() <= 20.0)
            {
              Vec2 initialChargeDirection = this._initialChargeDirection;
            }
            this._lastReformDestination = this._lastTarget.MedianPosition;
            medianPosition2 = this._lastTarget.MedianPosition;
            this._lastReformDestination.SetVec2(medianPosition2.AsVec2 + direction2 * this._desiredChargeStopDistance);
            this.CurrentOrder = MovementOrder.MovementOrderMove(this._lastReformDestination);
            this.CurrentFacingOrder = FacingOrder.FacingOrderLookAtDirection(direction2);
            break;
          case BehaviorTacticalCharge.ChargeState.Reforming:
            this.CurrentOrder = MovementOrder.MovementOrderMove(this._lastReformDestination);
            this.CurrentFacingOrder = FacingOrder.FacingOrderLookAtEnemy;
            break;
          case BehaviorTacticalCharge.ChargeState.Bracing:
            WorldPosition medianPosition3 = this.formation.QuerySystem.MedianPosition;
            medianPosition3.SetVec2(this._bracePosition);
            this.CurrentOrder = MovementOrder.MovementOrderMove(medianPosition3);
            break;
        }
      }
    }

    protected override void OnBehaviorActivatedAux()
    {
      this.CalculateCurrentOrder();
      this.formation.MovementOrder = this.CurrentOrder;
      this.formation.FacingOrder = this.CurrentFacingOrder;
      if (this.formation.QuerySystem.IsCavalryFormation || this.formation.QuerySystem.IsRangedCavalryFormation)
        this.formation.ArrangementOrder = ArrangementOrder.ArrangementOrderSkein;
      else
        this.formation.ArrangementOrder = ArrangementOrder.ArrangementOrderLine;
      this.formation.FiringOrder = FiringOrder.FiringOrderFireAtWill;
      this.formation.FormOrder = FormOrder.FormOrderWide;
      this.formation.WeaponUsageOrder = WeaponUsageOrder.WeaponUsageOrderUseAny;
    }

    public override TextObject GetBehaviorString()
    {
      string text;
      if (this.formation.QuerySystem.ClosestEnemyFormation != null)
        text = string.Join(" ", (object) GameTexts.FindText("str_formation_ai_side_strings", this.formation.QuerySystem.ClosestEnemyFormation.Formation.AI.Side.ToString()), (object) GameTexts.FindText("str_formation_class_string", this.formation.QuerySystem.ClosestEnemyFormation.Formation.PrimaryClass.GetName()));
      else
        text = "";
      MBTextManager.SetTextVariable("TARGET_FORMATION", text, false);
      return base.GetBehaviorString();
    }

    protected internal override float NavmeshlessTargetPositionPenalty => 1f;

    protected override float GetAiWeight()
    {
      FormationQuerySystem querySystem = this.formation.QuerySystem;
      if (querySystem.ClosestEnemyFormation == null)
        return 0.0f;
      Vec2 vec2 = querySystem.AveragePosition;
      ref Vec2 local = ref vec2;
      WorldPosition medianPosition1 = querySystem.ClosestEnemyFormation.MedianPosition;
      Vec2 asVec2_1 = medianPosition1.AsVec2;
      float num1 = local.Distance(asVec2_1) / querySystem.MovementSpeedMaximum;
      float num2 = querySystem.IsCavalryFormation || querySystem.IsRangedCavalryFormation ? ((double) num1 > 4.0 ? MBMath.Lerp(0.8f, 1.2f, (float) (1.0 - ((double) MBMath.ClampFloat(num1, 4f, 10f) - 4.0) / 6.0)) : MBMath.Lerp(0.8f, 1.2f, MBMath.ClampFloat(num1, 0.0f, 4f) / 4f)) : MBMath.Lerp(0.8f, 1f, (float) (1.0 - ((double) MBMath.ClampFloat(num1, 4f, 10f) - 4.0) / 6.0));
      WorldPosition medianPosition2 = querySystem.MedianPosition;
      medianPosition2.SetVec2(querySystem.AveragePosition);
      double navMeshZ1 = (double) medianPosition2.GetNavMeshZ();
      medianPosition1 = querySystem.ClosestEnemyFormation.MedianPosition;
      double navMeshZ2 = (double) medianPosition1.GetNavMeshZ();
      float num3 = (float) (navMeshZ1 - navMeshZ2);
      float num4 = 1f;
      if ((double) num1 <= 4.0)
      {
        double num5 = (double) num3;
        Vec2 averagePosition = querySystem.AveragePosition;
        medianPosition1 = querySystem.ClosestEnemyFormation.MedianPosition;
        Vec2 asVec2_2 = medianPosition1.AsVec2;
        vec2 = averagePosition - asVec2_2;
        double length = (double) vec2.Length;
        num4 = MBMath.Lerp(0.9f, 1.1f, (float) (((double) MBMath.ClampFloat((float) (num5 / length), -0.58f, 0.58f) + 0.579999983310699) / 1.1599999666214));
      }
      float num6 = 1f;
      if ((double) num1 <= 4.0 && (double) num1 >= 1.5)
        num6 = 1.2f;
      float num7 = 1f;
      if ((double) num1 <= 4.0 && querySystem.ClosestEnemyFormation.ClosestEnemyFormation != querySystem)
        num7 = 1.2f;
      float num8 = querySystem.GetClassWeightedFactor(1f, 1f, 1.5f, 1.5f) * querySystem.ClosestEnemyFormation.GetClassWeightedFactor(1f, 1f, 0.5f, 0.5f);
      return num2 * num4 * num6 * num7 * num8;
    }

    private enum ChargeState
    {
      Undetermined,
      Charging,
      ChargingPast,
      Reforming,
      Bracing,
    }
  }
}
