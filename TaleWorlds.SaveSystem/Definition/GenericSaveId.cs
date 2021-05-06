// Decompiled with JetBrains decompiler
// Type: TaleWorlds.SaveSystem.Definition.GenericSaveId
// Assembly: TaleWorlds.SaveSystem, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 786EF53D-D2EC-43A3-8B8C-4717B7406D76
// Assembly location: E:\SteamLibrary\steamapps\common\Mount & Blade II Bannerlord\bin\Win64_Shipping_Client\TaleWorlds.SaveSystem.dll

using System.Collections.Generic;
using TaleWorlds.Library;

namespace TaleWorlds.SaveSystem.Definition
{
  internal class GenericSaveId : SaveId
  {
    private readonly string _stringId;

    public SaveId BaseId { get; set; }

    public SaveId[] GenericTypeIDs { get; set; }

    public GenericSaveId(TypeSaveId baseId, SaveId[] saveIds)
    {
      this.BaseId = (SaveId) baseId;
      this.GenericTypeIDs = saveIds;
      this._stringId = this.CalculateStringId();
    }

    private string CalculateStringId()
    {
      string str = "";
      for (int index = 0; index < this.GenericTypeIDs.Length; ++index)
      {
        if (index != 0)
          str += ",";
        SaveId genericTypeId = this.GenericTypeIDs[index];
        str += genericTypeId.GetStringId();
      }
      return "G(" + this.BaseId.GetStringId() + ")-(" + str + ")";
    }

    public override string GetStringId() => this._stringId;

    public override void WriteTo(IWriter writer)
    {
      writer.WriteByte((byte) 1);
      this.BaseId.WriteTo(writer);
      writer.WriteByte((byte) this.GenericTypeIDs.Length);
      for (int index = 0; index < this.GenericTypeIDs.Length; ++index)
        this.GenericTypeIDs[index].WriteTo(writer);
    }

    public static GenericSaveId ReadFrom(IReader reader)
    {
      int num1 = (int) reader.ReadByte();
      TypeSaveId baseId = TypeSaveId.ReadFrom(reader);
      byte num2 = reader.ReadByte();
      List<SaveId> saveIdList = new List<SaveId>();
      for (int index = 0; index < (int) num2; ++index)
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
      return new GenericSaveId(baseId, saveIdList.ToArray());
    }
  }
}
