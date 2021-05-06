// Decompiled with JetBrains decompiler
// Type: TaleWorlds.Localization.TextProcessor.TokenDefinition
// Assembly: TaleWorlds.Localization, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 26BB3E5A-EB48-4ABD-B2FC-10EF6D7A7285
// Assembly location: E:\SteamLibrary\steamapps\common\Mount & Blade II Bannerlord\bin\Win64_Shipping_Client\TaleWorlds.Localization.dll

using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace TaleWorlds.Localization.TextProcessor
{
  internal class TokenDefinition
  {
    private Regex _regex;
    private readonly TokenType _returnsToken;
    private readonly int _precedence;

    public TokenDefinition(TokenType returnsToken, string regexPattern, int precedence)
    {
      this._regex = new Regex(regexPattern, RegexOptions.IgnoreCase | RegexOptions.Compiled);
      this._returnsToken = returnsToken;
      this._precedence = precedence;
    }

    internal TokenMatch CheckMatch(string str, int beginIndex)
    {
      beginIndex = this.SkipWhiteSpace(str, beginIndex);
      if (this._regex.IsMatch(str, beginIndex))
      {
        Match match = this._regex.Match(str, beginIndex);
        if (match != null && match.Index == beginIndex)
          return new TokenMatch(this._returnsToken, match.Value, match.Index, match.Index + match.Length, this._precedence);
      }
      return (TokenMatch) null;
    }

    private int SkipWhiteSpace(string str, int beginIndex)
    {
      int index = beginIndex;
      while (index < str.Length && char.IsWhiteSpace(str[index]))
        ++index;
      return index;
    }

    internal IEnumerable<TokenMatch> FindMatches(
      string inputString,
      int beginIndex,
      int endIndex)
    {
      string input = inputString;
      if (beginIndex != 0 || endIndex != inputString.Length)
      {
        if (endIndex == -1)
          endIndex = inputString.Length;
        input = inputString.Substring(beginIndex, endIndex - beginIndex);
      }
      MatchCollection matches = this._regex.Matches(input, beginIndex);
      for (int i = 0; i < matches.Count; ++i)
        yield return new TokenMatch(this._returnsToken, matches[i].Value, matches[i].Index + beginIndex, beginIndex + matches[i].Index + matches[i].Length, this._precedence);
    }
  }
}
