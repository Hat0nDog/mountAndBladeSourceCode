// Decompiled with JetBrains decompiler
// Type: TaleWorlds.Localization.TextProcessor.LanguageProcessors.GermanTextProcessor
// Assembly: TaleWorlds.Localization, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 26BB3E5A-EB48-4ABD-B2FC-10EF6D7A7285
// Assembly location: E:\SteamLibrary\steamapps\common\Mount & Blade II Bannerlord\bin\Win64_Shipping_Client\TaleWorlds.Localization.dll

using System;
using System.Globalization;
using System.Text;

namespace TaleWorlds.Localization.TextProcessor.LanguageProcessors
{
  public class GermanTextProcessor : LanguageSpecificTextProcessor
  {
    private static readonly CultureInfo CultureInfo = new CultureInfo("de-DE");
    private GermanTextProcessor.WordGenderEnum _curGender;
    private static char[] Vowels = new char[8]
    {
      'a',
      'e',
      'i',
      'o',
      'u',
      'ä',
      'ü',
      'ö'
    };
    private const string Consonants = "BbCcDdFfGgHhJjKkLlMmNnPpRrSsTtWwYyZz";

    public override CultureInfo CultureInfoForLanguage => GermanTextProcessor.CultureInfo;

    private bool Masculine => this._curGender == GermanTextProcessor.WordGenderEnum.Masculine;

    private bool Feminine => this._curGender == GermanTextProcessor.WordGenderEnum.Feminine;

    private bool Neuter => this._curGender == GermanTextProcessor.WordGenderEnum.Neuter;

    private bool Plural => this._curGender == GermanTextProcessor.WordGenderEnum.Plural;

    private string GetVowelEnding(StringBuilder outputString)
    {
      if (outputString.Length == 0)
        return "";
      char c = outputString[outputString.Length - 1];
      if (!this.IsVowel(c))
        return "";
      return outputString.Length > 1 && this.IsVowel(outputString[outputString.Length - 2]) ? outputString[outputString.Length - 2].ToString() + c.ToString() : c.ToString();
    }

    private char GetLastCharacter(StringBuilder outputString) => outputString.Length <= 0 ? '*' : outputString[outputString.Length - 1];

    private char GetSecondLastCharacter(StringBuilder outputString) => outputString.Length <= 1 ? '*' : outputString[outputString.Length - 2];

    private string GetEnding(StringBuilder outputString, int numChars)
    {
      numChars = Math.Min(numChars, outputString.Length);
      return outputString.ToString(outputString.Length - numChars, numChars);
    }

    public override void ProcessToken(
      string sourceText,
      ref int cursorPos,
      string token,
      StringBuilder outputString)
    {
      if (token == ".M")
        this.SetMasculine();
      else if (token == ".F")
        this.SetFeminine();
      else if (token == ".N")
        this.SetNeuter();
      else if (token == ".P")
        this.SetPlural();
      else if (token == ".p")
        this.AddSuffixPlural(outputString);
      else if (token == ".dp")
        this.AddSuffixDativePlural(outputString);
      else if (token == ".g")
        this.AddSuffixGenitive(outputString);
      else if (token == ".wn")
        this.AddSuffixWeakNominative(outputString);
      else if (token == ".mn")
        this.AddSuffixMixedNominative(outputString);
      else if (token == ".sn")
        this.AddSuffixStrongNominative(outputString);
      else if (token == ".wa")
        this.AddSuffixWeakAccusative(outputString);
      else if (token == ".ma")
        this.AddSuffixMixedAccusative(outputString);
      else if (token == ".sa")
        this.AddSuffixStrongAccusative(outputString);
      else if (token == ".sd")
      {
        this.AddSuffixStrongDative(outputString);
      }
      else
      {
        if (!(token == ".sg"))
          return;
        this.AddSuffixStrongGenitive(outputString);
      }
    }

    private void AddSuffixGenitive(StringBuilder outputString)
    {
      if (this.Feminine || !this.Masculine && !this.Neuter)
        return;
      this.AddSuffixGenitiveMN(outputString);
    }

    private void AddSuffixGenitiveMN(StringBuilder outputString)
    {
      if (!this.IsVowel(this.GetLastCharacter(outputString)))
        outputString.Append('e');
      outputString.Append('s');
    }

    private void AddSuffixDativePlural(StringBuilder outputString)
    {
      this.AddSuffixPlural(outputString);
      switch (this.GetLastCharacter(outputString))
      {
        case 'n':
          break;
        case 's':
          break;
        default:
          outputString.Append('n');
          break;
      }
    }

    private void AddSuffix(StringBuilder outputString, char v) => outputString.Append(v);

