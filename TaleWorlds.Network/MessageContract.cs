// Decompiled with JetBrains decompiler
// Type: TaleWorlds.Network.MessageContract
// Assembly: TaleWorlds.Network, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: EDF757B2-6540-4679-910A-3C5D58B2EF82
// Assembly location: E:\SteamLibrary\steamapps\common\Mount & Blade II Bannerlord\bin\Win64_Shipping_Client\TaleWorlds.Network.dll

using System;
using System.Collections.Generic;

namespace TaleWorlds.Network
{
  public abstract class MessageContract
  {
    private Type _myType;

    private static Dictionary<Type, byte> MessageContracts { get; set; }

    private static Dictionary<Type, MessageContractCreator> MessageContractCreators { get; set; }

    public byte MessageId => MessageContract.MessageContracts[this._myType];

    static MessageContract()
    {
      MessageContract.MessageContracts = new Dictionary<Type, byte>();
      MessageContract.MessageContractCreators = new Dictionary<Type, MessageContractCreator>();
    }

    internal static byte GetContractId(Type type)
    {
      MessageContract.InitializeMessageContract(type);
      return MessageContract.MessageContracts[type];
    }

    internal static MessageContractCreator GetContractCreator(Type type)
    {
      MessageContract.InitializeMessageContract(type);
      return MessageContract.MessageContractCreators[type];
    }

    private static void InitializeMessageContract(Type type)
    {
      if (MessageContract.MessageContracts.ContainsKey(type))
        return;
      object[] customAttributes = type.GetCustomAttributes(typeof (TaleWorlds.Network.MessageId), true);
      if (customAttributes.Length != 1)
        return;
      TaleWorlds.Network.MessageId messageId = customAttributes[0] as TaleWorlds.Network.MessageId;
      lock (MessageContract.MessageContracts)
      {
        if (MessageContract.MessageContracts.ContainsKey(type))
          return;
        MessageContract.MessageContracts.Add(type, messageId.Id);
        MessageContractCreator instance = Activator.CreateInstance(typeof (MessageContractCreator<>).MakeGenericType(type)) as MessageContractCreator;
        MessageContract.MessageContractCreators.Add(type, instance);
      }
    }

    protected MessageContract()
    {
      this._myType = this.GetType();
      MessageContract.InitializeMessageContract(this._myType);
    }

    public static MessageContract CreateMessageContract(Type messageContractType)
    {
      MessageContract.InitializeMessageContract(messageContractType);
      return MessageContract.MessageContractCreators[messageContractType].Invoke();
    }

    public abstract void SerializeToNetworkMessage(INetworkMessageWriter networkMessage);

    public abstract void DeserializeFromNetworkMessage(INetworkMessageReader networkMessage);
  }
}
