// Decompiled with JetBrains decompiler
// Type: RdcMan.EncryptionSettingsTabPage
// Assembly: RDCMan, Version=2.7.1406.0, Culture=neutral, PublicKeyToken=null
// MVID: 984CDC7A-116B-406E-B519-0DEE541B9019
// Assembly location: C:\Program Files (x86)\Microsoft\Remote Desktop Connection Manager\RDCMan.exe

using System;
using System.Drawing;
using System.Security.Cryptography.X509Certificates;
using System.Windows.Forms;

namespace RdcMan
{
  public class EncryptionSettingsTabPage : SettingsTabPage<EncryptionSettings>
  {
    public InheritanceControl InheritEncryptionSettings;
    protected ValueComboBox<EncryptionMethod> _passwordEncryptionMethodCombo;
    protected Label _passwordEncryptionDataLabel;
    protected Button _passwordEncryptionDataButton;
    protected Label _passwordEncryptionDataInfoLabel;
    private EncryptionMethod _passwordEncryptionMethodPrevious;

    public EncryptionSettingsTabPage(TabbedSettingsDialog dialog, EncryptionSettings settings)
      : base(dialog, settings)
    {
      int tabIndex = 0;
      int rowIndex1 = 0;
      this.CreateInheritanceControl(ref rowIndex1, ref tabIndex);
      if (this.InheritanceControl != null)
        this.InheritanceControl.EnabledChanged += (Action<bool>) (enabled => this.PasswordEncryptionMethodCombo_Changed((object) null, (EventArgs) null));
      this._passwordEncryptionMethodCombo = FormTools.AddLabeledEnumDropDown<EncryptionMethod>((Control) this, "Password encryption:", this.Settings.EncryptionMethod, ref rowIndex1, ref tabIndex, new Func<EncryptionMethod, string>(Encryption.EncryptionMethodToString));
      this._passwordEncryptionMethodCombo.Enter += new EventHandler(this.PasswordEncryptionMethodCombo_Enter);
      this._passwordEncryptionMethodCombo.SelectedIndexChanged += new EventHandler(this.PasswordEncryptionMethodCombo_Changed);
      this._passwordEncryptionDataLabel = FormTools.NewLabel(string.Empty, 0, rowIndex1);
      Button button1 = new Button();
      button1.Enabled = false;
      Button button2 = button1;
      int rowIndex2 = rowIndex1;
      int num1 = rowIndex2 + 1;
      Point point = FormTools.NewLocation(1, rowIndex2);
      button2.Location = point;
      button1.Width = 340;
      button1.TabIndex = tabIndex++;
      button1.TextAlign = ContentAlignment.MiddleLeft;
      this._passwordEncryptionDataButton = button1;
      this._passwordEncryptionDataButton.Click += new EventHandler(this.PasswordEncryptionMethodButton_Click);
      string empty = string.Empty;
      int rowIndex3 = num1;
      int num2 = rowIndex3 + 1;
      this._passwordEncryptionDataInfoLabel = FormTools.NewLabel(empty, 1, rowIndex3);
      this._passwordEncryptionDataInfoLabel.Width = 340;
      this.Controls.Add((Control) this._passwordEncryptionDataLabel, (Control) this._passwordEncryptionDataButton, (Control) this._passwordEncryptionDataInfoLabel);
    }

    protected override void UpdateControls()
    {
      if (this.Settings.EncryptionMethod.Value == EncryptionMethod.Certificate)
        this._passwordEncryptionDataButton.Tag = (object) Encryption.GetCertificate(this.Settings.CredentialData.Value);
      this._passwordEncryptionMethodCombo.SelectedValue = this.Settings.EncryptionMethod.Value;
      base.UpdateControls();
      this.PasswordEncryptionMethodCombo_Changed((object) null, (EventArgs) null);
    }

    protected override void UpdateSettings()
    {
      base.UpdateSettings();
      X509Certificate2 tag = (X509Certificate2) this._passwordEncryptionDataButton.Tag;
      this.Settings.CredentialData.Value = tag != null ? tag.Thumbprint : string.Empty;
      this.Settings.CredentialName.Value = this._passwordEncryptionDataButton.Text;
    }

    private void PasswordEncryptionMethodCombo_Enter(object sender, EventArgs e) => this._passwordEncryptionMethodPrevious = this._passwordEncryptionMethodCombo.SelectedValue;

    private void PasswordEncryptionMethodCombo_Changed(object sender, EventArgs e)
    {
      switch (this._passwordEncryptionMethodCombo.SelectedValue)
      {
        case EncryptionMethod.LogonCredentials:
          this._passwordEncryptionDataLabel.Text = "User name:";
          this._passwordEncryptionDataButton.Text = CredentialsUI.GetLoggedInUser();
          this._passwordEncryptionDataButton.Tag = (object) null;
          this._passwordEncryptionDataButton.Enabled = false;
          this._passwordEncryptionDataInfoLabel.Text = string.Empty;
          break;
        case EncryptionMethod.Certificate:
          if (!(this._passwordEncryptionDataButton.Tag is X509Certificate2 cert))
          {
            try
            {
              this.Enabled = false;
              cert = Encryption.SelectCertificate();
            }
            finally
            {
              this.Enabled = true;
            }
          }
          if (cert != null)
          {
            this.SetSelectedCertificate(cert);
            break;
          }
          this._passwordEncryptionMethodCombo.SelectedValue = this._passwordEncryptionMethodPrevious;
          break;
        default:
          throw new NotImplementedException("Unexpected encryption method '{0}'".InvariantFormat((object) this._passwordEncryptionMethodCombo.SelectedValue.ToString()));
      }
      this._passwordEncryptionMethodPrevious = this._passwordEncryptionMethodCombo.SelectedValue;
    }

    protected void SetSelectedCertificate(X509Certificate2 cert)
    {
      if (cert == null)
        return;
      this._passwordEncryptionDataButton.Text = cert.SimpleName();
      this._passwordEncryptionDataButton.Tag = (object) cert;
      this._passwordEncryptionDataButton.Enabled = this._passwordEncryptionMethodCombo.Enabled;
      this._passwordEncryptionDataLabel.Text = "Certificate:";
      this._passwordEncryptionDataInfoLabel.Text = "Valid from {0} to {1}".InvariantFormat((object) cert.NotBefore.ToUniversalTime().ToShortDateString(), (object) cert.NotAfter.ToUniversalTime().ToShortDateString());
    }

    private void PasswordEncryptionMethodButton_Click(object sender, EventArgs e)
    {
      X509Certificate2 cert;
      try
      {
        this.Enabled = false;
        cert = Encryption.SelectCertificate();
      }
      finally
      {
        this.Enabled = true;
      }
      this.SetSelectedCertificate(cert);
    }
  }
}
