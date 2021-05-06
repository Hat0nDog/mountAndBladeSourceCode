// Decompiled with JetBrains decompiler
// Type: TaleWorlds.MountAndBlade.MBMissile
// Assembly: TaleWorlds.MountAndBlade, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3C0E3F42-756F-4407-B817-06BA412A78B0
// Assembly location: E:\SteamLibrary\steamapps\common\Mount & Blade II Bannerlord\bin\Win64_Shipping_Client\TaleWorlds.MountAndBlade.dll

using TaleWorlds.Library;

namespace TaleWorlds.MountAndBlade
{
  public abstract class MBMissile
  {
    internal Mission Mission { get; }

    protected MBMissile(Mission mission) => this.Mission = mission;

    public int Index { get; set; }

    public virtual void SerializeToNetwork()
    {
    }

    public virtual void DeserializeFromNetwork()
    {
    }

    public Vec3 GetPosition() => MBAPI.IMBMission.GetPositionOfMissile(this.Mission.Pointer, this.Index);

    public Vec3 GetVelocity() => MBAPI.IMBMission.GetVelocityOfMissile(this.Mission.Pointer, this.Index);

    public bool GetHasRigidBody() => MBAPI.IMBMission.GetMissileHasRigidBody(this.Mission.Pointer, this.Index);
  }
}
