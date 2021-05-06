// Decompiled with JetBrains decompiler
// Type: TaleWorlds.Engine.TwoDimensionMeshDrawData
// Assembly: TaleWorlds.Engine, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: DD7DC4E8-6EB6-4CB8-8DE1-FC78E45F7FE8
// Assembly location: E:\SteamLibrary\steamapps\common\Mount & Blade II Bannerlord\bin\Win64_Shipping_Client\TaleWorlds.Engine.dll

using TaleWorlds.DotNet;
using TaleWorlds.Library;

namespace TaleWorlds.Engine
{
  [EngineStruct("rglTwo_dimension_mesh_draw_data")]
  public struct TwoDimensionMeshDrawData
  {
    public float DrawX;
    public float DrawY;
    public float ScreenWidth;
    public float ScreenHeight;
    public Vec2 ClipCircleCenter;
    public float ClipCircleRadius;
    public float ClipCircleSmoothingRadius;
    public uint Color;
    public float ColorFactor;
    public float AlphaFactor;
    public float HueFactor;
    public float SaturationFactor;
    public float ValueFactor;
    public float OverlayTextureWidth;
    public float OverlayTextureHeight;
    public Vec2 ClipRectPosition;
    public Vec2 ClipRectSize;
    public Vec2 StartCoordinate;
    public Vec2 Size;
    public int Layer;
    public float OverlayXOffset;
    public float OverlayYOffset;
    public float Width;
    public float Height;
    public float MinU;
    public float MinV;
    public float MaxU;
    public float MaxV;
    public uint Type;
  }
}
