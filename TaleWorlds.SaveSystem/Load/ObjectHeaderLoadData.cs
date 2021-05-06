// Decompiled with JetBrains decompiler
// Type: TaleWorlds.SaveSystem.Load.ObjectHeaderLoadData
// Assembly: TaleWorlds.SaveSystem, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 786EF53D-D2EC-43A3-8B8C-4717B7406D76
// Assembly location: E:\SteamLibrary\steamapps\common\Mount & Blade II Bannerlord\bin\Win64_Shipping_Client\TaleWorlds.SaveSystem.dll

using System.Runtime.Serialization;
using TaleWorlds.Library;
using TaleWorlds.SaveSystem.Definition;

namespace TaleWorlds.SaveSystem.Load
{
  public class ObjectHeaderLoadData
  {
    private SaveId _saveId;

    public int Id { get; private set; }

    public object LoadedObject { get; private set; }

    public object Target { get; private set; }

    public int PropertyCount { get; private set; }

    public int ChildStructCount { get; private set; }

    public TypeDefinition TypeDefinition { get; private set; }

    public LoadContext Context { get; private set; }

    public ObjectHeaderLoadData(LoadContext context, int id)
    {
      this.Context = context;
      this.Id = id;
    }

    public void InitialieReaders(SaveEntryFolder saveEntryFolder)
    {
      BinaryReader binaryReader = saveEntryFolder.GetEntry(new EntryId(-1, SaveEntryExtension.Basics)).GetBinaryReader();
      this._saveId = SaveId.ReadSaveIdFrom((IReader) binaryReader);
      this.PropertyCount = (int) binaryReader.ReadShort();
      this.ChildStructCount = (int) binaryReader.ReadShort();
    }

    public void CreateObject()
    {
      this.TypeDefinition = this.Context.DefinitionContext.TryGetTypeDefinition(this._saveId) as TypeDefinition;
      if (this.TypeDefinition == null)
        return;
      this.LoadedObject = FormatterServices.GetUninitializedObject(this.TypeDefinition.Type);
      this.Target = this.LoadedObject;
    }

    public void AdvancedResolveObject(MetaData metaData, ObjectLoadData objectLoadData) => this.Target = this.TypeDefinition.ObjectResolver.AdvancedResolveObject(this.LoadedObject, metaData, objectLoadData);

    public void ResolveObject() => this.Target = this.TypeDefinition.ObjectResolver.ResolveObject(this.LoadedObject);
  }
}
