// Decompiled with JetBrains decompiler
// Type: TaleWorlds.MountAndBlade.Mission
// Assembly: TaleWorlds.MountAndBlade, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3C0E3F42-756F-4407-B817-06BA412A78B0
// Assembly location: E:\SteamLibrary\steamapps\common\Mount & Blade II Bannerlord\bin\Win64_Shipping_Client\TaleWorlds.MountAndBlade.dll

using JetBrains.Annotations;
using NetworkMessages.FromServer;
using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using TaleWorlds.Core;
using TaleWorlds.DotNet;
using TaleWorlds.Engine;
using TaleWorlds.InputSystem;
using TaleWorlds.Library;
using TaleWorlds.ModuleManager;
using TaleWorlds.MountAndBlade.Network;
using TaleWorlds.MountAndBlade.Network.Messages;

namespace TaleWorlds.MountAndBlade
{
  public sealed class Mission : DotNetObject, IMission
  {
    public const int MaxRuntimeMissionObjects = 4095;
    private UIntPtr _missionPointer;
    private int _lastSceneMissionObjectIdCount;
    private int _lastRuntimeMissionObjectIdCount;
    private readonly List<MissionObject> _activeMissionObjects;
    private readonly List<MissionObject> _missionObjects;
    private readonly List<Mission.DynamicallyCreatedEntity> _addedEntitiesInfo;
    private readonly Stack<(int, float)> _emptyRuntimeMissionObjectIds;
    private MissionMode _missionMode;
    private static bool _isCameraFirstPerson = false;
    private static readonly object GetNearbyAgentsAuxLock = new object();
    public const int MaxDamage = 2000;
    public static bool DisableDying = false;
    private List<Agent> _activeAgents;
    private List<Agent> _allAgents;
    public IAgentVisualCreator AgentVisualCreator;
    private ConcurrentQueue<CombatLogData> _combatLogsCreated = new ConcurrentQueue<CombatLogData>();
    private bool _missionEnded;
    public bool IsFriendlyMission = true;
    public BasicCultureObject MusicCulture;
    private List<MissionLogic> _missionLogics;
    private MissionState _missionState;
    private MissionDeploymentPlan _currentDeploymentPlan;
    private List<MissionBehaviour> _otherMissionBehaviours;
    private List<IMissionListener> _listeners = new List<IMissionListener>();
    private List<MissionBehaviour> _missionBehaviourList;
    private Dictionary<int, Mission.Missile> _missiles;
    private Agent _mainAgent;
    public const int MaxNavMeshId = 1000000;
    private const int MaxNavMeshPerDynamicObject = 10;
    private int _NextDynamicNavMeshIdStart = 1000010;
    public bool AllowAiTicking = true;
    private List<FleePosition>[] _fleePositions = new List<FleePosition>[3];
    private readonly object _lockHelper = new object();
    private int _usedSpawnPathIndex = -1;
    private float _randomMiddlePointAddition;
    private Agent _mainAgentServer;
    public float MissionCloseTimeAfterFinish = 30f;
    private float _missionEndTime;
    public float NextCheckTimeEndMission = 10f;
    private SoundEvent _ambientSoundEvent;
    private int _agentCount;
    private Path _usedSpawnPath;
    public int NumOfFormationsSpawnedTeamOne;
    public int NumOfFormationsSpawnedTeamTwo;
    public bool IsOrderMenuOpen;
    private BasicTimer _leaveMissionTimer;
    private float _exitTimeInSeconds = 0.6f;
    private Timer _inMissionLoadingScreenTimer;
    private Action _onLoadingEndedAction;
    private const float SpeedBonusFactorForSwing = 0.7f;
    private const float SpeedBonusFactorForThrust = 0.5f;
    private const float NavigationMeshHeightLimit = 1.5f;
    private readonly List<Mission.DynamicEntityInfo> _dynamicEntities = new List<Mission.DynamicEntityInfo>();

    internal UIntPtr Pointer => this._missionPointer;

    private MissionInitializerRecord InitializerRecord { get; set; }

    public string SceneName => this.InitializerRecord.SceneName;

    public string SceneLevels => this.InitializerRecord.SceneLevels;

    public bool HasValidTerrainType => this.InitializerRecord.TerrainType >= 0;

    public TerrainType TerrainType => !this.HasValidTerrainType ? TerrainType.Water : (TerrainType) this.InitializerRecord.TerrainType;

    public Scene Scene { get; private set; }

    public IEnumerable<GameEntity> GetActiveEntitiesWithScriptComponentOfType<T>() => this._activeMissionObjects.Where<MissionObject>((Func<MissionObject, bool>) (amo => amo is T)).Select<MissionObject, GameEntity>((Func<MissionObject, GameEntity>) (amo => amo.GameEntity));

    public void AddActiveMissionObject(MissionObject missionObject)
    {
      this._missionObjects.Add(missionObject);
      this._activeMissionObjects.Add(missionObject);
    }

    public void ActivateMissionObject(MissionObject missionObject) => this._activeMissionObjects.Add(missionObject);

    public void DeactivateMissionObject(MissionObject missionObject) => this._activeMissionObjects.Remove(missionObject);

    public IReadOnlyList<MissionObject> ActiveMissionObjects => (IReadOnlyList<MissionObject>) this._activeMissionObjects;

    public IReadOnlyList<MissionObject> MissionObjects => (IReadOnlyList<MissionObject>) this._missionObjects;

    public IReadOnlyList<Mission.DynamicallyCreatedEntity> AddedEntitiesInfo => (IReadOnlyList<Mission.DynamicallyCreatedEntity>) this._addedEntitiesInfo;

    public Mission.MBBoundaryCollection Boundaries { get; private set; }

    public bool IsTeleportingAgents { get; set; }

    public bool ForceTickOccasionally { get; set; }

    private void FinalizeMission()
    {
      TeamAISiegeComponent.OnMissionFinalize();
      MBAPI.IMBMission.FinalizeMission(this.Pointer);
      this._missionPointer = UIntPtr.Zero;
      Mission.Current = (Mission) null;
    }

    public Mission.MissionCombatType CombatType
    {
      get => (Mission.MissionCombatType) MBAPI.IMBMission.GetCombatType(this.Pointer);
      set => MBAPI.IMBMission.SetCombatType(this.Pointer, (int) value);
    }

    public void SetMissionCombatType(Mission.MissionCombatType missionCombatType) => MBAPI.IMBMission.SetCombatType(this.Pointer, (int) missionCombatType);

    public MissionMode Mode => this._missionMode;

    public void ConversationCharacterChanged()
    {
      foreach (IMissionListener listener in this._listeners)
        listener.OnConversationCharacterChanged();
    }

    public void SetMissionMode(MissionMode newMode, bool atStart)
    {
      if (this._missionMode == newMode)
        return;
      MissionMode missionMode = this._missionMode;
      this._missionMode = newMode;
      if (this.CurrentState == Mission.State.Over)
        return;
      for (int index = 0; index < this._missionBehaviourList.Count; ++index)
        this._missionBehaviourList[index].OnMissionModeChange(missionMode, atStart);
      foreach (IMissionListener listener in this._listeners)
        listener.OnMissionModeChange(missionMode, atStart);
    }

    private Mission.AgentCreationResult CreateAgentInternal(
      AgentFlag agentFlags,
      int forcedAgentIndex,
      bool isFemale,
      ref AgentSpawnData spawnData,
      ref AgentCapsuleData capsuleData,
      ref AgentVisualsNativeData agentVisualsNativeData,
      ref AnimationSystemData animationSystemData,
      int instanceNo)
    {
      return MBAPI.IMBMission.CreateAgent(this.Pointer, (ulong) agentFlags, forcedAgentIndex, isFemale, ref spawnData, ref capsuleData.BodyCap, ref capsuleData.CrouchedBodyCap, ref agentVisualsNativeData, ref animationSystemData, instanceNo);
    }

    public float Time => MBAPI.IMBMission.GetTime(this.Pointer);

    public float GetAverageFps() => MBAPI.IMBMission.GetAverageFps(this.Pointer);

    public static bool ToggleDisableFallAvoid() => MBAPI.IMBMission.ToggleDisableFallAvoid();

    public bool IsPositionInsideBoundaries(Vec2 position) => MBAPI.IMBMission.IsPositionInsideBoundaries(this.Pointer, position);

    private bool IsFormationUnitPositionAvailableAux(
      ref WorldPosition formationPosition,
      ref WorldPosition unitPosition,
      ref WorldPosition nearestAvailableUnitPosition,
      float manhattanDistance)
    {
      return MBAPI.IMBMission.IsFormationUnitPositionAvailable(this.Pointer, ref formationPosition, ref unitPosition, ref nearestAvailableUnitPosition, manhattanDistance);
    }

    public Agent RayCastForClosestAgent(
      Vec3 sourcePoint,
      Vec3 targetPoint,
      out float collisionDistance,
      int excludedAgentIndex = -1,
      float rayThickness = 0.01f)
    {
      collisionDistance = float.NaN;
      return MBAPI.IMBMission.RayCastForClosestAgent(this.Pointer, sourcePoint, targetPoint, excludedAgentIndex, ref collisionDistance, rayThickness);
    }

    public bool RayCastForClosestAgentsLimbs(
      Vec3 sourcePoint,
      Vec3 targetPoint,
      int excludeAgentIndex,
      out float collisionDistance,
      ref int agentIndex,
      ref sbyte boneIndex)
    {
      collisionDistance = float.NaN;
      return MBAPI.IMBMission.RayCastForClosestAgentsLimbs(this.Pointer, sourcePoint, targetPoint, excludeAgentIndex, ref collisionDistance, ref agentIndex, ref boneIndex);
    }

    public bool RayCastForClosestAgentsLimbs(
      Vec3 sourcePoint,
      Vec3 targetPoint,
      out float collisionDistance,
      ref int agentIndex,
      ref sbyte boneIndex)
    {
      collisionDistance = float.NaN;
      return MBAPI.IMBMission.RayCastForClosestAgentsLimbs(this.Pointer, sourcePoint, targetPoint, -1, ref collisionDistance, ref agentIndex, ref boneIndex);
    }

    public bool RayCastForGivenAgentsLimbs(
      Vec3 sourcePoint,
      Vec3 rayFinishPoint,
      int givenAgentIndex,
      ref float collisionDistance,
      ref sbyte boneIndex)
    {
      return MBAPI.IMBMission.RayCastForGivenAgentsLimbs(this.Pointer, sourcePoint, rayFinishPoint, givenAgentIndex, ref collisionDistance, ref boneIndex);
    }

    internal AgentProximityMap.ProximityMapSearchStructInternal ProximityMapBeginSearch(
      Vec2 searchPos,
      float searchRadius)
    {
      return MBAPI.IMBMission.ProximityMapBeginSearch(this.Pointer, searchPos, searchRadius);
    }

    internal float ProximityMapMaxSearchRadius() => MBAPI.IMBMission.ProximityMapMaxSearchRadius(this.Pointer);

    public float GetBiggestAgentCollisionPadding() => MBAPI.IMBMission.GetBiggestAgentCollisionPadding(this.Pointer);

    public void SetReportStuckAgentsMode(bool value) => MBAPI.IMBMission.SetReportStuckAgentsMode(this.Pointer, value);

    internal void BatchFormationUnitPositions(
      MBList<Vec2i> orderedPositionIndices,
      MBList<Vec2> orderedLocalPositions,
      MBList2D<int> availabilityTable,
      MBList2D<WorldPosition> globalPositionTable,
      WorldPosition orderPosition,
      Vec2 direction,
      int fileCount,
      int rankCount)
    {
      MBAPI.IMBMission.BatchFormationUnitPositions(this.Pointer, orderedPositionIndices.RawArray, orderedLocalPositions.RawArray, availabilityTable.RawArray, globalPositionTable.RawArray, orderPosition, direction, fileCount, rankCount);
    }

    internal void ProximityMapFindNext(
      ref AgentProximityMap.ProximityMapSearchStructInternal searchStruct)
    {
      MBAPI.IMBMission.ProximityMapFindNext(this.Pointer, ref searchStruct);
    }

    [UsedImplicitly]
    [MBCallback]
    public void ResetMission()
    {
      foreach (IMissionListener missionListener in this._listeners.ToArray())
        missionListener.OnResetMission();
      foreach (Agent activeAgent in this._activeAgents)
        activeAgent.OnRemove();
      foreach (Agent allAgent in this._allAgents)
        allAgent.OnDelete();
      foreach (MissionBehaviour missionBehaviour in this.MissionBehaviours)
        missionBehaviour.OnClearScene();
      this.NumOfFormationsSpawnedTeamOne = 0;
      this.NumOfFormationsSpawnedTeamTwo = 0;
      foreach (Team team in (ReadOnlyCollection<Team>) this.Teams)
        team.Reset();
      MBAPI.IMBMission.ClearScene(this.Pointer);
      this._activeAgents.Clear();
      this._allAgents.Clear();
      this.MainAgent = (Agent) null;
      this.ClearMissiles();
      this._missiles.Clear();
      this._agentCount = 0;
      this.ResetMissionObjects();
      this.RemoveSpawnedMissionObjects();
      this._activeMissionObjects.Clear();
      this._activeMissionObjects.AddRange((IEnumerable<MissionObject>) this.MissionObjects);
      this.Scene.ClearDecals();
      PropertyChangedEventHandler onMissionReset = this.OnMissionReset;
      if (onMissionReset == null)
        return;
      onMissionReset((object) this, (PropertyChangedEventArgs) null);
    }

    public event PropertyChangedEventHandler OnMissionReset;

    public void Initialize()
    {
      Mission.Current = this;
      this.CurrentState = Mission.State.Initializing;
      MissionInitializerRecord initializerRecord = this.InitializerRecord;
      LoadingWindow.EnableGlobalLoadingWindow();
      MBAPI.IMBMission.InitializeMission(this.Pointer, ref initializerRecord);
    }

    [UsedImplicitly]
    [MBCallback]
    internal void OnSceneCreated(Scene scene) => this.Scene = scene;

    internal void Tick(float dt) => MBAPI.IMBMission.Tick(this.Pointer, dt);

    public void MakeSound(
      int soundIndex,
      Vec3 position,
      bool soundCanBePredicted,
      bool isReliable,
      int relatedAgent1,
      int relatedAgent2)
    {
      MBAPI.IMBMission.MakeSound(this.Pointer, soundIndex, position, soundCanBePredicted, isReliable, relatedAgent1, relatedAgent2);
    }

    public void MakeSound(
      int soundIndex,
      Vec3 position,
      bool soundCanBePredicted,
      bool isReliable,
      int relatedAgent1,
      int relatedAgent2,
      ref SoundEventParameter parameter)
    {
      MBAPI.IMBMission.MakeSoundWithParameter(this.Pointer, soundIndex, position, soundCanBePredicted, isReliable, relatedAgent1, relatedAgent2, parameter);
    }

    public void MakeSoundOnlyOnRelatedPeer(int soundIndex, Vec3 position, int relatedAgent) => MBAPI.IMBMission.MakeSoundOnlyOnRelatedPeer(this.Pointer, soundIndex, position, relatedAgent);

    public void AddSoundAlarmFactorToAgents(int ownerId, Vec3 position, float alarmFactor) => MBAPI.IMBMission.AddSoundAlarmFactorToAgents(this.Pointer, ownerId, position, alarmFactor);

    public void AddDynamicallySpawnedMissionObjectInfo(Mission.DynamicallyCreatedEntity entityInfo) => this._addedEntitiesInfo.Add(entityInfo);

    private void RemoveDynamicallySpawnedMissionObjectInfo(MissionObjectId id)
    {
      Mission.DynamicallyCreatedEntity dynamicallyCreatedEntity = this._addedEntitiesInfo.FirstOrDefault<Mission.DynamicallyCreatedEntity>((Func<Mission.DynamicallyCreatedEntity, bool>) (x => x.ObjectId == id));
      if (dynamicallyCreatedEntity == null)
        return;
      this._addedEntitiesInfo.Remove(dynamicallyCreatedEntity);
    }

    private int AddMissileAux(
      int forcedMissileIndex,
      bool isPrediction,
      Agent shooterAgent,
      in WeaponData weaponData,
      WeaponStatsData[] weaponStatsData,
      float damageBonus,
      ref Vec3 position,
      ref Vec3 direction,
      ref Mat3 orientation,
      float baseSpeed,
      float speed,
      bool addRigidBody,
      GameEntity gameEntityToIgnore,
      bool isPrimaryWeaponShot,
      out GameEntity missileEntity)
    {
      UIntPtr missileEntity1;
      int num = MBAPI.IMBMission.AddMissile(this.Pointer, isPrediction, shooterAgent.Index, in weaponData, weaponStatsData, weaponStatsData.Length, damageBonus, ref position, ref direction, ref orientation, baseSpeed, speed, addRigidBody, gameEntityToIgnore != null ? gameEntityToIgnore.Pointer : UIntPtr.Zero, forcedMissileIndex, isPrimaryWeaponShot, out missileEntity1);
      missileEntity = isPrediction ? (GameEntity) null : new GameEntity(missileEntity1);
      return num;
    }

    public Vec3 GetMissileCollisionPoint(
      Vec3 missileStartingPosition,
      Vec3 missileDirection,
      float missileSpeed,
      in WeaponData weaponData)
    {
      return MBAPI.IMBMission.GetMissileCollisionPoint(this.Pointer, missileStartingPosition, missileDirection, missileSpeed, in weaponData);
    }

    public void RemoveMissileAsClient(int missileIndex) => MBAPI.IMBMission.RemoveMissile(this.Pointer, missileIndex);

    public static float GetMissileVerticalAimCorrection(
      Vec3 vecToTarget,
      float missileStartingSpeed,
      ref WeaponStatsData weaponStatsData,
      float airFrictionConstant)
    {
      return MBAPI.IMBMission.GetMissileVerticalAimCorrection(vecToTarget, missileStartingSpeed, ref weaponStatsData, airFrictionConstant);
    }

    public static float GetMissileRange(float missileStartingSpeed, float heightDifference) => MBAPI.IMBMission.GetMissileRange(missileStartingSpeed, heightDifference);

    public void PrepareMissileWeaponForDrop(int missileIndex) => MBAPI.IMBMission.PrepareMissileWeaponForDrop(this.Pointer, missileIndex);

    public void AddParticleSystemBurstByName(
      string particleSystem,
      MatrixFrame frame,
      bool synchThroughNetwork)
    {
      MBAPI.IMBMission.AddParticleSystemBurstByName(this.Pointer, particleSystem, ref frame, synchThroughNetwork);
    }

    public int EnemyAlarmStateIndicator => MBAPI.IMBMission.GetEnemyAlarmStateIndicator(this.Pointer);

    public float PlayerAlarmIndicator => MBAPI.IMBMission.GetPlayerAlarmIndicator(this.Pointer);

    public bool IsLoadingFinished => MBAPI.IMBMission.GetIsLoadingFinished(this.Pointer);

    public Vec2 GetClosestBoundaryPosition(Vec2 position) => MBAPI.IMBMission.GetClosestBoundaryPosition(this.Pointer, position);

    private void ResetMissionObjects()
    {
      for (int index = this._dynamicEntities.Count - 1; index >= 0; --index)
      {
        Mission.DynamicEntityInfo dynamicEntity = this._dynamicEntities[index];
        dynamicEntity.Entity.RemoveEnginePhysics();
        dynamicEntity.Entity.Remove(74);
        this._dynamicEntities.RemoveAt(index);
      }
      foreach (MissionObject missionObject in (IEnumerable<MissionObject>) this.MissionObjects)
      {
        if (missionObject.CreatedAtRuntime)
          break;
        missionObject.OnMissionReset();
      }
    }

    private void RemoveSpawnedMissionObjects()
    {
      MissionObject[] array = this._missionObjects.ToArray();
      for (int index = array.Length - 1; index >= 0; --index)
      {
        MissionObject missionObject = array[index];
        if (missionObject.CreatedAtRuntime)
        {
          if ((NativeObject) missionObject.GameEntity != (NativeObject) null)
          {
            missionObject.GameEntity.RemoveAllChildren();
            missionObject.GameEntity.Remove(75);
          }
        }
        else
          break;
      }
      this._lastRuntimeMissionObjectIdCount = 0;
      this._emptyRuntimeMissionObjectIds.Clear();
      this._addedEntitiesInfo.Clear();
    }

    public int GetFreeRuntimeMissionObjectId()
    {
      float time = MBCommon.TimeType.Mission.GetTime();
      int num = -1;
      if (this._emptyRuntimeMissionObjectIds.Count > 0)
      {
        if ((double) time - (double) this._emptyRuntimeMissionObjectIds.Peek().Item2 > 30.0 || this._lastRuntimeMissionObjectIdCount >= 4095)
        {
          num = this._emptyRuntimeMissionObjectIds.Pop().Item1;
        }
        else
        {
          num = this._lastRuntimeMissionObjectIdCount;
          ++this._lastRuntimeMissionObjectIdCount;
        }
      }
      else if (this._lastRuntimeMissionObjectIdCount < 4095)
      {
        num = this._lastRuntimeMissionObjectIdCount;
        ++this._lastRuntimeMissionObjectIdCount;
      }
      return num;
    }

    private void ReturnRuntimeMissionObjectId(int id) => this._emptyRuntimeMissionObjectIds.Push((id, MBCommon.TimeType.Mission.GetTime()));

    public int GetFreeSceneMissionObjectId()
    {
      int missionObjectIdCount = this._lastSceneMissionObjectIdCount;
      ++this._lastSceneMissionObjectIdCount;
      return missionObjectIdCount;
    }

    public void SetCameraFrame(MatrixFrame cameraFrame, float zoomFactor)
    {
      cameraFrame.Fill();
      MBAPI.IMBMission.SetCameraFrame(this.Pointer, ref cameraFrame, zoomFactor);
    }

    public MatrixFrame GetCameraFrame() => MBAPI.IMBMission.GetCameraFrame(this.Pointer);

    public bool CameraIsFirstPerson
    {
      get => Mission._isCameraFirstPerson;
      set
      {
        if (Mission._isCameraFirstPerson == value)
          return;
        Mission._isCameraFirstPerson = value;
        MBAPI.IMBMission.SetCameraIsFirstPerson(value);
        this.ResetFirstThirdPersonView();
      }
    }

    public static float CameraAddedDistance
    {
      get => BannerlordConfig.CombatCameraDistance;
      set
      {
        if ((double) value == (double) BannerlordConfig.CombatCameraDistance)
          return;
        BannerlordConfig.CombatCameraDistance = value;
      }
    }

    public float ClearSceneTimerElapsedTime => MBAPI.IMBMission.GetClearSceneTimerElapsedTime(this.Pointer);

    public void ResetFirstThirdPersonView() => MBAPI.IMBMission.ResetFirstThirdPersonView(this.Pointer);

    public bool PauseAITick
    {
      get => MBAPI.IMBMission.GetPauseAITick(this.Pointer);
      set => MBAPI.IMBMission.SetPauseAITick(this.Pointer, value);
    }

    public float TimeSpeed
    {
      get => MBAPI.IMBMission.GetTimeSpeed(this.Pointer);
      set => MBAPI.IMBMission.SetTimeSpeed(this.Pointer, value);
    }

    public float TimeSpeedEnd => MBAPI.IMBMission.GetTimeSpeedEnd(this.Pointer);

    public float TimeSpeedPeriod
    {
      get => MBAPI.IMBMission.GetTimeSpeedPeriod(this.Pointer);
      set => MBAPI.IMBMission.SetTimeSpeedPeriod(this.Pointer, value);
    }

    public float TimeSpeedTimerElapsedTime => MBAPI.IMBMission.GetTimeSpeedTimerElapsedTime(this.Pointer);

    public void ClearAgentActions() => MBAPI.IMBMission.ClearAgentActions(this.Pointer);

    public void ClearMissiles() => MBAPI.IMBMission.ClearMissiles(this.Pointer);

    public void ClearCorpses(bool isMissionReset) => MBAPI.IMBMission.ClearCorpses(this.Pointer, isMissionReset);

    private Agent FindAgentWithIndexAux(int index) => index >= 0 ? MBAPI.IMBMission.FindAgentWithIndex(this.Pointer, index) : (Agent) null;

    private Agent GetClosestEnemyAgent(MBTeam team, Vec3 position, float radius) => MBAPI.IMBMission.GetClosestEnemy(this.Pointer, team.Index, position, radius);

    private Agent GetClosestAllyAgent(MBTeam team, Vec3 position, float radius) => MBAPI.IMBMission.GetClosestAlly(this.Pointer, team.Index, position, radius);

    private int GetNearbyEnemyAgentCount(MBTeam team, Vec2 position, float radius)
    {
      int allyCount = 0;
      int enemyCount = 0;
      MBAPI.IMBMission.GetAgentCountAroundPosition(this.Pointer, team.Index, position, radius, ref allyCount, ref enemyCount);
      return enemyCount;
    }

    public void ClearResources(bool forceClearGPUResources)
    {
      this.Scene = (Scene) null;
      this.ClearUnreferencedResources(forceClearGPUResources);
    }

    public void ClearUnreferencedResources(bool forceClearGPUResources)
    {
      Common.MemoryCleanup();
      if (!forceClearGPUResources)
        return;
      MBAPI.IMBMission.ClearResources(this.Pointer);
    }

    internal void OnEntityHit(
      GameEntity entity,
      Agent attackerAgent,
      int inflictedDamage,
      DamageTypes damageType,
      Vec3 impactPosition,
      Vec3 impactDirection,
      in MissionWeapon weapon)
    {
      bool flag1 = false;
      for (; (NativeObject) entity != (NativeObject) null; entity = entity.Parent)
      {
        bool flag2 = false;
        foreach (MissionObject scriptComponent in entity.GetScriptComponents<MissionObject>())
        {
          if (scriptComponent.IsOnHitCallable())
          {
            bool reportDamage;
            if (scriptComponent.OnHit(attackerAgent, inflictedDamage, impactPosition, impactDirection, in weapon, (ScriptComponentBehaviour) null, out reportDamage) && !flag2)
              flag2 = true;
            flag1 |= reportDamage;
          }
        }
        if (flag2)
          break;
      }
      if (!flag1 || attackerAgent.IsMount || attackerAgent.IsAIControlled)
        return;
      CombatLogData combatLog;
      ref CombatLogData local = ref combatLog;
      int num1 = attackerAgent.IsHuman ? 1 : 0;
      int num2 = attackerAgent.IsMine ? 1 : 0;
      int num3 = attackerAgent.RiderAgent != null ? 1 : 0;
      Agent riderAgent = attackerAgent.RiderAgent;
      int num4 = riderAgent != null ? (riderAgent.IsMine ? 1 : 0) : 0;
      int num5 = attackerAgent.IsMount ? 1 : 0;
      local = new CombatLogData(false, num1 != 0, num2 != 0, num3 != 0, num4 != 0, num5 != 0, false, false, false, false, false, false, true, false, false, false, 0.0f);
      combatLog.DamageType = damageType;
      combatLog.InflictedDamage = inflictedDamage;
      this.AddCombatLogSafe(attackerAgent, (Agent) null, entity, combatLog);
    }

    public float GetMainAgentMaxCameraZoom() => this.MainAgent != null ? MissionGameModels.Current.AgentStatCalculateModel.GetMaxCameraZoom(this.MainAgent) : 1f;

    public float GetAverageMoraleOfAgentsWithIndices(int[] agentIndices) => MBAPI.IMBMission.GetAverageMoraleOfAgents(this.Pointer, agentIndices.Length, agentIndices);

    public WorldPosition GetBestSlopeTowardsDirection(
      ref WorldPosition centerPosition,
      float halfSize,
      ref WorldPosition referencePosition)
    {
      return MBAPI.IMBMission.GetBestSlopeTowardsDirection(this.Pointer, ref centerPosition, halfSize, ref referencePosition);
    }

    public WorldPosition GetBestSlopeAngleHeightPosForDefending(
      WorldPosition enemyPosition,
      WorldPosition defendingPosition,
      int sampleSize,
      float distanceRatioAllowedFromDefendedPos,
      float distanceSqrdAllowedFromBoundary,
      float cosinusOfBestSlope,
      float cosinusOfMaxAcceptedSlope,
      float minSlopeScore,
      float maxSlopeScore,
      float excessiveSlopePenalty,
      float nearConeCenterRatio,
      float nearConeCenterBonus,
      float heightDifferenceCeiling,
      float maxDisplacementPenalty)
    {
      return MBAPI.IMBMission.GetBestSlopeAngleHeightPosForDefending(this.Pointer, enemyPosition, defendingPosition, sampleSize, distanceRatioAllowedFromDefendedPos, distanceSqrdAllowedFromBoundary, cosinusOfBestSlope, cosinusOfMaxAcceptedSlope, minSlopeScore, maxSlopeScore, excessiveSlopePenalty, nearConeCenterRatio, nearConeCenterBonus, heightDifferenceCeiling, maxDisplacementPenalty);
    }

    public Vec2 GetAveragePositionOfAgents(List<Agent> agents)
    {
      int num = 0;
      Vec2 zero = Vec2.Zero;
      foreach (Agent agent in agents)
      {
        ++num;
        zero += agent.Position.AsVec2;
      }
      return num == 0 ? Vec2.Invalid : zero * (1f / (float) num);
    }

    public Vec3 GetAverageDirectionOfAgents(IEnumerable<Agent> agents)
    {
      Vec3 vec3 = Vec3.Zero;
      int num = 0;
      foreach (Agent agent in agents)
      {
        vec3 = agent.GetMovementDirection();
        ++num;
      }
      return num == 0 ? Vec3.Zero : vec3 * (1f / (float) num);
    }

    public WorldPosition GetMedianPositionOfAgents(
      IEnumerable<Agent> agents,
      Vec2 averagePosition)
    {
      float num1 = float.MaxValue;
      Agent agent1 = (Agent) null;
      foreach (Agent agent2 in agents)
      {
        float num2 = agent2.Position.AsVec2.DistanceSquared(averagePosition);
        if ((double) num2 <= (double) num1)
        {
          agent1 = agent2;
          num1 = num2;
        }
      }
      return agent1.GetWorldPosition();
    }

    private IEnumerable<Agent> GetNearbyAgentsAux(
      Vec2 center,
      float radius,
      MBTeam team,
      Mission.GetNearbyAgentsAuxType type)
    {
      List<Agent> agentList = new List<Agent>();
      int[] agentIds = new int[40];
      lock (Mission.GetNearbyAgentsAuxLock)
      {
        int agentsArrayOffset = 0;
        while (true)
        {
          int retrievedAgentCount = -1;
          MBAPI.IMBMission.GetNearbyAgentsAux(this.Pointer, center, radius, team.Index, (int) type, agentsArrayOffset, agentIds, 40, ref retrievedAgentCount);
          for (int index = 0; index < retrievedAgentCount; ++index)
          {
            Agent managedObjectWithId = DotNetObject.GetManagedObjectWithId(agentIds[index]) as Agent;
            agentList.Add(managedObjectWithId);
          }
          if (retrievedAgentCount >= 40)
            agentsArrayOffset += 40;
          else
            break;
        }
      }
      return (IEnumerable<Agent>) agentList;
    }

    public void SetRandomDecideTimeOfAgentsWithIndices(
      int[] agentIndices,
      float? minAIReactionTime = null,
      float? maxAIReactionTime = null)
    {
      if (!minAIReactionTime.HasValue || !maxAIReactionTime.HasValue)
      {
        maxAIReactionTime = new float?(-1f);
        minAIReactionTime = maxAIReactionTime;
      }
      MBAPI.IMBMission.SetRandomDecideTimeOfAgents(this.Pointer, agentIndices.Length, agentIndices, minAIReactionTime.Value, maxAIReactionTime.Value);
    }

    public void SetBowMissileSpeedModifier(float modifier) => MBAPI.IMBMission.SetBowMissileSpeedModifier(this.Pointer, modifier);

    public void SetCrossbowMissileSpeedModifier(float modifier) => MBAPI.IMBMission.SetCrossbowMissileSpeedModifier(this.Pointer, modifier);

    public void SetThrowingMissileSpeedModifier(float modifier) => MBAPI.IMBMission.SetThrowingMissileSpeedModifier(this.Pointer, modifier);

    public void SetMissileRangeModifier(float modifier) => MBAPI.IMBMission.SetMissileRangeModifier(this.Pointer, modifier);

    public void SetLastMovementKeyPressed(Agent.MovementControlFlag lastMovementKeyPressed) => MBAPI.IMBMission.SetLastMovementKeyPressed(this.Pointer, lastMovementKeyPressed);

    public Vec2 GetWeightedPointOfEnemies(Agent agent, Vec2 basePoint) => MBAPI.IMBMission.GetWeightedPointOfEnemies(this.Pointer, agent.Index, basePoint);

    public bool GetPathBetweenPositions(ref NavigationData navData) => MBAPI.IMBMission.GetNavigationPoints(this.Pointer, ref navData);

    public void SetNavigationFaceCostWithIdAroundPosition(
      int navigationFaceId,
      Vec3 position,
      float cost)
    {
      MBAPI.IMBMission.SetNavigationFaceCostWithIdAroundPosition(this.Pointer, navigationFaceId, position, cost);
    }

    public WorldPosition GetStraightPathToTarget(
      Vec2 targetPosition,
      WorldPosition startingPosition,
      float samplingDistance = 1f,
      bool stopAtObstacle = true)
    {
      return MBAPI.IMBMission.GetStraightPathToTarget(this.Pointer, targetPosition, startingPosition, samplingDistance, stopAtObstacle);
    }

    public void FastForwardMission(float startTime, float endTime) => MBAPI.IMBMission.FastForwardMission(this.Pointer, startTime, endTime);

    public int GetDebugAgent() => MBAPI.IMBMission.GetDebugAgent(this.Pointer);

    public void AddAiDebugText(string str) => MBAPI.IMBMission.AddAiDebugText(this.Pointer, str);

    public void SetDebugAgent(int index) => MBAPI.IMBMission.SetDebugAgent(this.Pointer, index);

    public static float GetFirstPersonFov() => BannerlordConfig.FirstPersonFov;

    public event PropertyChangedEventHandler OnMainAgentChanged;

    public event Mission.OnBeforeAgentRemovedDelegate OnBeforeAgentRemoved;

    public static Mission Current { get; private set; }

    public IInputContext InputManager { get; set; }

    public bool IsFieldBattle => this.GetMissionBehaviour<FieldBattleController>() != null;

    public bool HasSpawnPath => (NativeObject) this._usedSpawnPath != (NativeObject) null;

    public IReadOnlyList<Agent> Agents => (IReadOnlyList<Agent>) this._activeAgents;

    public IReadOnlyList<Agent> AllAgents => (IReadOnlyList<Agent>) this._allAgents;

    public BattleSideEnum RetreatSide { get; private set; } = BattleSideEnum.None;

    public bool IsFastForward { get; private set; }

    public bool FixedDeltaTimeMode { get; set; }

    public float FixedDeltaTime { get; set; }

    public Mission.State CurrentState { get; private set; }

    public Mission.TeamCollection Teams { get; private set; }

    public Team AttackerTeam => this.Teams.Attacker;

    public Team DefenderTeam => this.Teams.Defender;

    public Team AttackerAllyTeam => this.Teams.AttackerAlly;

    public Team DefenderAllyTeam => this.Teams.DefenderAlly;

    public Team PlayerTeam
    {
      get => this.Teams.Player;
      set => this.Teams.Player = value;
    }

    public Team PlayerEnemyTeam => this.Teams.PlayerEnemy;

    public Team PlayerAllyTeam => this.Teams.PlayerAlly;

    public Team SpectatorTeam { get; set; }

    IMissionTeam IMission.PlayerTeam => (IMissionTeam) this.PlayerTeam;

    public MissionResult MissionResult { get; private set; }

    public bool IsMissionEnding => this.CurrentState != Mission.State.Over && this.MissionEnded();

    public IEnumerable<MissionLogic> MissionLogics => (IEnumerable<MissionLogic>) this._missionLogics;

    public List<MissionBehaviour> MissionBehaviours => this._missionBehaviourList;

    public IEnumerable<Mission.Missile> Missiles => (IEnumerable<Mission.Missile>) this._missiles.Values;

    public bool NeedsMemoryCleanup { get; internal set; }

    public void MakeDeploymentPlan()
    {
      if (this._currentDeploymentPlan.IsPlanMade)
        this.UnmakeDeploymentPlan();
      this._currentDeploymentPlan.PlanBattleDeployment(this._usedSpawnPath, this._randomMiddlePointAddition);
    }

    public void UnmakeDeploymentPlan()
    {
      this._currentDeploymentPlan.ClearTroops();
      this._currentDeploymentPlan.ClearPlan();
    }

