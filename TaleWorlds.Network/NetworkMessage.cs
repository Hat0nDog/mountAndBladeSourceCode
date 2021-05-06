// Decompiled with JetBrains decompiler
// Type: TaleWorlds.Network.NetworkMessage
// Assembly: TaleWorlds.Network, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: EDF757B2-6540-4679-910A-3C5D58B2EF82
// Assembly location: E:\SteamLibrary\steamapps\common\Mount & Blade II Bannerlord\bin\Win64_Shipping_Client\TaleWorlds.Network.dll

using System;
using System.Text;

namespace TaleWorlds.Network
{
  public class NetworkMessage : INetworkMessageWriter, INetworkMessageReader
  {
    private int _readCursor;
    private int _writeCursor;
    private bool _finalized;

    private byte[] Buffer => this.MessageBuffer.Buffer;

    internal MessageBuffer MessageBuffer { get; private set; }

    internal int DataLength
    {
      get => this.MessageBuffer.DataLength - 4;
      set => this.MessageBuffer.DataLength = value + 4;
    }

    private NetworkMessage()
    {
      this._writeCursor = 4;
      this._readCursor = 4;
      this._finalized = false;
    }

    internal static NetworkMessage CreateForReading(MessageBuffer messageBuffer) => new NetworkMessage()
    {
      MessageBuffer = messageBuffer
    };

    internal static NetworkMessage CreateForWriting()
    {
      NetworkMessage networkMessage = new NetworkMessage();
      networkMessage.MessageBuffer = new MessageBuffer(new byte[16777216]);
      networkMessage.Reset();
      return networkMessage;
    }

    public void Write(string data)
    {
      int writeCursor = this._writeCursor;
      this._writeCursor += 4;
      int num;
      if (data != null)
      {
        num = Encoding.UTF8.GetBytes(data, 0, data.Length, this.Buffer, this._writeCursor);
        this._writeCursor += num;
      }
      else
        num = -1;
      byte[] bytes = BitConverter.GetBytes(num);
      this.Buffer[writeCursor] = bytes[0];
      this.Buffer[writeCursor + 1] = bytes[1];
      this.Buffer[writeCursor + 2] = bytes[2];
      this.Buffer[writeCursor + 3] = bytes[3];
    }

    public void Write(int data)
    {
      byte[] bytes = BitConverter.GetBytes(data);
      this.Buffer[this._writeCursor] = bytes[0];
      this.Buffer[this._writeCursor + 1] = bytes[1];
      this.Buffer[this._writeCursor + 2] = bytes[2];
      this.Buffer[this._writeCursor + 3] = bytes[3];
      this._writeCursor += 4;
    }

    public void Write(short data)
    {
      byte[] bytes = BitConverter.GetBytes(data);
      this.Buffer[this._writeCursor] = bytes[0];
      this.Buffer[this._writeCursor + 1] = bytes[1];
      this._writeCursor += 2;
    }

    public void Write(bool data)
    {
      this.Buffer[this._writeCursor] = BitConverter.GetBytes(data)[0];
      ++this._writeCursor;
    }

    public void Write(byte data)
    {
      this.Buffer[this._writeCursor] = data;
      ++this._writeCursor;
    }

    public void Write(float data)
    {
      byte[] bytes = BitConverter.GetBytes(data);
      this.Buffer[this._writeCursor] = bytes[0];
      this.Buffer[this._writeCursor + 1] = bytes[1];
      this.Buffer[this._writeCursor + 2] = bytes[2];
      this.Buffer[this._writeCursor + 3] = bytes[3];
      this._writeCursor += 4;
    }

    public void Write(long data)
    {
      byte[] bytes = BitConverter.GetBytes(data);
      this.Buffer[this._writeCursor] = bytes[0];
      this.Buffer[this._writeCursor + 1] = bytes[1];
      this.Buffer[this._writeCursor + 2] = bytes[2];
      this.Buffer[this._writeCursor + 3] = bytes[3];
      this.Buffer[this._writeCursor + 4] = bytes[4];
      this.Buffer[this._writeCursor + 5] = bytes[5];
      this.Buffer[this._writeCursor + 6] = bytes[6];
      this.Buffer[this._writeCursor + 7] = bytes[7];
      this._writeCursor += 8;
    }

