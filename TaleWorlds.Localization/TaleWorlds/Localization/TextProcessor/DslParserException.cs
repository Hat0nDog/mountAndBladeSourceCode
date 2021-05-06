// Decompiled with JetBrains decompiler
// Type: TaleWorlds.Localization.TextProcessor.DslParserException
// Assembly: TaleWorlds.Localization, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 26BB3E5A-EB48-4ABD-B2FC-10EF6D7A7285
// Assembly location: E:\SteamLibrary\steamapps\common\Mount & Blade II Bannerlord\bin\Win64_Shipping_Client\TaleWorlds.Localization.dll

using System;

namespace TaleWorlds.Localization.TextProcessor
{
  internal class DslParserException : Exception
  {
    public DslParserException(string format)
    {
    }

    public enum DslOperator
    {
      NotDefined,
      Equals,
      NotEquals,
      In,
      NotIn,
      GreaterThan,
      GreaterOrEqual,
      LessThan,
      LessOrEqual,
    }

    public enum DslLogicalOperator
    {
      NotDefined,
      Or,
      And,
    }
  }
}
