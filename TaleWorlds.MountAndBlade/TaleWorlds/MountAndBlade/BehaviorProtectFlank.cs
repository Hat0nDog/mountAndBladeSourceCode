// Decompiled with JetBrains decompiler
// Type: TaleWorlds.MountAndBlade.BehaviorProtectFlank
// Assembly: TaleWorlds.MountAndBlade, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3C0E3F42-756F-4407-B817-06BA412A78B0
// Assembly location: E:\SteamLibrary\steamapps\common\Mount & Blade II Bannerlord\bin\Win64_Shipping_Client\TaleWorlds.MountAndBlade.dll

using System;
using System.Linq;
using TaleWorlds.Core;
using TaleWorlds.Engine;
using TaleWorlds.Library;
using TaleWorlds.Localization;

namespace TaleWorlds.MountAndBlade
{
  public class BehaviorProtectFlank : BehaviorComponent
  {
    private Formation _mainFormation;
    public FormationAI.BehaviorSide FlankSide;
    private BehaviorProtectFlank.BehaviourState _protectFlankState;
    private MovementOrder _movementOrder;
    private MovementOrder _chargeToTargetOrder;

    public BehaviorProtectFlank(Formation formation)
      : base(formation)
    {
      this._protectFlankState = BehaviorProtectFlank.BehaviourState.HoldingFlank;
      this.behaviorSide = formation.AI.Side;
      this._mainFormation = formation.Team.Formations.FirstOrDefault<Formation>((Func<Formation, bool>) (f => f.AI.IsMainFormation));
      this.CalculateCurrentOrder();
      this.CurrentOrder = this._movementOrder;
    }

    protected override void CalculateCurrentOrder()
    {
      if (this._mainFormation == null || this.formation.AI.IsMainFormation || this.formation.QuerySystem.ClosestEnemyFormation == null)
      {
        this.CurrentOrder = MovementOrder.MovementOrderStop;
        this.CurrentFacingOrder = FacingOrder.FacingOrderLookAtEnemy;
      }
      else
      {
        if (this._protectFlankState != BehaviorProtectFlank.BehaviourState.HoldingFlank && this._protectFlankState != BehaviorProtectFlank.BehaviourState.Returning)
          return;
        Vec2 direction = this._mainFormation.Direction;
        Vec2 vec2_1 = (this.formation.QuerySystem.Team.MedianTargetFormationPosition.AsVec2 - this._mainFormation.QuerySystem.MedianPosition.AsVec2).Normalized();
        Vec2 vec2_2 = this.behaviorSide == FormationAI.BehaviorSide.Right || this.FlankSide == FormationAI.BehaviorSide.Right ? this._mainFormation.CurrentPosition + vec2_1.RightVec().Normalized() * (float) ((double) this._mainFormation.Width + (double) this.formation.Width + 10.0) - vec2_1 * (this._mainFormation.Depth + this.formation.Depth) : (this.behaviorSide == FormationAI.BehaviorSide.Left || this.FlankSide == FormationAI.BehaviorSide.Left ? this._mainFormation.CurrentPosition + vec2_1.LeftVec().Normalized() * (float) ((double) this._mainFormation.Width + (double) this.formation.Width + 10.0) - vec2_1 * (this._mainFormation.Depth + this.formation.Depth) : this._mainFormation.CurrentPosition + vec2_1 * (float) (((double) this._mainFormation.Depth + (double) this.formation.Depth) * 0.5 + 10.0));
        WorldPosition medianPosition = this._mainFormation.QuerySystem.MedianPosition;
        medianPosition.SetVec2(vec2_2);
        this._movementOrder = MovementOrder.MovementOrderMove(medianPosition);
        this.CurrentOrder = this._movementOrder;
        this.CurrentFacingOrder = FacingOrder.FacingOrderLookAtDirection(direction);
      }
    }

