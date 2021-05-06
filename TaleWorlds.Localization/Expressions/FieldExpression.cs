// Decompiled with JetBrains decompiler
// Type: Expressions.FieldExpression
// Assembly: TaleWorlds.Localization, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 26BB3E5A-EB48-4ABD-B2FC-10EF6D7A7285
// Assembly location: E:\SteamLibrary\steamapps\common\Mount & Blade II Bannerlord\bin\Win64_Shipping_Client\TaleWorlds.Localization.dll

using TaleWorlds.Localization;
using TaleWorlds.Localization.TextProcessor;

namespace Expressions
{
  internal class FieldExpression : TextExpression
  {
    private TextExpression _innerExpression;
    private TextExpression part2;

    public string FieldName => this.RawValue;

    public TextExpression InnerExpression => this.part2;

    public FieldExpression(TextExpression innerExpression)
    {
      this._innerExpression = innerExpression;
      this.RawValue = innerExpression.RawValue;
    }

    public FieldExpression(TextExpression innerExpression, TextExpression part2)
      : this(innerExpression)
    {
      this.part2 = part2;
    }

    internal override string EvaluateString(TextProcessingContext context, TextObject parent) => "";

    internal override TokenType TokenType => TokenType.FieldExpression;
  }
}
