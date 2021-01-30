// Decompiled with JetBrains decompiler
// Type: RdcMan.GatewaySettingsTabPage
// Assembly: RDCMan, Version=2.7.1406.0, Culture=neutral, PublicKeyToken=null
// MVID: 984CDC7A-116B-406E-B519-0DEE541B9019
// Assembly location: C:\Program Files (x86)\Microsoft\Remote Desktop Connection Manager\RDCMan.exe

using System;
using System.Windows.Forms;

namespace RdcMan
{
  public class GatewaySettingsTabPage : CredentialsTabPage<GatewaySettings>
  {
    private CheckBox _useGatewayServerCheckBox;
    private ValueComboBox<RdpClient.GatewayLogonMethod> _gatewayLogonMethodCombo;
    private CheckBox _gatewayLocalBypassCheckBox;
    private RdcTextBox _gatewayHostNameTextBox;
    private CheckBox _gatewayCredSharingCheckBox;

    public GatewaySettingsTabPage(TabbedSettingsDialog dialog, GatewaySettings settings)
      : base(dialog, settings)
    {
    }

    public void CreateControls(LogonCredentialsDialogOptions options)
    {
      int tabIndex = 0;
      int rowIndex = 0;
      this.CreateInheritanceControl(ref rowIndex, ref tabIndex);
      if (this.InheritanceControl != null)
        this.InheritanceControl.EnabledChanged += (Action<bool>) (enabled =>
        {
          this._useGatewayServerCheckBox.Enabled = enabled;
          this.UseGatewayServerCheckBox_CheckedChanged((object) null, (EventArgs) null);
        });
      this._useGatewayServerCheckBox = (CheckBox) FormTools.AddCheckBox((Control) this, "&Use a TS Gateway server", this.Settings.UseGatewayServer, 1, ref rowIndex, ref tabIndex);
      this._useGatewayServerCheckBox.CheckedChanged += new EventHandler(this.UseGatewayServerCheckBox_CheckedChanged);
      this._gatewayHostNameTextBox = FormTools.AddLabeledTextBox((Control) this, "&Server name:", this.Settings.HostName, ref rowIndex, ref tabIndex);
      this._gatewayHostNameTextBox.Validate = (Func<string>) (() =>
      {
        if (this._gatewayHostNameTextBox.Enabled)
        {
          this._gatewayHostNameTextBox.Text = this._gatewayHostNameTextBox.Text.Trim();
          if (this._gatewayHostNameTextBox.Text.Length == 0)
            return "Please enter a server name";
        }
        return (string) null;
      });
      this._gatewayLocalBypassCheckBox = (CheckBox) FormTools.AddCheckBox((Control) this, "&Bypass for local addresses", this.Settings.BypassGatewayForLocalAddresses, 1, ref rowIndex, ref tabIndex);
      this._gatewayLogonMethodCombo = FormTools.AddLabeledEnumDropDown<RdpClient.GatewayLogonMethod>((Control) this, "&Logon method", this.Settings.LogonMethod, ref rowIndex, ref tabIndex, new Func<RdpClient.GatewayLogonMethod, string>(RdpClient.GatewayLogonMethodToString));
      this._gatewayLogonMethodCombo.SelectedValueChanged += new EventHandler(this.GatewayLogonMethodComboBox_SelectedValueChanged);
      if (!RdpClient.SupportsGatewayCredentials)
        return;
      this._gatewayCredSharingCheckBox = (CheckBox) FormTools.AddCheckBox((Control) this, "Share Gateway &credentials with remote computer", this.Settings.CredentialSharing, 1, ref rowIndex, ref tabIndex);
      this._gatewayCredSharingCheckBox.CheckedChanged += new EventHandler(this.GatewayCredSharingCheckBox_CheckedChanged);
      this._credentialsUI = new CredentialsUI(this.InheritanceControl);
      this._credentialsUI.AddControlsToParent((Control) this, options, ref rowIndex, ref tabIndex);
    }

    protected override void UpdateControls()
    {
      base.UpdateControls();
      if (this.InheritanceControl != null)
        return;
      this.UseGatewayServerCheckBox_CheckedChanged((object) null, (EventArgs) null);
    }

    private void GatewayCredSharingCheckBox_CheckedChanged(object sender, EventArgs e) => this.GatewayLogonMethodComboBox_SelectedValueChanged((object) null, (EventArgs) null);

    private void UseGatewayServerCheckBox_CheckedChanged(object sender, EventArgs e)
    {
      bool flag = this._useGatewayServerCheckBox.Enabled && this._useGatewayServerCheckBox.Checked;
      this._gatewayHostNameTextBox.Enabled = flag;
      this._gatewayLogonMethodCombo.Enabled = flag;
      this._gatewayLocalBypassCheckBox.Enabled = flag;
      if (RdpClient.SupportsGatewayCredentials)
        this._gatewayCredSharingCheckBox.Enabled = flag;
      this.GatewayLogonMethodComboBox_SelectedValueChanged((object) null, (EventArgs) null);
    }

    private void GatewayLogonMethodComboBox_SelectedValueChanged(object sender, EventArgs e)
    {
      bool enable = this._gatewayLogonMethodCombo.Enabled && this._gatewayLogonMethodCombo.SelectedValue == RdpClient.GatewayLogonMethod.NTLM;
      if (!RdpClient.SupportsGatewayCredentials)
        return;
      this._credentialsUI.EnableDisableControls(enable);
    }
  }
}
