// Decompiled with JetBrains decompiler
// Type: TaleWorlds.MountAndBlade.CustomBattle.CustomBattle.SelectionItem.FactionItemVM
// Assembly: TaleWorlds.MountAndBlade.CustomBattle, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 8065FEE3-AE77-4E53-B13C-3890101BA431
// Assembly location: E:\SteamLibrary\steamapps\common\Mount & Blade II Bannerlord\Modules\CustomBattle\bin\Win64_Shipping_Client\TaleWorlds.MountAndBlade.CustomBattle.dll

using TaleWorlds.Core;
using TaleWorlds.Core.ViewModelCollection;

namespace TaleWorlds.MountAndBlade.CustomBattle.CustomBattle.SelectionItem
{
  public class FactionItemVM : SelectorItemVM
  {
    public BasicCultureObject Faction { get; private set; }

    public FactionItemVM(BasicCultureObject faction)
      : base(faction.Name.ToString())
    {
      this.Faction = faction;
    }
  }
}
