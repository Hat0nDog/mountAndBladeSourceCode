// Decompiled with JetBrains decompiler
// Type: TaleWorlds.Library.StringWriter
// Assembly: TaleWorlds.Library, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 271D0DB8-3C5D-4850-A565-542B4CF7D347
// Assembly location: E:\SteamLibrary\steamapps\common\Mount & Blade II Bannerlord\bin\Win64_Shipping_Client\TaleWorlds.Library.dll

using System;
using System.Text;

namespace TaleWorlds.Library
{
  public class StringWriter : IWriter
  {
    private StringBuilder _stringBuilder;

    public string Data => this._stringBuilder.ToString();

    public StringWriter() => this._stringBuilder = new StringBuilder();

    private void AddToken(string token)
    {
      this._stringBuilder.Append(token);
      this._stringBuilder.Append(" ");
    }

    public void WriteSerializableObject(ISerializableObject serializableObject) => throw new NotImplementedException();

    public void WriteByte(byte value) => this.AddToken(Convert.ToString(value));

    public void WriteBytes(byte[] bytes) => throw new NotImplementedException();

    public void WriteInt(int value) => this.AddToken(Convert.ToString(value));

    public void WriteShort(short value) => this.AddToken(Convert.ToString(value));

    public void WriteString(string value)
    {
      this.WriteInt(value.Length);
      this.AddToken(value);
    }

    public void WriteColor(Color value)
    {
      this.WriteFloat(value.Red);
      this.WriteFloat(value.Green);
      this.WriteFloat(value.Blue);
      this.WriteFloat(value.Alpha);
    }

    public void WriteBool(bool value) => this.AddToken(value ? "1" : "0");

    public void WriteFloat(float value) => this.AddToken((double) value == 0.0 ? "0" : Convert.ToString(value));

    public void WriteUInt(uint value) => this.AddToken(Convert.ToString(value));

    public void WriteULong(ulong value) => this.AddToken(Convert.ToString(value));

    public void WriteLong(long value) => this.AddToken(Convert.ToString(value));

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

    public void WriteSByte(sbyte value) => this.AddToken(Convert.ToString(value));

    public void WriteUShort(ushort value) => this.AddToken(Convert.ToString(value));

    public void WriteDouble(double value) => this.AddToken(value == 0.0 ? "0" : Convert.ToString(value));
  }
}
