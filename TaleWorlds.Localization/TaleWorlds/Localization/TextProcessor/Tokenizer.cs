// Decompiled with JetBrains decompiler
// Type: TaleWorlds.Localization.TextProcessor.Tokenizer
// Assembly: TaleWorlds.Localization, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 26BB3E5A-EB48-4ABD-B2FC-10EF6D7A7285
// Assembly location: E:\SteamLibrary\steamapps\common\Mount & Blade II Bannerlord\bin\Win64_Shipping_Client\TaleWorlds.Localization.dll

using System.Collections.Generic;
using TaleWorlds.Library;

namespace TaleWorlds.Localization.TextProcessor
{
  internal class Tokenizer
  {
    private List<TokenDefinition> _tokenDefinitions;

    public Tokenizer()
    {
      this._tokenDefinitions = new List<TokenDefinition>();
      this._tokenDefinitions.Add(new TokenDefinition(TokenType.ConditionSeperator, "{\\?}", 1));
      this._tokenDefinitions.Add(new TokenDefinition(TokenType.Seperator, "{\\:}", 1));
      this._tokenDefinitions.Add(new TokenDefinition(TokenType.ConditionFinalizer, "{\\\\\\?}", 1));
      this._tokenDefinitions.Add(new TokenDefinition(TokenType.ConditionFollowUp, "{\\:\\?", 1));
      this._tokenDefinitions.Add(new TokenDefinition(TokenType.ConditionStarter, "{\\?", 1));
      this._tokenDefinitions.Add(new TokenDefinition(TokenType.SelectionSeperator, "{#}", 1));
      this._tokenDefinitions.Add(new TokenDefinition(TokenType.SelectionFinalizer, "{\\\\#}", 1));
      this._tokenDefinitions.Add(new TokenDefinition(TokenType.SelectionStarter, "{#", 1));
      this._tokenDefinitions.Add(new TokenDefinition(TokenType.FieldStarter, "{@", 1));
      this._tokenDefinitions.Add(new TokenDefinition(TokenType.FieldFinalizer, "{\\\\@}", 1));
      this._tokenDefinitions.Add(new TokenDefinition(TokenType.LanguageMarker, "{\\.[a-zA-Z_^][a-zA-Z\\d_]*}", 1));
      this._tokenDefinitions.Add(new TokenDefinition(TokenType.textId, "{=[a-zA-Z\\d_\\!\\*][a-zA-Z\\d_\\.]*}", 1));
      this._tokenDefinitions.Add(new TokenDefinition(TokenType.CloseBraces, "}", 1));
      this._tokenDefinitions.Add(new TokenDefinition(TokenType.OpenBraces, "{", 1));
      this._tokenDefinitions.Add(new TokenDefinition(TokenType.Multiply, "\\*", 1));
      this._tokenDefinitions.Add(new TokenDefinition(TokenType.Divide, "\\/", 1));
      this._tokenDefinitions.Add(new TokenDefinition(TokenType.Plus, "\\+", 1));
      this._tokenDefinitions.Add(new TokenDefinition(TokenType.Minus, "\\-", 1));
      this._tokenDefinitions.Add(new TokenDefinition(TokenType.Comma, ",", 1));
      this._tokenDefinitions.Add(new TokenDefinition(TokenType.CloseBrackets, "\\]", 1));
      this._tokenDefinitions.Add(new TokenDefinition(TokenType.OpenBrackets, "\\[", 1));
      this._tokenDefinitions.Add(new TokenDefinition(TokenType.CloseParenthesis, "\\)", 1));
      this._tokenDefinitions.Add(new TokenDefinition(TokenType.OpenParenthesis, "\\(", 1));
      this._tokenDefinitions.Add(new TokenDefinition(TokenType.ParameterWithMarkerOccurance, "\\$\\d+\\!.[a-zA-Z_][a-zA-Z\\d_]*", 1));
      this._tokenDefinitions.Add(new TokenDefinition(TokenType.ParameterWithAttribute, "\\$\\d+\\.[a-zA-Z_][a-zA-Z\\d_]*", 1));
      this._tokenDefinitions.Add(new TokenDefinition(TokenType.FunctionParam, "\\$\\d+", 2));
      this._tokenDefinitions.Add(new TokenDefinition(TokenType.Number, "\\d+", 2));
      this._tokenDefinitions.Add(new TokenDefinition(TokenType.Match, "match", 1));
      this._tokenDefinitions.Add(new TokenDefinition(TokenType.QualifiedIdentifier, "[a-zA-Z_][a-zA-Z\\d_]*\\.[a-zA-Z_][a-zA-Z\\d_]*", 1));
      this._tokenDefinitions.Add(new TokenDefinition(TokenType.FunctionIdentifier, "[a-zA-Z_][a-zA-Z\\d_]*\\(", 1));
      this._tokenDefinitions.Add(new TokenDefinition(TokenType.Identifier, "[a-zA-Z_][a-zA-Z\\d_]*", 1));
      this._tokenDefinitions.Add(new TokenDefinition(TokenType.MarkerOccuranceIdentifier, "\\!.[a-zA-Z_][a-zA-Z\\d_]*", 1));
      this._tokenDefinitions.Add(new TokenDefinition(TokenType.And, "and", 1));
      this._tokenDefinitions.Add(new TokenDefinition(TokenType.Or, "or", 1));
      this._tokenDefinitions.Add(new TokenDefinition(TokenType.Or, "not", 1));
      this._tokenDefinitions.Add(new TokenDefinition(TokenType.Equals, "==", 1));
      this._tokenDefinitions.Add(new TokenDefinition(TokenType.NotEquals, "!=", 1));
      this._tokenDefinitions.Add(new TokenDefinition(TokenType.GreaterOrEqual, ">=", 1));
      this._tokenDefinitions.Add(new TokenDefinition(TokenType.LessOrEqual, "<=", 1));
      this._tokenDefinitions.Add(new TokenDefinition(TokenType.GreaterThan, ">", 1));
      this._tokenDefinitions.Add(new TokenDefinition(TokenType.LessThan, "<", 1));
    }

