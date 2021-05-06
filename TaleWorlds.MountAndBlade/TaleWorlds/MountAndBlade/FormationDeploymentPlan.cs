// Decompiled with JetBrains decompiler
// Type: TaleWorlds.MountAndBlade.FormationDeploymentPlan
// Assembly: TaleWorlds.MountAndBlade, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3C0E3F42-756F-4407-B817-06BA412A78B0
// Assembly location: E:\SteamLibrary\steamapps\common\Mount & Blade II Bannerlord\bin\Win64_Shipping_Client\TaleWorlds.MountAndBlade.dll

using System;
using TaleWorlds.Core;
using TaleWorlds.Engine;

namespace TaleWorlds.MountAndBlade
{
  public class FormationDeploymentPlan
  {
    public readonly FormationClass Class;

    public float PlannedWidth { get; private set; }

    public float PlannedDepth { get; private set; }

    public int PlannedTroopCount { get; private set; }

    public WorldFrame? SpawnFrame { get; private set; }

    public WorldFrame? ReinforcementFrame { get; private set; }

    public bool HasDimensions => (double) this.PlannedWidth >= 9.99999974737875E-06 && (double) this.PlannedDepth >= 9.99999974737875E-06;

    public FormationDeploymentPlan(FormationClass fClass)
    {
      this.Class = fClass;
      this.Clear();
    }

    public FormationDeploymentFlank GetDefaultFlank(bool spawnWithHorses)
    {
      FormationDeploymentFlank formationDeploymentFlank = FormationDeploymentFlank.Count;
      if (!spawnWithHorses && this.Class.IsMounted())
      {
        if (this.Class == FormationClass.HeavyCavalry || this.Class == FormationClass.Cavalry)
          formationDeploymentFlank = FormationDeploymentFlank.Front;
        else if (this.Class == FormationClass.LightCavalry || this.Class == FormationClass.HorseArcher)
          formationDeploymentFlank = FormationDeploymentFlank.Rear;
      }
      else
      {
        switch (this.Class)
        {
          case FormationClass.Ranged:
          case FormationClass.NumberOfRegularFormations:
          case FormationClass.Bodyguard:
          case FormationClass.NumberOfAllFormations:
            formationDeploymentFlank = FormationDeploymentFlank.Rear;
            break;
          case FormationClass.Cavalry:
          case FormationClass.HeavyCavalry:
            formationDeploymentFlank = FormationDeploymentFlank.Left;
            break;
          case FormationClass.HorseArcher:
          case FormationClass.LightCavalry:
            formationDeploymentFlank = FormationDeploymentFlank.Right;
            break;
          default:
            formationDeploymentFlank = FormationDeploymentFlank.Front;
            break;
        }
      }
      return formationDeploymentFlank;
    }

    public FormationDeploymentOrder GetDeploymentOrder(int offset = 0) => FormationDeploymentOrder.GetDeploymentOrder(this.Class, offset);

    public bool HasFrame(bool isReinforcement = false) => isReinforcement ? this.ReinforcementFrame.HasValue && this.ReinforcementFrame.Value.Origin.IsValid : this.SpawnFrame.HasValue && this.SpawnFrame.Value.Origin.IsValid;

    public WorldFrame GetFrame(bool isReinforcement = false)
    {
      WorldFrame? nullable = isReinforcement ? this.ReinforcementFrame : this.SpawnFrame;
      return nullable.HasValue ? nullable.Value : Mission.Current.GetSceneMiddleFrame();
    }

    internal void Clear()
    {
      this.PlannedWidth = 0.0f;
      this.PlannedDepth = 0.0f;
      this.PlannedTroopCount = 0;
      this.SpawnFrame = new WorldFrame?(WorldFrame.Invalid);
      this.ReinforcementFrame = new WorldFrame?(WorldFrame.Invalid);
    }

    internal void SetPlannedTroopCount(int value) => this.PlannedTroopCount = value;

    internal void SetPlannedDimensions(float width, float depth)
    {
      this.PlannedWidth = Math.Max(0.0f, width);
      this.PlannedDepth = Math.Max(0.0f, depth);
    }

    internal void SetFrame(WorldFrame? frame, bool isReinforcement)
    {
      if (isReinforcement)
        this.ReinforcementFrame = frame;
      else
        this.SpawnFrame = frame;
    }
  }
}
