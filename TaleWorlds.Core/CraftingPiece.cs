﻿// Decompiled with JetBrains decompiler
// Type: TaleWorlds.Core.CraftingPiece
// Assembly: TaleWorlds.Core, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 43F7B45C-9AE1-4715-A37B-2364840F62AF
// Assembly location: E:\SteamLibrary\steamapps\common\Mount & Blade II Bannerlord\bin\Win64_Shipping_Client\TaleWorlds.Core.dll

using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml;
using TaleWorlds.Library;
using TaleWorlds.Localization;
using TaleWorlds.ObjectSystem;
using TaleWorlds.SaveSystem;

namespace TaleWorlds.Core
{
  [SaveableClass(10039)]
  public sealed class CraftingPiece : MBObjectBase
  {
    private static CraftingPiece[] _invalidCraftingPiece;
    public WeaponFlags AdditionalWeaponFlags;
    public ItemFlags AdditionalItemFlags;
    private int[] _materialCosts;
    private (CraftingMaterials, int)[] _materialsUsed;
    private List<string> _unavailableUsages;

    internal static void AutoGeneratedStaticCollectObjectsCraftingPiece(
      object o,
      List<object> collectedObjects)
    {
      ((MBObjectBase) o).AutoGeneratedInstanceCollectObjects(collectedObjects);
    }

    protected override void AutoGeneratedInstanceCollectObjects(List<object> collectedObjects) => base.AutoGeneratedInstanceCollectObjects(collectedObjects);

    public CraftingPiece() => this.InitializeLists();

    [LoadInitializationCallback]
    private void OnLoad(MetaData metaData) => this.InitializeLists();

    private void InitializeLists()
    {
      this._unavailableUsages = new List<string>();
      this._materialCosts = new int[9];
      this._materialsUsed = new (CraftingMaterials, int)[0];
      this.MaterialCosts = (IReadOnlyList<int>) Array.AsReadOnly<int>(this._materialCosts);
      this.UnavailableUsages = (IReadOnlyCollection<string>) this._unavailableUsages.AsReadOnly();
    }

    public static CraftingPiece GetInvalidCraftingPiece(
      CraftingPiece.PieceTypes pieceType)
    {
      if (CraftingPiece._invalidCraftingPiece == null)
        CraftingPiece._invalidCraftingPiece = new CraftingPiece[4];
      if (CraftingPiece._invalidCraftingPiece[(int) pieceType] == null)
        CraftingPiece._invalidCraftingPiece[(int) pieceType] = new CraftingPiece()
        {
          PieceType = pieceType,
          Name = new TextObject("{=!}Invalid"),
          IsValid = false
        };
      return CraftingPiece._invalidCraftingPiece[(int) pieceType];
    }

    public bool IsValid { get; private set; }

    public TextObject Name { get; private set; }

    public CraftingPiece.PieceTypes PieceType { get; private set; }

    public string MeshName { get; private set; }

    public BasicCultureObject Culture { get; private set; }

    public float Length { get; private set; }

    public float DistanceToNextPiece { get; private set; }

    public float DistanceToPreviousPiece { get; private set; }

    public float PieceOffset { get; private set; }

    public float PreviousPieceOffset { get; private set; }

    public float NextPieceOffset { get; private set; }

    public float Weight { get; private set; }

    public float Inertia { get; private set; }

    public float CenterOfMass { get; private set; }

    public int ArmorBonus { get; private set; }

    public int SwingDamageBonus { get; private set; }

    public int SwingSpeedBonus { get; private set; }

    public int ThrustDamageBonus { get; private set; }

    public int ThrustSpeedBonus { get; private set; }

    public int HandlingBonus { get; private set; }

    public int AccuracyBonus { get; private set; }

    public int PieceTier { get; private set; }

    public bool FullScale { get; private set; }

    public Vec3 ItemHolsterPosShift { get; private set; }

    public float Appearance { get; private set; }

    public bool IsGivenByDefault { get; private set; }

    public bool IsHiddenOnDesigner { get; private set; }

    public bool IsUnique { get; private set; }

    public string ItemUsageFeaturesToExclude { get; private set; }

    public IReadOnlyList<int> MaterialCosts { get; private set; }

    public IReadOnlyList<(CraftingMaterials, int)> MaterialsUsed => (IReadOnlyList<(CraftingMaterials, int)>) this._materialsUsed;

    public int CraftingCost { get; private set; }

