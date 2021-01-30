// Decompiled with JetBrains decompiler
// Type: RdcMan.NodePropertiesDialog
// Assembly: RDCMan, Version=2.7.1406.0, Culture=neutral, PublicKeyToken=null
// MVID: 984CDC7A-116B-406E-B519-0DEE541B9019
// Assembly location: C:\Program Files (x86)\Microsoft\Remote Desktop Connection Manager\RDCMan.exe

using System.Linq;
using System.Windows.Forms;

namespace RdcMan
{
  public abstract class NodePropertiesDialog : TabbedSettingsDialog
  {
    public RdcTreeNode AssociatedNode { get; private set; }

    public INodePropertiesPage PropertiesPage { get; protected set; }

    public override void InitButtons()
    {
      base.InitButtons();
      if (this.AssociatedNode == null)
        return;
      this._acceptButton.Enabled = this.AssociatedNode.AllowEdit(true);
    }

    protected NodePropertiesDialog(
      RdcTreeNode associatedNode,
      string dialogTitle,
      string acceptButtonText,
      Form parentForm)
      : base(dialogTitle, acceptButtonText, parentForm)
    {
      this.AssociatedNode = associatedNode;
    }

    public virtual void CreateControls(RdcTreeNode settings)
    {
      LogonCredentialsTabPage tabPage1 = (LogonCredentialsTabPage) settings.LogonCredentials.CreateTabPage((TabbedSettingsDialog) this);
      LogonCredentialsDialogOptions options1 = LogonCredentialsDialogOptions.ShowProfiles;
      if (settings.LogonCredentials.InheritSettingsType.Mode != InheritanceMode.Disabled)
        options1 |= LogonCredentialsDialogOptions.AllowInheritance;
      tabPage1.CreateControls(options1);
      this.AddTabPage((TabPage) tabPage1);
      GatewaySettingsTabPage tabPage2 = (GatewaySettingsTabPage) settings.GatewaySettings.CreateTabPage((TabbedSettingsDialog) this);
      LogonCredentialsDialogOptions options2 = LogonCredentialsDialogOptions.ShowProfiles;
      if (settings.GatewaySettings.InheritSettingsType.Mode != InheritanceMode.Disabled)
        options2 |= LogonCredentialsDialogOptions.AllowInheritance;
      tabPage2.CreateControls(options2);
      this.AddTabPage((TabPage) tabPage2);
      this.AddTabPage(settings.ConnectionSettings.CreateTabPage((TabbedSettingsDialog) this));
      this.AddTabPage(settings.RemoteDesktopSettings.CreateTabPage((TabbedSettingsDialog) this));
      this.AddTabPage(settings.LocalResourceSettings.CreateTabPage((TabbedSettingsDialog) this));
      this.AddTabPage(settings.SecuritySettings.CreateTabPage((TabbedSettingsDialog) this));
      this.AddTabPage(settings.DisplaySettings.CreateTabPage((TabbedSettingsDialog) this));
      this.InitButtons();
      this.ScaleAndLayout();
      settings.InheritSettings();
      settings.ResolveCredentials();
    }

    protected void PopulateCredentialsProfiles(GroupBase group)
    {
      FileGroup fileGroup = group?.FileGroup;
      foreach (ICredentialsTabPage credentialsTabPage in this.TabPages.OfType<ICredentialsTabPage>())
        credentialsTabPage.PopulateCredentialsProfiles(fileGroup);
    }
  }
}
