// Decompiled with JetBrains decompiler
// Type: TaleWorlds.MountAndBlade.TeamAISallyOutAttacker
// Assembly: TaleWorlds.MountAndBlade, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3C0E3F42-756F-4407-B817-06BA412A78B0
// Assembly location: E:\SteamLibrary\steamapps\common\Mount & Blade II Bannerlord\bin\Win64_Shipping_Client\TaleWorlds.MountAndBlade.dll

using System;
using System.Collections.Generic;
using System.Linq;
using TaleWorlds.Core;
using TaleWorlds.Engine;

namespace TaleWorlds.MountAndBlade
{
  public class TeamAISallyOutAttacker : TeamAISiegeComponent
  {
    internal IEnumerable<GameEntity> ArcherPositions;
    internal readonly List<UsableMachine> BesiegerRangedSiegeWeapons;

    public TeamAISallyOutAttacker(
      Mission currentMission,
      Team currentTeam,
      float thinkTimerTime,
      float applyTimerTime)
      : base(currentMission, currentTeam, thinkTimerTime, applyTimerTime)
    {
      this.ArcherPositions = currentMission.Scene.FindEntitiesWithTag("archer_position");
      this.BesiegerRangedSiegeWeapons = new List<UsableMachine>((IEnumerable<UsableMachine>) currentMission.ActiveMissionObjects.FindAllWithType<RangedSiegeWeapon>().Where<RangedSiegeWeapon>((Func<RangedSiegeWeapon, bool>) (w => w.Side == BattleSideEnum.Attacker && !w.IsDisabled)));
    }
  }
}
