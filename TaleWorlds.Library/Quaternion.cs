// Decompiled with JetBrains decompiler
// Type: TaleWorlds.Library.Quaternion
// Assembly: TaleWorlds.Library, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 271D0DB8-3C5D-4850-A565-542B4CF7D347
// Assembly location: E:\SteamLibrary\steamapps\common\Mount & Blade II Bannerlord\bin\Win64_Shipping_Client\TaleWorlds.Library.dll

using System;

namespace TaleWorlds.Library
{
  [Serializable]
  public struct Quaternion
  {
    public float W;
    public float X;
    public float Y;
    public float Z;

    public Quaternion(float x, float y, float z, float w)
    {
      this.X = x;
      this.Y = y;
      this.Z = z;
      this.W = w;
    }

    public override int GetHashCode() => base.GetHashCode();

    public override bool Equals(object obj) => base.Equals(obj);

    public static bool operator ==(Quaternion a, Quaternion b)
    {
      if ((ValueType) a == (ValueType) b)
        return true;
      return (ValueType) a != null && (ValueType) b != null && ((double) a.X == (double) b.X && (double) a.Y == (double) b.Y) && (double) a.Z == (double) b.Z && (double) a.W == (double) b.W;
    }

    public static bool operator !=(Quaternion a, Quaternion b) => !(a == b);

    public static Quaternion operator +(Quaternion a, Quaternion b) => new Quaternion(a.X + b.X, a.Y + b.Y, a.Z + b.Z, a.W + b.W);

    public static Quaternion operator -(Quaternion a, Quaternion b) => new Quaternion(a.X - b.X, a.Y - b.Y, a.Z - b.Z, a.W - b.W);

    public static Quaternion operator *(Quaternion a, float b) => new Quaternion(a.X * b, a.Y * b, a.Z * b, a.W * b);

    public static Quaternion operator *(float s, Quaternion v) => v * s;

    public float this[int i]
    {
      get
      {
        switch (i)
        {
          case 0:
            return this.W;
          case 1:
            return this.X;
          case 2:
            return this.Y;
          case 3:
            return this.Z;
          default:
            throw new IndexOutOfRangeException("Quaternion out of bounds.");
        }
      }
      set
      {
        switch (i)
        {
          case 0:
            this.W = value;
            break;
          case 1:
            this.X = value;
            break;
          case 2:
            this.Y = value;
            break;
          case 3:
            this.Z = value;
            break;
          default:
            throw new IndexOutOfRangeException("Quaternion out of bounds.");
        }
      }
    }

    public float Normalize()
    {
      float num1 = (float) Math.Sqrt((double) this.X * (double) this.X + (double) this.Y * (double) this.Y + (double) this.Z * (double) this.Z + (double) this.W * (double) this.W);
      if ((double) num1 <= 1.0000000116861E-07)
      {
        this.X = 0.0f;
        this.Y = 0.0f;
        this.Z = 0.0f;
        this.W = 1f;
      }
      else
      {
        float num2 = 1f / num1;
        this.X *= num2;
        this.Y *= num2;
        this.Z *= num2;
        this.W *= num2;
      }
      return num1;
    }

    public float SafeNormalize()
    {
      double num = Math.Sqrt((double) this.X * (double) this.X + (double) this.Y * (double) this.Y + (double) this.Z * (double) this.Z + (double) this.W * (double) this.W);
      if (num <= 1E-07)
      {
        this.X = 0.0f;
        this.Y = 0.0f;
        this.Z = 0.0f;
        this.W = 1f;
      }
      else
      {
        this.X /= (float) num;
        this.Y /= (float) num;
        this.Z /= (float) num;
        this.W /= (float) num;
      }
      return (float) num;
    }

