// Decompiled with JetBrains decompiler
// Type: TaleWorlds.MountAndBlade.BehaviorFlank
// Assembly: TaleWorlds.MountAndBlade, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3C0E3F42-756F-4407-B817-06BA412A78B0
// Assembly location: E:\SteamLibrary\steamapps\common\Mount & Blade II Bannerlord\bin\Win64_Shipping_Client\TaleWorlds.MountAndBlade.dll

using TaleWorlds.Core;
using TaleWorlds.Engine;
using TaleWorlds.Library;
using TaleWorlds.Localization;

namespace TaleWorlds.MountAndBlade
{
  public class BehaviorFlank : BehaviorComponent
  {
    public BehaviorFlank(Formation formation)
      : base(formation)
    {
      this.BehaviorCoherence = 0.5f;
      this.CalculateCurrentOrder();
    }

    protected override void CalculateCurrentOrder()
    {
      WorldPosition position = this.formation.AI.Side == FormationAI.BehaviorSide.Right ? this.formation.QuerySystem.Team.RightFlankEdgePosition : this.formation.QuerySystem.Team.LeftFlankEdgePosition;
      Vec2 direction = (position.AsVec2 - this.formation.QuerySystem.AveragePosition).Normalized();
      this.CurrentOrder = MovementOrder.MovementOrderMove(position);
      this.CurrentFacingOrder = FacingOrder.FacingOrderLookAtDirection(direction);
    }

    protected internal override void TickOccasionally()
    {
      this.CalculateCurrentOrder();
      this.formation.MovementOrder = this.CurrentOrder;
      this.formation.FacingOrder = this.CurrentFacingOrder;
    }

    public override TextObject GetBehaviorString()
    {
      MBTextManager.SetTextVariable("IS_GENERAL_SIDE", "0", false);
      MBTextManager.SetTextVariable("SIDE_STRING", GameTexts.FindText("str_formation_ai_side_strings", this.formation.AI.Side.ToString()), false);
      return base.GetBehaviorString();
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

    protected override float GetAiWeight()
    {
      FormationQuerySystem querySystem = this.formation.QuerySystem;
      if (querySystem.ClosestEnemyFormation == null || querySystem.ClosestEnemyFormation.ClosestEnemyFormation == querySystem)
        return 0.0f;
      Vec2 vec2 = (querySystem.ClosestEnemyFormation.MedianPosition.AsVec2 - querySystem.AveragePosition).Normalized();
      WorldPosition medianPosition = querySystem.ClosestEnemyFormation.ClosestEnemyFormation.MedianPosition;
      Vec2 asVec2_1 = medianPosition.AsVec2;
      medianPosition = querySystem.ClosestEnemyFormation.MedianPosition;
      Vec2 asVec2_2 = medianPosition.AsVec2;
      Vec2 v = (asVec2_1 - asVec2_2).Normalized();
      return (double) vec2.DotProduct(v) > -0.5 ? 0.0f : 1.2f;
    }
  }
}
