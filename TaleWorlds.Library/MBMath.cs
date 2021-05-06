// Decompiled with JetBrains decompiler
// Type: TaleWorlds.Library.MBMath
// Assembly: TaleWorlds.Library, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 271D0DB8-3C5D-4850-A565-542B4CF7D347
// Assembly location: E:\SteamLibrary\steamapps\common\Mount & Blade II Bannerlord\bin\Win64_Shipping_Client\TaleWorlds.Library.dll

using System;
using System.Collections.Generic;

namespace TaleWorlds.Library
{
  public static class MBMath
  {
    public const float TwoPI = 6.283185f;
    public const float PI = 3.141593f;
    public const float HalfPI = 1.570796f;
    public const float DegreesToRadians = 0.01745329f;
    public const float RadiansToDegrees = 57.29578f;
    public const float Epsilon = 1E-05f;

    public static bool IsValidValue(float f) => !float.IsNaN(f) && !float.IsInfinity(f);

    public static int ClampIndex(int value, int minValue, int maxValue) => MBMath.ClampInt(value, minValue, maxValue - 1);

    public static int ClampInt(int value, int minValue, int maxValue) => Math.Max(Math.Min(value, maxValue), minValue);

    public static float ClampFloat(float value, float minValue, float maxValue) => Math.Max(Math.Min(value, maxValue), minValue);

    public static void ClampUnit(ref float value) => value = MBMath.ClampFloat(value, 0.0f, 1f);

    public static int GetNumberOfBitsToRepresentNumber(uint value)
    {
      int num = 0;
      for (uint index = value; index > 0U; index >>= 1)
        ++num;
      return num;
    }

    public static IEnumerable<(T, int)> DistributeShares<T>(
      int totalAward,
      IEnumerable<T> stakeHolders,
      Func<T, int> shareFunction)
    {
      List<(T, int)> sharesList = new List<(T, int)>(20);
      int num1 = 0;
      foreach (T stakeHolder in stakeHolders)
      {
        int num2 = shareFunction(stakeHolder);
        sharesList.Add((stakeHolder, num2));
        num1 += num2;
      }
      if (num1 > 0)
      {
        int remainingShares = num1;
        int remaingAward = totalAward;
        for (int i = 0; i < sharesList.Count && remaingAward > 0; ++i)
        {
          int num2 = sharesList[i].Item2;
          int num3 = MathF.Round((float) remaingAward * (float) num2 / (float) remainingShares);
          if (num3 > remaingAward)
            num3 = remaingAward;
          remaingAward -= num3;
          remainingShares -= num2;
          yield return (sharesList[i].Item1, num3);
        }
      }
    }

    public static int GetNumberOfBitsToRepresentNumber(ulong value)
    {
      int num = 0;
      for (ulong index = value; index > 0UL; index >>= 1)
        ++num;
      return num;
    }

    public static float Lerp(
      float valueFrom,
      float valueTo,
      float amount,
      float minimumDifference = 1E-05f)
    {
      return (double) Math.Abs(valueFrom - valueTo) <= (double) minimumDifference ? valueTo : valueFrom + (valueTo - valueFrom) * amount;
    }

    public static float LinearExtrapolation(float valueFrom, float valueTo, float amount) => valueFrom + (valueTo - valueFrom) * amount;

    public static Vec3 Lerp(Vec3 vecFrom, Vec3 vecTo, float amount, float minimumDifference) => new Vec3(MBMath.Lerp(vecFrom.x, vecTo.x, amount, minimumDifference), MBMath.Lerp(vecFrom.y, vecTo.y, amount, minimumDifference), MBMath.Lerp(vecFrom.z, vecTo.z, amount, minimumDifference));

    public static Vec2 Lerp(Vec2 vecFrom, Vec2 vecTo, float amount, float minimumDifference) => new Vec2(MBMath.Lerp(vecFrom.x, vecTo.x, amount, minimumDifference), MBMath.Lerp(vecFrom.y, vecTo.y, amount, minimumDifference));

