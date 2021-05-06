// Decompiled with JetBrains decompiler
// Type: TaleWorlds.Engine.GameEntityWithWorldPosition
// Assembly: TaleWorlds.Engine, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: DD7DC4E8-6EB6-4CB8-8DE1-FC78E45F7FE8
// Assembly location: E:\SteamLibrary\steamapps\common\Mount & Blade II Bannerlord\bin\Win64_Shipping_Client\TaleWorlds.Engine.dll

using System;
using TaleWorlds.DotNet;
using TaleWorlds.Library;

namespace TaleWorlds.Engine
{
  public class GameEntityWithWorldPosition
  {
    private readonly WeakNativeObjectReference _gameEntity;
    private WorldPosition _worldPosition;
    private Mat3 _orthonormalRotation;

    public GameEntityWithWorldPosition(GameEntity gameEntity)
    {
      this._gameEntity = new WeakNativeObjectReference((NativeObject) gameEntity);
      float heightAtPosition = gameEntity.Scene.GetGroundHeightAtPosition(gameEntity.GlobalPosition);
      this._worldPosition = new WorldPosition(gameEntity.Scene, UIntPtr.Zero, new Vec3(gameEntity.GlobalPosition.AsVec2, heightAtPosition), false);
      this._worldPosition.GetGroundVec3();
      this._orthonormalRotation = gameEntity.GetGlobalFrame().rotation;
      this._orthonormalRotation.Orthonormalize();
    }

    public GameEntity GameEntity => this._gameEntity?.GetNativeObject() as GameEntity;

    public WorldPosition WorldPosition
    {
      get
      {
        Vec3 origin = this.GameEntity.GetGlobalFrame().origin;
        if (!this._worldPosition.AsVec2.NearlyEquals(origin.AsVec2))
          this._worldPosition.SetVec3(UIntPtr.Zero, origin, false);
        return this._worldPosition;
      }
    }

    public void InvalidateWorldPosition() => this._worldPosition.State = ZValidityState.Invalid;

    public WorldFrame WorldFrame
    {
      get
      {
        Mat3 rotation = this.GameEntity.GetGlobalFrame().rotation;
        if (!rotation.NearlyEquals(this._orthonormalRotation))
        {
          this._orthonormalRotation = rotation;
          this._orthonormalRotation.Orthonormalize();
        }
        return new WorldFrame(this._orthonormalRotation, this.WorldPosition);
      }
    }
  }
}
