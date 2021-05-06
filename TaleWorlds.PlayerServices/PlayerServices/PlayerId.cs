// Decompiled with JetBrains decompiler
// Type: TaleWorlds.PlayerServices.PlayerId
// Assembly: TaleWorlds.PlayerServices, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: FC0579B2-CBA1-4D57-8C1B-BA25E3FC058C
// Assembly location: E:\SteamLibrary\steamapps\common\Mount & Blade II Bannerlord\bin\Win64_Shipping_Client\TaleWorlds.PlayerServices.dll

using System;
using TaleWorlds.Library;

namespace TaleWorlds.PlayerServices
{
  [Serializable]
  public struct PlayerId : IComparable<PlayerId>
  {
    private byte _providedType;
    private byte _reserved1;
    private byte _reserved2;
    private byte _reserved3;
    private byte _reserved4;
    private byte _reserved5;
    private byte _reserved6;
    private byte _reserved7;
    private ulong _reservedBig;
    private ulong _id1;
    private ulong _id2;

    public ulong Id1 => this._id1;

    public ulong Id2 => this._id2;

    public bool IsValid => this._id1 != 0UL || this._id2 > 0UL;

    public PlayerIdProvidedTypes ProvidedType => (PlayerIdProvidedTypes) this._providedType;

    public ulong Part1 => BitConverter.ToUInt64(new byte[8]
    {
      this._providedType,
      this._reserved1,
      this._reserved2,
      this._reserved3,
      this._reserved4,
      this._reserved5,
      this._reserved6,
      this._reserved7
    }, 0);

    public ulong Part2 => this._reservedBig;

    public ulong Part3 => this._id1;

    public ulong Part4 => this._id2;

    public static PlayerId Empty => new PlayerId((byte) 0, 0UL, 0UL, 0UL);

    public PlayerId(byte providedType, ulong id1, ulong id2)
    {
      this._providedType = providedType;
      this._reserved1 = (byte) 0;
      this._reserved2 = (byte) 0;
      this._reserved3 = (byte) 0;
      this._reserved4 = (byte) 0;
      this._reserved5 = (byte) 0;
      this._reserved6 = (byte) 0;
      this._reserved7 = (byte) 0;
      this._reservedBig = 0UL;
      this._id1 = id1;
      this._id2 = id2;
    }

    public PlayerId(byte providedType, ulong reservedBig, ulong id1, ulong id2)
    {
      this._providedType = providedType;
      this._reserved1 = (byte) 0;
      this._reserved2 = (byte) 0;
      this._reserved3 = (byte) 0;
      this._reserved4 = (byte) 0;
      this._reserved5 = (byte) 0;
      this._reserved6 = (byte) 0;
      this._reserved7 = (byte) 0;
      this._reservedBig = reservedBig;
      this._id1 = id1;
      this._id2 = id2;
    }

    public PlayerId(byte providedType, string guid)
    {
      byte[] byteArray = Guid.Parse(guid).ToByteArray();
      this._providedType = providedType;
      this._reserved1 = (byte) 0;
      this._reserved2 = (byte) 0;
      this._reserved3 = (byte) 0;
      this._reserved4 = (byte) 0;
      this._reserved5 = (byte) 0;
      this._reserved6 = (byte) 0;
      this._reserved7 = (byte) 0;
      this._reservedBig = 0UL;
      this._id1 = BitConverter.ToUInt64(byteArray, 0);
      this._id2 = BitConverter.ToUInt64(byteArray, 8);
    }

    public PlayerId(ulong part1, ulong part2, ulong part3, ulong part4)
    {
      byte[] bytes = BitConverter.GetBytes(part1);
      this._providedType = bytes[0];
      this._reserved1 = bytes[1];
      this._reserved2 = bytes[2];
      this._reserved3 = bytes[3];
      this._reserved4 = bytes[4];
      this._reserved5 = bytes[5];
      this._reserved6 = bytes[6];
      this._reserved7 = bytes[7];
      this._reservedBig = part2;
      this._id1 = part3;
      this._id2 = part4;
    }

    public PlayerId(byte[] data)
    {
      this._providedType = data[0];
      this._reserved1 = data[1];
      this._reserved2 = data[2];
      this._reserved3 = data[3];
      this._reserved4 = data[4];
      this._reserved5 = data[5];
      this._reserved6 = data[6];
      this._reserved7 = data[7];
      this._reservedBig = BitConverter.ToUInt64(data, 8);
      this._id1 = BitConverter.ToUInt64(data, 16);
      this._id2 = BitConverter.ToUInt64(data, 24);
    }

