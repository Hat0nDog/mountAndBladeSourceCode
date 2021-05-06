// Decompiled with JetBrains decompiler
// Type: TaleWorlds.Library.NavigationPath
// Assembly: TaleWorlds.Library, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 271D0DB8-3C5D-4850-A565-542B4CF7D347
// Assembly location: E:\SteamLibrary\steamapps\common\Mount & Blade II Bannerlord\bin\Win64_Shipping_Client\TaleWorlds.Library.dll

using System.Runtime.Serialization;
using System.Security.Permissions;

namespace TaleWorlds.Library
{
  public class NavigationPath : ISerializable
  {
    private const int PathSize = 128;

    public Vec2[] PathPoints { get; private set; }

    public int Size { get; set; }

    public NavigationPath() => this.PathPoints = new Vec2[128];

    protected NavigationPath(SerializationInfo info, StreamingContext context)
    {
      this.PathPoints = new Vec2[128];
      this.Size = info.GetInt32("s");
      for (int index = 0; index < this.Size; ++index)
      {
        float single1 = info.GetSingle("x" + (object) index);
        float single2 = info.GetSingle("y" + (object) index);
        this.PathPoints[index] = new Vec2(single1, single2);
      }
    }

    [SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
    public virtual void GetObjectData(SerializationInfo info, StreamingContext context)
    {
      info.AddValue("s", this.Size);
      for (int index = 0; index < this.Size; ++index)
      {
        info.AddValue("x" + (object) index, this.PathPoints[index].x);
        info.AddValue("y" + (object) index, this.PathPoints[index].y);
      }
    }

    public Vec2 this[int i] => this.PathPoints[i];

    public void DebugRenderUsing(int index, Vec3 position)
    {
    }
  }
}
