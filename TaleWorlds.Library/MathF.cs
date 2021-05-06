// Decompiled with JetBrains decompiler
// Type: TaleWorlds.Library.MathF
// Assembly: TaleWorlds.Library, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 271D0DB8-3C5D-4850-A565-542B4CF7D347
// Assembly location: E:\SteamLibrary\steamapps\common\Mount & Blade II Bannerlord\bin\Win64_Shipping_Client\TaleWorlds.Library.dll

using System;

namespace TaleWorlds.Library
{
  public static class MathF
  {
    public const float DegToRad = 0.01745329f;
    public const float RadToDeg = 57.29578f;
    public const float TwoPI = 6.283185f;
    public const float PI = 3.141593f;
    public const float HalfPI = 1.570796f;
    public const float Epsilon = 1E-05f;

    public static float Sqrt(float x) => (float) Math.Sqrt((double) x);

    public static float Sin(float x) => (float) Math.Sin((double) x);

    public static float Cos(float x) => (float) Math.Cos((double) x);

    public static float Tan(float x) => (float) Math.Tan((double) x);

    public static float Pow(float x, float y) => (float) Math.Pow((double) x, (double) y);

    public static bool IsValidValue(float f) => !float.IsNaN(f) && !float.IsInfinity(f);

    public static float Clamp(float value, float minValue, float maxValue) => Math.Max(Math.Min(value, maxValue), minValue);

    public static float AngleClamp(float angle)
    {
      while ((double) angle < 0.0)
        angle += 6.283185f;
      while ((double) angle > 6.28318548202515)
        angle -= 6.283185f;
      return angle;
    }

    public static float Lerp(
      float valueFrom,
      float valueTo,
      float amount,
      float minimumDifference = 1E-05f)
    {
      return (double) Math.Abs(valueFrom - valueTo) <= (double) minimumDifference ? valueTo : valueFrom + (valueTo - valueFrom) * amount;
    }

    public static float AngleLerp(
      float angleFrom,
      float angleTo,
      float amount,
      float minimumDifference = 1E-05f)
    {
      float num1 = (float) (((double) angleTo - (double) angleFrom) % 6.28318548202515);
      float num2 = (float) (2.0 * (double) num1 % 6.28318548202515) - num1;
      return MathF.AngleClamp(angleFrom + num2 * amount);
    }

    public static int Round(float f) => (int) Math.Round((double) f);

    public static int Floor(float f) => (int) Math.Floor((double) f);

    public static int Ceiling(float f) => (int) Math.Ceiling((double) f);

    public static float Abs(float f) => (double) f < 0.0 ? -f : f;

    public static float Max(float a, float b, float c) => Math.Max(a, Math.Max(b, c));

    public static float Min(float a, float b, float c) => Math.Min(a, Math.Min(b, c));

    public static object Clamp(float ratio, object minPriceFactor, object maxPriceFactor) => throw new NotImplementedException();

    public static float PingPong(float min, float max, float time)
    {
      int num1 = (int) ((double) min * 100.0);
      int num2 = (int) ((double) max * 100.0);
      int num3 = (int) ((double) time * 100.0);
      int num4 = num2 - num1;
      int num5 = num3 / num4 % 2 == 0 ? 1 : 0;
      int num6 = num3 % num4;
      return (num5 != 0 ? (float) (num6 + num1) : (float) (num2 - num6)) / 100f;
    }

    public static int GreatestCommonDivisor(int a, int b)
    {
      int num;
      for (; b != 0; b = num)
      {
        num = a % b;
        a = b;
      }
      return a;
    }
  }
}
