// Decompiled with JetBrains decompiler
// Type: TaleWorlds.MountAndBlade.CustomBattle.CustomBattleAutoBlockModel
// Assembly: TaleWorlds.MountAndBlade.CustomBattle, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 8065FEE3-AE77-4E53-B13C-3890101BA431
// Assembly location: E:\SteamLibrary\steamapps\common\Mount & Blade II Bannerlord\Modules\CustomBattle\bin\Win64_Shipping_Client\TaleWorlds.MountAndBlade.CustomBattle.dll

using System.Collections.Generic;
using TaleWorlds.Library;
using TaleWorlds.MountAndBlade.ComponentInterfaces;

namespace TaleWorlds.MountAndBlade.CustomBattle
{
  public class CustomBattleAutoBlockModel : AutoBlockModel
  {
    public override Agent.UsageDirection GetBlockDirection(Mission mission)
    {
      Agent mainAgent = mission.MainAgent;
      float num1 = float.MinValue;
      Agent.UsageDirection usageDirection = Agent.UsageDirection.AttackDown;
      foreach (Agent agent in (IEnumerable<Agent>) mission.Agents)
      {
        if (agent.IsHuman)
        {
          switch (agent.GetCurrentActionStage(1))
          {
            case Agent.ActionStage.AttackReady:
            case Agent.ActionStage.AttackQuickReady:
            case Agent.ActionStage.AttackRelease:
              if (agent.IsEnemyOf(mainAgent))
              {
                Vec3 v1 = agent.Position - mainAgent.Position;
                float num2 = v1.Normalize();
                double num3 = (double) MBMath.ClampFloat(Vec3.DotProduct(v1, mainAgent.LookDirection) + 0.8f, 0.0f, 1f);
                float num4 = MBMath.ClampFloat((float) (1.0 / ((double) num2 + 0.5)), 0.0f, 1f);
                float num5 = MBMath.ClampFloat((float) (-(double) Vec3.DotProduct(v1, agent.LookDirection) + 0.5), 0.0f, 1f);
                double num6 = (double) num4;
                float num7 = (float) (num3 * num6) * num5;
                if ((double) num7 > (double) num1)
                {
                  num1 = num7;
                  usageDirection = agent.GetCurrentActionDirection(1);
                  if (usageDirection == Agent.UsageDirection.None)
                  {
                    usageDirection = Agent.UsageDirection.AttackDown;
                    continue;
                  }
                  continue;
                }
                continue;
              }
              continue;
            default:
              continue;
          }
        }
      }
      return usageDirection;
    }
  }
}
