// Decompiled with JetBrains decompiler
// Type: TaleWorlds.MountAndBlade.MissionAgentSpawnLogic
// Assembly: TaleWorlds.MountAndBlade, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3C0E3F42-756F-4407-B817-06BA412A78B0
// Assembly location: E:\SteamLibrary\steamapps\common\Mount & Blade II Bannerlord\bin\Win64_Shipping_Client\TaleWorlds.MountAndBlade.dll

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using TaleWorlds.Core;
using TaleWorlds.Engine;
using TaleWorlds.Library;
using TaleWorlds.MountAndBlade.Missions.Handlers;

namespace TaleWorlds.MountAndBlade
{
  public class MissionAgentSpawnLogic : MissionLogic, IMissionAgentSpawnLogic, IMissionBehavior
  {
    private OnPhaseChangedDelegate[] _onPhaseChanged = new OnPhaseChangedDelegate[2];
    private List<MissionAgentSpawnLogic.SpawnPhase>[] _phases;
    private static int _maxNumberOfAgentsForMissionCache;
    private const float BattleSizeRespawnThreshold = 0.1f;
    private const float SpawnEnforcementOnLossPercentage = 0.5f;
    private readonly int[] _numberOfTroopsInTotal;
    private readonly int[] _numberOfTroopsInQueueForReinforcement;
    private readonly int[] _numberOfTroopsToSpawnPerSideFormation;
    private readonly int _battleSize;
    private readonly MissionAgentSpawnLogic.MissionSide[] _missionSides;

    private MissionAgentSpawnLogic.SpawnPhase DefenderActivePhase => this._phases[0].FirstOrDefault<MissionAgentSpawnLogic.SpawnPhase>();

    private MissionAgentSpawnLogic.SpawnPhase AttackerActivePhase => this._phases[1].FirstOrDefault<MissionAgentSpawnLogic.SpawnPhase>();

    private static int MaxNumberOfAgentsForMission
    {
      get
      {
        if (MissionAgentSpawnLogic._maxNumberOfAgentsForMissionCache == 0)
          MissionAgentSpawnLogic._maxNumberOfAgentsForMissionCache = MBAPI.IMBAgent.GetMaximumNumberOfAgents();
        return MissionAgentSpawnLogic._maxNumberOfAgentsForMissionCache;
      }
    }

    private static int MaxNumberOfTroopsForMission => MissionAgentSpawnLogic.MaxNumberOfAgentsForMission / 2;

    private float BattleSizeEffect => MBMath.ClampFloat((float) this._battleSize / (float) (this.NumberOfActiveTroops + ((IEnumerable<int>) this._numberOfTroopsInQueueForReinforcement).Sum()), 0.0f, 1f);

    public int NumberOfActiveTroops => ((IEnumerable<MissionAgentSpawnLogic.MissionSide>) this._missionSides).Sum<MissionAgentSpawnLogic.MissionSide>((Func<MissionAgentSpawnLogic.MissionSide, int>) (ms => ms.NumberOfActiveTroops));

    public int NumRemovedTroops => ((IEnumerable<MissionAgentSpawnLogic.MissionSide>) this._missionSides).Sum<MissionAgentSpawnLogic.MissionSide>((Func<MissionAgentSpawnLogic.MissionSide, int>) (ms => ms.NumRemovedTroops));

    public int NumberOfTroopsCanBeSpawned => this._battleSize - (this.NumberOfActiveTroops + ((IEnumerable<int>) this._numberOfTroopsInQueueForReinforcement).Sum());

    public int NumberOfTroopsCanBeSpawnedFromDefenders => this._numberOfTroopsInQueueForReinforcement[0];

    public int NumberOfRemainingTroops => this.DefenderActivePhase.RemainingSpawnNumber + this.AttackerActivePhase.RemainingSpawnNumber;

    public int NumberOfActiveDefenderTroops
    {
      get
      {
        MissionAgentSpawnLogic.SpawnPhase defenderActivePhase = this.DefenderActivePhase;
        return defenderActivePhase == null ? 0 : defenderActivePhase.NumberActiveTroops;
      }
    }

