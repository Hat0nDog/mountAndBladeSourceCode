// Decompiled with JetBrains decompiler
// Type: TaleWorlds.MountAndBlade.SiegeLane
// Assembly: TaleWorlds.MountAndBlade, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3C0E3F42-756F-4407-B817-06BA412A78B0
// Assembly location: E:\SteamLibrary\steamapps\common\Mount & Blade II Bannerlord\bin\Win64_Shipping_Client\TaleWorlds.MountAndBlade.dll

using System;
using System.Collections.Generic;
using System.Linq;
using TaleWorlds.DotNet;
using TaleWorlds.Engine;

namespace TaleWorlds.MountAndBlade
{
  internal class SiegeLane
  {
    internal const float OpenLaneFactor = 1.5f;
    private SiegeQuerySystem _siegeQuerySystem;
    private readonly Formation[] _lastAssignedFormations;

    internal SiegeLane.LaneStateEnum LaneState { get; private set; }

    internal FormationAI.BehaviorSide LaneSide { get; }

    internal List<IPrimarySiegeWeapon> PrimarySiegeWeapons { get; private set; }

    internal bool IsOpen { get; private set; }

    internal bool IsBreach { get; private set; }

    internal bool HasGate { get; private set; }

    internal List<ICastleKeyPosition> DefensePoints { get; private set; }

    internal WorldPosition DefenderOrigin { get; private set; }

    internal WorldPosition AttackerOrigin { get; private set; }

    internal Formation GetLastAssignedFormation(int teamIndex) => teamIndex >= 0 ? this._lastAssignedFormations[teamIndex] : (Formation) null;

    internal void SetLastAssignedFormation(int teamIndex, Formation formation)
    {
      if (teamIndex < 0)
        return;
      this._lastAssignedFormations[teamIndex] = formation;
    }

    internal void SetSiegeQuerySystem(SiegeQuerySystem siegeQuerySystem) => this._siegeQuerySystem = siegeQuerySystem;

    public float GetLaneCapacity() => this.DefensePoints.Any<ICastleKeyPosition>((Func<ICastleKeyPosition, bool>) (dp => dp is WallSegment && (dp as WallSegment).IsBreachedWall)) || this.HasGate && this.DefensePoints.Where<ICastleKeyPosition>((Func<ICastleKeyPosition, bool>) (dp => dp is CastleGate)).All<ICastleKeyPosition>((Func<ICastleKeyPosition, bool>) (cg => (cg as CastleGate).IsGateOpen)) ? 60f : this.PrimarySiegeWeapons.Where<IPrimarySiegeWeapon>((Func<IPrimarySiegeWeapon, bool>) (psw => !(psw as SiegeWeapon).IsDestroyed)).Sum<IPrimarySiegeWeapon>((Func<IPrimarySiegeWeapon, float>) (psw => psw.SiegeWeaponPriority));

    public float LaneDifficulty => 0.0f;

    public bool IsLaneUnusable => !this.IsOpen && (!this.HasGate || this.DefensePoints.All<ICastleKeyPosition>((Func<ICastleKeyPosition, bool>) (dp => !(dp is CastleGate) || !(dp as CastleGate).IsGateOpen || !(dp as CastleGate).GameEntity.HasTag("outer_gate")))) && this.PrimarySiegeWeapons.All<IPrimarySiegeWeapon>((Func<IPrimarySiegeWeapon, bool>) (psw =>
    {
      switch (psw)
      {
        case UsableMachine _ when (NativeObject) (psw as UsableMachine).GameEntity == (NativeObject) null:
        case SiegeTower _ when (psw as SiegeTower).IsDestroyed:
          return true;
        case BatteringRam _ when !psw.HasCompletedAction():
          return (psw as BatteringRam).IsDestroyed;
        default:
          return false;
      }
    }));

    public SiegeLane(FormationAI.BehaviorSide laneSide, SiegeQuerySystem siegeQuerySystem)
    {
      this.LaneSide = laneSide;
      this.IsOpen = false;
      this.PrimarySiegeWeapons = new List<IPrimarySiegeWeapon>();
      this.DefensePoints = new List<ICastleKeyPosition>();
      this.IsBreach = false;
      this._siegeQuerySystem = siegeQuerySystem;
      this._lastAssignedFormations = new Formation[Mission.Current.Teams.Count];
      this.HasGate = false;
      this.LaneState = SiegeLane.LaneStateEnum.Active;
    }

