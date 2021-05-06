// Decompiled with JetBrains decompiler
// Type: TaleWorlds.MountAndBlade.AgentVisualsData
// Assembly: TaleWorlds.MountAndBlade, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3C0E3F42-756F-4407-B817-06BA412A78B0
// Assembly location: E:\SteamLibrary\steamapps\common\Mount & Blade II Bannerlord\bin\Win64_Shipping_Client\TaleWorlds.MountAndBlade.dll

using TaleWorlds.Core;
using TaleWorlds.Engine;
using TaleWorlds.Library;

namespace TaleWorlds.MountAndBlade
{
  public class AgentVisualsData
  {
    public MBAgentVisuals AgentVisuals;

    public MBActionSet ActionSetData { get; private set; }

    public MatrixFrame FrameData { get; private set; }

    public TaleWorlds.Core.BodyProperties BodyPropertiesData { get; private set; }

    public TaleWorlds.Core.Equipment EquipmentData { get; private set; }

    public int RightWieldedItemIndexData { get; private set; }

    public int LeftWieldedItemIndexData { get; private set; }

    public TaleWorlds.MountAndBlade.SkeletonType SkeletonTypeData { get; private set; }

    public TaleWorlds.Core.Banner BannerData { get; private set; }

    public GameEntity CachedWeaponSlot0Entity { get; private set; }

    public GameEntity CachedWeaponSlot1Entity { get; private set; }

    public GameEntity CachedWeaponSlot2Entity { get; private set; }

    public GameEntity CachedWeaponSlot3Entity { get; private set; }

    public GameEntity CachedWeaponSlot4Entity { get; private set; }

    public TaleWorlds.Engine.Scene SceneData { get; private set; }

    public TaleWorlds.Core.Monster MonsterData { get; private set; }

    public bool PrepareImmediatelyData { get; private set; }

    public bool UseScaledWeaponsData { get; private set; }

    public bool UseTranslucencyData { get; private set; }

    public bool UseTesselationData { get; private set; }

    public bool UseMorphAnimsData { get; private set; }

    public uint ClothColor1Data { get; private set; }

    public uint ClothColor2Data { get; private set; }

    public float ScaleData { get; private set; }

    public string CharacterObjectStringIdData { get; private set; }

    public ActionIndexCache ActionCodeData { get; private set; } = ActionIndexCache.act_none;

    public GameEntity EntityData { get; private set; }

    public bool HasClippingPlaneData { get; private set; }

    public string MountCreationKeyData { get; private set; }

    public bool AddColorRandomnessData { get; private set; }

    public AgentVisualsData(AgentVisualsData agentVisualsData)
    {
      this.AgentVisuals = agentVisualsData.AgentVisuals;
      this.ActionSetData = agentVisualsData.ActionSetData;
      this.FrameData = agentVisualsData.FrameData;
      this.BodyPropertiesData = agentVisualsData.BodyPropertiesData;
      this.EquipmentData = agentVisualsData.EquipmentData;
      this.RightWieldedItemIndexData = agentVisualsData.RightWieldedItemIndexData;
      this.LeftWieldedItemIndexData = agentVisualsData.LeftWieldedItemIndexData;
      this.SkeletonTypeData = agentVisualsData.SkeletonTypeData;
      this.BannerData = agentVisualsData.BannerData;
      this.CachedWeaponSlot0Entity = agentVisualsData.CachedWeaponSlot0Entity;
      this.CachedWeaponSlot1Entity = agentVisualsData.CachedWeaponSlot1Entity;
      this.CachedWeaponSlot2Entity = agentVisualsData.CachedWeaponSlot2Entity;
      this.CachedWeaponSlot3Entity = agentVisualsData.CachedWeaponSlot3Entity;
      this.CachedWeaponSlot4Entity = agentVisualsData.CachedWeaponSlot4Entity;
      this.SceneData = agentVisualsData.SceneData;
      this.MonsterData = agentVisualsData.MonsterData;
      this.PrepareImmediatelyData = agentVisualsData.PrepareImmediatelyData;
      this.UseScaledWeaponsData = agentVisualsData.UseScaledWeaponsData;
      this.UseTranslucencyData = agentVisualsData.UseTranslucencyData;
      this.UseTesselationData = agentVisualsData.UseTesselationData;
      this.UseMorphAnimsData = agentVisualsData.UseMorphAnimsData;
      this.ClothColor1Data = agentVisualsData.ClothColor1Data;
      this.ClothColor2Data = agentVisualsData.ClothColor2Data;
      this.ScaleData = agentVisualsData.ScaleData;
      this.ActionCodeData = agentVisualsData.ActionCodeData;
      this.EntityData = agentVisualsData.EntityData;
      this.CharacterObjectStringIdData = agentVisualsData.CharacterObjectStringIdData;
      this.HasClippingPlaneData = agentVisualsData.HasClippingPlaneData;
      this.MountCreationKeyData = agentVisualsData.MountCreationKeyData;
      this.AddColorRandomnessData = agentVisualsData.AddColorRandomnessData;
    }

    public AgentVisualsData()
    {
      this.ClothColor1Data = uint.MaxValue;
      this.ClothColor2Data = uint.MaxValue;
      this.RightWieldedItemIndexData = -1;
      this.LeftWieldedItemIndexData = -1;
      this.ScaleData = 0.0f;
    }

