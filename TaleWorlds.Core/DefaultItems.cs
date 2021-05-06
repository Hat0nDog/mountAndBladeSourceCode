// Decompiled with JetBrains decompiler
// Type: TaleWorlds.Core.DefaultItems
// Assembly: TaleWorlds.Core, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 43F7B45C-9AE1-4715-A37B-2364840F62AF
// Assembly location: E:\SteamLibrary\steamapps\common\Mount & Blade II Bannerlord\bin\Win64_Shipping_Client\TaleWorlds.Core.dll

using System.Collections.Generic;
using TaleWorlds.Localization;
using TaleWorlds.ObjectSystem;

namespace TaleWorlds.Core
{
  public class DefaultItems
  {
    private readonly ItemObject[] _itemObjects;
    private const float TradeGoodWeight = 10f;
    private const float HalfWeight = 5f;
    private const float IngotWeight = 0.5f;
    private const float TrashWeight = 1f;
    private const int IngotValue = 20;
    private const int TrashValue = 1;

    internal static DefaultItems Instance => Game.Current.DefaultItems;

    public DefaultItems(Game game) => this._itemObjects = this.RegisterAll(game).ToArray();

    public static ItemObject Grain => DefaultItems.Instance.ItemGrain;

    public static ItemObject Meat => DefaultItems.Instance.ItemMeat;

    public static ItemObject Hides => DefaultItems.Instance.ItemHides;

    public static ItemObject Tools => DefaultItems.Instance.ItemTools;

    public static ItemObject IronOre => DefaultItems.Instance.ItemIronOre;

    public static ItemObject HardWood => DefaultItems.Instance.ItemHardwood;

    public static ItemObject Charcoal => DefaultItems.Instance.ItemCharcoal;

    public static ItemObject IronIngot1 => DefaultItems.Instance.ItemIronIngot1;

    public static ItemObject IronIngot2 => DefaultItems.Instance.ItemIronIngot2;

    public static ItemObject IronIngot3 => DefaultItems.Instance.ItemIronIngot3;

    public static ItemObject IronIngot4 => DefaultItems.Instance.ItemIronIngot4;

    public static ItemObject IronIngot5 => DefaultItems.Instance.ItemIronIngot5;

    public static ItemObject IronIngot6 => DefaultItems.Instance.ItemIronIngot6;

    public static ItemObject Trash => DefaultItems.Instance.ItemTrash;

    protected ItemObject ItemGrain { get; private set; }

    protected ItemObject ItemMeat { get; private set; }

    protected ItemObject ItemHides { get; private set; }

    protected ItemObject ItemTools { get; private set; }

    protected ItemObject ItemIronOre { get; private set; }

    protected ItemObject ItemHardwood { get; private set; }

    protected ItemObject ItemCharcoal { get; private set; }

    protected ItemObject ItemIronIngot1 { get; private set; }

    protected ItemObject ItemIronIngot2 { get; private set; }

    protected ItemObject ItemIronIngot3 { get; private set; }

    protected ItemObject ItemIronIngot4 { get; private set; }

    protected ItemObject ItemIronIngot5 { get; private set; }

    protected ItemObject ItemIronIngot6 { get; private set; }

    protected ItemObject ItemTrash { get; private set; }