    public float NormalizeWeighted()
    {
      float num = (float) ((double) this.X * (double) this.X + (double) this.Y * (double) this.Y + (double) this.Z * (double) this.Z);
      if ((double) num <= 9.99999971718069E-10)
      {
        this.X = 1f;
        this.Y = 0.0f;
        this.Z = 0.0f;
        this.W = 0.0f;
      }
      else
        this.W = (float) Math.Sqrt(1.0 - (double) num);
      return num;
    }

    public void SetToRotationX(float angle)
    {
      float num1 = (float) Math.Sin((double) angle * 0.5);
      float num2 = (float) Math.Cos((double) angle * 0.5);
      this.X = num1;
      this.Y = 0.0f;
      this.Z = 0.0f;
      this.W = num2;
    }

    public void SetToRotationY(float angle)
    {
      float num1 = (float) Math.Sin((double) angle * 0.5);
      float num2 = (float) Math.Cos((double) angle * 0.5);
      this.X = 0.0f;
      this.Y = num1;
      this.Z = 0.0f;
      this.W = num2;
    }

    public void SetToRotationZ(float angle)
    {
      float num1 = (float) Math.Sin((double) angle * 0.5);
      float num2 = (float) Math.Cos((double) angle * 0.5);
      this.X = 0.0f;
      this.Y = 0.0f;
      this.Z = num1;
      this.W = num2;
    }

    public void Flip()
    {
      this.X = -this.X;
      this.Y = -this.Y;
      this.Z = -this.Z;
      this.W = -this.W;
    }

    public bool IsIdentity => (double) this.X == 0.0 && (double) this.Y == 0.0 && (double) this.Z == 0.0 && (double) this.W == 1.0;

    public bool IsUnit => MBMath.ApproximatelyEquals((float) ((double) this.X * (double) this.X + (double) this.Y * (double) this.Y + (double) this.Z * (double) this.Z + (double) this.W * (double) this.W), 1f, 0.2f);

    public static Quaternion Identity => new Quaternion(0.0f, 0.0f, 0.0f, 1f);

    public Quaternion TransformToParent(Quaternion q) => new Quaternion()
    {
      X = (float) ((double) this.Y * (double) q.Z - (double) this.Z * (double) q.Y + (double) this.W * (double) q.X + (double) this.X * (double) q.W),
      Y = (float) ((double) this.Z * (double) q.X - (double) this.X * (double) q.Z + (double) this.W * (double) q.Y + (double) this.Y * (double) q.W),
      Z = (float) ((double) this.X * (double) q.Y - (double) this.Y * (double) q.X + (double) this.W * (double) q.Z + (double) this.Z * (double) q.W),
      W = (float) ((double) this.W * (double) q.W - ((double) this.X * (double) q.X + (double) this.Y * (double) q.Y + (double) this.Z * (double) q.Z))
    };

    public Quaternion TransformToLocal(Quaternion q) => new Quaternion()
    {
      X = (float) ((double) this.Z * (double) q.Y - (double) this.Y * (double) q.Z + (double) this.W * (double) q.X - (double) this.X * (double) q.W),
      Y = (float) ((double) this.X * (double) q.Z - (double) this.Z * (double) q.X + (double) this.W * (double) q.Y - (double) this.Y * (double) q.W),
      Z = (float) ((double) this.Y * (double) q.X - (double) this.X * (double) q.Y + (double) this.W * (double) q.Z - (double) this.Z * (double) q.W),
      W = (float) ((double) this.W * (double) q.W + ((double) this.X * (double) q.X + (double) this.Y * (double) q.Y + (double) this.Z * (double) q.Z))
    };

    public Quaternion TransformToLocalWithoutNormalize(Quaternion q) => new Quaternion()
    {
      X = (float) ((double) this.Z * (double) q.Y - (double) this.Y * (double) q.Z + (double) this.W * (double) q.X - (double) this.X * (double) q.W),
      Y = (float) ((double) this.X * (double) q.Z - (double) this.Z * (double) q.X + (double) this.W * (double) q.Y - (double) this.Y * (double) q.W),
      Z = (float) ((double) this.Y * (double) q.X - (double) this.X * (double) q.Y + (double) this.W * (double) q.Z - (double) this.Z * (double) q.W),
      W = (float) ((double) this.W * (double) q.W + ((double) this.X * (double) q.X + (double) this.Y * (double) q.Y + (double) this.Z * (double) q.Z))
    };

