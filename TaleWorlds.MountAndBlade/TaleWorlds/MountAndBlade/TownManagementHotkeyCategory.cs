// Decompiled with JetBrains decompiler
// Type: TaleWorlds.MountAndBlade.TownManagementHotkeyCategory
// Assembly: TaleWorlds.MountAndBlade, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3C0E3F42-756F-4407-B817-06BA412A78B0
// Assembly location: E:\SteamLibrary\steamapps\common\Mount & Blade II Bannerlord\bin\Win64_Shipping_Client\TaleWorlds.MountAndBlade.dll

using TaleWorlds.InputSystem;

namespace TaleWorlds.MountAndBlade
{
  public class TownManagementHotkeyCategory : GenericPanelGameKeyCategory
  {
    public new const string CategoryId = "TownManagementHotkeyCategory";

    public TownManagementHotkeyCategory()
      : base(nameof (TownManagementHotkeyCategory))
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
      foreach (GameKey gameKey in GenericGameKeyContext.Current.GameKeys)
        this.RegisterGameKey(gameKey);
    }

    private void RegisterGameAxisKeys()
    {
    }
  }
}
