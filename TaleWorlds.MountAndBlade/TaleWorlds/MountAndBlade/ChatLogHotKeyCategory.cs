// Decompiled with JetBrains decompiler
// Type: TaleWorlds.MountAndBlade.ChatLogHotKeyCategory
// Assembly: TaleWorlds.MountAndBlade, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3C0E3F42-756F-4407-B817-06BA412A78B0
// Assembly location: E:\SteamLibrary\steamapps\common\Mount & Blade II Bannerlord\bin\Win64_Shipping_Client\TaleWorlds.MountAndBlade.dll

using System.Collections.Generic;
using TaleWorlds.InputSystem;

namespace TaleWorlds.MountAndBlade
{
  public class ChatLogHotKeyCategory : GenericPanelGameKeyCategory
  {
    public new const string CategoryId = "ChatLogHotKeyCategory";
    public const int InitiateAllChat = 6;
    public const int InitiateTeamChat = 7;
    public const int FinalizeChat = 8;
    public const string CycleChatTypes = "CycleChatTypes";
    public const string FinalizeChatAlternative = "FinalizeChatAlternative";

    public ChatLogHotKeyCategory()
      : base(nameof (ChatLogHotKeyCategory))
    {
      this.RegisterHotKeys();
      this.RegisterGameKeys();
      this.RegisterGameAxisKeys();
    }

    private void RegisterHotKeys()
    {
      List<Key> keys1 = new List<Key>()
      {
        new Key(InputKey.Tab)
      };
      List<Key> keys2 = new List<Key>()
      {
        new Key(InputKey.NumpadEnter)
      };
      this.RegisterHotKey(new HotKey("CycleChatTypes", nameof (ChatLogHotKeyCategory), keys1));
      this.RegisterHotKey(new HotKey("FinalizeChatAlternative", nameof (ChatLogHotKeyCategory), keys2));
    }

    private void RegisterGameKeys()
    {
      this.RegisterGameKey(new GameKey(6, "InitiateAllChat", nameof (ChatLogHotKeyCategory), InputKey.T, GameKeyMainCategories.ChatCategory));
      this.RegisterGameKey(new GameKey(7, "InitiateTeamChat", nameof (ChatLogHotKeyCategory), InputKey.Y, GameKeyMainCategories.ChatCategory));
      this.RegisterGameKey(new GameKey(8, "FinalizeChat", nameof (ChatLogHotKeyCategory), InputKey.Enter, GameKeyMainCategories.ChatCategory));
    }

    private void RegisterGameAxisKeys()
    {
    }
  }
}
