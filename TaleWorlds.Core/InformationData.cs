// Decompiled with JetBrains decompiler
// Type: TaleWorlds.Core.InformationData
// Assembly: TaleWorlds.Core, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 43F7B45C-9AE1-4715-A37B-2364840F62AF
// Assembly location: E:\SteamLibrary\steamapps\common\Mount & Blade II Bannerlord\bin\Win64_Shipping_Client\TaleWorlds.Core.dll

using System.Collections.Generic;
using TaleWorlds.Localization;
using TaleWorlds.SaveSystem;

namespace TaleWorlds.Core
{
  public abstract class InformationData
  {
    [SaveableField(2)]
    public readonly TextObject DescriptionText;

    protected virtual void AutoGeneratedInstanceCollectObjects(List<object> collectedObjects) => collectedObjects.Add((object) this.DescriptionText);

    internal static object AutoGeneratedGetMemberValueDescriptionText(object o) => (object) ((InformationData) o).DescriptionText;

    public abstract TextObject TitleText { get; }

    public abstract string SoundEventPath { get; }

    protected InformationData(TextObject description) => this.DescriptionText = description;
  }
}
