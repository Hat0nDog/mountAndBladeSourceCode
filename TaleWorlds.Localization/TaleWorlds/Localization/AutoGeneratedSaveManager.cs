﻿// Decompiled with JetBrains decompiler
// Type: TaleWorlds.Localization.AutoGeneratedSaveManager
// Assembly: TaleWorlds.Localization, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 26BB3E5A-EB48-4ABD-B2FC-10EF6D7A7285
// Assembly location: E:\SteamLibrary\steamapps\common\Mount & Blade II Bannerlord\bin\Win64_Shipping_Client\TaleWorlds.Localization.dll

using TaleWorlds.SaveSystem.Definition;

namespace TaleWorlds.Localization
{
  internal class AutoGeneratedSaveManager : IAutoGeneratedSaveManager
  {
    public void Initialize(DefinitionContext definitionContext)
    {
      TypeDefinition typeDefinition = (TypeDefinition) definitionContext.TryGetTypeDefinition((SaveId) new TypeSaveId(20001));
      typeDefinition.InitializeForAutoGeneration(new CollectObjectsDelegate(TextObject.AutoGeneratedStaticCollectObjectsTextObject));
      typeDefinition.GetPropertyDefinitionWithId(new MemberTypeId((byte) 2, (short) 2)).InitializeForAutoGeneration(new GetPropertyValueDelegate(TextObject.AutoGeneratedGetMemberValueAttributes));
      typeDefinition.GetFieldDefinitionWithId(new MemberTypeId((byte) 2, (short) 1)).InitializeForAutoGeneration(new GetFieldValueDelegate(TextObject.AutoGeneratedGetMemberValueValue));
    }
  }
}