    public AgentVisualsData Equipment(TaleWorlds.Core.Equipment equipment)
    {
      this.EquipmentData = equipment;
      return this;
    }

    public AgentVisualsData BodyProperties(TaleWorlds.Core.BodyProperties bodyProperties)
    {
      this.BodyPropertiesData = bodyProperties;
      return this;
    }

    public AgentVisualsData Frame(MatrixFrame frame)
    {
      this.FrameData = frame;
      return this;
    }

    public AgentVisualsData ActionSet(MBActionSet actionSet)
    {
      this.ActionSetData = actionSet;
      return this;
    }

    public AgentVisualsData Scene(TaleWorlds.Engine.Scene scene)
    {
      this.SceneData = scene;
      return this;
    }

    public AgentVisualsData Monster(TaleWorlds.Core.Monster monster)
    {
      this.MonsterData = monster;
      return this;
    }

    public AgentVisualsData PrepareImmediately(bool prepareImmediately)
    {
      this.PrepareImmediatelyData = prepareImmediately;
      return this;
    }

    public AgentVisualsData UseScaledWeapons(bool useScaledWeapons)
    {
      this.UseScaledWeaponsData = useScaledWeapons;
      return this;
    }

    public AgentVisualsData SkeletonType(TaleWorlds.MountAndBlade.SkeletonType skeletonType)
    {
      this.SkeletonTypeData = skeletonType;
      return this;
    }

    public AgentVisualsData UseMorphAnims(bool useMorphAnims)
    {
      this.UseMorphAnimsData = useMorphAnims;
      return this;
    }

    public AgentVisualsData ClothColor1(uint clothColor1)
    {
      this.ClothColor1Data = clothColor1;
      return this;
    }

    public AgentVisualsData ClothColor2(uint clothColor2)
    {
      this.ClothColor2Data = clothColor2;
      return this;
    }

    public AgentVisualsData Banner(TaleWorlds.Core.Banner banner)
    {
      this.BannerData = banner;
      return this;
    }

    public GameEntity GetCachedWeaponEntity(EquipmentIndex slotIndex)
    {
      switch (slotIndex)
      {
        case EquipmentIndex.WeaponItemBeginSlot:
          return this.CachedWeaponSlot0Entity;
        case EquipmentIndex.Weapon1:
          return this.CachedWeaponSlot1Entity;
        case EquipmentIndex.Weapon2:
          return this.CachedWeaponSlot2Entity;
        case EquipmentIndex.Weapon3:
          return this.CachedWeaponSlot3Entity;
        case EquipmentIndex.Weapon4:
          return this.CachedWeaponSlot4Entity;
        default:
          return (GameEntity) null;
      }
    }

    public AgentVisualsData CachedWeaponEntity(
      EquipmentIndex slotIndex,
      GameEntity cachedWeaponEntity)
    {
      switch (slotIndex)
      {
        case EquipmentIndex.WeaponItemBeginSlot:
          this.CachedWeaponSlot0Entity = cachedWeaponEntity;
          break;
        case EquipmentIndex.Weapon1:
          this.CachedWeaponSlot1Entity = cachedWeaponEntity;
          break;
        case EquipmentIndex.Weapon2:
          this.CachedWeaponSlot2Entity = cachedWeaponEntity;
          break;
        case EquipmentIndex.Weapon3:
          this.CachedWeaponSlot3Entity = cachedWeaponEntity;
          break;
        case EquipmentIndex.Weapon4:
          this.CachedWeaponSlot4Entity = cachedWeaponEntity;
          break;
      }
      return this;
    }

    public AgentVisualsData Entity(GameEntity entity)
    {
      this.EntityData = entity;
      return this;
    }

    public AgentVisualsData UseTranslucency(bool useTranslucency)
    {
      this.UseTranslucencyData = useTranslucency;
      return this;
    }

    public AgentVisualsData UseTesselation(bool useTesselation)
    {
      this.UseTesselationData = useTesselation;
      return this;
    }

    public AgentVisualsData ActionCode(ActionIndexCache actionCode)
    {
      this.ActionCodeData = actionCode;
      return this;
    }

    public AgentVisualsData RightWieldedItemIndex(int rightWieldedItemIndex)
    {
      this.RightWieldedItemIndexData = rightWieldedItemIndex;
      return this;
    }

    public AgentVisualsData LeftWieldedItemIndex(int leftWieldedItemIndex)
    {
      this.LeftWieldedItemIndexData = leftWieldedItemIndex;
      return this;
    }

    public AgentVisualsData Scale(float scale)
    {
      this.ScaleData = scale;
      return this;
    }

    public AgentVisualsData CharacterObjectStringId(string characterObjectStringId)
    {
      this.CharacterObjectStringIdData = characterObjectStringId;
      return this;
    }

    public AgentVisualsData HasClippingPlane(bool hasClippingPlane)
    {
      this.HasClippingPlaneData = hasClippingPlane;
      return this;
    }

    public AgentVisualsData MountCreationKey(string mountCreationKey)
    {
      this.MountCreationKeyData = mountCreationKey;
      return this;
    }

    public AgentVisualsData AddColorRandomness(bool addColorRandomness)
    {
      this.AddColorRandomnessData = addColorRandomness;
      return this;
    }
  }
}
