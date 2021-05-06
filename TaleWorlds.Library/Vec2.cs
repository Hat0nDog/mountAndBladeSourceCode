// Decompiled with JetBrains decompiler
// Type: TaleWorlds.Library.Vec2
// Assembly: TaleWorlds.Library, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 271D0DB8-3C5D-4850-A565-542B4CF7D347
// Assembly location: E:\SteamLibrary\steamapps\common\Mount & Blade II Bannerlord\bin\Win64_Shipping_Client\TaleWorlds.Library.dll

using System;
using System.Numerics;

namespace TaleWorlds.Library
{
  [Serializable]
  public struct Vec2
  {
    public float x;
    public float y;
    public static readonly Vec2 Side = new Vec2(1f, 0.0f);
    public static readonly Vec2 Forward = new Vec2(0.0f, 1f);
    public static readonly Vec2 One = new Vec2(1f, 1f);
    public static readonly Vec2 Zero = new Vec2(0.0f, 0.0f);
    public static readonly Vec2 Invalid = new Vec2(float.NaN, float.NaN);

    public float X => this.x;

    public float Y => this.y;

    public Vec2(float a, float b)
    {
      this.x = a;
      this.y = b;
    }

    public Vec2(Vec2 v)
    {
      this.x = v.x;
      this.y = v.y;
    }

    public Vec2(Vector2 v)
    {
      this.x = v.X;
      this.y = v.Y;
    }

    public Vec3 ToVec3(float z = 0.0f) => new Vec3(this.x, this.y, z);

    public static explicit operator Vector2(Vec2 vec2) => new Vector2(vec2.x, vec2.y);

    public static implicit operator Vec2(Vector2 vec2) => new Vec2(vec2.X, vec2.Y);

    public float this[int i]
    {
      get
      {
        if (i == 0)
          return this.x;
        if (i == 1)
          return this.y;
        throw new IndexOutOfRangeException("Vec2 out of bounds.");
      }
      set
      {
        if (i != 0)
        {
          if (i != 1)
            return;
          this.y = value;
        }
        else
          this.x = value;
      }
    }

    public float Normalize()
    {
      float length = this.Length;
      if ((double) length > 9.99999974737875E-06)
      {
        this.x /= length;
        this.y /= length;
      }
      else
      {
        this.x = 0.0f;
        this.y = 1f;
      }
      return length;
    }

    public Vec2 Normalized()
    {
      Vec2 vec2 = this;
      double num = (double) vec2.Normalize();
      return vec2;
    }

    public static WindingOrder GetWindingOrder(Vec2 first, Vec2 second, Vec2 third)
    {
      Vec2 vb = second - first;
      float num = Vec2.CCW(third - second, vb);
      if ((double) num > 0.0)
        return WindingOrder.Ccw;
      return (double) num < 0.0 ? WindingOrder.Cw : WindingOrder.None;
    }

    public static float CCW(Vec2 va, Vec2 vb) => (float) ((double) va.x * (double) vb.y - (double) va.y * (double) vb.x);

    public float Length => (float) Math.Sqrt((double) this.x * (double) this.x + (double) this.y * (double) this.y);

    public float LengthSquared => (float) ((double) this.x * (double) this.x + (double) this.y * (double) this.y);

    public override bool Equals(object obj) => obj != null && !(this.GetType() != obj.GetType()) && (double) ((Vec2) obj).x == (double) this.x && (double) ((Vec2) obj).y == (double) this.y;

    public override int GetHashCode() => (int) (1001.0 * (double) this.x + 10039.0 * (double) this.y);

    public static bool operator ==(Vec2 v1, Vec2 v2) => (double) v1.x == (double) v2.x && (double) v1.y == (double) v2.y;

    public static bool operator !=(Vec2 v1, Vec2 v2) => (double) v1.x != (double) v2.x || (double) v1.y != (double) v2.y;

    public static Vec2 operator -(Vec2 v) => new Vec2(-v.x, -v.y);

    public static Vec2 operator +(Vec2 v1, Vec2 v2) => new Vec2(v1.x + v2.x, v1.y + v2.y);

