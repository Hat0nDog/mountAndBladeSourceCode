// Decompiled with JetBrains decompiler
// Type: TaleWorlds.Core.BasicCharacterObject
// Assembly: TaleWorlds.Core, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 43F7B45C-9AE1-4715-A37B-2364840F62AF
// Assembly location: E:\SteamLibrary\steamapps\common\Mount & Blade II Bannerlord\bin\Win64_Shipping_Client\TaleWorlds.Core.dll

using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml;
using TaleWorlds.Library;
using TaleWorlds.Localization;
using TaleWorlds.ObjectSystem;
using TaleWorlds.SaveSystem;

namespace TaleWorlds.Core
{
  [SaveableClass(10037)]
  public class BasicCharacterObject : MBObjectBase
  {
    private TextObject _basicName;
    private MBEquipmentRoster _equipmentRoster;
    private BasicCultureObject _culture;
    private float _age;
    private bool _isBasicHero;
    protected MBCharacterSkills CharacterSkills;

    public virtual TextObject Name
    {
      get => this._basicName;
      set => this.SetName(value);
    }

    private void SetName(TextObject name) => this._basicName = name;

    public override TextObject GetName() => this.Name;

    public override string ToString() => this.Name.ToString();

    public virtual MBBodyProperty BodyPropertyRange { get; protected set; }

    public int DefaultFormationGroup { get; set; }

    public FormationClass DefaultFormationClass { get; protected set; }

    public FormationPositionPreference FormationPositionPreference { get; protected set; }

    public virtual bool IsFemale { get; set; }

    public bool FaceMeshCache { get; private set; }

    public virtual MBReadOnlyList<Equipment> AllEquipments
    {
      get
      {
        if (this._equipmentRoster != null)
          return this._equipmentRoster.AllEquipments;
        return new List<Equipment>()
        {
          MBEquipmentRoster.EmptyEquipment
        }.GetReadOnlyList<Equipment>();
      }
    }

    public virtual Equipment Equipment => this._equipmentRoster == null ? MBEquipmentRoster.EmptyEquipment : this._equipmentRoster.DefaultEquipment;

    private bool HasCivilianEquipment() => this.AllEquipments.Any<Equipment>((Func<Equipment, bool>) (eq => eq.IsCivilian));

    public void InitializeEquipmentsOnLoad(BasicCharacterObject character) => this._equipmentRoster = character._equipmentRoster;

    public Equipment GetFirstEquipment(bool civilianSet) => !civilianSet || !this.HasCivilianEquipment() ? this.Equipment : this.AllEquipments.FirstOrDefault<Equipment>((Func<Equipment, bool>) (eq => eq.IsCivilian));

    public virtual int Level { get; set; }

    public BasicCultureObject Culture
    {
      get => this._culture;
      set => this._culture = value;
    }

    public virtual bool IsPlayerCharacter => false;

    public virtual float Age
    {
      get => this._age;
      set => this._age = value;
    }

    public virtual int HitPoints => this.MaxHitPoints();

    public virtual BodyProperties GetBodyPropertiesMin(bool returnBaseValue = false) => this.BodyPropertyRange.BodyPropertyMin;

    public virtual BodyProperties GetBodyPropertiesMax() => this.BodyPropertyRange.BodyPropertyMax;

    public virtual BodyProperties GetBodyProperties(Equipment equipment, int seed = -1) => FaceGen.GetRandomBodyProperties(this.IsFemale, this.GetBodyPropertiesMin(), this.GetBodyPropertiesMax(), equipment != null ? (int) equipment.HairCoverType : 0, seed, this.HairTags, this.BeardTags, this.TattooTags);

    public void CreateBodyPropertyRange(string stringId) => this.BodyPropertyRange = MBObjectManager.Instance.CreateObject<MBBodyProperty>(stringId);

    public virtual void UpdatePlayerCharacterBodyProperties(
      BodyProperties properties,
      bool isFemale)
    {
      this.BodyPropertyRange.Init(properties, properties);
      this.IsFemale = isFemale;
    }

    [SaveableProperty(16)]
    public float FaceDirtAmount { get; set; }

    public virtual string HairTags { get; set; } = "";

    public virtual string BeardTags { get; set; } = "";

    public virtual string TattooTags { get; set; } = "";

    public virtual bool IsHero => this._isBasicHero;

    public bool IsSoldier { get; private set; }

    public BasicCharacterObject() => this.DefaultFormationClass = FormationClass.Infantry;

    [LoadInitializationCallback]
    private void OnLoad(MetaData metaData)
    {
      this.HairTags = this.HairTags ?? "";
      this.BeardTags = this.BeardTags ?? "";
      this.TattooTags = this.TattooTags ?? "";
    }

