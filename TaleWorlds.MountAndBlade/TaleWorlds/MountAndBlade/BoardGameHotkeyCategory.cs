// Decompiled with JetBrains decompiler
// Type: TaleWorlds.MountAndBlade.BoardGameHotkeyCategory
// Assembly: TaleWorlds.MountAndBlade, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3C0E3F42-756F-4407-B817-06BA412A78B0
// Assembly location: E:\SteamLibrary\steamapps\common\Mount & Blade II Bannerlord\bin\Win64_Shipping_Client\TaleWorlds.MountAndBlade.dll

using TaleWorlds.InputSystem;

namespace TaleWorlds.MountAndBlade
{
  public class BoardGameHotkeyCategory : GameKeyContext
  {
    public const string CategoryId = "BoardGameHotkeyCategory";
    public const string BoardGamePawnSelect = "BoardGamePawnSelect";
    public const string BoardGamePawnDeselect = "BoardGamePawnDeselect";
    public const string BoardGameDragPreview = "BoardGameDragPreview";

    public BoardGameHotkeyCategory()
      : base(nameof (BoardGameHotkeyCategory), 95)
    {
      this.RegisterHotKeys();
      this.RegisterGameKeys();
      this.RegisterGameAxisKeys();
    }

    private void RegisterHotKeys()
    {
      this.RegisterHotKey(new HotKey("BoardGamePawnSelect", nameof (BoardGameHotkeyCategory), InputKey.LeftMouseButton));
      this.RegisterHotKey(new HotKey("BoardGamePawnDeselect", nameof (BoardGameHotkeyCategory), InputKey.LeftMouseButton));
      this.RegisterHotKey(new HotKey("BoardGameDragPreview", nameof (BoardGameHotkeyCategory), InputKey.LeftMouseButton));
    }

    private void RegisterGameKeys()
    {
    }

    private void RegisterGameAxisKeys()
    {
    }
  }
}
