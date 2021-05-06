// Decompiled with JetBrains decompiler
// Type: TaleWorlds.Core.GameStateManager
// Assembly: TaleWorlds.Core, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 43F7B45C-9AE1-4715-A37B-2364840F62AF
// Assembly location: E:\SteamLibrary\steamapps\common\Mount & Blade II Bannerlord\bin\Win64_Shipping_Client\TaleWorlds.Core.dll

using System;
using System.Collections.Generic;
using System.Linq;
using TaleWorlds.Library;

namespace TaleWorlds.Core
{
  public class GameStateManager
  {
    private static GameStateManager _current;
    public static string StateActivateCommand;
    private IGameStateManagerListener _gameStateManagerListener;
    private List<GameState> _gameStates;
    private bool _activeStateDisabledByUser;
    private Queue<GameStateManager.GameStateJob> _gameStateJobs;

    public static GameStateManager Current
    {
      get => GameStateManager._current;
      set
      {
        if (GameStateManager._current != null)
          GameStateManager._current.CleanStates();
        GameStateManager._current = value;
      }
    }

    public GameStateManager.GameStateManagerType CurrentType { get; private set; }

    public IGameStateManagerOwner Owner { get; private set; }

    public IGameStateManagerListener GameStateManagerListener
    {
      get => this._gameStateManagerListener;
      set => this._gameStateManagerListener = value;
    }

    public IEnumerable<GameState> GameStates => (IEnumerable<GameState>) this._gameStates.AsReadOnly();

    public GameState ActiveState => this._gameStates.Count > 0 ? this._gameStates.Last<GameState>() : (GameState) null;

    public bool ActiveStateDisabledByUser
    {
      get => this._activeStateDisabledByUser;
      set
      {
        Debug.Print("ActiveStateDisabledByUser being set to: " + value.ToString());
        if (value == this._activeStateDisabledByUser)
          return;
        this._activeStateDisabledByUser = value;
      }
    }

    public GameStateManager(
      IGameStateManagerOwner owner,
      GameStateManager.GameStateManagerType gameStateManagerType)
    {
      this.Owner = owner;
      this.CurrentType = gameStateManagerType;
      this._gameStateJobs = new Queue<GameStateManager.GameStateJob>();
      this._gameStateManagerListener = (IGameStateManagerListener) null;
      this._gameStates = new List<GameState>();
    }

    internal GameState FindPredecessor(GameState gameState)
    {
      GameState gameState1 = (GameState) null;
      int num = this._gameStates.IndexOf(gameState);
      if (num > 0)
        gameState1 = this._gameStates[num - 1];
      return gameState1;
    }

    public void OnSavedGameLoadFinished()
    {
      if (this._gameStateManagerListener == null)
        return;
      this._gameStateManagerListener.OnSavedGameLoadFinished();
    }

    public T LastOrDefault<T>() where T : GameState => this._gameStates.LastOrDefault<GameState>((Func<GameState, bool>) (g => g is T)) as T;

    public T CreateState<T>() where T : GameState, new()
    {
      T obj = new T();
      this.HandleCreateState((GameState) obj);
      return obj;
    }

    public T CreateState<T>(params object[] parameters) where T : GameState, new()
    {
      GameState instance = (GameState) Activator.CreateInstance(typeof (T), parameters);
      this.HandleCreateState(instance);
      return (T) instance;
    }

    private void HandleCreateState(GameState state)
    {
      state.GameStateManager = this;
      if (this._gameStateManagerListener == null)
        return;
      this._gameStateManagerListener.OnCreateState(state);
    }

    public void OnTick(float dt)
    {
      if (this.ActiveState == null)
        return;
      if (this.ActiveStateDisabledByUser)
        this.ActiveState.OnIdleTick(dt);
      else
        this.ActiveState.OnTick(dt);
    }

    public void PushState(GameState gameState, int level = 0)
    {
      this._gameStateJobs.Enqueue(new GameStateManager.GameStateJob(GameStateManager.GameStateJob.JobType.Push, gameState, level));
      this.DoGameStateJobs();
    }

    public void PopState(int level = 0)
    {
      this._gameStateJobs.Enqueue(new GameStateManager.GameStateJob(GameStateManager.GameStateJob.JobType.Pop, (GameState) null, level));
      this.DoGameStateJobs();
    }

    public void CleanAndPushState(GameState gameState, int level = 0)
    {
      this._gameStateJobs.Enqueue(new GameStateManager.GameStateJob(GameStateManager.GameStateJob.JobType.CleanAndPushState, gameState, level));
      this.DoGameStateJobs();
    }

    public void CleanStates(int level = 0)
    {
      this._gameStateJobs.Enqueue(new GameStateManager.GameStateJob(GameStateManager.GameStateJob.JobType.CleanStates, (GameState) null, level));
      this.DoGameStateJobs();
    }