    public int GetDefaultFaceSeed(int rank)
    {
      int num = this.StringId.GetDeterministicHashCode() * 6791 + rank * 197;
      return (num >= 0 ? num : -num) % 2000;
    }

    public float GetStepSize() => (float) (0.800000011920929 + 0.200000002980232 * (double) this.GetSkillValue(DefaultSkills.Athletics) * 0.0033333299215883);

    public bool HasMount() => this.Equipment[10].Item != null;

    public virtual int MaxHitPoints() => Game.Current.HumanMonster.HitPoints;

    public virtual float GetPower()
    {
      float num = (float) this.Level + 0.0f;
      return (float) (0.200000002980232 + ((double) num + 10.0) * ((double) num + 10.0) * (1.0 / 400.0));
    }

    public virtual int GetMountKeySeed() => MBRandom.RandomInt();

    public virtual int GetSkillValue(SkillObject skill) => this.CharacterSkills.Skills.GetPropertyValue(skill);

    public void InitializeHeroBasicCharacterOnAfterLoad(
      BasicCharacterObject originCharacter,
      TextObject name)
    {
      this.Name = name;
      this.IsSoldier = originCharacter.IsSoldier;
      this._isBasicHero = originCharacter._isBasicHero;
      this.CharacterSkills = originCharacter.CharacterSkills;
      this.HairTags = originCharacter.HairTags;
      this.BeardTags = originCharacter.BeardTags;
      this.TattooTags = originCharacter.TattooTags;
      this.BodyPropertyRange = originCharacter.BodyPropertyRange;
      this.IsFemale = originCharacter.IsFemale;
      this.Culture = originCharacter.Culture;
      this.DefaultFormationGroup = originCharacter.DefaultFormationGroup;
      this.DefaultFormationClass = originCharacter.DefaultFormationClass;
      this.FormationPositionPreference = originCharacter.FormationPositionPreference;
      this._equipmentRoster = originCharacter._equipmentRoster;
    }

