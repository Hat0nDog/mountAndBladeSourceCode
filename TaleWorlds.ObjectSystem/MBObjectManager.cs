// Decompiled with JetBrains decompiler
// Type: TaleWorlds.ObjectSystem.MBObjectManager
// Assembly: TaleWorlds.ObjectSystem, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 525679E6-68C3-48B8-A030-465E146E69EE
// Assembly location: E:\SteamLibrary\steamapps\common\Mount & Blade II Bannerlord\bin\Win64_Shipping_Client\TaleWorlds.ObjectSystem.dll

using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Schema;
using System.Xml.Xsl;
using TaleWorlds.Library;
using TaleWorlds.ModuleManager;

namespace TaleWorlds.ObjectSystem
{
  public sealed class MBObjectManager
  {
    private static MBObjectManager _instance;
    internal List<MBObjectManager.IObjectTypeRecord> ObjectTypeRecords = new List<MBObjectManager.IObjectTypeRecord>();
    internal List<MBObjectManager.IObjectTypeRecord> NonSerializedObjectTypeRecords = new List<MBObjectManager.IObjectTypeRecord>();
    private List<IObjectManagerHandler> _handlers;
    private readonly Dictionary<string, int> _counters;

    public static MBObjectManager Instance
    {
      get => MBObjectManager._instance;
      set => MBObjectManager._instance = value;
    }

    private MBObjectManager() => this._counters = new Dictionary<string, int>();

    public static MBObjectManager Init()
    {
      MBObjectManager instance = MBObjectManager._instance;
      MBObjectManager._instance = new MBObjectManager();
      return MBObjectManager._instance;
    }

    public static void Init(MBObjectManager existingObjectManager) => MBObjectManager._instance = MBObjectManager._instance == null ? existingObjectManager : throw new Exception("MBObjectManager has already been initialized!");

    public void Destroy()
    {
      this.ClearAllObjects();
      MBObjectManager._instance = (MBObjectManager) null;
    }

    public bool LoadWithValidation { get; set; }

    public int GetCounter(string id, bool isUsed)
    {
      if (!this._counters.ContainsKey(id))
        this._counters.Add(id, 0);
      int counter = this._counters[id];
      if (!isUsed)
        return counter;
      this._counters[id]++;
      return counter;
    }

    public int NumRegisteredTypes => (this.ObjectTypeRecords != null ? this.ObjectTypeRecords.Count : 0) + (this.NonSerializedObjectTypeRecords != null ? this.NonSerializedObjectTypeRecords.Count : 0);

    public int MaxRegisteredTypes => 256;

    public void RegisterType<T>(
      string classPrefix,
      string classListPrefix,
      uint typeId,
      bool autoCreateInstance = true)
      where T : MBObjectBase
    {
      int numRegisteredTypes = this.NumRegisteredTypes;
      int maxRegisteredTypes = this.MaxRegisteredTypes;
      this.ObjectTypeRecords.Add((MBObjectManager.IObjectTypeRecord) new MBObjectManager.ObjectTypeRecord<T>(typeId, classPrefix, classListPrefix, autoCreateInstance));
    }

    public void RegisterNonSerializedType<T>(
      string classPrefix,
      string classListPrefix,
      uint typeId,
      bool autoCreateInstance = true)
      where T : MBObjectBase
    {
      if (this.NonSerializedObjectTypeRecords == null)
        this.NonSerializedObjectTypeRecords = new List<MBObjectManager.IObjectTypeRecord>();
      int numRegisteredTypes = this.NumRegisteredTypes;
      int maxRegisteredTypes = this.MaxRegisteredTypes;
      this.NonSerializedObjectTypeRecords.Add((MBObjectManager.IObjectTypeRecord) new MBObjectManager.ObjectTypeRecord<T>(typeId, classPrefix, classListPrefix, autoCreateInstance));
    }

    public string FindRegisteredClassPrefix(Type type)
    {
      foreach (MBObjectManager.IObjectTypeRecord objectTypeRecord in this.ObjectTypeRecords)
      {
        if (objectTypeRecord.ObjectClass == type)
          return objectTypeRecord.ElementName;
      }
      foreach (MBObjectManager.IObjectTypeRecord objectTypeRecord in this.NonSerializedObjectTypeRecords)
      {
        if (objectTypeRecord.ObjectClass == type)
          return objectTypeRecord.ElementName;
      }
      return (string) null;
    }

    public Type FindRegisteredType(string classPrefix)
    {
      foreach (MBObjectManager.IObjectTypeRecord objectTypeRecord in this.ObjectTypeRecords)
      {
        if (objectTypeRecord.ElementName == classPrefix)
          return objectTypeRecord.ObjectClass;
      }
      foreach (MBObjectManager.IObjectTypeRecord objectTypeRecord in this.NonSerializedObjectTypeRecords)
      {
        if (objectTypeRecord.ElementName == classPrefix)
          return objectTypeRecord.ObjectClass;
      }
      return (Type) null;
    }

    public T RegisterObject<T>(T obj) where T : MBObjectBase
    {
      MBObjectBase registeredObject;
      this.RegisterObjectInternalWithoutTypeId<T>(obj, false, out registeredObject);
      return registeredObject as T;
    }

    public T RegisterPresumedObject<T>(T obj) where T : MBObjectBase
    {
      MBObjectBase registeredObject;
      this.RegisterObjectInternalWithoutTypeId<T>(obj, true, out registeredObject);
      return registeredObject as T;
    }

    internal void TryRegisterObjectWithoutInitialization(MBObjectBase obj)
    {
      Type type = obj.GetType();
      foreach (MBObjectManager.IObjectTypeRecord objectTypeRecord in this.ObjectTypeRecords)
      {
        if (objectTypeRecord.ObjectClass == type)
        {
          objectTypeRecord.RegisterMBObjectWithoutInitialization(obj);
          return;
        }
      }
      foreach (MBObjectManager.IObjectTypeRecord objectTypeRecord in this.NonSerializedObjectTypeRecords)
      {
        if (objectTypeRecord.ObjectClass == type)
        {
          objectTypeRecord.RegisterMBObjectWithoutInitialization(obj);
          break;
        }
      }
    }

