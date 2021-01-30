// Decompiled with JetBrains decompiler
// Type: RdcMan.LogonSettingsDialog
// Assembly: RDCMan, Version=2.7.1406.0, Culture=neutral, PublicKeyToken=null
// MVID: 984CDC7A-116B-406E-B519-0DEE541B9019
// Assembly location: C:\Program Files (x86)\Microsoft\Remote Desktop Connection Manager\RDCMan.exe

using System;
using System.Windows.Forms;

namespace RdcMan
{
  public class LogonSettingsDialog : RdcDialog
  {
    protected CredentialsUI _logonCredentialsUI;

    protected LogonSettingsDialog(string title, string buttonText)
      : base(title, buttonText, (Form) null)
    {
      this.SuspendLayout();
      this._logonCredentialsUI = new CredentialsUI((InheritanceControl) null);
    }

    public bool PasswordChanged => this._logonCredentialsUI.PasswordChanged;

    public string ProfileName => this._logonCredentialsUI.ProfileComboBox.SelectedValue.ProfileName;

    public ProfileScope ProfileScope => this._logonCredentialsUI.ProfileComboBox.SelectedValue.ProfileScope;

    public string UserName => this._logonCredentialsUI.UserNameTextBox.Text;

    public PasswordSetting Password => new PasswordSetting((object) this._logonCredentialsUI.PasswordTextBox.Text)
    {
      IsDecrypted = true
    };

    public string Domain => this._logonCredentialsUI.DomainTextBox.Text;

    protected override void ShownCallback(object sender, EventArgs e) => this._logonCredentialsUI.UserNameTextBox.Focus();

    public static LogonSettingsDialog NewEditCredentialsDialog(
      CredentialsProfile credentials)
    {
      LogonSettingsDialog logonSettingsDialog = new LogonSettingsDialog("Edit Credentials", "Save");
      int rowIndex = 0;
      int tabIndex = 0;
      logonSettingsDialog._logonCredentialsUI.AddControlsToParent((Control) logonSettingsDialog, LogonCredentialsDialogOptions.None, ref rowIndex, ref tabIndex);
      logonSettingsDialog._logonCredentialsUI.EnableDisableControls(true);
      logonSettingsDialog._logonCredentialsUI.InitFromCredentials((ILogonCredentials) credentials);
      logonSettingsDialog.FinalizeLayout(rowIndex, tabIndex);
      return logonSettingsDialog;
    }

    protected void FinalizeLayout(int rowIndex, int tabIndex)
    {
      this.Height = FormTools.YPos(rowIndex + 1) + 16;
      this.InitButtons();
      this.ScaleAndLayout();
    }
  }
}
