// Decompiled with JetBrains decompiler
// Type: TaleWorlds.Localization.TextProcessor.LanguageProcessors.TurkishTextProcessor
// Assembly: TaleWorlds.Localization, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 26BB3E5A-EB48-4ABD-B2FC-10EF6D7A7285
// Assembly location: E:\SteamLibrary\steamapps\common\Mount & Blade II Bannerlord\bin\Win64_Shipping_Client\TaleWorlds.Localization.dll

using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;

namespace TaleWorlds.Localization.TextProcessor.LanguageProcessors
{
  public class TurkishTextProcessor : LanguageSpecificTextProcessor
  {
    private static CultureInfo _curCultureInfo = CultureInfo.InvariantCulture;
    private static char[] Vowels = new char[8]
    {
      'a',
      'ı',
      'o',
      'u',
      'e',
      'i',
      'ö',
      'ü'
    };
    private static char[] BackVowels = new char[4]
    {
      'a',
      'ı',
      'o',
      'u'
    };
    private static char[] FrontVowels = new char[4]
    {
      'e',
      'i',
      'ö',
      'ü'
    };
    private static char[] OpenVowels = new char[4]
    {
      'a',
      'e',
      'o',
      'ö'
    };
    private static char[] ClosedVowels = new char[4]
    {
      'ı',
      'i',
      'u',
      'ü'
    };
    private static char[] Consonants = new char[21]
    {
      'b',
      'c',
      'ç',
      'd',
      'f',
      'g',
      'ğ',
      'h',
      'j',
      'k',
      'l',
      'm',
      'n',
      'p',
      'r',
      's',
      'ş',
      't',
      'v',
      'y',
      'z'
    };
    private static char[] UnvoicedConsonants = new char[8]
    {
      'ç',
      'f',
      'h',
      'k',
      'p',
      's',
      'ş',
      't'
    };
    private static char[] HardUnvoicedConsonants = new char[4]
    {
      'p',
      'ç',
      't',
      'k'
    };
    private static string[] NonMutatingWord = new string[15]
    {
      "ak",
      "at",
      "ek",
      "et",
      "göç",
      "ip",
      "çöp",
      "ok",
      "ot",
      "saç",
      "sap",
      "süt",
      "üç",
      "suç",
      "top"
    };
    private static CultureInfo _cultureInfo = new CultureInfo("tr-TR");

    private bool IsVowel(char c) => ((IEnumerable<char>) TurkishTextProcessor.Vowels).Contains<char>(c);

    private bool IsBackVowel(char c) => ((IEnumerable<char>) TurkishTextProcessor.BackVowels).Contains<char>(c);

    private bool IsFrontVowel(char c) => ((IEnumerable<char>) TurkishTextProcessor.FrontVowels).Contains<char>(c);

    private bool IsClosedVowel(char c) => ((IEnumerable<char>) TurkishTextProcessor.ClosedVowels).Contains<char>(c);

    private bool IsConsonant(char c) => ((IEnumerable<char>) TurkishTextProcessor.Consonants).Contains<char>(c);

    private bool IsUnvoicedConsonant(char c) => ((IEnumerable<char>) TurkishTextProcessor.UnvoicedConsonants).Contains<char>(c);

    private bool IsHardUnvoicedConsonant(char c) => ((IEnumerable<char>) TurkishTextProcessor.HardUnvoicedConsonants).Contains<char>(c);

    private char FrontVowelToBackVowel(char c)
    {
      switch (c)
      {
        case 'e':
          return 'a';
        case 'i':
          return 'ı';
        case 'ö':
          return 'o';
        case 'ü':
          return 'u';
        default:
          return '*';
      }
    }

    private char OpenVowelToClosedVowel(char c)
    {
      switch (c)
      {
        case 'a':
          return 'ı';
        case 'e':
          return 'i';
        case 'o':
          return 'u';
        case 'ö':
          return 'ü';
        default:
          return '*';
      }
    }