    public void SetDeploymentPlanSpawnWithHorses(BattleSideEnum side, bool spawnWithHorses) => this._currentDeploymentPlan.SetSpawnWithHorsesForSide(side, spawnWithHorses);

    public void AddTroopsToDeploymentPlan(
      BattleSideEnum side,
      FormationClass fClass,
      int troopCount)
    {
      this._currentDeploymentPlan.AddTroops(side, fClass, troopCount);
    }

    public bool IsDeploymentPlanMade() => this._currentDeploymentPlan.IsPlanMade;

    public void AddFleePosition(FleePosition fleePosition)
    {
      BattleSideEnum side = fleePosition.GetSide();
      switch (side)
      {
        case BattleSideEnum.None:
          for (int index = 0; index < this._fleePositions.Length; ++index)
            this._fleePositions[index].Add(fleePosition);
          break;
        case BattleSideEnum.NumSides:
          break;
        default:
          this._fleePositions[(int) (side + 1)].Add(fleePosition);
          break;
      }
    }

    internal MBReadOnlyList<FleePosition> GetFleePositionsForSide(
      BattleSideEnum side)
    {
      int index;
      switch (side)
      {
        case BattleSideEnum.None:
          index = 0;
          break;
        case BattleSideEnum.NumSides:
          return (MBReadOnlyList<FleePosition>) null;
        default:
          index = (int) (side + 1);
          break;
      }
      return this._fleePositions[index].GetReadOnlyList<FleePosition>();
    }

    private WorldPosition GetClosestFleePosition(
      MBReadOnlyList<FleePosition> availableFleePositions,
      WorldPosition runnerPosition,
      float runnerSpeed,
      bool runnerHasMount,
      MBReadOnlyList<(float, WorldPosition, int, Vec2, Vec2, bool)> chaserData)
    {
      int num1 = chaserData != null ? chaserData.Count : 0;
      if (availableFleePositions.Count > 0)
      {
        float[] numArray = new float[availableFleePositions.Count];
        WorldPosition[] worldPositionArray = new WorldPosition[availableFleePositions.Count];
        for (int index = 0; index < availableFleePositions.Count; ++index)
        {
          numArray[index] = 1f;
          worldPositionArray[index] = new WorldPosition(Mission.Current.Scene, UIntPtr.Zero, availableFleePositions[index].GetClosestPointToEscape(runnerPosition.AsVec2), false);
          worldPositionArray[index].SetVec2(worldPositionArray[index].AsVec2 - runnerPosition.AsVec2);
        }
        for (int index1 = 0; index1 < num1; ++index1)
        {
          float num2 = chaserData[index1].Item1;
          if ((double) num2 > 0.0)
          {
            Vec2 asVec2 = chaserData[index1].Item2.AsVec2;
            int num3 = chaserData[index1].Item3;
            Vec2 vec2_1;
            if (num3 > 1)
            {
              Vec2 lineSegmentBegin = chaserData[index1].Item4;
              Vec2 lineSegmentEnd = chaserData[index1].Item5;
              vec2_1 = MBMath.GetClosestPointInLineSegmentToPoint(runnerPosition.AsVec2, lineSegmentBegin, lineSegmentEnd) - runnerPosition.AsVec2;
            }
            else
              vec2_1 = asVec2 - runnerPosition.AsVec2;
            for (int index2 = 0; index2 < availableFleePositions.Count; ++index2)
            {
              ref Vec2 local1 = ref vec2_1;
              Vec2 vec2_2 = worldPositionArray[index2].AsVec2;
              Vec2 v1 = vec2_2.Normalized();
              float num4 = local1.DotProduct(v1);
              if ((double) num4 > 0.0)
              {
                ref Vec2 local2 = ref vec2_1;
                vec2_2 = worldPositionArray[index2].AsVec2;
                vec2_2 = vec2_2.LeftVec();
                Vec2 v2 = vec2_2.Normalized();
                float num5 = Math.Max(Math.Abs(local2.DotProduct(v2)) / num2, 1f);
                float num6 = Math.Max(num4 / runnerSpeed, 1f);
                if ((double) num6 > (double) num5)
                {
                  float num7 = num6 / num5 / num4;
                  numArray[index2] += num7 * (float) num3;
                }
              }
            }
          }
        }
        for (int index = 0; index < availableFleePositions.Count; ++index)
        {
          WorldPosition point1 = new WorldPosition(this.Scene, UIntPtr.Zero, availableFleePositions[index].GetClosestPointToEscape(runnerPosition.AsVec2), false);
          float pathDistance;
          if (Mission.Current.Scene.GetPathDistanceBetweenPositions(ref runnerPosition, ref point1, 0.0f, out pathDistance))
            numArray[index] *= pathDistance;
          else
            numArray[index] = float.MaxValue;
        }
        int index3 = -1;
        float maxValue = float.MaxValue;
        for (int index1 = 0; index1 < availableFleePositions.Count; ++index1)
        {
          if ((double) maxValue > (double) numArray[index1])
          {
            index3 = index1;
            maxValue = numArray[index1];
          }
        }
        if (index3 >= 0)
        {
          Vec3 closestPointToEscape = availableFleePositions[index3].GetClosestPointToEscape(runnerPosition.AsVec2);
          return new WorldPosition(Mission.Current.Scene, UIntPtr.Zero, closestPointToEscape, false);
        }
      }
      float[] numArray1 = new float[4];
      for (int index = 0; index < num1; ++index)
      {
        Vec2 asVec2 = chaserData[index].Item2.AsVec2;
        int num2 = chaserData[index].Item3;
        Vec2 vec2_1;
        if (num2 > 1)
        {
          Vec2 lineSegmentBegin = chaserData[index].Item4;
          Vec2 lineSegmentEnd = chaserData[index].Item5;
          vec2_1 = MBMath.GetClosestPointInLineSegmentToPoint(runnerPosition.AsVec2, lineSegmentBegin, lineSegmentEnd) - runnerPosition.AsVec2;
        }
        else
          vec2_1 = asVec2 - runnerPosition.AsVec2;
        float length = vec2_1.Length;
        if (chaserData[index].Item6)
          length *= 0.5f;
        if (runnerHasMount)
          length *= 2f;
        double num3 = (double) MBMath.ClampFloat((float) (1.0 - ((double) length - 40.0) / 40.0), 0.01f, 1f);
        Vec2 vec2_2 = vec2_1.Normalized();
        float num4 = 1.2f;
        double num5 = (double) num2;
        double num6 = num3 * num5 * (double) num4;
        float num7 = (float) num6 * Math.Abs(vec2_2.x);
        float num8 = (float) num6 * Math.Abs(vec2_2.y);
        numArray1[(double) vec2_2.y < 0.0 ? 0 : 1] -= num8;
        numArray1[(double) vec2_2.x < 0.0 ? 2 : 3] -= num7;
        numArray1[(double) vec2_2.y < 0.0 ? 1 : 0] += num8;
        numArray1[(double) vec2_2.x < 0.0 ? 3 : 2] += num7;
      }
      float num9 = 0.04f;
      Vec3 min;
      Vec3 max;
      this.Scene.GetBoundingBox(out min, out max);
      Vec2 boundaryPosition1 = this.GetClosestBoundaryPosition(new Vec2(runnerPosition.X, min.y));
      Vec2 boundaryPosition2 = this.GetClosestBoundaryPosition(new Vec2(runnerPosition.X, max.y));
      Vec2 boundaryPosition3 = this.GetClosestBoundaryPosition(new Vec2(min.x, runnerPosition.Y));
      Vec2 boundaryPosition4 = this.GetClosestBoundaryPosition(new Vec2(max.x, runnerPosition.Y));
      float num10 = boundaryPosition2.y - boundaryPosition1.y;
      float num11 = boundaryPosition4.x - boundaryPosition3.x;
      numArray1[0] += (num10 - (runnerPosition.Y - boundaryPosition1.y)) * num9;
      numArray1[1] += (num10 - (boundaryPosition2.y - runnerPosition.Y)) * num9;
      numArray1[2] += (num11 - (runnerPosition.X - boundaryPosition3.x)) * num9;
      numArray1[3] += (num11 - (boundaryPosition4.x - runnerPosition.X)) * num9;
      Vec2 xy = (double) numArray1[0] < (double) numArray1[1] || (double) numArray1[0] < (double) numArray1[2] || (double) numArray1[0] < (double) numArray1[3] ? ((double) numArray1[1] < (double) numArray1[2] || (double) numArray1[1] < (double) numArray1[3] ? ((double) numArray1[2] < (double) numArray1[3] ? new Vec2(boundaryPosition4.x, boundaryPosition4.y) : new Vec2(boundaryPosition3.x, boundaryPosition3.y)) : new Vec2(boundaryPosition2.x, boundaryPosition2.y)) : new Vec2(boundaryPosition1.x, boundaryPosition1.y);
      return new WorldPosition(Mission.Current.Scene, UIntPtr.Zero, new Vec3(xy, runnerPosition.GetNavMeshZ()), false);
    }

    public WorldPosition GetAlternatePositionForNavmeshlessOrOutOfBoundsPosition(
      Vec2 directionTowards,
      WorldPosition originalPosition,
      ref float positionPenalty)
    {
      return MBAPI.IMBMission.GetAlternatePositionForNavmeshlessOrOutOfBoundsPosition(this.Pointer, ref directionTowards, ref originalPosition, ref positionPenalty);
    }

    public WorldPosition GetClosestFleePositionForFormation(Formation formation)
    {
      WorldPosition medianPosition = formation.QuerySystem.MedianPosition;
      float movementSpeedMaximum = formation.QuerySystem.MovementSpeedMaximum;
      bool runnerHasMount = formation.QuerySystem.IsCavalryFormation || formation.QuerySystem.IsRangedCavalryFormation;
      Team team = formation.Team;
      team.UpdateCachedEnemyDataForFleeing();
      return this.GetClosestFleePosition(this.GetFleePositionsForSide(team.Side), medianPosition, movementSpeedMaximum, runnerHasMount, team.CachedEnemyDataForFleeing);
    }

    public WorldPosition GetClosestFleePositionForAgent(Agent agent)
    {
      WorldPosition worldPosition = agent.GetWorldPosition();
      float forwardUnlimitedSpeed = agent.MaximumForwardUnlimitedSpeed;
      Team team = agent.Team;
      BattleSideEnum side = BattleSideEnum.None;
      bool runnerHasMount = agent.MountAgent != null;
      if (team != null)
      {
        team.UpdateCachedEnemyDataForFleeing();
        side = team.Side;
      }
      return this.GetClosestFleePosition(this.GetFleePositionsForSide(side), worldPosition, forwardUnlimitedSpeed, runnerHasMount, team?.CachedEnemyDataForFleeing);
    }

    public int GetNextDynamicNavMeshIdStart()
    {
      int dynamicNavMeshIdStart = this._NextDynamicNavMeshIdStart;
      this._NextDynamicNavMeshIdStart += 10;
      return dynamicNavMeshIdStart;
    }

    public Agent MainAgent
    {
      get => this._mainAgent;
      set
      {
        this._mainAgent = value;
        if (this.OnMainAgentChanged != null)
          this.OnMainAgentChanged((object) this, (PropertyChangedEventArgs) null);
        if (MBNetwork.IsClient)
          return;
        this.MainAgentServer = this._mainAgent;
      }
    }

    public Agent MainAgentServer
    {
      get => this._mainAgentServer;
      set => this._mainAgentServer = value;
    }

    public bool IsInventoryAccessible { private get; set; }

    public bool IsInventoryAccessAllowed => (Game.Current.GameType.IsInventoryAccessibleAtMission || Mission.Current != null && Mission.Current.Mode != MissionMode.Stealth && (Mission.Current.Mode != MissionMode.Battle && Mission.Current.Mode != MissionMode.Deployment) && (Mission.Current.Mode != MissionMode.Duel && Mission.Current.Mode != MissionMode.CutScene)) && this.IsInventoryAccessible;

    public bool IsQuestScreenAccessible { private get; set; }

    public bool IsQuestScreenAccessAllowed => (Game.Current.GameType.IsQuestScreenAccessibleAtMission || Mission.Current != null && Mission.Current.Mode != MissionMode.Battle && (Mission.Current.Mode != MissionMode.Deployment && Mission.Current.Mode != MissionMode.Duel)) && this.IsQuestScreenAccessible;

    public bool IsCharacterWindowAccessible { private get; set; }

    public bool IsCharacterWindowAccessAllowed => (Game.Current.GameType.IsCharacterWindowAccessibleAtMission || Mission.Current != null && Mission.Current.Mode != MissionMode.Stealth && (Mission.Current.Mode != MissionMode.Battle && Mission.Current.Mode != MissionMode.Deployment) && Mission.Current.Mode != MissionMode.Duel) && this.IsCharacterWindowAccessible;

    public bool IsPartyWindowAccessible { private get; set; }

    public bool IsPartyWindowAccessAllowed => Game.Current.GameType.IsPartyWindowAccessibleAtMission && Mission.Current != null && (Mission.Current.Mode != MissionMode.StartUp && Mission.Current.Mode != MissionMode.Stealth) && (Mission.Current.Mode != MissionMode.Battle && Mission.Current.Mode != MissionMode.Duel && (Mission.Current.Mode != MissionMode.Deployment && Mission.Current.Mode != MissionMode.CutScene)) && this.IsPartyWindowAccessible;

    public bool IsKingdomWindowAccessible { private get; set; }

    public bool IsKingdomWindowAccessAllowed => (Game.Current.GameType.IsKingdomWindowAccessibleAtMission || Mission.Current != null && Mission.Current.Mode != MissionMode.StartUp && (Mission.Current.Mode != MissionMode.Stealth && Mission.Current.Mode != MissionMode.Battle) && (Mission.Current.Mode != MissionMode.Deployment && Mission.Current.Mode != MissionMode.Duel && Mission.Current.Mode != MissionMode.CutScene)) && this.IsKingdomWindowAccessible;

    public bool IsClanWindowAccessible { private get; set; }

    public bool IsClanWindowAccessAllowed => (Game.Current.GameType.IsClanWindowAccessibleAtMission || Mission.Current != null && Mission.Current.Mode != MissionMode.StartUp && (Mission.Current.Mode != MissionMode.Stealth && Mission.Current.Mode != MissionMode.Battle) && (Mission.Current.Mode != MissionMode.Deployment && Mission.Current.Mode != MissionMode.Duel)) && this.IsClanWindowAccessible;

    public bool IsEncyclopediaWindowAccessible { private get; set; }

    public bool IsEncyclopediaWindowAccessAllowed => (Game.Current.GameType.IsEncyclopediaWindowAccessibleAtMission || Mission.Current != null && Mission.Current.Mode != MissionMode.Battle && (Mission.Current.Mode != MissionMode.Deployment && Mission.Current.Mode != MissionMode.Duel)) && this.IsEncyclopediaWindowAccessible;

    public bool IsBannerWindowAccessible { private get; set; }

    public bool IsBannerWindowAccessAllowed => (Game.Current.GameType.IsBannerWindowAccessibleAtMission || Mission.Current != null && Mission.Current.Mode != MissionMode.Battle && (Mission.Current.Mode != MissionMode.Deployment && Mission.Current.Mode != MissionMode.Duel) && Mission.Current.Mode != MissionMode.CutScene) && this.IsBannerWindowAccessible;

    public bool DoesMissionRequireCivilianEquipment { get; set; }

    public Mission.MissionTeamAITypeEnum MissionTeamAIType { get; set; }

    internal MissionTimeTracker MissionTimeTracker { get; private set; }

    private Lazy<MissionRecorder> _recorder => new Lazy<MissionRecorder>((Func<MissionRecorder>) (() => new MissionRecorder(this)));

    public MissionRecorder Recorder => this._recorder.Value;

    public Mission(MissionInitializerRecord rec, MissionState missionState)
    {
      this._missionPointer = MBAPI.IMBMission.CreateMission(this);
      this._missionObjects = new List<MissionObject>();
      this._activeMissionObjects = new List<MissionObject>();
      this._addedEntitiesInfo = new List<Mission.DynamicallyCreatedEntity>();
      this._emptyRuntimeMissionObjectIds = new Stack<(int, float)>();
      this.Boundaries = new Mission.MBBoundaryCollection(this);
      this.InitializerRecord = rec;
      this._usedSpawnPathIndex = -1;
      this.CurrentState = Mission.State.NewlyCreated;
      this.IsInventoryAccessible = false;
      this.IsQuestScreenAccessible = true;
      this.IsCharacterWindowAccessible = true;
      this.IsPartyWindowAccessible = true;
      this.IsKingdomWindowAccessible = true;
      this.IsClanWindowAccessible = true;
      this.IsBannerWindowAccessible = false;
      this.IsEncyclopediaWindowAccessible = true;
      this._missiles = new Dictionary<int, Mission.Missile>();
      this._activeAgents = new List<Agent>(256);
      this._allAgents = new List<Agent>(256);
      for (int index = 0; index < 3; ++index)
        this._fleePositions[index] = new List<FleePosition>(32);
      this._missionBehaviourList = new List<MissionBehaviour>();
      this._missionLogics = new List<MissionLogic>();
      this._otherMissionBehaviours = new List<MissionBehaviour>();
      this._missionState = missionState;
      this.Teams = new Mission.TeamCollection(this);
      this._currentDeploymentPlan = new MissionDeploymentPlan(this);
      this.MissionTimeTracker = new MissionTimeTracker();
    }

    private void FreeResources()
    {
      this.MainAgent = (Agent) null;
      this.Teams.ClearResources();
      this.SpectatorTeam = (Team) null;
      this._activeAgents = (List<Agent>) null;
      this._allAgents = (List<Agent>) null;
      if (GameNetwork.NetworkPeersValid)
      {
        foreach (NetworkCommunicator networkPeer in GameNetwork.NetworkPeers)
        {
          MissionPeer component = networkPeer.GetComponent<MissionPeer>();
          if (component != null)
          {
            component.ClearAllVisuals(true);
            networkPeer.RemoveComponent((PeerComponent) component);
          }
        }
      }
      this._missionState = (MissionState) null;
    }

    public void RetreatMission()
    {
      foreach (MissionLogic missionLogic in this.MissionLogics)
        missionLogic.OnRetreatMission();
      this.EndMission();
    }

    public int CountTeamPeers(Team team) => VirtualPlayer.Peers<MissionPeer>().Count<MissionPeer>((Func<MissionPeer, bool>) (p => p.Team == team));

    public void BalanceTeams()
    {
      if (MultiplayerOptions.OptionType.AutoTeamBalanceThreshold.GetIntValue() == 0)
        return;
      int num1 = this.CountTeamPeers(this.AttackerTeam);
      int num2;
      for (num2 = this.CountTeamPeers(this.DefenderTeam); num1 > num2 + MultiplayerTeamSelectComponent.GetAutoTeamBalanceDifference((AutoTeamBalanceLimits) MultiplayerOptions.OptionType.AutoTeamBalanceThreshold.GetIntValue()); ++num2)
      {
        VirtualPlayer.Peers<MissionPeer>().Where<MissionPeer>((Func<MissionPeer, bool>) (q => q.GetNetworkPeer().IsSynchronized && q.Team == this.AttackerTeam)).MaxBy<MissionPeer, DateTime>((Func<MissionPeer, DateTime>) (q => q.JoinTime)).Team = this.DefenderTeam;
        --num1;
      }
      for (; num2 > num1 + MultiplayerTeamSelectComponent.GetAutoTeamBalanceDifference((AutoTeamBalanceLimits) MultiplayerOptions.OptionType.AutoTeamBalanceThreshold.GetIntValue()); --num2)
      {
        VirtualPlayer.Peers<MissionPeer>().Where<MissionPeer>((Func<MissionPeer, bool>) (q => q.GetNetworkPeer().IsSynchronized && q.Team == this.DefenderTeam)).MaxBy<MissionPeer, DateTime>((Func<MissionPeer, DateTime>) (q => q.JoinTime)).Team = this.AttackerTeam;
        ++num1;
      }
    }

    public bool HasMissionBehaviour<T>() where T : MissionBehaviour => (object) this.GetMissionBehaviour<T>() != null;

    [UsedImplicitly]
    [MBCallback]
    internal void OnAgentAddedAsCorpse(Agent affectedAgent)
    {
      if (GameNetwork.IsClientOrReplay)
        return;
      for (int index = 0; index < affectedAgent.GetAttachedWeaponsCount(); ++index)
      {
        if (affectedAgent.GetAttachedWeapon(index).Item.ItemFlags.HasAnyFlag<ItemFlags>(ItemFlags.CanBePickedUpFromCorpse))
          this.SpawnAttachedWeaponOnCorpse(affectedAgent, index, -1);
      }
      affectedAgent.ClearAttachedWeapons();
    }

    internal void SpawnAttachedWeaponOnCorpse(
      Agent agent,
      int attachedWeaponIndex,
      int forcedSpawnIndex)
    {
      agent.AgentVisuals.GetSkeleton().ForceUpdateBoneFrames();
      MissionWeapon attachedWeapon = agent.GetAttachedWeapon(attachedWeaponIndex);
      GameEntity attachedWeaponEntity = agent.AgentVisuals.GetAttachedWeaponEntity(attachedWeaponIndex);
      attachedWeaponEntity.CreateAndAddScriptComponent(typeof (SpawnedItemEntity).Name);
      SpawnedItemEntity firstScriptOfType = attachedWeaponEntity.GetFirstScriptOfType<SpawnedItemEntity>();
      if (forcedSpawnIndex >= 0)
        firstScriptOfType.Id = new MissionObjectId(forcedSpawnIndex, true);
      if (GameNetwork.IsServerOrRecorder)
      {
        GameNetwork.BeginBroadcastModuleEvent();
        GameNetwork.WriteMessage((GameNetworkMessage) new NetworkMessages.FromServer.SpawnAttachedWeaponOnCorpse(agent, attachedWeaponIndex, firstScriptOfType.Id.Id));
        GameNetwork.EndBroadcastModuleEvent(GameNetwork.EventBroadcastFlags.AddToMissionRecord);
      }
      this.SpawnWeaponAux(attachedWeaponEntity, attachedWeapon, Mission.WeaponSpawnFlags.AsMissile | Mission.WeaponSpawnFlags.WithStaticPhysics, Vec3.Zero, Vec3.Zero, false);
    }

    [UsedImplicitly]
    [MBCallback]
    internal void OnAgentDeleted(Agent affectedAgent)
    {
      affectedAgent.State = AgentState.Deleted;
      foreach (MissionBehaviour missionBehaviour in this.MissionBehaviours)
        missionBehaviour.OnAgentDeleted(affectedAgent);
      this._allAgents.Remove(affectedAgent);
      affectedAgent.OnDelete();
      affectedAgent.SetTeam((Team) null, false);
    }

    [UsedImplicitly]
    [MBCallback]
    internal void OnAgentRemoved(
      Agent affectedAgent,
      Agent affectorAgent,
      AgentState agentState,
      KillingBlow killingBlow)
    {
      Mission.OnBeforeAgentRemovedDelegate beforeAgentRemoved = this.OnBeforeAgentRemoved;
      if (beforeAgentRemoved != null)
        beforeAgentRemoved(affectedAgent, affectorAgent, agentState, killingBlow);
      affectedAgent.State = agentState;
      if (affectorAgent != null && affectorAgent.Team != affectedAgent.Team)
        ++affectorAgent.KillCount;
      affectedAgent.Team?.DeactivateAgent(affectedAgent);
      foreach (MissionBehaviour missionBehaviour in this.MissionBehaviours)
        missionBehaviour.OnEarlyAgentRemoved(affectedAgent, affectorAgent, agentState, killingBlow);
      foreach (MissionBehaviour missionBehaviour in this.MissionBehaviours)
        missionBehaviour.OnAgentRemoved(affectedAgent, affectorAgent, agentState, killingBlow);
      int num = this.MainAgent == affectedAgent ? 1 : 0;
      if (num != 0)
      {
        affectedAgent.OnMainAgentWieldedItemChange = (Agent.OnMainAgentWieldedItemChangeDelegate) null;
        this.MainAgent = (Agent) null;
      }
      affectedAgent.OnAgentWieldedItemChange = (Action) null;
      affectedAgent.OnAgentMountedStateChanged = (Action) null;
      this._activeAgents.Remove(affectedAgent);
      affectedAgent.OnRemove();
      if (num == 0)
        return;
      affectedAgent.Team.DelegateCommandToAI();
    }

    internal void OnObjectDisabled(DestructableComponent destructionComponent)
    {
      destructionComponent.GameEntity.GetFirstScriptOfType<UsableMachine>()?.Disable();
      foreach (MissionBehaviour missionBehaviour in this.MissionBehaviours)
        missionBehaviour.OnObjectDisabled(destructionComponent);
    }

    [UsedImplicitly]
    [MBCallback]
    internal float OnAgentHit(
      Agent affectedAgent,
      Agent affectorAgent,
      int affectorWeaponSlotOrMissileIndex,
      bool isMissile,
      bool isBlocked,
      int damage,
      float movementSpeedDamageModifier,
      float hitDistance,
      AgentAttackType attackType,
      BoneBodyPartType victimHitBodyPart)
    {
      float shotDifficulty = -1f;
      MissionWeapon affectorWeapon = !isMissile ? (affectorAgent == null || affectorWeaponSlotOrMissileIndex < 0 ? MissionWeapon.Invalid : affectorAgent.Equipment[affectorWeaponSlotOrMissileIndex]) : this._missiles[affectorWeaponSlotOrMissileIndex].Weapon;
      if (affectorAgent != null & isMissile)
        shotDifficulty = this.GetShootDifficulty(affectedAgent, affectorAgent, victimHitBodyPart == BoneBodyPartType.Head);
      foreach (MissionBehaviour missionBehaviour in this.MissionBehaviours)
      {
        missionBehaviour.OnAgentHit(affectedAgent, affectorAgent, damage, in affectorWeapon);
        missionBehaviour.OnScoreHit(affectedAgent, affectorAgent, affectorWeapon.CurrentUsageItem, isBlocked, (float) damage, movementSpeedDamageModifier, hitDistance, attackType, shotDifficulty, victimHitBodyPart);
      }
      foreach (AgentComponent component in affectedAgent.Components)
        component.OnHit(affectorAgent, damage, in affectorWeapon);
      affectedAgent.CheckToDropFlaggedItem();
      return (float) damage;
    }

    internal MissionObjectId SpawnWeaponAsDropFromMissile(
      int missileIndex,
      MissionObject attachedMissionObject,
      ref MatrixFrame attachLocalFrame,
      Mission.WeaponSpawnFlags spawnFlags,
      ref Vec3 velocity,
      ref Vec3 angularVelocity,
      int forcedSpawnIndex)
    {
      this.PrepareMissileWeaponForDrop(missileIndex);
      Mission.Missile missile = this._missiles[missileIndex];
      attachedMissionObject?.AddStuckMissile(missile.Entity);
      if (attachedMissionObject != null)
        missile.Entity.SetGlobalFrame(attachedMissionObject.GameEntity.GetGlobalFrame().TransformToParent(attachLocalFrame));
      else
        missile.Entity.SetGlobalFrame(attachLocalFrame);
      missile.Entity.CreateAndAddScriptComponent(typeof (SpawnedItemEntity).Name);
      SpawnedItemEntity firstScriptOfType = missile.Entity.GetFirstScriptOfType<SpawnedItemEntity>();
      if (forcedSpawnIndex >= 0)
        firstScriptOfType.Id = new MissionObjectId(forcedSpawnIndex, true);
      this.SpawnWeaponAux(missile.Entity, missile.Weapon, spawnFlags, velocity, angularVelocity, true);
      return firstScriptOfType.Id;
    }

    [UsedImplicitly]
    [MBCallback]
    internal void SpawnWeaponAsDropFromAgent(
      Agent agent,
      EquipmentIndex equipmentIndex,
      ref Vec3 velocity,
      ref Vec3 angularVelocity,
      Mission.WeaponSpawnFlags spawnFlags)
    {
      Vec3 velocity1 = agent.Velocity;
      if ((double) (velocity - velocity1).LengthSquared > 100.0)
      {
        Vec3 vec3 = (velocity - velocity1).NormalizedCopy() * 10f;
        velocity = velocity1 + vec3;
      }
      this.SpawnWeaponAsDropFromAgentAux(agent, equipmentIndex, ref velocity, ref angularVelocity, spawnFlags, -1);
    }

    internal void SpawnWeaponAsDropFromAgentAux(
      Agent agent,
      EquipmentIndex equipmentIndex,
      ref Vec3 velocity,
      ref Vec3 angularVelocity,
      Mission.WeaponSpawnFlags spawnFlags,
      int forcedSpawnIndex)
    {
      agent.AgentVisuals.GetSkeleton().ForceUpdateBoneFrames();
      agent.PrepareWeaponForDropInEquipmentSlot(equipmentIndex, (uint) (spawnFlags & Mission.WeaponSpawnFlags.WithHolster) > 0U);
      GameEntity fromEquipmentSlot = agent.GetWeaponEntityFromEquipmentSlot(equipmentIndex);
      fromEquipmentSlot.CreateAndAddScriptComponent(typeof (SpawnedItemEntity).Name);
      SpawnedItemEntity firstScriptOfType = fromEquipmentSlot.GetFirstScriptOfType<SpawnedItemEntity>();
      if (forcedSpawnIndex >= 0)
        firstScriptOfType.Id = new MissionObjectId(forcedSpawnIndex, true);
      if (GameNetwork.IsServerOrRecorder)
      {
        GameNetwork.BeginBroadcastModuleEvent();
        GameNetwork.WriteMessage((GameNetworkMessage) new NetworkMessages.FromServer.SpawnWeaponAsDropFromAgent(agent, equipmentIndex, velocity, angularVelocity, spawnFlags, firstScriptOfType.Id.Id));
        GameNetwork.EndBroadcastModuleEvent(GameNetwork.EventBroadcastFlags.AddToMissionRecord);
      }
      this.SpawnWeaponAux(fromEquipmentSlot, agent.Equipment[equipmentIndex], spawnFlags, velocity, angularVelocity, true);
      if (!GameNetwork.IsClientOrReplay)
      {
        int attachmentIndex = 0;
        while (true)
        {
          int num = attachmentIndex;
          MissionWeapon attachedWeapon = agent.Equipment[equipmentIndex];
          int attachedWeaponsCount = attachedWeapon.GetAttachedWeaponsCount();
          if (num < attachedWeaponsCount)
          {
            attachedWeapon = agent.Equipment[equipmentIndex];
            attachedWeapon = attachedWeapon.GetAttachedWeapon(attachmentIndex);
            if (attachedWeapon.Item.ItemFlags.HasAnyFlag<ItemFlags>(ItemFlags.CanBePickedUpFromCorpse))
              this.SpawnAttachedWeaponOnSpawnedWeapon(firstScriptOfType, attachmentIndex, -1);
            ++attachmentIndex;
          }
          else
            break;
        }
      }
      agent.OnWeaponDrop(equipmentIndex);
    }

    internal void SpawnAttachedWeaponOnSpawnedWeapon(
      SpawnedItemEntity spawnedWeapon,
      int attachmentIndex,
      int forcedSpawnIndex)
    {
      MBDebug.Print("SpawnAttachedWeaponOnSpawnedWeapon for " + ((NativeObject) spawnedWeapon.GameEntity != (NativeObject) null ? (object) spawnedWeapon.GameEntity.Name : (object) "null entity") + " child count: " + (object) ((NativeObject) spawnedWeapon.GameEntity != (NativeObject) null ? spawnedWeapon.GameEntity.ChildCount : 0) + " attachmentIndex: " + (object) attachmentIndex);
      GameEntity child = spawnedWeapon.GameEntity.GetChild(attachmentIndex);
      MBDebug.Print("SpawnAttachedWeaponOnSpawnedWeapon 2");
      child.CreateAndAddScriptComponent(typeof (SpawnedItemEntity).Name);
      MBDebug.Print("SpawnAttachedWeaponOnSpawnedWeapon 3");
      SpawnedItemEntity firstScriptOfType = child.GetFirstScriptOfType<SpawnedItemEntity>();
      if (forcedSpawnIndex >= 0)
        firstScriptOfType.Id = new MissionObjectId(forcedSpawnIndex, true);
      MBDebug.Print("SpawnAttachedWeaponOnSpawnedWeapon 4");
      this.SpawnWeaponAux(child, spawnedWeapon.WeaponCopy.GetAttachedWeapon(attachmentIndex), Mission.WeaponSpawnFlags.AsMissile | Mission.WeaponSpawnFlags.WithStaticPhysics, Vec3.Zero, Vec3.Zero, false);
      MBDebug.Print("SpawnAttachedWeaponOnSpawnedWeapon 5");
      if (!GameNetwork.IsServerOrRecorder)
        return;
      GameNetwork.BeginBroadcastModuleEvent();
      GameNetwork.WriteMessage((GameNetworkMessage) new NetworkMessages.FromServer.SpawnAttachedWeaponOnSpawnedWeapon(spawnedWeapon, attachmentIndex, firstScriptOfType.Id.Id));
      GameNetwork.EndBroadcastModuleEvent(GameNetwork.EventBroadcastFlags.AddToMissionRecord);
    }

    public GameEntity SpawnWeaponWithNewEntity(
      ref MissionWeapon weapon,
      Mission.WeaponSpawnFlags spawnFlags,
      MatrixFrame frame)
    {
      return this.SpawnWeaponWithNewEntityAux(weapon, spawnFlags, frame, -1, (MissionObject) null, false);
    }

    public GameEntity SpawnWeaponWithNewEntityAux(
      MissionWeapon weapon,
      Mission.WeaponSpawnFlags spawnFlags,
      MatrixFrame frame,
      int forcedSpawnIndex,
      MissionObject attachedMissionObject,
      bool hasLifeTime)
    {
      GameEntity gameEntity = GameEntityExtensions.Instantiate(this.Scene, weapon, spawnFlags.HasAnyFlag<Mission.WeaponSpawnFlags>(Mission.WeaponSpawnFlags.WithHolster), true);
      gameEntity.CreateAndAddScriptComponent(typeof (SpawnedItemEntity).Name);
      SpawnedItemEntity firstScriptOfType = gameEntity.GetFirstScriptOfType<SpawnedItemEntity>();
      if (forcedSpawnIndex >= 0)
        firstScriptOfType.Id = new MissionObjectId(forcedSpawnIndex, true);
      attachedMissionObject?.GameEntity.AddChild(gameEntity);
      if (attachedMissionObject != null)
        gameEntity.SetGlobalFrame(attachedMissionObject.GameEntity.GetGlobalFrame().TransformToParent(frame));
      else
        gameEntity.SetGlobalFrame(frame);
      if (GameNetwork.IsServerOrRecorder)
      {
        GameNetwork.BeginBroadcastModuleEvent();
        GameNetwork.WriteMessage((GameNetworkMessage) new NetworkMessages.FromServer.SpawnWeaponWithNewEntity(weapon, spawnFlags, firstScriptOfType.Id.Id, frame, attachedMissionObject, true, hasLifeTime));
        GameNetwork.EndBroadcastModuleEvent(GameNetwork.EventBroadcastFlags.AddToMissionRecord);
        for (int attachmentIndex = 0; attachmentIndex < weapon.GetAttachedWeaponsCount(); ++attachmentIndex)
        {
          GameNetwork.BeginBroadcastModuleEvent();
          GameNetwork.WriteMessage((GameNetworkMessage) new AttachWeaponToSpawnedWeapon(weapon.GetAttachedWeapon(attachmentIndex), (MissionObject) firstScriptOfType, weapon.GetAttachedWeaponFrame(attachmentIndex)));
          GameNetwork.EndBroadcastModuleEvent(GameNetwork.EventBroadcastFlags.AddToMissionRecord);
        }
      }
      Vec3 zero = Vec3.Zero;
      this.SpawnWeaponAux(gameEntity, weapon, spawnFlags, zero, zero, hasLifeTime);
      return gameEntity;
    }

    public void AttachWeaponWithNewEntityToSpawnedWeapon(
      MissionWeapon weapon,
      SpawnedItemEntity spawnedItem,
      MatrixFrame attachLocalFrame)
    {
      GameEntity mbGameEntity = GameEntityExtensions.Instantiate(this.Scene, weapon, false, true);
      spawnedItem.GameEntity.AddChild(mbGameEntity);
      mbGameEntity.SetFrame(ref attachLocalFrame);
      spawnedItem.AttachWeaponToWeapon(weapon, ref attachLocalFrame);
    }

