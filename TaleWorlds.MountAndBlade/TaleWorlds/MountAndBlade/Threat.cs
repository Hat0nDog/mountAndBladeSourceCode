// Decompiled with JetBrains decompiler
// Type: TaleWorlds.MountAndBlade.Threat
// Assembly: TaleWorlds.MountAndBlade, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3C0E3F42-756F-4407-B817-06BA412A78B0
// Assembly location: E:\SteamLibrary\steamapps\common\Mount & Blade II Bannerlord\bin\Win64_Shipping_Client\TaleWorlds.MountAndBlade.dll

using System.Diagnostics;
using TaleWorlds.Library;

namespace TaleWorlds.MountAndBlade
{
  public class Threat
  {
    public ITargetable WeaponEntity;
    public Formation Formation;
    public Agent Agent;
    public float ThreatValue;

    public override int GetHashCode() => base.GetHashCode();

    public string Name
    {
      get
      {
        if (this.WeaponEntity != null)
          return this.WeaponEntity.Entity().Name;
        if (this.Agent != null)
          return this.Agent.Name.ToString();
        return this.Formation != null ? this.Formation.ToString() : "Invalid";
      }
    }

    public Vec3 Position
    {
      get
      {
        if (this.WeaponEntity != null)
          return (this.WeaponEntity.GetTargetEntity().GlobalBoxMax + this.WeaponEntity.GetTargetEntity().GlobalBoxMin) * 0.5f;
        if (this.Agent != null)
          return this.Agent.CollisionCapsuleCenter;
        return this.Formation != null ? this.Formation.GetMedianAgent(false, false, this.Formation.GetAveragePositionOfUnits(false, false)).Position : Vec3.Invalid;
      }
    }

    public Vec3 BoundingBoxMin
    {
      get
      {
        if (this.WeaponEntity != null)
          return this.WeaponEntity.GetTargetEntity().GlobalBoxMin;
        if (this.Agent != null)
          return this.Agent.CollisionCapsule.GetBoxMin();
        Formation formation = this.Formation;
        return Vec3.Invalid;
      }
    }

    public Vec3 BoundingBoxMax
    {
      get
      {
        if (this.WeaponEntity != null)
          return this.WeaponEntity.GetTargetEntity().GlobalBoxMax;
        if (this.Agent != null)
          return this.Agent.CollisionCapsule.GetBoxMax();
        Formation formation = this.Formation;
        return Vec3.Invalid;
      }
    }

    public override bool Equals(object obj) => obj is Threat threat && this.WeaponEntity == threat.WeaponEntity && this.Formation == threat.Formation;

    [Conditional("DEBUG")]
    public void DisplayDebugInfo()
    {
    }
  }
}
