// Decompiled with JetBrains decompiler
// Type: TaleWorlds.Library.Vec3
// Assembly: TaleWorlds.Library, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 271D0DB8-3C5D-4850-A565-542B4CF7D347
// Assembly location: E:\SteamLibrary\steamapps\common\Mount & Blade II Bannerlord\bin\Win64_Shipping_Client\TaleWorlds.Library.dll

using System;
using System.Numerics;
using System.Xml.Serialization;

namespace TaleWorlds.Library
{
  [Serializable]
  public struct Vec3
  {
    [XmlAttribute]
    public float x;
    [XmlAttribute]
    public float y;
    [XmlAttribute]
    public float z;
    [XmlAttribute]
    public float w;
    public static readonly Vec3 Side = new Vec3(1f);
    public static readonly Vec3 Forward = new Vec3(y: 1f);
    public static readonly Vec3 Up = new Vec3(z: 1f);
    public static readonly Vec3 One = new Vec3(1f, 1f, 1f);
    public static readonly Vec3 Zero = new Vec3();
    public static readonly Vec3 Invalid = new Vec3(float.NaN, float.NaN, float.NaN);

    public float X => this.x;

    public float Y => this.y;

    public float Z => this.z;

    public Vec3(float x = 0.0f, float y = 0.0f, float z = 0.0f, float w = -1f)
    {
      this.x = x;
      this.y = y;
      this.z = z;
      this.w = w;
    }

    public Vec3(Vec3 c, float w = -1f)
    {
      this.x = c.x;
      this.y = c.y;
      this.z = c.z;
      this.w = w;
    }

    public Vec3(Vec2 xy, float z = 0.0f, float w = -1f)
    {
      this.x = xy.x;
      this.y = xy.y;
      this.z = z;
      this.w = w;
    }

    public Vec3(Vector3 vector3)
      : this(vector3.X, vector3.Y, vector3.Z)
    {
    }

    public static Vec3 Abs(Vec3 vec) => new Vec3(MathF.Abs(vec.x), MathF.Abs(vec.y), MathF.Abs(vec.z));

    public static explicit operator Vector3(Vec3 vec3) => new Vector3(vec3.x, vec3.y, vec3.z);

    public float this[int i]
    {
      get
      {
        switch (i)
        {
          case 0:
            return this.x;
          case 1:
            return this.y;
          case 2:
            return this.z;
          case 3:
            return this.w;
          default:
            throw new IndexOutOfRangeException("Vec3 out of bounds.");
        }
      }
      set
      {
        switch (i)
        {
          case 0:
            this.x = value;
            break;
          case 1:
            this.y = value;
            break;
          case 2:
            this.z = value;
            break;
          case 3:
            this.w = value;
            break;
          default:
            throw new IndexOutOfRangeException("Vec3 out of bounds.");
        }
      }
    }

    public static float DotProduct(Vec3 v1, Vec3 v2) => (float) ((double) v1.x * (double) v2.x + (double) v1.y * (double) v2.y + (double) v1.z * (double) v2.z);

    public static Vec3 Lerp(Vec3 v1, Vec3 v2, float alpha) => v1 * (1f - alpha) + v2 * alpha;

    public static Vec3 Slerp(Vec3 start, Vec3 end, float percent)
    {
      float num1 = MBMath.ClampFloat(Vec3.DotProduct(start, end), -1f, 1f);
      float num2 = (float) Math.Acos((double) num1) * percent;
      Vec3 vec3 = end - start * num1;
      double num3 = (double) vec3.Normalize();
      return start * (float) Math.Cos((double) num2) + vec3 * (float) Math.Sin((double) num2);
    }

    public static Vec3 Vec3Max(Vec3 v1, Vec3 v2) => new Vec3(Math.Max(v1.x, v2.x), Math.Max(v1.y, v2.y), Math.Max(v1.z, v2.z));

    public static Vec3 Vec3Min(Vec3 v1, Vec3 v2) => new Vec3(Math.Min(v1.x, v2.x), Math.Min(v1.y, v2.y), Math.Min(v1.z, v2.z));

