// Decompiled with JetBrains decompiler
// Type: TaleWorlds.Library.Color
// Assembly: TaleWorlds.Library, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 271D0DB8-3C5D-4850-A565-542B4CF7D347
// Assembly location: E:\SteamLibrary\steamapps\common\Mount & Blade II Bannerlord\bin\Win64_Shipping_Client\TaleWorlds.Library.dll

using System;
using System.Globalization;
using System.Numerics;

namespace TaleWorlds.Library
{
  public struct Color
  {
    public float Red;
    public float Green;
    public float Blue;
    public float Alpha;

    public Color(float red, float green, float blue, float alpha = 1f)
    {
      this.Red = red;
      this.Green = green;
      this.Blue = blue;
      this.Alpha = alpha;
    }

    public Vector3 ToVector3() => new Vector3(this.Red, this.Green, this.Blue);

    public Vec3 ToVec3() => new Vec3(this.Red, this.Green, this.Blue, this.Alpha);

    public static Color operator *(Color c, float f)
    {
      double num1 = (double) c.Red * (double) f;
      float num2 = c.Green * f;
      float num3 = c.Blue * f;
      float num4 = c.Alpha * f;
      double num5 = (double) num2;
      double num6 = (double) num3;
      double num7 = (double) num4;
      return new Color((float) num1, (float) num5, (float) num6, (float) num7);
    }

    public static Color operator *(Color c1, Color c2) => new Color(c1.Red * c2.Red, c1.Green * c2.Green, c1.Blue * c2.Blue, c1.Alpha * c2.Alpha);

    public static Color operator +(Color c1, Color c2) => new Color(c1.Red + c2.Red, c1.Green + c2.Green, c1.Blue + c2.Blue, c1.Alpha + c2.Alpha);

    public static Color operator -(Color c1, Color c2) => new Color(c1.Red - c2.Red, c1.Green - c2.Green, c1.Blue - c2.Blue, c1.Alpha - c2.Alpha);

    public static Color Black => new Color(0.0f, 0.0f, 0.0f);

    public static Color White => new Color(1f, 1f, 1f);

    public static bool operator ==(Color a, Color b) => (double) a.Red == (double) b.Red && (double) a.Green == (double) b.Green && (double) a.Blue == (double) b.Blue && (double) a.Alpha == (double) b.Alpha;

    public static bool operator !=(Color a, Color b) => !(a == b);

    public override int GetHashCode() => base.GetHashCode();

    public override bool Equals(object obj) => obj != null && obj is Color color && this == color;

    public static Color FromVector3(Vector3 vector3) => new Color(vector3.X, vector3.Y, vector3.Z);

    public static Color FromVector3(Vec3 vector3) => new Color(vector3.x, vector3.y, vector3.z);

    public float Length() => (float) Math.Sqrt((double) this.Red * (double) this.Red + (double) this.Green * (double) this.Green + (double) this.Blue * (double) this.Blue + (double) this.Alpha * (double) this.Alpha);

    public uint ToUnsignedInteger() => (uint) (((int) (byte) ((double) this.Alpha * (double) byte.MaxValue) << 24) + ((int) (byte) ((double) this.Red * (double) byte.MaxValue) << 16) + ((int) (byte) ((double) this.Green * (double) byte.MaxValue) << 8)) + (uint) (byte) ((double) this.Blue * (double) byte.MaxValue);

    public static Color FromUint(uint color)
    {
      int num1 = (int) (byte) (color >> 24);
      byte num2 = (byte) (color >> 16);
      byte num3 = (byte) (color >> 8);
      byte num4 = (byte) color;
      float num5 = (float) num1 * 0.003921569f;
      double num6 = (double) num2 * 0.00392156885936856;
      float num7 = (float) num3 * 0.003921569f;
      float num8 = (float) num4 * 0.003921569f;
      double num9 = (double) num7;
      double num10 = (double) num8;
      double num11 = (double) num5;
      return new Color((float) num6, (float) num9, (float) num10, (float) num11);
    }

    public static Color ConvertStringToColor(string color)
    {
      string s1 = color.Substring(1, 2);
      string s2 = color.Substring(3, 2);
      string s3 = color.Substring(5, 2);
      string s4 = color.Substring(7, 2);
      return new Color((float) int.Parse(s1, NumberStyles.HexNumber) * 0.003921569f, (float) int.Parse(s2, NumberStyles.HexNumber) * 0.003921569f, (float) int.Parse(s3, NumberStyles.HexNumber) * 0.003921569f, (float) int.Parse(s4, NumberStyles.HexNumber) * 0.003921569f);
    }

    public static Color Lerp(Color start, Color end, float ratio)
    {
      double num1 = (double) start.Red * (1.0 - (double) ratio) + (double) end.Red * (double) ratio;
      float num2 = (float) ((double) start.Green * (1.0 - (double) ratio) + (double) end.Green * (double) ratio);
      float num3 = (float) ((double) start.Blue * (1.0 - (double) ratio) + (double) end.Blue * (double) ratio);
      float num4 = (float) ((double) start.Alpha * (1.0 - (double) ratio) + (double) end.Alpha * (double) ratio);
      double num5 = (double) num2;
      double num6 = (double) num3;
      double num7 = (double) num4;
      return new Color((float) num1, (float) num5, (float) num6, (float) num7);
    }

    public override string ToString()
    {
      byte num1 = (byte) ((double) this.Red * (double) byte.MaxValue);
      byte num2 = (byte) ((double) this.Green * (double) byte.MaxValue);
      byte num3 = (byte) ((double) this.Blue * (double) byte.MaxValue);
      byte num4 = (byte) ((double) this.Alpha * (double) byte.MaxValue);
      return "#" + num1.ToString("X2") + num2.ToString("X2") + num3.ToString("X2") + num4.ToString("X2");
    }

    public static string UIntToColorString(uint color)
    {
      string str1 = (color >> 24).ToString("X");
      if (str1.Length == 1)
        str1 = str1.Insert(0, "0");
      string str2 = (color >> 16).ToString("X");
      if (str2.Length == 1)
        str2 = str2.Insert(0, "0");
      string str3 = str2.Substring(Math.Max(0, str2.Length - 2));
      string str4 = (color >> 8).ToString("X");
      if (str4.Length == 1)
        str4 = str4.Insert(0, "0");
      string str5 = str4.Substring(Math.Max(0, str4.Length - 2));
      string str6 = color.ToString("X");
      if (str6.Length == 1)
        str6 = str6.Insert(0, "0");
      string str7 = str6.Substring(Math.Max(0, str6.Length - 2));
      return str3 + str5 + str7 + str1;
    }
  }
}