    public int NumberOfActiveAttackerTroops
    {
      get
      {
        MissionAgentSpawnLogic.SpawnPhase attackerActivePhase = this.AttackerActivePhase;
        return attackerActivePhase == null ? 0 : attackerActivePhase.NumberActiveTroops;
      }
    }

    public bool IsInitialSpawnOver => this.Mission.GetMissionBehaviour<SiegeDeploymentHandler>() == null && this.DefenderActivePhase.InitialSpawnNumber + this.AttackerActivePhase.InitialSpawnNumber == 0;

    public MissionAgentSpawnLogic(IMissionTroopSupplier[] suppliers, BattleSideEnum playerSide)
    {
      this._battleSize = Math.Min(BannerlordConfig.GetRealBattleSize(), MissionAgentSpawnLogic.MaxNumberOfTroopsForMission);
      this._missionSides = new MissionAgentSpawnLogic.MissionSide[2];
      for (int index = 0; index < 2; ++index)
      {
        IMissionTroopSupplier supplier = suppliers[index];
        bool isPlayerSide = (BattleSideEnum) index == playerSide;
        this._missionSides[index] = new MissionAgentSpawnLogic.MissionSide((BattleSideEnum) index, supplier, isPlayerSide);
      }
      this._numberOfTroopsInTotal = new int[2];
      this._numberOfTroopsInQueueForReinforcement = new int[2];
      this._numberOfTroopsToSpawnPerSideFormation = new int[11];
      this._phases = new List<MissionAgentSpawnLogic.SpawnPhase>[2];
      for (int index = 0; index < 2; ++index)
        this._phases[index] = new List<MissionAgentSpawnLogic.SpawnPhase>();
    }

    public void InitWithSinglePhase(
      int defenderTotalSpawn,
      int attackerTotalSpawn,
      int defenderInitialSpawn,
      int attackerInitialSpawn,
      bool spawnDefenders,
      bool spawnAttackers,
      float defenderAdvantageFactor = 1f)
    {
      this.AddPhase(BattleSideEnum.Defender, defenderTotalSpawn, defenderInitialSpawn);
      this.AddPhase(BattleSideEnum.Attacker, attackerTotalSpawn, attackerInitialSpawn);
      this.Init(spawnDefenders, spawnAttackers, defenderAdvantageFactor);
    }

    public void ReserveReinforcement(BattleSideEnum side, int numberOfTroops)
    {
      this._numberOfTroopsInQueueForReinforcement[(int) side] += numberOfTroops;
      foreach (MissionAgentSpawnLogic.SpawnPhase spawnPhase in this._phases[(int) side])
      {
        int num = Math.Min(numberOfTroops / this._phases[(int) side].Count, numberOfTroops);
        spawnPhase.RemainingSpawnNumber -= num;
        numberOfTroops -= num;
      }
    }

    public void SetSpawnTroops(BattleSideEnum side, bool spawnTroops, bool enforceSpawning = false)
    {
      this._missionSides[(int) side].SetSpawnTroops(spawnTroops);
      if (!(spawnTroops & enforceSpawning))
        return;
      this.CheckInitialSpawns();
    }

    public void SetSpawnHorses(BattleSideEnum side, bool spawnHorses)
    {
      this._missionSides[(int) side].SetSpawnWithHorses(spawnHorses);
      this.Mission.SetDeploymentPlanSpawnWithHorses(side, spawnHorses);
    }

    public void StopSpawner()
    {
      this._missionSides[0].SetSpawnTroops(false);
      this._missionSides[1].SetSpawnTroops(false);
    }