    public SiegeLane.LaneDefenseStates GetDefenseState()
    {
      switch (this.LaneState)
      {
        case SiegeLane.LaneStateEnum.Safe:
        case SiegeLane.LaneStateEnum.Unused:
          return SiegeLane.LaneDefenseStates.Empty;
        case SiegeLane.LaneStateEnum.Used:
        case SiegeLane.LaneStateEnum.Abandoned:
          return SiegeLane.LaneDefenseStates.Token;
        case SiegeLane.LaneStateEnum.Active:
        case SiegeLane.LaneStateEnum.Contested:
        case SiegeLane.LaneStateEnum.Conceited:
          return SiegeLane.LaneDefenseStates.Full;
        default:
          return SiegeLane.LaneDefenseStates.Full;
      }
    }

    private bool IsPowerBehindLane()
    {
      switch (this.LaneSide)
      {
        case FormationAI.BehaviorSide.Left:
          return this._siegeQuerySystem.LeftRegionMemberCount >= 30;
        case FormationAI.BehaviorSide.Middle:
          return this._siegeQuerySystem.MiddleRegionMemberCount >= 30;
        case FormationAI.BehaviorSide.Right:
          return this._siegeQuerySystem.RightRegionMemberCount >= 30;
        default:
          MBDebug.ShowWarning("Lane without side");
          return false;
      }
    }

    internal bool IsUnderAttack()
    {
      switch (this.LaneSide)
      {
        case FormationAI.BehaviorSide.Left:
          return this._siegeQuerySystem.LeftCloseAttackerCount >= 15;
        case FormationAI.BehaviorSide.Middle:
          return this._siegeQuerySystem.MiddleCloseAttackerCount >= 15;
        case FormationAI.BehaviorSide.Right:
          return this._siegeQuerySystem.RightCloseAttackerCount >= 15;
        default:
          MBDebug.ShowWarning("Lane without side");
          return false;
      }
    }

    internal bool IsDefended()
    {
      switch (this.LaneSide)
      {
        case FormationAI.BehaviorSide.Left:
          return this._siegeQuerySystem.LeftDefenderCount >= 15;
        case FormationAI.BehaviorSide.Middle:
          return this._siegeQuerySystem.MiddleDefenderCount >= 15;
        case FormationAI.BehaviorSide.Right:
          return this._siegeQuerySystem.RightDefenderCount >= 15;
        default:
          MBDebug.ShowWarning("Lane without side");
          return false;
      }
    }

    internal void DetermineLaneState()
    {
      if (this.LaneState == SiegeLane.LaneStateEnum.Conceited && !this.IsDefended())
        return;
      this.LaneState = !this.IsLaneUnusable ? (!Mission.Current.IsTeleportingAgents ? (this.IsOpen ? (this.IsPowerBehindLane() ? (!this.IsUnderAttack() || this.IsDefended() ? SiegeLane.LaneStateEnum.Contested : SiegeLane.LaneStateEnum.Conceited) : SiegeLane.LaneStateEnum.Abandoned) : (!this.PrimarySiegeWeapons.All<IPrimarySiegeWeapon>((Func<IPrimarySiegeWeapon, bool>) (psw => psw is IMoveableSiegeWeapon && !psw.HasCompletedAction() && !(psw as SiegeWeapon).IsUsed)) ? (this.IsPowerBehindLane() ? SiegeLane.LaneStateEnum.Active : SiegeLane.LaneStateEnum.Used) : SiegeLane.LaneStateEnum.Unused)) : SiegeLane.LaneStateEnum.Active) : SiegeLane.LaneStateEnum.Safe;
      if (!this.HasGate || this.LaneState >= SiegeLane.LaneStateEnum.Active || TeamAISiegeComponent.QuerySystem.InsideAttackerCount < 15)
        return;
      this.LaneState = SiegeLane.LaneStateEnum.Active;
    }

