// Decompiled with JetBrains decompiler
// Type: TaleWorlds.ObjectSystem.XmlResource
// Assembly: TaleWorlds.ObjectSystem, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 525679E6-68C3-48B8-A030-465E146E69EE
// Assembly location: E:\SteamLibrary\steamapps\common\Mount & Blade II Bannerlord\bin\Win64_Shipping_Client\TaleWorlds.ObjectSystem.dll

using System.Collections.Generic;
using System.IO;
using System.Xml;
using TaleWorlds.ModuleManager;

namespace TaleWorlds.ObjectSystem
{
  public static class XmlResource
  {
    public static List<MbObjectXmlInformation> XmlInformationList = new List<MbObjectXmlInformation>();
    public static List<MbObjectXmlInformation> MbprojXmls = new List<MbObjectXmlInformation>();

    public static void InitializeXmlInformationList(List<MbObjectXmlInformation> xmlInformation) => XmlResource.XmlInformationList = xmlInformation;

    public static void GetMbprojxmls(string moduleName)
    {
      string mbprojPath = ModuleHelper.GetMbprojPath(moduleName);
      if (!File.Exists(mbprojPath))
        return;
      XmlDocument xmlDocument = new XmlDocument();
      xmlDocument.Load(mbprojPath);
      XmlNodeList xmlNodeList = xmlDocument.SelectSingleNode("base").SelectNodes("file");
      if (xmlNodeList == null)
        return;
      foreach (XmlNode xmlNode in xmlNodeList)
      {
        string innerText1 = xmlNode.Attributes["id"].InnerText;
        string innerText2 = xmlNode.Attributes["name"].InnerText;
        MbObjectXmlInformation objectXmlInformation = new MbObjectXmlInformation()
        {
          Id = innerText1,
          Name = innerText2,
          ModuleName = moduleName,
          GameTypesIncluded = new List<string>()
        };
        XmlResource.MbprojXmls.Add(objectXmlInformation);
      }
    }

    public static void GetXmlListAndApply(string moduleName)
    {
      string path = ModuleHelper.GetPath(moduleName);
      XmlDocument xmlDocument = new XmlDocument();
      xmlDocument.Load(path);
      XmlNodeList xmlNodeList = xmlDocument.SelectSingleNode("Module").SelectNodes("Xmls/XmlNode");
      if (xmlNodeList == null)
        return;
      foreach (XmlNode xmlNode1 in xmlNodeList)
      {
        XmlNode xmlNode2 = xmlNode1.SelectSingleNode("XmlName");
        string innerText1 = xmlNode2.Attributes["id"].InnerText;
        string innerText2 = xmlNode2.Attributes["path"].InnerText;
        List<string> stringList = new List<string>();
        XmlNode xmlNode3 = xmlNode1.SelectSingleNode("IncludedGameTypes");
        if (xmlNode3 != null)
        {
          foreach (XmlNode childNode in xmlNode3.ChildNodes)
            stringList.Add(childNode.Attributes["value"].InnerText);
        }
        MbObjectXmlInformation objectXmlInformation = new MbObjectXmlInformation()
        {
          Id = innerText1,
          Name = innerText2,
          ModuleName = moduleName,
          GameTypesIncluded = stringList
        };
        XmlResource.XmlInformationList.Add(objectXmlInformation);
      }
    }
  }
}
