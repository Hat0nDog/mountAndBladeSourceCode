// Decompiled with JetBrains decompiler
// Type: TaleWorlds.Core.MBRandom
// Assembly: TaleWorlds.Core, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 43F7B45C-9AE1-4715-A37B-2364840F62AF
// Assembly location: E:\SteamLibrary\steamapps\common\Mount & Blade II Bannerlord\bin\Win64_Shipping_Client\TaleWorlds.Core.dll

using System;
using System.Collections.Generic;
using TaleWorlds.Library;

namespace TaleWorlds.Core
{
  public static class MBRandom
  {
    public const int MaxSeed = 2000;
    private const int ConstantRandomNumber = 16;

    public static Random Random => Game.Current.RandomGenerator;

    public static Random DeterministicRandom => Game.Current.DeterministicRandomGenerator;

    private static bool UseConstantRandom => false;

    public static float RandomFloat
    {
      get
      {
        double num = MBRandom.Random.NextDouble();
        return !MBRandom.UseConstantRandom ? (float) num : 0.16f;
      }
    }

    public static float RandomFloatRanged(float maxVal) => MBRandom.RandomFloat * maxVal;

    public static float RandomFloatRanged(float minVal, float maxVal) => minVal + MBRandom.RandomFloat * (maxVal - minVal);

    public static float RandomFloatNormal
    {
      get
      {
        int num1 = 4;
        float num2;
        float num3;
        do
        {
          num2 = (float) (2.0 * (double) MBRandom.RandomFloat - 1.0);
          float num4 = (float) (2.0 * (double) MBRandom.RandomFloat - 1.0);
          num3 = (float) ((double) num2 * (double) num2 + (double) num4 * (double) num4);
          --num1;
        }
        while ((double) num3 >= 1.0 || (double) num3 == 0.0 && num1 > 0);
        return (float) ((double) num2 * (double) num3 * 1.0);
      }
    }

    public static Vec3 InsideUnitSphere
    {
      get
      {
        float randomFloat1 = MBRandom.RandomFloat;
        float randomFloat2 = MBRandom.RandomFloat;
        double num1 = 2.0 * Math.PI * (double) randomFloat1;
        double num2 = Math.Acos(2.0 * (double) randomFloat2 - 1.0);
        return new Vec3(Convert.ToSingle(Math.Sin(num2) * Math.Cos(num1)), Convert.ToSingle(Math.Sin(num2) * Math.Sin(num1)), Convert.ToSingle(Math.Cos(num2)));
      }
    }

    public static int RandomInt()
    {
      int num = MBRandom.Random.Next();
      return !MBRandom.UseConstantRandom ? num : 16;
    }

    public static int RandomInt(int maxValue)
    {
      if (!MBRandom.UseConstantRandom)
        return MBRandom.Random.Next(maxValue);
      return maxValue != 0 ? 16 % maxValue : 0;
    }

    public static int RandomInt(int minValue, int maxValue)
    {
      int num = MBRandom.Random.Next(minValue, maxValue);
      return !MBRandom.UseConstantRandom ? num : (minValue + 16) % maxValue;
    }

    public static int DeterministicRandomInt(int maxValue) => !MBRandom.UseConstantRandom ? MBRandom.DeterministicRandom.Next(maxValue) : 16 % maxValue;

    public static int RoundRandomized(float f)
    {
      int num = MBMath.Floor(f);
      if ((double) MBRandom.RandomFloat < (double) (f - (float) num))
        ++num;
      return num;
    }

    public static T ChooseWeighted<T>(IEnumerable<T> candidates, Func<T, float> weightFunction) => MBRandom.ChooseWeighted<T>(candidates, weightFunction, out int _);

    public static T ChooseWeighted<T>(
      IEnumerable<T> candidates,
      Func<T, float> weightFunction,
      out int chosenIndex)
    {
      chosenIndex = -1;
      List<(T, float)> valueTupleList = new List<(T, float)>();
      float num1 = 0.0f;
      foreach (T candidate in candidates)
      {
        float num2 = weightFunction(candidate);
        if ((double) num2 > 0.0)
        {
          valueTupleList.Add((candidate, num2));
          num1 += num2;
        }
      }
      float num3 = MBRandom.RandomFloat * num1;
      for (int index = 0; index < valueTupleList.Count; ++index)
      {
        num3 -= valueTupleList[index].Item2;
        if ((double) num3 <= 0.0)
        {
          chosenIndex = index;
          return valueTupleList[index].Item1;
        }
      }
      if (valueTupleList.Count > 0)
      {
        chosenIndex = 0;
        return valueTupleList[0].Item1;
      }
      chosenIndex = -1;
      return default (T);
    }
  }
}