    private void SpawnWeaponAux(
      GameEntity weaponEntity,
      MissionWeapon weapon,
      Mission.WeaponSpawnFlags spawnFlags,
      Vec3 velocity,
      Vec3 angularVelocity,
      bool hasLifeTime)
    {
      weaponEntity.GetFirstScriptOfType<SpawnedItemEntity>().Initialize(weapon, hasLifeTime, spawnFlags);
      if (!spawnFlags.HasAnyFlag<Mission.WeaponSpawnFlags>(Mission.WeaponSpawnFlags.WithPhysics | Mission.WeaponSpawnFlags.WithStaticPhysics))
        return;
      BodyFlags bodyFlags = BodyFlags.OnlyCollideWithRaycast | BodyFlags.DroppedItem;
      if (weapon.Item.ItemFlags.HasAnyFlag<ItemFlags>(ItemFlags.CannotBePickedUp) || spawnFlags.HasAnyFlag<Mission.WeaponSpawnFlags>(Mission.WeaponSpawnFlags.CannotBePickedUp))
        bodyFlags |= BodyFlags.DoNotCollideWithRaycast;
      weaponEntity.BodyFlag |= bodyFlags;
      WeaponData weaponData = weapon.GetWeaponData(true);
      this.RecalculateBody(ref weaponData, weapon.Item.ItemComponent, weapon.Item.WeaponDesign, ref spawnFlags);
      weaponEntity.AddPhysics(weaponData.BaseWeight, weaponData.CenterOfMassShift, weaponData.Shape, velocity, angularVelocity, PhysicsMaterial.GetFromIndex(weaponData.PhysicsMaterialIndex), spawnFlags.HasAnyFlag<Mission.WeaponSpawnFlags>(Mission.WeaponSpawnFlags.WithStaticPhysics), 0);
      weaponData.DeinitializeManagedPointers();
    }

    internal void OnEquipItemsFromSpawnEquipmentBegin(Agent agent, Agent.CreationType creationType)
    {
      foreach (IMissionListener listener in this._listeners)
        listener.OnEquipItemsFromSpawnEquipmentBegin(agent, creationType);
    }

    internal void OnEquipItemsFromSpawnEquipment(Agent agent, Agent.CreationType creationType)
    {
      foreach (IMissionListener listener in this._listeners)
        listener.OnEquipItemsFromSpawnEquipment(agent, creationType);
    }

    [CommandLineFunctionality.CommandLineArgumentFunction("flee_enemies", "mission")]
    public static string MakeEnemiesFleeCheat(List<string> strings)
    {
      if (GameNetwork.IsClientOrReplay)
        return "is client";
      if (Mission.Current == null || Mission.Current.Agents == null)
        return "mission not available";
      foreach (Agent agent in Mission.Current.Agents.Where<Agent>((Func<Agent, bool>) (agent => agent.IsHuman && agent.IsEnemyOf(Agent.Main))))
        agent.GetComponent<MoraleAgentComponent>()?.Panic();
      return "enemies are fleeing";
    }

    public void RecalculateBody(
      ref WeaponData weaponData,
      ItemComponent itemComponent,
      WeaponDesign craftedWeaponData,
      ref Mission.WeaponSpawnFlags spawnFlags)
    {
      WeaponComponent weaponComponent = (WeaponComponent) itemComponent;
      ItemObject itemObject = weaponComponent.Item;
      weaponData.Shape = !spawnFlags.HasAnyFlag<Mission.WeaponSpawnFlags>(Mission.WeaponSpawnFlags.WithHolster) ? (string.IsNullOrEmpty(itemObject.BodyName) ? (PhysicsShape) null : PhysicsShape.GetFromResource(itemObject.BodyName)) : (string.IsNullOrEmpty(itemObject.HolsterBodyName) ? (PhysicsShape) null : PhysicsShape.GetFromResource(itemObject.HolsterBodyName));
      PhysicsShape physicsShape = weaponData.Shape;
      if ((NativeObject) physicsShape == (NativeObject) null)
        physicsShape = PhysicsShape.GetFromResource("bo_axe_short");
      if (!weaponComponent.Item.ItemFlags.HasAnyFlag<ItemFlags>(ItemFlags.DoNotScaleBodyAccordingToWeaponLength))
      {
        if (spawnFlags.HasAnyFlag<Mission.WeaponSpawnFlags>(Mission.WeaponSpawnFlags.WithHolster) || !itemObject.RecalculateBody)
        {
          weaponData.Shape = physicsShape;
        }
        else
        {
          PhysicsShape copy = physicsShape.CreateCopy();
          weaponData.Shape = copy;
          float num1 = (float) weaponComponent.PrimaryWeapon.WeaponLength * 0.01f;
          int num2 = physicsShape.CapsuleCount();
          if (num2 == 0)
            return;
          if (craftedWeaponData != (WeaponDesign) null)
          {
            copy.Clear();
            float num3 = 0.0f;
            float num4 = 0.0f;
            float z1 = 0.0f;
            for (int index = 0; index < craftedWeaponData.UsedPieces.Length; ++index)
            {
              WeaponDesignElement usedPiece = craftedWeaponData.UsedPieces[index];
              if (usedPiece.IsValid)
              {
                craftedWeaponData.Template.BuildOrders.First<PieceData>((Func<PieceData, bool>) (bo => bo.PieceType == usedPiece.CraftingPiece.PieceType));
                float scaledPieceOffset = usedPiece.ScaledPieceOffset;
                double piecePivotDistance = (double) craftedWeaponData.PiecePivotDistances[index];
                float val1 = (float) piecePivotDistance + scaledPieceOffset - usedPiece.ScaledDistanceToPreviousPiece;
                float num5 = (float) piecePivotDistance - scaledPieceOffset + usedPiece.ScaledDistanceToNextPiece;
                num3 = Math.Min(val1, num3);
                if ((double) num5 > (double) num4)
                {
                  num4 = num5;
                  z1 = (float) (((double) num5 + (double) val1) * 0.5);
                }
              }
            }
            WeaponDesignElement usedPiece1 = craftedWeaponData.UsedPieces[2];
            if (usedPiece1.IsValid)
            {
              float scaledPieceOffset = usedPiece1.ScaledPieceOffset;
              num3 -= scaledPieceOffset;
            }
            copy.AddCapsule(new CapsuleData(0.035f, new Vec3(z: craftedWeaponData.CraftedWeaponLength), new Vec3(z: num3)));
            bool flag = false;
            if (craftedWeaponData.UsedPieces[1].IsValid)
            {
              float piecePivotDistance = craftedWeaponData.PiecePivotDistances[1];
              copy.AddCapsule(new CapsuleData(0.05f, new Vec3(-0.1f, z: piecePivotDistance), new Vec3(0.1f, z: piecePivotDistance)));
              flag = true;
            }
            if (weaponComponent.PrimaryWeapon.WeaponClass == WeaponClass.OneHandedAxe || weaponComponent.PrimaryWeapon.WeaponClass == WeaponClass.TwoHandedAxe || weaponComponent.PrimaryWeapon.WeaponClass == WeaponClass.ThrowingAxe)
            {
              WeaponDesignElement usedPiece2 = craftedWeaponData.UsedPieces[0];
              double piecePivotDistance = (double) craftedWeaponData.PiecePivotDistances[0];
              float z2 = (float) (piecePivotDistance + (double) usedPiece2.CraftingPiece.Length * 0.800000011920929);
              float z3 = (float) (piecePivotDistance - (double) usedPiece2.CraftingPiece.Length * 0.800000011920929);
              float z4 = (float) piecePivotDistance + usedPiece2.CraftingPiece.Length;
              float z5 = (float) piecePivotDistance - usedPiece2.CraftingPiece.Length;
              float bladeWidth = usedPiece2.CraftingPiece.BladeData.BladeWidth;
              copy.AddCapsule(new CapsuleData(0.05f, new Vec3(z: z2), new Vec3(-bladeWidth, z: z4)));
              copy.AddCapsule(new CapsuleData(0.05f, new Vec3(z: z3), new Vec3(-bladeWidth, z: z5)));
              copy.AddCapsule(new CapsuleData(0.05f, new Vec3(-bladeWidth, z: z4), new Vec3(-bladeWidth, z: z5)));
              flag = true;
            }
            if (weaponComponent.PrimaryWeapon.WeaponClass == WeaponClass.TwoHandedPolearm || weaponComponent.PrimaryWeapon.WeaponClass == WeaponClass.Javelin)
            {
              float piecePivotDistance = craftedWeaponData.PiecePivotDistances[0];
              copy.AddCapsule(new CapsuleData(0.025f, new Vec3(-0.05f, z: piecePivotDistance), new Vec3(0.05f, z: piecePivotDistance)));
              flag = true;
            }
            if (!flag)
              copy.AddCapsule(new CapsuleData(0.025f, new Vec3(-0.05f, z: z1), new Vec3(0.05f, z: z1)));
            copy.CapsuleCount();
          }
          else
          {
            switch (weaponComponent.PrimaryWeapon.WeaponClass)
            {
              case WeaponClass.Dagger:
              case WeaponClass.OneHandedSword:
              case WeaponClass.TwoHandedSword:
              case WeaponClass.ThrowingKnife:
                CapsuleData data1 = new CapsuleData();
                copy.GetCapsule(ref data1, 0);
                float radius1 = data1.Radius;
                Vec3 p1_1 = data1.P1;
                Vec3 p2_1 = data1.P2;
                copy.SetCapsule(new CapsuleData(radius1, new Vec3(p1_1.x, p1_1.y, p1_1.z * num1), p2_1), 0);
                break;
              case WeaponClass.OneHandedAxe:
              case WeaponClass.TwoHandedAxe:
              case WeaponClass.Mace:
              case WeaponClass.TwoHandedMace:
              case WeaponClass.OneHandedPolearm:
              case WeaponClass.TwoHandedPolearm:
              case WeaponClass.LowGripPolearm:
              case WeaponClass.Arrow:
              case WeaponClass.Bolt:
              case WeaponClass.Crossbow:
              case WeaponClass.ThrowingAxe:
              case WeaponClass.Javelin:
              case WeaponClass.Banner:
                CapsuleData data2 = new CapsuleData();
                copy.GetCapsule(ref data2, 0);
                float radius2 = data2.Radius;
                Vec3 p1_2 = data2.P1;
                Vec3 p2_2 = data2.P2;
                copy.SetCapsule(new CapsuleData(radius2, new Vec3(p1_2.x, p1_2.y, p1_2.z * num1), p2_2), 0);
                for (int index = 1; index < num2; ++index)
                {
                  CapsuleData data3 = new CapsuleData();
                  copy.GetCapsule(ref data3, index);
                  float radius3 = data3.Radius;
                  Vec3 p1_3 = data3.P1;
                  Vec3 p2_3 = data3.P2;
                  copy.SetCapsule(new CapsuleData(radius3, new Vec3(p1_3.x, p1_3.y, p1_3.z * num1), new Vec3(p2_3.x, p2_3.y, p2_3.z * num1)), index);
                }
                break;
            }
          }
        }
      }
      weaponData.CenterOfMassShift = weaponData.Shape.GetWeaponCenterOfMass();
    }

    [UsedImplicitly]
    [MBCallback]
    internal void OnPreTick(float dt)
    {
      for (int index = this._missionBehaviourList.Count - 1; index >= 0; --index)
        this._missionBehaviourList[index].OnPreMissionTick(dt);
      this.TickDebugAgents();
    }

    [UsedImplicitly]
    [MBCallback]
    internal void ApplySkeletonScaleToAllEquippedItems(string itemName)
    {
      int count = this.Agents.Count;
      for (int index1 = 0; index1 < count; ++index1)
      {
        for (int index2 = 0; index2 < 12; ++index2)
        {
          EquipmentElement equipmentElement = this.Agents[index1].SpawnEquipment[index2];
          if (!equipmentElement.IsEmpty && equipmentElement.Item.StringId == itemName && equipmentElement.Item.HorseComponent?.SkeletonScale != null)
          {
            this.Agents[index1].AgentVisuals.ApplySkeletonScale(equipmentElement.Item.HorseComponent.SkeletonScale.MountSitBoneScale, equipmentElement.Item.HorseComponent.SkeletonScale.MountRadiusAdder, equipmentElement.Item.HorseComponent.SkeletonScale.BoneIndices, equipmentElement.Item.HorseComponent.SkeletonScale.Scales);
            break;
          }
        }
      }
    }

    [CommandLineFunctionality.CommandLineArgumentFunction("set_facial_anim_to_agent", "mission")]
    public static string SetFacialAnimToAgent(List<string> strings)
    {
      Mission current = Mission.Current;
      if (current == null)
        return "Mission could not be found";
      if (strings.Count != 2)
        return "Enter agent index and animation name please";
      int result;
      if (int.TryParse(strings[0], out result) && result >= 0)
      {
        foreach (Agent agent in (IEnumerable<Agent>) current.Agents)
        {
          if (agent.Index == result)
          {
            agent.SetAgentFacialAnimation(Agent.FacialAnimChannel.High, strings[1], true);
            return "Done";
          }
        }
      }
      return "Please enter a valid agent index";
    }

    [CommandLineFunctionality.CommandLineArgumentFunction("formation_speed_adjustment_enabled", "ai")]
    public static string EnableSpeedAdjustmentCommand(List<string> strings)
    {
      if (GameNetwork.IsSessionActive)
        return "Does not work on multiplayer.";
      FormationCohesionComponent.FormationSpeedAdjustmentEnabled = !FormationCohesionComponent.FormationSpeedAdjustmentEnabled;
      string str = "Speed Adjustment ";
      return !FormationCohesionComponent.FormationSpeedAdjustmentEnabled ? str + "disabled" : str + "enabled";
    }

    internal void OnTick(float dt, float realDt, bool updateCamera)
    {
      this.ApplyGeneratedCombatLogs();
      if (this.InputManager == null)
        this.InputManager = (IInputContext) new EmptyInputContext();
      this.MissionTimeTracker.Tick(dt);
      this.CheckMissionEnd(this.Time);
      if (this.IsFastForward && this.MissionEnded())
        this.IsFastForward = false;
      if (this.CurrentState == Mission.State.Continuing)
      {
        if (this._inMissionLoadingScreenTimer != null && this._inMissionLoadingScreenTimer.Check(this.Time))
        {
          this._inMissionLoadingScreenTimer = (Timer) null;
          if (this._onLoadingEndedAction != null)
            this._onLoadingEndedAction();
          LoadingWindow.DisableGlobalLoadingWindow();
        }
        for (int index = this._missionBehaviourList.Count - 1; index >= 0; --index)
          this._missionBehaviourList[index].OnPreDisplayMissionTick(dt);
        if (!GameNetwork.IsDedicatedServer & updateCamera)
          this._missionState.Handler.UpdateCamera(this, realDt);
        foreach (Agent allAgent in (IEnumerable<Agent>) this.AllAgents)
          allAgent.Tick(dt);
        foreach (Team team in (ReadOnlyCollection<Team>) this.Teams)
          team.Tick(dt);
        for (int index = this._missionBehaviourList.Count - 1; index >= 0; --index)
          this._missionBehaviourList[index].OnMissionTick(dt);
        this.HandleSpawnedItems();
      }
      for (int index = this._dynamicEntities.Count - 1; index >= 0; --index)
      {
        Mission.DynamicEntityInfo dynamicEntity = this._dynamicEntities[index];
        if (dynamicEntity.TimerToDisable.Check(MBCommon.GetTime(MBCommon.TimeType.Mission)))
        {
          dynamicEntity.Entity.RemoveEnginePhysics();
          dynamicEntity.Entity.Remove(79);
          this._dynamicEntities.RemoveAt(index);
        }
      }
      DebugNetworkEventStatistics.EndTick(dt);
      if (this.CurrentState != Mission.State.Continuing || !this.IsFriendlyMission)
        return;
      if (this.InputManager.IsGameKeyDown(4))
        this.OnEndMissionRequest();
      else
        this._leaveMissionTimer = (BasicTimer) null;
    }

    public void RemoveSpawnedItemsAndMissiles()
    {
      this.ClearMissiles();
      this._missiles.Clear();
      this.RemoveSpawnedMissionObjects();
    }

    internal void AfterStart()
    {
      this._activeAgents.Clear();
      this._allAgents.Clear();
      Utilities.EnableGlobalEditDataCacher();
      this.FindSpawnPath();
      for (int index = 0; index < this._missionBehaviourList.Count; ++index)
        this._missionBehaviourList[index].OnBehaviourInitialize();
      foreach (MBSubModuleBase subModule in Module.CurrentModule.SubModules)
        subModule.OnMissionBehaviourInitialize(this);
      foreach (MissionBehaviour missionBehaviour in this.MissionBehaviours)
        missionBehaviour.EarlyStart();
      foreach (MissionBehaviour missionBehaviour in this.MissionBehaviours)
        missionBehaviour.AfterStart();
      foreach (MissionObject missionObject in (IEnumerable<MissionObject>) this.MissionObjects)
        missionObject.AfterMissionStart();
      if (MissionGameModels.Current.ApplyWeatherEffectsModel != null)
        MissionGameModels.Current.ApplyWeatherEffectsModel.ApplyWeatherEffects();
      this.CurrentState = Mission.State.Continuing;
      Utilities.DisableGlobalEditDataCacher();
    }

    private void ApplyGeneratedCombatLogs()
    {
      if (this._combatLogsCreated.IsEmpty)
        return;
      CombatLogData result;
      while (this._combatLogsCreated.TryDequeue(out result))
        CombatLogManager.GenerateCombatLog(result);
    }

    public int GetMemberCountOfSide(BattleSideEnum side) => this.Teams.Where<Team>((Func<Team, bool>) (t => t.Side == side)).Sum<Team>((Func<Team, int>) (t => t.ActiveAgents.Count<Agent>()));

    private void FindSpawnPath()
    {
      if (this._usedSpawnPathIndex < 0)
      {
        int num1 = 0;
        int num2;
        do
        {
          num2 = num1 <= 20 ? MBRandom.RandomInt(3) + 1 : (num1 - 20) % 3 + 1;
          this._usedSpawnPath = this.Scene.GetPathWithName("spawn_path_0" + (object) num2);
          ++num1;
        }
        while ((NativeObject) this._usedSpawnPath == (NativeObject) null && num1 < 24);
        this._usedSpawnPathIndex = num2;
        this._randomMiddlePointAddition = (float) ((double) MBRandom.RandomFloat * 0.259999990463257 - 0.129999995231628);
      }
      else
        this._usedSpawnPath = this.Scene.GetPathWithName("spawn_path_0" + (object) this._usedSpawnPathIndex);
    }

    public WorldFrame GetFormationSpawnFrame(
      BattleSideEnum side,
      FormationClass formationClass,
      bool isReinforcement)
    {
      return !this._currentDeploymentPlan.IsPlanMade ? this.GetSceneMiddleFrame() : this._currentDeploymentPlan.GetFormationPlan(side, formationClass).GetFrame(isReinforcement);
    }

    public WorldFrame GetSpawnPathFrame(
      BattleSideEnum side,
      int battleSize,
      float spawnPathOffset = 0.0f)
    {
      if ((NativeObject) this._usedSpawnPath == (NativeObject) null)
        return this.GetSceneMiddleFrame();
      Vec2 attackerPosition;
      Vec2 attackerDirection;
      Vec2 defenderPosition;
      Vec2 defenderDirection;
      MissionDeploymentPlan.GetSpawnPathPositionsForBattleSides(this._usedSpawnPath, this._randomMiddlePointAddition + spawnPathOffset, battleSize, out attackerPosition, out attackerDirection, out defenderPosition, out defenderDirection);
      Mat3 identity = Mat3.Identity;
      Vec3 vec3;
      if (side == BattleSideEnum.Attacker)
      {
        vec3 = attackerPosition.ToVec3();
        identity.RotateAboutUp(attackerDirection.RotationInRadians);
      }
      else
      {
        vec3 = defenderPosition.ToVec3();
        identity.RotateAboutUp(defenderDirection.RotationInRadians);
      }
      WorldPosition origin = new WorldPosition(this.Scene, UIntPtr.Zero, vec3, false);
      return new WorldFrame(identity, origin);
    }

    private void BuildAgent(Agent agent, AgentBuildData agentBuildData)
    {
      if (agent == null)
        throw new MBNullParameterException(nameof (agent));
      agent.Build(agentBuildData);
      if (!agent.SpawnEquipment[EquipmentIndex.ArmorItemEndSlot].IsEmpty)
      {
        EquipmentElement equipmentElement = agent.SpawnEquipment[EquipmentIndex.ArmorItemEndSlot];
        if (equipmentElement.Item.HorseComponent.BodyLength != 0)
          agent.SetInitialAgentScale(0.01f * (float) equipmentElement.Item.HorseComponent.BodyLength);
      }
      agent.EquipItemsFromSpawnEquipment();
      agent.AgentVisuals.BatchLastLodMeshes();
      agent.PreloadForRendering();
      ActionIndexCache currentAction = agent.GetCurrentAction(0);
      if (currentAction != ActionIndexCache.act_none)
        agent.SetActionChannel(0, currentAction, startProgress: (MBRandom.RandomFloat * 0.8f));
      agent.InitializeComponents();
      if (agent.Controller == Agent.ControllerType.Player)
        this.ResetFirstThirdPersonView();
      this._activeAgents.Add(agent);
      this._allAgents.Add(agent);
    }

    private Agent CreateAgent(
      Monster monster,
      bool isFemale,
      int instanceNo,
      Agent.CreationType creationType,
      float stepSize,
      int forcedAgentIndex,
      int weight,
      BasicCharacterObject characterObject)
    {
      AnimationSystemData animationSystemData = monster.FillAnimationSystemData(stepSize, false);
      AgentVisualsNativeData agentVisualsNativeData = monster.FillAgentVisualsNativeData();
      AgentCapsuleData capsuleData = monster.FillCapsuleData();
      AgentSpawnData spawnData = monster.FillSpawnData();
      Agent agent = new Agent(this, this.CreateAgentInternal(monster.Flags, forcedAgentIndex, isFemale, ref spawnData, ref capsuleData, ref agentVisualsNativeData, ref animationSystemData, instanceNo), creationType, monster);
      agent.AddComponent((AgentComponent) new HealthAgentComponent(agent));
      agent.Character = characterObject;
      foreach (MissionBehaviour missionBehaviour in this.MissionBehaviours)
        missionBehaviour.OnAgentCreated(agent);
      return agent;
    }

    public void SetBattleAgentCount(int agentCount)
    {
      if (this._agentCount != 0 && this._agentCount <= agentCount)
        return;
      this._agentCount = agentCount;
    }

    public Agent SpawnAgent(
      AgentBuildData agentBuildData,
      bool spawnFromAgentVisuals = false,
      int formationTroopCount = 0)
    {
      BasicCharacterObject agentCharacter = agentBuildData.AgentCharacter;
      if (agentCharacter == null)
        throw new MBNullParameterException("npcCharacterObject");
      int forcedAgentIndex = -1;
      if (agentBuildData.AgentIndexOverriden)
        forcedAgentIndex = agentBuildData.AgentIndex;
      Agent agent1 = this.CreateAgent(agentBuildData.AgentMonster, agentBuildData.GenderOverriden ? agentBuildData.AgentIsFemale : agentCharacter.IsFemale, 0, Agent.CreationType.FromCharacterObj, agentCharacter.GetStepSize(), forcedAgentIndex, agentBuildData.AgentMonster.Weight, agentCharacter);
      agent1.FormationPositionPreference = agentCharacter.FormationPositionPreference;
      float age = agentBuildData.AgeOverriden ? (float) agentBuildData.AgentAge : agentCharacter.Age;
      if ((double) age == 0.0)
        agentBuildData.Age(29);
      else if (MBBodyProperties.GetMaturityType(age) < BodyMeshMaturityType.Teenager && (this.Mode == MissionMode.Battle || this.Mode == MissionMode.Duel || (this.Mode == MissionMode.Tournament || this.Mode == MissionMode.Stealth)))
        agentBuildData.Age(27);
      if (agentBuildData.BodyPropertiesOverriden)
      {
        agent1.UpdateBodyProperties(agentBuildData.AgentBodyProperties);
        if (!agentBuildData.AgeOverriden)
          agent1.Age = agentCharacter.Age;
      }
      agent1.BodyPropertiesSeed = agentBuildData.AgentEquipmentSeed;
      if (agentBuildData.AgeOverriden)
        agent1.Age = (float) agentBuildData.AgentAge;
      if (agentBuildData.GenderOverriden)
        agent1.IsFemale = agentBuildData.AgentIsFemale;
      agent1.SetTeam(agentBuildData.AgentTeam, false);
      agent1.SetClothingColor1(agentBuildData.AgentClothingColor1);
      agent1.SetClothingColor2(agentBuildData.AgentClothingColor2);
      agent1.SetRandomizeColors(agentBuildData.RandomizeColors);
      agent1.Origin = agentBuildData.AgentOrigin;
      if (agentBuildData.AgentFormation != null)
      {
        Formation agentFormation = agentBuildData.AgentFormation;
        bool canSpawnWithHorses = !agentBuildData.AgentNoHorses;
        FormationClass formationIndex = agentFormation.FormationIndex;
        bool flag = canSpawnWithHorses && formationIndex.IsMounted();
        if (!agentFormation.HasBeenPositioned)
          this.SpawnFormation(agentFormation, formationTroopCount, canSpawnWithHorses, flag, agentBuildData.AgentIsReinforcement);
        if (!agentBuildData.AgentInitialFrame.HasValue)
        {
          Mat3 identity = Mat3.Identity;
          identity.RotateAboutUp(agentFormation.Direction.RotationInRadians);
          WorldFrame formationFrame = new WorldFrame(identity, agentFormation.OrderPosition);
          WorldFrame? nullable = new WorldFrame?(formationFrame);
          WorldFrame? spawnFrameWithIndex;
          if (agentBuildData.EnforceSpawningOnInitialPoint)
          {
            int groupSpawnIndex = agentFormation.GroupSpawnIndex;
            ++agentFormation.GroupSpawnIndex;
            WorldFrame formationSpawnFrame = this.GetFormationSpawnFrame(agentFormation.Team.Side, agentFormation.FormationIndex, false);
            spawnFrameWithIndex = agentFormation.GetUnitSpawnFrameWithIndex(groupSpawnIndex, ref formationSpawnFrame, agentFormation.Width, formationTroopCount, agentFormation.UnitSpacing, flag);
          }
          else if (agentBuildData.AgentIsReinforcement)
          {
            int groupSpawnIndex = agentFormation.GroupSpawnIndex;
            ++agentFormation.GroupSpawnIndex;
            WorldFrame formationSpawnFrame = this.GetFormationSpawnFrame(agentFormation.Team.Side, agentFormation.FormationIndex, true);
            spawnFrameWithIndex = agentFormation.GetUnitSpawnFrameWithIndex(groupSpawnIndex, ref formationSpawnFrame, agentFormation.Width, formationTroopCount, agentFormation.UnitSpacing, flag);
          }
          else
          {
            int countOfUnits = agentFormation.CountOfUnits;
            spawnFrameWithIndex = agentFormation.GetUnitSpawnFrameWithIndex(countOfUnits, ref formationFrame, agentFormation.Width, formationTroopCount, agentFormation.UnitSpacing, flag);
          }
          if (spawnFrameWithIndex.HasValue && (double) agentBuildData.MakeUnitStandOutDistance != 0.0)
            spawnFrameWithIndex.Value.Origin.SetVec2(spawnFrameWithIndex.Value.Origin.AsVec2 + spawnFrameWithIndex.Value.Rotation.f.AsVec2 * agentBuildData.MakeUnitStandOutDistance);
          agentBuildData.InitialFrame(spawnFrameWithIndex.HasValue ? spawnFrameWithIndex.GetValueOrDefault().ToGroundMatrixFrame() : formationFrame.ToGroundMatrixFrame());
        }
      }
      MatrixFrame? agentInitialFrame = agentBuildData.AgentInitialFrame;
      agent1.InitialFrame = agentInitialFrame.GetValueOrDefault();
      if (agentCharacter.AllEquipments == null)
        Debug.Print("characterObject.AllEquipments is null for \"" + agentCharacter.StringId + "\".");
      if (agentCharacter.AllEquipments != null && agentCharacter.AllEquipments.Any<Equipment>((Func<Equipment, bool>) (eq => eq == null)))
        Debug.Print("Character with id \"" + agentCharacter.StringId + "\" has a null equipment in its AllEquipments.");
      if (agentCharacter.AllEquipments.Where<Equipment>((Func<Equipment, bool>) (eq => eq.IsCivilian)) == null)
        agentBuildData.CivilianEquipment(false);
      if (agentCharacter.IsHero)
        agentBuildData.FixedEquipment(true);
      Equipment equipment1 = agentBuildData.AgentOverridenSpawnEquipment == null ? (agentBuildData.AgentFixedEquipment ? agentCharacter.GetFirstEquipment(agentBuildData.AgentCivilianEquipment).Clone() : Equipment.GetRandomEquipmentElements(agent1.Character, !Game.Current.GameType.IsCoreOnlyGameMode, agentBuildData.AgentCivilianEquipment, agentBuildData.AgentEquipmentSeed)) : agentBuildData.AgentOverridenSpawnEquipment.Clone();
      Agent agent2 = (Agent) null;
      if (agentBuildData.AgentNoHorses)
      {
        equipment1[EquipmentIndex.ArmorItemEndSlot] = new EquipmentElement();
        equipment1[EquipmentIndex.HorseHarness] = new EquipmentElement();
      }
      if (agentBuildData.AgentNoWeapons)
      {
        equipment1[EquipmentIndex.WeaponItemBeginSlot] = new EquipmentElement();
        equipment1[EquipmentIndex.Weapon1] = new EquipmentElement();
        equipment1[EquipmentIndex.Weapon2] = new EquipmentElement();
        equipment1[EquipmentIndex.Weapon3] = new EquipmentElement();
        equipment1[EquipmentIndex.Weapon4] = new EquipmentElement();
      }
      if (agentBuildData.AgentNoArmor)
      {
        equipment1[EquipmentIndex.Gloves] = new EquipmentElement();
        equipment1[EquipmentIndex.Body] = new EquipmentElement();
        equipment1[EquipmentIndex.Cape] = new EquipmentElement();
        equipment1[EquipmentIndex.NumAllWeaponSlots] = new EquipmentElement();
        equipment1[EquipmentIndex.Leg] = new EquipmentElement();
      }
      EquipmentElement equipmentElement1;
      for (int index = 0; index < 5; ++index)
      {
        equipmentElement1 = equipment1[(EquipmentIndex) index];
        if (!equipmentElement1.IsEmpty)
        {
          equipmentElement1 = equipment1[(EquipmentIndex) index];
          if (equipmentElement1.Item.ItemFlags.HasAnyFlag<ItemFlags>(ItemFlags.CannotBePickedUp))
          {
            Equipment equipment2 = equipment1;
            int num = index;
            equipmentElement1 = new EquipmentElement();
            EquipmentElement equipmentElement2 = equipmentElement1;
            equipment2[(EquipmentIndex) num] = equipmentElement2;
          }
        }
      }
      agent1.InitializeSpawnEquipment(equipment1);
      agent1.InitializeMissionEquipment(agentBuildData.AgentOverridenSpawnMissionEquipment, agentBuildData.AgentBanner);
      if (agent1.RandomizeColors)
        agent1.Equipment.SetGlossMultipliersOfWeaponsRandomly(agentBuildData.AgentEquipmentSeed);
      equipmentElement1 = equipment1[EquipmentIndex.ArmorItemEndSlot];
      ItemObject itemObject = equipmentElement1.Item;
      if (itemObject != null && itemObject.HasHorseComponent)
      {
        int forcedAgentMountIndex = -1;
        if (agentBuildData.AgentMountIndexOverriden)
          forcedAgentMountIndex = agentBuildData.AgentMountIndex;
        agent2 = this.CreateHorseAgentFromRosterElements(equipment1[EquipmentIndex.ArmorItemEndSlot], equipment1[EquipmentIndex.HorseHarness], agentBuildData.AgentInitialFrame.GetValueOrDefault(), forcedAgentMountIndex, agentBuildData.AgentMountKey);
        Equipment spawnEquipment = new Equipment()
        {
          [EquipmentIndex.ArmorItemEndSlot] = equipment1[EquipmentIndex.ArmorItemEndSlot],
          [EquipmentIndex.HorseHarness] = equipment1[EquipmentIndex.HorseHarness]
        };
        agent2.InitializeSpawnEquipment(spawnEquipment);
        agent1.SetMountAgentBeforeBuild(agent2);
      }
      if (spawnFromAgentVisuals || !GameNetwork.IsClientOrReplay)
        agent1.Equipment.CheckLoadedAmmos();
      if (!agentBuildData.BodyPropertiesOverriden)
        agent1.UpdateBodyProperties(agentCharacter.GetBodyProperties(equipment1, agentBuildData.AgentEquipmentSeed));
      agent1.Formation = agentBuildData.AgentFormation;
      if (GameNetwork.IsServerOrRecorder && agent1.RiderAgent == null)
      {
        MatrixFrame valueOrDefault = agentBuildData.AgentInitialFrame.GetValueOrDefault();
        if (agent1.IsMount)
        {
          GameNetwork.BeginBroadcastModuleEvent();
          GameNetwork.WriteMessage((GameNetworkMessage) new CreateFreeMountAgent(agent1, valueOrDefault.origin, valueOrDefault.rotation.f.AsVec2));
          GameNetwork.EndBroadcastModuleEvent(GameNetwork.EventBroadcastFlags.AddToMissionRecord);
        }
        else
        {
          bool isPlayerAgent = agentBuildData.AgentMissionPeer != null;
          NetworkCommunicator networkCommunicator;
          if (!isPlayerAgent)
          {
            MissionPeer agentMissionPeer = agentBuildData.OwningAgentMissionPeer;
            networkCommunicator = agentMissionPeer != null ? agentMissionPeer.GetNetworkPeer() : (NetworkCommunicator) null;
          }
          else
            networkCommunicator = agentBuildData.AgentMissionPeer.GetNetworkPeer();
          NetworkCommunicator peer = networkCommunicator;
          GameNetwork.BeginBroadcastModuleEvent();
          GameNetwork.WriteMessage((GameNetworkMessage) new NetworkMessages.FromServer.CreateAgent(agent1, isPlayerAgent, valueOrDefault.origin, valueOrDefault.rotation.f.AsVec2, peer));
          GameNetwork.EndBroadcastModuleEvent(GameNetwork.EventBroadcastFlags.AddToMissionRecord);
        }
      }
      MultiplayerMissionAgentVisualSpawnComponent missionBehaviour1 = Mission.Current.GetMissionBehaviour<MultiplayerMissionAgentVisualSpawnComponent>();
      if (missionBehaviour1 != null && agentBuildData.AgentMissionPeer != null && agentBuildData.AgentMissionPeer.IsMine)
      {
        if (agentBuildData.AgentVisualsIndex == 0)
        {
          try
          {
            missionBehaviour1.OnMyAgentSpawned();
          }
          catch (Exception ex)
          {
            Debug.Print("OnMyAgentSpawnedFromVisual exception");
            Debug.Print(ex.ToString());
          }
        }
      }
      if (agent2 != null)
      {
        this.BuildAgent(agent2, agentBuildData);
        foreach (MissionBehaviour missionBehaviour2 in this.MissionBehaviours)
          missionBehaviour2.OnAgentBuild(agent2, (Banner) null);
      }
      this.BuildAgent(agent1, agentBuildData);
      if (agentBuildData.AgentMissionPeer != null)
        agent1.MissionPeer = agentBuildData.AgentMissionPeer;
      if (agentBuildData.OwningAgentMissionPeer != null)
        agent1.OwningAgentMissionPeer = agentBuildData.OwningAgentMissionPeer;
      foreach (MissionBehaviour missionBehaviour2 in this.MissionBehaviours)
        missionBehaviour2.OnAgentBuild(agent1, agentBuildData.AgentBanner ?? agentBuildData.AgentTeam?.Banner);
      agent1.AgentVisuals.CheckResources(true);
      if (agent1.IsAIControlled)
      {
        if (agent2 == null)
        {
          AgentFlag agentFlags = agent1.GetAgentFlags() & ~AgentFlag.CanRide;
          agent1.SetAgentFlags(agentFlags);
        }
        else
          agent1.SetRidingOrder(1);
      }
      return agent1;
    }

