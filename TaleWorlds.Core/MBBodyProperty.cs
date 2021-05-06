// Decompiled with JetBrains decompiler
// Type: TaleWorlds.Core.MBBodyProperty
// Assembly: TaleWorlds.Core, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 43F7B45C-9AE1-4715-A37B-2364840F62AF
// Assembly location: E:\SteamLibrary\steamapps\common\Mount & Blade II Bannerlord\bin\Win64_Shipping_Client\TaleWorlds.Core.dll

using System.Xml;
using TaleWorlds.ObjectSystem;

namespace TaleWorlds.Core
{
  public class MBBodyProperty : MBObjectBase
  {
    private BodyProperties _bodyPropertyMin;
    private BodyProperties _bodyPropertyMax;

    public BodyProperties BodyPropertyMin => this._bodyPropertyMin;

    public BodyProperties BodyPropertyMax => this._bodyPropertyMax;

    public void Init(BodyProperties bodyPropertyMin, BodyProperties bodyPropertyMax)
    {
      this._bodyPropertyMin = bodyPropertyMin;
      this._bodyPropertyMax = bodyPropertyMax;
      if ((double) this._bodyPropertyMax.Age <= 0.0)
        this._bodyPropertyMax = this._bodyPropertyMin;
      if ((double) this._bodyPropertyMin.Age > 0.0)
        return;
      this._bodyPropertyMin = this._bodyPropertyMax;
    }

    public override void Deserialize(MBObjectManager objectManager, XmlNode node)
    {
      base.Deserialize(objectManager, node);
      foreach (XmlNode childNode in node.ChildNodes)
      {
        if (childNode.Name == "BodyPropertiesMin")
          BodyProperties.FromXmlNode(childNode, out this._bodyPropertyMin);
        else if (childNode.Name == "BodyPropertiesMax")
          BodyProperties.FromXmlNode(childNode, out this._bodyPropertyMax);
      }
      if ((double) this._bodyPropertyMax.Age <= 0.0)
        this._bodyPropertyMax = this._bodyPropertyMin;
      if ((double) this._bodyPropertyMin.Age > 0.0)
        return;
      this._bodyPropertyMin = this._bodyPropertyMax;
    }
  }
}