    internal void OnInitialSpawnForSideEnded(BattleSideEnum side)
    {
      foreach (Team team in this.Mission.Teams.Where<Team>((Func<Team, bool>) (t => t.Side == side)))
        team.OnFormationUnitsSpawned();
      foreach (Team team in this.Mission.Teams.Where<Team>((Func<Team, bool>) (t => t.Side == side)))
      {
        foreach (Formation formation in team.Formations)
          formation.QuerySystem.EvaluateAllPreliminaryQueryData();
        team.MasterOrderController.OnOrderIssued += new OnOrderIssuedDelegate(this.OrderController_OnOrderIssued);
        if (team.SpecialFormations.Any<Formation>((Func<Formation, bool>) (f => f.CountOfUnits > 0)))
        {
          Agent generalAgent = team.GeneralAgent;
          foreach (Formation formation in team.SpecialFormations.Where<Formation>((Func<Formation, bool>) (f => f.CountOfUnits > 0)))
          {
            team.MasterOrderController.SelectFormation(formation);
            team.MasterOrderController.SetOrderWithAgent(OrderType.FollowMe, team.GeneralAgent);
            team.MasterOrderController.ClearSelectedFormations();
            formation.IsAIControlled = true;
          }
        }
        team.MasterOrderController.OnOrderIssued -= new OnOrderIssuedDelegate(this.OrderController_OnOrderIssued);
      }
    }

    private bool CheckInitialSpawns()
    {
      if (!this.IsInitialSpawnOver)
      {
        if (!this.Mission.IsDeploymentPlanMade())
        {
          for (int index1 = 0; index1 < 2; ++index1)
          {
            MissionAgentSpawnLogic.SpawnPhase activePhaseForSide = this.GetActivePhaseForSide((BattleSideEnum) index1);
            if (activePhaseForSide.InitialSpawnNumber > 0 && this._missionSides[index1].TroopSpawningActive)
            {
              this._missionSides[index1].PreSupplyTroops(activePhaseForSide.InitialSpawnNumber);
              this._missionSides[index1].GetPreSuppliedFormationTroopCounts(this._numberOfTroopsToSpawnPerSideFormation);
              for (int index2 = 0; index2 < this._numberOfTroopsToSpawnPerSideFormation.Length; ++index2)
              {
                if (this._numberOfTroopsToSpawnPerSideFormation[index2] > 0)
                  this.Mission.AddTroopsToDeploymentPlan((BattleSideEnum) index1, (FormationClass) index2, this._numberOfTroopsToSpawnPerSideFormation[index2]);
              }
            }
          }
          this.Mission.MakeDeploymentPlan();
        }
        List<int> intList = new List<int>();
        for (int index = 0; index < 2; ++index)
        {
          MissionAgentSpawnLogic.SpawnPhase activePhaseForSide = this.GetActivePhaseForSide((BattleSideEnum) index);
          if (activePhaseForSide.InitialSpawnNumber > 0 && this._missionSides[index].TroopSpawningActive)
          {
            this._missionSides[index].SpawnTroops(activePhaseForSide.InitialSpawnNumber, false, true);
            this.GetActivePhaseForSide((BattleSideEnum) index).OnInitialTroopsSpawned();
            intList.Add(index);
          }
        }
        if (this.IsInitialSpawnOver)
        {
          foreach (int index in intList)
          {
            this._missionSides[index].OnInitialSpawnOver();
            this.OnInitialSpawnForSideEnded((BattleSideEnum) index);
          }
        }
      }
      return this.IsInitialSpawnOver;
    }

