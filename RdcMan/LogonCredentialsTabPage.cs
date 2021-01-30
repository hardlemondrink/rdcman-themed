// Decompiled with JetBrains decompiler
// Type: RdcMan.LogonCredentialsTabPage
// Assembly: RDCMan, Version=2.7.1406.0, Culture=neutral, PublicKeyToken=null
// MVID: 984CDC7A-116B-406E-B519-0DEE541B9019
// Assembly location: C:\Program Files (x86)\Microsoft\Remote Desktop Connection Manager\RDCMan.exe

using System;
using System.Windows.Forms;

namespace RdcMan
{
  public class LogonCredentialsTabPage : CredentialsTabPage<LogonCredentials>
  {
    public LogonCredentialsTabPage(TabbedSettingsDialog dialog, LogonCredentials settings)
      : base(dialog, settings)
    {
    }

    public void CreateControls(LogonCredentialsDialogOptions options)
    {
      int tabIndex = 0;
      int rowIndex = 0;
      if ((options & LogonCredentialsDialogOptions.AllowInheritance) != LogonCredentialsDialogOptions.None)
      {
        this.CreateInheritanceControl(ref rowIndex, ref tabIndex);
        this.InheritanceControl.EnabledChanged += (Action<bool>) (enabled => this._credentialsUI.EnableDisableControls(enabled));
      }
      this._credentialsUI = new CredentialsUI(this.InheritanceControl);
      this._credentialsUI.AddControlsToParent((Control) this, options, ref rowIndex, ref tabIndex);
    }
  }
}
