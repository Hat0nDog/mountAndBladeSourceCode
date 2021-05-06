// Decompiled with JetBrains decompiler
// Type: TaleWorlds.SaveSystem.Definition.TypeSaveId
// Assembly: TaleWorlds.SaveSystem, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 786EF53D-D2EC-43A3-8B8C-4717B7406D76
// Assembly location: E:\SteamLibrary\steamapps\common\Mount & Blade II Bannerlord\bin\Win64_Shipping_Client\TaleWorlds.SaveSystem.dll

using TaleWorlds.Library;

namespace TaleWorlds.SaveSystem.Definition
{
  public class TypeSaveId : SaveId
  {
    private readonly string _stringId;

    public int Id { get; private set; }

    public TypeSaveId(int id)
    {
      this.Id = id;
      this._stringId = this.Id.ToString();
    }

    public override string GetStringId() => this._stringId;

    public override void WriteTo(IWriter writer)
    {
      writer.WriteByte((byte) 0);
      writer.WriteInt(this.Id);
    }

    public static TypeSaveId ReadFrom(IReader reader) => new TypeSaveId(reader.ReadInt());
  }
}
