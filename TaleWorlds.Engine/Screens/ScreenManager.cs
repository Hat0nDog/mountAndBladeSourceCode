// Decompiled with JetBrains decompiler
// Type: TaleWorlds.Engine.Screens.ScreenManager
// Assembly: TaleWorlds.Engine, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: DD7DC4E8-6EB6-4CB8-8DE1-FC78E45F7FE8
// Assembly location: E:\SteamLibrary\steamapps\common\Mount & Blade II Bannerlord\bin\Win64_Shipping_Client\TaleWorlds.Engine.dll

using System;
using System.Collections.Generic;
using System.Linq;
using TaleWorlds.DotNet;
using TaleWorlds.InputSystem;
using TaleWorlds.Library;

namespace TaleWorlds.Engine.Screens
{
  public static class ScreenManager
  {
    private static List<ScreenLayer> _lateTickLayers;
    private static List<ScreenBase> _screenList;
    private static List<GlobalLayer> _globalLayers;
    private static List<ScreenLayer> _sortedActiveLayers = new List<ScreenLayer>(16);
    private static ScreenLayer[] _sortedActiveLayersCopy = new ScreenLayer[16];
    private static ScreenLayer[] _sortedActiveLayersCopyForUpdate = new ScreenLayer[16];
    private static bool _activeMouseVisible = false;
    private static NativeArrayEnumerator<int> _lastPressedKeys;
    private static ScreenLayer _focusedLayer;
    private static List<InputKey> _pressedMouseButtons;

    public static ScreenManager.CursorAreas LatestCursorArea { get; private set; }

    public static float Scale { get; private set; } = 1f;

    public static Vec2 UsableArea { get; private set; } = new Vec2(1f, 1f);

    private static bool _isGamepadActive => EngineApplicationInterface.IInput.IsControllerConnected() && !EngineApplicationInterface.IInput.IsMouseActive();

    public static event ScreenManager.OnPushScreenEvent OnPushScreen;

    public static event ScreenManager.OnPopScreenEvent OnPopScreen;

    public static List<ScreenLayer> SortedActiveLayers
    {
      get
      {
        ScreenManager._sortedActiveLayers.Clear();
        if (ScreenManager.TopScreen != null)
        {
          for (int index = 0; index < ScreenManager.TopScreen.Layers.Count; ++index)
          {
            ScreenLayer layer = ScreenManager.TopScreen.Layers[index];
            if (layer.IsActive)
              ScreenManager._sortedActiveLayers.Add(layer);
          }
        }
        foreach (GlobalLayer globalLayer in ScreenManager._globalLayers)
        {
          if (globalLayer.Layer.IsActive)
            ScreenManager._sortedActiveLayers.Add(globalLayer.Layer);
        }
        ScreenManager._sortedActiveLayers.Sort();
        return ScreenManager._sortedActiveLayers;
      }
    }

    public static ScreenLayer FocusedLayer => ScreenManager._focusedLayer;

    static ScreenManager()
    {
      ScreenManager._globalLayers = new List<GlobalLayer>();
      ScreenManager._screenList = new List<ScreenBase>();
      ScreenManager._pressedMouseButtons = new List<InputKey>();
      ScreenManager._focusedLayer = (ScreenLayer) null;
      ScreenManager.FirstHitLayer = (ScreenLayer) null;
    }

    internal static void RefreshGlobalOrder()
    {
      int currentOrder = -2000;
      List<ScreenLayer> sortedActiveLayers = ScreenManager.SortedActiveLayers;
      if (ScreenManager._sortedActiveLayersCopy.Length != sortedActiveLayers.Capacity)
        ScreenManager._sortedActiveLayersCopy = new ScreenLayer[sortedActiveLayers.Capacity];
      sortedActiveLayers.CopyTo(ScreenManager._sortedActiveLayersCopy);
      int count = sortedActiveLayers.Count;
      for (int index = 0; index < count; ++index)
        ScreenManager._sortedActiveLayersCopy[index].RefreshGlobalOrder(ref currentOrder);
      ScreenManager._sortedActiveLayersCopy.Initialize();
    }

