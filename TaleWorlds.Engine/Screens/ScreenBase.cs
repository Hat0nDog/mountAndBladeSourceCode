// Decompiled with JetBrains decompiler
// Type: TaleWorlds.Engine.Screens.ScreenBase
// Assembly: TaleWorlds.Engine, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: DD7DC4E8-6EB6-4CB8-8DE1-FC78E45F7FE8
// Assembly location: E:\SteamLibrary\steamapps\common\Mount & Blade II Bannerlord\bin\Win64_Shipping_Client\TaleWorlds.Engine.dll

using System.Collections.Generic;
using TaleWorlds.InputSystem;
using TaleWorlds.Library;

namespace TaleWorlds.Engine.Screens
{
  public abstract class ScreenBase
  {
    private List<ScreenLayer> _layers;
    private List<ScreenLayer> _layersCopy;
    private bool _isInitialized;
    private bool _forceMouseVisible;

    public IInputContext DebugInput => Input.DebugInput;

    public IReadOnlyList<ScreenLayer> Layers => (IReadOnlyList<ScreenLayer>) this._layers;

    public bool IsActive { get; private set; }

    public bool IsPaused { get; private set; }

    internal void HandleInitialize()
    {
      Debug.Print(this.ToString() + "::HandleInitialize");
      if (this._isInitialized)
        return;
      this._isInitialized = true;
      this.OnInitialize();
    }

    internal void HandleFinalize()
    {
      Debug.Print(this.ToString() + "::HandleFinalize");
      if (this._isInitialized)
      {
        this._isInitialized = false;
        this.OnFinalize();
        for (int index = this._layers.Count - 1; index >= 0; --index)
          this._layers[index].HandleFinalize();
      }
      this.IsActive = false;
    }

    internal void HandleActivate()
    {
      Debug.Print(this.ToString() + "::HandleActivate");
      if (this.IsActive)
        return;
      this.IsActive = true;
      for (int index = this._layers.Count - 1; index >= 0; --index)
      {
        ScreenLayer layer = this._layers[index];
        if (!layer.IsActive)
          layer.HandleActivate();
      }
      this.OnActivate();
    }

    internal void HandleDeactivate()
    {
      Debug.Print(this.ToString() + "::HandleDeactivate");
      if (!this.IsActive)
        return;
      this.IsActive = false;
      for (int index = this._layers.Count - 1; index >= 0; --index)
      {
        ScreenLayer layer = this._layers[index];
        if (layer.IsActive)
          layer.HandleDeactivate();
      }
      this.OnDeactivate();
    }

    internal void HandleResume()
    {
      Debug.Print(this.ToString() + "::HandleResume");
      if (!this.IsPaused)
        return;
      Utilities.ClearOldResourcesAndObjects();
      for (int index = this._layers.Count - 1; index >= 0; --index)
      {
        ScreenLayer layer = this._layers[index];
        if (!layer.IsActive)
          layer.HandleActivate();
      }
      this.IsPaused = false;
      this.OnResume();
    }

    internal void HandlePause()
    {
      Debug.Print(this.ToString() + "::HandlePause");
      if (this.IsPaused)
        return;
      for (int index = this._layers.Count - 1; index >= 0; --index)
      {
        ScreenLayer layer = this._layers[index];
        if (layer.IsActive)
          layer.HandleDeactivate();
      }
      this.IsPaused = true;
      this.OnPause();
    }

    internal void FrameTick(float dt)
    {
      if (this.IsActive)
        this.OnFrameTick(dt);
      if (!this.IsActive)
        return;
      for (int index = 0; index < this._layers.Count; ++index)
      {
        if (this._layers[index].IsActive)
          this._layersCopy.Add(this._layers[index]);
      }
      for (int index = 0; index < this._layersCopy.Count; ++index)
      {
        if (!this._layersCopy[index].Finalized)
          this._layersCopy[index].Tick(dt);
      }
      ScreenManager.UpdateLateTickLayers(this._layersCopy);
    }

    public void ActivateAllLayers()
    {
      foreach (ScreenLayer layer in this._layers)
      {
        if (!layer.IsActive)
          layer.HandleActivate();
      }
    }

    public void DeactivateAllLayers()
    {
      foreach (ScreenLayer layer in this._layers)
      {
        if (layer.IsActive)
          layer.HandleDeactivate();
      }
    }

    public void Deactivate()
    {
      if (!this.IsActive)
        return;
      this.HandleDeactivate();
      this.IsActive = false;
    }

