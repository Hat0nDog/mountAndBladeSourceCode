// Decompiled with JetBrains decompiler
// Type: TaleWorlds.SaveSystem.Definition.ContainerSaveId
// Assembly: TaleWorlds.SaveSystem, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 786EF53D-D2EC-43A3-8B8C-4717B7406D76
// Assembly location: E:\SteamLibrary\steamapps\common\Mount & Blade II Bannerlord\bin\Win64_Shipping_Client\TaleWorlds.SaveSystem.dll

using System.Collections.Generic;
using TaleWorlds.Library;

namespace TaleWorlds.SaveSystem.Definition
{
  public class ContainerSaveId : SaveId
  {
    private readonly string _stringId;

    public ContainerType ContainerType { get; set; }

    public SaveId KeyId { get; set; }

    public SaveId ValueId { get; set; }

    public ContainerSaveId(ContainerType containerType, SaveId elementId)
    {
      this.ContainerType = containerType;
      this.KeyId = elementId;
      this._stringId = this.CalculateStringId();
    }

    public ContainerSaveId(ContainerType containerType, SaveId keyId, SaveId valueId)
    {
      this.ContainerType = containerType;
      this.KeyId = keyId;
      this.ValueId = valueId;
      this._stringId = this.CalculateStringId();
    }

    private string CalculateStringId()
    {
      string str;
      if (this.ContainerType == ContainerType.Dictionary)
        str = "C(" + (object) (int) this.ContainerType + ")-(" + this.KeyId.GetStringId() + "," + this.ValueId.GetStringId() + ")";
      else
        str = "C(" + (object) (int) this.ContainerType + ")-(" + this.KeyId.GetStringId() + ")";
      return str;
    }

    public override string GetStringId() => this._stringId;

    public override void WriteTo(IWriter writer)
    {
      writer.WriteByte((byte) 2);
      writer.WriteByte((byte) this.ContainerType);
      this.KeyId.WriteTo(writer);
      if (this.ContainerType != ContainerType.Dictionary)
        return;
      this.ValueId.WriteTo(writer);
    }

    public static ContainerSaveId ReadFrom(IReader reader)
    {
      ContainerType containerType = (ContainerType) reader.ReadByte();
      int num = containerType == ContainerType.Dictionary ? 2 : 1;
      List<SaveId> saveIdList = new List<SaveId>();
      for (int index = 0; index < num; ++index)
      {
        SaveId saveId = (SaveId) null;
        switch (reader.ReadByte())
        {
          case 0:
            saveId = (SaveId) TypeSaveId.ReadFrom(reader);
            break;
          case 1:
            saveId = (SaveId) GenericSaveId.ReadFrom(reader);
            break;
          case 2:
            saveId = (SaveId) ContainerSaveId.ReadFrom(reader);
            break;
        }
        saveIdList.Add(saveId);
      }
      SaveId keyId = saveIdList[0];
      SaveId valueId = saveIdList.Count > 1 ? saveIdList[1] : (SaveId) null;
      return new ContainerSaveId(containerType, keyId, valueId);
    }
  }
}