    public void CheckReinforcement(int numberOfTroops)
    {
      if (!this.IsInitialSpawnOver)
        return;
      for (int index = 0; index < 2; ++index)
      {
        int val1 = numberOfTroops;
        if (val1 > 0 && this._numberOfTroopsInQueueForReinforcement[index] > 0 && this._missionSides[index].TroopSpawningActive)
        {
          int number = Math.Min(MBMath.Floor((float) val1 * this.BattleSizeEffect), this._numberOfTroopsInQueueForReinforcement[index]);
          this._missionSides[index].SpawnTroops(number, true);
          InformationManager.AddQuickInformation(this._missionSides[index].IsPlayerSide ? GameTexts.FindText("str_new_reinforcements_have_arrived_for_ally_side") : GameTexts.FindText("str_new_reinforcements_have_arrived_for_enemy_side"));
          MatrixFrame cameraFrame = Mission.Current.GetCameraFrame();
          Vec3 position = cameraFrame.origin + cameraFrame.rotation.u;
          MBSoundEvent.PlaySound(this._missionSides[index].IsPlayerSide ? SoundEvent.GetEventIdFromString("event:/alerts/report/reinforcements_ally") : SoundEvent.GetEventIdFromString("event:/alerts/report/reinforcements_enemy"), position);
          int num1 = Math.Min(val1, this._numberOfTroopsInQueueForReinforcement[index]);
          this._numberOfTroopsInQueueForReinforcement[index] -= num1;
          int num2 = num1 - number;
          this.GetActivePhaseForSide((BattleSideEnum) index).RemainingSpawnNumber += num2;
          Debug.Print("Side: " + (object) (BattleSideEnum) index, color: Debug.DebugColor.Green, debugFilter: 64UL);
          Debug.Print("NumberOfTroopsRemovedFromQueue: " + (object) num1, color: Debug.DebugColor.Green, debugFilter: 64UL);
          Debug.Print("NumberOfTroopsAddedToRemainingPool: " + (object) num2, color: Debug.DebugColor.Green, debugFilter: 64UL);
        }
      }
    }

    public override void OnMissionTick(float dt)
    {
      if (!MBNetwork.IsClient && !this.CheckInitialSpawns())
        return;
      this.PhaseTick();
      this.BattleSizeSpawnTick();
    }

    public int GetTotalNumberOfTroopsForSide(BattleSideEnum side) => this._numberOfTroopsInTotal[(int) side];

    private void OrderController_OnOrderIssued(
      OrderType orderType,
      IEnumerable<Formation> appliedFormations,
      params object[] delegateParams)
    {
      DeploymentHandler.OrderController_OnOrderIssued_Aux(orderType, appliedFormations, delegateParams);
    }

    private void BattleSizeSpawnTick()
    {
      int troopsCanBeSpawned = this.NumberOfTroopsCanBeSpawned;
      if (this.NumberOfRemainingTroops <= 0 || troopsCanBeSpawned <= 0)
        return;
      int num1 = this.DefenderActivePhase.TotalSpawnNumber + this.AttackerActivePhase.TotalSpawnNumber;
      int number1 = MBMath.Round((float) troopsCanBeSpawned * (float) this.DefenderActivePhase.TotalSpawnNumber / (float) num1);
      int number2 = troopsCanBeSpawned - number1;
      float num2 = (float) (this.DefenderActivePhase.InitialSpawnedNumber - this._missionSides[0].NumberOfActiveTroops) / (float) this.DefenderActivePhase.InitialSpawnedNumber;
      float num3 = (float) (this.AttackerActivePhase.InitialSpawnedNumber - this._missionSides[1].NumberOfActiveTroops) / (float) this.AttackerActivePhase.InitialSpawnedNumber;
      if (MissionAgentSpawnLogic.MaxNumberOfAgentsForMission - this.Mission.AllAgents.Count < troopsCanBeSpawned * 2 || (double) troopsCanBeSpawned < (double) this._battleSize * 0.100000001490116 && (double) num2 < 0.5 && (double) num3 < 0.5)
        return;
      float val = MBMath.ClampFloat(num2 / 0.5f - num3 / 0.5f, -1f, 1f);
      if ((double) val > 0.0)
      {
        int num4 = MBMath.Round((float) number2 * val);
        number2 -= num4;
        number1 += num4;
      }
      else if ((double) val < 0.0)
      {
        float num4 = MBMath.Absf(val);
        int num5 = MBMath.Round((float) number1 * num4);
        number1 -= num5;
        number2 += num5;
      }
      int num6 = Math.Max(number1 - this.DefenderActivePhase.RemainingSpawnNumber, 0);
      int num7 = Math.Max(number2 - this.AttackerActivePhase.RemainingSpawnNumber, 0);
      if (num6 > 0 && num7 > 0)
      {
        number1 = this.DefenderActivePhase.RemainingSpawnNumber;
        number2 = this.AttackerActivePhase.RemainingSpawnNumber;
      }
      else if (num6 > 0)
      {
        number1 = this.DefenderActivePhase.RemainingSpawnNumber;
        number2 = Math.Min(number2 + num6, this.AttackerActivePhase.RemainingSpawnNumber);
      }
      else if (num7 > 0)
      {
        number2 = this.AttackerActivePhase.RemainingSpawnNumber;
        number1 = Math.Min(number1 + num7, this.DefenderActivePhase.RemainingSpawnNumber);
      }
      if (this._missionSides[0].TroopSpawningActive && number1 > 0)
        this.DefenderActivePhase.RemainingSpawnNumber -= this._missionSides[0].SpawnTroops(number1, true, true);
      if (!this._missionSides[1].TroopSpawningActive || number2 <= 0)
        return;
      this.AttackerActivePhase.RemainingSpawnNumber -= this._missionSides[1].SpawnTroops(number2, true, true);
    }

