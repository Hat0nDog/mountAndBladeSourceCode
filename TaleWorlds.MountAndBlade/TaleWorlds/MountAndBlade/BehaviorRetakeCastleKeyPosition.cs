// Decompiled with JetBrains decompiler
// Type: TaleWorlds.MountAndBlade.BehaviorRetakeCastleKeyPosition
// Assembly: TaleWorlds.MountAndBlade, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3C0E3F42-756F-4407-B817-06BA412A78B0
// Assembly location: E:\SteamLibrary\steamapps\common\Mount & Blade II Bannerlord\bin\Win64_Shipping_Client\TaleWorlds.MountAndBlade.dll

using System;
using System.Collections.Generic;
using System.Linq;
using TaleWorlds.Engine;
using TaleWorlds.Library;

namespace TaleWorlds.MountAndBlade
{
  public class BehaviorRetakeCastleKeyPosition : BehaviorComponent
  {
    private TeamAISiegeDefender _teamAISiegeDefender;
    private BehaviorRetakeCastleKeyPosition.BehaviourState _behaviourState;
    private MovementOrder _gatherOrder;
    private MovementOrder _attackOrder;
    private FacingOrder _gatheringFacingOrder;
    private FacingOrder _attackFacingOrder;
    private TacticalPosition _gatheringTacticalPos;
    private FormationAI.BehaviorSide _gatheringSide;

    public BehaviorRetakeCastleKeyPosition(Formation formation)
      : base(formation)
    {
      this._teamAISiegeDefender = formation.Team.TeamAI as TeamAISiegeDefender;
      this._behaviourState = BehaviorRetakeCastleKeyPosition.BehaviourState.UnSet;
      this.behaviorSide = formation.AI.Side;
      this.ResetOrderPositions();
    }

    protected override void CalculateCurrentOrder()
    {
      base.CalculateCurrentOrder();
      this.CurrentOrder = this._behaviourState == BehaviorRetakeCastleKeyPosition.BehaviourState.Attacking ? this._attackOrder : this._gatherOrder;
    }

    private FormationAI.BehaviorSide DetermineGatheringSide()
    {
      IEnumerable<SiegeLane> source = TeamAISiegeComponent.SiegeLanes.Where<SiegeLane>((Func<SiegeLane, bool>) (sl => sl.LaneSide != this.behaviorSide && sl.LaneState != SiegeLane.LaneStateEnum.Conceited));
      if (!source.Any<SiegeLane>())
        return this.behaviorSide;
      int nearestSafeSideDistance = source.Min<SiegeLane>((Func<SiegeLane, int>) (pgl => SiegeQuerySystem.SideDistance(1 << (int) (this.behaviorSide & (FormationAI.BehaviorSide) 31), 1 << (int) (pgl.LaneSide & (FormationAI.BehaviorSide) 31))));
      return source.Where<SiegeLane>((Func<SiegeLane, bool>) (pgl => SiegeQuerySystem.SideDistance(1 << (int) (this.behaviorSide & (FormationAI.BehaviorSide) 31), 1 << (int) (pgl.LaneSide & (FormationAI.BehaviorSide) 31)) == nearestSafeSideDistance)).MinBy<SiegeLane, float>((Func<SiegeLane, float>) (pgl => pgl.DefenderOrigin.GetGroundVec3().DistanceSquared(this.formation.QuerySystem.MedianPosition.GetGroundVec3()))).LaneSide;
    }

    private void ConfirmGatheringSide()
    {
      SiegeLane siegeLane = TeamAISiegeComponent.SiegeLanes.FirstOrDefault<SiegeLane>((Func<SiegeLane, bool>) (sl => sl.LaneSide == this._gatheringSide));
      if (siegeLane != null && siegeLane.LaneState < SiegeLane.LaneStateEnum.Conceited)
        return;
      this.ResetOrderPositions();
    }

