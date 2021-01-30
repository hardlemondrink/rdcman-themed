// Decompiled with JetBrains decompiler
// Type: RdcMan.TemporaryServer
// Assembly: RDCMan, Version=2.7.1406.0, Culture=neutral, PublicKeyToken=null
// MVID: 984CDC7A-116B-406E-B519-0DEE541B9019
// Assembly location: C:\Program Files (x86)\Microsoft\Remote Desktop Connection Manager\RDCMan.exe

using System.Windows.Forms;

namespace RdcMan
{
  public class TemporaryServer : Server
  {
    protected TemporaryServer()
    {
    }

    public static TemporaryServer CreateForQuickConnect() => new TemporaryServer();

    public static TemporaryServer Create(ConnectToDialog dlg)
    {
      TemporaryServer server = dlg.Server;
      dlg.UpdateSettings();
      server.Properties.ServerName.Value = dlg.QuickConnectTabPage.ServerNameTextBox.Text;
      server.Properties.DisplayName.Value = dlg.QuickConnectTabPage.ServerNameTextBox.Text;
      server.LogonCredentials.InheritSettingsType.Mode = InheritanceMode.None;
      server.ConnectionSettings.InheritSettingsType.Mode = InheritanceMode.None;
      server.FinishConstruction((GroupBase) ConnectToGroup.Instance);
      ConnectToGroup.Instance.IsInTree = true;
      return server;
    }

    public override bool CanDropOnTarget(RdcTreeNode targetNode)
    {
      if (this.FileGroup != null)
        return base.CanDropOnTarget(targetNode);
      if (!(targetNode is GroupBase groupBase))
        groupBase = targetNode.Parent as GroupBase;
      GroupBase groupBase1 = groupBase;
      return groupBase1.DropBehavior() != DragDropEffects.Link && groupBase1.CanDropServers();
    }
  }
}
