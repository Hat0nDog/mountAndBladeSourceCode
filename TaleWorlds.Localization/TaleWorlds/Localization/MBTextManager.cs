// Decompiled with JetBrains decompiler
// Type: TaleWorlds.Localization.MBTextManager
// Assembly: TaleWorlds.Localization, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 26BB3E5A-EB48-4ABD-B2FC-10EF6D7A7285
// Assembly location: E:\SteamLibrary\steamapps\common\Mount & Blade II Bannerlord\bin\Win64_Shipping_Client\TaleWorlds.Localization.dll

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using TaleWorlds.Library;
using TaleWorlds.Localization.TextProcessor;
using TaleWorlds.Localization.TextProcessor.LanguageProcessors;

namespace TaleWorlds.Localization
{
  public static class MBTextManager
  {
    private const string TurkishLanguageId = "Türkçe";
    private const string EnglishLanguageId = "English";
    private const string GermanLanguageId = "Deutsch";
    private const string PolishLanguageId = "Polski";
    private static readonly TextProcessingContext TextContext = new TextProcessingContext();
    private static LanguageSpecificTextProcessor _languageProcessor = (LanguageSpecificTextProcessor) new EnglishTextProcessor();
    private static string _currentLanguageId = "English";
    [ThreadStatic]
    private static StringBuilder _idStringBuilder;
    [ThreadStatic]
    private static StringBuilder _targetStringBuilder;
    internal static readonly Tokenizer Tokenizer = new Tokenizer();

    public static bool LanguageExistsInCurrentConfiguration(string language, bool developmentMode) => LocalizedTextManager.GetLanguageIds(developmentMode).Any<string>((Func<string, bool>) (l => l == language));

    public static bool ChangeLanguage(string language)
    {
      if (!LocalizedTextManager.GetLanguageIds(true).Any<string>((Func<string, bool>) (l => l == language)))
        return false;
      string str = language;
      MBTextManager._languageProcessor = str == "Türkçe" ? (LanguageSpecificTextProcessor) new TurkishTextProcessor() : (str == "English" ? (LanguageSpecificTextProcessor) new EnglishTextProcessor() : (str == "Deutsch" ? (LanguageSpecificTextProcessor) new GermanTextProcessor() : (str == "Polski" ? (LanguageSpecificTextProcessor) new PolishTextProcessor() : (LanguageSpecificTextProcessor) new DefaultTextProcessor())));
      MBTextManager._currentLanguageId = language;
      MBTextManager.LoadFunctionsOnLanguageChange(MBTextManager._currentLanguageId);
      return true;
    }

    private static void LoadFunctionsOnLanguageChange(string languageId)
    {
      MBTextManager.TextContext.ResetFunctions();
      MBReadOnlyDictionary<string, string> functionsOfLanguage = LocalizedTextManager.GetFunctionsOfLanguage(languageId);
      if (functionsOfLanguage == null)
        return;
      foreach (KeyValuePair<string, string> keyValuePair in functionsOfLanguage)
        MBTextManager.SetFunction(keyValuePair.Key, keyValuePair.Value);
    }

    private static TextObject ProcessNumber(object integer) => new TextObject(integer.ToString());

    internal static string ProcessTextToString(TextObject to)
    {
      if (to == null)
        return (string) null;
      if (TextObject.IsNullOrEmpty(to))
        return "";
      string localizedText = MBTextManager.GetLocalizedText(to.Value);
      string str;
      if (!string.IsNullOrEmpty(to.Value))
      {
        string text = MBTextManager.Process(localizedText, to);
        str = MBTextManager._languageProcessor.Process(text);
      }
      else
        str = "";
      return str;
    }

    internal static string ProcessWithoutLanguageProcessor(TextObject to)
    {
      if (to == null)
        return (string) null;
      if (TextObject.IsNullOrEmpty(to))
        return "";
      string localizedText = MBTextManager.GetLocalizedText(to.Value);
      return string.IsNullOrEmpty(to.Value) ? "" : MBTextManager.Process(localizedText, to);
    }

    private static string Process(string query, TextObject parent = null) => TextGrammarProcessor.Process(MBTextParser.Parse(MBTextManager.Tokenizer.Tokenize(query)), MBTextManager.TextContext, parent);

    public static void ClearAll() => MBTextManager.TextContext.ClearAll();

    public static void SetTextVariable(string variableName, string text, bool sendClients = false)
    {
      if (text == null)
        return;
      MBTextManager.TextContext.SetTextVariable(variableName, new TextObject(text));
    }

    public static void SetTextVariable(string variableName, TextObject text, bool sendClients = false)
    {
      if (text == null)
        return;
      MBTextManager.TextContext.SetTextVariable(variableName, text);
    }