    public static float Map(
      float input,
      float inputMinimum,
      float inputMaximum,
      float outputMinimum,
      float outputMaximum)
    {
      input = MBMath.ClampFloat(input, inputMinimum, inputMaximum);
      return (float) (((double) input - (double) inputMinimum) * ((double) outputMaximum - (double) outputMinimum) / ((double) inputMaximum - (double) inputMinimum)) + outputMinimum;
    }

    public static Mat3 Lerp(
      ref Mat3 matFrom,
      ref Mat3 matTo,
      float amount,
      float minimumDifference)
    {
      return new Mat3(MBMath.Lerp(matFrom.s, matTo.s, amount, minimumDifference), MBMath.Lerp(matFrom.f, matTo.f, amount, minimumDifference), MBMath.Lerp(matFrom.u, matTo.u, amount, minimumDifference));
    }

    public static float LerpRadians(
      float valueFrom,
      float valueTo,
      float amount,
      float minChange,
      float maxChange)
    {
      float betweenTwoAngles = MBMath.GetSmallestDifferenceBetweenTwoAngles(valueFrom, valueTo);
      if ((double) Math.Abs(betweenTwoAngles) <= (double) minChange)
        return valueTo;
      float num = (float) Math.Sign(betweenTwoAngles) * MBMath.ClampFloat(Math.Abs(betweenTwoAngles * amount), minChange, maxChange);
      return MBMath.WrapAngle(valueFrom + num);
    }

    public static float SplitLerp(
      float value1,
      float value2,
      float value3,
      float cutOff,
      float amount,
      float minimumDifference)
    {
      if ((double) amount <= (double) cutOff)
      {
        float num = cutOff;
        float amount1 = amount / num;
        return MBMath.Lerp(value1, value2, amount1, minimumDifference);
      }
      float num1 = 1f - cutOff;
      float amount2 = (amount - cutOff) / num1;
      return MBMath.Lerp(value2, value3, amount2, minimumDifference);
    }

    public static float InverseLerp(float valueFrom, float valueTo, float value) => (float) (((double) value - (double) valueFrom) / ((double) valueTo - (double) valueFrom));

    public static float SmoothStep(float edge0, float edge1, float value)
    {
      float num = MBMath.ClampFloat((float) (((double) value - (double) edge0) / ((double) edge1 - (double) edge0)), 0.0f, 1f);
      return (float) ((double) num * (double) num * (3.0 - 2.0 * (double) num));
    }

    public static float BilinearLerp(
      float topLeft,
      float topRight,
      float botLeft,
      float botRight,
      float x,
      float y)
    {
      return MBMath.Lerp(MBMath.Lerp(topLeft, topRight, x), MBMath.Lerp(botLeft, botRight, x), y);
    }

    public static float GetSmallestDifferenceBetweenTwoAngles(float fromAngle, float toAngle)
    {
      float num = toAngle - fromAngle;
      if ((double) num > 3.14159274101257)
        num -= 6.283185f;
      if ((double) num < -3.14159274101257)
        num = 6.283185f + num;
      return num;
    }

    public static float ClampAngle(float angle, float restrictionCenter, float restrictionRange)
    {
      restrictionRange /= 2f;
      float betweenTwoAngles = MBMath.GetSmallestDifferenceBetweenTwoAngles(restrictionCenter, angle);
      if ((double) betweenTwoAngles > (double) restrictionRange)
        angle = restrictionCenter + restrictionRange;
      else if ((double) betweenTwoAngles < -(double) restrictionRange)
        angle = restrictionCenter - restrictionRange;
      if ((double) angle > 3.14159274101257)
        angle -= 6.283185f;
      else if ((double) angle < -3.14159274101257)
        angle += 6.283185f;
      return angle;
    }

    public static float WrapAngle(float angle)
    {
      angle = (float) Math.IEEERemainder((double) angle, 2.0 * Math.PI);
      if ((double) angle <= -3.14159274101257)
        angle += 6.283185f;
      else if ((double) angle > 3.14159274101257)
        angle -= 6.283185f;
      return angle;
    }