    private char HardConsonantToSoftConsonant(char c)
    {
      switch (c)
      {
        case 'k':
          return 'ğ';
        case 'p':
          return 'b';
        case 't':
          return 'd';
        case 'ç':
          return 'c';
        default:
          return '*';
      }
    }

    private char GetLastVowel(StringBuilder outputText)
    {
      for (int index = outputText.Length - 1; index >= 0; --index)
      {
        if (this.IsVowel(outputText[index]))
          return outputText[index];
      }
      return '*';
    }

    public override void ProcessToken(
      string sourceText,
      ref int cursorPos,
      string token,
      StringBuilder outputString)
    {
      if (token == ".im")
        this.AddSuffix_im(outputString);
      else if (token == ".sin")
        this.AddSuffix_sin(outputString);
      else if (token == ".dir")
        this.AddSuffix_dir(outputString);
      else if (token == ".iz")
        this.AddSuffix_iz(outputString);
      else if (token == ".siniz")
        this.AddSuffix_siniz(outputString);
      else if (token == ".dirler")
        this.AddSuffix_dirler(outputString);
      else if (token == ".i")
        this.AddSuffix_i(outputString);
      else if (token == ".e")
        this.AddSuffix_e(outputString);
      else if (token == ".de")
        this.AddSuffix_de(outputString);
      else if (token == ".den")
        this.AddSuffix_den(outputString);
      else if (token == ".nin")
        this.AddSuffix_nin(outputString);
      else if (token == ".ler")
        this.AddSuffix_ler(outputString);
      else if (token == ".m")
        this.AddSuffix_m(outputString);
      else if (token == ".n")
        this.AddSuffix_n(outputString);
      else if (token == ".si")
        this.AddSuffix_si(outputString);
      else if (token == ".miz")
        this.AddSuffix_miz(outputString);
      else if (token == ".niz")
      {
        this.AddSuffix_niz(outputString);
      }
      else
      {
        if (!(token == ".leri"))
          return;
        this.AddSuffix_leri(outputString);
      }
    }

    private void AddSuffix_im(StringBuilder outputString)
    {
      char lastVowel = this.GetLastVowel(outputString);
      char ch = this.IsClosedVowel(lastVowel) ? lastVowel : this.OpenVowelToClosedVowel(lastVowel);
      this.SoftenLastCharacter(outputString);
      this.AddYIfNeeded(outputString);
      outputString.Append(ch);
      outputString.Append('m');
    }

    private void AddSuffix_sin(StringBuilder outputString)
    {
      char lastVowel = this.GetLastVowel(outputString);
      char ch = this.IsClosedVowel(lastVowel) ? lastVowel : this.OpenVowelToClosedVowel(lastVowel);
      outputString.Append('s');
      outputString.Append(ch);
      outputString.Append('n');
    }

    private void AddSuffix_dir(StringBuilder outputString)
    {
      char lastVowel = this.GetLastVowel(outputString);
      char ch = this.IsClosedVowel(lastVowel) ? lastVowel : this.OpenVowelToClosedVowel(lastVowel);
      char harmonizedD = this.GetHarmonizedD(outputString);
      outputString.Append(harmonizedD);
      outputString.Append(ch);
      outputString.Append('r');
    }

    private void AddSuffix_iz(StringBuilder outputString)
    {
      char lastVowel = this.GetLastVowel(outputString);
      char ch = this.IsClosedVowel(lastVowel) ? lastVowel : this.OpenVowelToClosedVowel(lastVowel);
      this.SoftenLastCharacter(outputString);
      this.AddYIfNeeded(outputString);
      outputString.Append(ch);
      outputString.Append('z');
    }

    private void AddSuffix_siniz(StringBuilder outputString)
    {
      char lastVowel = this.GetLastVowel(outputString);
      char ch = this.IsClosedVowel(lastVowel) ? lastVowel : this.OpenVowelToClosedVowel(lastVowel);
      outputString.Append('s');
      outputString.Append(ch);
      outputString.Append('n');
      outputString.Append(ch);
      outputString.Append('z');
    }

