// Decompiled with JetBrains decompiler
// Type: RdcMan.DefaultGroupPropertiesDialog
// Assembly: RDCMan, Version=2.7.1406.0, Culture=neutral, PublicKeyToken=null
// MVID: 984CDC7A-116B-406E-B519-0DEE541B9019
// Assembly location: C:\Program Files (x86)\Microsoft\Remote Desktop Connection Manager\RDCMan.exe

using System.Windows.Forms;

namespace RdcMan
{
  internal class DefaultGroupPropertiesDialog : GroupBasePropertiesDialog
  {
    protected DefaultGroupPropertiesDialog(GroupBase group, Form parentForm)
      : base(group, "Default Settings Group Properties", "OK", parentForm)
    {
    }

    public static DefaultGroupPropertiesDialog NewPropertiesDialog(
      GroupBase group,
      Form parentForm)
    {
      DefaultGroupPropertiesDialog propertiesDialog = new DefaultGroupPropertiesDialog(group, parentForm);
      propertiesDialog.CreateControls((RdcTreeNode) group);
      propertiesDialog.PopulateCredentialsProfiles((GroupBase) null);
      propertiesDialog.PopulateCredentialsManagementTab(Program.CredentialsProfiles);
      return propertiesDialog;
    }

    public override void CreateControls(RdcTreeNode settingsNode)
    {
      base.CreateControls(settingsNode);
      this.AddTabPage(settingsNode.EncryptionSettings.CreateTabPage((TabbedSettingsDialog) this));
      this.CreateProfileManagementTabPage();
    }
  }
}