    public static bool IsBetween(float numberToCheck, float bottom, float top) => (double) numberToCheck > (double) bottom && (double) numberToCheck < (double) top;

    public static bool IsBetween(int value, int minValue, int maxValue) => value >= minValue && value < maxValue;

    public static bool IsBetweenInclusive(float numberToCheck, float bottom, float top) => (double) numberToCheck >= (double) bottom && (double) numberToCheck <= (double) top;

    public static uint ColorFromRGBA(float red, float green, float blue, float alpha)
    {
      byte maxValue = byte.MaxValue;
      return (uint) (((int) (uint) ((double) alpha * (double) maxValue) << 24) + ((int) (uint) ((double) red * (double) maxValue) << 16) + ((int) (uint) ((double) green * (double) maxValue) << 8)) + (uint) ((double) blue * (double) maxValue);
    }

    public static Color HSBtoRGB(
      float hue,
      float saturation,
      float brightness,
      float outputAlpha)
    {
      float num1 = 0.0f;
      float num2 = 0.0f;
      float num3 = 0.0f;
      float num4 = brightness * saturation;
      float num5 = num4 * (1f - Math.Abs((float) ((double) hue / 60.0 % 2.0 - 1.0)));
      float num6 = brightness - num4;
      switch ((int) ((double) hue / 60.0 % 6.0))
      {
        case 0:
          num1 = num4;
          num2 = num5;
          num3 = 0.0f;
          break;
        case 1:
          num1 = num5;
          num2 = num4;
          num3 = 0.0f;
          break;
        case 2:
          num1 = 0.0f;
          num2 = num4;
          num3 = num5;
          break;
        case 3:
          num1 = 0.0f;
          num2 = num5;
          num3 = num4;
          break;
        case 4:
          num1 = num5;
          num2 = 0.0f;
          num3 = num4;
          break;
        case 5:
          num1 = num4;
          num2 = 0.0f;
          num3 = num5;
          break;
      }
      return new Color(num1 + num6, num2 + num6, num3 + num6, outputAlpha);
    }

    public static Vec3 RGBtoHSB(Color rgb)
    {
      Vec3 vec3 = new Vec3();
      float num1 = Math.Min(Math.Min(rgb.Red, rgb.Green), rgb.Blue);
      float num2 = Math.Max(Math.Max(rgb.Red, rgb.Green), rgb.Blue);
      float num3 = num2 - num1;
      vec3.z = num2;
      vec3.x = (double) Math.Abs(num3) >= 9.99999974737875E-05 ? ((double) Math.Abs(num2 - rgb.Red) >= 9.99999974737875E-05 ? ((double) Math.Abs(num2 - rgb.Green) >= 9.99999974737875E-05 ? (float) (60.0 * (((double) rgb.Red - (double) rgb.Green) / (double) num3 + 4.0)) : (float) (60.0 * (((double) rgb.Blue - (double) rgb.Red) / (double) num3 + 2.0))) : (float) (60.0 * (((double) rgb.Green - (double) rgb.Blue) / (double) num3 % 6.0))) : 0.0f;
      vec3.x %= 360f;
      if ((double) vec3.x < 0.0)
        vec3.x += 360f;
      vec3.y = (double) Math.Abs(num2) >= 9.99999974737875E-05 ? num3 / num2 : 0.0f;
      return vec3;
    }

    public static Vec3 GammaCorrectRGB(float gamma, Vec3 rgb)
    {
      float num = 1f / gamma;
      rgb.x = (float) Math.Pow((double) rgb.x, (double) num);
      rgb.y = (float) Math.Pow((double) rgb.y, (double) num);
      rgb.z = (float) Math.Pow((double) rgb.z, (double) num);
      return rgb;
    }

    public static float ToRadians(this float f) => f * ((float) Math.PI / 180f);

    public static float ToDegrees(this float f) => f * 57.29578f;

    public static bool ApproximatelyEqualsTo(this float f, float comparedValue, float epsilon = 1E-05f) => (double) Math.Abs(f - comparedValue) <= (double) epsilon;

