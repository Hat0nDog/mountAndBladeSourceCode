// Decompiled with JetBrains decompiler
// Type: TaleWorlds.MountAndBlade.ModuleExtensions
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
  public static class ModuleExtensions
  {
    public static void RaiseAiBehaviorPreference(
      this IEnumerable<Formation> formations,
      System.Type behaviourType)
    {
      formations.Select<Formation, FormationAI>((Func<Formation, FormationAI>) (f => f.AI)).RaiseBehaviorPreference(behaviourType);
    }

    public static void RaiseAiBehaviorPreference(
      this IEnumerable<Formation> formations,
      Func<BehaviorComponent, bool> predicate)
    {
      formations.Select<Formation, FormationAI>((Func<Formation, FormationAI>) (f => f.AI)).RaiseBehaviorPreference(predicate);
    }

    public static void AddAiSpecificBehavior(
      this IEnumerable<Formation> formations,
      BehaviorComponent behaviour)
    {
      formations.Select<Formation, FormationAI>((Func<Formation, FormationAI>) (f => f.AI)).AddSpecificBehavior(behaviour);
    }

    public static void RaiseBehaviorPreference(
      this IEnumerable<FormationAI> ais,
      Func<BehaviorComponent, bool> predicate)
    {
      foreach (FormationAI ai in ais)
        ai.SetBehaviorPreference(predicate);
    }

    public static void RaiseBehaviorPreference(
      this IEnumerable<FormationAI> ais,
      System.Type behaviourType)
    {
      foreach (FormationAI ai in ais)
        ai.SetBehaviorPreference((Func<BehaviorComponent, bool>) (b => b.GetType() == behaviourType));
    }

    public static void AddSpecificBehavior(
      this IEnumerable<FormationAI> ais,
      BehaviorComponent behaviour)
    {
      foreach (FormationAI ai in ais)
        ai.AddSpecificBehavior(behaviour);
    }

    public static bool IsRanged(this FormationClass formationClass) => formationClass == FormationClass.Ranged || formationClass == FormationClass.HorseArcher;

    public static bool IsCavalry(this FormationClass formationClass) => formationClass == FormationClass.Cavalry || formationClass == FormationClass.LightCavalry || formationClass == FormationClass.HeavyCavalry;

    public static bool IsInfantry(this FormationClass formationClass) => formationClass == FormationClass.Infantry || formationClass == FormationClass.HeavyInfantry || formationClass == FormationClass.NumberOfDefaultFormations;

    public static bool IsMounted(this FormationClass formationClass) => formationClass == FormationClass.Cavalry || formationClass == FormationClass.LightCavalry || formationClass == FormationClass.HeavyCavalry || formationClass == FormationClass.HorseArcher;

    public static FormationClass FallbackClass(this FormationClass formationClass)
    {
      switch (formationClass)
      {
        case FormationClass.Infantry:
        case FormationClass.HeavyInfantry:
        case FormationClass.NumberOfRegularFormations:
        case FormationClass.Bodyguard:
          return FormationClass.Infantry;
        case FormationClass.Ranged:
        case FormationClass.NumberOfDefaultFormations:
          return FormationClass.Ranged;
        case FormationClass.Cavalry:
        case FormationClass.HeavyCavalry:
          return FormationClass.Cavalry;
        case FormationClass.HorseArcher:
        case FormationClass.LightCavalry:
          return FormationClass.HorseArcher;
        default:
          return FormationClass.Infantry;
      }
    }

    public static bool IsRanged(this Formation formation) => formation.PrimaryClass.IsRanged();

    public static bool IsCavalry(this Formation formation) => formation.PrimaryClass.IsCavalry();

    public static bool IsInfantry(this Formation formation) => formation.PrimaryClass.IsInfantry();

    public static bool IsMounted(this Formation formation) => formation.PrimaryClass.IsMounted();

    public static bool IsUsingMachine(this Formation formation, UsableMachine usable) => formation.Detachments.Contains((IDetachment) usable);

    public static IEnumerable<UsableMachine> GetUsedMachines(
      this Formation formation)
    {
      return formation.Detachments.Select<IDetachment, UsableMachine>((Func<IDetachment, UsableMachine>) (d => d as UsableMachine)).Where<UsableMachine>((Func<UsableMachine, bool>) (u => u != null));
    }

    public static void StartUsingMachine(
      this Formation formation,
      UsableMachine usable,
      bool isPlayerOrder = false)
    {
      if (!isPlayerOrder && (!formation.IsAIControlled || Mission.Current.IsMissionEnding))
        return;
      formation.JoinDetachment((IDetachment) usable);
    }

    public static void StopUsingMachine(
      this Formation formation,
      UsableMachine usable,
      bool isPlayerOrder = false)
    {
      if (!isPlayerOrder && !formation.IsAIControlled)
        return;
      formation.LeaveDetachment((IDetachment) usable);
    }

    public static WorldPosition ToWorldPosition(this Vec3 rawPosition) => new WorldPosition(Mission.Current.Scene, rawPosition);
  }
}