    private void RegisterObjectInternalWithoutTypeId<T>(
      T obj,
      bool presumed,
      out MBObjectBase registeredObject)
      where T : MBObjectBase
    {
      obj.GetType();
      Type type = typeof (T);
      foreach (MBObjectManager.IObjectTypeRecord objectTypeRecord in this.ObjectTypeRecords)
      {
        if (objectTypeRecord.ObjectClass == type)
        {
          objectTypeRecord.RegisterMBObject((MBObjectBase) obj, presumed, out registeredObject);
          return;
        }
      }
      foreach (MBObjectManager.IObjectTypeRecord objectTypeRecord in this.NonSerializedObjectTypeRecords)
      {
        if (objectTypeRecord.ObjectClass == type)
        {
          objectTypeRecord.RegisterMBObject((MBObjectBase) obj, presumed, out registeredObject);
          return;
        }
      }
      registeredObject = (MBObjectBase) null;
    }

    public void UnregisterObject(MBObjectBase obj)
    {
      if (obj == null)
        return;
      Type type = obj.GetType();
      foreach (MBObjectManager.IObjectTypeRecord objectTypeRecord in this.ObjectTypeRecords)
      {
        if (type == objectTypeRecord.ObjectClass)
        {
          objectTypeRecord.UnregisterMBObject(obj);
          this.AfterUnregisterObject(obj);
          return;
        }
      }
      foreach (MBObjectManager.IObjectTypeRecord objectTypeRecord in this.NonSerializedObjectTypeRecords)
      {
        if (type == objectTypeRecord.ObjectClass)
        {
          objectTypeRecord.UnregisterMBObject(obj);
          this.AfterUnregisterObject(obj);
          break;
        }
      }
    }

    private void AfterUnregisterObject(MBObjectBase obj)
    {
      if (this._handlers == null)
        return;
      foreach (IObjectManagerHandler handler in this._handlers)
        handler.AfterUnregisterObject(obj);
    }

    public T GetObject<T>(Func<T, bool> predicate) where T : MBObjectBase
    {
      foreach (MBObjectManager.IObjectTypeRecord objectTypeRecord in this.ObjectTypeRecords)
      {
        if (typeof (T).IsSealed)
        {
          if (objectTypeRecord.ObjectClass == typeof (T))
            return ((IEnumerable<T>) objectTypeRecord).FirstOrDefault<T>(predicate);
        }
        else if (typeof (T).IsAssignableFrom(objectTypeRecord.ObjectClass))
          return objectTypeRecord.OfType<T>().FirstOrDefault<T>(predicate);
      }
      foreach (MBObjectManager.IObjectTypeRecord objectTypeRecord in this.NonSerializedObjectTypeRecords)
      {
        if (typeof (T).IsSealed)
        {
          if (objectTypeRecord.ObjectClass == typeof (T))
            return ((IEnumerable<T>) objectTypeRecord).FirstOrDefault<T>(predicate);
        }
        else if (typeof (T).IsAssignableFrom(objectTypeRecord.ObjectClass))
          return objectTypeRecord.OfType<T>().FirstOrDefault<T>(predicate);
      }
      throw new MBTypeNotRegisteredException();
    }

    public T GetObject<T>(string objectName) where T : MBObjectBase
    {
      bool flag = false;
      foreach (MBObjectManager.IObjectTypeRecord objectTypeRecord in this.ObjectTypeRecords)
      {
        if (typeof (T).IsSealed)
        {
          if (objectTypeRecord.ObjectClass == typeof (T))
            return ((MBObjectManager.ObjectTypeRecord<T>) objectTypeRecord).GetObject(objectName);
        }
        else if (typeof (T).IsAssignableFrom(objectTypeRecord.ObjectClass))
        {
          flag = true;
          if (objectTypeRecord.GetMBObject(objectName) is T mbObject4)
            return mbObject4;
        }
      }
      foreach (MBObjectManager.IObjectTypeRecord objectTypeRecord in this.NonSerializedObjectTypeRecords)
      {
        if (typeof (T).IsSealed)
        {
          if (objectTypeRecord.ObjectClass == typeof (T))
            return ((MBObjectManager.ObjectTypeRecord<T>) objectTypeRecord).GetObject(objectName);
        }
        else if (typeof (T).IsAssignableFrom(objectTypeRecord.ObjectClass))
        {
          flag = true;
          if (objectTypeRecord.GetMBObject(objectName) is T mbObject6)
            return mbObject6;
        }
      }
      if (flag)
        return default (T);
      throw new MBTypeNotRegisteredException();
    }

    public bool ContainsObject<T>(string objectName) where T : MBObjectBase
    {
      bool flag1 = false;
      foreach (MBObjectManager.IObjectTypeRecord objectTypeRecord in this.ObjectTypeRecords)
      {
        if (typeof (T).IsSealed)
        {
          if (objectTypeRecord.ObjectClass == typeof (T))
            return objectTypeRecord.ContainsObject(objectName);
        }
        else if (typeof (T).IsAssignableFrom(objectTypeRecord.ObjectClass))
        {
          flag1 = true;
          bool flag2 = objectTypeRecord.ContainsObject(objectName);
          if (flag2)
            return flag2;
        }
      }
      foreach (MBObjectManager.IObjectTypeRecord objectTypeRecord in this.NonSerializedObjectTypeRecords)
      {
        if (typeof (T).IsSealed)
        {
          if (objectTypeRecord.ObjectClass == typeof (T))
            return objectTypeRecord.ContainsObject(objectName);
        }
        else if (typeof (T).IsAssignableFrom(objectTypeRecord.ObjectClass))
        {
          flag1 = true;
          bool flag2 = objectTypeRecord.ContainsObject(objectName);
          if (flag2)
            return flag2;
        }
      }
      if (flag1)
        return false;
      throw new MBTypeNotRegisteredException();
    }

    public void AfterLoad()
    {
      foreach (MBObjectManager.IObjectTypeRecord objectTypeRecord in this.ObjectTypeRecords)
        objectTypeRecord.PreAfterLoad();
      foreach (MBObjectManager.IObjectTypeRecord objectTypeRecord in this.NonSerializedObjectTypeRecords)
        objectTypeRecord.PreAfterLoad();
      foreach (MBObjectManager.IObjectTypeRecord objectTypeRecord in this.ObjectTypeRecords)
        objectTypeRecord.AfterLoad();
      foreach (MBObjectManager.IObjectTypeRecord objectTypeRecord in this.NonSerializedObjectTypeRecords)
        objectTypeRecord.AfterLoad();
    }

