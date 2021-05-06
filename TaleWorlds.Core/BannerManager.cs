// Decompiled with JetBrains decompiler
// Type: TaleWorlds.Core.BannerManager
// Assembly: TaleWorlds.Core, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 43F7B45C-9AE1-4715-A37B-2364840F62AF
// Assembly location: E:\SteamLibrary\steamapps\common\Mount & Blade II Bannerlord\bin\Win64_Shipping_Client\TaleWorlds.Core.dll

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml;
using TaleWorlds.Library;

namespace TaleWorlds.Core
{
  public class BannerManager
  {
    public const int DarkRed = 1;
    public const int Green = 120;
    public const int Blue = 119;
    public const int Purple = 4;
    public const int DarkPurple = 6;
    public const int Orange = 9;
    public const int DarkBlue = 12;
    public const int Red = 118;
    public const int Yellow = 121;
    public MBReadOnlyList<BannerIconGroup> BannerIconGroups;
    public MBReadOnlyDictionary<int, BannerColor> ReadOnlyColorPalette;
    private Dictionary<int, BannerColor> _colorPalette;
    private List<BannerIconGroup> _bannerIconGroups;
    private int _availablePatternCount;
    private int _availableIconCount;

    public static BannerManager Instance { get; private set; }

    public int BaseBackgroundId { get; private set; }

    private BannerManager()
    {
      this._bannerIconGroups = new List<BannerIconGroup>();
      this._colorPalette = new Dictionary<int, BannerColor>();
      this.BannerIconGroups = this._bannerIconGroups.GetReadOnlyList<BannerIconGroup>();
      this.ReadOnlyColorPalette = this._colorPalette.GetReadOnlyDictionary<int, BannerColor>();
    }

    public static void Initialize()
    {
      if (BannerManager.Instance != null)
        return;
      BannerManager.Instance = new BannerManager();
    }

    public static MBReadOnlyDictionary<int, BannerColor> ColorPalette => BannerManager.Instance.ReadOnlyColorPalette;

    public static uint GetColor(int index) => index >= 0 && index < BannerManager.ColorPalette.Count ? BannerManager.ColorPalette[index].Color : 3735928559U;

    public static int GetColorId(uint color)
    {
      for (int key = 0; key < BannerManager.ColorPalette.Count; ++key)
      {
        if ((int) BannerManager.ColorPalette[key].Color == (int) color)
          return key;
      }
      return -1;
    }

    public BannerIconData GetIconDataFromIconId(int id)
    {
      foreach (BannerIconGroup bannerIconGroup in this._bannerIconGroups)
      {
        BannerIconData bannerIconData;
        if (bannerIconGroup.AllIcons.TryGetValue(id, out bannerIconData))
          return bannerIconData;
      }
      return new BannerIconData();
    }

    public int GetRandomBackgroundId(Random random)
    {
      int index = random.Next(0, this._availablePatternCount);
      foreach (BannerIconGroup bannerIconGroup in this.BannerIconGroups)
      {
        if (bannerIconGroup.IsPattern)
        {
          if (index < bannerIconGroup.AllBackgrounds.Count)
            return bannerIconGroup.AllBackgrounds.ElementAt<KeyValuePair<int, string>>(index).Key;
          index -= bannerIconGroup.AllBackgrounds.Count;
        }
      }
      return -1;
    }

    public int GetRandomBannerIconId(Random random)
    {
      int index = random.Next(0, this._availableIconCount);
      foreach (BannerIconGroup bannerIconGroup in this.BannerIconGroups)
      {
        if (!bannerIconGroup.IsPattern)
        {
          if (index < bannerIconGroup.AvailableIcons.Count)
            return bannerIconGroup.AvailableIcons.ElementAt<KeyValuePair<int, BannerIconData>>(index).Key;
          index -= bannerIconGroup.AvailableIcons.Count;
        }
      }
      return -1;
    }

    public string GetBackgroundMeshName(int id)
    {
      foreach (BannerIconGroup bannerIconGroup in this.BannerIconGroups)
      {
        if (bannerIconGroup.IsPattern && bannerIconGroup.AllBackgrounds.ContainsKey(id))
          return bannerIconGroup.AllBackgrounds[id];
      }
      return (string) null;
    }

    public string GetIconSourceTextureName(int id)
    {
      foreach (BannerIconGroup bannerIconGroup in this.BannerIconGroups)
      {
        if (!bannerIconGroup.IsPattern && bannerIconGroup.AllBackgrounds.ContainsKey(id))
          return bannerIconGroup.AllBackgrounds[id];
      }
      return (string) null;
    }

    public void SetBaseBackgroundId(int id) => this.BaseBackgroundId = id;

    public void LoadBannerIcons(string xmlPath) => this.LoadFromXml(this.LoadXmlFile(xmlPath));

    private XmlDocument LoadXmlFile(string path)
    {
      Debug.Print("opening " + path);
      XmlDocument xmlDocument = new XmlDocument();
      StreamReader streamReader = new StreamReader(path);
      xmlDocument.LoadXml(streamReader.ReadToEnd());
      streamReader.Close();
      return xmlDocument;
    }

    private void LoadFromXml(XmlDocument doc)
    {
      Debug.Print("loading banner_icons.xml:");
      if (doc.ChildNodes.Count <= 1)
        throw new TWXmlLoadException("Incorrect XML document format.");
      if (doc.ChildNodes[1].Name != "base")
        throw new TWXmlLoadException("Incorrect XML document format.");
      XmlNode childNode1 = doc.ChildNodes[1].ChildNodes[0];
      if (childNode1.Name != "BannerIconData")
        throw new TWXmlLoadException("Incorrect XML document format.");
      if (childNode1.Name == "BannerIconData")
      {
        foreach (XmlNode childNode2 in childNode1.ChildNodes)
        {
          if (childNode2.Name == "BannerIconGroup")
          {
            BannerIconGroup bannerIconGroup = new BannerIconGroup();
            bannerIconGroup.Deserialize(childNode2);
            this._bannerIconGroups.Add(bannerIconGroup);
          }
          if (childNode2.Name == "BannerColors")
          {
            foreach (XmlNode childNode3 in childNode2.ChildNodes)
            {
              if (childNode3.Name == "Color")
                this._colorPalette.Add(Convert.ToInt32(childNode3.Attributes["id"].Value), new BannerColor(Convert.ToUInt32(childNode3.Attributes["hex"].Value, 16), Convert.ToBoolean(childNode3.Attributes["player_can_choose_for_sigil"]?.Value ?? "false"), Convert.ToBoolean(childNode3.Attributes["player_can_choose_for_background"]?.Value ?? "false")));
            }
          }
        }
      }
      foreach (BannerIconGroup bannerIconGroup in this._bannerIconGroups)
      {
        if (bannerIconGroup.IsPattern)
          this._availablePatternCount += bannerIconGroup.AllBackgrounds.Count;
        else
          this._availableIconCount += bannerIconGroup.AvailableIcons.Count;
      }
    }
  }
}
