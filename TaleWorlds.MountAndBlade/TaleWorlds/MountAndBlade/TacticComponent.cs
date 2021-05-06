// Decompiled with JetBrains decompiler
// Type: TaleWorlds.MountAndBlade.TacticComponent
// Assembly: TaleWorlds.MountAndBlade, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3C0E3F42-756F-4407-B817-06BA412A78B0
// Assembly location: E:\SteamLibrary\steamapps\common\Mount & Blade II Bannerlord\bin\Win64_Shipping_Client\TaleWorlds.MountAndBlade.dll

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using TaleWorlds.Core;
using TaleWorlds.Engine;
using TaleWorlds.Library;

namespace TaleWorlds.MountAndBlade
{
  public abstract class TacticComponent
  {
    public static readonly int MoveHornSoundIndex = SoundEvent.GetEventIdFromString("event:/ui/mission/horns/move");
    public static readonly int AttackHornSoundIndex = SoundEvent.GetEventIdFromString("event:/ui/mission/horns/attack");
    public static readonly int RetreatHornSoundIndex = SoundEvent.GetEventIdFromString("event:/ui/mission/horns/retreat");
    protected readonly Team team;
    protected bool IsTacticReapplyNeeded;
    private bool _areFormationsCreated;
    protected Formation _mainInfantry;
    protected Formation _archers;
    protected Formation _leftCavalry;
    protected Formation _rightCavalry;
    protected Formation _rangedCavalry;
    private float _hornCooldownExpireTime;
    private const float HornCooldownTime = 10f;

    protected IEnumerable<Formation> Formations => this.team.Formations;

    protected TacticComponent(Team team) => this.team = team;

    protected internal virtual void OnApply()
    {
    }

    protected internal virtual void OnCancel()
    {
    }

    protected internal virtual void TickOccasionally()
    {
      if (this.team.TeamAI.GetIsFirstTacticChosen)
        this.team.TeamAI.OnTacticAppliedForFirstTime();
      if (!Mission.Current.IsMissionEnding)
        return;
      this.StopUsingAllMachines();
    }

    private static void GetUnitCountByAttackType(
      Formation formation,
      out int unitCount,
      out int rangedCount,
      out int semiRangedCount,
      out int nonRangedCount)
    {
      FormationQuerySystem querySystem = formation.QuerySystem;
      unitCount = formation.CountOfUnits;
      rangedCount = (int) ((double) querySystem.RangedUnitRatio * (double) unitCount);
      semiRangedCount = 0;
      nonRangedCount = unitCount - rangedCount;
    }

    protected static float GetFormationGroupEffectivenessOverOrder(
      IEnumerable<Formation> formationGroup,
      OrderType orderType,
      IOrderable targetObject = null)
    {
      int unitCount;
      int rangedCount;
      int semiRangedCount;
      int nonRangedCount;
      TacticComponent.GetUnitCountByAttackType(formationGroup.FirstOrDefault<Formation>(), out unitCount, out rangedCount, out semiRangedCount, out nonRangedCount);
      switch (orderType)
      {
        case OrderType.Charge:
          return (float) ((double) nonRangedCount * 1.0 + (double) semiRangedCount * 0.899999976158142 + (double) rangedCount * 0.300000011920929) / (float) unitCount;
        case OrderType.Use:
          double num1 = ((double) nonRangedCount * 1.0 + (double) semiRangedCount * 0.899999976158142 + (double) rangedCount * 0.300000011920929) / (double) unitCount;
          int maxUserCount = (targetObject as UsableMachine).MaxUserCount;
          double num2 = (double) Math.Min((float) unitCount * 1f / (float) maxUserCount, 1f);
          return (float) (num1 * num2);
        case OrderType.PointDefence:
          double num3 = ((double) nonRangedCount * 0.100000001490116 + (double) semiRangedCount * 0.300000011920929 + (double) rangedCount * 1.0) / (double) unitCount;
          int num4 = (targetObject as IPointDefendable).DefencePoints.Count<DefencePoint>() * 3;
          double num5 = (double) Math.Min((float) unitCount * 1f / (float) num4, 1f);
          return (float) (num3 * num5);
        default:
          return 1f;
      }
    }

