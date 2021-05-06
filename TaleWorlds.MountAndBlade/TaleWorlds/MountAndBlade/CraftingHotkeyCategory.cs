// Decompiled with JetBrains decompiler
// Type: TaleWorlds.MountAndBlade.CraftingHotkeyCategory
// Assembly: TaleWorlds.MountAndBlade, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3C0E3F42-756F-4407-B817-06BA412A78B0
// Assembly location: E:\SteamLibrary\steamapps\common\Mount & Blade II Bannerlord\bin\Win64_Shipping_Client\TaleWorlds.MountAndBlade.dll

using TaleWorlds.InputSystem;

namespace TaleWorlds.MountAndBlade
{
  public class CraftingHotkeyCategory : GenericCampaignPanelsGameKeyCategory
  {
    public new const string CategoryId = "CraftingHotkeyCategory";
    public const string Zoom = "Zoom";
    public const string Rotate = "Rotate";
    public const string Ascend = "Ascend";
    public const string ResetCamera = "ResetCamera";
    public const string Copy = "Copy";
    public const string Paste = "Paste";

    public CraftingHotkeyCategory()
      : base(nameof (CraftingHotkeyCategory))
    {
      this.RegisterHotKeys();
      this.RegisterGameKeys();
      this.RegisterGameAxisKeys();
    }

    private void RegisterHotKeys()
    {
      this.RegisterHotKey(new HotKey("Ascend", nameof (CraftingHotkeyCategory), InputKey.MiddleMouseButton));
      this.RegisterHotKey(new HotKey("Rotate", nameof (CraftingHotkeyCategory), InputKey.LeftMouseButton));
      this.RegisterHotKey(new HotKey("Zoom", nameof (CraftingHotkeyCategory), InputKey.RightMouseButton));
      this.RegisterHotKey(new HotKey("Copy", nameof (CraftingHotkeyCategory), InputKey.C));
      this.RegisterHotKey(new HotKey("Paste", nameof (CraftingHotkeyCategory), InputKey.V));
    }

    private void RegisterGameKeys()
    {
    }

    private void RegisterGameAxisKeys()
    {
    }
  }
}
