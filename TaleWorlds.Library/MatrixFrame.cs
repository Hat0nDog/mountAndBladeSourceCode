// Decompiled with JetBrains decompiler
// Type: TaleWorlds.Library.MatrixFrame
// Assembly: TaleWorlds.Library, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 271D0DB8-3C5D-4850-A565-542B4CF7D347
// Assembly location: E:\SteamLibrary\steamapps\common\Mount & Blade II Bannerlord\bin\Win64_Shipping_Client\TaleWorlds.Library.dll

using System;

namespace TaleWorlds.Library
{
  [Serializable]
  public struct MatrixFrame
  {
    public Mat3 rotation;
    public Vec3 origin;

    public MatrixFrame(Mat3 rot, Vec3 o)
    {
      this.rotation = rot;
      this.origin = o;
    }

    public MatrixFrame(
      float _11,
      float _12,
      float _13,
      float _21,
      float _22,
      float _23,
      float _31,
      float _32,
      float _33,
      float _41,
      float _42,
      float _43)
    {
      this.rotation = new Mat3(_11, _12, _13, _21, _22, _23, _31, _32, _33);
      this.origin = new Vec3(_41, _42, _43);
    }

    public MatrixFrame(
      float _11,
      float _12,
      float _13,
      float _14,
      float _21,
      float _22,
      float _23,
      float _24,
      float _31,
      float _32,
      float _33,
      float _34,
      float _41,
      float _42,
      float _43,
      float _44)
    {
      this.rotation = new Mat3();
      this.rotation.s = new Vec3(_11, _12, _13, _14);
      this.rotation.f = new Vec3(_21, _22, _23, _24);
      this.rotation.u = new Vec3(_31, _32, _33, _34);
      this.origin = new Vec3(_41, _42, _43, _44);
    }

    public Vec3 TransformToParent(Vec3 v) => new Vec3((float) ((double) this.rotation.s.x * (double) v.x + (double) this.rotation.f.x * (double) v.y + (double) this.rotation.u.x * (double) v.z) + this.origin.x, (float) ((double) this.rotation.s.y * (double) v.x + (double) this.rotation.f.y * (double) v.y + (double) this.rotation.u.y * (double) v.z) + this.origin.y, (float) ((double) this.rotation.s.z * (double) v.x + (double) this.rotation.f.z * (double) v.y + (double) this.rotation.u.z * (double) v.z) + this.origin.z);

    public Vec3 TransformToLocal(Vec3 v)
    {
      Vec3 vec3 = v - this.origin;
      return new Vec3((float) ((double) this.rotation.s.x * (double) vec3.x + (double) this.rotation.s.y * (double) vec3.y + (double) this.rotation.s.z * (double) vec3.z), (float) ((double) this.rotation.f.x * (double) vec3.x + (double) this.rotation.f.y * (double) vec3.y + (double) this.rotation.f.z * (double) vec3.z), (float) ((double) this.rotation.u.x * (double) vec3.x + (double) this.rotation.u.y * (double) vec3.y + (double) this.rotation.u.z * (double) vec3.z));
    }

    public bool NearlyEquals(MatrixFrame rhs, float epsilon = 1E-05f) => this.rotation.NearlyEquals(rhs.rotation, epsilon) && this.origin.NearlyEquals(rhs.origin, epsilon);

    public Vec3 TransformToLocalNonOrthogonal(Vec3 v) => new MatrixFrame(this.rotation.s.x, this.rotation.s.y, this.rotation.s.z, 0.0f, this.rotation.f.x, this.rotation.f.y, this.rotation.f.z, 0.0f, this.rotation.u.x, this.rotation.u.y, this.rotation.u.z, 0.0f, this.origin.x, this.origin.y, this.origin.z, 1f).Inverse().TransformToParent(v);

    public MatrixFrame TransformToLocalNonOrthogonal(ref MatrixFrame frame) => new MatrixFrame(this.rotation.s.x, this.rotation.s.y, this.rotation.s.z, 0.0f, this.rotation.f.x, this.rotation.f.y, this.rotation.f.z, 0.0f, this.rotation.u.x, this.rotation.u.y, this.rotation.u.z, 0.0f, this.origin.x, this.origin.y, this.origin.z, 1f).Inverse().TransformToParent(frame);

    public static MatrixFrame Lerp(MatrixFrame m1, MatrixFrame m2, float alpha)
    {
      MatrixFrame matrixFrame;
      matrixFrame.rotation = Mat3.Lerp(m1.rotation, m2.rotation, alpha);
      matrixFrame.origin = Vec3.Lerp(m1.origin, m2.origin, alpha);
      return matrixFrame;
    }