    public bool IsSideDepleted(BattleSideEnum side) => this._phases[(int) side].Count == 1 && this._missionSides[(int) side].NumberOfActiveTroops == 0 && this.GetActivePhaseForSide(side).RemainingSpawnNumber == 0 && this._numberOfTroopsInQueueForReinforcement[(int) side] == 0;

    private MissionAgentSpawnLogic.SpawnPhase GetActivePhaseForSide(
      BattleSideEnum side)
    {
      if (side == BattleSideEnum.Defender)
        return this.DefenderActivePhase;
      if (side == BattleSideEnum.Attacker)
        return this.AttackerActivePhase;
      throw new Exception("Wrong side.");
    }

    public void AddPhase(BattleSideEnum side, int totalSpawn, int initialSpawn)
    {
      MissionAgentSpawnLogic.SpawnPhase spawnPhase = new MissionAgentSpawnLogic.SpawnPhase()
      {
        TotalSpawnNumber = totalSpawn,
        InitialSpawnNumber = initialSpawn,
        RemainingSpawnNumber = totalSpawn - initialSpawn
      };
      this._phases[(int) side].Add(spawnPhase);
      this._numberOfTroopsInTotal[(int) side] += totalSpawn;
    }

    public void Init(bool spawnDefenders, bool spawnAttackers, float defenderAdvantageFactor = 1f)
    {
      foreach (IEnumerable<MissionAgentSpawnLogic.SpawnPhase> phase in this._phases)
      {
        if (!phase.Any<MissionAgentSpawnLogic.SpawnPhase>())
          throw new Exception("There must be at least 1 phase (initial phase) on each side before Init was called.");
      }
      int[] numArray1 = new int[2]
      {
        this._phases[0].Sum<MissionAgentSpawnLogic.SpawnPhase>((Func<MissionAgentSpawnLogic.SpawnPhase, int>) (p => p.TotalSpawnNumber)),
        this._phases[1].Sum<MissionAgentSpawnLogic.SpawnPhase>((Func<MissionAgentSpawnLogic.SpawnPhase, int>) (p => p.TotalSpawnNumber))
      };
      int num1 = ((IEnumerable<int>) numArray1).Sum();
      float[] numArray2 = new float[2]
      {
        (float) numArray1[0] / (float) num1,
        (float) numArray1[1] / (float) num1
      };
      if ((double) numArray2[0] < 0.600000023841858)
      {
        numArray2[0] *= defenderAdvantageFactor;
        if ((double) numArray2[0] > 0.600000023841858)
          numArray2[0] = 0.6f;
        numArray2[1] = 1f - numArray2[0];
      }
      BattleSideEnum side = (double) numArray2[0] < (double) numArray2[1] ? BattleSideEnum.Defender : BattleSideEnum.Attacker;
      BattleSideEnum oppositeSide = side.GetOppositeSide();
      int[] numArray3 = new int[2];
      numArray3[(int) side] = MBMath.Ceiling(numArray2[(int) side] * (float) this._battleSize);
      numArray3[(int) oppositeSide] = this._battleSize - numArray3[(int) side];
      for (int index = 0; index < 2; ++index)
      {
        foreach (MissionAgentSpawnLogic.SpawnPhase spawnPhase in this._phases[index])
        {
          if (spawnPhase.InitialSpawnNumber > numArray3[index])
          {
            int num2 = numArray3[index];
            int num3 = spawnPhase.InitialSpawnNumber - num2;
            spawnPhase.InitialSpawnNumber = num2;
            spawnPhase.RemainingSpawnNumber += num3;
          }
        }
      }
      Mission.Current.SetBattleAgentCount(Math.Min(this.DefenderActivePhase.InitialSpawnNumber, this.AttackerActivePhase.InitialSpawnNumber));
      this._missionSides[0].SetSpawnTroops(spawnDefenders);
      this._missionSides[1].SetSpawnTroops(spawnAttackers);
    }