    private void RemoveLastCharacter(StringBuilder outputString) => outputString.Remove(outputString.Length - 1, 1);

    private bool IsLastCharVowel(StringBuilder outputString) => this.IsVowel(outputString[outputString.Length - 1]);

    private void AddSuffixPlural(StringBuilder outputString)
    {
      if (this.Feminine)
        this.AddSuffixPluralF(outputString);
      else if (this.Masculine)
      {
        this.AddSuffixPluralM(outputString);
      }
      else
      {
        if (!this.Neuter)
          return;
        this.AddSuffixPluralN(outputString);
      }
    }

    private void AddSuffixPluralN(StringBuilder outputString)
    {
      this.GetEnding(outputString, 2);
      outputString.Append('e');
    }

    private void AddSuffixPluralM(StringBuilder outputString) => outputString.Append('e');

    private void AddSuffixPluralF(StringBuilder outputString)
    {
      if (this.GetLastCharacter(outputString) == 'e')
        outputString.Append('n');
      else if (this.GetEnding(outputString, 2) == "in")
        outputString.Append("nen");
      else
        outputString.Append("en");
    }

    private void SetFeminine() => this._curGender = GermanTextProcessor.WordGenderEnum.Feminine;

    private void SetNeuter() => this._curGender = GermanTextProcessor.WordGenderEnum.Neuter;

    private void SetMasculine() => this._curGender = GermanTextProcessor.WordGenderEnum.Masculine;

    private void SetPlural() => this._curGender = GermanTextProcessor.WordGenderEnum.Plural;

    private bool IsVowel(char c) => Array.IndexOf<char>(GermanTextProcessor.Vowels, c) >= 0;

    private int FindLastVowel(StringBuilder outputText)
    {
      for (int index = outputText.Length - 1; index >= 0; --index)
      {
        if (this.IsVowel(outputText[index]))
          return index;
      }
      return -1;
    }

    private void ModifyAdjective(StringBuilder outputString)
    {
      string ending = this.GetEnding(outputString, 2);
      if (ending[1] == 'e')
      {
        this.RemoveLastCharacter(outputString);
      }
      else
      {
        if (!ending.Equals("el") && (!ending.Equals("er") || !this.IsVowel(this.GetEnding(outputString, 3)[0])) && !outputString.ToString().Contains("hoch"))
          return;
        outputString.Remove(outputString.Length - 2, 1);
      }
    }

    private void AddSuffixWeakNominative(StringBuilder outputString)
    {
      this.ModifyAdjective(outputString);
      if (this.Plural)
        outputString.Append("en");
      else
        outputString.Append('e');
    }

    private void AddSuffixMixedNominative(StringBuilder outputString)
    {
      this.ModifyAdjective(outputString);
      if (this.Masculine)
        outputString.Append("er");
      else if (this.Feminine)
        outputString.Append("e");
      else if (this.Neuter)
        outputString.Append("es");
      else
        outputString.Append("en");
    }

    private void AddSuffixStrongNominative(StringBuilder outputString)
    {
      this.ModifyAdjective(outputString);
      if (this.Masculine)
        outputString.Append("er");
      else if (this.Feminine)
        outputString.Append("e");
      else if (this.Neuter)
        outputString.Append("es");
      else
        outputString.Append("e");
    }

    private void AddSuffixWeakAccusative(StringBuilder outputString)
    {
      this.ModifyAdjective(outputString);
      if (this.Masculine || this.Plural)
        outputString.Append("en");
      else
        outputString.Append("e");
    }

    private void AddSuffixMixedAccusative(StringBuilder outputString)
    {
      this.ModifyAdjective(outputString);
      if (this.Masculine || this.Plural)
        outputString.Append("en");
      else if (this.Feminine)
        outputString.Append("e");
      else
        outputString.Append("es");
    }

    private void AddSuffixStrongAccusative(StringBuilder outputString)
    {
      this.ModifyAdjective(outputString);
      if (this.Masculine)
        outputString.Append("en");
      else if (this.Feminine || this.Plural)
        outputString.Append("e");
      else
        outputString.Append("es");
    }

    private void AddSuffixStrongDative(StringBuilder outputString)
    {
      this.ModifyAdjective(outputString);
      if (this.Feminine)
        outputString.Append("er");
      else if (this.Plural)
        outputString.Append("en");
      else
        outputString.Append("em");
    }

    private void AddSuffixStrongGenitive(StringBuilder outputString)
    {
      this.ModifyAdjective(outputString);
      if (this.Feminine || this.Plural)
        outputString.Append("er");
      else
        outputString.Append("en");
    }

    private enum WordGenderEnum
    {
      Masculine,
      Feminine,
      Neuter,
      Plural,
    }
  }
}
