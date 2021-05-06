// Decompiled with JetBrains decompiler
// Type: TaleWorlds.MountAndBlade.ScoreboardHotKeyCategory
// Assembly: TaleWorlds.MountAndBlade, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3C0E3F42-756F-4407-B817-06BA412A78B0
// Assembly location: E:\SteamLibrary\steamapps\common\Mount & Blade II Bannerlord\bin\Win64_Shipping_Client\TaleWorlds.MountAndBlade.dll

using System;
using System.Collections.Generic;
using TaleWorlds.InputSystem;

namespace TaleWorlds.MountAndBlade
{
  public class ScoreboardHotKeyCategory : GameKeyContext
  {
    public const string CategoryId = "ScoreboardHotKeyCategory";
    public const string Quit = "Quit";
    public const int ToggleShow = 4;
    public const int ShowMouse = 32;
    public const string HoldShow = "HoldShow";
    public const string Play = "Play";
    public const string FastForward = "FastForward";
    public const string MenuShowContextMenu = "MenuShowContextMenu";

    public ScoreboardHotKeyCategory()
      : base(nameof (ScoreboardHotKeyCategory), 95)
    {
      this.RegisterHotKeys();
      this.RegisterGameKeys();
      this.RegisterGameAxisKeys();
    }

    private void RegisterHotKeys()
    {
      this.RegisterHotKey(new HotKey("Play", nameof (ScoreboardHotKeyCategory), new List<Key>()
      {
        new Key(InputKey.D1),
        new Key(InputKey.ControllerRLeft)
      }));
      this.RegisterHotKey(new HotKey("FastForward", nameof (ScoreboardHotKeyCategory), new List<Key>()
      {
        new Key(InputKey.D2),
        new Key(InputKey.ControllerRUp)
      }));
      this.RegisterHotKey(new HotKey("Quit", nameof (ScoreboardHotKeyCategory), new List<Key>()
      {
        new Key(InputKey.F),
        new Key(InputKey.ControllerROption)
      }));
      this.RegisterHotKey(new HotKey("MenuShowContextMenu", nameof (ScoreboardHotKeyCategory), InputKey.RightMouseButton));
      this.RegisterHotKey(new HotKey("HoldShow", nameof (ScoreboardHotKeyCategory), new List<Key>()
      {
        new Key(InputKey.Tab),
        new Key(InputKey.ControllerRRight)
      }));
    }

    private void RegisterGameKeys()
    {
      this.RegisterGameKey(GenericGameKeyContext.Current.GameKeys.Find((Predicate<GameKey>) (g => g.Id.Equals(4))));
      this.RegisterGameKey(new GameKey(32, "ShowMouse", nameof (ScoreboardHotKeyCategory), InputKey.MiddleMouseButton, InputKey.ControllerLThumb, GameKeyMainCategories.ActionCategory));
    }

    private void RegisterGameAxisKeys()
    {
    }
  }
}
