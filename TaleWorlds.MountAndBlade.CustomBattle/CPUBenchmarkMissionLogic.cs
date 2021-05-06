// Decompiled with JetBrains decompiler
// Type: TaleWorlds.MountAndBlade.CustomBattle.CPUBenchmarkMissionLogic
// Assembly: TaleWorlds.MountAndBlade.CustomBattle, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 8065FEE3-AE77-4E53-B13C-3890101BA431
// Assembly location: E:\SteamLibrary\steamapps\common\Mount & Blade II Bannerlord\Modules\CustomBattle\bin\Win64_Shipping_Client\TaleWorlds.MountAndBlade.CustomBattle.dll

using System;
using System.Collections.Generic;
using TaleWorlds.Core;
using TaleWorlds.DotNet;
using TaleWorlds.Engine;
using TaleWorlds.Engine.Screens;
using TaleWorlds.InputSystem;
using TaleWorlds.Library;
using TaleWorlds.Localization;
using TaleWorlds.MountAndBlade.View.Screen;
using TaleWorlds.ObjectSystem;

namespace TaleWorlds.MountAndBlade.CustomBattle
{
  public class CPUBenchmarkMissionLogic : MissionLogic
  {
    private const float FormationDistDiff = 20f;
    private readonly int _attackerInfCount;
    private readonly int _attackerRangedCount;
    private readonly int _attackerCavCount;
    private readonly int _defenderInfCount;
    private int _curPath;
    private readonly int _defenderCavCount;
    private float benchmark_exit;
    private bool benchmark_finished;
    private Path[] _paths;
    private Path[] _targets;
    private float _cameraSpeed;
    private float _curPathSpeed;
    private float _curPathLenght;
    private float _nextPathSpeed;
    private float _prevPathSpeed;
    private float _cameraPassedDistanceOnPath;
    private MissionAgentSpawnLogic _missionAgentSpawnLogic;
    private bool _formationsSetUp;
    private Formation _defLeftInf;
    private Formation _defMidCav;
    private Formation _defRightInf;
    private Formation _defLeftBInf;
    private Formation _defMidBInf;
    private Formation _defRightBInf;
    private Formation _attLeftInf;
    private Formation _attRightInf;
    private Formation _attLeftRanged;
    private Formation _attRightRanged;
    private Formation _attLeftCav;
    private Formation _attRightCav;
    private Camera benchmarkCamera;
    private int[] _agentCounts;
    private CPUBenchmarkMissionLogic.BattlePhase _battlePhase;
    private bool _isCurPhaseInPlay;
    private float _totalTime;
    private bool benchmarkStarted;

    public CPUBenchmarkMissionLogic(
      int attackerInfCount,
      int attackerRangedCount,
      int attackerCavCount,
      int defenderInfCount,
      int defenderCavCount)
    {
      this._attackerInfCount = attackerInfCount;
      this._attackerRangedCount = attackerRangedCount;
      this._attackerCavCount = attackerCavCount;
      this._defenderInfCount = defenderInfCount;
      this._defenderCavCount = defenderCavCount;
    }

    public override void OnBehaviourInitialize()
    {
      base.OnBehaviourInitialize();
      Utilities.EnableSingleGPUQueryPerFrame();
      this._missionAgentSpawnLogic = this.Mission.GetMissionBehaviour<MissionAgentSpawnLogic>();
      this._paths = this.Mission.Scene.GetPathsWithNamePrefix("CameraPath");
      this._targets = this.Mission.Scene.GetPathsWithNamePrefix("CameraTarget");
      Array.Sort<Path>(this._paths, (Comparison<Path>) ((x, y) => x.GetName().CompareTo(y.GetName())));
      Array.Sort<Path>(this._targets, (Comparison<Path>) ((x, y) => x.GetName().CompareTo(y.GetName())));
      if (this._paths.Length == 0)
        return;
      this._curPath = 0;
      this._cameraPassedDistanceOnPath = 0.0f;
      string name1 = this._paths[this._curPath].GetName();
      int num1 = name1.LastIndexOf('_');
      this._curPathSpeed = this._cameraSpeed = float.Parse(name1.Substring(num1 + 1));
      this._curPathLenght = this._paths[this._curPath].GetTotalLength();
      if (this._paths.Length <= this._curPath + 1)
        return;
      string name2 = this._paths[this._curPath + 1].GetName();
      int num2 = name2.LastIndexOf('_');
      this._nextPathSpeed = float.Parse(name2.Substring(num2 + 1));
    }

    public override void AfterStart()
    {
      base.AfterStart();
      this.Mission.SetMissionMode(MissionMode.Benchmark, true);
      this.Mission.DefenderTeam.ClearTacticOptions();
      this.Mission.AttackerTeam.ClearTacticOptions();
      this.Mission.DefenderTeam.AddTacticOption((TacticComponent) new TacticStop(this.Mission.Teams.Defender));
      this.Mission.AttackerTeam.AddTacticOption((TacticComponent) new TacticStop(this.Mission.Teams.Attacker));
      this._agentCounts = new int[2];
    }

