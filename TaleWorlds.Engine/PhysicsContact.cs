// Decompiled with JetBrains decompiler
// Type: TaleWorlds.Engine.PhysicsContact
// Assembly: TaleWorlds.Engine, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: DD7DC4E8-6EB6-4CB8-8DE1-FC78E45F7FE8
// Assembly location: E:\SteamLibrary\steamapps\common\Mount & Blade II Bannerlord\bin\Win64_Shipping_Client\TaleWorlds.Engine.dll

using System;
using TaleWorlds.DotNet;

namespace TaleWorlds.Engine
{
  [EngineStruct("rglPhysics_contact")]
  public struct PhysicsContact
  {
    public readonly PhysicsContactPair ContactPair0;
    public readonly PhysicsContactPair ContactPair1;
    public readonly PhysicsContactPair ContactPair2;
    public readonly PhysicsContactPair ContactPair3;
    public readonly PhysicsContactPair ContactPair4;
    public readonly PhysicsContactPair ContactPair5;
    public readonly PhysicsContactPair ContactPair6;
    public readonly PhysicsContactPair ContactPair7;
    public readonly PhysicsContactPair ContactPair8;
    public readonly PhysicsContactPair ContactPair9;
    public readonly PhysicsContactPair ContactPair10;
    public readonly PhysicsContactPair ContactPair11;
    public readonly PhysicsContactPair ContactPair12;
    public readonly PhysicsContactPair ContactPair13;
    public readonly PhysicsContactPair ContactPair14;
    public readonly PhysicsContactPair ContactPair15;
    public readonly IntPtr body0;
    public readonly IntPtr body1;
    public readonly int NumberOfContactPairs;

    public PhysicsContactPair this[int index]
    {
      get
      {
        switch (index)
        {
          case 0:
            return this.ContactPair0;
          case 1:
            return this.ContactPair1;
          case 2:
            return this.ContactPair2;
          case 3:
            return this.ContactPair3;
          case 4:
            return this.ContactPair4;
          case 5:
            return this.ContactPair5;
          case 6:
            return this.ContactPair6;
          case 7:
            return this.ContactPair7;
          case 8:
            return this.ContactPair8;
          case 9:
            return this.ContactPair9;
          case 10:
            return this.ContactPair10;
          case 11:
            return this.ContactPair11;
          case 12:
            return this.ContactPair12;
          case 13:
            return this.ContactPair13;
          case 14:
            return this.ContactPair14;
          case 15:
            return this.ContactPair15;
          default:
            return new PhysicsContactPair();
        }
      }
    }
  }
}
