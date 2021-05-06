// Decompiled with JetBrains decompiler
// Type: Expressions.TextExpression
// Assembly: TaleWorlds.Localization, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 26BB3E5A-EB48-4ABD-B2FC-10EF6D7A7285
// Assembly location: E:\SteamLibrary\steamapps\common\Mount & Blade II Bannerlord\bin\Win64_Shipping_Client\TaleWorlds.Localization.dll

using TaleWorlds.Localization;
using TaleWorlds.Localization.TextProcessor;

namespace Expressions
{
  internal abstract class TextExpression
  {
    internal abstract string EvaluateString(TextProcessingContext context, TextObject parent);

    internal abstract TokenType TokenType { get; }

    internal string RawValue { get; set; }

    internal int EvaluateAsNumber(
      TextExpression exp,
      TextProcessingContext context,
      TextObject parent)
    {
      if (exp is NumeralExpression numeralExpression)
        return numeralExpression.EvaluateNumber(context, parent);
      string s = exp.EvaluateString(context, parent);
      int num = 0;
      ref int local = ref num;
      if (int.TryParse(s, out local))
        return num;
      return exp.RawValue.Length != 0 ? 1 : 0;
    }
  }
}
