// Decompiled with JetBrains decompiler
// Type: TaleWorlds.Engine.IBodyPart
// Assembly: TaleWorlds.Engine, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: DD7DC4E8-6EB6-4CB8-8DE1-FC78E45F7FE8
// Assembly location: E:\SteamLibrary\steamapps\common\Mount & Blade II Bannerlord\bin\Win64_Shipping_Client\TaleWorlds.Engine.dll

using TaleWorlds.Library;

namespace TaleWorlds.Engine
{
  [ApplicationInterfaceBase]
  internal interface IBodyPart
  {
    [EngineMethod("do_segments_intersect", false)]
    bool DoSegmentsIntersect(
      Vec2 line1Start,
      Vec2 line1Direction,
      Vec2 line2Start,
      Vec2 line2Direction,
      ref Vec2 intersectionPoint);
  }
}
