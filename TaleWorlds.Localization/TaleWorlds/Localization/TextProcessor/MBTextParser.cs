// Decompiled with JetBrains decompiler
// Type: TaleWorlds.Localization.TextProcessor.MBTextParser
// Assembly: TaleWorlds.Localization, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 26BB3E5A-EB48-4ABD-B2FC-10EF6D7A7285
// Assembly location: E:\SteamLibrary\steamapps\common\Mount & Blade II Bannerlord\bin\Win64_Shipping_Client\TaleWorlds.Localization.dll

using Expressions;
using System.Collections.Generic;

namespace TaleWorlds.Localization.TextProcessor
{
  internal class MBTextParser
  {
    private Stack<TextExpression> _symbolSequence;
    private TextExpression _lookaheadFirst;
    private TextExpression _lookaheadSecond;
    private TextExpression _lookaheadThird;
    private MBTextModel _queryModel;
    private const string ExpectedObjectErrorText = "ERROR:";

    internal TextExpression LookAheadFirst => this._lookaheadFirst;

    internal TextExpression LookAheadSecond => this._lookaheadSecond;

    internal TextExpression LookAheadThird => this._lookaheadThird;

    private TextExpression GetSimpleToken(TokenType tokenType, string strValue)
    {
      switch (tokenType)
      {
        case TokenType.Number:
          return (TextExpression) new SimpleNumberExpression(strValue);
        case TokenType.Identifier:
          return (TextExpression) new VariableExpression(strValue, (VariableExpression) null);
        case TokenType.Text:
          return (TextExpression) new SimpleText(strValue);
        case TokenType.LanguageMarker:
          return (TextExpression) new LangaugeMarkerExpression(strValue);
        case TokenType.QualifiedIdentifier:
          return (TextExpression) new QualifiedIdentifierExpression(strValue);
        case TokenType.ParameterWithAttribute:
          return (TextExpression) new ParameterWithAttributeExpression(strValue);
        case TokenType.textId:
          return (TextExpression) new TextIdExpression(strValue);
        default:
          return (TextExpression) new SimpleToken(tokenType, strValue);
      }
    }

    private void LoadSequenceStack(List<MBTextToken> tokens)
    {
      this._symbolSequence = new Stack<TextExpression>();
      for (int index = tokens.Count - 1; index >= 0; --index)
        this._symbolSequence.Push(this.GetSimpleToken(tokens[index].TokenType, tokens[index].Value));
    }

    private void PushToken(TextExpression token)
    {
      this._symbolSequence.Push(token);
      this.UpdateLookAheads();
    }

    private void UpdateLookAheads()
    {
      this._lookaheadFirst = this._symbolSequence.Count != 0 ? this._symbolSequence.Peek() : (TextExpression) SimpleToken.SequenceTerminator;
      if (this._symbolSequence.Count < 2)
      {
        this._lookaheadSecond = (TextExpression) SimpleToken.SequenceTerminator;
      }
      else
      {
        TextExpression textExpression = this._symbolSequence.Pop();
        this._lookaheadSecond = this._symbolSequence.Peek();
        this._symbolSequence.Push(textExpression);
      }
      if (this._symbolSequence.Count < 3)
      {
        this._lookaheadThird = (TextExpression) SimpleToken.SequenceTerminator;
      }
      else
      {
        TextExpression textExpression1 = this._symbolSequence.Pop();
        TextExpression textExpression2 = this._symbolSequence.Pop();
        this._lookaheadThird = this._symbolSequence.Peek();
        this._symbolSequence.Push(textExpression2);
        this._symbolSequence.Push(textExpression1);
      }
    }

    private void DiscardToken()
    {
      if (this._symbolSequence.Count > 0)
        this._symbolSequence.Pop();
      this.UpdateLookAheads();
    }

    private void DiscardToken(TokenType tokenType)
    {
      int tokenType1 = (int) this._lookaheadFirst.TokenType;
      this.DiscardToken();
    }

    private void Statements() => this._queryModel.AddRootExpression(this.GetRootExpressions());

