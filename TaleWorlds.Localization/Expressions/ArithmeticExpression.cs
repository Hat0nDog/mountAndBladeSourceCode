// Decompiled with JetBrains decompiler
// Type: Expressions.ArithmeticExpression
// Assembly: TaleWorlds.Localization, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 26BB3E5A-EB48-4ABD-B2FC-10EF6D7A7285
// Assembly location: E:\SteamLibrary\steamapps\common\Mount & Blade II Bannerlord\bin\Win64_Shipping_Client\TaleWorlds.Localization.dll

using TaleWorlds.Localization;
using TaleWorlds.Localization.TextProcessor;

namespace Expressions
{
  internal class ArithmeticExpression : NumeralExpression
  {
    private readonly ArithmeticOperation _op;
    private readonly TextExpression _exp1;
    private readonly TextExpression _exp2;

    public ArithmeticExpression(ArithmeticOperation op, TextExpression exp1, TextExpression exp2)
    {
      this._op = op;
      this._exp1 = exp1;
      this._exp2 = exp2;
      this.RawValue = exp1.RawValue + (object) op + exp2.RawValue;
    }

    internal override int EvaluateNumber(TextProcessingContext context, TextObject parent)
    {
      switch (this._op)
      {
        case ArithmeticOperation.Add:
          return this.EvaluateAsNumber(this._exp1, context, parent) + this.EvaluateAsNumber(this._exp2, context, parent);
        case ArithmeticOperation.Subtract:
          return this.EvaluateAsNumber(this._exp1, context, parent) - this.EvaluateAsNumber(this._exp2, context, parent);
        case ArithmeticOperation.Multiply:
          return this.EvaluateAsNumber(this._exp1, context, parent) * this.EvaluateAsNumber(this._exp2, context, parent);
        case ArithmeticOperation.Divide:
          return this.EvaluateAsNumber(this._exp1, context, parent) / this.EvaluateAsNumber(this._exp2, context, parent);
        default:
          return 0;
      }
    }

    internal override string EvaluateString(TextProcessingContext context, TextObject parent) => this.EvaluateNumber(context, parent).ToString();

    internal override TokenType TokenType => this._op != ArithmeticOperation.Add && this._op != ArithmeticOperation.Subtract ? TokenType.ArithmeticProduct : TokenType.ArithmeticSum;
  }
}
