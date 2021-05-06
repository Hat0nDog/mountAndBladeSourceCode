// Decompiled with JetBrains decompiler
// Type: TaleWorlds.MountAndBlade.Formation
// Assembly: TaleWorlds.MountAndBlade, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3C0E3F42-756F-4407-B817-06BA412A78B0
// Assembly location: E:\SteamLibrary\steamapps\common\Mount & Blade II Bannerlord\bin\Win64_Shipping_Client\TaleWorlds.MountAndBlade.dll

using NetworkMessages.FromServer;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using TaleWorlds.Core;
using TaleWorlds.Engine;
using TaleWorlds.Library;
using TaleWorlds.Localization;
using TaleWorlds.MountAndBlade.Missions.Handlers;
using TaleWorlds.MountAndBlade.Network.Messages;
using TaleWorlds.MountAndBlade.Source.Missions.Handlers.Logic;

namespace TaleWorlds.MountAndBlade
{
  public sealed class Formation : IFormation
  {
    public readonly Team Team;
    public readonly int Index;
    public readonly FormationClass FormationIndex;
    private FormationClass _initialClass = FormationClass.NumberOfAllFormations;
    private bool _postponeCostlyOperations;
    private IFormationArrangement _arrangement;
    private int[] _agentIndicesCache;
    private Agent _playerOwner;
    private string _bannerCode;
    public Banner Banner;
    private bool _containsAgentVisuals;
    private bool _isAIControlled = true;
    private bool _isPlayerInFormation;
    private List<Agent> detachedUnits;
    private List<Agent> looseDetachedUnits;
    private WorldPosition _orderPosition;
    private Vec2 _direction;
    private int _unitSpacing;
    private Vec2 _orderLocalAveragePosition;
    private bool _orderLocalAveragePositionIsDirty = true;
    private int _formationOrderDefensivenessFactor = 2;
    private MovementOrder _movementOrder;
    private FacingOrder _facingOrder;
    private ArrangementOrder _arrangementOrder;
    private FormOrder _formOrder;
    private RidingOrder _ridingOrder;
    private WeaponUsageOrder _weaponUsageOrder;
    private FiringOrder _firingOrder;
    private bool? OverridenHasAnyMountedUnit;
    internal bool HasBeenPositioned;
    internal int GroupSpawnIndex;
    private List<IDetachment> _detachments;
    private List<IDetachment> _detachmentsToLeave = new List<IDetachment>(4);
    internal Vec2? ReferencePosition;
    private bool isArrangementShapeChanged;
    private static Formation simulationFormationTemp;
    private static int simulationFormationUniqueIdentifier;
    private Agent _captain;
    private Vec2 _smoothedAverageUnitPosition = Vec2.Invalid;
    public const float AveragePositionCalculatePeriod = 0.05f;

    public event Action<Formation> OnUnitCountChanged;

    internal event Action<Formation> OnUnitSpacingChanged;

    internal event Action<Formation> OnTick;

    public FormationClass PrimaryClass => this.QuerySystem.MainClass;

    public FormationClass InitialClass => this._initialClass == FormationClass.NumberOfAllFormations ? this.FormationIndex : this._initialClass;

    internal IFormationArrangement arrangement
    {
      get => this._arrangement;
      set
      {
        if (this._arrangement != null)
        {
          this._arrangement.OnWidthChanged -= new Action(this.Arrangement_OnWidthChanged);
          this._arrangement.OnShapeChanged -= new Action(this.Arrangement_OnShapeChanged);
        }
        this._arrangement = value;
        if (this._arrangement != null)
        {
          this._arrangement.OnWidthChanged += new Action(this.Arrangement_OnWidthChanged);
          this._arrangement.OnShapeChanged += new Action(this.Arrangement_OnShapeChanged);
        }
        this.Arrangement_OnWidthChanged();
        this.Arrangement_OnShapeChanged();
      }
    }

    public bool HasUnitsWithCondition(Func<Agent, bool> function)
    {
      foreach (IFormationUnit allUnit in this.arrangement.GetAllUnits())
      {
        if (function((Agent) allUnit))
          return true;
      }
      for (int index = 0; index < this.detachedUnits.Count; ++index)
      {
        if (function(this.detachedUnits[index]))
          return true;
      }
      return false;
    }

    public int GetCountOfUnitsWithCondition(Func<Agent, bool> function)
    {
      int num = 0;
      foreach (IFormationUnit allUnit in this.arrangement.GetAllUnits())
      {
        if (function((Agent) allUnit))
          ++num;
      }
      for (int index = 0; index < this.detachedUnits.Count; ++index)
      {
        if (function(this.detachedUnits[index]))
          ++num;
      }
      return num;
    }

    public int[] CollectUnitIndices()
    {
      if (this._agentIndicesCache == null || this._agentIndicesCache.Length != this.CountOfUnits)
        this._agentIndicesCache = new int[this.CountOfUnits];
      int index1 = 0;
      foreach (IFormationUnit allUnit in this.arrangement.GetAllUnits())
      {
        this._agentIndicesCache[index1] = ((Agent) allUnit).Index;
        ++index1;
      }
      for (int index2 = 0; index2 < this.detachedUnits.Count; ++index2)
      {
        this._agentIndicesCache[index1] = this.detachedUnits[index2].Index;
        ++index1;
      }
      return this._agentIndicesCache;
    }

    public Agent GetFirstUnit() => this.GetUnitWithIndex(0);

    public Agent GetUnitWithIndex(int unitIndex)
    {
      if (this.arrangement.GetAllUnits().Count > unitIndex)
        return (Agent) this.arrangement.GetAllUnits()[unitIndex];
      unitIndex -= this.arrangement.GetAllUnits().Count;
      return this.detachedUnits.Count > unitIndex ? this.detachedUnits[unitIndex] : (Agent) null;
    }

    public void ApplyActionOnEachUnit(Action<Agent> action)
    {
      foreach (IFormationUnit allUnit in this.arrangement.GetAllUnits())
        action((Agent) allUnit);
      for (int index = 0; index < this.detachedUnits.Count; ++index)
        action(this.detachedUnits[index]);
    }

    public void ApplyActionOnEachUnitViaBackupList(Action<Agent> action)
    {
      if (this.arrangement.GetAllUnits().Count > 0)
      {
        foreach (IFormationUnit formationUnit in this.arrangement.GetAllUnits().ToArray())
          action((Agent) formationUnit);
      }
      if (this.detachedUnits.Count <= 0)
        return;
      foreach (Agent agent in this.detachedUnits.ToArray())
        action(agent);
    }

    public void ApplyActionOnEachUnit(
      Action<Agent, List<(Agent, WorldFrame)>> action,
      List<(Agent, WorldFrame)> list)
    {
      foreach (IFormationUnit allUnit in this.arrangement.GetAllUnits())
        action((Agent) allUnit, list);
      for (int index = 0; index < this.detachedUnits.Count; ++index)
        action(this.detachedUnits[index], list);
    }

    public Vec2 GetAveragePositionOfUnits(bool excludeDetachedUnits, bool excludePlayer)
    {
      int num = excludeDetachedUnits ? this.CountOfUnitsWithoutDetachedOnes : this.CountOfUnits;
      if (num > 0)
      {
        Vec2 zero = Vec2.Zero;
        foreach (Agent allUnit in this.arrangement.GetAllUnits())
        {
          if (!excludePlayer || !allUnit.IsMainAgent)
            zero += allUnit.Position.AsVec2;
          else
            --num;
        }
        if (excludeDetachedUnits)
        {
          for (int index = 0; index < this.looseDetachedUnits.Count; ++index)
            zero += this.looseDetachedUnits[index].Position.AsVec2;
        }
        else
        {
          for (int index = 0; index < this.detachedUnits.Count; ++index)
            zero += this.detachedUnits[index].Position.AsVec2;
        }
        if (num > 0)
          return zero * (1f / (float) num);
      }
      return Vec2.Invalid;
    }

    public Agent GetMedianAgent(
      bool excludeDetachedUnits,
      bool excludePlayer,
      Vec2 averagePosition)
    {
      float num1 = float.MaxValue;
      Agent agent = (Agent) null;
      foreach (Agent allUnit in this.arrangement.GetAllUnits())
      {
        if (!excludePlayer || !allUnit.IsMainAgent)
        {
          float num2 = allUnit.Position.AsVec2.DistanceSquared(averagePosition);
          if ((double) num2 <= (double) num1)
          {
            agent = allUnit;
            num1 = num2;
          }
        }
      }
      if (excludeDetachedUnits)
      {
        for (int index = 0; index < this.looseDetachedUnits.Count; ++index)
        {
          float num2 = this.looseDetachedUnits[index].Position.AsVec2.DistanceSquared(averagePosition);
          if ((double) num2 <= (double) num1)
          {
            agent = this.looseDetachedUnits[index];
            num1 = num2;
          }
        }
      }
      else
      {
        for (int index = 0; index < this.detachedUnits.Count; ++index)
        {
          float num2 = this.detachedUnits[index].Position.AsVec2.DistanceSquared(averagePosition);
          if ((double) num2 <= (double) num1)
          {
            agent = this.detachedUnits[index];
            num1 = num2;
          }
        }
      }
      return agent;
    }

    public int CountUnitsOnNavMeshIDMod10(int navMeshID, bool includeOnlyPositionedUnits)
    {
      int num = 0;
      foreach (IFormationUnit allUnit in this.arrangement.GetAllUnits())
      {
        if (((Agent) allUnit).GetCurrentNavigationFaceId() % 10 == navMeshID && (!includeOnlyPositionedUnits || this.arrangement.GetUnpositionedUnits() == null || this.arrangement.GetUnpositionedUnits().IndexOf(allUnit) < 0))
          ++num;
      }
      if (!includeOnlyPositionedUnits)
      {
        foreach (Agent detachedUnit in this.detachedUnits)
        {
          if (detachedUnit.GetCurrentNavigationFaceId() % 10 == navMeshID)
            ++num;
        }
      }
      return num;
    }

