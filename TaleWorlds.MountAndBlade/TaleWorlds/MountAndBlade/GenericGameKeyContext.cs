// Decompiled with JetBrains decompiler
// Type: TaleWorlds.MountAndBlade.GenericGameKeyContext
// Assembly: TaleWorlds.MountAndBlade, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3C0E3F42-756F-4407-B817-06BA412A78B0
// Assembly location: E:\SteamLibrary\steamapps\common\Mount & Blade II Bannerlord\bin\Win64_Shipping_Client\TaleWorlds.MountAndBlade.dll

using System.Collections.Generic;
using TaleWorlds.InputSystem;

namespace TaleWorlds.MountAndBlade
{
  public class GenericGameKeyContext
  {
    public const string CategoryId = "Generic";
    public const int Up = 0;
    public const int Down = 1;
    public const int Right = 3;
    public const int Left = 2;
    public const string MovementAxisX = "MovementAxisX";
    public const string MovementAxisY = "MovementAxisY";
    public const string CameraAxisX = "CameraAxisX";
    public const string CameraAxisY = "CameraAxisY";
    public const int Leave = 4;
    public const int ShowIndicators = 5;
    private static GenericGameKeyContext _current;

    public static GenericGameKeyContext Current => GenericGameKeyContext._current ?? (GenericGameKeyContext._current = new GenericGameKeyContext());

    public List<GameKey> GameKeys { get; }

    public List<GameAxisKey> GameAxisKeys { get; }

    private GenericGameKeyContext()
    {
      List<GameKey> gameKeyList = new List<GameKey>();
      List<GameAxisKey> gameAxisKeyList = new List<GameAxisKey>();
      GameKey positiveKey1 = new GameKey(0, nameof (Up), "Generic", InputKey.W, InputKey.ControllerLStickUp, GameKeyMainCategories.ActionCategory);
      GameKey negativeKey1 = new GameKey(1, nameof (Down), "Generic", InputKey.S, InputKey.ControllerLStickDown, GameKeyMainCategories.ActionCategory);
      GameKey negativeKey2 = new GameKey(2, nameof (Left), "Generic", InputKey.A, InputKey.ControllerLStickLeft, GameKeyMainCategories.ActionCategory);
      GameKey positiveKey2 = new GameKey(3, nameof (Right), "Generic", InputKey.D, InputKey.ControllerLStickRight, GameKeyMainCategories.ActionCategory);
      gameKeyList.Add(new GameKey(4, nameof (Leave), "Generic", InputKey.Tab, InputKey.ControllerRRight, GameKeyMainCategories.ActionCategory));
      gameKeyList.Add(new GameKey(5, nameof (ShowIndicators), "Generic", InputKey.LeftAlt, InputKey.ControllerLBumper, GameKeyMainCategories.ActionCategory));
      gameKeyList.Add(positiveKey1);
      gameKeyList.Add(negativeKey1);
      gameKeyList.Add(negativeKey2);
      gameKeyList.Add(positiveKey2);
      gameAxisKeyList.Add(new GameAxisKey(nameof (MovementAxisX), InputKey.ControllerLStick, positiveKey2, negativeKey2));
      gameAxisKeyList.Add(new GameAxisKey(nameof (MovementAxisY), InputKey.ControllerLStick, positiveKey1, negativeKey1, GameAxisKey.AxisType.Y));
      gameAxisKeyList.Add(new GameAxisKey(nameof (CameraAxisX), InputKey.ControllerRStick, (GameKey) null, (GameKey) null));
      gameAxisKeyList.Add(new GameAxisKey(nameof (CameraAxisY), InputKey.ControllerRStick, (GameKey) null, (GameKey) null, GameAxisKey.AxisType.Y));
      this.GameKeys = gameKeyList;
      this.GameAxisKeys = gameAxisKeyList;
    }
  }
}
