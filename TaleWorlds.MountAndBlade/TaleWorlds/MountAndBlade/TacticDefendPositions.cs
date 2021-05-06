// Decompiled with JetBrains decompiler
// Type: TaleWorlds.MountAndBlade.TacticDefendPositions
// Assembly: TaleWorlds.MountAndBlade, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3C0E3F42-756F-4407-B817-06BA412A78B0
// Assembly location: E:\SteamLibrary\steamapps\common\Mount & Blade II Bannerlord\bin\Win64_Shipping_Client\TaleWorlds.MountAndBlade.dll

using System.Collections.Generic;
using TaleWorlds.Library;

namespace TaleWorlds.MountAndBlade
{
  public class TacticDefendPositions : TacticComponent
  {
    private Mission mission;
    private IEnumerable<Vec3> targets;
    public float[] ThreatLevelBorderDistances = new float[3]
    {
      5f,
      20f,
      50f
    };

    public TacticDefendPositions(
      Team team,
      Mission mission,
      IEnumerable<Vec3> targets,
      float[] threatLevelBorderDistances = null)
      : base(team)
    {
      this.mission = mission;
      this.targets = targets;
      if (threatLevelBorderDistances != null)
        this.ThreatLevelBorderDistances = threatLevelBorderDistances;
      int num = 0;
      while (num < this.ThreatLevelBorderDistances.Length - 2)
        ++num;
    }
  }
}
