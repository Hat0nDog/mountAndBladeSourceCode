// Decompiled with JetBrains decompiler
// Type: Expressions.SelectionExpression
// Assembly: TaleWorlds.Localization, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 26BB3E5A-EB48-4ABD-B2FC-10EF6D7A7285
// Assembly location: E:\SteamLibrary\steamapps\common\Mount & Blade II Bannerlord\bin\Win64_Shipping_Client\TaleWorlds.Localization.dll

using System.Collections.Generic;
using TaleWorlds.Localization;
using TaleWorlds.Localization.TextProcessor;

namespace Expressions
{
  internal class SelectionExpression : TextExpression
  {
    private TextExpression _selection;
    private List<TextExpression> _selectionExpressions;

    public SelectionExpression(TextExpression selection, List<TextExpression> selectionExpressions)
    {
      this._selection = selection;
      this._selectionExpressions = selectionExpressions;
    }

    internal override string EvaluateString(TextProcessingContext context, TextObject parent)
    {
      int asNumber = this.EvaluateAsNumber(this._selection, context, parent);
      return asNumber >= 0 && asNumber < this._selectionExpressions.Count ? this._selectionExpressions[asNumber].EvaluateString(context, parent) : "";
    }

    internal override TokenType TokenType => TokenType.SelectionExpression;
  }
}
