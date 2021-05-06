// Decompiled with JetBrains decompiler
// Type: TaleWorlds.Library.BinaryReader
// Assembly: TaleWorlds.Library, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 271D0DB8-3C5D-4850-A565-542B4CF7D347
// Assembly location: E:\SteamLibrary\steamapps\common\Mount & Blade II Bannerlord\bin\Win64_Shipping_Client\TaleWorlds.Library.dll

using System;
using System.Text;

namespace TaleWorlds.Library
{
  public class BinaryReader : IReader
  {
    private int _cursor;
    private byte[] _buffer;

    public byte[] Data { get; private set; }

    public BinaryReader(byte[] data)
    {
      this.Data = data;
      this._cursor = 0;
      this._buffer = new byte[4];
    }

    public int UnreadByteCount => this.Data.Length - this._cursor;

    public ISerializableObject ReadSerializableObject() => throw new NotImplementedException();

    public int Read3ByteInt()
    {
      this._buffer[0] = this.ReadByte();
      this._buffer[1] = this.ReadByte();
      this._buffer[2] = this.ReadByte();
      this._buffer[3] = this._buffer[0] != byte.MaxValue || this._buffer[1] != byte.MaxValue || this._buffer[2] != byte.MaxValue ? (byte) 0 : byte.MaxValue;
      return BitConverter.ToInt32(this._buffer, 0);
    }

    public int ReadInt()
    {
      int int32 = BitConverter.ToInt32(this.Data, this._cursor);
      this._cursor += 4;
      return int32;
    }

    public short ReadShort()
    {
      int int16 = (int) BitConverter.ToInt16(this.Data, this._cursor);
      this._cursor += 2;
      return (short) int16;
    }

    public void ReadFloats(float[] output, int count)
    {
      int count1 = count * 4;
      Buffer.BlockCopy((Array) this.Data, this._cursor, (Array) output, 0, count1);
      this._cursor += count1;
    }

    public void ReadShorts(short[] output, int count)
    {
      int count1 = count * 2;
      Buffer.BlockCopy((Array) this.Data, this._cursor, (Array) output, 0, count1);
      this._cursor += count1;
    }

    public string ReadString()
    {
      int count = this.ReadInt();
      string str = (string) null;
      if (count >= 0)
      {
        str = Encoding.UTF8.GetString(this.Data, this._cursor, count);
        this._cursor += count;
      }
      return str;
    }

    public Color ReadColor() => new Color(this.ReadFloat(), this.ReadFloat(), this.ReadFloat(), this.ReadFloat());

    public bool ReadBool()
    {
      int num = (int) this.Data[this._cursor];
      ++this._cursor;
      return num == 1;
    }

    public float ReadFloat()
    {
      double single = (double) BitConverter.ToSingle(this.Data, this._cursor);
      this._cursor += 4;
      return (float) single;
    }

    public uint ReadUInt()
    {
      int uint32 = (int) BitConverter.ToUInt32(this.Data, this._cursor);
      this._cursor += 4;
      return (uint) uint32;
    }

    public ulong ReadULong()
    {
      long uint64 = (long) BitConverter.ToUInt64(this.Data, this._cursor);
      this._cursor += 8;
      return (ulong) uint64;
    }

    public long ReadLong()
    {
      long int64 = BitConverter.ToInt64(this.Data, this._cursor);
      this._cursor += 8;
      return int64;
    }

    public byte ReadByte()
    {
      int num = (int) this.Data[this._cursor];
      ++this._cursor;
      return (byte) num;
    }

    public byte[] ReadBytes(int length)
    {
      byte[] numArray = new byte[length];
      Array.Copy((Array) this.Data, this._cursor, (Array) numArray, 0, length);
      this._cursor += length;
      return numArray;
    }

    public Vec2 ReadVec2() => new Vec2(this.ReadFloat(), this.ReadFloat());

    public Vec3 ReadVec3()
    {
      double num1 = (double) this.ReadFloat();
      float num2 = this.ReadFloat();
      float num3 = this.ReadFloat();
      float num4 = this.ReadFloat();
      double num5 = (double) num2;
      double num6 = (double) num3;
      double num7 = (double) num4;
      return new Vec3((float) num1, (float) num5, (float) num6, (float) num7);
    }

    public Vec3i ReadVec3Int()
    {
      int x = this.ReadInt();
      int num1 = this.ReadInt();
      int num2 = this.ReadInt();
      int y = num1;
      int z = num2;
      return new Vec3i(x, y, z);
    }

    public sbyte ReadSByte()
    {
      int num = (int) (sbyte) this.Data[this._cursor];
      ++this._cursor;
      return (sbyte) num;
    }

    public ushort ReadUShort()
    {
      int uint16 = (int) BitConverter.ToUInt16(this.Data, this._cursor);
      this._cursor += 2;
      return (ushort) uint16;
    }

    public double ReadDouble()
    {
      double num = BitConverter.ToDouble(this.Data, this._cursor);
      this._cursor += 8;
      return num;
    }
  }
}
