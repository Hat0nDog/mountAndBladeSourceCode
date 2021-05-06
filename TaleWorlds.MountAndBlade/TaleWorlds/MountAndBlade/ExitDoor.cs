// Decompiled with JetBrains decompiler
// Type: TaleWorlds.MountAndBlade.ExitDoor
// Assembly: TaleWorlds.MountAndBlade, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3C0E3F42-756F-4407-B817-06BA412A78B0
// Assembly location: E:\SteamLibrary\steamapps\common\Mount & Blade II Bannerlord\bin\Win64_Shipping_Client\TaleWorlds.MountAndBlade.dll

using TaleWorlds.Core;
using TaleWorlds.Engine;
using TaleWorlds.Localization;

namespace TaleWorlds.MountAndBlade
{
  public class ExitDoor : UsableMachine
  {
    private static readonly ActionIndexCache act_pickup_middle_begin = ActionIndexCache.Create(nameof (act_pickup_middle_begin));
    private static readonly ActionIndexCache act_pickup_middle_begin_left_stance = ActionIndexCache.Create(nameof (act_pickup_middle_begin_left_stance));
    private static readonly ActionIndexCache act_pickup_middle_end = ActionIndexCache.Create(nameof (act_pickup_middle_end));
    private static readonly ActionIndexCache act_pickup_middle_end_left_stance = ActionIndexCache.Create(nameof (act_pickup_middle_end_left_stance));

    public override TextObject GetActionTextForStandingPoint(
      UsableMissionObject usableGameObject)
    {
      TextObject textObject = new TextObject("{=gqQPSAQZ}({KEY}) Leave Area");
      textObject.SetTextVariable("KEY", Game.Current.GameTextManager.GetHotKeyGameText("CombatHotKeyCategory", 13));
      return textObject;
    }

    public override string GetDescriptionText(GameEntity gameEntity = null) => string.Empty;

    protected internal override void OnInit()
    {
      base.OnInit();
      this.SetScriptComponentToTick(this.GetTickRequirement());
    }

    protected override ScriptComponentBehaviour.TickRequirement GetTickRequirement() => ScriptComponentBehaviour.TickRequirement.Tick;

    protected internal override void OnTick(float dt)
    {
      base.OnTick(dt);
      foreach (StandingPoint standingPoint in this.StandingPoints)
      {
        if (standingPoint.HasUser)
        {
          Agent userAgent = standingPoint.UserAgent;
          ActionIndexCache currentAction1 = userAgent.GetCurrentAction(0);
          ActionIndexCache currentAction2 = userAgent.GetCurrentAction(1);
          if (!(currentAction2 == ActionIndexCache.act_none) || !(currentAction1 == ExitDoor.act_pickup_middle_begin) && !(currentAction1 == ExitDoor.act_pickup_middle_begin_left_stance))
          {
            if (currentAction2 == ActionIndexCache.act_none && (currentAction1 == ExitDoor.act_pickup_middle_end || currentAction1 == ExitDoor.act_pickup_middle_end_left_stance))
            {
              Mission.Current.EndMission();
              userAgent.StopUsingGameObject();
            }
            else if (currentAction2 != ActionIndexCache.act_none || !userAgent.SetActionChannel(0, userAgent.GetIsLeftStance() ? ExitDoor.act_pickup_middle_begin_left_stance : ExitDoor.act_pickup_middle_begin))
              userAgent.StopUsingGameObject();
          }
        }
      }
    }
  }
}
