// Decompiled with JetBrains decompiler
// Type: TaleWorlds.MountAndBlade.IMissionListener
// Assembly: TaleWorlds.MountAndBlade, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3C0E3F42-756F-4407-B817-06BA412A78B0
// Assembly location: E:\SteamLibrary\steamapps\common\Mount & Blade II Bannerlord\bin\Win64_Shipping_Client\TaleWorlds.MountAndBlade.dll

using TaleWorlds.Core;

namespace TaleWorlds.MountAndBlade
{
  public interface IMissionListener
  {
    void OnEquipItemsFromSpawnEquipmentBegin(Agent agent, Agent.CreationType creationType);

    void OnEquipItemsFromSpawnEquipment(Agent agent, Agent.CreationType creationType);

    void OnEndMission();

    void OnMissionModeChange(MissionMode oldMissionMode, bool atStart);

    void OnConversationCharacterChanged();

    void OnResetMission();
  }
}
