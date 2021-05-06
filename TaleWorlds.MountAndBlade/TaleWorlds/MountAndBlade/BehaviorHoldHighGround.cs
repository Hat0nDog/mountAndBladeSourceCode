// Decompiled with JetBrains decompiler
// Type: TaleWorlds.MountAndBlade.BehaviorHoldHighGround
// Assembly: TaleWorlds.MountAndBlade, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3C0E3F42-756F-4407-B817-06BA412A78B0
// Assembly location: E:\SteamLibrary\steamapps\common\Mount & Blade II Bannerlord\bin\Win64_Shipping_Client\TaleWorlds.MountAndBlade.dll

using TaleWorlds.Engine;
using TaleWorlds.Library;

namespace TaleWorlds.MountAndBlade
{
  public class BehaviorHoldHighGround : BehaviorComponent
  {
    public BehaviorHoldHighGround(Formation formation)
      : base(formation)
    {
      this.CalculateCurrentOrder();
    }

    protected override void CalculateCurrentOrder()
    {
      WorldPosition medianPosition;
      Vec2 direction;
      if (this.formation.QuerySystem.ClosestEnemyFormation != null)
      {
        medianPosition = this.formation.QuerySystem.MedianPosition;
        medianPosition.SetVec2((double) this.formation.QuerySystem.AveragePosition.DistanceSquared(this.formation.QuerySystem.ClosestEnemyFormation.MedianPosition.AsVec2) > 400.0 ? this.formation.QuerySystem.HighGroundCloseToForeseenBattleGround : this.formation.QuerySystem.AveragePosition);
        Vec2 vec2_1;
        if ((double) this.formation.QuerySystem.AveragePosition.DistanceSquared(this.formation.QuerySystem.HighGroundCloseToForeseenBattleGround) <= 25.0)
        {
          Vec2 vec2_2 = this.formation.Direction;
          vec2_2 = (double) vec2_2.DotProduct((this.formation.QuerySystem.ClosestEnemyFormation.MedianPosition.AsVec2 - this.formation.QuerySystem.AveragePosition).Normalized()) < 0.5 ? this.formation.QuerySystem.ClosestEnemyFormation.MedianPosition.AsVec2 - this.formation.QuerySystem.AveragePosition : this.formation.Direction;
          vec2_1 = vec2_2.Normalized();
        }
        else
          vec2_1 = (this.formation.QuerySystem.Team.MedianTargetFormationPosition.AsVec2 - medianPosition.AsVec2).Normalized();
        direction = vec2_1;
      }
      else
      {
        direction = this.formation.Direction;
        medianPosition = this.formation.QuerySystem.MedianPosition;
        medianPosition.SetVec2(this.formation.QuerySystem.AveragePosition);
      }
      this.CurrentOrder = MovementOrder.MovementOrderMove(medianPosition);
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
      this.CalculateCurrentOrder();
      this.formation.MovementOrder = this.CurrentOrder;
      this.formation.FacingOrder = this.CurrentFacingOrder;
      this.formation.ArrangementOrder = ArrangementOrder.ArrangementOrderLine;
      this.formation.FiringOrder = FiringOrder.FiringOrderFireAtWill;
      this.formation.FormOrder = FormOrder.FormOrderDeep;
      this.formation.WeaponUsageOrder = WeaponUsageOrder.WeaponUsageOrderUseAny;
    }

    protected override float GetAiWeight() => this.formation.QuerySystem.ClosestEnemyFormation == null ? 0.0f : 1f;
  }
}
