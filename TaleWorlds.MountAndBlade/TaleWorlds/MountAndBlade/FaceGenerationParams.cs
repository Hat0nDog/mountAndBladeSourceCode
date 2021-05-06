// Decompiled with JetBrains decompiler
// Type: TaleWorlds.MountAndBlade.FaceGenerationParams
// Assembly: TaleWorlds.MountAndBlade, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3C0E3F42-756F-4407-B817-06BA412A78B0
// Assembly location: E:\SteamLibrary\steamapps\common\Mount & Blade II Bannerlord\bin\Win64_Shipping_Client\TaleWorlds.MountAndBlade.dll

using System.Runtime.InteropServices;
using TaleWorlds.Core;
using TaleWorlds.DotNet;
using TaleWorlds.Library;

namespace TaleWorlds.MountAndBlade
{
  [EngineStruct("Face_generation_params")]
  public struct FaceGenerationParams
  {
    public int seed_;
    public int _curBeard;
    public int _currentHair;
    public int _curEyebrow;
    public int _currentGender;
    public int _curFaceTexture;
    public int _curMouthTexture;
    public int _curFaceTattoo;
    public int _currentVoice;
    public int hair_filter_;
    public int beard_filter_;
    public int tattoo_filter_;
    public int face_texture_filter_;
    public float _tattooZeroProbability;
    [MarshalAs(UnmanagedType.ByValArray, SizeConst = 320)]
    public float[] KeyWeights;
    public float _curAge;
    public float _curWeight;
    public float _curBuild;
    public float _curSkinColorOffset;
    public float _curHairColorOffset;
    public float _curEyeColorOffset;
    public float face_dirt_amount_;
    public float _curFaceTattooColorOffset1;
    public float _heightMultiplier;
    public float _voicePitch;
    public bool _isHairFlipped;
    public bool _useCache;
    public bool _padding1;
    public bool _padding2;

    public static FaceGenerationParams Create()
    {
      FaceGenerationParams generationParams;
      generationParams.seed_ = 0;
      generationParams._curBeard = 0;
      generationParams._currentHair = 0;
      generationParams._curEyebrow = 0;
      generationParams._isHairFlipped = false;
      generationParams._currentGender = 0;
      generationParams._curFaceTexture = 0;
      generationParams._curMouthTexture = 0;
      generationParams._curFaceTattoo = 0;
      generationParams._currentVoice = 0;
      generationParams.hair_filter_ = 0;
      generationParams.beard_filter_ = 0;
      generationParams.tattoo_filter_ = 0;
      generationParams.face_texture_filter_ = 0;
      generationParams._tattooZeroProbability = 0.0f;
      generationParams.KeyWeights = new float[320];
      generationParams._curAge = 0.0f;
      generationParams._curWeight = 0.0f;
      generationParams._curBuild = 0.0f;
      generationParams._curSkinColorOffset = 0.0f;
      generationParams._curHairColorOffset = 0.0f;
      generationParams._curEyeColorOffset = 0.0f;
      generationParams.face_dirt_amount_ = 0.0f;
      generationParams._curFaceTattooColorOffset1 = 0.0f;
      generationParams._heightMultiplier = 0.0f;
      generationParams._voicePitch = 0.0f;
      generationParams._useCache = false;
      generationParams._padding1 = false;
      generationParams._padding2 = false;
      return generationParams;
    }

    public void SetGenderAndAdjustParams(int gender, int curAge)
    {
      this._currentGender = gender;
      int hairNum = 0;
      int beardNum = 0;
      int faceTextureNum = 0;
      int mouthTextureNum = 0;
      int eyebrowNum = 0;
      int soundNum = 0;
      int faceTattooNum = 0;
      float scale = 0.0f;
      MBBodyProperties.GetParamsMax(gender, curAge, ref hairNum, ref beardNum, ref faceTextureNum, ref mouthTextureNum, ref faceTattooNum, ref soundNum, ref eyebrowNum, ref scale);
      this._currentHair = MBMath.ClampInt(this._currentHair, 0, hairNum - 1);
      this._curBeard = MBMath.ClampInt(this._curBeard, 0, beardNum - 1);
      this._curFaceTexture = MBMath.ClampInt(this._curFaceTexture, 0, faceTextureNum - 1);
      this._curMouthTexture = MBMath.ClampInt(this._curMouthTexture, 0, mouthTextureNum - 1);
      this._curFaceTattoo = MBMath.ClampInt(this._curFaceTattoo, 0, faceTattooNum - 1);
      this._currentVoice = MBMath.ClampInt(this._currentVoice, 0, soundNum - 1);
      this._voicePitch = MBMath.ClampFloat(this._voicePitch, 0.0f, 1f);
      this._curEyebrow = MBMath.ClampInt(this._curEyebrow, 0, eyebrowNum - 1);
    }

    public void SetRandomParamsExceptKeys(int gender, int minAge, out float scale)
    {
      int hairNum = 0;
      int beardNum = 0;
      int faceTextureNum = 0;
      int mouthTextureNum = 0;
      int eyebrowNum = 0;
      int soundNum = 0;
      int faceTattooNum = 0;
      scale = 0.0f;
      MBBodyProperties.GetParamsMax(gender, minAge, ref hairNum, ref beardNum, ref faceTextureNum, ref mouthTextureNum, ref faceTattooNum, ref soundNum, ref eyebrowNum, ref scale);
      this._currentHair = MBRandom.RandomInt(hairNum);
      this._curBeard = MBRandom.RandomInt(beardNum);
      this._curFaceTexture = MBRandom.RandomInt(faceTextureNum);
      this._curMouthTexture = MBRandom.RandomInt(mouthTextureNum);
      this._curFaceTattoo = MBRandom.RandomInt(faceTattooNum);
      this._currentVoice = MBRandom.RandomInt(soundNum);
      this._voicePitch = MBRandom.RandomFloat;
      this._curEyebrow = MBRandom.RandomInt(eyebrowNum);
      this._curSkinColorOffset = MBRandom.RandomFloat;
      this._curHairColorOffset = MBRandom.RandomFloat;
      this._curEyeColorOffset = MBRandom.RandomFloat;
      this._curFaceTattooColorOffset1 = MBRandom.RandomFloat;
      this._heightMultiplier = MBRandom.RandomFloat;
    }
  }
}