    public Agent.UnderAttackType GetUnderAttackTypeOfUnits(float timeLimit = 3f)
    {
      float val1_1 = float.MinValue;
      float val1_2 = float.MinValue;
      timeLimit += MBCommon.TimeType.Mission.GetTime();
      foreach (IFormationUnit allUnit in this.arrangement.GetAllUnits())
      {
        val1_1 = Math.Max(val1_1, ((Agent) allUnit).LastMeleeHitTime);
        val1_2 = Math.Max(val1_2, ((Agent) allUnit).LastRangedHitTime);
        if ((double) val1_2 >= 0.0 && (double) val1_2 < (double) timeLimit)
          return Agent.UnderAttackType.UnderRangedAttack;
        if ((double) val1_1 >= 0.0 && (double) val1_1 < (double) timeLimit)
          return Agent.UnderAttackType.UnderMeleeAttack;
      }
      for (int index = 0; index < this.detachedUnits.Count; ++index)
      {
        val1_1 = Math.Max(val1_1, this.detachedUnits[index].LastMeleeHitTime);
        val1_2 = Math.Max(val1_2, this.detachedUnits[index].LastRangedHitTime);
        if ((double) val1_2 >= 0.0 && (double) val1_2 < (double) timeLimit)
          return Agent.UnderAttackType.UnderRangedAttack;
        if ((double) val1_1 >= 0.0 && (double) val1_1 < (double) timeLimit)
          return Agent.UnderAttackType.UnderMeleeAttack;
      }
      return Agent.UnderAttackType.NotUnderAttack;
    }

    public Agent.MovementBehaviourType GetMovementTypeOfUnits()
    {
      float curMissionTime = MBCommon.TimeType.Mission.GetTime();
      int retreatingCount = 0;
      int attackingCount = 0;
      this.ApplyActionOnEachUnit((Action<Agent>) (agent =>
      {
        if (agent.IsAIControlled && (agent.IsRetreating() || agent.Formation != null && agent.Formation.MovementOrder.OrderType == OrderType.Retreat))
          ++retreatingCount;
        if ((double) curMissionTime - (double) agent.LastMeleeAttackTime >= 3.0)
          return;
        ++attackingCount;
      }));
      if (this.CountOfUnits > 0 && (double) retreatingCount / (double) this.CountOfUnits > 0.300000011920929)
        return Agent.MovementBehaviourType.Flee;
      return attackingCount > 0 ? Agent.MovementBehaviourType.Engaged : Agent.MovementBehaviourType.Idle;
    }

    internal IEnumerable<Agent> GetUnitsWithoutDetachedOnes()
    {
      foreach (IFormationUnit allUnit in this.arrangement.GetAllUnits())
        yield return allUnit as Agent;
      for (int i = 0; i < this.looseDetachedUnits.Count; ++i)
        yield return this.looseDetachedUnits[i];
    }

    public int CountOfUnits => this.arrangement.UnitCount + this.detachedUnits.Count;

    public int CountOfUnitsWithoutDetachedOnes => this.arrangement.UnitCount + this.looseDetachedUnits.Count;

    internal List<IFormationUnit> UnitsWithoutLooseDetachedOnes => this.arrangement.GetAllUnits();

    internal int CountOfUnitsWithoutLooseDetachedOnes => this.arrangement.UnitCount;

    public Agent PlayerOwner
    {
      get => this._playerOwner;
      set
      {
        this._playerOwner = value;
        this.IsAIControlled = value == null;
      }
    }

    public string BannerCode
    {
      set
      {
        this._bannerCode = value;
        if (!GameNetwork.IsServer)
          return;
        GameNetwork.BeginBroadcastModuleEvent();
        GameNetwork.WriteMessage((GameNetworkMessage) new InitializeFormation(this, this.Team, this._bannerCode));
        GameNetwork.EndBroadcastModuleEvent(GameNetwork.EventBroadcastFlags.None);
      }
      get => this._bannerCode;
    }

    public bool ContainsAgentVisuals
    {
      get => this._containsAgentVisuals;
      set => this._containsAgentVisuals = value;
    }

    public bool IsSplittableByAI
    {
      get
      {
        if (this.IsAIControlled)
          return true;
        if (this.Team.IsPlayerGeneral)
          return false;
        return !this.Team.IsPlayerSergeant || this.PlayerOwner != Agent.Main;
      }
    }

    public bool IsAIControlled
    {
      get => this._isAIControlled;
      set
      {
        if (this._isAIControlled == value)
          return;
        this._isAIControlled = value;
        if (!this._isAIControlled || this.AI.ActiveBehavior == null || this.CountOfUnits <= 0)
          return;
        bool tickOccasionally = Mission.Current.ForceTickOccasionally;
        Mission.Current.ForceTickOccasionally = true;
        BehaviorComponent activeBehavior1 = this.AI.ActiveBehavior;
        this.AI.Tick();
        Mission.Current.ForceTickOccasionally = tickOccasionally;
        BehaviorComponent activeBehavior2 = this.AI.ActiveBehavior;
        if (activeBehavior1 == activeBehavior2)
          this.AI.ActiveBehavior.OnBehaviorActivated();
        this.MovementOrder = this.AI.ActiveBehavior.CurrentOrder;
      }
    }

    public bool IsPlayerInFormation
    {
      get => this._isPlayerInFormation;
      set
      {
        this._isPlayerInFormation = value;
        MBTextManager.SetTextVariable("SIDE_STRING", this.AI.Side.ToString(), false);
        MBTextManager.SetTextVariable("CLASS_NAME", this.PrimaryClass.GetName(), false);
        InformationManager.AddQuickInformation(GameTexts.FindText("str_formation_soldier_join_text"));
      }
    }

    public MBReadOnlyList<Agent> LooseDetachedUnits { get; private set; }

    public WorldPosition OrderPosition => this._orderPosition;

    public Vec2 OrderLocalAveragePosition
    {
      get
      {
        if (this._orderLocalAveragePositionIsDirty)
        {
          this._orderLocalAveragePositionIsDirty = false;
          this._orderLocalAveragePosition = new Vec2();
          if (this.UnitsWithoutLooseDetachedOnes.Any<IFormationUnit>())
          {
            int num = 0;
            foreach (IFormationUnit looseDetachedOne in this.UnitsWithoutLooseDetachedOnes)
            {
              Vec2? positionOfUnitOrDefault = this.arrangement.GetLocalPositionOfUnitOrDefault(looseDetachedOne);
              if (positionOfUnitOrDefault.HasValue)
              {
                this._orderLocalAveragePosition += positionOfUnitOrDefault.Value;
                ++num;
              }
            }
            if (num > 0)
              this._orderLocalAveragePosition *= 1f / (float) num;
          }
        }
        return this._orderLocalAveragePosition;
      }
    }

    public Vec2 Direction => this._direction;

    public int UnitSpacing => this._unitSpacing;

    public MovementOrder MovementOrder
    {
      get => this._movementOrder;
      set
      {
        if (value == (object) null)
          value = MovementOrder.MovementOrderStop;
        int num = !this.MovementOrder.AreOrdersPracticallySame(this._movementOrder, value, this.IsAIControlled) ? 1 : 0;
        if (num != 0)
          this._movementOrder.OnCancel(this);
        if (num == 0)
          return;
        if (MovementOrder.GetMovementOrderDefensivenessChange(this._movementOrder.OrderEnum, value.OrderEnum) != 0)
        {
          this._formationOrderDefensivenessFactor = MovementOrder.GetMovementOrderDefensiveness(value.OrderEnum) != 0 ? MovementOrder.GetMovementOrderDefensiveness(value.OrderEnum) + ArrangementOrder.GetArrangementOrderDefensiveness(this._arrangementOrder.OrderEnum) : 0;
          this.UpdateAgentDrivenPropertiesBasedOnOrderDefensiveness();
        }
        this._movementOrder = value;
        this._movementOrder.OnApply(this);
      }
    }

    internal AttackEntityOrderDetachment AttackEntityOrderDetachment { get; private set; }

    public FacingOrder FacingOrder
    {
      get => this._facingOrder;
      set => this._facingOrder = value;
    }

    public ArrangementOrder ArrangementOrder
    {
      get => this._arrangementOrder;
      set
      {
        if (value.OrderType != this._arrangementOrder.OrderType)
        {
          this._arrangementOrder.OnCancel(this);
          int defensivenessChange = ArrangementOrder.GetArrangementOrderDefensivenessChange(this._arrangementOrder.OrderEnum, value.OrderEnum);
          if (defensivenessChange != 0 && MovementOrder.GetMovementOrderDefensiveness(this._movementOrder.OrderEnum) != 0)
          {
            this._formationOrderDefensivenessFactor += defensivenessChange;
            this.UpdateAgentDrivenPropertiesBasedOnOrderDefensiveness();
          }
          if (this.FormOrder.OrderEnum == FormOrder.FormOrderEnum.Custom)
            this.FormOrder = FormOrder.FormOrderCustom(Formation.TransformCustomWidthBetweenArrangementOrientations(this._arrangementOrder.OrderEnum, value.OrderEnum, this.FormOrder.CustomWidth));
          this._arrangementOrder = value;
          this._arrangementOrder.OnApply(this);
        }
        else
          this._arrangementOrder.SoftUpdate(this);
      }
    }

    public FormOrder FormOrder
    {
      get => this._formOrder;
      set
      {
        this._formOrder = value;
        this._formOrder.OnApply(this);
      }
    }

