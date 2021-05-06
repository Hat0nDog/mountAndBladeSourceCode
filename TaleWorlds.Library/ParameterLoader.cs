// Decompiled with JetBrains decompiler
// Type: TaleWorlds.Library.ParameterLoader
// Assembly: TaleWorlds.Library, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 271D0DB8-3C5D-4850-A565-542B4CF7D347
// Assembly location: E:\SteamLibrary\steamapps\common\Mount & Blade II Bannerlord\bin\Win64_Shipping_Client\TaleWorlds.Library.dll

using System;
using System.IO;
using System.Xml;

namespace TaleWorlds.Library
{
  public class ParameterLoader
  {
    public static ParameterContainer LoadParametersFromClientProfile(
      string configurationName)
    {
      ParameterContainer parameters = new ParameterContainer();
      XmlDocument xmlDocument = new XmlDocument();
      xmlDocument.LoadXml(VirtualFolders.GetFileContent(BasePath.Name + "Parameters/ClientProfile.xml"));
      ParameterLoader.LoadParametersInto("ClientProfiles/" + xmlDocument.ChildNodes[0].Attributes["Value"].InnerText + "/" + configurationName + ".xml", parameters);
      return parameters;
    }

    public static void LoadParametersInto(string fileFullName, ParameterContainer parameters)
    {
      XmlDocument xmlDocument = new XmlDocument();
      xmlDocument.LoadXml(VirtualFolders.GetFileContent(BasePath.Name + "Parameters/" + fileFullName));
      foreach (XmlNode childNode1 in xmlDocument.FirstChild.ChildNodes)
      {
        if (childNode1.Name == "Parameters")
        {
          foreach (XmlNode childNode2 in childNode1.ChildNodes)
          {
            string innerText = childNode2.Attributes["Name"].InnerText;
            string str1;
            string str2;
            string str3 = !ParameterLoader.TryGetFromFile(childNode2, out str1) ? (!ParameterLoader.TryGetFromEnvironment(childNode2, out str2) ? (childNode2.Attributes["DefaultValue"] == null ? childNode2.Attributes["Value"].InnerText : childNode2.Attributes["DefaultValue"].InnerText) : str2) : str1;
            parameters.AddParameter(innerText, str3, true);
          }
        }
      }
    }

    private static bool TryGetFromFile(XmlNode node, out string value)
    {
      value = "";
      if (node.Attributes?["LoadFromFile"] != null && node.Attributes["LoadFromFile"].InnerText.ToLower() == "true")
      {
        string innerText = node.Attributes["File"].InnerText;
        if (File.Exists(innerText))
        {
          string str = File.ReadAllText(innerText);
          value = str;
          return true;
        }
      }
      return false;
    }

    private static bool TryGetFromEnvironment(XmlNode node, out string value)
    {
      value = "";
      if (node.Attributes?["GetFromEnvironment"] != null && node.Attributes["GetFromEnvironment"].InnerText.ToLower() == "true")
      {
        string environmentVariable = Environment.GetEnvironmentVariable(node.Attributes["Variable"].InnerText);
        if (!string.IsNullOrEmpty(environmentVariable))
        {
          value = environmentVariable;
          return true;
        }
      }
      return false;
    }
  }
}