    public static void RemoveGlobalLayer(GlobalLayer layer)
    {
      ScreenManager._globalLayers.Remove(layer);
      layer.Layer.HandleDeactivate();
      ScreenManager.RefreshGlobalOrder();
    }

    public static void AddGlobalLayer(GlobalLayer layer, bool isFocusable)
    {
      ScreenManager._globalLayers.Add(layer);
      layer.Layer.HandleActivate();
      ScreenManager._globalLayers.Sort();
      ScreenManager.RefreshGlobalOrder();
    }

    public static bool ScreenTypeExistsAtList(ScreenBase screen)
    {
      System.Type type = screen.GetType();
      foreach (object screen1 in ScreenManager._screenList)
      {
        if (screen1.GetType() == type)
          return true;
      }
      return false;
    }

    public static void UpdateLayout()
    {
      foreach (GlobalLayer globalLayer in ScreenManager._globalLayers)
        globalLayer.UpdateLayout();
      foreach (ScreenBase screen in ScreenManager._screenList)
        screen.UpdateLayout();
    }

    public static void SetSuspendLayer(ScreenLayer layer, bool isSuspended)
    {
      if (isSuspended)
        layer.HandleDeactivate();
      else
        layer.HandleActivate();
      layer.LastActiveState = !isSuspended;
    }

    public static void OnFinalize()
    {
      ScreenManager.DeactivateAndFinalizeAllScreens();
      ScreenManager._screenList = (List<ScreenBase>) null;
      ScreenManager._globalLayers = (List<GlobalLayer>) null;
      ScreenManager._focusedLayer = (ScreenLayer) null;
    }

    private static void DeactivateAndFinalizeAllScreens()
    {
      for (int index = ScreenManager._screenList.Count - 1; index >= 0; --index)
        ScreenManager._screenList[index].HandlePause();
      for (int index = ScreenManager._screenList.Count - 1; index >= 0; --index)
        ScreenManager._screenList[index].HandleDeactivate();
      for (int index = ScreenManager._screenList.Count - 1; index >= 0; --index)
        ScreenManager._screenList[index].HandleFinalize();
      ScreenManager._screenList.Clear();
      Common.MemoryCleanup();
    }

    internal static void UpdateLateTickLayers(List<ScreenLayer> layers) => ScreenManager._lateTickLayers = layers;

    [EngineCallback]
    internal static void PreTick(float dt) => ScreenManager.EarlyUpdate();

    [EngineCallback]
    public static void Tick(float dt)
    {
      for (int index = 0; index < ScreenManager._globalLayers.Count; ++index)
        ScreenManager._globalLayers[index].EarlyTick(dt);
      ScreenManager.Update();
      ScreenManager._lateTickLayers = (List<ScreenLayer>) null;
      if (ScreenManager.TopScreen != null)
      {
        ScreenManager.TopScreen.FrameTick(dt);
        ScreenManager.FindPredecessor(ScreenManager.TopScreen)?.IdleTick(dt);
      }
      for (int index = 0; index < ScreenManager._globalLayers.Count; ++index)
        ScreenManager._globalLayers[index].Tick(dt);
      ScreenManager.LateUpdate(dt);
    }

