// Decompiled with JetBrains decompiler
// Type: TaleWorlds.MountAndBlade.BehaviorStop
// Assembly: TaleWorlds.MountAndBlade, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3C0E3F42-756F-4407-B817-06BA412A78B0
// Assembly location: E:\SteamLibrary\steamapps\common\Mount & Blade II Bannerlord\bin\Win64_Shipping_Client\TaleWorlds.MountAndBlade.dll

namespace TaleWorlds.MountAndBlade
{
  public class BehaviorStop : BehaviorComponent
  {
    public BehaviorStop(Formation formation)
      : base(formation)
    {
      this.CurrentOrder = MovementOrder.MovementOrderStop;
      this.BehaviorCoherence = 0.0f;
    }

    protected internal override void TickOccasionally() => this.formation.MovementOrder = this.CurrentOrder;

    protected override void OnBehaviorActivatedAux()
    {
      this.formation.ArrangementOrder = this.formation.QuerySystem.HasShield ? ArrangementOrder.ArrangementOrderShieldWall : ArrangementOrder.ArrangementOrderLine;
      this.formation.FacingOrder = FacingOrder.FacingOrderLookAtEnemy;
      this.formation.FiringOrder = FiringOrder.FiringOrderFireAtWill;
      this.formation.FormOrder = FormOrder.FormOrderDeep;
      this.formation.WeaponUsageOrder = WeaponUsageOrder.WeaponUsageOrderUseAny;
      this._lastPlayerInformTime = MBCommon.GetTime(MBCommon.TimeType.Mission);
    }

    protected internal override float NavmeshlessTargetPositionPenalty => 1f;

    protected override float GetAiWeight() => 0.01f;
  }
}
