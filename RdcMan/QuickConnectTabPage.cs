// Decompiled with JetBrains decompiler
// Type: RdcMan.QuickConnectTabPage
// Assembly: RDCMan, Version=2.7.1406.0, Culture=neutral, PublicKeyToken=null
// MVID: 984CDC7A-116B-406E-B519-0DEE541B9019
// Assembly location: C:\Program Files (x86)\Microsoft\Remote Desktop Connection Manager\RDCMan.exe

using System.Windows.Forms;

namespace RdcMan
{
  public class QuickConnectTabPage : LogonCredentialsTabPage
  {
    public QuickConnectTabPage(TabbedSettingsDialog dialog, LogonCredentials settings)
      : base(dialog, settings)
    {
    }

    public RdcTextBox ServerNameTextBox { get; private set; }

    public void CreateControls(bool serverName, FileGroup fileGroup)
    {
      int tabIndex = 0;
      int rowIndex = 0;
      if (serverName)
      {
        this.ServerNameTextBox = FormTools.AddLabeledTextBox((Control) this, "&Server name:", ref rowIndex, ref tabIndex);
        this.ServerNameTextBox.Enabled = true;
      }
      this._credentialsUI = new CredentialsUI(this.InheritanceControl);
      this._credentialsUI.AddControlsToParent((Control) this, LogonCredentialsDialogOptions.ShowProfiles, ref rowIndex, ref tabIndex);
      this._credentialsUI.PopulateCredentialsProfiles(fileGroup);
    }

    public void OnShown()
    {
      if (this.ServerNameTextBox != null)
        this.ServerNameTextBox.Focus();
      else
        this._credentialsUI.ProfileComboBox.Focus();
    }
  }
}
