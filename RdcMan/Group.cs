// Decompiled with JetBrains decompiler
// Type: RdcMan.Group
// Assembly: RDCMan, Version=2.7.1406.0, Culture=neutral, PublicKeyToken=null
// MVID: 984CDC7A-116B-406E-B519-0DEE541B9019
// Assembly location: C:\Program Files (x86)\Microsoft\Remote Desktop Connection Manager\RDCMan.exe

using System.Collections.Generic;
using System.Windows.Forms;
using System.Xml;

namespace RdcMan
{
  public class Group : GroupBase
  {
    public const string XmlNodeName = "group";

    protected Group()
    {
    }

    internal static Group CreateForAddDialog() => new Group();

    public static Group Create(string name, GroupBase parent)
    {
      Group group = new Group();
      group.Properties.GroupName.Value = name;
      group.FinishConstruction(parent);
      return group;
    }

    internal static Group Create(GroupPropertiesDialog dlg)
    {
      Group associatedNode = dlg.AssociatedNode as Group;
      associatedNode.UpdateSettings((NodePropertiesDialog) dlg);
      associatedNode.FinishConstruction(dlg.PropertiesPage.ParentGroup);
      return associatedNode;
    }

    internal static Group Create(
      XmlNode xmlNode,
      GroupBase parent,
      ICollection<string> errors)
    {
      Group group = new Group();
      group.ReadXml(xmlNode, errors);
      group.FinishConstruction(parent);
      return group;
    }

    protected override void InitSettings()
    {
      this.Properties = (CommonNodeSettings) new GroupSettings();
      base.InitSettings();
    }

    internal override void WriteXml(XmlTextWriter tw)
    {
      tw.WriteStartElement("group");
      base.WriteXml(tw);
      tw.WriteEndElement();
    }

    public override void DoPropertiesDialog(Form parentForm, string activeTabName)
    {
      using (GroupPropertiesDialog propertiesDialog = GroupPropertiesDialog.NewPropertiesDialog(this, parentForm))
      {
        propertiesDialog.SetActiveTab(activeTabName);
        if (propertiesDialog.ShowDialog() != DialogResult.OK)
          return;
        this.UpdateSettings((NodePropertiesDialog) propertiesDialog);
        ServerTree.Instance.OnNodeChanged((RdcTreeNode) this, ChangeType.PropertyChanged);
      }
    }

    private void FinishConstruction(GroupBase parent)
    {
      this.Text = this.Properties.GroupName.Value;
      ServerTree.Instance.AddNode((RdcTreeNode) this, parent);
      this.ChangeImageIndex(ImageConstants.Group);
    }
  }
}