    public static Vec3 CrossProduct(Vec3 va, Vec3 vb) => new Vec3((float) ((double) va.y * (double) vb.z - (double) va.z * (double) vb.y), (float) ((double) va.z * (double) vb.x - (double) va.x * (double) vb.z), (float) ((double) va.x * (double) vb.y - (double) va.y * (double) vb.x));

    public static Vec3 operator -(Vec3 v) => new Vec3(-v.x, -v.y, -v.z);

    public static Vec3 operator +(Vec3 v1, Vec3 v2) => new Vec3(v1.x + v2.x, v1.y + v2.y, v1.z + v2.z);

    public static Vec3 operator -(Vec3 v1, Vec3 v2) => new Vec3(v1.x - v2.x, v1.y - v2.y, v1.z - v2.z);

    public static Vec3 operator *(Vec3 v, float f) => new Vec3(v.x * f, v.y * f, v.z * f);

    public static Vec3 operator *(float f, Vec3 v) => new Vec3(v.x * f, v.y * f, v.z * f);

    public static Vec3 operator *(Vec3 v, MatrixFrame frame) => new Vec3((float) ((double) frame.rotation.s.x * (double) v.x + (double) frame.rotation.f.x * (double) v.y + (double) frame.rotation.u.x * (double) v.z + (double) frame.origin.x * (double) v.w), (float) ((double) frame.rotation.s.y * (double) v.x + (double) frame.rotation.f.y * (double) v.y + (double) frame.rotation.u.y * (double) v.z + (double) frame.origin.y * (double) v.w), (float) ((double) frame.rotation.s.z * (double) v.x + (double) frame.rotation.f.z * (double) v.y + (double) frame.rotation.u.z * (double) v.z + (double) frame.origin.z * (double) v.w), (float) ((double) frame.rotation.s.w * (double) v.x + (double) frame.rotation.f.w * (double) v.y + (double) frame.rotation.u.w * (double) v.z + (double) frame.origin.w * (double) v.w));

    public static Vec3 operator /(Vec3 v, float f)
    {
      f = 1f / f;
      return new Vec3(v.x * f, v.y * f, v.z * f);
    }

    public static bool operator ==(Vec3 v1, Vec3 v2) => (double) v1.x == (double) v2.x && (double) v1.y == (double) v2.y && (double) v1.z == (double) v2.z;

    public static bool operator !=(Vec3 v1, Vec3 v2) => (double) v1.x != (double) v2.x || (double) v1.y != (double) v2.y || (double) v1.z != (double) v2.z;

    public float Length => (float) Math.Sqrt((double) this.x * (double) this.x + (double) this.y * (double) this.y + (double) this.z * (double) this.z);

    public float LengthSquared => (float) ((double) this.x * (double) this.x + (double) this.y * (double) this.y + (double) this.z * (double) this.z);

    public override bool Equals(object obj) => obj != null && !(this.GetType() != obj.GetType()) && ((double) ((Vec3) obj).x == (double) this.x && (double) ((Vec3) obj).y == (double) this.y) && (double) ((Vec3) obj).z == (double) this.z;

    public override int GetHashCode() => (int) (1001.0 * (double) this.x + 10039.0 * (double) this.y + 117.0 * (double) this.z);

    public bool IsValid => !float.IsNaN(this.x) && !float.IsNaN(this.y) && (!float.IsNaN(this.z) && !float.IsInfinity(this.x)) && !float.IsInfinity(this.y) && !float.IsInfinity(this.z);

    public bool IsValidXYZW => !float.IsNaN(this.x) && !float.IsNaN(this.y) && (!float.IsNaN(this.z) && !float.IsNaN(this.w)) && (!float.IsInfinity(this.x) && !float.IsInfinity(this.y) && !float.IsInfinity(this.z)) && !float.IsInfinity(this.w);

    public bool IsUnit
    {
      get
      {
        float lengthSquared = this.LengthSquared;
        return (double) lengthSquared > 0.980100035667419 && (double) lengthSquared < 1.02009999752045;
      }
    }

