// Decompiled with JetBrains decompiler
// Type: TaleWorlds.MountAndBlade.TacticRally
// Assembly: TaleWorlds.MountAndBlade, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3C0E3F42-756F-4407-B817-06BA412A78B0
// Assembly location: E:\SteamLibrary\steamapps\common\Mount & Blade II Bannerlord\bin\Win64_Shipping_Client\TaleWorlds.MountAndBlade.dll

using System.Collections.Generic;
using System.Linq;
using TaleWorlds.Library;

namespace TaleWorlds.MountAndBlade
{
  public class TacticRally : TacticComponent
  {
    private IEnumerable<Vec3> targets;

    public TacticRally(Team team, IEnumerable<Vec3> targets)
      : base(team)
    {
      List<Vec3> list = targets.ToList<Vec3>();
      list.Shuffle<Vec3>();
      this.targets = (IEnumerable<Vec3>) list;
    }
  }
}
