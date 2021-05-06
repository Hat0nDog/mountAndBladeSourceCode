// Decompiled with JetBrains decompiler
// Type: TaleWorlds.Network.MessageContractHandlerManager
// Assembly: TaleWorlds.Network, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: EDF757B2-6540-4679-910A-3C5D58B2EF82
// Assembly location: E:\SteamLibrary\steamapps\common\Mount & Blade II Bannerlord\bin\Win64_Shipping_Client\TaleWorlds.Network.dll

using System;
using System.Collections.Generic;
using TaleWorlds.Library;

namespace TaleWorlds.Network
{
  public class MessageContractHandlerManager
  {
    private Dictionary<byte, MessageContractHandler> MessageHandlers { get; set; }

    private Dictionary<byte, Type> MessageContractTypes { get; set; }

    public MessageContractHandlerManager()
    {
      this.MessageHandlers = new Dictionary<byte, MessageContractHandler>();
      this.MessageContractTypes = new Dictionary<byte, Type>();
    }

    public void AddMessageHandler<T>(MessageContractHandlerDelegate<T> handler) where T : MessageContract
    {
      MessageContractHandler<T> messageContractHandler = new MessageContractHandler<T>(handler);
      Type type = typeof (T);
      byte contractId = MessageContract.GetContractId(type);
      this.MessageContractTypes.Add(contractId, type);
      this.MessageHandlers.Add(contractId, (MessageContractHandler) messageContractHandler);
    }

    public void HandleMessage(MessageContract messageContract) => this.MessageHandlers[messageContract.MessageId].Invoke(messageContract);

    public void HandleNetworkMessage(NetworkMessage networkMessage)
    {
      byte key = networkMessage.ReadByte();
      Type messageContractType = this.MessageContractTypes[key];
      MessageContract messageContract = MessageContract.CreateMessageContract(messageContractType);
      Debug.Print("Message with id: " + (object) key + " / contract:" + (object) messageContractType + "received and processing...");
      messageContract.DeserializeFromNetworkMessage((INetworkMessageReader) networkMessage);
      this.HandleMessage(messageContract);
    }

    internal Type GetMessageContractType(byte id) => this.MessageContractTypes[id];

    public bool ContainsMessageHandler(byte id) => this.MessageContractTypes.ContainsKey(id);
  }
}