    private static void GamepadNavigationTick()
    {
      bool flag = false;
      ScreenManager.GamepadNavigationTypes type = ScreenManager.GamepadNavigationTypes.None;
      if (Input.IsKeyReleased(InputKey.ControllerLLeft))
        type = ScreenManager.GamepadNavigationTypes.Left;
      else if (Input.IsKeyReleased(InputKey.ControllerLRight))
        type = ScreenManager.GamepadNavigationTypes.Right;
      else if (Input.IsKeyReleased(InputKey.ControllerLDown))
        type = ScreenManager.GamepadNavigationTypes.Down;
      else if (Input.IsKeyReleased(InputKey.ControllerLUp))
        type = ScreenManager.GamepadNavigationTypes.Up;
      if (type != ScreenManager.GamepadNavigationTypes.None)
      {
        for (int index = 0; index < ScreenManager._globalLayers.Count; ++index)
        {
          if (!flag && ScreenManager._globalLayers[index].OnGamepadNavigation(ScreenManager.LatestCursorArea, type))
          {
            flag = true;
            break;
          }
        }
        if (!flag)
        {
          for (int index = 0; index < ScreenManager._screenList.Count; ++index)
          {
            if (ScreenManager._screenList[index].OnGamepadNavigation(ScreenManager.LatestCursorArea, type))
            {
              flag = true;
              break;
            }
          }
        }
      }
      if ((flag ? 0 : (ScreenManager.LatestCursorArea == ScreenManager.CursorAreas.Bottom && type == ScreenManager.GamepadNavigationTypes.Up || ScreenManager.LatestCursorArea == ScreenManager.CursorAreas.Left && type == ScreenManager.GamepadNavigationTypes.Right || ScreenManager.LatestCursorArea == ScreenManager.CursorAreas.Right && type == ScreenManager.GamepadNavigationTypes.Left ? 1 : (ScreenManager.LatestCursorArea != ScreenManager.CursorAreas.Top ? 0 : (type == ScreenManager.GamepadNavigationTypes.Down ? 1 : 0)))) == 0)
        return;
      ScreenManager.UpdateCurrentCursorArea(ScreenManager.CursorAreas.Center);
    }

    [EngineCallback]
    internal static void LateTick(float dt)
    {
      if (ScreenManager._lateTickLayers != null)
      {
        for (int index = 0; index < ScreenManager._lateTickLayers.Count; ++index)
        {
          if (!ScreenManager._lateTickLayers[index].Finalized)
            ScreenManager._lateTickLayers[index].LateTick(dt);
        }
        ScreenManager._lateTickLayers.Clear();
      }
      for (int index = 0; index < ScreenManager._globalLayers.Count; ++index)
        ScreenManager._globalLayers[index].LateTick(dt);
      ScreenManager.GamepadNavigationTick();
    }

    [EngineCallback]
    internal static void OnOnscreenKeyboardDone(string inputText) => ScreenManager.FocusedLayer?.OnOnScreenKeyboardDone(inputText);

    public static void ReplaceTopScreen(ScreenBase screen)
    {
      Debug.Print("ReplaceToTopScreen");
      if (ScreenManager._screenList.Count > 0)
      {
        ScreenManager.TopScreen.HandlePause();
        ScreenManager.TopScreen.HandleDeactivate();
        ScreenManager.TopScreen.HandleFinalize();
        ScreenManager.OnPopScreenEvent onPopScreen = ScreenManager.OnPopScreen;
        if (onPopScreen != null)
          onPopScreen(ScreenManager.TopScreen);
        ScreenManager._screenList.Remove(ScreenManager.TopScreen);
      }
      ScreenManager._screenList.Add(screen);
      screen.HandleInitialize();
      screen.HandleActivate();
      screen.HandleResume();
      ScreenManager.RefreshGlobalOrder();
      ScreenManager.OnPushScreenEvent onPushScreen = ScreenManager.OnPushScreen;
      if (onPushScreen == null)
        return;
      onPushScreen(screen);
    }

    public static List<ScreenLayer> GetPersistentInputRestrictions()
    {
      List<ScreenLayer> screenLayerList = new List<ScreenLayer>();
      foreach (GlobalLayer globalLayer in ScreenManager._globalLayers)
        screenLayerList.Add(globalLayer.Layer);
      return screenLayerList;
    }

    public static void SetAndActivateRootScreen(ScreenBase screen)
    {
      Debug.Print(nameof (SetAndActivateRootScreen));
      if (ScreenManager.TopScreen != null)
        throw new Exception("TopScreen is not null.");
      ScreenManager._screenList.Add(screen);
      screen.HandleInitialize();
      screen.HandleActivate();
      screen.HandleResume();
      ScreenManager.RefreshGlobalOrder();
      ScreenManager.OnPushScreenEvent onPushScreen = ScreenManager.OnPushScreen;
      if (onPushScreen == null)
        return;
      onPushScreen(screen);
    }

