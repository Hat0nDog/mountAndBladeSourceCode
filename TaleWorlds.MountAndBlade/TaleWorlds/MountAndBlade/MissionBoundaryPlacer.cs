// Decompiled with JetBrains decompiler
// Type: TaleWorlds.MountAndBlade.MissionBoundaryPlacer
// Assembly: TaleWorlds.MountAndBlade, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3C0E3F42-756F-4407-B817-06BA412A78B0
// Assembly location: E:\SteamLibrary\steamapps\common\Mount & Blade II Bannerlord\bin\Win64_Shipping_Client\TaleWorlds.MountAndBlade.dll

using System;
using System.Collections.Generic;
using System.Linq;
using TaleWorlds.Engine;
using TaleWorlds.Library;

namespace TaleWorlds.MountAndBlade
{
  public class MissionBoundaryPlacer : MissionLogic
  {
    public override void EarlyStart()
    {
      base.EarlyStart();
      this.AddBoundaries();
    }

    public void AddBoundaries()
    {
      IEnumerable<GameEntity> entitiesWithTag = this.Mission.Scene.FindEntitiesWithTag("walk_area_vertex");
      if (entitiesWithTag.Count<GameEntity>() >= 3)
      {
        this.Mission.Boundaries.Add("walk_area", (ICollection<Vec2>) entitiesWithTag.Select<GameEntity, Vec2>((Func<GameEntity, Vec2>) (v => v.GlobalPosition.AsVec2)).ToList<Vec2>());
      }
      else
      {
        Vec3 min;
        Vec3 max;
        this.Mission.Scene.GetBoundingBox(out min, out max);
        float num1 = Math.Min(2f, max.x - min.x);
        float num2 = Math.Min(2f, max.y - min.y);
        this.Mission.Boundaries.Add("scene_boundary", (ICollection<Vec2>) new List<Vec2>()
        {
          new Vec2(min.x + num1, min.y + num2),
          new Vec2(max.x - num1, min.y + num2),
          new Vec2(max.x - num1, max.y - num2),
          new Vec2(min.x + num1, max.y - num2)
        });
      }
    }
  }
}