    private void AddSuffix_dirler(StringBuilder outputString)
    {
      char lastVowel = this.GetLastVowel(outputString);
      char ch1 = this.IsClosedVowel(lastVowel) ? lastVowel : this.OpenVowelToClosedVowel(lastVowel);
      char ch2 = this.IsBackVowel(lastVowel) ? 'a' : 'e';
      char harmonizedD = this.GetHarmonizedD(outputString);
      outputString.Append(harmonizedD);
      outputString.Append(ch1);
      outputString.Append('r');
      outputString.Append('l');
      outputString.Append(ch2);
      outputString.Append('r');
    }

    private void AddSuffix_i(StringBuilder outputString)
    {
      char lastVowel = this.GetLastVowel(outputString);
      char ch = this.IsClosedVowel(lastVowel) ? lastVowel : this.OpenVowelToClosedVowel(lastVowel);
      this.SoftenLastCharacter(outputString);
      this.AddYIfNeeded(outputString);
      outputString.Append(ch);
    }

    private void AddSuffix_e(StringBuilder outputString)
    {
      char ch = this.IsBackVowel(this.GetLastVowel(outputString)) ? 'a' : 'e';
      this.SoftenLastCharacter(outputString);
      this.AddYIfNeeded(outputString);
      outputString.Append(ch);
    }

    private void AddSuffix_de(StringBuilder outputString)
    {
      char ch = this.IsBackVowel(this.GetLastVowel(outputString)) ? 'a' : 'e';
      char harmonizedD = this.GetHarmonizedD(outputString);
      outputString.Append(harmonizedD);
      outputString.Append(ch);
    }

    private void AddSuffix_den(StringBuilder outputString)
    {
      char ch = this.IsBackVowel(this.GetLastVowel(outputString)) ? 'a' : 'e';
      char harmonizedD = this.GetHarmonizedD(outputString);
      outputString.Append(harmonizedD);
      outputString.Append(ch);
      outputString.Append('n');
    }

    private void AddSuffix_nin(StringBuilder outputString)
    {
      char lastVowel = this.GetLastVowel(outputString);
      char ch = this.IsClosedVowel(lastVowel) ? lastVowel : this.OpenVowelToClosedVowel(lastVowel);
      char lastCharacter = this.GetLastCharacter(outputString);
      this.SoftenLastCharacter(outputString);
      if (this.IsVowel(lastCharacter))
        outputString.Append('n');
      outputString.Append(ch);
      outputString.Append('n');
    }

    private void AddSuffix_ler(StringBuilder outputString)
    {
      char ch = this.IsBackVowel(this.GetLastVowel(outputString)) ? 'a' : 'e';
      outputString.Append('l');
      outputString.Append(ch);
      outputString.Append('r');
    }

    private void AddSuffix_m(StringBuilder outputString)
    {
      char lastVowel = this.GetLastVowel(outputString);
      char ch = this.IsClosedVowel(lastVowel) ? lastVowel : this.OpenVowelToClosedVowel(lastVowel);
      char lastCharacter = this.GetLastCharacter(outputString);
      this.SoftenLastCharacter(outputString);
      if (this.IsConsonant(lastCharacter))
        outputString.Append(ch);
      outputString.Append('m');
    }

    private void AddSuffix_n(StringBuilder outputString)
    {
      char lastVowel = this.GetLastVowel(outputString);
      char ch = this.IsClosedVowel(lastVowel) ? lastVowel : this.OpenVowelToClosedVowel(lastVowel);
      char lastCharacter = this.GetLastCharacter(outputString);
      this.SoftenLastCharacter(outputString);
      if (this.IsConsonant(lastCharacter))
        outputString.Append(ch);
      outputString.Append('n');
    }

