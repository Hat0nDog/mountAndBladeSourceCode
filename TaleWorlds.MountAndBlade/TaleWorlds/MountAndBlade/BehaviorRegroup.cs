// Decompiled with JetBrains decompiler
// Type: TaleWorlds.MountAndBlade.BehaviorRegroup
// Assembly: TaleWorlds.MountAndBlade, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3C0E3F42-756F-4407-B817-06BA412A78B0
// Assembly location: E:\SteamLibrary\steamapps\common\Mount & Blade II Bannerlord\bin\Win64_Shipping_Client\TaleWorlds.MountAndBlade.dll

using TaleWorlds.Engine;
using TaleWorlds.Library;

namespace TaleWorlds.MountAndBlade
{
  public class BehaviorRegroup : BehaviorComponent
  {
    public BehaviorRegroup(Formation formation)
      : base(formation)
    {
      this.BehaviorCoherence = 1f;
      this.CalculateCurrentOrder();
    }

    protected override void CalculateCurrentOrder()
    {
      Vec2 direction = this.formation.QuerySystem.ClosestEnemyFormation == null ? this.formation.Direction : (this.formation.QuerySystem.ClosestEnemyFormation.MedianPosition.AsVec2 - this.formation.QuerySystem.AveragePosition).Normalized();
      WorldPosition medianPosition = this.formation.QuerySystem.MedianPosition;
      medianPosition.SetVec2(this.formation.QuerySystem.AveragePosition);
      this.CurrentOrder = MovementOrder.MovementOrderMove(medianPosition);
      this.CurrentFacingOrder = FacingOrder.FacingOrderLookAtDirection(direction);
    }

    protected internal override void TickOccasionally()
    {
      this.CalculateCurrentOrder();
      this.formation.MovementOrder = this.CurrentOrder;
      this.formation.FacingOrder = this.CurrentFacingOrder;
    }

    protected override float GetAiWeight()
    {
      FormationQuerySystem querySystem = this.formation.QuerySystem;
      return this.formation.AI.ActiveBehavior == null ? 0.0f : MBMath.Lerp(0.1f, 1.2f, MBMath.ClampFloat((float) ((double) this.formation.AI.ActiveBehavior.BehaviorCoherence * ((double) querySystem.FormationIntegrityData.DeviationOfPositionsExcludeFarAgents + 1.0) / ((double) querySystem.IdealAverageDisplacement + 1.0)), 0.0f, 3f) / 3f);
    }
  }
}