    public bool IsNonZero => (double) this.x != 0.0 || (double) this.y != 0.0 || (double) this.z != 0.0;

    public Vec3 NormalizedCopy()
    {
      Vec3 vec3 = this;
      double num = (double) vec3.Normalize();
      return vec3;
    }

    public float Normalize()
    {
      float length = this.Length;
      if ((double) length > 9.99999974737875E-06)
      {
        float num = 1f / length;
        this.x *= num;
        this.y *= num;
        this.z *= num;
      }
      else
      {
        this.x = 0.0f;
        this.y = 1f;
        this.z = 0.0f;
      }
      return length;
    }

    public Vec3 ClampedCopy(float min, float max)
    {
      Vec3 vec3 = this;
      vec3.x = MathF.Clamp(vec3.x, min, max);
      vec3.y = MathF.Clamp(vec3.y, min, max);
      vec3.z = MathF.Clamp(vec3.z, min, max);
      return vec3;
    }

    public Vec3 ClampedCopy(float min, float max, out bool valueClamped)
    {
      Vec3 vec3 = this;
      valueClamped = false;
      for (int i = 0; i < 3; ++i)
      {
        if ((double) vec3[i] < (double) min)
        {
          vec3[i] = min;
          valueClamped = true;
        }
        else if ((double) vec3[i] > (double) max)
        {
          vec3[i] = max;
          valueClamped = true;
        }
      }
      return vec3;
    }

    public void NormalizeWithoutChangingZ()
    {
      this.z = MBMath.ClampFloat(this.z, -0.99999f, 0.99999f);
      float length = this.AsVec2.Length;
      float num1 = MathF.Sqrt((float) (1.0 - (double) this.z * (double) this.z));
      if ((double) length >= (double) num1 - 1.0000000116861E-07 && (double) length <= (double) num1 + 1.0000000116861E-07)
        return;
      if ((double) length > 9.99999971718069E-10)
      {
        float num2 = num1 / length;
        this.x *= num2;
        this.y *= num2;
      }
      else
      {
        this.x = 0.0f;
        this.y = num1;
      }
    }

    public bool NearlyEquals(Vec3 v, float epsilon = 1E-05f) => (double) Math.Abs(this.x - v.x) < (double) epsilon && (double) Math.Abs(this.y - v.y) < (double) epsilon && (double) Math.Abs(this.z - v.z) < (double) epsilon;

    public void RotateAboutX(float a)
    {
      float num1 = (float) Math.Sin((double) a);
      float num2 = (float) Math.Cos((double) a);
      float num3 = (float) ((double) this.y * (double) num2 - (double) this.z * (double) num1);
      this.z = (float) ((double) this.z * (double) num2 + (double) this.y * (double) num1);
      this.y = num3;
    }

    public void RotateAboutY(float a)
    {
      float num1 = (float) Math.Sin((double) a);
      float num2 = (float) Math.Cos((double) a);
      float num3 = (float) ((double) this.x * (double) num2 + (double) this.z * (double) num1);
      this.z = (float) ((double) this.z * (double) num2 - (double) this.x * (double) num1);
      this.x = num3;
    }

    public void RotateAboutZ(float a)
    {
      float num1 = (float) Math.Sin((double) a);
      float num2 = (float) Math.Cos((double) a);
      float num3 = (float) ((double) this.x * (double) num2 - (double) this.y * (double) num1);
      this.y = (float) ((double) this.y * (double) num2 + (double) this.x * (double) num1);
      this.x = num3;
    }

