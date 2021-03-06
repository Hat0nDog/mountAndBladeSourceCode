// Decompiled with JetBrains decompiler
// Type: NetworkMessages.FromClient.RequestToSpawnAsBot
// Assembly: TaleWorlds.MountAndBlade, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3C0E3F42-756F-4407-B817-06BA412A78B0
// Assembly location: E:\SteamLibrary\steamapps\common\Mount & Blade II Bannerlord\bin\Win64_Shipping_Client\TaleWorlds.MountAndBlade.dll

using TaleWorlds.MountAndBlade;
using TaleWorlds.MountAndBlade.Network.Messages;

namespace NetworkMessages.FromClient
{
  [DefineGameNetworkMessageType(GameNetworkMessageSendType.FromClient)]
  public sealed class RequestToSpawnAsBot : GameNetworkMessage
  {
    protected override bool OnRead() => true;

    protected override void OnWrite()
    {
    }

    protected override MultiplayerMessageFilter OnGetLogFilter() => MultiplayerMessageFilter.General | MultiplayerMessageFilter.AgentsDetailed;

    protected override string OnGetLogFormat() => "Request to spawn as a bot";
  }
}
