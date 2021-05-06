// Decompiled with JetBrains decompiler
// Type: TaleWorlds.Engine.IPath
// Assembly: TaleWorlds.Engine, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: DD7DC4E8-6EB6-4CB8-8DE1-FC78E45F7FE8
// Assembly location: E:\SteamLibrary\steamapps\common\Mount & Blade II Bannerlord\bin\Win64_Shipping_Client\TaleWorlds.Engine.dll

using System;
using TaleWorlds.Library;

namespace TaleWorlds.Engine
{
  [ApplicationInterfaceBase]
  internal interface IPath
  {
    [EngineMethod("get_number_of_points", false)]
    int GetNumberOfPoints(UIntPtr ptr);

    [EngineMethod("get_hermite_frame_wrt_dt", false)]
    void GetHermiteFrameForDt(UIntPtr ptr, ref MatrixFrame frame, float phase, int firstPoint);

    [EngineMethod("get_hermite_frame_wrt_distance", false)]
    void GetHermiteFrameForDistance(UIntPtr ptr, ref MatrixFrame frame, float distance);

    [EngineMethod("get_hermite_frame_and_color_wrt_distance", false)]
    void GetHermiteFrameAndColorForDistance(
      UIntPtr ptr,
      out MatrixFrame frame,
      out Vec3 color,
      float distance);

    [EngineMethod("get_arc_length", false)]
    float GetArcLength(UIntPtr ptr, int firstPoint);

    [EngineMethod("get_points", false)]
    void GetPoints(UIntPtr ptr, MatrixFrame[] points);

    [EngineMethod("get_path_length", false)]
    float GetTotalLength(UIntPtr ptr);

    [EngineMethod("get_path_version", false)]
    int GetVersion(UIntPtr ptr);

    [EngineMethod("set_frame_of_point", false)]
    void SetFrameOfPoint(UIntPtr ptr, int pointIndex, ref MatrixFrame frame);

    [EngineMethod("set_tangent_position_of_point", false)]
    void SetTangentPositionOfPoint(
      UIntPtr ptr,
      int pointIndex,
      int tangentIndex,
      ref Vec3 position);

    [EngineMethod("add_path_point", false)]
    int AddPathPoint(UIntPtr ptr, int newNodeIndex);

    [EngineMethod("delete_path_point", false)]
    void DeletePathPoint(UIntPtr ptr, int newNodeIndex);

    [EngineMethod("get_name", false)]
    string GetName(UIntPtr ptr);
  }
}
