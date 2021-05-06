// Decompiled with JetBrains decompiler
// Type: TaleWorlds.Localization.TextProcessor.MBTextToken
// Assembly: TaleWorlds.Localization, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 26BB3E5A-EB48-4ABD-B2FC-10EF6D7A7285
// Assembly location: E:\SteamLibrary\steamapps\common\Mount & Blade II Bannerlord\bin\Win64_Shipping_Client\TaleWorlds.Localization.dll

namespace TaleWorlds.Localization.TextProcessor
{
  internal class MBTextToken
  {
    internal MBTextToken(TokenType tokenType)
    {
      this.TokenType = tokenType;
      this.Value = string.Empty;
    }

    internal MBTextToken(TokenType tokenType, string value)
    {
      this.TokenType = tokenType;
      this.Value = value;
    }

    internal TokenType TokenType { get; set; }

    public string Value { get; set; }

    public MBTextToken Clone() => new MBTextToken(this.TokenType, this.Value);
  }
}
