// Decompiled with JetBrains decompiler
// Type: TaleWorlds.MountAndBlade.TeamAISiegeAttacker
// Assembly: TaleWorlds.MountAndBlade, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3C0E3F42-756F-4407-B817-06BA412A78B0
// Assembly location: E:\SteamLibrary\steamapps\common\Mount & Blade II Bannerlord\bin\Win64_Shipping_Client\TaleWorlds.MountAndBlade.dll

using System;
using System.Collections.Generic;
using System.Linq;
using TaleWorlds.Core;
using TaleWorlds.Engine;
using TaleWorlds.Library;

namespace TaleWorlds.MountAndBlade
{
  public class TeamAISiegeAttacker : TeamAISiegeComponent
  {
    private readonly List<ArcherPosition> _archerPositions;

    internal MBReadOnlyList<ArcherPosition> ArcherPositions { get; }

    public TeamAISiegeAttacker(
      Mission currentMission,
      Team currentTeam,
      float thinkTimerTime,
      float applyTimerTime)
      : base(currentMission, currentTeam, thinkTimerTime, applyTimerTime)
    {
      this._archerPositions = currentMission.Scene.FindEntitiesWithTag("archer_position_attacker").Select<GameEntity, ArcherPosition>((Func<GameEntity, ArcherPosition>) (ap => new ArcherPosition(ap, TeamAISiegeComponent.QuerySystem))).ToList<ArcherPosition>();
      this.ArcherPositions = this._archerPositions.GetReadOnlyList<ArcherPosition>();
      Mission.Current.GetMissionBehaviour<SiegeMissionController>().PlayerDeploymentFinish += new CampaignSiegeMissionEvent(this.OnDeploymentFinished);
    }

    public void OnDeploymentFinished() => this._archerPositions.ForEach((Action<ArcherPosition>) (ap => ap.OnDeploymentFinished(TeamAISiegeComponent.QuerySystem, BattleSideEnum.Attacker)));
  }
}