    public static MatrixFrame Slerp(MatrixFrame m1, MatrixFrame m2, float alpha)
    {
      MatrixFrame matrixFrame;
      matrixFrame.origin = Vec3.Lerp(m1.origin, m2.origin, alpha);
      Quaternion quaternion = Quaternion.Slerp(Quaternion.QuaternionFromMat3(m1.rotation), Quaternion.QuaternionFromMat3(m2.rotation), alpha);
      matrixFrame.rotation = quaternion.ToMat3;
      return matrixFrame;
    }

    public MatrixFrame TransformToParent(MatrixFrame m) => new MatrixFrame(this.rotation.TransformToParent(m.rotation), this.TransformToParent(m.origin));

    public MatrixFrame TransformToLocal(MatrixFrame m) => new MatrixFrame(this.rotation.TransformToLocal(m.rotation), this.TransformToLocal(m.origin));

    public Vec3 TransformToParentWithW(Vec3 _s) => new Vec3((float) ((double) this.rotation.s.x * (double) _s.x + (double) this.rotation.f.x * (double) _s.y + (double) this.rotation.u.x * (double) _s.z + (double) this.origin.x * (double) _s.w), (float) ((double) this.rotation.s.y * (double) _s.x + (double) this.rotation.f.y * (double) _s.y + (double) this.rotation.u.y * (double) _s.z + (double) this.origin.y * (double) _s.w), (float) ((double) this.rotation.s.z * (double) _s.x + (double) this.rotation.f.z * (double) _s.y + (double) this.rotation.u.z * (double) _s.z + (double) this.origin.z * (double) _s.w), (float) ((double) this.rotation.s.w * (double) _s.x + (double) this.rotation.f.w * (double) _s.y + (double) this.rotation.u.w * (double) _s.z + (double) this.origin.w * (double) _s.w));

    public MatrixFrame GetUnitRotFrame(float removedScale) => new MatrixFrame(this.rotation.GetUnitRotation(removedScale), this.origin);

    public static MatrixFrame Identity => new MatrixFrame(Mat3.Identity, new Vec3(w: 1f));

    public static MatrixFrame Zero => new MatrixFrame(new Mat3(Vec3.Zero, Vec3.Zero, Vec3.Zero), new Vec3(w: 1f));

