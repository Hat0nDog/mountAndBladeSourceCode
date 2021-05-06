// Decompiled with JetBrains decompiler
// Type: TaleWorlds.MountAndBlade.IMissionSystemHandler
// Assembly: TaleWorlds.MountAndBlade, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3C0E3F42-756F-4407-B817-06BA412A78B0
// Assembly location: E:\SteamLibrary\steamapps\common\Mount & Blade II Bannerlord\bin\Win64_Shipping_Client\TaleWorlds.MountAndBlade.dll

using System.Collections.Generic;

namespace TaleWorlds.MountAndBlade
{
  public interface IMissionSystemHandler
  {
    void OnPreLoad(Mission mission);

    void OnMissionAfterStarting(Mission mission);

    void OnMissionLoadingFinished(Mission mission);

    void BeforeMissionTick(Mission mission, float realDt);

    void AfterMissionTick(Mission mission, float realDt);

    void UpdateCamera(Mission mission, float realDt);

    bool RenderIsReady();

    IEnumerable<MissionBehaviour> OnAddBehaviours(
      IEnumerable<MissionBehaviour> behaviours,
      Mission mission,
      string missionName,
      bool addDefaultMissionBehaviours);
  }
}
