// Decompiled with JetBrains decompiler
// Type: Expressions.SimpleToken
// Assembly: TaleWorlds.Localization, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 26BB3E5A-EB48-4ABD-B2FC-10EF6D7A7285
// Assembly location: E:\SteamLibrary\steamapps\common\Mount & Blade II Bannerlord\bin\Win64_Shipping_Client\TaleWorlds.Localization.dll

using TaleWorlds.Localization;
using TaleWorlds.Localization.TextProcessor;

namespace Expressions
{
  internal class SimpleToken : TextExpression
  {
    public static readonly SimpleToken SequenceTerminator = new SimpleToken(TokenType.SequenceTerminator, ".");
    private readonly TokenType _tokenType;

    public SimpleToken(TokenType tokenType, string value)
    {
      this.RawValue = value;
      this._tokenType = tokenType;
    }

    internal override string EvaluateString(TextProcessingContext context, TextObject parent)
    {
      switch (this.TokenType)
      {
        case TokenType.FunctionParam:
          return context.GetFunctionParam(this.RawValue).ToString();
        case TokenType.ParameterWithMarkerOccurance:
          return context.GetParameterWithMarkerOccurance(this.RawValue, parent).ToString();
        default:
          return this.RawValue;
      }
    }

    internal override TokenType TokenType => this._tokenType;
  }
}