    public RidingOrder RidingOrder
    {
      get => this._ridingOrder;
      set
      {
        if (!(this._ridingOrder != value))
          return;
        this._ridingOrder = value;
        this.ApplyActionOnEachUnit((Action<Agent>) (agent => agent.SetRidingOrder((int) value.OrderEnum)));
        this.Arrangement_OnShapeChanged();
      }
    }

    public WeaponUsageOrder WeaponUsageOrder
    {
      get => this._weaponUsageOrder;
      set => this._weaponUsageOrder = value;
    }

    public FiringOrder FiringOrder
    {
      get => this._firingOrder;
      set => this._firingOrder = value;
    }

    public FormationAI AI { get; private set; }

    public Formation TargetFormation { get; set; }

    public FormationQuerySystem QuerySystem { get; private set; }

    private bool IsSimulationFormation => this.Team == null;

    public bool HasAnyMountedUnit => this.OverridenHasAnyMountedUnit.HasValue ? this.OverridenHasAnyMountedUnit.Value : (int) ((double) this.QuerySystem.GetRangedCavalryUnitRatioWithoutExpiration * (double) this.CountOfUnits + 9.99999974737875E-06) + (int) ((double) this.QuerySystem.GetCavalryUnitRatioWithoutExpiration * (double) this.CountOfUnits + 9.99999974737875E-06) > 0;

    public IEnumerable<FormationClass> SecondaryClasses
    {
      get
      {
        FormationClass primaryClass = this.PrimaryClass;
        if (primaryClass != FormationClass.Infantry && (double) this.QuerySystem.InfantryUnitRatio > 0.0)
          yield return FormationClass.Infantry;
        if (primaryClass != FormationClass.Ranged && (double) this.QuerySystem.RangedUnitRatio > 0.0)
          yield return FormationClass.Ranged;
        if (primaryClass != FormationClass.Cavalry && (double) this.QuerySystem.CavalryUnitRatio > 0.0)
          yield return FormationClass.Cavalry;
        if (primaryClass != FormationClass.HorseArcher && (double) this.QuerySystem.RangedCavalryUnitRatio > 0.0)
          yield return FormationClass.HorseArcher;
      }
    }

    internal MBReadOnlyList<IDetachment> Detachments { get; private set; }

    public float Width
    {
      get => this.arrangement.Width;
      private set => this.arrangement.Width = value;
    }

    public float Depth => this.arrangement.Depth;

    internal float MinimumWidth => this.arrangement.MinimumWidth;

    internal event Action<Formation> OnWidthChanged;

    internal event Action OnShapeChanged;

    public int? OverridenUnitCount { get; private set; }

    public bool IsSpawning { get; private set; }

    public bool IsAITickedAfterSplit { get; set; }

    internal bool HasPlayer { get; private set; }

    internal Formation(Team team, int index)
    {
      this.Team = team;
      this.Index = index;
      this.FormationIndex = (FormationClass) index;
      this.IsSpawning = false;
      this.Reset();
    }

    public void ReleaseFormationFromAI() => this._isAIControlled = false;

    private void Arrangement_OnWidthChanged()
    {
      Action<Formation> onWidthChanged = this.OnWidthChanged;
      if (onWidthChanged == null)
        return;
      onWidthChanged(this);
    }

    private void Arrangement_OnShapeChanged()
    {
      this._orderLocalAveragePositionIsDirty = true;
      this.isArrangementShapeChanged = true;
    }

    private void ResetAux()
    {
      this.detachedUnits = new List<Agent>();
      this.looseDetachedUnits = new List<Agent>();
      this.LooseDetachedUnits = new MBReadOnlyList<Agent>(this.looseDetachedUnits);
      this._detachments = new List<IDetachment>();
      this.Detachments = this._detachments.GetReadOnlyList<IDetachment>();
      this.AttackEntityOrderDetachment = (AttackEntityOrderDetachment) null;
      this.AI = new FormationAI(this);
      this.QuerySystem = new FormationQuerySystem(this);
      this.SetPositioning(direction: new Vec2?(Vec2.Forward), unitSpacing: new int?(1));
      this.MovementOrder = MovementOrder.MovementOrderStop;
      if (this.OverridenHasAnyMountedUnit.HasValue)
      {
        bool? hasAnyMountedUnit = this.OverridenHasAnyMountedUnit;
        bool flag = true;
        if (hasAnyMountedUnit.GetValueOrDefault() == flag & hasAnyMountedUnit.HasValue)
        {
          this.ArrangementOrder = ArrangementOrder.ArrangementOrderSkein;
          goto label_4;
        }
      }
      this.FormOrder = FormOrder.FormOrderWide;
      this.ArrangementOrder = ArrangementOrder.ArrangementOrderLine;
label_4:
      this.RidingOrder = RidingOrder.RidingOrderFree;
      this.WeaponUsageOrder = WeaponUsageOrder.WeaponUsageOrderUseAny;
      this.FiringOrder = FiringOrder.FiringOrderFireAtWill;
      this.Width = (float) (0.0 * ((double) this.Interval + (double) this.UnitDiameter)) + this.UnitDiameter;
      this.HasBeenPositioned = false;
      this.GroupSpawnIndex = 0;
      this.HasPlayer = false;
    }

    private void ResetForSimulation()
    {
      this.arrangement.Reset();
      this.ResetAux();
    }

    public void Reset()
    {
      this.arrangement = (IFormationArrangement) new LineFormation((IFormation) this);
      this.ResetAux();
      this.FacingOrder = FacingOrder.FacingOrderLookAtEnemy;
      this.ContainsAgentVisuals = false;
      this.PlayerOwner = (Agent) null;
    }

    private void CopyOrdersFrom(Formation target)
    {
      this.MovementOrder = target.MovementOrder;
      this.FormOrder = target.FormOrder;
      this.SetPositioning(unitSpacing: new int?(target.UnitSpacing));
      this.RidingOrder = target.RidingOrder;
      this.WeaponUsageOrder = target.WeaponUsageOrder;
      this.FiringOrder = target.FiringOrder;
      this.IsAIControlled = target.IsAIControlled || !target.Team.IsPlayerGeneral;
      this.AI.Side = target.AI.Side;
      this.MovementOrder = target.MovementOrder;
      this.FacingOrder = target.FacingOrder;
      this.ArrangementOrder = target.ArrangementOrder;
    }

    internal IEnumerable<Formation> Split(int count = 2)
    {
      foreach (Formation formation in this.Team.FormationsIncludingEmpty)
        formation._postponeCostlyOperations = true;
      IEnumerable<Formation> source = this.Team.MasterOrderController.SplitFormation(this, count);
      if (source.Count<Formation>() > 1 && this.Team != null)
      {
        this.Team.RegisterRecentlySplitFormations(source.ToList<Formation>());
        foreach (Formation formation in source)
          formation.QuerySystem.Expire();
      }
      foreach (Formation formation in this.Team.FormationsIncludingEmpty)
        formation._postponeCostlyOperations = false;
      return source;
    }

    internal IEnumerable<IFormationUnit> GetUnitsToPop(int count)
    {
      int count1 = Math.Min(count, this.arrangement.UnitCount);
      IEnumerable<IFormationUnit> formationUnits = count1 == 0 ? Enumerable.Empty<IFormationUnit>() : this.arrangement.GetUnitsToPop(count1);
      int count2 = count - formationUnits.Count<IFormationUnit>();
      if (count2 > 0)
      {
        IEnumerable<Agent> source = this.looseDetachedUnits.Take<Agent>(count2);
        count2 -= source.Count<Agent>();
        formationUnits = formationUnits.Concat<IFormationUnit>((IEnumerable<IFormationUnit>) source);
      }
      if (count2 > 0)
      {
        IEnumerable<Agent> source = this.detachedUnits.Take<Agent>(count2);
        int num = count2 - source.Count<Agent>();
        formationUnits = formationUnits.Concat<IFormationUnit>((IEnumerable<IFormationUnit>) source);
      }
      return formationUnits;
    }

    internal void TransferUnits(Formation target, int unitCount)
    {
      this._postponeCostlyOperations = true;
      target._postponeCostlyOperations = true;
      this.Team.MasterOrderController.TransferUnits(this, target, unitCount);
      this._postponeCostlyOperations = false;
      target._postponeCostlyOperations = false;
      this.QuerySystem.Expire();
      target.QuerySystem.Expire();
    }

    internal void TransferUnitsAux(Formation target, int unitCount, bool isPlayerOrder = false)
    {
      if (!isPlayerOrder && !this.IsSplittableByAI)
        return;
      MBDebug.Print(this.FormationIndex.GetName() + " has " + (object) this.CountOfUnits + " units, " + target.FormationIndex.GetName() + " has " + (object) target.CountOfUnits + " units");
      MBDebug.Print(this.Team.Side.ToString() + " " + this.FormationIndex.GetName() + " transfers " + (object) unitCount + " units to " + target.FormationIndex.GetName());
      if (unitCount == 0)
        return;
      if (target.CountOfUnits == 0)
      {
        target.CopyOrdersFrom(this);
        target.SetPositioning(new WorldPosition?(this._orderPosition), new Vec2?(this._direction), new int?(this._unitSpacing));
      }
      foreach (Agent agent in new List<IFormationUnit>(this.GetUnitsToPop(unitCount)))
        agent.Formation = target;
      this.Team.TriggerOnFormationsChanged(this);
      this.Team.TriggerOnFormationsChanged(target);
      MBDebug.Print(this.FormationIndex.GetName() + " has " + (object) this.CountOfUnits + " units, " + target.FormationIndex.GetName() + " has " + (object) target.CountOfUnits + " units");
    }

    private bool IsDeployment => Mission.Current.GetMissionBehaviour<SiegeDeploymentHandler>() != null;

