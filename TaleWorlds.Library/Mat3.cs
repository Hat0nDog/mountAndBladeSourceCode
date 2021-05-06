// Decompiled with JetBrains decompiler
// Type: TaleWorlds.Library.Mat3
// Assembly: TaleWorlds.Library, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 271D0DB8-3C5D-4850-A565-542B4CF7D347
// Assembly location: E:\SteamLibrary\steamapps\common\Mount & Blade II Bannerlord\bin\Win64_Shipping_Client\TaleWorlds.Library.dll

using System;

namespace TaleWorlds.Library
{
  [Serializable]
  public struct Mat3
  {
    public Vec3 s;
    public Vec3 f;
    public Vec3 u;

    public Mat3(Vec3 s, Vec3 f, Vec3 u)
    {
      this.s = s;
      this.f = f;
      this.u = u;
    }

    public Mat3(
      float sx,
      float sy,
      float sz,
      float fx,
      float fy,
      float fz,
      float ux,
      float uy,
      float uz)
    {
      this.s = new Vec3(sx, sy, sz);
      this.f = new Vec3(fx, fy, fz);
      this.u = new Vec3(ux, uy, uz);
    }

    public Vec3 this[int i]
    {
      get
      {
        switch (i)
        {
          case 0:
            return this.s;
          case 1:
            return this.f;
          case 2:
            return this.u;
          default:
            throw new IndexOutOfRangeException("Vec3 out of bounds.");
        }
      }
      set
      {
        switch (i)
        {
          case 0:
            this.s = value;
            break;
          case 1:
            this.f = value;
            break;
          case 2:
            this.u = value;
            break;
          default:
            throw new IndexOutOfRangeException("Vec3 out of bounds.");
        }
      }
    }

    public void RotateAboutSide(float a)
    {
      float sa;
      float ca;
      MBMath.SinCos(a, out sa, out ca);
      Vec3 vec3 = this.f * ca + this.u * sa;
      this.u = this.u * ca - this.f * sa;
      this.f = vec3;
    }

    public void RotateAboutForward(float a)
    {
      float sa;
      float ca;
      MBMath.SinCos(a, out sa, out ca);
      Vec3 vec3_1 = this.s * ca - this.u * sa;
      Vec3 vec3_2 = this.u * ca + this.s * sa;
      this.s = vec3_1;
      this.u = vec3_2;
    }

    public void RotateAboutUp(float a)
    {
      float sa;
      float ca;
      MBMath.SinCos(a, out sa, out ca);
      Vec3 vec3_1 = this.s * ca + this.f * sa;
      Vec3 vec3_2 = this.f * ca - this.s * sa;
      this.s = vec3_1;
      this.f = vec3_2;
    }

    public void RotateAboutAnArbitraryVector(Vec3 v, float a)
    {
      this.s = this.s.RotateAboutAnArbitraryVector(v, a);
      this.f = this.f.RotateAboutAnArbitraryVector(v, a);
      this.u = this.u.RotateAboutAnArbitraryVector(v, a);
    }

    public bool IsOrthonormal()
    {
      bool flag = this.s.IsUnit && this.f.IsUnit && this.u.IsUnit;
      float num = Vec3.DotProduct(this.s, this.f);
      if ((double) num > 0.00999999977648258 || (double) num < -0.00999999977648258)
        flag = false;
      else if (!this.u.NearlyEquals(Vec3.CrossProduct(this.s, this.f), 0.01f))
        flag = false;
      return flag;
    }

    public bool NearlyEquals(Mat3 rhs, float epsilon = 1E-05f) => this.s.NearlyEquals(rhs.s, epsilon) && this.f.NearlyEquals(rhs.f, epsilon) && this.u.NearlyEquals(rhs.u, epsilon);

    public Vec3 TransformToParent(Vec3 v) => new Vec3((float) ((double) this.s.x * (double) v.x + (double) this.f.x * (double) v.y + (double) this.u.x * (double) v.z), (float) ((double) this.s.y * (double) v.x + (double) this.f.y * (double) v.y + (double) this.u.y * (double) v.z), (float) ((double) this.s.z * (double) v.x + (double) this.f.z * (double) v.y + (double) this.u.z * (double) v.z));

    public Vec3 TransformToParent(ref Vec3 v) => new Vec3((float) ((double) this.s.x * (double) v.x + (double) this.f.x * (double) v.y + (double) this.u.x * (double) v.z), (float) ((double) this.s.y * (double) v.x + (double) this.f.y * (double) v.y + (double) this.u.y * (double) v.z), (float) ((double) this.s.z * (double) v.x + (double) this.f.z * (double) v.y + (double) this.u.z * (double) v.z));

    public Vec3 TransformToLocal(Vec3 v) => new Vec3((float) ((double) this.s.x * (double) v.x + (double) this.s.y * (double) v.y + (double) this.s.z * (double) v.z), (float) ((double) this.f.x * (double) v.x + (double) this.f.y * (double) v.y + (double) this.f.z * (double) v.z), (float) ((double) this.u.x * (double) v.x + (double) this.u.y * (double) v.y + (double) this.u.z * (double) v.z));

    public Mat3 TransformToParent(Mat3 m) => new Mat3(this.TransformToParent(m.s), this.TransformToParent(m.f), this.TransformToParent(m.u));

    public Mat3 TransformToLocal(Mat3 m)
    {
      Mat3 mat3;
      mat3.s = this.TransformToLocal(m.s);
      mat3.f = this.TransformToLocal(m.f);
      mat3.u = this.TransformToLocal(m.u);
      return mat3;
    }

    public void Orthonormalize()
    {
      double num1 = (double) this.f.Normalize();
      this.s = Vec3.CrossProduct(this.f, this.u);
      double num2 = (double) this.s.Normalize();
      this.u = Vec3.CrossProduct(this.s, this.f);
    }

