// Decompiled with JetBrains decompiler
// Type: RdcMan.QuickConnectDialog
// Assembly: RDCMan, Version=2.7.1406.0, Culture=neutral, PublicKeyToken=null
// MVID: 984CDC7A-116B-406E-B519-0DEE541B9019
// Assembly location: C:\Program Files (x86)\Microsoft\Remote Desktop Connection Manager\RDCMan.exe

using System;
using System.Windows.Forms;

namespace RdcMan
{
  public class QuickConnectDialog : TabbedSettingsDialog
  {
    protected QuickConnectDialog(string title, string buttonText, Form parentForm)
      : base(title, buttonText, parentForm)
    {
    }

    public QuickConnectTabPage QuickConnectTabPage { get; private set; }

    public void CreateControls(
      bool inputServerName,
      LogonCredentials logonCredentials,
      ConnectionSettings connectionSettings,
      FileGroup fileGroup)
    {
      this.QuickConnectTabPage = new QuickConnectTabPage((TabbedSettingsDialog) this, logonCredentials);
      this.QuickConnectTabPage.CreateControls(inputServerName, fileGroup);
      this.AddTabPage((TabPage) this.QuickConnectTabPage);
      connectionSettings.InheritSettingsType.Mode = InheritanceMode.Disabled;
      this.AddTabPage(connectionSettings.CreateTabPage((TabbedSettingsDialog) this));
      this.InitButtons();
    }

    protected override void ShownCallback(object sender, EventArgs e)
    {
      base.ShownCallback(sender, e);
      this.QuickConnectTabPage.OnShown();
    }
  }
}