    public MBObjectBase GetObject(MBGUID objectId)
    {
      foreach (MBObjectManager.IObjectTypeRecord objectTypeRecord in this.ObjectTypeRecords)
      {
        if ((int) objectTypeRecord.TypeNo == (int) objectId.GetTypeIndex())
          return objectTypeRecord.GetMBObject(objectId);
      }
      foreach (MBObjectManager.IObjectTypeRecord objectTypeRecord in this.NonSerializedObjectTypeRecords)
      {
        if ((int) objectTypeRecord.TypeNo == (int) objectId.GetTypeIndex())
          return objectTypeRecord.GetMBObject(objectId);
      }
      throw new MBTypeNotRegisteredException();
    }

    public MBObjectBase GetObject(string typeName, string objectName)
    {
      foreach (MBObjectManager.IObjectTypeRecord objectTypeRecord in this.ObjectTypeRecords)
      {
        if (objectTypeRecord.ElementName == typeName)
          return objectTypeRecord.GetMBObject(objectName);
      }
      foreach (MBObjectManager.IObjectTypeRecord objectTypeRecord in this.NonSerializedObjectTypeRecords)
      {
        if (objectTypeRecord.ElementName == typeName)
          return objectTypeRecord.GetMBObject(objectName);
      }
      throw new MBTypeNotRegisteredException();
    }

    private MBObjectBase GetPresumedObject(
      string typeName,
      string objectName,
      bool isInitialize = false)
    {
      foreach (MBObjectManager.IObjectTypeRecord objectTypeRecord in this.ObjectTypeRecords)
      {
        if (objectTypeRecord.ElementName == typeName)
        {
          MBObjectBase mbObject = objectTypeRecord.GetMBObject(objectName);
          if (mbObject != null)
            return mbObject;
          MBObjectBase mbObjectBase = objectTypeRecord.AutoCreate ? objectTypeRecord.CreatePresumedMBObject(objectName) : throw new MBCanNotCreatePresumedObjectException();
          MBObjectBase registeredObject;
          objectTypeRecord.RegisterMBObject(mbObjectBase, true, out registeredObject);
          return registeredObject;
        }
      }
      foreach (MBObjectManager.IObjectTypeRecord objectTypeRecord in this.NonSerializedObjectTypeRecords)
      {
        if (objectTypeRecord.ElementName == typeName)
        {
          MBObjectBase mbObject = objectTypeRecord.GetMBObject(objectName);
          if (mbObject != null)
            return mbObject;
          MBObjectBase mbObjectBase = objectTypeRecord.AutoCreate ? objectTypeRecord.CreatePresumedMBObject(objectName) : throw new MBCanNotCreatePresumedObjectException();
          MBObjectBase registeredObject;
          objectTypeRecord.RegisterMBObject(mbObjectBase, true, out registeredObject);
          return registeredObject;
        }
      }
      throw new MBTypeNotRegisteredException();
    }

    public MBReadOnlyList<T> GetObjectTypeList<T>() where T : MBObjectBase
    {
      if (typeof (T).IsSealed)
      {
        foreach (MBObjectManager.IObjectTypeRecord objectTypeRecord in this.ObjectTypeRecords)
        {
          if (objectTypeRecord.ObjectClass == typeof (T))
            return ((MBObjectManager.ObjectTypeRecord<T>) objectTypeRecord).GetObjectsList();
        }
        foreach (MBObjectManager.IObjectTypeRecord objectTypeRecord in this.NonSerializedObjectTypeRecords)
        {
          if (objectTypeRecord.ObjectClass == typeof (T))
            return ((MBObjectManager.ObjectTypeRecord<T>) objectTypeRecord).GetObjectsList();
        }
        return (MBReadOnlyList<T>) null;
      }
      List<T> list = new List<T>();
      foreach (MBObjectManager.IObjectTypeRecord objectTypeRecord in this.ObjectTypeRecords)
      {
        if (typeof (T).IsAssignableFrom(objectTypeRecord.ObjectClass))
        {
          foreach (object obj in (IEnumerable) objectTypeRecord.GetList())
            list.Add((T) obj);
        }
      }
      foreach (MBObjectManager.IObjectTypeRecord objectTypeRecord in this.NonSerializedObjectTypeRecords)
      {
        if (typeof (T).IsAssignableFrom(objectTypeRecord.ObjectClass))
        {
          foreach (object obj in (IEnumerable) objectTypeRecord.GetList())
            list.Add((T) obj);
        }
      }
      return list.GetReadOnlyList<T>();
    }

    public IList<MBObjectBase> CreateObjectTypeList(Type objectClassType)
    {
      foreach (MBObjectManager.IObjectTypeRecord objectTypeRecord in this.ObjectTypeRecords)
      {
        if (objectTypeRecord.ObjectClass == objectClassType)
        {
          List<MBObjectBase> mbObjectBaseList = new List<MBObjectBase>();
          foreach (object obj in (IEnumerable) objectTypeRecord)
          {
            MBObjectBase mbObjectBase = obj as MBObjectBase;
            mbObjectBaseList.Add(mbObjectBase);
          }
          return (IList<MBObjectBase>) mbObjectBaseList;
        }
      }
      foreach (MBObjectManager.IObjectTypeRecord objectTypeRecord in this.NonSerializedObjectTypeRecords)
      {
        if (objectTypeRecord.ObjectClass == objectClassType)
        {
          List<MBObjectBase> mbObjectBaseList = new List<MBObjectBase>();
          foreach (object obj in (IEnumerable) objectTypeRecord)
          {
            MBObjectBase mbObjectBase = obj as MBObjectBase;
            mbObjectBaseList.Add(mbObjectBase);
          }
          return (IList<MBObjectBase>) mbObjectBaseList;
        }
      }
      return (IList<MBObjectBase>) null;
    }

