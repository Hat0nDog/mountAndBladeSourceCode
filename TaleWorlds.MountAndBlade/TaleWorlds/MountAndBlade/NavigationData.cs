// Decompiled with JetBrains decompiler
// Type: TaleWorlds.MountAndBlade.NavigationData
// Assembly: TaleWorlds.MountAndBlade, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3C0E3F42-756F-4407-B817-06BA412A78B0
// Assembly location: E:\SteamLibrary\steamapps\common\Mount & Blade II Bannerlord\bin\Win64_Shipping_Client\TaleWorlds.MountAndBlade.dll

using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using TaleWorlds.DotNet;
using TaleWorlds.Library;

namespace TaleWorlds.MountAndBlade
{
  [EngineStruct("Navigation_data")]
  [Serializable]
  public struct NavigationData
  {
    private const int MaxPathSize = 1024;
    [MarshalAs(UnmanagedType.ByValArray, SizeConst = 1024)]
    public Vec2[] Points;
    public Vec3 StartPoint;
    public Vec3 EndPoint;
    public readonly int PointSize;
    public readonly float AgentRadius;

    public NavigationData(Vec3 startPoint, Vec3 endPoint, float agentRadius)
    {
      this.Points = new Vec2[1024];
      this.StartPoint = startPoint;
      this.EndPoint = endPoint;
      this.Points[0] = startPoint.AsVec2;
      this.Points[1] = endPoint.AsVec2;
      this.PointSize = 2;
      this.AgentRadius = agentRadius;
    }

    [Conditional("DEBUG")]
    public void TickDebug()
    {
      int num = 0;
      while (num < this.PointSize - 1)
        ++num;
    }
  }
}