    protected static float GetFormationEffectivenessOverOrder(
      Formation formation,
      OrderType orderType,
      IOrderable targetObject = null)
    {
      int unitCount;
      int rangedCount;
      int semiRangedCount;
      int nonRangedCount;
      TacticComponent.GetUnitCountByAttackType(formation, out unitCount, out rangedCount, out semiRangedCount, out nonRangedCount);
      switch (orderType)
      {
        case OrderType.Charge:
          return (float) ((double) nonRangedCount * 1.0 + (double) semiRangedCount * 0.899999976158142 + (double) rangedCount * 0.300000011920929) / (float) unitCount;
        case OrderType.Use:
          float minDistanceSquared = float.MaxValue;
          formation.ApplyActionOnEachUnit((Action<Agent>) (agent => minDistanceSquared = Math.Min(agent.Position.DistanceSquared((targetObject as UsableMachine).GameEntity.GlobalPosition), minDistanceSquared)));
          return 1f / MBMath.ClampFloat(minDistanceSquared, 1f, float.MaxValue);
        case OrderType.PointDefence:
          double num1 = ((double) nonRangedCount * 0.100000001490116 + (double) semiRangedCount * 0.300000011920929 + (double) rangedCount * 1.0) / (double) unitCount;
          int num2 = (targetObject as IPointDefendable).DefencePoints.Count<DefencePoint>() * 3;
          double num3 = (double) Math.Min((float) unitCount * 1f / (float) num2, 1f);
          return (float) (num1 * num3);
        default:
          return 1f;
      }
    }

    [Conditional("DEBUG")]
    protected internal virtual void DebugTick(float dt)
    {
    }

    private static int GetAvailableUnitCount(
      Formation formation,
      IEnumerable<(Formation, UsableMachine, int)> appliedCombinations)
    {
      int num1 = appliedCombinations.Where<(Formation, UsableMachine, int)>((Func<(Formation, UsableMachine, int), bool>) (c => c.Item1 == formation)).Sum<(Formation, UsableMachine, int)>((Func<(Formation, UsableMachine, int), int>) (c => c.Item3));
      int num2 = 0;
      return formation.CountOfUnits - (num1 + num2);
    }

    private static int GetVacantSlotCount(
      UsableMachine weapon,
      IEnumerable<(Formation, UsableMachine, int)> appliedCombinations)
    {
      int num = appliedCombinations.Where<(Formation, UsableMachine, int)>((Func<(Formation, UsableMachine, int), bool>) (c => c.Item2 == weapon)).Sum<(Formation, UsableMachine, int)>((Func<(Formation, UsableMachine, int), int>) (c => c.Item3));
      return weapon.MaxUserCount - num;
    }

