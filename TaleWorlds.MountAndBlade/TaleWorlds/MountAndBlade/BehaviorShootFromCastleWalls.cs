// Decompiled with JetBrains decompiler
// Type: TaleWorlds.MountAndBlade.BehaviorShootFromCastleWalls
// Assembly: TaleWorlds.MountAndBlade, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3C0E3F42-756F-4407-B817-06BA412A78B0
// Assembly location: E:\SteamLibrary\steamapps\common\Mount & Blade II Bannerlord\bin\Win64_Shipping_Client\TaleWorlds.MountAndBlade.dll

using TaleWorlds.DotNet;
using TaleWorlds.Engine;

namespace TaleWorlds.MountAndBlade
{
  public class BehaviorShootFromCastleWalls : BehaviorComponent
  {
    private GameEntity _archerPosition;
    private TacticalPosition _tacticalArcherPosition;

    public GameEntity ArcherPosition
    {
      get => this._archerPosition;
      set
      {
        if (!((NativeObject) this._archerPosition != (NativeObject) value))
          return;
        this.OnArcherPositionSet(value);
      }
    }

    public BehaviorShootFromCastleWalls(Formation formation)
      : base(formation)
    {
      this.OnArcherPositionSet(this._archerPosition);
      this.BehaviorCoherence = 0.0f;
    }

    private void OnArcherPositionSet(GameEntity value)
    {
      this._archerPosition = value;
      if ((NativeObject) this._archerPosition != (NativeObject) null)
      {
        this._tacticalArcherPosition = this._archerPosition.GetFirstScriptOfType<TacticalPosition>();
        if (this._tacticalArcherPosition != null)
        {
          this.CurrentOrder = MovementOrder.MovementOrderMove(this._tacticalArcherPosition.Position);
          this.CurrentFacingOrder = FacingOrder.FacingOrderLookAtDirection(this._tacticalArcherPosition.Direction);
        }
        else
        {
          this.CurrentOrder = MovementOrder.MovementOrderMove(this._archerPosition.GlobalPosition.ToWorldPosition());
          this.CurrentFacingOrder = FacingOrder.FacingOrderLookAtEnemy;
        }
      }
      else
      {
        this._tacticalArcherPosition = (TacticalPosition) null;
        WorldPosition medianPosition = this.formation.QuerySystem.MedianPosition;
        medianPosition.SetVec2(this.formation.CurrentPosition);
        this.CurrentOrder = MovementOrder.MovementOrderMove(medianPosition);
        this.CurrentFacingOrder = FacingOrder.FacingOrderLookAtEnemy;
      }
    }

    protected internal override void TickOccasionally()
    {
      this.formation.MovementOrder = this.CurrentOrder;
      this.formation.FacingOrder = this.CurrentFacingOrder;
      if (this._tacticalArcherPosition == null)
        return;
      this.formation.FormOrder = FormOrder.FormOrderCustom(this._tacticalArcherPosition.Width);
    }

    protected override void OnBehaviorActivatedAux()
    {
      this.formation.MovementOrder = this.CurrentOrder;
      this.formation.FacingOrder = this.CurrentFacingOrder;
      this.formation.ArrangementOrder = ArrangementOrder.ArrangementOrderScatter;
      this.formation.FiringOrder = FiringOrder.FiringOrderFireAtWill;
      this.formation.FormOrder = FormOrder.FormOrderWide;
      this.formation.WeaponUsageOrder = WeaponUsageOrder.WeaponUsageOrderUseAny;
    }

    protected internal override float NavmeshlessTargetPositionPenalty => 1f;

    protected override float GetAiWeight() => (float) (10.0 * ((double) this.formation.QuerySystem.RangedCavalryUnitRatio + (double) this.formation.QuerySystem.RangedUnitRatio));
  }
}
