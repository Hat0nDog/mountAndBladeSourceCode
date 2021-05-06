// Decompiled with JetBrains decompiler
// Type: TaleWorlds.SaveSystem.Save.MemberSaveData
// Assembly: TaleWorlds.SaveSystem, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 786EF53D-D2EC-43A3-8B8C-4717B7406D76
// Assembly location: E:\SteamLibrary\steamapps\common\Mount & Blade II Bannerlord\bin\Win64_Shipping_Client\TaleWorlds.SaveSystem.dll

using TaleWorlds.SaveSystem.Definition;

namespace TaleWorlds.SaveSystem.Save
{
  internal abstract class MemberSaveData : VariableSaveData
  {
    public ObjectSaveData ObjectSaveData { get; private set; }

    protected MemberSaveData(ObjectSaveData objectSaveData)
      : base(objectSaveData.Context)
    {
      this.ObjectSaveData = objectSaveData;
    }

    public abstract void Initialize(TypeDefinitionBase typeDefinition);

    public abstract void InitializeAsCustomStruct(int structId);
  }
}
