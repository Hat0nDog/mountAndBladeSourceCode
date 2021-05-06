// Decompiled with JetBrains decompiler
// Type: TaleWorlds.Engine.Intersection
// Assembly: TaleWorlds.Engine, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: DD7DC4E8-6EB6-4CB8-8DE1-FC78E45F7FE8
// Assembly location: E:\SteamLibrary\steamapps\common\Mount & Blade II Bannerlord\bin\Win64_Shipping_Client\TaleWorlds.Engine.dll

using System;
using TaleWorlds.DotNet;
using TaleWorlds.Library;

namespace TaleWorlds.Engine
{
  [EngineStruct("rglIntersection")]
  public struct Intersection
  {
    internal UIntPtr doNotUse;
    internal UIntPtr doNotUse2;
    public float Penetration;
    internal uint doNotUse3;
    public Vec3 IntersectionPoint;
    public Vec3 IntersectionNormal;

    public static bool DoSegmentsIntersect(
      Vec2 line1Start,
      Vec2 line1Direction,
      Vec2 line2Start,
      Vec2 line2Direction,
      ref Vec2 intersectionPoint)
    {
      return EngineApplicationInterface.IBodyPart.DoSegmentsIntersect(line1Start, line1Direction, line2Start, line2Direction, ref intersectionPoint);
    }
  }
}
