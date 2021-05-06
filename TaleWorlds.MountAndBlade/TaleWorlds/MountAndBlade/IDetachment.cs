// Decompiled with JetBrains decompiler
// Type: TaleWorlds.MountAndBlade.IDetachment
// Assembly: TaleWorlds.MountAndBlade, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3C0E3F42-756F-4407-B817-06BA412A78B0
// Assembly location: E:\SteamLibrary\steamapps\common\Mount & Blade II Bannerlord\bin\Win64_Shipping_Client\TaleWorlds.MountAndBlade.dll

using System.Collections.Generic;
using TaleWorlds.Core;
using TaleWorlds.Engine;

namespace TaleWorlds.MountAndBlade
{
  public interface IDetachment
  {
    IEnumerable<Agent> Agents { get; }

    float? GetWeightOfNextSlot(BattleSideEnum side);

    float GetDetachmentWeight(BattleSideEnum side);

    List<(int, float)> GetSlotIndexWeightTuples();

    bool IsSlotAtIndexAvailableForAgent(int slotIndex, Agent agent);

    bool IsAgentEligible(Agent agent);

    void AddAgentAtSlotIndex(Agent agent, int slotIndex);

    Agent GetMovingAgentAtSlotIndex(int slotIndex);

    void MarkSlotAtIndex(int slotIndex);

    bool IsDetachmentRecentlyEvaluated();

    void UnmarkDetachment();

    float? GetWeightOfAgentAtNextSlot(IEnumerable<Agent> candidates, out Agent match);

    float? GetWeightOfAgentAtNextSlot(
      IEnumerable<AgentValuePair<float>> agentTemplateScores,
      out Agent match);

    float GetTemplateWeightOfAgent(Agent candidate);

    float[] GetTemplateCostsOfAgent(Agent candidate, float[] oldValue);

    float GetExactCostOfAgentAtSlot(Agent candidate, int slotIndex);

    float GetWeightOfOccupiedSlot(Agent detachedAgent);

    float? GetWeightOfAgentAtOccupiedSlot(
      Agent detachedAgent,
      IEnumerable<Agent> candidates,
      out Agent match);

    bool IsStandingPointAvailableForAgent(Agent agent);

    void AddAgent(Agent agent, int slotIndex = -1);

    void RemoveAgent(Agent detachedAgent);

    WorldFrame? GetAgentFrame(Agent detachedAgent);

    bool IsLoose { get; }
  }
}
