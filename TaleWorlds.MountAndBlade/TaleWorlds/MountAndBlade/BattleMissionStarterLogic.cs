// Decompiled with JetBrains decompiler
// Type: TaleWorlds.MountAndBlade.BattleMissionStarterLogic
// Assembly: TaleWorlds.MountAndBlade, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3C0E3F42-756F-4407-B817-06BA412A78B0
// Assembly location: E:\SteamLibrary\steamapps\common\Mount & Blade II Bannerlord\bin\Win64_Shipping_Client\TaleWorlds.MountAndBlade.dll

using TaleWorlds.Core;

namespace TaleWorlds.MountAndBlade
{
  public class BattleMissionStarterLogic : MissionLogic
  {
    public BattleMissionStarterLogic()
    {
    }

    public BattleMissionStarterLogic(
      IMissionTroopSupplier defenderTroopSupplier = null,
      IMissionTroopSupplier attackerTroopSupplier = null)
    {
    }

    public override void AfterStart() => this.Mission.SetMissionMode(MissionMode.Battle, true);
  }
}
