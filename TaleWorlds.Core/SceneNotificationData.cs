// Decompiled with JetBrains decompiler
// Type: TaleWorlds.Core.SceneNotificationData
// Assembly: TaleWorlds.Core, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 43F7B45C-9AE1-4715-A37B-2364840F62AF
// Assembly location: E:\SteamLibrary\steamapps\common\Mount & Blade II Bannerlord\bin\Win64_Shipping_Client\TaleWorlds.Core.dll

using System;

namespace TaleWorlds.Core
{
  public class SceneNotificationData
  {
    public string TitleText;
    public string AffirmativeDescriptionText;
    public string NegativeDescriptionText;
    public string SceneID;
    public bool IsAffirmativeOptionShown;
    public bool IsNegativeOptionShown;
    public string SoundEventPath;
    public string AffirmativeText;
    public string NegativeText;
    public Action AffirmativeAction;
    public Action NegativeAction;
    public string AffirmativeHintText;

    public SceneNotificationData(
      string sceneID,
      string titleText,
      string affirmativeDescriptionText,
      string negativeDescriptionText,
      bool isAffirmativeOptionShown,
      bool isNegativeOptionShown,
      string affirmativeText,
      string negativeText,
      Action affirmativeAction,
      Action negativeAction,
      string soundEventPath = "",
      string affirmativeHintText = null)
    {
      this.SceneID = sceneID;
      this.TitleText = titleText;
      this.AffirmativeDescriptionText = affirmativeDescriptionText;
      this.NegativeDescriptionText = negativeDescriptionText;
      this.IsAffirmativeOptionShown = isAffirmativeOptionShown;
      this.IsNegativeOptionShown = isNegativeOptionShown;
      this.AffirmativeText = affirmativeText;
      this.NegativeText = negativeText;
      this.AffirmativeAction = affirmativeAction;
      this.NegativeAction = negativeAction;
      this.SoundEventPath = soundEventPath;
      this.AffirmativeHintText = affirmativeHintText;
    }
  }
}
