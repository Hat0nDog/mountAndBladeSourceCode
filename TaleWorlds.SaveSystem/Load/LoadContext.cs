// Decompiled with JetBrains decompiler
// Type: TaleWorlds.SaveSystem.Load.LoadContext
// Assembly: TaleWorlds.SaveSystem, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 786EF53D-D2EC-43A3-8B8C-4717B7406D76
// Assembly location: E:\SteamLibrary\steamapps\common\Mount & Blade II Bannerlord\bin\Win64_Shipping_Client\TaleWorlds.SaveSystem.dll

using System;
using System.Globalization;
using TaleWorlds.Library;
using TaleWorlds.SaveSystem.Definition;

namespace TaleWorlds.SaveSystem.Load
{
  public class LoadContext
  {
    private int _objectCount;
    private int _stringCount;
    private int _containerCount;
    private ObjectHeaderLoadData[] _objectHeaderLoadDatas;
    private ContainerHeaderLoadData[] _containerHeaderLoadDatas;
    private string[] _strings;

    public object RootObject { get; private set; }

    public DefinitionContext DefinitionContext { get; private set; }

    public ISaveDriver Driver { get; private set; }

    public LoadContext(DefinitionContext definitionContext, ISaveDriver driver)
    {
      this.DefinitionContext = definitionContext;
      this._objectHeaderLoadDatas = (ObjectHeaderLoadData[]) null;
      this._containerHeaderLoadDatas = (ContainerHeaderLoadData[]) null;
      this._strings = (string[]) null;
      this.Driver = driver;
    }

    internal static ObjectLoadData CreateLoadData(
      LoadData loadData,
      int i,
      ObjectHeaderLoadData header)
    {
      ArchiveDeserializer archiveDeserializer = new ArchiveDeserializer();
      archiveDeserializer.LoadFrom(loadData.GameData.ObjectData[i]);
      SaveEntryFolder rootFolder = archiveDeserializer.RootFolder;
      ObjectLoadData objectLoadData = new ObjectLoadData(header);
      FolderId folderId = new FolderId(i, SaveFolderExtension.Object);
      SaveEntryFolder childFolder = rootFolder.GetChildFolder(folderId);
      objectLoadData.InitializeReaders(childFolder);
      objectLoadData.FillCreatedObject();
      objectLoadData.Read();
      objectLoadData.FillObject();
      return objectLoadData;
    }

    public bool Load(LoadData loadData, bool loadAsLateInitialize)
    {
      try
      {
        using (new PerformanceTestBlock("LoadContext::Load Headers"))
        {
          using (new PerformanceTestBlock("LoadContext::Load And Create Header"))
          {
            ArchiveDeserializer archiveDeserializer = new ArchiveDeserializer();
            archiveDeserializer.LoadFrom(loadData.GameData.Header);
            SaveEntryFolder headerRootFolder = archiveDeserializer.RootFolder;
            BinaryReader binaryReader = headerRootFolder.GetEntry(new EntryId(-1, SaveEntryExtension.Config)).GetBinaryReader();
            this._objectCount = binaryReader.ReadInt();
            this._stringCount = binaryReader.ReadInt();
            this._containerCount = binaryReader.ReadInt();
            this._objectHeaderLoadDatas = new ObjectHeaderLoadData[this._objectCount];
            this._containerHeaderLoadDatas = new ContainerHeaderLoadData[this._containerCount];
            this._strings = new string[this._stringCount];
            TWParallel.For(0, this._objectCount, (Action<int>) (i =>
            {
              ObjectHeaderLoadData objectHeaderLoadData = new ObjectHeaderLoadData(this, i);
              SaveEntryFolder childFolder = headerRootFolder.GetChildFolder(new FolderId(i, SaveFolderExtension.Object));
              objectHeaderLoadData.InitialieReaders(childFolder);
              this._objectHeaderLoadDatas[i] = objectHeaderLoadData;
            }));
            TWParallel.For(0, this._containerCount, (Action<int>) (i =>
            {
              ContainerHeaderLoadData containerHeaderLoadData = new ContainerHeaderLoadData(this, i);
              SaveEntryFolder childFolder = headerRootFolder.GetChildFolder(new FolderId(i, SaveFolderExtension.Container));
              containerHeaderLoadData.InitialieReaders(childFolder);
              this._containerHeaderLoadDatas[i] = containerHeaderLoadData;
            }));
          }
          using (new PerformanceTestBlock("LoadContext::Create Objects"))
          {
            foreach (ObjectHeaderLoadData objectHeaderLoadData in this._objectHeaderLoadDatas)
            {
              objectHeaderLoadData.CreateObject();
              if (objectHeaderLoadData.Id == 0)
                this.RootObject = objectHeaderLoadData.Target;
            }
            foreach (ContainerHeaderLoadData containerHeaderLoadData in this._containerHeaderLoadDatas)
            {
              if (containerHeaderLoadData.GetObjectTypeDefinition())
                containerHeaderLoadData.CreateObject();
            }
          }
        }
        GC.Collect();
        GC.WaitForPendingFinalizers();
        using (new PerformanceTestBlock("LoadContext::Load Strings"))
        {
          ArchiveDeserializer saveArchive = new ArchiveDeserializer();
          saveArchive.LoadFrom(loadData.GameData.Strings);
          for (int id = 0; id < this._stringCount; ++id)
          {
            string str = LoadContext.LoadString(saveArchive, id);
            this._strings[id] = str;
          }
        }
        GC.Collect();
        GC.WaitForPendingFinalizers();
        using (new PerformanceTestBlock("LoadContext::Resolve Objects"))
        {
          for (int i = 0; i < this._objectHeaderLoadDatas.Length; ++i)
          {
            ObjectHeaderLoadData objectHeaderLoadData = this._objectHeaderLoadDatas[i];
            TypeDefinition typeDefinition = objectHeaderLoadData.TypeDefinition;
            if (typeDefinition != null)
            {
              object loadedObject = objectHeaderLoadData.LoadedObject;
              if (typeDefinition.ObjectResolver.CheckIfRequiresAdvancedResolving(loadedObject))
              {
                ObjectLoadData loadData1 = LoadContext.CreateLoadData(loadData, i, objectHeaderLoadData);
                objectHeaderLoadData.AdvancedResolveObject(loadData.MetaData, loadData1);
              }
              else
                objectHeaderLoadData.ResolveObject();
            }
          }
        }
        GC.Collect();
        GC.WaitForPendingFinalizers();
        using (new PerformanceTestBlock("LoadContext::Load Object Datas"))
          TWParallel.For(0, this._objectCount, (Action<int>) (i =>
          {
            ObjectHeaderLoadData objectHeaderLoadData = this._objectHeaderLoadDatas[i];
            if (objectHeaderLoadData.Target != objectHeaderLoadData.LoadedObject)
              return;
            LoadContext.CreateLoadData(loadData, i, objectHeaderLoadData);
          }));
        using (new PerformanceTestBlock("LoadContext::Load Container Datas"))
          TWParallel.For(0, this._containerCount, (Action<int>) (i =>
          {
            byte[] binaryArchive = loadData.GameData.ContainerData[i];
            ArchiveDeserializer archiveDeserializer = new ArchiveDeserializer();
            archiveDeserializer.LoadFrom(binaryArchive);
            SaveEntryFolder rootFolder = archiveDeserializer.RootFolder;
            ContainerLoadData containerLoadData = new ContainerLoadData(this._containerHeaderLoadDatas[i]);
            containerLoadData.InitializeReaders(rootFolder.GetChildFolder(new FolderId(i, SaveFolderExtension.Container)));
            containerLoadData.FillCreatedObject();
            containerLoadData.Read();
            containerLoadData.FillObject();
          }));
        GC.Collect();
        GC.WaitForPendingFinalizers();
        if (!loadAsLateInitialize)
          this.CreateLoadCallbackInitializator(loadData).InitializeObjects();
        return true;
      }
      catch
      {
        return false;
      }
    }

