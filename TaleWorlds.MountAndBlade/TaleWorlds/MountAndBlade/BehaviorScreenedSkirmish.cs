// Decompiled with JetBrains decompiler
// Type: TaleWorlds.MountAndBlade.BehaviorScreenedSkirmish
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
  public class BehaviorScreenedSkirmish : BehaviorComponent
  {
    private Formation _mainFormation;
    private bool _isFireAtWill = true;

    public BehaviorScreenedSkirmish(Formation formation)
      : base(formation)
    {
      this.behaviorSide = formation.AI.Side;
      this._mainFormation = formation.Team.Formations.FirstOrDefault<Formation>((Func<Formation, bool>) (f => f.AI.IsMainFormation));
      this.CalculateCurrentOrder();
    }

    protected override void CalculateCurrentOrder()
    {
      Vec2 vec2_1;
      if (this.formation.QuerySystem.ClosestSignificantlyLargeEnemyFormation != null && this._mainFormation != null)
      {
        Vec2 vec2_2 = (this.formation.QuerySystem.ClosestSignificantlyLargeEnemyFormation.MedianPosition.AsVec2 - this.formation.QuerySystem.AveragePosition).Normalized();
        Vec2 v = (this._mainFormation.QuerySystem.MedianPosition.AsVec2 - this.formation.QuerySystem.AveragePosition).Normalized();
        vec2_1 = (double) vec2_2.DotProduct(v) <= 0.5 ? vec2_2 : this._mainFormation.FacingOrder.GetDirection(this._mainFormation);
      }
      else
        vec2_1 = this.formation.Direction;
      WorldPosition medianPosition;
      if (this._mainFormation == null)
      {
        medianPosition = this.formation.QuerySystem.MedianPosition;
        medianPosition.SetVec2(this.formation.QuerySystem.AveragePosition);
      }
      else
      {
        medianPosition = this._mainFormation.QuerySystem.MedianPosition;
        medianPosition.SetVec2(medianPosition.AsVec2 - vec2_1 * (float) (((double) this._mainFormation.Depth + (double) this.formation.Depth) * 0.5));
      }
      WorldPosition worldPosition = this.CurrentOrder.GetPosition(this.formation);
      if (worldPosition.IsValid && this.formation.QuerySystem.ClosestSignificantlyLargeEnemyFormation.IsRangedCavalryFormation)
      {
        Vec2 averagePosition = this.formation.QuerySystem.AveragePosition;
        ref Vec2 local = ref averagePosition;
        worldPosition = this.formation.QuerySystem.ClosestSignificantlyLargeEnemyFormation.MedianPosition;
        Vec2 asVec2 = worldPosition.GetNavMeshVec3().AsVec2;
        if ((double) local.DistanceSquared(asVec2) < (double) this.formation.QuerySystem.ClosestSignificantlyLargeEnemyFormation.MissileRange * (double) this.formation.QuerySystem.ClosestSignificantlyLargeEnemyFormation.MissileRange)
        {
          worldPosition = this.CurrentOrder.GetPosition(this.formation);
          if ((double) worldPosition.GetNavMeshVec3().DistanceSquared(medianPosition.GetNavMeshVec3()) < (double) this.formation.Depth * (double) this.formation.Depth)
            goto label_10;
        }
      }
      this.CurrentOrder = MovementOrder.MovementOrderMove(medianPosition);
label_10:
      Vec2 vec2_3 = this.CurrentFacingOrder.GetDirection(this.formation);
      if (vec2_3.IsValid && this.CurrentFacingOrder.OrderEnum != FacingOrder.FacingOrderEnum.LookAtEnemy)
      {
        vec2_3 = this.formation.QuerySystem.AveragePosition;
        ref Vec2 local = ref vec2_3;
        worldPosition = this.formation.QuerySystem.ClosestSignificantlyLargeEnemyFormation.MedianPosition;
        Vec2 asVec2 = worldPosition.GetNavMeshVec3().AsVec2;
        if ((double) local.DistanceSquared(asVec2) < (double) this.formation.QuerySystem.MissileRange * (double) this.formation.QuerySystem.MissileRange)
        {
          if (this.formation.QuerySystem.ClosestSignificantlyLargeEnemyFormation.IsRangedCavalryFormation)
            return;
          vec2_3 = this.CurrentFacingOrder.GetDirection(this.formation);
          if ((double) vec2_3.DotProduct(vec2_1) > (double) MBMath.Lerp(0.5f, 1f, (float) (1.0 - (double) MBMath.ClampFloat(this.formation.Width, 1f, 20f) * 0.0500000007450581)))
            return;
        }
      }
      this.CurrentFacingOrder = FacingOrder.FacingOrderLookAtDirection(vec2_1);
    }

    protected internal override void TickOccasionally()
    {
      this.CalculateCurrentOrder();
      WorldPosition position = this.CurrentOrder.GetPosition(this.formation);
      int num1;
      if (this.formation.QuerySystem.ClosestEnemyFormation != null)
      {
        double num2 = (double) this._mainFormation.QuerySystem.MedianPosition.AsVec2.DistanceSquared(this.formation.QuerySystem.ClosestEnemyFormation.MedianPosition.AsVec2);
        Vec2 averagePosition = this.formation.QuerySystem.AveragePosition;
        double num3 = (double) averagePosition.DistanceSquared(this.formation.QuerySystem.ClosestEnemyFormation.MedianPosition.AsVec2);
        if (num2 > num3)
        {
          averagePosition = this.formation.QuerySystem.AveragePosition;
          num1 = (double) averagePosition.DistanceSquared(position.AsVec2) <= ((double) this._mainFormation.Depth + (double) this.formation.Depth) * ((double) this._mainFormation.Depth + (double) this.formation.Depth) * 0.25 ? 1 : 0;
          goto label_4;
        }
      }
      num1 = 1;
label_4:
      bool flag = num1 != 0;
      if (flag != this._isFireAtWill)
      {
        this._isFireAtWill = flag;
        this.formation.FiringOrder = this._isFireAtWill ? FiringOrder.FiringOrderFireAtWill : FiringOrder.FiringOrderHoldYourFire;
      }
      if (this._mainFormation != null && (double) Math.Abs(this._mainFormation.Width - this.formation.Width) > 10.0)
        this.formation.FormOrder = FormOrder.FormOrderCustom(this._mainFormation.Width);
      this.formation.MovementOrder = this.CurrentOrder;
      this.formation.FacingOrder = this.CurrentFacingOrder;
    }

    protected override void OnBehaviorActivatedAux()
    {
      this.CalculateCurrentOrder();
      this.formation.MovementOrder = this.CurrentOrder;
      this.formation.FacingOrder = this.CurrentFacingOrder;
      this.formation.ArrangementOrder = ArrangementOrder.ArrangementOrderLine;
      this.formation.FiringOrder = FiringOrder.FiringOrderFireAtWill;
      this.formation.FormOrder = FormOrder.FormOrderWide;
      this.formation.WeaponUsageOrder = WeaponUsageOrder.WeaponUsageOrderUseAny;
    }

    public override TextObject GetBehaviorString()
    {
      string text;
      if (this._mainFormation != null)
        text = string.Join(" ", (object) GameTexts.FindText("str_formation_ai_side_strings", this._mainFormation.AI.Side.ToString()), (object) GameTexts.FindText("str_formation_class_string", this._mainFormation.PrimaryClass.GetName()));
      else
        text = "";
      MBTextManager.SetTextVariable("TARGET_FORMATION", text, false);
      return base.GetBehaviorString();
    }

    protected override float GetAiWeight()
    {
      if (this._mainFormation == null || !this._mainFormation.AI.IsMainFormation)
        this._mainFormation = this.formation.Team.Formations.FirstOrDefault<Formation>((Func<Formation, bool>) (f => f.AI.IsMainFormation));
      if (this.behaviorSide != this.formation.AI.Side)
        this.behaviorSide = this.formation.AI.Side;
      if (this._mainFormation == null || this.formation.AI.IsMainFormation || this.formation.QuerySystem.ClosestEnemyFormation == null)
        return 0.0f;
      FormationQuerySystem querySystem = this.formation.QuerySystem;
      double num1 = (double) MBMath.Lerp(0.1f, 1f, MBMath.ClampFloat(querySystem.RangedUnitRatio + querySystem.RangedCavalryUnitRatio, 0.0f, 0.5f) * 2f);
      Vec2 vec2 = this._mainFormation.Direction.Normalized();
      ref Vec2 local1 = ref vec2;
      Vec2 asVec2_1 = this.formation.QuerySystem.ClosestEnemyFormation.MedianPosition.AsVec2;
      WorldPosition worldPosition = this._mainFormation.QuerySystem.MedianPosition;
      Vec2 asVec2_2 = worldPosition.AsVec2;
      Vec2 v = (asVec2_1 - asVec2_2).Normalized();
      float num2 = MBMath.LinearExtrapolation(0.5f, 1.1f, (float) (((double) local1.DotProduct(v) + 1.0) / 2.0));
      worldPosition = this.CurrentOrder.GetPosition(this.formation);
      Vec2 asVec2_3 = worldPosition.AsVec2;
      ref Vec2 local2 = ref asVec2_3;
      worldPosition = querySystem.ClosestEnemyFormation.MedianPosition;
      Vec2 asVec2_4 = worldPosition.AsVec2;
      float num3 = MBMath.Lerp(0.5f, 1.2f, (float) ((8.0 - (double) MBMath.ClampFloat(local2.Distance(asVec2_4) / querySystem.ClosestEnemyFormation.MovementSpeedMaximum, 4f, 8f)) / 4.0));
      double reliabilityFactor = (double) this.formation.QuerySystem.MainFormationReliabilityFactor;
      return (float) (num1 * reliabilityFactor) * num2 * num3;
    }
  }
}