    public static void CleanAndPushScreen(ScreenBase screen)
    {
      Debug.Print(nameof (CleanAndPushScreen));
      ScreenManager.DeactivateAndFinalizeAllScreens();
      ScreenManager._screenList.Add(screen);
      screen.HandleInitialize();
      screen.HandleActivate();
      screen.HandleResume();
      ScreenManager.RefreshGlobalOrder();
      ScreenManager.OnPushScreenEvent onPushScreen = ScreenManager.OnPushScreen;
      if (onPushScreen == null)
        return;
      onPushScreen(screen);
    }

    public static void PushScreen(ScreenBase screen)
    {
      Debug.Print(nameof (PushScreen));
      if (ScreenManager._screenList.Count > 0)
      {
        ScreenManager.TopScreen.HandlePause();
        if (ScreenManager.TopScreen.IsActive)
          ScreenManager.TopScreen.HandleDeactivate();
      }
      ScreenManager._screenList.Add(screen);
      screen.HandleInitialize();
      screen.HandleActivate();
      screen.HandleResume();
      ScreenManager.RefreshGlobalOrder();
      ScreenManager.OnPushScreenEvent onPushScreen = ScreenManager.OnPushScreen;
      if (onPushScreen == null)
        return;
      onPushScreen(screen);
    }

    public static void PopScreen()
    {
      Debug.Print(nameof (PopScreen));
      if (ScreenManager._screenList.Count > 0)
      {
        ScreenManager.TopScreen.HandlePause();
        ScreenManager.TopScreen.HandleDeactivate();
        ScreenManager.TopScreen.HandleFinalize();
        ScreenManager.OnPopScreenEvent onPopScreen = ScreenManager.OnPopScreen;
        if (onPopScreen != null)
          onPopScreen(ScreenManager.TopScreen);
        ScreenManager._screenList.Remove(ScreenManager.TopScreen);
      }
      if (ScreenManager._screenList.Count > 0)
      {
        ScreenBase topScreen1 = ScreenManager.TopScreen;
        ScreenManager.TopScreen.HandleActivate();
        ScreenBase topScreen2 = ScreenManager.TopScreen;
        if (topScreen1 == topScreen2)
          ScreenManager.TopScreen.HandleResume();
      }
      ScreenManager.RefreshGlobalOrder();
    }

    public static void CleanScreens()
    {
      Debug.Print(nameof (CleanScreens));
      while (ScreenManager._screenList.Count > 0)
      {
        ScreenManager.TopScreen.HandlePause();
        ScreenManager.TopScreen.HandleDeactivate();
        ScreenManager.TopScreen.HandleFinalize();
        ScreenManager.OnPopScreenEvent onPopScreen = ScreenManager.OnPopScreen;
        if (onPopScreen != null)
          onPopScreen(ScreenManager.TopScreen);
        ScreenManager._screenList.Remove(ScreenManager.TopScreen);
      }
      ScreenManager.RefreshGlobalOrder();
    }

    private static ScreenBase FindPredecessor(ScreenBase screen)
    {
      ScreenBase screenBase = (ScreenBase) null;
      int num = ScreenManager._screenList.IndexOf(screen);
      if (num > 0)
        screenBase = ScreenManager._screenList[num - 1];
      return screenBase;
    }

    [EngineCallback]
    internal static void Update(NativeArray lastKeysPressed)
    {
      if (ScreenManager._lastPressedKeys == null)
        ScreenManager._lastPressedKeys = new NativeArrayEnumerator<int>(lastKeysPressed);
      if (ScreenManager.TopScreen != null)
        ScreenManager.TopScreen.Update((IReadOnlyList<int>) ScreenManager._lastPressedKeys);
      foreach (GlobalLayer globalLayer in ScreenManager._globalLayers)
        globalLayer.Update((IReadOnlyList<int>) ScreenManager._lastPressedKeys);
    }