    private void SetupFormations()
    {
      MatrixFrame globalFrame1 = this.Mission.Scene.FindEntityWithTag("defend_right").GetGlobalFrame();
      MatrixFrame globalFrame2 = this.Mission.Scene.FindEntityWithTag("defend_mid").GetGlobalFrame();
      MatrixFrame globalFrame3 = this.Mission.Scene.FindEntityWithTag("defend_left").GetGlobalFrame();
      MatrixFrame globalFrame4 = this.Mission.Scene.FindEntityWithTag("attacker_right").GetGlobalFrame();
      MatrixFrame globalFrame5 = this.Mission.Scene.FindEntityWithTag("attacker_mid").GetGlobalFrame();
      MatrixFrame globalFrame6 = this.Mission.Scene.FindEntityWithTag("attacker_left").GetGlobalFrame();
      this._defLeftInf = this.Mission.DefenderTeam.GetFormation(FormationClass.Infantry);
      this._defMidCav = this.Mission.DefenderTeam.GetFormation(FormationClass.Ranged);
      this._defRightInf = this.Mission.DefenderTeam.GetFormation(FormationClass.Cavalry);
      this._defLeftBInf = this.Mission.DefenderTeam.GetFormation(FormationClass.HorseArcher);
      this._defMidBInf = this.Mission.DefenderTeam.GetFormation(FormationClass.NumberOfDefaultFormations);
      this._defRightBInf = this.Mission.DefenderTeam.GetFormation(FormationClass.HeavyInfantry);
      this._attLeftInf = this.Mission.AttackerTeam.GetFormation(FormationClass.Infantry);
      this._attRightInf = this.Mission.AttackerTeam.GetFormation(FormationClass.Ranged);
      this._attLeftRanged = this.Mission.AttackerTeam.GetFormation(FormationClass.Cavalry);
      this._attRightRanged = this.Mission.AttackerTeam.GetFormation(FormationClass.HorseArcher);
      this._attLeftCav = this.Mission.AttackerTeam.GetFormation(FormationClass.NumberOfDefaultFormations);
      this._attRightCav = this.Mission.AttackerTeam.GetFormation(FormationClass.LightCavalry);
      int num1 = this._defenderInfCount / 6;
      float num2 = (float) this._defenderInfCount / 3.8f;
      int num3 = 0;
      int num4 = this._attackerInfCount / 2;
      int num5 = 0;
      int num6 = this._attackerRangedCount / 2;
      int num7 = 0;
      int num8 = this._attackerCavCount / 2;
      int num9 = 0;
      foreach (Agent agent in (IEnumerable<Agent>) this.Mission.Agents)
      {
        if (agent.Team != null && agent.Character != null)
        {
          ++this._agentCounts[(int) agent.Team.Side];
          if (agent.Team.IsDefender)
          {
            if (agent.Character.DefaultFormationClass == FormationClass.Cavalry)
              agent.Formation = this._defMidCav;
            else if ((double) num3 < (double) num2)
            {
              ++num3;
              agent.Formation = this._defLeftInf;
            }
            else if ((double) num3 < (double) num2 * 2.0)
            {
              ++num3;
              agent.Formation = this._defRightInf;
            }
            else if ((double) num3 < (double) num2 * 2.0 + (double) num1)
            {
              ++num3;
              agent.Formation = this._defLeftBInf;
            }
            else if ((double) num3 < (double) num2 * 2.0 + (double) (num1 * 2))
            {
              ++num3;
              agent.Formation = this._defMidBInf;
            }
            else
              agent.Formation = this._defRightBInf;
          }
          else if (agent.Team.IsAttacker)
          {
            switch (agent.Character.DefaultFormationClass)
            {
              case FormationClass.Infantry:
                if (num5 < num4)
                {
                  ++num5;
                  agent.Formation = this._attLeftInf;
                  continue;
                }
                agent.Formation = this._attRightInf;
                continue;
              case FormationClass.Ranged:
                if (num7 < num6)
                {
                  ++num7;
                  agent.Formation = this._attLeftRanged;
                  continue;
                }
                agent.Formation = this._attRightRanged;
                continue;
              case FormationClass.Cavalry:
                if (num9 < num8)
                {
                  ++num9;
                  agent.Formation = this._attLeftCav;
                  continue;
                }
                agent.Formation = this._attRightCav;
                continue;
              default:
                continue;
            }
          }
        }
      }
      this.Mission.IsTeleportingAgents = true;
      this._defLeftInf.ArrangementOrder = ArrangementOrder.ArrangementOrderLine;
      this._defMidCav.ArrangementOrder = ArrangementOrder.ArrangementOrderLine;
      this._defRightInf.ArrangementOrder = ArrangementOrder.ArrangementOrderLine;
      this._defLeftBInf.ArrangementOrder = ArrangementOrder.ArrangementOrderLine;
      this._defMidBInf.ArrangementOrder = ArrangementOrder.ArrangementOrderLine;
      this._defRightBInf.ArrangementOrder = ArrangementOrder.ArrangementOrderLine;
      this._attLeftInf.ArrangementOrder = ArrangementOrder.ArrangementOrderLine;
      this._attRightInf.ArrangementOrder = ArrangementOrder.ArrangementOrderLine;
      this._attLeftRanged.ArrangementOrder = ArrangementOrder.ArrangementOrderLoose;
      this._attRightRanged.ArrangementOrder = ArrangementOrder.ArrangementOrderLoose;
      this._attLeftCav.ArrangementOrder = ArrangementOrder.ArrangementOrderLine;
      this._attRightCav.ArrangementOrder = ArrangementOrder.ArrangementOrderLine;
      this._defLeftInf.FormOrder = FormOrder.FormOrderCustom(35f);
      this._defMidCav.FormOrder = FormOrder.FormOrderCustom(30f);
      this._defRightInf.FormOrder = FormOrder.FormOrderCustom(35f);
      this._defLeftBInf.FormOrder = FormOrder.FormOrderCustom(25f);
      this._defMidBInf.FormOrder = FormOrder.FormOrderCustom(25f);
      this._defRightBInf.FormOrder = FormOrder.FormOrderCustom(25f);
      this._attLeftInf.FormOrder = FormOrder.FormOrderCustom(25f);
      this._attRightInf.FormOrder = FormOrder.FormOrderCustom(25f);
      this._attLeftRanged.FormOrder = FormOrder.FormOrderCustom(50f);
      this._attRightRanged.FormOrder = FormOrder.FormOrderCustom(50f);
      this._attLeftCav.FormOrder = FormOrder.FormOrderCustom(30f);
      this._attRightCav.FormOrder = FormOrder.FormOrderCustom(30f);
      this._defLeftInf.SetPositioning(new WorldPosition?(new WorldPosition(this.Mission.Scene, globalFrame3.origin + globalFrame3.rotation.f * 20f * 1.125f + 8f * globalFrame3.rotation.s)));
      this._defMidCav.SetPositioning(new WorldPosition?(new WorldPosition(this.Mission.Scene, globalFrame2.origin - globalFrame2.rotation.f * 20f)));
      this._defRightInf.SetPositioning(new WorldPosition?(new WorldPosition(this.Mission.Scene, globalFrame1.origin + globalFrame1.rotation.f * 20f * 1.125f - 8f * globalFrame1.rotation.s)));
      this._defLeftBInf.SetPositioning(new WorldPosition?(new WorldPosition(this.Mission.Scene, globalFrame3.origin - globalFrame3.rotation.s * 10f)));
      this._defMidBInf.SetPositioning(new WorldPosition?(new WorldPosition(this.Mission.Scene, globalFrame2.origin)));
      this._defRightBInf.SetPositioning(new WorldPosition?(new WorldPosition(this.Mission.Scene, globalFrame1.origin + globalFrame1.rotation.s * 10f)));
      Vec3 vec3_1 = globalFrame5.origin - globalFrame6.origin;
      Vec3 vec3_2 = globalFrame5.origin - globalFrame4.origin;
      this._attLeftInf.SetPositioning(new WorldPosition?(new WorldPosition(this.Mission.Scene, globalFrame6.origin + 0.65f * vec3_1)));
      this._attRightInf.SetPositioning(new WorldPosition?(new WorldPosition(this.Mission.Scene, globalFrame4.origin + 0.65f * vec3_2)));
      this._attLeftRanged.SetPositioning(new WorldPosition?(new WorldPosition(this.Mission.Scene, globalFrame6.origin + globalFrame6.rotation.f * 20f - 0.3f * vec3_1)));
      this._attRightRanged.SetPositioning(new WorldPosition?(new WorldPosition(this.Mission.Scene, globalFrame4.origin + globalFrame4.rotation.f * 20f - 0.3f * vec3_2)));
      this._attLeftCav.SetPositioning(new WorldPosition?(new WorldPosition(this.Mission.Scene, globalFrame6.origin - globalFrame6.rotation.f * 20f * 0.1f - globalFrame6.rotation.s * 25f)));
      this._attRightCav.SetPositioning(new WorldPosition?(new WorldPosition(this.Mission.Scene, globalFrame4.origin - globalFrame4.rotation.f * 20f * 0.1f + globalFrame4.rotation.s * 25f)));
      this._defLeftInf.MovementOrder = MovementOrder.MovementOrderMove(this._defLeftInf.OrderPosition);
      this._defMidCav.MovementOrder = MovementOrder.MovementOrderMove(this._defMidCav.OrderPosition);
      this._defRightInf.MovementOrder = MovementOrder.MovementOrderMove(this._defRightInf.OrderPosition);
      this._defLeftBInf.MovementOrder = MovementOrder.MovementOrderMove(this._defLeftBInf.OrderPosition);
      this._defMidBInf.MovementOrder = MovementOrder.MovementOrderMove(this._defMidBInf.OrderPosition);
      this._defRightBInf.MovementOrder = MovementOrder.MovementOrderMove(this._defRightBInf.OrderPosition);
      this._attLeftInf.MovementOrder = MovementOrder.MovementOrderMove(this._attLeftInf.OrderPosition);
      this._attRightInf.MovementOrder = MovementOrder.MovementOrderMove(this._attRightInf.OrderPosition);
      this._attLeftRanged.MovementOrder = MovementOrder.MovementOrderMove(this._attLeftRanged.OrderPosition);
      this._attRightRanged.MovementOrder = MovementOrder.MovementOrderMove(this._attRightRanged.OrderPosition);
      this._attLeftCav.MovementOrder = MovementOrder.MovementOrderMove(this._attLeftCav.OrderPosition);
      this._attRightCav.MovementOrder = MovementOrder.MovementOrderMove(this._attRightCav.OrderPosition);
      foreach (Formation formation in this.Mission.AttackerTeam.Formations)
      {
        formation.ReleaseFormationFromAI();
        formation.FiringOrder = FiringOrder.FiringOrderHoldYourFire;
      }
      foreach (Formation formation in this.Mission.DefenderTeam.Formations)
      {
        formation.ReleaseFormationFromAI();
        formation.FiringOrder = FiringOrder.FiringOrderHoldYourFire;
      }
      this._formationsSetUp = true;
    }

