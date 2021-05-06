// Decompiled with JetBrains decompiler
// Type: TaleWorlds.MountAndBlade.InventoryHotKeyCategory
// Assembly: TaleWorlds.MountAndBlade, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3C0E3F42-756F-4407-B817-06BA412A78B0
// Assembly location: E:\SteamLibrary\steamapps\common\Mount & Blade II Bannerlord\bin\Win64_Shipping_Client\TaleWorlds.MountAndBlade.dll

using TaleWorlds.InputSystem;

namespace TaleWorlds.MountAndBlade
{
  public class InventoryHotKeyCategory : GenericCampaignPanelsGameKeyCategory
  {
    public new const string CategoryId = "InventoryHotKeyCategory";
    public const string SwitchAlternative = "SwitchAlternative";

    public InventoryHotKeyCategory()
      : base(nameof (InventoryHotKeyCategory))
    {
      this.RegisterHotKeys();
      this.RegisterGameKeys();
      this.RegisterGameAxisKeys();
    }

    private void RegisterHotKeys() => this.RegisterHotKey(new HotKey("SwitchAlternative", nameof (InventoryHotKeyCategory), InputKey.LeftAlt));

    private void RegisterGameKeys()
    {
    }

    private void RegisterGameAxisKeys()
    {
    }
  }
}