    public static void SetTextVariable(string variableName, int content)
    {
      TextObject text = MBTextManager.ProcessNumber((object) content);
      MBTextManager.SetTextVariable(variableName, text, false);
    }

    public static void SetTextVariable(string variableName, float content)
    {
      TextObject text = MBTextManager.ProcessNumber((object) content);
      MBTextManager.SetTextVariable(variableName, text, false);
    }

    public static void SetTextVariable(string variableName, object content)
    {
      if (content == null)
        return;
      TextObject text = new TextObject(content.ToString());
      MBTextManager.SetTextVariable(variableName, text, false);
    }

    public static void SetTextVariable(string variableName, int arrayIndex, object content)
    {
      if (content == null)
        return;
      string text = content.ToString();
      MBTextManager.SetTextVariable(variableName + ":" + (object) arrayIndex, text, false);
    }

    public static void SetFunction(string funcName, string functionBody)
    {
      MBTextModel functionBody1 = MBTextParser.Parse(MBTextManager.Tokenizer.Tokenize(functionBody));
      MBTextManager.TextContext.SetFunction(funcName, functionBody1);
    }

    internal static string GetLocalizedText(string text)
    {
      if (text == null || text.Length <= 2 || (text[0] != '{' || text[1] != '='))
        return text;
      if (MBTextManager._idStringBuilder == null)
        MBTextManager._idStringBuilder = new StringBuilder(8);
      else
        MBTextManager._idStringBuilder.Clear();
      if (MBTextManager._targetStringBuilder == null)
        MBTextManager._targetStringBuilder = new StringBuilder(100);
      else
        MBTextManager._targetStringBuilder.Clear();
      for (int index1 = 2; index1 < text.Length; ++index1)
      {
        if (text[index1] != '}')
        {
          MBTextManager._idStringBuilder.Append(text[index1]);
        }
        else
        {
          for (int index2 = index1 + 1; index2 < text.Length; ++index2)
            MBTextManager._targetStringBuilder.Append(text[index2]);
          string localizedText = "";
          if (MBTextManager._currentLanguageId == "English")
            return MBTextManager.RemoveComments(MBTextManager._targetStringBuilder.ToString());
          if ((MBTextManager._idStringBuilder.Length != 1 || MBTextManager._idStringBuilder[0] != '*') && (MBTextManager._idStringBuilder.Length != 1 || MBTextManager._idStringBuilder[0] != '!'))
          {
            if (MBTextManager._currentLanguageId != "English")
              localizedText = LocalizedTextManager.GetTranslatedText(MBTextManager._currentLanguageId, MBTextManager._idStringBuilder.ToString());
            if (localizedText != null)
              return MBTextManager.RemoveComments(localizedText);
            break;
          }
          break;
        }
      }
      return MBTextManager._targetStringBuilder.ToString();
    }

    private static string RemoveComments(string localizedText)
    {
      string pattern = "{%.+?}";
      foreach (Match match in Regex.Matches(localizedText, pattern))
        localizedText = localizedText.Replace(match.Value, "");
      return localizedText;
    }

    public static string DiscardAnimationTagsAndSoundPath(string text) => Regex.Replace(text, "\\[.+\\]", "");

    public static string[] GetConversationAnimationsAndSoundPath(
      TextObject to,
      out string soundPath)
    {
      string str1 = to.CopyTextObject().ToString();
      StringBuilder stringBuilder1 = new StringBuilder();
      string[] strArray = new string[4];
      bool flag1 = false;
      int index1 = 0;
      StringBuilder stringBuilder2 = new StringBuilder();
      bool flag2 = false;
      if (!string.IsNullOrEmpty(str1))
      {
        for (int index2 = 0; index2 < str1.Length; ++index2)
        {
          if (str1[index2] == '[')
            flag1 = true;
          else if (flag1)
          {
            if (str1[index2] == ',' || str1[index2] == ']')
            {
              strArray[index1] = stringBuilder1.ToString();
              stringBuilder1.Clear();
              if (str1[index2] == ']')
                flag1 = false;
            }
            else if (str1[index2] == ':')
            {
              string str2 = stringBuilder1.ToString();
              stringBuilder1.Clear();
              if (str2 == "ib")
                index1 = 0;
              else if (str2 == "if")
                index1 = 1;
              else if (str2 == "rb")
                index1 = 2;
              else if (str2 == "rf")
                index1 = 3;
              else if (str2 == "sp")
              {
                flag2 = true;
                flag1 = false;
              }
            }
            else if (str1[index2] != ' ')
              stringBuilder1.Append(str1[index2]);
          }
          else if (flag2)
          {
            if (str1[index2] == ']')
              flag2 = false;
            else
              stringBuilder2.Append(str1[index2]);
          }
        }
      }
      soundPath = stringBuilder2.ToString();
      return strArray;
    }
  }
}