    public static Quaternion Slerp(Quaternion from, Quaternion to, float t)
    {
      float num1 = from.Dotp4(to);
      float num2;
      if ((double) num1 < 0.0)
      {
        num1 = -num1;
        num2 = -1f;
      }
      else
        num2 = 1f;
      float num3;
      float num4;
      if (0.9 >= (double) num1)
      {
        float num5 = (float) Math.Acos((double) num1);
        float num6 = 1f / (float) Math.Sin((double) num5);
        float num7 = t * num5;
        num3 = (float) Math.Sin((double) num5 - (double) num7) * num6;
        num4 = (float) Math.Sin((double) num7) * num6;
      }
      else
      {
        num3 = 1f - t;
        num4 = t;
      }
      float num8 = num4 * num2;
      return new Quaternion()
      {
        X = (float) ((double) num3 * (double) from.X + (double) num8 * (double) to.X),
        Y = (float) ((double) num3 * (double) from.Y + (double) num8 * (double) to.Y),
        Z = (float) ((double) num3 * (double) from.Z + (double) num8 * (double) to.Z),
        W = (float) ((double) num3 * (double) from.W + (double) num8 * (double) to.W)
      };
    }

    public static Quaternion Lerp(Quaternion from, Quaternion to, float t)
    {
      float num1 = from.Dotp4(to);
      float num2 = 1f - t;
      float num3;
      if ((double) num1 < 0.0)
      {
        float num4 = -num1;
        num3 = -t;
      }
      else
        num3 = t;
      return new Quaternion()
      {
        X = (float) ((double) num2 * (double) from.X + (double) num3 * (double) to.X),
        Y = (float) ((double) num2 * (double) from.Y + (double) num3 * (double) to.Y),
        Z = (float) ((double) num2 * (double) from.Z + (double) num3 * (double) to.Z),
        W = (float) ((double) num2 * (double) from.W + (double) num3 * (double) to.W)
      };
    }

    public static Mat3 Mat3FromQuaternion(Quaternion quat)
    {
      Mat3 mat3 = new Mat3();
      float num1 = quat.X + quat.X;
      float num2 = quat.Y + quat.Y;
      float num3 = quat.Z + quat.Z;
      float num4 = quat.X * num1;
      float num5 = quat.X * num2;
      float num6 = quat.X * num3;
      float num7 = quat.Y * num2;
      float num8 = quat.Y * num3;
      float num9 = quat.Z * num3;
      float num10 = quat.W * num1;
      float num11 = quat.W * num2;
      float num12 = quat.W * num3;
      mat3.s.x = (float) (1.0 - ((double) num7 + (double) num9));
      mat3.s.y = num5 + num12;
      mat3.s.z = num6 - num11;
      mat3.f.x = num5 - num12;
      mat3.f.y = (float) (1.0 - ((double) num4 + (double) num9));
      mat3.f.z = num8 + num10;
      mat3.u.x = num6 + num11;
      mat3.u.y = num8 - num10;
      mat3.u.z = (float) (1.0 - ((double) num4 + (double) num7));
      return mat3;
    }

