// Decompiled with JetBrains decompiler
// Type: TaleWorlds.Core.MultiSelectionInquiryData
// Assembly: TaleWorlds.Core, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 43F7B45C-9AE1-4715-A37B-2364840F62AF
// Assembly location: E:\SteamLibrary\steamapps\common\Mount & Blade II Bannerlord\bin\Win64_Shipping_Client\TaleWorlds.Core.dll

using System;
using System.Collections.Generic;

namespace TaleWorlds.Core
{
  public class MultiSelectionInquiryData
  {
    public readonly string TitleText;
    public readonly string DescriptionText;
    public readonly List<InquiryElement> InquiryElements;
    public readonly bool IsExitShown;
    public readonly int MaxSelectableOptionCount;
    public readonly int MinSelectableOptionCount;
    public readonly string SoundEventPath;
    public readonly string AffirmativeText;
    public readonly string NegativeText;
    public readonly Action<List<InquiryElement>> AffirmativeAction;
    public readonly Action<List<InquiryElement>> NegativeAction;

    public MultiSelectionInquiryData(
      string titleText,
      string descriptionText,
      List<InquiryElement> inquiryElements,
      bool isExitShown,
      int maxSelectableOptionCount,
      string affirmativeText,
      string negativeText,
      Action<List<InquiryElement>> affirmativeAction,
      Action<List<InquiryElement>> negativeAction,
      string soundEventPath = "")
    {
      this.TitleText = titleText;
      this.DescriptionText = descriptionText;
      this.InquiryElements = inquiryElements;
      this.IsExitShown = isExitShown;
      this.AffirmativeText = affirmativeText;
      this.NegativeText = negativeText;
      this.AffirmativeAction = affirmativeAction;
      this.NegativeAction = negativeAction;
      this.MaxSelectableOptionCount = maxSelectableOptionCount;
      this.SoundEventPath = soundEventPath;
    }
  }
}
