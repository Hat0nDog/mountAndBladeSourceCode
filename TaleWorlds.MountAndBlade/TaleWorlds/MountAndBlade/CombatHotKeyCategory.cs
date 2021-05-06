// Decompiled with JetBrains decompiler
// Type: TaleWorlds.MountAndBlade.CombatHotKeyCategory
// Assembly: TaleWorlds.MountAndBlade, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3C0E3F42-756F-4407-B817-06BA412A78B0
// Assembly location: E:\SteamLibrary\steamapps\common\Mount & Blade II Bannerlord\bin\Win64_Shipping_Client\TaleWorlds.MountAndBlade.dll

using System;
using TaleWorlds.InputSystem;

namespace TaleWorlds.MountAndBlade
{
  public class CombatHotKeyCategory : GenericPanelGameKeyCategory
  {
    public new const string CategoryId = "CombatHotKeyCategory";
    public const int MoveForward = 0;
    public const int MoveLeft = 2;
    public const int MoveBackward = 1;
    public const int MoveRight = 3;
    public const string MoveAxisX = "MovementAxisX";
    public const string MoveAxisY = "MovementAxisY";
    public const string CameraAxisX = "CameraAxisX";
    public const string CameraAxisY = "CameraAxisY";
    public const int MissionScreenHotkeyCameraZoomIn = 28;
    public const int MissionScreenHotkeyCameraZoomOut = 29;
    public const int Action = 13;
    public const int Jump = 14;
    public const int Crouch = 15;
    public const int Attack = 9;
    public const int Defend = 10;
    public const int Kick = 16;
    public const int ToggleWeaponMode = 17;
    public const int ToggleWalkMode = 30;
    public const int EquipWeapon1 = 18;
    public const int EquipWeapon2 = 19;
    public const int EquipWeapon3 = 20;
    public const int EquipWeapon4 = 21;
    public const int EquipPrimaryWeapon = 11;
    public const int EquipSecondaryWeapon = 12;
    public const int DropWeapon = 22;
    public const int SheathWeapon = 23;
    public const int Zoom = 24;
    public const int ViewCharacter = 25;
    public const int LockTarget = 26;
    public const int CameraToggle = 27;
    public const int Cheer = 31;
    public const int ShowIndicators = 5;
    public const string DeploymentCameraIsActive = "DeploymentCameraIsActive";
    public const string ToggleZoom = "ToggleZoom";

    public CombatHotKeyCategory()
      : base(nameof (CombatHotKeyCategory))
    {
      this.RegisterHotKeys();
      this.RegisterGameKeys();
      this.RegisterGameAxisKeys();
    }

    private void RegisterHotKeys()
    {
      this.RegisterHotKey(new HotKey("DeploymentCameraIsActive", nameof (CombatHotKeyCategory), InputKey.MiddleMouseButton));
      this.RegisterHotKey(new HotKey("ToggleZoom", nameof (CombatHotKeyCategory), InputKey.ControllerRThumb));
    }