    private void OnPushState(GameState gameState)
    {
      GameState activeState1 = this.ActiveState;
      bool isTopGameState = this._gameStates.Count == 0;
      int lastIndex = this._gameStates.FindLastIndex((Predicate<GameState>) (state => state.Level <= gameState.Level));
      if (lastIndex == -1)
        this._gameStates.Add(gameState);
      else
        this._gameStates.Insert(lastIndex + 1, gameState);
      GameState activeState2 = this.ActiveState;
      if (activeState2 == activeState1)
        return;
      if (activeState1 != null && activeState1.Activated)
        activeState1.HandleDeactivate();
      if (this._gameStateManagerListener != null)
        this._gameStateManagerListener.OnPushState(activeState2, isTopGameState);
      activeState2.HandleInitialize();
      activeState2.HandleActivate();
      this.Owner.OnStateChanged(activeState1);
    }

    private void OnPopState(int level)
    {
      GameState activeState1 = this.ActiveState;
      int lastIndex = this._gameStates.FindLastIndex((Predicate<GameState>) (state => state.Level == level));
      GameState gameState = this._gameStates[lastIndex];
      gameState.HandleDeactivate();
      gameState.HandleFinalize();
      this._gameStates.RemoveAt(lastIndex);
      GameState activeState2 = this.ActiveState;
      if (this._gameStateManagerListener != null)
        this._gameStateManagerListener.OnPopState(gameState);
      if (activeState2 == activeState1)
        return;
      if (activeState2 != null)
        activeState2.HandleActivate();
      else if (this._gameStateJobs.Count == 0 || this._gameStateJobs.Peek().Job != GameStateManager.GameStateJob.JobType.Push && this._gameStateJobs.Peek().Job != GameStateManager.GameStateJob.JobType.CleanAndPushState)
        this.Owner.OnStateStackEmpty();
      this.Owner.OnStateChanged(gameState);
    }

    private void OnCleanAndPushState(GameState gameState)
    {
      int num = -1;
      for (int index = 0; index < this._gameStates.Count; ++index)
      {
        if (this._gameStates[index].Level >= gameState.Level)
        {
          num = index - 1;
          break;
        }
      }
      GameState activeState = this.ActiveState;
      for (int index = this._gameStates.Count - 1; index > num; --index)
      {
        GameState gameState1 = this._gameStates[index];
        if (gameState1.Activated)
          gameState1.HandleDeactivate();
        gameState1.HandleFinalize();
        this._gameStates.RemoveAt(index);
      }
      this.OnPushState(gameState);
      this.Owner.OnStateChanged(activeState);
    }

    private void OnCleanStates(int popLevel)
    {
      int num = -1;
      for (int index = 0; index < this._gameStates.Count; ++index)
      {
        if (this._gameStates[index].Level >= popLevel)
        {
          num = index - 1;
          break;
        }
      }
      GameState activeState1 = this.ActiveState;
      for (int index = this._gameStates.Count - 1; index > num; --index)
      {
        GameState gameState = this._gameStates[index];
        if (gameState.Activated)
          gameState.HandleDeactivate();
        gameState.HandleFinalize();
        this._gameStates.RemoveAt(index);
      }
      if (this._gameStateManagerListener != null)
        this._gameStateManagerListener.OnCleanStates();
      GameState activeState2 = this.ActiveState;
      if (activeState1 == activeState2)
        return;
      if (activeState2 != null)
        activeState2.HandleActivate();
      else if (this._gameStateJobs.Count == 0 || this._gameStateJobs.Peek().Job != GameStateManager.GameStateJob.JobType.Push && this._gameStateJobs.Peek().Job != GameStateManager.GameStateJob.JobType.CleanAndPushState)
        this.Owner.OnStateStackEmpty();
      this.Owner.OnStateChanged(activeState1);
    }

    private void DoGameStateJobs()
    {
      while (this._gameStateJobs.Count > 0)
      {
        GameStateManager.GameStateJob gameStateJob = this._gameStateJobs.Dequeue();
        switch (gameStateJob.Job)
        {
          case GameStateManager.GameStateJob.JobType.Push:
            this.OnPushState(gameStateJob.GameState);
            continue;
          case GameStateManager.GameStateJob.JobType.Pop:
            this.OnPopState(gameStateJob.PopLevel);
            continue;
          case GameStateManager.GameStateJob.JobType.CleanAndPushState:
            this.OnCleanAndPushState(gameStateJob.GameState);
            continue;
          case GameStateManager.GameStateJob.JobType.CleanStates:
            this.OnCleanStates(gameStateJob.PopLevel);
            continue;
          default:
            continue;
        }
      }
    }

    public enum GameStateManagerType
    {
      Game,
      Global,
    }

    private struct GameStateJob
    {
      public readonly GameStateManager.GameStateJob.JobType Job;
      public readonly GameState GameState;
      public readonly int PopLevel;

      public GameStateJob(
        GameStateManager.GameStateJob.JobType job,
        GameState gameState,
        int popLevel)
      {
        this.Job = job;
        this.GameState = gameState;
        this.PopLevel = popLevel;
      }

      public enum JobType
      {
        None,
        Push,
        Pop,
        CleanAndPushState,
        CleanStates,
      }
    }
  }
}
