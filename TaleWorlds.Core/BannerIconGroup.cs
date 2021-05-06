// Decompiled with JetBrains decompiler
// Type: TaleWorlds.Core.BannerIconGroup
// Assembly: TaleWorlds.Core, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 43F7B45C-9AE1-4715-A37B-2364840F62AF
// Assembly location: E:\SteamLibrary\steamapps\common\Mount & Blade II Bannerlord\bin\Win64_Shipping_Client\TaleWorlds.Core.dll

using System;
using System.Collections.Generic;
using System.Xml;
using TaleWorlds.Library;
using TaleWorlds.Localization;

namespace TaleWorlds.Core
{
  public class BannerIconGroup
  {
    public MBReadOnlyDictionary<int, BannerIconData> AllIcons;
    public MBReadOnlyDictionary<int, string> AllBackgrounds;
    public MBReadOnlyDictionary<int, BannerIconData> AvailableIcons;
    private Dictionary<int, BannerIconData> _allIcons;
    private Dictionary<int, string> _allBackgrounds;
    private Dictionary<int, BannerIconData> _availableIcons;

    public TextObject Name { get; private set; }

    public bool IsPattern { get; private set; }

    public int Id { get; private set; }

    internal BannerIconGroup()
    {
    }

    public void Deserialize(XmlNode xmlNode)
    {
      this._allIcons = new Dictionary<int, BannerIconData>();
      this._availableIcons = new Dictionary<int, BannerIconData>();
      this._allBackgrounds = new Dictionary<int, string>();
      this.AllIcons = new MBReadOnlyDictionary<int, BannerIconData>(this._allIcons);
      this.AvailableIcons = new MBReadOnlyDictionary<int, BannerIconData>(this._availableIcons);
      this.AllBackgrounds = new MBReadOnlyDictionary<int, string>(this._allBackgrounds);
      this.Id = Convert.ToInt32(xmlNode.Attributes["id"].Value);
      this.Name = new TextObject(xmlNode.Attributes["name"].Value);
      this.IsPattern = Convert.ToBoolean(xmlNode.Attributes["is_pattern"].Value);
      foreach (XmlNode childNode in xmlNode.ChildNodes)
      {
        if (childNode.Name == "Icon")
        {
          int int32 = Convert.ToInt32(childNode.Attributes["id"].Value);
          string materialName = childNode.Attributes["material_name"].Value;
          int textureIndex = int.Parse(childNode.Attributes["texture_index"].Value);
          if (!this._allIcons.ContainsKey(int32))
            this._allIcons.Add(int32, new BannerIconData(materialName, textureIndex));
          if (childNode.Attributes["is_reserved"] == null || !Convert.ToBoolean(childNode.Attributes["is_reserved"].Value))
            this._availableIcons.Add(int32, new BannerIconData(materialName, textureIndex));
        }
        else if (childNode.Name == "Background")
        {
          int int32 = Convert.ToInt32(childNode.Attributes["id"].Value);
          string str = childNode.Attributes["mesh_name"].Value;
          if (childNode.Attributes["is_base_background"] != null && Convert.ToBoolean(childNode.Attributes["is_base_background"].Value))
            BannerManager.Instance.SetBaseBackgroundId(int32);
          if (!this._allBackgrounds.ContainsKey(int32))
            this._allBackgrounds.Add(int32, str);
        }
      }
    }
  }
}
