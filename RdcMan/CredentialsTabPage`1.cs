// Decompiled with JetBrains decompiler
// Type: RdcMan.CredentialsTabPage`1
// Assembly: RDCMan, Version=2.7.1406.0, Culture=neutral, PublicKeyToken=null
// MVID: 984CDC7A-116B-406E-B519-0DEE541B9019
// Assembly location: C:\Program Files (x86)\Microsoft\Remote Desktop Connection Manager\RDCMan.exe

namespace RdcMan
{
  public abstract class CredentialsTabPage<TSettingGroup> : 
    SettingsTabPage<TSettingGroup>,
    ICredentialsTabPage
    where TSettingGroup : LogonCredentials
  {
    protected CredentialsUI _credentialsUI;

    protected CredentialsTabPage(TabbedSettingsDialog dialog, TSettingGroup settings)
      : base(dialog, settings)
    {
    }

    public CredentialsProfile Credentials => this._credentialsUI.ProfileComboBox.SelectedValue;

    public void PopulateCredentialsProfiles(FileGroup file)
    {
      if (this._credentialsUI == null)
        return;
      this._credentialsUI.PopulateCredentialsProfiles(file);
    }

    protected override void UpdateControls()
    {
      base.UpdateControls();
      if (this._credentialsUI == null)
        return;
      this._credentialsUI.InitFromCredentials((ILogonCredentials) this.Settings);
    }

    protected override void UpdateSettings()
    {
      base.UpdateSettings();
      if (this._credentialsUI == null)
        return;
      this.Settings.ProfileName.UpdateValue(this._credentialsUI.ProfileComboBox.SelectedValue.ProfileName, this._credentialsUI.ProfileComboBox.SelectedValue.ProfileScope);
      this.Settings.UserName.Value = this._credentialsUI.UserNameTextBox.Text;
      if (this._credentialsUI.PasswordChanged)
        this.Settings.Password.SetPlainText(this._credentialsUI.PasswordTextBox.Text);
      this.Settings.Domain.Value = this._credentialsUI.DomainTextBox.Text;
    }
  }
}