    protected static void AssignUnitsToWeapons(
      IEnumerable<Formation> formations,
      IEnumerable<UsableMachine> weapons,
      List<(Formation, UsableMachine, int)> appliedCombinations = null)
    {
      if (appliedCombinations == null)
        appliedCombinations = new List<(Formation, UsableMachine, int)>();
      foreach (Formation formation1 in formations.Where<Formation>((Func<Formation, bool>) (f => TacticComponent.GetAvailableUnitCount(f, (IEnumerable<(Formation, UsableMachine, int)>) appliedCombinations) == 0)))
      {
        Formation formation = formation1;
        foreach (UsableMachine usable in weapons.Where<UsableMachine>((Func<UsableMachine, bool>) (w => formation.IsUsingMachine(w))))
          formation.StopUsingMachine(usable);
      }
      List<Tuple<UsableMachine, Formation>> source = weapons.CombineWith<UsableMachine, Formation>(formations.Where<Formation>((Func<Formation, bool>) (f => (uint) TacticComponent.GetAvailableUnitCount(f, (IEnumerable<(Formation, UsableMachine, int)>) appliedCombinations) > 0U)));
      while (!source.IsEmpty<Tuple<UsableMachine, Formation>>())
      {
        Tuple<UsableMachine, Formation> tuple = source.MaxBy<Tuple<UsableMachine, Formation>, float>((Func<Tuple<UsableMachine, Formation>, float>) (c => (float) ((double) TacticComponent.GetFormationEffectivenessOverOrder(c.Item2, OrderType.Use, (IOrderable) c.Item1) * (c.Item2.IsUsingMachine(c.Item1) ? 1.5 : 1.0) * (0.899999976158142 + (double) MBRandom.RandomFloat * 0.200000002980232))));
        UsableMachine siegeWeapon = tuple.Item1;
        Formation formation = tuple.Item2;
        if (!formation.IsUsingMachine(siegeWeapon))
          formation.StartUsingMachine(siegeWeapon);
        int vacantSlotCount = TacticComponent.GetVacantSlotCount(siegeWeapon, (IEnumerable<(Formation, UsableMachine, int)>) appliedCombinations);
        int availableUnitCount = TacticComponent.GetAvailableUnitCount(formation, (IEnumerable<(Formation, UsableMachine, int)>) appliedCombinations);
        int num1 = Math.Min(vacantSlotCount, availableUnitCount);
        source.Remove(tuple);
        appliedCombinations.Add(ValueTuple.Create<Formation, UsableMachine, int>(formation, siegeWeapon, num1));
        int num2 = vacantSlotCount - num1;
        int num3 = availableUnitCount - num1;
        if (num2 == 0)
        {
          source.RemoveAll((Predicate<Tuple<UsableMachine, Formation>>) (c => c.Item1 == siegeWeapon));
          foreach (Formation formation1 in formations.Where<Formation>((Func<Formation, bool>) (f => f.IsUsingMachine(siegeWeapon))).Except<Formation>(appliedCombinations.Where<(Formation, UsableMachine, int)>((Func<(Formation, UsableMachine, int), bool>) (c => c.Item2 == siegeWeapon)).Select<(Formation, UsableMachine, int), Formation>((Func<(Formation, UsableMachine, int), Formation>) (c => c.Item1))))
            formation1.StopUsingMachine(siegeWeapon);
        }
        if (num3 == 0)
          source.RemoveAll((Predicate<Tuple<UsableMachine, Formation>>) (c => c.Item2 == formation));
      }
      foreach (Formation formation1 in formations)
      {
        Formation formation = formation1;
        foreach (UsableMachine usable in formation.GetUsedMachines().Where<UsableMachine>((Func<UsableMachine, bool>) (um => um is SiegeWeapon)).Except<UsableMachine>(appliedCombinations.Where<(Formation, UsableMachine, int)>((Func<(Formation, UsableMachine, int), bool>) (c => c.Item1 == formation)).Select<(Formation, UsableMachine, int), UsableMachine>((Func<(Formation, UsableMachine, int), UsableMachine>) (c => c.Item2))).ToList<UsableMachine>())
          formation.StopUsingMachine(usable);
      }
    }

    protected List<Formation> ConsolidateFormations(
      List<Formation> formationsToBeConsolidated,
      int neededCount)
    {
      if (formationsToBeConsolidated.Count <= neededCount || neededCount <= 0)
        return formationsToBeConsolidated;
      List<Formation> list1 = formationsToBeConsolidated.OrderByDescending<Formation, int>((Func<Formation, int>) (f => f.CountOfUnits + (!f.IsAIControlled ? 10000 : 0))).ToList<Formation>();
      List<Formation> list2 = list1.Take<Formation>(neededCount).Reverse<Formation>().ToList<Formation>();
      list1.RemoveRange(0, neededCount);
      Queue<Formation> source = new Queue<Formation>((IEnumerable<Formation>) list2);
      List<Formation> formationList = new List<Formation>();
      foreach (Formation formation in list1)
      {
        if (!formation.IsAIControlled)
        {
          formationList.Add(formation);
        }
        else
        {
          if (source.IsEmpty<Formation>())
            source = new Queue<Formation>((IEnumerable<Formation>) list2);
          Formation target = source.Dequeue();
          formation.TransferUnits(target, formation.CountOfUnits);
        }
      }
      return list2.Concat<Formation>((IEnumerable<Formation>) formationList).ToList<Formation>();
    }