    public void SpawnFormation(
      Formation formation,
      int formationTroopCount,
      bool canSpawnWithHorses,
      bool isMounted,
      bool isReinforcement)
    {
      WorldFrame worldFrame;
      if (this._currentDeploymentPlan.IsPlanMade)
      {
        FormationDeploymentPlan formationPlan = this._currentDeploymentPlan.GetFormationPlan(formation.Team.Side, formation.FormationIndex);
        worldFrame = formationPlan.GetFrame();
        if (formationPlan.PlannedTroopCount > 0 && formationPlan.HasDimensions)
          formation.FormOrder = FormOrder.FormOrderCustom(formationPlan.PlannedWidth);
      }
      else
        worldFrame = this.GetSceneMiddleFrame();
      formation.SetPositioning(new WorldPosition?(worldFrame.Origin), new Vec2?(worldFrame.Rotation.f.AsVec2));
    }

    public Agent SpawnMonster(
      ItemRosterElement rosterElement,
      ItemRosterElement harnessRosterElement,
      MatrixFrame initialFrame,
      int forcedAgentIndex = -1)
    {
      return this.SpawnMonster(rosterElement.EquipmentElement, harnessRosterElement.EquipmentElement, initialFrame, forcedAgentIndex);
    }

    public Agent SpawnMonster(
      EquipmentElement equipmentElement,
      EquipmentElement harnessRosterElement,
      MatrixFrame initialFrame,
      int forcedAgentIndex = -1)
    {
      Agent fromRosterElements = this.CreateHorseAgentFromRosterElements(equipmentElement, harnessRosterElement, initialFrame, forcedAgentIndex, MountCreationKey.GetRandomMountKey(equipmentElement.Item, MBRandom.RandomInt()));
      Equipment spawnEquipment = new Equipment()
      {
        [EquipmentIndex.ArmorItemEndSlot] = equipmentElement,
        [EquipmentIndex.HorseHarness] = harnessRosterElement
      };
      fromRosterElements.InitializeSpawnEquipment(spawnEquipment);
      if (GameNetwork.IsServerOrRecorder)
      {
        GameNetwork.BeginBroadcastModuleEvent();
        GameNetwork.WriteMessage((GameNetworkMessage) new CreateFreeMountAgent(fromRosterElements, initialFrame.origin, initialFrame.rotation.f.AsVec2));
        GameNetwork.EndBroadcastModuleEvent(GameNetwork.EventBroadcastFlags.AddToMissionRecord);
      }
      this.BuildAgent(fromRosterElements, (AgentBuildData) null);
      return fromRosterElements;
    }

    public Agent SpawnTroop(
      IAgentOriginBase troopOrigin,
      bool isPlayerSide,
      bool hasFormation,
      bool spawnWithHorse,
      bool isReinforcement,
      bool enforceSpawningOnInitialPoint,
      int formationTroopCount,
      int formationTroopIndex,
      bool isAlarmed,
      bool wieldInitialWeapons,
      bool forceDismounted = false,
      string specialActionSet = null,
      MatrixFrame? initFrame = null)
    {
      BasicCharacterObject troop = troopOrigin.Troop;
      Team agentTeam = Mission.GetAgentTeam(troopOrigin, isPlayerSide);
      MatrixFrame frame = initFrame ?? Mission.Current.GetFormationSpawnFrame(agentTeam.Side, FormationClass.NumberOfAllFormations, false).ToGroundMatrixFrame();
      if (troop.IsPlayerCharacter && !forceDismounted)
        spawnWithHorse = true;
      AgentBuildData agentBuildData = new AgentBuildData(troop).Team(agentTeam).Banner(troopOrigin.Banner).ClothingColor1(agentTeam.Color).ClothingColor2(agentTeam.Color2).TroopOrigin(troopOrigin).NoHorses(!spawnWithHorse).CivilianEquipment(Mission.Current.DoesMissionRequireCivilianEquipment);
      if (!troop.IsPlayerCharacter)
        agentBuildData.IsReinforcement(isReinforcement).SpawnOnInitialPoint(enforceSpawningOnInitialPoint);
      if (!hasFormation || troop.IsPlayerCharacter)
        agentBuildData.InitialFrame(frame);
      if (spawnWithHorse)
        agentBuildData.MountKey(MountCreationKey.GetRandomMountKey(troop.Equipment[EquipmentIndex.ArmorItemEndSlot].Item, troop.GetMountKeySeed()));
      if (hasFormation && !troop.IsPlayerCharacter)
      {
        Formation formation = agentTeam.GetFormation(troop.GetFormationClass(troopOrigin.BattleCombatant));
        agentBuildData.Formation(formation);
        agentBuildData.FormationTroopCount(formationTroopCount).FormationTroopIndex(formationTroopIndex);
      }
      if (isPlayerSide && troop == Game.Current.PlayerTroop)
        agentBuildData.Controller(Agent.ControllerType.Player);
      Agent agent = Mission.Current.SpawnAgent(agentBuildData, formationTroopCount: formationTroopCount);
      if (agent.Character.IsHero)
        agent.SetAgentFlags(agent.GetAgentFlags() | AgentFlag.IsUnique);
      if (agent.IsAIControlled & isAlarmed)
        agent.SetWatchState(AgentAIStateFlagComponent.WatchState.Alarmed);
      if (wieldInitialWeapons)
        agent.WieldInitialWeapons();
      if (!specialActionSet.IsStringNoneOrEmpty())
      {
        AnimationSystemData animationSystemData = agentBuildData.AgentMonster.FillAnimationSystemData(MBGlobals.GetActionSet(specialActionSet), agent.Character.GetStepSize(), false);
        AgentVisualsNativeData agentVisualsNativeData = agentBuildData.AgentMonster.FillAgentVisualsNativeData();
        agent.SetActionSet(ref agentVisualsNativeData, ref animationSystemData);
      }
      return agent;
    }

    public Agent ReplaceBotWithPlayer(Agent botAgent, MissionPeer missionPeer)
    {
      if (GameNetwork.IsClientOrReplay || botAgent == null)
        return (Agent) null;
      if (GameNetwork.IsServer)
      {
        NetworkCommunicator networkPeer = missionPeer.GetNetworkPeer();
        if (!networkPeer.IsServerPeer)
        {
          GameNetwork.BeginModuleEventAsServer(networkPeer);
          GameNetwork.WriteMessage((GameNetworkMessage) new NetworkMessages.FromServer.ReplaceBotWithPlayer(networkPeer, botAgent));
          GameNetwork.EndModuleEventAsServer();
        }
      }
      if (botAgent.Formation != null)
        botAgent.Formation.PlayerOwner = botAgent;
      botAgent.OwningAgentMissionPeer = (MissionPeer) null;
      botAgent.MissionPeer = missionPeer;
      botAgent.Formation = missionPeer.ControlledFormation;
      AgentFlag agentFlags = botAgent.GetAgentFlags();
      if (!agentFlags.HasAnyFlag<AgentFlag>(AgentFlag.CanRide))
        botAgent.SetAgentFlags(agentFlags | AgentFlag.CanRide);
      --missionPeer.BotsUnderControlAlive;
      GameNetwork.BeginBroadcastModuleEvent();
      GameNetwork.WriteMessage((GameNetworkMessage) new BotsControlledChange(missionPeer.GetNetworkPeer(), missionPeer.BotsUnderControlAlive, missionPeer.BotsUnderControlTotal));
      GameNetwork.EndBroadcastModuleEvent(GameNetwork.EventBroadcastFlags.None);
      if (botAgent.Formation != null)
        missionPeer.Team.AssignPlayerAsSergeantOfFormation(missionPeer, missionPeer.ControlledFormation.FormationIndex);
      return botAgent;
    }

    private Agent CreateHorseAgentFromRosterElements(
      EquipmentElement horse,
      EquipmentElement monsterHarness,
      MatrixFrame initialFrame,
      int forcedAgentMountIndex,
      string horseCreationKey)
    {
      HorseComponent horseComponent = horse.Item.HorseComponent;
      float num = (double) horse.Weight > 0.0 ? horse.Weight : (float) horseComponent.Monster.Weight;
      Agent agent = this.CreateAgent(horseComponent.Monster, false, 0, Agent.CreationType.FromHorseObj, 1f, forcedAgentMountIndex, (int) num, (BasicCharacterObject) null);
      agent.InitialFrame = initialFrame;
      agent.BaseHealthLimit = (float) horse.GetModifiedMountHitPoints();
      agent.HealthLimit = agent.BaseHealthLimit;
      agent.Health = agent.HealthLimit;
      agent.SetMountInitialValues(horse.GetModifiedItemName(), horseCreationKey);
      return agent;
    }

    internal void OnAgentInteraction(Agent requesterAgent, Agent targetAgent)
    {
      if (requesterAgent == Agent.Main && targetAgent.IsMount)
      {
        Agent.Main.Mount(targetAgent);
      }
      else
      {
        if (!targetAgent.IsHuman)
          return;
        foreach (MissionBehaviour missionBehaviour in this.MissionBehaviours)
          missionBehaviour.OnAgentInteraction(requesterAgent, targetAgent);
        if (!GameNetwork.IsSessionActive)
          return;
        requesterAgent.MissionRepresentative.OnAgentInteraction(targetAgent);
      }
    }

    [UsedImplicitly]
    [MBCallback]
    public void EndMission()
    {
      Debug.Print("I called EndMission", debugFilter: 17179869184UL);
      this._missionEndTime = -1f;
      this.NextCheckTimeEndMission = -1f;
      this._missionEnded = true;
      this.CurrentState = Mission.State.EndingNextFrame;
    }

    private void EndMissionInternal()
    {
      MBDebug.Print("I called EndMissionInternal", debugFilter: 17179869184UL);
      foreach (IMissionListener missionListener in this._listeners.ToArray())
        missionListener.OnEndMission();
      this.StopSoundEvents();
      foreach (MissionBehaviour missionBehaviour in this.MissionBehaviours)
        missionBehaviour.HandleOnCloseMission();
      foreach (Agent agent in (IEnumerable<Agent>) this.Agents)
        agent.OnRemove();
      foreach (Agent allAgent in (IEnumerable<Agent>) this.AllAgents)
      {
        allAgent.OnDelete();
        allAgent.Clear();
      }
      this.Teams.Clear();
      foreach (MissionObject missionObject in (IEnumerable<MissionObject>) this.MissionObjects)
        missionObject.OnEndMission();
      this.CurrentState = Mission.State.Over;
      if (GameNetwork.IsSessionActive && this.GetMissionBehaviour<MissionNetworkComponent>() != null)
        this.RemoveMissionBehaviour((MissionBehaviour) this.GetMissionBehaviour<MissionNetworkComponent>());
      for (int index = this._missionBehaviourList.Count - 1; index >= 0; --index)
        this.RemoveMissionBehaviour(this._missionBehaviourList[index]);
      this.FreeResources();
      this.FinalizeMission();
      this._missionLogics.Clear();
    }

    private void StopSoundEvents()
    {
      if (this._ambientSoundEvent == null)
        return;
      this._ambientSoundEvent.Stop();
    }

    public void AddMissionBehaviour(MissionBehaviour missionBehaviour)
    {
      this._missionBehaviourList.Add(missionBehaviour);
      missionBehaviour.Mission = this;
      switch (missionBehaviour.BehaviourType)
      {
        case MissionBehaviourType.Logic:
          this._missionLogics.Add(missionBehaviour as MissionLogic);
          break;
        case MissionBehaviourType.Other:
          this._otherMissionBehaviours.Add(missionBehaviour);
          break;
      }
      missionBehaviour.OnCreated();
    }

    public T GetMissionBehaviour<T>() where T : class, IMissionBehavior
    {
      foreach (MissionBehaviour missionBehaviour in this._missionBehaviourList)
      {
        if (missionBehaviour is T obj1)
          return obj1;
      }
      return default (T);
    }

    public void RemoveMissionBehaviour(MissionBehaviour missionBehaviour)
    {
      missionBehaviour.OnRemoveBehaviour();
      switch (missionBehaviour.BehaviourType)
      {
        case MissionBehaviourType.Logic:
          this._missionLogics.Remove(missionBehaviour as MissionLogic);
          break;
        case MissionBehaviourType.Other:
          this._otherMissionBehaviours.Remove(missionBehaviour);
          break;
      }
      this._missionBehaviourList.Remove(missionBehaviour);
      missionBehaviour.Mission = (Mission) null;
    }

    public void JoinEnemyTeam()
    {
      if (this.PlayerTeam == this.DefenderTeam)
      {
        Agent leader = this.AttackerTeam.Leader;
        if (leader == null)
          return;
        if (this.MainAgent != null && this.MainAgent.IsActive())
          this.MainAgent.Controller = Agent.ControllerType.AI;
        leader.Controller = Agent.ControllerType.Player;
        this.PlayerTeam = this.AttackerTeam;
      }
      else
      {
        if (this.PlayerTeam != this.AttackerTeam)
          return;
        Agent leader = this.DefenderTeam.Leader;
        if (leader == null)
          return;
        if (this.MainAgent != null && this.MainAgent.IsActive())
          this.MainAgent.Controller = Agent.ControllerType.AI;
        leader.Controller = Agent.ControllerType.Player;
        this.PlayerTeam = this.DefenderTeam;
      }
    }

    public void OnEndMissionRequest()
    {
      foreach (MissionLogic missionLogic in this.MissionLogics)
      {
        bool canLeave;
        InquiryData data = missionLogic.OnEndMissionRequest(out canLeave);
        if (!canLeave)
        {
          this._leaveMissionTimer = (BasicTimer) null;
          return;
        }
        if (data != null)
        {
          this._leaveMissionTimer = (BasicTimer) null;
          InformationManager.ShowInquiry(data, true);
          return;
        }
      }
      if (this._leaveMissionTimer != null)
      {
        if ((double) this._leaveMissionTimer.ElapsedTime <= (double) this._exitTimeInSeconds)
          return;
        this._leaveMissionTimer = (BasicTimer) null;
        this.EndMission();
      }
      else
        this._leaveMissionTimer = new BasicTimer(MBCommon.TimeType.Mission);
    }

    public float GetMissionEndTimerValue() => this._leaveMissionTimer == null ? -1f : this._leaveMissionTimer.ElapsedTime;

    public float GetMissionEndTimeInSeconds() => this._exitTimeInSeconds;

    public void OnEndMissionResult()
    {
      foreach (MissionLogic missionLogic in this.MissionLogics.ToArray<MissionLogic>())
        missionLogic.OnBattleEnded();
      this.RetreatMission();
    }

    public bool IsAgentInteractionAllowed() => this.MissionLogics.All<MissionLogic>((Func<MissionLogic, bool>) (x => x.IsAgentInteractionAllowed()));

    public bool IsOrderShoutingAllowed() => this.MissionLogics.All<MissionLogic>((Func<MissionLogic, bool>) (x => x.IsOrderShoutingAllowed()));

    public bool MissionEnded() => this._missionEnded;

    private bool CheckMissionEnded()
    {
      foreach (MissionLogic missionLogic in this.MissionLogics)
      {
        MissionResult missionResult = (MissionResult) null;
        ref MissionResult local = ref missionResult;
        if (missionLogic.MissionEnded(ref local))
        {
          this.MissionResult = missionResult;
          this._missionEnded = true;
          this.MissionResultReady(missionResult);
          return true;
        }
      }
      return false;
    }

    private void MissionResultReady(MissionResult missionResult)
    {
      foreach (MissionLogic missionLogic in this.MissionLogics)
        missionLogic.OnMissionResultReady(missionResult);
    }

    private void CheckMissionEnd(float currentTime)
    {
      if (!GameNetwork.IsClient && (double) currentTime > (double) this.NextCheckTimeEndMission)
      {
        if (this.CurrentState == Mission.State.Continuing)
        {
          if (this._missionEnded)
            return;
          this.NextCheckTimeEndMission += 0.1f;
          this.CheckMissionEnded();
          if (!this._missionEnded)
            return;
          this._missionEndTime = currentTime + this.MissionCloseTimeAfterFinish;
          this.NextCheckTimeEndMission += 5f;
          foreach (MissionLogic missionLogic in this.MissionLogics)
            missionLogic.ShowBattleResults();
        }
        else if ((double) currentTime > (double) this._missionEndTime)
          this.EndMissionInternal();
        else
          this.NextCheckTimeEndMission += 5f;
      }
      else
      {
        if (this.CurrentState == Mission.State.Continuing || (double) currentTime <= (double) this.NextCheckTimeEndMission)
          return;
        this.EndMissionInternal();
      }
    }

    public bool IsPlayerCloseToAnEnemy(float distance = 5f)
    {
      if (this.MainAgent == null)
        return false;
      Vec3 position = this.MainAgent.Position;
      float num = distance * distance;
      foreach (Agent agent in this.GetAgentsInRange(position.AsVec2, distance))
      {
        if (agent != this.MainAgent && agent.IsActive() && agent.GetAgentFlags().HasAnyFlag<AgentFlag>(AgentFlag.CanAttack) && ((double) agent.Position.DistanceSquared(position) <= (double) num && (!agent.IsAIControlled || agent.IsAlarmed()) && agent.IsEnemyOf(this.MainAgent)))
          return true;
      }
      return false;
    }

    public Vec2 GetAveragePositionOfEnemies(Team currentTeam)
    {
      Vec2 zero = Vec2.Zero;
      int num = 0;
      foreach (Team team in (ReadOnlyCollection<Team>) this.Teams)
      {
        if (team.MBTeam.IsValid && currentTeam.IsEnemyOf(team))
        {
          foreach (Agent activeAgent in team.ActiveAgents)
          {
            zero += activeAgent.Position.AsVec2;
            ++num;
          }
        }
      }
      return num > 0 ? zero * (1f / (float) num) : Vec2.Invalid;
    }

    public Vec2 GetAveragePositionOfTeam(Team team)
    {
      Vec2 zero = Vec2.Zero;
      MBReadOnlyList<Agent> activeAgents = team.ActiveAgents;
      int num = 0;
      foreach (Agent agent in activeAgents)
      {
        zero += agent.Position.AsVec2;
        ++num;
      }
      return num <= 0 ? Vec2.Invalid : zero * (1f / (float) num);
    }

    public WorldPosition GetMedianPositionOfTeam(Team team, Vec2 averagePosition) => team.ActiveAgents.Any<Agent>() ? this.GetMedianPositionOfAgents((IEnumerable<Agent>) team.ActiveAgents, averagePosition) : WorldPosition.Invalid;

    public Vec2 GetWeightedAverageOfEnemies(
      Team currentTeam,
      Vec2 basePoint,
      bool quadrapleDistanceWeight = false)
    {
      Vec2 zero = Vec2.Zero;
      float num1 = 0.0f;
      for (int index = 0; index < this.Teams.Count; ++index)
      {
        Team team = this.Teams[index];
        if (team.MBTeam.IsValid && currentTeam.IsEnemyOf(team))
        {
          foreach (Agent activeAgent in team.ActiveAgents)
          {
            Vec2 asVec2 = activeAgent.Position.AsVec2;
            float num2;
            if (!quadrapleDistanceWeight)
            {
              num2 = (basePoint - asVec2).LengthSquared + 25f;
            }
            else
            {
              float lengthSquared = (basePoint - asVec2).LengthSquared;
              num2 = lengthSquared * lengthSquared + 15f;
            }
            float num3 = 1f / num2;
            zero += asVec2 * num3;
            num1 += num3;
          }
        }
      }
      return (double) num1 > 0.0 ? zero * (1f / num1) : Vec2.Invalid;
    }

    public Vec3 GetRandomPositionAroundPoint(
      Vec3 center,
      float minDistance,
      float maxDistance,
      bool nearFirst = false)
    {
      Vec3 vec3_1 = center;
      Vec3 vec3_2 = new Vec3(-1f);
      vec3_2.RotateAboutZ(6.283185f * MBRandom.RandomFloat);
      float num = maxDistance - minDistance;
      if (nearFirst)
      {
        for (int index1 = 4; index1 > 0; --index1)
        {
          for (int index2 = 0; (double) index2 <= 10.0; ++index2)
          {
            vec3_2.RotateAboutZ(1.256637f);
            Vec3 position = vec3_1 + vec3_2 * (minDistance + num / (float) index1);
            if (this.Scene.GetNavigationMeshForPosition(ref position))
              return position;
          }
        }
      }
      else
      {
        for (int index1 = 1; index1 < 5; ++index1)
        {
          for (int index2 = 0; (double) index2 <= 10.0; ++index2)
          {
            vec3_2.RotateAboutZ(1.256637f);
            Vec3 position = vec3_1 + vec3_2 * (minDistance + num / (float) index1);
            if (this.Scene.GetNavigationMeshForPosition(ref position))
              return position;
          }
        }
      }
      return vec3_1;
    }

    public WorldFrame GetSceneMiddleFrame()
    {
      Vec2 vec2 = Vec2.Invalid;
      ICollection<Vec2> source = this.Boundaries.First<KeyValuePair<string, ICollection<Vec2>>>().Value;
      if (source.Count > 0)
        vec2 = source.Aggregate<Vec2>((Func<Vec2, Vec2, Vec2>) ((a, b) => a + b)) * (1f / (float) source.Count);
      return new WorldFrame(Mat3.Identity, new WorldPosition(Mission.Current.Scene, UIntPtr.Zero, vec2.ToVec3(), false));
    }

    internal WorldPosition FindBestDefendingPosition(
      WorldPosition enemyPosition,
      WorldPosition defendedPosition)
    {
      return this.GetBestSlopeAngleHeightPosForDefending(enemyPosition, defendedPosition, 10, 0.5f, 4f, 0.5f, 0.7071068f, 0.1f, 1f, 0.7f, 0.5f, 1.2f, 20f, 0.6f);
    }

    internal WorldPosition FindPositionWithBiggestSlopeTowardsDirectionInSquare(
      ref WorldPosition center,
      float halfSize,
      ref WorldPosition referencePosition)
    {
      return this.GetBestSlopeTowardsDirection(ref center, halfSize, ref referencePosition);
    }

    public void AddCustomMissile(
      Agent shooterAgent,
      MissionWeapon missileWeapon,
      Vec3 position,
      Vec3 direction,
      Mat3 orientation,
      float baseSpeed,
      float speed,
      bool addRigidBody,
      MissionObject missionObjectToIgnore,
      int forcedMissileIndex = -1)
    {
      WeaponData weaponData = missileWeapon.GetWeaponData(true);
      WeaponStatsData[] weaponStatsData = missileWeapon.GetWeaponStatsData();
      GameEntity missileEntity;
      int num = this.AddMissileAux(forcedMissileIndex, false, shooterAgent, in weaponData, weaponStatsData, 0.0f, ref position, ref direction, ref orientation, baseSpeed, speed, addRigidBody, missionObjectToIgnore?.GameEntity, false, out missileEntity);
      weaponData.DeinitializeManagedPointers();
      Mission.Missile missile1 = new Mission.Missile(this, missileEntity);
      missile1.ShooterAgent = shooterAgent;
      missile1.Weapon = missileWeapon;
      missile1.MissionObjectToIgnore = missionObjectToIgnore;
      missile1.Index = num;
      Mission.Missile missile2 = missile1;
      MBDebug.Print("AddMissile - " + (object) num);
      this._missiles.Add(num, missile2);
      if (!GameNetwork.IsServerOrRecorder)
        return;
      GameNetwork.BeginBroadcastModuleEvent();
      GameNetwork.WriteMessage((GameNetworkMessage) new CreateMissile(num, shooterAgent, EquipmentIndex.None, missileWeapon, position, direction, speed, orientation, addRigidBody, missionObjectToIgnore, false));
      GameNetwork.EndBroadcastModuleEvent(GameNetwork.EventBroadcastFlags.AddToMissionRecord);
    }

    [UsedImplicitly]
    [MBCallback]
    internal void OnAgentShootMissile(
      Agent shooterAgent,
      EquipmentIndex weaponIndex,
      Vec3 position,
      Vec3 velocity,
      Mat3 orientation,
      bool hasRigidBody,
      bool isPrimaryWeaponShot,
      int forcedMissileIndex)
    {
      float damageBonus = 0.0f;
      MissionWeapon ammoWeapon;
      if (shooterAgent.Equipment[weaponIndex].CurrentUsageItem.IsRangedWeapon && shooterAgent.Equipment[weaponIndex].CurrentUsageItem.IsConsumable)
      {
        ammoWeapon = shooterAgent.Equipment[weaponIndex];
      }
      else
      {
        ammoWeapon = shooterAgent.Equipment[weaponIndex].AmmoWeapon;
        if (shooterAgent.Equipment[weaponIndex].CurrentUsageItem.WeaponFlags.HasAnyFlag<WeaponFlags>(WeaponFlags.HasString))
          damageBonus = (float) shooterAgent.Equipment[weaponIndex].GetModifiedThrustDamageForCurrentUsage();
      }
      ammoWeapon.Amount = (short) 1;
      WeaponData weaponData = ammoWeapon.GetWeaponData(true);
      WeaponStatsData[] weaponStatsData = ammoWeapon.GetWeaponStatsData();
      Vec3 direction = velocity;
      float missileStartSpeed = direction.Normalize();
      WeaponComponentData currentUsageItem = shooterAgent.Equipment[shooterAgent.GetWieldedItemIndex(Agent.HandIndex.MainHand)].CurrentUsageItem;
      float effectiveMissileSpeed = MissionGameModels.Current.AgentApplyDamageModel.CalculateEffectiveMissileSpeed(shooterAgent, currentUsageItem, ref direction, missileStartSpeed);
      bool isPrediction = GameNetwork.IsClient && forcedMissileIndex == -1;
      float speedForCurrentUsage = (float) shooterAgent.Equipment[shooterAgent.GetWieldedItemIndex(Agent.HandIndex.MainHand)].GetModifiedMissileSpeedForCurrentUsage();
      MPPerkObject.MPPerkHandler perkHandler = MPPerkObject.GetPerkHandler(shooterAgent);
      if (perkHandler != null)
        speedForCurrentUsage *= perkHandler.GetThrowingWeaponSpeed(currentUsageItem) + 1f;
      GameEntity missileEntity;
      int num = this.AddMissileAux(forcedMissileIndex, isPrediction, shooterAgent, in weaponData, weaponStatsData, damageBonus, ref position, ref direction, ref orientation, speedForCurrentUsage, effectiveMissileSpeed, hasRigidBody, (GameEntity) null, isPrimaryWeaponShot, out missileEntity);
      weaponData.DeinitializeManagedPointers();
      if (!isPrediction)
      {
        Mission.Missile missile1 = new Mission.Missile(this, missileEntity);
        missile1.ShooterAgent = shooterAgent;
        missile1.Weapon = ammoWeapon;
        missile1.Index = num;
        Mission.Missile missile2 = missile1;
        missileEntity.ManualInvalidate();
        this._missiles.Add(num, missile2);
        if (GameNetwork.IsServerOrRecorder)
        {
          GameNetwork.BeginBroadcastModuleEvent();
          GameNetwork.WriteMessage((GameNetworkMessage) new CreateMissile(num, shooterAgent, weaponIndex, MissionWeapon.Invalid, position, direction, effectiveMissileSpeed, orientation, hasRigidBody, (MissionObject) null, isPrimaryWeaponShot));
          GameNetwork.EndBroadcastModuleEvent(GameNetwork.EventBroadcastFlags.AddToMissionRecord);
        }
      }
      foreach (MissionBehaviour missionBehaviour in this.MissionBehaviours)
        missionBehaviour.OnAgentShootMissile(shooterAgent, weaponIndex, position, velocity, orientation, hasRigidBody, forcedMissileIndex);
      shooterAgent?.UpdateLastRangedAttackTimeDueToAnAttack(MBCommon.TimeType.Mission.GetTime());
    }

    [UsedImplicitly]
    [MBCallback]
    internal AgentState GetAgentState(
      Agent affectorAgent,
      Agent agent,
      DamageTypes damageType)
    {
      float useSurgeryProbability;
      float stateProbability = MissionGameModels.Current.AgentDecideKilledOrUnconsciousModel.GetAgentStateProbability(affectorAgent, agent, damageType, out useSurgeryProbability);
      AgentState agentState = AgentState.None;
      bool usedSurgery = false;
      foreach (MissionBehaviour missionBehaviour in this.MissionBehaviours)
      {
        if (missionBehaviour is IAgentStateDecider)
        {
          agentState = (missionBehaviour as IAgentStateDecider).GetAgentState(agent, stateProbability, out usedSurgery);
          break;
        }
      }
      if (agentState == AgentState.None)
      {
        float randomFloat = MBRandom.RandomFloat;
        if ((double) randomFloat < (double) stateProbability && !agent.Character.IsPlayerCharacter)
        {
          agentState = AgentState.Killed;
          usedSurgery = true;
        }
        else
        {
          agentState = AgentState.Unconscious;
          if ((double) randomFloat > 1.0 - (double) useSurgeryProbability)
            usedSurgery = true;
        }
      }
      if (usedSurgery && affectorAgent.Team != null && (agent.Team != null && affectorAgent.Team == agent.Team))
        usedSurgery = false;
      for (int index = 0; index < this._missionBehaviourList.Count; ++index)
        this._missionBehaviourList[index].OnGetAgentState(agent, usedSurgery);
      return agentState;
    }

    internal void OnAgentMount(Agent agent)
    {
      foreach (MissionBehaviour missionBehaviour in this.MissionBehaviours)
        missionBehaviour.OnAgentMount(agent);
    }

    internal void OnAgentDismount(Agent agent)
    {
      foreach (MissionBehaviour missionBehaviour in this.MissionBehaviours)
        missionBehaviour.OnAgentDismount(agent);
    }

    internal void OnObjectUsed(Agent userAgent, UsableMissionObject usableGameObject)
    {
      foreach (MissionBehaviour missionBehaviour in this.MissionBehaviours)
        missionBehaviour.OnObjectUsed(userAgent, usableGameObject);
    }

    internal void OnObjectStoppedBeingUsed(Agent userAgent, UsableMissionObject usableGameObject)
    {
      foreach (MissionBehaviour missionBehaviour in this.MissionBehaviours)
        missionBehaviour.OnObjectStoppedBeingUsed(userAgent, usableGameObject);
    }

    internal void InitializeStartingBehaviours(
      MissionLogic[] logicBehaviours,
      MissionBehaviour[] otherBehaviours,
      MissionNetwork[] networkBehaviours)
    {
      foreach (MissionBehaviour logicBehaviour in logicBehaviours)
        this.AddMissionBehaviour(logicBehaviour);
      foreach (MissionBehaviour networkBehaviour in networkBehaviours)
        this.AddMissionBehaviour(networkBehaviour);
      foreach (MissionBehaviour otherBehaviour in otherBehaviours)
        this.AddMissionBehaviour(otherBehaviour);
    }

    public Agent GetClosestEnemyAgent(Team team, Vec3 position, float radius) => this.GetClosestEnemyAgent(team.MBTeam, position, radius);

    public Agent GetClosestAllyAgent(Team team, Vec3 position, float radius) => this.GetClosestAllyAgent(team.MBTeam, position, radius);

    public int GetNearbyEnemyAgentCount(Team team, Vec2 position, float radius) => this.GetNearbyEnemyAgentCount(team.MBTeam, position, radius);

    public IEnumerable<Agent> GetAgentsInRange(
      Vec2 pos,
      float range,
      bool extendRangeByBiggestAgentCollisionPadding = false)
    {
      if (extendRangeByBiggestAgentCollisionPadding)
        range += Mission.Current.GetBiggestAgentCollisionPadding() + 1f;
      if (AgentProximityMap.CanSearchRadius(range))
      {
        AgentProximityMap.ProximityMapSearchStruct ss = AgentProximityMap.BeginSearch(pos, range);
        while (ss.LastFoundAgent != null)
        {
          yield return ss.LastFoundAgent;
          AgentProximityMap.FindNext(ref ss);
        }
        ss = new AgentProximityMap.ProximityMapSearchStruct();
      }
      else
      {
        foreach (Agent agent in (IEnumerable<Agent>) this.Agents)
        {
          if ((double) agent.Position.AsVec2.DistanceSquared(pos) <= (double) range * (double) range)
            yield return agent;
        }
      }
    }

    private void HandleSpawnedItems()
    {
      if (GameNetwork.IsClientOrReplay)
        return;
      int num = 0;
      for (int index = this._missionObjects.Count - 1; index >= 0; --index)
      {
        MissionObject missionObject = this._missionObjects[index];
        if (missionObject.CreatedAtRuntime && missionObject is SpawnedItemEntity)
        {
          SpawnedItemEntity spawnedItemEntity = missionObject as SpawnedItemEntity;
          if (!spawnedItemEntity.IsDeactivated && !spawnedItemEntity.HasUser && (spawnedItemEntity.HasLifeTime && spawnedItemEntity.MovingAgents.Count == 0) && (num > 500 || spawnedItemEntity.IsReadyToBeDeleted()))
          {
            spawnedItemEntity.GameEntity.ClearEntityComponents(true, false, false);
            spawnedItemEntity.GameEntity.Remove(80);
            spawnedItemEntity.OnSpawnedItemEntityRemoved();
          }
          else
            ++num;
        }
      }
    }

    internal bool OnMissionObjectRemoved(MissionObject missionObject, int removeReason)
    {
      if (!GameNetwork.IsClientOrReplay && missionObject.CreatedAtRuntime)
      {
        this.ReturnRuntimeMissionObjectId(missionObject.Id.Id);
        if (GameNetwork.IsServerOrRecorder)
        {
          this.RemoveDynamicallySpawnedMissionObjectInfo(missionObject.Id);
          GameNetwork.BeginBroadcastModuleEvent();
          GameNetwork.WriteMessage((GameNetworkMessage) new RemoveMissionObject(missionObject.Id));
          GameNetwork.EndBroadcastModuleEvent(GameNetwork.EventBroadcastFlags.AddToMissionRecord);
        }
      }
      this._activeMissionObjects.Remove(missionObject);
      return this._missionObjects.Remove(missionObject);
    }

    public bool AgentLookingAtAgent(Agent agent1, Agent agent2)
    {
      Vec3 v1 = agent2.Position - agent1.Position;
      float num1 = v1.Normalize();
      float num2 = Vec3.DotProduct(v1, agent1.LookDirection);
      return ((double) num2 >= 1.0 ? 0 : ((double) num2 > 0.860000014305115 ? 1 : 0)) != 0 && (double) num1 < 4.0;
    }

    public Agent FindAgentWithIndex(int agentId) => this.FindAgentWithIndexAux(agentId);

    public static Agent.UnderAttackType GetUnderAttackTypeOfAgents(
      IEnumerable<Agent> agents,
      float timeLimit = 3f)
    {
      float val1_1 = float.MinValue;
      float val1_2 = float.MinValue;
      timeLimit += MBCommon.TimeType.Mission.GetTime();
      foreach (Agent agent in agents)
      {
        val1_1 = Math.Max(val1_1, agent.LastMeleeHitTime);
        val1_2 = Math.Max(val1_2, agent.LastRangedHitTime);
        if ((double) val1_2 >= 0.0 && (double) val1_2 < (double) timeLimit)
          return Agent.UnderAttackType.UnderRangedAttack;
        if ((double) val1_1 >= 0.0 && (double) val1_1 < (double) timeLimit)
          return Agent.UnderAttackType.UnderMeleeAttack;
      }
      return Agent.UnderAttackType.NotUnderAttack;
    }

    public static Team GetAgentTeam(IAgentOriginBase troopOrigin, bool isPlayerSide)
    {
      if (Mission.Current == null)
        return (Team) null;
      Team team = Mission.Current.PlayerTeam;
      if (troopOrigin.IsUnderPlayersCommand)
        team = Mission.Current.PlayerTeam;
      else if (isPlayerSide)
      {
        if (Mission.Current.PlayerAllyTeam != null)
          team = Mission.Current.PlayerAllyTeam;
      }
      else
        team = Mission.Current.PlayerEnemyTeam;
      return team;
    }

    public Agent.MovementBehaviourType GetMovementTypeOfAgents(IEnumerable<Agent> agents)
    {
      float time = MBCommon.TimeType.Mission.GetTime();
      int num1 = 0;
      int num2 = 0;
      int num3 = 0;
      foreach (Agent agent in agents)
      {
        ++num1;
        if (agent.IsAIControlled && (agent.IsRetreating() || agent.Formation != null && agent.Formation.MovementOrder.OrderType == OrderType.Retreat))
          ++num2;
        if ((double) time - (double) agent.LastMeleeAttackTime < 3.0)
          ++num3;
      }
      if ((double) num2 * 1.0 / (double) num1 > 0.300000011920929)
        return Agent.MovementBehaviourType.Flee;
      return num3 > 0 ? Agent.MovementBehaviourType.Engaged : Agent.MovementBehaviourType.Idle;
    }

    public void OnRenderingStarted()
    {
      foreach (MissionBehaviour missionBehaviour in this.MissionBehaviours)
        missionBehaviour.OnRenderingStarted();
    }

