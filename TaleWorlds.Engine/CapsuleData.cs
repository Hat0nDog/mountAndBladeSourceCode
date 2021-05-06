// Decompiled with JetBrains decompiler
// Type: TaleWorlds.Engine.CapsuleData
// Assembly: TaleWorlds.Engine, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: DD7DC4E8-6EB6-4CB8-8DE1-FC78E45F7FE8
// Assembly location: E:\SteamLibrary\steamapps\common\Mount & Blade II Bannerlord\bin\Win64_Shipping_Client\TaleWorlds.Engine.dll

using System;
using TaleWorlds.DotNet;
using TaleWorlds.Library;

namespace TaleWorlds.Engine
{
  [EngineStruct("rglCapsule_data")]
  public struct CapsuleData
  {
    public Vec3 P1;
    public Vec3 P2;
    public float Radius;
    internal float LocalRadius;
    internal Vec3 LocalP1;
    internal Vec3 LocalP2;

    public CapsuleData(float radius, Vec3 p1, Vec3 p2)
    {
      this.Radius = radius;
      this.P1 = p1;
      this.P2 = p2;
      this.LocalRadius = radius;
      this.LocalP1 = p1;
      this.LocalP2 = p2;
    }

    public Vec3 GetBoxMin() => new Vec3(Math.Min(this.P1.x, this.P2.x) - this.Radius, Math.Min(this.P1.y, this.P2.y) - this.Radius, Math.Min(this.P1.z, this.P2.z) - this.Radius);

    public Vec3 GetBoxMax() => new Vec3(Math.Max(this.P1.x, this.P2.x) + this.Radius, Math.Max(this.P1.y, this.P2.y) + this.Radius, Math.Max(this.P1.z, this.P2.z) + this.Radius);
  }
}
