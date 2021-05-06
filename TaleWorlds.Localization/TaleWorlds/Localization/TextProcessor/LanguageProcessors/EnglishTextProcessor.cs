// Decompiled with JetBrains decompiler
// Type: TaleWorlds.Localization.TextProcessor.LanguageProcessors.EnglishTextProcessor
// Assembly: TaleWorlds.Localization, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 26BB3E5A-EB48-4ABD-B2FC-10EF6D7A7285
// Assembly location: E:\SteamLibrary\steamapps\common\Mount & Blade II Bannerlord\bin\Win64_Shipping_Client\TaleWorlds.Localization.dll

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;

namespace TaleWorlds.Localization.TextProcessor.LanguageProcessors
{
  public class EnglishTextProcessor : LanguageSpecificTextProcessor
  {
    private Dictionary<string, string> IrregularNouns = new Dictionary<string, string>()
    {
      {
        "man",
        "men"
      },
      {
        "footman",
        "footmen"
      },
      {
        "crossbowman",
        "crossbowmen"
      },
      {
        "pikeman",
        "pikemen"
      },
      {
        "shieldman",
        "shieldmen"
      },
      {
        "shieldsman",
        "shieldsmen"
      },
      {
        "woman",
        "women"
      },
      {
        "child",
        "children"
      },
      {
        "mouse",
        "mice"
      },
      {
        "louse",
        "lice"
      },
      {
        "tooth",
        "teeth"
      },
      {
        "goose",
        "geese"
      },
      {
        "foot",
        "feet"
      },
      {
        "ox",
        "oxen"
      },
      {
        "sheep",
        "sheep"
      },
      {
        "fish",
        "fish"
      },
      {
        "species",
        "species"
      },
      {
        "aircraft",
        "aircraft"
      },
      {
        "news",
        "news"
      },
      {
        "advice",
        "advice"
      },
      {
        "information",
        "information"
      },
      {
        "luggage",
        "luggage"
      },
      {
        "athletics",
        "athletics"
      },
      {
        "linguistics",
        "linguistics"
      },
      {
        "curriculum",
        "curricula"
      },
      {
        "analysis",
        "analyses"
      },
      {
        "ellipsis",
        "ellipses"
      },
      {
        "bison",
        "bison"
      },
      {
        "corpus",
        "corpora"
      },
      {
        "crisis",
        "crises"
      },
      {
        "criterion",
        "criteria"
      },
      {
        "die",
        "dice"
      },
      {
        "graffito",
        "graffiti"
      },
      {
        "cactus",
        "cacti"
      },
      {
        "focus",
        "foci"
      },
      {
        "fungus",
        "fungi"
      },
      {
        "headquarters",
        "headquarters"
      },
      {
        "trousers",
        "trousers"
      },
      {
        "cattle",
        "cattle"
      },
      {
        "scissors",
        "scissors"
      },
      {
        "index",
        "indices"
      },
      {
        "vertex",
        "vertices"
      },
      {
        "matrix",
        "matrices"
      },
      {
        "radius",
        "radii"
      },
      {
        "photo",
        "photos"
      },
      {
        "piano",
        "pianos"
      },
      {
        "dwarf",
        "dwarves"
      },
      {
        "wharf",
        "wharves"
      },
      {
        "formula",
        "formulae"
      },
      {
        "moose",
        "moose"
      },
      {
        "phenomenon",
        "phenomena"
      }
    };
    private string[] Sibilants = new string[6]
    {
      "s",
      "x",
      "ch",
      "sh",
      "es",
      "ss"
    };
    private const string Vowels = "aeiouAEIOU";
    private const string Consonants = "bcdfghjklmnpqrstvwxyzBCDFGHJKLMNPQRSTVWXYZ";

    public override void ProcessToken(
      string sourceText,
      ref int cursorPos,
      string token,
      StringBuilder outputString)
    {
      char ch = token[1];
      switch (ch)
      {
        case 'A':
          if (this.CheckNextCharIsVowel(sourceText, cursorPos))
          {
            outputString.Append("An");
            break;
          }
          outputString.Append("A");
          break;
        case 'a':
          if (this.CheckNextCharIsVowel(sourceText, cursorPos))
          {
            outputString.Append("an");
            break;
          }
          outputString.Append("a");
          break;
        case 's':
          string source = "";
          int startIndex = 0;
          for (int index = outputString.Length - 1; index >= 0; --index)
          {
            if (outputString[index] == ' ')
            {
              startIndex = index + 1;
              break;
            }
            source += outputString[index].ToString();
          }
          string str = new string(source.Reverse<char>().ToArray<char>());
          int length = str.Length;
          if (str.Length <= 1)
            break;
          string resultPlural;
          if (this.HandleIrregularNouns(str, out resultPlural))
          {
            outputString.Replace(str, resultPlural, startIndex, length);
            break;
          }
          if (this.Handle_ves_Suffix(str, out resultPlural))
          {
            outputString.Replace(str, resultPlural, startIndex, length);
            break;
          }
          if (this.Handle_ies_Suffix(str, out resultPlural))
          {
            outputString.Replace(str, resultPlural, startIndex, length);
            break;
          }
          if (this.Handle_es_Suffix(str, out resultPlural))
          {
            outputString.Replace(str, resultPlural, startIndex, length);
            break;
          }
          if (this.Handle_s_Suffix(str, out resultPlural))
          {
            outputString.Replace(str, resultPlural, startIndex, length);
            break;
          }
          outputString.Append(ch);
          break;
      }
    }