    private void ResetOrderPositions()
    {
      this._behaviourState = BehaviorRetakeCastleKeyPosition.BehaviourState.UnSet;
      this._gatheringSide = this.DetermineGatheringSide();
      SiegeLane siegeLane = TeamAISiegeComponent.SiegeLanes.FirstOrDefault<SiegeLane>((Func<SiegeLane, bool>) (sl => sl.LaneSide == this._gatheringSide));
      WorldFrame defenseWaitFrame = siegeLane.DefensePoints.FirstOrDefault<ICastleKeyPosition>().DefenseWaitFrame;
      this._gatheringTacticalPos = siegeLane.DefensePoints.FirstOrDefault<ICastleKeyPosition>().WaitPosition;
      if (this._gatheringTacticalPos != null)
      {
        this._gatherOrder = MovementOrder.MovementOrderMove(this._gatheringTacticalPos.Position);
        this._gatheringFacingOrder = FacingOrder.FacingOrderLookAtEnemy;
      }
      else if (defenseWaitFrame.Origin.IsValid)
      {
        double num = (double) defenseWaitFrame.Rotation.f.Normalize();
        this._gatherOrder = MovementOrder.MovementOrderMove(defenseWaitFrame.Origin);
        this._gatheringFacingOrder = FacingOrder.FacingOrderLookAtEnemy;
      }
      else
      {
        this._gatherOrder = MovementOrder.MovementOrderMove(this.formation.QuerySystem.MedianPosition);
        this._gatheringFacingOrder = FacingOrder.FacingOrderLookAtEnemy;
      }
      this._attackOrder = MovementOrder.MovementOrderMove(TeamAISiegeComponent.SiegeLanes.FirstOrDefault<SiegeLane>((Func<SiegeLane, bool>) (sl => sl.LaneSide == this.behaviorSide)).DefensePoints.FirstOrDefault<ICastleKeyPosition>().MiddleFrame.Origin);
      this._attackFacingOrder = FacingOrder.FacingOrderLookAtEnemy;
      this.CurrentOrder = this._behaviourState == BehaviorRetakeCastleKeyPosition.BehaviourState.Attacking ? this._attackOrder : this._gatherOrder;
      this.CurrentFacingOrder = this._behaviourState == BehaviorRetakeCastleKeyPosition.BehaviourState.Attacking ? this._attackFacingOrder : this._gatheringFacingOrder;
    }

    internal override void OnValidBehaviorSideSet()
    {
      bool flag = false;
      if (this.behaviorSide != this.formation.AI.Side)
        flag = true;
      base.OnValidBehaviorSideSet();
      if (!flag)
        return;
      this.ResetOrderPositions();
    }

    protected internal override void TickOccasionally()
    {
      base.TickOccasionally();
      if (this._behaviourState != BehaviorRetakeCastleKeyPosition.BehaviourState.Attacking)
        this.ConfirmGatheringSide();
      bool flag = true;
      if (this._behaviourState != BehaviorRetakeCastleKeyPosition.BehaviourState.Attacking)
      {
        WorldPosition worldPosition = this.formation.QuerySystem.MedianPosition;
        Vec3 navMeshVec3_1 = worldPosition.GetNavMeshVec3();
        ref Vec3 local = ref navMeshVec3_1;
        worldPosition = this._gatherOrder.GetPosition(this.formation);
        Vec3 navMeshVec3_2 = worldPosition.GetNavMeshVec3();
        flag = (double) local.DistanceSquared(navMeshVec3_2) < 100.0 || (double) this.formation.QuerySystem.FormationIntegrityData.DeviationOfPositionsExcludeFarAgents / ((double) this.formation.QuerySystem.IdealAverageDisplacement != 0.0 ? (double) this.formation.QuerySystem.IdealAverageDisplacement : 1.0) <= 3.0;
      }
      BehaviorRetakeCastleKeyPosition.BehaviourState behaviourState = flag ? BehaviorRetakeCastleKeyPosition.BehaviourState.Attacking : BehaviorRetakeCastleKeyPosition.BehaviourState.Gathering;
      if (behaviourState != this._behaviourState)
      {
        this._behaviourState = behaviourState;
        this.CurrentOrder = this._behaviourState == BehaviorRetakeCastleKeyPosition.BehaviourState.Attacking ? this._attackOrder : this._gatherOrder;
        this.CurrentFacingOrder = this._behaviourState == BehaviorRetakeCastleKeyPosition.BehaviourState.Attacking ? this._attackFacingOrder : this._gatheringFacingOrder;
      }
      this.formation.MovementOrder = this.CurrentOrder;
      this.formation.FacingOrder = this.CurrentFacingOrder;
      if (this._behaviourState != BehaviorRetakeCastleKeyPosition.BehaviourState.Gathering || this._gatheringTacticalPos == null)
        return;
      this.formation.FormOrder = FormOrder.FormOrderCustom(this._gatheringTacticalPos.Width);
    }

    protected override void OnBehaviorActivatedAux()
    {
      this._behaviourState = BehaviorRetakeCastleKeyPosition.BehaviourState.UnSet;
      this.behaviorSide = this.formation.AI.Side;
      this.ResetOrderPositions();
      this.formation.MovementOrder = this.CurrentOrder;
      this.formation.FacingOrder = this.CurrentFacingOrder;
      this.formation.ArrangementOrder = ArrangementOrder.ArrangementOrderLine;
      this.formation.FiringOrder = FiringOrder.FiringOrderFireAtWill;
      this.formation.FormOrder = FormOrder.FormOrderWide;
      this.formation.WeaponUsageOrder = WeaponUsageOrder.WeaponUsageOrderUseAny;
    }

    protected internal override float NavmeshlessTargetPositionPenalty => 1f;

    protected override float GetAiWeight() => 1f;

    private enum BehaviourState
    {
      UnSet,
      Gathering,
      Attacking,
    }
  }
}
