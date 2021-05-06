// Decompiled with JetBrains decompiler
// Type: TaleWorlds.Core.GameManagerBase
// Assembly: TaleWorlds.Core, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 43F7B45C-9AE1-4715-A37B-2364840F62AF
// Assembly location: E:\SteamLibrary\steamapps\common\Mount & Blade II Bannerlord\bin\Win64_Shipping_Client\TaleWorlds.Core.dll

using System;
using System.Collections.Generic;

namespace TaleWorlds.Core
{
  public abstract class GameManagerBase
  {
    private EntitySystem<GameManagerComponent> _entitySystem;
    private GameManagerLoadingSteps _stepNo;
    private Game _game;
    private bool _initialized;

    public static GameManagerBase Current { get; private set; }

    public Game Game
    {
      get => this._game;
      internal set
      {
        if (value == null)
        {
          this._game = (Game) null;
          this._initialized = false;
        }
        else
        {
          this._game = value;
          this.Initialize();
        }
      }
    }

    public void Initialize()
    {
      if (this._initialized)
        return;
      this._initialized = true;
    }

    protected GameManagerBase()
    {
      GameManagerBase.Current = this;
      this._entitySystem = new EntitySystem<GameManagerComponent>();
      this._stepNo = GameManagerLoadingSteps.PreInitializeZerothStep;
    }

    public IEnumerable<GameManagerComponent> Components => (IEnumerable<GameManagerComponent>) this._entitySystem.Components;

    public GameManagerComponent AddComponent(Type componentType)
    {
      GameManagerComponent managerComponent = this._entitySystem.AddComponent(componentType);
      managerComponent.GameManager = this;
      return managerComponent;
    }

    public T AddComponent<T>() where T : GameManagerComponent, new() => (T) this.AddComponent(typeof (T));

    public GameManagerComponent GetComponent(Type componentType) => this._entitySystem.GetComponent(componentType);

    public T GetComponent<T>() where T : GameManagerComponent => this._entitySystem.GetComponent<T>();

    public IEnumerable<T> GetComponents<T>() where T : GameManagerComponent => (IEnumerable<T>) this._entitySystem.GetComponents<T>();

    public void RemoveComponent<T>() where T : GameManagerComponent => this.RemoveComponent((GameManagerComponent) this._entitySystem.GetComponent<T>());

    public void RemoveComponent(GameManagerComponent component) => this._entitySystem.RemoveComponent(component);

    public void OnTick(float dt)
    {
      foreach (GameManagerComponent component in (IEnumerable<GameManagerComponent>) this._entitySystem.Components)
        component.OnTick();
      if (this.Game == null)
        return;
      this.Game.OnTick(dt);
    }

    public void OnGameNetworkBegin()
    {
      foreach (GameManagerComponent component in (IEnumerable<GameManagerComponent>) this._entitySystem.Components)
        component.OnGameNetworkBegin();
      if (this.Game == null)
        return;
      this.Game.OnGameNetworkBegin();
    }

    public void OnGameNetworkEnd()
    {
      foreach (GameManagerComponent component in (IEnumerable<GameManagerComponent>) this._entitySystem.Components)
        component.OnGameNetworkEnd();
      if (this.Game == null)
        return;
      this.Game.OnGameNetworkEnd();
    }

    public void OnPlayerConnect(VirtualPlayer peer)
    {
      foreach (GameManagerComponent component in (IEnumerable<GameManagerComponent>) this._entitySystem.Components)
        component.OnEarlyPlayerConnect(peer);
      if (this.Game != null)
        this.Game.OnEarlyPlayerConnect(peer);
      foreach (GameManagerComponent component in (IEnumerable<GameManagerComponent>) this._entitySystem.Components)
        component.OnPlayerConnect(peer);
      if (this.Game == null)
        return;
      this.Game.OnPlayerConnect(peer);
    }

    public void OnPlayerDisconnect(VirtualPlayer peer)
    {
      foreach (GameManagerComponent component in (IEnumerable<GameManagerComponent>) this._entitySystem.Components)
        component.OnPlayerDisconnect(peer);
      if (this.Game == null)
        return;
      this.Game.OnPlayerDisconnect(peer);
    }

    public virtual void OnGameEnd(Game game)
    {
      GameManagerBase.Current = (GameManagerBase) null;
      this.Game = (Game) null;
    }

    protected virtual void DoLoadingForGameManager(
      GameManagerLoadingSteps gameManagerLoadingStep,
      out GameManagerLoadingSteps nextStep)
    {
      nextStep = GameManagerLoadingSteps.None;
    }

    public bool DoLoadingForGameManager()
    {
      bool flag = false;
      GameManagerLoadingSteps nextStep = GameManagerLoadingSteps.None;
      switch (this._stepNo)
      {
        case GameManagerLoadingSteps.PreInitializeZerothStep:
          this.DoLoadingForGameManager(GameManagerLoadingSteps.PreInitializeZerothStep, out nextStep);
          if (nextStep == GameManagerLoadingSteps.FirstInitializeFirstStep)
          {
            ++this._stepNo;
            break;
          }
          break;
        case GameManagerLoadingSteps.FirstInitializeFirstStep:
          this.DoLoadingForGameManager(GameManagerLoadingSteps.FirstInitializeFirstStep, out nextStep);
          if (nextStep == GameManagerLoadingSteps.WaitSecondStep)
          {
            ++this._stepNo;
            break;
          }
          break;
        case GameManagerLoadingSteps.WaitSecondStep:
          this.DoLoadingForGameManager(GameManagerLoadingSteps.WaitSecondStep, out nextStep);
          if (nextStep == GameManagerLoadingSteps.SecondInitializeThirdState)
          {
            ++this._stepNo;
            break;
          }
          break;
        case GameManagerLoadingSteps.SecondInitializeThirdState:
          this.DoLoadingForGameManager(GameManagerLoadingSteps.SecondInitializeThirdState, out nextStep);
          if (nextStep == GameManagerLoadingSteps.PostInitializeFourthState)
          {
            ++this._stepNo;
            break;
          }
          break;
        case GameManagerLoadingSteps.PostInitializeFourthState:
          this.DoLoadingForGameManager(GameManagerLoadingSteps.PostInitializeFourthState, out nextStep);
          if (nextStep == GameManagerLoadingSteps.FinishLoadingFifthStep)
          {
            ++this._stepNo;
            break;
          }
          break;
        case GameManagerLoadingSteps.FinishLoadingFifthStep:
          this.DoLoadingForGameManager(GameManagerLoadingSteps.FinishLoadingFifthStep, out nextStep);
          if (nextStep == GameManagerLoadingSteps.None)
          {
            ++this._stepNo;
            flag = true;
            break;
          }
          break;
        case GameManagerLoadingSteps.LoadingIsOver:
          flag = true;
          break;
      }
      return flag;
    }

    public virtual void OnLoadFinished()
    {
    }

    public abstract void OnGameStart(Game game, IGameStarter gameStarter);

    public abstract void BeginGameStart(Game game);

    public abstract void OnCampaignEarlyStart(Game game, object starterObject);

    public abstract void OnCampaignStart(Game game, object starterObject);

    public abstract void OnGameInitializationFinished(Game game);

    public abstract void OnAfterCampaignStart(Game game);

    public abstract void OnGameCreated(Game game, object initializerObject);

    public abstract void OnGameLoaded(Game game, object initializerObject);

    public abstract float ApplicationTime { get; }

    public abstract bool CheatMode { get; }

    public abstract bool IsDevelopmentMode { get; }

    public abstract bool IsEditModeOn { get; }

    public abstract bool DeterministicMode { get; }
  }
}