    private bool IsRootExpression(TokenType tokenType) => tokenType == TokenType.Text || tokenType == TokenType.SimpleExpression || (tokenType == TokenType.ConditionalExpression || tokenType == TokenType.textId) || (tokenType == TokenType.SelectionExpression || tokenType == TokenType.MultiStatement || tokenType == TokenType.FieldExpression) || tokenType == TokenType.LanguageMarker;

    private void GetRootExpressionsImp(List<TextExpression> expList)
    {
      while (true)
      {
        do
          ;
        while (this.RunRootGrammarRulesExceptCollapse());
        if (this.IsRootExpression(this.LookAheadFirst.TokenType))
        {
          TextExpression lookAheadFirst = this.LookAheadFirst;
          this.DiscardToken();
          expList.Add(lookAheadFirst);
        }
        else
          break;
      }
    }

    private TextExpression GetRootExpressions()
    {
      List<TextExpression> expList = new List<TextExpression>();
      this.GetRootExpressionsImp(expList);
      if (expList.Count == 0)
        return (TextExpression) null;
      return expList.Count == 1 ? expList[0] : (TextExpression) new MultiStatement((IEnumerable<TextExpression>) expList);
    }

    private bool RunRootGrammarRulesExceptCollapse() => this.CheckSimpleStatement() || this.CheckConditionalStatement() || this.CheckSelectionStatement() || this.CheckFieldStatement();

    private bool CollapseStatements()
    {
      if (!this.IsRootExpression(this.LookAheadFirst.TokenType) || this.LookAheadFirst.TokenType == TokenType.MultiStatement)
        return false;
      List<TextExpression> textExpressionList = new List<TextExpression>();
      TextExpression lookAheadFirst1 = this.LookAheadFirst;
      this.DiscardToken();
      textExpressionList.Add(lookAheadFirst1);
      bool flag = false;
      while (!flag)
      {
        do
          ;
        while (this.RunRootGrammarRulesExceptCollapse());
        if (this.IsRootExpression(this.LookAheadFirst.TokenType))
        {
          TextExpression lookAheadFirst2 = this.LookAheadFirst;
          this.DiscardToken();
          textExpressionList.Add(lookAheadFirst2);
        }
        else
          flag = true;
      }
      this.PushToken((TextExpression) new MultiStatement((IEnumerable<TextExpression>) textExpressionList));
      return true;
    }

    private bool CheckSimpleStatement()
    {
      if (this.LookAheadFirst.TokenType != TokenType.OpenBraces)
        return false;
      this.DiscardToken(TokenType.OpenBraces);
      bool flag = false;
      while (!flag)
        flag = !this.DoExpressionRules();
      if (this.IsArithmeticExpression(this.LookAheadFirst.TokenType))
      {
        TextExpression token = (TextExpression) new SimpleExpression(this.LookAheadFirst);
        this.DiscardToken();
        this.DiscardToken(TokenType.CloseBraces);
        this.PushToken(token);
      }
      else
        this.DiscardToken(TokenType.CloseBraces);
      return true;
    }

    private bool CheckFieldStatement()
    {
      if (this.LookAheadFirst.TokenType != TokenType.FieldStarter)
        return false;
      this.DiscardToken(TokenType.FieldStarter);
      bool flag = false;
      while (!flag)
        flag = !this.DoExpressionRules();
      if (this.LookAheadFirst.TokenType != TokenType.Identifier)
        return false;
      TextExpression lookAheadFirst = this.LookAheadFirst;
      this.DiscardToken(TokenType.Identifier);
      this.DiscardToken(TokenType.CloseBraces);
      TextExpression textExpression = this.GetRootExpressions() ?? (TextExpression) new SimpleToken(TokenType.Text, "");
      this.DiscardToken(TokenType.FieldFinalizer);
      TextExpression part2 = textExpression;
      this.PushToken((TextExpression) new FieldExpression(lookAheadFirst, part2));
      return true;
    }