    public void LoadXML(
      string id,
      bool isDevelopment,
      string gameType,
      Type typeOfGameMenusCallbacks = null,
      bool skipXmlFilterForEditor = false)
    {
      List<Tuple<string, string>> toBeMerged = new List<Tuple<string, string>>();
      List<string> xsltList = new List<string>();
      foreach (MbObjectXmlInformation xmlInformation in XmlResource.XmlInformationList)
      {
        if (xmlInformation.Id == id && (skipXmlFilterForEditor | isDevelopment || xmlInformation.GameTypesIncluded.Count == 0 || xmlInformation.GameTypesIncluded.Contains(gameType)))
        {
          string xmlPath = ModuleHelper.GetXmlPath(xmlInformation.ModuleName, xmlInformation.Name);
          if (File.Exists(xmlPath))
          {
            toBeMerged.Add(Tuple.Create<string, string>(ModuleHelper.GetXmlPath(xmlInformation.ModuleName, xmlInformation.Name), ModuleHelper.GetXsdPath(xmlInformation.ModuleName, xmlInformation.Id)));
            string xsltPath = ModuleHelper.GetXsltPath(xmlInformation.ModuleName, xmlInformation.Name);
            if (File.Exists(xsltPath))
              xsltList.Add(xsltPath);
            else
              xsltList.Add("");
          }
          else
          {
            string path = xmlPath.Replace(".xml", "");
            if (Directory.Exists(path))
            {
              foreach (FileInfo file in new DirectoryInfo(path).GetFiles("*.xml"))
              {
                string str = path + "/" + file.Name;
                toBeMerged.Add(Tuple.Create<string, string>(str, ModuleHelper.GetXsdPath(xmlInformation.ModuleName, xmlInformation.Id)));
                str.Replace(".xml", ".xsl");
                if (File.Exists(xmlPath))
                  xsltList.Add(xmlPath);
                else
                  xsltList.Add("");
              }
            }
          }
        }
      }
      XmlDocument mergedXmlFile = MBObjectManager.CreateMergedXmlFile(toBeMerged, xsltList, false);
      try
      {
        this.LoadXml(mergedXmlFile, typeOfGameMenusCallbacks, isDevelopment);
      }
      catch (Exception ex)
      {
        throw new XmlException();
      }
    }

    public static XmlDocument GetMergedXmlForNative(
      string id,
      out List<string> usedPaths)
    {
      usedPaths = new List<string>();
      List<Tuple<string, string>> toBeMerged = new List<Tuple<string, string>>();
      List<string> xsltList = new List<string>();
      foreach (MbObjectXmlInformation mbprojXml in XmlResource.MbprojXmls)
      {
        if (mbprojXml.Id == id)
        {
          if (File.Exists(ModuleHelper.GetXmlPathForNative(mbprojXml.ModuleName, mbprojXml.Name)))
          {
            usedPaths.Add(ModuleHelper.GetXmlPathForNativeWBase(mbprojXml.ModuleName, mbprojXml.Name));
            toBeMerged.Add(Tuple.Create<string, string>(ModuleHelper.GetXmlPathForNative(mbprojXml.ModuleName, mbprojXml.Name), string.Empty));
          }
          string xsltPathForNative = ModuleHelper.GetXsltPathForNative(mbprojXml.ModuleName, mbprojXml.Name);
          if (File.Exists(xsltPathForNative))
            xsltList.Add(xsltPathForNative);
          else
            xsltList.Add("");
        }
      }
      return MBObjectManager.CreateMergedXmlFile(toBeMerged, xsltList, true);
    }

    public static XmlDocument CreateMergedXmlFile(
      List<Tuple<string, string>> toBeMerged,
      List<string> xsltList,
      bool skipValidation)
    {
      XmlDocument xmlDocument = MBObjectManager.CreateDocumentFromXmlFile(toBeMerged.First<Tuple<string, string>>().Item1, toBeMerged.First<Tuple<string, string>>().Item2, skipValidation);
      for (int index = 1; index < toBeMerged.Count; ++index)
      {
        if (xsltList[index] != "")
          xmlDocument = MBObjectManager.ApplyXslt(xsltList[index], xmlDocument);
        XmlDocument documentFromXmlFile = MBObjectManager.CreateDocumentFromXmlFile(toBeMerged[index].Item1, toBeMerged[index].Item2, skipValidation);
        xmlDocument = MBObjectManager.MergeTwoXmls(xmlDocument, documentFromXmlFile);
      }
      return xmlDocument;
    }

    public static XmlDocument ApplyXslt(string xsltPath, XmlDocument baseDocument)
    {
      XmlReader input = (XmlReader) new XmlNodeReader((XmlNode) baseDocument);
      XslCompiledTransform compiledTransform = new XslCompiledTransform();
      compiledTransform.Load(xsltPath);
      XmlDocument xmlDocument = new XmlDocument(baseDocument.CreateNavigator().NameTable);
      using (XmlWriter results = xmlDocument.CreateNavigator().AppendChild())
      {
        compiledTransform.Transform(input, results);
        results.Close();
      }
      return xmlDocument;
    }

    public static XmlDocument MergeTwoXmls(
      XmlDocument xmlDocument1,
      XmlDocument xmlDocument2)
    {
      XDocument xdocument = MBObjectManager.ToXDocument(xmlDocument1);
      xdocument.Root.Add((object) MBObjectManager.ToXDocument(xmlDocument2).Root.Elements());
      return MBObjectManager.ToXmlDocument(xdocument);
    }

    public static XDocument ToXDocument(XmlDocument xmlDocument)
    {
      using (XmlNodeReader xmlNodeReader = new XmlNodeReader((XmlNode) xmlDocument))
      {
        int content = (int) xmlNodeReader.MoveToContent();
        return XDocument.Load((XmlReader) xmlNodeReader);
      }
    }

    public static XmlDocument ToXmlDocument(XDocument xDocument)
    {
      XmlDocument xmlDocument = new XmlDocument();
      using (xDocument.CreateReader())
        xmlDocument.Load(xDocument.CreateReader());
      return xmlDocument;
    }

    public void LoadOneXmlFromFile(
      string xmlPath,
      string xsdPath,
      Type typeOfGameMenusCallbacks = null,
      bool skipValidation = false)
    {
      try
      {
        this.LoadXml(MBObjectManager.CreateDocumentFromXmlFile(xmlPath, xsdPath, skipValidation), typeOfGameMenusCallbacks);
      }
      catch (Exception ex)
      {
        throw new XmlException();
      }
    }

    public XmlDocument LoadXMLFromFileSkipValidation(string xmlPath, string xsdPath)
    {
      try
      {
        return MBObjectManager.CreateDocumentFromXmlFile(xmlPath, xsdPath, true);
      }
      catch
      {
        throw;
      }
    }

