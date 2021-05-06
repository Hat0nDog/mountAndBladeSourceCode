// Decompiled with JetBrains decompiler
// Type: TaleWorlds.Core.BasicCultureObject
// Assembly: TaleWorlds.Core, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 43F7B45C-9AE1-4715-A37B-2364840F62AF
// Assembly location: E:\SteamLibrary\steamapps\common\Mount & Blade II Bannerlord\bin\Win64_Shipping_Client\TaleWorlds.Core.dll

using System;
using System.Xml;
using TaleWorlds.Localization;
using TaleWorlds.ObjectSystem;

namespace TaleWorlds.Core
{
  public class BasicCultureObject : MBObjectBase
  {
    public TextObject Name { get; private set; }

    public bool IsMainCulture { get; private set; }

    public bool IsBandit { get; private set; }

    public bool CanHaveSettlement { get; private set; }

    public uint Color { get; private set; }

    public uint Color2 { get; private set; }

    public uint ClothAlternativeColor { get; private set; }

    public uint ClothAlternativeColor2 { get; private set; }

    public uint BackgroundColor1 { get; private set; }

    public uint ForegroundColor1 { get; private set; }

    public uint BackgroundColor2 { get; private set; }

    public uint ForegroundColor2 { get; private set; }

    public string EncounterBackgroundMesh { get; set; }

    public string BannerKey { get; set; }

    public override string ToString() => this.Name.ToString();

    public override void Deserialize(MBObjectManager objectManager, XmlNode node)
    {
      base.Deserialize(objectManager, node);
      this.Name = new TextObject(node.Attributes["name"].Value);
      this.Color = node.Attributes["color"] == null ? uint.MaxValue : Convert.ToUInt32(node.Attributes["color"].Value, 16);
      this.Color2 = node.Attributes["color2"] == null ? uint.MaxValue : Convert.ToUInt32(node.Attributes["color2"].Value, 16);
      this.ClothAlternativeColor = node.Attributes["cloth_alternative_color1"] == null ? uint.MaxValue : Convert.ToUInt32(node.Attributes["cloth_alternative_color1"].Value, 16);
      this.ClothAlternativeColor2 = node.Attributes["cloth_alternative_color2"] == null ? uint.MaxValue : Convert.ToUInt32(node.Attributes["cloth_alternative_color2"].Value, 16);
      this.BackgroundColor1 = node.Attributes["banner_background_color1"] == null ? uint.MaxValue : Convert.ToUInt32(node.Attributes["banner_background_color1"].Value, 16);
      this.ForegroundColor1 = node.Attributes["banner_foreground_color1"] == null ? uint.MaxValue : Convert.ToUInt32(node.Attributes["banner_foreground_color1"].Value, 16);
      this.BackgroundColor2 = node.Attributes["banner_background_color2"] == null ? uint.MaxValue : Convert.ToUInt32(node.Attributes["banner_background_color2"].Value, 16);
      this.ForegroundColor2 = node.Attributes["banner_foreground_color2"] == null ? uint.MaxValue : Convert.ToUInt32(node.Attributes["banner_foreground_color2"].Value, 16);
      this.IsMainCulture = node.Attributes["is_main_culture"] != null && Convert.ToBoolean(node.Attributes["is_main_culture"].Value);
      this.EncounterBackgroundMesh = node.Attributes["encounter_background_mesh"] == null ? (string) null : node.Attributes["encounter_background_mesh"].Value;
      this.BannerKey = node.Attributes["faction_banner_key"] == null ? (string) null : node.Attributes["faction_banner_key"].Value;
      this.IsBandit = false;
      this.IsBandit = node.Attributes["is_bandit"] != null && Convert.ToBoolean(node.Attributes["is_bandit"].Value);
      this.CanHaveSettlement = false;
      this.CanHaveSettlement = node.Attributes["can_have_settlement"] != null && Convert.ToBoolean(node.Attributes["can_have_settlement"].Value);
    }

    public CultureCode GetCultureCode()
    {
      CultureCode result;
      return Enum.TryParse<CultureCode>(this.StringId, true, out result) ? result : CultureCode.Invalid;
    }
  }
}