    public static ScreenBase TopScreen => ScreenManager._screenList != null && ScreenManager._screenList.Count > 0 ? ScreenManager._screenList.Last<ScreenBase>() : (ScreenBase) null;

    public static bool IsMouseCursorHidden() => !Input.IsMouseActive && EngineApplicationInterface.IScreen.GetMouseVisible();

    private static void SetMouseVisible(bool value)
    {
      ScreenManager._activeMouseVisible = value;
      EngineApplicationInterface.IScreen.SetMouseVisible(value);
    }

    private static bool? GetMouseInput()
    {
      bool flag1 = ScreenManager._pressedMouseButtons.Count == 0;
      if (Input.IsKeyPressed(InputKey.LeftMouseButton))
        ScreenManager._pressedMouseButtons.Add(InputKey.LeftMouseButton);
      if (Input.IsKeyPressed(InputKey.RightMouseButton))
        ScreenManager._pressedMouseButtons.Add(InputKey.RightMouseButton);
      if (Input.IsKeyPressed(InputKey.MiddleMouseButton))
        ScreenManager._pressedMouseButtons.Add(InputKey.MiddleMouseButton);
      if (Input.IsKeyPressed(InputKey.X1MouseButton))
        ScreenManager._pressedMouseButtons.Add(InputKey.X1MouseButton);
      if (Input.IsKeyPressed(InputKey.X2MouseButton))
        ScreenManager._pressedMouseButtons.Add(InputKey.X2MouseButton);
      if (Input.IsKeyPressed(InputKey.ControllerRDown))
        ScreenManager._pressedMouseButtons.Add(InputKey.ControllerRDown);
      for (int index = ScreenManager._pressedMouseButtons.Count - 1; index >= 0; --index)
      {
        if (!Input.IsKeyDown(ScreenManager._pressedMouseButtons[index]))
          ScreenManager._pressedMouseButtons.RemoveAt(index);
      }
      bool flag2 = ScreenManager._pressedMouseButtons.Count == 0;
      if (flag1 && !flag2)
        return new bool?(true);
      return !flag1 & flag2 ? new bool?(false) : new bool?();
    }

    public static ScreenLayer FirstHitLayer { get; private set; }

    private static void EarlyUpdate()
    {
      ScreenManager.UsableArea = EngineApplicationInterface.IScreen.GetUsableAreaPercentages();
      ScreenManager.RefreshGlobalOrder();
      ScreenManager.UpdateMouseVisibility();
      List<ScreenLayer> sortedActiveLayers = ScreenManager.SortedActiveLayers;
      if (ScreenManager._sortedActiveLayersCopy.Length != sortedActiveLayers.Capacity)
        ScreenManager._sortedActiveLayersCopy = new ScreenLayer[sortedActiveLayers.Capacity];
      sortedActiveLayers.CopyTo(ScreenManager._sortedActiveLayersCopy);
      int count = sortedActiveLayers.Count;
      TaleWorlds.Library.InputType p1 = TaleWorlds.Library.InputType.None;
      for (int index = 0; index < count; ++index)
        ScreenManager._sortedActiveLayersCopy[index].MouseEnabled = true;
      bool? mouseInput = ScreenManager.GetMouseInput();
      for (int index = count - 1; index >= 0; --index)
      {
        ScreenLayer layer = ScreenManager._sortedActiveLayersCopy[index];
        if (!layer.Finalized)
        {
          bool? isMousePressed = new bool?();
          bool? nullable = mouseInput;
          bool flag = false;
          if (nullable.GetValueOrDefault() == flag & nullable.HasValue)
            isMousePressed = new bool?(false);
          TaleWorlds.Library.InputType handledInputs = TaleWorlds.Library.InputType.None;
          InputUsageMask inputUsageMask = layer.InputUsageMask;
          if (layer.HitTest())
          {
            if (ScreenManager.FirstHitLayer == null)
            {
              ScreenManager.FirstHitLayer = layer;
              MouseManager.ActivateMouseCursor(layer.ActiveCursor);
            }
            if (!p1.HasAnyFlag<TaleWorlds.Library.InputType>(TaleWorlds.Library.InputType.MouseButton) && inputUsageMask.HasAnyFlag<InputUsageMask>(InputUsageMask.MouseButtons))
            {
              isMousePressed = mouseInput;
              handledInputs |= TaleWorlds.Library.InputType.MouseButton;
              p1 |= TaleWorlds.Library.InputType.MouseButton;
            }
            if (!p1.HasAnyFlag<TaleWorlds.Library.InputType>(TaleWorlds.Library.InputType.MouseWheel) && inputUsageMask.HasAnyFlag<InputUsageMask>(InputUsageMask.MouseWheels))
            {
              handledInputs |= TaleWorlds.Library.InputType.MouseWheel;
              p1 |= TaleWorlds.Library.InputType.MouseWheel;
            }
          }
          if (ScreenManager.FocusTest(layer))
          {
            handledInputs |= TaleWorlds.Library.InputType.Key;
            p1 |= TaleWorlds.Library.InputType.Key;
          }
          layer.EarlyProcessEvents(handledInputs, isMousePressed);
        }
      }
      ScreenManager._sortedActiveLayersCopy.Initialize();
    }