    public void ShowInMissionLoadingScreen(int durationInSecond, Action onLoadingEndedAction)
    {
      this._inMissionLoadingScreenTimer = new Timer(this.Time, (float) durationInSecond);
      this._onLoadingEndedAction = onLoadingEndedAction;
      LoadingWindow.EnableGlobalLoadingWindow();
    }

    [UsedImplicitly]
    [MBCallback]
    internal bool CanGiveDamageToAgentShield(Agent attacker, Agent defender)
    {
      int num = !GameNetwork.IsSessionActive || MultiplayerOptions.OptionType.FriendlyFireDamageMeleeFriendPercent.GetIntValue() <= 0 && MultiplayerOptions.OptionType.FriendlyFireDamageMeleeSelfPercent.GetIntValue() <= 0 || this.Mode == MissionMode.Duel ? 1 : (attacker == null ? 0 : (attacker.Controller == Agent.ControllerType.AI ? 1 : 0));
      return !this.CancelsDamageAndBlocksAttackBecauseOfNonEnemyCase(attacker, defender);
    }

    [UsedImplicitly]
    [MBCallback]
    internal void MeleeHitCallback(
      ref AttackCollisionData collisionData,
      Agent attacker,
      Agent victim,
      GameEntity realHitEntity,
      ref float inOutMomentumRemaining,
      ref MeleeCollisionReaction colReaction,
      CrushThroughState crushThroughState,
      Vec3 blowDir,
      Vec3 swingDir,
      ref HitParticleResultData hitParticleResultData,
      bool crushedThroughWithoutAgentCollision)
    {
      hitParticleResultData.Reset();
      bool flag = collisionData.CollisionResult == CombatCollisionResult.Parried || collisionData.CollisionResult == CombatCollisionResult.Blocked || collisionData.CollisionResult == CombatCollisionResult.ChamberBlocked;
      if (collisionData.IsAlternativeAttack && !flag && (victim != null && victim.IsHuman) && (collisionData.CollisionBoneIndex != (sbyte) -1 && (collisionData.VictimHitBodyPart == BoneBodyPartType.BipedalArmLeft || collisionData.VictimHitBodyPart == BoneBodyPartType.BipedalArmRight)))
        colReaction = MeleeCollisionReaction.ContinueChecking;
      if (colReaction == MeleeCollisionReaction.ContinueChecking)
        return;
      bool cancelDamage;
      if (this.CancelsDamageAndBlocksAttackBecauseOfNonEnemyCase(attacker, victim))
      {
        collisionData.AttackerStunPeriod = ManagedParameters.Instance.GetManagedParameter(ManagedParametersEnum.StunPeriodAttackerFriendlyFire);
        cancelDamage = true;
      }
      else
        cancelDamage = flag && !collisionData.AttackBlockedWithShield;
      MissionWeapon attackerWeapon = !collisionData.IsMissile ? (collisionData.AffectorWeaponSlotOrMissileIndex >= 0 ? attacker.Equipment[collisionData.AffectorWeaponSlotOrMissileIndex] : MissionWeapon.Invalid) : this._missiles[collisionData.AffectorWeaponSlotOrMissileIndex].Weapon;
      if (crushThroughState == CrushThroughState.CrushedThisFrame && !collisionData.IsAlternativeAttack)
        this.UpdateMomentumRemaining(ref inOutMomentumRemaining, new Blow(), in collisionData, attacker, victim, in attackerWeapon, true);
      WeaponComponentData shieldOnBack;
      CombatLogData combatLog;
      this.GetAttackCollisionResults(attacker, victim, realHitEntity, inOutMomentumRemaining, ref collisionData, in attackerWeapon, (uint) crushThroughState > 0U, cancelDamage, crushedThroughWithoutAgentCollision, out shieldOnBack, out combatLog);
      if (!collisionData.IsAlternativeAttack && attacker.IsDoingPassiveAttack && (!cancelDamage && !MBNetwork.IsSessionActive) && (double) ManagedOptions.GetConfig(ManagedOptions.ManagedOptionsType.ReportDamage) > 0.0)
      {
        if (attacker.HasMount)
        {
          if (attacker.IsMainAgent)
            InformationManager.DisplayMessage(new InformationMessage(GameTexts.FindText("ui_delivered_couched_lance_damage").ToString(), Color.ConvertStringToColor("#AE4AD9FF")));
          else if (victim != null && victim.IsMainAgent)
            InformationManager.DisplayMessage(new InformationMessage(GameTexts.FindText("ui_received_couched_lance_damage").ToString(), Color.ConvertStringToColor("#D65252FF")));
        }
        else if (attacker.IsMainAgent)
          InformationManager.DisplayMessage(new InformationMessage(GameTexts.FindText("ui_delivered_braced_polearm_damage").ToString(), Color.ConvertStringToColor("#AE4AD9FF")));
        else if (victim != null && victim.IsMainAgent)
          InformationManager.DisplayMessage(new InformationMessage(GameTexts.FindText("ui_received_braced_polearm_damage").ToString(), Color.ConvertStringToColor("#D65252FF")));
      }
      if (collisionData.CollidedWithShieldOnBack && shieldOnBack != null && (victim != null && victim.IsMainAgent))
        InformationManager.DisplayMessage(new InformationMessage(GameTexts.FindText("ui_hit_shield_on_back").ToString(), Color.ConvertStringToColor("#FFFFFFFF")));
      if (!crushedThroughWithoutAgentCollision)
      {
        Blow meleeBlow = this.CreateMeleeBlow(attacker, victim, in collisionData, in attackerWeapon, crushThroughState, blowDir, swingDir, cancelDamage);
        if (!flag && (victim != null && victim.IsActive() || (NativeObject) realHitEntity != (NativeObject) null))
          this.RegisterBlow(attacker, victim, realHitEntity, meleeBlow, ref collisionData, in attackerWeapon, ref combatLog);
        this.UpdateMomentumRemaining(ref inOutMomentumRemaining, meleeBlow, in collisionData, attacker, victim, in attackerWeapon, false);
        bool isFatalHit = victim != null && (double) victim.Health <= 0.0;
        bool isShruggedOff = (uint) (meleeBlow.BlowFlag & BlowFlags.ShrugOff) > 0U;
        this.DecideAgentHitParticles(meleeBlow, victim, ref collisionData, ref hitParticleResultData);
        this.DecideWeaponCollisionReaction(meleeBlow, in collisionData, attacker, victim, in attackerWeapon, isFatalHit, isShruggedOff, out colReaction);
      }
      else
        colReaction = MeleeCollisionReaction.ContinueChecking;
    }

    private bool HitWithAnotherBone(
      in AttackCollisionData collisionData,
      Agent attacker,
      in MissionWeapon attackerWeapon)
    {
      int weaponAttachBoneIndex = attackerWeapon.IsEmpty || attacker == null || !attacker.IsHuman ? -1 : (int) attacker.Monster.GetBoneToAttachForItemFlags(attackerWeapon.Item.ItemFlags);
      return Mission.HitWithAnotherBone(in collisionData, weaponAttachBoneIndex);
    }

    private static bool HitWithAnotherBone(
      in AttackCollisionData collisionData,
      int weaponAttachBoneIndex)
    {
      return collisionData.AttackBoneIndex != (sbyte) -1 && weaponAttachBoneIndex != -1 && weaponAttachBoneIndex != (int) collisionData.AttackBoneIndex;
    }

    private void DecideAgentHitParticles(
      Blow blow,
      Agent victim,
      ref AttackCollisionData collisionData,
      ref HitParticleResultData hprd)
    {
      if (victim == null || blow.InflictedDamage <= 0 && (double) victim.Health > 0.0)
        return;
      if (!blow.WeaponRecord.HasWeapon() || blow.WeaponRecord.WeaponFlags.HasAnyFlag<WeaponFlags>(WeaponFlags.NoBlood) || collisionData.IsAlternativeAttack)
      {
        hprd.StartHitParticleIndex = ParticleSystemManager.GetRuntimeIdByName("psys_game_sweat_sword_enter");
        hprd.ContinueHitParticleIndex = ParticleSystemManager.GetRuntimeIdByName("psys_game_sweat_sword_enter");
        hprd.EndHitParticleIndex = ParticleSystemManager.GetRuntimeIdByName("psys_game_sweat_sword_enter");
      }
      else
      {
        hprd.StartHitParticleIndex = ParticleSystemManager.GetRuntimeIdByName("psys_game_blood_sword_enter");
        hprd.ContinueHitParticleIndex = ParticleSystemManager.GetRuntimeIdByName("psys_game_blood_sword_inside");
        hprd.EndHitParticleIndex = ParticleSystemManager.GetRuntimeIdByName("psys_game_blood_sword_exit");
      }
    }

    private void DecideWeaponCollisionReaction(
      Blow registeredBlow,
      in AttackCollisionData collisionData,
      Agent attacker,
      Agent defender,
      in MissionWeapon attackerWeapon,
      bool isFatalHit,
      bool isShruggedOff,
      out MeleeCollisionReaction colReaction)
    {
      if (collisionData.IsColliderAgent && collisionData.StrikeType == 1 && collisionData.CollisionHitResultFlags.HasAnyFlag<CombatHitResultFlags>(CombatHitResultFlags.HitWithStartOfTheAnimation))
        colReaction = MeleeCollisionReaction.Staggered;
      else if (!collisionData.IsColliderAgent && collisionData.PhysicsMaterialIndex != -1 && PhysicsMaterial.GetFromIndex(collisionData.PhysicsMaterialIndex).GetFlags().HasAnyFlag<PhysicsMaterialFlags>(PhysicsMaterialFlags.AttacksCanPassThrough))
        colReaction = MeleeCollisionReaction.SlicedThrough;
      else if (!collisionData.IsColliderAgent || registeredBlow.InflictedDamage <= 0)
        colReaction = MeleeCollisionReaction.Bounced;
      else if (collisionData.StrikeType == 1 && attacker.IsDoingPassiveAttack)
        colReaction = MissionGameModels.Current.AgentApplyDamageModel.DecidePassiveAttackCollisionReaction(attacker, defender, isFatalHit);
      else if (this.HitWithAnotherBone(in collisionData, attacker, in attackerWeapon))
      {
        colReaction = MeleeCollisionReaction.Bounced;
      }
      else
      {
        WeaponClass weaponClass = !attackerWeapon.IsEmpty ? attackerWeapon.CurrentUsageItem.WeaponClass : WeaponClass.Undefined;
        int num1 = attackerWeapon.IsEmpty ? 0 : (!isFatalHit ? 1 : 0);
        int num2 = isShruggedOff ? 1 : 0;
        colReaction = (num1 & num2) != 0 || attackerWeapon.IsEmpty && defender != null && defender.IsHuman && !collisionData.IsAlternativeAttack && (collisionData.VictimHitBodyPart == BoneBodyPartType.Chest || collisionData.VictimHitBodyPart == BoneBodyPartType.ShoulderLeft || (collisionData.VictimHitBodyPart == BoneBodyPartType.ShoulderRight || collisionData.VictimHitBodyPart == BoneBodyPartType.Abdomen) || collisionData.VictimHitBodyPart == BoneBodyPartType.BipedalLegs) ? MeleeCollisionReaction.Bounced : ((weaponClass == WeaponClass.OneHandedAxe || weaponClass == WeaponClass.TwoHandedAxe) && (!isFatalHit && (double) collisionData.InflictedDamage < (double) defender.HealthLimit * 0.5) || attackerWeapon.IsEmpty && !collisionData.IsAlternativeAttack && collisionData.AttackDirection == Agent.UsageDirection.AttackUp || collisionData.ThrustTipHit && (sbyte) collisionData.DamageType == (sbyte) 1 && (!attackerWeapon.IsEmpty && defender.CanThrustAttackStickToBone(collisionData.CollisionBoneIndex)) ? MeleeCollisionReaction.Stuck : MeleeCollisionReaction.SlicedThrough);
        if (!collisionData.AttackBlockedWithShield && !collisionData.CollidedWithShieldOnBack || colReaction != MeleeCollisionReaction.SlicedThrough)
          return;
        colReaction = MeleeCollisionReaction.Bounced;
      }
    }

    private void RegisterBlow(
      Agent attacker,
      Agent victim,
      GameEntity realHitEntity,
      Blow b,
      ref AttackCollisionData collisionData,
      in MissionWeapon attackerWeapon,
      ref CombatLogData combatLogData)
    {
      b.VictimBodyPart = collisionData.VictimHitBodyPart;
      if (!collisionData.AttackBlockedWithShield)
      {
        Blow blowAsReflection = attacker.CreateBlowFromBlowAsReflection(b);
        if (collisionData.IsColliderAgent)
        {
          if (b.SelfInflictedDamage > 0 && attacker != null && attacker.IsFriendOf(victim))
          {
            if (victim.IsMount && attacker.MountAgent != null)
              attacker.MountAgent.RegisterBlow(blowAsReflection);
            else
              attacker.RegisterBlow(blowAsReflection);
          }
          if (b.InflictedDamage > 0)
          {
            combatLogData.IsFatalDamage = victim != null && (double) victim.Health - (double) b.InflictedDamage < 1.0;
            combatLogData.InflictedDamage = b.InflictedDamage - combatLogData.ModifiedDamage;
            this.PrintAttackCollisionResults(attacker, victim, realHitEntity, ref collisionData, ref combatLogData);
          }
          victim.RegisterBlow(b);
        }
        else if (collisionData.EntityExists)
        {
          MissionWeapon weapon = b.IsMissile ? this._missiles[b.WeaponRecord.AffectorWeaponSlotOrMissileIndex].Weapon : (b.WeaponRecord.HasWeapon() ? attacker.Equipment[b.WeaponRecord.AffectorWeaponSlotOrMissileIndex] : MissionWeapon.Invalid);
          this.OnEntityHit(realHitEntity, attacker, b.InflictedDamage, (DamageTypes) collisionData.DamageType, b.Position, b.SwingDirection, in weapon);
          if (b.SelfInflictedDamage > 0)
            attacker.RegisterBlow(blowAsReflection);
        }
      }
      foreach (MissionBehaviour missionBehaviour in this.MissionBehaviours)
        missionBehaviour.OnRegisterBlow(attacker, victim, realHitEntity, b, ref collisionData, in attackerWeapon);
    }

    private void UpdateMomentumRemaining(
      ref float momentumRemaining,
      Blow b,
      in AttackCollisionData collisionData,
      Agent attacker,
      Agent victim,
      in MissionWeapon attackerWeapon,
      bool isCrushThrough)
    {
      float num = momentumRemaining;
      momentumRemaining = 0.0f;
      if (isCrushThrough)
      {
        momentumRemaining = num * 0.3f;
      }
      else
      {
        if (b.InflictedDamage <= 0 || collisionData.AttackBlockedWithShield || (collisionData.CollidedWithShieldOnBack || !collisionData.IsColliderAgent) || collisionData.IsHorseCharge)
          return;
        if (attacker != null && attacker.IsDoingPassiveAttack)
        {
          momentumRemaining = num * 0.5f;
        }
        else
        {
          if (this.HitWithAnotherBone(in collisionData, attacker, in attackerWeapon) || (attackerWeapon.IsEmpty || b.StrikeType == StrikeType.Thrust) || (attackerWeapon.IsEmpty || !attackerWeapon.CurrentUsageItem.CanHitMultipleTargets))
            return;
          momentumRemaining = num * (float) (1.0 - (double) b.AbsorbedByArmor / (double) b.InflictedDamage);
          momentumRemaining *= 0.5f;
          if ((double) momentumRemaining >= 0.25)
            return;
          momentumRemaining = 0.0f;
        }
      }
    }

    private Blow CreateMissileBlow(
      Agent attackerAgent,
      in AttackCollisionData collisionData,
      in MissionWeapon attackerWeapon,
      Vec3 missilePosition,
      Vec3 missileStartingPosition)
    {
      Blow blow = new Blow(attackerAgent.Index)
      {
        BlowFlag = attackerWeapon.CurrentUsageItem.WeaponFlags.HasAnyFlag<WeaponFlags>(WeaponFlags.CanKnockDown) ? BlowFlags.KnockDown : BlowFlags.None,
        Direction = collisionData.MissileVelocity.NormalizedCopy()
      };
      blow.SwingDirection = blow.Direction;
      ref Blow local1 = ref blow;
      AttackCollisionData attackCollisionData1 = collisionData;
      Vec3 collisionGlobalPosition = attackCollisionData1.CollisionGlobalPosition;
      local1.Position = collisionGlobalPosition;
      ref Blow local2 = ref blow;
      attackCollisionData1 = collisionData;
      int collisionBoneIndex = (int) attackCollisionData1.CollisionBoneIndex;
      local2.BoneIndex = (sbyte) collisionBoneIndex;
      ref Blow local3 = ref blow;
      AttackCollisionData attackCollisionData2 = collisionData;
      int strikeType = attackCollisionData2.StrikeType;
      local3.StrikeType = (StrikeType) strikeType;
      ref Blow local4 = ref blow;
      attackCollisionData2 = collisionData;
      int damageType = (int) (sbyte) attackCollisionData2.DamageType;
      local4.DamageType = (DamageTypes) damageType;
      blow.VictimBodyPart = collisionData.VictimHitBodyPart;
      ref BlowWeaponRecord local5 = ref blow.WeaponRecord;
      ItemObject itemObject = attackerWeapon.Item;
      MissionWeapon missionWeapon = attackerWeapon;
      WeaponComponentData currentUsageItem = missionWeapon.CurrentUsageItem;
      int slotOrMissileIndex = collisionData.AffectorWeaponSlotOrMissileIndex;
      Monster monster = attackerAgent.Monster;
      missionWeapon = attackerWeapon;
      int itemFlags = (int) missionWeapon.Item.ItemFlags;
      int attachForItemFlags = (int) monster.GetBoneToAttachForItemFlags((ItemFlags) itemFlags);
      Vec3 startingPosition = missileStartingPosition;
      Vec3 currentPosition = missilePosition;
      Vec3 missileVelocity = collisionData.MissileVelocity;
      local5.FillAsMissileBlow(itemObject, currentUsageItem, slotOrMissileIndex, (sbyte) attachForItemFlags, startingPosition, currentPosition, missileVelocity);
      blow.BaseMagnitude = collisionData.BaseMagnitude;
      blow.MovementSpeedDamageModifier = collisionData.MovementSpeedDamageModifier;
      blow.AbsorbedByArmor = (float) collisionData.AbsorbedByArmor;
      blow.InflictedDamage = collisionData.InflictedDamage;
      blow.SelfInflictedDamage = collisionData.SelfInflictedDamage;
      blow.DamageCalculated = true;
      return blow;
    }

    private Blow CreateMeleeBlow(
      Agent attackerAgent,
      Agent victimAgent,
      in AttackCollisionData collisionData,
      in MissionWeapon attackerWeapon,
      CrushThroughState crushThroughState,
      Vec3 blowDirection,
      Vec3 swingDirection,
      bool cancelDamage)
    {
      Blow blow = new Blow(attackerAgent.Index);
      blow.VictimBodyPart = collisionData.VictimHitBodyPart;
      if (collisionData.AttackBlockedWithShield)
        return blow;
      bool flag1 = this.HitWithAnotherBone(in collisionData, attackerAgent, in attackerWeapon);
      MissionWeapon missionWeapon;
      if (collisionData.IsAlternativeAttack)
      {
        ref Blow local = ref blow;
        missionWeapon = attackerWeapon;
        int num = missionWeapon.IsEmpty ? 1 : 2;
        local.AttackType = (AgentAttackType) num;
      }
      else
        blow.AttackType = AgentAttackType.Standard;
      missionWeapon = attackerWeapon;
      int num1;
      if (!missionWeapon.IsEmpty)
      {
        Monster monster = attackerAgent.Monster;
        missionWeapon = attackerWeapon;
        int itemFlags = (int) missionWeapon.Item.ItemFlags;
        num1 = (int) monster.GetBoneToAttachForItemFlags((ItemFlags) itemFlags);
      }
      else
        num1 = -1;
      sbyte num2 = (sbyte) num1;
      ref BlowWeaponRecord local1 = ref blow.WeaponRecord;
      missionWeapon = attackerWeapon;
      ItemObject itemObject = missionWeapon.Item;
      missionWeapon = attackerWeapon;
      WeaponComponentData currentUsageItem1 = missionWeapon.CurrentUsageItem;
      AttackCollisionData attackCollisionData = collisionData;
      int slotOrMissileIndex = attackCollisionData.AffectorWeaponSlotOrMissileIndex;
      int num3 = (int) num2;
      local1.FillAsMeleeBlow(itemObject, currentUsageItem1, slotOrMissileIndex, (sbyte) num3);
      ref Blow local2 = ref blow;
      attackCollisionData = collisionData;
      int strikeType = attackCollisionData.StrikeType;
      local2.StrikeType = (StrikeType) strikeType;
      ref Blow local3 = ref blow;
      missionWeapon = attackerWeapon;
      int num4;
      if (!missionWeapon.IsEmpty && !flag1)
      {
        attackCollisionData = collisionData;
        if (!attackCollisionData.IsAlternativeAttack)
        {
          attackCollisionData = collisionData;
          num4 = (int) (sbyte) attackCollisionData.DamageType;
          goto label_12;
        }
      }
      num4 = 2;
label_12:
      local3.DamageType = (DamageTypes) num4;
      ref Blow local4 = ref blow;
      attackCollisionData = collisionData;
      int num5 = attackCollisionData.IsAlternativeAttack ? 1 : 0;
      local4.NoIgnore = num5 != 0;
      ref Blow local5 = ref blow;
      attackCollisionData = collisionData;
      double attackerStunPeriod = (double) attackCollisionData.AttackerStunPeriod;
      local5.AttackerStunPeriod = (float) attackerStunPeriod;
      ref Blow local6 = ref blow;
      attackCollisionData = collisionData;
      double defenderStunPeriod = (double) attackCollisionData.DefenderStunPeriod;
      local6.DefenderStunPeriod = (float) defenderStunPeriod;
      blow.BlowFlag = BlowFlags.None;
      ref Blow local7 = ref blow;
      attackCollisionData = collisionData;
      Vec3 collisionGlobalPosition = attackCollisionData.CollisionGlobalPosition;
      local7.Position = collisionGlobalPosition;
      ref Blow local8 = ref blow;
      attackCollisionData = collisionData;
      int collisionBoneIndex = (int) attackCollisionData.CollisionBoneIndex;
      local8.BoneIndex = (sbyte) collisionBoneIndex;
      blow.Direction = blowDirection;
      blow.SwingDirection = swingDirection;
      if (cancelDamage)
      {
        blow.BaseMagnitude = 0.0f;
        blow.MovementSpeedDamageModifier = 0.0f;
        blow.InflictedDamage = 0;
        blow.SelfInflictedDamage = 0;
        blow.AbsorbedByArmor = 0.0f;
      }
      else
      {
        blow.BaseMagnitude = collisionData.BaseMagnitude;
        blow.MovementSpeedDamageModifier = collisionData.MovementSpeedDamageModifier;
        blow.InflictedDamage = collisionData.InflictedDamage;
        blow.SelfInflictedDamage = collisionData.SelfInflictedDamage;
        blow.AbsorbedByArmor = (float) collisionData.AbsorbedByArmor;
      }
      blow.DamageCalculated = true;
      if (crushThroughState != CrushThroughState.None)
        blow.BlowFlag |= BlowFlags.CrushThrough;
      if (blow.StrikeType == StrikeType.Thrust)
      {
        attackCollisionData = collisionData;
        if (!attackCollisionData.ThrustTipHit)
          blow.BlowFlag |= BlowFlags.NonTipThrust;
      }
      attackCollisionData = collisionData;
      if (attackCollisionData.IsColliderAgent)
      {
        bool flag2 = Mission.DecideAgentShrugOffBlow(victimAgent, collisionData, ref blow);
        if (victimAgent.IsHuman)
        {
          if (victimAgent.HasMount)
          {
            Agent attackerAgent1 = attackerAgent;
            Agent victimAgent1 = victimAgent;
            ref readonly AttackCollisionData local9 = ref collisionData;
            missionWeapon = attackerWeapon;
            WeaponComponentData currentUsageItem2 = missionWeapon.CurrentUsageItem;
            int num6 = flag2 ? 1 : 0;
            ref Blow local10 = ref blow;
            Mission.DecideAgentDismountedByBlow(attackerAgent1, victimAgent1, in local9, currentUsageItem2, num6 != 0, ref local10);
          }
          else
          {
            Agent attacker = attackerAgent;
            Agent victim = victimAgent;
            ref readonly AttackCollisionData local9 = ref collisionData;
            missionWeapon = attackerWeapon;
            WeaponComponentData currentUsageItem2 = missionWeapon.CurrentUsageItem;
            int num6 = flag2 ? 1 : 0;
            ref Blow local10 = ref blow;
            Mission.DecideAgentKnockedByBlow(attacker, victim, in local9, currentUsageItem2, num6 != 0, ref local10);
          }
        }
        else if (victimAgent.IsMount)
        {
          float combatDifficulty = this.GetDamageMultiplierOfCombatDifficulty(victimAgent);
          Agent attackerAgent1 = attackerAgent;
          Agent victimAgent1 = victimAgent;
          ref readonly AttackCollisionData local9 = ref collisionData;
          missionWeapon = attackerWeapon;
          WeaponComponentData currentUsageItem2 = missionWeapon.CurrentUsageItem;
          double num6 = (double) combatDifficulty;
          Vec3 blowDirection1 = blowDirection;
          ref Blow local10 = ref blow;
          Mission.DecideMountRearedByBlow(attackerAgent1, victimAgent1, in local9, currentUsageItem2, (float) num6, blowDirection1, ref local10);
        }
      }
      return blow;
    }

    [UsedImplicitly]
    [MBCallback]
    internal void MissileAreaDamageCallback(
      ref AttackCollisionData collisionDataInput,
      ref Blow blowInput,
      Agent alreadyDamagedAgent,
      Agent shooterAgent,
      bool isBigExplosion)
    {
      float range = isBigExplosion ? 2f : 1f;
      float num1 = isBigExplosion ? 1f : 0.5f;
      AttackCollisionData attackCollisionData1 = collisionDataInput;
      blowInput.VictimBodyPart = collisionDataInput.VictimHitBodyPart;
      Blow blow = blowInput;
      foreach (Agent agent in this.GetAgentsInRange(blowInput.Position.AsVec2, range, true).ToList<Agent>())
      {
        Vec3 position = agent.Position;
        if (agent != shooterAgent && agent.State == AgentState.Active && agent != alreadyDamagedAgent)
        {
          Blow b = blowInput;
          AttackCollisionData attackCollisionData2 = collisionDataInput;
          b.DamageCalculated = false;
          float num2 = float.MaxValue;
          int num3 = -1;
          Skeleton skeleton = agent.AgentVisuals.GetSkeleton();
          int boneCount = (int) skeleton.GetBoneCount();
          for (int boneIndex = 0; boneIndex < boneCount; ++boneIndex)
          {
            float num4 = agent.AgentVisuals.GetGlobalFrame().TransformToParent(skeleton.GetBoneEntitialFrame(boneIndex).origin).DistanceSquared(blowInput.Position);
            if ((double) num4 < (double) num2)
            {
              num3 = boneIndex;
              num2 = num4;
            }
          }
          if ((double) num2 <= (double) range * (double) range)
          {
            float num4 = (float) Math.Sqrt((double) num2);
            float num5 = 1f;
            if ((double) num4 > (double) num1)
            {
              float num6 = MBMath.Lerp(1f, 3f, (float) (((double) num4 - (double) num1) / ((double) range - (double) num1)));
              num5 = (float) (1.0 / ((double) num6 * (double) num6));
            }
            attackCollisionData2.SetCollisionBoneIndexForAreaDamage((sbyte) num3);
            MissionWeapon attackerWeapon = this._missiles[attackCollisionData2.AffectorWeaponSlotOrMissileIndex].Weapon;
            CombatLogData combatLog;
            this.GetAttackCollisionResults(shooterAgent, agent, (GameEntity) null, 1f, ref attackCollisionData2, in attackerWeapon, false, false, false, out WeaponComponentData _, out combatLog);
            b.BaseMagnitude = attackCollisionData2.BaseMagnitude;
            b.MovementSpeedDamageModifier = attackCollisionData2.MovementSpeedDamageModifier;
            b.InflictedDamage = attackCollisionData2.InflictedDamage;
            b.SelfInflictedDamage = attackCollisionData2.SelfInflictedDamage;
            b.AbsorbedByArmor = (float) attackCollisionData2.AbsorbedByArmor;
            b.DamageCalculated = true;
            b.InflictedDamage = (int) Math.Round((double) b.InflictedDamage * (double) num5);
            b.SelfInflictedDamage = (int) Math.Round((double) b.SelfInflictedDamage * (double) num5);
            this.RegisterBlow(shooterAgent, agent, (GameEntity) null, b, ref attackCollisionData2, in attackerWeapon, ref combatLog);
          }
        }
      }
    }

    [UsedImplicitly]
    [MBCallback]
    internal void OnMissileRemoved(int missileIndex) => this._missiles.Remove(missileIndex);

    [UsedImplicitly]
    [MBCallback]
    internal bool MissileHitCallback(
      out int hitParticleIndex,
      ref AttackCollisionData collisionData,
      Vec3 missileStartingPosition,
      Vec3 missilePosition,
      Vec3 missileAngularVelocity,
      Vec3 movementVelocity,
      MatrixFrame attachGlobalFrame,
      MatrixFrame affectedShieldGlobalFrame,
      int numDamagedAgents,
      Agent attacker,
      Agent victim,
      GameEntity hitEntity)
    {
      Mission.Missile missile = this._missiles[collisionData.AffectorWeaponSlotOrMissileIndex];
      MissionWeapon attackerWeapon = missile.Weapon;
      WeaponFlags weaponFlags1 = attackerWeapon.CurrentUsageItem.WeaponFlags;
      float momentumRemaining = 1f;
      WeaponComponentData shieldOnBack = (WeaponComponentData) null;
      MissionGameModels.Current.AgentApplyDamageModel.DecideMissileWeaponFlags(attacker, missile.Weapon, ref weaponFlags1);
      CombatLogData combatLog;
      MissionWeapon weapon;
      if (collisionData.AttackBlockedWithShield && weaponFlags1.HasAnyFlag<WeaponFlags>(WeaponFlags.CanPenetrateShield))
      {
        this.GetAttackCollisionResults(attacker, victim, hitEntity, momentumRemaining, ref collisionData, in attackerWeapon, false, false, false, out shieldOnBack, out combatLog);
        EquipmentIndex wieldedItemIndex = victim.GetWieldedItemIndex(Agent.HandIndex.OffHand);
        double inflictedDamage = (double) collisionData.InflictedDamage;
        double managedParameter1 = (double) ManagedParameters.Instance.GetManagedParameter(ManagedParametersEnum.ShieldPenetrationOffset);
        double managedParameter2 = (double) ManagedParameters.Instance.GetManagedParameter(ManagedParametersEnum.ShieldPenetrationFactor);
        weapon = victim.Equipment[wieldedItemIndex];
        double armorForCurrentUsage = (double) weapon.GetGetModifiedArmorForCurrentUsage();
        double num1 = managedParameter2 * armorForCurrentUsage;
        double num2 = managedParameter1 + num1;
        if (inflictedDamage > num2)
        {
          AttackCollisionData.UpdateDataForShieldPenetration(ref collisionData);
          momentumRemaining *= (float) (0.400000005960464 + (double) MBRandom.RandomFloat * 0.200000002980232);
        }
      }
      hitParticleIndex = -1;
      bool flag1 = !GameNetwork.IsSessionActive;
      bool missileHasPhysics = collisionData.MissileHasPhysics;
      PhysicsMaterial fromIndex = PhysicsMaterial.GetFromIndex(collisionData.PhysicsMaterialIndex);
      int num3 = fromIndex.IsValid ? (int) fromIndex.GetFlags() : 0;
      bool flag2 = (weaponFlags1 & WeaponFlags.AmmoSticksWhenShot) > (WeaponFlags) 0;
      bool flag3 = (num3 & 1) == 0;
      bool flag4 = (uint) (num3 & 8) > 0U;
      MissionObject attachedMissionObject = (MissionObject) null;
      if (victim == null && (NativeObject) hitEntity != (NativeObject) null)
      {
        GameEntity gameEntity = hitEntity;
        do
        {
          attachedMissionObject = gameEntity.GetFirstScriptOfType<MissionObject>();
          gameEntity = gameEntity.Parent;
        }
        while (attachedMissionObject == null && (NativeObject) gameEntity != (NativeObject) null);
        hitEntity = attachedMissionObject?.GameEntity;
      }
      Mission.MissileCollisionReaction collisionReaction1 = !flag4 ? (!weaponFlags1.HasAnyFlag<WeaponFlags>(WeaponFlags.Burning) ? (!flag3 || !flag2 ? Mission.MissileCollisionReaction.BounceBack : Mission.MissileCollisionReaction.Stick) : Mission.MissileCollisionReaction.BecomeInvisible) : Mission.MissileCollisionReaction.PassThrough;
      bool isCanceled = false;
      Mission.MissileCollisionReaction collisionReaction2;
      if (collisionData.MissileGoneUnderWater)
      {
        collisionReaction2 = Mission.MissileCollisionReaction.BecomeInvisible;
        hitParticleIndex = 0;
      }
      else if (victim == null)
      {
        if ((NativeObject) hitEntity != (NativeObject) null)
        {
          this.GetAttackCollisionResults(attacker, victim, hitEntity, momentumRemaining, ref collisionData, in attackerWeapon, false, false, false, out shieldOnBack, out combatLog);
          Blow missileBlow = this.CreateMissileBlow(attacker, in collisionData, in attackerWeapon, missilePosition, missileStartingPosition);
          this.RegisterBlow(attacker, (Agent) null, hitEntity, missileBlow, ref collisionData, in attackerWeapon, ref combatLog);
        }
        collisionReaction2 = collisionReaction1;
        hitParticleIndex = 0;
      }
      else if (collisionData.AttackBlockedWithShield)
      {
        this.GetAttackCollisionResults(attacker, victim, hitEntity, momentumRemaining, ref collisionData, in attackerWeapon, false, false, false, out shieldOnBack, out combatLog);
        collisionReaction2 = collisionData.IsShieldBroken ? Mission.MissileCollisionReaction.BecomeInvisible : collisionReaction1;
        hitParticleIndex = 0;
      }
      else if (collisionData.MissileBlockedWithWeapon)
      {
        this.GetAttackCollisionResults(attacker, victim, hitEntity, momentumRemaining, ref collisionData, in attackerWeapon, false, false, false, out shieldOnBack, out combatLog);
        collisionReaction2 = Mission.MissileCollisionReaction.BounceBack;
        hitParticleIndex = 0;
      }
      else
      {
        if (attacker != null && attacker.IsFriendOf(victim))
        {
          if (!missileHasPhysics)
          {
            if (flag1)
            {
              if (attacker.Controller == Agent.ControllerType.AI)
                isCanceled = true;
            }
            else if (MultiplayerOptions.OptionType.FriendlyFireDamageRangedFriendPercent.GetIntValue() <= 0 && MultiplayerOptions.OptionType.FriendlyFireDamageRangedSelfPercent.GetIntValue() <= 0 || this.Mode == MissionMode.Duel)
              isCanceled = true;
          }
        }
        else if (victim.IsHuman && !attacker.IsEnemyOf(victim))
          isCanceled = true;
        else if (flag1 && attacker != null && (attacker.Controller == Agent.ControllerType.AI && victim.RiderAgent != null) && attacker.IsFriendOf(victim.RiderAgent))
          isCanceled = true;
        if (isCanceled)
        {
          if (flag1 && attacker == Agent.Main && attacker.IsFriendOf(victim))
            InformationManager.DisplayMessage(new InformationMessage(GameTexts.FindText("ui_you_hit_a_friendly_troop").ToString(), Color.ConvertStringToColor("#D65252FF")));
          collisionReaction2 = Mission.MissileCollisionReaction.BecomeInvisible;
        }
        else
        {
          bool flag5 = (weaponFlags1 & WeaponFlags.MultiplePenetration) > (WeaponFlags) 0;
          this.GetAttackCollisionResults(attacker, victim, (GameEntity) null, momentumRemaining, ref collisionData, in attackerWeapon, false, false, false, out shieldOnBack, out combatLog);
          Blow missileBlow = this.CreateMissileBlow(attacker, in collisionData, in attackerWeapon, missilePosition, missileStartingPosition);
          if (((!collisionData.IsColliderAgent ? 0 : (!collisionData.CollidedWithShieldOnBack ? 1 : 0)) & (flag5 ? 1 : 0)) != 0 && numDamagedAgents > 0)
          {
            missileBlow.InflictedDamage /= numDamagedAgents;
            missileBlow.SelfInflictedDamage /= numDamagedAgents;
          }
          if (collisionData.IsColliderAgent)
          {
            bool isInitialBlowShrugOff = Mission.DecideAgentShrugOffBlow(victim, collisionData, ref missileBlow);
            if (victim.IsHuman)
            {
              if (victim.HasMount)
                Mission.DecideAgentDismountedByBlow(attacker, victim, in collisionData, attackerWeapon.CurrentUsageItem, isInitialBlowShrugOff, ref missileBlow);
              else
                Mission.DecideAgentKnockedByBlow(attacker, victim, in collisionData, attackerWeapon.CurrentUsageItem, isInitialBlowShrugOff, ref missileBlow);
            }
          }
          if (victim.State == AgentState.Active)
            this.RegisterBlow(attacker, victim, (GameEntity) null, missileBlow, ref collisionData, in attackerWeapon, ref combatLog);
          hitParticleIndex = ParticleSystemManager.GetRuntimeIdByName("psys_game_blood_sword_enter");
          if (flag5 && numDamagedAgents < 3)
          {
            collisionReaction2 = Mission.MissileCollisionReaction.PassThrough;
          }
          else
          {
            collisionReaction2 = collisionReaction1;
            if (collisionReaction1 == Mission.MissileCollisionReaction.Stick && !collisionData.CollidedWithShieldOnBack)
            {
              bool flag6 = this.CombatType == Mission.MissionCombatType.Combat;
              if (flag6)
              {
                bool flag7 = victim.IsHuman && collisionData.VictimHitBodyPart == BoneBodyPartType.Head;
                flag6 = victim.State != AgentState.Active || !flag7;
              }
              if (flag6)
              {
                float managedParameter = ManagedParameters.Instance.GetManagedParameter(ManagedParametersEnum.MissileMinimumDamageToStick);
                float num1 = 2f * managedParameter;
                if (!GameNetwork.IsClientOrReplay && (double) missileBlow.InflictedDamage < (double) managedParameter && (double) missileBlow.AbsorbedByArmor > (double) num1)
                  collisionReaction2 = Mission.MissileCollisionReaction.BounceBack;
              }
              else
                collisionReaction2 = Mission.MissileCollisionReaction.BecomeInvisible;
            }
          }
        }
      }
      if (collisionData.CollidedWithShieldOnBack && shieldOnBack != null && (victim != null && victim.IsMainAgent))
        InformationManager.DisplayMessage(new InformationMessage(GameTexts.FindText("ui_hit_shield_on_back").ToString(), Color.ConvertStringToColor("#FFFFFFFF")));
      MatrixFrame attachLocalFrame;
      if (!collisionData.MissileHasPhysics && !collisionData.MissileGoneUnderWater)
      {
        bool flag5 = collisionReaction2 == Mission.MissileCollisionReaction.Stick;
        ref MatrixFrame local = ref attachGlobalFrame;
        AttackCollisionData collisionData1 = collisionData;
        weapon = missile.Weapon;
        WeaponComponentData currentUsageItem = weapon.CurrentUsageItem;
        Agent affectedAgent = victim;
        GameEntity hitEntity1 = hitEntity;
        Vec3 missileMovementVelocity = movementVelocity;
        Vec3 missileRotationSpeed = missileAngularVelocity;
        MatrixFrame shieldGlobalFrame = affectedShieldGlobalFrame;
        int num1 = flag5 ? 1 : 0;
        attachLocalFrame = this.CalculateAttachedLocalFrame(ref local, collisionData1, currentUsageItem, affectedAgent, hitEntity1, missileMovementVelocity, missileRotationSpeed, shieldGlobalFrame, num1 != 0);
      }
      else
      {
        attachLocalFrame = attachGlobalFrame;
        attachedMissionObject = (MissionObject) null;
      }
      Vec3 velocity = Vec3.Zero;
      Vec3 angularVelocity = Vec3.Zero;
      if (collisionReaction2 == Mission.MissileCollisionReaction.BounceBack)
      {
        WeaponFlags weaponFlags2 = weaponFlags1 & WeaponFlags.AmmoBreakOnBounceBackMask;
        if (weaponFlags2 == WeaponFlags.AmmoCanBreakOnBounceBack && (double) collisionData.MissileVelocity.Length > (double) ManagedParameters.Instance.GetManagedParameter(ManagedParametersEnum.BreakableProjectileMinimumBreakSpeed) || weaponFlags2 == WeaponFlags.AmmoBreaksOnBounceBack)
        {
          collisionReaction2 = Mission.MissileCollisionReaction.BecomeInvisible;
          hitParticleIndex = ParticleSystemManager.GetRuntimeIdByName("psys_game_broken_arrow");
        }
        else
          missile.CalculateBounceBackVelocity(missileAngularVelocity, collisionData, out velocity, out angularVelocity);
      }
      this.HandleMissileCollisionReaction(collisionData.AffectorWeaponSlotOrMissileIndex, collisionReaction2, attachLocalFrame, attacker, victim, collisionData.AttackBlockedWithShield, collisionData.CollisionBoneIndex, attachedMissionObject, velocity, angularVelocity, -1);
      foreach (MissionBehaviour missionBehaviour in this.MissionBehaviours)
        missionBehaviour.OnMissileHit(attacker, victim, isCanceled);
      return collisionReaction2 != Mission.MissileCollisionReaction.PassThrough;
    }

