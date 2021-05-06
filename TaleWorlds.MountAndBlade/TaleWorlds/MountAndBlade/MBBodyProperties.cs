// Decompiled with JetBrains decompiler
// Type: TaleWorlds.MountAndBlade.MBBodyProperties
// Assembly: TaleWorlds.MountAndBlade, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3C0E3F42-756F-4407-B817-06BA412A78B0
// Assembly location: E:\SteamLibrary\steamapps\common\Mount & Blade II Bannerlord\bin\Win64_Shipping_Client\TaleWorlds.MountAndBlade.dll

using System;
using System.Collections.Generic;
using TaleWorlds.Core;
using TaleWorlds.Library;

namespace TaleWorlds.MountAndBlade
{
  public static class MBBodyProperties
  {
    public static int GetNumEditableDeformKeys(bool initialGender, int age) => MBAPI.IMBFaceGen.GetNumEditableDeformKeys(initialGender, (float) age);

    public static void GetParamsFromKey(
      ref FaceGenerationParams faceGenerationParams,
      BodyProperties bodyProperties,
      bool earsAreHidden)
    {
      MBAPI.IMBFaceGen.GetParamsFromKey(ref faceGenerationParams, ref bodyProperties, earsAreHidden);
    }

    public static void GetParamsMax(
      int curGender,
      int curAge,
      ref int hairNum,
      ref int beardNum,
      ref int faceTextureNum,
      ref int mouthTextureNum,
      ref int faceTattooNum,
      ref int soundNum,
      ref int eyebrowNum,
      ref float scale)
    {
      MBAPI.IMBFaceGen.GetParamsMax(curGender, (float) curAge, ref hairNum, ref beardNum, ref faceTextureNum, ref mouthTextureNum, ref faceTattooNum, ref soundNum, ref eyebrowNum, ref scale);
    }

    public static void GetZeroProbabilities(
      int curGender,
      float curAge,
      ref float tattooZeroProbability)
    {
      MBAPI.IMBFaceGen.GetZeroProbabilities(curGender, curAge, ref tattooZeroProbability);
    }

    public static void ProduceNumericKeyWithParams(
      FaceGenerationParams faceGenerationParams,
      bool earsAreHidden,
      ref BodyProperties bodyProperties)
    {
      MBAPI.IMBFaceGen.ProduceNumericKeyWithParams(ref faceGenerationParams, earsAreHidden, ref bodyProperties);
    }

    public static void ProduceNumericKeyWithDefaultValues(
      ref BodyProperties initialBodyProperties,
      bool earsAreHidden,
      int gender,
      int age)
    {
      MBAPI.IMBFaceGen.ProduceNumericKeyWithDefaultValues(ref initialBodyProperties, earsAreHidden, gender, (float) age);
    }

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
      BodyProperties outBodyProperties = new BodyProperties();
      MBAPI.IMBFaceGen.GetRandomBodyProperties(isFemale ? 1 : 0, ref bodyPropertiesMin, ref bodyPropertiesMax, hairCoverType, seed, hairTags, beardTags, tatooTags, ref outBodyProperties);
      return outBodyProperties;
    }

    public static DeformKeyData GetDeformKeyData(int keyNo, int gender, int age)
    {
      DeformKeyData deformKeyData = new DeformKeyData();
      MBAPI.IMBFaceGen.GetDeformKeyData(keyNo, ref deformKeyData, gender, (float) age);
      return deformKeyData;
    }

    public static int GetFaceGenInstancesLength(int gender, int age) => MBAPI.IMBFaceGen.GetFaceGenInstancesLength(gender, (float) age);

    public static bool EnforceConstraints(ref FaceGenerationParams faceGenerationParams) => MBAPI.IMBFaceGen.EnforceConstraints(ref faceGenerationParams);

    public static float GetScaleFromKey(int gender, BodyProperties bodyProperties) => MBAPI.IMBFaceGen.GetScaleFromKey(gender, ref bodyProperties);

    public static int GetHairColorCount(int curGender, int age) => MBAPI.IMBFaceGen.GetHairColorCount(curGender, (float) age);

    public static List<uint> GetHairColorGradientPoints(int curGender, int age)
    {
      int hairColorCount = MBBodyProperties.GetHairColorCount(curGender, age);
      List<uint> uintList = new List<uint>();
      Vec3[] colors = new Vec3[hairColorCount];
      MBAPI.IMBFaceGen.GetHairColorGradientPoints(curGender, (float) age, colors);
      foreach (Vec3 vec3 in colors)
        uintList.Add(MBMath.ColorFromRGBA(vec3.x, vec3.y, vec3.z, 1f));
      return uintList;
    }