    protected static float CalculateNotEngagingTacticalAdvantage(TeamQuerySystem team)
    {
      float val1 = team.CavalryRatio + team.RangedCavalryRatio;
      float val2 = team.EnemyCavalryRatio + team.EnemyRangedCavalryRatio;
      return (float) Math.Pow((double) MBMath.ClampFloat((double) val1 > 0.0 ? val2 / val1 : 1.5f, 1f, 1.5f), 1.5 * (double) Math.Max(val1, val2));
    }

    protected void SplitFormationClassIntoGivenNumber(
      Func<Formation, bool> formationClass,
      int count)
    {
      IEnumerable<Formation> formations = this.Formations.Where<Formation>((Func<Formation, bool>) (f => formationClass(f)));
      List<Formation> list = formations.Where<Formation>((Func<Formation, bool>) (f => !f.IsSplittableByAI)).ToList<Formation>();
      IEnumerable<Formation> source1 = formations.Except<Formation>((IEnumerable<Formation>) list);
      int count1 = count - list.Count;
      if (!source1.Any<Formation>() || count1 > 1 && source1.Count<Formation>() == count1)
        return;
      Formation biggestFormation = source1.MaxBy<Formation, int>((Func<Formation, int>) (f => f.CountOfUnits));
      foreach (Formation formation in source1.Where<Formation>((Func<Formation, bool>) (f => f != biggestFormation)))
        formation.TransferUnits(biggestFormation, formation.CountOfUnits);
      if (count1 > 1)
      {
        IEnumerable<Formation> source2 = biggestFormation.Split(count1);
        foreach (Formation formation in source2)
        {
          formation.AI.Side = FormationAI.BehaviorSide.BehaviorSideNotSet;
          formation.AI.IsMainFormation = false;
          formation.AI.ResetBehaviorWeights();
          TacticComponent.SetDefaultBehaviorWeights(formation);
          this.team.ClearRecentlySplitFormations(formation);
        }
        this.team.RegisterRecentlySplitFormations(source2.ToList<Formation>());
      }
      this.IsTacticReapplyNeeded = true;
    }

    protected virtual bool CheckAndSetAvailableFormationsChanged() => false;

    protected bool AreFormationsCreated
    {
      get
      {
        if (this._areFormationsCreated)
          return true;
        if (!this.Formations.Any<Formation>())
          return false;
        this._areFormationsCreated = true;
        this.ManageFormationCounts();
        this.CheckAndSetAvailableFormationsChanged();
        return true;
      }
    }

    protected void AssignTacticFormations1121()
    {
      this.ManageFormationCounts(1, 1, 2, 1);
      this._mainInfantry = TacticComponent.ChooseAndSortByPriority(this.Formations, (Func<Formation, bool>) (f => f.QuerySystem.IsInfantryFormation), (Func<Formation, bool>) (f => f.IsAIControlled), (Func<Formation, float>) (f => f.QuerySystem.FormationPower)).FirstOrDefault<Formation>();
      if (this._mainInfantry != null)
        this._mainInfantry.AI.IsMainFormation = true;
      this._archers = TacticComponent.ChooseAndSortByPriority(this.Formations, (Func<Formation, bool>) (f => f.QuerySystem.IsRangedFormation), (Func<Formation, bool>) (f => f.IsAIControlled), (Func<Formation, float>) (f => f.QuerySystem.FormationPower)).FirstOrDefault<Formation>();
      List<Formation> formationList = TacticComponent.ChooseAndSortByPriority(this.Formations, (Func<Formation, bool>) (f => f.QuerySystem.IsCavalryFormation), (Func<Formation, bool>) (f => f.IsAIControlled), (Func<Formation, float>) (f => f.QuerySystem.FormationPower));
      if (formationList.Count > 0)
      {
        this._leftCavalry = formationList[0];
        this._leftCavalry.AI.Side = FormationAI.BehaviorSide.Left;
        if (formationList.Count > 1)
        {
          this._rightCavalry = formationList[1];
          this._rightCavalry.AI.Side = FormationAI.BehaviorSide.Right;
        }
        else
          this._rightCavalry = (Formation) null;
      }
      else
      {
        this._leftCavalry = (Formation) null;
        this._rightCavalry = (Formation) null;
      }
      this._rangedCavalry = TacticComponent.ChooseAndSortByPriority(this.Formations, (Func<Formation, bool>) (f => f.QuerySystem.IsRangedCavalryFormation), (Func<Formation, bool>) (f => f.IsAIControlled), (Func<Formation, float>) (f => f.QuerySystem.FormationPower)).FirstOrDefault<Formation>();
    }