    public Vec3 RotateAboutAnArbitraryVector(Vec3 vec, float a)
    {
      float x = vec.x;
      float y = vec.y;
      float z = vec.z;
      float num1 = x * this.x;
      float num2 = x * this.y;
      float num3 = x * this.z;
      float num4 = y * this.x;
      float num5 = y * this.y;
      float num6 = y * this.z;
      float num7 = z * this.x;
      float num8 = z * this.y;
      float num9 = z * this.z;
      float sa;
      float ca;
      MBMath.SinCos(a, out sa, out ca);
      return new Vec3()
      {
        x = (float) ((double) x * ((double) num1 + (double) num5 + (double) num9) + ((double) this.x * ((double) y * (double) y + (double) z * (double) z) - (double) x * ((double) num5 + (double) num9)) * (double) ca + (-(double) num8 + (double) num6) * (double) sa),
        y = (float) ((double) y * ((double) num1 + (double) num5 + (double) num9) + ((double) this.y * ((double) x * (double) x + (double) z * (double) z) - (double) y * ((double) num1 + (double) num9)) * (double) ca + ((double) num7 - (double) num3) * (double) sa),
        z = (float) ((double) z * ((double) num1 + (double) num5 + (double) num9) + ((double) this.z * ((double) x * (double) x + (double) y * (double) y) - (double) z * ((double) num1 + (double) num5)) * (double) ca + (-(double) num4 + (double) num2) * (double) sa)
      };
    }

    public Vec3 Reflect(Vec3 normal) => this - normal * (2f * Vec3.DotProduct(this, normal));

    public Vec3 ProjectOnUnitVector(Vec3 ov) => ov * (float) ((double) this.x * (double) ov.x + (double) this.y * (double) ov.y + (double) this.z * (double) ov.z);

    public float DistanceSquared(Vec3 v) => (float) (((double) v.x - (double) this.x) * ((double) v.x - (double) this.x) + ((double) v.y - (double) this.y) * ((double) v.y - (double) this.y) + ((double) v.z - (double) this.z) * ((double) v.z - (double) this.z));

    public float Distance(Vec3 v) => (float) Math.Sqrt(((double) v.x - (double) this.x) * ((double) v.x - (double) this.x) + ((double) v.y - (double) this.y) * ((double) v.y - (double) this.y) + ((double) v.z - (double) this.z) * ((double) v.z - (double) this.z));

    public static float AngleBetweenTwoVectors(Vec3 v1, Vec3 v2) => (float) Math.Acos((double) MathF.Clamp(Vec3.DotProduct(v1, v2) / (v1.Length * v2.Length), -1f, 1f));

    public Vec2 AsVec2
    {
      get => new Vec2(this.x, this.y);
      set
      {
        this.x = value.x;
        this.y = value.y;
      }
    }

    public override string ToString() => "(" + (object) this.x + ", " + (object) this.y + ", " + (object) this.z + ")";

    public uint ToARGB
    {
      get
      {
        int num = (int) (uint) ((double) this.w * 256.0);
        uint val1_1 = (uint) ((double) this.x * 256.0);
        uint val1_2 = (uint) ((double) this.y * 256.0);
        uint val1_3 = (uint) ((double) this.z * 256.0);
        return (uint) ((int) Math.Min((uint) num, (uint) byte.MaxValue) << 24 | (int) Math.Min(val1_1, (uint) byte.MaxValue) << 16 | (int) Math.Min(val1_2, (uint) byte.MaxValue) << 8) | Math.Min(val1_3, (uint) byte.MaxValue);
      }
    }

    public float RotationZ => (float) Math.Atan2(-(double) this.x, (double) this.y);

    public float RotationX => (float) Math.Atan2((double) this.z, Math.Sqrt((double) this.x * (double) this.x + (double) this.y * (double) this.y));

    public static Vec3 Parse(string input)
    {
      input = input.Replace(" ", "");
      string[] strArray = input.Split(',');
      double num1 = strArray.Length >= 3 && strArray.Length <= 4 ? (double) float.Parse(strArray[0]) : throw new ArgumentOutOfRangeException();
      float num2 = float.Parse(strArray[1]);
      float num3 = float.Parse(strArray[2]);
      float num4 = strArray.Length == 4 ? float.Parse(strArray[3]) : -1f;
      double num5 = (double) num2;
      double num6 = (double) num3;
      double num7 = (double) num4;
      return new Vec3((float) num1, (float) num5, (float) num6, (float) num7);
    }
  }
}