    public static int GetTatooColorCount(int curGender, int age) => MBAPI.IMBFaceGen.GetTatooColorCount(curGender, (float) age);

    public static List<uint> GetTatooColorGradientPoints(int curGender, int age)
    {
      int tatooColorCount = MBBodyProperties.GetTatooColorCount(curGender, age);
      List<uint> uintList = new List<uint>();
      Vec3[] colors = new Vec3[tatooColorCount];
      MBAPI.IMBFaceGen.GetTatooColorGradientPoints(curGender, (float) age, colors);
      foreach (Vec3 vec3 in colors)
        uintList.Add(MBMath.ColorFromRGBA(vec3.x, vec3.y, vec3.z, 1f));
      return uintList;
    }

    public static int GetSkinColorCount(int curGender, int age) => MBAPI.IMBFaceGen.GetSkinColorCount(curGender, (float) age);

    public static BodyMeshMaturityType GetMaturityType(float age) => (BodyMeshMaturityType) MBAPI.IMBFaceGen.GetMaturityType(age);

    public static List<uint> GetSkinColorGradientPoints(int curGender, int age)
    {
      int skinColorCount = MBBodyProperties.GetSkinColorCount(curGender, age);
      List<uint> uintList = new List<uint>();
      Vec3[] colors = new Vec3[skinColorCount];
      MBAPI.IMBFaceGen.GetSkinColorGradientPoints(curGender, (float) age, colors);
      foreach (Vec3 vec3 in colors)
        uintList.Add(MBMath.ColorFromRGBA(vec3.x, vec3.y, vec3.z, 1f));
      return uintList;
    }

    public static List<bool> GetVoiceTypeUsableForPlayerData(
      int curGender,
      float age,
      int voiceTypeCount)
    {
      bool[] aiArray = new bool[voiceTypeCount];
      MBAPI.IMBFaceGen.GetVoiceTypeUsableForPlayerData(curGender, age, aiArray);
      return new List<bool>((IEnumerable<bool>) aiArray);
    }

    public static void SetHair(ref BodyProperties bodyProperties, int hair, int beard, int tattoo)
    {
      FaceGenerationParams faceGenerationParams = FaceGenerationParams.Create();
      MBBodyProperties.GetParamsFromKey(ref faceGenerationParams, bodyProperties, false);
      if (hair > -1)
        faceGenerationParams._currentHair = hair;
      if (beard > -1)
        faceGenerationParams._curBeard = beard;
      if (tattoo > -1)
        faceGenerationParams._curFaceTattoo = tattoo;
      MBBodyProperties.ProduceNumericKeyWithParams(faceGenerationParams, false, ref bodyProperties);
    }

    public static void SetBody(ref BodyProperties bodyProperties, int build, int weight)
    {
      FaceGenerationParams faceGenerationParams = FaceGenerationParams.Create();
      MBBodyProperties.GetParamsFromKey(ref faceGenerationParams, bodyProperties, false);
      MBBodyProperties.ProduceNumericKeyWithParams(faceGenerationParams, false, ref bodyProperties);
    }

    public static void SetPigmentation(
      ref BodyProperties bodyProperties,
      int skinColor,
      int hairColor,
      int eyeColor)
    {
      FaceGenerationParams faceGenerationParams = FaceGenerationParams.Create();
      MBBodyProperties.GetParamsFromKey(ref faceGenerationParams, bodyProperties, false);
      MBBodyProperties.ProduceNumericKeyWithParams(faceGenerationParams, false, ref bodyProperties);
    }

