// Decompiled with JetBrains decompiler
// Type: TaleWorlds.MountAndBlade.GameKeyCategory.PhotoModeHotKeyCategory
// Assembly: TaleWorlds.MountAndBlade, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3C0E3F42-756F-4407-B817-06BA412A78B0
// Assembly location: E:\SteamLibrary\steamapps\common\Mount & Blade II Bannerlord\bin\Win64_Shipping_Client\TaleWorlds.MountAndBlade.dll

using TaleWorlds.InputSystem;

namespace TaleWorlds.MountAndBlade.GameKeyCategory
{
  public class PhotoModeHotKeyCategory : GenericPanelGameKeyCategory
  {
    public new const string CategoryId = "PhotoModeHotKeyCategory";
    public const int HideUI = 81;
    public const int CameraRollLeft = 82;
    public const int CameraRollRight = 83;
    public const int ToggleCameraFollowMode = 85;
    public const int TakePicture = 84;
    public const int ToggleMouse = 86;
    public const int ToggleVignette = 87;
    public const int Reset = 94;

    public PhotoModeHotKeyCategory()
      : base(nameof (PhotoModeHotKeyCategory))
    {
      this.RegisterHotKeys();
      this.RegisterGameKeys();
      this.RegisterGameAxisKeys();
    }

    private void RegisterHotKeys()
    {
    }

    private void RegisterGameKeys()
    {
      this.RegisterGameKey(new GameKey(81, "HideUI", nameof (PhotoModeHotKeyCategory), InputKey.H, InputKey.ControllerRUp, GameKeyMainCategories.PhotoModeCategory));
      this.RegisterGameKey(new GameKey(82, "CameraRollLeft", nameof (PhotoModeHotKeyCategory), InputKey.Q, InputKey.ControllerLBumper, GameKeyMainCategories.PhotoModeCategory));
      this.RegisterGameKey(new GameKey(83, "CameraRollRight", nameof (PhotoModeHotKeyCategory), InputKey.E, InputKey.ControllerRBumper, GameKeyMainCategories.PhotoModeCategory));
      this.RegisterGameKey(new GameKey(85, "ToggleCameraFollowMode", nameof (PhotoModeHotKeyCategory), InputKey.V, InputKey.ControllerRLeft, GameKeyMainCategories.PhotoModeCategory));
      this.RegisterGameKey(new GameKey(84, "TakePicture", nameof (PhotoModeHotKeyCategory), InputKey.Enter, InputKey.ControllerRDown, GameKeyMainCategories.PhotoModeCategory));
      this.RegisterGameKey(new GameKey(86, "ToggleMouse", nameof (PhotoModeHotKeyCategory), InputKey.C, InputKey.ControllerLThumb, GameKeyMainCategories.PhotoModeCategory));
      this.RegisterGameKey(new GameKey(87, "ToggleVignette", nameof (PhotoModeHotKeyCategory), InputKey.X, InputKey.ControllerRThumb, GameKeyMainCategories.PhotoModeCategory));
      this.RegisterGameKey(new GameKey(94, "Reset", nameof (PhotoModeHotKeyCategory), InputKey.T, InputKey.ControllerLOption, GameKeyMainCategories.PhotoModeCategory));
    }

    private void RegisterGameAxisKeys()
    {
    }
  }
}
