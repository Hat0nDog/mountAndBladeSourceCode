// Decompiled with JetBrains decompiler
// Type: TaleWorlds.SaveSystem.Load.ContainerHeaderLoadData
// Assembly: TaleWorlds.SaveSystem, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 786EF53D-D2EC-43A3-8B8C-4717B7406D76
// Assembly location: E:\SteamLibrary\steamapps\common\Mount & Blade II Bannerlord\bin\Win64_Shipping_Client\TaleWorlds.SaveSystem.dll

using System;
using TaleWorlds.Library;
using TaleWorlds.SaveSystem.Definition;

namespace TaleWorlds.SaveSystem.Load
{
  public class ContainerHeaderLoadData
  {
    public int Id { get; private set; }

    public object Target { get; private set; }

    public LoadContext Context { get; private set; }

    public ContainerDefinition TypeDefinition { get; private set; }

    public SaveId SaveId { get; private set; }

    public int ElementCount { get; private set; }

    public ContainerType ContainerType { get; private set; }

    public ContainerHeaderLoadData(LoadContext context, int id)
    {
      this.Context = context;
      this.Id = id;
    }

    public bool GetObjectTypeDefinition()
    {
      this.TypeDefinition = this.Context.DefinitionContext.TryGetTypeDefinition(this.SaveId) as ContainerDefinition;
      return this.TypeDefinition != null;
    }

    public void CreateObject()
    {
      Type type = this.TypeDefinition.Type;
      if (this.ContainerType == ContainerType.Array)
        this.Target = Activator.CreateInstance(type, (object) this.ElementCount);
      else
        this.Target = Activator.CreateInstance(type);
    }

    public void InitialieReaders(SaveEntryFolder saveEntryFolder)
    {
      BinaryReader binaryReader = saveEntryFolder.GetEntry(new EntryId(-1, SaveEntryExtension.Object)).GetBinaryReader();
      this.SaveId = SaveId.ReadSaveIdFrom((IReader) binaryReader);
      this.ContainerType = (ContainerType) binaryReader.ReadByte();
      this.ElementCount = binaryReader.ReadInt();
    }
  }
}
