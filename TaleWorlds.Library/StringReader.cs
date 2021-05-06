// Decompiled with JetBrains decompiler
// Type: TaleWorlds.Library.StringReader
// Assembly: TaleWorlds.Library, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 271D0DB8-3C5D-4850-A565-542B4CF7D347
// Assembly location: E:\SteamLibrary\steamapps\common\Mount & Blade II Bannerlord\bin\Win64_Shipping_Client\TaleWorlds.Library.dll

using System;
using System.Globalization;

namespace TaleWorlds.Library
{
  public class StringReader : IReader
  {
    private string[] _tokens;
    private int _currentIndex;

    public string Data { get; private set; }

    private string GetNextToken()
    {
      string token = this._tokens[this._currentIndex];
      ++this._currentIndex;
      return token;
    }

    public StringReader(string data)
    {
      this.Data = data;
      this._tokens = data.Split(new char[1]{ ' ' }, StringSplitOptions.RemoveEmptyEntries);
    }

    public ISerializableObject ReadSerializableObject() => throw new NotImplementedException();

    public int ReadInt() => Convert.ToInt32(this.GetNextToken());

    public short ReadShort() => Convert.ToInt16(this.GetNextToken());

    public string ReadString()
    {
      int num1 = this.ReadInt();
      int num2 = 0;
      string str = "";
      while (num2 < num1)
      {
        string nextToken = this.GetNextToken();
        str += nextToken;
        num2 = str.Length;
        if (num2 < num1)
          str += " ";
      }
      if (str.Length != num1)
        throw new Exception("invalid string format, length does not match");
      return str;
    }

    public Color ReadColor() => new Color(this.ReadFloat(), this.ReadFloat(), this.ReadFloat(), this.ReadFloat());

    public bool ReadBool()
    {
      string nextToken = this.GetNextToken();
      if (nextToken == "1")
        return true;
      return !(nextToken == "0") && Convert.ToBoolean(nextToken);
    }

    public float ReadFloat() => Convert.ToSingle(this.GetNextToken(), (IFormatProvider) CultureInfo.InvariantCulture);

    public uint ReadUInt() => Convert.ToUInt32(this.GetNextToken());

    public ulong ReadULong() => Convert.ToUInt64(this.GetNextToken());

    public long ReadLong() => Convert.ToInt64(this.GetNextToken());

    public byte ReadByte() => Convert.ToByte(this.GetNextToken());

    public byte[] ReadBytes(int length) => throw new NotImplementedException();

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

    public sbyte ReadSByte() => Convert.ToSByte(this.GetNextToken());

    public ushort ReadUShort() => Convert.ToUInt16(this.GetNextToken());

    public double ReadDouble() => Convert.ToDouble(this.GetNextToken());
  }
}
