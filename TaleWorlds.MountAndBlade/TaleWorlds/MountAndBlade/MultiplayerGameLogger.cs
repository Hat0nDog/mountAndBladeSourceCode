// Decompiled with JetBrains decompiler
// Type: TaleWorlds.MountAndBlade.MultiplayerGameLogger
// Assembly: TaleWorlds.MountAndBlade, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3C0E3F42-756F-4407-B817-06BA412A78B0
// Assembly location: E:\SteamLibrary\steamapps\common\Mount & Blade II Bannerlord\bin\Win64_Shipping_Client\TaleWorlds.MountAndBlade.dll

using NetworkMessages.FromClient;
using System.Collections.Generic;
using TaleWorlds.Core;
using TaleWorlds.MountAndBlade.Diamond;
using TaleWorlds.MountAndBlade.Network.Messages;

namespace TaleWorlds.MountAndBlade
{
  public class MultiplayerGameLogger : GameHandler
  {
    private ChatBox _chatBox;

    public List<GameLog> GameLogs { get; private set; }

    protected override void OnGameStart()
    {
    }

    public override void OnBeforeSave()
    {
    }

    public override void OnAfterSave()
    {
    }

    protected override void OnGameNetworkBegin()
    {
      this.GameLogs = new List<GameLog>();
      this._chatBox = Game.Current.GetGameHandler<ChatBox>();
      GameNetwork.NetworkMessageHandlerRegisterer handlerRegisterer = new GameNetwork.NetworkMessageHandlerRegisterer(GameNetwork.NetworkMessageHandlerRegisterer.RegisterMode.Add);
      if (!GameNetwork.IsServer)
        return;
      handlerRegisterer.Register<PlayerMessageAll>(new GameNetworkMessage.ClientMessageHandlerDelegate<PlayerMessageAll>(this.HandleClientEventPlayerMessageAll));
      handlerRegisterer.Register<PlayerMessageTeam>(new GameNetworkMessage.ClientMessageHandlerDelegate<PlayerMessageTeam>(this.HandleClientEventPlayerMessageTeam));
    }

    private bool HandleClientEventPlayerMessageAll(
      NetworkCommunicator networkPeer,
      PlayerMessageAll message)
    {
      this.GameLogs.Add(new GameLog(GameLogType.ChatMessage, networkPeer.VirtualPlayer.Id, MBCommon.GetTime(MBCommon.TimeType.Mission))
      {
        Data = {
          {
            "Message",
            message.Message
          },
          {
            "IsTeam",
            false.ToString()
          },
          {
            "IsMuted",
            this._chatBox?.IsPlayerMuted(networkPeer.VirtualPlayer.Id).ToString()
          },
          {
            "IsGlobalMuted",
            networkPeer.IsMuted.ToString()
          }
        }
      });
      return true;
    }

    private bool HandleClientEventPlayerMessageTeam(
      NetworkCommunicator networkPeer,
      PlayerMessageTeam message)
    {
      this.GameLogs.Add(new GameLog(GameLogType.ChatMessage, networkPeer.VirtualPlayer.Id, MBCommon.GetTime(MBCommon.TimeType.Mission))
      {
        Data = {
          {
            "Message",
            message.Message
          },
          {
            "IsTeam",
            true.ToString()
          },
          {
            "IsMuted",
            this._chatBox?.IsPlayerMuted(networkPeer.VirtualPlayer.Id).ToString()
          },
          {
            "IsGlobalMuted",
            networkPeer.IsMuted.ToString()
          }
        }
      });
      return true;
    }
  }
}