    public static void UpdateCurrentCursorArea(ScreenManager.CursorAreas newArea)
    {
      ScreenManager.LatestCursorArea = newArea;
      if (newArea != ScreenManager.CursorAreas.Center || !ScreenManager._isGamepadActive)
        return;
      Input.SetMousePosition((int) Screen.RealScreenResolutionWidth / 2, (int) Screen.RealScreenResolutionHeight / 2);
    }

    private static void Update()
    {
      ScreenManager.UpdateMouseVisibility();
      List<ScreenLayer> sortedActiveLayers = ScreenManager.SortedActiveLayers;
      if (ScreenManager._sortedActiveLayersCopyForUpdate.Length != sortedActiveLayers.Capacity)
        ScreenManager._sortedActiveLayersCopyForUpdate = new ScreenLayer[sortedActiveLayers.Capacity];
      sortedActiveLayers.CopyTo(ScreenManager._sortedActiveLayersCopyForUpdate);
      for (int index = sortedActiveLayers.Count - 1; index >= 0; --index)
      {
        ScreenLayer screenLayer = ScreenManager._sortedActiveLayersCopyForUpdate[index];
        if (!screenLayer.Finalized)
          screenLayer.ProcessEvents();
      }
      for (int count = sortedActiveLayers.Count; count < ScreenManager._sortedActiveLayersCopyForUpdate.Length; ++count)
        ScreenManager._sortedActiveLayersCopyForUpdate[count] = (ScreenLayer) null;
    }

    private static void LateUpdate(float dt)
    {
      ScreenManager.UpdateMouseVisibility();
      List<ScreenLayer> sortedActiveLayers = ScreenManager.SortedActiveLayers;
      if (ScreenManager._sortedActiveLayersCopy.Length != sortedActiveLayers.Capacity)
        ScreenManager._sortedActiveLayersCopy = new ScreenLayer[sortedActiveLayers.Capacity];
      sortedActiveLayers.CopyTo(ScreenManager._sortedActiveLayersCopy);
      int count = sortedActiveLayers.Count;
      bool flag = false;
      for (int index = 0; index < count; ++index)
      {
        ScreenLayer layer = ScreenManager._sortedActiveLayersCopy[index];
        if (ScreenManager.FocusTest(layer))
          flag = true;
        layer.LateProcessEvents();
        if (!flag)
        {
          ScreenLayer focusedLayer = ScreenManager._focusedLayer;
        }
      }
      for (int index = 0; index < count; ++index)
      {
        ScreenLayer screenLayer = ScreenManager._sortedActiveLayersCopy[index];
        screenLayer.OnLateUpdate(dt);
        if (screenLayer != ScreenManager.FirstHitLayer)
          screenLayer.Input.ResetLastDownKeys();
      }
      ScreenManager._sortedActiveLayersCopy.Initialize();
      ScreenManager.FirstHitLayer?.Input?.UpdateLastDownKeys();
      ScreenManager.FirstHitLayer = (ScreenLayer) null;
    }

