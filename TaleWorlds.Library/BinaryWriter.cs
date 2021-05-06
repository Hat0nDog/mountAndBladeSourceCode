// Decompiled with JetBrains decompiler
// Type: TaleWorlds.Library.BinaryWriter
// Assembly: TaleWorlds.Library, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 271D0DB8-3C5D-4850-A565-542B4CF7D347
// Assembly location: E:\SteamLibrary\steamapps\common\Mount & Blade II Bannerlord\bin\Win64_Shipping_Client\TaleWorlds.Library.dll

using System;
using System.Text;

namespace TaleWorlds.Library
{
  public class BinaryWriter : IWriter
  {
    private byte[] _data;
    private int _availableIndex;

    public byte[] Data
    {
      get
      {
        byte[] numArray = new byte[this._availableIndex];
        Buffer.BlockCopy((Array) this._data, 0, (Array) numArray, 0, this._availableIndex);
        return numArray;
      }
    }

    public int Length => this._availableIndex;

    public BinaryWriter()
    {
      this._data = new byte[4096];
      this._availableIndex = 0;
    }

    public BinaryWriter(int capacity)
    {
      this._data = new byte[capacity];
      this._availableIndex = 0;
    }

    public void Clear() => this._availableIndex = 0;

    public void EnsureLength(int added)
    {
      int num = this._availableIndex + added;
      if (num <= this._data.Length)
        return;
      int length = this._data.Length * 2;
      if (num > length)
        length = num;
      byte[] numArray = new byte[length];
      Buffer.BlockCopy((Array) this._data, 0, (Array) numArray, 0, this._availableIndex);
      this._data = numArray;
    }

    public void WriteSerializableObject(ISerializableObject serializableObject) => throw new NotImplementedException();

    public void WriteByte(byte value)
    {
      this.EnsureLength(1);
      this._data[this._availableIndex] = value;
      ++this._availableIndex;
    }

    public void WriteBytes(byte[] bytes)
    {
      this.EnsureLength(bytes.Length);
      Buffer.BlockCopy((Array) bytes, 0, (Array) this._data, this._availableIndex, bytes.Length);
      this._availableIndex += bytes.Length;
    }

    public void Write3ByteInt(int value)
    {
      this.EnsureLength(3);
      this._data[this._availableIndex++] = (byte) value;
      this._data[this._availableIndex++] = (byte) (value >> 8);
      this._data[this._availableIndex++] = (byte) (value >> 16);
    }

    public void WriteInt(int value)
    {
      this.EnsureLength(4);
      this._data[this._availableIndex++] = (byte) value;
      this._data[this._availableIndex++] = (byte) (value >> 8);
      this._data[this._availableIndex++] = (byte) (value >> 16);
      this._data[this._availableIndex++] = (byte) (value >> 24);
    }

    public void WriteShort(short value)
    {
      this.EnsureLength(2);
      this._data[this._availableIndex++] = (byte) value;
      this._data[this._availableIndex++] = (byte) ((uint) value >> 8);
    }

    public void WriteString(string value)
    {
      if (!string.IsNullOrEmpty(value))
      {
        byte[] bytes = Encoding.UTF8.GetBytes(value);
        this.WriteInt(bytes.Length);
        this.WriteBytes(bytes);
      }
      else
        this.WriteInt(0);
    }

    public void WriteFloats(float[] value, int count)
    {
      int num = count * 4;
      this.EnsureLength(num);
      Buffer.BlockCopy((Array) value, 0, (Array) this._data, this._availableIndex, num);
      this._availableIndex += num;
    }

    public void WriteShorts(short[] value, int count)
    {
      int length = count * 2;
      this.EnsureLength(length);
      byte[] numArray = new byte[length];
      Buffer.BlockCopy((Array) value, 0, (Array) this._data, this._availableIndex, length);
      this._availableIndex += length;
    }

