// Decompiled with JetBrains decompiler
// Type: RdcMan.ConnectToDialog
// Assembly: RDCMan, Version=2.7.1406.0, Culture=neutral, PublicKeyToken=null
// MVID: 984CDC7A-116B-406E-B519-0DEE541B9019
// Assembly location: C:\Program Files (x86)\Microsoft\Remote Desktop Connection Manager\RDCMan.exe

using System.Windows.Forms;

namespace RdcMan
{
  public class ConnectToDialog : QuickConnectDialog
  {
    private ConnectToDialog(string title, string buttonText, Form parentForm)
      : base(title, buttonText, parentForm)
    {
    }

    public TemporaryServer Server { get; private set; }

    public static ConnectToDialog NewConnectToDialog(Form parentForm)
    {
      ConnectToDialog connectToDialog = new ConnectToDialog("Connect To", "Connect", parentForm);
      connectToDialog.Server = TemporaryServer.CreateForQuickConnect();
      connectToDialog.CreateControls(true, connectToDialog.Server.LogonCredentials, connectToDialog.Server.ConnectionSettings, (FileGroup) null);
      return connectToDialog;
    }
  }
}
