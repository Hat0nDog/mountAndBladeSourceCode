// Decompiled with JetBrains decompiler
// Type: Expressions.ParameterWithAttributeExpression
// Assembly: TaleWorlds.Localization, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 26BB3E5A-EB48-4ABD-B2FC-10EF6D7A7285
// Assembly location: E:\SteamLibrary\steamapps\common\Mount & Blade II Bannerlord\bin\Win64_Shipping_Client\TaleWorlds.Localization.dll

using TaleWorlds.Localization;
using TaleWorlds.Localization.TextProcessor;

namespace Expressions
{
  internal class ParameterWithAttributeExpression : TextExpression
  {
    private readonly string _parameter;
    private readonly string _attribute;

    public ParameterWithAttributeExpression(string identifierName)
    {
      this._parameter = identifierName.Remove(identifierName.IndexOf('.'));
      this._attribute = identifierName.Substring(identifierName.IndexOf('.'));
    }

    internal override TokenType TokenType => TokenType.ParameterWithAttribute;

    internal override string EvaluateString(TextProcessingContext context, TextObject parent)
    {
      TextObject paramWithoutEvaluate = context.GetFunctionParamWithoutEvaluate(this._parameter);
      (TextObject value, bool doesValueExist) qualifiedVariableValue = context.GetQualifiedVariableValue(paramWithoutEvaluate.ToString() + this._attribute, parent);
      TextObject textObject = qualifiedVariableValue.value;
      return qualifiedVariableValue.doesValueExist ? textObject.ToString() : "";
    }
  }
}
