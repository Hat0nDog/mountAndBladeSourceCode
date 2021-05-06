// Decompiled with JetBrains decompiler
// Type: TaleWorlds.Localization.TextProcessor.TextGrammarProcessor
// Assembly: TaleWorlds.Localization, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 26BB3E5A-EB48-4ABD-B2FC-10EF6D7A7285
// Assembly location: E:\SteamLibrary\steamapps\common\Mount & Blade II Bannerlord\bin\Win64_Shipping_Client\TaleWorlds.Localization.dll

using Expressions;
using System.Collections.Generic;
using TaleWorlds.Library;

namespace TaleWorlds.Localization.TextProcessor
{
  public static class TextGrammarProcessor
  {
    public static string Process(
      MBTextModel dataRepresentation,
      TextProcessingContext textContext,
      TextObject parent = null)
    {
      MBStringBuilder mbStringBuilder = new MBStringBuilder();
      mbStringBuilder.Initialize(callerMemberName: nameof (Process));
      foreach (TextExpression rootExpression in (IEnumerable<TextExpression>) dataRepresentation.RootExpressions)
      {
        if (rootExpression != null)
        {
          string str = rootExpression.EvaluateString(textContext, parent).ToString();
          mbStringBuilder.Append<string>(str);
        }
      }
      return mbStringBuilder.ToStringAndRelease();
    }
  }
}
