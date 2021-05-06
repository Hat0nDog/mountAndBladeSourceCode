// Decompiled with JetBrains decompiler
// Type: TaleWorlds.Engine.Ray
// Assembly: TaleWorlds.Engine, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: DD7DC4E8-6EB6-4CB8-8DE1-FC78E45F7FE8
// Assembly location: E:\SteamLibrary\steamapps\common\Mount & Blade II Bannerlord\bin\Win64_Shipping_Client\TaleWorlds.Engine.dll

using TaleWorlds.DotNet;
using TaleWorlds.Library;

namespace TaleWorlds.Engine
{
  [EngineStruct("rglRay")]
  public struct Ray
  {
    private Vec3 _origin;
    private Vec3 _direction;
    private float _maxDistance;

    public Vec3 Origin
    {
      get => this._origin;
      private set => this._origin = value;
    }

    public Vec3 Direction
    {
      get => this._direction;
      private set => this._direction = value;
    }

    public float MaxDistance
    {
      get => this._maxDistance;
      private set => this._maxDistance = value;
    }

    public Vec3 EndPoint => this.Origin + this.Direction * this.MaxDistance;

    public Ray(Vec3 origin, Vec3 direction, float maxDistance = 3.402823E+38f)
      : this()
    {
      this.Reset(origin, direction, maxDistance);
    }

    public void Reset(Vec3 origin, Vec3 direction, float maxDistance = 3.402823E+38f)
    {
      this._origin = origin;
      this._direction = direction;
      this._maxDistance = maxDistance;
      double num = (double) this._direction.Normalize();
    }
  }
}
