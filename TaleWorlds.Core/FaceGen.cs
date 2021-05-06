// Decompiled with JetBrains decompiler
// Type: TaleWorlds.Core.FaceGen
// Assembly: TaleWorlds.Core, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 43F7B45C-9AE1-4715-A37B-2364840F62AF
// Assembly location: E:\SteamLibrary\steamapps\common\Mount & Blade II Bannerlord\bin\Win64_Shipping_Client\TaleWorlds.Core.dll

namespace TaleWorlds.Core
{
  public class FaceGen
  {
    public static bool ShowDebugValues;
    private static IFaceGen _instance;

    public static void SetInstance(IFaceGen faceGen) => FaceGen._instance = faceGen;

    public static BodyProperties GetRandomBodyProperties(
      bool isFemale,
      BodyProperties bodyPropertiesMin,
      BodyProperties bodyPropertiesMax,
      int hairCoverType,
      int seed,
      string hairTags,
      string beardTags,
      string tatooTags)
    {
      return FaceGen._instance != null ? FaceGen._instance.GetRandomBodyProperties(isFemale, bodyPropertiesMin, bodyPropertiesMax, hairCoverType, seed, hairTags, beardTags, tatooTags) : bodyPropertiesMin;
    }

    public static void GenerateParentKey(
      BodyProperties childBodyProperties,
      ref BodyProperties motherBodyProperties,
      ref BodyProperties fatherBodyProperties)
    {
      if (FaceGen._instance == null)
        return;
      FaceGen._instance.GenerateParentBody(childBodyProperties, ref motherBodyProperties, ref fatherBodyProperties);
    }

    public static void SetHair(ref BodyProperties bodyProperties, int hair, int beard, int tattoo)
    {
      if (FaceGen._instance == null)
        return;
      FaceGen._instance.SetHair(ref bodyProperties, hair, beard, tattoo);
    }

    public static void SetBody(ref BodyProperties bodyProperties, int build, int weight)
    {
      if (FaceGen._instance == null)
        return;
      FaceGen._instance.SetBody(ref bodyProperties, build, weight);
    }

    public static void SetPigmentation(
      ref BodyProperties bodyProperties,
      int skinColor,
      int hairColor,
      int eyeColor)
    {
      if (FaceGen._instance == null)
        return;
      FaceGen._instance.SetPigmentation(ref bodyProperties, skinColor, hairColor, eyeColor);
    }

    public static BodyProperties GetBodyPropertiesWithAge(
      ref BodyProperties originalBodyProperties,
      float age)
    {
      return FaceGen._instance != null ? FaceGen._instance.GetBodyPropertiesWithAge(ref originalBodyProperties, age) : originalBodyProperties;
    }

    public static BodyMeshMaturityType GetMaturityTypeWithAge(float age) => FaceGen._instance != null ? FaceGen._instance.GetMaturityTypeWithAge(age) : BodyMeshMaturityType.Child;
  }
}