    public void Activate()
    {
      if (this.IsActive)
        return;
      this.HandleActivate();
      this.IsActive = true;
    }

    public void UpdateLayout()
    {
      for (int index = 0; index < this._layers.Count; ++index)
      {
        if (!this._layers[index].Finalized)
          this._layers[index].UpdateLayout();
      }
    }

    internal void IdleTick(float dt) => this.OnIdleTick(dt);

    protected virtual void OnInitialize()
    {
    }

    protected virtual void OnFinalize()
    {
    }

    protected virtual void OnPause()
    {
    }

    protected virtual void OnResume()
    {
    }

    protected virtual void OnActivate()
    {
    }

    protected virtual void OnDeactivate()
    {
    }

    protected virtual void OnFrameTick(float dt)
    {
    }

    protected virtual void OnIdleTick(float dt)
    {
    }

    public void AddLayer(ScreenLayer layer)
    {
      if (this._layers.Contains(layer))
        return;
      this._layers.Add(layer);
      this._layers.Sort();
      if (!this.IsActive)
        return;
      layer.LastActiveState = true;
      layer.HandleActivate();
    }

    public void RemoveLayer(ScreenLayer layer)
    {
      if (this.IsActive)
      {
        layer.LastActiveState = false;
        layer.HandleDeactivate();
      }
      layer.HandleFinalize();
      this._layers.Remove(layer);
      ScreenManager.RefreshGlobalOrder();
    }

    public bool HasLayer(ScreenLayer layer) => this._layers.Contains(layer);

    public T FindLayer<T>() where T : ScreenLayer
    {
      foreach (ScreenLayer layer in this._layers)
      {
        if (layer is T obj1)
          return obj1;
      }
      return default (T);
    }

    public T FindLayer<T>(string name) where T : ScreenLayer
    {
      foreach (ScreenLayer layer in this._layers)
      {
        if (layer is T && layer.Name == name)
          return (T) layer;
      }
      return default (T);
    }

    public void SetLayerCategoriesState(string[] categoryIds, bool isActive)
    {
      for (int index = 0; index < this._layers.Count; ++index)
      {
        ScreenLayer layer = this._layers[index];
        if (((IReadOnlyList<string>) categoryIds).IndexOf<string>(layer._categoryId) >= 0)
        {
          if (isActive && !layer.IsActive)
            layer.HandleActivate();
          else if (!isActive && layer.IsActive)
            layer.HandleDeactivate();
        }
      }
    }

    public void SetLayerCategoriesStateAndToggleOthers(string[] categoryIds, bool isActive)
    {
      foreach (ScreenLayer layer in this._layers)
      {
        if (((IReadOnlyList<string>) categoryIds).IndexOf<string>(layer._categoryId) >= 0)
        {
          if (isActive && !layer.IsActive)
            layer.HandleActivate();
          else if (!isActive && layer.IsActive)
            layer.HandleDeactivate();
        }
        else if (layer.IsActive)
          layer.HandleDeactivate();
        else
          layer.HandleActivate();
      }
    }

    public void SetLayerCategoriesStateAndDeactivateOthers(string[] categoryIds, bool isActive)
    {
      foreach (ScreenLayer layer in this._layers)
      {
        if (((IReadOnlyList<string>) categoryIds).IndexOf<string>(layer._categoryId) >= 0)
        {
          if (isActive && !layer.IsActive)
            layer.HandleActivate();
          else if (!isActive && layer.IsActive)
            layer.HandleDeactivate();
        }
        else if (layer.IsActive)
          layer.HandleDeactivate();
      }
    }

    protected ScreenBase()
    {
      this.IsPaused = true;
      this.IsActive = false;
      this._layers = new List<ScreenLayer>();
      this._layersCopy = new List<ScreenLayer>();
    }

    public virtual bool MouseVisible
    {
      get => this._forceMouseVisible;
      set => this._forceMouseVisible = value;
    }

    internal void Update(IReadOnlyList<int> lastKeysPressed)
    {
      if (!this.IsActive)
        return;
      foreach (ScreenLayer layer in this._layers)
      {
        if (layer.IsActive)
          layer.Update(lastKeysPressed);
      }
    }

    public virtual bool OnGamepadNavigation(
      ScreenManager.CursorAreas currentCursorArea,
      ScreenManager.GamepadNavigationTypes type)
    {
      return false;
    }
  }
}
