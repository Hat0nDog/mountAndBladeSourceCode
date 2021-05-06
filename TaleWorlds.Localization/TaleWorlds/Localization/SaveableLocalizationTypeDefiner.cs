// Decompiled with JetBrains decompiler
// Type: TaleWorlds.Localization.SaveableLocalizationTypeDefiner
// Assembly: TaleWorlds.Localization, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 26BB3E5A-EB48-4ABD-B2FC-10EF6D7A7285
// Assembly location: E:\SteamLibrary\steamapps\common\Mount & Blade II Bannerlord\bin\Win64_Shipping_Client\TaleWorlds.Localization.dll

using System.Collections.Generic;
using TaleWorlds.SaveSystem;

namespace TaleWorlds.Localization
{
  public class SaveableLocalizationTypeDefiner : SaveableTypeDefiner
  {
    public SaveableLocalizationTypeDefiner()
      : base(20000)
    {
    }

    protected override void DefineClassTypes() => this.AddClassDefinition(typeof (TextObject), 1);

    protected override void DefineContainerDefinitions() => this.ConstructContainerDefinition(typeof (Dictionary<string, TextObject>));
  }
}
