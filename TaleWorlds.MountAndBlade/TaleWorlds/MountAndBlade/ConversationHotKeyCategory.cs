// Decompiled with JetBrains decompiler
// Type: TaleWorlds.MountAndBlade.ConversationHotKeyCategory
// Assembly: TaleWorlds.MountAndBlade, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3C0E3F42-756F-4407-B817-06BA412A78B0
// Assembly location: E:\SteamLibrary\steamapps\common\Mount & Blade II Bannerlord\bin\Win64_Shipping_Client\TaleWorlds.MountAndBlade.dll

using System.Collections.Generic;
using TaleWorlds.InputSystem;

namespace TaleWorlds.MountAndBlade
{
  public class ConversationHotKeyCategory : GenericPanelGameKeyCategory
  {
    public new const string CategoryId = "ConversationHotKeyCategory";
    public const string ContinueKey = "ContinueKey";
    public const string ContinueClick = "ContinueClick";

    public ConversationHotKeyCategory()
      : base(nameof (ConversationHotKeyCategory))
    {
      this.RegisterHotKeys();
      this.RegisterGameKeys();
      this.RegisterGameAxisKeys();
    }

    private void RegisterHotKeys()
    {
      this.RegisterHotKey(new HotKey("ContinueKey", nameof (ConversationHotKeyCategory), new List<Key>()
      {
        new Key(InputKey.Space),
        new Key(InputKey.Enter),
        new Key(InputKey.NumpadEnter),
        new Key(InputKey.ControllerRDown)
      }));
      this.RegisterHotKey(new HotKey("ContinueClick", nameof (ConversationHotKeyCategory), InputKey.LeftMouseButton));
    }

    private void RegisterGameKeys()
    {
    }

    private void RegisterGameAxisKeys()
    {
    }
  }
}
