// Decompiled with JetBrains decompiler
// Type: TaleWorlds.Engine.InputSystem.CheatsHotKeyCategory
// Assembly: TaleWorlds.Engine, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: DD7DC4E8-6EB6-4CB8-8DE1-FC78E45F7FE8
// Assembly location: E:\SteamLibrary\steamapps\common\Mount & Blade II Bannerlord\bin\Win64_Shipping_Client\TaleWorlds.Engine.dll

using TaleWorlds.InputSystem;

namespace TaleWorlds.Engine.InputSystem
{
  public class CheatsHotKeyCategory : GameKeyContext
  {
    public const string CategoryId = "Cheats";
    public const string MissionScreenHotkeyIncreaseCameraSpeed = "MissionScreenHotkeyIncreaseCameraSpeed";
    public const string MissionScreenHotkeyDecreaseCameraSpeed = "MissionScreenHotkeyDecreaseCameraSpeed";
    public const string ResetCameraSpeed = "ResetCameraSpeed";
    public const string MissionScreenHotkeyIncreaseSlowMotionFactor = "MissionScreenHotkeyIncreaseSlowMotionFactor";
    public const string MissionScreenHotkeyDecreaseSlowMotionFactor = "MissionScreenHotkeyDecreaseSlowMotionFactor";
    public const string EnterSlowMotion = "EnterSlowMotion";
    public const string Pause = "Pause";
    public const string MissionScreenHotkeyHealYourSelf = "MissionScreenHotkeyHealYourSelf";
    public const string MissionScreenHotkeyHealYourHorse = "MissionScreenHotkeyHealYourHorse";
    public const string MissionScreenHotkeyKillEnemyAgent = "MissionScreenHotkeyKillEnemyAgent";
    public const string MissionScreenHotkeyKillAllEnemyAgents = "MissionScreenHotkeyKillAllEnemyAgents";
    public const string MissionScreenHotkeyKillEnemyHorse = "MissionScreenHotkeyKillEnemyHorse";
    public const string MissionScreenHotkeyKillAllEnemyHorses = "MissionScreenHotkeyKillAllEnemyHorses";
    public const string MissionScreenHotkeyKillFriendlyAgent = "MissionScreenHotkeyKillFriendlyAgent";
    public const string MissionScreenHotkeyKillAllFriendlyAgents = "MissionScreenHotkeyKillAllFriendlyAgents";
    public const string MissionScreenHotkeyKillFriendlyHorse = "MissionScreenHotkeyKillFriendlyHorse";
    public const string MissionScreenHotkeyKillAllFriendlyHorses = "MissionScreenHotkeyKillAllFriendlyHorses";
    public const string MissionScreenHotkeyKillYourSelf = "MissionScreenHotkeyKillYourSelf";
    public const string MissionScreenHotkeyKillYourHorse = "MissionScreenHotkeyKillYourHorse";
    public const string MissionScreenHotkeyGhostCam = "MissionScreenHotkeyGhostCam";
    public const string MissionScreenHotkeySwitchAgentToAi = "MissionScreenHotkeySwitchAgentToAi";
    public const string MissionScreenHotkeyControlFollowedAgent = "MissionScreenHotkeyControlFollowedAgent";
    public const string MissionScreenHotkeyTeleportMainAgent = "MissionScreenHotkeyTeleportMainAgent";

