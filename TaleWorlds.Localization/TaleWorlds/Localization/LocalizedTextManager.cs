// Decompiled with JetBrains decompiler
// Type: TaleWorlds.Localization.LocalizedTextManager
// Assembly: TaleWorlds.Localization, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 26BB3E5A-EB48-4ABD-B2FC-10EF6D7A7285
// Assembly location: E:\SteamLibrary\steamapps\common\Mount & Blade II Bannerlord\bin\Win64_Shipping_Client\TaleWorlds.Localization.dll

using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Xml;
using TaleWorlds.Library;

namespace TaleWorlds.Localization
{
  public static class LocalizedTextManager
  {
    public const string DefaultEnglishLanguageId = "English";
    private static Dictionary<string, bool> _languageIds = new Dictionary<string, bool>();
    private static readonly Dictionary<string, LocalizedTextManager.LocalizedText> _gameTextDictionary = new Dictionary<string, LocalizedTextManager.LocalizedText>();
    private static readonly Dictionary<string, Dictionary<string, string>> _functionDictionary = new Dictionary<string, Dictionary<string, string>>();

    public static string GetTranslatedText(string languageId, string id)
    {
      LocalizedTextManager.LocalizedText localizedText;
      return LocalizedTextManager._gameTextDictionary.TryGetValue(id, out localizedText) ? localizedText.GetTranslatedText(languageId) : (string) null;
    }

    public static List<string> GetLanguageIds(bool developmentMode)
    {
      List<string> stringList = new List<string>();
      foreach (KeyValuePair<string, bool> languageId in LocalizedTextManager._languageIds)
      {
        if (developmentMode || !languageId.Value)
          stringList.Add(languageId.Key);
      }
      return stringList;
    }

    public static void AddLanguage(string languageId, bool languageUnderDevelopment)
    {
      if (!LocalizedTextManager._languageIds.ContainsKey(languageId))
        LocalizedTextManager._languageIds.Add(languageId, languageUnderDevelopment);
      else
        LocalizedTextManager._languageIds[languageId] = LocalizedTextManager._languageIds[languageId] | languageUnderDevelopment;
    }

    public static MBReadOnlyDictionary<string, string> GetFunctionsOfLanguage(
      string languageId)
    {
      return LocalizedTextManager._functionDictionary.ContainsKey(languageId) ? LocalizedTextManager._functionDictionary[languageId].GetReadOnlyDictionary<string, string>() : (MBReadOnlyDictionary<string, string>) null;
    }

    public static void LoadLocalizationXmls(string[] loadedModules)
    {
      LocalizedTextManager._languageIds.Clear();
      LocalizedTextManager.AddLanguage("English", false);
      foreach (string loadedModule in loadedModules)
      {
        string path = loadedModule + "/ModuleData/Languages";
        if (Directory.Exists(path))
        {
          foreach (string file in Directory.GetFiles(path, "*.xml", SearchOption.AllDirectories))
            LocalizedTextManager.LoadLocalizedTexts(file);
        }
      }
    }

    [CommandLineFunctionality.CommandLineArgumentFunction("check_for_erros", "localization")]
    public static string CheckValidity(List<string> strings)
    {
      if (File.Exists("faulty_translation_lines.txt"))
        File.Delete("faulty_translation_lines.txt");
      bool flag = false;
      foreach (KeyValuePair<string, LocalizedTextManager.LocalizedText> gameText in LocalizedTextManager._gameTextDictionary)
      {
        string key = gameText.Key;
        flag = gameText.Value.CheckValidity(key) | flag;
      }
      return !flag ? "No errors are found." : "Errors are written into 'faulty_translation_lines.txt' file in the binary folder.";
    }

    private static void LoadLocalizedTexts(string xmlPath)
    {
      XmlDocument doc = LocalizedTextManager.LoadXmlFile(xmlPath);
      if (doc == null)
        return;
      LocalizedTextManager.LoadFromXml(doc);
    }

    private static XmlDocument LoadXmlFile(string path)
    {
      try
      {
        Debug.Print("opening " + path);
        XmlDocument xmlDocument = new XmlDocument();
        StreamReader streamReader = new StreamReader(path);
        xmlDocument.LoadXml(streamReader.ReadToEnd());
        streamReader.Close();
        return xmlDocument;
      }
      catch
      {
      }
      return (XmlDocument) null;
    }

