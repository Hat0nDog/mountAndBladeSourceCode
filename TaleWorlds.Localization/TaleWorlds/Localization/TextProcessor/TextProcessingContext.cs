// Decompiled with JetBrains decompiler
// Type: TaleWorlds.Localization.TextProcessor.TextProcessingContext
// Assembly: TaleWorlds.Localization, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 26BB3E5A-EB48-4ABD-B2FC-10EF6D7A7285
// Assembly location: E:\SteamLibrary\steamapps\common\Mount & Blade II Bannerlord\bin\Win64_Shipping_Client\TaleWorlds.Localization.dll

using Expressions;
using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using TaleWorlds.Library;

namespace TaleWorlds.Localization.TextProcessor
{
  public class TextProcessingContext
  {
    private readonly Dictionary<string, TextObject> _variables = new Dictionary<string, TextObject>((IEqualityComparer<string>) new CaseInsensitiveComparer());
    private readonly Dictionary<string, MBTextModel> _functions = new Dictionary<string, MBTextModel>();
    private readonly Stack<TextObject[]> _curParams = new Stack<TextObject[]>();
    private readonly Stack<TextObject[]> _curParamsWithoutEvaluate = new Stack<TextObject[]>();

    internal void SetTextVariable(string variableName, TextObject data) => this._variables[variableName] = data;

    internal MultiStatement GetVariableValue(string variableName, TextObject parent)
    {
      TextObject variable = (TextObject) null;
      MBTextModel mbTextModel = (MBTextModel) null;
      if (parent == null || !parent.GetVariableValue(variableName, out variable))
        this._variables.TryGetValue(variableName, out variable);
      if (variable != null)
        mbTextModel = MBTextParser.Parse(MBTextManager.Tokenizer.Tokenize(variable.ToString()));
      if (mbTextModel == null)
        return (MultiStatement) null;
      return mbTextModel.RootExpressions.Count == 1 && mbTextModel.RootExpressions[0] is MultiStatement ? new MultiStatement((IEnumerable<TextExpression>) (mbTextModel.RootExpressions[0] as MultiStatement).SubStatements) : new MultiStatement((IEnumerable<TextExpression>) mbTextModel.RootExpressions);
    }

    internal (TextObject, bool) GetVariableValueAsTextObject(
      string variableName,
      TextObject parent)
    {
      TextObject variable;
      if (parent != null && parent.GetVariableValue(variableName, out variable) || this._variables.TryGetValue(variableName, out variable))
        return (variable, true);
      if (parent != null)
        variable = TextProcessingContext.FindNestedFieldValue(parent.Value, variableName);
      return variable == null || variable.Length != 0 ? (variable, true) : (new TextObject("{=!}ERROR: " + variableName + " variable has not been set before."), false);
    }

    internal MultiStatement GetArrayAccess(string variableName, int index)
    {
      TextObject textObject;
      return this._variables.TryGetValue(variableName + ":" + (object) index, out textObject) ? new MultiStatement((IEnumerable<TextExpression>) MBTextParser.Parse(MBTextManager.Tokenizer.Tokenize(textObject.ToString())).RootExpressions) : (MultiStatement) null;
    }

    private int CountMarkerOccurancesInString(string searchedIdentifier, string text) => new Regex("{." + searchedIdentifier + "}").IsMatch(text) ? 1 : 0;

    internal string GetParameterWithMarkerOccurance(string token, TextObject parent)
    {
      int length = token.IndexOf('!');
      if (length == -1)
        return "";
      string rawValue = token.Substring(0, length);
      string searchedIdentifier = token.Substring(length + 2, token.Length - length - 2);
      TextObject paramWithoutEvaluate = this.GetFunctionParamWithoutEvaluate(rawValue);
      TextObject to;
      return (parent?.Attributes != null && parent.TryGetAttributesValue(paramWithoutEvaluate.ToString(), out to) || this._variables.TryGetValue(paramWithoutEvaluate.ToString(), out to)) && to.Length > 0 ? this.CountMarkerOccurancesInString(searchedIdentifier, MBTextManager.ProcessWithoutLanguageProcessor(to)).ToString() : "";
    }

    internal static bool IsDeclaration(string token) => token.Length > 1 && token[0] == '@';

    internal static bool IsDeclarationFinalizer(string token) => token.Length == 2 && (token[0] == '\\' || token[0] == '/') && token[1] == '@';

    private static TextObject FindNestedFieldValue(string text, string identifier)
    {
      string[] fieldNames = identifier.Split(new char[1]
      {
        '.'
      }, StringSplitOptions.RemoveEmptyEntries);
      return new TextObject(TextProcessingContext.GetFieldValue(text, fieldNames));
    }

