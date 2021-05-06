// Decompiled with JetBrains decompiler
// Type: Expressions.SimpleNumberExpression
// Assembly: TaleWorlds.Localization, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 26BB3E5A-EB48-4ABD-B2FC-10EF6D7A7285
// Assembly location: E:\SteamLibrary\steamapps\common\Mount & Blade II Bannerlord\bin\Win64_Shipping_Client\TaleWorlds.Localization.dll

using TaleWorlds.Localization;
using TaleWorlds.Localization.TextProcessor;

namespace Expressions
{
  internal class SimpleNumberExpression : TextExpression
  {
    public SimpleNumberExpression(string value) => this.RawValue = value;

    internal override TokenType TokenType => TokenType.Number;

    internal override string EvaluateString(TextProcessingContext context, TextObject parent) => this.RawValue;
  }
}
