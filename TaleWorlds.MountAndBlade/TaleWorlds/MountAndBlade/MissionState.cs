// Decompiled with JetBrains decompiler
// Type: TaleWorlds.MountAndBlade.MissionState
// Assembly: TaleWorlds.MountAndBlade, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3C0E3F42-756F-4407-B817-06BA412A78B0
// Assembly location: E:\SteamLibrary\steamapps\common\Mount & Blade II Bannerlord\bin\Win64_Shipping_Client\TaleWorlds.MountAndBlade.dll

using NetworkMessages.FromClient;
using System;
using System.Collections.Generic;
using System.Linq;
using TaleWorlds.Core;
using TaleWorlds.Engine;
using TaleWorlds.MountAndBlade.Network.Messages;
using TaleWorlds.MountAndBlade.Source.Missions.Handlers;

namespace TaleWorlds.MountAndBlade
{
  public class MissionState : GameState
  {
    private const int MissionFastForwardSpeedMultiplier = 10;
    private bool _missionInitializing;
    private bool _firstMissionTickAfterLoading = true;
    private int _tickCountBeforeLoad;
    public static bool RecordMission;
    public float MissionFastForwardAmount;
    public float MissionEndTime;
    private int _missionTickCount;

    public IMissionSystemHandler Handler { get; set; }

    public override bool IsMission => true;

    public static MissionState Current { get; private set; }

    public Mission CurrentMission { get; private set; }

    public string MissionName { get; private set; }

    public bool Paused { get; set; }

    protected override void OnInitialize()
    {
      base.OnInitialize();
      MissionState.Current = this;
    }

    protected override void OnFinalize()
    {
      base.OnFinalize();
      this.CurrentMission.ClearResources(this.CurrentMission.NeedsMemoryCleanup);
      this.CurrentMission = (Mission) null;
      MissionState.Current = (MissionState) null;
    }

    protected override void OnActivate()
    {
      base.OnActivate();
      if (this.CurrentMission?.MissionBehaviours == null)
        return;
      foreach (MissionBehaviour missionBehaviour in this.CurrentMission.MissionBehaviours)
        missionBehaviour.OnMissionActivate();
    }

    protected override void OnDeactivate()
    {
      base.OnDeactivate();
      if (this.CurrentMission == null || this.CurrentMission.MissionBehaviours == null)
        return;
      foreach (MissionBehaviour missionBehaviour in this.CurrentMission.MissionBehaviours)
        missionBehaviour.OnMissionDeactivate();
    }

    protected override void OnTick(float realDt)
    {
      base.OnTick(realDt);
      if (this.CurrentMission.CurrentState == Mission.State.NewlyCreated || this.CurrentMission.CurrentState == Mission.State.Initializing)
      {
        if (this.CurrentMission.CurrentState == Mission.State.NewlyCreated)
          this.CurrentMission.ClearUnreferencedResources(this.CurrentMission.NeedsMemoryCleanup);
        this.TickLoading(realDt);
      }
      else if (this.CurrentMission.CurrentState == Mission.State.Continuing || this.CurrentMission.MissionEnded())
      {
        if ((double) this.MissionFastForwardAmount != 0.0)
        {
          this.CurrentMission.FastForwardMission(this.MissionFastForwardAmount, 0.033f);
          this.MissionFastForwardAmount = 0.0f;
        }
        bool flag = false;
        if ((double) this.MissionEndTime != 0.0 && (double) this.CurrentMission.Time > (double) this.MissionEndTime)
        {
          this.CurrentMission.EndMission();
          flag = true;
        }
        if (!flag && (this.Handler == null || this.Handler.RenderIsReady()))
          this.TickMission(realDt);
        if (flag && MBEditor._isEditorMissionOn)
        {
          MBEditor.LeaveEditMissionMode();
          this.TickMission(realDt);
        }
      }
      if (this.CurrentMission.CurrentState != Mission.State.Over)
        return;
      Game.Current.GameStateManager.PopState();
    }

