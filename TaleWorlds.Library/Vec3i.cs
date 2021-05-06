// Decompiled with JetBrains decompiler
// Type: TaleWorlds.Library.Vec3i
// Assembly: TaleWorlds.Library, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 271D0DB8-3C5D-4850-A565-542B4CF7D347
// Assembly location: E:\SteamLibrary\steamapps\common\Mount & Blade II Bannerlord\bin\Win64_Shipping_Client\TaleWorlds.Library.dll

using System;

namespace TaleWorlds.Library
{
  [Serializable]
  public struct Vec3i
  {
    public int X;
    public int Y;
    public int Z;
    public static readonly Vec3i Zero = new Vec3i();

    public Vec3i(int x = 0, int y = 0, int z = 0)
    {
      this.X = x;
      this.Y = y;
      this.Z = z;
    }

    public static bool operator ==(Vec3i v1, Vec3i v2) => v1.X == v2.X && v1.Y == v2.Y && v1.Z == v2.Z;

    public static bool operator !=(Vec3i v1, Vec3i v2) => v1.X != v2.X || v1.Y != v2.Y || v1.Z != v2.Z;

    public Vec3 ToVec3() => new Vec3((float) this.X, (float) this.Y, (float) this.Z);

    public int this[int index]
    {
      get
      {
        if (index == 0)
          return this.X;
        return index != 1 ? this.Z : this.Y;
      }
      set
      {
        if (index == 0)
          this.X = value;
        else if (index == 1)
          this.Y = value;
        else
          this.Z = value;
      }
    }

    public static Vec3i operator *(Vec3i v, int mult) => new Vec3i(v.X * mult, v.Y * mult, v.Z * mult);

    public static Vec3i operator +(Vec3i v1, Vec3i v2) => new Vec3i(v1.X + v2.X, v1.Y + v2.Y, v1.Z + v2.Z);

    public static Vec3i operator -(Vec3i v1, Vec3i v2) => new Vec3i(v1.X - v2.X, v1.Y - v2.Y, v1.Z - v2.Z);

    public override bool Equals(object obj) => obj != null && !(this.GetType() != obj.GetType()) && (((Vec3i) obj).X == this.X && ((Vec3i) obj).Y == this.Y) && ((Vec3i) obj).Z == this.Z;

    public override int GetHashCode() => (this.X * 397 ^ this.Y) * 397 ^ this.Z;

    public override string ToString() => string.Format("{0}: {1}, {2}: {3}, {4}: {5}", (object) "X", (object) this.X, (object) "Y", (object) this.Y, (object) "Z", (object) this.Z);
  }
}
