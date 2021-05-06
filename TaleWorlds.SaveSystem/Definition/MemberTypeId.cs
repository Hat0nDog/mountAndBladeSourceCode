// Decompiled with JetBrains decompiler
// Type: TaleWorlds.SaveSystem.Definition.MemberTypeId
// Assembly: TaleWorlds.SaveSystem, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 786EF53D-D2EC-43A3-8B8C-4717B7406D76
// Assembly location: E:\SteamLibrary\steamapps\common\Mount & Blade II Bannerlord\bin\Win64_Shipping_Client\TaleWorlds.SaveSystem.dll

namespace TaleWorlds.SaveSystem.Definition
{
  public struct MemberTypeId
  {
    public byte TypeLevel;
    public short LocalSaveId;

    public short SaveId => (short) ((int) (short) ((int) this.TypeLevel << 8) + (int) this.LocalSaveId);

    public static MemberTypeId Invalid => new MemberTypeId((byte) 0, (short) -1);

    public override string ToString() => "(" + (object) this.TypeLevel + "," + (object) this.LocalSaveId + ")";

    public MemberTypeId(byte typeLevel, short localSaveId)
    {
      this.TypeLevel = typeLevel;
      this.LocalSaveId = localSaveId;
    }
  }
}
