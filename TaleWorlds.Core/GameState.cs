// Decompiled with JetBrains decompiler
// Type: TaleWorlds.Core.GameState
// Assembly: TaleWorlds.Core, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 43F7B45C-9AE1-4715-A37B-2364840F62AF
// Assembly location: E:\SteamLibrary\steamapps\common\Mount & Blade II Bannerlord\bin\Win64_Shipping_Client\TaleWorlds.Core.dll

using TaleWorlds.Library;
using TaleWorlds.ObjectSystem;

namespace TaleWorlds.Core
{
  public abstract class GameState : MBObjectBase
  {
    public int Level;
    private IGameStateListener _listener;
    public static int NumberOfListenerActivations;

    public GameState Predecessor => this.GameStateManager.FindPredecessor(this);

    public bool IsActive => this.GameStateManager != null && this.GameStateManager.ActiveState == this;

    public IGameStateListener Listener
    {
      get => this._listener;
      set => this._listener = value;
    }

    public GameStateManager GameStateManager { get; internal set; }

    public virtual bool IsMusicMenuState => false;

    public virtual bool IsMenuState => false;

    public virtual bool IsMission => false;

    internal void HandleInitialize()
    {
      this.OnInitialize();
      if (this.Listener == null)
        return;
      this.Listener.OnInitialize();
    }

    protected virtual void OnInitialize()
    {
    }

    public virtual void OnLoad()
    {
    }

    internal void HandleFinalize()
    {
      this.OnFinalize();
      if (this.Listener != null)
        this.Listener.OnFinalize();
      this._listener = (IGameStateListener) null;
      this.GameStateManager = (GameStateManager) null;
    }

    protected virtual void OnFinalize()
    {
    }

    internal void HandleActivate()
    {
      GameState.NumberOfListenerActivations = 0;
      if (!this.IsActive)
        return;
      this.OnActivate();
      if (this.IsActive && this.Listener != null && GameState.NumberOfListenerActivations == 0)
      {
        this.Listener.OnActivate();
        ++GameState.NumberOfListenerActivations;
      }
      if (string.IsNullOrEmpty(GameStateManager.StateActivateCommand))
        return;
      CommandLineFunctionality.CallFunction(GameStateManager.StateActivateCommand, "", out bool _);
    }

    public bool Activated { get; private set; }

    protected virtual void OnActivate() => this.Activated = true;

    internal void HandleDeactivate()
    {
      this.OnDeactivate();
      if (this.Listener == null)
        return;
      this.Listener.OnDeactivate();
    }

    protected virtual void OnDeactivate() => this.Activated = false;

    protected internal virtual void OnTick(float dt)
    {
    }

    protected internal virtual void OnIdleTick(float dt)
    {
    }
  }
}