    public static Vec2 operator -(Vec2 v1, Vec2 v2) => new Vec2(v1.x - v2.x, v1.y - v2.y);

    public static Vec2 operator *(Vec2 v, float f) => new Vec2(v.x * f, v.y * f);

    public static Vec2 operator *(float f, Vec2 v) => new Vec2(v.x * f, v.y * f);

    public bool IsUnit()
    {
      float length = this.Length;
      return (double) length > 0.95 && (double) length < 1.05;
    }

    public bool IsNonZero()
    {
      float num = 1E-05f;
      return (double) this.x > (double) num || (double) this.x < -(double) num || (double) this.y > (double) num || (double) this.y < -(double) num;
    }

    public bool NearlyEquals(Vec2 v, float epsilon = 1E-05f) => (double) Math.Abs(this.x - v.x) < (double) epsilon && (double) Math.Abs(this.y - v.y) < (double) epsilon;

    public void RotateCCW(float angleInRadians)
    {
      float num1 = (float) Math.Sin((double) angleInRadians);
      float num2 = (float) Math.Cos((double) angleInRadians);
      float num3 = (float) ((double) this.x * (double) num2 - (double) this.y * (double) num1);
      this.y = (float) ((double) this.y * (double) num2 + (double) this.x * (double) num1);
      this.x = num3;
    }

    public float DotProduct(Vec2 v) => (float) ((double) v.x * (double) this.x + (double) v.y * (double) this.y);

    public static float DotProduct(Vec2 va, Vec2 vb) => (float) ((double) va.x * (double) vb.x + (double) va.y * (double) vb.y);

    public float RotationInRadians => (float) Math.Atan2(-(double) this.x, (double) this.y);

    public static Vec2 FromRotation(float rotation) => new Vec2(-(float) Math.Sin((double) rotation), (float) Math.Cos((double) rotation));

    public Vec2 TransformToLocalUnitF(Vec2 a) => new Vec2((float) ((double) this.y * (double) a.x - (double) this.x * (double) a.y), (float) ((double) this.x * (double) a.x + (double) this.y * (double) a.y));

    public Vec2 TransformToParentUnitF(Vec2 a) => new Vec2((float) ((double) this.y * (double) a.x + (double) this.x * (double) a.y), (float) (-(double) this.x * (double) a.x + (double) this.y * (double) a.y));

    public Vec2 TransformToLocalUnitFLeftHanded(Vec2 a) => new Vec2((float) (-(double) this.y * (double) a.x + (double) this.x * (double) a.y), (float) ((double) this.x * (double) a.x + (double) this.y * (double) a.y));

    public Vec2 TransformToParentUnitFLeftHanded(Vec2 a) => new Vec2((float) (-(double) this.y * (double) a.x + (double) this.x * (double) a.y), (float) ((double) this.x * (double) a.x + (double) this.y * (double) a.y));

    public Vec2 RightVec() => new Vec2(this.y, -this.x);

    public Vec2 LeftVec() => new Vec2(-this.y, this.x);

    public static Vec2 Max(Vec2 v1, Vec2 v2) => new Vec2(Math.Max(v1.x, v2.x), Math.Max(v1.y, v2.y));

    public static Vec2 Max(Vec2 v1, float f) => new Vec2(Math.Max(v1.x, f), Math.Max(v1.y, f));

    public static Vec2 Min(Vec2 v1, Vec2 v2) => new Vec2(Math.Min(v1.x, v2.x), Math.Min(v1.y, v2.y));

    public static Vec2 Min(Vec2 v1, float f) => new Vec2(Math.Min(v1.x, f), Math.Min(v1.y, f));

    public override string ToString() => "(Vec2) X: " + (object) this.x + " Y: " + (object) this.y;

    public float DistanceSquared(Vec2 v) => (float) (((double) v.x - (double) this.x) * ((double) v.x - (double) this.x) + ((double) v.y - (double) this.y) * ((double) v.y - (double) this.y));

    public float Distance(Vec2 v) => (float) Math.Sqrt(((double) v.x - (double) this.x) * ((double) v.x - (double) this.x) + ((double) v.y - (double) this.y) * ((double) v.y - (double) this.y));