    public void HandleMissileCollisionReaction(
      int missileIndex,
      Mission.MissileCollisionReaction collisionReaction,
      MatrixFrame attachLocalFrame,
      Agent attackerAgent,
      Agent attachedAgent,
      bool attachedToShield,
      sbyte attachedBoneIndex,
      MissionObject attachedMissionObject,
      Vec3 bounceBackVelocity,
      Vec3 bounceBackAngularVelocity,
      int forcedSpawnIndex)
    {
      Mission.Missile missile = this._missiles[missileIndex];
      MissionObjectId missionObjectId = new MissionObjectId(-1, true);
      switch (collisionReaction)
      {
        case Mission.MissileCollisionReaction.Stick:
          missile.Entity.SetVisibilityExcludeParents(true);
          if (attachedAgent != null)
          {
            this.PrepareMissileWeaponForDrop(missileIndex);
            if (attachedToShield)
            {
              EquipmentIndex wieldedItemIndex = attachedAgent.GetWieldedItemIndex(Agent.HandIndex.OffHand);
              attachedAgent.AttachWeaponToWeapon(wieldedItemIndex, missile.Weapon, missile.Entity, ref attachLocalFrame);
              break;
            }
            attachedAgent.AttachWeaponToBone(missile.Weapon, missile.Entity, attachedBoneIndex, ref attachLocalFrame);
            break;
          }
          Vec3 zero = Vec3.Zero;
          missionObjectId = this.SpawnWeaponAsDropFromMissile(missileIndex, attachedMissionObject, ref attachLocalFrame, Mission.WeaponSpawnFlags.AsMissile | Mission.WeaponSpawnFlags.WithStaticPhysics, ref zero, ref zero, forcedSpawnIndex);
          break;
        case Mission.MissileCollisionReaction.BounceBack:
          missile.Entity.SetVisibilityExcludeParents(true);
          missionObjectId = this.SpawnWeaponAsDropFromMissile(missileIndex, (MissionObject) null, ref attachLocalFrame, Mission.WeaponSpawnFlags.AsMissile | Mission.WeaponSpawnFlags.WithPhysics, ref bounceBackVelocity, ref bounceBackAngularVelocity, forcedSpawnIndex);
          break;
        case Mission.MissileCollisionReaction.BecomeInvisible:
          missile.Entity.Remove(81);
          break;
      }
      bool flag = collisionReaction != Mission.MissileCollisionReaction.PassThrough;
      if (GameNetwork.IsServerOrRecorder)
      {
        GameNetwork.BeginBroadcastModuleEvent();
        GameNetwork.WriteMessage((GameNetworkMessage) new NetworkMessages.FromServer.HandleMissileCollisionReaction(missileIndex, collisionReaction, attachLocalFrame, attackerAgent, attachedAgent, attachedToShield, attachedBoneIndex, attachedMissionObject, bounceBackVelocity, bounceBackAngularVelocity, missionObjectId.Id));
        GameNetwork.EndBroadcastModuleEvent(GameNetwork.EventBroadcastFlags.AddToMissionRecord);
      }
      else if (GameNetwork.IsClientOrReplay && flag)
        this.RemoveMissileAsClient(missileIndex);
      foreach (MissionBehaviour missionBehaviour in this.MissionBehaviours)
        missionBehaviour.OnMissileCollisionReaction(collisionReaction, attackerAgent, attachedAgent, attachedBoneIndex);
    }

    [UsedImplicitly]
    [MBCallback]
    internal void ChargeDamageCallback(
      ref AttackCollisionData collisionData,
      Blow blow,
      Agent attacker,
      Agent victim)
    {
      if (attacker.RiderAgent != null && !attacker.IsEnemyOf(victim))
        return;
      WeaponComponentData shieldOnBack;
      CombatLogData combatLog;
      this.GetAttackCollisionResults(attacker, victim, (GameEntity) null, 1f, ref collisionData, in MissionWeapon.Invalid, false, false, false, out shieldOnBack, out combatLog);
      if (collisionData.CollidedWithShieldOnBack && shieldOnBack != null && (victim != null && victim.IsMainAgent))
        InformationManager.DisplayMessage(new InformationMessage(GameTexts.FindText("ui_hit_shield_on_back").ToString(), Color.ConvertStringToColor("#FFFFFFFF")));
      if ((double) collisionData.InflictedDamage <= 0.0)
        return;
      blow.BaseMagnitude = collisionData.BaseMagnitude;
      blow.MovementSpeedDamageModifier = collisionData.MovementSpeedDamageModifier;
      blow.InflictedDamage = collisionData.InflictedDamage;
      blow.SelfInflictedDamage = collisionData.SelfInflictedDamage;
      blow.AbsorbedByArmor = (float) collisionData.AbsorbedByArmor;
      blow.DamageCalculated = true;
      Mission.DecideAgentKnockedByBlow(attacker, victim, in collisionData, (WeaponComponentData) null, false, ref blow);
      Agent attacker1 = attacker;
      Agent victim1 = victim;
      Blow b = blow;
      ref AttackCollisionData local1 = ref collisionData;
      MissionWeapon missionWeapon = new MissionWeapon();
      ref MissionWeapon local2 = ref missionWeapon;
      ref CombatLogData local3 = ref combatLog;
      this.RegisterBlow(attacker1, victim1, (GameEntity) null, b, ref local1, in local2, ref local3);
    }

    [UsedImplicitly]
    [MBCallback]
    internal void FallDamageCallback(
      ref AttackCollisionData collisionData,
      Blow b,
      Agent attacker,
      Agent victim)
    {
      CombatLogData combatLog;
      this.GetAttackCollisionResults(attacker, victim, (GameEntity) null, 1f, ref collisionData, in MissionWeapon.Invalid, false, false, false, out WeaponComponentData _, out combatLog);
      b.BaseMagnitude = collisionData.BaseMagnitude;
      b.MovementSpeedDamageModifier = collisionData.MovementSpeedDamageModifier;
      b.InflictedDamage = collisionData.InflictedDamage;
      b.SelfInflictedDamage = collisionData.SelfInflictedDamage;
      b.AbsorbedByArmor = (float) collisionData.AbsorbedByArmor;
      b.DamageCalculated = true;
      if (b.InflictedDamage <= 0)
        return;
      Agent riderAgent = victim.RiderAgent;
      Agent attacker1 = attacker;
      Agent victim1 = victim;
      Blow b1 = b;
      ref AttackCollisionData local1 = ref collisionData;
      MissionWeapon missionWeapon = new MissionWeapon();
      ref MissionWeapon local2 = ref missionWeapon;
      ref CombatLogData local3 = ref combatLog;
      this.RegisterBlow(attacker1, victim1, (GameEntity) null, b1, ref local1, in local2, ref local3);
      if (riderAgent == null)
        return;
      this.FallDamageCallback(ref collisionData, b, riderAgent, riderAgent);
    }

    public void KillAgentsOnEntity(GameEntity entity, Agent destroyerAgent, bool burnAgents)
    {
      if ((NativeObject) entity == (NativeObject) null)
        return;
      int ownerId = destroyerAgent == null ? -1 : destroyerAgent.Index;
      Vec3 boundingBoxMin = entity.GetBoundingBoxMin();
      Vec3 boundingBoxMax = entity.GetBoundingBoxMax();
      Vec2 vec2 = (boundingBoxMax.AsVec2 + boundingBoxMin.AsVec2) * 0.5f;
      float range = (boundingBoxMax.AsVec2 - boundingBoxMin.AsVec2).Length * 0.5f;
      Blow blow = new Blow(ownerId);
      blow.DamageCalculated = true;
      blow.BaseMagnitude = 2000f;
      blow.InflictedDamage = 2000;
      blow.Direction = new Vec3(z: -1f);
      blow.BoneIndex = (sbyte) 0;
      blow.WeaponRecord.FillAsMeleeBlow((ItemObject) null, (WeaponComponentData) null, -1, (sbyte) 0);
      if (burnAgents)
      {
        blow.WeaponRecord.WeaponFlags |= WeaponFlags.AffectsArea | WeaponFlags.Burning;
        blow.WeaponRecord.CurrentPosition = blow.Position;
        blow.WeaponRecord.StartingPosition = blow.Position;
      }
      Vec2 asVec2 = entity.GetGlobalFrame().TransformToParent(vec2.ToVec3()).AsVec2;
      List<Agent> agentList = new List<Agent>();
      foreach (Agent agent in this.GetAgentsInRange(asVec2, range))
      {
        GameEntity gameEntity = agent.GetSteppedEntity();
        while ((NativeObject) gameEntity != (NativeObject) null && !((NativeObject) gameEntity == (NativeObject) entity))
          gameEntity = gameEntity.Parent;
        if ((NativeObject) gameEntity != (NativeObject) null)
          agentList.Add(agent);
      }
      foreach (Agent agent in agentList)
      {
        blow.Position = agent.Position;
        agent.RegisterBlow(blow);
      }
    }

    public void KillAgentCheat(Agent agent)
    {
      if (GameNetwork.IsClientOrReplay)
        return;
      Blow blow = new Blow(this.MainAgent != null ? this.MainAgent.Index : agent.Index);
      blow.DamageType = DamageTypes.Blunt;
      blow.BoneIndex = agent.Monster.HeadLookDirectionBoneIndex;
      blow.Position = agent.Position;
      blow.Position.z += agent.GetEyeGlobalHeight();
      blow.BaseMagnitude = 2000f;
      blow.WeaponRecord.FillAsMeleeBlow((ItemObject) null, (WeaponComponentData) null, -1, (sbyte) -1);
      blow.InflictedDamage = 2000;
      blow.SwingDirection = agent.LookDirection;
      if (this.InputManager.IsGameKeyDown(2))
      {
        blow.SwingDirection = agent.Frame.rotation.TransformToParent(new Vec3(-1f));
        double num = (double) blow.SwingDirection.Normalize();
      }
      else if (this.InputManager.IsGameKeyDown(3))
      {
        blow.SwingDirection = agent.Frame.rotation.TransformToParent(new Vec3(1f));
        double num = (double) blow.SwingDirection.Normalize();
      }
      else if (this.InputManager.IsGameKeyDown(1))
      {
        blow.SwingDirection = agent.Frame.rotation.TransformToParent(new Vec3(y: -1f));
        double num = (double) blow.SwingDirection.Normalize();
      }
      else if (this.InputManager.IsGameKeyDown(0))
      {
        blow.SwingDirection = agent.Frame.rotation.TransformToParent(new Vec3(y: 1f));
        double num = (double) blow.SwingDirection.Normalize();
      }
      blow.Direction = blow.SwingDirection;
      blow.DamageCalculated = true;
      agent.RegisterBlow(blow);
    }

    public bool KillCheats(bool killAll, bool killEnemy, bool killHorse, bool killYourself)
    {
      bool flag1 = false;
      if (!GameNetwork.IsClientOrReplay)
      {
        if (killYourself)
        {
          if (this.MainAgent != null)
          {
            if (killHorse)
            {
              if (this.MainAgent.MountAgent != null)
              {
                this.KillAgentCheat(this.MainAgent.MountAgent);
                flag1 = true;
              }
            }
            else
            {
              this.KillAgentCheat(this.MainAgent);
              flag1 = true;
            }
          }
        }
        else
        {
          bool flag2 = false;
          for (int index = this.Agents.Count - 1; index >= 0 && !flag2; --index)
          {
            Agent agent = this.Agents[index];
            if (agent != this.MainAgent && agent.GetAgentFlags().HasAnyFlag<AgentFlag>(AgentFlag.CanAttack) && this.PlayerTeam != null)
            {
              if (killEnemy)
              {
                if (agent.Team.IsValid && this.PlayerTeam.IsEnemyOf(agent.Team))
                {
                  if (killHorse && agent.HasMount)
                  {
                    if (agent.MountAgent != null)
                    {
                      this.KillAgentCheat(agent.MountAgent);
                      if (!killAll)
                        flag2 = true;
                      flag1 = true;
                    }
                  }
                  else
                  {
                    this.KillAgentCheat(agent);
                    if (!killAll)
                      flag2 = true;
                    flag1 = true;
                  }
                }
              }
              else if (agent.Team.IsValid && this.PlayerTeam.IsFriendOf(agent.Team))
              {
                if (killHorse)
                {
                  if (agent.MountAgent != null)
                  {
                    this.KillAgentCheat(agent.MountAgent);
                    if (!killAll)
                      flag2 = true;
                    flag1 = true;
                  }
                }
                else
                {
                  this.KillAgentCheat(agent);
                  if (!killAll)
                    flag2 = true;
                  flag1 = true;
                }
              }
            }
          }
        }
      }
      return flag1;
    }

    private bool CancelsDamageAndBlocksAttackBecauseOfNonEnemyCase(Agent attacker, Agent victim)
    {
      if (victim == null || attacker == null)
        return false;
      int num1 = !GameNetwork.IsSessionActive || MultiplayerOptions.OptionType.FriendlyFireDamageMeleeFriendPercent.GetIntValue() <= 0 && MultiplayerOptions.OptionType.FriendlyFireDamageMeleeSelfPercent.GetIntValue() <= 0 || this.Mode == MissionMode.Duel ? 1 : (attacker.Controller == Agent.ControllerType.AI ? 1 : 0);
      bool flag = attacker.IsFriendOf(victim);
      int num2 = flag ? 1 : 0;
      if ((num1 & num2) != 0)
        return true;
      return victim.IsHuman && !flag && !attacker.IsEnemyOf(victim);
    }

    private static float ChargeDamageDotProduct(
      Vec3 victimPosition,
      Vec3 chargerMovementDirection,
      Vec3 collisionPoint)
    {
      Vec2 va = victimPosition.AsVec2 - collisionPoint.AsVec2;
      double num = (double) va.Normalize();
      Vec2 asVec2 = chargerMovementDirection.AsVec2;
      return Vec2.DotProduct(va, asVec2);
    }

    internal static float SpeedGraphFunction(
      float progress,
      StrikeType strikeType,
      Agent.UsageDirection attackDir)
    {
      int num = strikeType == StrikeType.Thrust ? 1 : 0;
      bool flag = attackDir == Agent.UsageDirection.AttackUp;
      ManagedParametersEnum managedParameterEnum1;
      ManagedParametersEnum managedParameterEnum2;
      ManagedParametersEnum managedParameterEnum3;
      ManagedParametersEnum managedParameterEnum4;
      if (num != 0)
      {
        managedParameterEnum1 = ManagedParametersEnum.ThrustCombatSpeedGraphZeroProgressValue;
        managedParameterEnum2 = ManagedParametersEnum.ThrustCombatSpeedGraphFirstMaximumPoint;
        managedParameterEnum3 = ManagedParametersEnum.ThrustCombatSpeedGraphSecondMaximumPoint;
        managedParameterEnum4 = ManagedParametersEnum.ThrustCombatSpeedGraphOneProgressValue;
      }
      else if (flag)
      {
        managedParameterEnum1 = ManagedParametersEnum.OverSwingCombatSpeedGraphZeroProgressValue;
        managedParameterEnum2 = ManagedParametersEnum.OverSwingCombatSpeedGraphFirstMaximumPoint;
        managedParameterEnum3 = ManagedParametersEnum.OverSwingCombatSpeedGraphSecondMaximumPoint;
        managedParameterEnum4 = ManagedParametersEnum.OverSwingCombatSpeedGraphOneProgressValue;
      }
      else
      {
        managedParameterEnum1 = ManagedParametersEnum.SwingCombatSpeedGraphZeroProgressValue;
        managedParameterEnum2 = ManagedParametersEnum.SwingCombatSpeedGraphFirstMaximumPoint;
        managedParameterEnum3 = ManagedParametersEnum.SwingCombatSpeedGraphSecondMaximumPoint;
        managedParameterEnum4 = ManagedParametersEnum.SwingCombatSpeedGraphOneProgressValue;
      }
      float managedParameter1 = ManagedParameters.Instance.GetManagedParameter(managedParameterEnum1);
      float managedParameter2 = ManagedParameters.Instance.GetManagedParameter(managedParameterEnum2);
      float managedParameter3 = ManagedParameters.Instance.GetManagedParameter(managedParameterEnum3);
      float managedParameter4 = ManagedParameters.Instance.GetManagedParameter(managedParameterEnum4);
      return (double) progress >= (double) managedParameter2 ? ((double) managedParameter3 >= (double) progress ? 1f : (float) (((double) managedParameter4 - 1.0) / (1.0 - (double) managedParameter3) * ((double) progress - (double) managedParameter3) + 1.0)) : (1f - managedParameter1) / managedParameter2 * progress + managedParameter1;
    }

    public static void ComputeBlowMagnitude(
      ref AttackCollisionData acd,
      MissionWeapon weapon,
      ref AttackInformation attackInformation,
      float momentumRemaining,
      bool cancelDamage,
      bool hitWithAnotherBone,
      Vec3 attackerVel,
      Vec3 victimVel,
      out float specialMagnitude,
      out int speedBonusInt)
    {
      StrikeType strikeType = (StrikeType) acd.StrikeType;
      Agent.UsageDirection attackDirection = acd.AttackDirection;
      bool attackerIsDoingPassiveAttack = !attackInformation.IsAttackerAgentNull && attackInformation.IsAttackerAgentHuman && attackInformation.IsAttackerAgentActive && attackInformation.IsAttackerAgentDoingPassiveAttack;
      Mission.ComputeBlowMagnitudeImp(ref acd, ref attackInformation, attackerVel, victimVel, momentumRemaining, cancelDamage, hitWithAnotherBone, out specialMagnitude, out speedBonusInt, strikeType, attackDirection, in weapon, attackerIsDoingPassiveAttack);
      specialMagnitude = MBMath.ClampFloat(specialMagnitude, 0.0f, 500f);
    }

    private static Vec3 GetAgentVelocityContribution(
      bool hasAgentMountAgent,
      Vec2 agentMovementVelocity,
      Vec3 agentMountMovementDirection,
      float agentMovementDirectionAsAngle)
    {
      Vec3 vec3 = Vec3.Zero;
      if (hasAgentMountAgent)
      {
        vec3 = agentMovementVelocity.y * agentMountMovementDirection;
      }
      else
      {
        vec3.AsVec2 = agentMovementVelocity;
        vec3.RotateAboutZ(agentMovementDirectionAsAngle);
      }
      return vec3;
    }

    private static void ComputeBlowMagnitudeImp(
      ref AttackCollisionData acd,
      ref AttackInformation attackInformation,
      Vec3 attackerAgentVelocity,
      Vec3 victimAgentVelocity,
      float momentumRemaining,
      bool cancelDamage,
      bool hitWithAnotherBone,
      out float specialMagnitude,
      out int speedBonusInt,
      StrikeType strikeType,
      Agent.UsageDirection attackDir,
      in MissionWeapon weapon,
      bool attackerIsDoingPassiveAttack)
    {
      acd.MovementSpeedDamageModifier = 0.0f;
      speedBonusInt = 0;
      if (acd.IsMissile)
        Mission.ComputeBlowMagnitudeMissile(ref acd, weapon.Item, attackInformation.IsVictimAgentNull, momentumRemaining, acd.MissileTotalDamage, out acd.BaseMagnitude, out specialMagnitude, victimAgentVelocity);
      else if (acd.IsFallDamage)
        Mission.ComputeBlowMagnitudeFromFall(ref acd, attackInformation.DoesVictimHaveMountAgent, attackInformation.VictimAgentScale, attackInformation.VictimAgentWeight, attackInformation.VictimAgentTotalEncumbrance, attackInformation.IsVictimAgentHuman, out acd.BaseMagnitude, out specialMagnitude);
      else if (acd.IsHorseCharge)
        Mission.ComputeBlowMagnitudeFromHorseCharge(ref acd, attackInformation.AttackerAgentMovementDirection, attackerAgentVelocity, attackInformation.AttackerAgentMountChargeDamageProperty, victimAgentVelocity, attackInformation.VictimAgentPosition, out acd.BaseMagnitude, out specialMagnitude);
      else
        Mission.ComputeBlowMagnitudeMelee(attackInformation.AttackerAgentCharacter, attackInformation.AttackerCaptainCharacter, ref acd, attackInformation.IsAttackerAgentNull, attackInformation.AttackerAgentCurrentWeaponOffset, attackInformation.IsVictimAgentNull, attackInformation.DoesAttackerHaveMountAgent, attackInformation.DoesVictimHaveMountAgent, attackInformation.IsVictimAgentMount, momentumRemaining, cancelDamage, hitWithAnotherBone, out acd.BaseMagnitude, out specialMagnitude, out acd.MovementSpeedDamageModifier, out speedBonusInt, strikeType, attackDir, in weapon, attackerIsDoingPassiveAttack, attackerAgentVelocity, victimAgentVelocity);
    }

    private static void ComputeBlowMagnitudeMelee(
      BasicCharacterObject attackerCharacter,
      BasicCharacterObject attackerCaptainCharacter,
      ref AttackCollisionData acd,
      bool isAttackerAgentNull,
      Vec3 attackerCurrentWeaponOffset,
      bool isVictimAgentNull,
      bool doesAttackerHaveMount,
      bool doesVictimHaveMount,
      bool isVictimMount,
      float momentumRemaining,
      bool cancelDamage,
      bool hitWithAnotherBone,
      out float baseMagnitude,
      out float specialMagnitude,
      out float movementSpeedDamageModifier,
      out int speedBonusInt,
      StrikeType strikeType,
      Agent.UsageDirection attackDir,
      in MissionWeapon weapon,
      bool attackerIsDoingPassiveAttack,
      Vec3 attackerVel,
      Vec3 victimVel)
    {
      movementSpeedDamageModifier = 0.0f;
      speedBonusInt = 0;
      specialMagnitude = 0.0f;
      baseMagnitude = 0.0f;
      if (acd.IsAlternativeAttack)
      {
        baseMagnitude = 3f * momentumRemaining;
        specialMagnitude = baseMagnitude;
      }
      else
      {
        Vec3 weaponBlowDir = acd.WeaponBlowDir;
        Vec3 vec3 = attackerVel - victimVel;
        float num1 = vec3.Normalize();
        Vec3 v2 = vec3;
        float num2 = Vec3.DotProduct(weaponBlowDir, v2);
        if ((double) num2 > 0.0)
          num2 = Math.Min(num2 + 0.2f, 1f);
        float exraLinearSpeed = num1 * num2;
        if (weapon.IsEmpty)
        {
          baseMagnitude = Mission.SpeedGraphFunction(acd.AttackProgress, strikeType, attackDir) * momentumRemaining * ManagedParameters.Instance.GetManagedParameter(ManagedParametersEnum.FistFightDamageMultiplier);
          specialMagnitude = baseMagnitude;
        }
        else
        {
          float z = attackerCurrentWeaponOffset.z;
          WeaponComponentData currentUsageItem = weapon.CurrentUsageItem;
          float maxValue = currentUsageItem.GetRealWeaponLength() + z;
          float impactPointAsPercent = MBMath.ClampFloat(acd.CollisionDistanceOnWeapon, -0.2f, maxValue) / maxValue;
          if (attackerIsDoingPassiveAttack)
          {
            baseMagnitude = doesAttackerHaveMount || doesVictimHaveMount || isVictimMount ? CombatStatCalculator.CalculateBaseBlowMagnitudeForPassiveUsage(weapon.Item.Weight, exraLinearSpeed) : 0.0f;
            baseMagnitude = MissionGameModels.Current.AgentApplyDamageModel.CalculatePassiveAttackDamage(attackerCharacter, ref acd, baseMagnitude);
          }
          else
          {
            float progressEffect = Mission.SpeedGraphFunction(acd.AttackProgress, strikeType, attackDir);
            baseMagnitude = Mission.CalculateBaseBlowMagnitude(attackerCharacter, attackerCaptainCharacter, in weapon, strikeType, progressEffect, impactPointAsPercent, exraLinearSpeed, doesAttackerHaveMount);
            if ((double) baseMagnitude >= 0.0 && (double) progressEffect > 0.699999988079071)
            {
              float baseBlowMagnitude = Mission.CalculateBaseBlowMagnitude(attackerCharacter, attackerCaptainCharacter, in weapon, strikeType, progressEffect, impactPointAsPercent, 0.0f, doesAttackerHaveMount);
              movementSpeedDamageModifier = Mission.CalculateSpeedBonus(baseMagnitude, baseBlowMagnitude);
              speedBonusInt = MBMath.Round(100f * movementSpeedDamageModifier);
              speedBonusInt = MBMath.ClampInt(speedBonusInt, -1000, 1000);
            }
          }
          baseMagnitude *= momentumRemaining;
          float num3 = 1f;
          if (hitWithAnotherBone)
            num3 = strikeType != StrikeType.Thrust ? ManagedParameters.Instance.GetManagedParameter(ManagedParametersEnum.SwingHitWithArmDamageMultiplier) : ManagedParameters.Instance.GetManagedParameter(ManagedParametersEnum.ThrustHitWithArmDamageMultiplier);
          else if (strikeType == StrikeType.Thrust && !acd.ThrustTipHit && !acd.AttackBlockedWithShield)
            num3 = ManagedParameters.Instance.GetManagedParameter(ManagedParametersEnum.NonTipThrustHitDamageMultiplier);
          baseMagnitude *= num3;
          specialMagnitude = Mission.ConvertBaseAttackMagnitude(currentUsageItem, strikeType, baseMagnitude);
        }
      }
    }

    private static float CalculateSpeedBonus(
      float baseMagnitude,
      float baseMagnitudeWithoutSpeedBonus)
    {
      return (float) ((double) baseMagnitude / (double) baseMagnitudeWithoutSpeedBonus - 1.0);
    }

    private static void ComputeBlowMagnitudeFromHorseCharge(
      ref AttackCollisionData acd,
      Vec3 attackerAgentMovementDirection,
      Vec3 attackerAgentVelocity,
      float agentMountChargeDamageProperty,
      Vec3 victimAgentVelocity,
      Vec3 victimAgentPosition,
      out float baseMagnitude,
      out float specialMagnitude)
    {
      Vec3 ov = attackerAgentMovementDirection;
      Vec3 vec3_1 = victimAgentVelocity.ProjectOnUnitVector(ov);
      Vec3 vec3_2 = attackerAgentVelocity - vec3_1;
      float num1 = Mission.ChargeDamageDotProduct(victimAgentPosition, attackerAgentMovementDirection, acd.CollisionGlobalPosition);
      float num2 = vec3_2.Length * num1;
      baseMagnitude = num2 * num2 * num1 * agentMountChargeDamageProperty;
      specialMagnitude = baseMagnitude;
    }

    private static void ComputeBlowMagnitudeFromFall(
      ref AttackCollisionData acd,
      bool hasVictimMountAgent,
      float victimAgentScale,
      float victimAgentWeight,
      float victimAgentTotalEncumbrance,
      bool isVictimAgentHuman,
      out float baseMagnitude,
      out float specialMagnitude)
    {
      float num1 = victimAgentScale;
      float num2 = victimAgentWeight * num1 * num1;
      float num3 = (float) Math.Sqrt(1.0 + (double) victimAgentTotalEncumbrance / (double) num2);
      float num4 = -acd.VictimAgentCurVelocity.z;
      if (hasVictimMountAgent)
      {
        float managedParameter = ManagedParameters.Instance.GetManagedParameter(ManagedParametersEnum.FallSpeedReductionMultiplierForRiderDamage);
        num4 *= managedParameter;
      }
      float num5 = !isVictimAgentHuman ? 1.41f : 1f;
      float managedParameter1 = ManagedParameters.Instance.GetManagedParameter(ManagedParametersEnum.FallDamageMultiplier);
      float managedParameter2 = ManagedParameters.Instance.GetManagedParameter(ManagedParametersEnum.FallDamageAbsorbtion);
      baseMagnitude = (num4 * num4 * managedParameter1 - managedParameter2) * num3 * num5;
      baseMagnitude = MBMath.ClampFloat(baseMagnitude, 0.0f, 499.9f);
      specialMagnitude = baseMagnitude;
    }

    private static void ComputeBlowMagnitudeMissile(
      ref AttackCollisionData acd,
      ItemObject weaponItem,
      bool isVictimAgentNull,
      float momentumRemaining,
      float missileTotalDamage,
      out float baseMagnitude,
      out float specialMagnitude,
      Vec3 victimVel)
    {
      double num1 = (isVictimAgentNull ? (double) acd.MissileVelocity.Length : (double) (victimVel - acd.MissileVelocity).Length) / (double) acd.MissileStartingBaseSpeed;
      float num2 = (float) (num1 * num1);
      baseMagnitude = num2 * missileTotalDamage * momentumRemaining;
      specialMagnitude = baseMagnitude;
    }

    private static float ConvertBaseAttackMagnitude(
      WeaponComponentData weapon,
      StrikeType strikeType,
      float baseMagnitude)
    {
      return baseMagnitude * (strikeType == StrikeType.Thrust ? weapon.ThrustDamageFactor : weapon.SwingDamageFactor);
    }

    private static float CalculateBaseBlowMagnitude(
      BasicCharacterObject attackerCharacter,
      BasicCharacterObject attackerCaptainCharacter,
      in MissionWeapon weapon,
      StrikeType strikeType,
      float progressEffect,
      float impactPointAsPercent,
      float exraLinearSpeed,
      bool doesAttackerHaveMount)
    {
      WeaponComponentData currentUsageItem = weapon.CurrentUsageItem;
      float num1 = MathF.Sqrt(progressEffect);
      float num2;
      if (strikeType == StrikeType.Thrust)
      {
        exraLinearSpeed *= 0.5f;
        float thrustWeaponSpeed = (float) weapon.GetModifiedThrustSpeedForCurrentUsage() / 11.76471f * num1;
        num2 = Game.Current.BasicModels.StrikeMagnitudeModel.CalculateStrikeMagnitudeForThrust(attackerCharacter, attackerCaptainCharacter, thrustWeaponSpeed, weapon.Item.Weight, exraLinearSpeed, doesAttackerHaveMount, currentUsageItem.WeaponClass);
      }
      else
      {
        exraLinearSpeed *= 0.7f;
        float angularSpeed = (float) weapon.GetModifiedSwingSpeedForCurrentUsage() / 4.545455f * num1;
        num2 = Mission.CalculateBaseBlowMagnitudeForSwing(attackerCharacter, attackerCaptainCharacter, in weapon, angularSpeed, impactPointAsPercent, exraLinearSpeed, doesAttackerHaveMount, currentUsageItem.WeaponClass);
      }
      return num2;
    }

    private static float CalculateBaseBlowMagnitudeForSwing(
      BasicCharacterObject attackerCharacter,
      BasicCharacterObject attackerCaptainCharacter,
      in MissionWeapon weapon,
      float angularSpeed,
      float impactPointAsPercent,
      float exraLinearSpeed,
      bool doesAttackerHaveMount,
      WeaponClass weaponClass)
    {
      WeaponComponentData currentUsageItem = weapon.CurrentUsageItem;
      float num1 = MBMath.ClampFloat(0.4f / currentUsageItem.GetRealWeaponLength(), 0.0f, 1f);
      float num2 = Math.Min(0.93f, impactPointAsPercent);
      float num3 = Math.Min(0.93f, impactPointAsPercent + num1);
      float num4 = 0.0f;
      for (int index = 0; index < 5; ++index)
      {
        float impactPointAsPercent1 = num2 + (float) ((double) index / 4.0 * ((double) num3 - (double) num2));
        float magnitudeForSwing = Game.Current.BasicModels.StrikeMagnitudeModel.CalculateStrikeMagnitudeForSwing(attackerCharacter, attackerCaptainCharacter, angularSpeed, impactPointAsPercent1, weapon.Item.Weight, currentUsageItem.GetRealWeaponLength(), currentUsageItem.Inertia, currentUsageItem.CenterOfMass, exraLinearSpeed, doesAttackerHaveMount, weaponClass);
        if ((double) num4 < (double) magnitudeForSwing)
          num4 = magnitudeForSwing;
      }
      return num4;
    }