    private void TickDetachments(float dt)
    {
      this._detachmentsToLeave.Clear();
      foreach (IDetachment detachment in this.Detachments)
      {
        if (detachment is UsableMachine usableMachine1 && usableMachine1.Ai != null && !this.IsDeployment)
        {
          usableMachine1.Ai.Tick((Func<Agent, bool>) (a => a != null && a.IsAIControlled && a.Formation == this), this.Team, dt);
          if (usableMachine1.Ai.HasActionCompleted || usableMachine1.IsDisabledForBattleSideAI(this.Team.Side))
            this._detachmentsToLeave.Add(detachment);
        }
      }
      foreach (IDetachment detachment in this._detachmentsToLeave)
        this.LeaveDetachment(detachment);
    }

    internal WorldFrame? GetUnitSpawnFrameWithIndex(
      int unitIndex,
      ref WorldFrame formationFrame,
      float width,
      int unitCount,
      int unitSpacing,
      bool isMountedFormation)
    {
      return Formation.GetUnitPositionWithIndexAccordingToNewOrder((Formation) null, unitIndex, ref formationFrame, this.arrangement, width, unitSpacing, unitCount, isMountedFormation, this.Index, out float _);
    }

    internal WorldFrame? GetUnitPositionWithIndexAccordingToNewOrder(
      Formation simulationFormation,
      int unitIndex,
      ref WorldFrame positionFrame,
      float width,
      int unitSpacing)
    {
      int unitCount = this.arrangement.UnitCount;
      return Formation.GetUnitPositionWithIndexAccordingToNewOrder(simulationFormation, unitIndex, ref positionFrame, this.arrangement, width, unitSpacing, unitCount, this.HasAnyMountedUnit, this.Index, out float _);
    }

    internal WorldFrame? GetUnitPositionWithIndexAccordingToNewOrder(
      Formation simulationFormation,
      int unitIndex,
      ref WorldFrame positionFrame,
      float width,
      int unitSpacing,
      int overridenUnitCount)
    {
      return Formation.GetUnitPositionWithIndexAccordingToNewOrder(simulationFormation, unitIndex, ref positionFrame, this.arrangement, width, unitSpacing, overridenUnitCount, this.HasAnyMountedUnit, this.Index, out float _);
    }

    internal WorldFrame? GetUnitPositionWithIndexAccordingToNewOrder(
      Formation simulationFormation,
      int unitIndex,
      ref WorldFrame positionFrame,
      float width,
      int unitSpacing,
      out float actualWidth)
    {
      int unitCount = this.arrangement.UnitCount;
      return Formation.GetUnitPositionWithIndexAccordingToNewOrder(simulationFormation, unitIndex, ref positionFrame, this.arrangement, width, unitSpacing, unitCount, this.HasAnyMountedUnit, this.Index, out actualWidth);
    }

    internal static float GetLastSimulatedFormationsOccupationWidthIfLesserThanActualWidth(
      Formation simulationFormation)
    {
      float occupationWidth = simulationFormation.arrangement.GetOccupationWidth(simulationFormation.OverridenUnitCount.GetValueOrDefault());
      return (double) simulationFormation.Width > (double) occupationWidth ? occupationWidth : -1f;
    }

    ~Formation()
    {
      if (this.IsSimulationFormation)
        return;
      Formation.simulationFormationTemp = (Formation) null;
    }

    private static WorldFrame? GetUnitPositionWithIndexAccordingToNewOrder(
      Formation simulationFormation,
      int unitIndex,
      ref WorldFrame positionFrame,
      IFormationArrangement arrangement,
      float width,
      int unitSpacing,
      int unitCount,
      bool isMounted,
      int index,
      out float actualWidth)
    {
      WorldFrame? nullable1 = new WorldFrame?();
      if (simulationFormation == null)
      {
        if (Formation.simulationFormationTemp == null || Formation.simulationFormationUniqueIdentifier != index)
          Formation.simulationFormationTemp = new Formation((Team) null, -1);
        simulationFormation = Formation.simulationFormationTemp;
      }
      Vec2 direction;
      if (simulationFormation.UnitSpacing == unitSpacing && (double) Math.Abs((float) ((double) simulationFormation.Width - (double) width + 9.99999974737875E-06)) < (double) simulationFormation.Interval + (double) simulationFormation.UnitDiameter - 9.99999974737875E-06)
      {
        WorldPosition orderPosition = simulationFormation.OrderPosition;
        if (orderPosition.IsValid)
        {
          orderPosition = simulationFormation.OrderPosition;
          if (orderPosition.GetGroundVec3().NearlyEquals(positionFrame.Origin.GetGroundVec3(), 0.1f))
          {
            direction = simulationFormation.Direction;
            if (direction.NearlyEquals(positionFrame.Rotation.f.AsVec2, 0.1f) && !(simulationFormation.arrangement.GetType() != arrangement.GetType()))
              goto label_9;
          }
        }
      }
      simulationFormation.OverridenHasAnyMountedUnit = new bool?(isMounted);
      simulationFormation.ResetForSimulation();
      simulationFormation.SetPositioning(unitSpacing: new int?(unitSpacing));
      simulationFormation.OverridenUnitCount = new int?(unitCount);
      simulationFormation.SetPositioning(new WorldPosition?(positionFrame.Origin), new Vec2?(positionFrame.Rotation.f.AsVec2));
      simulationFormation.Rearrange(arrangement.Clone((IFormation) simulationFormation));
      simulationFormation.arrangement.DeepCopyFrom(arrangement);
      simulationFormation.Width = width;
      Formation.simulationFormationUniqueIdentifier = index;
label_9:
      actualWidth = simulationFormation.Width;
      if ((double) width >= (double) actualWidth)
      {
        Vec2? nullable2 = simulationFormation.arrangement.GetLocalPositionOfUnitOrDefault(unitIndex);
        if (!nullable2.HasValue)
          nullable2 = simulationFormation.arrangement.CreateNewPosition(unitIndex);
        if (nullable2.HasValue)
        {
          direction = simulationFormation.Direction;
          Vec2 parentUnitF = direction.TransformToParentUnitF(nullable2.Value);
          WorldPosition orderPosition = simulationFormation.OrderPosition;
          orderPosition.SetVec2(orderPosition.AsVec2 + parentUnitF);
          Mat3 rotation = positionFrame.Rotation;
          rotation.Orthonormalize();
          nullable1 = new WorldFrame?(new WorldFrame(rotation, orderPosition));
        }
      }
      return nullable1;
    }

    internal IEnumerable<WorldFrame> GetUnavailableUnitPositionsAccordingToNewOrder(
      Formation simulationFormation,
      WorldFrame positionFrame,
      float width,
      int unitSpacing)
    {
      return Formation.GetUnavailableUnitPositionsAccordingToNewOrder(this, simulationFormation, positionFrame, this.arrangement, width, unitSpacing);
    }

    private static IEnumerable<WorldFrame> GetUnavailableUnitPositionsAccordingToNewOrder(
      Formation formation,
      Formation simulationFormation,
      WorldFrame positionFrame,
      IFormationArrangement arrangement,
      float width,
      int unitSpacing)
    {
      if (simulationFormation == null)
      {
        if (Formation.simulationFormationTemp == null || Formation.simulationFormationUniqueIdentifier != formation.Index)
          Formation.simulationFormationTemp = new Formation((Team) null, -1);
        simulationFormation = Formation.simulationFormationTemp;
      }
      Vec2 direction;
      if (simulationFormation.UnitSpacing == unitSpacing && (double) Math.Abs(simulationFormation.Width - width) < (double) simulationFormation.Interval + (double) simulationFormation.UnitDiameter)
      {
        WorldPosition orderPosition = simulationFormation.OrderPosition;
        if (orderPosition.IsValid)
        {
          orderPosition = simulationFormation.OrderPosition;
          if (orderPosition.GetGroundVec3().NearlyEquals(positionFrame.Origin.GetGroundVec3(), 0.1f))
          {
            direction = simulationFormation.Direction;
            if (direction.NearlyEquals(positionFrame.Rotation.f.AsVec2, 0.1f) && !(simulationFormation.arrangement.GetType() != arrangement.GetType()))
              goto label_12;
          }
        }
      }
      simulationFormation.OverridenHasAnyMountedUnit = new bool?(formation.HasAnyMountedUnit);
      simulationFormation.ResetForSimulation();
      simulationFormation.SetPositioning(unitSpacing: new int?(unitSpacing));
      simulationFormation.OverridenUnitCount = new int?(formation.CountOfUnitsWithoutDetachedOnes);
      simulationFormation.SetPositioning(new WorldPosition?(positionFrame.Origin), new Vec2?(positionFrame.Rotation.f.AsVec2));
      simulationFormation.Rearrange(arrangement.Clone((IFormation) simulationFormation));
      simulationFormation.arrangement.DeepCopyFrom(arrangement);
      simulationFormation.Width = width;
      Formation.simulationFormationUniqueIdentifier = formation.Index;
label_12:
      foreach (Vec2 unavailableUnitPosition in simulationFormation.arrangement.GetUnavailableUnitPositions())
      {
        direction = simulationFormation.Direction;
        Vec2 parentUnitF = direction.TransformToParentUnitF(unavailableUnitPosition);
        WorldPosition orderPosition = simulationFormation.OrderPosition;
        orderPosition.SetVec2(orderPosition.AsVec2 + parentUnitF);
        yield return new WorldFrame(positionFrame.Rotation, orderPosition);
      }
    }

    public float UnitDiameter => Formation.GetDefaultUnitDiameter(this.CalculateHasSignificantNumberOfMounted && !(this.RidingOrder == RidingOrder.RidingOrderDismount));

    float IFormation.MinimumInterval => Formation.GetDefaultMinimumInterval(this.HasAnyMountedUnit && !(this.RidingOrder == RidingOrder.RidingOrderDismount));

