// Decompiled with JetBrains decompiler
// Type: TaleWorlds.Engine.MaterialFlags
// Assembly: TaleWorlds.Engine, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: DD7DC4E8-6EB6-4CB8-8DE1-FC78E45F7FE8
// Assembly location: E:\SteamLibrary\steamapps\common\Mount & Blade II Bannerlord\bin\Win64_Shipping_Client\TaleWorlds.Engine.dll

using System;

namespace TaleWorlds.Engine
{
  [Flags]
  public enum MaterialFlags : uint
  {
    RenderFrontToBack = 1,
    NoDepthTest = 2,
    DontDrawToDepthRenderTarget = 4,
    NoModifyDepthBuffer = 8,
    CullFrontFaces = 16, // 0x00000010
    TwoSided = 32, // 0x00000020
    AlphaBlendSort = 64, // 0x00000040
    DontOptimizeMesh = 128, // 0x00000080
    AlphaBlendNone = 0,
    AlphaBlendModulate = 256, // 0x00000100
    AlphaBlendAdd = 512, // 0x00000200
    AlphaBlendMultiply = AlphaBlendAdd | AlphaBlendModulate, // 0x00000300
    AlphaBlendFactor = 1792, // 0x00000700
    AlphaBlendMask = AlphaBlendFactor, // 0x00000700
    AlphaBlendBits = NoModifyDepthBuffer, // 0x00000008
    BillboardNone = 0,
    Billboard2d = 4096, // 0x00001000
    Billboard3d = 8192, // 0x00002000
    BillboardMask = Billboard3d | Billboard2d, // 0x00003000
    Skybox = 131072, // 0x00020000
    MultiPassAlpha = 262144, // 0x00040000
    GbufferAlphaBlend = 524288, // 0x00080000
    RequiresForwardRendering = 1048576, // 0x00100000
    AvoidRecomputationOfNormals = 2097152, // 0x00200000
    RenderOrderPlus1 = 150994944, // 0x09000000
    RenderOrderPlus2 = 167772160, // 0x0A000000
    RenderOrderPlus3 = 184549376, // 0x0B000000
    RenderOrderPlus4 = 201326592, // 0x0C000000
    RenderOrderPlus5 = 218103808, // 0x0D000000
    RenderOrderPlus6 = 234881024, // 0x0E000000
    RenderOrderPlus7 = 251658240, // 0x0F000000
    GreaterDepthNoWrite = 268435456, // 0x10000000
    AlwaysDepthTest = 536870912, // 0x20000000
    RenderToAmbientOcclusionBuffer = 1073741824, // 0x40000000
  }
}
