// Decompiled with JetBrains decompiler
// Type: TaleWorlds.MountAndBlade.FleePosition
// Assembly: TaleWorlds.MountAndBlade, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3C0E3F42-756F-4407-B817-06BA412A78B0
// Assembly location: E:\SteamLibrary\steamapps\common\Mount & Blade II Bannerlord\bin\Win64_Shipping_Client\TaleWorlds.MountAndBlade.dll

using System;
using System.Collections.Generic;
using TaleWorlds.Core;
using TaleWorlds.Engine;
using TaleWorlds.Library;

namespace TaleWorlds.MountAndBlade
{
  public class FleePosition : ScriptComponentBehaviour
  {
    private List<Vec3> _nodes = new List<Vec3>();
    private BattleSideEnum _side;
    public string Side = "both";

    private static bool IsSallyOut()
    {
      SiegeMissionController missionBehaviour = Mission.Current.GetMissionBehaviour<SiegeMissionController>();
      return missionBehaviour != null && missionBehaviour.IsSallyOut;
    }

    protected internal override void OnInit()
    {
      this.CollectNodes();
      bool flag = false;
      this._side = !(this.Side == "both") ? (!(this.Side == "attacker") ? (!(this.Side == "defender") ? BattleSideEnum.None : (flag ? BattleSideEnum.Attacker : BattleSideEnum.Defender)) : (flag ? BattleSideEnum.Defender : BattleSideEnum.Attacker)) : BattleSideEnum.None;
      if (this.GameEntity.HasTag("sally_out"))
      {
        if (!FleePosition.IsSallyOut())
          return;
        Mission.Current.AddFleePosition(this);
      }
      else
        Mission.Current.AddFleePosition(this);
    }

    public BattleSideEnum GetSide() => this._side;

    protected internal override void OnEditorInit() => this.CollectNodes();

    private void CollectNodes()
    {
      this._nodes.Clear();
      int childCount = this.GameEntity.ChildCount;
      for (int index = 0; index < childCount; ++index)
        this._nodes.Add(this.GameEntity.GetChild(index).GlobalPosition);
      if (!this._nodes.IsEmpty<Vec3>())
        return;
      this._nodes.Add(this.GameEntity.GlobalPosition);
    }

    protected internal override void OnEditorTick(float dt)
    {
      this.CollectNodes();
      bool flag = this.GameEntity.IsSelectedOnEditor();
      int childCount = this.GameEntity.ChildCount;
      for (int index = 0; !flag && index < childCount; ++index)
        flag = this.GameEntity.GetChild(index).IsSelectedOnEditor();
      if (!flag)
        return;
      for (int index = 0; index < this._nodes.Count; ++index)
      {
        int num = this._nodes.Count - 1;
      }
    }

    public Vec3 GetClosestPointToEscape(Vec2 position)
    {
      if (this._nodes.Count == 1)
        return this._nodes[0];
      float num1 = float.MaxValue;
      Vec3 vec3_1 = this._nodes[0];
      for (int index = 0; index < this._nodes.Count - 1; ++index)
      {
        Vec3 node1 = this._nodes[index];
        Vec3 node2 = this._nodes[index + 1];
        float num2 = node1.DistanceSquared(node2);
        if ((double) num2 > 0.0)
        {
          float num3 = Math.Max(0.0f, Math.Min(1f, Vec2.DotProduct(position - node1.AsVec2, node2.AsVec2 - node1.AsVec2) / num2));
          Vec3 vec3_2 = node1 + num3 * (node2 - node1);
          float num4 = vec3_2.AsVec2.DistanceSquared(position);
          if ((double) num1 > (double) num4)
          {
            num1 = num4;
            vec3_1 = vec3_2;
          }
        }
        else
        {
          num1 = 0.0f;
          vec3_1 = node1;
        }
      }
      return vec3_1;
    }

    protected internal override bool MovesEntity() => true;
  }
}
