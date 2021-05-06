// Decompiled with JetBrains decompiler
// Type: TaleWorlds.Library.PathFinder
// Assembly: TaleWorlds.Library, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 271D0DB8-3C5D-4850-A565-542B4CF7D347
// Assembly location: E:\SteamLibrary\steamapps\common\Mount & Blade II Bannerlord\bin\Win64_Shipping_Client\TaleWorlds.Library.dll

using System.Collections.Generic;

namespace TaleWorlds.Library
{
  public abstract class PathFinder
  {
    public static float BuildingCost = 5000f;
    public static float WaterCost = 400f;
    public static float ShallowWaterCost = 100f;

    public virtual void Destroy()
    {
    }

    public abstract void Initialize(Vec3 bbSize);

    public abstract bool FindPath(
      Vec3 wSource,
      Vec3 wDestination,
      List<Vec3> path,
      float craftWidth = 5f);
  }
}
