// Decompiled with JetBrains decompiler
// Type: TaleWorlds.MountAndBlade.Source.Missions.HideoutPhasedMissionController
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

namespace TaleWorlds.MountAndBlade.Source.Missions
{
  public class HideoutPhasedMissionController : MissionLogic
  {
    private GameEntity[] _spawnPoints;
    private Stack<MatrixFrame[]> _spawnPointFrames;
    public const int PhaseCount = 4;
    private bool _IsNewlyPopulatedFormationGivenOrder = true;
    private float _dtSum;

    public override MissionBehaviourType BehaviourType => MissionBehaviourType.Logic;

    public override bool IsOrderShoutingAllowed() => false;

    public override void OnBehaviourInitialize()
    {
      base.OnBehaviourInitialize();
      this.ReadySpawnPointLogic();
    }

    public override void AfterStart()
    {
      base.AfterStart();
      MissionAgentSpawnLogic missionBehaviour = this.Mission.GetMissionBehaviour<MissionAgentSpawnLogic>();
      if (missionBehaviour == null || !this.IsPhasingInitialized)
        return;
      missionBehaviour.AddPhaseChangeAction(BattleSideEnum.Defender, new OnPhaseChangedDelegate(this.OnPhaseChanged));
    }

    private bool IsPhasingInitialized => true;

    private void ReadySpawnPointLogic()
    {
      List<GameEntity> list1 = Mission.Current.GetActiveEntitiesWithScriptComponentOfType<HideoutSpawnPointGroup>().ToList<GameEntity>();
      if (!list1.Any<GameEntity>())
        return;
      HideoutSpawnPointGroup[] hideoutSpawnPointGroupArray = new HideoutSpawnPointGroup[list1.Count];
      foreach (GameEntity gameEntity in list1)
      {
        HideoutSpawnPointGroup firstScriptOfType = gameEntity.GetFirstScriptOfType<HideoutSpawnPointGroup>();
        hideoutSpawnPointGroupArray[firstScriptOfType.PhaseNumber - 1] = firstScriptOfType;
      }
      List<HideoutSpawnPointGroup> list2 = ((IEnumerable<HideoutSpawnPointGroup>) hideoutSpawnPointGroupArray).ToList<HideoutSpawnPointGroup>();
      list2.RemoveAt(0);
      for (int index = 0; index < 3; ++index)
        list2.RemoveAt(MBRandom.RandomInt(list2.Count));
      this._spawnPointFrames = new Stack<MatrixFrame[]>();
      for (int index = 0; index < hideoutSpawnPointGroupArray.Length; ++index)
      {
        if (!list2.Contains(hideoutSpawnPointGroupArray[index]))
        {
          this._spawnPointFrames.Push(hideoutSpawnPointGroupArray[index].GetSpawnPointFrames());
          Debug.Print("Spawn " + (object) hideoutSpawnPointGroupArray[index].PhaseNumber + " is active.", color: Debug.DebugColor.Green, debugFilter: 64UL);
        }
        hideoutSpawnPointGroupArray[index].RemoveWithAllChildren();
      }
      this.CreateSpawnPoints();
    }

    private void CreateSpawnPoints()
    {
      MatrixFrame[] matrixFrameArray = this._spawnPointFrames.Pop();
      this._spawnPoints = new GameEntity[matrixFrameArray.Length];
      for (int index = 0; index < matrixFrameArray.Length; ++index)
      {
        if (!matrixFrameArray[index].IsIdentity)
        {
          this._spawnPoints[index] = GameEntity.CreateEmpty(this.Mission.Scene);
          this._spawnPoints[index].SetGlobalFrame(matrixFrameArray[index]);
          this._spawnPoints[index].AddTag("defender_" + ((FormationClass) index).GetName().ToLower());
        }
      }
    }

    private void OnPhaseChanged()
    {
      if (!this._spawnPointFrames.Any<MatrixFrame[]>())
        return;
      for (int index = 0; index < this._spawnPoints.Length; ++index)
      {
        if (!((NativeObject) this._spawnPoints[index] == (NativeObject) null))
          this._spawnPoints[index].Remove(78);
      }
      this.CreateSpawnPoints();
      this._IsNewlyPopulatedFormationGivenOrder = false;
    }

    public override void OnMissionTick(float dt)
    {
      base.OnMissionTick(dt);
      if (!this._IsNewlyPopulatedFormationGivenOrder)
      {
        foreach (Formation formation in this.Mission.Teams.Where<Team>((Func<Team, bool>) (t => t.Side == BattleSideEnum.Defender)).SelectMany<Team, Formation>((Func<Team, IEnumerable<Formation>>) (t => t.FormationsIncludingSpecial)))
        {
          formation.MovementOrder = MovementOrder.MovementOrderMove(formation.QuerySystem.MedianPosition);
          this._IsNewlyPopulatedFormationGivenOrder = true;
        }
      }
      this._dtSum = 0.0f;
    }

    private bool CheckTimer(float dt)
    {
      this._dtSum += dt;
      if ((double) this._dtSum < 5.0)
        return false;
      this._dtSum = 0.0f;
      return true;
    }
  }
}
