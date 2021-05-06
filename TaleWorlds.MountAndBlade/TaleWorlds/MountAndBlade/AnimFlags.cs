// Decompiled with JetBrains decompiler
// Type: TaleWorlds.MountAndBlade.AnimFlags
// Assembly: TaleWorlds.MountAndBlade, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3C0E3F42-756F-4407-B817-06BA412A78B0
// Assembly location: E:\SteamLibrary\steamapps\common\Mount & Blade II Bannerlord\bin\Win64_Shipping_Client\TaleWorlds.MountAndBlade.dll

using System;
using TaleWorlds.DotNet;

namespace TaleWorlds.MountAndBlade
{
  [EngineStruct("Anim_flags")]
  [Flags]
  public enum AnimFlags : ulong
  {
    amf_priority_continue = 1,
    amf_priority_jump = 2,
    amf_priority_ride = amf_priority_jump, // 0x0000000000000002
    amf_priority_crouch = amf_priority_ride, // 0x0000000000000002
    amf_priority_attack = 10, // 0x000000000000000A
    amf_priority_cancel = 12, // 0x000000000000000C
    amf_priority_defend = amf_priority_cancel | amf_priority_crouch, // 0x000000000000000E
    amf_priority_defend_parry = amf_priority_defend | amf_priority_continue, // 0x000000000000000F
    amf_priority_throw = amf_priority_defend_parry, // 0x000000000000000F
    amf_priority_blocked = amf_priority_throw, // 0x000000000000000F
    amf_priority_parried = amf_priority_blocked, // 0x000000000000000F
    amf_priority_kick = 33, // 0x0000000000000021
    amf_priority_jump_end = amf_priority_kick, // 0x0000000000000021
    amf_priority_reload = 60, // 0x000000000000003C
    amf_priority_mount = 64, // 0x0000000000000040
    amf_priority_equip = 70, // 0x0000000000000046
    amf_priority_rear = amf_priority_mount | amf_priority_attack, // 0x000000000000004A
    amf_priority_upperbody_while_kick = amf_priority_rear | amf_priority_continue, // 0x000000000000004B
    amf_priority_striked = 80, // 0x0000000000000050
    amf_priority_fall_from_horse = amf_priority_striked | amf_priority_continue, // 0x0000000000000051
    amf_priority_die = amf_priority_fall_from_horse | amf_priority_defend, // 0x000000000000005F
    amf_priority_mask = 255, // 0x00000000000000FF
    anf_disable_agent_agent_collisions = 256, // 0x0000000000000100
    anf_ignore_all_collisions = 512, // 0x0000000000000200
    anf_ignore_static_body_collisions = 1024, // 0x0000000000000400
    anf_use_last_step_point_as_data = 2048, // 0x0000000000000800
    anf_make_bodyfall_sound = 4096, // 0x0000000000001000
    anf_client_prediction = 8192, // 0x0000000000002000
    anf_keep = 16384, // 0x0000000000004000
    anf_restart = 32768, // 0x0000000000008000
    anf_client_owner_prediction = 65536, // 0x0000000000010000
    anf_make_walk_sound = 131072, // 0x0000000000020000
    anf_disable_hand_ik = 262144, // 0x0000000000040000
    anf_stick_item_to_left_hand = 524288, // 0x0000000000080000
    anf_blends_according_to_look_slope = 1048576, // 0x0000000000100000
    anf_synch_with_horse = 2097152, // 0x0000000000200000
    anf_use_left_hand_during_attack = 4194304, // 0x0000000000400000
    anf_lock_camera = 8388608, // 0x0000000000800000
    anf_lock_movement = 16777216, // 0x0000000001000000
    anf_synch_with_movement = 33554432, // 0x0000000002000000
    anf_enable_hand_spring_ik = 67108864, // 0x0000000004000000
    anf_enable_hand_blend_ik = 134217728, // 0x0000000008000000
    anf_synch_with_ladder_movement = 268435456, // 0x0000000010000000
    anf_do_not_keep_track_of_sound = 536870912, // 0x0000000020000000
    anf_reset_camera_height = 1073741824, // 0x0000000040000000
    anf_disable_alternative_randomization = 2147483648, // 0x0000000080000000
    anf_enforce_lowerbody = 68719476736, // 0x0000001000000000
    anf_enforce_all = 137438953472, // 0x0000002000000000
    anf_cyclic = 274877906944, // 0x0000004000000000
    anf_enforce_root_rotation = 549755813888, // 0x0000008000000000
    anf_allow_head_movement = 1099511627776, // 0x0000010000000000
    anf_disable_foot_ik = 2199023255552, // 0x0000020000000000
    anf_affected_by_movement = 4398046511104, // 0x0000040000000000
    anf_update_bounding_volume = 8796093022208, // 0x0000080000000000
    anf_align_with_ground = 17592186044416, // 0x0000100000000000
    anf_ignore_slope = 35184372088832, // 0x0000200000000000
    anf_displace_position = 70368744177664, // 0x0000400000000000
    anf_enable_left_hand_ik = 140737488355328, // 0x0000800000000000
    anf_ignore_scale_on_root_position = 281474976710656, // 0x0001000000000000
    anf_animation_layer_flags_mask = 4503530907893760, // 0x000FFFF000000000
    anf_animation_layer_flags_bits = 36, // 0x0000000000000024
    anf_randomization_weight_1 = 1152921504606846976, // 0x1000000000000000
    anf_randomization_weight_2 = 2305843009213693952, // 0x2000000000000000
    anf_randomization_weight_4 = 4611686018427387904, // 0x4000000000000000
    anf_randomization_weight_8 = 9223372036854775808, // 0x8000000000000000
    anf_randomization_weight_mask = anf_randomization_weight_8 | anf_randomization_weight_4 | anf_randomization_weight_2 | anf_randomization_weight_1, // 0xF000000000000000
  }
}
