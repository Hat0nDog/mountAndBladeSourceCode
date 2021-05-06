// Decompiled with JetBrains decompiler
// Type: TaleWorlds.Library.CubicBezier
// Assembly: TaleWorlds.Library, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 271D0DB8-3C5D-4850-A565-542B4CF7D347
// Assembly location: E:\SteamLibrary\steamapps\common\Mount & Blade II Bannerlord\bin\Win64_Shipping_Client\TaleWorlds.Library.dll

using System;

namespace TaleWorlds.Library
{
  public class CubicBezier
  {
    private readonly double _x1;
    private readonly double _y1;
    private readonly double _x2;
    private readonly double _y2;
    private readonly double _y0;
    private readonly double _y3;
    private const int NewtonIterations = 4;
    private const double NewtonMinSlope = 0.001;
    private const double SubdivisionPrecision = 1E-07;
    private const int SubdivisionMaxIterations = 10;
    private const int KSplineTableSize = 11;
    private const double KSampleStepSize = 0.1;
    private readonly double[] _sampleValues = new double[11];

    public static CubicBezier CreateEase(
      double controlPoint1X,
      double controlPoint1Y,
      double controlPoint2X,
      double controlPoint2Y)
    {
      return new CubicBezier(controlPoint1X, controlPoint1Y, controlPoint2X, controlPoint2Y);
    }

    public static CubicBezier CreateYBeginToYEndWithRelativeControlDirs(
      double yBegin,
      double yEnd,
      double controlDir1X,
      double controlDir1Y,
      double controlDir2X,
      double controlDir2Y)
    {
      return new CubicBezier(controlDir1X, yBegin + controlDir1Y, 1.0 + controlDir2X, yEnd + controlDir2Y, yBegin, yEnd);
    }

    public static CubicBezier CreateYBeginToYEnd(
      double yBegin,
      double yEnd,
      double controlPoint1X,
      double controlPoint1Y,
      double controlPoint2X,
      double controlPoint2Y)
    {
      return new CubicBezier(controlPoint1X, controlPoint1Y, controlPoint2X, controlPoint2Y, yBegin, yEnd);
    }

    private CubicBezier(double x1, double y1, double x2, double y2, double yBegin = 0.0, double yEnd = 1.0)
    {
      this._y0 = yBegin;
      this._y3 = yEnd;
      this._x1 = x1;
      this._y1 = y1;
      this._x2 = x2;
      this._y2 = y2;
      if (0.0 > this._x1 || this._x1 > 1.0 || (0.0 > this._x2 || this._x2 > 1.0))
        throw new ArgumentOutOfRangeException();
      for (int index = 0; index < 11; ++index)
        this._sampleValues[index] = CubicBezier.CalcBezierX((double) index * 0.1, this._x1, this._x2);
    }

    public double Sample(double x)
    {
      if (CubicBezier.AlmostEq(x, 0.0))
        return this._y0;
      if (CubicBezier.AlmostEq(x, 1.0))
        return this._y3;
      double num = this._y3 - this._y0;
      return CubicBezier.AlmostEq(this._x1 * num, this._y1 - this._y0) && CubicBezier.AlmostEq((1.0 - this._x2) * num, this._y3 - this._y2) ? this._y0 + x * num : CubicBezier.CalcBezierY(this.GetTForX(x), this._y0, this._y1, this._y2, this._y3);
    }

    private static bool AlmostEq(double a, double b) => Math.Abs(a - b) < 1E-09;

    private static double AY(double aA0, double aA1, double aA2, double aA3) => 1.0 * aA3 - 3.0 * aA2 + 3.0 * aA1 - 1.0 * aA0;

    private static double A(double aA1, double aA2) => 1.0 - 3.0 * aA2 + 3.0 * aA1;

    private static double BY(double aA0, double aA1, double aA2) => 3.0 * aA2 - 6.0 * aA1 + 3.0 * aA0;

    private static double B(double aA1, double aA2) => 3.0 * aA2 - 6.0 * aA1;

    private static double CY(double aA0, double aA1) => 3.0 * aA1 - 3.0 * aA0;

    private static double C(double aA1) => 3.0 * aA1;

    private static double DY(double aA0) => aA0;

    private static double CalcBezierY(double aT, double aA0, double aA1, double aA2, double aA3) => ((CubicBezier.AY(aA0, aA1, aA2, aA3) * aT + CubicBezier.BY(aA0, aA1, aA2)) * aT + CubicBezier.CY(aA0, aA1)) * aT + CubicBezier.DY(aA0);

    private static double CalcBezierX(double aT, double aA1, double aA2) => ((CubicBezier.A(aA1, aA2) * aT + CubicBezier.B(aA1, aA2)) * aT + CubicBezier.C(aA1)) * aT;

    private static double GetSlopeX(double aT, double aA1, double aA2) => 3.0 * CubicBezier.A(aA1, aA2) * aT * aT + 2.0 * CubicBezier.B(aA1, aA2) * aT + CubicBezier.C(aA1);

    private static double BinarySubdivide(
      double aX,
      double aA,
      double aB,
      double mX1,
      double mX2)
    {
      int num1 = 0;
      double aT;
      double num2;
      do
      {
        aT = aA + (aB - aA) / 2.0;
        num2 = CubicBezier.CalcBezierX(aT, mX1, mX2) - aX;
        if (num2 > 0.0)
          aB = aT;
        else
          aA = aT;
      }
      while (Math.Abs(num2) > 1E-07 && ++num1 < 10);
      return aT;
    }

    private double NewtonRaphsonIterate(double aX, double aGuessT, double mX1, double mX2)
    {
      for (int index = 0; index < 4; ++index)
      {
        double slopeX = CubicBezier.GetSlopeX(aGuessT, mX1, mX2);
        if (Math.Abs(slopeX) < 1E-07)
          return aGuessT;
        double num = CubicBezier.CalcBezierX(aGuessT, mX1, mX2) - aX;
        aGuessT -= num / slopeX;
      }
      return aGuessT;
    }

    private double GetTForX(double aX)
    {
      double aA = 0.0;
      int index1 = 1;
      for (int index2 = 10; index1 != index2 && this._sampleValues[index1] <= aX; ++index1)
        aA += 0.1;
      int index3 = index1 - 1;
      double num1 = (aX - this._sampleValues[index3]) / (this._sampleValues[index3 + 1] - this._sampleValues[index3]);
      double num2 = aA + num1 * 0.1;
      double slopeX = CubicBezier.GetSlopeX(num2, this._x1, this._x2);
      return slopeX < 0.001 ? (!CubicBezier.AlmostEq(slopeX, 0.0) ? CubicBezier.BinarySubdivide(aX, aA, aA + 0.1, this._x1, this._x2) : num2) : this.NewtonRaphsonIterate(aX, num2, this._x1, this._x2);
    }
  }
}