    public override void OnMissionTick(float dt) => this.benchmarkStarted = true;

    public override void OnPreMissionTick(float dt)
    {
      base.OnMissionTick(dt);
      if (!this.benchmarkStarted)
        return;
      this._totalTime += dt;
      Utilities.SetBenchmarkStatus(4, "  Battle Size: " + (object) (this._attackerCavCount + this._attackerInfCount + this._attackerRangedCount) + " (" + (object) this.Mission.AttackerTeam.ActiveAgents.Count + ") vs (" + (object) this.Mission.DefenderTeam.ActiveAgents.Count + ") " + (object) (this._defenderCavCount + this._defenderInfCount));
      if ((double) this.benchmark_exit != 0.0)
      {
        if ((double) this._totalTime - (double) this.benchmark_exit < 3.0)
        {
          MBDebug.RenderText(385f, 10f, "Hold 'ESC' to exit benchmark. " + (object) (float) (3.0 - ((double) this._totalTime - (double) this.benchmark_exit)), Colors.Red.ToUnsignedInteger());
        }
        else
        {
          MBDebug.RenderText(10f, 10f, "Exiting benchmark in " + (object) (float) (9.0 - ((double) this._totalTime - (double) this.benchmark_exit)) + " sec.", Colors.Green.ToUnsignedInteger());
          Utilities.SetBenchmarkStatus(2, "");
          MouseManager.ShowCursor(true);
          this.benchmark_finished = true;
        }
      }
      else if (!this.benchmark_finished)
        MBDebug.RenderText(385f, 10f, "Current Time:" + (object) this._totalTime);
      if (Input.IsKeyPressed(InputKey.Escape) && (double) this.benchmark_exit == 0.0)
        this.benchmark_exit = this._totalTime;
      if (Input.IsKeyReleased(InputKey.Escape) && (double) this.benchmark_exit != 0.0 && (double) this._totalTime - (double) this.benchmark_exit < 3.0)
        this.benchmark_exit = 0.0f;
      if (!this._formationsSetUp && this._missionAgentSpawnLogic.IsInitialSpawnOver)
      {
        this.SetupFormations();
        Utilities.SetBenchmarkStatus(1, "");
      }
      if (this._formationsSetUp)
        this.Check();
      if ((double) this._totalTime > 92.0)
      {
        Utilities.SetBenchmarkStatus(2, "");
        MouseManager.ShowCursor(true);
        this.benchmark_finished = true;
      }
      if ((double) this.benchmark_exit != 0.0 && (double) this._totalTime - (double) this.benchmark_exit > 9.0)
      {
        Utilities.SetBenchmarkStatus(0, "    Battle Size: " + (object) (this._attackerCavCount + this._attackerInfCount + this._attackerRangedCount) + " vs " + (object) (this._defenderCavCount + this._defenderInfCount));
        Mission.Current.EndMission();
      }
      if (!(ScreenManager.TopScreen is MissionScreen topScreen))
        return;
      Camera combatCamera = topScreen.CombatCamera;
      if (!((NativeObject) combatCamera != (NativeObject) null) || this._curPath >= this._paths.Length)
        return;
      if ((NativeObject) this.benchmarkCamera == (NativeObject) null)
      {
        this.benchmarkCamera = Camera.CreateCamera();
        this.benchmarkCamera.SetFovHorizontal(combatCamera.HorizontalFov, combatCamera.GetAspectRatio(), combatCamera.Near, combatCamera.Far);
      }
      if ((double) this._cameraPassedDistanceOnPath > (double) this._curPathLenght / 6.0 * 5.0)
        this._cameraSpeed = MathF.Lerp(this._curPathSpeed, this._curPath != this._paths.Length - 1 ? (float) (((double) this._nextPathSpeed + (double) this._curPathSpeed) / 2.0) : 5f, (float) (((double) this._cameraPassedDistanceOnPath - (double) this._curPathLenght / 6.0 * 5.0) / ((double) this._curPathLenght / 6.0)));
      if ((double) this._cameraPassedDistanceOnPath < (double) this._curPathLenght / 6.0)
        this._cameraSpeed = MathF.Lerp(this._curPath != 0 ? (float) (((double) this._curPathSpeed + (double) this._prevPathSpeed) / 2.0) : 5f, this._curPathSpeed, this._cameraPassedDistanceOnPath / (this._curPathLenght / 6f));
      this._cameraPassedDistanceOnPath += this._cameraSpeed * dt;
      if ((double) this._cameraPassedDistanceOnPath >= (double) this._paths[this._curPath].GetTotalLength())
      {
        if (this._curPath != this._paths.Length - 1)
        {
          ++this._curPath;
          this._curPathLenght = this._paths[this._curPath].GetTotalLength();
          this._prevPathSpeed = this._curPathSpeed;
          this._curPathSpeed = this._nextPathSpeed;
          this._cameraPassedDistanceOnPath = this._cameraSpeed * dt;
          if (this._paths.Length > this._curPath + 1)
          {
            string name = this._paths[this._curPath + 1].GetName();
            int num = name.LastIndexOf('_');
            this._nextPathSpeed = float.Parse(name.Substring(num + 1));
          }
        }
        else
        {
          Utilities.SetBenchmarkStatus(0, "    Battle Size: " + (object) (this._attackerCavCount + this._attackerInfCount + this._attackerRangedCount) + " vs " + (object) (this._defenderCavCount + this._defenderInfCount));
          Mission.Current.EndMission();
        }
      }
      this.benchmarkCamera.LookAt(this._paths[this._curPath].GetFrameForDistance(Math.Min(this._paths[this._curPath].GetTotalLength(), this._cameraPassedDistanceOnPath)).origin, this._targets[this._curPath].GetFrameForDistance(Math.Min(1f, this._cameraPassedDistanceOnPath / this._paths[this._curPath].GetTotalLength()) * this._targets[this._curPath].GetTotalLength()).origin, Vec3.Up);
      topScreen.UpdateFreeCamera(this.benchmarkCamera.Frame);
      topScreen.CustomCamera = topScreen.CombatCamera;
    }

