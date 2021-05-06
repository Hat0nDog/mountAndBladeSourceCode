// Decompiled with JetBrains decompiler
// Type: Expressions.ComparisonExpression
// Assembly: TaleWorlds.Localization, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 26BB3E5A-EB48-4ABD-B2FC-10EF6D7A7285
// Assembly location: E:\SteamLibrary\steamapps\common\Mount & Blade II Bannerlord\bin\Win64_Shipping_Client\TaleWorlds.Localization.dll

using TaleWorlds.Localization;
using TaleWorlds.Localization.TextProcessor;

namespace Expressions
{
  internal class ComparisonExpression : NumeralExpression
  {
    private readonly ComparisonOperation _op;
    private readonly TextExpression _exp1;
    private readonly TextExpression _exp2;

    public ComparisonExpression(ComparisonOperation op, TextExpression exp1, TextExpression exp2)
    {
      this._op = op;
      this._exp1 = exp1;
      this._exp2 = exp2;
      this.RawValue = exp1.RawValue + (object) op + exp2.RawValue;
    }

    internal bool EvaluateBoolean(TextProcessingContext context, TextObject parent)
    {
      switch (this._op)
      {
        case ComparisonOperation.Equals:
          return this.EvaluateAsNumber(this._exp1, context, parent) == this.EvaluateAsNumber(this._exp2, context, parent);
        case ComparisonOperation.NotEquals:
          return this.EvaluateAsNumber(this._exp1, context, parent) != this.EvaluateAsNumber(this._exp2, context, parent);
        case ComparisonOperation.GreaterThan:
          return this.EvaluateAsNumber(this._exp1, context, parent) > this.EvaluateAsNumber(this._exp2, context, parent);
        case ComparisonOperation.GreaterOrEqual:
          return this.EvaluateAsNumber(this._exp1, context, parent) >= this.EvaluateAsNumber(this._exp2, context, parent);
        case ComparisonOperation.LessThan:
          return this.EvaluateAsNumber(this._exp1, context, parent) < this.EvaluateAsNumber(this._exp2, context, parent);
        case ComparisonOperation.LessOrEqual:
          return this.EvaluateAsNumber(this._exp1, context, parent) <= this.EvaluateAsNumber(this._exp2, context, parent);
        default:
          return false;
      }
    }

    internal override TokenType TokenType => TokenType.ComparisonExpression;

    internal override int EvaluateNumber(TextProcessingContext context, TextObject parent) => !this.EvaluateBoolean(context, parent) ? 0 : 1;

    internal override string EvaluateString(TextProcessingContext context, TextObject parent) => this.EvaluateNumber(context, parent).ToString();
  }
}