    public MatrixFrame Inverse()
    {
      this.AssertFilled();
      MatrixFrame matrix = new MatrixFrame();
      float num1 = (float) ((double) this[2, 2] * (double) this[3, 3] - (double) this[2, 3] * (double) this[3, 2]);
      float num2 = (float) ((double) this[1, 2] * (double) this[3, 3] - (double) this[1, 3] * (double) this[3, 2]);
      float num3 = (float) ((double) this[1, 2] * (double) this[2, 3] - (double) this[1, 3] * (double) this[2, 2]);
      float num4 = (float) ((double) this[0, 2] * (double) this[3, 3] - (double) this[0, 3] * (double) this[3, 2]);
      float num5 = (float) ((double) this[0, 2] * (double) this[2, 3] - (double) this[0, 3] * (double) this[2, 2]);
      float num6 = (float) ((double) this[0, 2] * (double) this[1, 3] - (double) this[0, 3] * (double) this[1, 2]);
      float num7 = (float) ((double) this[2, 1] * (double) this[3, 3] - (double) this[2, 3] * (double) this[3, 1]);
      float num8 = (float) ((double) this[1, 1] * (double) this[3, 3] - (double) this[1, 3] * (double) this[3, 1]);
      float num9 = (float) ((double) this[1, 1] * (double) this[2, 3] - (double) this[1, 3] * (double) this[2, 1]);
      float num10 = (float) ((double) this[0, 1] * (double) this[3, 3] - (double) this[0, 3] * (double) this[3, 1]);
      float num11 = (float) ((double) this[0, 1] * (double) this[2, 3] - (double) this[0, 3] * (double) this[2, 1]);
      float num12 = (float) ((double) this[1, 1] * (double) this[3, 3] - (double) this[1, 3] * (double) this[3, 1]);
      float num13 = (float) ((double) this[0, 1] * (double) this[1, 3] - (double) this[0, 3] * (double) this[1, 1]);
      float num14 = (float) ((double) this[2, 1] * (double) this[3, 2] - (double) this[2, 2] * (double) this[3, 1]);
      float num15 = (float) ((double) this[1, 1] * (double) this[3, 2] - (double) this[1, 2] * (double) this[3, 1]);
      float num16 = (float) ((double) this[1, 1] * (double) this[2, 2] - (double) this[1, 2] * (double) this[2, 1]);
      float num17 = (float) ((double) this[0, 1] * (double) this[3, 2] - (double) this[0, 2] * (double) this[3, 1]);
      float num18 = (float) ((double) this[0, 1] * (double) this[2, 2] - (double) this[0, 2] * (double) this[2, 1]);
      float num19 = (float) ((double) this[0, 1] * (double) this[1, 2] - (double) this[0, 2] * (double) this[1, 1]);
      matrix[0, 0] = (float) ((double) this[1, 1] * (double) num1 - (double) this[2, 1] * (double) num2 + (double) this[3, 1] * (double) num3);
      matrix[0, 1] = (float) (-(double) this[0, 1] * (double) num1 + (double) this[2, 1] * (double) num4 - (double) this[3, 1] * (double) num5);
      matrix[0, 2] = (float) ((double) this[0, 1] * (double) num2 - (double) this[1, 1] * (double) num4 + (double) this[3, 1] * (double) num6);
      matrix[0, 3] = (float) (-(double) this[0, 1] * (double) num3 + (double) this[1, 1] * (double) num5 - (double) this[2, 1] * (double) num6);
      matrix[1, 0] = (float) (-(double) this[1, 0] * (double) num1 + (double) this[2, 0] * (double) num2 - (double) this[3, 0] * (double) num3);
      matrix[1, 1] = (float) ((double) this[0, 0] * (double) num1 - (double) this[2, 0] * (double) num4 + (double) this[3, 0] * (double) num5);
      matrix[1, 2] = (float) (-(double) this[0, 0] * (double) num2 + (double) this[1, 0] * (double) num4 - (double) this[3, 0] * (double) num6);
      matrix[1, 3] = (float) ((double) this[0, 0] * (double) num3 - (double) this[1, 0] * (double) num5 + (double) this[2, 0] * (double) num6);
      matrix[2, 0] = (float) ((double) this[1, 0] * (double) num7 - (double) this[2, 0] * (double) num8 + (double) this[3, 0] * (double) num9);
      matrix[2, 1] = (float) (-(double) this[0, 0] * (double) num7 + (double) this[2, 0] * (double) num10 - (double) this[3, 0] * (double) num11);
      matrix[2, 2] = (float) ((double) this[0, 0] * (double) num12 - (double) this[1, 0] * (double) num10 + (double) this[3, 0] * (double) num13);
      matrix[2, 3] = (float) (-(double) this[0, 0] * (double) num9 + (double) this[1, 0] * (double) num11 - (double) this[2, 0] * (double) num13);
      matrix[3, 0] = (float) (-(double) this[1, 0] * (double) num14 + (double) this[2, 0] * (double) num15 - (double) this[3, 0] * (double) num16);
      matrix[3, 1] = (float) ((double) this[0, 0] * (double) num14 - (double) this[2, 0] * (double) num17 + (double) this[3, 0] * (double) num18);
      matrix[3, 2] = (float) (-(double) this[0, 0] * (double) num15 + (double) this[1, 0] * (double) num17 - (double) this[3, 0] * (double) num19);
      matrix[3, 3] = (float) ((double) this[0, 0] * (double) num16 - (double) this[1, 0] * (double) num18 + (double) this[2, 0] * (double) num19);
      float w = (float) ((double) this[0, 0] * (double) matrix[0, 0] + (double) this[1, 0] * (double) matrix[0, 1] + (double) this[2, 0] * (double) matrix[0, 2] + (double) this[3, 0] * (double) matrix[0, 3]);
      if ((double) w != 1.0)
        MatrixFrame.DivideWith(ref matrix, w);
      return matrix;
    }

    private static void DivideWith(ref MatrixFrame matrix, float w)
    {
      float num = 1f / w;
      for (int i = 0; i < 4; ++i)
      {
        for (int j = 0; j < 4; ++j)
          matrix[i, j] *= num;
      }
    }