    public float MaximumInterval => this.HasAnyMountedUnit && !(this.RidingOrder == RidingOrder.RidingOrderDismount) ? 1.6f : 0.2f * (float) Math.Pow(2.0, 3.0);

    public static float InfantryInterval(int unitSpacing) => 0.2f * (float) Math.Pow((double) unitSpacing, 3.0);

    public static float CavalryInterval(int unitSpacing) => (float) (0.400000005960464 + 0.600000023841858 * (double) unitSpacing);

    public static float InfantryDistance(int unitSpacing) => 0.2f * (float) Math.Pow((double) unitSpacing, 2.0);

    public static float CavalryDistance(int unitSpacing) => (float) (2.0 + 0.600000023841858 * (double) unitSpacing);

    public float Interval => this.CalculateHasSignificantNumberOfMounted && !(this.RidingOrder == RidingOrder.RidingOrderDismount) ? Formation.CavalryInterval(this.UnitSpacing) : Formation.InfantryInterval(this.UnitSpacing);

    public bool CalculateHasSignificantNumberOfMounted => this.OverridenHasAnyMountedUnit.HasValue ? this.OverridenHasAnyMountedUnit.Value : (double) this.QuerySystem.CavalryUnitRatio + (double) this.QuerySystem.RangedCavalryUnitRatio >= 0.100000001490116;

    public float Distance => this.CalculateHasSignificantNumberOfMounted && !(this.RidingOrder == RidingOrder.RidingOrderDismount) ? Formation.CavalryDistance(this.UnitSpacing) : Formation.InfantryDistance(this.UnitSpacing);

    internal Vec2 GetWallDirectionOfRelativeFormationLocation(Agent unit)
    {
      if (this.detachedUnits.Contains(unit))
        return Vec2.Invalid;
      Vec2? formationLocation = this.arrangement.GetLocalWallDirectionOfRelativeFormationLocation((IFormationUnit) unit);
      return formationLocation.HasValue ? this.Direction.TransformToParentUnitF(formationLocation.Value) : Vec2.Invalid;
    }

    public Vec2 GetDirectionOfUnit(Agent unit)
    {
      if (this.detachedUnits.Contains(unit))
        return unit.GetMovementDirection().AsVec2;
      Vec2? directionOfUnitOrDefault = this.arrangement.GetLocalDirectionOfUnitOrDefault((IFormationUnit) unit);
      return directionOfUnitOrDefault.HasValue ? this.Direction.TransformToParentUnitF(directionOfUnitOrDefault.Value) : unit.GetMovementDirection().AsVec2;
    }

    private WorldPosition GetOrderPositionOfUnitAux(Agent unit)
    {
      WorldPosition? positionOfUnitOrDefault = this.arrangement.GetWorldPositionOfUnitOrDefault((IFormationUnit) unit);
      if (positionOfUnitOrDefault.HasValue)
        return positionOfUnitOrDefault.Value;
      WorldPosition position = this._movementOrder.GetPosition(this);
      return position.GetNavMesh() == UIntPtr.Zero || !Mission.Current.IsPositionInsideBoundaries(position.AsVec2) ? unit.GetWorldPosition() : position;
    }

    public WorldPosition GetOrderPositionOfUnit(Agent unit)
    {
      if (this.detachedUnits.Contains(unit) && (this.MovementOrder.MovementState != MovementOrder.MovementStateEnum.Charge || unit.Detachment != null && !unit.Detachment.IsLoose))
      {
        WorldFrame? detachmentFrame = this.GetDetachmentFrame(unit);
        return detachmentFrame.HasValue ? detachmentFrame.Value.Origin : WorldPosition.Invalid;
      }
      switch (this.MovementOrder.MovementState)
      {
        case MovementOrder.MovementStateEnum.Charge:
          return WorldPosition.Invalid;
        case MovementOrder.MovementStateEnum.Hold:
          return this.GetOrderPositionOfUnitAux(unit);
        case MovementOrder.MovementStateEnum.Retreat:
          return WorldPosition.Invalid;
        case MovementOrder.MovementStateEnum.StandGround:
          return unit.GetWorldPosition();
        default:
          return WorldPosition.Invalid;
      }
    }

    public Vec2 GetLocalPositionOfUnit(Agent unit) => this.detachedUnits.Contains(unit) ? Vec2.Zero : this.arrangement.GetLocalPositionOfUnitOrDefault((IFormationUnit) unit) ?? Vec2.Zero;

    public Vec2 GetCurrentGlobalPositionOfUnit(Agent unit, bool blendWithOrderDirection)
    {
      if (this.detachedUnits.Contains(unit))
        return unit.Position.AsVec2;
      Vec2? positionOfUnitOrDefault = this.arrangement.GetLocalPositionOfUnitOrDefault((IFormationUnit) unit);
      return positionOfUnitOrDefault.HasValue ? (blendWithOrderDirection ? this.CurrentDirection : this.QuerySystem.EstimatedDirection).TransformToParentUnitF(positionOfUnitOrDefault.Value) + this.CurrentPosition : unit.Position.AsVec2;
    }

    [Conditional("DEBUG")]
    private void TickOrderDebug()
    {
      WorldPosition medianPosition = this.QuerySystem.MedianPosition;
      WorldPosition orderPosition = this.OrderPosition;
      medianPosition.SetVec2(this.QuerySystem.AveragePosition);
      if (orderPosition.IsValid)
      {
        if (!this.MovementOrder.GetPosition(this).IsValid)
        {
          if (this.AI == null)
            return;
          BehaviorComponent activeBehavior = this.AI.ActiveBehavior;
        }
        else
        {
          if (this.AI == null)
            return;
          BehaviorComponent activeBehavior = this.AI.ActiveBehavior;
        }
      }
      else
      {
        if (this.AI == null)
          return;
        BehaviorComponent activeBehavior = this.AI.ActiveBehavior;
      }
    }

    [Conditional("DEBUG")]
    private void TickDebug(float dt)
    {
      if (!MBDebug.IsDisplayingHighLevelAI || this.IsSimulationFormation || this.MovementOrder.OrderEnum != MovementOrder.MovementOrderEnum.FollowEntity)
        return;
      string name = this.MovementOrder.TargetEntity.Name;
    }

    [Conditional("DEBUG")]
    public void DebugArrangements()
    {
      foreach (Team team in (ReadOnlyCollection<Team>) Mission.Current.Teams)
      {
        foreach (Formation formation in team.FormationsIncludingSpecial)
          formation.ApplyActionOnEachUnit((Action<Agent>) (agent => agent.AgentVisuals.SetContourColor(new uint?())));
      }
      this.ApplyActionOnEachUnit((Action<Agent>) (agent => agent.AgentVisuals.SetContourColor(new uint?(4294901760U))));
      Vec2 direction = this.Direction;
      Vec3 vec3_1 = direction.ToVec3();
      vec3_1.RotateAboutZ(1.570796f);
      int num1 = this.IsSimulationFormation ? 1 : 0;
      Vec3 vec3_2 = vec3_1 * this.Width * 0.5f;
      direction = this.Direction;
      Vec3 vec3_3 = direction.ToVec3() * this.Depth * 0.5f;
      int num2 = this.OrderPosition.IsValid ? 1 : 0;
      this.QuerySystem.MedianPosition.SetVec2(this.CurrentPosition);
      this.ApplyActionOnEachUnit((Action<Agent>) (agent =>
      {
        WorldPosition orderPositionOfUnit = this.GetOrderPositionOfUnit(agent);
        if (!orderPositionOfUnit.IsValid)
          return;
        Vec2 directionOfUnit = this.GetDirectionOfUnit(agent);
        double num3 = (double) directionOfUnit.Normalize();
        Vec2 vec2 = directionOfUnit * 0.1f;
        Vec3 vec3_4 = orderPositionOfUnit.GetGroundVec3() + vec2.ToVec3();
        Vec3 vec3_5 = orderPositionOfUnit.GetGroundVec3() - vec2.LeftVec().ToVec3();
        Vec3 vec3_6 = orderPositionOfUnit.GetGroundVec3() + vec2.LeftVec().ToVec3();
        IFormationUnit formationUnit = (IFormationUnit) agent;
        "(" + (object) formationUnit.FormationFileIndex + "," + (object) formationUnit.FormationRankIndex + ")";
      }));
      int num4 = this.OrderPosition.IsValid ? 1 : 0;
      foreach (IDetachment detachment in this.Detachments)
      {
        UsableMachine usableMachine = detachment as UsableMachine;
        RangedSiegeWeapon rangedSiegeWeapon = detachment as RangedSiegeWeapon;
      }
      if (!(this.arrangement is ColumnFormation))
        return;
      this.ApplyActionOnEachUnit((Action<Agent>) (agent =>
      {
        agent.GetFollowedUnit();
        IFormationUnit formationUnit = (IFormationUnit) agent;
        "(" + (object) formationUnit.FormationFileIndex + "," + (object) formationUnit.FormationRankIndex + ")";
      }));
    }

    bool IFormation.GetIsLocalPositionAvailable(
      Vec2 localPosition,
      Vec2? nearestAvailableUnitPositionLocal)
    {
      Vec2 parentUnitF1 = this.Direction.TransformToParentUnitF(localPosition);
      WorldPosition orderPosition = this.OrderPosition;
      orderPosition.SetVec2(this.OrderPosition.AsVec2 + parentUnitF1);
      WorldPosition nearestAvailableUnitPosition = WorldPosition.Invalid;
      if (nearestAvailableUnitPositionLocal.HasValue)
      {
        Vec2 parentUnitF2 = this.Direction.TransformToParentUnitF(nearestAvailableUnitPositionLocal.Value);
        nearestAvailableUnitPosition = this.OrderPosition;
        nearestAvailableUnitPosition.SetVec2(this.OrderPosition.AsVec2 + parentUnitF2);
      }
      float manhattanDistance = (float) ((double) Math.Abs(localPosition.x) + (double) Math.Abs(localPosition.y) + ((double) this.Interval + (double) this.Distance) * 2.0);
      return Mission.Current.IsFormationUnitPositionAvailable(ref this._orderPosition, ref orderPosition, ref nearestAvailableUnitPosition, manhattanDistance, this.Team);
    }