    private void PhaseTick()
    {
      bool flag = false;
      for (int index = 0; index < 2; ++index)
      {
        MissionAgentSpawnLogic.SpawnPhase activePhaseForSide = this.GetActivePhaseForSide((BattleSideEnum) index);
        activePhaseForSide.NumberActiveTroops = this._missionSides[index].NumberOfActiveTroops;
        if (activePhaseForSide.NumberActiveTroops == 0 && activePhaseForSide.RemainingSpawnNumber == 0 && this._phases[index].Count > 1)
        {
          this._phases[index].Remove(activePhaseForSide);
          if (this.GetActivePhaseForSide((BattleSideEnum) index) != null)
          {
            flag = true;
            if (this._onPhaseChanged[index] != null)
              this._onPhaseChanged[index]();
            Debug.Print("New spawn phase!", color: Debug.DebugColor.Green, debugFilter: 64UL);
          }
        }
      }
      if (!flag || !this.Mission.IsDeploymentPlanMade())
        return;
      this.Mission.UnmakeDeploymentPlan();
    }

    public void AddPhaseChangeAction(BattleSideEnum side, OnPhaseChangedDelegate onPhaseChanged) => this._onPhaseChanged[(int) side] += onPhaseChanged;

    private class MissionSide
    {
      private readonly BattleSideEnum _side;
      private readonly IMissionTroopSupplier _troopSupplier;
      private bool _spawnWithHorses;
      private readonly MBList<Formation> _spawnedFormations;
      private List<IAgentOriginBase> _preSuppliedTroops;

      public bool TroopSpawningActive { get; private set; }

      public bool IsPlayerSide { get; }

      public int NumberOfActiveTroops => this._troopSupplier.NumActiveTroops;

      public int NumRemovedTroops => this._troopSupplier.NumRemovedTroops;

      public MissionSide(
        BattleSideEnum side,
        IMissionTroopSupplier troopSupplier,
        bool isPlayerSide)
      {
        this._side = side;
        this._spawnWithHorses = true;
        this._spawnedFormations = new MBList<Formation>();
        this._preSuppliedTroops = new List<IAgentOriginBase>();
        this.IsPlayerSide = isPlayerSide;
        this._troopSupplier = troopSupplier;
      }

      public void PreSupplyTroops(int number)
      {
        if (number <= 0)
          return;
        this._preSuppliedTroops.AddRange(this._troopSupplier.SupplyTroops(number));
      }

      public void GetPreSuppliedFormationTroopCounts(int[] formationTroopCounts)
      {
        if (formationTroopCounts == null || formationTroopCounts.Length != 11)
          return;
        for (int index = 0; index < formationTroopCounts.Length; ++index)
          formationTroopCounts[index] = 0;
        foreach (IAgentOriginBase preSuppliedTroop in this._preSuppliedTroops)
        {
          FormationClass formationClass = preSuppliedTroop.Troop.GetFormationClass(preSuppliedTroop.BattleCombatant);
          ++formationTroopCounts[(int) formationClass];
        }
      }

