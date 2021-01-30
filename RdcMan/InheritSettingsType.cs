// Decompiled with JetBrains decompiler
// Type: RdcMan.InheritSettingsType
// Assembly: RDCMan, Version=2.7.1406.0, Culture=neutral, PublicKeyToken=null
// MVID: 984CDC7A-116B-406E-B519-0DEE541B9019
// Assembly location: C:\Program Files (x86)\Microsoft\Remote Desktop Connection Manager\RDCMan.exe

using System;
using System.Xml;

namespace RdcMan
{
  public class InheritSettingsType
  {
    public InheritSettingsType() => this.Mode = InheritanceMode.FromParent;

    public GroupBase GetInheritedSettingsNode(RdcTreeNode node)
    {
      switch (this.Mode)
      {
        case InheritanceMode.FromParent:
          return node.Parent != null ? node.Parent as GroupBase : (GroupBase) DefaultSettingsGroup.Instance;
        case InheritanceMode.None:
        case InheritanceMode.Disabled:
          return (GroupBase) null;
        default:
          throw new Exception("Unexpected inheritance kind");
      }
    }

    public void WriteXml(XmlTextWriter tw) => tw.WriteAttributeString("inherit", this.Mode.ToString());

    public InheritanceMode Mode { get; set; }
  }
}