    public CheatsHotKeyCategory()
      : base("Cheats", 0)
    {
      this.RegisterCheatHotkey(nameof (Pause), InputKey.F11, HotKey.Modifiers.Control);
      this.RegisterCheatHotkey(nameof (MissionScreenHotkeyIncreaseCameraSpeed), InputKey.Up, HotKey.Modifiers.Control);
      this.RegisterCheatHotkey(nameof (MissionScreenHotkeyDecreaseCameraSpeed), InputKey.Down, HotKey.Modifiers.Control);
      this.RegisterCheatHotkey(nameof (ResetCameraSpeed), InputKey.MiddleMouseButton, HotKey.Modifiers.Control);
      this.RegisterCheatHotkey(nameof (MissionScreenHotkeyIncreaseSlowMotionFactor), InputKey.NumpadPlus, HotKey.Modifiers.Shift);
      this.RegisterCheatHotkey(nameof (MissionScreenHotkeyDecreaseSlowMotionFactor), InputKey.NumpadMinus, HotKey.Modifiers.Shift);
      this.RegisterCheatHotkey(nameof (EnterSlowMotion), InputKey.F9, HotKey.Modifiers.Control);
      this.RegisterCheatHotkey(nameof (MissionScreenHotkeySwitchAgentToAi), InputKey.F5, HotKey.Modifiers.Control);
      this.RegisterCheatHotkey(nameof (MissionScreenHotkeyControlFollowedAgent), InputKey.Numpad5, HotKey.Modifiers.Alt | HotKey.Modifiers.Control);
      this.RegisterCheatHotkey(nameof (MissionScreenHotkeyHealYourSelf), InputKey.H, HotKey.Modifiers.Control, HotKey.Modifiers.Shift);
      this.RegisterCheatHotkey(nameof (MissionScreenHotkeyHealYourHorse), InputKey.H, HotKey.Modifiers.Shift | HotKey.Modifiers.Control);
      this.RegisterCheatHotkey(nameof (MissionScreenHotkeyKillEnemyAgent), InputKey.F4, HotKey.Modifiers.Control);
      this.RegisterCheatHotkey(nameof (MissionScreenHotkeyKillAllEnemyAgents), InputKey.F4, HotKey.Modifiers.Alt | HotKey.Modifiers.Control);
      this.RegisterCheatHotkey(nameof (MissionScreenHotkeyKillEnemyHorse), InputKey.F4, HotKey.Modifiers.Shift | HotKey.Modifiers.Control);
      this.RegisterCheatHotkey(nameof (MissionScreenHotkeyKillAllEnemyHorses), InputKey.F4, HotKey.Modifiers.Shift | HotKey.Modifiers.Alt | HotKey.Modifiers.Control);
      this.RegisterCheatHotkey(nameof (MissionScreenHotkeyKillFriendlyAgent), InputKey.F2, HotKey.Modifiers.Control);
      this.RegisterCheatHotkey(nameof (MissionScreenHotkeyKillAllFriendlyAgents), InputKey.F2, HotKey.Modifiers.Alt | HotKey.Modifiers.Control);
      this.RegisterCheatHotkey(nameof (MissionScreenHotkeyKillFriendlyHorse), InputKey.F2, HotKey.Modifiers.Shift | HotKey.Modifiers.Control);
      this.RegisterCheatHotkey(nameof (MissionScreenHotkeyKillAllFriendlyHorses), InputKey.F2, HotKey.Modifiers.Shift | HotKey.Modifiers.Alt | HotKey.Modifiers.Control);
      this.RegisterCheatHotkey(nameof (MissionScreenHotkeyKillYourSelf), InputKey.F3, HotKey.Modifiers.Control);
      this.RegisterCheatHotkey(nameof (MissionScreenHotkeyKillYourHorse), InputKey.F3, HotKey.Modifiers.Shift | HotKey.Modifiers.Control);
      this.RegisterCheatHotkey(nameof (MissionScreenHotkeyGhostCam), InputKey.K, HotKey.Modifiers.Control);
      this.RegisterCheatHotkey(nameof (MissionScreenHotkeyTeleportMainAgent), InputKey.X, HotKey.Modifiers.Alt);
    }

    private void RegisterCheatHotkey(
      string id,
      InputKey hotkeyKey,
      HotKey.Modifiers modifiers,
      HotKey.Modifiers negativeModifiers = HotKey.Modifiers.None)
    {
      this.RegisterHotKey(new HotKey(id, "Cheats", hotkeyKey, modifiers, negativeModifiers));
    }
  }
}
