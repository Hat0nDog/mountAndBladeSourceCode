// Decompiled with JetBrains decompiler
// Type: TaleWorlds.Core.SkeletonScale
// Assembly: TaleWorlds.Core, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 43F7B45C-9AE1-4715-A37B-2364840F62AF
// Assembly location: E:\SteamLibrary\steamapps\common\Mount & Blade II Bannerlord\bin\Win64_Shipping_Client\TaleWorlds.Core.dll

using System.Collections.Generic;
using System.Xml;
using TaleWorlds.Library;
using TaleWorlds.ObjectSystem;

namespace TaleWorlds.Core
{
  public sealed class SkeletonScale : MBObjectBase
  {
    public string SkeletonModel { get; private set; }

    public Vec3 MountSitBoneScale { get; private set; }

    public float MountRadiusAdder { get; private set; }

    public Vec3[] Scales { get; private set; }

    public List<string> BoneNames { get; private set; }

    public sbyte[] BoneIndices { get; private set; }

    public SkeletonScale() => this.BoneNames = (List<string>) null;

    public override void Deserialize(MBObjectManager objectManager, XmlNode node)
    {
      base.Deserialize(objectManager, node);
      this.SkeletonModel = node.Attributes["skeleton"].InnerText;
      XmlAttribute attribute1 = node.Attributes["mount_sit_bone_scale"];
      Vec3 vec3_1 = new Vec3(1f, 1f, 1f);
      if (attribute1 != null)
      {
        string[] strArray = attribute1.Value.Split(',');
        if (strArray.Length == 3)
        {
          float.TryParse(strArray[0], out vec3_1.x);
          float.TryParse(strArray[1], out vec3_1.y);
          float.TryParse(strArray[2], out vec3_1.z);
        }
      }
      this.MountSitBoneScale = vec3_1;
      XmlAttribute attribute2 = node.Attributes["mount_radius_adder"];
      if (attribute2 != null)
        this.MountRadiusAdder = float.Parse(attribute2.Value);
      this.BoneNames = new List<string>();
      foreach (XmlNode childNode1 in node.ChildNodes)
      {
        if (childNode1.Name == "BoneScales")
        {
          List<Vec3> vec3List = new List<Vec3>();
          foreach (XmlNode childNode2 in childNode1.ChildNodes)
          {
            if (childNode2.Attributes != null && childNode2.Name == "BoneScale")
            {
              XmlAttribute attribute3 = childNode2.Attributes["scale"];
              Vec3 vec3_2 = new Vec3();
              if (attribute3 != null)
              {
                string[] strArray = attribute3.Value.Split(',');
                if (strArray.Length == 3)
                {
                  float.TryParse(strArray[0], out vec3_2.x);
                  float.TryParse(strArray[1], out vec3_2.y);
                  float.TryParse(strArray[2], out vec3_2.z);
                }
              }
              this.BoneNames.Add(childNode2.Attributes["bone_name"].InnerText);
              vec3List.Add(vec3_2);
            }
          }
          this.Scales = vec3List.ToArray();
        }
      }
    }

    public void SetBoneIndices(sbyte[] boneIndices)
    {
      this.BoneIndices = boneIndices;
      this.BoneNames = (List<string>) null;
    }
  }
}
