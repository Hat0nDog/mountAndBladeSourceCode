// Decompiled with JetBrains decompiler
// Type: TaleWorlds.MountAndBlade.FormationAgentComponent
// Assembly: TaleWorlds.MountAndBlade, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3C0E3F42-756F-4407-B817-06BA412A78B0
// Assembly location: E:\SteamLibrary\steamapps\common\Mount & Blade II Bannerlord\bin\Win64_Shipping_Client\TaleWorlds.MountAndBlade.dll

using TaleWorlds.Core;
using TaleWorlds.Library;

namespace TaleWorlds.MountAndBlade
{
  public class FormationAgentComponent : AgentComponent
  {
    private Vec2 _incoherenceDeltaPosition;

    public FormationAgentComponent(Agent agent)
      : base(agent)
    {
    }

    protected internal override void Initialize() => this._incoherenceDeltaPosition = new Vec2((float) (0.400000005960464 * ((double) MBRandom.RandomFloat - 0.5)), (float) (0.400000005960464 * ((double) MBRandom.RandomFloat - 0.5)));

    protected internal override void OnDisciplineChanged()
    {
    }
  }
}
