// Decompiled with JetBrains decompiler
// Type: RdcMan.GroupPropertiesDialog
// Assembly: RDCMan, Version=2.7.1406.0, Culture=neutral, PublicKeyToken=null
// MVID: 984CDC7A-116B-406E-B519-0DEE541B9019
// Assembly location: C:\Program Files (x86)\Microsoft\Remote Desktop Connection Manager\RDCMan.exe

using System;
using System.Windows.Forms;

namespace RdcMan
{
  internal class GroupPropertiesDialog : GroupBasePropertiesDialog
  {
    protected GroupPropertiesDialog(
      GroupBase group,
      string dialogTitle,
      string acceptButtonText,
      Form parentForm)
      : base(group, dialogTitle, acceptButtonText, parentForm)
    {
    }

    public override void CreateControls(RdcTreeNode settingsNode)
    {
      GroupPropertiesTabPage tabPage = settingsNode.Properties.CreateTabPage((TabbedSettingsDialog) this) as GroupPropertiesTabPage;
      this.PropertiesPage = (INodePropertiesPage) tabPage;
      this.AddTabPage((TabPage) tabPage);
      this.PropertiesPage.ParentGroupChanged += new Action<GroupBase>(((NodePropertiesDialog) this).PopulateCredentialsProfiles);
      base.CreateControls(settingsNode);
    }

    public static GroupPropertiesDialog NewAddDialog(GroupBase parent)
    {
      Group forAddDialog = Group.CreateForAddDialog();
      GroupPropertiesDialog propertiesDialog = new GroupPropertiesDialog((GroupBase) forAddDialog, "Add Group", "Add", (Form) null);
      propertiesDialog.CreateControls((RdcTreeNode) forAddDialog);
      if (parent != null && !parent.CanAddGroups())
        parent = (GroupBase) null;
      if (propertiesDialog.PropertiesPage.PopulateParentDropDown((GroupBase) null, parent))
        return propertiesDialog;
      propertiesDialog.Dispose();
      return (GroupPropertiesDialog) null;
    }

    public static GroupPropertiesDialog NewPropertiesDialog(
      Group group,
      Form parentForm)
    {
      GroupPropertiesDialog propertiesDialog = new GroupPropertiesDialog((GroupBase) group, group.Text + " Group Properties", "OK", parentForm);
      propertiesDialog.CreateControls((RdcTreeNode) group);
      propertiesDialog.PropertiesPage.PopulateParentDropDown((GroupBase) group, group.Parent as GroupBase);
      return propertiesDialog;
    }
  }
}