    internal void OnMassUnitTransferStart() => this._postponeCostlyOperations = true;

    internal void OnMassUnitTransferEnd()
    {
      this.FormOrder = this.FormOrder;
      this.QuerySystem.Expire();
      this._postponeCostlyOperations = false;
    }

    public void OnBatchUnitRemovalStart()
    {
      this._postponeCostlyOperations = true;
      this.arrangement.OnBatchRemoveStart();
    }

    public void OnBatchUnitRemovalEnd()
    {
      this.arrangement.OnBatchRemoveEnd();
      this.FormOrder = this.FormOrder;
      this.QuerySystem.ExpireAfterUnitAddRemove();
      this._postponeCostlyOperations = false;
    }

    public void OnUnitAddedOrRemoved()
    {
      if (!this._postponeCostlyOperations)
      {
        this.FormOrder = this.FormOrder;
        this.QuerySystem.ExpireAfterUnitAddRemove();
      }
      Action<Formation> unitCountChanged = this.OnUnitCountChanged;
      if (unitCountChanged == null)
        return;
      unitCountChanged(this);
    }

    private void OnUnitAttachedOrDetached() => this.FormOrder = this.FormOrder;

    internal void AddUnit(Agent unit)
    {
      int num = this.arrangement.AddUnit((IFormationUnit) unit) ? 1 : 0;
      if (num != 0 && this.HasPlayer && unit.IsAIControlled)
      {
        IFormationUnit secondUnit = this.arrangement.GetAllUnits().FirstOrDefault<IFormationUnit>((Func<IFormationUnit, bool>) (u => !((Agent) u).IsAIControlled));
        if (secondUnit != null)
          this.SwitchUnitLocations((IFormationUnit) unit, secondUnit);
      }
      if (num != 0 && Mission.Current.HasMissionBehaviour<AmmoSupplyLogic>() && Mission.Current.GetMissionBehaviour<AmmoSupplyLogic>().IsAgentEligibleForAmmoSupply(unit))
      {
        unit.SetScriptedCombatFlags(unit.GetScriptedCombatFlags() | Agent.AISpecialCombatModeFlags.IgnoreAmmoLimitForRangeCalculation);
        unit.ResetAiWaitBeforeShootFactor();
        unit.UpdateAgentStats();
      }
      if (!unit.IsAIControlled)
        this.HasPlayer = true;
      if (this._initialClass == FormationClass.NumberOfAllFormations && unit.Character != null)
        this._initialClass = (FormationClass) unit.Character.DefaultFormationGroup;
      this.MovementOrder.OnUnitJoinOrLeave(this, unit, true);
      this.OnUnitAddedOrRemoved();
    }

    internal void RemoveUnit(Agent unit)
    {
      if (this.detachedUnits.Contains(unit))
      {
        this.Detachments.FirstOrDefault<IDetachment>((Func<IDetachment, bool>) (d => d.Agents.Contains<Agent>(unit))).RemoveAgent(unit);
        this.detachedUnits.Remove(unit);
        this.looseDetachedUnits.Remove(unit);
        unit.Detachment = (IDetachment) null;
        unit.DetachmentWeight = -1f;
      }
      else
        this.arrangement.RemoveUnit((IFormationUnit) unit);
      this.MovementOrder.OnUnitJoinOrLeave(this, unit, false);
      this.OnUnitAddedOrRemoved();
      if (unit != this.Captain)
        return;
      this.Captain = (Agent) null;
    }

    internal void DetachUnit(Agent unit, bool isLoose)
    {
      this.arrangement.RemoveUnit((IFormationUnit) unit);
      this.detachedUnits.Add(unit);
      if (isLoose)
        this.looseDetachedUnits.Add(unit);
      this.OnUnitAttachedOrDetached();
    }

    internal void AttachUnit(Agent unit)
    {
      this.detachedUnits.Remove(unit);
      this.looseDetachedUnits.Remove(unit);
      this.arrangement.AddUnit((IFormationUnit) unit);
      unit.DetachmentWeight = -1f;
      unit.Detachment = (IDetachment) null;
      this.OnUnitAttachedOrDetached();
    }

    internal bool IsUnitDetached(Agent unit) => this.detachedUnits.Contains(unit);

    internal void OnUnitDetachmentChanged(
      Agent unit,
      bool isOldDetachmentLoose,
      bool isNewDetachmentLoose)
    {
      if (isOldDetachmentLoose && !isNewDetachmentLoose)
      {
        this.looseDetachedUnits.Remove(unit);
      }
      else
      {
        if (!(!isOldDetachmentLoose & isNewDetachmentLoose))
          return;
        this.looseDetachedUnits.Add(unit);
      }
    }

    private IFormationUnit GetClosestUnitToAux(
      Vec2 position,
      List<IFormationUnit> unitsWithSpaces,
      float? maxDistance)
    {
      if (unitsWithSpaces == null)
        unitsWithSpaces = this.arrangement.GetAllUnits();
      IFormationUnit formationUnit = (IFormationUnit) null;
      float num1 = maxDistance.HasValue ? maxDistance.Value * maxDistance.Value : float.MaxValue;
      for (int index = 0; index < unitsWithSpaces.Count; ++index)
      {
        IFormationUnit unitsWithSpace = unitsWithSpaces[index];
        if (unitsWithSpace != null)
        {
          float num2 = ((Agent) unitsWithSpace).Position.AsVec2.DistanceSquared(position);
          if ((double) num1 > (double) num2)
          {
            num1 = num2;
            formationUnit = unitsWithSpace;
          }
        }
      }
      return formationUnit;
    }

    IFormationUnit IFormation.GetClosestUnitTo(
      Vec2 localPosition,
      List<IFormationUnit> unitsWithSpaces,
      float? maxDistance)
    {
      return this.GetClosestUnitToAux(this.OrderPosition.AsVec2 + this.Direction.TransformToParentUnitF(localPosition), unitsWithSpaces, maxDistance);
    }

    IFormationUnit IFormation.GetClosestUnitTo(
      IFormationUnit targetUnit,
      List<IFormationUnit> unitsWithSpaces,
      float? maxDistance)
    {
      return this.GetClosestUnitToAux(((Agent) targetUnit).Position.AsVec2, unitsWithSpaces, maxDistance);
    }

    internal void SwitchUnitLocations(IFormationUnit firstUnit, IFormationUnit secondUnit)
    {
      if (this.detachedUnits.Any<Agent>((Func<Agent, bool>) (du => du == firstUnit || du == secondUnit)) || firstUnit.FormationFileIndex == -1 && secondUnit.FormationFileIndex == -1)
        return;
      if (firstUnit.FormationFileIndex == -1)
        this.arrangement.SwitchUnitLocationsWithUnpositionedUnit(secondUnit, firstUnit);
      else if (secondUnit.FormationFileIndex == -1)
        this.arrangement.SwitchUnitLocationsWithUnpositionedUnit(firstUnit, secondUnit);
      else
        this.arrangement.SwitchUnitLocations(firstUnit, secondUnit);
    }

    internal IFormationUnit GetNeighbourUnit(
      IFormationUnit unit,
      int fileOffset,
      int rankOffset)
    {
      foreach (Agent detachedUnit in this.detachedUnits)
      {
        if (detachedUnit == unit)
          return (IFormationUnit) null;
      }
      return this.arrangement.GetNeighbourUnit(unit, fileOffset, rankOffset);
    }

    internal Vec2 CurrentDirection => (this.QuerySystem.EstimatedDirection * 0.8f + this.Direction * 0.2f).Normalized();

    public Vec2 CurrentPosition => this.QuerySystem.GetAveragePositionWithMaxAge(0.1f) + this.CurrentDirection.TransformToParentUnitF(-this.OrderLocalAveragePosition);

    private void SetOrderPosition(WorldPosition pos) => this._orderPosition = pos;

    public void SetPositioning(WorldPosition? position = null, Vec2? direction = null, int? unitSpacing = null)
    {
      WorldPosition orderPosition = this.OrderPosition;
      Vec2 direction1 = this.Direction;
      WorldPosition? newPosition = new WorldPosition?();
      Vec2? newDirection = new Vec2?();
      int? nullable = new int?();
      if (position.HasValue && position.Value.IsValid)
      {
        if (!this.HasBeenPositioned && !this.IsSimulationFormation)
          this.HasBeenPositioned = true;
        if (!Mission.Current.IsPositionInsideBoundaries(position.Value.AsVec2))
        {
          Vec2 boundaryPosition = Mission.Current.GetClosestBoundaryPosition(position.Value.AsVec2);
          if (this.OrderPosition.AsVec2 != boundaryPosition)
          {
            WorldPosition worldPosition = position.Value;
            worldPosition.SetVec2(boundaryPosition);
            newPosition = new WorldPosition?(worldPosition);
          }
        }
        else if (this.OrderPosition.AsVec2 != position.Value.AsVec2)
          newPosition = new WorldPosition?(position.Value);
      }
      if (direction.HasValue && this.Direction != direction.Value)
        newDirection = new Vec2?(direction.Value);
      if (unitSpacing.HasValue && this.UnitSpacing != unitSpacing.Value)
        nullable = new int?(unitSpacing.Value);
      int num = newPosition.HasValue || newDirection.HasValue ? 1 : (nullable.HasValue ? 1 : 0);
      if (num != 0)
      {
        this.arrangement.BeforeFormationFrameChange();
        if (newPosition.HasValue)
          this._orderPosition = newPosition.Value;
        if (newDirection.HasValue)
          this._direction = newDirection.Value;
        if (nullable.HasValue)
        {
          this._unitSpacing = nullable.Value;
          Action<Formation> unitSpacingChanged = this.OnUnitSpacingChanged;
          if (unitSpacingChanged != null)
            unitSpacingChanged(this);
          this.Arrangement_OnShapeChanged();
          this.arrangement.AreLocalPositionsDirty = true;
        }
      }
      if (!this.IsSimulationFormation && this.arrangement.IsTurnBackwardsNecessary(orderPosition, newPosition, direction1, newDirection))
        this.arrangement.TurnBackwards();
      if (num != 0)
        this.arrangement.OnFormationFrameChanged();
      if (!newPosition.HasValue)
        return;
      this.ArrangementOrder.OnOrderPositionChanged(this, orderPosition.Position);
    }