    internal LoadCallbackInitializator CreateLoadCallbackInitializator(
      LoadData loadData)
    {
      return new LoadCallbackInitializator(loadData, this._objectHeaderLoadDatas, this._objectCount);
    }

    private static string LoadString(ArchiveDeserializer saveArchive, int id) => saveArchive.RootFolder.GetChildFolder(new FolderId(-1, SaveFolderExtension.Strings)).GetEntry(new EntryId(id, SaveEntryExtension.Txt)).GetBinaryReader().ReadString();

    public static bool TryConvertType(Type sourceType, Type targetType, ref object data)
    {
      Func<Type, bool> isInt = (Func<Type, bool>) (type => type == typeof (long) || type == typeof (int) || (type == typeof (short) || type == typeof (ulong)) || type == typeof (uint) || type == typeof (ushort));
      Func<Type, bool> isFloat = (Func<Type, bool>) (type => type == typeof (double) || type == typeof (float));
      Func<Type, bool> func = (Func<Type, bool>) (type => isInt(type) || isFloat(type));
      if (func(sourceType))
      {
        if (func(targetType))
        {
          try
          {
            data = Convert.ChangeType(data, targetType);
            return true;
          }
          catch
          {
            return false;
          }
        }
      }
      if (!func(sourceType) || !(targetType == typeof (string)))
        return false;
      if (isInt(sourceType))
      {
        long int64 = Convert.ToInt64(data);
        data = (object) int64.ToString();
      }
      else if (isFloat(sourceType))
      {
        double num = Convert.ToDouble(data);
        data = (object) num.ToString((IFormatProvider) CultureInfo.InvariantCulture);
      }
      return true;
    }

    public ObjectHeaderLoadData GetObjectWithId(int id)
    {
      ObjectHeaderLoadData objectHeaderLoadData = (ObjectHeaderLoadData) null;
      if (id != -1)
        objectHeaderLoadData = this._objectHeaderLoadDatas[id];
      return objectHeaderLoadData;
    }

    public ContainerHeaderLoadData GetContainerWithId(int id)
    {
      ContainerHeaderLoadData containerHeaderLoadData = (ContainerHeaderLoadData) null;
      if (id != -1)
        containerHeaderLoadData = this._containerHeaderLoadDatas[id];
      return containerHeaderLoadData;
    }

    public string GetStringWithId(int id)
    {
      string str = (string) null;
      if (id != -1)
        str = this._strings[id];
      return str;
    }
  }
}
