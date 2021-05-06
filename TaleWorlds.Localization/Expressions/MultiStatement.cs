// Decompiled with JetBrains decompiler
// Type: Expressions.MultiStatement
// Assembly: TaleWorlds.Localization, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 26BB3E5A-EB48-4ABD-B2FC-10EF6D7A7285
// Assembly location: E:\SteamLibrary\steamapps\common\Mount & Blade II Bannerlord\bin\Win64_Shipping_Client\TaleWorlds.Localization.dll

using System.Collections.Generic;
using System.Linq;
using TaleWorlds.Library;
using TaleWorlds.Localization;
using TaleWorlds.Localization.TextProcessor;

namespace Expressions
{
  internal class MultiStatement : TextExpression
  {
    private List<TextExpression> _subStatements = new List<TextExpression>();

    public MultiStatement(IEnumerable<TextExpression> subStatements) => this._subStatements = subStatements.ToList<TextExpression>();

    public IReadOnlyList<TextExpression> SubStatements => (IReadOnlyList<TextExpression>) this._subStatements;

    internal override TokenType TokenType => TokenType.MultiStatement;

    public void AddStatement(TextExpression s2) => this._subStatements.Add(s2);

    internal override string EvaluateString(TextProcessingContext context, TextObject parent)
    {
      MBStringBuilder mbStringBuilder = new MBStringBuilder();
      mbStringBuilder.Initialize(callerMemberName: nameof (EvaluateString));
      foreach (TextExpression subStatement in this._subStatements)
      {
        if (subStatement != null)
          mbStringBuilder.Append<string>(subStatement.EvaluateString(context, parent));
      }
      return mbStringBuilder.ToStringAndRelease();
    }
  }
}
