// Decompiled with JetBrains decompiler
// Type: TaleWorlds.MountAndBlade.CustomBattle.CustomBattleSubModule
// Assembly: TaleWorlds.MountAndBlade.CustomBattle, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 8065FEE3-AE77-4E53-B13C-3890101BA431
// Assembly location: E:\SteamLibrary\steamapps\common\Mount & Blade II Bannerlord\Modules\CustomBattle\bin\Win64_Shipping_Client\TaleWorlds.MountAndBlade.CustomBattle.dll

using System;
using TaleWorlds.Localization;

namespace TaleWorlds.MountAndBlade.CustomBattle
{
  public class CustomBattleSubModule : MBSubModuleBase
  {
    protected override void OnSubModuleLoad()
    {
      base.OnSubModuleLoad();
      Module.CurrentModule.AddInitialStateOption(new InitialStateOption("CustomBattle", new TextObject("{=4gOGGbeQ}Custom Battle"), 5000, (Action) (() => MBGameManager.StartNewGame((MBGameManager) new CustomGameManager())), (Func<bool>) (() => false)));
    }
  }
}
