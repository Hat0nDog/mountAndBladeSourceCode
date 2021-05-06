// Decompiled with JetBrains decompiler
// Type: TaleWorlds.MountAndBlade.FaceGenHotkeyCategory
// Assembly: TaleWorlds.MountAndBlade, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3C0E3F42-756F-4407-B817-06BA412A78B0
// Assembly location: E:\SteamLibrary\steamapps\common\Mount & Blade II Bannerlord\bin\Win64_Shipping_Client\TaleWorlds.MountAndBlade.dll

using System;
using TaleWorlds.InputSystem;

namespace TaleWorlds.MountAndBlade
{
  public class FaceGenHotkeyCategory : GenericCampaignPanelsGameKeyCategory
  {
    public new const string CategoryId = "FaceGenHotkeyCategory";
    public const string Zoom = "Zoom";
    public const string ControllerZoomIn = "ControllerZoomIn";
    public const string ControllerZoomOut = "ControllerZoomOut";
    public const string Rotate = "Rotate";
    public const string ControllerRotationAxis = "CameraAxisX";
    public const string Ascend = "Ascend";
    public const string Copy = "Copy";
    public const string Paste = "Paste";

    public FaceGenHotkeyCategory()
      : base(nameof (FaceGenHotkeyCategory))
    {
      this.RegisterHotKeys();
      this.RegisterGameKeys();
      this.RegisterGameAxisKeys();
    }

    private void RegisterHotKeys()
    {
      this.RegisterHotKey(new HotKey("Ascend", nameof (FaceGenHotkeyCategory), InputKey.MiddleMouseButton));
      this.RegisterHotKey(new HotKey("Rotate", nameof (FaceGenHotkeyCategory), InputKey.LeftMouseButton));
      this.RegisterHotKey(new HotKey("Zoom", nameof (FaceGenHotkeyCategory), InputKey.RightMouseButton));
      this.RegisterHotKey(new HotKey("Copy", nameof (FaceGenHotkeyCategory), InputKey.C));
      this.RegisterHotKey(new HotKey("Paste", nameof (FaceGenHotkeyCategory), InputKey.V));
      this.RegisterHotKey(new HotKey("ControllerZoomIn", nameof (FaceGenHotkeyCategory), InputKey.ControllerRTrigger));
      this.RegisterHotKey(new HotKey("ControllerZoomOut", nameof (FaceGenHotkeyCategory), InputKey.ControllerLTrigger));
    }

    private void RegisterGameKeys()
    {
    }

    private void RegisterGameAxisKeys() => this.RegisterGameAxisKey(GenericGameKeyContext.Current.GameAxisKeys.Find((Predicate<GameAxisKey>) (g => g.Id.Equals("CameraAxisX"))));
  }
}
