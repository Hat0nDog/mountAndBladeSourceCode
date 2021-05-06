// Decompiled with JetBrains decompiler
// Type: Expressions.FunctionCall
// Assembly: TaleWorlds.Localization, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 26BB3E5A-EB48-4ABD-B2FC-10EF6D7A7285
// Assembly location: E:\SteamLibrary\steamapps\common\Mount & Blade II Bannerlord\bin\Win64_Shipping_Client\TaleWorlds.Localization.dll

using System.Collections.Generic;
using System.Linq;
using TaleWorlds.Localization;
using TaleWorlds.Localization.TextProcessor;

namespace Expressions
{
  internal class FunctionCall : TextExpression
  {
    private string _functionName;
    private List<TextExpression> _functionParams;

    public FunctionCall(string functionName, IEnumerable<TextExpression> functionParams)
    {
      this._functionName = functionName;
      this._functionParams = functionParams.ToList<TextExpression>();
      this.RawValue = this._functionName;
    }

    internal override string EvaluateString(TextProcessingContext context, TextObject parent) => context.CallFunction(this._functionName, this._functionParams, parent).ToString();

    internal override TokenType TokenType => TokenType.FunctionCall;
  }
}
