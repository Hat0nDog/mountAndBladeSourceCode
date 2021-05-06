// Decompiled with JetBrains decompiler
// Type: TaleWorlds.Core.Monster
// Assembly: TaleWorlds.Core, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 43F7B45C-9AE1-4715-A37B-2364840F62AF
// Assembly location: E:\SteamLibrary\steamapps\common\Mount & Blade II Bannerlord\bin\Win64_Shipping_Client\TaleWorlds.Core.dll

using System;
using System.Xml;
using TaleWorlds.Library;
using TaleWorlds.ObjectSystem;

namespace TaleWorlds.Core
{
  public sealed class Monster : MBObjectBase
  {
    public static Func<string, string, sbyte> GetBoneIndexWithId;
    private IMonsterMissionData _monsterMissionData;

    public float BodyCapsuleRadius { get; private set; }

    public Vec3 BodyCapsulePoint1 { get; private set; }

    public Vec3 BodyCapsulePoint2 { get; private set; }

    public float CrouchedBodyCapsuleRadius { get; private set; }

    public Vec3 CrouchedBodyCapsulePoint1 { get; private set; }

    public Vec3 CrouchedBodyCapsulePoint2 { get; private set; }

    public AgentFlag Flags { get; private set; }

    public int Weight { get; private set; }

    public int HitPoints { get; private set; }

    public string ActionSetCode { get; private set; }

    public string MonsterUsage { get; private set; }

    public int NumPaces { get; private set; }

    public float WalkingSpeedLimit { get; private set; }

    public float CrouchWalkingSpeedLimit { get; private set; }

    public float JumpAcceleration { get; private set; }

    public float AbsorbedDamageRatio { get; private set; }

    public string SoundAndCollisionInfoClassName { get; private set; }

    public float RiderCameraHeightAdder { get; private set; }

    public float RiderBodyCapsuleHeightAdder { get; private set; }

    public float RiderBodyCapsuleForwardAdder { get; private set; }

    public float StandingEyeHeight { get; private set; }

    public float CrouchEyeHeight { get; private set; }

    public float MountedEyeHeight { get; private set; }

    public float RiderEyeHeightAdder { get; private set; }

    public Vec3 EyeOffsetWrtHead { get; private set; }

    public Vec3 FirstPersonCameraOffsetWrtHead { get; private set; }

    public float ArmLength { get; private set; }

    public float ArmWeight { get; private set; }

    public float JumpSpeedLimit { get; private set; }

    public float RelativeSpeedLimitForCharge { get; private set; }

    public int FamilyType { get; private set; }

    public sbyte MainHandItemBoneIndex { get; private set; }

    public sbyte OffHandItemBoneIndex { get; private set; }

    public sbyte MainHandItemSecondaryBoneIndex { get; private set; }

    public sbyte OffHandItemSecondaryBoneIndex { get; private set; }

    public sbyte RiderSitBoneIndex { get; private set; }

    public sbyte ReinHandleBoneIndex { get; private set; }

    public sbyte ReinCollision1BoneIndex { get; private set; }

    public sbyte ReinCollision2BoneIndex { get; private set; }

    public sbyte HeadLookDirectionBoneIndex { get; private set; }

    public sbyte ThoraxLookDirectionBoneIndex { get; private set; }

    public sbyte MainHandNumBonesForIk { get; private set; }

    public sbyte OffHandNumBonesForIk { get; private set; }

    public Vec3 ReinHandleLeftLocalPosition { get; private set; }

    public Vec3 ReinHandleRightLocalPosition { get; private set; }

    public IMonsterMissionData MonsterMissionData => this._monsterMissionData ?? (this._monsterMissionData = Game.Current.MonsterMissionDataCreator.CreateMonsterMissionData(this));

