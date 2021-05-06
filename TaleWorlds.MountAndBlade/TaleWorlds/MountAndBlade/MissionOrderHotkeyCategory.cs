// Decompiled with JetBrains decompiler
// Type: TaleWorlds.MountAndBlade.MissionOrderHotkeyCategory
// Assembly: TaleWorlds.MountAndBlade, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3C0E3F42-756F-4407-B817-06BA412A78B0
// Assembly location: E:\SteamLibrary\steamapps\common\Mount & Blade II Bannerlord\bin\Win64_Shipping_Client\TaleWorlds.MountAndBlade.dll

using TaleWorlds.InputSystem;

namespace TaleWorlds.MountAndBlade
{
  public class MissionOrderHotkeyCategory : GameKeyContext
  {
    public const string CategoryId = "MissionOrderHotkeyCategory";
    public const int ViewOrders = 58;
    public const int SelectOrder1 = 59;
    public const int SelectOrder2 = 60;
    public const int SelectOrder3 = 61;
    public const int SelectOrder4 = 62;
    public const int SelectOrder5 = 63;
    public const int SelectOrder6 = 64;
    public const int SelectOrder7 = 65;
    public const int SelectOrder8 = 66;
    public const int SelectOrderReturn = 67;
    public const int EveryoneHear = 68;
    public const int Group0Hear = 69;
    public const int Group1Hear = 70;
    public const int Group2Hear = 71;
    public const int Group3Hear = 72;
    public const int Group4Hear = 73;
    public const int Group5Hear = 74;
    public const int Group6Hear = 75;
    public const int Group7Hear = 76;
    public const int HoldOrder = 77;
    public const int SelectNextGroup = 78;
    public const int SelectPreviousGroup = 79;
    public const int ToggleGroupSelection = 80;

    public MissionOrderHotkeyCategory()
      : base(nameof (MissionOrderHotkeyCategory), 95)
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
      this.RegisterGameKey(new GameKey(58, "ViewOrders", nameof (MissionOrderHotkeyCategory), InputKey.BackSpace, GameKeyMainCategories.OrderMenuCategory));
      this.RegisterGameKey(new GameKey(59, "SelectOrder1", nameof (MissionOrderHotkeyCategory), InputKey.F1, InputKey.ControllerRLeft, GameKeyMainCategories.OrderMenuCategory));
      this.RegisterGameKey(new GameKey(60, "SelectOrder2", nameof (MissionOrderHotkeyCategory), InputKey.F2, InputKey.ControllerRDown, GameKeyMainCategories.OrderMenuCategory));
      this.RegisterGameKey(new GameKey(61, "SelectOrder3", nameof (MissionOrderHotkeyCategory), InputKey.F3, InputKey.ControllerRRight, GameKeyMainCategories.OrderMenuCategory));
      this.RegisterGameKey(new GameKey(62, "SelectOrder4", nameof (MissionOrderHotkeyCategory), InputKey.F4, GameKeyMainCategories.OrderMenuCategory));
      this.RegisterGameKey(new GameKey(63, "SelectOrder5", nameof (MissionOrderHotkeyCategory), InputKey.F5, GameKeyMainCategories.OrderMenuCategory));
      this.RegisterGameKey(new GameKey(64, "SelectOrder6", nameof (MissionOrderHotkeyCategory), InputKey.F6, GameKeyMainCategories.OrderMenuCategory));
      this.RegisterGameKey(new GameKey(65, "SelectOrder7", nameof (MissionOrderHotkeyCategory), InputKey.F7, GameKeyMainCategories.OrderMenuCategory));
      this.RegisterGameKey(new GameKey(66, "SelectOrder8", nameof (MissionOrderHotkeyCategory), InputKey.F8, GameKeyMainCategories.OrderMenuCategory));
      this.RegisterGameKey(new GameKey(67, "SelectOrderReturn", nameof (MissionOrderHotkeyCategory), InputKey.F9, GameKeyMainCategories.OrderMenuCategory));
      this.RegisterGameKey(new GameKey(68, "EveryoneHear", nameof (MissionOrderHotkeyCategory), InputKey.D0, GameKeyMainCategories.OrderMenuCategory));
      this.RegisterGameKey(new GameKey(69, "Group0Hear", nameof (MissionOrderHotkeyCategory), InputKey.D1, GameKeyMainCategories.OrderMenuCategory));
      this.RegisterGameKey(new GameKey(70, "Group1Hear", nameof (MissionOrderHotkeyCategory), InputKey.D2, GameKeyMainCategories.OrderMenuCategory));
      this.RegisterGameKey(new GameKey(71, "Group2Hear", nameof (MissionOrderHotkeyCategory), InputKey.D3, GameKeyMainCategories.OrderMenuCategory));
      this.RegisterGameKey(new GameKey(72, "Group3Hear", nameof (MissionOrderHotkeyCategory), InputKey.D4, GameKeyMainCategories.OrderMenuCategory));
      this.RegisterGameKey(new GameKey(73, "Group4Hear", nameof (MissionOrderHotkeyCategory), InputKey.D5, GameKeyMainCategories.OrderMenuCategory));
      this.RegisterGameKey(new GameKey(74, "Group5Hear", nameof (MissionOrderHotkeyCategory), InputKey.D6, GameKeyMainCategories.OrderMenuCategory));
      this.RegisterGameKey(new GameKey(75, "Group6Hear", nameof (MissionOrderHotkeyCategory), InputKey.D7, GameKeyMainCategories.OrderMenuCategory));
      this.RegisterGameKey(new GameKey(76, "Group7Hear", nameof (MissionOrderHotkeyCategory), InputKey.D8, GameKeyMainCategories.OrderMenuCategory));
      this.RegisterGameKey(new GameKey(77, "HoldOrder", nameof (MissionOrderHotkeyCategory), InputKey.Invalid, InputKey.ControllerLBumper, GameKeyMainCategories.OrderMenuCategory));
      this.RegisterGameKey(new GameKey(78, "SelectNextGroup", nameof (MissionOrderHotkeyCategory), InputKey.Invalid, InputKey.ControllerLRight, GameKeyMainCategories.OrderMenuCategory));
      this.RegisterGameKey(new GameKey(79, "SelectPreviousGroup", nameof (MissionOrderHotkeyCategory), InputKey.Invalid, InputKey.ControllerLLeft, GameKeyMainCategories.OrderMenuCategory));
      this.RegisterGameKey(new GameKey(80, "ToggleGroupSelection", nameof (MissionOrderHotkeyCategory), InputKey.Invalid, InputKey.ControllerLDown, GameKeyMainCategories.OrderMenuCategory));
    }

    private void RegisterGameAxisKeys()
    {
    }
  }
}
