// Decompiled with JetBrains decompiler
// Type: TaleWorlds.MountAndBlade.DetachmentData
// Assembly: TaleWorlds.MountAndBlade, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3C0E3F42-756F-4407-B817-06BA412A78B0
// Assembly location: E:\SteamLibrary\steamapps\common\Mount & Blade II Bannerlord\bin\Win64_Shipping_Client\TaleWorlds.MountAndBlade.dll

using System;
using System.Collections.Generic;
using System.Linq;

namespace TaleWorlds.MountAndBlade
{
  internal class DetachmentData
  {
    public List<Formation> joinedFormations = new List<Formation>();
    public List<AgentValuePair<float[]>> agentScores = new List<AgentValuePair<float[]>>();
    public float firstTime;

    public int AgentCount => this.joinedFormations.Sum<Formation>((Func<Formation, int>) (f => f.arrangement.UnitCount - (f.IsPlayerInFormation || f.HasPlayer ? 1 : 0)));

    public bool IsPrecalculated()
    {
      for (int index = this.agentScores.Count - 1; index >= 0; --index)
      {
        if (this.agentScores[index].Agent.IsDetachedFromFormation)
          this.agentScores.RemoveAt(index);
      }
      int count = this.agentScores.Count;
      return count > 0 && count >= this.AgentCount;
    }

    public DetachmentData() => this.firstTime = MBCommon.TimeType.Mission.GetTime();
  }
}
