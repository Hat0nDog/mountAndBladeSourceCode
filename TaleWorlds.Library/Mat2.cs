// Decompiled with JetBrains decompiler
// Type: TaleWorlds.Library.Mat2
// Assembly: TaleWorlds.Library, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 271D0DB8-3C5D-4850-A565-542B4CF7D347
// Assembly location: E:\SteamLibrary\steamapps\common\Mount & Blade II Bannerlord\bin\Win64_Shipping_Client\TaleWorlds.Library.dll

using System;

namespace TaleWorlds.Library
{
  public struct Mat2
  {
    public Vec2 s;
    public Vec2 f;
    public static Mat2 Identity = new Mat2(1f, 0.0f, 0.0f, 1f);

    public Mat2(float sx, float sy, float fx, float fy)
    {
      this.s.x = sx;
      this.s.y = sy;
      this.f.x = fx;
      this.f.y = fy;
    }

    public void RotateCounterClockWise(float a)
    {
      float num1 = (float) Math.Sin((double) a);
      float num2 = (float) Math.Cos((double) a);
      Vec2 vec2_1 = this.s * num2 + this.f * num1;
      Vec2 vec2_2 = this.f * num2 - this.s * num1;
      this.s = vec2_1;
      this.f = vec2_2;
    }
  }
}