    public PlayerId(Guid guid)
    {
      byte[] byteArray = guid.ToByteArray();
      this._providedType = (byte) 0;
      this._reserved1 = (byte) 0;
      this._reserved2 = (byte) 0;
      this._reserved3 = (byte) 0;
      this._reserved4 = (byte) 0;
      this._reserved5 = (byte) 0;
      this._reserved6 = (byte) 0;
      this._reserved7 = (byte) 0;
      this._reservedBig = 0UL;
      this._id1 = BitConverter.ToUInt64(byteArray, 0);
      this._id2 = BitConverter.ToUInt64(byteArray, 8);
    }

    public byte[] ToByteArray()
    {
      byte[] numArray = new byte[32];
      numArray[0] = this._providedType;
      numArray[1] = this._reserved1;
      numArray[2] = this._reserved2;
      numArray[3] = this._reserved3;
      numArray[4] = this._reserved4;
      numArray[5] = this._reserved5;
      numArray[6] = this._reserved6;
      numArray[7] = this._reserved7;
      byte[] bytes1 = BitConverter.GetBytes(this._reservedBig);
      byte[] bytes2 = BitConverter.GetBytes(this._id1);
      byte[] bytes3 = BitConverter.GetBytes(this._id2);
      for (int index = 0; index < 8; ++index)
      {
        numArray[8 + index] = bytes1[index];
        numArray[16 + index] = bytes2[index];
        numArray[24 + index] = bytes3[index];
      }
      return numArray;
    }

    public void Serialize(IWriter writer)
    {
      writer.WriteULong(this.Part1);
      writer.WriteULong(this.Part2);
      writer.WriteULong(this.Part3);
      writer.WriteULong(this.Part4);
    }

    public void Deserialize(IReader reader)
    {
      long num1 = (long) reader.ReadULong();
      ulong num2 = reader.ReadULong();
      ulong num3 = reader.ReadULong();
      ulong num4 = reader.ReadULong();
      byte[] bytes = BitConverter.GetBytes((ulong) num1);
      this._providedType = bytes[0];
      this._reserved1 = bytes[1];
      this._reserved2 = bytes[2];
      this._reserved3 = bytes[3];
      this._reserved4 = bytes[4];
      this._reserved5 = bytes[5];
      this._reserved6 = bytes[6];
      this._reserved7 = bytes[7];
      this._reservedBig = num2;
      this._id1 = num3;
      this._id2 = num4;
    }

    public override string ToString() => this.Part1.ToString() + "." + (object) this.Part2 + "." + (object) this.Part3 + "." + (object) this.Part4;

    public static bool operator ==(PlayerId a, PlayerId b) => (long) a.Part1 == (long) b.Part1 && (long) a.Part2 == (long) b.Part2 && (long) a.Part3 == (long) b.Part3 && (long) a.Part4 == (long) b.Part4;

    public static bool operator !=(PlayerId a, PlayerId b) => (long) a.Part1 != (long) b.Part1 || (long) a.Part2 != (long) b.Part2 || (long) a.Part3 != (long) b.Part3 || (long) a.Part4 != (long) b.Part4;

    public override bool Equals(object o) => o != null && o is PlayerId playerId && this == playerId;

    public override int GetHashCode()
    {
      ulong num1 = this.Part1;
      int hashCode1 = num1.GetHashCode();
      num1 = this.Part2;
      int hashCode2 = num1.GetHashCode();
      num1 = this.Part3;
      int hashCode3 = num1.GetHashCode();
      num1 = this.Part4;
      int hashCode4 = num1.GetHashCode();
      int num2 = hashCode2;
      return hashCode1 ^ num2 ^ hashCode3 ^ hashCode4;
    }

    public static PlayerId FromString(string id)
    {
      if (string.IsNullOrEmpty(id))
        return new PlayerId();
      string[] strArray = id.Split('.');
      return new PlayerId(Convert.ToUInt64(strArray[0]), Convert.ToUInt64(strArray[1]), Convert.ToUInt64(strArray[2]), Convert.ToUInt64(strArray[3]));
    }

    public int CompareTo(PlayerId other)
    {
      if ((long) this.Part1 != (long) other.Part1)
        return this.Part1.CompareTo(other.Part1);
      if ((long) this.Part2 != (long) other.Part2)
        return this.Part2.CompareTo(other.Part2);
      return (long) this.Part3 != (long) other.Part3 ? this.Part3.CompareTo(other.Part3) : this.Part4.CompareTo(other.Part4);
    }
  }
}