    internal void Tick(float dt)
    {
      if ((!this.Team.HasTeamAi ? 1 : (this.IsAIControlled ? 0 : (!this.Team.IsPlayerSergeant ? 1 : 0))) == 0 && this.CountOfUnitsWithoutDetachedOnes > 0)
        this.AI.Tick();
      else
        this.IsAITickedAfterSplit = true;
      int num = 0;
      while (true)
      {
        MovementOrder movementOrder = this.MovementOrder;
        if (!movementOrder.IsApplicable(this) && num++ < 10)
        {
          movementOrder = this.MovementOrder;
          this.MovementOrder = movementOrder.GetSubstituteOrder(this);
        }
        else
          break;
      }
      this._arrangementOrder.Tick(this);
      this._movementOrder.Tick(this);
      WorldPosition position = this._movementOrder.GetPosition(this);
      Vec2 direction = this._facingOrder.GetDirection(this, this._movementOrder._targetAgent);
      if (position.IsValid || direction.IsValid)
        this.SetPositioning(new WorldPosition?(position), new Vec2?(direction));
      this.TickDetachments(dt);
      Action<Formation> onTick = this.OnTick;
      if (onTick != null)
        onTick(this);
      this.SmoothAverageUnitPosition(dt);
      if (!this.isArrangementShapeChanged)
        return;
      Action onShapeChanged = this.OnShapeChanged;
      if (onShapeChanged != null)
        onShapeChanged();
      this.isArrangementShapeChanged = false;
    }

    public WorldPosition FrontAttachmentPoint
    {
      get
      {
        WorldPosition orderPosition = this.OrderPosition;
        orderPosition.SetVec2(orderPosition.AsVec2 + this.Direction * (float) (((double) this.Distance + (double) this.UnitDiameter) / 2.0));
        return orderPosition;
      }
    }

    public WorldPosition RearAttachmentPoint
    {
      get
      {
        WorldPosition orderPosition = this.OrderPosition;
        orderPosition.SetVec2(orderPosition.AsVec2 - this.Direction * (this.Depth + (float) (((double) this.Distance - (double) this.UnitDiameter) / 2.0)));
        return orderPosition;
      }
    }

    public WorldPosition LeftAttachmentPoint
    {
      get
      {
        WorldPosition orderPosition = this.OrderPosition;
        orderPosition.SetVec2(orderPosition.AsVec2 + this.Direction.LeftVec() * (float) (((double) this.Width + (double) this.Interval) / 2.0));
        return orderPosition;
      }
    }

    public WorldPosition RightAttachmentPoint
    {
      get
      {
        WorldPosition orderPosition = this.OrderPosition;
        orderPosition.SetVec2(orderPosition.AsVec2 + this.Direction.RightVec() * (float) (((double) this.Width + (double) this.Interval) / 2.0));
        return orderPosition;
      }
    }

    private int GetHeroPointForCaptainSelection(Agent agent) => agent.Character.Level + 100 * agent.Character.GetSkillValue(DefaultSkills.Charm);

    private void OnCaptainChanged() => this.ApplyActionOnEachUnit((Action<Agent>) (agent =>
    {
      agent.UpdateAgentProperties();
      MissionGameModels.Current.AgentStatCalculateModel.UpdateAgentStats(agent, agent.AgentDrivenProperties);
    }));

    public Agent Captain
    {
      get => this._captain;
      set
      {
        if (this._captain == value)
          return;
        this._captain = value;
        this.OnCaptainChanged();
      }
    }

    internal float GetAverageMaximumMovementSpeedOfUnits()
    {
      if (this.CountOfUnitsWithoutDetachedOnes == 0)
        return 0.1f;
      float num = 0.0f;
      foreach (Agent withoutDetachedOne in this.GetUnitsWithoutDetachedOnes())
        num += withoutDetachedOne.RunSpeedCached;
      return num / (float) this.CountOfUnitsWithoutDetachedOnes;
    }

    internal float GetMovementSpeedOfUnits()
    {
      float? runRestriction;
      float? walkRestriction;
      this.ArrangementOrder.GetMovementSpeedRestriction(out runRestriction, out walkRestriction);
      if (!runRestriction.HasValue && !walkRestriction.HasValue)
        runRestriction = new float?(1f);
      return walkRestriction.HasValue ? (this.CountOfUnits == 0 ? 0.1f : (this.CountOfUnitsWithoutDetachedOnes == 0 ? (IEnumerable<Agent>) this.detachedUnits : this.GetUnitsWithoutDetachedOnes()).Min<Agent>((Func<Agent, float>) (u => u.WalkSpeedCached)) * walkRestriction.Value) : (this.CountOfUnits == 0 ? 0.1f : (this.CountOfUnitsWithoutDetachedOnes == 0 ? (IEnumerable<Agent>) this.detachedUnits : this.GetUnitsWithoutDetachedOnes()).Average<Agent>((Func<Agent, float>) (u => u.RunSpeedCached)) * runRestriction.Value);
    }

    public float GetFormationPower()
    {
      float sum = 0.0f;
      this.ApplyActionOnEachUnit((Action<Agent>) (agent => sum += agent.CharPowerCached));
      return sum;
    }

    public float GetFormationMeleeFightingPower()
    {
      float sum = 0.0f;
      this.ApplyActionOnEachUnit((Action<Agent>) (agent => sum += agent.CharPowerCached * (this.FormationIndex == FormationClass.Ranged || this.FormationIndex == FormationClass.HorseArcher ? 0.4f : 1f)));
      return sum;
    }

    internal void OnAgentLostMount(Agent agent)
    {
      if (agent.IsDetachedFromFormation)
        return;
      this._arrangement.OnUnitLostMount((IFormationUnit) agent);
    }

    public void OnFormationDispersed()
    {
      this.arrangement.OnFormationDispersed();
      this.ApplyActionOnEachUnit((Action<Agent>) (agent => agent.UpdateCachedAndFormationValues(true, false)));
    }

    internal void JoinDetachment(IDetachment detachment)
    {
      if (!this.Team.DetachmentManager.Detachments.Contains(detachment))
        this.Team.DetachmentManager.MakeDetachment(detachment);
      this._detachments.Add(detachment);
      this.Team.DetachmentManager.OnFormationJoinDetachment(this, detachment);
    }

    internal void FormAttackEntityDetachment(GameEntity targetEntity)
    {
      this.AttackEntityOrderDetachment = new AttackEntityOrderDetachment(targetEntity);
      this.JoinDetachment((IDetachment) this.AttackEntityOrderDetachment);
    }

    internal void LeaveDetachment(IDetachment detachment)
    {
      foreach (Agent agent in detachment.Agents.Where<Agent>((Func<Agent, bool>) (a => a.Formation == this && !a.IsMainAgent)).ToList<Agent>())
      {
        detachment.RemoveAgent(agent);
        this.AttachUnit(agent);
      }
      this._detachments.Remove(detachment);
      this.Team.DetachmentManager.OnFormationLeaveDetachment(this, detachment);
    }

    internal void DisbandAttackEntityDetachment()
    {
      if (this.AttackEntityOrderDetachment == null)
        return;
      this.Team.DetachmentManager.DestroyDetachment((IDetachment) this.AttackEntityOrderDetachment);
      this.AttackEntityOrderDetachment = (AttackEntityOrderDetachment) null;
    }

    internal IDetachment GetDetachment(Agent agent) => this.Detachments.FirstOrDefault<IDetachment>((Func<IDetachment, bool>) (d => d.Agents.Contains<Agent>(agent)));

    internal IDetachment GetDetachmentOrDefault(Agent agent) => this.Detachments.SingleOrDefault<IDetachment>((Func<IDetachment, bool>) (d => d.Agents.Contains<Agent>(agent)));

    internal WorldFrame? GetDetachmentFrame(Agent agent) => this.Detachments.FirstOrDefault<IDetachment>((Func<IDetachment, bool>) (d => d.Agents.Contains<Agent>(agent))).GetAgentFrame(agent);

    [Conditional("DEBUG")]
    private void AssertDetachments()
    {
      foreach (Agent detachedUnit in this.detachedUnits)
      {
        Agent du = detachedUnit;
        this.Detachments.Count<IDetachment>((Func<IDetachment, bool>) (d => d.Agents.Contains<Agent>(du)));
      }
      foreach (Agent agent in this.Detachments.SelectMany<IDetachment, Agent>((Func<IDetachment, IEnumerable<Agent>>) (d => d.Agents)).Where<Agent>((Func<Agent, bool>) (a => a.Formation == this)))
        ;
      foreach (Agent looseDetachedUnit in this.looseDetachedUnits)
      {
        Agent ldu = looseDetachedUnit;
        this.Detachments.Where<IDetachment>((Func<IDetachment, bool>) (d => d.IsLoose)).Count<IDetachment>((Func<IDetachment, bool>) (d => d.Agents.Contains<Agent>(ldu)));
      }
      foreach (Agent agent in this.Detachments.Where<IDetachment>((Func<IDetachment, bool>) (d => d.IsLoose)).SelectMany<IDetachment, Agent>((Func<IDetachment, IEnumerable<Agent>>) (d => d.Agents)).Where<Agent>((Func<Agent, bool>) (a => a.Formation == this)))
        ;
      foreach (IDetachment detachment in this.Detachments)
        ;
    }