    public static Quaternion QuaternionFromMat3(Mat3 m)
    {
      Quaternion quaternion = new Quaternion();
      float x;
      if ((double) m[2][2] < 0.0)
      {
        if ((double) m[0][0] > (double) m[1][1])
        {
          Vec3 vec3 = m[0];
          double num1 = 1.0 + (double) vec3[0];
          vec3 = m[1];
          double num2 = (double) vec3[1];
          double num3 = num1 - num2;
          vec3 = m[2];
          double num4 = (double) vec3[2];
          x = (float) (num3 - num4);
          ref Quaternion local1 = ref quaternion;
          vec3 = m[1];
          double num5 = (double) vec3[2];
          vec3 = m[2];
          double num6 = (double) vec3[1];
          double num7 = num5 - num6;
          local1.W = (float) num7;
          quaternion.X = x;
          ref Quaternion local2 = ref quaternion;
          vec3 = m[0];
          double num8 = (double) vec3[1];
          vec3 = m[1];
          double num9 = (double) vec3[0];
          double num10 = num8 + num9;
          local2.Y = (float) num10;
          ref Quaternion local3 = ref quaternion;
          vec3 = m[2];
          double num11 = (double) vec3[0];
          vec3 = m[0];
          double num12 = (double) vec3[2];
          double num13 = num11 + num12;
          local3.Z = (float) num13;
        }
        else
        {
          Vec3 vec3 = m[0];
          double num1 = 1.0 - (double) vec3[0];
          vec3 = m[1];
          double num2 = (double) vec3[1];
          double num3 = num1 + num2;
          vec3 = m[2];
          double num4 = (double) vec3[2];
          x = (float) (num3 - num4);
          ref Quaternion local1 = ref quaternion;
          vec3 = m[2];
          double num5 = (double) vec3[0];
          vec3 = m[0];
          double num6 = (double) vec3[2];
          double num7 = num5 - num6;
          local1.W = (float) num7;
          ref Quaternion local2 = ref quaternion;
          vec3 = m[0];
          double num8 = (double) vec3[1];
          vec3 = m[1];
          double num9 = (double) vec3[0];
          double num10 = num8 + num9;
          local2.X = (float) num10;
          quaternion.Y = x;
          ref Quaternion local3 = ref quaternion;
          vec3 = m[1];
          double num11 = (double) vec3[2];
          vec3 = m[2];
          double num12 = (double) vec3[1];
          double num13 = num11 + num12;
          local3.Z = (float) num13;
        }
      }
      else if ((double) m[0][0] < -(double) m[1][1])
      {
        Vec3 vec3 = m[0];
        double num1 = 1.0 - (double) vec3[0];
        vec3 = m[1];
        double num2 = (double) vec3[1];
        double num3 = num1 - num2;
        vec3 = m[2];
        double num4 = (double) vec3[2];
        x = (float) (num3 + num4);
        ref Quaternion local1 = ref quaternion;
        vec3 = m[0];
        double num5 = (double) vec3[1];
        vec3 = m[1];
        double num6 = (double) vec3[0];
        double num7 = num5 - num6;
        local1.W = (float) num7;
        ref Quaternion local2 = ref quaternion;
        vec3 = m[2];
        double num8 = (double) vec3[0];
        vec3 = m[0];
        double num9 = (double) vec3[2];
        double num10 = num8 + num9;
        local2.X = (float) num10;
        ref Quaternion local3 = ref quaternion;
        vec3 = m[1];
        double num11 = (double) vec3[2];
        vec3 = m[2];
        double num12 = (double) vec3[1];
        double num13 = num11 + num12;
        local3.Y = (float) num13;
        quaternion.Z = x;
      }
      else
      {
        Vec3 vec3 = m[0];
        double num1 = 1.0 + (double) vec3[0];
        vec3 = m[1];
        double num2 = (double) vec3[1];
        double num3 = num1 + num2;
        vec3 = m[2];
        double num4 = (double) vec3[2];
        x = (float) (num3 + num4);
        quaternion.W = x;
        ref Quaternion local1 = ref quaternion;
        vec3 = m[1];
        double num5 = (double) vec3[2];
        vec3 = m[2];
        double num6 = (double) vec3[1];
        double num7 = num5 - num6;
        local1.X = (float) num7;
        ref Quaternion local2 = ref quaternion;
        vec3 = m[2];
        double num8 = (double) vec3[0];
        vec3 = m[0];
        double num9 = (double) vec3[2];
        double num10 = num8 - num9;
        local2.Y = (float) num10;
        ref Quaternion local3 = ref quaternion;
        vec3 = m[0];
        double num11 = (double) vec3[1];
        vec3 = m[1];
        double num12 = (double) vec3[0];
        double num13 = num11 - num12;
        local3.Z = (float) num13;
      }
      float num = 0.5f / MathF.Sqrt(x);
      quaternion.W *= num;
      quaternion.X *= num;
      quaternion.Y *= num;
      quaternion.Z *= num;
      return quaternion;
    }