    internal static void UpdateMouseVisibility()
    {
      List<ScreenLayer> sortedActiveLayers = ScreenManager.SortedActiveLayers;
      if (ScreenManager._sortedActiveLayersCopy.Length != sortedActiveLayers.Capacity)
        ScreenManager._sortedActiveLayersCopy = new ScreenLayer[sortedActiveLayers.Capacity];
      sortedActiveLayers.CopyTo(ScreenManager._sortedActiveLayersCopy);
      int count = sortedActiveLayers.Count;
      for (int index = 0; index < count; ++index)
      {
        if (ScreenManager._sortedActiveLayersCopy[index].InputRestrictions.MouseVisibility)
        {
          if (ScreenManager._activeMouseVisible)
            return;
          ScreenManager.SetMouseVisible(true);
          return;
        }
      }
      if (ScreenManager._activeMouseVisible)
        ScreenManager.SetMouseVisible(false);
      ScreenManager._sortedActiveLayersCopy.Initialize();
    }

    public static bool GetMouseVisibility() => ScreenManager._activeMouseVisible;

    public static void TrySetFocus(ScreenLayer layer)
    {
      if (ScreenManager._focusedLayer != null && ScreenManager._focusedLayer.InputRestrictions.Order > layer.InputRestrictions.Order && layer.IsActive || !layer.IsFocusLayer && !layer.FocusTest())
        return;
      if (ScreenManager._focusedLayer != null && ScreenManager._focusedLayer != layer)
        ScreenManager._focusedLayer.OnLoseFocus();
      ScreenManager._focusedLayer = layer;
    }

    public static void TryLoseFocus(ScreenLayer layer)
    {
      if (ScreenManager._focusedLayer != layer)
        return;
      ScreenManager._focusedLayer?.OnLoseFocus();
      List<ScreenLayer> sortedActiveLayers = ScreenManager.SortedActiveLayers;
      if (ScreenManager._sortedActiveLayersCopy.Length != sortedActiveLayers.Capacity)
        ScreenManager._sortedActiveLayersCopy = new ScreenLayer[sortedActiveLayers.Capacity];
      sortedActiveLayers.CopyTo(ScreenManager._sortedActiveLayersCopy);
      for (int index = sortedActiveLayers.Count - 1; index >= 0; --index)
      {
        ScreenLayer screenLayer = ScreenManager._sortedActiveLayersCopy[index];
        if (screenLayer.IsActive && screenLayer.IsFocusLayer && layer != screenLayer)
        {
          ScreenManager._focusedLayer = screenLayer;
          return;
        }
      }
      ScreenManager._sortedActiveLayersCopy.Initialize();
      ScreenManager._focusedLayer = (ScreenLayer) null;
    }

    private static bool FocusTest(ScreenLayer layer) => ScreenManager._focusedLayer == layer;

    public static void OnScaleChange(float newScale)
    {
      ScreenManager.Scale = newScale;
      foreach (GlobalLayer globalLayer in ScreenManager._globalLayers)
        globalLayer.UpdateLayout();
      foreach (ScreenBase screen in ScreenManager._screenList)
        screen.UpdateLayout();
    }

    public enum GamepadNavigationTypes
    {
      None,
      Up,
      Down,
      Left,
      Right,
    }

    public enum CursorAreas
    {
      Center,
      Left,
      Right,
      Bottom,
      Top,
    }

    public delegate void OnPushScreenEvent(ScreenBase pushedScreen);

    public delegate void OnPopScreenEvent(ScreenBase poppedScreen);
  }
}
