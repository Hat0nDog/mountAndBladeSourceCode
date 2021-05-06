// Decompiled with JetBrains decompiler
// Type: TaleWorlds.Localization.TextProcessor.MBTextModel
// Assembly: TaleWorlds.Localization, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 26BB3E5A-EB48-4ABD-B2FC-10EF6D7A7285
// Assembly location: E:\SteamLibrary\steamapps\common\Mount & Blade II Bannerlord\bin\Win64_Shipping_Client\TaleWorlds.Localization.dll

using Expressions;
using System.Collections.Generic;

namespace TaleWorlds.Localization.TextProcessor
{
  public class MBTextModel
  {
    internal List<TextExpression> _rootExpressions = new List<TextExpression>();

    internal IReadOnlyList<TextExpression> RootExpressions => (IReadOnlyList<TextExpression>) this._rootExpressions;

    internal void AddRootExpression(TextExpression newExp) => this._rootExpressions.Add(newExp);
  }
}
