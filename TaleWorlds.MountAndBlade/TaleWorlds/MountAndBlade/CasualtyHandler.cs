// Decompiled with JetBrains decompiler
// Type: TaleWorlds.MountAndBlade.CasualtyHandler
// Assembly: TaleWorlds.MountAndBlade, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3C0E3F42-756F-4407-B817-06BA412A78B0
// Assembly location: E:\SteamLibrary\steamapps\common\Mount & Blade II Bannerlord\bin\Win64_Shipping_Client\TaleWorlds.MountAndBlade.dll

using System.Collections.Generic;
using TaleWorlds.Core;

namespace TaleWorlds.MountAndBlade
{
  public class CasualtyHandler : MissionLogic
  {
    private readonly Dictionary<Formation, int> _casualtyCounts = new Dictionary<Formation, int>();
    private readonly Dictionary<Formation, float> _powerLoss = new Dictionary<Formation, float>();

    public override void OnAgentRemoved(
      Agent affectedAgent,
      Agent affectorAgent,
      AgentState agentState,
      KillingBlow killingBlow)
    {
      if (affectedAgent.Formation == null)
        return;
      if (this._casualtyCounts.ContainsKey(affectedAgent.Formation))
        this._casualtyCounts[affectedAgent.Formation]++;
      else
        this._casualtyCounts[affectedAgent.Formation] = 1;
      if (this._powerLoss.ContainsKey(affectedAgent.Formation))
        this._powerLoss[affectedAgent.Formation] += affectedAgent.Character.GetPower();
      else
        this._powerLoss[affectedAgent.Formation] = affectedAgent.Character.GetPower();
    }

    public int GetCasualtyCountOfFormation(Formation formation)
    {
      int num;
      if (!this._casualtyCounts.TryGetValue(formation, out num))
      {
        num = 0;
        this._casualtyCounts[formation] = 0;
      }
      return num;
    }

    public float GetCasualtyPowerLossOfFormation(Formation formation)
    {
      float num;
      if (!this._powerLoss.TryGetValue(formation, out num))
      {
        num = 0.0f;
        this._powerLoss[formation] = 0.0f;
      }
      return num;
    }
  }
}
