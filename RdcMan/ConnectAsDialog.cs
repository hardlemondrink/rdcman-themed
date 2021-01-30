// Decompiled with JetBrains decompiler
// Type: RdcMan.ConnectAsDialog
// Assembly: RDCMan, Version=2.7.1406.0, Culture=neutral, PublicKeyToken=null
// MVID: 984CDC7A-116B-406E-B519-0DEE541B9019
// Assembly location: C:\Program Files (x86)\Microsoft\Remote Desktop Connection Manager\RDCMan.exe

using System.Windows.Forms;

namespace RdcMan
{
  public class ConnectAsDialog : QuickConnectDialog
  {
    private ConnectAsDialog(string title, string buttonText, Form parentForm)
      : base(title, buttonText, parentForm)
    {
    }

    public LogonCredentials LogonCredentials { get; private set; }

    public ConnectionSettings ConnectionSettings { get; private set; }

    public static ConnectAsDialog NewConnectAsDialog(
      RdcTreeNode node,
      Form parentForm)
    {
      ConnectAsDialog connectAsDialog = new ConnectAsDialog("Connect " + node.Text + " As", "Connect", parentForm);
      connectAsDialog.LogonCredentials = new LogonCredentials();
      connectAsDialog.ConnectionSettings = new ConnectionSettings();
      if (node.LogonCredentials != null)
        connectAsDialog.LogonCredentials.Copy((SettingsGroup) node.LogonCredentials);
      if (node.ConnectionSettings != null)
        connectAsDialog.ConnectionSettings.Copy((SettingsGroup) node.ConnectionSettings);
      connectAsDialog.CreateControls(false, connectAsDialog.LogonCredentials, connectAsDialog.ConnectionSettings, node.FileGroup);
      return connectAsDialog;
    }
  }
}