    public override void Deserialize(MBObjectManager objectManager, XmlNode node)
    {
      base.Deserialize(objectManager, node);
      XmlAttribute attribute1 = node.Attributes["name"];
      if (attribute1 != null)
        this._basicName = new TextObject(attribute1.Value);
      XmlNode attribute2 = (XmlNode) node.Attributes["occupation"];
      if (attribute2 != null)
        this.IsSoldier = attribute2.InnerText.IndexOf("soldier", StringComparison.OrdinalIgnoreCase) >= 0;
      this._isBasicHero = XmlHelper.ReadBool(node, "is_hero");
      this.FaceMeshCache = XmlHelper.ReadBool(node, "face_mesh_cache");
      this.CharacterSkills = !(objectManager.ReadObjectReferenceFromXml("skill_template", typeof (MBCharacterSkills), node) is MBCharacterSkills mbCharacterSkills) ? MBObjectManager.Instance.CreateObject<MBCharacterSkills>(this.StringId) : mbCharacterSkills;
      BodyProperties bodyProperties1 = new BodyProperties();
      BodyProperties bodyProperties2 = new BodyProperties();
      foreach (XmlNode childNode1 in node.ChildNodes)
      {
        if (childNode1.Name == "Skills" || childNode1.Name == "skills")
        {
          if (mbCharacterSkills == null)
            this.CharacterSkills.Init(objectManager, childNode1);
        }
        else if (childNode1.Name == "Equipments" || childNode1.Name == "equipments")
        {
          List<XmlNode> xmlNodeList = new List<XmlNode>();
          foreach (XmlNode childNode2 in childNode1.ChildNodes)
          {
            if (childNode2.Name == "equipment")
              xmlNodeList.Add(childNode2);
          }
          foreach (XmlNode childNode2 in childNode1.ChildNodes)
          {
            if (childNode2.Name == "EquipmentRoster" || childNode2.Name == "equipmentRoster")
            {
              if (this._equipmentRoster == null)
                this._equipmentRoster = MBObjectManager.Instance.CreateObject<MBEquipmentRoster>(this.StringId);
              this._equipmentRoster.Init(objectManager, childNode2);
            }
            else if (childNode2.Name == "EquipmentSet" || childNode2.Name == "equipmentSet")
            {
              string innerText = childNode2.Attributes["id"].InnerText;
              bool isCivilian = childNode2.Attributes["civilian"] != null && bool.Parse(childNode2.Attributes["civilian"].InnerText);
              if (this._equipmentRoster == null)
                this._equipmentRoster = MBObjectManager.Instance.CreateObject<MBEquipmentRoster>(this.StringId);
              this._equipmentRoster.AddEquipmentRoster(MBObjectManager.Instance.GetObject<MBEquipmentRoster>(innerText), isCivilian);
            }
          }
          if (xmlNodeList.Any<XmlNode>())
            this._equipmentRoster.AddOverridenEquipments(objectManager, xmlNodeList);
        }
        else if (childNode1.Name == "face")
        {
          this.HairTags = "";
          this.BeardTags = "";
          this.TattooTags = "";
          foreach (XmlNode childNode2 in childNode1.ChildNodes)
          {
            if (childNode2.Name == "hair_tags")
            {
              foreach (XmlNode childNode3 in childNode2.ChildNodes)
                this.HairTags = this.HairTags + childNode3.Attributes["name"].Value + ",";
            }
            else if (childNode2.Name == "beard_tags")
            {
              foreach (XmlNode childNode3 in childNode2.ChildNodes)
                this.BeardTags = this.BeardTags + childNode3.Attributes["name"].Value + ",";
            }
            else if (childNode2.Name == "tattoo_tags")
            {
              foreach (XmlNode childNode3 in childNode2.ChildNodes)
                this.TattooTags = this.TattooTags + childNode3.Attributes["name"].Value + ",";
            }
            else if (childNode2.Name == "BodyProperties")
            {
              if (BodyProperties.FromXmlNode(childNode2, out bodyProperties1))
                ;
            }
            else if (childNode2.Name == "BodyPropertiesMax")
            {
              if (!BodyProperties.FromXmlNode(childNode2, out bodyProperties2))
                bodyProperties1 = bodyProperties2;
            }
            else if (childNode2.Name == "face_key_template")
              this.BodyPropertyRange = objectManager.ReadObjectReferenceFromXml<MBBodyProperty>("value", childNode2);
          }
        }
      }
      if (this.BodyPropertyRange == null)
      {
        this.BodyPropertyRange = MBObjectManager.Instance.CreateObject<MBBodyProperty>(this.StringId);
        this.BodyPropertyRange.Init(bodyProperties1, bodyProperties2);
      }
      this.IsFemale = false;
      this.DefaultFormationGroup = 0;
      XmlNode attribute3 = (XmlNode) node.Attributes["is_female"];
      if (attribute3 != null)
        this.IsFemale = Convert.ToBoolean(attribute3.InnerText);
      this.Culture = objectManager.ReadObjectReferenceFromXml<BasicCultureObject>("culture", node);
      XmlNode attribute4 = (XmlNode) node.Attributes["age"];
      this.Age = attribute4 == null ? Math.Max(20f, this.BodyPropertyRange.BodyPropertyMax.Age) : (float) Convert.ToInt32(attribute4.InnerText);
      XmlNode attribute5 = (XmlNode) node.Attributes["level"];
      this.Level = attribute5 != null ? Convert.ToInt32(attribute5.InnerText) : 1;
      XmlNode attribute6 = (XmlNode) node.Attributes["default_group"];
      if (attribute6 != null)
        this.DefaultFormationGroup = this.FetchDefaultFormationGroup(attribute6.InnerText);
      this.DefaultFormationClass = (FormationClass) this.DefaultFormationGroup;
      XmlNode attribute7 = (XmlNode) node.Attributes["formation_position_preference"];
      this.FormationPositionPreference = attribute7 != null ? (FormationPositionPreference) Enum.Parse(typeof (FormationPositionPreference), attribute7.InnerText) : FormationPositionPreference.Middle;
      XmlNode attribute8 = (XmlNode) node.Attributes["default_equipment_set"];
      if (attribute8 != null)
        this._equipmentRoster.InitializeDefaultEquipment(attribute8.Value);
      this._equipmentRoster?.OrderEquipments();
    }

    protected int FetchDefaultFormationGroup(string innerText)
    {
      FormationClass result;
      return Enum.TryParse<FormationClass>(innerText, true, out result) ? (int) result : -1;
    }

    public virtual FormationClass GetFormationClass(IBattleCombatant owner) => this.DefaultFormationClass;

    internal static void AutoGeneratedStaticCollectObjectsBasicCharacterObject(
      object o,
      List<object> collectedObjects)
    {
      ((MBObjectBase) o).AutoGeneratedInstanceCollectObjects(collectedObjects);
    }

    protected override void AutoGeneratedInstanceCollectObjects(List<object> collectedObjects) => base.AutoGeneratedInstanceCollectObjects(collectedObjects);

    internal static object AutoGeneratedGetMemberValueFaceDirtAmount(object o) => (object) ((BasicCharacterObject) o).FaceDirtAmount;
  }
}
