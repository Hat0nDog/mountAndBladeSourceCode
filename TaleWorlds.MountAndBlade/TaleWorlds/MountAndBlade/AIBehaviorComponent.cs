// Decompiled with JetBrains decompiler
// Type: TaleWorlds.MountAndBlade.AIBehaviorComponent
// Assembly: TaleWorlds.MountAndBlade, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3C0E3F42-756F-4407-B817-06BA412A78B0
// Assembly location: E:\SteamLibrary\steamapps\common\Mount & Blade II Bannerlord\bin\Win64_Shipping_Client\TaleWorlds.MountAndBlade.dll

namespace TaleWorlds.MountAndBlade
{
  public class AIBehaviorComponent : AgentComponent
  {
    private BehaviorValues[] behaviorValues;
    private bool hasNewValues;

    public AIBehaviorComponent(Agent agent)
      : base(agent)
    {
      this.behaviorValues = new BehaviorValues[7];
      this.SetDefaults();
      this.Agent.SetAllBehaviorParams(this.behaviorValues);
      this.hasNewValues = false;
    }

    private void SetDefaults()
    {
      this.Set(AISimpleBehaviorKind.GoToPos, 3f, 7f, 5f, 20f, 6f);
      this.Set(AISimpleBehaviorKind.Melee, 8f, 7f, 4f, 20f, 1f);
      this.Set(AISimpleBehaviorKind.Ranged, 2f, 7f, 4f, 20f, 5f);
      this.Set(AISimpleBehaviorKind.ChargeHorseback, 2f, 25f, 5f, 30f, 5f);
      this.Set(AISimpleBehaviorKind.RangedHorseback, 2f, 15f, 6.5f, 30f, 5.5f);
      this.Set(AISimpleBehaviorKind.AttackEntityMelee, 5f, 12f, 7.5f, 30f, 4f);
      this.Set(AISimpleBehaviorKind.AttackEntityRanged, 5.5f, 12f, 8f, 30f, 4.5f);
    }

    public void Set(
      AISimpleBehaviorKind behavior,
      float y1,
      float x2,
      float y2,
      float x3,
      float y3)
    {
      this.behaviorValues[(int) behavior].y1 = y1;
      this.behaviorValues[(int) behavior].x2 = x2;
      this.behaviorValues[(int) behavior].y2 = y2;
      this.behaviorValues[(int) behavior].x3 = x3;
      this.behaviorValues[(int) behavior].y3 = y3;
      this.hasNewValues = true;
    }

    protected internal override void OnTickAsAI(float dt)
    {
      if (!this.hasNewValues)
        return;
      this.Agent.SetAllBehaviorParams(this.behaviorValues);
      this.hasNewValues = false;
    }
  }
}
