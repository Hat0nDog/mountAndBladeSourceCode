// Decompiled with JetBrains decompiler
// Type: TaleWorlds.MountAndBlade.MultiplayerHotkeyCategory
// Assembly: TaleWorlds.MountAndBlade, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3C0E3F42-756F-4407-B817-06BA412A78B0
// Assembly location: E:\SteamLibrary\steamapps\common\Mount & Blade II Bannerlord\bin\Win64_Shipping_Client\TaleWorlds.MountAndBlade.dll

using TaleWorlds.InputSystem;

namespace TaleWorlds.MountAndBlade
{
  public class MultiplayerHotkeyCategory : GameKeyContext
  {
    public const string CategoryId = "MultiplayerHotkeyCategory";
    private const string _storeCameraPositionBase = "StoreCameraPosition";
    public const string StoreCameraPosition1 = "StoreCameraPosition1";
    public const string StoreCameraPosition2 = "StoreCameraPosition2";
    public const string StoreCameraPosition3 = "StoreCameraPosition3";
    public const string StoreCameraPosition4 = "StoreCameraPosition4";
    public const string StoreCameraPosition5 = "StoreCameraPosition5";
    public const string StoreCameraPosition6 = "StoreCameraPosition6";
    public const string StoreCameraPosition7 = "StoreCameraPosition7";
    public const string StoreCameraPosition8 = "StoreCameraPosition8";
    public const string StoreCameraPosition9 = "StoreCameraPosition9";
    private const string _spectateCameraPositionBase = "SpectateCameraPosition";
    public const string SpectateCameraPosition1 = "SpectateCameraPosition1";
    public const string SpectateCameraPosition2 = "SpectateCameraPosition2";
    public const string SpectateCameraPosition3 = "SpectateCameraPosition3";
    public const string SpectateCameraPosition4 = "SpectateCameraPosition4";
    public const string SpectateCameraPosition5 = "SpectateCameraPosition5";
    public const string SpectateCameraPosition6 = "SpectateCameraPosition6";
    public const string SpectateCameraPosition7 = "SpectateCameraPosition7";
    public const string SpectateCameraPosition8 = "SpectateCameraPosition8";
    public const string SpectateCameraPosition9 = "SpectateCameraPosition9";

    public MultiplayerHotkeyCategory()
      : base(nameof (MultiplayerHotkeyCategory), 95)
    {
      this.RegisterHotKeys();
      this.RegisterGameKeys();
      this.RegisterGameAxisKeys();
    }

    private void RegisterHotKeys()
    {
      for (int index = 1; index <= 9; ++index)
        this.RegisterHotKey(new HotKey("StoreCameraPosition" + index.ToString(), nameof (MultiplayerHotkeyCategory), (InputKey) (11 + index)));
      for (int index = 1; index <= 9; ++index)
        this.RegisterHotKey(new HotKey("SpectateCameraPosition" + index.ToString(), nameof (MultiplayerHotkeyCategory), (InputKey) (11 + index)));
    }

    private void RegisterGameKeys()
    {
    }

    private void RegisterGameAxisKeys()
    {
    }
  }
}
