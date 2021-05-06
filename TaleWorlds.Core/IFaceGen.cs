// Decompiled with JetBrains decompiler
// Type: TaleWorlds.Core.IFaceGen
// Assembly: TaleWorlds.Core, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 43F7B45C-9AE1-4715-A37B-2364840F62AF
// Assembly location: E:\SteamLibrary\steamapps\common\Mount & Blade II Bannerlord\bin\Win64_Shipping_Client\TaleWorlds.Core.dll

namespace TaleWorlds.Core
{
  public interface IFaceGen
  {
    BodyProperties GetRandomBodyProperties(
      bool isFemale,
      BodyProperties bodyPropertiesMin,
      BodyProperties bodyPropertiesMax,
      int hairCoverType,
      int seed,
      string hairTags,
      string beardTags,
      string tatooTags);

    void GenerateParentBody(
      BodyProperties childBodyProperties,
      ref BodyProperties motherBodyProperties,
      ref BodyProperties fatherBodyProperties);

    void SetBody(ref BodyProperties bodyProperties, int build, int weight);

    void SetHair(ref BodyProperties bodyProperties, int hair, int beard, int tattoo);

    void SetPigmentation(
      ref BodyProperties bodyProperties,
      int skinColor,
      int hairColor,
      int eyeColor);

    BodyProperties GetBodyPropertiesWithAge(
      ref BodyProperties bodyProperties,
      float age);

    BodyMeshMaturityType GetMaturityTypeWithAge(float age);
  }
}
