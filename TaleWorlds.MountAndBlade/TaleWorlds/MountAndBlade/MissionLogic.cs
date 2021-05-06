// Decompiled with JetBrains decompiler
// Type: TaleWorlds.MountAndBlade.MissionLogic
// Assembly: TaleWorlds.MountAndBlade, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3C0E3F42-756F-4407-B817-06BA412A78B0
// Assembly location: E:\SteamLibrary\steamapps\common\Mount & Blade II Bannerlord\bin\Win64_Shipping_Client\TaleWorlds.MountAndBlade.dll

using TaleWorlds.Core;

namespace TaleWorlds.MountAndBlade
{
  public abstract class MissionLogic : MissionBehaviour
  {
    public override MissionBehaviourType BehaviourType => MissionBehaviourType.Logic;

    public virtual bool MissionEnded(ref MissionResult missionResult) => false;

    public virtual InquiryData OnEndMissionRequest(out bool canLeave)
    {
      canLeave = true;
      return (InquiryData) null;
    }

    public virtual void OnBattleEnded()
    {
    }

    public virtual void ShowBattleResults()
    {
    }

    public virtual void AccelerateHorseKeyPressAnswer()
    {
    }

    public virtual void OnRetreatMission()
    {
    }

    public virtual bool IsAgentInteractionAllowed() => true;

    public virtual bool IsOrderShoutingAllowed() => true;

    public virtual void OnMissionResultReady(MissionResult missionResult)
    {
    }
  }
}