    public void Rotate(float radian, Vec3 axis)
    {
      float sa;
      float ca;
      MBMath.SinCos(radian, out sa, out ca);
      MatrixFrame matrixFrame = new MatrixFrame();
      matrixFrame[0, 0] = (float) ((double) axis.x * (double) axis.x * (1.0 - (double) ca)) + ca;
      matrixFrame[1, 0] = (float) ((double) axis.x * (double) axis.y * (1.0 - (double) ca) - (double) axis.z * (double) sa);
      matrixFrame[2, 0] = (float) ((double) axis.x * (double) axis.z * (1.0 - (double) ca) + (double) axis.y * (double) sa);
      matrixFrame[3, 0] = 0.0f;
      matrixFrame[0, 1] = (float) ((double) axis.y * (double) axis.x * (1.0 - (double) ca) + (double) axis.z * (double) sa);
      matrixFrame[1, 1] = (float) ((double) axis.y * (double) axis.y * (1.0 - (double) ca)) + ca;
      matrixFrame[2, 1] = (float) ((double) axis.y * (double) axis.z * (1.0 - (double) ca) - (double) axis.x * (double) sa);
      matrixFrame[3, 1] = 0.0f;
      matrixFrame[0, 2] = (float) ((double) axis.x * (double) axis.z * (1.0 - (double) ca) - (double) axis.y * (double) sa);
      matrixFrame[1, 2] = (float) ((double) axis.y * (double) axis.z * (1.0 - (double) ca) + (double) axis.x * (double) sa);
      matrixFrame[2, 2] = (float) ((double) axis.z * (double) axis.z * (1.0 - (double) ca)) + ca;
      matrixFrame[3, 2] = 0.0f;
      matrixFrame[0, 3] = 0.0f;
      matrixFrame[1, 3] = 0.0f;
      matrixFrame[2, 3] = 0.0f;
      matrixFrame[3, 3] = 1f;
      this.origin = this.TransformToParent(matrixFrame.origin);
      this.rotation = this.rotation.TransformToParent(matrixFrame.rotation);
    }

    public static MatrixFrame operator *(MatrixFrame m1, MatrixFrame m2) => m1.TransformToParent(m2);

    public static bool operator ==(MatrixFrame m1, MatrixFrame m2) => m1.origin == m2.origin && m1.rotation == m2.rotation;

    public static bool operator !=(MatrixFrame m1, MatrixFrame m2) => m1.origin != m2.origin || m1.rotation != m2.rotation;

    public override string ToString() => "MatrixFrame:\n" + "Rotation:\n" + this.rotation.ToString() + "Origin: " + (object) this.origin.x + ", " + (object) this.origin.y + ", " + (object) this.origin.z + "\n";

    public override bool Equals(object obj) => this == (MatrixFrame) obj;

    public override int GetHashCode() => base.GetHashCode();

    public MatrixFrame Strafe(float a)
    {
      this.origin += this.rotation.s * a;
      return this;
    }

    public MatrixFrame Advance(float a)
    {
      this.origin += this.rotation.f * a;
      return this;
    }

    public MatrixFrame Elevate(float a)
    {
      this.origin += this.rotation.u * a;
      return this;
    }

    public void Scale(Vec3 scalingVector)
    {
      MatrixFrame identity = MatrixFrame.Identity;
      identity.rotation.s.x = scalingVector.x;
      identity.rotation.f.y = scalingVector.y;
      identity.rotation.u.z = scalingVector.z;
      this.origin = this.TransformToParent(identity.origin);
      this.rotation = this.rotation.TransformToParent(identity.rotation);
    }

    public bool IsIdentity => !this.origin.IsNonZero && this.rotation.IsIdentity();

    public bool IsZero => !this.origin.IsNonZero && this.rotation.IsZero();

    public float this[int i, int j]
    {
      get
      {
        switch (i)
        {
          case 0:
            return this.rotation.s[j];
          case 1:
            return this.rotation.f[j];
          case 2:
            return this.rotation.u[j];
          case 3:
            return this.origin[j];
          default:
            throw new IndexOutOfRangeException("MatrixFrame out of bounds.");
        }
      }
      set
      {
        switch (i)
        {
          case 0:
            this.rotation.s[j] = value;
            break;
          case 1:
            this.rotation.f[j] = value;
            break;
          case 2:
            this.rotation.u[j] = value;
            break;
          case 3:
            this.origin[j] = value;
            break;
          default:
            throw new IndexOutOfRangeException("MatrixFrame out of bounds.");
        }
      }
    }

    public static MatrixFrame CreateLookAt(Vec3 position, Vec3 target, Vec3 upVector)
    {
      Vec3 vec3_1 = target - position;
      double num1 = (double) vec3_1.Normalize();
      Vec3 vec3_2 = Vec3.CrossProduct(upVector, vec3_1);
      double num2 = (double) vec3_2.Normalize();
      Vec3 v1 = Vec3.CrossProduct(vec3_1, vec3_2);
      return new MatrixFrame(vec3_2.x, v1.x, vec3_1.x, 0.0f, vec3_2.y, v1.y, vec3_1.y, 0.0f, vec3_2.z, v1.z, vec3_1.z, 0.0f, -Vec3.DotProduct(vec3_2, position), -Vec3.DotProduct(v1, position), -Vec3.DotProduct(vec3_1, position), 1f);
    }

    public void Fill()
    {
      this.rotation.s.w = 0.0f;
      this.rotation.f.w = 0.0f;
      this.rotation.u.w = 0.0f;
      this.origin.w = 1f;
    }

    private void AssertFilled()
    {
    }
  }
}