    private bool CheckConditionalStatement()
    {
      if (this.LookAheadFirst.TokenType != TokenType.ConditionStarter)
        return false;
      bool flag = false;
      List<TextExpression> conditionExpressions = new List<TextExpression>();
      List<TextExpression> resultExpressions2 = new List<TextExpression>();
      while (!flag)
      {
        TokenType tokenType = this.LookAheadFirst.TokenType;
        if (this.LookAheadFirst.TokenType == TokenType.ConditionStarter || this.LookAheadFirst.TokenType == TokenType.ConditionFollowUp)
        {
          this.DiscardToken();
          do
            ;
          while (this.DoExpressionRules());
          if (!this.IsArithmeticExpression(this.LookAheadFirst.TokenType))
            return false;
          conditionExpressions.Add(this.LookAheadFirst);
          this.DiscardToken();
          this.DiscardToken(TokenType.CloseBraces);
        }
        else
        {
          if (tokenType != TokenType.ConditionSeperator && tokenType != TokenType.Seperator)
            return false;
          this.DiscardToken();
          flag = true;
        }
        TextExpression textExpression = this.GetRootExpressions() ?? (TextExpression) new SimpleToken(TokenType.Text, "");
        resultExpressions2.Add(textExpression);
      }
      do
        ;
      while (!flag);
      this.DiscardToken(TokenType.ConditionFinalizer);
      this.PushToken((TextExpression) new ConditionExpression(conditionExpressions, resultExpressions2));
      return true;
    }

    private bool CheckSelectionStatement()
    {
      if (this.LookAheadFirst.TokenType != TokenType.SelectionStarter)
        return false;
      this.DiscardToken(TokenType.SelectionStarter);
      do
        ;
      while (this.DoExpressionRules());
      if (!this.IsArithmeticExpression(this.LookAheadFirst.TokenType))
        return false;
      TextExpression lookAheadFirst = this.LookAheadFirst;
      this.DiscardToken();
      this.DiscardToken(TokenType.CloseBraces);
      bool flag = false;
      List<TextExpression> selectionExpressions = new List<TextExpression>();
      do
      {
        TextExpression textExpression = this.GetRootExpressions() ?? (TextExpression) new SimpleToken(TokenType.Text, "");
        selectionExpressions.Add(textExpression);
        switch (this.LookAheadFirst.TokenType)
        {
          case TokenType.SelectionSeperator:
            this.DiscardToken();
            break;
          case TokenType.SelectionFinalizer:
            flag = true;
            this.DiscardToken();
            break;
          default:
            return false;
        }
      }
      while (!flag);
      this.PushToken((TextExpression) new SelectionExpression(lookAheadFirst, selectionExpressions));
      return true;
    }

    private bool DoExpressionRules() => this.ConsumeArrayAccessExpression() || this.ConsumeFunction() || (this.ConsumeMarkerOccuranceExpression() || this.ConsumeNegativeAritmeticExpression()) || (this.ConsumeParenthesisExpression() || this.ConsumeInnerAritmeticExpression() || (this.ConsumeOuterAritmeticExpression() || this.ConsumeComparisonExpression()));

    private bool ConsumeFunction()
    {
      if (this.LookAheadFirst.TokenType != TokenType.FunctionIdentifier)
        return false;
      string functionName = this.LookAheadFirst.RawValue.Substring(0, this.LookAheadFirst.RawValue.Length - 1);
      this.DiscardToken();
      bool flag = false;
      List<TextExpression> textExpressionList = new List<TextExpression>();
      while (this.LookAheadFirst.TokenType != TokenType.CloseParenthesis && !flag)
      {
        if (textExpressionList.Count > 0)
          this.DiscardToken(TokenType.Comma);
        do
          ;
        while (this.DoExpressionRules());
        if (!this.IsArithmeticExpression(this.LookAheadFirst.TokenType))
          return false;
        textExpressionList.Add(this.LookAheadFirst);
        this.DiscardToken();
      }
      this.DiscardToken(TokenType.CloseParenthesis);
      this.PushToken((TextExpression) new FunctionCall(functionName, (IEnumerable<TextExpression>) textExpressionList));
      return true;
    }

    private bool ConsumeMarkerOccuranceExpression()
    {
      if (this.LookAheadFirst.TokenType != TokenType.Identifier || this.LookAheadSecond.TokenType != TokenType.MarkerOccuranceIdentifier)
        return false;
      VariableExpression lookAheadFirst = this.LookAheadFirst as VariableExpression;
      TextExpression lookAheadSecond = this.LookAheadSecond;
      this.DiscardToken();
      this.DiscardToken();
      this.PushToken((TextExpression) new MarkerOccuranceTextExpression(lookAheadSecond.RawValue.Substring(2), lookAheadFirst));
      return true;
    }

