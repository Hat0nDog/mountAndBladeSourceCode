// Decompiled with JetBrains decompiler
// Type: TaleWorlds.MountAndBlade.Network.Gameplay.Perks.Effects.RandomEquipmentEffect
// Assembly: TaleWorlds.MountAndBlade, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3C0E3F42-756F-4407-B817-06BA412A78B0
// Assembly location: E:\SteamLibrary\steamapps\common\Mount & Blade II Bannerlord\bin\Win64_Shipping_Client\TaleWorlds.MountAndBlade.dll

using System.Collections.Generic;
using System.Xml;
using TaleWorlds.Core;
using TaleWorlds.ObjectSystem;

namespace TaleWorlds.MountAndBlade.Network.Gameplay.Perks.Effects
{
  public class RandomEquipmentEffect : MPRandomOnSpawnPerkEffect
  {
    protected static string StringType = "RandomEquipmentOnSpawn";
    private List<List<(EquipmentIndex, EquipmentElement)>> _groups;

    protected RandomEquipmentEffect()
    {
    }

    protected override void Deserialize(XmlNode node)
    {
      base.Deserialize(node);
      this._groups = new List<List<(EquipmentIndex, EquipmentElement)>>();
      foreach (XmlNode childNode1 in node.ChildNodes)
      {
        if (childNode1.NodeType != XmlNodeType.Comment && childNode1.NodeType != XmlNodeType.SignificantWhitespace && childNode1.Name == "Group")
        {
          List<(EquipmentIndex, EquipmentElement)> valueTupleList = new List<(EquipmentIndex, EquipmentElement)>();
          foreach (XmlNode childNode2 in childNode1.ChildNodes)
          {
            if (childNode2.NodeType != XmlNodeType.Comment && childNode2.NodeType != XmlNodeType.SignificantWhitespace)
            {
              EquipmentElement equipmentElement = new EquipmentElement();
              EquipmentIndex equipmentIndex = EquipmentIndex.None;
              XmlAttribute attribute1 = childNode2.Attributes?["item"];
              if (attribute1 != null)
                equipmentElement = new EquipmentElement(MBObjectManager.Instance.GetObject<ItemObject>(attribute1.Value));
              XmlAttribute attribute2 = childNode2.Attributes?["slot"];
              if (attribute2 != null)
                equipmentIndex = Equipment.GetEquipmentIndexFromOldEquipmentIndexName(attribute2.Value);
              valueTupleList.Add((equipmentIndex, equipmentElement));
            }
          }
          if (valueTupleList.Count > 0)
            this._groups.Add(valueTupleList);
        }
      }
    }

    public override List<(EquipmentIndex, EquipmentElement)> GetAlternativeEquipments(
      bool isPlayer,
      List<(EquipmentIndex, EquipmentElement)> alternativeEquipments)
    {
      if (this.EffectTarget == MPOnSpawnPerkEffectBase.Target.Any || (isPlayer ? (this.EffectTarget == MPOnSpawnPerkEffectBase.Target.Player ? 1 : 0) : (this.EffectTarget == MPOnSpawnPerkEffectBase.Target.Troops ? 1 : 0)) != 0)
      {
        if (alternativeEquipments == null)
          alternativeEquipments = new List<(EquipmentIndex, EquipmentElement)>((IEnumerable<(EquipmentIndex, EquipmentElement)>) this._groups.GetRandomElement<List<(EquipmentIndex, EquipmentElement)>>());
        else
          alternativeEquipments.AddRange((IEnumerable<(EquipmentIndex, EquipmentElement)>) this._groups.GetRandomElement<List<(EquipmentIndex, EquipmentElement)>>());
      }
      return alternativeEquipments;
    }
  }
}
