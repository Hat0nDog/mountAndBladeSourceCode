// Decompiled with JetBrains decompiler
// Type: Expressions.MarkerOccuranceTextExpression
// Assembly: TaleWorlds.Localization, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 26BB3E5A-EB48-4ABD-B2FC-10EF6D7A7285
// Assembly location: E:\SteamLibrary\steamapps\common\Mount & Blade II Bannerlord\bin\Win64_Shipping_Client\TaleWorlds.Localization.dll

using System;
using System.Collections.Generic;
using TaleWorlds.Library;
using TaleWorlds.Localization;
using TaleWorlds.Localization.TextProcessor;

namespace Expressions
{
  internal class MarkerOccuranceTextExpression : TextExpression
  {
    private VariableExpression _innerVariable;
    private string _identifierName;

    public string IdentifierName => this._identifierName;

    public MarkerOccuranceTextExpression(string identifierName, VariableExpression innerExpression)
    {
      this.RawValue = identifierName;
      this._identifierName = identifierName;
      this._innerVariable = innerExpression;
    }

    private string MarkerOccuranceExpression(string identifierName, string text)
    {
      int i = 0;
      int num1 = 0;
      int num2 = 0;
      MBStringBuilder mbStringBuilder = new MBStringBuilder();
      mbStringBuilder.Initialize(callerMemberName: nameof (MarkerOccuranceExpression));
      for (; i < text.Length; ++i)
      {
        if (text[i] != '{')
        {
          if (num1 == 1 && num2 == 0)
            mbStringBuilder.Append(text[i]);
        }
        else
        {
          string token = TextProcessingContext.ReadFirstToken(text, ref i);
          if (TextProcessingContext.IsDeclarationFinalizer(token))
          {
            --num1;
            if (num2 > num1)
              num2 = num1;
          }
          else if (TextProcessingContext.IsDeclaration(token))
          {
            string strB = token.Substring(1);
            int num3 = num2 != num1 ? 0 : (string.Compare(identifierName, strB, StringComparison.InvariantCultureIgnoreCase) == 0 ? 1 : 0);
            ++num1;
            if (num3 != 0)
              num2 = num1;
          }
        }
      }
      return mbStringBuilder.ToStringAndRelease();
    }

    internal override string EvaluateString(TextProcessingContext context, TextObject parent)
    {
      MultiStatement multiStatement = this._innerVariable.GetValue(context, parent);
      if (multiStatement != null)
      {
        foreach (TextExpression subStatement in (IEnumerable<TextExpression>) multiStatement.SubStatements)
        {
          if (subStatement.TokenType == TokenType.LanguageMarker && subStatement.RawValue.Substring(2, subStatement.RawValue.Length - 3) == this.IdentifierName)
            return "1";
        }
      }
      return "0";
    }

    internal override TokenType TokenType => TokenType.MarkerOccuranceExpression;
  }
}