    public float GetDamageMultiplierOfCombatDifficulty(Agent affectedAgent)
    {
      float num = 1f;
      if (!MBNetwork.IsSessionActive)
      {
        if (affectedAgent.IsMainAgent || affectedAgent.RiderAgent != null && affectedAgent.RiderAgent.IsMainAgent)
          num = this.InitializerRecord.DamageToPlayerMultiplier;
        else if (this.MainAgent != null && (!affectedAgent.IsMount && affectedAgent.IsFriendOf(this.MainAgent) || affectedAgent.RiderAgent != null && affectedAgent.RiderAgent.IsFriendOf(this.MainAgent)))
          num = this.InitializerRecord.DamageToFriendsMultiplier;
      }
      return num;
    }

    public static void ComputeBlowDamage(
      ref AttackInformation attackInformation,
      ref AttackCollisionData attackCollisionData,
      in MissionWeapon attackerWeapon,
      DamageTypes damageType,
      float magnitude,
      int speedBonus,
      bool cancelDamage,
      out int inflictedDamage,
      out int absorbedByArmor)
    {
      float armorAmountFloat = attackInformation.ArmorAmountFloat;
      WeaponComponentData shieldOnBack = attackInformation.ShieldOnBack;
      AgentFlag victimAgentFlag = attackInformation.VictimAgentFlag;
      float absorbedDamageRatio1 = attackInformation.VictimAgentAbsorbedDamageRatio;
      float multiplierOfBone = attackInformation.DamageMultiplierOfBone;
      float difficultyMultiplier = attackInformation.CombatDifficultyMultiplier;
      Vec3 collisionGlobalPosition = attackCollisionData.CollisionGlobalPosition;
      int num1 = attackCollisionData.AttackBlockedWithShield ? 1 : 0;
      int num2 = attackCollisionData.CollidedWithShieldOnBack ? 1 : 0;
      bool isFallDamage = attackCollisionData.IsFallDamage;
      BasicCharacterObject attackerAgentCharacter = attackInformation.AttackerAgentCharacter;
      BasicCharacterObject captainCharacter1 = attackInformation.AttackerCaptainCharacter;
      BasicCharacterObject victimAgentCharacter = attackInformation.VictimAgentCharacter;
      BasicCharacterObject captainCharacter2 = attackInformation.VictimCaptainCharacter;
      float armorEffectiveness = 0.0f;
      if (!isFallDamage)
        armorEffectiveness = Game.Current.BasicModels.StrikeMagnitudeModel.CalculateAdjustedArmorForBlow(armorAmountFloat, attackerAgentCharacter, captainCharacter1, victimAgentCharacter, captainCharacter2, attackerWeapon.CurrentUsageItem);
      if (num2 != 0 && shieldOnBack != null)
        armorEffectiveness += 10f;
      float absorbedDamageRatio2 = absorbedDamageRatio1;
      float rawDamage = Game.Current.BasicModels.StrikeMagnitudeModel.ComputeRawDamage(damageType, magnitude, armorEffectiveness, absorbedDamageRatio2);
      float num3 = 1f;
      if (num1 == 0 && !isFallDamage)
        num3 = num3 * multiplierOfBone * difficultyMultiplier;
      float num4 = rawDamage * num3;
      inflictedDamage = MBMath.ClampInt((int) num4, 0, 2000);
      int num5 = MBMath.ClampInt((int) ((double) Game.Current.BasicModels.StrikeMagnitudeModel.ComputeRawDamage(damageType, magnitude, 0.0f, absorbedDamageRatio2) * (double) num3), 0, 2000);
      absorbedByArmor = num5 - inflictedDamage;
    }

    public static void ComputeBlowDamageOnShield(
      bool isAttackerAgentNull,
      bool isAttackerAgentActive,
      bool isAttackerAgentDoingPassiveAttack,
      bool canGiveDamageToAgentShield,
      bool isVictimAgentLeftStance,
      MissionWeapon victimShield,
      ref AttackCollisionData attackCollisionData,
      WeaponComponentData attackerWeapon,
      float blowMagnitude)
    {
      attackCollisionData.InflictedDamage = 0;
      if (!(victimShield.CurrentUsageItem.WeaponFlags.HasAnyFlag<WeaponFlags>(WeaponFlags.CanBlockRanged) & canGiveDamageToAgentShield))
        return;
      DamageTypes damageType = (DamageTypes) attackCollisionData.DamageType;
      int armorForCurrentUsage = victimShield.GetGetModifiedArmorForCurrentUsage();
      float absorbedDamageRatio = 1f;
      float rawDamage = Game.Current.BasicModels.StrikeMagnitudeModel.ComputeRawDamage(damageType, blowMagnitude, (float) armorForCurrentUsage, absorbedDamageRatio);
      if (attackCollisionData.IsMissile)
      {
        if (attackerWeapon.WeaponClass == WeaponClass.Javelin)
          rawDamage *= 0.15f;
        else if (attackerWeapon.WeaponClass == WeaponClass.ThrowingAxe)
          rawDamage *= 0.15f;
        else
          rawDamage *= 0.1f;
      }
      else if (attackCollisionData.DamageType == 1)
        rawDamage *= 0.5f;
      else if (attackCollisionData.DamageType == 2)
        rawDamage *= 0.5f;
      if (attackerWeapon != null && attackerWeapon.WeaponFlags.HasAnyFlag<WeaponFlags>(WeaponFlags.BonusAgainstShield))
        rawDamage *= 2f;
      if ((double) rawDamage <= 0.0)
        return;
      if (!isVictimAgentLeftStance)
        rawDamage *= ManagedParameters.Instance.GetManagedParameter(ManagedParametersEnum.ShieldRightStanceBlockDamageMultiplier);
      if (attackCollisionData.CorrectSideShieldBlock)
        rawDamage *= ManagedParameters.Instance.GetManagedParameter(ManagedParametersEnum.ShieldCorrectSideBlockDamageMultiplier);
      float shieldDamage = MissionGameModels.Current.AgentApplyDamageModel.CalculateShieldDamage(rawDamage);
      attackCollisionData.InflictedDamage = (int) shieldDamage;
    }

    public float GetShootDifficulty(Agent affectedAgent, Agent affectorAgent, bool isHeadShot)
    {
      Vec2 vec2 = affectedAgent.MovementVelocity - affectorAgent.MovementVelocity;
      Vec3 va = new Vec3(vec2.x, vec2.y);
      Vec3 vb = affectedAgent.Position - affectorAgent.Position;
      float num1 = vb.Normalize();
      float num2 = va.Normalize();
      float length = Vec3.CrossProduct(va, vb).Length;
      float num3 = MBMath.ClampFloat((float) (0.300000011920929 * ((4.0 + (double) num1) / 4.0) * ((4.0 + (double) length * (double) num2) / 4.0)), 1f, 12f);
      if (isHeadShot)
        num3 *= 1.2f;
      return num3;
    }

    private static float ComputeRelativeSpeedDiffOfAgents(Agent agentA, Agent agentB)
    {
      Vec3 vec3_1 = Vec3.Zero;
      if (agentA.MountAgent != null)
      {
        vec3_1 = agentA.MountAgent.MovementVelocity.y * agentA.MountAgent.GetMovementDirection();
      }
      else
      {
        vec3_1.AsVec2 = agentA.MovementVelocity;
        vec3_1.RotateAboutZ(agentA.MovementDirectionAsAngle);
      }
      Vec3 vec3_2 = Vec3.Zero;
      if (agentB.MountAgent != null)
      {
        vec3_2 = agentB.MountAgent.MovementVelocity.y * agentB.MountAgent.GetMovementDirection();
      }
      else
      {
        vec3_2.AsVec2 = agentB.MovementVelocity;
        vec3_2.RotateAboutZ(agentB.MovementDirectionAsAngle);
      }
      return (vec3_1 - vec3_2).Length;
    }

    private static bool DecideAgentShrugOffBlow(
      Agent victimAgent,
      AttackCollisionData collisionData,
      ref Blow blow)
    {
      int num1 = (double) victimAgent.Health - (double) collisionData.InflictedDamage >= 1.0 ? 1 : 0;
      bool flag = false;
      if (num1 != 0)
      {
        ManagedParametersEnum managedParameterEnum = blow.DamageType != DamageTypes.Cut ? (blow.DamageType != DamageTypes.Pierce ? ManagedParametersEnum.DamageInterruptAttackThresholdBlunt : ManagedParametersEnum.DamageInterruptAttackThresholdPierce) : ManagedParametersEnum.DamageInterruptAttackThresholdCut;
        float thresholdMultiplier = MissionGameModels.Current.AgentApplyDamageModel.CalculateStaggerThresholdMultiplier(victimAgent);
        float num2 = ManagedParameters.Instance.GetManagedParameter(managedParameterEnum) * thresholdMultiplier;
        float? interruptionThreshold = MPPerkObject.GetPerkHandler(victimAgent)?.GetDamageInterruptionThreshold();
        if (interruptionThreshold.HasValue && (double) interruptionThreshold.Value > 0.0)
          num2 = interruptionThreshold.Value;
        if ((double) collisionData.InflictedDamage <= (double) num2)
        {
          blow.BlowFlag |= BlowFlags.ShrugOff;
          flag = true;
        }
      }
      return flag;
    }

    private static void DecideAgentDismountedByBlow(
      Agent attackerAgent,
      Agent victimAgent,
      in AttackCollisionData collisionData,
      WeaponComponentData attackerWeapon,
      bool isInitialBlowShrugOff,
      ref Blow blow)
    {
      if (victimAgent.HasMount && !isInitialBlowShrugOff)
      {
        bool flag1 = (double) blow.InflictedDamage / (double) victimAgent.HealthLimit > 0.25;
        bool flag2 = MBMath.IsBetween((int) blow.VictimBodyPart, 0, 6);
        if (!((double) victimAgent.Health - (double) collisionData.InflictedDamage >= 1.0 & flag1 & flag2))
          return;
        if (blow.StrikeType == StrikeType.Thrust && blow.WeaponRecord.WeaponFlags.HasAnyFlag<WeaponFlags>(WeaponFlags.CanDismount))
        {
          blow.BlowFlag |= BlowFlags.CanDismount;
        }
        else
        {
          float numberToCheck = 0.0f + MissionGameModels.Current.AgentApplyDamageModel.CalculateDismountChanceBonus(attackerAgent, attackerWeapon);
          if ((MBMath.IsBetween(numberToCheck, 0.0f, 1f) ? (double) MBRandom.RandomFloat : 0.100000001490116) > (double) numberToCheck)
            return;
          blow.BlowFlag |= BlowFlags.CanDismount;
        }
      }
      else
      {
        int num = victimAgent.HasMount ? 1 : 0;
      }
    }

    private static void DecideAgentKnockedByBlow(
      Agent attacker,
      Agent victim,
      in AttackCollisionData collisionData,
      WeaponComponentData attackerWeapon,
      bool isInitialBlowShrugOff,
      ref Blow blow)
    {
      if (!victim.IsHuman || victim.HasMount)
        return;
      float num1 = (float) collisionData.InflictedDamage / Math.Max(victim.HealthLimit, 1f);
      bool flag1 = (double) num1 > 0.25;
      bool flag2 = MBMath.IsBetween((int) collisionData.VictimHitBodyPart, 0, 6);
      if (collisionData.IsHorseCharge)
      {
        if ((double) Mission.ChargeDamageDotProduct(victim.Position, attacker.GetMovementDirection(), collisionData.CollisionGlobalPosition) < 0.699999988079071)
          blow.BlowFlag &= ~BlowFlags.KnockBack;
        else
          blow.BlowFlag |= BlowFlags.KnockBack;
      }
      else if (collisionData.IsAlternativeAttack)
        blow.BlowFlag |= BlowFlags.KnockBack;
      else if (attackerWeapon != null && !attackerWeapon.WeaponFlags.HasAnyFlag<WeaponFlags>(WeaponFlags.CanKnockDown) && flag1)
      {
        float numberToCheck = 0.0f;
        if (!blow.BlowFlag.HasAnyFlag<BlowFlags>(BlowFlags.CrushThrough))
        {
          if (blow.StrikeType == StrikeType.Thrust && blow.WeaponRecord.WeaponFlags.HasAnyFlag<WeaponFlags>(WeaponFlags.WideGrip))
            numberToCheck += 0.5f;
          else if (attackerWeapon.IsConsumable)
            numberToCheck += 0.4f;
          else if (!attackerWeapon.IsConsumable && (double) num1 >= 0.5)
            numberToCheck += 0.3f;
        }
        else
          numberToCheck += 0.3f;
        if (flag2)
          numberToCheck += MissionGameModels.Current.AgentApplyDamageModel.CalculateKnockBackChanceBonus(attacker, attackerWeapon);
        if ((MBMath.IsBetween(numberToCheck, 0.0f, 1f) ? (double) MBRandom.RandomFloat : 0.100000001490116) <= (double) numberToCheck)
          blow.BlowFlag |= BlowFlags.KnockBack;
      }
      float numberToCheck1 = 0.0f;
      if (!blow.WeaponRecord.HasWeapon() || blow.WeaponRecord.WeaponFlags.HasAnyFlag<WeaponFlags>(WeaponFlags.CanKnockDown))
      {
        if (blow.BlowFlag.HasAnyFlag<BlowFlags>(BlowFlags.KnockBack) && (double) blow.BaseMagnitude >= 10.0)
          ++numberToCheck1;
        else if (blow.DamageType == DamageTypes.Blunt)
        {
          float num2 = 0.015f * Math.Min((float) (((double) blow.BaseMagnitude - 20.0) * 0.5), 10f) * Math.Min((float) ((!blow.WeaponRecord.HasWeapon() ? 1.0 : (double) blow.WeaponRecord.Weight) * 0.330000013113022), 2f);
          numberToCheck1 += num2;
        }
      }
      if (flag1 & flag2)
        numberToCheck1 += MissionGameModels.Current.AgentApplyDamageModel.CalculateKnockDownChanceBonus(attacker, attackerWeapon);
      if ((MBMath.IsBetween(numberToCheck1, 0.0f, 1f) ? (double) MBRandom.RandomFloat : 0.100000001490116) > (double) numberToCheck1)
        return;
      blow.BlowFlag |= BlowFlags.KnockDown;
    }

    private static void DecideMountRearedByBlow(
      Agent attackerAgent,
      Agent victimAgent,
      in AttackCollisionData collisionData,
      WeaponComponentData attackerWeapon,
      float rearDamageThresholdMultiplier,
      Vec3 blowDirection,
      ref Blow blow)
    {
      if (!victimAgent.IsMount || attackerWeapon == null || (!attackerWeapon.WeaponFlags.HasAnyFlag<WeaponFlags>(WeaponFlags.WideGrip) || blow.StrikeType != StrikeType.Thrust) || (!collisionData.ThrustTipHit || attackerAgent == null || (attackerAgent.HasMount || !victimAgent.GetAgentFlags().HasAnyFlag<AgentFlag>(AgentFlag.CanRear)) || ((double) victimAgent.MovementVelocity.y <= 5.0 || (double) Vec3.DotProduct(blowDirection, victimAgent.Frame.rotation.f) >= -0.349999994039536 || (double) collisionData.InflictedDamage < (double) ManagedParameters.Instance.GetManagedParameter(ManagedParametersEnum.MakesRearAttackDamageThreshold) * (double) rearDamageThresholdMultiplier)))
        return;
      blow.BlowFlag |= BlowFlags.MakesRear;
    }

    private static float GetEntityDamageMultiplier(
      bool isAttackerAgentDoingPassiveAttack,
      WeaponComponentData weapon,
      DamageTypes damageType,
      bool isWoodenBody)
    {
      float num = 1f;
      if (isAttackerAgentDoingPassiveAttack)
        num *= 0.2f;
      if (weapon != null)
      {
        if (weapon.WeaponFlags.HasAnyFlag<WeaponFlags>(WeaponFlags.BonusAgainstShield))
          num *= 1.2f;
        switch (damageType)
        {
          case DamageTypes.Cut:
            num *= 0.8f;
            break;
          case DamageTypes.Pierce:
            num *= 0.1f;
            break;
        }
        if (isWoodenBody && weapon.WeaponFlags.HasAnyFlag<WeaponFlags>(WeaponFlags.Burning))
          num *= 1.5f;
      }
      return num;
    }

    private MatrixFrame CalculateAttachedLocalFrame(
      ref MatrixFrame attachedGlobalFrame,
      AttackCollisionData collisionData,
      WeaponComponentData missileWeapon,
      Agent affectedAgent,
      GameEntity hitEntity,
      Vec3 missileMovementVelocity,
      Vec3 missileRotationSpeed,
      MatrixFrame shieldGlobalFrame,
      bool shouldMissilePenetrate)
    {
      MatrixFrame frame = attachedGlobalFrame;
      bool isNonZero = missileWeapon.RotationSpeed.IsNonZero;
      bool flag = affectedAgent != null && !collisionData.AttackBlockedWithShield && missileWeapon.WeaponFlags.HasAnyFlag<WeaponFlags>(WeaponFlags.AmmoSticksWhenShot);
      float managedParameter1 = ManagedParameters.Instance.GetManagedParameter(flag ? (isNonZero ? ManagedParametersEnum.RotatingProjectileMinPenetration : ManagedParametersEnum.ProjectileMinPenetration) : ManagedParametersEnum.ObjectMinPenetration);
      float managedParameter2 = ManagedParameters.Instance.GetManagedParameter(flag ? (isNonZero ? ManagedParametersEnum.RotatingProjectileMaxPenetration : ManagedParametersEnum.ProjectileMaxPenetration) : ManagedParametersEnum.ObjectMaxPenetration);
      Vec3 vec3_1 = missileMovementVelocity;
      float num1 = vec3_1.Normalize();
      float num2 = MBMath.ClampFloat(flag ? (float) collisionData.InflictedDamage / affectedAgent.HealthLimit : num1 / ManagedParameters.Instance.GetManagedParameter(ManagedParametersEnum.ProjectileMaxPenetrationSpeed), 0.0f, 1f);
      if (shouldMissilePenetrate)
      {
        float num3 = managedParameter1 + (managedParameter2 - managedParameter1) * num2;
        frame.origin += vec3_1 * num3;
      }
      MatrixFrame matrixFrame1;
      if (missileRotationSpeed.IsNonZero)
      {
        float managedParameter3 = ManagedParameters.Instance.GetManagedParameter(flag ? ManagedParametersEnum.AgentProjectileNormalWeight : ManagedParametersEnum.ProjectileNormalWeight);
        matrixFrame1 = missileWeapon.GetMissileStartingFrame();
        Vec3 parent = matrixFrame1.TransformToParent(missileRotationSpeed);
        Vec3 vec3_2 = -collisionData.CollisionGlobalNormal;
        float num3 = parent.x * parent.x;
        float num4 = parent.y * parent.y;
        float num5 = parent.z * parent.z;
        int i = (double) num3 <= (double) num4 || (double) num3 <= (double) num5 ? ((double) num4 > (double) num5 ? 1 : 2) : 0;
        vec3_2 -= vec3_2.ProjectOnUnitVector(frame.rotation[i]);
        Vec3 v = Vec3.CrossProduct(vec3_1, vec3_2.NormalizedCopy());
        float num6 = v.Normalize();
        frame.rotation.RotateAboutAnArbitraryVector(v, num6 * managedParameter3);
      }
      if (!collisionData.AttackBlockedWithShield && affectedAgent != null)
      {
        float num3 = Vec3.DotProduct(collisionData.CollisionGlobalNormal, vec3_1) + 1f;
        if ((double) num3 > 0.5)
          frame.origin -= num3 * 0.1f * collisionData.CollisionGlobalNormal;
      }
      ref MatrixFrame local = ref frame;
      matrixFrame1 = missileWeapon.GetMissileStartingFrame();
      MatrixFrame parent1 = matrixFrame1.TransformToParent(missileWeapon.StickingFrame);
      frame = local.TransformToParent(parent1);
      frame = frame.TransformToParent(missileWeapon.GetMissileStartingFrame());
      if (collisionData.AttackBlockedWithShield)
        frame = shieldGlobalFrame.TransformToLocal(frame);
      else if (affectedAgent != null)
      {
        if (flag)
        {
          MBAgentVisuals agentVisuals = affectedAgent.AgentVisuals;
          matrixFrame1 = agentVisuals.GetGlobalFrame();
          MatrixFrame matrixFrame2 = matrixFrame1.TransformToParent(agentVisuals.GetSkeleton().GetBoneEntitialFrameWithIndex((byte) collisionData.CollisionBoneIndex));
          matrixFrame2 = matrixFrame2.GetUnitRotFrame(affectedAgent.AgentScale);
          frame = matrixFrame2.TransformToLocalNonOrthogonal(ref frame);
        }
      }
      else if ((NativeObject) hitEntity != (NativeObject) null)
      {
        if (collisionData.CollisionBoneIndex >= (sbyte) 0)
        {
          frame = hitEntity.Skeleton.GetBoneEntitialFrameWithIndex((byte) collisionData.CollisionBoneIndex).TransformToLocalNonOrthogonal(ref frame);
        }
        else
        {
          matrixFrame1 = hitEntity.GetGlobalFrame();
          frame = matrixFrame1.TransformToLocalNonOrthogonal(ref frame);
        }
      }
      return frame;
    }

    [UsedImplicitly]
    [MBCallback]
    internal void GetDefendCollisionResults(
      Agent attackerAgent,
      Agent defenderAgent,
      CombatCollisionResult collisionResult,
      int attackerWeaponSlotIndex,
      bool isAlternativeAttack,
      StrikeType strikeType,
      Agent.UsageDirection attackDirection,
      float collisionDistanceOnWeapon,
      float attackProgress,
      bool attackIsParried,
      bool isPassiveUsageHit,
      ref float defenderStunPeriod,
      ref float attackerStunPeriod,
      ref bool crushedThrough)
    {
      bool chamber = false;
      Mission.GetDefendCollisionResultsAux(attackerAgent, defenderAgent, collisionResult, attackerWeaponSlotIndex, isAlternativeAttack, strikeType, attackDirection, collisionDistanceOnWeapon, attackProgress, attackIsParried, isPassiveUsageHit, ref defenderStunPeriod, ref attackerStunPeriod, ref crushedThrough, ref chamber);
      if (!(crushedThrough | chamber) || !attackerAgent.CanLogCombatFor && !defenderAgent.CanLogCombatFor)
        return;
      CombatLogData combatLog = new CombatLogData(false, attackerAgent.IsHuman, attackerAgent.IsMine, attackerAgent.RiderAgent != null, attackerAgent.RiderAgent != null && attackerAgent.RiderAgent.IsMine, attackerAgent.IsMount, defenderAgent.IsHuman, defenderAgent.IsMine, (double) defenderAgent.Health <= 0.0, defenderAgent.HasMount, defenderAgent.RiderAgent != null && defenderAgent.RiderAgent.IsMine, defenderAgent.IsMount, false, defenderAgent.RiderAgent == attackerAgent, crushedThrough, chamber, 0.0f);
      this.AddCombatLogSafe(attackerAgent, defenderAgent, (GameEntity) null, combatLog);
    }

    private static void GetDefendCollisionResultsAux(
      Agent attackerAgent,
      Agent defenderAgent,
      CombatCollisionResult collisionResult,
      int attackerWeaponSlotIndex,
      bool isAlternativeAttack,
      StrikeType strikeType,
      Agent.UsageDirection attackDirection,
      float collisionDistanceOnWeapon,
      float attackProgress,
      bool attackIsParried,
      bool isPassiveUsageHit,
      ref float defenderStunPeriod,
      ref float attackerStunPeriod,
      ref bool crushedThrough,
      ref bool chamber)
    {
      MissionWeapon missionWeapon1 = attackerWeaponSlotIndex >= 0 ? attackerAgent.Equipment[attackerWeaponSlotIndex] : MissionWeapon.Invalid;
      WeaponComponentData attackerWeapon = missionWeapon1.IsEmpty ? (WeaponComponentData) null : missionWeapon1.CurrentUsageItem;
      EquipmentIndex wieldedItemIndex = defenderAgent.GetWieldedItemIndex(Agent.HandIndex.OffHand);
      if (wieldedItemIndex == EquipmentIndex.None)
        wieldedItemIndex = defenderAgent.GetWieldedItemIndex(Agent.HandIndex.MainHand);
      MissionWeapon missionWeapon2;
      ItemObject itemObject1;
      if (wieldedItemIndex == EquipmentIndex.None)
      {
        itemObject1 = (ItemObject) null;
      }
      else
      {
        missionWeapon2 = defenderAgent.Equipment[wieldedItemIndex];
        itemObject1 = missionWeapon2.Item;
      }
      ItemObject itemObject2 = itemObject1;
      WeaponComponentData weaponComponentData1;
      if (wieldedItemIndex == EquipmentIndex.None)
      {
        weaponComponentData1 = (WeaponComponentData) null;
      }
      else
      {
        missionWeapon2 = defenderAgent.Equipment[wieldedItemIndex];
        weaponComponentData1 = missionWeapon2.CurrentUsageItem;
      }
      WeaponComponentData weaponComponentData2 = weaponComponentData1;
      float totalAttackEnergy = 10f;
      attackerStunPeriod = strikeType == StrikeType.Thrust ? ManagedParameters.Instance.GetManagedParameter(ManagedParametersEnum.StunPeriodAttackerThrust) : ManagedParameters.Instance.GetManagedParameter(ManagedParametersEnum.StunPeriodAttackerSwing);
      chamber = false;
      if (!missionWeapon1.IsEmpty)
      {
        float z = attackerAgent.GetCurWeaponOffset().z;
        float realWeaponLength = attackerWeapon.GetRealWeaponLength();
        float num1 = realWeaponLength + z;
        float impactPoint = MBMath.ClampFloat((0.2f + collisionDistanceOnWeapon) / num1, 0.1f, 0.98f);
        float speedDiffOfAgents = Mission.ComputeRelativeSpeedDiffOfAgents(attackerAgent, defenderAgent);
        float num2 = strikeType != StrikeType.Thrust ? CombatStatCalculator.CalculateBaseBlowMagnitudeForSwing((float) missionWeapon1.GetModifiedSwingSpeedForCurrentUsage() / 4.545455f * Mission.SpeedGraphFunction(attackProgress, strikeType, attackDirection), realWeaponLength, missionWeapon1.Item.Weight, attackerWeapon.Inertia, attackerWeapon.CenterOfMass, impactPoint, speedDiffOfAgents) : CombatStatCalculator.CalculateBaseBlowMagnitudeForThrust((float) missionWeapon1.GetModifiedThrustSpeedForCurrentUsage() / 11.76471f * Mission.SpeedGraphFunction(attackProgress, strikeType, attackDirection), missionWeapon1.Item.Weight, speedDiffOfAgents);
        if (strikeType == StrikeType.Thrust)
          num2 *= 0.8f;
        else if (attackDirection == Agent.UsageDirection.AttackUp)
          num2 *= 1.25f;
        totalAttackEnergy += num2;
      }
      float num = 1f;
      defenderStunPeriod = totalAttackEnergy * ManagedParameters.Instance.GetManagedParameter(ManagedParametersEnum.StunMomentumTransferFactor);
      if (weaponComponentData2 != null)
      {
        if (weaponComponentData2.IsShield)
        {
          float managedParameter = ManagedParameters.Instance.GetManagedParameter(ManagedParametersEnum.StunDefendWeaponWeightOffsetShield);
          num += managedParameter * itemObject2.Weight;
        }
        else
        {
          num = 0.9f + ManagedParameters.Instance.GetManagedParameter(ManagedParametersEnum.StunDefendWeaponWeightMultiplierWeaponWeight) * itemObject2.Weight;
          switch (itemObject2.ItemType)
          {
            case ItemObject.ItemTypeEnum.TwoHandedWeapon:
              ManagedParameters.Instance.GetManagedParameter(ManagedParametersEnum.StunDefendWeaponWeightBonusTwoHanded);
              break;
            case ItemObject.ItemTypeEnum.Polearm:
              num += ManagedParameters.Instance.GetManagedParameter(ManagedParametersEnum.StunDefendWeaponWeightBonusPolearm);
              break;
          }
        }
        switch (collisionResult)
        {
          case CombatCollisionResult.Parried:
            attackerStunPeriod += 0.1f;
            num += ManagedParameters.Instance.GetManagedParameter(ManagedParametersEnum.StunDefendWeaponWeightBonusActiveBlocked);
            break;
          case CombatCollisionResult.ChamberBlocked:
            attackerStunPeriod += 0.2f;
            num += ManagedParameters.Instance.GetManagedParameter(ManagedParametersEnum.StunDefendWeaponWeightBonusChamberBlocked);
            chamber = true;
            break;
        }
      }
      if (!defenderAgent.GetIsLeftStance())
        num += ManagedParameters.Instance.GetManagedParameter(ManagedParametersEnum.StunDefendWeaponWeightBonusRightStance);
      defenderStunPeriod /= num;
      float attackerStunMultiplier;
      float defenderStunMultiplier;
      MissionGameModels.Current.AgentApplyDamageModel.CalculateCollisionStunMultipliers(attackerAgent, defenderAgent, isAlternativeAttack, collisionResult, attackerWeapon, weaponComponentData2, out attackerStunMultiplier, out defenderStunMultiplier);
      attackerStunPeriod *= attackerStunMultiplier;
      defenderStunPeriod *= defenderStunMultiplier;
      float managedParameter1 = ManagedParameters.Instance.GetManagedParameter(ManagedParametersEnum.StunPeriodMax);
      attackerStunPeriod = Math.Min(attackerStunPeriod, managedParameter1);
      defenderStunPeriod = Math.Min(defenderStunPeriod, managedParameter1);
      crushedThrough = !chamber && MissionGameModels.Current.AgentApplyDamageModel.DecideCrushedThrough(attackerAgent, defenderAgent, totalAttackEnergy, attackDirection, strikeType, weaponComponentData2, isPassiveUsageHit);
    }

    private CombatLogData GetAttackCollisionResults(
      Agent attackerAgent,
      Agent victimAgent,
      GameEntity hitObject,
      float momentumRemaining,
      ref AttackCollisionData attackCollisionData,
      in MissionWeapon attackerWeapon,
      bool crushedThrough,
      bool cancelDamage,
      bool crushedThroughWithoutAgentCollision,
      out WeaponComponentData shieldOnBack,
      out CombatLogData combatLog)
    {
      AttackInformation attackInformation = new AttackInformation(attackerAgent, victimAgent, hitObject, in attackCollisionData, in attackerWeapon);
      shieldOnBack = attackInformation.ShieldOnBack;
      MPPerkObject.MPCombatPerkHandler combatPerkHandler1 = MPPerkObject.GetCombatPerkHandler(attackerAgent, victimAgent);
      int speedBonus;
      Mission.GetAttackCollisionResults(ref attackInformation, crushedThrough, momentumRemaining, ref attackCollisionData, in attackerWeapon, cancelDamage, out combatLog, out speedBonus);
      float inflictedDamage = (float) attackCollisionData.InflictedDamage;
      float baseDamage = inflictedDamage;
      if (combatPerkHandler1 != null && !attackCollisionData.IsFallDamage && !attackCollisionData.IsHorseCharge)
      {
        float num1 = (float) speedBonus / 100f;
        float num2 = num1 * combatPerkHandler1.GetSpeedBonusEffectiveness() + num1;
        attackCollisionData.BaseMagnitude *= (float) (((double) num2 + 1.0) / ((double) num1 + 1.0));
      }
      if ((double) inflictedDamage > 0.0)
      {
        if (attackCollisionData.AttackBlockedWithShield && combatPerkHandler1 != null)
        {
          float num = 1f + combatPerkHandler1.GetShieldDamage(attackCollisionData.CorrectSideShieldBlock) + combatPerkHandler1.GetShieldDamageTaken(attackCollisionData.CorrectSideShieldBlock);
          baseDamage = Math.Max(0.0f, baseDamage * num);
        }
        float damage = MissionGameModels.Current.AgentApplyDamageModel.CalculateDamage(ref attackInformation, ref attackCollisionData, in attackerWeapon, baseDamage);
        if (combatPerkHandler1 != null)
        {
          MPPerkObject.MPCombatPerkHandler combatPerkHandler2 = combatPerkHandler1;
          MissionWeapon missionWeapon = attackerWeapon;
          WeaponComponentData currentUsageItem1 = missionWeapon.CurrentUsageItem;
          int damageType1 = (int) combatLog.DamageType;
          int num1 = attackCollisionData.IsAlternativeAttack ? 1 : 0;
          double num2 = 1.0 + (double) combatPerkHandler2.GetDamage(currentUsageItem1, (DamageTypes) damageType1, num1 != 0);
          MPPerkObject.MPCombatPerkHandler combatPerkHandler3 = combatPerkHandler1;
          missionWeapon = attackerWeapon;
          WeaponComponentData currentUsageItem2 = missionWeapon.CurrentUsageItem;
          int damageType2 = (int) combatLog.DamageType;
          double damageTaken = (double) combatPerkHandler3.GetDamageTaken(currentUsageItem2, (DamageTypes) damageType2);
          float num3 = Math.Max(0.0f, (float) (num2 + damageTaken));
          if (attackInformation.IsHeadShot)
          {
            missionWeapon = attackerWeapon;
            if (missionWeapon.CurrentUsageItem != null)
            {
              missionWeapon = attackerWeapon;
              if (!missionWeapon.CurrentUsageItem.IsConsumable)
              {
                missionWeapon = attackerWeapon;
                if (!missionWeapon.CurrentUsageItem.IsRangedWeapon)
                  goto label_11;
              }
              num3 += combatPerkHandler1.GetRangedHeadShotDamage();
            }
          }
label_11:
          damage *= num3;
        }
        combatLog.ModifiedDamage = MathF.Round(damage - inflictedDamage);
        attackCollisionData.InflictedDamage = MathF.Round(damage);
      }
      else
      {
        combatLog.ModifiedDamage = 0;
        attackCollisionData.InflictedDamage = 0;
      }
      if (!attackCollisionData.IsFallDamage && attackInformation.IsFriendlyFire)
      {
        if (!attackInformation.IsAttackerAIControlled && GameNetwork.IsSessionActive)
        {
          int num1 = attackCollisionData.IsMissile ? MultiplayerOptions.OptionType.FriendlyFireDamageRangedSelfPercent.GetIntValue() : MultiplayerOptions.OptionType.FriendlyFireDamageMeleeSelfPercent.GetIntValue();
          attackCollisionData.SelfInflictedDamage = MBMath.Round((float) attackCollisionData.InflictedDamage * ((float) num1 * 0.01f));
          int num2 = attackCollisionData.IsMissile ? MultiplayerOptions.OptionType.FriendlyFireDamageRangedFriendPercent.GetIntValue() : MultiplayerOptions.OptionType.FriendlyFireDamageMeleeFriendPercent.GetIntValue();
          attackCollisionData.InflictedDamage = MBMath.Round((float) attackCollisionData.InflictedDamage * ((float) num2 * 0.01f));
          if (attackInformation.IsVictimAgentMount && !attackInformation.DoesAttackerHaveMountAgent)
            attackCollisionData.InflictedDamage = MBMath.Round((float) attackCollisionData.InflictedDamage * 0.1f);
          combatLog.InflictedDamage = attackCollisionData.InflictedDamage;
        }
        combatLog.IsFriendlyFire = true;
      }
      if (attackCollisionData.AttackBlockedWithShield && attackCollisionData.InflictedDamage > 0 && (int) attackInformation.VictimShield.HitPoints - attackCollisionData.InflictedDamage <= 0)
        attackCollisionData.IsShieldBroken = true;
      if (!crushedThroughWithoutAgentCollision)
      {
        combatLog.BodyPartHit = attackCollisionData.VictimHitBodyPart;
        combatLog.IsVictimEntity = (NativeObject) hitObject != (NativeObject) null;
      }
      return combatLog;
    }

