// Decompiled with JetBrains decompiler
// Type: RdcMan.MenuHelper
// Assembly: RDCMan, Version=2.7.1406.0, Culture=neutral, PublicKeyToken=null
// MVID: 984CDC7A-116B-406E-B519-0DEE541B9019
// Assembly location: C:\Program Files (x86)\Microsoft\Remote Desktop Connection Manager\RDCMan.exe

using MSTSCLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace RdcMan
{
  public static class MenuHelper
  {
    public static void AddSessionMenuItems(ContextMenuStrip menu, ServerBase server)
    {
      bool isConnected = server.IsConnected;
      ToolStripMenuItem toolStripMenuItem1 = (ToolStripMenuItem) new DelegateMenuItem("&Connect server", MenuNames.SessionConnect, new Action(((RdcTreeNode) server).Connect));
      toolStripMenuItem1.Enabled = !isConnected;
      menu.Items.Add((ToolStripItem) toolStripMenuItem1);
      ToolStripMenuItem toolStripMenuItem2 = (ToolStripMenuItem) new DelegateMenuItem("Connect server &as...", MenuNames.SessionConnectAs, new Action(((RdcTreeNode) server).DoConnectAs));
      toolStripMenuItem2.Enabled = !isConnected;
      menu.Items.Add((ToolStripItem) toolStripMenuItem2);
      ToolStripMenuItem toolStripMenuItem3 = (ToolStripMenuItem) new DelegateMenuItem("R&econnect server", MenuNames.SessionReconnect, new Action(((RdcTreeNode) server).Reconnect));
      toolStripMenuItem3.Enabled = isConnected;
      menu.Items.Add((ToolStripItem) toolStripMenuItem3);
      menu.Items.Add("-");
      ToolStripMenuItem parentItem1 = menu.Items.Add("Send keys", MenuNames.SessionSendKeys);
      parentItem1.Enabled = isConnected;
      MenuHelper.AddSendKeysMenuItems(parentItem1, (Func<ServerBase>) (() => server));
      if (RdpClient.SupportsRemoteSessionActions)
      {
        ToolStripMenuItem parentItem2 = menu.Items.Add("Remote actions", MenuNames.SessionRemoteActions);
        parentItem2.Enabled = isConnected;
        MenuHelper.AddRemoteActionsMenuItems(parentItem2, (Func<ServerBase>) (() => server));
      }
      menu.Items.Add("-");
      ToolStripMenuItem toolStripMenuItem4 = (ToolStripMenuItem) new DelegateMenuItem("&Disconnect server", MenuNames.SessionDisconnect, new Action(((RdcTreeNode) server).Disconnect));
      toolStripMenuItem4.Enabled = isConnected;
      menu.Items.Add((ToolStripItem) toolStripMenuItem4);
      menu.Items.Add("-");
      ToolStripMenuItem toolStripMenuItem5 = (ToolStripMenuItem) new DelegateMenuItem("Log off server", MenuNames.SessionLogOff, new Action(((RdcTreeNode) server).LogOff));
      toolStripMenuItem5.Enabled = !Policies.DisableLogOff && isConnected;
      menu.Items.Add((ToolStripItem) toolStripMenuItem5);
      menu.Items.Add((ToolStripItem) new DelegateMenuItem("&List sessions", MenuNames.SessionListSessions, (Action) (() => Program.ShowForm((Form) new ListSessionsForm(server)))));
    }

    public static void AddSendKeysMenuItems(
      ToolStripMenuItem parentItem,
      Func<ServerBase> getServer)
    {
      SendKeysMenuItem[] sendKeysMenuItemArray = new SendKeysMenuItem[4]
      {
        new SendKeysMenuItem("Security dialog", new Keys[3]
        {
          Keys.ControlKey,
          Keys.Menu,
          Keys.Delete
        }),
        new SendKeysMenuItem("Window menu", new Keys[2]
        {
          Keys.Menu,
          Keys.Space
        }),
        new SendKeysMenuItem("Task manager", new Keys[3]
        {
          Keys.ControlKey,
          Keys.ShiftKey,
          Keys.Escape
        }),
        new SendKeysMenuItem("Start menu", new Keys[2]
        {
          Keys.ControlKey,
          Keys.Escape
        })
      };
      foreach (SendKeysMenuItem sendKeysMenuItem in new List<SendKeysMenuItem>((IEnumerable<SendKeysMenuItem>) sendKeysMenuItemArray))
      {
        sendKeysMenuItem.Click += (EventHandler) ((sender, e) => SendKeys.Send((sender as SendKeysMenuItem).KeyCodes, getServer()));
        parentItem.DropDownItems.Add((ToolStripItem) sendKeysMenuItem);
      }
    }

    public static void AddRemoteActionsMenuItems(
      ToolStripMenuItem parentItem,
      Func<ServerBase> getServer)
    {
      ToolStripMenuItem[] toolStripMenuItemArray1 = new ToolStripMenuItem[5];
      ToolStripMenuItem[] toolStripMenuItemArray2 = toolStripMenuItemArray1;
      ToolStripMenuItem toolStripMenuItem1 = new ToolStripMenuItem("App commands");
      toolStripMenuItem1.Tag = (object) RemoteSessionActionType.RemoteSessionActionAppbar;
      ToolStripMenuItem toolStripMenuItem2 = toolStripMenuItem1;
      toolStripMenuItemArray2[0] = toolStripMenuItem2;
      ToolStripMenuItem[] toolStripMenuItemArray3 = toolStripMenuItemArray1;
      ToolStripMenuItem toolStripMenuItem3 = new ToolStripMenuItem("Charms");
      toolStripMenuItem3.Tag = (object) RemoteSessionActionType.RemoteSessionActionCharms;
      ToolStripMenuItem toolStripMenuItem4 = toolStripMenuItem3;
      toolStripMenuItemArray3[1] = toolStripMenuItem4;
      ToolStripMenuItem[] toolStripMenuItemArray4 = toolStripMenuItemArray1;
      ToolStripMenuItem toolStripMenuItem5 = new ToolStripMenuItem("Snap");
      toolStripMenuItem5.Tag = (object) RemoteSessionActionType.RemoteSessionActionSnap;
      ToolStripMenuItem toolStripMenuItem6 = toolStripMenuItem5;
      toolStripMenuItemArray4[2] = toolStripMenuItem6;
      ToolStripMenuItem[] toolStripMenuItemArray5 = toolStripMenuItemArray1;
      ToolStripMenuItem toolStripMenuItem7 = new ToolStripMenuItem("Switch apps");
      toolStripMenuItem7.Tag = (object) RemoteSessionActionType.RemoteSessionActionAppSwitch;
      ToolStripMenuItem toolStripMenuItem8 = toolStripMenuItem7;
      toolStripMenuItemArray5[3] = toolStripMenuItem8;
      ToolStripMenuItem[] toolStripMenuItemArray6 = toolStripMenuItemArray1;
      ToolStripMenuItem toolStripMenuItem9 = new ToolStripMenuItem("Start");
      toolStripMenuItem9.Tag = (object) RemoteSessionActionType.RemoteSessionActionStartScreen;
      ToolStripMenuItem toolStripMenuItem10 = toolStripMenuItem9;
      toolStripMenuItemArray6[4] = toolStripMenuItem10;
      foreach (ToolStripMenuItem toolStripMenuItem11 in new List<ToolStripMenuItem>((IEnumerable<ToolStripMenuItem>) toolStripMenuItemArray1))
      {
        toolStripMenuItem11.Click += (EventHandler) ((sender, e) => getServer().ServerNode.SendRemoteAction((RemoteSessionActionType) (sender as ToolStripMenuItem).Tag));
        parentItem.DropDownItems.Add((ToolStripItem) toolStripMenuItem11);
      }
    }

    public static void AddDockingMenuItems(ContextMenuStrip menu, ServerBase server)
    {
      bool isConnected = server.IsConnected;
      bool clientFullScreen = server.IsClientFullScreen;
      ToolStripMenuItem toolStripMenuItem1 = (ToolStripMenuItem) new DelegateMenuItem("&Full screen", MenuNames.SessionFullScreen, (Action) (() =>
      {
        ServerTree.Instance.SelectedNode = (TreeNode) server;
        server.GoFullScreen();
      }));
      toolStripMenuItem1.Enabled = isConnected && !clientFullScreen;
      menu.Items.Add((ToolStripItem) toolStripMenuItem1);
      ToolStripMenuItem toolStripMenuItem2 = (ToolStripMenuItem) new DelegateMenuItem("&Undock", MenuNames.SessionUndock, new Action(server.Undock));
      toolStripMenuItem2.Enabled = server.IsClientDocked && !clientFullScreen;
      menu.Items.Add((ToolStripItem) toolStripMenuItem2);
      ToolStripMenuItem toolStripMenuItem3 = (ToolStripMenuItem) new DelegateMenuItem("Undoc&k and connect", MenuNames.SessionUndockAndConnect, (Action) (() =>
      {
        server.Undock();
        server.Connect();
      }));
      toolStripMenuItem3.Enabled = server.IsClientDocked && !isConnected && !clientFullScreen;
      menu.Items.Add((ToolStripItem) toolStripMenuItem3);
      ToolStripMenuItem toolStripMenuItem4 = (ToolStripMenuItem) new DelegateMenuItem("D&ock", MenuNames.SessionDock, new Action(server.Dock));
      toolStripMenuItem4.Enabled = server.IsClientUndocked;
      menu.Items.Add((ToolStripItem) toolStripMenuItem4);
    }

    public static void AddMaintenanceMenuItems(ContextMenuStrip menu, ServerBase server)
    {
      ToolStripMenuItem toolStripMenuItem1 = (ToolStripMenuItem) new DelegateMenuItem("Remo&ve server", MenuNames.EditRemove, (Action) (() => ServerTree.Instance.ConfirmRemove((RdcTreeNode) server, true)));
      toolStripMenuItem1.Enabled = server.CanRemove(false);
      menu.Items.Add((ToolStripItem) toolStripMenuItem1);
      menu.Items.Add("-");
      ToolStripMenuItem toolStripMenuItem2 = (ToolStripMenuItem) new DelegateMenuItem("Add to favorites", MenuNames.EditAddToFavorites, (Action) (() => FavoritesGroup.Instance.AddReference(server)));
      toolStripMenuItem2.Enabled = server.AllowEdit(false);
      menu.Items.Add((ToolStripItem) toolStripMenuItem2);
      menu.Items.Add("-");
      ToolStripMenuItem toolStripMenuItem3 = (ToolStripMenuItem) new DelegateMenuItem("P&roperties", MenuNames.EditProperties, (Action) (() => server.DoPropertiesDialog()));
      toolStripMenuItem3.Enabled = server.HasProperties;
      menu.Items.Add((ToolStripItem) toolStripMenuItem3);
    }

    public static void ConnectTo()
    {
      using (ConnectToDialog dialog = ConnectToDialog.NewConnectToDialog((Form) Program.TheForm))
      {
        if (dialog.ShowDialog() != DialogResult.OK)
          return;
        Server server = (Server) TemporaryServer.Create(dialog);
        server.Connect();
        ServerTree.Instance.SelectedNode = (TreeNode) server;
      }
    }

    public static void FindServers()
    {
      using (FindServersDialog findServersDialog = new FindServersDialog())
      {
        if (findServersDialog.ShowDialog() != DialogResult.OK)
          return;
        ServerBase serverBase = findServersDialog.SelectedServers.FirstOrDefault<ServerBase>();
        if (serverBase == null)
          return;
        ServerTree.Instance.SelectedNode = (TreeNode) serverBase;
      }
    }

    public static void AddFavorite(RdcTreeNode node)
    {
      if (!(node is ServerBase serverBase))
        return;
      FavoritesGroup.Instance.AddReference(serverBase);
    }

    public static void ShowGlobalOptionsDialog()
    {
      using (GlobalOptionsDialog globalOptionsDialog = GlobalOptionsDialog.New())
      {
        if (globalOptionsDialog.ShowDialog() != DialogResult.OK)
          return;
        globalOptionsDialog.UpdatePreferences();
        Program.Preferences.NeedToSave = true;
        Program.Preferences.Save();
        Program.TheForm.LockWindowSize();
        Program.TheForm.SetMainMenuVisibility();
        ServerTree.Instance.UpdateColors();
        ServerTree.Instance.SortAllNodes();
        ServerTree.Instance.OnGroupChanged(ServerTree.Instance.RootNode, ChangeType.PropertyChanged);
        Program.TheForm.UpdateAutoSaveTimer();
      }
    }
  }
}
