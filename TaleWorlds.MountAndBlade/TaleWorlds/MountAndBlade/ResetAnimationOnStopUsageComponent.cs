// Decompiled with JetBrains decompiler
// Type: TaleWorlds.MountAndBlade.ResetAnimationOnStopUsageComponent
// Assembly: TaleWorlds.MountAndBlade, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3C0E3F42-756F-4407-B817-06BA412A78B0
// Assembly location: E:\SteamLibrary\steamapps\common\Mount & Blade II Bannerlord\bin\Win64_Shipping_Client\TaleWorlds.MountAndBlade.dll

namespace TaleWorlds.MountAndBlade
{
  public class ResetAnimationOnStopUsageComponent : UsableMissionObjectComponent
  {
    private readonly ActionIndexCache _successfulResetAction = ActionIndexCache.act_none;

    public ResetAnimationOnStopUsageComponent(ActionIndexCache successfulResetActionCode) => this._successfulResetAction = successfulResetActionCode;

    protected internal override void OnUseStopped(Agent userAgent, bool isSuccessful = true)
    {
      ActionIndexCache actionIndexCache = isSuccessful ? this._successfulResetAction : ActionIndexCache.act_none;
      if (actionIndexCache == ActionIndexCache.act_none)
        userAgent.SetActionChannel(1, actionIndexCache, additionalFlags: 72UL);
      userAgent.SetActionChannel(0, actionIndexCache, additionalFlags: 72UL);
    }
  }
}
