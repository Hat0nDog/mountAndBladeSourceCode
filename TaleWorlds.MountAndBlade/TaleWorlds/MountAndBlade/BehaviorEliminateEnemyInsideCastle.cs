// Decompiled with JetBrains decompiler
// Type: TaleWorlds.MountAndBlade.BehaviorEliminateEnemyInsideCastle
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
  public class BehaviorEliminateEnemyInsideCastle : BehaviorComponent
  {
    private TeamAISiegeDefender _teamAISiegeDefender;
    private BehaviorEliminateEnemyInsideCastle.BehaviourState _behaviourState;
    private MovementOrder _gatherOrder;
    private MovementOrder _attackOrder;
    private FacingOrder _gatheringFacingOrder;
    private FacingOrder _attackFacingOrder;
    private TacticalPosition _gatheringTacticalPos;
    private Formation _targetEnemyFormation;

    public BehaviorEliminateEnemyInsideCastle(Formation formation)
      : base(formation)
    {
      this._teamAISiegeDefender = formation.Team.TeamAI as TeamAISiegeDefender;
      this._behaviourState = BehaviorEliminateEnemyInsideCastle.BehaviourState.UnSet;
      this.behaviorSide = formation.AI.Side;
      this.ResetOrderPositions();
    }

    protected override void CalculateCurrentOrder()
    {
      base.CalculateCurrentOrder();
      this.CurrentOrder = this._behaviourState == BehaviorEliminateEnemyInsideCastle.BehaviourState.Attacking ? this._attackOrder : this._gatherOrder;
    }

    private void DetermineMostImportantInvadingEnemyFormation()
    {
      IEnumerable<Formation> source = this.formation.QuerySystem.Team.EnemyTeams.SelectMany<TeamQuerySystem, Formation>((Func<TeamQuerySystem, IEnumerable<Formation>>) (et => et.Team.Formations)).Where<Formation>((Func<Formation, bool>) (f => TeamAISiegeComponent.IsFormationInsideCastle(f, true)));
      if (source.Any<Formation>())
        this._targetEnemyFormation = source.MaxBy<Formation, float>((Func<Formation, float>) (efi => efi.QuerySystem.FormationPower));
      else
        this._targetEnemyFormation = (Formation) null;
    }

    private void ConfirmGatheringSide()
    {
      SiegeLane siegeLane = TeamAISiegeComponent.SiegeLanes.FirstOrDefault<SiegeLane>((Func<SiegeLane, bool>) (sl => sl.LaneSide == this.behaviorSide));
      if (siegeLane != null && siegeLane.LaneState < SiegeLane.LaneStateEnum.Conceited)
        return;
      this.ResetOrderPositions();
    }

    private FormationAI.BehaviorSide DetermineGatheringSide()
    {
      this.DetermineMostImportantInvadingEnemyFormation();
      if (this._targetEnemyFormation == null)
      {
        if (this._behaviourState == BehaviorEliminateEnemyInsideCastle.BehaviourState.Attacking)
          this._behaviourState = BehaviorEliminateEnemyInsideCastle.BehaviourState.UnSet;
        return this.behaviorSide;
      }
      int connectedSides = TeamAISiegeComponent.QuerySystem.DeterminePositionAssociatedSide(this._targetEnemyFormation.QuerySystem.MedianPosition.GetNavMeshVec3());
      IEnumerable<SiegeLane> source1 = TeamAISiegeComponent.SiegeLanes.Where<SiegeLane>((Func<SiegeLane, bool>) (sl => sl.LaneState != SiegeLane.LaneStateEnum.Conceited && !SiegeQuerySystem.AreSidesRelated(sl.LaneSide, connectedSides)));
      FormationAI.BehaviorSide behaviorSide = this.behaviorSide;
      if (source1.Any<SiegeLane>())
      {
        if (source1.Count<SiegeLane>() > 1)
        {
          int leastDangerousLaneState = source1.Min<SiegeLane>((Func<SiegeLane, int>) (pgl => (int) pgl.LaneState));
          IEnumerable<SiegeLane> source2 = source1.Where<SiegeLane>((Func<SiegeLane, bool>) (pgl => pgl.LaneState == (SiegeLane.LaneStateEnum) leastDangerousLaneState));
          behaviorSide = source2.Count<SiegeLane>() <= 1 ? source2.First<SiegeLane>().LaneSide : source2.MinBy<SiegeLane, int>((Func<SiegeLane, int>) (ldl => SiegeQuerySystem.SideDistance(1 << connectedSides, 1 << (int) (ldl.LaneSide & (FormationAI.BehaviorSide) 31)))).LaneSide;
        }
        else
          behaviorSide = source1.First<SiegeLane>().LaneSide;
      }
      return behaviorSide;
    }

    private void ResetOrderPositions()
    {
      this.behaviorSide = this.DetermineGatheringSide();
      SiegeLane siegeLane = TeamAISiegeComponent.SiegeLanes.FirstOrDefault<SiegeLane>((Func<SiegeLane, bool>) (sl => sl.LaneSide == this.behaviorSide));
      WorldFrame worldFrame = siegeLane == null ? WorldFrame.Invalid : siegeLane.DefensePoints.FirstOrDefault<ICastleKeyPosition>().DefenseWaitFrame;
      this._gatheringTacticalPos = siegeLane != null ? siegeLane.DefensePoints.FirstOrDefault<ICastleKeyPosition>().WaitPosition : (TacticalPosition) null;
      if (this._gatheringTacticalPos != null)
      {
        this._gatherOrder = MovementOrder.MovementOrderMove(this._gatheringTacticalPos.Position);
        this._gatheringFacingOrder = FacingOrder.FacingOrderLookAtEnemy;
      }
      else if (worldFrame.Origin.IsValid)
      {
        double num = (double) worldFrame.Rotation.f.Normalize();
        this._gatherOrder = MovementOrder.MovementOrderMove(worldFrame.Origin);
        this._gatheringFacingOrder = FacingOrder.FacingOrderLookAtEnemy;
      }
      else
      {
        this._gatherOrder = MovementOrder.MovementOrderMove(this.formation.QuerySystem.MedianPosition);
        this._gatheringFacingOrder = FacingOrder.FacingOrderLookAtEnemy;
      }
      this._attackOrder = MovementOrder.MovementOrderChargeToTarget(this._targetEnemyFormation);
      this._attackFacingOrder = FacingOrder.FacingOrderLookAtEnemy;
      this.CurrentOrder = this._behaviourState == BehaviorEliminateEnemyInsideCastle.BehaviourState.Attacking ? this._attackOrder : this._gatherOrder;
      this.CurrentFacingOrder = this._behaviourState == BehaviorEliminateEnemyInsideCastle.BehaviourState.Attacking ? this._attackFacingOrder : this._gatheringFacingOrder;
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
      if (this._behaviourState != BehaviorEliminateEnemyInsideCastle.BehaviourState.Attacking)
        this.ConfirmGatheringSide();
      bool flag;
      if (this._behaviourState == BehaviorEliminateEnemyInsideCastle.BehaviourState.Attacking)
      {
        flag = this._targetEnemyFormation != null;
      }
      else
      {
        int num;
        if (this._targetEnemyFormation != null)
        {
          WorldPosition worldPosition = this.formation.QuerySystem.MedianPosition;
          Vec3 navMeshVec3_1 = worldPosition.GetNavMeshVec3();
          ref Vec3 local = ref navMeshVec3_1;
          worldPosition = this._gatherOrder.GetPosition(this.formation);
          Vec3 navMeshVec3_2 = worldPosition.GetNavMeshVec3();
          num = (double) local.DistanceSquared(navMeshVec3_2) < 100.0 ? 1 : ((double) this.formation.QuerySystem.FormationIntegrityData.DeviationOfPositionsExcludeFarAgents / ((double) this.formation.QuerySystem.IdealAverageDisplacement != 0.0 ? (double) this.formation.QuerySystem.IdealAverageDisplacement : 1.0) <= 3.0 ? 1 : 0);
        }
        else
          num = 0;
        flag = num != 0;
      }
      BehaviorEliminateEnemyInsideCastle.BehaviourState behaviourState = flag ? BehaviorEliminateEnemyInsideCastle.BehaviourState.Attacking : BehaviorEliminateEnemyInsideCastle.BehaviourState.Gathering;
      if (behaviourState != this._behaviourState)
      {
        this._behaviourState = behaviourState;
        this.CurrentOrder = this._behaviourState == BehaviorEliminateEnemyInsideCastle.BehaviourState.Attacking ? this._attackOrder : this._gatherOrder;
        this.CurrentFacingOrder = this._behaviourState == BehaviorEliminateEnemyInsideCastle.BehaviourState.Attacking ? this._attackFacingOrder : this._gatheringFacingOrder;
      }
      this.formation.MovementOrder = this.CurrentOrder;
      this.formation.FacingOrder = this.CurrentFacingOrder;
      if (this._behaviourState != BehaviorEliminateEnemyInsideCastle.BehaviourState.Gathering || this._gatheringTacticalPos == null)
        return;
      this.formation.FormOrder = FormOrder.FormOrderCustom(this._gatheringTacticalPos.Width);
    }

    protected override void OnBehaviorActivatedAux()
    {
      this._behaviourState = BehaviorEliminateEnemyInsideCastle.BehaviourState.UnSet;
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
