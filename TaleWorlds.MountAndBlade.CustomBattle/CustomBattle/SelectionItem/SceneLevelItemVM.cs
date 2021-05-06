// Decompiled with JetBrains decompiler
// Type: TaleWorlds.MountAndBlade.CustomBattle.CustomBattle.SelectionItem.SceneLevelItemVM
// Assembly: TaleWorlds.MountAndBlade.CustomBattle, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 8065FEE3-AE77-4E53-B13C-3890101BA431
// Assembly location: E:\SteamLibrary\steamapps\common\Mount & Blade II Bannerlord\Modules\CustomBattle\bin\Win64_Shipping_Client\TaleWorlds.MountAndBlade.CustomBattle.dll

using TaleWorlds.Core.ViewModelCollection;

namespace TaleWorlds.MountAndBlade.CustomBattle.CustomBattle.SelectionItem
{
  public class SceneLevelItemVM : SelectorItemVM
  {
    public int Level { get; private set; }

    public SceneLevelItemVM(int level)
      : base(level.ToString())
    {
      this.Level = level;
    }
  }
}