    private void Check()
    {
      float time = this.Mission.Time;
      if (this._battlePhase == CPUBenchmarkMissionLogic.BattlePhase.Start && (double) time >= 5.0)
      {
        this.Mission.IsTeleportingAgents = false;
        this._battlePhase = CPUBenchmarkMissionLogic.BattlePhase.ArrowShower;
      }
      else
      {
        if (this._battlePhase == CPUBenchmarkMissionLogic.BattlePhase.Start)
          return;
        if (!this._isCurPhaseInPlay)
        {
          Debug.Print("State: " + (object) this._battlePhase, color: Debug.DebugColor.Cyan, debugFilter: 64UL);
          switch (this._battlePhase)
          {
            case CPUBenchmarkMissionLogic.BattlePhase.ArrowShower:
              this._attLeftRanged.FiringOrder = FiringOrder.FiringOrderFireAtWill;
              this._attRightRanged.FiringOrder = FiringOrder.FiringOrderFireAtWill;
              this._defLeftBInf.FiringOrder = FiringOrder.FiringOrderFireAtWill;
              this._defRightBInf.FiringOrder = FiringOrder.FiringOrderFireAtWill;
              this._defMidBInf.FiringOrder = FiringOrder.FiringOrderFireAtWill;
              this._defLeftInf.ArrangementOrder = ArrangementOrder.ArrangementOrderShieldWall;
              this._defRightInf.ArrangementOrder = ArrangementOrder.ArrangementOrderShieldWall;
              this._defLeftInf.FormOrder = FormOrder.FormOrderCustom(35f);
              this._defRightInf.FormOrder = FormOrder.FormOrderCustom(35f);
              this._attLeftInf.ArrangementOrder = ArrangementOrder.ArrangementOrderShieldWall;
              this._attRightInf.ArrangementOrder = ArrangementOrder.ArrangementOrderShieldWall;
              break;
            case CPUBenchmarkMissionLogic.BattlePhase.MeleePosition:
              Vec3 vec3_1 = -(this._attLeftInf.OrderPosition.Position - this._defRightInf.OrderPosition.Position);
              Vec3 vec3_2 = -(this._attRightInf.OrderPosition.Position - this._defLeftInf.OrderPosition.Position);
              vec3_1.RotateAboutZ((float) Math.PI / 36f);
              vec3_2.RotateAboutZ(-1f * (float) Math.PI / 36f);
              this._attLeftInf.MovementOrder = MovementOrder.MovementOrderMove(new WorldPosition(this.Mission.Scene, this._attLeftInf.OrderPosition.Position + vec3_1));
              this._attRightInf.MovementOrder = MovementOrder.MovementOrderMove(new WorldPosition(this.Mission.Scene, this._attRightInf.OrderPosition.Position + vec3_2));
              break;
            case CPUBenchmarkMissionLogic.BattlePhase.Cav1Pos:
              Vec3 position1 = this._attLeftRanged.OrderPosition.Position;
              Vec2 direction1 = this._attLeftRanged.Direction;
              Vec3 vec3_3 = position1 - 15f * direction1.ToVec3();
              direction1.RotateCCW(1.570796f);
              this._attLeftCav.MovementOrder = MovementOrder.MovementOrderMove(new WorldPosition(this.Mission.Scene, vec3_3 + 60f * direction1.ToVec3()));
              break;
            case CPUBenchmarkMissionLogic.BattlePhase.Cav1PosDef:
              MatrixFrame globalFrame1 = this.Mission.Scene.FindEntityWithTag("defend_right").GetGlobalFrame();
              this._defMidCav.MovementOrder = MovementOrder.MovementOrderMove(new WorldPosition(this.Mission.Scene, globalFrame1.origin + 40f * globalFrame1.rotation.s));
              break;
            case CPUBenchmarkMissionLogic.BattlePhase.CavalryPosition:
              Vec3 position2 = this._attRightRanged.OrderPosition.Position;
              Vec2 direction2 = this._attRightRanged.Direction;
              Vec3 vec3_4 = position2 + 20f * direction2.ToVec3();
              direction2.RotateCCW(-1.570796f);
              this._attRightCav.MovementOrder = MovementOrder.MovementOrderMove(new WorldPosition(this.Mission.Scene, vec3_4 + 80f * direction2.ToVec3()));
              this._attLeftInf.MovementOrder = MovementOrder.MovementOrderCharge;
              this._attRightInf.MovementOrder = MovementOrder.MovementOrderCharge;
              this._defLeftBInf.FiringOrder = FiringOrder.FiringOrderFireAtWill;
              break;
            case CPUBenchmarkMissionLogic.BattlePhase.MeleeAttack:
              this._defLeftInf.FiringOrder = FiringOrder.FiringOrderFireAtWill;
              this._defMidBInf.FiringOrder = FiringOrder.FiringOrderFireAtWill;
              this._defRightBInf.FiringOrder = FiringOrder.FiringOrderFireAtWill;
              this._attLeftInf.ArrangementOrder = ArrangementOrder.ArrangementOrderLine;
              this._attRightInf.ArrangementOrder = ArrangementOrder.ArrangementOrderLine;
              this._attLeftInf.MovementOrder = MovementOrder.MovementOrderMove(new WorldPosition(this.Mission.Scene, this._defRightInf.GetAveragePositionOfUnits(true, false).ToVec3()));
              this._attRightInf.MovementOrder = MovementOrder.MovementOrderMove(new WorldPosition(this.Mission.Scene, this._defLeftInf.GetAveragePositionOfUnits(true, false).ToVec3()));
              break;
            case CPUBenchmarkMissionLogic.BattlePhase.RangedAdvance:
              Vec2 averagePositionOfUnits1 = this._attLeftRanged.GetAveragePositionOfUnits(true, false);
              Vec3 vec3_5 = averagePositionOfUnits1.ToVec3();
              averagePositionOfUnits1 = this._attLeftRanged.GetAveragePositionOfUnits(true, false);
              Vec3 vec3_6 = averagePositionOfUnits1.ToVec3();
              averagePositionOfUnits1 = this._defRightInf.GetAveragePositionOfUnits(true, false);
              Vec3 vec3_7 = averagePositionOfUnits1.ToVec3();
              Vec3 vec3_8 = 0.15f * (vec3_6 - vec3_7);
              Vec3 position3 = vec3_5 - vec3_8;
              averagePositionOfUnits1 = this._attRightRanged.GetAveragePositionOfUnits(true, false);
              Vec3 vec3_9 = averagePositionOfUnits1.ToVec3();
              averagePositionOfUnits1 = this._attRightRanged.GetAveragePositionOfUnits(true, false);
              Vec3 vec3_10 = averagePositionOfUnits1.ToVec3();
              averagePositionOfUnits1 = this._defLeftInf.GetAveragePositionOfUnits(true, false);
              Vec3 vec3_11 = averagePositionOfUnits1.ToVec3();
              Vec3 vec3_12 = 0.15f * (vec3_10 - vec3_11);
              Vec3 position4 = vec3_9 - vec3_12;
              this._attLeftRanged.MovementOrder = MovementOrder.MovementOrderMove(new WorldPosition(this.Mission.Scene, position3));
              this._attRightRanged.MovementOrder = MovementOrder.MovementOrderMove(new WorldPosition(this.Mission.Scene, position4));
              break;
            case CPUBenchmarkMissionLogic.BattlePhase.CavalryAdvance:
              this.Mission.Scene.FindEntityWithTag("attacker_mid").GetGlobalFrame();
              MatrixFrame globalFrame2 = this.Mission.Scene.FindEntityWithTag("defend_right").GetGlobalFrame();
              this.Mission.Scene.FindEntityWithTag("defend_left").GetGlobalFrame();
              Vec3 position5 = globalFrame2.origin + globalFrame2.rotation.s * 68f + 10f * this._attLeftRanged.Direction.ToVec3();
              this._attLeftCav.MovementOrder = MovementOrder.MovementOrderMove(new WorldPosition(this.Mission.Scene, position5));
              this._defMidCav.MovementOrder = MovementOrder.MovementOrderMove(new WorldPosition(this.Mission.Scene, position5));
              break;
            case CPUBenchmarkMissionLogic.BattlePhase.CavalryCharge:
              MatrixFrame globalFrame3 = this.Mission.Scene.FindEntityWithTag("defend_left").GetGlobalFrame();
              this._defLeftBInf.FacingOrder = FacingOrder.FacingOrderLookAtDirection((this._attRightCav.CurrentPosition - this._defLeftBInf.CurrentPosition).Normalized());
              this._defLeftBInf.MovementOrder = MovementOrder.MovementOrderMove(new WorldPosition(this.Mission.Scene, globalFrame3.origin - globalFrame3.rotation.s * 10f));
              this._attRightCav.MovementOrder = MovementOrder.MovementOrderChargeToTarget(this._defLeftBInf);
              this._attLeftCav.MovementOrder = MovementOrder.MovementOrderChargeToTarget(this._attLeftInf);
              this._defMidCav.MovementOrder = MovementOrder.MovementOrderChargeToTarget(this._attRightInf);
              break;
            case CPUBenchmarkMissionLogic.BattlePhase.CavalryCharge2:
              this._attRightCav.MovementOrder = MovementOrder.MovementOrderMove(new WorldPosition(this.Mission.Scene, this._defLeftBInf.OrderPosition.Position));
              this._attLeftRanged.FiringOrder = FiringOrder.FiringOrderFireAtWill;
              this._attLeftRanged.MovementOrder = MovementOrder.MovementOrderAdvance;
              this._attRightRanged.FiringOrder = FiringOrder.FiringOrderFireAtWill;
              this._attRightRanged.MovementOrder = MovementOrder.MovementOrderAdvance;
              break;
            case CPUBenchmarkMissionLogic.BattlePhase.RangedAdvance2:
              Vec2 averagePositionOfUnits2 = this._attLeftRanged.GetAveragePositionOfUnits(true, false);
              Vec3 vec3_13 = averagePositionOfUnits2.ToVec3();
              averagePositionOfUnits2 = this._attLeftRanged.GetAveragePositionOfUnits(true, false);
              Vec3 vec3_14 = averagePositionOfUnits2.ToVec3();
              averagePositionOfUnits2 = this._defRightInf.GetAveragePositionOfUnits(true, false);
              Vec3 vec3_15 = averagePositionOfUnits2.ToVec3();
              Vec3 vec3_16 = 0.15f * (vec3_14 - vec3_15);
              Vec3 position6 = vec3_13 - vec3_16;
              averagePositionOfUnits2 = this._attRightRanged.GetAveragePositionOfUnits(true, false);
              Vec3 vec3_17 = averagePositionOfUnits2.ToVec3();
              averagePositionOfUnits2 = this._attRightRanged.GetAveragePositionOfUnits(true, false);
              Vec3 vec3_18 = averagePositionOfUnits2.ToVec3();
              averagePositionOfUnits2 = this._defLeftInf.GetAveragePositionOfUnits(true, false);
              Vec3 vec3_19 = averagePositionOfUnits2.ToVec3();
              Vec3 vec3_20 = 0.15f * (vec3_18 - vec3_19);
              Vec3 position7 = vec3_17 - vec3_20;
              this._attLeftRanged.MovementOrder = MovementOrder.MovementOrderMove(new WorldPosition(this.Mission.Scene, position6));
              this._attRightRanged.MovementOrder = MovementOrder.MovementOrderMove(new WorldPosition(this.Mission.Scene, position7));
              break;
            case CPUBenchmarkMissionLogic.BattlePhase.FullCharge:
              using (IEnumerator<Formation> enumerator = this.Mission.AttackerTeam.Formations.GetEnumerator())
              {
                while (enumerator.MoveNext())
                {
                  Formation current = enumerator.Current;
                  if (current != this._attLeftRanged && current != this._attRightRanged && current != this._attRightCav)
                    current.MovementOrder = MovementOrder.MovementOrderCharge;
                }
                break;
              }
          }
          this._isCurPhaseInPlay = true;
        }
        else
        {
          switch (this._battlePhase)
          {
            case CPUBenchmarkMissionLogic.BattlePhase.ArrowShower:
              if ((double) time <= 14.0)
                break;
              this._isCurPhaseInPlay = false;
              this._battlePhase = CPUBenchmarkMissionLogic.BattlePhase.MeleePosition;
              break;
            case CPUBenchmarkMissionLogic.BattlePhase.MeleePosition:
              if ((double) time <= 19.0)
                break;
              this._isCurPhaseInPlay = false;
              this._battlePhase = CPUBenchmarkMissionLogic.BattlePhase.MeleeAttack;
              break;
            case CPUBenchmarkMissionLogic.BattlePhase.Cav1Pos:
              if ((double) time <= 19.0)
                break;
              this._isCurPhaseInPlay = false;
              this._battlePhase = CPUBenchmarkMissionLogic.BattlePhase.Cav1PosDef;
              break;
            case CPUBenchmarkMissionLogic.BattlePhase.Cav1PosDef:
              if ((double) time <= 24.0)
                break;
              this._isCurPhaseInPlay = false;
              this._battlePhase = CPUBenchmarkMissionLogic.BattlePhase.CavalryAdvance;
              break;
            case CPUBenchmarkMissionLogic.BattlePhase.CavalryPosition:
              if ((double) time <= 74.5)
                break;
              this._isCurPhaseInPlay = false;
              this._battlePhase = CPUBenchmarkMissionLogic.BattlePhase.CavalryCharge;
              break;
            case CPUBenchmarkMissionLogic.BattlePhase.MeleeAttack:
              if ((double) time <= 19.0)
                break;
              this._isCurPhaseInPlay = false;
              this._battlePhase = CPUBenchmarkMissionLogic.BattlePhase.Cav1Pos;
              break;
            case CPUBenchmarkMissionLogic.BattlePhase.RangedAdvance:
              if ((double) time <= 60.0)
                break;
              this._isCurPhaseInPlay = false;
              this._battlePhase = CPUBenchmarkMissionLogic.BattlePhase.CavalryPosition;
              break;
            case CPUBenchmarkMissionLogic.BattlePhase.CavalryAdvance:
              if ((double) time <= 30.0)
                break;
              this._isCurPhaseInPlay = false;
              this._battlePhase = CPUBenchmarkMissionLogic.BattlePhase.RangedAdvance;
              break;
            case CPUBenchmarkMissionLogic.BattlePhase.CavalryCharge:
              if ((double) time <= 92.0)
                break;
              this._isCurPhaseInPlay = false;
              this._battlePhase = CPUBenchmarkMissionLogic.BattlePhase.CavalryCharge2;
              break;
            case CPUBenchmarkMissionLogic.BattlePhase.CavalryCharge2:
              if ((double) time <= 93.0)
                break;
              this._isCurPhaseInPlay = false;
              this._battlePhase = CPUBenchmarkMissionLogic.BattlePhase.RangedAdvance2;
              break;
            case CPUBenchmarkMissionLogic.BattlePhase.RangedAdvance2:
              if ((double) time <= 94.0)
                break;
              this._isCurPhaseInPlay = false;
              this._battlePhase = CPUBenchmarkMissionLogic.BattlePhase.FullCharge;
              break;
          }
        }
      }
    }

