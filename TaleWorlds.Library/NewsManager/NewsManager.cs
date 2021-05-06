// Decompiled with JetBrains decompiler
// Type: TaleWorlds.Library.NewsManager.NewsManager
// Assembly: TaleWorlds.Library, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 271D0DB8-3C5D-4850-A565-542B4CF7D347
// Assembly location: E:\SteamLibrary\steamapps\common\Mount & Blade II Bannerlord\bin\Win64_Shipping_Client\TaleWorlds.Library.dll

using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using System.Xml;

namespace TaleWorlds.Library.NewsManager
{
  public class NewsManager
  {
    private string _newsSourceURL;
    private List<NewsItem> _newsItems;
    private bool _isNewsItemCacheDirty = true;
    private string _configPath;
    private const string DataFolder = "/Mount and Blade II Bannerlord/Configs/";
    private const string FileName = "NewsFeedConfig.xml";

    public MBReadOnlyList<NewsItem> NewsItems { get; private set; }

    public bool IsInPreviewMode { get; private set; }

    public string LocalizationID { get; private set; }

    public NewsManager()
    {
      this._newsItems = new List<NewsItem>();
      this.NewsItems = new MBReadOnlyList<NewsItem>(this._newsItems);
      this.UpdateConfigSettings();
    }

    public async Task<MBReadOnlyList<NewsItem>> GetNewsItems(
      bool forceRefresh)
    {
      await this.UpdateNewsItems(forceRefresh);
      return this.NewsItems;
    }

    public void SetNewsSourceURL(string url) => this._newsSourceURL = url;

    public async Task UpdateNewsItems(bool forceRefresh)
    {
      if (ApplicationPlatform.CurrentPlatform == Platform.Durango || ApplicationPlatform.CurrentPlatform == Platform.GDKDesktop)
        return;
      if (!(this._isNewsItemCacheDirty | forceRefresh))
        return;
      try
      {
        if (Uri.IsWellFormedUriString(this._newsSourceURL, UriKind.Absolute))
        {
          this._newsItems = await TaleWorlds.Library.NewsManager.NewsManager.DeserializeObjectAsync<List<NewsItem>>(await Common.PlatformWebHelper.DownloadStringTaskAsync(this._newsSourceURL));
          this.NewsItems = new MBReadOnlyList<NewsItem>(this._newsItems);
        }
      }
      catch (Exception ex)
      {
      }
      this._isNewsItemCacheDirty = false;
    }

    public static Task<T> DeserializeObjectAsync<T>(string json)
    {
      try
      {
        using (new System.IO.StringReader(json))
          return Task.FromResult<T>(JsonConvert.DeserializeObject<T>(json));
      }
      catch (Exception ex)
      {
        Console.WriteLine((object) ex);
        return Task.FromResult<T>(default (T));
      }
    }

    private void UpdateConfigSettings()
    {
      this._configPath = this.GetConfigXMLPath();
      XmlDocument configDocument = new XmlDocument();
      configDocument.Load(this._configPath);
      this.IsInPreviewMode = this.GetIsInPreviewMode(configDocument);
      this.LocalizationID = this.GetLocalizationCode(configDocument);
    }

    private bool GetIsInPreviewMode(XmlDocument configDocument) => bool.Parse(configDocument.ChildNodes[0].SelectSingleNode("UsePreviewLink").Attributes["Value"].InnerText);

    private string GetLocalizationCode(XmlDocument configDocument) => configDocument.ChildNodes[0].SelectSingleNode("LocalizationID").Attributes["Value"].InnerText;

    public void UpdateLocalizationID(string localizationID)
    {
      XmlDocument xmlDocument = new XmlDocument();
      xmlDocument.Load(this._configPath);
      xmlDocument.ChildNodes[0].SelectSingleNode("LocalizationID").Attributes["Value"].Value = localizationID;
      xmlDocument.Save(this._configPath);
    }

    private string GetConfigXMLPath()
    {
      string str = (Common.PlatformFileHelper?.DocumentsPath ?? Environment.GetFolderPath(Environment.SpecialFolder.Personal)) + "/Mount and Blade II Bannerlord/Configs/NewsFeedConfig.xml";
      bool flag1 = File.Exists(str);
      bool flag2 = true;
      if (flag1)
      {
        XmlDocument xmlDocument = new XmlDocument();
        try
        {
          xmlDocument.Load(str);
          flag2 = xmlDocument.HasChildNodes && xmlDocument.FirstChild.HasChildNodes;
        }
        catch (Exception ex)
        {
          Console.WriteLine((object) ex);
          flag2 = false;
        }
      }
      if (!flag1 || !flag2)
      {
        try
        {
          XmlDocument xmlDocument = new XmlDocument();
          XmlNode element = (XmlNode) xmlDocument.CreateElement("Root");
          xmlDocument.AppendChild(element);
          ((XmlElement) element.AppendChild((XmlNode) xmlDocument.CreateElement("LocalizationID"))).SetAttribute("Value", "en");
          ((XmlElement) element.AppendChild((XmlNode) xmlDocument.CreateElement("UsePreviewLink"))).SetAttribute("Value", "False");
          xmlDocument.Save(str);
        }
        catch (Exception ex)
        {
          Console.WriteLine((object) ex);
        }
      }
      return str;
    }
  }
}
