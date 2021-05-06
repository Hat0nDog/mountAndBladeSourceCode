// Decompiled with JetBrains decompiler
// Type: Expressions.VariableExpression
// Assembly: TaleWorlds.Localization, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 26BB3E5A-EB48-4ABD-B2FC-10EF6D7A7285
// Assembly location: E:\SteamLibrary\steamapps\common\Mount & Blade II Bannerlord\bin\Win64_Shipping_Client\TaleWorlds.Localization.dll

using System.Collections.Generic;
using TaleWorlds.Library;
using TaleWorlds.Localization;
using TaleWorlds.Localization.TextProcessor;

namespace Expressions
{
  internal class VariableExpression : TextExpression
  {
    private VariableExpression _innerVariable;
    private string _identifierName;

    public string IdentifierName => this._identifierName;

    public VariableExpression(string identifierName, VariableExpression innerExpression)
    {
      this.RawValue = identifierName;
      this._identifierName = identifierName;
      this._innerVariable = innerExpression;
    }

    internal MultiStatement GetValue(TextProcessingContext context, TextObject parent)
    {
      if (this._innerVariable == null)
        return context.GetVariableValue(this._identifierName, parent);
      MultiStatement multiStatement = this._innerVariable.GetValue(context, parent);
      switch (multiStatement)
      {
        case null:
        case null:
          return (MultiStatement) null;
        default:
          using (IEnumerator<TextExpression> enumerator = multiStatement.SubStatements.GetEnumerator())
          {
            while (enumerator.MoveNext())
            {
              if (enumerator.Current is FieldExpression current4 && current4.FieldName == this._identifierName)
              {
                if (current4.InnerExpression is MultiStatement)
                  return current4.InnerExpression as MultiStatement;
                return new MultiStatement((IEnumerable<TextExpression>) new TextExpression[1]
                {
                  current4.InnerExpression
                });
              }
            }
            goto case null;
          }
      }
    }

    internal override string EvaluateString(TextProcessingContext context, TextObject parent)
    {
      MultiStatement multiStatement = this.GetValue(context, parent);
      if (multiStatement == null)
        return "";
      MBStringBuilder mbStringBuilder = new MBStringBuilder();
      mbStringBuilder.Initialize(callerMemberName: nameof (EvaluateString));
      foreach (TextExpression subStatement in (IEnumerable<TextExpression>) multiStatement.SubStatements)
      {
        if (subStatement != null)
          mbStringBuilder.Append<string>(subStatement.EvaluateString(context, parent));
      }
      return mbStringBuilder.ToStringAndRelease();
    }

    internal override TokenType TokenType => TokenType.Identifier;
  }
}