    private void AddSuffix_si(StringBuilder outputString)
    {
      char lastVowel = this.GetLastVowel(outputString);
      char ch = this.IsClosedVowel(lastVowel) ? lastVowel : this.OpenVowelToClosedVowel(lastVowel);
      char lastCharacter = this.GetLastCharacter(outputString);
      this.SoftenLastCharacter(outputString);
      if (this.IsVowel(lastCharacter))
        outputString.Append('s');
      outputString.Append(ch);
    }

    private void AddSuffix_miz(StringBuilder outputString)
    {
      char lastVowel = this.GetLastVowel(outputString);
      char ch = this.IsClosedVowel(lastVowel) ? lastVowel : this.OpenVowelToClosedVowel(lastVowel);
      char lastCharacter = this.GetLastCharacter(outputString);
      this.SoftenLastCharacter(outputString);
      if (this.IsConsonant(lastCharacter))
        outputString.Append(ch);
      outputString.Append('m');
      outputString.Append(ch);
      outputString.Append('z');
    }

    private void AddSuffix_niz(StringBuilder outputString)
    {
      char lastVowel = this.GetLastVowel(outputString);
      char ch = this.IsClosedVowel(lastVowel) ? lastVowel : this.OpenVowelToClosedVowel(lastVowel);
      char lastCharacter = this.GetLastCharacter(outputString);
      this.SoftenLastCharacter(outputString);
      if (this.IsConsonant(lastCharacter))
        outputString.Append(ch);
      outputString.Append('n');
      outputString.Append(ch);
      outputString.Append('z');
    }

    private void AddSuffix_leri(StringBuilder outputString)
    {
      char lastVowel = this.GetLastVowel(outputString);
      char ch1 = this.IsBackVowel(lastVowel) ? 'a' : 'e';
      char ch2 = this.IsBackVowel(lastVowel) ? 'ı' : 'i';
      outputString.Append('l');
      outputString.Append(ch1);
      outputString.Append('r');
      outputString.Append(ch2);
    }

    private char GetHarmonizedD(StringBuilder outputString)
    {
      char c = this.GetLastCharacter(outputString);
      if (c == '\'')
        c = this.GetSecondLastCharacter(outputString);
      return !this.IsUnvoicedConsonant(c) ? 'd' : 't';
    }

    private void AddYIfNeeded(StringBuilder outputString)
    {
      char lastCharacter = this.GetLastCharacter(outputString);
      if (!this.IsVowel(lastCharacter) && (lastCharacter != '\'' || !this.IsVowel(this.GetSecondLastCharacter(outputString))))
        return;
      outputString.Append('y');
    }

    private void SoftenLastCharacter(StringBuilder outputString)
    {
      char lastCharacter = this.GetLastCharacter(outputString);
      if (!this.IsHardUnvoicedConsonant(lastCharacter) || this.LastWordNonMutating(outputString))
        return;
      outputString[outputString.Length - 1] = this.HardConsonantToSoftConsonant(lastCharacter);
    }

    private bool LastWordNonMutating(StringBuilder outputString)
    {
      int num = -1;
      for (int index = outputString.Length - 1; index >= 0 && num < 0; --index)
      {
        if (outputString[index] == ' ')
          num = index;
      }
      if (num >= outputString.Length - 1)
        return false;
      string str = outputString.ToString(num + 1, outputString.Length - num - 1);
      return ((IEnumerable<string>) TurkishTextProcessor.NonMutatingWord).Contains<string>(str.ToLower(this.CultureInfoForLanguage));
    }

    private char GetLastCharacter(StringBuilder outputString) => outputString.Length <= 0 ? '*' : outputString[outputString.Length - 1];

    private char GetSecondLastCharacter(StringBuilder outputString) => outputString.Length <= 1 ? '*' : outputString[outputString.Length - 2];

    public override CultureInfo CultureInfoForLanguage => TurkishTextProcessor._cultureInfo;
  }
}