    private void TickMission(float realDt)
    {
      if (this._firstMissionTickAfterLoading && this.CurrentMission != null && this.CurrentMission.CurrentState == Mission.State.Continuing)
      {
        if (GameNetwork.IsClient)
        {
          MBDebug.Print("Client: I finished loading. Sending confirmation to server.", debugFilter: 17179869184UL);
          GameNetwork.BeginModuleEventAsClient();
          GameNetwork.WriteMessage((GameNetworkMessage) new FinishedLoading());
          GameNetwork.EndModuleEventAsClient();
          GameNetwork.SyncRelevantGameOptionsToServer();
        }
        this._firstMissionTickAfterLoading = false;
      }
      if (Game.Current.DeterministicMode)
        Game.Current.ResetRandomGenerator(this._missionTickCount);
      this.Handler?.BeforeMissionTick(this.CurrentMission, realDt);
      this.CurrentMission.PauseAITick = false;
      if (GameNetwork.IsSessionActive && (double) this.CurrentMission.ClearSceneTimerElapsedTime < 0.0)
        this.CurrentMission.PauseAITick = true;
      float dt = realDt;
      if (this.Paused || MBCommon.IsPaused)
      {
        dt = 0.0f;
      }
      else
      {
        if (this.CurrentMission.FixedDeltaTimeMode)
          dt = this.CurrentMission.FixedDeltaTime;
        if (this.CurrentMission.Scene.SlowMotionMode)
          dt *= this.CurrentMission.Scene.SlowMotionFactor;
      }
      if (!GameNetwork.IsSessionActive && NativeConfig.CheatMode)
      {
        if ((double) this.CurrentMission.TimeSpeedPeriod > 0.0)
        {
          float num = this.CurrentMission.TimeSpeedTimerElapsedTime / this.CurrentMission.TimeSpeedPeriod;
          if ((double) num >= 1.0)
          {
            this.CurrentMission.TimeSpeed = this.CurrentMission.TimeSpeedEnd;
            dt *= this.CurrentMission.TimeSpeed;
            this.CurrentMission.TimeSpeedPeriod = 0.0f;
          }
          else
            dt *= (float) ((double) this.CurrentMission.TimeSpeedEnd * (double) num + (double) this.CurrentMission.TimeSpeed * (1.0 - (double) num));
        }
        else
          dt *= this.CurrentMission.TimeSpeed;
      }
      if ((double) this.CurrentMission.ClearSceneTimerElapsedTime < -0.300000011920929 && !GameNetwork.IsClientOrReplay)
        this.CurrentMission.ClearAgentActions();
      if (this.CurrentMission.CurrentState == Mission.State.Continuing || this.CurrentMission.MissionEnded())
      {
        if (this.CurrentMission.IsFastForward)
        {
          float num = dt * 9f;
          while ((double) num > 9.99999997475243E-07)
          {
            if ((double) num > 0.100000001490116)
            {
              this.TickMissionAux(0.1f, 0.1f, false);
              if (this.CurrentMission.CurrentState != Mission.State.Over)
                num -= 0.1f;
              else
                break;
            }
            else
            {
              if ((double) num > 1.0 / 300.0)
                this.TickMissionAux(num, num, false);
              num = 0.0f;
            }
          }
          if (this.CurrentMission.CurrentState != Mission.State.Over)
            this.TickMissionAux(dt, realDt, true);
        }
        else
          this.TickMissionAux(dt, realDt, true);
      }
      if (this.Handler != null)
        this.Handler.AfterMissionTick(this.CurrentMission, realDt);
      ++this._missionTickCount;
      int num1 = Game.Current.DeterministicMode ? 1 : 0;
    }

    private void TickMissionAux(float dt, float realDt, bool updateCamera)
    {
      this.CurrentMission.Tick(dt);
      if (this._missionTickCount <= 2)
        return;
      this.CurrentMission.OnTick(dt, realDt, updateCamera);
    }

    private void TickLoading(float realDt)
    {
      ++this._tickCountBeforeLoad;
      if (!this._missionInitializing && this._tickCountBeforeLoad > 0)
      {
        this.LoadMission();
      }
      else
      {
        if (!this._missionInitializing || !this.CurrentMission.IsLoadingFinished)
          return;
        this.FinishMissionLoading();
      }
    }

    private void LoadMission()
    {
      this.Handler?.OnPreLoad(this.CurrentMission);
      Utilities.ClearOldResourcesAndObjects();
      this._missionInitializing = true;
      this.CurrentMission.Initialize();
    }

    private void CreateMission(MissionInitializerRecord rec) => this.CurrentMission = new Mission(rec, this);

