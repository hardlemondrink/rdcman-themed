// Decompiled with JetBrains decompiler
// Type: RdcMan.ServerPropertiesDialog
// Assembly: RDCMan, Version=2.7.1406.0, Culture=neutral, PublicKeyToken=null
// MVID: 984CDC7A-116B-406E-B519-0DEE541B9019
// Assembly location: C:\Program Files (x86)\Microsoft\Remote Desktop Connection Manager\RDCMan.exe

using System;
using System.Windows.Forms;

namespace RdcMan
{
  internal class ServerPropertiesDialog : NodePropertiesDialog
  {
    private ServerPropertiesDialog(
      Server server,
      string dialogTitle,
      string acceptButtonText,
      Form parentForm)
      : base((RdcTreeNode) server, dialogTitle, acceptButtonText, parentForm)
    {
    }

    private void CreateServerPropertiesPage(RdcTreeNode settings)
    {
      ServerPropertiesTabPage tabPage = settings.Properties.CreateTabPage((TabbedSettingsDialog) this) as ServerPropertiesTabPage;
      this.PropertiesPage = (INodePropertiesPage) tabPage;
      this.AddTabPage((TabPage) tabPage);
      this.PropertiesPage.ParentGroupChanged += new Action<GroupBase>(((NodePropertiesDialog) this).PopulateCredentialsProfiles);
    }

    private void CreateImportServersPage(RdcTreeNode settings)
    {
      ImportServersPropertiesPage serversPropertiesPage = new ImportServersPropertiesPage((TabbedSettingsDialog) this);
      this.PropertiesPage = (INodePropertiesPage) serversPropertiesPage;
      this.AddTabPage((TabPage) serversPropertiesPage);
      this.PropertiesPage.ParentGroupChanged += new Action<GroupBase>(((NodePropertiesDialog) this).PopulateCredentialsProfiles);
    }

    public static ServerPropertiesDialog NewAddDialog(GroupBase parent)
    {
      Server forAddDialog = Server.CreateForAddDialog();
      ServerPropertiesDialog propertiesDialog = new ServerPropertiesDialog(forAddDialog, "Add Server", "Add", (Form) null);
      propertiesDialog.CreateServerPropertiesPage((RdcTreeNode) forAddDialog);
      propertiesDialog.CreateControls((RdcTreeNode) forAddDialog);
      if (propertiesDialog.PropertiesPage.PopulateParentDropDown((GroupBase) null, parent))
        return propertiesDialog;
      propertiesDialog.Dispose();
      return (ServerPropertiesDialog) null;
    }

    public static ServerPropertiesDialog NewImportDialog(GroupBase parent)
    {
      Server forAddDialog = Server.CreateForAddDialog();
      ServerPropertiesDialog propertiesDialog = new ServerPropertiesDialog(forAddDialog, "Import Servers", "Import", (Form) null);
      propertiesDialog.CreateImportServersPage((RdcTreeNode) forAddDialog);
      propertiesDialog.CreateControls((RdcTreeNode) forAddDialog);
      if (propertiesDialog.PropertiesPage.PopulateParentDropDown((GroupBase) null, parent))
        return propertiesDialog;
      propertiesDialog.Dispose();
      return (ServerPropertiesDialog) null;
    }

    public static ServerPropertiesDialog NewPropertiesDialog(
      Server server,
      Form parentForm)
    {
      ServerPropertiesDialog propertiesDialog = new ServerPropertiesDialog(server, server.DisplayName + " Server Properties", "OK", parentForm);
      propertiesDialog.CreateServerPropertiesPage((RdcTreeNode) server);
      propertiesDialog.CreateControls((RdcTreeNode) server);
      if (server.FileGroup == null)
        propertiesDialog.PropertiesPage.SetParentDropDown(server.Parent as GroupBase);
      propertiesDialog.PropertiesPage.PopulateParentDropDown((GroupBase) null, server.Parent as GroupBase);
      return propertiesDialog;
    }
  }
}