    public void Write(ulong data)
    {
      byte[] bytes = BitConverter.GetBytes(data);
      this.Buffer[this._writeCursor] = bytes[0];
      this.Buffer[this._writeCursor + 1] = bytes[1];
      this.Buffer[this._writeCursor + 2] = bytes[2];
      this.Buffer[this._writeCursor + 3] = bytes[3];
      this.Buffer[this._writeCursor + 4] = bytes[4];
      this.Buffer[this._writeCursor + 5] = bytes[5];
      this.Buffer[this._writeCursor + 6] = bytes[6];
      this.Buffer[this._writeCursor + 7] = bytes[7];
      this._writeCursor += 8;
    }

    public void Write(Guid data)
    {
      byte[] byteArray = data.ToByteArray();
      for (int index = 0; index < byteArray.Length; ++index)
        this.Buffer[this._writeCursor + index] = byteArray[index];
      this._writeCursor += byteArray.Length;
    }

    public void Write(byte[] data)
    {
      this.Write(data.Length);
      for (int index = 0; index < data.Length; ++index)
        this.Buffer[this._writeCursor + index] = data[index];
      this._writeCursor += data.Length;
    }

    internal void Write(MessageContract messageContract)
    {
      this.Write(messageContract.MessageId);
      messageContract.SerializeToNetworkMessage((INetworkMessageWriter) this);
    }

    public int ReadInt32()
    {
      int int32 = BitConverter.ToInt32(this.Buffer, this._readCursor);
      this._readCursor += 4;
      return int32;
    }

    public short ReadInt16()
    {
      int int16 = (int) BitConverter.ToInt16(this.Buffer, this._readCursor);
      this._readCursor += 2;
      return (short) int16;
    }

    public bool ReadBoolean()
    {
      int num = BitConverter.ToBoolean(this.Buffer, this._readCursor) ? 1 : 0;
      ++this._readCursor;
      return num != 0;
    }

    public byte ReadByte()
    {
      int num = (int) this.Buffer[this._readCursor];
      ++this._readCursor;
      return (byte) num;
    }

    public string ReadString()
    {
      int count = this.ReadInt32();
      string str = (string) null;
      if (count >= 0)
      {
        str = Encoding.UTF8.GetString(this.Buffer, this._readCursor, count);
        this._readCursor += count;
      }
      return str;
    }

    public float ReadFloat()
    {
      double single = (double) BitConverter.ToSingle(this.Buffer, this._readCursor);
      this._readCursor += 4;
      return (float) single;
    }

    public long ReadInt64()
    {
      long int64 = BitConverter.ToInt64(this.Buffer, this._readCursor);
      this._readCursor += 8;
      return int64;
    }

    public ulong ReadUInt64()
    {
      long uint64 = (long) BitConverter.ToUInt64(this.Buffer, this._readCursor);
      this._readCursor += 8;
      return (ulong) uint64;
    }

    public Guid ReadGuid()
    {
      byte[] b = new byte[16];
      for (int index = 0; index < b.Length; ++index)
        b[index] = this.Buffer[this._readCursor + index];
      this._readCursor += b.Length;
      return new Guid(b);
    }

    public byte[] ReadByteArray()
    {
      byte[] numArray = new byte[this.ReadInt32()];
      for (int index = 0; index < numArray.Length; ++index)
        numArray[index] = this.Buffer[this._readCursor + index];
      this._readCursor += numArray.Length;
      return numArray;
    }

    internal void Reset()
    {
      this._writeCursor = 4;
      this._readCursor = 4;
      this._finalized = false;
      this.DataLength = 0;
    }

    internal void ResetRead() => this._readCursor = 4;

    internal void BeginRead() => this._readCursor = 4;

    internal void BeginWrite()
    {
      this._writeCursor = 4;
      this._finalized = false;
    }

    internal void FinalizeWrite()
    {
      if (this._finalized)
        return;
      this._finalized = true;
      this.DataLength = this._writeCursor - 4;
    }

    internal void UpdateHeader()
    {
      this.Buffer[0] = (byte) (this.DataLength & (int) byte.MaxValue);
      this.Buffer[1] = (byte) (this.DataLength >> 8 & (int) byte.MaxValue);
      this.Buffer[2] = (byte) (this.DataLength >> 16 & (int) byte.MaxValue);
      this.Buffer[3] = (byte) (this.DataLength >> 24 & (int) byte.MaxValue);
    }

    internal string GetDebugText() => BitConverter.ToString(this.Buffer, 0, this.DataLength);
  }
}
