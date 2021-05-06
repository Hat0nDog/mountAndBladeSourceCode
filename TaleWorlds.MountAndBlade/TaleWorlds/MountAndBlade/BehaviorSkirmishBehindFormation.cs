// Decompiled with JetBrains decompiler
// Type: TaleWorlds.MountAndBlade.BehaviorSkirmishBehindFormation
// Assembly: TaleWorlds.MountAndBlade, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3C0E3F42-756F-4407-B817-06BA412A78B0
// Assembly location: E:\SteamLibrary\steamapps\common\Mount & Blade II Bannerlord\bin\Win64_Shipping_Client\TaleWorlds.MountAndBlade.dll

using TaleWorlds.Core;
using TaleWorlds.Engine;
using TaleWorlds.Library;
using TaleWorlds.Localization;

namespace TaleWorlds.MountAndBlade
{
  public class BehaviorSkirmishBehindFormation : BehaviorComponent
  {
    public Formation ReferenceFormation;
    private bool _isFireAtWill = true;

    public BehaviorSkirmishBehindFormation(Formation formation)
      : base(formation)
    {
      this.behaviorSide = formation.AI.Side;
      this.CalculateCurrentOrder();
    }

    protected override void CalculateCurrentOrder()
    {
      Vec2 vec2_1;
      Vec2 vec2_2;
      if (this.formation.QuerySystem.ClosestEnemyFormation != null)
      {
        vec2_1 = this.formation.Direction;
        vec2_2 = ((double) vec2_1.DotProduct((this.formation.QuerySystem.ClosestEnemyFormation.MedianPosition.AsVec2 - this.formation.QuerySystem.AveragePosition).Normalized()) > 0.5 ? this.formation.QuerySystem.ClosestEnemyFormation.MedianPosition.AsVec2 - this.formation.QuerySystem.AveragePosition : this.formation.Direction).Normalized();
      }
      else
        vec2_2 = this.formation.Direction;
      WorldPosition medianPosition;
      if (this.ReferenceFormation == null)
      {
        medianPosition = this.formation.QuerySystem.MedianPosition;
        medianPosition.SetVec2(this.formation.QuerySystem.AveragePosition);
      }
      else
      {
        medianPosition = this.ReferenceFormation.QuerySystem.MedianPosition;
        medianPosition.SetVec2(medianPosition.AsVec2 - vec2_2 * (float) (((double) this.ReferenceFormation.Depth + (double) this.formation.Depth) * 0.5));
      }
      WorldPosition worldPosition = this.CurrentOrder.GetPosition(this.formation);
      if (worldPosition.IsValid && this.formation.QuerySystem.ClosestSignificantlyLargeEnemyFormation.IsRangedCavalryFormation)
      {
        vec2_1 = this.formation.QuerySystem.AveragePosition;
        ref Vec2 local = ref vec2_1;
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
      vec2_1 = this.CurrentFacingOrder.GetDirection(this.formation);
      if (vec2_1.IsValid && this.CurrentFacingOrder.OrderEnum != FacingOrder.FacingOrderEnum.LookAtEnemy)
      {
        vec2_1 = this.formation.QuerySystem.AveragePosition;
        ref Vec2 local = ref vec2_1;
        worldPosition = this.formation.QuerySystem.ClosestSignificantlyLargeEnemyFormation.MedianPosition;
        Vec2 asVec2 = worldPosition.GetNavMeshVec3().AsVec2;
        if ((double) local.DistanceSquared(asVec2) < (double) this.formation.QuerySystem.MissileRange * (double) this.formation.QuerySystem.MissileRange)
        {
          if (this.formation.QuerySystem.ClosestSignificantlyLargeEnemyFormation.IsRangedCavalryFormation)
            return;
          vec2_1 = this.CurrentFacingOrder.GetDirection(this.formation);
          if ((double) vec2_1.DotProduct(vec2_2) > (double) MBMath.Lerp(0.5f, 1f, (float) (1.0 - (double) MBMath.ClampFloat(this.formation.Width, 1f, 20f) * 0.0500000007450581)))
            return;
        }
      }
      this.CurrentFacingOrder = FacingOrder.FacingOrderLookAtDirection(vec2_2);
    }

    protected internal override void TickOccasionally()
    {
      this.CalculateCurrentOrder();
      int num1;
      if (this.formation.QuerySystem.ClosestEnemyFormation != null)
      {
        Vec2 asVec2_1 = this.ReferenceFormation.QuerySystem.MedianPosition.AsVec2;
        ref Vec2 local1 = ref asVec2_1;
        WorldPosition worldPosition = this.formation.QuerySystem.ClosestEnemyFormation.MedianPosition;
        Vec2 asVec2_2 = worldPosition.AsVec2;
        double num2 = (double) local1.DistanceSquared(asVec2_2);
        Vec2 averagePosition1 = this.formation.QuerySystem.AveragePosition;
        ref Vec2 local2 = ref averagePosition1;
        worldPosition = this.formation.QuerySystem.ClosestEnemyFormation.MedianPosition;
        Vec2 asVec2_3 = worldPosition.AsVec2;
        double num3 = (double) local2.DistanceSquared(asVec2_3);
        if (num2 > num3)
        {
          Vec2 averagePosition2 = this.formation.QuerySystem.AveragePosition;
          ref Vec2 local3 = ref averagePosition2;
          worldPosition = this.CurrentOrder.GetPosition(this.formation);
          Vec2 asVec2_4 = worldPosition.AsVec2;
          num1 = (double) local3.DistanceSquared(asVec2_4) <= ((double) this.ReferenceFormation.Depth + (double) this.formation.Depth) * ((double) this.ReferenceFormation.Depth + (double) this.formation.Depth) * 0.25 ? 1 : 0;
          goto label_4;
        }
      }
      num1 = 1;
label_4:
      bool flag = num1 != 0;
      if (flag != this._isFireAtWill)
      {
        this._isFireAtWill = flag;
        if (this._isFireAtWill)
          this.formation.FiringOrder = FiringOrder.FiringOrderFireAtWill;
        else
          this.formation.FiringOrder = FiringOrder.FiringOrderHoldYourFire;
      }
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

    public override TextObject GetBehaviorString()
    {
      string text;
      if (this.ReferenceFormation != null)
        text = string.Join(" ", (object) GameTexts.FindText("str_formation_ai_side_strings", this.ReferenceFormation.AI.Side.ToString()), (object) GameTexts.FindText("str_formation_class_string", this.ReferenceFormation.PrimaryClass.GetName()));
      else
        text = "";
      MBTextManager.SetTextVariable("TARGET_FORMATION", text, false);
      return base.GetBehaviorString();
    }

    protected override float GetAiWeight() => 10f;
  }
}