    private Mission HandleOpenNew(
      string missionName,
      MissionInitializerRecord rec,
      InitializeMissionBehvaioursDelegate handler,
      bool addDefaultMissionBehaviors)
    {
      this.MissionName = missionName;
      this.CreateMission(rec);
      IEnumerable<MissionBehaviour> behaviours = handler(this.CurrentMission);
      if (addDefaultMissionBehaviors)
        behaviours = MissionState.AddDefaultMissionBehavioursTo(this.CurrentMission, behaviours);
      foreach (MissionBehaviour missionBehaviour in behaviours)
        missionBehaviour.OnAfterMissionCreated();
      this.AddBehavioursToMission(behaviours);
      if (this.Handler != null)
        this.AddBehavioursToMission(this.Handler.OnAddBehaviours((IEnumerable<MissionBehaviour>) new MissionBehaviour[0], this.CurrentMission, missionName, addDefaultMissionBehaviors));
      return this.CurrentMission;
    }

    private void AddBehavioursToMission(IEnumerable<MissionBehaviour> behaviours) => this.CurrentMission.InitializeStartingBehaviours(behaviours.OfType<MissionLogic>().Where<MissionLogic>((Func<MissionLogic, bool>) (behaviour => !(behaviour is MissionNetwork))).ToArray<MissionLogic>(), behaviours.Where<MissionBehaviour>((Func<MissionBehaviour, bool>) (behaviour =>
    {
      switch (behaviour)
      {
        case null:
        case MissionNetwork _:
          return false;
        default:
          return !(behaviour is MissionLogic);
      }
    })).ToArray<MissionBehaviour>(), behaviours.OfType<MissionNetwork>().ToArray<MissionNetwork>());

    private static bool IsRecordingActive()
    {
      if (GameNetwork.IsServer)
        return MultiplayerOptions.OptionType.EnableMissionRecording.GetBoolValue();
      return MissionState.RecordMission && Game.Current.GameType.IsCoreOnlyGameMode;
    }

    public static Mission OpenNew(
      string missionName,
      MissionInitializerRecord rec,
      InitializeMissionBehvaioursDelegate handler,
      bool addDefaultMissionBehaviours = true,
      bool needsMemoryCleanup = true)
    {
      if (!GameNetwork.IsClientOrReplay && !GameNetwork.IsServer)
        MBCommon.CurrentGameType = MissionState.IsRecordingActive() ? MBCommon.GameType.SingleRecord : MBCommon.GameType.Single;
      Game.Current.OnMissionIsStarting(missionName, rec);
      MissionState state = Game.Current.GameStateManager.CreateState<MissionState>();
      Game.Current.GameStateManager.PushState((GameState) state);
      Mission mission = state.HandleOpenNew(missionName, rec, handler, addDefaultMissionBehaviours);
      mission.NeedsMemoryCleanup = needsMemoryCleanup;
      return mission;
    }

    private static IEnumerable<MissionBehaviour> AddDefaultMissionBehavioursTo(
      Mission mission,
      IEnumerable<MissionBehaviour> behaviours)
    {
      List<MissionBehaviour> first = new List<MissionBehaviour>();
      if (GameNetwork.IsSessionActive || GameNetwork.IsReplay)
        first.Add((MissionBehaviour) new MissionNetworkComponent());
      if (MissionState.IsRecordingActive() && !GameNetwork.IsReplay)
        first.Add((MissionBehaviour) new RecordMissionLogic());
      first.Add((MissionBehaviour) new BasicMissionHandler());
      first.Add((MissionBehaviour) new CasualtyHandler());
      first.Add((MissionBehaviour) new AgentDefaultAILogic());
      return first.Concat<MissionBehaviour>(behaviours);
    }

    private void FinishMissionLoading()
    {
      this._missionInitializing = false;
      this.CurrentMission.Scene.SetOwnerThread();
      for (int index = 0; index < 2; ++index)
        this.CurrentMission.Tick(1f / 1000f);
      this.Handler?.OnMissionAfterStarting(this.CurrentMission);
      this.CurrentMission.AfterStart();
      this.Handler?.OnMissionLoadingFinished(this.CurrentMission);
      this.CurrentMission.Scene.ResumeLoadingRenderings();
    }
  }
}
