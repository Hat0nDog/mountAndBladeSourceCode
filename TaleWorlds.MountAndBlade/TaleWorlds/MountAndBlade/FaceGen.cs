// Decompiled with JetBrains decompiler
// Type: TaleWorlds.MountAndBlade.FaceGen
// Assembly: TaleWorlds.MountAndBlade, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3C0E3F42-756F-4407-B817-06BA412A78B0
// Assembly location: E:\SteamLibrary\steamapps\common\Mount & Blade II Bannerlord\bin\Win64_Shipping_Client\TaleWorlds.MountAndBlade.dll

using TaleWorlds.Core;

namespace TaleWorlds.MountAndBlade
{
  public class FaceGen : IFaceGen
  {
    public static void CreateInstance() => TaleWorlds.Core.FaceGen.SetInstance((IFaceGen) new FaceGen());

    public BodyProperties GetRandomBodyProperties(
      bool isFemale,
      BodyProperties bodyPropertiesMin,
      BodyProperties bodyPropertiesMax,
      int hairCoverType,
      int seed,
      string hairTags,
      string beardTags,
      string tatooTags)
    {
      return MBBodyProperties.GetRandomBodyProperties(isFemale, bodyPropertiesMin, bodyPropertiesMax, hairCoverType, seed, hairTags, beardTags, tatooTags);
    }

    void IFaceGen.GenerateParentBody(
      BodyProperties childBodyProperties,
      ref BodyProperties motherBodyProperties,
      ref BodyProperties fatherBodyProperties)
    {
      MBBodyProperties.GenerateParentKey(childBodyProperties, ref motherBodyProperties, ref fatherBodyProperties);
    }

    void IFaceGen.SetHair(
      ref BodyProperties bodyProperties,
      int hair,
      int beard,
      int tattoo)
    {
      MBBodyProperties.SetHair(ref bodyProperties, hair, beard, tattoo);
    }

    void IFaceGen.SetBody(ref BodyProperties bodyProperties, int build, int weight) => MBBodyProperties.SetBody(ref bodyProperties, build, weight);

    void IFaceGen.SetPigmentation(
      ref BodyProperties bodyProperties,
      int skinColor,
      int hairColor,
      int eyeColor)
    {
      MBBodyProperties.SetPigmentation(ref bodyProperties, skinColor, hairColor, eyeColor);
    }

    public BodyProperties GetBodyPropertiesWithAge(
      ref BodyProperties bodyProperties,
      float age)
    {
      return MBBodyProperties.GetBodyPropertiesWithAge(ref bodyProperties, age);
    }

    public void GetParamsFromBody(
      ref FaceGenerationParams faceGenerationParams,
      BodyProperties bodyProperties,
      bool earsAreHidden)
    {
      MBBodyProperties.GetParamsFromKey(ref faceGenerationParams, bodyProperties, earsAreHidden);
    }

    public BodyMeshMaturityType GetMaturityTypeWithAge(float age) => MBBodyProperties.GetMaturityType(age);
  }
}