    private void RegisterGameKeys()
    {
      this.RegisterGameKey(GenericGameKeyContext.Current.GameKeys.Find((Predicate<GameKey>) (g => g.Id.Equals(0))));
      this.RegisterGameKey(GenericGameKeyContext.Current.GameKeys.Find((Predicate<GameKey>) (g => g.Id.Equals(1))));
      this.RegisterGameKey(GenericGameKeyContext.Current.GameKeys.Find((Predicate<GameKey>) (g => g.Id.Equals(3))));
      this.RegisterGameKey(GenericGameKeyContext.Current.GameKeys.Find((Predicate<GameKey>) (g => g.Id.Equals(2))));
      this.RegisterGameKey(new GameKey(9, "Attack", nameof (CombatHotKeyCategory), InputKey.LeftMouseButton, InputKey.ControllerRTrigger, GameKeyMainCategories.ActionCategory));
      this.RegisterGameKey(new GameKey(10, "Defend", nameof (CombatHotKeyCategory), InputKey.RightMouseButton, InputKey.ControllerLTrigger, GameKeyMainCategories.ActionCategory));
      this.RegisterGameKey(new GameKey(11, "EquipPrimaryWeapon", nameof (CombatHotKeyCategory), InputKey.MouseScrollUp, InputKey.ControllerRBumper, GameKeyMainCategories.ActionCategory));
      this.RegisterGameKey(new GameKey(12, "EquipSecondaryWeapon", nameof (CombatHotKeyCategory), InputKey.MouseScrollDown, GameKeyMainCategories.ActionCategory));
      this.RegisterGameKey(new GameKey(13, "Action", nameof (CombatHotKeyCategory), InputKey.F, InputKey.ControllerRUp, GameKeyMainCategories.ActionCategory));
      this.RegisterGameKey(new GameKey(14, "Jump", nameof (CombatHotKeyCategory), InputKey.Space, InputKey.ControllerRDown, GameKeyMainCategories.ActionCategory));
      this.RegisterGameKey(new GameKey(15, "Crouch", nameof (CombatHotKeyCategory), InputKey.Z, InputKey.ControllerLDown, GameKeyMainCategories.ActionCategory));
      this.RegisterGameKey(new GameKey(16, "Kick", nameof (CombatHotKeyCategory), InputKey.E, InputKey.ControllerRLeft, GameKeyMainCategories.ActionCategory));
      this.RegisterGameKey(new GameKey(17, "ToggleWeaponMode", nameof (CombatHotKeyCategory), InputKey.X, InputKey.ControllerLUp, GameKeyMainCategories.ActionCategory));
      this.RegisterGameKey(new GameKey(18, "EquipWeapon1", nameof (CombatHotKeyCategory), InputKey.Numpad1, GameKeyMainCategories.ActionCategory));
      this.RegisterGameKey(new GameKey(19, "EquipWeapon2", nameof (CombatHotKeyCategory), InputKey.Numpad2, GameKeyMainCategories.ActionCategory));
      this.RegisterGameKey(new GameKey(20, "EquipWeapon3", nameof (CombatHotKeyCategory), InputKey.Numpad3, GameKeyMainCategories.ActionCategory));
      this.RegisterGameKey(new GameKey(21, "EquipWeapon4", nameof (CombatHotKeyCategory), InputKey.Numpad4, GameKeyMainCategories.ActionCategory));
      this.RegisterGameKey(new GameKey(22, "DropWeapon", nameof (CombatHotKeyCategory), InputKey.G, GameKeyMainCategories.ActionCategory));
      this.RegisterGameKey(new GameKey(23, "SheathWeapon", nameof (CombatHotKeyCategory), InputKey.BackSlash, GameKeyMainCategories.ActionCategory));
      this.RegisterGameKey(new GameKey(24, "Zoom", nameof (CombatHotKeyCategory), InputKey.LeftShift, GameKeyMainCategories.ActionCategory));
      this.RegisterGameKey(new GameKey(25, "ViewCharacter", nameof (CombatHotKeyCategory), InputKey.Tilde, InputKey.ControllerLLeft, GameKeyMainCategories.ActionCategory));
      this.RegisterGameKey(new GameKey(26, "LockTarget", nameof (CombatHotKeyCategory), InputKey.MiddleMouseButton, GameKeyMainCategories.ActionCategory));
      this.RegisterGameKey(new GameKey(27, "CameraToggle", nameof (CombatHotKeyCategory), InputKey.R, InputKey.ControllerLThumb, GameKeyMainCategories.ActionCategory));
      this.RegisterGameKey(new GameKey(28, "MissionScreenHotkeyCameraZoomIn", nameof (CombatHotKeyCategory), InputKey.NumpadPlus, GameKeyMainCategories.ActionCategory));
      this.RegisterGameKey(new GameKey(29, "MissionScreenHotkeyCameraZoomOut", nameof (CombatHotKeyCategory), InputKey.NumpadMinus, GameKeyMainCategories.ActionCategory));
      this.RegisterGameKey(new GameKey(30, "ToggleWalkMode", nameof (CombatHotKeyCategory), InputKey.CapsLock, GameKeyMainCategories.ActionCategory));
      this.RegisterGameKey(new GameKey(31, "Cheer", nameof (CombatHotKeyCategory), InputKey.O, GameKeyMainCategories.ActionCategory));
      this.RegisterGameKey(GenericGameKeyContext.Current.GameKeys.Find((Predicate<GameKey>) (g => g.Id.Equals(5))));
    }

    private void RegisterGameAxisKeys()
    {
      GameAxisKey gameKey1 = GenericGameKeyContext.Current.GameAxisKeys.Find((Predicate<GameAxisKey>) (g => g.Id.Equals("MovementAxisX")));
      GameAxisKey gameKey2 = GenericGameKeyContext.Current.GameAxisKeys.Find((Predicate<GameAxisKey>) (g => g.Id.Equals("MovementAxisY")));
      this.RegisterGameAxisKey(gameKey1);
      this.RegisterGameAxisKey(gameKey2);
      GameAxisKey gameKey3 = GenericGameKeyContext.Current.GameAxisKeys.Find((Predicate<GameAxisKey>) (g => g.Id.Equals("CameraAxisX")));
      GameAxisKey gameKey4 = GenericGameKeyContext.Current.GameAxisKeys.Find((Predicate<GameAxisKey>) (g => g.Id.Equals("CameraAxisY")));
      this.RegisterGameAxisKey(gameKey3);
      this.RegisterGameAxisKey(gameKey4);
    }
  }
}
