﻿// Decompiled with JetBrains decompiler
// Type: TaleWorlds.MountAndBlade.BehaviorRetreatToKeep
// Assembly: TaleWorlds.MountAndBlade, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3C0E3F42-756F-4407-B817-06BA412A78B0
// Assembly location: E:\SteamLibrary\steamapps\common\Mount & Blade II Bannerlord\bin\Win64_Shipping_Client\TaleWorlds.MountAndBlade.dll

namespace TaleWorlds.MountAndBlade
{
  public class BehaviorRetreatToKeep : BehaviorComponent
  {
    public BehaviorRetreatToKeep(Formation formation)
      : base(formation)
    {
      this.CurrentOrder = MovementOrder.MovementOrderRetreat;
      this.BehaviorCoherence = 0.0f;
    }

    protected internal override void TickOccasionally()
    {
      base.TickOccasionally();
      if (this.formation.AI.ActiveBehavior != this)
        return;
      this.formation.MovementOrder = this.CurrentOrder;
    }

    protected override void OnBehaviorActivatedAux()
    {
      this.formation.ArrangementOrder = ArrangementOrder.ArrangementOrderLoose;
      this.formation.FiringOrder = FiringOrder.FiringOrderFireAtWill;
      this.formation.FormOrder = FormOrder.FormOrderWider;
      this.formation.WeaponUsageOrder = WeaponUsageOrder.WeaponUsageOrderUseAny;
    }

    protected internal override float NavmeshlessTargetPositionPenalty => 1f;

    protected override float GetAiWeight() => 1f;
  }
}
