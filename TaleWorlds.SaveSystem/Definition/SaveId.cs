// Decompiled with JetBrains decompiler
// Type: TaleWorlds.SaveSystem.Definition.SaveId
// Assembly: TaleWorlds.SaveSystem, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 786EF53D-D2EC-43A3-8B8C-4717B7406D76
// Assembly location: E:\SteamLibrary\steamapps\common\Mount & Blade II Bannerlord\bin\Win64_Shipping_Client\TaleWorlds.SaveSystem.dll

using TaleWorlds.Library;

namespace TaleWorlds.SaveSystem.Definition
{
  public abstract class SaveId
  {
    public abstract string GetStringId();

    public override int GetHashCode() => this.GetStringId().GetHashCode();

    public override bool Equals(object obj) => obj != null && !(obj.GetType() != this.GetType()) && this.GetStringId() == ((SaveId) obj).GetStringId();

    public abstract void WriteTo(IWriter writer);

    public static SaveId ReadSaveIdFrom(IReader reader)
    {
      byte num = reader.ReadByte();
      SaveId saveId = (SaveId) null;
      switch (num)
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
      return saveId;
    }
  }
}