    internal void DetermineOrigins()
    {
      if (this.IsBreach)
      {
        WallSegment wallSegment = this.DefensePoints.FirstOrDefault<ICastleKeyPosition>((Func<ICastleKeyPosition, bool>) (dp => dp is WallSegment && (dp as WallSegment).IsBreachedWall)) as WallSegment;
        this.DefenderOrigin = wallSegment.MiddleFrame.Origin;
        this.AttackerOrigin = wallSegment.AttackerWaitFrame.Origin;
      }
      else
      {
        this.HasGate = this.DefensePoints.Any<ICastleKeyPosition>((Func<ICastleKeyPosition, bool>) (dp => dp is CastleGate));
        IEnumerable<IPrimarySiegeWeapon> source = this.PrimarySiegeWeapons.Any<IPrimarySiegeWeapon>() ? (IEnumerable<IPrimarySiegeWeapon>) this.PrimarySiegeWeapons : Mission.Current.MissionObjects.FindAllWithType<SiegeWeapon>().Where<SiegeWeapon>((Func<SiegeWeapon, bool>) (sw => sw is IPrimarySiegeWeapon && (sw as IPrimarySiegeWeapon).WeaponSide == this.LaneSide)).Select<SiegeWeapon, IPrimarySiegeWeapon>((Func<SiegeWeapon, IPrimarySiegeWeapon>) (sw => sw as IPrimarySiegeWeapon));
        if (source.FirstOrDefault<IPrimarySiegeWeapon>((Func<IPrimarySiegeWeapon, bool>) (psw => psw is IMoveableSiegeWeapon)) is IMoveableSiegeWeapon moveableSiegeWeapon2)
        {
          this.DefenderOrigin = ((moveableSiegeWeapon2 as IPrimarySiegeWeapon).TargetCastlePosition as ICastleKeyPosition).MiddleFrame.Origin;
          this.AttackerOrigin = moveableSiegeWeapon2.GetInitialFrame().origin.ToWorldPosition();
        }
        else
        {
          SiegeLadder siegeLadder = source.FirstOrDefault<IPrimarySiegeWeapon>((Func<IPrimarySiegeWeapon, bool>) (psw => psw is SiegeLadder)) as SiegeLadder;
          this.DefenderOrigin = (siegeLadder.TargetCastlePosition as ICastleKeyPosition).MiddleFrame.Origin;
          this.AttackerOrigin = siegeLadder.InitialWaitPosition.GetGlobalFrame().origin.ToWorldPosition();
        }
      }
    }

    public void RefreshLane()
    {
      this.PrimarySiegeWeapons.RemoveAll((Predicate<IPrimarySiegeWeapon>) (psw => psw is SiegeWeapon && (psw as SiegeWeapon).IsDisabled));
      if (this.DefensePoints.Any<ICastleKeyPosition>((Func<ICastleKeyPosition, bool>) (dp => dp is WallSegment && (dp as WallSegment).IsBreachedWall)))
      {
        this.IsOpen = true;
        this.IsBreach = true;
      }
      else if (this.DefensePoints.Any<ICastleKeyPosition>((Func<ICastleKeyPosition, bool>) (dp => dp is CastleGate)) && this.DefensePoints.All<ICastleKeyPosition>((Func<ICastleKeyPosition, bool>) (dp => !(dp is CastleGate) || (dp as CastleGate).IsDestroyed || (dp as CastleGate).State == CastleGate.GateState.Open)) || this.DefensePoints.All<ICastleKeyPosition>((Func<ICastleKeyPosition, bool>) (dp => dp is WallSegment)) && this.PrimarySiegeWeapons.Any<IPrimarySiegeWeapon>((Func<IPrimarySiegeWeapon, bool>) (psw => psw.HasCompletedAction() && !(psw as UsableMachine).IsDestroyed)))
        this.IsOpen = true;
      else
        this.IsOpen = false;
    }

    internal void SetPrimarySiegeWeapons(List<IPrimarySiegeWeapon> primarySiegeWeapons) => this.PrimarySiegeWeapons = primarySiegeWeapons;

    internal void SetDefensePoints(List<ICastleKeyPosition> defensePoints) => this.DefensePoints = defensePoints;

    internal enum LaneStateEnum
    {
      Safe,
      Unused,
      Used,
      Active,
      Abandoned,
      Contested,
      Conceited,
    }

    internal enum LaneDefenseStates
    {
      Empty,
      Token,
      Full,
    }
  }
}
