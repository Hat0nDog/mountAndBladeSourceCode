// Decompiled with JetBrains decompiler
// Type: TaleWorlds.Core.ManagedParameters
// Assembly: TaleWorlds.Core, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 43F7B45C-9AE1-4715-A37B-2364840F62AF
// Assembly location: E:\SteamLibrary\steamapps\common\Mount & Blade II Bannerlord\bin\Win64_Shipping_Client\TaleWorlds.Core.dll

using System;
using System.IO;
using System.Xml;
using TaleWorlds.Library;

namespace TaleWorlds.Core
{
  public sealed class ManagedParameters : IManagedParametersInitializer
  {
    private readonly float[] _managedParametersArray = new float[74];

    public static ManagedParameters Instance { get; } = new ManagedParameters();

    public static float GetParameter(ManagedParametersEnum managedParameterType) => ManagedParameters.Instance._managedParametersArray[(int) managedParameterType];

    public static void SetParameter(ManagedParametersEnum managedParameterType, float newValue) => ManagedParameters.Instance._managedParametersArray[(int) managedParameterType] = newValue;

    public void Initialize(string relativeXmlPath) => this.LoadFromXml((XmlNode) ManagedParameters.LoadXmlFile(relativeXmlPath));

    private ManagedParameters()
    {
    }

    private static XmlDocument LoadXmlFile(string path)
    {
      Debug.Print("opening " + path);
      XmlDocument xmlDocument = new XmlDocument();
      StreamReader streamReader = new StreamReader(path);
      xmlDocument.LoadXml(streamReader.ReadToEnd());
      streamReader.Close();
      return xmlDocument;
    }

    private void LoadFromXml(XmlNode doc)
    {
      Debug.Print("loading managed_core_parameters.xml");
      if (doc.ChildNodes.Count <= 1)
        throw new TWXmlLoadException("Incorrect XML document format.");
      if (doc.ChildNodes[1].Name != "base")
        throw new TWXmlLoadException("Incorrect XML document format.");
      if (doc.ChildNodes[1].ChildNodes[0].Name != "managed_core_parameters")
        throw new TWXmlLoadException("Incorrect XML document format.");
      XmlNode xmlNode = (XmlNode) null;
      if (doc.ChildNodes[1].ChildNodes[0].Name == "managed_core_parameters")
        xmlNode = doc.ChildNodes[1].ChildNodes[0].ChildNodes[0];
      for (; xmlNode != null; xmlNode = xmlNode.NextSibling)
      {
        ManagedParametersEnum result;
        if (xmlNode.Name == "managed_core_parameter" && xmlNode.NodeType != XmlNodeType.Comment && Enum.TryParse<ManagedParametersEnum>(xmlNode.Attributes["id"].Value, true, out result))
          this._managedParametersArray[(int) result] = float.Parse(xmlNode.Attributes["value"].Value);
      }
    }

    public float GetManagedParameter(ManagedParametersEnum managedParameterEnum) => this._managedParametersArray[(int) managedParameterEnum];
  }
}
