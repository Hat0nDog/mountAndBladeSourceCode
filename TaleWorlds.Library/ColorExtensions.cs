// Decompiled with JetBrains decompiler
// Type: TaleWorlds.Library.ColorExtensions
// Assembly: TaleWorlds.Library, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 271D0DB8-3C5D-4850-A565-542B4CF7D347
// Assembly location: E:\SteamLibrary\steamapps\common\Mount & Blade II Bannerlord\bin\Win64_Shipping_Client\TaleWorlds.Library.dll

namespace TaleWorlds.Library
{
  public static class ColorExtensions
  {
    public static Color AddFactorInHSB(
      this Color rgbColor,
      float hueDifference,
      float saturationDifference,
      float brighnessDifference)
    {
      Vec3 vec3 = MBMath.RGBtoHSB(rgbColor);
      vec3.x = (float) (((double) vec3.x + (double) hueDifference * 360.0) % 360.0);
      if ((double) vec3.x < 0.0)
        vec3.x += 360f;
      vec3.y = MBMath.ClampFloat(vec3.y + saturationDifference, 0.0f, 1f);
      vec3.z = MBMath.ClampFloat(vec3.z + brighnessDifference, 0.0f, 1f);
      return MBMath.HSBtoRGB(vec3.x, vec3.y, vec3.z, rgbColor.Alpha);
    }
  }
}
