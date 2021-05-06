// Decompiled with JetBrains decompiler
// Type: TaleWorlds.Localization.TextProcessor.TokenMatch
// Assembly: TaleWorlds.Localization, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 26BB3E5A-EB48-4ABD-B2FC-10EF6D7A7285
// Assembly location: E:\SteamLibrary\steamapps\common\Mount & Blade II Bannerlord\bin\Win64_Shipping_Client\TaleWorlds.Localization.dll

namespace TaleWorlds.Localization.TextProcessor
{
  internal class TokenMatch
  {
    internal TokenMatch(
      TokenType tokenType,
      string value,
      int startIndex,
      int endIndex,
      int precedence)
    {
      this.TokenType = tokenType;
      this.Value = value;
      this.StartIndex = startIndex;
      this.EndIndex = endIndex;
      this.Precedence = precedence;
    }

    internal TokenType TokenType { get; private set; }

    internal string Value { get; private set; }

    internal int StartIndex { get; private set; }

    internal int EndIndex { get; private set; }

    internal int Precedence { get; private set; }
  }
}
