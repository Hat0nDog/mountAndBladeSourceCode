// Decompiled with JetBrains decompiler
// Type: TaleWorlds.MountAndBlade.WeaponComponentMissionExtensions
// Assembly: TaleWorlds.MountAndBlade, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3C0E3F42-756F-4407-B817-06BA412A78B0
// Assembly location: E:\SteamLibrary\steamapps\common\Mount & Blade II Bannerlord\bin\Win64_Shipping_Client\TaleWorlds.MountAndBlade.dll

using System;
using TaleWorlds.Core;
using TaleWorlds.DotNet;
using TaleWorlds.Engine;
using TaleWorlds.Library;

namespace TaleWorlds.MountAndBlade
{
  public static class WeaponComponentMissionExtensions
  {
    public static int GetItemUsageIndex(this WeaponComponentData weaponComponentData) => MBItem.GetItemUsageIndex(weaponComponentData.ItemUsage);

    public static Vec3 GetWeaponCenterOfMass(this PhysicsShape body) => WeaponComponentMissionExtensions.CalculateCenterOfMass(body);

    [MBCallback]
    internal static Vec3 CalculateCenterOfMass(PhysicsShape body)
    {
      if ((NativeObject) body == (NativeObject) null)
        return Vec3.Zero;
      Vec3 zero = Vec3.Zero;
      float num1 = 0.0f;
      int num2 = body.CapsuleCount();
      for (int index = 0; index < num2; ++index)
      {
        CapsuleData data = new CapsuleData();
        body.GetCapsule(ref data, index);
        Vec3 vec3 = (data.P1 + data.P2) * 0.5f;
        float num3 = data.P1.Distance(data.P2);
        float num4 = (float) ((double) data.Radius * (double) data.Radius * 3.14159274101257 * (1.33333337306976 * (double) data.Radius + (double) num3));
        num1 += num4;
        zero += vec3 * num4;
      }
      int num5 = body.SphereCount();
      for (int index = 0; index < num5; ++index)
      {
        SphereData data = new SphereData();
        body.GetSphere(ref data, index);
        float num3 = 4.18879f * data.Radius * data.Radius * data.Radius;
        num1 += num3;
        zero += data.Origin * num3;
      }
      Vec3 vec3_1;
      if ((double) num1 > 0.0)
      {
        vec3_1 = zero / num1;
        if ((double) Math.Abs(vec3_1.x) < 0.00999999977648258)
          vec3_1.x = 0.0f;
        if ((double) Math.Abs(vec3_1.y) < 0.00999999977648258)
          vec3_1.y = 0.0f;
        if ((double) Math.Abs(vec3_1.z) < 0.00999999977648258)
          vec3_1.z = 0.0f;
      }
      else
        vec3_1 = Vec3.Zero;
      return vec3_1;
    }
  }
}