    public List<MBTextToken> Tokenize(string text)
    {
      List<TokenMatch> tokenMatchesAndText = this.FindTokenMatchesAndText(text);
      List<MBTextToken> mbTextTokenList = new List<MBTextToken>(tokenMatchesAndText.Count + 1);
      foreach (TokenMatch tokenMatch in tokenMatchesAndText)
        mbTextTokenList.Add(new MBTextToken(tokenMatch.TokenType, tokenMatch.Value));
      mbTextTokenList.Add(new MBTextToken(TokenType.SequenceTerminator));
      return mbTextTokenList;
    }

    private List<TokenMatch> FindTokenMatchesAndText(string text)
    {
      List<TokenMatch> tokenMatches = new List<TokenMatch>();
      MBStringBuilder mbStringBuilder = new MBStringBuilder();
      mbStringBuilder.Initialize(callerMemberName: nameof (FindTokenMatchesAndText));
      int num = 0;
      while (num < text.Length)
      {
        if (text[num] == '{')
        {
          if (mbStringBuilder.Length > 0)
          {
            tokenMatches.Add(new TokenMatch(TokenType.Text, mbStringBuilder.ToStringAndRelease(), 0, num, 1));
            mbStringBuilder.Initialize(callerMemberName: nameof (FindTokenMatchesAndText));
          }
          int expressionEnd = this.FindExpressionEnd(text, num + 1);
          if (!this.FindTokenMatches(text, num, expressionEnd, tokenMatches))
          {
            tokenMatches.Clear();
            tokenMatches.Add(new TokenMatch(TokenType.Text, mbStringBuilder.ToStringAndRelease(), 0, text.Length, 1));
            return tokenMatches;
          }
          num = expressionEnd;
        }
        else
        {
          mbStringBuilder.Append(text[num]);
          ++num;
        }
      }
      string stringAndRelease = mbStringBuilder.ToStringAndRelease();
      if (stringAndRelease.Length > 0)
        tokenMatches.Add(new TokenMatch(TokenType.Text, stringAndRelease, 0, num, 1));
      return tokenMatches;
    }

    private int FindExpressionEnd(string text, int startIndex)
    {
      int index1 = startIndex;
      for (int index2 = 1; index1 < text.Length && index2 > 0; ++index1)
      {
        switch (text[index1])
        {
          case '{':
            ++index2;
            break;
          case '}':
            --index2;
            break;
        }
      }
      return index1;
    }

    private bool FindTokenMatches(
      string text,
      int beginIndex,
      int endIndex,
      List<TokenMatch> tokenMatches)
    {
      int beginIndex1 = beginIndex;
      while (beginIndex1 < endIndex)
      {
        bool flag = false;
        foreach (TokenDefinition tokenDefinition in this._tokenDefinitions)
        {
          TokenMatch tokenMatch = tokenDefinition.CheckMatch(text, beginIndex1);
          if (tokenMatch != null && tokenMatch.EndIndex != beginIndex1)
          {
            tokenMatches.Add(tokenMatch);
            beginIndex1 = tokenMatch.EndIndex;
            flag = true;
            break;
          }
        }
        if (!flag)
          return false;
      }
      return true;
    }
  }
}
