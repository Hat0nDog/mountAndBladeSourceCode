// Decompiled with JetBrains decompiler
// Type: TaleWorlds.MountAndBlade.MapHotKeyCategory
// Assembly: TaleWorlds.MountAndBlade, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3C0E3F42-756F-4407-B817-06BA412A78B0
// Assembly location: E:\SteamLibrary\steamapps\common\Mount & Blade II Bannerlord\bin\Win64_Shipping_Client\TaleWorlds.MountAndBlade.dll

using System;
using System.Collections.Generic;
using TaleWorlds.InputSystem;

namespace TaleWorlds.MountAndBlade
{
  public class MapHotKeyCategory : GameKeyContext
  {
    public const string CategoryId = "MapHotKeyCategory";
    public const int QuickSave = 46;
    public const int PartyMoveUp = 42;
    public const int PartyMoveLeft = 45;
    public const int PartyMoveDown = 43;
    public const int PartyMoveRight = 44;
    public const int MapMoveUp = 0;
    public const int MapMoveLeft = 2;
    public const int MapMoveDown = 1;
    public const int MapMoveRight = 3;
    public const int MapFastMove = 47;
    public const int MapZoomIn = 48;
    public const int MapZoomOut = 49;
    public const int MapCameraFollowMode = 54;
    public const int MapExtendInfoBar = 55;
    public const int MapTrackSettlement = 56;
    public const int MapGoToEncylopedia = 57;
    public const string MapClick = "MapClick";
    public const string MapFollowModifier = "MapFollowModifier";
    public const int MapTimeStop = 50;
    public const int MapTimeNormal = 51;
    public const int MapTimeFastForward = 52;
    public const int MapTimeTogglePause = 53;
    public const int MapShowPartyNames = 5;

    public MapHotKeyCategory()
      : base(nameof (MapHotKeyCategory), 95)
    {
      this.RegisterHotKeys();
      this.RegisterGameKeys();
      this.RegisterGameAxisKeys();
    }

    private void RegisterHotKeys()
    {
      this.RegisterHotKey(new HotKey("MapClick", nameof (MapHotKeyCategory), new List<Key>()
      {
        new Key(InputKey.LeftMouseButton),
        new Key(InputKey.ControllerRDown)
      }));
      this.RegisterHotKey(new HotKey("MapFollowModifier", nameof (MapHotKeyCategory), new List<Key>()
      {
        new Key(InputKey.LeftAlt),
        new Key(InputKey.ControllerRBumper)
      }));
    }

    private void RegisterGameKeys()
    {
      foreach (GameKey gameKey in GenericGameKeyContext.Current.GameKeys)
        this.RegisterGameKey(gameKey);
      this.RegisterGameKey(new GameKey(42, "PartyMoveUp", nameof (MapHotKeyCategory), InputKey.Up, GameKeyMainCategories.CampaignMapCategory));
      this.RegisterGameKey(new GameKey(43, "PartyMoveDown", nameof (MapHotKeyCategory), InputKey.Down, GameKeyMainCategories.CampaignMapCategory));
      this.RegisterGameKey(new GameKey(44, "PartyMoveRight", nameof (MapHotKeyCategory), InputKey.Right, GameKeyMainCategories.CampaignMapCategory));
      this.RegisterGameKey(new GameKey(45, "PartyMoveLeft", nameof (MapHotKeyCategory), InputKey.Left, GameKeyMainCategories.CampaignMapCategory));
      this.RegisterGameKey(new GameKey(46, "QuickSave", nameof (MapHotKeyCategory), InputKey.F5, GameKeyMainCategories.CampaignMapCategory));
      this.RegisterGameKey(new GameKey(47, "MapFastMove", nameof (MapHotKeyCategory), InputKey.LeftShift, GameKeyMainCategories.CampaignMapCategory));
      this.RegisterGameKey(new GameKey(48, "MapZoomIn", nameof (MapHotKeyCategory), InputKey.MouseScrollUp, InputKey.ControllerRTrigger, GameKeyMainCategories.CampaignMapCategory));
      this.RegisterGameKey(new GameKey(49, "MapZoomOut", nameof (MapHotKeyCategory), InputKey.MouseScrollDown, InputKey.ControllerLTrigger, GameKeyMainCategories.CampaignMapCategory));
      this.RegisterGameKey(new GameKey(50, "MapTimeStop", nameof (MapHotKeyCategory), InputKey.D1, GameKeyMainCategories.CampaignMapCategory));
      this.RegisterGameKey(new GameKey(51, "MapTimeNormal", nameof (MapHotKeyCategory), InputKey.D2, GameKeyMainCategories.CampaignMapCategory));
      this.RegisterGameKey(new GameKey(52, "MapTimeFastForward", nameof (MapHotKeyCategory), InputKey.D3, GameKeyMainCategories.CampaignMapCategory));
      this.RegisterGameKey(new GameKey(53, "MapTimeTogglePause", nameof (MapHotKeyCategory), InputKey.Space, InputKey.ControllerRLeft, GameKeyMainCategories.CampaignMapCategory));
      this.RegisterGameKey(new GameKey(54, "MapCameraFollowMode", nameof (MapHotKeyCategory), InputKey.Invalid, InputKey.ControllerLThumb, GameKeyMainCategories.CampaignMapCategory));
      this.RegisterGameKey(new GameKey(55, "MapExtendInfoBar", nameof (MapHotKeyCategory), InputKey.Invalid, InputKey.ControllerRBumper, GameKeyMainCategories.CampaignMapCategory));
      this.RegisterGameKey(new GameKey(56, "MapTrackSettlement", nameof (MapHotKeyCategory), InputKey.Invalid, InputKey.ControllerRThumb, GameKeyMainCategories.CampaignMapCategory));
      this.RegisterGameKey(new GameKey(57, "MapGoToEncylopedia", nameof (MapHotKeyCategory), InputKey.Invalid, InputKey.ControllerLBumper, GameKeyMainCategories.CampaignMapCategory));
      this.RegisterGameKey(GenericGameKeyContext.Current.GameKeys.Find((Predicate<GameKey>) (g => g.Id.Equals(5))));
    }

    private void RegisterGameAxisKeys()
    {
      GameAxisKey gameKey1 = GenericGameKeyContext.Current.GameAxisKeys.Find((Predicate<GameAxisKey>) (g => g.Id.Equals("CameraAxisX")));
      GameAxisKey gameKey2 = GenericGameKeyContext.Current.GameAxisKeys.Find((Predicate<GameAxisKey>) (g => g.Id.Equals("CameraAxisY")));
      this.RegisterGameAxisKey(gameKey1);
      this.RegisterGameAxisKey(gameKey2);
      GameAxisKey gameKey3 = GenericGameKeyContext.Current.GameAxisKeys.Find((Predicate<GameAxisKey>) (g => g.Id.Equals("MovementAxisX")));
      GameAxisKey gameKey4 = GenericGameKeyContext.Current.GameAxisKeys.Find((Predicate<GameAxisKey>) (g => g.Id.Equals("MovementAxisY")));
      this.RegisterGameAxisKey(gameKey3);
      this.RegisterGameAxisKey(gameKey4);
    }
  }
}
