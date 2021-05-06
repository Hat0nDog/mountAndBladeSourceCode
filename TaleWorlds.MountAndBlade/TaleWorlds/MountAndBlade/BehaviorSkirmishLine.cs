// Decompiled with JetBrains decompiler
// Type: TaleWorlds.MountAndBlade.BehaviorSkirmishLine
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
  public class BehaviorSkirmishLine : BehaviorComponent
  {
    private Formation _mainFormation;

    public BehaviorSkirmishLine(Formation formation)
      : base(formation)
    {
      this.behaviorSide = FormationAI.BehaviorSide.BehaviorSideNotSet;
      this._mainFormation = formation.Team.Formations.FirstOrDefault<Formation>((Func<Formation, bool>) (f => f.AI.IsMainFormation));
      this.CalculateCurrentOrder();
    }

    protected override void CalculateCurrentOrder()
    {
      Vec2 vec2_1;
      WorldPosition medianPosition;
      Vec2 vec2_2;
      WorldPosition worldPosition;
      if (this.formation.QuerySystem.ClosestEnemyFormation == null || this._mainFormation == null)
      {
        vec2_1 = this.formation.Direction;
        medianPosition = this.formation.QuerySystem.MedianPosition;
        medianPosition.SetVec2(this.formation.QuerySystem.AveragePosition);
      }
      else
      {
        vec2_2 = this.formation.Direction;
        ref Vec2 local = ref vec2_2;
        worldPosition = this.formation.QuerySystem.ClosestEnemyFormation.MedianPosition;
        Vec2 asVec2_1 = worldPosition.AsVec2;
        worldPosition = this._mainFormation.QuerySystem.MedianPosition;
        Vec2 asVec2_2 = worldPosition.AsVec2;
        Vec2 v = (asVec2_1 - asVec2_2).Normalized();
        Vec2 vec2_3;
        if ((double) local.DotProduct(v) >= 0.5)
        {
          vec2_3 = this.formation.Direction;
        }
        else
        {
          worldPosition = this.formation.QuerySystem.ClosestEnemyFormation.MedianPosition;
          Vec2 asVec2_3 = worldPosition.AsVec2;
          worldPosition = this._mainFormation.QuerySystem.MedianPosition;
          Vec2 asVec2_4 = worldPosition.AsVec2;
          vec2_3 = asVec2_3 - asVec2_4;
        }
        vec2_1 = vec2_3.Normalized();
        worldPosition = this._mainFormation.OrderPosition;
        Vec2 asVec2_5 = worldPosition.AsVec2;
        worldPosition = this._mainFormation.QuerySystem.MedianPosition;
        Vec2 asVec2_6 = worldPosition.AsVec2;
        Vec2 vec2_4 = asVec2_5 - asVec2_6;
        float num1 = this._mainFormation.QuerySystem.MovementSpeed * 7f;
        float length = vec2_4.Length;
        if ((double) length > 0.0)
        {
          float num2 = num1 / length;
          if ((double) num2 < 1.0)
            vec2_4 *= num2;
        }
        medianPosition = this._mainFormation.QuerySystem.MedianPosition;
        medianPosition.SetVec2(medianPosition.AsVec2 + vec2_1 * 8f + vec2_4);
      }
      this.CurrentOrder = MovementOrder.MovementOrderMove(medianPosition);
      vec2_2 = this.CurrentFacingOrder.GetDirection(this.formation);
      if (vec2_2.IsValid && this.CurrentFacingOrder.OrderEnum != FacingOrder.FacingOrderEnum.LookAtEnemy)
      {
        if (this.formation.QuerySystem.ClosestSignificantlyLargeEnemyFormation == null)
          return;
        vec2_2 = this.formation.QuerySystem.AveragePosition;
        ref Vec2 local = ref vec2_2;
        worldPosition = this.formation.QuerySystem.ClosestSignificantlyLargeEnemyFormation.MedianPosition;
        Vec2 asVec2 = worldPosition.GetNavMeshVec3().AsVec2;
        if ((double) local.DistanceSquared(asVec2) < (double) this.formation.QuerySystem.MissileRange * (double) this.formation.QuerySystem.MissileRange)
        {
          if (this.formation.QuerySystem.ClosestSignificantlyLargeEnemyFormation.IsRangedCavalryFormation)
            return;
          vec2_2 = this.CurrentFacingOrder.GetDirection(this.formation);
          if ((double) vec2_2.DotProduct(vec2_1) > (double) MBMath.Lerp(0.5f, 1f, (float) (1.0 - (double) MBMath.ClampFloat(this.formation.Width, 1f, 20f) * 0.0500000007450581)))
            return;
        }
      }
      this.CurrentFacingOrder = FacingOrder.FacingOrderLookAtDirection(vec2_1);
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

    internal override void OnValidBehaviorSideSet()
    {
      base.OnValidBehaviorSideSet();
      this._mainFormation = this.formation.Team.Formations.FirstOrDefault<Formation>((Func<Formation, bool>) (f => f.AI.IsMainFormation));
    }

    protected internal override void TickOccasionally()
    {
      this.CalculateCurrentOrder();
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
      this.formation.FormOrder = FormOrder.FormOrderWider;
      this.formation.WeaponUsageOrder = WeaponUsageOrder.WeaponUsageOrderUseAny;
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
      float num2 = MBMath.Lerp(0.5f, 1.2f, (float) (((double) MBMath.ClampFloat(this.CurrentOrder.GetPosition(this.formation).AsVec2.Distance(querySystem.ClosestEnemyFormation.MedianPosition.AsVec2) / querySystem.ClosestEnemyFormation.MovementSpeedMaximum, 4f, 8f) - 4.0) / 4.0));
      double reliabilityFactor = (double) querySystem.MainFormationReliabilityFactor;
      return (float) (num1 * reliabilityFactor) * num2;
    }
  }
}
