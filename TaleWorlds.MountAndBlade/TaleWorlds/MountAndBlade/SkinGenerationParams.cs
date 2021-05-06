// Decompiled with JetBrains decompiler
// Type: TaleWorlds.MountAndBlade.SkinGenerationParams
// Assembly: TaleWorlds.MountAndBlade, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3C0E3F42-756F-4407-B817-06BA412A78B0
// Assembly location: E:\SteamLibrary\steamapps\common\Mount & Blade II Bannerlord\bin\Win64_Shipping_Client\TaleWorlds.MountAndBlade.dll

using TaleWorlds.Core;
using TaleWorlds.DotNet;

namespace TaleWorlds.MountAndBlade
{
  [EngineStruct("Skin_generation_params")]
  public struct SkinGenerationParams
  {
    public int _skinMeshesVisibilityMask;
    public Equipment.UnderwearTypes _underwearType;
    public int _bodyMeshType;
    public int _hairCoverType;
    public int _beardCoverType;
    public int _bodyDeformType;
    public bool _prepareImmediately;
    public bool _useTranslucency;
    public bool _useTesselation;
    public float _faceDirtAmount;
    public int _gender;

    public static SkinGenerationParams Create()
    {
      SkinGenerationParams generationParams;
      generationParams._skinMeshesVisibilityMask = 481;
      generationParams._underwearType = Equipment.UnderwearTypes.FullUnderwear;
      generationParams._bodyMeshType = 0;
      generationParams._hairCoverType = 0;
      generationParams._beardCoverType = 0;
      generationParams._prepareImmediately = false;
      generationParams._bodyDeformType = -1;
      generationParams._faceDirtAmount = 0.0f;
      generationParams._gender = 0;
      generationParams._useTranslucency = false;
      generationParams._useTesselation = false;
      return generationParams;
    }

    public SkinGenerationParams(
      int skinMeshesVisibilityMask,
      Equipment.UnderwearTypes underwearType,
      int bodyMeshType,
      int hairCoverType,
      int beardCoverType,
      int bodyDeformType,
      bool prepareImmediately,
      float faceDirtAmount,
      int gender,
      bool useTranslucency,
      bool useTesselation)
    {
      this._skinMeshesVisibilityMask = skinMeshesVisibilityMask;
      this._underwearType = underwearType;
      this._bodyMeshType = bodyMeshType;
      this._hairCoverType = hairCoverType;
      this._beardCoverType = beardCoverType;
      this._bodyDeformType = bodyDeformType;
      this._prepareImmediately = prepareImmediately;
      this._faceDirtAmount = faceDirtAmount;
      this._gender = gender;
      this._useTranslucency = useTranslucency;
      this._useTesselation = useTesselation;
    }
  }
}