    private void CheckAndChangeState()
    {
      WorldPosition position = this._movementOrder.GetPosition(this.formation);
      switch (this._protectFlankState)
      {
        case BehaviorProtectFlank.BehaviourState.HoldingFlank:
          if (this.formation.QuerySystem.ClosestEnemyFormation == null)
            break;
          float num1 = (float) (50.0 + ((double) this.formation.Depth + (double) this.formation.QuerySystem.ClosestEnemyFormation.Formation.Depth) / 2.0);
          if ((double) this.formation.QuerySystem.ClosestEnemyFormation.MedianPosition.AsVec2.DistanceSquared(position.AsVec2) >= (double) num1 * (double) num1)
            break;
          this._chargeToTargetOrder = MovementOrder.MovementOrderChargeToTarget(this.formation.QuerySystem.ClosestEnemyFormation.Formation);
          this.CurrentOrder = this._chargeToTargetOrder;
          this._protectFlankState = BehaviorProtectFlank.BehaviourState.Charging;
          break;
        case BehaviorProtectFlank.BehaviourState.Charging:
          if (this.formation.QuerySystem.ClosestEnemyFormation == null)
          {
            this.CurrentOrder = this._movementOrder;
            this._protectFlankState = BehaviorProtectFlank.BehaviourState.Returning;
            break;
          }
          float num2 = (float) (60.0 + ((double) this.formation.Depth + (double) this.formation.QuerySystem.ClosestEnemyFormation.Formation.Depth) / 2.0);
          if ((double) this.formation.QuerySystem.AveragePosition.DistanceSquared(position.AsVec2) <= (double) num2 * (double) num2)
            break;
          this.CurrentOrder = this._movementOrder;
          this._protectFlankState = BehaviorProtectFlank.BehaviourState.Returning;
          break;
        case BehaviorProtectFlank.BehaviourState.Returning:
          if ((double) this.formation.QuerySystem.AveragePosition.DistanceSquared(position.AsVec2) >= 400.0)
            break;
          this._protectFlankState = BehaviorProtectFlank.BehaviourState.HoldingFlank;
          break;
      }
    }

    internal override void OnValidBehaviorSideSet()
    {
      base.OnValidBehaviorSideSet();
      this._mainFormation = this.formation.Team.Formations.FirstOrDefault<Formation>((Func<Formation, bool>) (f => f.AI.IsMainFormation));
    }

    protected internal override void TickOccasionally()
    {
      this.CheckAndChangeState();
      this.CalculateCurrentOrder();
      this.formation.MovementOrder = this.CurrentOrder;
      this.formation.FacingOrder = this.CurrentFacingOrder;
      if (this._protectFlankState == BehaviorProtectFlank.BehaviourState.HoldingFlank && this.formation.QuerySystem.ClosestSignificantlyLargeEnemyFormation != null && (double) this.formation.QuerySystem.AveragePosition.DistanceSquared(this.formation.QuerySystem.ClosestSignificantlyLargeEnemyFormation.MedianPosition.AsVec2) > 1600.0 && (double) this.formation.QuerySystem.UnderRangedAttackRatio > 0.200000002980232 - (this.formation.ArrangementOrder.OrderEnum == ArrangementOrder.ArrangementOrderEnum.Loose ? 0.100000001490116 : 0.0))
        this.formation.ArrangementOrder = ArrangementOrder.ArrangementOrderLoose;
      else
        this.formation.ArrangementOrder = ArrangementOrder.ArrangementOrderLine;
    }

    protected override void OnBehaviorActivatedAux()
    {
      this.CalculateCurrentOrder();
      this.formation.MovementOrder = this.CurrentOrder;
      this.formation.FacingOrder = this.CurrentFacingOrder;
      this.formation.ArrangementOrder = ArrangementOrder.ArrangementOrderLine;
      this.formation.FiringOrder = FiringOrder.FiringOrderFireAtWill;
      this.formation.FormOrder = FormOrder.FormOrderDeep;
      this.formation.WeaponUsageOrder = WeaponUsageOrder.WeaponUsageOrderUseAny;
    }

    public override TextObject GetBehaviorString()
    {
      TextObject text1 = GameTexts.FindText("str_formation_ai_side_strings", this.formation.AI.Side.ToString());
      MBTextManager.SetTextVariable("IS_GENERAL_SIDE", "0", false);
      MBTextManager.SetTextVariable("SIDE_STRING", text1, false);
      string text2;
      if (this._mainFormation != null)
        text2 = string.Join(" ", (object) GameTexts.FindText("str_formation_ai_side_strings", this._mainFormation.AI.Side.ToString()), (object) GameTexts.FindText("str_formation_class_string", this._mainFormation.PrimaryClass.GetName()));
      else
        text2 = "";
      MBTextManager.SetTextVariable("TARGET_FORMATION", text2, false);
      return base.GetBehaviorString();
    }

    protected override float GetAiWeight()
    {
      if (this._mainFormation == null || !this._mainFormation.AI.IsMainFormation)
        this._mainFormation = this.formation.Team.Formations.FirstOrDefault<Formation>((Func<Formation, bool>) (f => f.AI.IsMainFormation));
      return this._mainFormation == null || this.formation.AI.IsMainFormation ? 0.0f : 1.2f;
    }

    private enum BehaviourState
    {
      HoldingFlank,
      Charging,
      Returning,
    }
  }
}
