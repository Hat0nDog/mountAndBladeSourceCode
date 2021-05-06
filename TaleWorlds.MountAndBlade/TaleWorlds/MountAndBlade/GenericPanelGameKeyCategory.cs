// Decompiled with JetBrains decompiler
// Type: TaleWorlds.MountAndBlade.GenericPanelGameKeyCategory
// Assembly: TaleWorlds.MountAndBlade, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3C0E3F42-756F-4407-B817-06BA412A78B0
// Assembly location: E:\SteamLibrary\steamapps\common\Mount & Blade II Bannerlord\bin\Win64_Shipping_Client\TaleWorlds.MountAndBlade.dll

using System;
using System.Collections.Generic;
using TaleWorlds.InputSystem;

namespace TaleWorlds.MountAndBlade
{
  public class GenericPanelGameKeyCategory : GameKeyContext
  {
    public const string CategoryId = "GenericPanelGameKeyCategory";
    public const string Exit = "Exit";
    public const string Confirm = "Confirm";
    public const string ToggleEscapeMenu = "ToggleEscapeMenu";
    public const string SwitchToPreviousTab = "SwitchToPreviousTab";
    public const string SwitchToNextTab = "SwitchToNextTab";
    public const string GiveAll = "GiveAll";
    public const string TakeAll = "TakeAll";
    public const string Randomize = "Randomize";
    public const string Start = "Start";
    public const string Delete = "Delete";
    public const int Leave = 4;

    public GenericPanelGameKeyCategory(string categoryId = "GenericPanelGameKeyCategory")
      : base(categoryId, 95)
    {
      this.RegisterHotKeys();
      this.RegisterGameKeys();
      this.RegisterGameAxisKeys();
    }

    private void RegisterHotKeys()
    {
      List<Key> keys1 = new List<Key>()
      {
        new Key(InputKey.Escape),
        new Key(InputKey.ControllerRRight)
      };
      List<Key> keys2 = new List<Key>()
      {
        new Key(InputKey.Enter),
        new Key(InputKey.NumpadEnter),
        new Key(InputKey.ControllerRLeft)
      };
      List<Key> keys3 = new List<Key>()
      {
        new Key(InputKey.Escape),
        new Key(InputKey.ControllerROption)
      };
      List<Key> keys4 = new List<Key>()
      {
        new Key(InputKey.Q),
        new Key(InputKey.ControllerLBumper)
      };
      List<Key> keys5 = new List<Key>()
      {
        new Key(InputKey.E),
        new Key(InputKey.ControllerRBumper)
      };
      List<Key> keys6 = new List<Key>()
      {
        new Key(InputKey.D),
        new Key(InputKey.ControllerRTrigger)
      };
      List<Key> keys7 = new List<Key>()
      {
        new Key(InputKey.A),
        new Key(InputKey.ControllerLTrigger)
      };
      List<Key> keys8 = new List<Key>()
      {
        new Key(InputKey.R),
        new Key(InputKey.ControllerRUp)
      };
      List<Key> keys9 = new List<Key>()
      {
        new Key(InputKey.ControllerROption)
      };
      List<Key> keys10 = new List<Key>()
      {
        new Key(InputKey.Delete),
        new Key(InputKey.ControllerRUp)
      };
      this.RegisterHotKey(new HotKey("Exit", nameof (GenericPanelGameKeyCategory), keys1));
      this.RegisterHotKey(new HotKey("Confirm", nameof (GenericPanelGameKeyCategory), keys2));
      this.RegisterHotKey(new HotKey("ToggleEscapeMenu", nameof (GenericPanelGameKeyCategory), keys3));
      this.RegisterHotKey(new HotKey("SwitchToPreviousTab", nameof (GenericPanelGameKeyCategory), keys4));
      this.RegisterHotKey(new HotKey("SwitchToNextTab", nameof (GenericPanelGameKeyCategory), keys5));
      this.RegisterHotKey(new HotKey("GiveAll", nameof (GenericPanelGameKeyCategory), keys6));
      this.RegisterHotKey(new HotKey("TakeAll", nameof (GenericPanelGameKeyCategory), keys7));
      this.RegisterHotKey(new HotKey("Randomize", nameof (GenericPanelGameKeyCategory), keys8));
      this.RegisterHotKey(new HotKey("Start", nameof (GenericPanelGameKeyCategory), keys9));
      this.RegisterHotKey(new HotKey("Delete", nameof (GenericPanelGameKeyCategory), keys10));
    }

    private void RegisterGameKeys() => this.RegisterGameKey(GenericGameKeyContext.Current.GameKeys.Find((Predicate<GameKey>) (g => g.Id.Equals(4))));

    private void RegisterGameAxisKeys()
    {
    }
  }
}
