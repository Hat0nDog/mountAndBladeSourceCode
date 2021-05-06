// Decompiled with JetBrains decompiler
// Type: TaleWorlds.MountAndBlade.BehaviorSallyOut
// Assembly: TaleWorlds.MountAndBlade, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3C0E3F42-756F-4407-B817-06BA412A78B0
// Assembly location: E:\SteamLibrary\steamapps\common\Mount & Blade II Bannerlord\bin\Win64_Shipping_Client\TaleWorlds.MountAndBlade.dll

using System;
using System.Linq;
using TaleWorlds.Engine;

namespace TaleWorlds.MountAndBlade
{
  public class BehaviorSallyOut : BehaviorComponent
  {
    private readonly TeamAISiegeDefender _teamAISiegeDefender;
    private MovementOrder _gatherOrder;
    private MovementOrder _attackOrder;
    private TacticalPosition _gatheringTacticalPos;

    private bool _calculateAreGatesOutsideOpen
    {
      get
      {
        if (this._teamAISiegeDefender.OuterGate != null && !this._teamAISiegeDefender.OuterGate.IsGateOpen)
          return false;
        return this._teamAISiegeDefender.InnerGate == null || this._teamAISiegeDefender.InnerGate.IsGateOpen;
      }
    }

    private bool _calculateShouldStartAttacking => this._calculateAreGatesOutsideOpen || !TeamAISiegeComponent.IsFormationInsideCastle(this.formation, true);

    public BehaviorSallyOut(Formation formation)
      : base(formation)
    {
      this._teamAISiegeDefender = formation.Team.TeamAI as TeamAISiegeDefender;
      this.behaviorSide = formation.AI.Side;
      this.ResetOrderPositions();
    }

    protected override void CalculateCurrentOrder()
    {
      base.CalculateCurrentOrder();
      this.CurrentOrder = this._calculateShouldStartAttacking ? this._attackOrder : this._gatherOrder;
    }

    private void ResetOrderPositions()
    {
      SiegeLane siegeLane = TeamAISiegeComponent.SiegeLanes.FirstOrDefault<SiegeLane>((Func<SiegeLane, bool>) (sl => sl.LaneSide == FormationAI.BehaviorSide.Middle));
      WorldFrame worldFrame = (siegeLane != null ? siegeLane.DefensePoints.FirstOrDefault<ICastleKeyPosition>()?.DefenseWaitFrame : new WorldFrame?()) ?? WorldFrame.Invalid;
      this._gatheringTacticalPos = siegeLane != null ? siegeLane.DefensePoints.FirstOrDefault<ICastleKeyPosition>()?.WaitPosition : (TacticalPosition) null;
      if (this._gatheringTacticalPos != null)
        this._gatherOrder = MovementOrder.MovementOrderMove(this._gatheringTacticalPos.Position);
      else if (worldFrame.Origin.IsValid)
      {
        double num = (double) worldFrame.Rotation.f.Normalize();
        this._gatherOrder = MovementOrder.MovementOrderMove(worldFrame.Origin);
      }
      else
        this._gatherOrder = MovementOrder.MovementOrderMove(this.formation.QuerySystem.MedianPosition);
      this._attackOrder = MovementOrder.MovementOrderCharge;
      this.CurrentOrder = this._calculateShouldStartAttacking ? this._attackOrder : this._gatherOrder;
    }

    protected internal override void TickOccasionally()
    {
      base.TickOccasionally();
      this.CalculateCurrentOrder();
      this.formation.MovementOrder = this.CurrentOrder;
      if (this._calculateAreGatesOutsideOpen)
        return;
      CastleGate castleGate = this._teamAISiegeDefender.InnerGate == null || this._teamAISiegeDefender.InnerGate.IsGateOpen ? this._teamAISiegeDefender.OuterGate : this._teamAISiegeDefender.InnerGate;
      if (this.formation.IsUsingMachine((UsableMachine) castleGate))
        return;
      this.formation.StartUsingMachine((UsableMachine) castleGate);
    }

    protected override void OnBehaviorActivatedAux()
    {
      this.behaviorSide = this.formation.AI.Side;
      this.ResetOrderPositions();
      this.formation.MovementOrder = this.CurrentOrder;
      this.formation.FacingOrder = FacingOrder.FacingOrderLookAtEnemy;
      this.formation.ArrangementOrder = ArrangementOrder.ArrangementOrderLine;
      this.formation.FiringOrder = FiringOrder.FiringOrderFireAtWill;
      this.formation.FormOrder = FormOrder.FormOrderWide;
      this.formation.WeaponUsageOrder = WeaponUsageOrder.WeaponUsageOrderUseAny;
    }

    protected internal override float NavmeshlessTargetPositionPenalty => 1f;

    protected override float GetAiWeight() => 10f;
  }
}
