// Decompiled with JetBrains decompiler
// Type: Expressions.ConditionExpression
// Assembly: TaleWorlds.Localization, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 26BB3E5A-EB48-4ABD-B2FC-10EF6D7A7285
// Assembly location: E:\SteamLibrary\steamapps\common\Mount & Blade II Bannerlord\bin\Win64_Shipping_Client\TaleWorlds.Localization.dll

using System.Collections.Generic;
using TaleWorlds.Library;
using TaleWorlds.Localization;
using TaleWorlds.Localization.TextProcessor;

namespace Expressions
{
  internal class ConditionExpression : TextExpression
  {
    private TextExpression[] _conditionExpressions;
    private TextExpression[] _resultExpressions;

    public ConditionExpression(
      TextExpression condition,
      TextExpression part1,
      TextExpression part2)
    {
      this._conditionExpressions = new TextExpression[1]
      {
        condition
      };
      this._resultExpressions = new TextExpression[2]
      {
        part1,
        part2
      };
    }

    public ConditionExpression(
      List<TextExpression> conditionExpressions,
      List<TextExpression> resultExpressions2)
    {
      this._conditionExpressions = conditionExpressions.ToArray();
      this._resultExpressions = resultExpressions2.ToArray();
    }

    internal override string EvaluateString(TextProcessingContext context, TextObject parent)
    {
      bool flag = false;
      int index = 0;
      TextExpression textExpression = (TextExpression) null;
      while (!flag && index < this._conditionExpressions.Length)
      {
        TextExpression conditionExpression = this._conditionExpressions[index];
        string str = conditionExpression.EvaluateString(context, parent);
        if (str.Length != 0)
          flag = conditionExpression.TokenType != TokenType.ParameterWithAttribute ? (uint) this.EvaluateAsNumber(conditionExpression, context, parent) > 0U : !str.IsStringNoneOrEmpty();
        if (flag)
        {
          if (index < this._resultExpressions.Length)
            textExpression = this._resultExpressions[index];
        }
        else
          ++index;
      }
      if (textExpression == null && index < this._resultExpressions.Length)
        textExpression = this._resultExpressions[index];
      return textExpression?.EvaluateString(context, parent) ?? "";
    }

    internal override TokenType TokenType => TokenType.ConditionalExpression;
  }
}
