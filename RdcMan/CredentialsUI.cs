// Decompiled with JetBrains decompiler
// Type: RdcMan.CredentialsUI
// Assembly: RDCMan, Version=2.7.1406.0, Culture=neutral, PublicKeyToken=null
// MVID: 984CDC7A-116B-406E-B519-0DEE541B9019
// Assembly location: C:\Program Files (x86)\Microsoft\Remote Desktop Connection Manager\RDCMan.exe

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace RdcMan
{
  public class CredentialsUI
  {
    public const char DomainSeparator = '\\';
    private const string DummyPassword = "1234567890";
    private FileGroup _file;
    private bool _usingCustomCredentials;
    private Button _saveProfileButton;
    private InheritanceControl _inheritSettings;
    private int _globalStoreChangeId;
    private int _fileStoreChangeId;

    public ValueComboBox<CredentialsProfile> ProfileComboBox { get; private set; }

    public RdcTextBox UserNameTextBox { get; private set; }

    public RdcTextBox DomainTextBox { get; private set; }

    public RdcTextBox PasswordTextBox { get; private set; }

    public bool PasswordChanged { get; private set; }

    private static bool HasDomainUser(string userName) => userName.IndexOf('\\') != -1;

    public static string GetUserName(string userName)
    {
      int num = userName.IndexOf('\\');
      return num == -1 ? userName : userName.Substring(num + 1);
    }

    public static string GetQualifiedUserName(string userName, string domain)
    {
      string userName1 = userName;
      if (!CredentialsUI.HasDomainUser(userName1) && !string.IsNullOrEmpty(domain))
        userName1 = domain + "\\" + userName1;
      return userName1;
    }

    public static string GetLoggedInUser() => CredentialsUI.GetQualifiedUserName(Environment.UserName, Environment.UserDomainName);

    public CredentialsUI(InheritanceControl inheritSettings)
    {
      this._inheritSettings = inheritSettings;
      this.PasswordChanged = true;
    }

    public void AddControlsToParent(
      Control parent,
      LogonCredentialsDialogOptions options,
      ref int rowIndex,
      ref int tabIndex)
    {
      if ((options & LogonCredentialsDialogOptions.ShowProfiles) != LogonCredentialsDialogOptions.None)
      {
        this.ProfileComboBox = FormTools.AddLabeledValueDropDown<CredentialsProfile>(parent, "Profile", ref rowIndex, ref tabIndex, (Func<CredentialsProfile, string>) null, (IEnumerable<CredentialsProfile>) null);
        this.ProfileComboBox.SelectedIndexChanged += new EventHandler(this.OnProfileChanged);
        this.ProfileComboBox.VisibleChanged += new EventHandler(this.OnProfileVisible);
        Button button = new Button();
        button.TabIndex = tabIndex++;
        button.Text = "Save";
        this._saveProfileButton = button;
        this._saveProfileButton.Location = new Point(this.ProfileComboBox.Right - this._saveProfileButton.Width, this.ProfileComboBox.Location.Y - 1);
        this._saveProfileButton.Click += new EventHandler(this.SaveProfileButton_Click);
        parent.Controls.Add((Control) this._saveProfileButton);
        this.ProfileComboBox.Width -= this._saveProfileButton.Width;
      }
      this.UserNameTextBox = FormTools.AddLabeledTextBox(parent, "&User name:", ref rowIndex, ref tabIndex);
      this.UserNameTextBox.TextChanged += new EventHandler(this.OnUserNameChanged);
      this.PasswordTextBox = FormTools.AddLabeledTextBox(parent, "&Password:", ref rowIndex, ref tabIndex);
      this.PasswordTextBox.PasswordChar = '●';
      this.PasswordTextBox.TextChanged += new EventHandler(this.OnPasswordChanged);
      this.DomainTextBox = FormTools.AddLabeledTextBox(parent, "&Domain:", ref rowIndex, ref tabIndex);
    }

    public void PopulateCredentialsProfiles(FileGroup file)
    {
      if (file != null && this._file == file)
        return;
      this._file = file;
      this.PopulateCredentialsProfilesWorker();
    }

    public void InitFromCredentials(ILogonCredentials credentials)
    {
      if (this.ProfileComboBox != null)
      {
        this._usingCustomCredentials = LogonCredentials.IsCustomProfile(credentials.ProfileName);
        this.ProfileComboBox.SelectedIndex = this.ProfileComboBox.FindItem(LogonCredentials.ConstructQualifiedName(credentials));
      }
      this.UserNameTextBox.Text = credentials.UserName;
      this.InitPassword(credentials.Password);
      this.DomainTextBox.Text = credentials.Domain;
    }

    public void InitPassword(PasswordSetting password)
    {
      if (password != null && password.IsDecrypted && !string.IsNullOrEmpty(password.Value))
      {
        this.PasswordTextBox.Text = "1234567890";
        this.PasswordChanged = false;
      }
      else
        this.PasswordTextBox.Text = string.Empty;
    }

    public void EnableDisableControls(bool enable)
    {
      enable = ((enable ? 1 : 0) & (this._inheritSettings == null ? 1 : (!this._inheritSettings.FromParentCheck.Checked ? 1 : 0))) != 0;
      if (this.ProfileComboBox != null)
      {
        this.ProfileComboBox.Enabled = enable;
        enable &= this._usingCustomCredentials;
      }
      if (this._saveProfileButton != null)
        this._saveProfileButton.Enabled = enable;
      this.UserNameTextBox.Enabled = enable;
      this.DomainTextBox.Enabled = enable;
      this.PasswordTextBox.Enabled = enable;
      this.OnUserNameChanged((object) null, (EventArgs) null);
    }

    private void OnUserNameChanged(object sender, EventArgs e)
    {
      if (this._inheritSettings != null && this._inheritSettings.FromParentCheck.Checked)
        return;
      int length = this.UserNameTextBox.Text.IndexOf('\\');
      if (length == -1)
      {
        this.DomainTextBox.Enabled = this.UserNameTextBox.Enabled;
      }
      else
      {
        this.DomainTextBox.Enabled = false;
        this.DomainTextBox.Text = this.UserNameTextBox.Text.Substring(0, length);
      }
    }

    private void OnPasswordChanged(object sender, EventArgs e) => this.PasswordChanged = true;

    private void OnProfileChanged(object sender, EventArgs e)
    {
      if (this._inheritSettings != null && this._inheritSettings.FromParentCheck.Checked)
        return;
      ILogonCredentials selectedValue = (ILogonCredentials) this.ProfileComboBox.SelectedValue;
      this._usingCustomCredentials = LogonCredentials.IsCustomProfile(selectedValue.ProfileName);
      this.EnableDisableControls(true);
      if (this._usingCustomCredentials)
      {
        this.UserNameTextBox.Text = Environment.UserName;
        this.InitPassword((PasswordSetting) null);
        this.DomainTextBox.Text = Environment.UserDomainName;
      }
      else
      {
        this.UserNameTextBox.Text = selectedValue.UserName;
        this.InitPassword(selectedValue.Password);
        this.DomainTextBox.Text = selectedValue.Domain;
      }
    }

    private void OnProfileVisible(object sender, EventArgs e) => this.PopulateCredentialsProfilesIfChanged();

    private void PopulateCredentialsProfilesIfChanged()
    {
      if (this._globalStoreChangeId == Program.CredentialsProfiles.ChangeId && (this._file == null || this._fileStoreChangeId == this._file.CredentialsProfiles.ChangeId))
        return;
      this.PopulateCredentialsProfilesWorker();
    }

    private void PopulateCredentialsProfilesWorker()
    {
      CredentialsProfile selectedValue = this.ProfileComboBox.SelectedValue;
      this.ProfileComboBox.ClearItems();
      this.ProfileComboBox.AddItem("Custom", new CredentialsProfile("Custom", ProfileScope.Local, string.Empty, string.Empty, string.Empty));
      this.ProfileComboBox.SelectedIndex = 0;
      this.PopulateComboFromStore(Program.CredentialsProfiles);
      this._globalStoreChangeId = Program.CredentialsProfiles.ChangeId;
      if (this._file != null)
      {
        this.PopulateComboFromStore(this._file.CredentialsProfiles);
        this._fileStoreChangeId = this._file.CredentialsProfiles.ChangeId;
      }
      this.ProfileComboBox.SelectedValue = selectedValue;
    }

    private void PopulateComboFromStore(CredentialsStore store)
    {
      foreach (CredentialsProfile profile in store.Profiles)
        this.ProfileComboBox.AddItem(profile.QualifiedName, profile);
    }

    private void SaveProfileButton_Click(object sender, EventArgs e)
    {
      using (SaveCredentialsDialog credentialsDialog = new SaveCredentialsDialog(this._file, CredentialsUI.GetQualifiedUserName(this.UserNameTextBox.Text, this.DomainTextBox.Text)))
      {
        if (credentialsDialog.ShowDialog() == DialogResult.OK)
        {
          ProfileScope profileScope = credentialsDialog.ProfileScope;
          CredentialsStore credentialsProfiles = Program.CredentialsProfiles;
          if (profileScope == ProfileScope.File)
            credentialsProfiles = this._file.CredentialsProfiles;
          string profileName = credentialsDialog.ProfileName;
          bool flag = !credentialsProfiles.Contains(profileName);
          CredentialsProfile newValue = new CredentialsProfile(profileName, profileScope, this.UserNameTextBox.Text, this.PasswordTextBox.Text, this.DomainTextBox.Text);
          credentialsProfiles[profileName] = newValue;
          string qualifiedName = newValue.QualifiedName;
          if (flag)
            this.ProfileComboBox.AddItem(qualifiedName, newValue);
          else
            this.ProfileComboBox.ReplaceItem(qualifiedName, newValue);
          this.ProfileComboBox.SelectedValue = newValue;
        }
      }
      this.ProfileComboBox.Focus();
    }
  }
}