    public static void GetAttackCollisionResults(
      ref AttackInformation attackInformation,
      bool crushedThrough,
      float momentumRemaining,
      ref AttackCollisionData attackCollisionData,
      in MissionWeapon attackerWeapon,
      bool cancelDamage,
      out CombatLogData combatLog,
      out int speedBonus)
    {
      float distance = 0.0f;
      Vec3 vec3;
      if (attackCollisionData.IsMissile)
      {
        vec3 = attackCollisionData.MissileStartingPosition - attackCollisionData.CollisionGlobalPosition;
        distance = vec3.Length;
      }
      combatLog = new CombatLogData(attackInformation.IsVictimAgentSameWithAttackerAgent, attackInformation.IsAttackerAgentHuman, attackInformation.IsAttackerAgentMine, attackInformation.DoesAttackerHaveRiderAgent, attackInformation.IsAttackerAgentRiderAgentMine, attackInformation.IsAttackerAgentMount, attackInformation.IsVictimAgentHuman, attackInformation.IsVictimAgentMine, false, attackInformation.DoesVictimHaveRiderAgent, attackInformation.IsVictimAgentRiderAgentMine, attackInformation.IsVictimAgentMount, false, attackInformation.IsVictimRiderAgentSameAsAttackerAgent, false, false, distance);
      bool hitWithAnotherBone = Mission.HitWithAnotherBone(in attackCollisionData, attackInformation.WeaponAttachBoneIndex);
      Vec3 velocityContribution1 = Mission.GetAgentVelocityContribution(attackInformation.DoesAttackerHaveMountAgent, attackInformation.AttackerAgentMovementVelocity, attackInformation.AttackerAgentMountMovementDirection, attackInformation.AttackerMovementDirectionAsAngle);
      Vec3 velocityContribution2 = Mission.GetAgentVelocityContribution(attackInformation.DoesVictimHaveMountAgent, attackInformation.VictimAgentMovementVelocity, attackInformation.VictimAgentMountMovementDirection, attackInformation.VictimMovementDirectionAsAngle);
      if (attackCollisionData.IsColliderAgent)
      {
        combatLog.IsRangedAttack = attackCollisionData.IsMissile;
        ref CombatLogData local = ref combatLog;
        double length;
        if (!attackCollisionData.IsMissile)
        {
          vec3 = velocityContribution1 - velocityContribution2;
          length = (double) vec3.Length;
        }
        else
        {
          vec3 = velocityContribution2 - attackCollisionData.MissileVelocity;
          length = (double) vec3.Length;
        }
        local.HitSpeed = (float) length;
      }
      float specialMagnitude;
      Mission.ComputeBlowMagnitude(ref attackCollisionData, attackerWeapon, ref attackInformation, momentumRemaining, cancelDamage, hitWithAnotherBone, velocityContribution1, velocityContribution2, out specialMagnitude, out speedBonus);
      MissionWeapon missionWeapon = attackerWeapon;
      DamageTypes damageType = (missionWeapon.IsEmpty | hitWithAnotherBone || attackCollisionData.IsAlternativeAttack || attackCollisionData.IsFallDamage ? 1 : (attackCollisionData.IsHorseCharge ? 1 : 0)) != 0 ? DamageTypes.Blunt : (DamageTypes) attackCollisionData.DamageType;
      combatLog.DamageType = damageType;
      if (!attackCollisionData.IsColliderAgent && attackCollisionData.EntityExists)
      {
        string name = PhysicsMaterial.GetFromIndex(attackCollisionData.PhysicsMaterialIndex).Name;
        bool flag = name == "wood" || name == "wood_weapon" || name == "wood_shield";
        ref float local = ref attackCollisionData.BaseMagnitude;
        double num1 = (double) local;
        int num2 = attackInformation.IsAttackerAgentDoingPassiveAttack ? 1 : 0;
        missionWeapon = attackerWeapon;
        WeaponComponentData currentUsageItem = missionWeapon.CurrentUsageItem;
        int num3 = (int) damageType;
        int num4 = flag ? 1 : 0;
        double damageMultiplier = (double) Mission.GetEntityDamageMultiplier(num2 != 0, currentUsageItem, (DamageTypes) num3, num4 != 0);
        local = (float) (num1 * damageMultiplier);
        attackCollisionData.InflictedDamage = MBMath.ClampInt((int) attackCollisionData.BaseMagnitude, 0, 2000);
        combatLog.InflictedDamage = attackCollisionData.InflictedDamage;
      }
      if (!attackCollisionData.IsColliderAgent || attackInformation.IsVictimAgentNull)
        return;
      if (attackCollisionData.IsAlternativeAttack)
        specialMagnitude = attackCollisionData.BaseMagnitude;
      if (attackCollisionData.AttackBlockedWithShield)
      {
        int num1 = attackInformation.IsAttackerAgentNull ? 1 : 0;
        int num2 = attackInformation.IsAttackerAgentActive ? 1 : 0;
        int num3 = attackInformation.IsAttackerAgentDoingPassiveAttack ? 1 : 0;
        int num4 = attackInformation.CanGiveDamageToAgentShield ? 1 : 0;
        int num5 = attackInformation.IsVictimAgentLeftStance ? 1 : 0;
        MissionWeapon victimShield = attackInformation.VictimShield;
        ref AttackCollisionData local = ref attackCollisionData;
        missionWeapon = attackerWeapon;
        WeaponComponentData currentUsageItem = missionWeapon.CurrentUsageItem;
        double baseMagnitude = (double) attackCollisionData.BaseMagnitude;
        Mission.ComputeBlowDamageOnShield(num1 != 0, num2 != 0, num3 != 0, num4 != 0, num5 != 0, victimShield, ref local, currentUsageItem, (float) baseMagnitude);
        attackCollisionData.AbsorbedByArmor = attackCollisionData.InflictedDamage;
      }
      else if (attackCollisionData.MissileBlockedWithWeapon)
      {
        attackCollisionData.InflictedDamage = 0;
        attackCollisionData.AbsorbedByArmor = 0;
      }
      else
        Mission.ComputeBlowDamage(ref attackInformation, ref attackCollisionData, in attackerWeapon, damageType, specialMagnitude, speedBonus, cancelDamage, out attackCollisionData.InflictedDamage, out attackCollisionData.AbsorbedByArmor);
      combatLog.InflictedDamage = attackCollisionData.InflictedDamage;
      combatLog.AbsorbedDamage = attackCollisionData.AbsorbedByArmor;
      combatLog.AttackProgress = attackCollisionData.AttackProgress;
    }

    private void PrintAttackCollisionResults(
      Agent attackerAgent,
      Agent victimAgent,
      GameEntity hitEntity,
      ref AttackCollisionData attackCollisionData,
      ref CombatLogData combatLog)
    {
      if ((!attackCollisionData.IsColliderAgent ? 0 : (!attackCollisionData.AttackBlockedWithShield ? 1 : 0)) == 0 || !attackerAgent.CanLogCombatFor && !victimAgent.CanLogCombatFor || victimAgent.State != AgentState.Active)
        return;
      this.AddCombatLogSafe(attackerAgent, victimAgent, hitEntity, combatLog);
    }

    private void AddCombatLogSafe(
      Agent attackerAgent,
      Agent victimAgent,
      GameEntity hitEntity,
      CombatLogData combatLog)
    {
      combatLog.SetVictimAgent(victimAgent);
      if (GameNetwork.IsServerOrRecorder)
      {
        CombatLogNetworkMessage logNetworkMessage = new CombatLogNetworkMessage(attackerAgent, victimAgent, hitEntity, combatLog);
        NetworkCommunicator communicator1 = (attackerAgent == null ? (Agent) null : (attackerAgent.IsHuman ? attackerAgent : attackerAgent.RiderAgent))?.MissionPeer?.Peer.Communicator as NetworkCommunicator;
        NetworkCommunicator communicator2 = (victimAgent == null ? (Agent) null : (victimAgent.IsHuman ? victimAgent : victimAgent.RiderAgent))?.MissionPeer?.Peer.Communicator as NetworkCommunicator;
        if (communicator1 != null && !communicator1.IsServerPeer)
        {
          GameNetwork.BeginModuleEventAsServer(communicator1);
          GameNetwork.WriteMessage((GameNetworkMessage) logNetworkMessage);
          GameNetwork.EndModuleEventAsServer();
        }
        if (communicator2 != null && !communicator2.IsServerPeer && communicator2 != communicator1)
        {
          GameNetwork.BeginModuleEventAsServer(communicator2);
          GameNetwork.WriteMessage((GameNetworkMessage) logNetworkMessage);
          GameNetwork.EndModuleEventAsServer();
        }
      }
      this._combatLogsCreated.Enqueue(combatLog);
    }

    public MissionObject CreateMissionObjectFromPrefab(
      string prefab,
      MatrixFrame frame)
    {
      if (GameNetwork.IsClientOrReplay)
        return (MissionObject) null;
      GameEntity gameEntity = GameEntity.Instantiate(this.Scene, prefab, frame);
      MissionObject firstScriptOfType1 = gameEntity.GetFirstScriptOfType<MissionObject>();
      List<MissionObjectId> childObjectIds = new List<MissionObjectId>();
      foreach (GameEntity child in gameEntity.GetChildren())
      {
        MissionObject firstScriptOfType2;
        if ((firstScriptOfType2 = child.GetFirstScriptOfType<MissionObject>()) != null)
          childObjectIds.Add(firstScriptOfType2.Id);
      }
      if (GameNetwork.IsServerOrRecorder)
      {
        GameNetwork.BeginBroadcastModuleEvent();
        GameNetwork.WriteMessage((GameNetworkMessage) new CreateMissionObject(firstScriptOfType1.Id, prefab, frame, childObjectIds));
        GameNetwork.EndBroadcastModuleEvent(GameNetwork.EventBroadcastFlags.AddToMissionRecord);
        this.AddDynamicallySpawnedMissionObjectInfo(new Mission.DynamicallyCreatedEntity(prefab, firstScriptOfType1.Id, frame, ref childObjectIds));
      }
      return firstScriptOfType1;
    }

    public IEnumerable<Agent> GetNearbyAllyAgents(
      Vec2 center,
      float radius,
      Team team)
    {
      return this.GetNearbyAgentsAux(center, radius, team.MBTeam, Mission.GetNearbyAgentsAuxType.Friend);
    }

    public IEnumerable<Agent> GetNearbyEnemyAgents(
      Vec2 center,
      float radius,
      Team team)
    {
      return this.GetNearbyAgentsAux(center, radius, team.MBTeam, Mission.GetNearbyAgentsAuxType.Enemy);
    }

    public IEnumerable<Agent> GetNearbyAgents(Vec2 center, float radius) => this.GetNearbyAgentsAux(center, radius, MBTeam.InvalidTeam, Mission.GetNearbyAgentsAuxType.All);

    internal event Func<WorldPosition, Team, bool> IsFormationUnitPositionAvailable_AdditionalCondition;

    public bool IsFormationUnitPositionAvailable(
      ref WorldPosition formationPosition,
      ref WorldPosition unitPosition,
      ref WorldPosition nearestAvailableUnitPosition,
      float manhattanDistance,
      Team team)
    {
      return formationPosition.IsValid && unitPosition.IsValid && (this.IsFormationUnitPositionAvailable_AdditionalCondition == null || this.IsFormationUnitPositionAvailable_AdditionalCondition(unitPosition, team)) && this.IsFormationUnitPositionAvailableAux(ref formationPosition, ref unitPosition, ref nearestAvailableUnitPosition, manhattanDistance);
    }

    public bool IsFormationUnitPositionAvailable(ref WorldPosition unitPosition, Team team)
    {
      WorldPosition formationPosition = unitPosition;
      float manhattanDistance = 1f;
      WorldPosition invalid = WorldPosition.Invalid;
      return this.IsFormationUnitPositionAvailable(ref formationPosition, ref unitPosition, ref invalid, manhattanDistance, team);
    }

    private void TickDebugAgents()
    {
    }

    public void AddTimerToDynamicEntity(GameEntity gameEntity, float timeToKill = 10f) => this._dynamicEntities.Add(new Mission.DynamicEntityInfo()
    {
      Entity = gameEntity,
      TimerToDisable = new Timer(MBCommon.GetTime(MBCommon.TimeType.Mission), timeToKill)
    });

    public void AddListener(IMissionListener listener) => this._listeners.Add(listener);

    public void RemoveListener(IMissionListener listener) => this._listeners.Remove(listener);

    public void OnAgentFleeing(Agent agent)
    {
      for (int index = this._missionBehaviourList.Count - 1; index >= 0; --index)
        this._missionBehaviourList[index].OnAgentFleeing(agent);
      if (agent.Formation == null)
        return;
      agent.Formation.Team.DetachmentManager.OnAgentRemoved(agent);
      agent.Formation = (Formation) null;
    }

    public void OnAgentPanicked(Agent agent)
    {
      for (int index = this._missionBehaviourList.Count - 1; index >= 0; --index)
        this._missionBehaviourList[index].OnAgentPanicked(agent);
    }

    public void SetFastForwardingFromUI(bool fastForwarding) => this.IsFastForward = fastForwarding;

    [UsedImplicitly]
    [MBCallback]
    internal static void DebugLogNativeMissionNetworkEvent(
      int eventEnum,
      string eventName,
      int bitCount)
    {
      int eventType = eventEnum + CompressionBasic.NetworkComponentEventTypeFromServerCompressionInfo.GetMaximumValue() + 1;
      DebugNetworkEventStatistics.StartEvent(eventName, eventType);
      DebugNetworkEventStatistics.AddDataToStatistic(bitCount);
      DebugNetworkEventStatistics.EndEvent();
    }

    [CommandLineFunctionality.CommandLineArgumentFunction("toggleDisableDying", "mission")]
    public static string ToggleDisableDying(List<string> strings)
    {
      if (GameNetwork.IsSessionActive)
        return "Does not work on multiplayer.";
      int result = 0;
      if (strings.Count > 0 && !int.TryParse(strings[0], out result))
        return "Please write the arguments in the correct format. Correct format is: 'toggleDisableDying [index]' or just 'toggleDisableDying' for making all agents invincible.";
      if (strings.Count == 0 || result == -1)
      {
        Mission.DisableDying = !Mission.DisableDying;
        return Mission.DisableDying ? "Dying disabled for all" : "Dying not disabled for all";
      }
      Agent agentWithIndex = Mission.Current.FindAgentWithIndex(result);
      agentWithIndex.SetInvulnerable(!agentWithIndex.Invulnerable);
      return "Disable Dying for agent " + result.ToString() + ": " + agentWithIndex.Invulnerable.ToString();
    }

    [CommandLineFunctionality.CommandLineArgumentFunction("killAgent", "mission")]
    public static string KillAgent(List<string> strings)
    {
      if (GameNetwork.IsSessionActive)
        return "Does not work on multiplayer.";
      int result;
      if (strings.Count == 0 || !int.TryParse(strings[0], out result))
        return "Please write the arguments in the correct format. Correct format is: 'killAgent [index]'";
      Agent agentWithIndex = Mission.Current.FindAgentWithIndex(result);
      if (agentWithIndex == null)
        return "Agent " + result.ToString() + " not found.";
      if (agentWithIndex.State != AgentState.Active)
        return "Agent " + result.ToString() + " already dead.";
      agentWithIndex.Die(new Blow(result));
      return "Agent " + result.ToString() + " died.";
    }

    [CommandLineFunctionality.CommandLineArgumentFunction("set_battering_ram_speed", "mission")]
    public static string IncreaseBatteringRamSpeeds(List<string> strings)
    {
      if (GameNetwork.IsSessionActive)
        return "Does not work on multiplayer.";
      float result;
      if (strings.Count == 0 || !float.TryParse(strings[0], out result))
        return "Please enter a speed value";
      foreach (MissionObject activeMissionObject in (IEnumerable<MissionObject>) Mission.Current.ActiveMissionObjects)
      {
        if (activeMissionObject.GameEntity.HasScriptOfType<BatteringRam>())
        {
          activeMissionObject.GameEntity.GetFirstScriptOfType<BatteringRam>().MovementComponent.MaxSpeed = result;
          activeMissionObject.GameEntity.GetFirstScriptOfType<BatteringRam>().MovementComponent.MinSpeed = result;
        }
      }
      return "Battering ram max speed increased.";
    }

    [CommandLineFunctionality.CommandLineArgumentFunction("set_siege_tower_speed", "mission")]
    public static string IncreaseSiegeTowerSpeed(List<string> strings)
    {
      if (GameNetwork.IsSessionActive)
        return "Does not work on multiplayer.";
      float result;
      if (strings.Count == 0 || !float.TryParse(strings[0], out result))
        return "Please enter a speed value";
      foreach (MissionObject activeMissionObject in (IEnumerable<MissionObject>) Mission.Current.ActiveMissionObjects)
      {
        if (activeMissionObject.GameEntity.HasScriptOfType<SiegeTower>())
          activeMissionObject.GameEntity.GetFirstScriptOfType<SiegeTower>().MovementComponent.MaxSpeed = result;
      }
      return "Siege tower max speed increased.";
    }

    [CommandLineFunctionality.CommandLineArgumentFunction("reload_managed_core_params", "game")]
    public static string LoadParamsDebug(List<string> strings)
    {
      if (GameNetwork.IsSessionActive)
        return "Does not work on multiplayer.";
      ManagedParameters.Instance.Initialize(ModuleHelper.GetXmlPath("Native", "managed_core_parameters"));
      return "Managed core parameters reloaded.";
    }

    public class MBBoundaryCollection : 
      IDictionary<string, ICollection<Vec2>>,
      ICollection<KeyValuePair<string, ICollection<Vec2>>>,
      IEnumerable<KeyValuePair<string, ICollection<Vec2>>>,
      IEnumerable,
      INotifyCollectionChanged
    {
      private Mission _mission;

      IEnumerator IEnumerable.GetEnumerator()
      {
        int count = this.Count;
        for (int i = 0; i < count; ++i)
        {
          string boundaryName = MBAPI.IMBMission.GetBoundaryName(this._mission.Pointer, i);
          List<Vec2> boundaryPoints = this.GetBoundaryPoints(boundaryName);
          yield return (object) new KeyValuePair<string, ICollection<Vec2>>(boundaryName, (ICollection<Vec2>) boundaryPoints);
        }
      }

      public IEnumerator<KeyValuePair<string, ICollection<Vec2>>> GetEnumerator()
      {
        int count = this.Count;
        for (int i = 0; i < count; ++i)
        {
          string boundaryName = MBAPI.IMBMission.GetBoundaryName(this._mission.Pointer, i);
          List<Vec2> boundaryPoints = this.GetBoundaryPoints(boundaryName);
          yield return new KeyValuePair<string, ICollection<Vec2>>(boundaryName, (ICollection<Vec2>) boundaryPoints);
        }
      }

      public int Count => MBAPI.IMBMission.GetBoundaryCount(this._mission.Pointer);

      public float GetBoundaryRadius(string name) => MBAPI.IMBMission.GetBoundaryRadius(this._mission.Pointer, name);

      public bool IsReadOnly => false;

      internal MBBoundaryCollection(Mission mission) => this._mission = mission;

      public void Add(KeyValuePair<string, ICollection<Vec2>> item) => this.Add(item.Key, item.Value);

      public void Clear()
      {
        foreach (KeyValuePair<string, ICollection<Vec2>> keyValuePair in this)
          this.Remove(keyValuePair.Key);
      }

      public bool Contains(KeyValuePair<string, ICollection<Vec2>> item) => this.ContainsKey(item.Key);

      public void CopyTo(KeyValuePair<string, ICollection<Vec2>>[] array, int arrayIndex)
      {
        if (array == null)
          throw new ArgumentNullException(nameof (array));
        if (arrayIndex < 0)
          throw new ArgumentOutOfRangeException(nameof (arrayIndex));
        foreach (KeyValuePair<string, ICollection<Vec2>> keyValuePair in this)
        {
          array[arrayIndex] = keyValuePair;
          ++arrayIndex;
          if (arrayIndex >= array.Length)
            throw new ArgumentException("Not enough size in array.");
        }
      }

      public bool Remove(KeyValuePair<string, ICollection<Vec2>> item) => this.Remove(item.Key);

      public ICollection<string> Keys
      {
        get
        {
          List<string> stringList = new List<string>();
          foreach (KeyValuePair<string, ICollection<Vec2>> keyValuePair in this)
            stringList.Add(keyValuePair.Key);
          return (ICollection<string>) stringList;
        }
      }

      public ICollection<ICollection<Vec2>> Values
      {
        get
        {
          List<ICollection<Vec2>> vec2sList = new List<ICollection<Vec2>>();
          foreach (KeyValuePair<string, ICollection<Vec2>> keyValuePair in this)
            vec2sList.Add(keyValuePair.Value);
          return (ICollection<ICollection<Vec2>>) vec2sList;
        }
      }

      public ICollection<Vec2> this[string name]
      {
        get
        {
          List<Vec2> vec2List = name != null ? this.GetBoundaryPoints(name) : throw new ArgumentNullException(nameof (name));
          return vec2List.Count != 0 ? (ICollection<Vec2>) vec2List : throw new KeyNotFoundException();
        }
        set
        {
          if (name == null)
            throw new ArgumentNullException(nameof (name));
          this.Add(name, value);
        }
      }

      public void Add(string name, ICollection<Vec2> points) => this.Add(name, points, true);

      public void Add(string name, ICollection<Vec2> points, bool isAllowanceInside)
      {
        if (name == null)
          throw new ArgumentNullException(nameof (name));
        if (points == null)
          throw new ArgumentNullException(nameof (points));
        if (points.Count < 3)
          throw new ArgumentException("At least three points are required.");
        int num = MBAPI.IMBMission.AddBoundary(this._mission.Pointer, name, points.ToArray<Vec2>(), points.Count, isAllowanceInside) ? 1 : 0;
        if (num == 0)
          throw new ArgumentException("An element with the same name already exists.");
        if (num == 0 || this.CollectionChanged == null)
          return;
        this.CollectionChanged((object) this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, (object) name));
      }

      public bool ContainsKey(string name)
      {
        if (name == null)
          throw new ArgumentNullException(nameof (name));
        return this.GetBoundaryPoints(name).Count > 0;
      }

      public bool Remove(string name)
      {
        if (name == null)
          throw new ArgumentNullException(nameof (name));
        int num = MBAPI.IMBMission.RemoveBoundary(this._mission.Pointer, name) ? 1 : 0;
        if (num == 0)
          return num != 0;
        if (this.CollectionChanged == null)
          return num != 0;
        this.CollectionChanged((object) this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, (object) name));
        return num != 0;
      }

      public bool TryGetValue(string name, out ICollection<Vec2> points)
      {
        points = name != null ? (ICollection<Vec2>) this.GetBoundaryPoints(name) : throw new ArgumentNullException(nameof (name));
        return points.Count > 0;
      }

      private List<Vec2> GetBoundaryPoints(string name)
      {
        List<Vec2> vec2List = new List<Vec2>();
        Vec2[] boundaryPoints = new Vec2[10];
        for (int boundaryPointOffset = 0; boundaryPointOffset < 1000; boundaryPointOffset += 10)
        {
          int retrievedPointCount = -1;
          MBAPI.IMBMission.GetBoundaryPoints(this._mission.Pointer, name, boundaryPointOffset, boundaryPoints, 10, ref retrievedPointCount);
          vec2List.AddRange(((IEnumerable<Vec2>) boundaryPoints).Take<Vec2>(retrievedPointCount));
          if (retrievedPointCount < 10)
            break;
        }
        return vec2List;
      }

      public event NotifyCollectionChangedEventHandler CollectionChanged;
    }

    public class DynamicallyCreatedEntity
    {
      public string Prefab;
      public MissionObjectId ObjectId;
      public MatrixFrame Frame;
      public List<MissionObjectId> ChildObjectIds;

      public DynamicallyCreatedEntity(
        string prefab,
        MissionObjectId objectId,
        MatrixFrame frame,
        ref List<MissionObjectId> childObjectIds)
      {
        this.Prefab = prefab;
        this.ObjectId = objectId;
        this.Frame = frame;
        this.ChildObjectIds = childObjectIds;
      }
    }

    [Flags]
    public enum WeaponSpawnFlags
    {
      None = 0,
      WithHolster = 1,
      WithoutHolster = 2,
      AsMissile = 4,
      WithPhysics = 8,
      WithStaticPhysics = 16, // 0x00000010
      UseAnimationSpeed = 32, // 0x00000020
      CannotBePickedUp = 64, // 0x00000040
    }

    [EngineStruct("Mission_combat_type")]
    public enum MissionCombatType
    {
      Combat,
      ArenaCombat,
      NoCombat,
    }

    [EngineStruct("Agent_creation_result")]
    internal struct AgentCreationResult
    {
      internal int Index;
      internal UIntPtr AgentPtr;
      internal UIntPtr PositionPtr;
      internal UIntPtr IndexPtr;
      internal UIntPtr FlagsPtr;
      internal UIntPtr StatePtr;
    }

    private enum GetNearbyAgentsAuxType
    {
      Friend = 1,
      Enemy = 2,
      All = 3,
    }

    public class Missile : MBMissile
    {
      public Missile(Mission mission, GameEntity entity)
        : base(mission)
      {
        this.Entity = entity;
      }

      public GameEntity Entity { get; private set; }

      public MissionWeapon Weapon { get; set; }

      public Agent ShooterAgent { get; set; }

      public MissionObject MissionObjectToIgnore { get; set; }

      public void CalculateBounceBackVelocity(
        Vec3 rotationSpeed,
        AttackCollisionData collisionData,
        out Vec3 velocity,
        out Vec3 angularVelocity)
      {
        Vec3 missileVelocity = collisionData.MissileVelocity;
        MissionWeapon weapon = this.Weapon;
        double num1 = (double) weapon.CurrentUsageItem.WeaponLength * 0.00999999977648258;
        weapon = this.Weapon;
        double scaleFactor = (double) weapon.Item.ScaleFactor;
        float num2 = (float) (num1 * scaleFactor);
        PhysicsMaterial fromIndex = PhysicsMaterial.GetFromIndex(collisionData.PhysicsMaterialIndex);
        float num3;
        float num4;
        if (fromIndex.IsValid)
        {
          num3 = fromIndex.GetDynamicFriction();
          num4 = fromIndex.GetRestitution();
        }
        else
        {
          num3 = 0.3f;
          num4 = 0.4f;
        }
        PhysicsMaterial fromName = PhysicsMaterial.GetFromName(this.Weapon.Item.PrimaryWeapon.PhysicsMaterial);
        float num5;
        float num6;
        if (fromName.IsValid)
        {
          num5 = fromName.GetDynamicFriction();
          num6 = fromName.GetRestitution();
        }
        else
        {
          num5 = 0.3f;
          num6 = 0.4f;
        }
        float num7 = (float) (((double) num3 + (double) num5) * 0.5);
        float num8 = (float) (((double) num4 + (double) num6) * 0.5);
        Vec3 vec3_1 = missileVelocity.Reflect(collisionData.CollisionGlobalNormal);
        float num9 = Vec3.DotProduct(vec3_1, collisionData.CollisionGlobalNormal);
        Vec3 v2 = collisionData.CollisionGlobalNormal.RotateAboutAnArbitraryVector(Vec3.CrossProduct(vec3_1, collisionData.CollisionGlobalNormal).NormalizedCopy(), 1.570796f);
        float num10 = Vec3.DotProduct(vec3_1, v2);
        velocity = collisionData.CollisionGlobalNormal * (num8 * num9) + v2 * (num10 * num7);
        velocity += collisionData.CollisionGlobalNormal;
        angularVelocity = -Vec3.CrossProduct(collisionData.CollisionGlobalNormal, velocity);
        float lengthSquared = angularVelocity.LengthSquared;
        float weight = this.Weapon.GetWeight();
        float num11;
        switch (this.Weapon.CurrentUsageItem.WeaponClass)
        {
          case WeaponClass.Arrow:
          case WeaponClass.Bolt:
            num11 = (float) (0.25 * (double) weight * 0.0549999997019768 * 0.0549999997019768 + 0.0833333283662796 * (double) weight * (double) num2 * (double) num2);
            break;
          case WeaponClass.Stone:
            float num12 = 0.1f;
            num11 = 0.4f * weight * num12 * num12;
            break;
          case WeaponClass.Boulder:
            float num13 = 0.4f;
            num11 = 0.4f * weight * num13 * num13;
            break;
          case WeaponClass.ThrowingAxe:
            float num14 = 0.2f;
            num11 = (float) (0.25 * (double) weight * (double) num14 * (double) num14 + 0.0833333283662796 * (double) weight * (double) num2 * (double) num2) + 0.5f * weight * num14 * num14;
            Vec3 vec3_2 = rotationSpeed * num4;
            angularVelocity = this.Entity.GetGlobalFrame().rotation.TransformToParent(rotationSpeed * num4);
            break;
          case WeaponClass.ThrowingKnife:
            float num15 = 0.2f;
            num11 = (float) (0.25 * (double) weight * (double) num15 * (double) num15 + 0.0833333283662796 * (double) weight * (double) num2 * (double) num2) + 0.5f * weight * num15 * num15;
            Vec3 vec3_3 = rotationSpeed * num4;
            angularVelocity = this.Entity.GetGlobalFrame().rotation.TransformToParent(rotationSpeed * num4);
            break;
          case WeaponClass.Javelin:
            float num16 = 0.155f;
            num11 = (float) (0.25 * (double) weight * (double) num16 * (double) num16 + 0.0833333283662796 * (double) weight * (double) num2 * (double) num2);
            break;
          default:
            num11 = 0.0f;
            break;
        }
        float num17 = 0.5f * num11 * lengthSquared;
        float length = missileVelocity.Length;
        float num18 = MathF.Sqrt((float) ((0.5 * (double) weight * (double) length * (double) length - (double) num17) * 2.0) / weight);
        velocity *= num18 / length;
      }
    }

    public class SpectatorData
    {
      public Agent AgentToFollow { get; private set; }

      public IAgentVisual AgentVisualToFollow { get; private set; }

      public SpectatorCameraTypes CameraType { get; private set; }

      public SpectatorData(
        Agent agentToFollow,
        IAgentVisual agentVisualToFollow,
        SpectatorCameraTypes cameraType)
      {
        this.AgentToFollow = agentToFollow;
        this.CameraType = cameraType;
        this.AgentVisualToFollow = agentVisualToFollow;
      }
    }

    public enum State
    {
      NewlyCreated,
      Initializing,
      Continuing,
      EndingNextFrame,
      Over,
    }

    public enum MissileCollisionReaction
    {
      Invalid = -1, // 0xFFFFFFFF
      Stick = 0,
      PassThrough = 1,
      BounceBack = 2,
      BecomeInvisible = 3,
      Count = 4,
    }

    public delegate void OnBeforeAgentRemovedDelegate(
      Agent affectedAgent,
      Agent affectorAgent,
      AgentState agentState,
      KillingBlow killingBlow);

    public enum BattleSizeQualifier
    {
      Small,
      Medium,
    }

    public enum MissionTeamAITypeEnum
    {
      NoTeamAI,
      FieldBattle,
      Siege,
      SallyOut,
    }

    private class DynamicEntityInfo
    {
      public GameEntity Entity;
      public Timer TimerToDisable;
    }

    public sealed class TeamCollection : ReadOnlyCollection<Team>
    {
      private Mission mission;
      private Team _playerTeam;

      public event Action<Team, Team> OnPlayerTeamChanged;

      public Team this[MBTeam team] => this.Find(team);

      public Team Attacker { get; private set; }

      public Team Defender { get; private set; }

      public Team AttackerAlly { get; private set; }

      public Team DefenderAlly { get; private set; }

      public Team Player
      {
        get => this._playerTeam;
        set
        {
          if (this._playerTeam == value)
            return;
          this.SetPlayerTeamAux(value == null ? -1 : this.Items.IndexOf(value));
        }
      }

      public Team PlayerEnemy { get; private set; }

      public Team PlayerAlly { get; private set; }

      internal TeamCollection(Mission mission)
        : base((IList<Team>) new List<Team>())
      {
        this.mission = mission;
      }

      private MBTeam AddNative() => new MBTeam(this.mission, MBAPI.IMBMission.AddTeam(this.mission.Pointer));

      public Team Add(
        BattleSideEnum side,
        uint color = 4294967295,
        uint color2 = 4294967295,
        Banner banner = null,
        bool isPlayerGeneral = true,
        bool isPlayerSergeant = false,
        bool isSettingRelations = true)
      {
        MBDebug.Print("----------Mission-AddTeam-" + (object) side);
        Team team = new Team(this.AddNative(), side, color, color2, banner);
        if (!GameNetwork.IsClientOrReplay)
          team.SetPlayerRole(isPlayerGeneral, isPlayerSergeant);
        this.Items.Add(team);
        foreach (MissionBehaviour missionBehaviour in this.mission.MissionBehaviours)
          missionBehaviour.OnAddTeam(team);
        if (isSettingRelations)
          this.SetRelations(team);
        switch (side)
        {
          case BattleSideEnum.Defender:
            if (this.Defender == null)
            {
              this.Defender = team;
              break;
            }
            if (this.DefenderAlly == null)
            {
              this.DefenderAlly = team;
              break;
            }
            break;
          case BattleSideEnum.Attacker:
            if (this.Attacker == null)
            {
              this.Attacker = team;
              break;
            }
            if (this.AttackerAlly == null)
            {
              this.AttackerAlly = team;
              break;
            }
            break;
        }
        this.AdjustPlayerTeams();
        foreach (MissionBehaviour missionBehaviour in this.mission.MissionBehaviours)
          missionBehaviour.AfterAddTeam(team);
        return team;
      }

      public Team Find(MBTeam mbTeam) => !mbTeam.IsValid ? Team.Invalid : this.Items.Single<Team>((Func<Team, bool>) (t => t.MBTeam == mbTeam));

      internal void ClearResources()
      {
        this.Attacker = (Team) null;
        this.AttackerAlly = (Team) null;
        this.Defender = (Team) null;
        this.DefenderAlly = (Team) null;
        this._playerTeam = (Team) null;
        this.PlayerEnemy = (Team) null;
        this.PlayerAlly = (Team) null;
        Team.Invalid = (Team) null;
      }

      public void Clear()
      {
        foreach (Team team in (IEnumerable<Team>) this.Items)
          team.Clear();
        this.Items.Clear();
        this.ClearResources();
        MBAPI.IMBMission.ResetTeams(this.mission.Pointer);
      }

      private void SetRelations(Team team)
      {
        foreach (Team otherTeam in this.Items.Where<Team>((Func<Team, bool>) (i => team.Side.IsOpponentOf(i.Side))))
          team.SetIsEnemyOf(otherTeam, true);
      }

      private void SetPlayerTeamAux(int index)
      {
        Team playerTeam = this._playerTeam;
        this._playerTeam = index == -1 ? (Team) null : this.Items[index];
        this.AdjustPlayerTeams();
        Action<Team, Team> playerTeamChanged = this.OnPlayerTeamChanged;
        if (playerTeamChanged == null)
          return;
        playerTeamChanged(playerTeam, this._playerTeam);
      }

      private void AdjustPlayerTeams()
      {
        if (this.Player == null)
        {
          this.PlayerEnemy = (Team) null;
          this.PlayerAlly = (Team) null;
        }
        else if (this.Player == this.Attacker)
        {
          this.PlayerEnemy = this.Defender == null || !this.Player.IsEnemyOf(this.Defender) ? (Team) null : this.Defender;
          if (this.AttackerAlly != null && this.Player.IsFriendOf(this.AttackerAlly))
            this.PlayerAlly = this.AttackerAlly;
          else
            this.PlayerAlly = (Team) null;
        }
        else
        {
          if (this.Player != this.Defender)
            return;
          this.PlayerEnemy = this.Attacker == null || !this.Player.IsEnemyOf(this.Attacker) ? (Team) null : this.Attacker;
          if (this.DefenderAlly != null && this.Player.IsFriendOf(this.DefenderAlly))
            this.PlayerAlly = this.DefenderAlly;
          else
            this.PlayerAlly = (Team) null;
        }
      }

      private int TeamCountNative => MBAPI.IMBMission.GetNumberOfTeams(this.mission.Pointer);

      public IEnumerable<Team> GetEnemiesOf(Team team) => this.Items.Where<Team>((Func<Team, bool>) (t => t != team && t.IsEnemyOf(team)));

      public IEnumerable<Team> GetAlliesOf(Team team, bool includeItself = false) => includeItself ? this.Items.Where<Team>((Func<Team, bool>) (t => t.IsFriendOf(team))) : this.Items.Where<Team>((Func<Team, bool>) (t => t != team && t.IsFriendOf(team)));
    }
  }
}
