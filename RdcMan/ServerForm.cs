// Decompiled with JetBrains decompiler
// Type: RdcMan.ServerForm
// Assembly: RDCMan, Version=2.7.1406.0, Culture=neutral, PublicKeyToken=null
// MVID: 984CDC7A-116B-406E-B519-0DEE541B9019
// Assembly location: C:\Program Files (x86)\Microsoft\Remote Desktop Connection Manager\RDCMan.exe

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace RdcMan
{
  internal class ServerForm : RdcBaseForm, IUndockedServerForm
  {
    private static readonly Dictionary<Keys, Action<ServerForm>> Shortcuts;
    private static readonly List<ServerForm> ServerForms = new List<ServerForm>();
    private ToolStripMenuItem _sessionConnectServerMenuItem;
    private ToolStripMenuItem _sessionConnectServerAsMenuItem;
    private ToolStripMenuItem _sessionReconnectServerMenuItem;
    private ToolStripMenuItem _sessionSendKeysMenuItem;
    private ToolStripMenuItem _sessionRemoteActionsMenuItem;
    private ToolStripMenuItem _sessionDisconnectServerMenuItem;
    private ToolStripMenuItem _sessionFullScreenMenuItem;
    private ToolStripMenuItem _sessionScreenCaptureMenuItem;
    private Size _clientSize;
    private readonly Server _server;

    static ServerForm()
    {
      ServerForm.Shortcuts = new Dictionary<Keys, Action<ServerForm>>()
      {
        {
          Keys.Return,
          (Action<ServerForm>) (f => f._server.Connect())
        },
        {
          Keys.Return | Keys.Shift,
          (Action<ServerForm>) (f => f._server.DoConnectAs())
        },
        {
          Keys.Return | Keys.Alt,
          (Action<ServerForm>) (f => f._server.DoPropertiesDialog())
        }
      };
      ServerTree.Instance.GroupChanged += new Action<GroupChangedEventArgs>(ServerForm.OnGroupChanged);
      ServerTree.Instance.ServerChanged += new Action<ServerChangedEventArgs>(ServerForm.OnServerChanged);
    }

    private static void OnGroupChanged(GroupChangedEventArgs e)
    {
      if (!e.ChangeType.HasFlag((Enum) ChangeType.PropertyChanged))
        return;
      using (Helpers.Timer("updating server form settings from group {0}", (object) e.Group.Text))
      {
        if (e.Group == ServerTree.Instance.RootNode)
          ServerForm.UpdateFromGlobalSettings();
        ServerForm.UpdateFromServerSettings();
      }
    }

    private static void OnServerChanged(ServerChangedEventArgs e)
    {
      if (!e.ChangeType.HasFlag((Enum) ChangeType.PropertyChanged))
        return;
      using (Helpers.Timer("updating server form settings from server {0}", (object) e.Server.DisplayName))
        ServerForm.UpdateFromServerSettings();
    }

    private static void UpdateFromServerSettings() => ServerForm.ServerForms.ForEach((Action<ServerForm>) (f =>
    {
      f._server.InheritSettings();
      f._server.SetClientSizeProperties();
      f.SetTitle();
    }));

    public ServerForm(Server server)
    {
      this._server = server;
      server.InheritSettings();
      this.Icon = Program.TheForm.Icon;
      this.SetTitle();
      Size size = server.RemoteDesktopSettings.DesktopSizeSameAsClientAreaSize.Value || server.RemoteDesktopSettings.DesktopSizeFullScreen.Value ? Program.TheForm.GetClientSize() : server.RemoteDesktopSettings.DesktopSize.Value;
      this.CreateMainMenu();
      this.SetMainMenuVisibility();
      this.SetClientSize(size);
      this.ScaleAndLayout();
      this.Controls.Add(this._server.Client.Control);
      this._server.SetClientSizeProperties();
      ServerForm.ServerForms.Add(this);
    }

    MenuStrip IUndockedServerForm.MainMenuStrip => (MenuStrip) this._menuStrip;

    ServerBase IUndockedServerForm.Server => (ServerBase) this._server;

    private static void UpdateFromGlobalSettings() => ServerForm.ServerForms.ForEach((Action<ServerForm>) (f =>
    {
      f.SetMainMenuVisibility();
      f.SetClientSize(f._clientSize);
    }));

    public override void SetClientSize(Size size)
    {
      int num = Program.Preferences.HideMainMenu ? 0 : this._menuPanel.Height;
      this.ClientSize = new Size(size.Width, size.Height + num);
    }

    public override Size GetClientSize() => this._clientSize;

    protected override void OnShown(EventArgs e) => this._server.Client.Control.Show();

    protected override void OnClosed(EventArgs e)
    {
      ServerForm.ServerForms.Remove(this);
      this._server.LeaveFullScreen();
      this.Controls.Remove(this._server.Client.Control);
      this._server.Dock();
    }

    protected override void OnSizeChanged(EventArgs e)
    {
      base.OnSizeChanged(e);
      this._clientSize = new Size(this.ClientSize.Width, this.ClientSize.Height - (Program.Preferences.HideMainMenu ? 0 : this._menuPanel.Height));
      this.LayoutContent();
    }

    protected override void LayoutContent()
    {
      this._server.Client.Control.Bounds = new Rectangle(0, Program.Preferences.HideMainMenu ? 0 : this._menuPanel.Height, this._clientSize.Width, this._clientSize.Height);
      this._menuPanel.Width = this.ClientSize.Width;
    }

    protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
    {
      Action<ServerForm> action;
      if (this._menuStrip.IsActive || !ServerForm.Shortcuts.TryGetValue(keyData, out action))
        return base.ProcessCmdKey(ref msg, keyData);
      action(this);
      return true;
    }

    protected void CreateMainMenu()
    {
      this._sessionConnectServerMenuItem = (ToolStripMenuItem) new DelegateMenuItem("&Connect server", MenuNames.SessionConnect, "Enter", (Action) (() => this._server.Connect()));
      this._sessionConnectServerAsMenuItem = (ToolStripMenuItem) new DelegateMenuItem("Connect server &as...", MenuNames.SessionConnectAs, "Shift+Enter", (Action) (() => this._server.DoConnectAs()));
      this._sessionReconnectServerMenuItem = (ToolStripMenuItem) new DelegateMenuItem("R&econnect server", MenuNames.SessionReconnect, (Action) (() => this._server.Reconnect()));
      this._sessionDisconnectServerMenuItem = (ToolStripMenuItem) new DelegateMenuItem("&Disconnect server", MenuNames.SessionDisconnect, (Action) (() => this._server.Disconnect()));
      this._sessionFullScreenMenuItem = (ToolStripMenuItem) new DelegateMenuItem("&Full screen", MenuNames.SessionFullScreen, (Action) (() => this._server.Client.MsRdpClient.FullScreen = true));
      DelegateMenuItem delegateMenuItem1 = new DelegateMenuItem("Doc&k", MenuNames.SessionDock, (Action) (() => this.Close()));
      this._sessionScreenCaptureMenuItem = (ToolStripMenuItem) new DelegateMenuItem("Screen capture", MenuNames.SessionScreenCapture, (Action) (() => this._server.ScreenCapture()));
      DelegateMenuItem delegateMenuItem2 = new DelegateMenuItem("P&roperties", MenuNames.EditProperties, "Alt+Enter", (Action) (() => this._server.DoPropertiesDialog()));
      ToolStripMenuItem toolStripMenuItem1 = this._menuStrip.Add("&Session", MenuNames.Session);
      toolStripMenuItem1.DropDownItems.Add((ToolStripItem) this._sessionConnectServerMenuItem);
      toolStripMenuItem1.DropDownItems.Add((ToolStripItem) this._sessionConnectServerAsMenuItem);
      toolStripMenuItem1.DropDownItems.Add((ToolStripItem) this._sessionReconnectServerMenuItem);
      toolStripMenuItem1.DropDownItems.Add("-");
      this._sessionSendKeysMenuItem = toolStripMenuItem1.DropDownItems.Add("Send keys", MenuNames.SessionSendKeys);
      MenuHelper.AddSendKeysMenuItems(this._sessionSendKeysMenuItem, (Func<ServerBase>) (() => (ServerBase) this._server));
      if (RdpClient.SupportsRemoteSessionActions)
      {
        this._sessionRemoteActionsMenuItem = toolStripMenuItem1.DropDownItems.Add("Remote actions", MenuNames.SessionRemoteActions);
        MenuHelper.AddRemoteActionsMenuItems(this._sessionRemoteActionsMenuItem, (Func<ServerBase>) (() => (ServerBase) this._server));
      }
      toolStripMenuItem1.DropDownItems.Add("-");
      toolStripMenuItem1.DropDownItems.Add((ToolStripItem) this._sessionDisconnectServerMenuItem);
      toolStripMenuItem1.DropDownItems.Add("-");
      toolStripMenuItem1.DropDownItems.Add((ToolStripItem) this._sessionFullScreenMenuItem);
      toolStripMenuItem1.DropDownItems.Add((ToolStripItem) delegateMenuItem1);
      toolStripMenuItem1.DropDownItems.Add("-");
      toolStripMenuItem1.DropDownItems.Add((ToolStripItem) this._sessionScreenCaptureMenuItem);
      toolStripMenuItem1.DropDownItems.Add("-");
      toolStripMenuItem1.DropDownItems.Add((ToolStripItem) delegateMenuItem2);
      ToolStripMenuItem toolStripMenuItem2 = this._menuStrip.Add("&View", MenuNames.View).DropDownItems.Add("&Client size", MenuNames.ViewClientSize);
      foreach (Size stockSiz in SizeHelper.StockSizes)
      {
        ClientSizeCheckedMenuItem sizeCheckedMenuItem = new ClientSizeCheckedMenuItem((RdcBaseForm) this, stockSiz);
        toolStripMenuItem2.DropDownItems.Add((ToolStripItem) sizeCheckedMenuItem);
      }
      toolStripMenuItem2.DropDownItems.Add((ToolStripItem) new CustomClientSizeCheckedMenuItem((RdcBaseForm) this, "&Custom"));
      toolStripMenuItem2.DropDownItems.Add((ToolStripItem) new ToolStripMenuItem("From remote desktop size", (Image) null, (EventHandler) ((s, e) => this.SetClientSize(this._server.IsConnected ? this._server.Client.DesktopSize : this._server.RemoteDesktopSettings.DesktopSize.Value))));
    }

    protected override void UpdateMainMenu()
    {
      this.UpdateMenuItems(this._menuStrip.Items);
      bool isConnected = this._server.IsConnected;
      this._sessionConnectServerMenuItem.Enabled = !isConnected;
      this._sessionConnectServerAsMenuItem.Enabled = !isConnected;
      this._sessionReconnectServerMenuItem.Enabled = isConnected;
      this._sessionSendKeysMenuItem.Enabled = isConnected;
      if (RdpClient.SupportsRemoteSessionActions)
        this._sessionRemoteActionsMenuItem.Enabled = isConnected;
      this._sessionDisconnectServerMenuItem.Enabled = isConnected;
      this._sessionFullScreenMenuItem.Enabled = isConnected;
      this._sessionScreenCaptureMenuItem.Enabled = isConnected;
    }

    private void SetTitle() => this.Text = this._server.GetQualifiedNameForUI();
  }
}