    private static void LoadXmlWithValidation(
      string xmlPath,
      string xsdPath,
      XmlDocument xmlDocument)
    {
      Debug.Print("opening " + xsdPath);
      XmlSchemaSet schemas = new XmlSchemaSet();
      XmlTextReader xmlTextReader = (XmlTextReader) null;
      try
      {
        xmlTextReader = new XmlTextReader(xsdPath);
        schemas.Add((string) null, (XmlReader) xmlTextReader);
      }
      catch (FileNotFoundException ex)
      {
        Debug.Print("xsd file of " + xmlPath + " could not be found!", color: Debug.DebugColor.Red);
      }
      catch (Exception ex)
      {
        Debug.Print("xsd file of " + xmlPath + " could not read!", color: Debug.DebugColor.Red);
      }
      XmlReaderSettings settings1 = new XmlReaderSettings();
      settings1.ValidationType = ValidationType.None;
      settings1.Schemas.Add(schemas);
      settings1.ValidationFlags |= XmlSchemaValidationFlags.ReportValidationWarnings;
      settings1.ValidationEventHandler += new System.Xml.Schema.ValidationEventHandler(MBObjectManager.ValidationEventHandler);
      settings1.CloseInput = true;
      try
      {
        XmlReader reader1 = XmlReader.Create(xmlPath, settings1);
        xmlDocument.Load(reader1);
        reader1.Close();
        XmlReaderSettings settings2 = new XmlReaderSettings();
        settings2.ValidationType = ValidationType.Schema;
        settings2.Schemas.Add(schemas);
        settings2.ValidationFlags |= XmlSchemaValidationFlags.ReportValidationWarnings;
        settings2.ValidationEventHandler += new System.Xml.Schema.ValidationEventHandler(MBObjectManager.ValidationEventHandler);
        settings2.CloseInput = true;
        XmlReader reader2 = XmlReader.Create(xmlPath, settings2);
        xmlDocument.Load(reader2);
        reader2.Close();
      }
      catch (Exception ex)
      {
        string localPath = new Uri(xmlDocument.BaseURI).LocalPath;
      }
      xmlTextReader?.Close();
    }

    private static void ValidationEventHandler(object sender, ValidationEventArgs e)
    {
      XmlReader xmlReader = (XmlReader) sender;
      string str = string.Empty;
      switch (e.Severity)
      {
        case XmlSeverityType.Error:
          str = str + "Error: " + e.Message;
          break;
        case XmlSeverityType.Warning:
          str = str + "Warning: " + e.Message;
          break;
      }
      Debug.Print(str + "\nNode: " + xmlReader.Name + "  Value: " + xmlReader.Value + "\nLine: " + (object) e.Exception.LineNumber + "\nXML Path: " + xmlReader.BaseURI, color: Debug.DebugColor.Red);
    }

    private static XmlDocument CreateDocumentFromXmlFile(
      string xmlPath,
      string xsdPath,
      bool forceSkipValidation = false)
    {
      Debug.Print("opening " + xmlPath);
      XmlDocument xmlDocument = new XmlDocument();
      StreamReader streamReader = new StreamReader(xmlPath);
      string end = streamReader.ReadToEnd();
      if (!forceSkipValidation)
        MBObjectManager.LoadXmlWithValidation(xmlPath, xsdPath, xmlDocument);
      else
        xmlDocument.LoadXml(end);
      streamReader.Close();
      return xmlDocument;
    }

    public void GetAllInstancesOfObjectType<T>(ref List<T> returnList) where T : MBObjectBase
    {
      foreach (MBObjectManager.IObjectTypeRecord objectTypeRecord1 in this.ObjectTypeRecords)
      {
        if (objectTypeRecord1.ObjectClass == typeof (T))
        {
          MBObjectManager.ObjectTypeRecord<T> objectTypeRecord2 = (MBObjectManager.ObjectTypeRecord<T>) objectTypeRecord1;
          int count = objectTypeRecord2.RegisteredObjectsList.Count;
          for (int index = 0; index < count; ++index)
            returnList.Add(objectTypeRecord2.RegisteredObjectsList[index]);
        }
      }
      foreach (MBObjectManager.IObjectTypeRecord objectTypeRecord1 in this.NonSerializedObjectTypeRecords)
      {
        if (objectTypeRecord1.ObjectClass == typeof (T))
        {
          MBObjectManager.ObjectTypeRecord<T> objectTypeRecord2 = (MBObjectManager.ObjectTypeRecord<T>) objectTypeRecord1;
          int count = objectTypeRecord2.RegisteredObjectsList.Count;
          for (int index = 0; index < count; ++index)
            returnList.Add(objectTypeRecord2.RegisteredObjectsList[index]);
        }
      }
    }

    public void LoadXml(XmlDocument doc, Type typeOfGameMenusCallbacks, bool isDevelopment = false)
    {
      int i = 0;
      bool flag = false;
      string typeName = (string) null;
      for (; i < doc.ChildNodes.Count; ++i)
      {
        int tmpNodeIndex = i;
        using (IEnumerator<MBObjectManager.IObjectTypeRecord> enumerator = this.ObjectTypeRecords.Where<MBObjectManager.IObjectTypeRecord>((Func<MBObjectManager.IObjectTypeRecord, bool>) (objectTypeRecord => objectTypeRecord.ElementListName == doc.ChildNodes[tmpNodeIndex].Name)).GetEnumerator())
        {
          if (enumerator.MoveNext())
          {
            typeName = enumerator.Current.ElementName;
            flag = true;
          }
        }
        using (IEnumerator<MBObjectManager.IObjectTypeRecord> enumerator = this.NonSerializedObjectTypeRecords.Where<MBObjectManager.IObjectTypeRecord>((Func<MBObjectManager.IObjectTypeRecord, bool>) (objectTypeRecord => objectTypeRecord.ElementListName == doc.ChildNodes[tmpNodeIndex].Name)).GetEnumerator())
        {
          if (enumerator.MoveNext())
          {
            typeName = enumerator.Current.ElementName;
            flag = true;
          }
        }
        if (flag)
          break;
      }
      if (!flag)
        return;
      for (XmlNode node = doc.ChildNodes[i].ChildNodes[0]; node != null; node = node.NextSibling)
      {
        if (node.NodeType != XmlNodeType.Comment)
        {
          string objectName = node.Attributes["id"].Value;
          MBObjectBase presumedObject = this.GetPresumedObject(typeName, objectName, true);
          if (typeOfGameMenusCallbacks != (Type) null)
            presumedObject.Deserialize(this, node, typeOfGameMenusCallbacks);
          else
            presumedObject.Deserialize(this, node);
          presumedObject.AfterInitialized();
        }
      }
    }

    public MBObjectBase CreateObjectFromXmlNode(XmlNode node)
    {
      string name = node.Name;
      foreach (MBObjectManager.IObjectTypeRecord objectTypeRecord in this.ObjectTypeRecords)
      {
        if (objectTypeRecord.ElementName == name)
        {
          string objectName = node.Attributes["id"].Value;
          MBObjectBase presumedObject = this.GetPresumedObject(objectTypeRecord.ElementName, objectName);
          presumedObject.Deserialize(this, node);
          presumedObject.AfterInitialized();
          return presumedObject;
        }
      }
      foreach (MBObjectManager.IObjectTypeRecord objectTypeRecord in this.NonSerializedObjectTypeRecords)
      {
        if (objectTypeRecord.ElementName == name)
        {
          string objectName = node.Attributes["id"].Value;
          MBObjectBase presumedObject = this.GetPresumedObject(objectTypeRecord.ElementName, objectName);
          presumedObject.Deserialize(this, node);
          presumedObject.AfterInitialized();
          return presumedObject;
        }
      }
      return (MBObjectBase) null;
    }

