// Decompiled with JetBrains decompiler
// Type: TaleWorlds.Library.Vec2i
// Assembly: TaleWorlds.Library, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 271D0DB8-3C5D-4850-A565-542B4CF7D347
// Assembly location: E:\SteamLibrary\steamapps\common\Mount & Blade II Bannerlord\bin\Win64_Shipping_Client\TaleWorlds.Library.dll

using System;

namespace TaleWorlds.Library
{
  [Serializable]
  public struct Vec2i : IEquatable<Vec2i>
  {
    public int X;
    public int Y;

    public int Item1 => this.X;

    public int Item2 => this.Y;

    public Vec2i(int x = 0, int y = 0)
    {
      this.X = x;
      this.Y = y;
    }

    public static bool operator ==(Vec2i a, Vec2i b) => a.X == b.X && a.Y == b.Y;

    public static bool operator !=(Vec2i a, Vec2i b) => a.X != b.X || a.Y != b.Y;

    public override bool Equals(object obj) => obj != null && !(this.GetType() != obj.GetType()) && ((Vec2i) obj).X == this.X && ((Vec2i) obj).Y == this.Y;

    public bool Equals(Vec2i value) => value.X == this.X && value.Y == this.Y;

    public override int GetHashCode() => (23 * 31 + this.X.GetHashCode()) * 31 + this.Y.GetHashCode();
  }
}
