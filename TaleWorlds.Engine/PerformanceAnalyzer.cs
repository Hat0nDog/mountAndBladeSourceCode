// Decompiled with JetBrains decompiler
// Type: TaleWorlds.Engine.PerformanceAnalyzer
// Assembly: TaleWorlds.Engine, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: DD7DC4E8-6EB6-4CB8-8DE1-FC78E45F7FE8
// Assembly location: E:\SteamLibrary\steamapps\common\Mount & Blade II Bannerlord\bin\Win64_Shipping_Client\TaleWorlds.Engine.dll

using System;
using System.Collections.Generic;
using System.Xml;

namespace TaleWorlds.Engine
{
  public class PerformanceAnalyzer
  {
    private List<PerformanceAnalyzer.PerformanceObject> objects = new List<PerformanceAnalyzer.PerformanceObject>();
    private PerformanceAnalyzer.PerformanceObject currentObject;

    public void Start(string name)
    {
      PerformanceAnalyzer.PerformanceObject performanceObject = new PerformanceAnalyzer.PerformanceObject(name);
      this.currentObject = performanceObject;
      this.objects.Add(performanceObject);
    }

    public void End() => this.currentObject = (PerformanceAnalyzer.PerformanceObject) null;

    public void FinalizeAndWrite(string filePath)
    {
      try
      {
        XmlDocument xmlDocument = new XmlDocument();
        XmlNode element1 = (XmlNode) xmlDocument.CreateElement("objects");
        xmlDocument.AppendChild(element1);
        foreach (PerformanceAnalyzer.PerformanceObject performanceObject in this.objects)
        {
          XmlNode element2 = (XmlNode) xmlDocument.CreateElement("object");
          XmlNode node = element2;
          XmlDocument document = xmlDocument;
          performanceObject.Write(node, document);
          element1.AppendChild(element2);
        }
        xmlDocument.Save(filePath);
      }
      catch (Exception ex)
      {
        MBDebug.ShowWarning("Exception occurred while trying to write " + filePath + ": " + ex.ToString());
      }
    }

    public void Tick(float dt)
    {
      if (this.currentObject == null)
        return;
      this.currentObject.AddFps(Utilities.GetFps(), Utilities.GetMainFps(), Utilities.GetRendererFps());
    }

    private class PerformanceObject
    {
      private string name;
      private int frameCount;
      private float totalMainFps;
      private float totalRendererFps;
      private float totalFps;

      private float AverageMainFps => this.frameCount > 0 ? this.totalMainFps / (float) this.frameCount : 0.0f;

      private float AverageRendererFps => this.frameCount > 0 ? this.totalRendererFps / (float) this.frameCount : 0.0f;

      private float AverageFps => this.frameCount > 0 ? this.totalFps / (float) this.frameCount : 0.0f;

      public void AddFps(float fps, float main, float renderer)
      {
        ++this.frameCount;
        this.totalFps += fps;
        this.totalMainFps += main;
        this.totalRendererFps += renderer;
      }

      public void Write(XmlNode node, XmlDocument document)
      {
        XmlAttribute attribute1 = document.CreateAttribute("name");
        attribute1.Value = this.name;
        node.Attributes.Append(attribute1);
        XmlAttribute attribute2 = document.CreateAttribute("frameCount");
        attribute2.Value = this.frameCount.ToString();
        node.Attributes.Append(attribute2);
        XmlAttribute attribute3 = document.CreateAttribute("averageFps");
        attribute3.Value = this.AverageFps.ToString();
        node.Attributes.Append(attribute3);
        XmlAttribute attribute4 = document.CreateAttribute("averageMainFps");
        attribute4.Value = this.AverageMainFps.ToString();
        node.Attributes.Append(attribute4);
        XmlAttribute attribute5 = document.CreateAttribute("averageRendererFps");
        attribute5.Value = this.AverageRendererFps.ToString();
        node.Attributes.Append(attribute5);
      }

      public PerformanceObject(string objectName) => this.name = objectName;
    }
  }
}