    public void OrthonormalizeAccordingToForwardAndKeepUpAsZAxis()
    {
      this.f.z = 0.0f;
      double num = (double) this.f.Normalize();
      this.u = Vec3.Up;
      this.s = Vec3.CrossProduct(this.f, this.u);
    }

    public Mat3 GetUnitRotation(float removedScale)
    {
      float num = 1f / removedScale;
      return new Mat3(this.s * num, this.f * num, this.u * num);
    }

    public Vec3 MakeUnit() => new Vec3()
    {
      x = this.s.Normalize(),
      y = this.f.Normalize(),
      z = this.u.Normalize()
    };

    public bool IsUnit() => this.s.IsUnit && this.f.IsUnit && this.u.IsUnit;

    public void ApplyScaleLocal(float scaleAmount)
    {
      this.s *= scaleAmount;
      this.f *= scaleAmount;
      this.u *= scaleAmount;
    }

    public void ApplyScaleLocal(Vec3 scaleAmountXYZ)
    {
      this.s *= scaleAmountXYZ.x;
      this.f *= scaleAmountXYZ.y;
      this.u *= scaleAmountXYZ.z;
    }

    public bool HasScale() => !this.s.IsUnit || !this.f.IsUnit || !this.u.IsUnit;

    public Vec3 GetScaleVector() => new Vec3(this.s.Length, this.f.Length, this.u.Length);

    public Vec3 GetScaleVectorSquared() => new Vec3(this.s.LengthSquared, this.f.LengthSquared, this.u.LengthSquared);

    public void ToQuaternion(out Quaternion quat) => quat = Quaternion.QuaternionFromMat3(this);

    public Quaternion ToQuaternion() => Quaternion.QuaternionFromMat3(this);

    public static Mat3 Lerp(Mat3 m1, Mat3 m2, float alpha)
    {
      Mat3 identity = Mat3.Identity;
      identity.f = Vec3.Lerp(m1.f, m2.f, alpha);
      identity.u = Vec3.Lerp(m1.u, m2.u, alpha);
      identity.Orthonormalize();
      return identity;
    }

    public static Mat3 CreateMat3WithForward(in Vec3 direction)
    {
      Mat3 identity = Mat3.Identity;
      identity.f = direction;
      double num1 = (double) identity.f.Normalize();
      identity.u = (double) Math.Abs(identity.f.z) >= 0.990000009536743 ? new Vec3(y: 1f) : new Vec3(z: 1f);
      identity.s = Vec3.CrossProduct(identity.f, identity.u);
      double num2 = (double) identity.s.Normalize();
      identity.u = Vec3.CrossProduct(identity.s, identity.f);
      double num3 = (double) identity.u.Normalize();
      return identity;
    }

    public Vec3 GetEulerAngles()
    {
      Mat3 mat3 = this;
      mat3.Orthonormalize();
      return new Vec3((float) Math.Asin((double) mat3.f.z), (float) Math.Atan2(-(double) mat3.s.z, (double) mat3.u.z), (float) Math.Atan2(-(double) mat3.f.x, (double) mat3.f.y));
    }

    public Mat3 Transpose() => new Mat3(this.s.x, this.f.x, this.u.x, this.s.y, this.f.y, this.u.y, this.s.z, this.f.z, this.u.z);

    public static Mat3 Identity => new Mat3(new Vec3(1f), new Vec3(y: 1f), new Vec3(z: 1f));

    public static Mat3 operator *(Mat3 v, float a) => new Mat3(v.s * a, v.f * a, v.u * a);

    public static bool operator ==(Mat3 m1, Mat3 m2) => m1.f == m2.f && m1.u == m2.u;

    public static bool operator !=(Mat3 m1, Mat3 m2) => m1.f != m2.f || m1.u != m2.u;

    public override string ToString() => "Mat3: " + "s: " + (object) this.s.x + ", " + (object) this.s.y + ", " + (object) this.s.z + ";" + "f: " + (object) this.f.x + ", " + (object) this.f.y + ", " + (object) this.f.z + ";" + "u: " + (object) this.u.x + ", " + (object) this.u.y + ", " + (object) this.u.z + ";" + "\n";

    public override bool Equals(object obj) => this == (Mat3) obj;

    public override int GetHashCode() => base.GetHashCode();

    public bool IsIdentity() => (double) this.s.x == 1.0 && (double) this.s.y == 0.0 && ((double) this.s.z == 0.0 && (double) this.f.x == 0.0) && ((double) this.f.y == 1.0 && (double) this.f.z == 0.0 && ((double) this.u.x == 0.0 && (double) this.u.y == 0.0)) && (double) this.u.z == 1.0;

    public bool IsZero() => (double) this.s.x == 0.0 && (double) this.s.y == 0.0 && ((double) this.s.z == 0.0 && (double) this.f.x == 0.0) && ((double) this.f.y == 0.0 && (double) this.f.z == 0.0 && ((double) this.u.x == 0.0 && (double) this.u.y == 0.0)) && (double) this.u.z == 0.0;

    public bool IsUniformScaled()
    {
      Vec3 scaleVectorSquared = this.GetScaleVectorSquared();
      return MBMath.ApproximatelyEquals(scaleVectorSquared.x, scaleVectorSquared.y, 0.01f) && MBMath.ApproximatelyEquals(scaleVectorSquared.x, scaleVectorSquared.z, 0.01f);
    }

    public void ApplyEulerAngles(Vec3 eulerAngles)
    {
      this.RotateAboutUp(eulerAngles.z);
      this.RotateAboutSide(eulerAngles.x);
      this.RotateAboutForward(eulerAngles.y);
    }
  }
}
