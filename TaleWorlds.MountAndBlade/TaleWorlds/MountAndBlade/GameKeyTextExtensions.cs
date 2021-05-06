// Decompiled with JetBrains decompiler
// Type: TaleWorlds.MountAndBlade.GameKeyTextExtensions
// Assembly: TaleWorlds.MountAndBlade, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3C0E3F42-756F-4407-B817-06BA412A78B0
// Assembly location: E:\SteamLibrary\steamapps\common\Mount & Blade II Bannerlord\bin\Win64_Shipping_Client\TaleWorlds.MountAndBlade.dll

using TaleWorlds.Core;
using TaleWorlds.InputSystem;
using TaleWorlds.Localization;

namespace TaleWorlds.MountAndBlade
{
  public static class GameKeyTextExtensions
  {
    public static TextObject GetHotKeyGameText(
      this GameTextManager gameTextManager,
      string categoryName,
      string hotKeyId)
    {
      return gameTextManager.FindText("str_game_key_text", HotKeyManager.GetHotKeyId(categoryName, hotKeyId).ToLower());
    }

    public static TextObject GetHotKeyGameText(
      this GameTextManager gameTextManager,
      string categoryName,
      int gameKeyId)
    {
      return gameTextManager.FindText("str_game_key_text", HotKeyManager.GetHotKeyId(categoryName, gameKeyId).ToLower());
    }
  }
}