    protected static List<Formation> ChooseAndSortByPriority(
      IEnumerable<Formation> formations,
      Func<Formation, bool> isEligible,
      Func<Formation, bool> isPrioritized,
      Func<Formation, float> score)
    {
      formations = formations.Where<Formation>((Func<Formation, bool>) (f => isEligible(f)));
      IOrderedEnumerable<Formation> first = formations.Where<Formation>((Func<Formation, bool>) (f => isPrioritized(f))).OrderByDescending<Formation, float>((Func<Formation, float>) (f => score(f)));
      IOrderedEnumerable<Formation> orderedEnumerable = formations.Except<Formation>((IEnumerable<Formation>) first).OrderByDescending<Formation, float>((Func<Formation, float>) (f => score(f)));
      return first.Concat<Formation>((IEnumerable<Formation>) orderedEnumerable).ToList<Formation>();
    }

    protected virtual void ManageFormationCounts()
    {
    }

    protected void ManageFormationCounts(
      int infantryCount,
      int rangedCount,
      int cavalryCount,
      int rangedCavalryCount)
    {
      this.SplitFormationClassIntoGivenNumber((Func<Formation, bool>) (f => f.QuerySystem.IsInfantryFormation), infantryCount);
      this.SplitFormationClassIntoGivenNumber((Func<Formation, bool>) (f => f.QuerySystem.IsRangedFormation), rangedCount);
      this.SplitFormationClassIntoGivenNumber((Func<Formation, bool>) (f => f.QuerySystem.IsCavalryFormation), cavalryCount);
      this.SplitFormationClassIntoGivenNumber((Func<Formation, bool>) (f => f.QuerySystem.IsRangedCavalryFormation), rangedCavalryCount);
    }

    protected virtual void StopUsingAllMachines()
    {
      foreach (Formation formation in this.Formations)
      {
        foreach (UsableMachine usable in formation.GetUsedMachines().ToList<UsableMachine>())
        {
          formation.StopUsingMachine(usable);
          if (usable is SiegeWeapon siegeWeapon3)
            siegeWeapon3.ForcedUse = false;
        }
      }
    }

    protected void SoundTacticalHorn(int soundCode)
    {
      float time = MBCommon.GetTime(MBCommon.TimeType.Mission);
      if ((double) time <= (double) this._hornCooldownExpireTime)
        return;
      this._hornCooldownExpireTime = time + 10f;
      SoundEvent.PlaySound2D(soundCode);
    }

    internal static void SetDefaultBehaviorWeights(Formation f)
    {
      f.AI.SetBehaviorWeight<BehaviorCharge>(1f);
      f.AI.SetBehaviorWeight<BehaviorPullBack>(1f);
      f.AI.SetBehaviorWeight<BehaviorStop>(1f);
      f.AI.SetBehaviorWeight<BehaviorReserve>(1f);
    }

    internal virtual float GetTacticWeight() => 0.0f;

    protected bool CheckAndDetermineFormation(
      ref Formation formation,
      Func<Formation, bool> isEligible)
    {
      if (formation != null && formation.CountOfUnits != 0 && isEligible(formation))
        return true;
      List<Formation> list = this.Formations.Where<Formation>(isEligible).ToList<Formation>();
      if (list.Any<Formation>())
      {
        formation = list.MaxBy<Formation, int>((Func<Formation, int>) (f => f.CountOfUnits));
        this.IsTacticReapplyNeeded = true;
        return true;
      }
      if (formation != null)
      {
        formation = (Formation) null;
        this.IsTacticReapplyNeeded = true;
      }
      return false;
    }

    internal virtual bool ResetTacticalPositions() => false;
  }
}