    internal (TextObject value, bool doesValueExist) GetQualifiedVariableValue(
      string token,
      TextObject parent)
    {
      int length = token.IndexOf('.');
      if (length == -1)
        return this.GetVariableValueAsTextObject(token, parent);
      string str1 = token.Substring(0, length);
      string str2 = token.Substring(length + 1, token.Length - (length + 1));
      if (parent?.Attributes != null)
      {
        TextObject parent1;
        if (parent.TryGetAttributesValue(str1, out parent1))
        {
          (TextObject value4, bool doesValueExist4) = this.GetQualifiedVariableValue(str2, parent1);
          if (!value4.Equals(TextObject.Empty))
            return (value4, doesValueExist4);
        }
      }
      else
      {
        TextObject textObject;
        if (this._variables.TryGetValue(str1, out textObject) && textObject.Length > 0)
          return (TextProcessingContext.FindNestedFieldValue(textObject.Value, str2), true);
        foreach (KeyValuePair<string, TextObject> variable in this._variables)
        {
          if (variable.Key == str1 && variable.Value.Attributes != null)
          {
            foreach (KeyValuePair<string, object> attribute in variable.Value.Attributes)
            {
              if (attribute.Key == str2)
                return (TextObject.TryGetOrCreateFromObject(attribute.Value), true);
            }
          }
        }
      }
      return (TextObject.Empty, false);
    }

    private static string GetFieldValue(string text, string[] fieldNames)
    {
      int i = 0;
      int index = 0;
      int num1 = 0;
      MBStringBuilder mbStringBuilder = new MBStringBuilder();
      mbStringBuilder.Initialize(callerMemberName: nameof (GetFieldValue));
      for (; i < text.Length; ++i)
      {
        if (text[i] != '{')
        {
          if (index == fieldNames.Length && num1 == index)
            mbStringBuilder.Append(text[i]);
        }
        else
        {
          string token = TextProcessingContext.ReadFirstToken(text, ref i);
          if (TextProcessingContext.IsDeclarationFinalizer(token))
          {
            --index;
            if (num1 > index)
              num1 = index;
          }
          else if (TextProcessingContext.IsDeclaration(token))
          {
            string strB = token.Substring(1);
            int num2 = num1 != index || index >= fieldNames.Length ? 0 : (string.Compare(fieldNames[index], strB, StringComparison.InvariantCultureIgnoreCase) == 0 ? 1 : 0);
            ++index;
            if (num2 != 0)
              num1 = index;
          }
        }
      }
      return mbStringBuilder.ToStringAndRelease();
    }

    internal static string ReadFirstToken(string text, ref int i)
    {
      int num1 = i;
      while (i < text.Length && text[i] != '}')
        ++i;
      int num2 = i - num1;
      return text.Substring(num1 + 1, num2 - 1);
    }

    internal TextObject CallFunction(
      string functionName,
      List<TextExpression> functionParams,
      TextObject parent)
    {
      TextObject[] textObjectArray1 = new TextObject[functionParams.Count];
      TextObject[] textObjectArray2 = new TextObject[functionParams.Count];
      for (int index = 0; index < functionParams.Count; ++index)
      {
        textObjectArray1[index] = new TextObject(functionParams[index].EvaluateString(this, parent));
        textObjectArray2[index] = new TextObject(functionParams[index].RawValue);
      }
      this._curParams.Push(textObjectArray1);
      this._curParamsWithoutEvaluate.Push(textObjectArray2);
      MBStringBuilder mbStringBuilder = new MBStringBuilder();
      MBTextModel functionBody = this.GetFunctionBody(functionName);
      mbStringBuilder.Initialize(callerMemberName: nameof (CallFunction));
      foreach (TextExpression rootExpression in (IEnumerable<TextExpression>) functionBody.RootExpressions)
        mbStringBuilder.Append<string>(rootExpression.EvaluateString(this, parent));
      string stringAndRelease = mbStringBuilder.ToStringAndRelease();
      this._curParams.Pop();
      return new TextObject(stringAndRelease);
    }

    public void SetFunction(string functionName, MBTextModel functionBody) => this._functions[functionName] = functionBody;

    public void ResetFunctions() => this._functions.Clear();

    public MBTextModel GetFunctionBody(string functionName)
    {
      MBTextModel mbTextModel;
      this._functions.TryGetValue(functionName, out mbTextModel);
      return mbTextModel;
    }

    public TextObject GetFunctionParam(string rawValue)
    {
      int result;
      if (!int.TryParse(rawValue.Substring(1), out result))
        return TextObject.Empty;
      return this._curParams.Count > 0 && this._curParams.Peek().Length > result ? this._curParams.Peek()[result] : new TextObject("Can't find parameter:" + rawValue);
    }

    public TextObject GetFunctionParamWithoutEvaluate(string rawValue)
    {
      int result;
      if (!int.TryParse(rawValue.Substring(1), out result))
        return TextObject.Empty;
      return this._curParamsWithoutEvaluate.Count > 0 && this._curParamsWithoutEvaluate.Peek().Length > result ? this._curParamsWithoutEvaluate.Peek()[result] : new TextObject("Can't find parameter:" + rawValue);
    }

    internal void ClearAll() => this._variables.Clear();
  }
}
