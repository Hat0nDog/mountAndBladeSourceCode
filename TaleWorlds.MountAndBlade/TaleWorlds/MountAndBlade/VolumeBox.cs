// Decompiled with JetBrains decompiler
// Type: TaleWorlds.MountAndBlade.VolumeBox
// Assembly: TaleWorlds.MountAndBlade, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3C0E3F42-756F-4407-B817-06BA412A78B0
// Assembly location: E:\SteamLibrary\steamapps\common\Mount & Blade II Bannerlord\bin\Win64_Shipping_Client\TaleWorlds.MountAndBlade.dll

using System;
using System.Collections.Generic;
using System.Linq;
using TaleWorlds.Library;

namespace TaleWorlds.MountAndBlade
{
  public class VolumeBox : MissionObject
  {
    private VolumeBox.VolumeBoxDelegate _volumeBoxIsOccupiedDelegate;

    protected internal override void OnInit()
    {
    }

    public void AddToCheckList(Agent agent)
    {
    }

    public void RemoveFromCheckList(Agent agent)
    {
    }

    public void SetIsOccupiedDelegate(VolumeBox.VolumeBoxDelegate volumeBoxDelegate) => this._volumeBoxIsOccupiedDelegate = volumeBoxDelegate;

    public IEnumerable<Agent> GetAgentsIn(Predicate<Agent> predicate)
    {
      MatrixFrame globalFrame = this.GameEntity.GetGlobalFrame();
      return Mission.Current.GetAgentsInRange(globalFrame.origin.AsVec2, globalFrame.rotation.GetScaleVector().AsVec2.Length).Where<Agent>((Func<Agent, bool>) (agent => predicate(agent) && this.IsPointIn(agent.Position)));
    }

    public IEnumerable<Agent> GetAgentsIn() => this.GetAgentsIn((Predicate<Agent>) (a => true));

    public bool HasAgentsIn(Predicate<Agent> predicate) => this.GetAgentsIn(predicate).Any<Agent>();

    public bool HasAgentsIn() => this.HasAgentsIn((Predicate<Agent>) (a => true));

    public bool IsPointIn(Vec3 point)
    {
      MatrixFrame globalFrame = this.GameEntity.GetGlobalFrame();
      Vec3 scaleVector = globalFrame.rotation.GetScaleVector();
      globalFrame.rotation.ApplyScaleLocal(new Vec3(1f / scaleVector.x, 1f / scaleVector.y, 1f / scaleVector.z));
      point = globalFrame.TransformToLocal(point);
      return (double) MathF.Abs(point.x) <= (double) scaleVector.x / 2.0 && (double) MathF.Abs(point.y) <= (double) scaleVector.y / 2.0 && (double) MathF.Abs(point.z) <= (double) scaleVector.z / 2.0;
    }

    public delegate void VolumeBoxDelegate(VolumeBox volumeBox, List<Agent> agentsInVolume);
  }
}