    public MBObjectBase CreateObjectWithoutDeserialize(XmlNode node)
    {
      string name = node.Name;
      foreach (MBObjectManager.IObjectTypeRecord objectTypeRecord in this.ObjectTypeRecords)
      {
        if (objectTypeRecord.ElementName == name)
        {
          string objectName = node.Attributes["id"].Value;
          MBObjectBase presumedObject = this.GetPresumedObject(objectTypeRecord.ElementName, objectName);
          presumedObject.Initialize();
          presumedObject.AfterInitialized();
          return presumedObject;
        }
      }
      foreach (MBObjectManager.IObjectTypeRecord objectTypeRecord in this.NonSerializedObjectTypeRecords)
      {
        if (objectTypeRecord.ElementName == name)
        {
          string objectName = node.Attributes["id"].Value;
          MBObjectBase presumedObject = this.GetPresumedObject(objectTypeRecord.ElementName, objectName);
          presumedObject.Initialize();
          presumedObject.AfterInitialized();
          return presumedObject;
        }
      }
      return (MBObjectBase) null;
    }

    public void ClearEmptyObjects()
    {
      foreach (MBObjectManager.IObjectTypeRecord objectTypeRecord in this.ObjectTypeRecords)
      {
        List<MBObjectBase> mbObjectBaseList = new List<MBObjectBase>();
        foreach (MBObjectBase mbObjectBase in (IEnumerable) objectTypeRecord)
        {
          if (!mbObjectBase.IsReady)
            mbObjectBaseList.Add(mbObjectBase);
        }
        foreach (MBObjectBase mbObjectBase in mbObjectBaseList)
          this.UnregisterObject(mbObjectBase);
      }
      foreach (MBObjectManager.IObjectTypeRecord objectTypeRecord in this.NonSerializedObjectTypeRecords)
      {
        List<MBObjectBase> mbObjectBaseList = new List<MBObjectBase>();
        foreach (MBObjectBase mbObjectBase in (IEnumerable) objectTypeRecord)
        {
          if (!mbObjectBase.IsReady)
            mbObjectBaseList.Add(mbObjectBase);
        }
        foreach (MBObjectBase mbObjectBase in mbObjectBaseList)
          this.UnregisterObject(mbObjectBase);
      }
    }

    public void ClearAllObjects()
    {
      for (int index = this.ObjectTypeRecords.Count - 1; index >= 0; --index)
      {
        List<MBObjectBase> mbObjectBaseList = new List<MBObjectBase>();
        foreach (MBObjectBase mbObjectBase in (IEnumerable) this.ObjectTypeRecords[index])
          mbObjectBaseList.Add(mbObjectBase);
        foreach (MBObjectBase mbObjectBase in mbObjectBaseList)
        {
          this.ObjectTypeRecords[index].UnregisterMBObject(mbObjectBase);
          this.AfterUnregisterObject(mbObjectBase);
        }
      }
      for (int index = this.NonSerializedObjectTypeRecords.Count - 1; index >= 0; --index)
      {
        List<MBObjectBase> mbObjectBaseList = new List<MBObjectBase>();
        foreach (MBObjectBase mbObjectBase in (IEnumerable) this.NonSerializedObjectTypeRecords[index])
          mbObjectBaseList.Add(mbObjectBase);
        foreach (MBObjectBase mbObjectBase in mbObjectBaseList)
        {
          this.NonSerializedObjectTypeRecords[index].UnregisterMBObject(mbObjectBase);
          this.AfterUnregisterObject(mbObjectBase);
        }
      }
    }

    public void ClearAllObjectsWithType(Type type)
    {
      for (int index = this.ObjectTypeRecords.Count - 1; index >= 0; --index)
      {
        if (this.ObjectTypeRecords[index].ObjectClass == type)
        {
          List<MBObjectBase> mbObjectBaseList = new List<MBObjectBase>();
          foreach (MBObjectBase mbObjectBase in (IEnumerable) this.ObjectTypeRecords[index])
            mbObjectBaseList.Add(mbObjectBase);
          foreach (MBObjectBase mbObjectBase in mbObjectBaseList)
            this.UnregisterObject(mbObjectBase);
        }
      }
      for (int index = this.NonSerializedObjectTypeRecords.Count - 1; index >= 0; --index)
      {
        if (this.NonSerializedObjectTypeRecords[index].ObjectClass == type)
        {
          List<MBObjectBase> mbObjectBaseList = new List<MBObjectBase>();
          foreach (MBObjectBase mbObjectBase in (IEnumerable) this.NonSerializedObjectTypeRecords[index])
            mbObjectBaseList.Add(mbObjectBase);
          foreach (MBObjectBase mbObjectBase in mbObjectBaseList)
            this.UnregisterObject(mbObjectBase);
        }
      }
    }

    public T ReadObjectReferenceFromXml<T>(string attributeName, XmlNode node) where T : MBObjectBase
    {
      if (node.Attributes[attributeName] == null)
        return default (T);
      string exceptionString = node.Attributes[attributeName].Value;
      string typeName = exceptionString.Split(".".ToCharArray())[0];
      string objectName = !(typeName == exceptionString) ? exceptionString.Split(".".ToCharArray())[1] : throw new MBInvalidReferenceException(exceptionString);
      if (typeName == string.Empty || objectName == string.Empty)
        throw new MBInvalidReferenceException(exceptionString);
      return this.GetPresumedObject(typeName, objectName) as T;
    }

    public MBObjectBase ReadObjectReferenceFromXml(
      string attributeName,
      Type objectType,
      XmlNode node)
    {
      if (node.Attributes[attributeName] == null)
        return (MBObjectBase) null;
      string exceptionString = node.Attributes[attributeName].Value;
      string typeName = exceptionString.Split(".".ToCharArray())[0];
      string objectName = !(typeName == exceptionString) ? exceptionString.Split(".".ToCharArray())[1] : throw new MBInvalidReferenceException(exceptionString);
      if (typeName == string.Empty || objectName == string.Empty)
        throw new MBInvalidReferenceException(exceptionString);
      return this.GetPresumedObject(typeName, objectName);
    }