    private bool CheckNextCharIsVowel(string sourceText, int cursorPos)
    {
      for (; cursorPos < sourceText.Length; ++cursorPos)
      {
        char ch = sourceText[cursorPos];
        if ("aeiouAEIOU".Contains<char>(ch))
          return true;
        if ("bcdfghjklmnpqrstvwxyzBCDFGHJKLMNPQRSTVWXYZ".Contains<char>(ch))
          return false;
      }
      return false;
    }

    private bool HandleIrregularNouns(string text, out string resultPlural)
    {
      resultPlural = (string) null;
      char.IsLower(text[text.Length - 1]);
      string str;
      if (!this.IrregularNouns.TryGetValue(text.ToLower(), out str))
        return false;
      if (text.All<char>((Func<char, bool>) (c => char.IsUpper(c))))
        resultPlural = str.ToUpper();
      else if (char.IsUpper(text[0]))
      {
        char[] charArray = str.ToCharArray();
        charArray[0] = char.ToUpper(charArray[0]);
        resultPlural = new string(charArray);
      }
      else
        resultPlural = str.ToLower();
      return true;
    }

    private bool Handle_ves_Suffix(string text, out string resultPlural)
    {
      resultPlural = (string) null;
      bool flag = char.IsLower(text[text.Length - 1]);
      char lower1 = char.ToLower(text[text.Length - 1]);
      char lower2 = char.ToLower(text[text.Length - 2]);
      if (lower2 != 'o' && "aeiouAEIOU".Contains<char>(lower2) && lower1 == 'f')
      {
        resultPlural = text.Remove(text.Length - 1);
        resultPlural += flag ? "ves" : "VES";
        return true;
      }
      if (lower2 == 'f' && "aeiouAEIOU".Contains<char>(lower1))
      {
        resultPlural = text.Remove(text.Length - 2, 2);
        resultPlural += flag ? "v" : "V";
        resultPlural += (flag ? lower1 : char.ToUpper(lower1)).ToString();
        resultPlural += flag ? "s" : "S";
        return true;
      }
      if (lower2 != 'l' || lower1 != 'f')
        return false;
      resultPlural = text.Remove(text.Length - 1);
      resultPlural += flag ? "ves" : "VES";
      return true;
    }

    private bool Handle_ies_Suffix(string text, out string resultPlural)
    {
      resultPlural = (string) null;
      bool flag = char.IsLower(text[text.Length - 1]);
      char lower = char.ToLower(text[text.Length - 1]);
      if (!"bcdfghjklmnpqrstvwxyzBCDFGHJKLMNPQRSTVWXYZ".Contains<char>(char.ToLower(text[text.Length - 2])) || lower != 'y')
        return false;
      resultPlural = text.Remove(text.Length - 1);
      resultPlural += flag ? "ies" : "IES";
      return true;
    }

    private bool Handle_es_Suffix(string text, out string resultPlural)
    {
      resultPlural = (string) null;
      bool flag = char.IsLower(text[text.Length - 1]);
      string str1 = text[text.Length - 1].ToString();
      string str2 = text[text.Length - 2].ToString();
      if (str1 == "z")
      {
        resultPlural = text;
        resultPlural += flag ? "zes" : "ZES";
        return true;
      }
      if (((IEnumerable<string>) this.Sibilants).Contains<string>(str1))
      {
        resultPlural = text;
        resultPlural += flag ? "es" : "ES";
        return true;
      }
      if (((IEnumerable<string>) this.Sibilants).Contains<string>(str2 + str1))
      {
        resultPlural = text;
        resultPlural += flag ? "es" : "ES";
        return true;
      }
      if ("bcdfghjklmnpqrstvwxyzBCDFGHJKLMNPQRSTVWXYZ".Contains(str2) && str1 == "o")
      {
        resultPlural = text;
        resultPlural += flag ? "es" : "ES";
        return true;
      }
      if (str2 == "o" && str1 == "e")
      {
        resultPlural = text;
        resultPlural = resultPlural.Remove(resultPlural.Length - 1);
        resultPlural += flag ? "es" : "ES";
        return true;
      }
      if (!(str2 == "i") || !(str1 == "s"))
        return false;
      resultPlural = text;
      resultPlural = resultPlural.Remove(resultPlural.Length - 2);
      resultPlural += flag ? "es" : "ES";
      return true;
    }

    private bool Handle_s_Suffix(string text, out string resultPlural)
    {
      resultPlural = (string) null;
      bool flag = char.IsLower(text[text.Length - 1]);
      char lower1 = char.ToLower(text[text.Length - 1]);
      char lower2 = char.ToLower(text[text.Length - 2]);
      if ("bcdfghjklmnpqrstvwxyzBCDFGHJKLMNPQRSTVWXYZ".Contains<char>(lower1))
      {
        resultPlural = text;
        resultPlural += flag ? "s" : "S";
        return true;
      }
      if (lower1 == 'e')
      {
        resultPlural = text;
        resultPlural += flag ? "s" : "S";
        return true;
      }
      if ("aeiouAEIOU".Contains<char>(lower2) && lower1 == 'y')
      {
        resultPlural = text;
        resultPlural += flag ? "s" : "S";
        return true;
      }
      if (lower2 == 'f' && lower1 == 'f')
      {
        resultPlural = text;
        resultPlural += flag ? "s" : "S";
        return true;
      }
      if (lower2 == 'o' && lower1 == 'f')
      {
        resultPlural = text;
        resultPlural += flag ? "s" : "S";
        return true;
      }
      if (!"aeiouAEIOU".Contains<char>(lower2) || lower1 != 'o')
        return false;
      resultPlural = text;
      resultPlural += flag ? "s" : "S";
      return true;
    }

    public override CultureInfo CultureInfoForLanguage => CultureInfo.InvariantCulture;
  }
}