    public static void GenerateParentKey(
      BodyProperties childBodyProperties,
      ref BodyProperties motherBodyProperties,
      ref BodyProperties fatherBodyProperties)
    {
      FaceGenerationParams faceGenerationParams1 = FaceGenerationParams.Create();
      FaceGenerationParams faceGenerationParams2 = FaceGenerationParams.Create();
      FaceGenerationParams faceGenerationParams3 = FaceGenerationParams.Create();
      MBBodyProperties.GenerationType[] generationTypeArray = new MBBodyProperties.GenerationType[4];
      for (int index = 0; index < generationTypeArray.Length; ++index)
        generationTypeArray[index] = (MBBodyProperties.GenerationType) MBRandom.RandomInt(2);
      MBBodyProperties.GetParamsFromKey(ref faceGenerationParams1, childBodyProperties, false);
      int genInstancesLength = MBBodyProperties.GetFaceGenInstancesLength(faceGenerationParams1._currentGender, (int) faceGenerationParams1._curAge);
      for (int keyNo = 0; keyNo < genInstancesLength; ++keyNo)
      {
        DeformKeyData deformKeyData = MBBodyProperties.GetDeformKeyData(keyNo, faceGenerationParams1._currentGender, (int) faceGenerationParams1._curAge);
        if (deformKeyData.GroupId >= 0 && deformKeyData.GroupId != 0 && (deformKeyData.GroupId != 5 && deformKeyData.GroupId != 6))
        {
          float num = MBRandom.RandomFloat * Math.Min(faceGenerationParams1.KeyWeights[keyNo], 1f - faceGenerationParams1.KeyWeights[keyNo]);
          if (generationTypeArray[deformKeyData.GroupId - 1] == MBBodyProperties.GenerationType.FromMother)
          {
            faceGenerationParams3.KeyWeights[keyNo] = faceGenerationParams1.KeyWeights[keyNo];
            faceGenerationParams2.KeyWeights[keyNo] = faceGenerationParams1.KeyWeights[keyNo] + num;
          }
          else if (generationTypeArray[deformKeyData.GroupId - 1] == MBBodyProperties.GenerationType.FromFather)
          {
            faceGenerationParams2.KeyWeights[keyNo] = faceGenerationParams1.KeyWeights[keyNo];
            faceGenerationParams3.KeyWeights[keyNo] = faceGenerationParams1.KeyWeights[keyNo] + num;
          }
          else
          {
            faceGenerationParams3.KeyWeights[keyNo] = faceGenerationParams1.KeyWeights[keyNo] + num;
            faceGenerationParams2.KeyWeights[keyNo] = faceGenerationParams1.KeyWeights[keyNo] - num;
          }
        }
      }
      faceGenerationParams2._curAge = faceGenerationParams1._curAge + (float) MBRandom.RandomInt(18, 25);
      faceGenerationParams2.SetRandomParamsExceptKeys(0, (int) faceGenerationParams2._curAge, out float _);
      faceGenerationParams2._curFaceTattoo = 0;
      faceGenerationParams3._curAge = faceGenerationParams1._curAge + (float) MBRandom.RandomInt(18, 22);
      faceGenerationParams3.SetRandomParamsExceptKeys(1, (int) faceGenerationParams3._curAge, out float _);
      faceGenerationParams3._curFaceTattoo = 0;
      faceGenerationParams3._heightMultiplier = faceGenerationParams2._heightMultiplier * MBRandom.RandomFloatRanged(0.7f, 0.9f);
      if (faceGenerationParams3._currentHair == 0)
        faceGenerationParams3._currentHair = 1;
      float num1 = MBRandom.RandomFloat * Math.Min(faceGenerationParams1._curSkinColorOffset, 1f - faceGenerationParams1._curSkinColorOffset);
      if (MBRandom.RandomInt(2) == 1)
      {
        faceGenerationParams2._curSkinColorOffset = faceGenerationParams1._curSkinColorOffset + num1;
        faceGenerationParams3._curSkinColorOffset = faceGenerationParams1._curSkinColorOffset - num1;
      }
      else
      {
        faceGenerationParams2._curSkinColorOffset = faceGenerationParams1._curSkinColorOffset - num1;
        faceGenerationParams3._curSkinColorOffset = faceGenerationParams1._curSkinColorOffset + num1;
      }
      MBBodyProperties.ProduceNumericKeyWithParams(faceGenerationParams3, false, ref motherBodyProperties);
      MBBodyProperties.ProduceNumericKeyWithParams(faceGenerationParams2, false, ref fatherBodyProperties);
    }

    public static BodyProperties GetBodyPropertiesWithAge(
      ref BodyProperties bodyProperties,
      float age)
    {
      FaceGenerationParams faceGenerationParams = new FaceGenerationParams();
      MBBodyProperties.GetParamsFromKey(ref faceGenerationParams, bodyProperties, false);
      faceGenerationParams._curAge = age;
      BodyProperties bodyProperties1 = new BodyProperties();
      MBBodyProperties.ProduceNumericKeyWithParams(faceGenerationParams, false, ref bodyProperties1);
      return bodyProperties1;
    }

    public enum GenerationType
    {
      FromMother,
      FromFather,
      Count,
    }
  }
}