    public static float DistanceToLine(Vec2 line1, Vec2 line2, Vec2 point) => Math.Abs((float) (((double) line2.x - (double) line1.x) * ((double) line1.y - (double) point.y) - ((double) line1.x - (double) point.x) * ((double) line2.y - (double) line1.y))) / (float) Math.Sqrt(Math.Pow((double) line2.x - (double) line1.x, 2.0) + Math.Pow((double) line2.y - (double) line1.y, 2.0));

    public static float DistanceToLineSegmentSquared(Vec2 line1, Vec2 line2, Vec2 point) => point.DistanceSquared(MBMath.GetClosestPointInLineSegmentToPoint(point, line1, line2));

    public float DistanceToLineSegment(Vec2 v, Vec2 w, out Vec2 closestPointOnLineSegment) => MathF.Sqrt(this.DistanceSquaredToLineSegment(v, w, out closestPointOnLineSegment));

    public float DistanceSquaredToLineSegment(Vec2 v, Vec2 w, out Vec2 closestPointOnLineSegment)
    {
      Vec2 vec2_1 = this;
      float num1 = v.DistanceSquared(w);
      if ((double) num1 == 0.0)
      {
        closestPointOnLineSegment = v;
      }
      else
      {
        float num2 = Vec2.DotProduct(vec2_1 - v, w - v) / num1;
        if ((double) num2 < 0.0)
          closestPointOnLineSegment = v;
        else if ((double) num2 > 1.0)
        {
          closestPointOnLineSegment = w;
        }
        else
        {
          Vec2 vec2_2 = v + (w - v) * num2;
          closestPointOnLineSegment = vec2_2;
        }
      }
      return vec2_1.DistanceSquared(closestPointOnLineSegment);
    }

    public static Vec2 Lerp(Vec2 v1, Vec2 v2, float alpha) => v1 * (1f - alpha) + v2 * alpha;

    public static Vec2 Slerp(Vec2 start, Vec2 end, float percent)
    {
      float num1 = MBMath.ClampFloat(Vec2.DotProduct(start, end), -1f, 1f);
      float num2 = (float) Math.Acos((double) num1) * percent;
      Vec2 vec2 = end - start * num1;
      double num3 = (double) vec2.Normalize();
      return start * (float) Math.Cos((double) num2) + vec2 * (float) Math.Sin((double) num2);
    }

    public float AngleBetween(Vec2 vector2) => (float) Math.Atan2((double) this.x * (double) vector2.y - (double) vector2.x * (double) this.y, (double) this.x * (double) vector2.x + (double) this.y * (double) vector2.y);

    public bool IsValid => !float.IsNaN(this.x) && !float.IsNaN(this.y) && !float.IsInfinity(this.x) && !float.IsInfinity(this.y);

    public static int SideOfLine(Vec2 point, Vec2 line1, Vec2 line2) => Math.Sign((float) (((double) line2.x - (double) line1.x) * ((double) point.y - (double) line1.y) - ((double) point.x - (double) line1.x) * ((double) line2.y - (double) line1.y)));

    public struct StackArray6Vec2
    {
      private Vec2 _element0;
      private Vec2 _element1;
      private Vec2 _element2;
      private Vec2 _element3;
      private Vec2 _element4;
      private Vec2 _element5;
      public const int Length = 6;

      public Vec2 this[int index]
      {
        get
        {
          switch (index)
          {
            case 0:
              return this._element0;
            case 1:
              return this._element1;
            case 2:
              return this._element2;
            case 3:
              return this._element3;
            case 4:
              return this._element4;
            case 5:
              return this._element5;
            default:
              return Vec2.Zero;
          }
        }
        set
        {
          switch (index)
          {
            case 0:
              this._element0 = value;
              break;
            case 1:
              this._element1 = value;
              break;
            case 2:
              this._element2 = value;
              break;
            case 3:
              this._element3 = value;
              break;
            case 4:
              this._element4 = value;
              break;
            case 5:
              this._element5 = value;
              break;
          }
        }
      }
    }
  }
}