    public static bool ApproximatelyEquals(float first, float second, float epsilon = 1E-05f) => (double) Math.Abs(first - second) <= (double) epsilon;

    public static int Round(float f) => (int) Math.Round((double) f);

    public static int Round(this double f) => (int) Math.Round(f);

    public static int Floor(float f) => (int) Math.Floor((double) f);

    public static int Floor(double f) => (int) Math.Floor(f);

    public static int Ceiling(float f) => (int) Math.Ceiling((double) f);

    public static int Ceiling(double f) => (int) Math.Ceiling(f);

    public static float Absf(float val) => (double) val <= 0.0 ? -val : val;

    public static Vec3 GetClosestPointInLineSegmentToPoint(
      Vec3 point,
      Vec3 lineSegmentBegin,
      Vec3 lineSegmentEnd)
    {
      Vec3 vec3 = lineSegmentEnd - lineSegmentBegin;
      if (!vec3.IsNonZero)
        return lineSegmentBegin;
      float num = Vec3.DotProduct(point - lineSegmentBegin, vec3) / Vec3.DotProduct(vec3, vec3);
      if ((double) num < 0.0)
        return lineSegmentBegin;
      return (double) num > 1.0 ? lineSegmentEnd : lineSegmentBegin + vec3 * num;
    }

    public static bool GetRayPlaneIntersectionPoint(
      ref Vec3 planeNormal,
      ref Vec3 planeCenter,
      ref Vec3 rayOrigin,
      ref Vec3 rayDirection,
      out float t)
    {
      float num = Vec3.DotProduct(planeNormal, rayDirection);
      if ((double) num > 9.99999997475243E-07)
      {
        Vec3 v1 = planeCenter - rayOrigin;
        t = Vec3.DotProduct(v1, planeNormal) / num;
        return (double) t >= 0.0;
      }
      t = -1f;
      return false;
    }

    public static Vec2 GetClosestPointInLineSegmentToPoint(
      Vec2 point,
      Vec2 lineSegmentBegin,
      Vec2 lineSegmentEnd)
    {
      Vec2 vec2 = lineSegmentEnd - lineSegmentBegin;
      if (!vec2.IsNonZero())
        return lineSegmentBegin;
      float num = Vec2.DotProduct(point - lineSegmentBegin, vec2) / Vec2.DotProduct(vec2, vec2);
      if ((double) num < 0.0)
        return lineSegmentBegin;
      return (double) num > 1.0 ? lineSegmentEnd : lineSegmentBegin + vec2 * num;
    }

    public static void SinCos(float a, out float sa, out float ca)
    {
      sa = (float) Math.Sin((double) a);
      ca = (float) Math.Cos((double) a);
    }

    public static string ToOrdinal(int number)
    {
      if (number < 0)
        return number.ToString();
      switch ((long) (number % 100))
      {
        case 11:
        case 12:
        case 13:
          return number.ToString() + "th";
        default:
          switch (number % 10)
          {
            case 1:
              return number.ToString() + "st";
            case 2:
              return number.ToString() + "nd";
            case 3:
              return number.ToString() + "rd";
            default:
              return number.ToString() + "th";
          }
      }
    }

    public static int IndexOfMax<T>(IReadOnlyList<T> array, Func<T, int> func)
    {
      int num1 = int.MinValue;
      int num2 = -1;
      for (int index = 0; index < array.Count; ++index)
      {
        int num3 = func(array[index]);
        if (num3 > num1)
        {
          num1 = num3;
          num2 = index;
        }
      }
      return num2;
    }

    public static int ArrayIndexOfMax<T>(T[] array, Func<T, float> func)
    {
      float num1 = float.MinValue;
      int num2 = -1;
      for (int index = 0; index < array.Length; ++index)
      {
        float num3 = func(array[index]);
        if ((double) num3 > (double) num1)
        {
          num1 = num3;
          num2 = index;
        }
      }
      return num2;
    }