    private bool ConsumeArrayAccessExpression()
    {
      if (this.LookAheadFirst.TokenType == TokenType.Identifier && this.LookAheadSecond.TokenType == TokenType.OpenBrackets)
      {
        TextExpression lookAheadFirst1 = this.LookAheadFirst;
        this.DiscardToken();
        this.DiscardToken(TokenType.OpenBrackets);
        do
          ;
        while (this.DoExpressionRules());
        if (this.IsArithmeticExpression(this.LookAheadFirst.TokenType))
        {
          TextExpression lookAheadFirst2 = this.LookAheadFirst;
          this.DiscardToken();
          this.DiscardToken(TokenType.CloseBrackets);
          this.PushToken((TextExpression) new ArrayReference(lookAheadFirst1.RawValue, lookAheadFirst2));
          return true;
        }
      }
      return false;
    }

    private bool ConsumeNegativeAritmeticExpression()
    {
      if (this.LookAheadFirst.TokenType == TokenType.Minus)
      {
        int num = (int) this.ConsumeAritmeticOperation();
        if (this.IsArithmeticExpression(this.LookAheadFirst.TokenType))
        {
          this.PushToken((TextExpression) new ArithmeticExpression(ArithmeticOperation.Subtract, (TextExpression) new SimpleToken(TokenType.Number, "0"), this.LookAheadFirst));
          return true;
        }
      }
      return false;
    }

    private bool ConsumeParenthesisExpression()
    {
      if (this.LookAheadFirst.TokenType != TokenType.OpenParenthesis)
        return false;
      this.DiscardToken(TokenType.OpenParenthesis);
      do
        ;
      while (this.DoExpressionRules());
      if (this.IsArithmeticExpression(this.LookAheadFirst.TokenType))
      {
        ParanthesisExpression paranthesisExpression = new ParanthesisExpression(this.LookAheadFirst);
        this.DiscardToken();
        this.DiscardToken(TokenType.CloseParenthesis);
        this.PushToken((TextExpression) paranthesisExpression);
        return true;
      }
      this.DiscardToken(TokenType.CloseParenthesis);
      return true;
    }

    private bool IsArithmeticExpression(TokenType t) => t == TokenType.ArithmeticProduct || t == TokenType.ArithmeticSum || (t == TokenType.Identifier || t == TokenType.QualifiedIdentifier) || (t == TokenType.MarkerOccuranceExpression || t == TokenType.ParameterWithMarkerOccurance || (t == TokenType.Number || t == TokenType.ParenthesisExpression)) || (t == TokenType.ComparisonExpression || t == TokenType.FunctionCall || (t == TokenType.FunctionParam || t == TokenType.ArrayAccess)) || t == TokenType.ParameterWithAttribute;

    private bool ConsumeInnerAritmeticExpression()
    {
      TokenType tokenType1 = this.LookAheadFirst.TokenType;
      TokenType tokenType2 = this.LookAheadSecond.TokenType;
      int tokenType3 = (int) this.LookAheadThird.TokenType;
      if (!this.IsArithmeticExpression(tokenType1) || tokenType2 != TokenType.Multiply && tokenType2 != TokenType.Divide)
        return false;
      TextExpression lookAheadFirst1 = this.LookAheadFirst;
      this.DiscardToken();
      ArithmeticOperation op = this.ConsumeAritmeticOperation();
      if (!this.IsArithmeticExpression(this.LookAheadFirst.TokenType))
      {
        while (this.DoExpressionRules())
          ;
      }
      TextExpression lookAheadFirst2 = this.LookAheadFirst;
      this.DiscardToken();
      this.PushToken((TextExpression) new ArithmeticExpression(op, lookAheadFirst1, lookAheadFirst2));
      return true;
    }

