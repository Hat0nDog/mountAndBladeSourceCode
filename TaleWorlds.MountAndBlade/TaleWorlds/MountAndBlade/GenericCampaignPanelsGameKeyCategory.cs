// Decompiled with JetBrains decompiler
// Type: TaleWorlds.MountAndBlade.GenericCampaignPanelsGameKeyCategory
// Assembly: TaleWorlds.MountAndBlade, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3C0E3F42-756F-4407-B817-06BA412A78B0
// Assembly location: E:\SteamLibrary\steamapps\common\Mount & Blade II Bannerlord\bin\Win64_Shipping_Client\TaleWorlds.MountAndBlade.dll

using TaleWorlds.InputSystem;

namespace TaleWorlds.MountAndBlade
{
  public class GenericCampaignPanelsGameKeyCategory : GenericPanelGameKeyCategory
  {
    public new const string CategoryId = "GenericCampaignPanelsGameKeyCategory";
    public const string FiveStackModifier = "FiveStackModifier";
    public const string FiveStackModifierAlt = "FiveStackModifierAlt";
    public const string EntireStackModifier = "EntireStackModifier";
    public const string EntireStackModifierAlt = "EntireStackModifierAlt";
    public const int BannerWindow = 33;
    public const int CharacterWindow = 34;
    public const int InventoryWindow = 35;
    public const int EncyclopediaWindow = 36;
    public const int PartyWindow = 40;
    public const int KingdomWindow = 37;
    public const int ClanWindow = 38;
    public const int QuestsWindow = 39;
    public const int FacegenWindow = 41;

    public GenericCampaignPanelsGameKeyCategory(string categoryId = "GenericCampaignPanelsGameKeyCategory")
      : base(categoryId)
    {
      this.RegisterHotKeys();
      this.RegisterGameKeys();
      this.RegisterGameAxisKeys();
    }

    private void RegisterHotKeys()
    {
      this.RegisterHotKey(new HotKey("FiveStackModifier", nameof (GenericCampaignPanelsGameKeyCategory), InputKey.LeftShift));
      this.RegisterHotKey(new HotKey("FiveStackModifierAlt", nameof (GenericCampaignPanelsGameKeyCategory), InputKey.RightShift));
      this.RegisterHotKey(new HotKey("EntireStackModifier", nameof (GenericCampaignPanelsGameKeyCategory), InputKey.LeftControl));
      this.RegisterHotKey(new HotKey("EntireStackModifierAlt", nameof (GenericCampaignPanelsGameKeyCategory), InputKey.RightControl));
    }

    private void RegisterGameKeys()
    {
      this.RegisterGameKey(new GameKey(33, "BannerWindow", nameof (GenericCampaignPanelsGameKeyCategory), InputKey.B, GameKeyMainCategories.MenuShortcutCategory));
      this.RegisterGameKey(new GameKey(34, "CharacterWindow", nameof (GenericCampaignPanelsGameKeyCategory), InputKey.C, GameKeyMainCategories.MenuShortcutCategory));
      this.RegisterGameKey(new GameKey(35, "InventoryWindow", nameof (GenericCampaignPanelsGameKeyCategory), InputKey.I, GameKeyMainCategories.MenuShortcutCategory));
      this.RegisterGameKey(new GameKey(36, "EncyclopediaWindow", nameof (GenericCampaignPanelsGameKeyCategory), InputKey.N, InputKey.ControllerLOption, GameKeyMainCategories.MenuShortcutCategory));
      this.RegisterGameKey(new GameKey(37, "KingdomWindow", nameof (GenericCampaignPanelsGameKeyCategory), InputKey.K, GameKeyMainCategories.MenuShortcutCategory));
      this.RegisterGameKey(new GameKey(38, "ClanWindow", nameof (GenericCampaignPanelsGameKeyCategory), InputKey.L, GameKeyMainCategories.MenuShortcutCategory));
      this.RegisterGameKey(new GameKey(39, "QuestsWindow", nameof (GenericCampaignPanelsGameKeyCategory), InputKey.J, GameKeyMainCategories.MenuShortcutCategory));
      this.RegisterGameKey(new GameKey(40, "PartyWindow", nameof (GenericCampaignPanelsGameKeyCategory), InputKey.P, GameKeyMainCategories.MenuShortcutCategory));
      this.RegisterGameKey(new GameKey(41, "FacegenWindow", nameof (GenericCampaignPanelsGameKeyCategory), InputKey.V, GameKeyMainCategories.MenuShortcutCategory));
    }

    private void RegisterGameAxisKeys()
    {
    }
  }
}
