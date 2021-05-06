// Decompiled with JetBrains decompiler
// Type: TaleWorlds.Core.IAgent
// Assembly: TaleWorlds.Core, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 43F7B45C-9AE1-4715-A37B-2364840F62AF
// Assembly location: E:\SteamLibrary\steamapps\common\Mount & Blade II Bannerlord\bin\Win64_Shipping_Client\TaleWorlds.Core.dll

namespace TaleWorlds.Core
{
  public interface IAgent
  {
    BasicCharacterObject Character { get; }

    bool IsEnemyOf(IAgent agent);

    bool IsFriendOf(IAgent agent);

    AgentState State { get; }

    IMissionTeam Team { get; }

    IAgentOriginBase Origin { get; }

    float Age { get; }

    bool IsActive();

    void SetAsConversationAgent(bool set);
  }
}
