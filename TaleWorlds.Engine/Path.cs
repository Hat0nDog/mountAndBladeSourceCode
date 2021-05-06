// Decompiled with JetBrains decompiler
// Type: TaleWorlds.Engine.Path
// Assembly: TaleWorlds.Engine, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: DD7DC4E8-6EB6-4CB8-8DE1-FC78E45F7FE8
// Assembly location: E:\SteamLibrary\steamapps\common\Mount & Blade II Bannerlord\bin\Win64_Shipping_Client\TaleWorlds.Engine.dll

using System;
using TaleWorlds.DotNet;
using TaleWorlds.Library;

namespace TaleWorlds.Engine
{
  [EngineClass("rglPath")]
  public sealed class Path : NativeObject
  {
    public int NumberOfPoints => EngineApplicationInterface.IPath.GetNumberOfPoints(this.Pointer);

    public float TotalDistance => EngineApplicationInterface.IPath.GetTotalLength(this.Pointer);

    internal Path(UIntPtr pointer)
      : base(pointer)
    {
    }

    public MatrixFrame GetHermiteFrameForDt(float phase, int first_point)
    {
      MatrixFrame identity = MatrixFrame.Identity;
      EngineApplicationInterface.IPath.GetHermiteFrameForDt(this.Pointer, ref identity, phase, first_point);
      return identity;
    }

    public MatrixFrame GetFrameForDistance(float distance)
    {
      MatrixFrame identity = MatrixFrame.Identity;
      EngineApplicationInterface.IPath.GetHermiteFrameForDistance(this.Pointer, ref identity, distance);
      return identity;
    }

    public void GetFrameAndColorForDistance(float distance, out MatrixFrame frame, out Vec3 color)
    {
      frame = MatrixFrame.Identity;
      EngineApplicationInterface.IPath.GetHermiteFrameAndColorForDistance(this.Pointer, out frame, out color, distance);
    }

    public float GetArcLength(int first_point) => EngineApplicationInterface.IPath.GetArcLength(this.Pointer, first_point);

    public void GetPoints(MatrixFrame[] points) => EngineApplicationInterface.IPath.GetPoints(this.Pointer, points);

    public float GetTotalLength() => EngineApplicationInterface.IPath.GetTotalLength(this.Pointer);

    public int GetVersion() => EngineApplicationInterface.IPath.GetVersion(this.Pointer);

    public void SetFrameOfPoint(int pointIndex, ref MatrixFrame frame) => EngineApplicationInterface.IPath.SetFrameOfPoint(this.Pointer, pointIndex, ref frame);

    public void SetTangentPositionOfPoint(int pointIndex, int tangentIndex, ref Vec3 position) => EngineApplicationInterface.IPath.SetTangentPositionOfPoint(this.Pointer, pointIndex, tangentIndex, ref position);

    public int AddPathPoint(int newNodeIndex) => EngineApplicationInterface.IPath.AddPathPoint(this.Pointer, newNodeIndex);

    public void DeletePathPoint(int nodeIndex) => EngineApplicationInterface.IPath.DeletePathPoint(this.Pointer, nodeIndex);

    public string GetName() => EngineApplicationInterface.IPath.GetName(this.Pointer);
  }
}
