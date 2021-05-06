// Decompiled with JetBrains decompiler
// Type: TaleWorlds.Core.VirtualPlayer
// Assembly: TaleWorlds.Core, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 43F7B45C-9AE1-4715-A37B-2364840F62AF
// Assembly location: E:\SteamLibrary\steamapps\common\Mount & Blade II Bannerlord\bin\Win64_Shipping_Client\TaleWorlds.Core.dll

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using TaleWorlds.Library;
using TaleWorlds.PlayerServices;

namespace TaleWorlds.Core
{
  public class VirtualPlayer
  {
    private static Dictionary<Type, object> _peerComponents = new Dictionary<Type, object>();
    private static Dictionary<Type, uint> _peerComponentIds;
    private static Dictionary<uint, Type> _peerComponentTypes;
    public readonly ICommunicator Communicator;
    private EntitySystem<PeerComponent> _peerEntitySystem;
    public readonly int Index;

    public static event Action<VirtualPlayer, PeerComponent> OnPeerComponentPreRemoved;

    static VirtualPlayer() => VirtualPlayer.FindPeerComponents();

    private static void FindPeerComponents()
    {
      Debug.Print("Searching Peer Components");
      VirtualPlayer._peerComponentIds = new Dictionary<Type, uint>();
      VirtualPlayer._peerComponentTypes = new Dictionary<uint, Type>();
      Assembly[] assemblies = AppDomain.CurrentDomain.GetAssemblies();
      List<Type> typeList = new List<Type>();
      foreach (Assembly assembly in assemblies)
      {
        if (VirtualPlayer.CheckAssemblyForPeerComponent(assembly))
        {
          Type[] types = assembly.GetTypes();
          typeList.AddRange(((IEnumerable<Type>) types).Where<Type>((Func<Type, bool>) (q => typeof (PeerComponent).IsAssignableFrom(q) && typeof (PeerComponent) != q)));
        }
      }
      foreach (Type key in typeList)
      {
        uint djB2 = (uint) Common.GetDJB2(key.Name);
        VirtualPlayer._peerComponentIds.Add(key, djB2);
        VirtualPlayer._peerComponentTypes.Add(djB2, key);
      }
      Debug.Print("Found " + (object) typeList.Count + " peer components");
    }

    private static bool CheckAssemblyForPeerComponent(Assembly assembly)
    {
      Assembly assembly1 = Assembly.GetAssembly(typeof (PeerComponent));
      if (assembly == assembly1)
        return true;
      foreach (AssemblyName referencedAssembly in assembly.GetReferencedAssemblies())
      {
        if (referencedAssembly.FullName == assembly1.FullName)
          return true;
      }
      return false;
    }

    private static void EnsurePeerTypeList<T>() where T : PeerComponent
    {
      if (VirtualPlayer._peerComponents.ContainsKey(typeof (T)))
        return;
      VirtualPlayer._peerComponents.Add(typeof (T), (object) new List<T>());
    }

    private static void EnsurePeerTypeList(Type type)
    {
      if (VirtualPlayer._peerComponents.ContainsKey(type))
        return;
      IList instance = Activator.CreateInstance(typeof (List<>).MakeGenericType(type)) as IList;
      VirtualPlayer._peerComponents.Add(type, (object) instance);
    }

    public static List<T> Peers<T>() where T : PeerComponent
    {
      VirtualPlayer.EnsurePeerTypeList<T>();
      return VirtualPlayer._peerComponents[typeof (T)] as List<T>;
    }

    public static void Reset() => VirtualPlayer._peerComponents = new Dictionary<Type, object>();

    public string BannerCode { get; set; }

    public BodyProperties BodyProperties { get; set; }

    public bool IsFemale { get; set; }

    public PlayerId Id { get; set; }

    public bool IsMine => MBNetwork.MyPeer == this;

    public string UserName { get; private set; }

    public int ChosenBadgeIndex { get; set; }

    public VirtualPlayer(int index, string name, ICommunicator communicator)
    {
      this._peerEntitySystem = new EntitySystem<PeerComponent>();
      this.UserName = name;
      this.Index = index;
      this.Communicator = communicator;
    }

    public T AddComponent<T>() where T : PeerComponent, new()
    {
      T obj = this._peerEntitySystem.AddComponent<T>();
      obj.Peer = this;
      obj.TypeId = VirtualPlayer._peerComponentIds[typeof (T)];
      VirtualPlayer.EnsurePeerTypeList<T>();
      (VirtualPlayer._peerComponents[typeof (T)] as List<T>).Add(obj);
      this.Communicator.OnAddComponent((PeerComponent) obj);
      obj.Initialize();
      return obj;
    }

    public PeerComponent AddComponent(Type peerComponentType)
    {
      PeerComponent component = this._peerEntitySystem.AddComponent(peerComponentType);
      component.Peer = this;
      component.TypeId = VirtualPlayer._peerComponentIds[peerComponentType];
      VirtualPlayer.EnsurePeerTypeList(peerComponentType);
      (VirtualPlayer._peerComponents[peerComponentType] as IList).Add((object) component);
      this.Communicator.OnAddComponent(component);
      component.Initialize();
      return component;
    }

    public PeerComponent AddComponent(uint componentId) => this.AddComponent(VirtualPlayer._peerComponentTypes[componentId]);

    public PeerComponent GetComponent(uint componentId) => this.GetComponent(VirtualPlayer._peerComponentTypes[componentId]);

    public T GetComponent<T>() where T : PeerComponent => this._peerEntitySystem.GetComponent<T>();

    public PeerComponent GetComponent(Type peerComponentType) => this._peerEntitySystem.GetComponent(peerComponentType);

    public void RemoveComponent<T>(bool synched = true) where T : PeerComponent
    {
      T component = this._peerEntitySystem.GetComponent<T>();
      if ((object) component == null)
        return;
      if (VirtualPlayer.OnPeerComponentPreRemoved != null)
        VirtualPlayer.OnPeerComponentPreRemoved(this, (PeerComponent) component);
      this._peerEntitySystem.RemoveComponent((PeerComponent) component);
      (VirtualPlayer._peerComponents[typeof (T)] as List<T>).Remove(component);
      if (!synched)
        return;
      this.Communicator.OnRemoveComponent((PeerComponent) component);
    }

    public void RemoveComponent(PeerComponent component)
    {
      if (VirtualPlayer.OnPeerComponentPreRemoved != null)
        VirtualPlayer.OnPeerComponentPreRemoved(this, component);
      this._peerEntitySystem.RemoveComponent(component);
      (VirtualPlayer._peerComponents[component.GetType()] as IList).Remove((object) component);
      this.Communicator.OnRemoveComponent(component);
    }

    public void OnDisconnect()
    {
      foreach (PeerComponent component in (IEnumerable<PeerComponent>) this._peerEntitySystem.Components)
      {
        IList peerComponent = VirtualPlayer._peerComponents[component.GetType()] as IList;
        if (peerComponent.Contains((object) component))
          peerComponent.Remove((object) component);
      }
    }

    public void SynchronizeComponentsTo(VirtualPlayer peer)
    {
      foreach (PeerComponent component in (IEnumerable<PeerComponent>) this._peerEntitySystem.Components)
        this.Communicator.OnSynchronizeComponentTo(peer, component);
    }
  }
}
