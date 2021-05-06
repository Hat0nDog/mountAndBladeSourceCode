// Decompiled with JetBrains decompiler
// Type: TaleWorlds.MountAndBlade.PeerVisualsHolder
// Assembly: TaleWorlds.MountAndBlade, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3C0E3F42-756F-4407-B817-06BA412A78B0
// Assembly location: E:\SteamLibrary\steamapps\common\Mount & Blade II Bannerlord\bin\Win64_Shipping_Client\TaleWorlds.MountAndBlade.dll

namespace TaleWorlds.MountAndBlade
{
  public class PeerVisualsHolder
  {
    public MissionPeer Peer { get; private set; }

    public int VisualsIndex { get; private set; }

    public IAgentVisual AgentVisuals { get; private set; }

    public IAgentVisual MountAgentVisuals { get; private set; }

    public PeerVisualsHolder(
      MissionPeer peer,
      int index,
      IAgentVisual agentVisuals,
      IAgentVisual mountVisuals)
    {
      this.Peer = peer;
      this.VisualsIndex = index;
      this.AgentVisuals = agentVisuals;
      this.MountAgentVisuals = mountVisuals;
    }

    public void SetMountVisuals(IAgentVisual mountAgentVisuals) => this.MountAgentVisuals = mountAgentVisuals;
  }
}
