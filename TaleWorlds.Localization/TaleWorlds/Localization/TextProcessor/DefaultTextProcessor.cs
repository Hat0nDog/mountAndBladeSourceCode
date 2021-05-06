// Decompiled with JetBrains decompiler
// Type: TaleWorlds.Localization.TextProcessor.DefaultTextProcessor
// Assembly: TaleWorlds.Localization, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 26BB3E5A-EB48-4ABD-B2FC-10EF6D7A7285
// Assembly location: E:\SteamLibrary\steamapps\common\Mount & Blade II Bannerlord\bin\Win64_Shipping_Client\TaleWorlds.Localization.dll

using System.Globalization;
using System.Text;

namespace TaleWorlds.Localization.TextProcessor
{
  public class DefaultTextProcessor : LanguageSpecificTextProcessor
  {
    public override void ProcessToken(
      string sourceText,
      ref int cursorPos,
      string token,
      StringBuilder outputString)
    {
    }

    public override CultureInfo CultureInfoForLanguage => CultureInfo.InvariantCulture;
  }
}