    public override void OnAgentRemoved(
      Agent affectedAgent,
      Agent affectorAgent,
      AgentState agentState,
      KillingBlow blow)
    {
      if (affectedAgent.Team != null && affectedAgent.Character != null)
        --this._agentCounts[(int) affectedAgent.Team.Side];
      base.OnAgentRemoved(affectedAgent, affectorAgent, agentState, blow);
    }

    [CommandLineFunctionality.CommandLineArgumentFunction("cpu_benchmark_mission", "benchmark")]
    public static string CPUBenchmarkMission(List<string> strings)
    {
      CPUBenchmarkMissionLogic.OpenCPUBenchmarkMission("benchmark_battle_11");
      return "Success";
    }

    [CommandLineFunctionality.CommandLineArgumentFunction("cpu_benchmark", "benchmark")]
    public static string CPUBenchmark(List<string> strings)
    {
      MBGameManager.StartNewGame((MBGameManager) new CustomGameManager());
      return "";
    }

    [CommandLineFunctionality.CommandLineArgumentFunction("benchmark_start", "state_string")]
    public static string BenchmarkStateStart(List<string> strings)
    {
      switch (GameStateManager.Current.ActiveState)
      {
        case InitialState _:
          MBGameManager.StartNewGame((MBGameManager) new CustomGameManager());
          break;
        case CustomBattleState _:
          GameStateManager.StateActivateCommand = "state_string.benchmark_end";
          CPUBenchmarkMissionLogic.OpenCPUBenchmarkMission("benchmark_battle_11");
          break;
      }
      return "";
    }

