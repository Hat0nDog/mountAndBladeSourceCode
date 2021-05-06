// Decompiled with JetBrains decompiler
// Type: TaleWorlds.Localization.TextProcessor.LanguageSpecificTextProcessor
// Assembly: TaleWorlds.Localization, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 26BB3E5A-EB48-4ABD-B2FC-10EF6D7A7285
// Assembly location: E:\SteamLibrary\steamapps\common\Mount & Blade II Bannerlord\bin\Win64_Shipping_Client\TaleWorlds.Localization.dll

using System.Globalization;
using System.Text;

namespace TaleWorlds.Localization.TextProcessor
{
  public abstract class LanguageSpecificTextProcessor
  {
    public abstract void ProcessToken(
      string sourceText,
      ref int cursorPos,
      string token,
      StringBuilder outputString);

    public abstract CultureInfo CultureInfoForLanguage { get; }

    public string Process(string text)
    {
      if (text == null)
        return (string) null;
      bool flag = false;
      for (int index = 0; index < text.Length; ++index)
      {
        if (text[index] == '{')
        {
          flag = true;
          break;
        }
      }
      if (!flag)
        return text;
      StringBuilder outputString = new StringBuilder();
      int index1 = 0;
      while (index1 < text.Length)
      {
        if (text[index1] != '{')
        {
          outputString.Append(text[index1]);
          ++index1;
        }
        else
        {
          string token = LanguageSpecificTextProcessor.ReadFirstToken(text, ref index1);
          if (LanguageSpecificTextProcessor.IsPostProcessToken(token))
            this.ProcessTokenInternal(text, ref index1, token, outputString);
        }
      }
      return outputString.ToString();
    }

    private void ProcessTokenInternal(
      string sourceText,
      ref int cursorPos,
      string token,
      StringBuilder outputString)
    {
      CultureInfo cultureInfoForLanguage = this.CultureInfoForLanguage;
      char ch = token[1];
      if (ch == '^' && token.Length == 2)
      {
        int nextChar = this.FindNextChar(sourceText, cursorPos);
        if (nextChar > cursorPos && nextChar < sourceText.Length)
          outputString.Append(sourceText.Substring(cursorPos, nextChar - 1));
        if (nextChar >= sourceText.Length)
          return;
        outputString.Append(char.ToUpper(sourceText[nextChar], cultureInfoForLanguage));
        cursorPos = nextChar + 1;
      }
      else if (ch == '_' && token.Length == 2)
      {
        int nextChar = this.FindNextChar(sourceText, cursorPos);
        if (nextChar > cursorPos && nextChar < sourceText.Length)
          outputString.Append(sourceText.Substring(cursorPos, nextChar - 1));
        if (nextChar >= sourceText.Length)
          return;
        outputString.Append(char.ToLower(sourceText[nextChar], cultureInfoForLanguage));
        cursorPos = nextChar + 1;
      }
      else
        this.ProcessToken(sourceText, ref cursorPos, token, outputString);
    }

    private int FindNextChar(string sourceText, int cursorPos)
    {
      int index = cursorPos;
      while (index < sourceText.Length && char.IsWhiteSpace(sourceText, index))
        ++index;
      return index;
    }

    private static bool IsPostProcessToken(string token) => token.Length > 1 && token[0] == '.';

    private static string ReadFirstToken(string text, ref int i)
    {
      int num = i;
      while (i < text.Length && text[i] != '}')
        ++i;
      int length = i - num - 1;
      if (i < text.Length)
        ++i;
      return text.Substring(num + 1, length);
    }
  }
}