    public int RequiredSkillValue { get; private set; }

    public IReadOnlyCollection<string> UnavailableUsages { get; private set; }

    public BladeData BladeData { get; private set; }

    public override void Deserialize(MBObjectManager objectManager, XmlNode node)
    {
      base.Deserialize(objectManager, node);
      this.IsValid = true;
      if (this._unavailableUsages.Any<string>())
        this._unavailableUsages.Clear();
      this.Name = new TextObject(node.Attributes["name"].InnerText);
      this.PieceType = (CraftingPiece.PieceTypes) Enum.Parse(typeof (CraftingPiece.PieceTypes), node.Attributes["piece_type"].InnerText, true);
      this.MeshName = node.Attributes["mesh"].InnerText;
      this.Culture = node.Attributes["mesh"] != null ? (BasicCultureObject) objectManager.ReadObjectReferenceFromXml("culture", typeof (BasicCultureObject), node) : (BasicCultureObject) null;
      this.Appearance = node.Attributes["appearance"] != null ? float.Parse(node.Attributes["appearance"].Value) : 0.5f;
      this.CraftingCost = node.Attributes["CraftingCost"] != null ? int.Parse(node.Attributes["CraftingCost"].Value) : 0;
      XmlAttribute attribute1 = node.Attributes["weight"];
      this.Weight = attribute1 != null ? float.Parse(attribute1.Value) : 0.0f;
      XmlAttribute attribute2 = node.Attributes["length"];
      if (attribute2 != null)
      {
        this.Length = 0.01f * float.Parse(attribute2.Value);
        this.DistanceToNextPiece = this.Length / 2f;
        this.DistanceToPreviousPiece = this.Length / 2f;
      }
      else
      {
        XmlAttribute attribute3 = node.Attributes["distance_to_next_piece"];
        XmlAttribute attribute4 = node.Attributes["distance_to_previous_piece"];
        this.DistanceToNextPiece = 0.01f * float.Parse(attribute3.Value);
        this.DistanceToPreviousPiece = 0.01f * float.Parse(attribute4.Value);
        this.Length = this.DistanceToNextPiece + this.DistanceToPreviousPiece;
      }
      this.Inertia = 0.08333334f * this.Weight * this.Length * this.Length;
      XmlAttribute attribute5 = node.Attributes["center_of_mass"];
      this.CenterOfMass = this.Length * (attribute5 != null ? float.Parse(attribute5.Value) : 0.5f);
      XmlAttribute attribute6 = node.Attributes["item_holster_pos_shift"];
      Vec3 vec3 = new Vec3();
      if (attribute6 != null)
      {
        string[] strArray = attribute6.Value.Split(',');
        if (strArray.Length == 3)
        {
          float.TryParse(strArray[0], out vec3.x);
          float.TryParse(strArray[1], out vec3.y);
          float.TryParse(strArray[2], out vec3.z);
        }
      }
      this.ItemHolsterPosShift = vec3;
      XmlAttribute attribute7 = node.Attributes["tier"];
      this.PieceTier = attribute7 != null ? int.Parse(attribute7.Value) : 1;
      this.IsUnique = XmlHelper.ReadBool(node, "is_unique");
      this.IsGivenByDefault = XmlHelper.ReadBool(node, "is_default");
      this.IsHiddenOnDesigner = XmlHelper.ReadBool(node, "is_hidden");
      XmlAttribute attribute8 = node.Attributes["full_scale"];
      this.FullScale = attribute8 != null ? attribute8.InnerText == "true" : this.PieceType == CraftingPiece.PieceTypes.Guard || this.PieceType == CraftingPiece.PieceTypes.Pommel;
      XmlAttribute attribute9 = node.Attributes["excluded_item_usage_features"];
      this.ItemUsageFeaturesToExclude = attribute9 != null ? attribute9.InnerText : "";
      XmlAttribute attribute10 = node.Attributes["required_skill_value"];
      this.RequiredSkillValue = attribute10 != null ? int.Parse(attribute10.Value) : 0;
      foreach (XmlNode childNode1 in node.ChildNodes)
      {
        if (childNode1.Attributes != null)
        {
          string name = childNode1.Name;
          if (!(name == "PieceUsages"))
          {
            if (!(name == "StatContributions"))
            {
              if (!(name == "BladeData"))
              {
                if (!(name == "BuildData"))
                {
                  if (!(name == "Materials"))
                  {
                    if (name == "Flags")
                    {
                      this.AdditionalItemFlags = (ItemFlags) 0;
                      this.AdditionalWeaponFlags = (WeaponFlags) 0;
                      foreach (XmlNode childNode2 in childNode1.ChildNodes)
                      {
                        XmlAttribute attribute3 = childNode2.Attributes["name"];
                        XmlAttribute attribute4 = childNode2.Attributes["type"];
                        if (attribute4 == null || attribute4.Value == "WeaponFlags")
                          this.AdditionalWeaponFlags |= (WeaponFlags) Enum.Parse(typeof (WeaponFlags), attribute3.Value, true);
                        else
                          this.AdditionalItemFlags |= (ItemFlags) Enum.Parse(typeof (ItemFlags), attribute3.Value, true);
                      }
                    }
                  }
                  else
                  {
                    List<(CraftingMaterials, int)> valueTupleList = new List<(CraftingMaterials, int)>();
                    foreach (XmlNode childNode2 in childNode1.ChildNodes)
                    {
                      string str = childNode2.Attributes["id"].Value;
                      string s = childNode2.Attributes["count"].Value;
                      CraftingMaterials result;
                      Enum.TryParse<CraftingMaterials>(str, out result);
                      int num;
                      ref int local = ref num;
                      if (int.TryParse(s, out local) && num > 0)
                        valueTupleList.Add((result, num));
                      this._materialCosts[(int) result] = num;
                    }
                    this._materialsUsed = valueTupleList.ToArray();
                  }
                }
                else
                {
                  XmlAttribute attribute3 = childNode1.Attributes["piece_offset"];
                  XmlAttribute attribute4 = childNode1.Attributes["previous_piece_offset"];
                  XmlAttribute attribute11 = childNode1.Attributes["next_piece_offset"];
                  this.PieceOffset = attribute3 != null ? 0.01f * float.Parse(attribute3.Value) : 0.0f;
                  this.PreviousPieceOffset = attribute4 != null ? 0.01f * float.Parse(attribute4.Value) : 0.0f;
                  this.NextPieceOffset = attribute11 != null ? 0.01f * float.Parse(attribute11.Value) : 0.0f;
                }
              }
              else
              {
                this.BladeData = new BladeData(this.PieceType, this.Length);
                this.BladeData.Deserialize(objectManager, childNode1);
              }
            }
            else
            {
              XmlAttribute attribute3 = childNode1.Attributes["armor_bonus"];
              this.ArmorBonus = attribute3 != null ? int.Parse(attribute3.Value) : 0;
              XmlAttribute attribute4 = childNode1.Attributes["handling_bonus"];
              this.HandlingBonus = attribute4 != null ? int.Parse(attribute4.Value) : 0;
              XmlAttribute attribute11 = childNode1.Attributes["swing_damage_bonus"];
              this.SwingDamageBonus = attribute11 != null ? int.Parse(attribute11.Value) : 0;
              XmlAttribute attribute12 = childNode1.Attributes["swing_speed_bonus"];
              this.SwingSpeedBonus = attribute12 != null ? int.Parse(attribute12.Value) : 0;
              XmlAttribute attribute13 = childNode1.Attributes["thrust_damage_bonus"];
              this.ThrustDamageBonus = attribute13 != null ? int.Parse(attribute13.Value) : 0;
              XmlAttribute attribute14 = childNode1.Attributes["thrust_speed_bonus"];
              this.ThrustSpeedBonus = attribute14 != null ? int.Parse(attribute14.Value) : 0;
              XmlAttribute attribute15 = childNode1.Attributes["accuracy_bonus"];
              this.AccuracyBonus = attribute15 != null ? int.Parse(attribute15.Value) : 0;
            }
          }
          else
          {
            string innerText = childNode1.Attributes["unavailable_usages"].InnerText;
            char[] chArray = new char[1]{ ':' };
            foreach (string str in innerText.Split(chArray))
              this._unavailableUsages.Add(str);
          }
        }
      }
      int pieceType = (int) this.PieceType;
    }

    public static MBReadOnlyList<CraftingPiece> All => Game.Current.ObjectManager.GetObjectTypeList<CraftingPiece>();

    public enum PieceTypes
    {
      Invalid = -1, // 0xFFFFFFFF
      Blade = 0,
      Guard = 1,
      Handle = 2,
      Pommel = 3,
      NumberOfPieceTypes = 4,
    }
  }
}