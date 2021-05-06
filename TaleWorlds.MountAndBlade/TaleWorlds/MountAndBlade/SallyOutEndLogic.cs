// Decompiled with JetBrains decompiler
// Type: TaleWorlds.MountAndBlade.SallyOutEndLogic
// Assembly: TaleWorlds.MountAndBlade, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3C0E3F42-756F-4407-B817-06BA412A78B0
// Assembly location: E:\SteamLibrary\steamapps\common\Mount & Blade II Bannerlord\bin\Win64_Shipping_Client\TaleWorlds.MountAndBlade.dll

using System;
using System.Collections.ObjectModel;
using System.Linq;
using TaleWorlds.Core;

namespace TaleWorlds.MountAndBlade
{
  public class SallyOutEndLogic : MissionLogic
  {
    private SallyOutEndLogic.EndConditionCheckState _checkState;
    private float _nextCheckTime;
    private float _dtSum;

    public bool IsSallyOutOver { get; private set; }

    public override void OnMissionTick(float dt)
    {
      if (!this.CheckTimer(dt))
        return;
      if (this._checkState == SallyOutEndLogic.EndConditionCheckState.Deactive)
      {
        foreach (Team team in this.Mission.Teams.Where<Team>((Func<Team, bool>) (t => t.Side == BattleSideEnum.Defender)))
        {
          foreach (Formation formation in team.FormationsIncludingSpecial)
          {
            if (!TeamAISiegeComponent.IsFormationInsideCastle(formation, true, 0.1f))
            {
              this._checkState = SallyOutEndLogic.EndConditionCheckState.Active;
              return;
            }
          }
        }
      }
      else
      {
        if (this._checkState != SallyOutEndLogic.EndConditionCheckState.Idle)
          return;
        this._checkState = SallyOutEndLogic.EndConditionCheckState.Active;
      }
    }

    public override bool MissionEnded(ref MissionResult missionResult)
    {
      if (this.IsSallyOutOver)
      {
        missionResult = MissionResult.CreateSuccessful((IMission) this.Mission);
        return true;
      }
      if (this._checkState != SallyOutEndLogic.EndConditionCheckState.Active)
        return false;
      foreach (Team team in (ReadOnlyCollection<Team>) this.Mission.Teams)
      {
        switch (team.Side)
        {
          case BattleSideEnum.Defender:
            if (team.FormationsIncludingSpecial.Any<Formation>() && team.Formations.Any<Formation>((Func<Formation, bool>) (f => !TeamAISiegeComponent.IsFormationInsideCastle(f, false, 0.9f))))
            {
              this._checkState = SallyOutEndLogic.EndConditionCheckState.Idle;
              return false;
            }
            continue;
          case BattleSideEnum.Attacker:
            if (TeamAISiegeComponent.IsFormationGroupInsideCastle(team.FormationsIncludingSpecial, false, 0.1f))
            {
              this._checkState = SallyOutEndLogic.EndConditionCheckState.Idle;
              return false;
            }
            continue;
          default:
            continue;
        }
      }
      this.IsSallyOutOver = true;
      missionResult = MissionResult.CreateSuccessful((IMission) this.Mission);
      return true;
    }

    private bool CheckTimer(float dt)
    {
      this._dtSum += dt;
      if ((double) this._dtSum < (double) this._nextCheckTime)
        return false;
      this._dtSum = 0.0f;
      this._nextCheckTime = (float) (0.800000011920929 + (double) MBRandom.RandomFloat * 0.400000005960464);
      return true;
    }

    private enum EndConditionCheckState
    {
      Deactive,
      Active,
      Idle,
    }
  }
}