    private static void LoadFromXml(XmlDocument doc)
    {
      Debug.Print("Loading localized text xml.");
      if (doc.ChildNodes.Count <= 1 || doc.ChildNodes[1].Name != "base" || doc.ChildNodes[1].ChildNodes[0].Name != "tags" || !(doc.ChildNodes[1].ChildNodes[1].Name == "strings") && !(doc.ChildNodes[1].ChildNodes[1].Name == "functions") || doc.ChildNodes[1].ChildNodes[2] != null && (doc.ChildNodes[1].ChildNodes[2] == null || !(doc.ChildNodes[1].ChildNodes[2].Name == "strings")))
        throw new TWXmlLoadException("Incorrect XML document format!");
      string str = "";
      bool languageUnderDevelopment = false;
      XmlNodeList childNodes = doc.ChildNodes[1].ChildNodes[0].ChildNodes;
      for (int i = 0; i < childNodes.Count; ++i)
      {
        if (childNodes[i].Attributes?["language"] != null)
          str = childNodes[i].Attributes["language"].Value;
        else if (childNodes[i].Attributes?["under_development"] != null)
          languageUnderDevelopment = Convert.ToBoolean(childNodes[i].Attributes?["under_development"].Value);
      }
      if (str.IsStringNoneOrEmpty())
        return;
      if (!LocalizedTextManager._languageIds.ContainsKey(str))
        LocalizedTextManager.AddLanguage(str, languageUnderDevelopment);
      foreach (XmlNode childNode in doc.ChildNodes[1].ChildNodes)
      {
        if (childNode.Name == "strings")
        {
          for (XmlNode node = doc.ChildNodes[1].ChildNodes[1].ChildNodes[0]; node != null; node = node.NextSibling)
          {
            if (node.Name == "string" && node.NodeType != XmlNodeType.Comment)
              LocalizedTextManager.DeserilaizeStrings(node, str);
          }
        }
        else if (childNode.Name == "functions")
        {
          for (XmlNode node = doc.ChildNodes[1].ChildNodes[1].ChildNodes[0]; node != null; node = node.NextSibling)
          {
            if (node.Name == "function" && node.NodeType != XmlNodeType.Comment)
              LocalizedTextManager.DeserilaizeFunctions(node, str);
          }
        }
      }
    }

    private static void DeserilaizeStrings(XmlNode node, string languageId)
    {
      if (node.Attributes == null)
        throw new TWXmlLoadException("Node attributes are null!");
      string key = node.Attributes["id"].Value;
      string translation = node.Attributes["text"].Value;
      if (!LocalizedTextManager._gameTextDictionary.ContainsKey(key))
      {
        LocalizedTextManager.LocalizedText localizedText = new LocalizedTextManager.LocalizedText();
        LocalizedTextManager._gameTextDictionary.Add(key, localizedText);
      }
      LocalizedTextManager._gameTextDictionary[key].AddTranslation(languageId, translation);
    }

    private static void DeserilaizeFunctions(XmlNode node, string languageId)
    {
      if (node.Attributes == null)
        throw new TWXmlLoadException("Node attributes are null!");
      string key = node.Attributes["functionName"].Value;
      string str = node.Attributes["functionBody"].Value;
      if (!LocalizedTextManager._functionDictionary.ContainsKey(languageId))
        LocalizedTextManager._functionDictionary.Add(languageId, new Dictionary<string, string>());
      if (LocalizedTextManager._functionDictionary[languageId].ContainsKey(key))
        LocalizedTextManager._functionDictionary[languageId][key] = str;
      else
        LocalizedTextManager._functionDictionary[languageId].Add(key, str);
    }

    private class LocalizedText
    {
      private readonly Dictionary<string, string> _localizedTextDictionary;

      public LocalizedText() => this._localizedTextDictionary = new Dictionary<string, string>();

      public void AddTranslation(string language, string translation)
      {
        if (this._localizedTextDictionary.ContainsKey(language))
          return;
        this._localizedTextDictionary.Add(language, translation);
      }

      public string GetTranslatedText(string languageId)
      {
        string str;
        return this._localizedTextDictionary.TryGetValue(languageId, out str) || this._localizedTextDictionary.TryGetValue("English", out str) ? str : (string) null;
      }

      public bool CheckValidity(string id)
      {
        bool flag1 = false;
        foreach (KeyValuePair<string, string> localizedText in this._localizedTextDictionary)
        {
          string str1 = localizedText.Value;
          int num1 = 0;
          int num2 = 0;
          bool flag2 = false;
          foreach (char ch in str1)
          {
            switch (ch)
            {
              case '{':
                ++num1;
                break;
              case '}':
                ++num2;
                break;
            }
            if (num1 > num2 && ch == ' ')
              flag2 = true;
          }
          if (flag2)
          {
            File.AppendAllText("faulty_translation_lines.txt", "Text_id:" + id + " within language " + localizedText.Key + " has empty space inside left and right brackets '{ , }'. Faulty string: " + str1 + "\n\n", Encoding.Unicode);
            flag1 = true;
          }
          int num3 = 0;
          int num4 = 0;
          string str2 = str1;
          while (true)
          {
            do
            {
              int num5 = str2.IndexOf("{?");
              if (num5 != -1)
              {
                int startIndex = Math.Min(num5 + 1, str2.Length - 1);
                str2 = str2.Substring(startIndex);
              }
              else
                goto label_16;
            }
            while (str2.Length <= 2 || str2[1] == '}');
            ++num3;
          }
label_16:
          string str3 = str1;
          while (true)
          {
            int num5 = str3.IndexOf("{\\?}");
            if (num5 != -1)
            {
              ++num4;
              int startIndex = Math.Min(num5 + 1, str1.Length - 1);
              str3 = str3.Substring(startIndex);
            }
            else
              break;
          }
          if (num1 != num2)
          {
            File.AppendAllText("faulty_translation_lines.txt", "Text_id:" + id + " within language " + localizedText.Key + " does not have a matching number of left-right brackets. Faulty string: " + str1 + "\n\n", Encoding.Unicode);
            flag1 = true;
          }
          if (num3 != num4)
          {
            File.AppendAllText("faulty_translation_lines.txt", "Text_id:" + id + " within language " + localizedText.Key + " have not-matching number of condition starters and enders. Faulty string: " + str1 + "\n\n", Encoding.Unicode);
            flag1 = true;
          }
        }
        return flag1;
      }
    }
  }
}
