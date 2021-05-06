// Decompiled with JetBrains decompiler
// Type: TaleWorlds.Engine.WorldFrame
// Assembly: TaleWorlds.Engine, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: DD7DC4E8-6EB6-4CB8-8DE1-FC78E45F7FE8
// Assembly location: E:\SteamLibrary\steamapps\common\Mount & Blade II Bannerlord\bin\Win64_Shipping_Client\TaleWorlds.Engine.dll

using TaleWorlds.Library;

namespace TaleWorlds.Engine
{
  public struct WorldFrame
  {
    public Mat3 Rotation;
    public WorldPosition Origin;
    public static readonly WorldFrame Invalid = new WorldFrame(Mat3.Identity, WorldPosition.Invalid);

    public WorldFrame(Mat3 rotation, WorldPosition origin)
    {
      this.Rotation = rotation;
      this.Origin = origin;
    }

    public MatrixFrame ToGroundMatrixFrame() => new MatrixFrame(this.Rotation, this.Origin.GetGroundVec3());

    public MatrixFrame ToNavMeshMatrixFrame() => new MatrixFrame(this.Rotation, this.Origin.GetNavMeshVec3());
  }
}
