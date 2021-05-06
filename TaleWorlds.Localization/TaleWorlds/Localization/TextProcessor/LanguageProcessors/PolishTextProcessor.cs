// Decompiled with JetBrains decompiler
// Type: TaleWorlds.Localization.TextProcessor.LanguageProcessors.PolishTextProcessor
// Assembly: TaleWorlds.Localization, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 26BB3E5A-EB48-4ABD-B2FC-10EF6D7A7285
// Assembly location: E:\SteamLibrary\steamapps\common\Mount & Blade II Bannerlord\bin\Win64_Shipping_Client\TaleWorlds.Localization.dll

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace TaleWorlds.Localization.TextProcessor.LanguageProcessors
{
  public class PolishTextProcessor : LanguageSpecificTextProcessor
  {
    private static readonly CultureInfo CultureInfo = new CultureInfo("pl-PL");
    private PolishTextProcessor.WordGenderEnum _curGender;
    private readonly List<string> _wordGroups = new List<string>();
    private readonly List<string> _wordGroupsNoTags = new List<string>();
    private static readonly char[] _vowels = new char[9]
    {
      'a',
      'ą',
      'e',
      'ę',
      'i',
      'o',
      'ó',
      'u',
      'y'
    };
    private static readonly char[] _softConsonants = new char[5]
    {
      'ć',
      'ń',
      'ś',
      'ź',
      'j'
    };
    private static readonly string[] _hardenedConsonants = new string[6]
    {
      "dz",
      "c",
      "sz",
      "rz",
      "cz",
      "ż"
    };
    private static readonly string[] _hardConsonants = new string[16]
    {
      "g",
      "k",
      "ch",
      "r",
      "w",
      "f",
      "p",
      "m",
      "b",
      "d",
      "t",
      "n",
      "s",
      "z",
      "ł",
      "h"
    };
    private static readonly Dictionary<string, string> _palatalization = new Dictionary<string, string>()
    {
      {
        "g",
        "gi"
      },
      {
        "k",
        "ki"
      },
      {
        "ch",
        "sz"
      },
      {
        "r",
        "rz"
      },
      {
        "w",
        "wi"
      },
      {
        "f",
        "fi"
      },
      {
        "p",
        "pi"
      },
      {
        "m",
        "mi"
      },
      {
        "j",
        "j"
      },
      {
        "b",
        "bi"
      },
      {
        "d",
        "dzi"
      },
      {
        "t",
        "ci"
      },
      {
        "n",
        "ni"
      },
      {
        "s",
        "si"
      },
      {
        "z",
        "zi"
      },
      {
        "ł",
        "l"
      },
      {
        "ś",
        "si"
      },
      {
        "ź",
        "zi"
      },
      {
        "ń",
        "ni"
      },
      {
        "ć",
        "ci"
      }
    };
    private static readonly string[] _nounTokens = new string[14]
    {
      ".n",
      ".p",
      ".a",
      ".g",
      ".i",
      ".l",
      ".d",
      ".v",
      ".ap",
      ".gp",
      ".ip",
      ".lp",
      ".dp",
      ".vp"
    };
    private const string Consonants = "BbCcĆćDdFfGgHhJjKkLlŁłMmNnŃńPpRrSsŚśTtWwZzŹźŻż";

    public override CultureInfo CultureInfoForLanguage => PolishTextProcessor.CultureInfo;

    private bool MasculinePersonal => this._curGender == PolishTextProcessor.WordGenderEnum.MasculinePersonal;

    private bool MasculineAnimate => this._curGender == PolishTextProcessor.WordGenderEnum.MasculineAnimate;

    private bool MasculineInanimate => this._curGender == PolishTextProcessor.WordGenderEnum.MasculineInanimate;

    private bool Feminine => this._curGender == PolishTextProcessor.WordGenderEnum.Feminine;

    private bool Neuter => this._curGender == PolishTextProcessor.WordGenderEnum.Neuter;

    private char GetLastCharacter(StringBuilder outputString) => outputString.Length <= 0 ? '*' : outputString[outputString.Length - 1];

    private string GetEnding(StringBuilder outputString, int numChars)
    {
      numChars = Math.Min(numChars, outputString.Length);
      return outputString.ToString(outputString.Length - numChars, numChars);
    }

    private void PalatalizeConsonant(StringBuilder outputString, string lastTwoCharacters)
    {
      int length = 1;
      string str;
      if (!PolishTextProcessor._palatalization.TryGetValue(lastTwoCharacters[1].ToString(), out str) && PolishTextProcessor._palatalization.TryGetValue(lastTwoCharacters, out str))
      {
        length = 2;
        str = "";
      }
      outputString.Remove(outputString.Length - length, length);
      outputString.Append(str);
    }

    public override void ProcessToken(
      string sourceText,
      ref int cursorPos,
      string token,
      StringBuilder outputString)
    {
      if (token == ".MP")
        this.SetMasculinePersonal();
      else if (token == ".MA")
        this.SetMasculineAnimate();
      else if (token == ".MI")
        this.SetMasculineInanimate();
      else if (token == ".F")
        this.SetFeminine();
      else if (token == ".N")
        this.SetNeuter();
      else if (token == ".aj" || token == ".nn")
      {
        this._wordGroups.Add(sourceText);
        this._wordGroupsNoTags.Add(this.Process(sourceText.Replace("{.nnp}", "").Replace("{.ajp}", "").Replace("{.nn}", "").Replace("{.aj}", "")));
      }
      else if (token == ".nnp" || token == ".ajp")
      {
        if (token == ".nnp")
          this.AddSuffixNounNominativePlural(outputString);
        else
          this.AddSuffixAdjectiveNominativePlural(outputString);
        this._wordGroups.Add(sourceText);
        this._wordGroupsNoTags.Add(this.Process(sourceText.Replace("{.nnp}", "{.p}").Replace("{.ajp}", "{.jp}").Replace("{.nn}", "").Replace("{.aj}", "")));
      }
      else
      {
        int wordGroupIndex;
        if (Array.IndexOf<string>(PolishTextProcessor._nounTokens, token) >= 0 && this.IsWordGroup(token.Length, sourceText, cursorPos, out wordGroupIndex))
        {
          if (wordGroupIndex < 0)
            return;
          token = "{" + token + "}";
          this.AddSuffixWordGroup(token, wordGroupIndex, outputString);
        }
        else if (token == ".p")
          this.AddSuffixNounNominativePlural(outputString);
        else if (token == ".a")
          this.AddSuffixNounAccusative(outputString);
        else if (token == ".ap")
          this.AddSuffixNounAccusativePlural(outputString);
        else if (token == ".v")
          this.AddSuffixNounVocative(outputString);
        else if (token == ".vp")
          this.AddSuffixNounVocativePlural(outputString);
        else if (token == ".g")
          this.AddSuffixNounGenitive(outputString);
        else if (token == ".gp")
          this.AddSuffixNounGenitivePlural(outputString);
        else if (token == ".d")
          this.AddSuffixNounDative(outputString);
        else if (token == ".dp")
          this.AddSuffixNounDativePlural(outputString);
        else if (token == ".l")
          this.AddSuffixNounLocative(outputString);
        else if (token == ".lp")
          this.AddSuffixNounLocativePlural(outputString);
        else if (token == ".i")
          this.AddSuffixNounInstrumental(outputString);
        else if (token == ".ip")
          this.AddSuffixNounInstrumentalPlural(outputString);
        else if (token == ".j")
          this.AddSuffixAdjectiveNominative(outputString);
        else if (token == ".jp")
          this.AddSuffixAdjectiveNominativePlural(outputString);
        else if (token == ".ja")
          this.AddSuffixAdjectiveAccusative(outputString);
        else if (token == ".jap")
          this.AddSuffixAdjectiveAccusativePlural(outputString);
        else if (token == ".jv")
          this.AddSuffixAdjectiveVocative(outputString);
        else if (token == ".jvp")
          this.AddSuffixAdjectiveVocativePlural(outputString);
        else if (token == ".jg")
          this.AddSuffixAdjectiveGenitive(outputString);
        else if (token == ".jgp")
          this.AddSuffixAdjectiveGenitivePlural(outputString);
        else if (token == ".jd")
          this.AddSuffixAdjectiveDative(outputString);
        else if (token == ".jdp")
          this.AddSuffixAdjectiveDativePlural(outputString);
        else if (token == ".jl")
          this.AddSuffixAdjectiveLocative(outputString);
        else if (token == ".jlp")
          this.AddSuffixAdjectiveLocativePlural(outputString);
        else if (token == ".ji")
        {
          this.AddSuffixAdjectiveInstrumental(outputString);
        }
        else
        {
          if (!(token == ".jip"))
            return;
          this.AddSuffixAdjectiveInstrumentalPlural(outputString);
        }
      }
    }

    private void AddSuffixWordGroup(string token, int wordGroupIndex, StringBuilder outputString)
    {
      string wordGroup = this._wordGroups[wordGroupIndex];
      outputString.Remove(outputString.Length - this._wordGroupsNoTags[wordGroupIndex].Length, this._wordGroupsNoTags[wordGroupIndex].Length);
      this._wordGroups.RemoveAt(wordGroupIndex);
      this._wordGroupsNoTags.RemoveAt(wordGroupIndex);
      string str1 = wordGroup.Replace("{.nn}", token);
      string text;
      if (token.Equals("{.n}"))
      {
        text = str1.Replace("{.nnp}", "{.p}").Replace("{.ajp}", "{.jp}").Replace("{.aj}", "{.j}");
      }
      else
      {
        string str2 = str1.Replace("{.aj}", token.Insert(2, "j"));
        text = !token.Contains("p") ? str2.Replace("{.nnp}", token.Insert(3, "p")).Replace("{.ajp}", token.Insert(2, "j").Insert(4, "p")) : str2.Replace("{.nnp}", token).Replace("{.ajp}", token.Insert(2, "j"));
      }
      outputString.Append(this.Process(text));
    }

    private bool IsWordGroup(
      int tokenLength,
      string sourceText,
      int curPos,
      out int wordGroupIndex)
    {
      for (int index = 0; index < this._wordGroupsNoTags.Count && curPos - tokenLength - 2 - this._wordGroupsNoTags[index].Length >= 0; ++index)
      {
        if (sourceText.Substring(curPos - tokenLength - 2 - this._wordGroupsNoTags[index].Length, this._wordGroupsNoTags[index].Length).Equals(this._wordGroupsNoTags[index]))
        {
          wordGroupIndex = index;
          return true;
        }
      }
      wordGroupIndex = -1;
      return false;
    }

    private void AddSuffixNounNominativePlural(StringBuilder outputString)
    {
      string ending = this.GetEnding(outputString, 2);
      if ((this.MasculineAnimate || this.MasculineInanimate || this.MasculinePersonal) && ending[0] == 'ó')
      {
        outputString.Remove(outputString.Length - 2, 2);
        outputString.Append("o" + ending[1].ToString());
      }
      if (this.Neuter)
      {
        if (this.IsVowel(ending[1]))
          outputString.Remove(outputString.Length - 1, 1);
        if (this.GetEnding(outputString, 2).Equals("um"))
          return;
        outputString.Append('a');
      }
      else if (this.Feminine || this.MasculineAnimate || (this.MasculineInanimate || this.GetLastCharacter(outputString) == 'a'))
      {
        if (ending[1] == 'a')
        {
          outputString.Remove(outputString.Length - 1, 1);
          ending = this.GetEnding(outputString, 2);
        }
        if (this.GetEnding(outputString, 3).Equals("ość"))
        {
          outputString.Remove(outputString.Length - 1, 1);
          outputString.Append("ci");
        }
        else if (this.MasculineInanimate && ending.Equals("to"))
        {
          outputString.Remove(outputString.Length - 1, 1);
          outputString.Append('a');
        }
        else if (ending.Equals("ch"))
          outputString.Append("y");
        else if (ending[1] == 'g' || ending[1] == 'k')
          outputString.Append('i');
        else if (outputString.Length > 4 && this.GetEnding(outputString, 5).Equals("rzecz"))
          outputString.Append('y');
        else if (this.IsVowel(ending[1]) || this.IsSoftConsonant(ending) || this.IsHardenedConsonant(ending))
        {
          if (this.IsSoftConsonant(ending))
          {
            this.PalatalizeConsonant(outputString, ending);
            outputString.Append("e");
          }
          else
            outputString.Append('e');
        }
        else
          outputString.Append('y');
      }
      else if (ending.Equals("ch"))
      {
        outputString.Remove(outputString.Length - 2, 2);
        outputString.Append("si");
      }
      else if (ending[1] == 'g')
      {
        outputString.Remove(outputString.Length - 1, 1);
        outputString.Append("dzy");
      }
      else if (ending[1] == 'k')
      {
        outputString.Remove(outputString.Length - 1, 1);
        outputString.Append("cy");
      }
      else if (ending[1] == 'r')
      {
        outputString.Remove(outputString.Length - 1, 1);
        outputString.Append("rzy");
      }
      else if (ending[1] == 't')
      {
        outputString.Remove(outputString.Length - 1, 1);
        outputString.Append("ci");
      }
      else if (this.IsSoftConsonant(ending) || this.IsHardenedConsonant(ending))
      {
        if (this.IsSoftConsonant(ending))
        {
          this.PalatalizeConsonant(outputString, ending);
          outputString.Append("e");
        }
        else
          outputString.Append('e');
      }
      else
        outputString.Append('i');
    }

    private void AddSuffixNounAccusative(StringBuilder outputString)
    {
      string ending = this.GetEnding(outputString, 2);
      if (this.MasculineAnimate || this.MasculinePersonal)
      {
        if (ending[0] == 'ó')
        {
          outputString.Remove(outputString.Length - 2, 2);
          outputString.Append("o" + ending[1].ToString());
        }
        if (this.IsVowel(ending[1]))
        {
          if (ending[1] == 'a')
            return;
          outputString.Append('a');
        }
        else if (this.IsSoftConsonant(ending))
        {
          this.PalatalizeConsonant(outputString, ending);
          outputString.Append("a");
        }
        else
          outputString.Append('a');
      }
      else
      {
        if (!this.Feminine || !this.IsVowel(ending[1]))
          return;
        outputString.Remove(outputString.Length - 1, 1);
        outputString.Append('ę');
      }
    }

    private void AddSuffixNounAccusativePlural(StringBuilder outputString)
    {
      string ending = this.GetEnding(outputString, 2);
      if ((this.MasculineAnimate || this.MasculineInanimate || this.MasculinePersonal) && ending[0] == 'ó')
      {
        outputString.Remove(outputString.Length - 2, 2);
        outputString.Append("o" + ending[1].ToString());
      }
      if (this.Neuter)
      {
        if (this.IsVowel(ending[1]))
          outputString.Remove(outputString.Length - 1, 1);
        if (this.GetEnding(outputString, 2).Equals("um"))
          return;
        outputString.Append('a');
      }
      else if (this.Feminine || this.MasculineAnimate || (this.MasculineInanimate || this.GetLastCharacter(outputString) == 'a'))
      {
        if (ending[1] == 'a')
        {
          outputString.Remove(outputString.Length - 1, 1);
          ending = this.GetEnding(outputString, 2);
        }
        if (this.GetEnding(outputString, 3).Equals("ość"))
        {
          outputString.Remove(outputString.Length - 1, 1);
          outputString.Append("ci");
        }
        else if (this.MasculineInanimate && ending.Equals("to"))
        {
          outputString.Remove(outputString.Length - 1, 1);
          outputString.Append('a');
        }
        else if (ending[1] == 'g' || ending[1] == 'k')
          outputString.Append('i');
        else if (outputString.Length > 4 && this.GetEnding(outputString, 5).Equals("rzecz"))
          outputString.Append('y');
        else if (this.IsVowel(ending[1]) || this.IsSoftConsonant(ending) || this.IsHardenedConsonant(ending))
          outputString.Append('e');
        else
          outputString.Append('y');
      }
      else if (this.IsSoftConsonant(ending))
      {
        this.PalatalizeConsonant(outputString, ending);
        outputString.Append("ów");
      }
      else
        outputString.Append("ów");
    }

    private void AddSuffixNounGenitive(StringBuilder outputString)
    {
      string ending = this.GetEnding(outputString, 2);
      if ((this.MasculineAnimate || this.MasculineInanimate || this.MasculinePersonal) && ending[0] == 'ó')
      {
        outputString.Remove(outputString.Length - 2, 2);
        outputString.Append("o" + ending[1].ToString());
      }
      if (this.Feminine)
      {
        if (ending[1] == 'a')
        {
          outputString.Remove(outputString.Length - 1, 1);
          ending = this.GetEnding(outputString, 2);
        }
        if (this.IsVowel(ending[1]))
          return;
        if (this.IsSoftConsonant(ending))
          this.PalatalizeConsonant(outputString, ending);
        else if (ending[1] == 'g' || ending[1] == 'k' || this.GetEnding(outputString, 2).Equals("ch"))
          outputString.Append('i');
        else
          outputString.Append('y');
      }
      else
      {
        if (this.IsVowel(ending[1]))
          outputString.Remove(outputString.Length - 1, 1);
        if (this.GetEnding(outputString, 2).Equals("um"))
          return;
        if (this.IsSoftConsonant(ending))
          this.PalatalizeConsonant(outputString, ending);
        outputString.Append('a');
      }
    }

    private void AddSuffixNounGenitivePlural(StringBuilder outputString)
    {
      if (this.IsVowel(this.GetLastCharacter(outputString)))
        outputString.Remove(outputString.Length - 1, 1);
      string ending = this.GetEnding(outputString, 2);
      if ((this.MasculineAnimate || this.MasculineInanimate || this.MasculinePersonal) && ending[0] == 'ó')
      {
        outputString.Remove(outputString.Length - 2, 2);
        outputString.Append("o" + ending[1].ToString());
      }
      if (this.Feminine && this.IsHardenedConsonant(ending))
        outputString.Append('y');
      else if (this.MasculinePersonal)
      {
        if (this.IsSoftConsonant(ending))
          this.PalatalizeConsonant(outputString, ending);
        outputString.Append("ów");
      }
      else
      {
        if (!this.MasculineAnimate && !this.MasculineInanimate)
          return;
        if (this.MasculineInanimate && ending.Equals("to"))
          outputString.Remove(outputString.Length - 1, 1);
        else if (this.IsSoftConsonant(ending))
        {
          outputString.Append('i');
        }
        else
        {
          if (this.IsSoftConsonant(ending))
            this.PalatalizeConsonant(outputString, ending);
          outputString.Append("ów");
        }
      }
    }

    private void AddSuffixNounDative(StringBuilder outputString)
    {
      char lastCharacter = this.GetLastCharacter(outputString);
      string ending = this.GetEnding(outputString, 2);
      if ((this.MasculineAnimate || this.MasculineInanimate || this.MasculinePersonal) && ending[0] == 'ó')
      {
        outputString.Remove(outputString.Length - 2, 2);
        outputString.Append("o" + ending[1].ToString());
      }
      if (this.Feminine)
      {
        if (lastCharacter == 'a')
        {
          outputString.Remove(outputString.Length - 1, 1);
          lastCharacter = this.GetLastCharacter(outputString);
          ending = this.GetEnding(outputString, 2);
        }
        if (this.IsVowel(lastCharacter))
          return;
        if (this.IsSoftConsonant(ending))
          this.PalatalizeConsonant(outputString, ending);
        else if (this.IsHardenedConsonant(ending))
          outputString.Append('y');
        else if (ending.Equals("ch"))
        {
          outputString.Remove(outputString.Length - 2, 2);
          outputString.Append("szie");
        }
        else if (lastCharacter == 'g')
        {
          outputString.Remove(outputString.Length - 1, 1);
          outputString.Append("dzie");
        }
        else if (lastCharacter == 'k')
        {
          outputString.Remove(outputString.Length - 1, 1);
          outputString.Append("cie");
        }
        else
          outputString.Append("ie");
      }
      else if (this.Neuter)
      {
        if (this.IsVowel(lastCharacter))
          outputString.Remove(outputString.Length - 1, 1);
        if (this.GetEnding(outputString, 2).Equals("um"))
          return;
        outputString.Append('u');
      }
      else if (this.MasculineInanimate && ending.Equals("to"))
      {
        outputString.Remove(outputString.Length - 1, 1);
        outputString.Append('u');
      }
      else
      {
        if (this.IsSoftConsonant(ending))
          this.PalatalizeConsonant(outputString, ending);
        outputString.Append("owi");
      }
    }

    private void AddSuffixNounDativePlural(StringBuilder outputString)
    {
      string ending = this.GetEnding(outputString, 2);
      if ((this.MasculineAnimate || this.MasculineInanimate || this.MasculinePersonal) && ending[0] == 'ó')
      {
        outputString.Remove(outputString.Length - 2, 2);
        outputString.Append("o" + ending[1].ToString());
      }
      if (this.IsVowel(ending[1]))
        outputString.Remove(outputString.Length - 1, 1);
      if (ending.Equals("um"))
        return;
      if (this.IsSoftConsonant(ending))
        this.PalatalizeConsonant(outputString, ending);
      outputString.Append("om");
    }

    private void AddSuffixNounLocative(StringBuilder outputString)
    {
      int lastCharacter = (int) this.GetLastCharacter(outputString);
      string ending = this.GetEnding(outputString, 2);
      if ((this.MasculineAnimate || this.MasculineInanimate || this.MasculinePersonal) && ending[0] == 'ó')
      {
        outputString.Remove(outputString.Length - 2, 2);
        outputString.Append("o" + ending[1].ToString());
      }
      if (this.Feminine)
      {
        if (ending[1] == 'a')
        {
          outputString.Remove(outputString.Length - 1, 1);
          ending = this.GetEnding(outputString, 2);
        }
        if (!this.IsVowel(ending[1]))
        {
          if (this.IsSoftConsonant(ending))
            outputString.Append('i');
          else if (this.IsHardenedConsonant(ending))
            outputString.Append('y');
          else if (ending.Equals("ch"))
          {
            outputString.Remove(outputString.Length - 2, 2);
            outputString.Append("szie");
          }
          else if (ending[1] == 'g')
          {
            outputString.Remove(outputString.Length - 1, 1);
            outputString.Append("dzie");
          }
          else if (ending[1] == 'k')
          {
            outputString.Remove(outputString.Length - 1, 1);
            outputString.Append("cie");
          }
          else
            outputString.Append("ie");
        }
        else
        {
          if (this.IsVowel(ending[1]))
            outputString.Remove(outputString.Length - 1, 1);
          if (this.IsSoftConsonant(ending) || this.IsHardenedConsonant(ending) || (ending.Equals("ch") || ending[1] == 'g') || ending[1] == 'k')
            outputString.Append('u');
          else
            outputString.Append('e');
        }
      }
      else
      {
        if (this.IsVowel(ending[1]))
        {
          outputString.Remove(outputString.Length - 1, 1);
          ending = this.GetEnding(outputString, 2);
        }
        if (!this.IsVowel(ending[1]) && !ending.Equals("um"))
        {
          if (this.IsSoftConsonant(ending))
          {
            this.PalatalizeConsonant(outputString, ending);
            outputString.Append("u");
          }
          else if (this.IsHardenedConsonant(ending) || ending.Equals("ch") || (ending[1] == 'g' || ending[1] == 'k'))
          {
            outputString.Append("u");
          }
          else
          {
            this.PalatalizeConsonant(outputString, ending);
            outputString.Append("e");
          }
        }
        else
          outputString.Append("u");
      }
    }

    private void AddSuffixNounLocativePlural(StringBuilder outputString)
    {
      if (this.IsVowel(this.GetLastCharacter(outputString)))
        outputString.Remove(outputString.Length - 1, 1);
      string ending = this.GetEnding(outputString, 2);
      if ((this.MasculineAnimate || this.MasculineInanimate || this.MasculinePersonal) && ending[0] == 'ó')
      {
        outputString.Remove(outputString.Length - 2, 2);
        outputString.Append("o" + ending[1].ToString());
      }
      if (this.IsSoftConsonant(ending))
        this.PalatalizeConsonant(outputString, ending);
      outputString.Append("ach");
    }

    private void AddSuffixNounVocative(StringBuilder outputString)
    {
      char lastCharacter = this.GetLastCharacter(outputString);
      string ending = this.GetEnding(outputString, 2);
      if ((this.MasculineAnimate || this.MasculineInanimate || this.MasculinePersonal) && ending[0] == 'ó')
      {
        outputString.Remove(outputString.Length - 2, 2);
        outputString.Append("o" + ending[1].ToString());
      }
      if (this.Feminine || lastCharacter == 'a')
      {
        if (lastCharacter == 'a')
        {
          outputString.Remove(outputString.Length - 1, 1);
          outputString.Append('o');
        }
        else if (this.IsSoftConsonant(ending))
          this.PalatalizeConsonant(outputString, ending);
        else
          outputString.Append('i');
      }
      else
      {
        if (this.Neuter)
          return;
        if (this.IsSoftConsonant(ending))
        {
          this.PalatalizeConsonant(outputString, ending);
          outputString.Append("u");
        }
        else if (this.IsHardenedConsonant(ending) || ending.Equals("ch") || (lastCharacter == 'g' || lastCharacter == 'k'))
        {
          outputString.Append("u");
        }
        else
        {
          this.PalatalizeConsonant(outputString, ending);
          outputString.Append("e");
        }
      }
    }

    private void AddSuffixNounVocativePlural(StringBuilder outputString) => this.AddSuffixNounNominativePlural(outputString);

    private void AddSuffixNounInstrumental(StringBuilder outputString)
    {
      string ending = this.GetEnding(outputString, 2);
      if ((this.MasculineAnimate || this.MasculineInanimate || this.MasculinePersonal) && ending[0] == 'ó')
      {
        outputString.Remove(outputString.Length - 2, 2);
        outputString.Append("o" + ending[1].ToString());
      }
      if (this.Feminine)
      {
        if (ending[1] == 'a')
        {
          outputString.Remove(outputString.Length - 1, 1);
          ending = this.GetEnding(outputString, 2);
        }
        if (this.IsSoftConsonant(ending))
          this.PalatalizeConsonant(outputString, ending);
        outputString.Append('ą');
      }
      else
      {
        if (this.IsVowel(ending[1]))
        {
          outputString.Remove(outputString.Length - 1, 1);
          ending = this.GetEnding(outputString, 2);
        }
        if (ending.Equals("um"))
          return;
        if (this.IsSoftConsonant(ending) || ending[1] == 'k')
          this.PalatalizeConsonant(outputString, ending);
        outputString.Append("em");
      }
    }

    private void AddSuffixNounInstrumentalPlural(StringBuilder outputString)
    {
      string ending = this.GetEnding(outputString, 2);
      if ((this.MasculineAnimate || this.MasculineInanimate || this.MasculinePersonal) && ending[0] == 'ó')
      {
        outputString.Remove(outputString.Length - 2, 2);
        outputString.Append("o" + ending[1].ToString());
      }
      if (this.IsVowel(ending[1]))
      {
        outputString.Remove(outputString.Length - 1, 1);
        ending = this.GetEnding(outputString, 2);
      }
      if (this.IsSoftConsonant(ending))
        this.PalatalizeConsonant(outputString, ending);
      outputString.Append("ami");
    }

    private void AddSuffixAdjectiveNominative(StringBuilder outputString)
    {
      char ch = this.RemoveSuffixFromAdjective(outputString);
      if (this.Feminine)
        outputString.Append('a');
      else if (this.Neuter)
      {
        outputString.Append('e');
      }
      else
      {
        if (ch != 'y')
          return;
        outputString.Append('y');
      }
    }

    private void AddSuffixAdjectiveNominativePlural(StringBuilder outputString)
    {
      char ch = this.RemoveSuffixFromAdjective(outputString);
      string ending = this.GetEnding(outputString, 2);
      if (this.MasculinePersonal)
      {
        if (ch != 'y')
          return;
        this.PalatalizeConsonant(outputString, ending);
        if (this.GetLastCharacter(outputString) == 'l')
        {
          outputString.Append('i');
        }
        else
        {
          if (this.IsVowel(this.GetLastCharacter(outputString)))
            return;
          outputString.Append('y');
        }
      }
      else
        outputString.Append('e');
    }

    private void AddSuffixAdjectiveAccusative(StringBuilder outputString)
    {
      int num = (int) this.RemoveSuffixFromAdjective(outputString);
      if (this.Feminine)
        outputString.Append('ą');
      else if (this.Neuter)
        outputString.Append('e');
      else if (this.MasculineAnimate || this.MasculinePersonal)
      {
        outputString.Append("ego");
      }
      else
      {
        if (!this.MasculineInanimate)
          return;
        outputString.Append('y');
      }
    }

    private void AddSuffixAdjectiveAccusativePlural(StringBuilder outputString)
    {
      char ch = this.RemoveSuffixFromAdjective(outputString);
      if (this.MasculinePersonal)
      {
        if (ch == 'y')
          outputString.Append("y");
        outputString.Append("ch");
      }
      else
        outputString.Append('e');
    }

    private void AddSuffixAdjectiveVocative(StringBuilder outputString) => this.AddSuffixAdjectiveNominative(outputString);

    private void AddSuffixAdjectiveVocativePlural(StringBuilder outputString) => this.AddSuffixAdjectiveNominativePlural(outputString);

    private void AddSuffixAdjectiveGenitive(StringBuilder outputString)
    {
      int num = (int) this.RemoveSuffixFromAdjective(outputString);
      if (this.Feminine)
        outputString.Append("ej");
      else
        outputString.Append("ego");
    }

    private void AddSuffixAdjectiveGenitivePlural(StringBuilder outputString)
    {
      if ('y' == this.RemoveSuffixFromAdjective(outputString))
        outputString.Append("y");
      outputString.Append("ch");
    }

    private void AddSuffixAdjectiveDative(StringBuilder outputString)
    {
      int num = (int) this.RemoveSuffixFromAdjective(outputString);
      if (this.Feminine)
        outputString.Append("ej");
      else
        outputString.Append("emu");
    }

    private void AddSuffixAdjectiveDativePlural(StringBuilder outputString)
    {
      if ('y' == this.RemoveSuffixFromAdjective(outputString))
        outputString.Append("y");
      outputString.Append("m");
    }

    private void AddSuffixAdjectiveLocative(StringBuilder outputString)
    {
      char ch = this.RemoveSuffixFromAdjective(outputString);
      if (this.Feminine)
      {
        outputString.Append("ej");
      }
      else
      {
        if (ch == 'y')
          outputString.Append("y");
        outputString.Append("m");
      }
    }

    private void AddSuffixAdjectiveLocativePlural(StringBuilder outputString) => this.AddSuffixAdjectiveGenitivePlural(outputString);

    private void AddSuffixAdjectiveInstrumental(StringBuilder outputString)
    {
      char ch = this.RemoveSuffixFromAdjective(outputString);
      if (this.Feminine)
      {
        outputString.Append('ą');
      }
      else
      {
        if (ch == 'y')
          outputString.Append("y");
        outputString.Append("m");
      }
    }

    private void AddSuffixAdjectiveInstrumentalPlural(StringBuilder outputString)
    {
      if ('y' == this.RemoveSuffixFromAdjective(outputString))
        outputString.Append("y");
      outputString.Append("mi");
    }

    private char RemoveSuffixFromAdjective(StringBuilder outputString)
    {
      if (this.GetLastCharacter(outputString) == 'i')
        return 'i';
      outputString.Remove(outputString.Length - 1, 1);
      return this.GetLastCharacter(outputString) == 'i' ? 'i' : 'y';
    }

    private void SetFeminine() => this._curGender = PolishTextProcessor.WordGenderEnum.Feminine;

    private void SetNeuter() => this._curGender = PolishTextProcessor.WordGenderEnum.Neuter;

    private void SetMasculineAnimate() => this._curGender = PolishTextProcessor.WordGenderEnum.MasculineAnimate;

    private void SetMasculineInanimate() => this._curGender = PolishTextProcessor.WordGenderEnum.MasculineInanimate;

    private void SetMasculinePersonal() => this._curGender = PolishTextProcessor.WordGenderEnum.MasculinePersonal;

    private bool IsVowel(char c) => Array.IndexOf<char>(PolishTextProcessor._vowels, c) >= 0;

    private bool IsSoftConsonant(string s) => Array.IndexOf<char>(PolishTextProcessor._softConsonants, s[1]) >= 0;

    private bool IsHardenedConsonant(string s) => Array.IndexOf<string>(PolishTextProcessor._hardenedConsonants, s) >= 0 || Array.IndexOf<string>(PolishTextProcessor._hardenedConsonants, s[1].ToString()) >= 0;

    private enum WordGenderEnum
    {
      MasculinePersonal,
      MasculineAnimate,
      MasculineInanimate,
      Feminine,
      Neuter,
    }
  }
}