    public void WriteObjectReferenceToXml(
      string attributeName,
      MBObjectBase mbObject,
      XmlWriter writer)
    {
      Type type = mbObject.GetType();
      string str = "";
      foreach (MBObjectManager.IObjectTypeRecord objectTypeRecord in this.ObjectTypeRecords)
      {
        if (objectTypeRecord.ObjectClass == type)
          str = objectTypeRecord.ElementName;
      }
      foreach (MBObjectManager.IObjectTypeRecord objectTypeRecord in this.NonSerializedObjectTypeRecords)
      {
        if (objectTypeRecord.ObjectClass == type)
          str = objectTypeRecord.ElementName;
      }
      if (str == "")
        throw new MBTypeMismatchException("Class Prefix is null.");
      writer.WriteAttributeString(attributeName, str + "." + mbObject.StringId);
    }

    public T CreateObject<T>(string stringId) where T : MBObjectBase, new()
    {
      T obj = new T();
      obj.StringId = stringId;
      this.RegisterObject<T>(obj);
      if (this._handlers != null)
      {
        foreach (IObjectManagerHandler handler in this._handlers)
          handler.AfterCreateObject((MBObjectBase) obj);
      }
      return obj;
    }

    public T CreateObject<T>() where T : MBObjectBase, new() => this.CreateObject<T>(typeof (T).ToString() + "_1");

    public void DebugPrint(PrintOutputDelegate printOutput)
    {
      printOutput("-Printing MBObjectManager Debug-");
      foreach (MBObjectManager.IObjectTypeRecord objectTypeRecord in this.ObjectTypeRecords)
        printOutput(objectTypeRecord.DebugBasicDump());
      foreach (MBObjectManager.IObjectTypeRecord objectTypeRecord in this.NonSerializedObjectTypeRecords)
        printOutput(objectTypeRecord.DebugBasicDump());
    }

    public void AddHandler(IObjectManagerHandler handler)
    {
      if (this._handlers == null)
        this._handlers = new List<IObjectManagerHandler>();
      this._handlers.Add(handler);
    }

    public void RemoveHandler(IObjectManagerHandler handler) => this._handlers.Remove(handler);

    public string DebugDump()
    {
      string contents = "" + "--------------------------------------\r\n" + "----Printing MBObjectManager Debug----\r\n" + "--------------------------------------\r\n" + "\r\n";
      foreach (MBObjectManager.IObjectTypeRecord objectTypeRecord in this.ObjectTypeRecords)
        contents += objectTypeRecord.DebugDump();
      foreach (MBObjectManager.IObjectTypeRecord objectTypeRecord in this.NonSerializedObjectTypeRecords)
        contents += objectTypeRecord.DebugDump();
      File.WriteAllText("mbobjectmanagerdump.txt", contents);
      return contents;
    }

    public IDictionary<string, T> GetObjectDictionary<T>() => throw new NotImplementedException();

    public void ReInitialize()
    {
      List<string> stringList = new List<string>();
      foreach (MBObjectManager.IObjectTypeRecord objectTypeRecord in this.ObjectTypeRecords.ToList<MBObjectManager.IObjectTypeRecord>())
      {
        if (stringList.Contains(objectTypeRecord.ElementName))
          this.ObjectTypeRecords.Remove(objectTypeRecord);
        else
          stringList.Add(objectTypeRecord.ElementName);
      }
      foreach (MBObjectManager.IObjectTypeRecord objectTypeRecord in this.ObjectTypeRecords.ToList<MBObjectManager.IObjectTypeRecord>())
        objectTypeRecord.ReInitialize();
    }

    public string GetObjectTypeIds()
    {
      string str = "";
      foreach (MBObjectManager.IObjectTypeRecord objectTypeRecord in this.ObjectTypeRecords)
        str = str + (object) objectTypeRecord.TypeNo + " - " + objectTypeRecord.GetType().FullName + "\n";
      foreach (MBObjectManager.IObjectTypeRecord objectTypeRecord in this.NonSerializedObjectTypeRecords)
        str = str + (object) objectTypeRecord.TypeNo + " - " + objectTypeRecord.GetType().FullName + "\n";
      return str;
    }

    internal interface IObjectTypeRecord : IEnumerable
    {
      bool AutoCreate { get; }

      string ElementName { get; }

      string ElementListName { get; }

      Type ObjectClass { get; }

      uint TypeNo { get; }

      void ReInitialize();

      MBObjectBase CreatePresumedMBObject(string objectName);

      void RegisterMBObject(MBObjectBase obj, bool presumed, out MBObjectBase registeredObject);

      void RegisterMBObjectWithoutInitialization(MBObjectBase obj);

      void UnregisterMBObject(MBObjectBase obj);

      MBObjectBase GetMBObject(string objId);

      MBObjectBase GetMBObject(MBGUID objId);

      bool ContainsObject(string objId);

      string DebugDump();

      string DebugBasicDump();

      IList GetList();

      void PreAfterLoad();

      void AfterLoad();
    }