    internal void Rearrange(IFormationArrangement arrangement)
    {
      if (this.arrangement.GetType() == arrangement.GetType())
        return;
      IFormationArrangement arrangement1 = this.arrangement;
      IFormationArrangement arrangement2 = arrangement;
      this.arrangement = arrangement;
      arrangement1.RearrangeTo(arrangement2);
      arrangement2.RearrangeFrom(arrangement1);
      arrangement1.RearrangeTransferUnits(arrangement2);
      this.FormOrder = this.FormOrder;
      this.MovementOrder.OnArrangementChanged(this);
    }

    void IFormation.SetUnitToFollow(
      IFormationUnit unit,
      IFormationUnit toFollow,
      Vec2 vector)
    {
      (unit as Agent).SetColumnwiseFollowAgent(toFollow as Agent, ref vector);
    }

    public void GetClosestAttachmentPointToTheOrderPosition(
      out bool left,
      out bool right,
      out bool front,
      out bool rear)
    {
      WorldPosition orderPosition = this.OrderPosition;
      float val1_1 = orderPosition.AsVec2.DistanceSquared(this.LeftAttachmentPoint.AsVec2);
      float val2_1 = orderPosition.AsVec2.DistanceSquared(this.RightAttachmentPoint.AsVec2);
      float val1_2 = orderPosition.AsVec2.DistanceSquared(this.FrontAttachmentPoint.AsVec2);
      float val2_2 = orderPosition.AsVec2.DistanceSquared(this.RearAttachmentPoint.AsVec2);
      left = false;
      right = false;
      front = false;
      rear = false;
      float num = Math.Min(Math.Min(val1_1, val2_1), Math.Min(val1_2, val2_2));
      if ((double) val1_2 == (double) num)
        front = true;
      else if ((double) val2_2 == (double) num)
        rear = true;
      else if ((double) val1_1 == (double) num)
        left = true;
      else
        right = true;
    }

    private IFormationUnit GetNeighbourUnitOfLeftSide(IFormationUnit unit) => ((IEnumerable<IFormationUnit>) this.detachedUnits).Contains<IFormationUnit>(unit) ? (IFormationUnit) null : this.arrangement.GetNeighbourUnitOfLeftSide(unit);

    private IFormationUnit GetNeighbourUnitOfRightSide(IFormationUnit unit) => ((IEnumerable<IFormationUnit>) this.detachedUnits).Contains<IFormationUnit>(unit) ? (IFormationUnit) null : this.arrangement.GetNeighbourUnitOfRightSide(unit);

    internal bool IsLoose => ArrangementOrder.GetUnitLooseness(this.ArrangementOrder.OrderEnum);

    public Vec2 GetMiddleFrontUnitPositionOffset() => this.Direction.TransformToParentUnitF(this.arrangement.GetLocalPositionOfReservedUnitPosition());

    public Vec2 SmoothedAverageUnitPosition => this._smoothedAverageUnitPosition;

    private void SmoothAverageUnitPosition(float dt) => this._smoothedAverageUnitPosition = !this._smoothedAverageUnitPosition.IsValid ? this.QuerySystem.AveragePosition : Vec2.Lerp(this._smoothedAverageUnitPosition, this.QuerySystem.AveragePosition, dt * 3f);

    internal void TickForColumnArrangementInitialPositioning(Formation formation)
    {
      if ((double) (this.ReferencePosition.Value - this.OrderPosition.AsVec2).LengthSquared < 1.0)
        return;
      this.ArrangementOrder.RearrangeAux(this, true);
    }

    private static float TransformCustomWidthBetweenArrangementOrientations(
      ArrangementOrder.ArrangementOrderEnum orderTypeOld,
      ArrangementOrder.ArrangementOrderEnum orderTypeNew,
      float currentCustomWidth)
    {
      if (orderTypeOld != ArrangementOrder.ArrangementOrderEnum.Column && orderTypeNew == ArrangementOrder.ArrangementOrderEnum.Column)
        return currentCustomWidth * 0.1f;
      return orderTypeOld == ArrangementOrder.ArrangementOrderEnum.Column && orderTypeNew != ArrangementOrder.ArrangementOrderEnum.Column ? currentCustomWidth / 0.1f : currentCustomWidth;
    }

    bool IFormation.BatchUnitPositions(
      MBList<Vec2i> orderedPositionIndices,
      MBList<Vec2> orderedLocalPositions,
      MBList2D<int> availabilityTable,
      MBList2D<WorldPosition> globalPositionTable,
      int fileCount,
      int rankCount)
    {
      WorldPosition orderPosition = this.OrderPosition;
      if (!((!orderPosition.IsValid ? UIntPtr.Zero : orderPosition.GetNavMesh()) != UIntPtr.Zero))
        return false;
      Mission.Current.BatchFormationUnitPositions(orderedPositionIndices, orderedLocalPositions, availabilityTable, globalPositionTable, orderPosition, this.Direction, fileCount, rankCount);
      return true;
    }

    public static bool IsDefenseRelatedAIDrivenComponent(DrivenProperty drivenProperty) => drivenProperty == DrivenProperty.AIDecideOnAttackChance || drivenProperty == DrivenProperty.AIAttackOnDecideChance || (drivenProperty == DrivenProperty.AIAttackOnParryChance || drivenProperty == DrivenProperty.AiUseShieldAgainstEnemyMissileProbability) || drivenProperty == DrivenProperty.AiDefendWithShieldDecisionChanceValue;

    internal float CalculateFormationDirectionEnforcingFactorForRank(int rankIndex) => rankIndex == -1 ? 0.0f : this.ArrangementOrder.CalculateFormationDirectionEnforcingFactorForRank(rankIndex, this.arrangement.RankCount);

    private void UpdateAgentDrivenPropertiesBasedOnOrderDefensiveness() => this.ApplyActionOnEachUnit((Action<Agent>) (agent => agent.Defensiveness = (float) this._formationOrderDefensivenessFactor));

    public static List<WorldFrame> GetFormationFramesForBeforeFormationCreation(
      float width,
      int manCount,
      bool areMounted,
      WorldPosition spawnOrigin,
      Mat3 spawnRotation)
    {
      List<Formation.AgentArrangementData> agentArrangementDataList = new List<Formation.AgentArrangementData>();
      Formation formation = new Formation((Team) null, -1);
      formation.SetOrderPosition(spawnOrigin);
      formation._direction = spawnRotation.f.AsVec2;
      LineFormation lineFormation = new LineFormation((IFormation) formation);
      lineFormation.Width = width;
      for (int index = 0; index < manCount; ++index)
        agentArrangementDataList.Add(new Formation.AgentArrangementData(index, (IFormationArrangement) lineFormation));
      lineFormation.OnFormationFrameChanged();
      foreach (Formation.AgentArrangementData agentArrangementData in agentArrangementDataList)
        lineFormation.AddUnit((IFormationUnit) agentArrangementData);
      List<WorldFrame> worldFrameList = new List<WorldFrame>();
      int positionIndicesCount = lineFormation.GetCachedOrderedAndAvailableUnitPositionIndicesCount();
      for (int i = 0; i < positionIndicesCount; ++i)
      {
        Vec2i unitPositionIndexAt = lineFormation.GetCachedOrderedAndAvailableUnitPositionIndexAt(i);
        WorldPosition globalPositionAtIndex = lineFormation.GetGlobalPositionAtIndex(unitPositionIndexAt.X, unitPositionIndexAt.Y);
        worldFrameList.Add(new WorldFrame(spawnRotation, globalPositionAtIndex));
      }
      return worldFrameList;
    }

    public static float GetDefaultUnitDiameter(bool isMounted) => isMounted ? ManagedParameters.Instance.GetManagedParameter(ManagedParametersEnum.QuadrupedalRadius) * 2f : ManagedParameters.Instance.GetManagedParameter(ManagedParametersEnum.BipedalRadius) * 2f;

    public static float GetDefaultMinimumInterval(bool isMounted) => isMounted ? 0.4f : 0.0f;

    public void BeginSpawn(int unitCount, bool isMounted)
    {
      this.IsSpawning = true;
      this.OverridenUnitCount = new int?(unitCount);
      this.OverridenHasAnyMountedUnit = new bool?(isMounted);
    }

    public void EndSpawn()
    {
      this.IsSpawning = false;
      this.OverridenUnitCount = new int?();
      this.OverridenHasAnyMountedUnit = new bool?();
    }

    private class AgentArrangementData : IFormationUnit
    {
      public int Index;

      public IFormationArrangement Formation { get; set; }

      public int FormationFileIndex { get; set; } = -1;

      public int FormationRankIndex { get; set; } = -1;

      public IFormationUnit FollowedUnit { get; set; }

      public AgentArrangementData(int index, IFormationArrangement arrangement)
      {
        this.Index = index;
        this.Formation = arrangement;
      }
    }

    [StructLayout(LayoutKind.Explicit)]
    private struct Int64ToInt32
    {
      [FieldOffset(0)]
      public long Int64Value;
      [FieldOffset(0)]
      public int LeftInt32;
      [FieldOffset(4)]
      public int RightInt32;
    }
  }
}
