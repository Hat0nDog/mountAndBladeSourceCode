// Decompiled with JetBrains decompiler
// Type: TaleWorlds.MountAndBlade.Objects.Usables.SiegeMachineStonePile
// Assembly: TaleWorlds.MountAndBlade, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3C0E3F42-756F-4407-B817-06BA412A78B0
// Assembly location: E:\SteamLibrary\steamapps\common\Mount & Blade II Bannerlord\bin\Win64_Shipping_Client\TaleWorlds.MountAndBlade.dll

using TaleWorlds.Core;
using TaleWorlds.Engine;
using TaleWorlds.Localization;
using TaleWorlds.MountAndBlade.Objects.Siege;

namespace TaleWorlds.MountAndBlade.Objects.Usables
{
  public class SiegeMachineStonePile : UsableMachine, ISpawnable
  {
    private bool _spawnedFromSpawner;

    protected internal override void OnInit() => base.OnInit();

    public override TextObject GetActionTextForStandingPoint(
      UsableMissionObject usableGameObject)
    {
      if (!usableGameObject.GameEntity.HasTag(this.AmmoPickUpTag))
        return TextObject.Empty;
      TextObject textObject = new TextObject("{=jfcceEoE}{PILE_TYPE} Pile");
      textObject.SetTextVariable("PILE_TYPE", new TextObject("{=1CPdu9K0}Stone"));
      return textObject;
    }

    public override string GetDescriptionText(GameEntity gameEntity = null)
    {
      if (!gameEntity.HasTag(this.AmmoPickUpTag))
        return string.Empty;
      TextObject textObject = new TextObject("{=bNYm3K6b}({KEY}) Pick Up");
      textObject.SetTextVariable("KEY", Game.Current.GameTextManager.GetHotKeyGameText("CombatHotKeyCategory", 13));
      return textObject.ToString();
    }

    public void SetSpawnedFromSpawner() => this._spawnedFromSpawner = true;

    public override OrderType GetOrder(BattleSideEnum side) => OrderType.None;
  }
}
