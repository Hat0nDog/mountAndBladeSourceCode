// Decompiled with JetBrains decompiler
// Type: TaleWorlds.Engine.WorldPosition
// Assembly: TaleWorlds.Engine, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: DD7DC4E8-6EB6-4CB8-8DE1-FC78E45F7FE8
// Assembly location: E:\SteamLibrary\steamapps\common\Mount & Blade II Bannerlord\bin\Win64_Shipping_Client\TaleWorlds.Engine.dll

using System;
using TaleWorlds.DotNet;
using TaleWorlds.Library;

namespace TaleWorlds.Engine
{
  [EngineStruct("rglWorld_position::Plain_world_position")]
  public struct WorldPosition
  {
    public UIntPtr Scene;
    public UIntPtr NavMesh;
    public UIntPtr NearestNavMesh;
    public Vec3 Position;
    public Vec3 Normal;
    public Vec2 LastValidZPosition;
    public ZValidityState State;
    public static readonly WorldPosition Invalid = new WorldPosition((TaleWorlds.Engine.Scene) null, UIntPtr.Zero, Vec3.Invalid, false);

    public WorldPosition(TaleWorlds.Engine.Scene scene, Vec3 position)
      : this(scene, UIntPtr.Zero, position, false)
    {
    }

    public WorldPosition(TaleWorlds.Engine.Scene scene, UIntPtr navMesh, Vec3 position, bool hasValidZ)
    {
      this.Scene = scene != null ? scene.Pointer : UIntPtr.Zero;
      this.NavMesh = navMesh;
      this.NearestNavMesh = this.NavMesh;
      this.Position = position;
      this.Normal = Vec3.Zero;
      if (hasValidZ)
      {
        this.LastValidZPosition = this.Position.AsVec2;
        this.State = ZValidityState.Valid;
      }
      else
      {
        this.LastValidZPosition = Vec2.Invalid;
        this.State = ZValidityState.Invalid;
      }
    }

    public void SetVec3(UIntPtr navMesh, Vec3 position, bool hasValidZ)
    {
      this.NavMesh = navMesh;
      this.NearestNavMesh = this.NavMesh;
      this.Position = position;
      this.Normal = Vec3.Zero;
      if (hasValidZ)
      {
        this.LastValidZPosition = this.Position.AsVec2;
        this.State = ZValidityState.Valid;
      }
      else
      {
        this.LastValidZPosition = Vec2.Invalid;
        this.State = ZValidityState.Invalid;
      }
    }

    public Vec2 AsVec2 => this.Position.AsVec2;

    public float X => this.Position.x;

    public float Y => this.Position.y;

    private void ValidateZ(ZValidityState minimumValidityState)
    {
      if (this.State >= minimumValidityState)
        return;
      EngineApplicationInterface.IScene.WorldPositionValidateZ(ref this, (int) minimumValidityState);
    }

    public UIntPtr GetNavMesh()
    {
      this.ValidateZ(ZValidityState.ValidAccordingToNavMesh);
      return this.NavMesh;
    }

    public float GetNavMeshZ()
    {
      this.ValidateZ(ZValidityState.ValidAccordingToNavMesh);
      return this.State >= ZValidityState.ValidAccordingToNavMesh ? this.Position.z : float.NaN;
    }

    public float GetGroundZ()
    {
      this.ValidateZ(ZValidityState.Valid);
      return this.State >= ZValidityState.Valid ? this.Position.z : float.NaN;
    }

    public Vec3 GetNavMeshVec3() => new Vec3(this.Position.AsVec2, this.GetNavMeshZ());

    public Vec3 GetGroundVec3() => new Vec3(this.Position.AsVec2, this.GetGroundZ());

    public void SetVec2(Vec2 value)
    {
      if (!(this.Position.AsVec2 != value))
        return;
      if (this.State != ZValidityState.Invalid)
        this.State = ZValidityState.Invalid;
      else if (!this.LastValidZPosition.IsValid)
      {
        this.ValidateZ(ZValidityState.ValidAccordingToNavMesh);
        this.State = ZValidityState.Invalid;
      }
      this.Position.x = value.x;
      this.Position.y = value.y;
    }

    public bool IsValid => this.AsVec2.IsValid && this.Scene != UIntPtr.Zero;
  }
}