    internal class ObjectTypeRecord<T> : 
      MBObjectManager.IObjectTypeRecord,
      IEnumerable,
      IEnumerable<T>
      where T : MBObjectBase
    {
      private bool _autoCreate;
      private string _elementName;
      private string _elementListName;
      private uint _objCount;
      private uint _typeNo;
      private Dictionary<string, T> _registeredObjects;
      private Dictionary<MBGUID, T> _registeredObjectsWithGuid;

      bool MBObjectManager.IObjectTypeRecord.AutoCreate => this._autoCreate;

      string MBObjectManager.IObjectTypeRecord.ElementName => this._elementName;

      string MBObjectManager.IObjectTypeRecord.ElementListName => this._elementListName;

      Type MBObjectManager.IObjectTypeRecord.ObjectClass => typeof (T);

      uint MBObjectManager.IObjectTypeRecord.TypeNo => this._typeNo;

      internal List<T> RegisteredObjectsList { get; private set; }

      internal ObjectTypeRecord(
        uint newTypeNo,
        string classPrefix,
        string classListPrefix,
        bool autoCreate)
      {
        this._typeNo = newTypeNo;
        this._elementName = classPrefix;
        this._elementListName = classListPrefix;
        this._autoCreate = autoCreate;
        this._registeredObjects = new Dictionary<string, T>();
        this._registeredObjectsWithGuid = new Dictionary<MBGUID, T>();
        this.RegisteredObjectsList = new List<T>();
        this._objCount = 0U;
      }

      void MBObjectManager.IObjectTypeRecord.ReInitialize()
      {
        uint num = 0;
        foreach (T registeredObjects in this.RegisteredObjectsList)
        {
          uint subId = registeredObjects.Id.SubId;
          if (subId > num)
            num = subId;
        }
        this._objCount = num + 1U;
      }

      IEnumerator<T> IEnumerable<T>.GetEnumerator() => this.EnumerateElements();

      IEnumerator IEnumerable.GetEnumerator() => (IEnumerator) this.EnumerateElements();

      internal MBGUID GetNewId()
      {
        ++this._objCount;
        return new MBGUID(this._typeNo, this._objCount);
      }

      MBObjectBase MBObjectManager.IObjectTypeRecord.CreatePresumedMBObject(
        string objectName)
      {
        return (MBObjectBase) this.CreatePresumedObject(objectName);
      }

      private T CreatePresumedObject(string objectName)
      {
        T instance = Activator.CreateInstance<T>();
        instance.StringId = objectName;
        instance.IsReady = false;
        instance.IsInitialized = false;
        return instance;
      }

      MBObjectBase MBObjectManager.IObjectTypeRecord.GetMBObject(
        string objId)
      {
        return (MBObjectBase) this.GetObject(objId);
      }

      internal T GetObject(string objId)
      {
        T obj = default (T);
        this._registeredObjects.TryGetValue(objId, out obj);
        return obj;
      }

      bool MBObjectManager.IObjectTypeRecord.ContainsObject(string objId) => this._registeredObjects.ContainsKey(objId);

      public MBObjectBase GetMBObject(MBGUID objId)
      {
        T obj = default (T);
        this._registeredObjectsWithGuid.TryGetValue(objId, out obj);
        return (MBObjectBase) obj;
      }

      void MBObjectManager.IObjectTypeRecord.RegisterMBObjectWithoutInitialization(
        MBObjectBase mbObject)
      {
        T obj = (T) mbObject;
        if (string.IsNullOrEmpty(obj.StringId) || (obj.Id.InternalValue == 0U || this._registeredObjects.ContainsKey(obj.StringId)))
          return;
        this._registeredObjects.Add(obj.StringId, obj);
        this._registeredObjectsWithGuid.Add(obj.Id, obj);
        this.RegisteredObjectsList.Add(obj);
      }

      void MBObjectManager.IObjectTypeRecord.RegisterMBObject(
        MBObjectBase obj,
        bool presumed,
        out MBObjectBase registeredObject)
      {
        if (obj is T)
          this.RegisterObject(obj as T, presumed, out registeredObject);
        else
          registeredObject = (MBObjectBase) null;
      }

      internal void RegisterObject(T obj, bool presumed, out MBObjectBase registeredObject)
      {
        T obj1;
        if (this._registeredObjects.TryGetValue(obj.StringId, out obj1))
        {
          if ((object) obj1 == (object) obj | presumed)
          {
            registeredObject = (MBObjectBase) obj1;
            return;
          }
          (string str2, long _) = this.GetIdParts(obj.StringId);
          if (this._registeredObjects.ContainsKey(obj.StringId))
          {
            long objCount = (long) this._objCount;
            for (obj.StringId = str2 + objCount.ToString(); this._registeredObjects.ContainsKey(obj.StringId); obj.StringId = str2 + objCount.ToString())
              ++objCount;
          }
        }
        this._registeredObjects.Add(obj.StringId, obj);
        obj.Id = this.GetNewId();
        this._registeredObjectsWithGuid.Add(obj.Id, obj);
        this.RegisteredObjectsList.Add(obj);
        obj.IsReady = !presumed;
        obj.IsRegistered = true;
        obj.AfterRegister();
        registeredObject = (MBObjectBase) obj;
      }

      private (string str, long number) GetIdParts(string stringId)
      {
        int index = stringId.Length - 1;
        while (index > 0 && char.IsDigit(stringId[index]))
          --index;
        string str = stringId.Substring(0, index + 1);
        long result = 0;
        if (index < stringId.Length - 1)
          long.TryParse(stringId.Substring(index + 1, stringId.Length - index - 1), out result);
        long num = result;
        return (str, num);
      }

      void MBObjectManager.IObjectTypeRecord.UnregisterMBObject(MBObjectBase obj)
      {
        if (!(obj is T obj1))
          throw new MBIllegalRegisterException();
        this.UnregisterObject(obj1);
      }

      private void UnregisterObject(T obj)
      {
        obj.IsReady = false;
        obj.IsRegistered = false;
        if (this._registeredObjects.ContainsKey(obj.StringId) && (object) this._registeredObjects[obj.StringId] == (object) obj)
          this._registeredObjects.Remove(obj.StringId);
        if (this._registeredObjectsWithGuid.ContainsKey(obj.Id) && (object) this._registeredObjectsWithGuid[obj.Id] == (object) obj)
          this._registeredObjectsWithGuid.Remove(obj.Id);
        this.RegisteredObjectsList.Remove(obj);
      }

      internal MBReadOnlyList<T> GetObjectsList() => this.RegisteredObjectsList.GetReadOnlyList<T>();

      IList MBObjectManager.IObjectTypeRecord.GetList() => (IList) this.RegisteredObjectsList;

      string MBObjectManager.IObjectTypeRecord.DebugDump()
      {
        string str = "" + "**************************************\r\n" + this._elementName + " " + (object) this._objCount + "\r\n" + "**************************************\r\n" + "\r\n";
        foreach (KeyValuePair<MBGUID, T> keyValuePair in this._registeredObjectsWithGuid)
          str = str + keyValuePair.Key.ToString() + " " + keyValuePair.Value.ToString() + "\r\n";
        return str;
      }

      string MBObjectManager.IObjectTypeRecord.DebugBasicDump() => this._elementName + " " + (object) this._objCount;

      private IEnumerator<T> EnumerateElements()
      {
        for (int i = 0; i < this.RegisteredObjectsList.Count; ++i)
          yield return this.RegisteredObjectsList[i];
      }

      void MBObjectManager.IObjectTypeRecord.PreAfterLoad()
      {
        for (int index = this.RegisteredObjectsList.Count - 1; index >= 0; --index)
          this.RegisteredObjectsList[index].PreAfterLoadInternal();
      }

      void MBObjectManager.IObjectTypeRecord.AfterLoad()
      {
        for (int index = this.RegisteredObjectsList.Count - 1; index >= 0; --index)
          this.RegisteredObjectsList[index].AfterLoadInternal();
      }
    }
  }
}
