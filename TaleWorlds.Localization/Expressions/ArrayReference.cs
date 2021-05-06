// Decompiled with JetBrains decompiler
// Type: Expressions.ArrayReference
// Assembly: TaleWorlds.Localization, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 26BB3E5A-EB48-4ABD-B2FC-10EF6D7A7285
// Assembly location: E:\SteamLibrary\steamapps\common\Mount & Blade II Bannerlord\bin\Win64_Shipping_Client\TaleWorlds.Localization.dll

using System.Collections.Generic;
using TaleWorlds.Library;
using TaleWorlds.Localization;
using TaleWorlds.Localization.TextProcessor;

namespace Expressions
{
  internal class ArrayReference : TextExpression
  {
    private TextExpression _indexExp;

    public ArrayReference(string rawValue, TextExpression indexExp)
    {
      this.RawValue = rawValue;
      this._indexExp = indexExp;
    }

    internal override string EvaluateString(TextProcessingContext context, TextObject parent)
    {
      int asNumber = this.EvaluateAsNumber(this._indexExp, context, parent);
      MultiStatement arrayAccess = context.GetArrayAccess(this.RawValue, asNumber);
      if (arrayAccess == null)
        return "";
      MBStringBuilder mbStringBuilder = new MBStringBuilder();
      mbStringBuilder.Initialize(callerMemberName: nameof (EvaluateString));
      foreach (TextExpression subStatement in (IEnumerable<TextExpression>) arrayAccess.SubStatements)
        mbStringBuilder.Append<string>(subStatement.EvaluateString(context, parent));
      return mbStringBuilder.ToStringAndRelease();
    }

    internal override TokenType TokenType => TokenType.ArrayAccess;
  }
}