    [CommandLineFunctionality.CommandLineArgumentFunction("benchmark_end", "state_string")]
    public static string BenchmarkStateEnd(List<string> strings)
    {
      if (GameStateManager.Current.ActiveState is CustomBattleState)
      {
        GameStateManager.StateActivateCommand = (string) null;
        Game.Current.GameStateManager.PopState();
      }
      return "";
    }

    public static Mission OpenCPUBenchmarkMission(string scene)
    {
      int realBattleSize = BannerlordConfig.GetRealBattleSize();
      int attackerInfCount = realBattleSize / 100 * 18;
      int attackerRangedCount = realBattleSize / 100 * 10;
      int attackerCavCount = realBattleSize / 100 * 8;
      int defenderInfCount = realBattleSize / 100 * 59;
      int defenderCavCount = realBattleSize / 100 * 5;
      IMissionTroopSupplier[] troopSuppliers = new IMissionTroopSupplier[2];
      BasicCultureObject culture = MBObjectManager.Instance.GetObject<BasicCultureObject>("empire");
      Banner banner1 = new Banner("11.4.124.4345.4345.768.768.1.0.0.163.0.5.512.512.769.764.1.0.0");
      Banner banner2 = new Banner("11.45.126.4345.4345.768.768.1.0.0.462.0.13.512.512.769.764.1.0.0");
      CustomBattleCombatant playerParty = new CustomBattleCombatant(new TextObject("{=!}Player Party"), culture, banner1);
      playerParty.Side = BattleSideEnum.Attacker;
      playerParty.AddCharacter(MBObjectManager.Instance.GetObject<BasicCharacterObject>("imperial_legionary"), attackerInfCount);
      playerParty.AddCharacter(MBObjectManager.Instance.GetObject<BasicCharacterObject>("imperial_palatine_guard"), attackerRangedCount);
      playerParty.AddCharacter(MBObjectManager.Instance.GetObject<BasicCharacterObject>("imperial_cataphract"), attackerCavCount);
      CustomBattleCombatant enemyParty = new CustomBattleCombatant(new TextObject("{=!}Enemy Party"), culture, banner2);
      enemyParty.Side = BattleSideEnum.Defender;
      enemyParty.AddCharacter(MBObjectManager.Instance.GetObject<BasicCharacterObject>("battanian_wildling"), defenderInfCount);
      enemyParty.AddCharacter(MBObjectManager.Instance.GetObject<BasicCharacterObject>("battanian_horseman"), defenderCavCount);
      CustomBattleTroopSupplier battleTroopSupplier1 = new CustomBattleTroopSupplier(playerParty, true);
      troopSuppliers[(int) playerParty.Side] = (IMissionTroopSupplier) battleTroopSupplier1;
      CustomBattleTroopSupplier battleTroopSupplier2 = new CustomBattleTroopSupplier(enemyParty, false);
      troopSuppliers[(int) enemyParty.Side] = (IMissionTroopSupplier) battleTroopSupplier2;
      return MissionState.OpenNew("CPUBenchmarkMission", new MissionInitializerRecord(scene)
      {
        DoNotUseLoadingScreen = false,
        PlayingInCampaignMode = false
      }, (InitializeMissionBehvaioursDelegate) (missionController => (IEnumerable<MissionBehaviour>) new MissionBehaviour[10]
      {
        (MissionBehaviour) new MissionCombatantsLogic((IEnumerable<IBattleCombatant>) null, (IBattleCombatant) playerParty, (IBattleCombatant) enemyParty, (IBattleCombatant) playerParty, Mission.MissionTeamAITypeEnum.FieldBattle, false),
        (MissionBehaviour) new MissionAgentSpawnLogic(troopSuppliers, BattleSideEnum.Attacker),
        (MissionBehaviour) new CPUBenchmarkMissionSpawnHandler(enemyParty, playerParty),
        (MissionBehaviour) new CPUBenchmarkMissionLogic(attackerInfCount, attackerRangedCount, attackerCavCount, defenderInfCount, defenderCavCount),
        (MissionBehaviour) new AgentBattleAILogic(),
        (MissionBehaviour) new AgentVictoryLogic(),
        (MissionBehaviour) new MissionHardBorderPlacer(),
        (MissionBehaviour) new MissionBoundaryPlacer(),
        (MissionBehaviour) new MissionBoundaryCrossingHandler(),
        (MissionBehaviour) new AgentFadeOutLogic()
      }));
    }

    private enum BattlePhase
    {
      Start,
      ArrowShower,
      MeleePosition,
      Cav1Pos,
      Cav1PosDef,
      CavalryPosition,
      MeleeAttack,
      RangedAdvance,
      CavalryAdvance,
      CavalryCharge,
      CavalryCharge2,
      RangedAdvance2,
      FullCharge,
    }
  }
}