    private bool ConsumeOuterAritmeticExpression()
    {
      TokenType tokenType1 = this.LookAheadFirst.TokenType;
      TokenType tokenType2 = this.LookAheadSecond.TokenType;
      if (this.IsArithmeticExpression(tokenType1) && (tokenType2 == TokenType.Plus || tokenType2 == TokenType.Minus))
      {
        TextExpression lookAheadFirst1 = this.LookAheadFirst;
        this.DiscardToken();
        ArithmeticOperation op = this.ConsumeAritmeticOperation();
        do
          ;
        while (this.DoExpressionRules());
        if (this.IsArithmeticExpression(this.LookAheadFirst.TokenType))
        {
          TextExpression lookAheadFirst2 = this.LookAheadFirst;
          this.DiscardToken();
          this.PushToken((TextExpression) new ArithmeticExpression(op, lookAheadFirst1, lookAheadFirst2));
          return true;
        }
      }
      return false;
    }

    private ArithmeticOperation ConsumeAritmeticOperation()
    {
      int num = this.LookAheadFirst.TokenType == TokenType.Plus ? 0 : (this.LookAheadFirst.TokenType == TokenType.Minus ? 1 : (this.LookAheadFirst.TokenType == TokenType.Multiply ? 2 : (this.LookAheadFirst.TokenType == TokenType.Divide ? 3 : 0)));
      this.DiscardToken();
      return (ArithmeticOperation) num;
    }

    private bool ConsumeComparisonExpression()
    {
      TokenType tokenType1 = this.LookAheadFirst.TokenType;
      TokenType tokenType2 = this.LookAheadSecond.TokenType;
      if (!this.IsArithmeticExpression(tokenType1) || !this.IsComparisonOperator(tokenType2))
        return false;
      TextExpression lookAheadFirst1 = this.LookAheadFirst;
      this.DiscardToken();
      ComparisonOperation comparisonOp = this.GetComparisonOp(tokenType2);
      this.DiscardToken();
      do
        ;
      while (this.DoExpressionRules());
      if (!this.IsArithmeticExpression(this.LookAheadFirst.TokenType))
        return false;
      TextExpression lookAheadFirst2 = this.LookAheadFirst;
      this.DiscardToken();
      this.PushToken((TextExpression) new ComparisonExpression(comparisonOp, lookAheadFirst1, lookAheadFirst2));
      return true;
    }

    private bool IsComparisonOperator(TokenType tokenType) => tokenType == TokenType.Equals || tokenType == TokenType.NotEquals || (tokenType == TokenType.GreaterThan || tokenType == TokenType.GreaterOrEqual) || (tokenType == TokenType.GreaterThan || tokenType == TokenType.LessOrEqual) || tokenType == TokenType.LessThan;

    private BooleanOperation GetBooleanOp(TokenType tokenType)
    {
      if (tokenType == TokenType.Or)
        return BooleanOperation.Or;
      return tokenType != TokenType.And && tokenType == TokenType.Not ? BooleanOperation.Not : BooleanOperation.And;
    }

    private ComparisonOperation GetComparisonOp(TokenType tokenType)
    {
      if (tokenType == TokenType.Equals)
        return ComparisonOperation.Equals;
      if (tokenType == TokenType.NotEquals)
        return ComparisonOperation.NotEquals;
      if (tokenType == TokenType.GreaterThan)
        return ComparisonOperation.GreaterThan;
      if (tokenType == TokenType.GreaterOrEqual)
        return ComparisonOperation.GreaterOrEqual;
      if (tokenType == TokenType.GreaterThan)
        return ComparisonOperation.GreaterThan;
      if (tokenType == TokenType.LessOrEqual)
        return ComparisonOperation.LessOrEqual;
      return tokenType != TokenType.LessThan ? ComparisonOperation.Equals : ComparisonOperation.LessThan;
    }

    private bool CheckConditionalExpression()
    {
      if (this.LookAheadFirst.TokenType != TokenType.ConditionStarter)
        return false;
      this.DiscardToken(TokenType.ConditionStarter);
      return true;
    }

    private MBTextModel ParseInternal(List<MBTextToken> tokens)
    {
      this.LoadSequenceStack(tokens);
      this.UpdateLookAheads();
      this._queryModel = new MBTextModel();
      this.Statements();
      this.DiscardToken(TokenType.SequenceTerminator);
      return this._queryModel;
    }

    internal static MBTextModel Parse(List<MBTextToken> tokens) => new MBTextParser().ParseInternal(tokens);
  }
}
