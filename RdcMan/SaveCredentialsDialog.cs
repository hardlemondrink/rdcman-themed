// Decompiled with JetBrains decompiler
// Type: RdcMan.SaveCredentialsDialog
// Assembly: RDCMan, Version=2.7.1406.0, Culture=neutral, PublicKeyToken=null
// MVID: 984CDC7A-116B-406E-B519-0DEE541B9019
// Assembly location: C:\Program Files (x86)\Microsoft\Remote Desktop Connection Manager\RDCMan.exe

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace RdcMan
{
  internal class SaveCredentialsDialog : RdcDialog
  {
    private RdcTextBox _profileNameTextBox;
    private ValueComboBox<ProfileScope> _locationComboBox;
    private FileGroup _file;

    public string ProfileName => this._profileNameTextBox.Text;

    public ProfileScope ProfileScope => this._locationComboBox.SelectedValue;

    public SaveCredentialsDialog(FileGroup file, string name)
      : base("Save profile for " + name, "Save")
    {
      this._file = file;
      this.InitializeComponent(name);
    }

    private void InitializeComponent(string name)
    {
      this.Size = new Size(512, 150);
      int rowIndex = 0;
      int tabIndex = 0;
      this._profileNameTextBox = FormTools.AddLabeledTextBox((Control) this, "Profile &name", ref rowIndex, ref tabIndex);
      this._profileNameTextBox.Enabled = true;
      this._profileNameTextBox.Text = name;
      this._profileNameTextBox.Validate = new Func<string>(this.ValidateProfileName);
      this._locationComboBox = FormTools.AddLabeledValueDropDown<ProfileScope>((Control) this, "&Location", ref rowIndex, ref tabIndex, (Func<ProfileScope, string>) null, (IEnumerable<ProfileScope>) null);
      this._locationComboBox.AddItem("Global", ProfileScope.Global);
      this._locationComboBox.SelectedIndex = 0;
      if (this._file != null)
      {
        this._locationComboBox.AddItem(this._file.Text, ProfileScope.File);
        this._locationComboBox.SelectedIndex = 1;
      }
      this.InitButtons();
      this.ScaleAndLayout();
    }

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
  }
}
