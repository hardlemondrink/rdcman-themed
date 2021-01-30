// Decompiled with JetBrains decompiler
// Type: RdcMan.INodePropertiesPage
// Assembly: RDCMan, Version=2.7.1406.0, Culture=neutral, PublicKeyToken=null
// MVID: 984CDC7A-116B-406E-B519-0DEE541B9019
// Assembly location: C:\Program Files (x86)\Microsoft\Remote Desktop Connection Manager\RDCMan.exe

using System;

namespace RdcMan
{
  public interface INodePropertiesPage
  {
    event Action<GroupBase> ParentGroupChanged;

    GroupBase ParentGroup { get; }

    bool PopulateParentDropDown(GroupBase excludeGroup, GroupBase defaultParent);

    void SetParentDropDown(GroupBase group);
  }
}