    public void WriteColor(Color value)
    {
      this.WriteFloat(value.Red);
      this.WriteFloat(value.Green);
      this.WriteFloat(value.Blue);
      this.WriteFloat(value.Alpha);
    }

    public void WriteBool(bool value)
    {
      this.EnsureLength(1);
      this._data[this._availableIndex] = value ? (byte) 1 : (byte) 0;
      ++this._availableIndex;
    }

    public void WriteFloat(float value)
    {
      byte[] bytes = BitConverter.GetBytes(value);
      this.EnsureLength(bytes.Length);
      Buffer.BlockCopy((Array) bytes, 0, (Array) this._data, this._availableIndex, bytes.Length);
      this._availableIndex += bytes.Length;
    }

    public void WriteUInt(uint value)
    {
      this.EnsureLength(4);
      this._data[this._availableIndex++] = (byte) value;
      this._data[this._availableIndex++] = (byte) (value >> 8);
      this._data[this._availableIndex++] = (byte) (value >> 16);
      this._data[this._availableIndex++] = (byte) (value >> 24);
    }

    public void WriteULong(ulong value)
    {
      this.EnsureLength(8);
      this._data[this._availableIndex++] = (byte) value;
      this._data[this._availableIndex++] = (byte) (value >> 8);
      this._data[this._availableIndex++] = (byte) (value >> 16);
      this._data[this._availableIndex++] = (byte) (value >> 24);
      this._data[this._availableIndex++] = (byte) (value >> 32);
      this._data[this._availableIndex++] = (byte) (value >> 40);
      this._data[this._availableIndex++] = (byte) (value >> 48);
      this._data[this._availableIndex++] = (byte) (value >> 56);
    }

    public void WriteLong(long value)
    {
      this.EnsureLength(8);
      this._data[this._availableIndex++] = (byte) value;
      this._data[this._availableIndex++] = (byte) (value >> 8);
      this._data[this._availableIndex++] = (byte) (value >> 16);
      this._data[this._availableIndex++] = (byte) (value >> 24);
      this._data[this._availableIndex++] = (byte) (value >> 32);
      this._data[this._availableIndex++] = (byte) (value >> 40);
      this._data[this._availableIndex++] = (byte) (value >> 48);
      this._data[this._availableIndex++] = (byte) (value >> 56);
    }

    public void WriteVec2(Vec2 vec2)
    {
      this.WriteFloat(vec2.x);
      this.WriteFloat(vec2.y);
    }

    public void WriteVec3(Vec3 vec3)
    {
      this.WriteFloat(vec3.x);
      this.WriteFloat(vec3.y);
      this.WriteFloat(vec3.z);
      this.WriteFloat(vec3.w);
    }

    public void WriteVec3Int(Vec3i vec3)
    {
      this.WriteInt(vec3.X);
      this.WriteInt(vec3.Y);
      this.WriteInt(vec3.Z);
    }

    public void WriteSByte(sbyte value)
    {
      this.EnsureLength(1);
      this._data[this._availableIndex] = (byte) value;
      ++this._availableIndex;
    }

    public void WriteUShort(ushort value)
    {
      this.EnsureLength(2);
      this._data[this._availableIndex++] = (byte) value;
      this._data[this._availableIndex++] = (byte) ((uint) value >> 8);
    }

    public void WriteDouble(double value)
    {
      byte[] bytes = BitConverter.GetBytes(value);
      this.EnsureLength(bytes.Length);
      Buffer.BlockCopy((Array) bytes, 0, (Array) this._data, this._availableIndex, bytes.Length);
      this._availableIndex += bytes.Length;
    }

    public void AppendData(BinaryWriter writer)
    {
      this.EnsureLength(writer._availableIndex);
      Buffer.BlockCopy((Array) writer._data, 0, (Array) this._data, this._availableIndex, writer._availableIndex);
      this._availableIndex += writer._availableIndex;
    }
  }
}
