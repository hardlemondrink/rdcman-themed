// Decompiled with JetBrains decompiler
// Type: RdcMan.FileGroupPropertiesDialog
// Assembly: RDCMan, Version=2.7.1406.0, Culture=neutral, PublicKeyToken=null
// MVID: 984CDC7A-116B-406E-B519-0DEE541B9019
// Assembly location: C:\Program Files (x86)\Microsoft\Remote Desktop Connection Manager\RDCMan.exe

using System;
using System.Windows.Forms;

namespace RdcMan
{
  internal class FileGroupPropertiesDialog : GroupBasePropertiesDialog
  {
    protected FileGroupPropertiesDialog(
      FileGroup group,
      string dialogTitle,
      string acceptButtonText,
      Form parentForm)
      : base((GroupBase) group, dialogTitle, acceptButtonText, parentForm)
    {
    }

    public static FileGroupPropertiesDialog NewPropertiesDialog(
      FileGroup group,
      Form parentForm)
    {
      FileGroupPropertiesDialog propertiesDialog = new FileGroupPropertiesDialog(group, group.Text + " File Properties", "OK", parentForm);
      propertiesDialog.CreateControls((RdcTreeNode) group);
      return propertiesDialog;
    }

    public override void CreateControls(RdcTreeNode settings)
    {
      FileGroupPropertiesTabPage tabPage = settings.Properties.CreateTabPage((TabbedSettingsDialog) this) as FileGroupPropertiesTabPage;
      this.PropertiesPage = (INodePropertiesPage) tabPage;
      this.AddTabPage((TabPage) tabPage);
      this.PropertiesPage.ParentGroupChanged += new Action<GroupBase>(((NodePropertiesDialog) this).PopulateCredentialsProfiles);
      base.CreateControls(settings);
      this.AddTabPage(settings.EncryptionSettings.CreateTabPage((TabbedSettingsDialog) this));
      this.CreateProfileManagementTabPage();
      this.PopulateCredentialsProfiles((GroupBase) (this.AssociatedNode as FileGroup));
      this.PopulateCredentialsManagementTab((this.AssociatedNode as FileGroup).CredentialsProfiles);
    }
  }
}
