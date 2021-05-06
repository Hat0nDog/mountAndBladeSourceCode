// Decompiled with JetBrains decompiler
// Type: TaleWorlds.Engine.TwoDimensionTextMeshDrawData
// Assembly: TaleWorlds.Engine, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: DD7DC4E8-6EB6-4CB8-8DE1-FC78E45F7FE8
// Assembly location: E:\SteamLibrary\steamapps\common\Mount & Blade II Bannerlord\bin\Win64_Shipping_Client\TaleWorlds.Engine.dll

using TaleWorlds.DotNet;
using TaleWorlds.Library;

namespace TaleWorlds.Engine
{
  [EngineStruct("rglTwo_dimension_text_mesh_draw_data")]
  public struct TwoDimensionTextMeshDrawData
  {
    public float DrawX;
    public float DrawY;
    public float ScreenWidth;
    public float ScreenHeight;
    public uint Color;
    public float ScaleFactor;
    public float SmoothingConstant;
    public float ColorFactor;
    public float AlphaFactor;
    public float HueFactor;
    public float SaturationFactor;
    public float ValueFactor;
    public Vec2 ClipRectPosition;
    public Vec2 ClipRectSize;
    public uint GlowColor;
    public Vec3 OutlineColor;
    public float OutlineAmount;
    public float GlowRadius;
    public float Blur;
    public float ShadowOffset;
    public float ShadowAngle;
    public int Layer;
    public ulong HashCode1;
    public ulong HashCode2;
  }
}