    public static void AxisAngleFromQuaternion(out Vec3 axis, out float angle, Quaternion quat)
    {
      axis = new Vec3();
      float w = quat.W;
      if ((double) w > 0.99999988079071)
      {
        axis.x = 1f;
        axis.y = 0.0f;
        axis.z = 0.0f;
        angle = 0.0f;
      }
      else
      {
        float num = (float) Math.Sqrt(1.0 - (double) w * (double) w);
        if ((double) num < 9.99999974737875E-05)
          num = 1f;
        axis.x = quat.X / num;
        axis.y = quat.Y / num;
        axis.z = quat.Z / num;
        angle = (float) Math.Acos((double) w) * 2f;
      }
    }

    public static Quaternion QuaternionFromAxisAngle(Vec3 axis, float angle)
    {
      Quaternion quaternion = new Quaternion();
      float num1 = (float) Math.Sin((double) angle * 0.5);
      float num2 = (float) Math.Cos((double) angle * 0.5);
      quaternion.X = axis.x * num1;
      quaternion.Y = axis.y * num1;
      quaternion.Z = axis.z * num1;
      quaternion.W = num2;
      return quaternion;
    }

    public static Vec3 EulerAngleFromQuaternion(Quaternion quat)
    {
      float w = quat.W;
      float x = quat.X;
      float y = quat.Y;
      float z = quat.Z;
      float num1 = w * w;
      float num2 = x * x;
      float num3 = y * y;
      float num4 = z * z;
      return new Vec3()
      {
        z = (float) Math.Atan2(2.0 * ((double) x * (double) y + (double) z * (double) w), (double) num2 - (double) num3 - (double) num4 + (double) num1),
        x = (float) Math.Atan2(2.0 * ((double) y * (double) z + (double) x * (double) w), -(double) num2 - (double) num3 + (double) num4 + (double) num1),
        y = (float) Math.Asin(-2.0 * ((double) x * (double) z - (double) y * (double) w))
      };
    }

    public static Quaternion FindShortestArcAsQuaternion(Vec3 v0, Vec3 v1)
    {
      Vec3 vec3_1 = Vec3.CrossProduct(v0, v1);
      float num1 = Vec3.DotProduct(v0, v1);
      if ((double) num1 < -0.999990000000253)
      {
        Vec3 vec3_2 = new Vec3();
        Vec3 vec3_3 = (double) Math.Abs(v0.z) >= 0.800000011920929 ? Vec3.CrossProduct(v0, new Vec3(1f)) : Vec3.CrossProduct(v0, new Vec3(z: 1f));
        double num2 = (double) vec3_3.Normalize();
        return new Quaternion(vec3_3.x, vec3_3.y, vec3_3.z, 0.0f);
      }
      float num3 = (float) Math.Sqrt((1.0 + (double) num1) * 2.0);
      float num4 = 1f / num3;
      return new Quaternion(vec3_1.x * num4, vec3_1.y * num4, vec3_1.z * num4, num3 * 0.5f);
    }

    public float Dotp4(Quaternion q2) => (float) ((double) this.X * (double) q2.X + (double) this.Y * (double) q2.Y + (double) this.Z * (double) q2.Z + (double) this.W * (double) q2.W);

    public Mat3 ToMat3 => Quaternion.Mat3FromQuaternion(this);

    public bool InverseDirection(Quaternion q2) => (double) this.Dotp4(q2) < 0.0;
  }
}