      public int SpawnTroops(int number, bool isReinforcement, bool enforceSpawningOnInitialPoint = false)
      {
        if (number <= 0)
          return 0;
        int formationTroopIndex = 0;
        List<IAgentOriginBase> agentOriginBaseList1 = new List<IAgentOriginBase>();
        int count1 = Math.Min(this._preSuppliedTroops.Count, number);
        if (count1 > 0)
        {
          for (int index = 0; index < count1; ++index)
            agentOriginBaseList1.Add(this._preSuppliedTroops[index]);
          this._preSuppliedTroops.RemoveRange(0, count1);
        }
        int numberToAllocate = number - count1;
        agentOriginBaseList1.AddRange(this._troopSupplier.SupplyTroops(numberToAllocate));
        List<IAgentOriginBase> agentOriginBaseList2 = new List<IAgentOriginBase>();
        for (int index = 0; index < 8; ++index)
        {
          agentOriginBaseList2.Clear();
          IAgentOriginBase agentOriginBase1 = (IAgentOriginBase) null;
          FormationClass formationClass = (FormationClass) index;
          foreach (IAgentOriginBase agentOriginBase2 in agentOriginBaseList1)
          {
            if (formationClass == agentOriginBase2.Troop.GetFormationClass(agentOriginBase2.BattleCombatant))
            {
              if (agentOriginBase2.Troop == Game.Current.PlayerTroop)
                agentOriginBase1 = agentOriginBase2;
              else
                agentOriginBaseList2.Add(agentOriginBase2);
            }
          }
          if (agentOriginBase1 != null)
            agentOriginBaseList2.Add(agentOriginBase1);
          int count2 = agentOriginBaseList2.Count;
          if (count2 > 0)
          {
            foreach (IAgentOriginBase troopOrigin in agentOriginBaseList2)
            {
              Formation formation = Mission.GetAgentTeam(troopOrigin, this.IsPlayerSide).GetFormation(formationClass);
              bool isMounted = this._spawnWithHorses && (formationClass == FormationClass.Cavalry || formationClass == FormationClass.LightCavalry || formationClass == FormationClass.HeavyCavalry || formationClass == FormationClass.HorseArcher);
              if (formation != null && !formation.HasBeenPositioned)
              {
                formation.BeginSpawn(count2, isMounted);
                Mission.Current.SpawnFormation(formation, count2, this._spawnWithHorses, isMounted, isReinforcement);
                this._spawnedFormations.Add(formation);
              }
              Mission.Current.SpawnTroop(troopOrigin, this.IsPlayerSide, true, this._spawnWithHorses, isReinforcement, enforceSpawningOnInitialPoint, count2, formationTroopIndex, true, true);
              ++formationTroopIndex;
            }
          }
        }
        if (formationTroopIndex > 0)
        {
          foreach (Team team in (ReadOnlyCollection<Team>) Mission.Current.Teams)
            team.QuerySystem.Expire();
          Debug.Print(formationTroopIndex.ToString() + " troops spawned on " + (object) this._side + " side.", color: Debug.DebugColor.DarkGreen, debugFilter: 64UL);
        }
        foreach (Team team in (ReadOnlyCollection<Team>) Mission.Current.Teams)
        {
          foreach (Formation formation in team.Formations)
            formation.GroupSpawnIndex = 0;
        }
        return formationTroopIndex;
      }

      public void SetSpawnWithHorses(bool spawnWithHorses) => this._spawnWithHorses = spawnWithHorses;

      public void SetSpawnTroops(bool spawnTroops) => this.TroopSpawningActive = spawnTroops;

      public void OnInitialSpawnOver()
      {
        foreach (Formation spawnedFormation in this._spawnedFormations)
          spawnedFormation.EndSpawn();
      }
    }

    private class SpawnPhase
    {
      public int TotalSpawnNumber;
      public int InitialSpawnedNumber;
      public int InitialSpawnNumber;
      public int RemainingSpawnNumber;
      public int NumberActiveTroops;

      public void OnInitialTroopsSpawned()
      {
        this.InitialSpawnedNumber = this.InitialSpawnNumber;
        this.InitialSpawnNumber = 0;
      }
    }
  }
}