    public static (int, int) ArrayIndicesOfMax2<T>(T[] array, Func<T, float> func)
    {
      float num1 = float.MinValue;
      float num2 = float.MinValue;
      int num3 = -1;
      int num4 = -1;
      for (int index = 0; index < array.Length; ++index)
      {
        float num5 = func(array[index]);
        if ((double) num5 > (double) num2)
        {
          if ((double) num5 > (double) num1)
          {
            num2 = num1;
            num4 = num3;
            num1 = num5;
            num3 = index;
          }
          else
          {
            num2 = num5;
            num4 = index;
          }
        }
      }
      return (num3, num4);
    }

    public static (int, int, int) ArrayIndexOfMax3<T>(T[] array, Func<T, float> func)
    {
      float num1 = float.MinValue;
      float num2 = float.MinValue;
      float num3 = float.MinValue;
      int num4 = -1;
      int num5 = -1;
      int num6 = -1;
      for (int index = 0; index < array.Length; ++index)
      {
        float num7 = func(array[index]);
        if ((double) num7 > (double) num3)
        {
          if ((double) num7 > (double) num2)
          {
            num3 = num2;
            num6 = num5;
            if ((double) num7 > (double) num1)
            {
              num2 = num1;
              num5 = num4;
              num1 = num7;
              num4 = index;
            }
            else
            {
              num2 = num7;
              num5 = index;
            }
          }
          else
          {
            num3 = num7;
            num6 = index;
          }
        }
      }
      return (num4, num5, num6);
    }

    public static (int, int, int, int) ArrayIndexOfMax4<T>(
      IReadOnlyList<T> array,
      Func<T, float> func)
    {
      float num1 = float.MinValue;
      float num2 = float.MinValue;
      float num3 = float.MinValue;
      float num4 = float.MinValue;
      int num5 = -1;
      int num6 = -1;
      int num7 = -1;
      int num8 = -1;
      for (int index = 0; index < array.Count; ++index)
      {
        float num9 = func(array[index]);
        if ((double) num9 > (double) num4)
        {
          if ((double) num9 > (double) num3)
          {
            num4 = num3;
            num8 = num7;
            if ((double) num9 > (double) num2)
            {
              num3 = num2;
              num7 = num6;
              if ((double) num9 > (double) num1)
              {
                num2 = num1;
                num6 = num5;
                num1 = num9;
                num5 = index;
              }
              else
              {
                num2 = num9;
                num6 = index;
              }
            }
            else
            {
              num3 = num9;
              num7 = index;
            }
          }
          else
          {
            num4 = num9;
            num8 = index;
          }
        }
      }
      return (num5, num6, num7, num8);
    }

    public static T MaxElement<T>(IEnumerable<T> collection, Func<T, float> func)
    {
      float num1 = float.MinValue;
      T obj1 = default (T);
      foreach (T obj2 in collection)
      {
        float num2 = func(obj2);
        if ((double) num2 > (double) num1)
        {
          num1 = num2;
          obj1 = obj2;
        }
      }
      return obj1;
    }

    public static (T, T) MaxElements2<T>(IEnumerable<T> collection, Func<T, float> func)
    {
      float num1 = float.MinValue;
      float num2 = float.MinValue;
      T obj1 = default (T);
      T obj2 = default (T);
      foreach (T obj3 in collection)
      {
        float num3 = func(obj3);
        if ((double) num3 > (double) num2)
        {
          if ((double) num3 > (double) num1)
          {
            num2 = num1;
            obj2 = obj1;
            num1 = num3;
            obj1 = obj3;
          }
          else
          {
            num2 = num3;
            obj2 = obj3;
          }
        }
      }
      return (obj1, obj2);
    }

    public static (T, T, T) MaxElements3<T>(IEnumerable<T> collection, Func<T, float> func)
    {
      float num1 = float.MinValue;
      float num2 = float.MinValue;
      float num3 = float.MinValue;
      T obj1 = default (T);
      T obj2 = default (T);
      T obj3 = default (T);
      foreach (T obj4 in collection)
      {
        float num4 = func(obj4);
        if ((double) num4 > (double) num3)
        {
          if ((double) num4 > (double) num2)
          {
            num3 = num2;
            obj3 = obj2;
            if ((double) num4 > (double) num1)
            {
              num2 = num1;
              obj2 = obj1;
              num1 = num4;
              obj1 = obj4;
            }
            else
            {
              num2 = num4;
              obj2 = obj4;
            }
          }
          else
          {
            num3 = num4;
            obj3 = obj4;
          }
        }
      }
      return (obj1, obj2, obj3);
    }