    private List<ItemObject> RegisterAll(Game game)
    {
      List<ItemObject> items = new List<ItemObject>();
      this.ItemGrain = MBObjectManager.Instance.RegisterPresumedObject<ItemObject>(new ItemObject("grain"));
      this.ItemMeat = MBObjectManager.Instance.RegisterPresumedObject<ItemObject>(new ItemObject("meat"));
      this.ItemHides = MBObjectManager.Instance.RegisterPresumedObject<ItemObject>(new ItemObject("hides"));
      this.ItemTools = MBObjectManager.Instance.RegisterPresumedObject<ItemObject>(new ItemObject("tools"));
      this.ItemIronOre = MBObjectManager.Instance.RegisterPresumedObject<ItemObject>(new ItemObject("iron"));
      this.ItemHardwood = MBObjectManager.Instance.RegisterPresumedObject<ItemObject>(new ItemObject("hardwood"));
      this.ItemCharcoal = MBObjectManager.Instance.RegisterPresumedObject<ItemObject>(new ItemObject("charcoal"));
      this.ItemIronIngot1 = MBObjectManager.Instance.RegisterPresumedObject<ItemObject>(new ItemObject("ironIngot1"));
      this.ItemIronIngot2 = MBObjectManager.Instance.RegisterPresumedObject<ItemObject>(new ItemObject("ironIngot2"));
      this.ItemIronIngot3 = MBObjectManager.Instance.RegisterPresumedObject<ItemObject>(new ItemObject("ironIngot3"));
      this.ItemIronIngot4 = MBObjectManager.Instance.RegisterPresumedObject<ItemObject>(new ItemObject("ironIngot4"));
      this.ItemIronIngot5 = MBObjectManager.Instance.RegisterPresumedObject<ItemObject>(new ItemObject("ironIngot5"));
      this.ItemIronIngot6 = MBObjectManager.Instance.RegisterPresumedObject<ItemObject>(new ItemObject("ironIngot6"));
      this.ItemTrash = MBObjectManager.Instance.RegisterPresumedObject<ItemObject>(new ItemObject("trash"));
      this.InitializeItem(items, this.ItemGrain, new TextObject("{=Itv3fgJm}Grain{@Plural}loads of grain{\\@}"), "merchandise_grain", DefaultItemCategories.Grain, 10, 10f, ItemObject.ItemTypeEnum.Goods, true);
      this.InitializeItem(items, this.ItemMeat, new TextObject("{=LmwhFv5p}Meat{@Plural}loads of meat{\\@}"), "merchandise_meat", DefaultItemCategories.Meat, 30, 10f, ItemObject.ItemTypeEnum.Goods, true);
      this.InitializeItem(items, this.ItemHides, new TextObject("{=4kvKQuXM}Hides{@Plural}loads of hide{\\@}"), "merchandise_hides_b", DefaultItemCategories.Hides, 60, 10f, ItemObject.ItemTypeEnum.Goods);
      this.InitializeItem(items, this.ItemTools, new TextObject("{=n3cjEB0X}Tools{@Plural}loads of tools{\\@}"), "bd_pickaxe_b", DefaultItemCategories.Tools, 100, 10f, ItemObject.ItemTypeEnum.Goods);
      this.InitializeItem(items, this.ItemIronOre, new TextObject("{=Kw6BkhIf}Iron Ore{@Plural}loads of iron ore{\\@}"), "iron_ore", DefaultItemCategories.Iron, 50, 10f, ItemObject.ItemTypeEnum.Goods);
      this.InitializeItem(items, this.ItemHardwood, new TextObject("{=ExjMoUiT}Hardwood{@Plural}hardwood logs{\\@}"), "hardwood", DefaultItemCategories.Wood, 25, 10f, ItemObject.ItemTypeEnum.Goods);
      this.InitializeItem(items, this.ItemCharcoal, new TextObject("{=iQadPYNe}Charcoal{@Plural}loads of charcoal{\\@}"), "charcoal", DefaultItemCategories.Wood, 50, 5f, ItemObject.ItemTypeEnum.Goods);
      this.InitializeItem(items, this.ItemIronIngot1, new TextObject("{=gOpodlt1}Crude Iron{@Plural}loads of crude iron{\\@}"), "crude_iron", DefaultItemCategories.Iron, 20, 0.5f, ItemObject.ItemTypeEnum.Goods);
      this.InitializeItem(items, this.ItemIronIngot2, new TextObject("{=7HvtT8bm}Wrought Iron{@Plural}loads of wrought iron{\\@}"), "wrought_iron", DefaultItemCategories.Iron, 30, 0.5f, ItemObject.ItemTypeEnum.Goods);
      this.InitializeItem(items, this.ItemIronIngot3, new TextObject("{=8w0aaDbn}Iron{@Plural}loads of iron{\\@}"), "iron_a", DefaultItemCategories.Iron, 60, 0.5f, ItemObject.ItemTypeEnum.Goods);
      this.InitializeItem(items, this.ItemIronIngot4, new TextObject("{=UfuLKuaI}Steel{@Plural}loads of steel{\\@}"), "steel", DefaultItemCategories.Iron, 100, 0.5f, ItemObject.ItemTypeEnum.Goods);
      this.InitializeItem(items, this.ItemIronIngot5, new TextObject("{=azjMBa86}Fine Steel{@Plural}loads of fine steel{\\@}"), "fine_steel", DefaultItemCategories.Iron, 160, 0.5f, ItemObject.ItemTypeEnum.Goods);
      this.InitializeItem(items, this.ItemIronIngot6, new TextObject("{=vLVAfcta}Thamaskene Steel{@Plural}loads of thamaskene steel{\\@}"), "thamaskene_steel", DefaultItemCategories.Iron, 260, 0.5f, ItemObject.ItemTypeEnum.Goods);
      this.InitializeItem(items, this.ItemTrash, new TextObject("{=ZvZN6UkU}Trash Item"), "iron_ore", DefaultItemCategories.Tools, 1, 1f, ItemObject.ItemTypeEnum.Goods);
      return items;
    }

    private void InitializeItem(
      List<ItemObject> items,
      ItemObject item,
      TextObject name,
      string meshName,
      ItemCategory category,
      int value,
      float weight,
      ItemObject.ItemTypeEnum itemType,
      bool isFood = false)
    {
      ItemObject.InitializeTradeGood(item, name, meshName, category, value, weight, itemType, isFood);
      items.Add(item);
    }
  }
}