    public override void Deserialize(MBObjectManager objectManager, XmlNode node)
    {
      base.Deserialize(objectManager, node);
      XmlAttribute attribute1 = node.Attributes["action_set"];
      if (attribute1 != null && !string.IsNullOrEmpty(attribute1.Value))
        this.ActionSetCode = attribute1.Value;
      XmlAttribute attribute2 = node.Attributes["monster_usage"];
      this.MonsterUsage = attribute2 == null || string.IsNullOrEmpty(attribute2.Value) ? "" : attribute2.Value;
      this.Weight = 1;
      XmlAttribute attribute3 = node.Attributes["weight"];
      int result1;
      if (attribute3 != null && !string.IsNullOrEmpty(attribute3.Value) && int.TryParse(attribute3.Value, out result1))
        this.Weight = result1;
      this.HitPoints = 1;
      XmlAttribute attribute4 = node.Attributes["hit_points"];
      int result2;
      if (attribute4 != null && !string.IsNullOrEmpty(attribute4.Value) && int.TryParse(attribute4.Value, out result2))
        this.HitPoints = result2;
      this.NumPaces = 0;
      XmlAttribute attribute5 = node.Attributes["num_paces"];
      int result3;
      if (attribute5 != null && !string.IsNullOrEmpty(attribute5.Value) && int.TryParse(attribute5.Value, out result3))
        this.NumPaces = result3;
      XmlAttribute attribute6 = node.Attributes["walking_speed_limit"];
      float result4;
      if (attribute6 != null && !string.IsNullOrEmpty(attribute6.Value) && float.TryParse(attribute6.Value, out result4))
        this.WalkingSpeedLimit = result4;
      XmlAttribute attribute7 = node.Attributes["crouch_walking_speed_limit"];
      if (attribute7 != null && !string.IsNullOrEmpty(attribute7.Value))
      {
        float result5;
        if (float.TryParse(attribute7.Value, out result5))
          this.CrouchWalkingSpeedLimit = result5;
      }
      else
        this.CrouchWalkingSpeedLimit = this.WalkingSpeedLimit;
      XmlAttribute attribute8 = node.Attributes["jump_acceleration"];
      float result6;
      if (attribute8 != null && !string.IsNullOrEmpty(attribute8.Value) && float.TryParse(attribute8.Value, out result6))
        this.JumpAcceleration = result6;
      XmlAttribute attribute9 = node.Attributes["absorbed_damage_ratio"];
      if (attribute9 != null && !string.IsNullOrEmpty(attribute9.Value))
      {
        float result5;
        if (float.TryParse(attribute9.Value, out result5))
        {
          if ((double) result5 < 0.0)
            result5 = 0.0f;
          this.AbsorbedDamageRatio = result5;
        }
      }
      else
        this.AbsorbedDamageRatio = 1f;
      XmlAttribute attribute10 = node.Attributes["sound_and_collision_info_class"];
      if (attribute10 != null && !string.IsNullOrEmpty(attribute10.Value))
        this.SoundAndCollisionInfoClassName = attribute10.Value;
      this.RiderCameraHeightAdder = 0.0f;
      XmlAttribute attribute11 = node.Attributes["rider_camera_height_adder"];
      float result7;
      if (attribute11 != null && !string.IsNullOrEmpty(attribute11.Value) && float.TryParse(attribute11.Value, out result7))
        this.RiderCameraHeightAdder = result7;
      this.RiderBodyCapsuleHeightAdder = 0.0f;
      XmlAttribute attribute12 = node.Attributes["rider_body_capsule_height_adder"];
      float result8;
      if (attribute12 != null && !string.IsNullOrEmpty(attribute12.Value) && float.TryParse(attribute12.Value, out result8))
        this.RiderBodyCapsuleHeightAdder = result8;
      this.RiderBodyCapsuleForwardAdder = 0.0f;
      XmlAttribute attribute13 = node.Attributes["rider_body_capsule_forward_adder"];
      float result9;
      if (attribute13 != null && !string.IsNullOrEmpty(attribute13.Value) && float.TryParse(attribute13.Value, out result9))
        this.RiderBodyCapsuleForwardAdder = result9;
      XmlAttribute attribute14 = node.Attributes["preliminary_collision_capsule_radius_multiplier"];
      if (attribute14 != null)
        string.IsNullOrEmpty(attribute14.Value);
      XmlAttribute attribute15 = node.Attributes["rider_preliminary_collision_capsule_height_multiplier"];
      if (attribute15 != null)
        string.IsNullOrEmpty(attribute15.Value);
      XmlAttribute attribute16 = node.Attributes["rider_preliminary_collision_capsule_height_adder"];
      if (attribute16 != null)
        string.IsNullOrEmpty(attribute16.Value);
      this.StandingEyeHeight = 0.0f;
      XmlAttribute attribute17 = node.Attributes["standing_eye_height"];
      float result10;
      if (attribute17 != null && !string.IsNullOrEmpty(attribute17.Value) && float.TryParse(attribute17.Value, out result10))
        this.StandingEyeHeight = result10;
      this.CrouchEyeHeight = 0.0f;
      XmlAttribute attribute18 = node.Attributes["crouch_eye_height"];
      float result11;
      if (attribute18 != null && !string.IsNullOrEmpty(attribute18.Value) && float.TryParse(attribute18.Value, out result11))
        this.CrouchEyeHeight = result11;
      this.MountedEyeHeight = 0.0f;
      XmlAttribute attribute19 = node.Attributes["mounted_eye_height"];
      float result12;
      if (attribute19 != null && !string.IsNullOrEmpty(attribute19.Value) && float.TryParse(attribute19.Value, out result12))
        this.MountedEyeHeight = result12;
      this.RiderEyeHeightAdder = 0.0f;
      XmlAttribute attribute20 = node.Attributes["rider_eye_height_adder"];
      float result13;
      if (attribute20 != null && !string.IsNullOrEmpty(attribute20.Value) && float.TryParse(attribute20.Value, out result13))
        this.RiderEyeHeightAdder = result13;
      this.EyeOffsetWrtHead = new Vec3(0.01f, 0.01f, 0.01f);
      XmlAttribute attribute21 = node.Attributes["eye_offset_wrt_head"];
      Vec3 v1;
      if (attribute21 != null && !string.IsNullOrEmpty(attribute21.Value) && Monster.ReadVec3(attribute21.Value, out v1))
        this.EyeOffsetWrtHead = v1;
      this.FirstPersonCameraOffsetWrtHead = new Vec3(0.01f, 0.01f, 0.01f);
      XmlAttribute attribute22 = node.Attributes["first_person_camera_offset_wrt_head"];
      Vec3 v2;
      if (attribute22 != null && !string.IsNullOrEmpty(attribute22.Value) && Monster.ReadVec3(attribute22.Value, out v2))
        this.FirstPersonCameraOffsetWrtHead = v2;
      this.ArmLength = 0.0f;
      XmlAttribute attribute23 = node.Attributes["arm_length"];
      float result14;
      if (attribute23 != null && !string.IsNullOrEmpty(attribute23.Value) && float.TryParse(attribute23.Value, out result14))
        this.ArmLength = result14;
      this.ArmWeight = 0.0f;
      XmlAttribute attribute24 = node.Attributes["arm_weight"];
      float result15;
      if (attribute24 != null && !string.IsNullOrEmpty(attribute24.Value) && float.TryParse(attribute24.Value, out result15))
        this.ArmWeight = result15;
      this.JumpSpeedLimit = 0.0f;
      XmlAttribute attribute25 = node.Attributes["jump_speed_limit"];
      float result16;
      if (attribute25 != null && !string.IsNullOrEmpty(attribute25.Value) && float.TryParse(attribute25.Value, out result16))
        this.JumpSpeedLimit = result16;
      this.RelativeSpeedLimitForCharge = float.MaxValue;
      XmlAttribute attribute26 = node.Attributes["relative_speed_limit_for_charge"];
      float result17;
      if (attribute26 != null && !string.IsNullOrEmpty(attribute26.Value) && float.TryParse(attribute26.Value, out result17))
        this.RelativeSpeedLimitForCharge = result17;
      this.FamilyType = 0;
      XmlAttribute attribute27 = node.Attributes["family_type"];
      int result18;
      if (attribute27 != null && !string.IsNullOrEmpty(attribute27.Value) && int.TryParse(attribute27.Value, out result18))
        this.FamilyType = result18;
      XmlAttribute attribute28 = node.Attributes["main_hand_item_bone"];
      this.MainHandItemBoneIndex = Monster.GetBoneIndexWithId == null || attribute28 == null ? (sbyte) -1 : Monster.GetBoneIndexWithId(this.ActionSetCode, attribute28.Value);
      XmlAttribute attribute29 = node.Attributes["off_hand_item_bone"];
      this.OffHandItemBoneIndex = Monster.GetBoneIndexWithId == null || attribute29 == null ? (sbyte) -1 : Monster.GetBoneIndexWithId(this.ActionSetCode, attribute29.Value);
      XmlAttribute attribute30 = node.Attributes["main_hand_item_secondary_bone"];
      this.MainHandItemSecondaryBoneIndex = Monster.GetBoneIndexWithId == null || attribute30 == null ? (sbyte) -1 : Monster.GetBoneIndexWithId(this.ActionSetCode, attribute30.Value);
      XmlAttribute attribute31 = node.Attributes["off_hand_item_secondary_bone"];
      this.OffHandItemSecondaryBoneIndex = Monster.GetBoneIndexWithId == null || attribute31 == null ? (sbyte) -1 : Monster.GetBoneIndexWithId(this.ActionSetCode, attribute31.Value);
      XmlAttribute attribute32 = node.Attributes["rider_sit_bone"];
      this.RiderSitBoneIndex = Monster.GetBoneIndexWithId == null || attribute32 == null ? (sbyte) -1 : Monster.GetBoneIndexWithId(this.ActionSetCode, attribute32.Value);
      XmlAttribute attribute33 = node.Attributes["rein_handle_bone"];
      this.ReinHandleBoneIndex = Monster.GetBoneIndexWithId == null || attribute33 == null ? (sbyte) -1 : Monster.GetBoneIndexWithId(this.ActionSetCode, attribute33.Value);
      XmlAttribute attribute34 = node.Attributes["rein_collision_1_bone"];
      this.ReinCollision1BoneIndex = Monster.GetBoneIndexWithId == null || attribute34 == null ? (sbyte) -1 : Monster.GetBoneIndexWithId(this.ActionSetCode, attribute34.Value);
      XmlAttribute attribute35 = node.Attributes["rein_collision_2_bone"];
      this.ReinCollision2BoneIndex = Monster.GetBoneIndexWithId == null || attribute35 == null ? (sbyte) -1 : Monster.GetBoneIndexWithId(this.ActionSetCode, attribute35.Value);
      XmlAttribute attribute36 = node.Attributes["head_look_direction_bone"];
      this.HeadLookDirectionBoneIndex = Monster.GetBoneIndexWithId == null || attribute36 == null ? (sbyte) -1 : Monster.GetBoneIndexWithId(this.ActionSetCode, attribute36.Value);
      XmlAttribute attribute37 = node.Attributes["thorax_look_direction_bone"];
      this.ThoraxLookDirectionBoneIndex = Monster.GetBoneIndexWithId == null || attribute37 == null ? (sbyte) -1 : Monster.GetBoneIndexWithId(this.ActionSetCode, attribute37.Value);
      XmlNode attribute38 = (XmlNode) node.Attributes["rein_handle_left_local_pos"];
      if (attribute38 != null)
      {
        string[] strArray = attribute38.Value.Split(',');
        if (strArray.Length == 3)
        {
          Vec3 zero = Vec3.Zero;
          float.TryParse(strArray[0], out zero.x);
          float.TryParse(strArray[1], out zero.y);
          float.TryParse(strArray[2], out zero.z);
          this.ReinHandleLeftLocalPosition = zero;
        }
      }
      XmlNode attribute39 = (XmlNode) node.Attributes["rein_handle_right_local_pos"];
      if (attribute39 != null)
      {
        string[] strArray = attribute39.Value.Split(',');
        if (strArray.Length == 3)
        {
          Vec3 zero = Vec3.Zero;
          float.TryParse(strArray[0], out zero.x);
          float.TryParse(strArray[1], out zero.y);
          float.TryParse(strArray[2], out zero.z);
          this.ReinHandleRightLocalPosition = zero;
        }
      }
      XmlAttribute attribute40 = node.Attributes["main_hand_num_bones_for_ik"];
      this.MainHandNumBonesForIk = attribute40 != null ? sbyte.Parse(attribute40.Value) : (sbyte) 0;
      XmlAttribute attribute41 = node.Attributes["off_hand_num_bones_for_ik"];
      this.OffHandNumBonesForIk = attribute41 != null ? sbyte.Parse(attribute41.Value) : (sbyte) 0;
      this.Flags = AgentFlag.None;
      foreach (XmlNode childNode1 in node.ChildNodes)
      {
        if (childNode1.Name == "Flags")
        {
          foreach (AgentFlag agentFlag in Enum.GetValues(typeof (AgentFlag)))
          {
            XmlAttribute attribute42 = childNode1.Attributes[agentFlag.ToString()];
            if (attribute42 != null && !attribute42.Value.Equals("false", StringComparison.InvariantCultureIgnoreCase))
              this.Flags |= agentFlag;
          }
        }
        else if (childNode1.Name == "Capsules")
        {
          foreach (XmlNode childNode2 in childNode1.ChildNodes)
          {
            if (childNode2.Attributes != null && (childNode2.Name == "preliminary_collision_capsule" || childNode2.Name == "body_capsule" || childNode2.Name == "crouched_body_capsule"))
            {
              bool flag = true;
              Vec3 vec3_1 = new Vec3(z: 0.01f);
              Vec3 vec3_2 = Vec3.Zero;
              float result5 = 0.01f;
              if (childNode2.Attributes["pos1"] != null)
              {
                Vec3 v3;
                flag = Monster.ReadVec3(childNode2.Attributes["pos1"].Value, out v3) & flag;
                if (flag)
                  vec3_1 = v3;
              }
              if (childNode2.Attributes["pos2"] != null)
              {
                Vec3 v3;
                flag = Monster.ReadVec3(childNode2.Attributes["pos2"].Value, out v3) & flag;
                if (flag)
                  vec3_2 = v3;
              }
              if (childNode2.Attributes["radius"] != null)
              {
                string s = childNode2.Attributes["radius"].Value.Trim();
                flag = flag && float.TryParse(s, out result5);
              }
              if (flag && !childNode2.Name.StartsWith("p"))
              {
                if (childNode2.Name.StartsWith("c"))
                {
                  this.CrouchedBodyCapsuleRadius = result5;
                  this.CrouchedBodyCapsulePoint1 = vec3_1;
                  this.CrouchedBodyCapsulePoint2 = vec3_2;
                }
                else
                {
                  this.BodyCapsuleRadius = result5;
                  this.BodyCapsulePoint1 = vec3_1;
                  this.BodyCapsulePoint2 = vec3_2;
                }
              }
            }
          }
        }
      }
    }

    private static bool ReadVec3(string str, out Vec3 v)
    {
      str = str.Trim();
      string[] strArray = str.Split(",".ToCharArray());
      v = new Vec3();
      return float.TryParse(strArray[0], out v.x) && float.TryParse(strArray[1], out v.y) && float.TryParse(strArray[2], out v.z);
    }

    public sbyte GetBoneToAttachForItemFlags(ItemFlags itemFlags)
    {
      switch (itemFlags & ItemFlags.AttachmentMask)
      {
        case ItemFlags.ForceAttachOffHandPrimaryItemBone:
          return this.OffHandItemBoneIndex;
        case ItemFlags.ForceAttachOffHandSecondaryItemBone:
          return this.OffHandItemSecondaryBoneIndex;
        default:
          return this.MainHandItemBoneIndex;
      }
    }
  }
}
