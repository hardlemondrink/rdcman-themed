// Decompiled with JetBrains decompiler
// Type: RdcMan.AddCredentialsDialog
// Assembly: RDCMan, Version=2.7.1406.0, Culture=neutral, PublicKeyToken=null
// MVID: 984CDC7A-116B-406E-B519-0DEE541B9019
// Assembly location: C:\Program Files (x86)\Microsoft\Remote Desktop Connection Manager\RDCMan.exe

using System;
using System.Windows.Forms;

namespace RdcMan
{
  internal class AddCredentialsDialog : LogonSettingsDialog
  {
    private FileGroup _file;
    private RdcTextBox _profileNameTextBox;
    private bool _profileNameUserCreated;

    public AddCredentialsDialog(RdcTreeNode node)
      : base("Add Credentials", "Save")
    {
      this._file = node.FileGroup;
      int rowIndex = 0;
      int tabIndex = 0;
      this._profileNameTextBox = FormTools.AddLabeledTextBox((Control) this, "Profile &name:", ref rowIndex, ref tabIndex);
      this._profileNameTextBox.Enabled = true;
      this._profileNameTextBox.TextChanged += (EventHandler) ((s, e) => this._profileNameUserCreated = true);
      this._profileNameTextBox.Validate = new Func<string>(this.ValidateProfileName);
      this._logonCredentialsUI.AddControlsToParent((Control) this, LogonCredentialsDialogOptions.None, ref rowIndex, ref tabIndex);
      this._logonCredentialsUI.UserNameTextBox.TextChanged += new EventHandler(this.CredentialsChanged);
      this._logonCredentialsUI.DomainTextBox.TextChanged += new EventHandler(this.CredentialsChanged);
      this._logonCredentialsUI.EnableDisableControls(true);
      this.FinalizeLayout(rowIndex, tabIndex);
    }

    public new string ProfileName => this._profileNameTextBox.Text;

    public new ProfileScope ProfileScope => this._file != null ? ProfileScope.File : ProfileScope.Global;

    private string ValidateProfileName()
    {
      this._profileNameTextBox.Text = this._profileNameTextBox.Text.Trim();
      if (string.IsNullOrEmpty(this._profileNameTextBox.Text))
        return "Please enter a profile name";
      if (LogonCredentials.IsCustomProfile(this.ProfileName))
        return "'{0}' is a reserved profile name".InvariantFormat((object) "Custom");
      CredentialsStore credentialsProfiles = Program.CredentialsProfiles;
      string str = "Global";
      if (this.ProfileScope == ProfileScope.File)
      {
        credentialsProfiles = this._file.CredentialsProfiles;
        str = this._file.Text;
      }
      if (credentialsProfiles.Contains(this.ProfileName))
      {
        if (FormTools.YesNoDialog(this.ProfileName + " already exists in " + str + Environment.NewLine + "Update?", MessageBoxDefaultButton.Button2) != DialogResult.Yes)
          return "Profile exists";
      }
      return (string) null;
    }

    private void CredentialsChanged(object sender, EventArgs e)
    {
      if (this._profileNameUserCreated)
        return;
      this._profileNameTextBox.Text = CredentialsUI.GetQualifiedUserName(this._logonCredentialsUI.UserNameTextBox.Text, this._logonCredentialsUI.DomainTextBox.Text);
      this._profileNameUserCreated = false;
    }
  }
}
