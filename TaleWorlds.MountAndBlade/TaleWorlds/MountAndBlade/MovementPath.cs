// Decompiled with JetBrains decompiler
// Type: TaleWorlds.MountAndBlade.MovementPath
// Assembly: TaleWorlds.MountAndBlade, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3C0E3F42-756F-4407-B817-06BA412A78B0
// Assembly location: E:\SteamLibrary\steamapps\common\Mount & Blade II Bannerlord\bin\Win64_Shipping_Client\TaleWorlds.MountAndBlade.dll

using System.Diagnostics;
using TaleWorlds.Library;

namespace TaleWorlds.MountAndBlade
{
  internal class MovementPath
  {
    private float[] _lineLengthAccumulations;
    private NavigationData _navigationData;

    private int LineCount => this._navigationData.PointSize - 1;

    public Vec2 InitialDirection { get; }

    public Vec2 FinalDirection { get; }

    public Vec3 Destination => this._navigationData.EndPoint;

    public MovementPath(NavigationData navigationData, Vec2 initialDirection, Vec2 finalDirection)
    {
      this._navigationData = navigationData;
      this.InitialDirection = initialDirection;
      this.FinalDirection = finalDirection;
    }

    public MovementPath(
      Vec3 currentPosition,
      Vec3 orderPosition,
      float agentRadius,
      Vec2 previousDirection,
      Vec2 finalDirection)
      : this(new NavigationData(currentPosition, orderPosition, agentRadius), previousDirection, finalDirection)
    {
    }

    private void UpdateLineLengths()
    {
      if (this._lineLengthAccumulations != null)
        return;
      this._lineLengthAccumulations = new float[this.LineCount];
      for (int index = 0; index < this.LineCount; ++index)
      {
        this._lineLengthAccumulations[index] = (this._navigationData.Points[index + 1] - this._navigationData.Points[index]).Length;
        if (index > 0)
          this._lineLengthAccumulations[index] += this._lineLengthAccumulations[index - 1];
      }
    }

    private float GetPathProggress(Vec2 point, int lineIndex)
    {
      this.UpdateLineLengths();
      float lengthAccumulation = this._lineLengthAccumulations[this.LineCount - 1];
      return (double) lengthAccumulation == 0.0 ? 1f : ((lineIndex > 0 ? this._lineLengthAccumulations[lineIndex - 1] : 0.0f) + (point - this._navigationData.Points[lineIndex]).Length) / lengthAccumulation;
    }

    private void GetClosestPointTo(Vec2 point, out Vec2 closest, out int lineIndex)
    {
      closest = Vec2.Invalid;
      lineIndex = -1;
      float num1 = float.MaxValue;
      for (int index = 0; index < this.LineCount; ++index)
      {
        Vec2 lineSegmentToPoint = MBMath.GetClosestPointInLineSegmentToPoint(point, this._navigationData.Points[index], this._navigationData.Points[index + 1]);
        float num2 = lineSegmentToPoint.DistanceSquared(point);
        if ((double) num2 < (double) num1)
        {
          num1 = num2;
          closest = lineSegmentToPoint;
          lineIndex = index;
        }
      }
    }

    [Conditional("DEBUG")]
    public void TickDebug(Vec2 position)
    {
      Vec2 closest;
      int lineIndex;
      this.GetClosestPointTo(position, out closest, out lineIndex);
      double num = (double) Vec2.Slerp(this.InitialDirection, this.FinalDirection, this.GetPathProggress(closest, lineIndex)).Normalize();
    }
  }
}