    public static (T, T, T, T) MaxElements4<T>(IEnumerable<T> collection, Func<T, float> func)
    {
      float num1 = float.MinValue;
      float num2 = float.MinValue;
      float num3 = float.MinValue;
      float num4 = float.MinValue;
      T obj1 = default (T);
      T obj2 = default (T);
      T obj3 = default (T);
      T obj4 = default (T);
      foreach (T obj5 in collection)
      {
        float num5 = func(obj5);
        if ((double) num5 > (double) num4)
        {
          if ((double) num5 > (double) num3)
          {
            num4 = num3;
            obj4 = obj3;
            if ((double) num5 > (double) num2)
            {
              num3 = num2;
              obj3 = obj2;
              if ((double) num5 > (double) num1)
              {
                num2 = num1;
                obj2 = obj1;
                num1 = num5;
                obj1 = obj5;
              }
              else
              {
                num2 = num5;
                obj2 = obj5;
              }
            }
            else
            {
              num3 = num5;
              obj3 = obj5;
            }
          }
          else
          {
            num4 = num5;
            obj4 = obj5;
          }
        }
      }
      return (obj1, obj2, obj3, obj4);
    }

    public static (T, T, T, T, T) MaxElements5<T>(IEnumerable<T> collection, Func<T, float> func)
    {
      float num1 = float.MinValue;
      float num2 = float.MinValue;
      float num3 = float.MinValue;
      float num4 = float.MinValue;
      float num5 = float.MinValue;
      T obj1 = default (T);
      T obj2 = default (T);
      T obj3 = default (T);
      T obj4 = default (T);
      T obj5 = default (T);
      foreach (T obj6 in collection)
      {
        float num6 = func(obj6);
        if ((double) num6 > (double) num5)
        {
          if ((double) num6 > (double) num4)
          {
            num5 = num4;
            obj5 = obj4;
            if ((double) num6 > (double) num3)
            {
              num4 = num3;
              obj4 = obj3;
              if ((double) num6 > (double) num2)
              {
                num3 = num2;
                obj3 = obj2;
                if ((double) num6 > (double) num1)
                {
                  num2 = num1;
                  obj2 = obj1;
                  num1 = num6;
                  obj1 = obj6;
                }
                else
                {
                  num2 = num6;
                  obj2 = obj6;
                }
              }
              else
              {
                num3 = num6;
                obj3 = obj6;
              }
            }
            else
            {
              num4 = num6;
              obj4 = obj6;
            }
          }
          else
          {
            num5 = num6;
            obj5 = obj6;
          }
        }
      }
      return (obj1, obj2, obj3, obj4, obj5);
    }

    public static IList<T> TopologySort<T>(
      IEnumerable<T> source,
      Func<T, IEnumerable<T>> getDependencies)
    {
      List<T> sorted = new List<T>();
      Dictionary<T, bool> visited = new Dictionary<T, bool>();
      foreach (T obj in source)
        MBMath.Visit<T>(obj, getDependencies, sorted, visited);
      return (IList<T>) sorted;
    }

    private static void Visit<T>(
      T item,
      Func<T, IEnumerable<T>> getDependencies,
      List<T> sorted,
      Dictionary<T, bool> visited)
    {
      bool flag;
      if (visited.TryGetValue(item, out flag))
      {
        int num = flag ? 1 : 0;
      }
      else
      {
        visited[item] = true;
        IEnumerable<T> objs = getDependencies(item);
        if (objs != null)
        {
          foreach (T obj in objs)
            MBMath.Visit<T>(obj, getDependencies, sorted, visited);
        }
        visited[item] = false;
        sorted.Add(item);
      }
    }
  }
}
