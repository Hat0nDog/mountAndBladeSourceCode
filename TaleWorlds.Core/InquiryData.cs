// Decompiled with JetBrains decompiler
// Type: TaleWorlds.Core.InquiryData
// Assembly: TaleWorlds.Core, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 43F7B45C-9AE1-4715-A37B-2364840F62AF
// Assembly location: E:\SteamLibrary\steamapps\common\Mount & Blade II Bannerlord\bin\Win64_Shipping_Client\TaleWorlds.Core.dll

using System;

namespace TaleWorlds.Core
{
  public class InquiryData
  {
    public string TitleText;
    public string Text;
    public readonly bool IsAffirmativeOptionShown;
    public readonly bool IsNegativeOptionShown;
    public readonly string AffirmativeText;
    public readonly string NegativeText;
    public readonly string SoundEventPath;
    public Action AffirmativeAction;
    public readonly Action NegativeAction;

    public InquiryData(
      string titleText,
      string text,
      bool isAffirmativeOptionShown,
      bool isNegativeOptionShown,
      string affirmativeText,
      string negativeText,
      Action affirmativeAction,
      Action negativeAction,
      string soundEventPath = "")
    {
      this.TitleText = titleText;
      this.Text = text;
      this.IsAffirmativeOptionShown = isAffirmativeOptionShown;
      this.IsNegativeOptionShown = isNegativeOptionShown;
      this.AffirmativeText = affirmativeText;
      this.NegativeText = negativeText;
      this.AffirmativeAction = affirmativeAction;
      this.NegativeAction = negativeAction;
      this.SoundEventPath = soundEventPath;
    }

    public void SetText(string text) => this.Text = text;

    public void SetTitleText(string titleText) => this.TitleText = titleText;

    public void SetAffirmativeAction(Action newAffirmativeAction) => this.AffirmativeAction = newAffirmativeAction;
  }
}
