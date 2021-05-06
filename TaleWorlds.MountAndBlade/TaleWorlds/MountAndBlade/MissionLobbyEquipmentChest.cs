// Decompiled with JetBrains decompiler
// Type: TaleWorlds.MountAndBlade.MissionLobbyEquipmentChest
// Assembly: TaleWorlds.MountAndBlade, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3C0E3F42-756F-4407-B817-06BA412A78B0
// Assembly location: E:\SteamLibrary\steamapps\common\Mount & Blade II Bannerlord\bin\Win64_Shipping_Client\TaleWorlds.MountAndBlade.dll

using System.Collections;
using System.Collections.Generic;
using TaleWorlds.Core;
using TaleWorlds.MountAndBlade.Diamond;

namespace TaleWorlds.MountAndBlade
{
  public class MissionLobbyEquipmentChest : IEnumerable<KeyValuePair<int, ItemObject>>, IEnumerable
  {
    private Dictionary<int, ItemObject> _items;

    public MissionLobbyEquipmentChest(InventoryData chest)
    {
      this._items = new Dictionary<int, ItemObject>();
      foreach (ItemData itemData in chest.Items)
        this.AddItem(itemData.GetItemObject());
    }

    public ItemObject GetItem(int itemIndex) => this._items[itemIndex];

    public int GetItemIndex(ItemObject item)
    {
      foreach (KeyValuePair<int, ItemObject> keyValuePair in this._items)
      {
        if (keyValuePair.Value == item)
          return keyValuePair.Key;
      }
      return -1;
    }

    public int AddItem(ItemObject itemObject)
    {
      int avaliableItemNo = this.FindAvaliableItemNo();
      this._items.Add(avaliableItemNo, itemObject);
      return avaliableItemNo;
    }

    private int FindAvaliableItemNo()
    {
      int key = 0;
      while (this._items.ContainsKey(key))
        ++key;
      return key;
    }

    public IEnumerator<KeyValuePair<int, ItemObject>> GetEnumerator()
    {
      foreach (KeyValuePair<int, ItemObject> keyValuePair in this._items)
        yield return keyValuePair;
    }

    IEnumerator IEnumerable.GetEnumerator() => (IEnumerator) this.GetEnumerator();
  }
}
