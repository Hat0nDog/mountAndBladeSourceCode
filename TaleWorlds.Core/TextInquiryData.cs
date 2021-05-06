// Decompiled with JetBrains decompiler
// Type: TaleWorlds.Core.TextInquiryData
// Assembly: TaleWorlds.Core, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 43F7B45C-9AE1-4715-A37B-2364840F62AF
// Assembly location: E:\SteamLibrary\steamapps\common\Mount & Blade II Bannerlord\bin\Win64_Shipping_Client\TaleWorlds.Core.dll

using System;

namespace TaleWorlds.Core
{
  public class TextInquiryData
  {
    public string TitleText;
    public string Text = "";
    public readonly bool IsAffirmativeOptionShown;
    public readonly bool IsNegativeOptionShown;
    public readonly bool IsInputObfuscated;
    public readonly string AffirmativeText;
    public readonly string NegativeText;
    public readonly string SoundEventPath;
    public readonly string DefaultInputText;
    public readonly Action<string> AffirmativeAction;
    public readonly Action NegativeAction;
    public readonly Func<string, bool> TextCondition;

    public TextInquiryData(
      string titleText,
      string text,
      bool isAffirmativeOptionShown,
      bool isNegativeOptionShown,
      string affirmativeText,
      string negativeText,
      Action<string> affirmativeAction,
      Action negativeAction,
      bool shouldInputBeObfuscated = false,
      Func<string, bool> textCondition = null,
      string soundEventPath = "",
      string defaultInputText = "")
    {
      this.TitleText = titleText;
      this.Text = text;
      this.IsAffirmativeOptionShown = isAffirmativeOptionShown;
      this.IsNegativeOptionShown = isNegativeOptionShown;
      this.AffirmativeText = affirmativeText;
      this.NegativeText = negativeText;
      this.AffirmativeAction = affirmativeAction;
      this.NegativeAction = negativeAction;
      this.TextCondition = textCondition;
      this.IsInputObfuscated = shouldInputBeObfuscated;
      this.SoundEventPath = soundEventPath;
      this.DefaultInputText = defaultInputText;
    }
  }
}
