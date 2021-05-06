// Decompiled with JetBrains decompiler
// Type: TaleWorlds.Core.AtmosphereState
// Assembly: TaleWorlds.Core, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 43F7B45C-9AE1-4715-A37B-2364840F62AF
// Assembly location: E:\SteamLibrary\steamapps\common\Mount & Blade II Bannerlord\bin\Win64_Shipping_Client\TaleWorlds.Core.dll

using TaleWorlds.Library;

namespace TaleWorlds.Core
{
  public class AtmosphereState
  {
    public Vec3 Position = Vec3.Zero;
    public float TemperatureAverage;
    public float TemperatureVariance;
    public float HumidityAverage;
    public float HumidityVariance;
    public float distanceForMaxWeight = 1f;
    public float distanceForMinWeight = 1f;
    public string ColorGradeTexture = "";

    public AtmosphereState()
    {
    }

    public AtmosphereState(
      Vec3 position,
      float tempAv,
      float tempVar,
      float humAv,
      float humVar,
      string colorGradeTexture)
    {
      this.Position = position;
      this.TemperatureAverage = tempAv;
      this.TemperatureVariance = tempVar;
      this.HumidityAverage = humAv;
      this.HumidityVariance = humVar;
      this.ColorGradeTexture = colorGradeTexture;
    }
  }
}
