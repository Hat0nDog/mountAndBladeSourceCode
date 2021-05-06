// Decompiled with JetBrains decompiler
// Type: TaleWorlds.MountAndBlade.TeamAISiegeDefender
// Assembly: TaleWorlds.MountAndBlade, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3C0E3F42-756F-4407-B817-06BA412A78B0
// Assembly location: E:\SteamLibrary\steamapps\common\Mount & Blade II Bannerlord\bin\Win64_Shipping_Client\TaleWorlds.MountAndBlade.dll

using System;
using System.Collections.Generic;
using System.Linq;
using TaleWorlds.Core;
using TaleWorlds.DotNet;
using TaleWorlds.Engine;
using TaleWorlds.Library;

namespace TaleWorlds.MountAndBlade
{
  public class TeamAISiegeDefender : TeamAISiegeComponent
  {
    internal Vec3 MurderHolePosition;

    internal List<ArcherPosition> ArcherPositions { get; }

    public TeamAISiegeDefender(
      Mission currentMission,
      Team currentTeam,
      float thinkTimerTime,
      float applyTimerTime)
      : base(currentMission, currentTeam, thinkTimerTime, applyTimerTime)
    {
      TeamAISiegeComponent.QuerySystem = new SiegeQuerySystem(this.Team, (IEnumerable<SiegeLane>) TeamAISiegeComponent.SiegeLanes);
      TeamAISiegeComponent.QuerySystem.Expire();
      this.ArcherPositions = currentMission.Scene.FindEntitiesWithTag("archer_position").Where<GameEntity>((Func<GameEntity, bool>) (ap => (NativeObject) ap.Parent == (NativeObject) null || ap.Parent.IsVisibleIncludeParents())).Select<GameEntity, ArcherPosition>((Func<GameEntity, ArcherPosition>) (ap => new ArcherPosition(ap, TeamAISiegeComponent.QuerySystem))).ToList<ArcherPosition>();
      Mission.Current.GetMissionBehaviour<SiegeMissionController>().PlayerDeploymentFinish += new CampaignSiegeMissionEvent(this.OnDeploymentFinished);
    }

    public void OnDeploymentFinished()
    {
      TeamAISiegeComponent.SiegeLanes.Clear();
      for (int i = 0; i < 3; i++)
      {
        TeamAISiegeComponent.SiegeLanes.Add(new SiegeLane((FormationAI.BehaviorSide) i, TeamAISiegeComponent.QuerySystem));
        SiegeLane siegeLane = TeamAISiegeComponent.SiegeLanes[i];
        siegeLane.SetPrimarySiegeWeapons(this.PrimarySiegeWeapons.Where<SiegeWeapon>((Func<SiegeWeapon, bool>) (psw => psw is IPrimarySiegeWeapon && (psw as IPrimarySiegeWeapon).WeaponSide == (FormationAI.BehaviorSide) i)).Select<SiegeWeapon, IPrimarySiegeWeapon>((Func<SiegeWeapon, IPrimarySiegeWeapon>) (um => um as IPrimarySiegeWeapon)).ToList<IPrimarySiegeWeapon>());
        siegeLane.SetDefensePoints(this.CastleKeyPositions.Where<MissionObject>((Func<MissionObject, bool>) (ckp => (ckp as ICastleKeyPosition).DefenseSide == (FormationAI.BehaviorSide) i)).Select<MissionObject, ICastleKeyPosition>((Func<MissionObject, ICastleKeyPosition>) (dp => dp as ICastleKeyPosition)).ToList<ICastleKeyPosition>());
        siegeLane.RefreshLane();
        siegeLane.DetermineLaneState();
        siegeLane.DetermineOrigins();
      }
      TeamAISiegeComponent.QuerySystem = new SiegeQuerySystem(this.Team, (IEnumerable<SiegeLane>) TeamAISiegeComponent.SiegeLanes);
      TeamAISiegeComponent.QuerySystem.Expire();
      TeamAISiegeComponent.SiegeLanes.ForEach((Action<SiegeLane>) (sl => sl.SetSiegeQuerySystem(TeamAISiegeComponent.QuerySystem)));
      this.ArcherPositions.ForEach((Action<ArcherPosition>) (ap => ap.OnDeploymentFinished(TeamAISiegeComponent.QuerySystem, BattleSideEnum.Defender)));
    }
  }
}
