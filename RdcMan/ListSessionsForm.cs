// Decompiled with JetBrains decompiler
// Type: RdcMan.ListSessionsForm
// Assembly: RDCMan, Version=2.7.1406.0, Culture=neutral, PublicKeyToken=null
// MVID: 984CDC7A-116B-406E-B519-0DEE541B9019
// Assembly location: C:\Program Files (x86)\Microsoft\Remote Desktop Connection Manager\RDCMan.exe

using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Threading;
using System.Windows.Forms;
using Win32;

namespace RdcMan
{
  internal class ListSessionsForm : Form
  {
    private IContainer components;
    public Label StatusLabel;
    public ListView SessionListView;
    private ColumnHeader idHeader1;
    private ColumnHeader stateHeader1;
    private ColumnHeader userHeader1;
    private ColumnHeader clientHeader1;
    private Button RefreshButton;
    private readonly RemoteSessions _remoteSessions;
    private int[] _sortOrder = new int[1];
    private readonly object _queryLock = new object();
    private bool _areQuerying;

    protected override void Dispose(bool disposing)
    {
      if (disposing && this.components != null)
        this.components.Dispose();
      base.Dispose(disposing);
    }

    private void InitializeComponent()
    {
      this.StatusLabel = new Label();
      this.SessionListView = new ListView();
      this.idHeader1 = new ColumnHeader();
      this.stateHeader1 = new ColumnHeader();
      this.userHeader1 = new ColumnHeader();
      this.clientHeader1 = new ColumnHeader();
      this.RefreshButton = new Button();
      this.SuspendLayout();
      this.StatusLabel.Location = new Point(12, 9);
      this.StatusLabel.Name = "StatusLabel";
      this.StatusLabel.Size = new Size(238, 24);
      this.StatusLabel.TabIndex = 0;
      this.StatusLabel.Text = "Querying sessions...";
      this.SessionListView.Columns.AddRange(new ColumnHeader[4]
      {
        this.idHeader1,
        this.stateHeader1,
        this.userHeader1,
        this.clientHeader1
      });
      this.SessionListView.FullRowSelect = true;
      this.SessionListView.Location = new Point(10, 42);
      this.SessionListView.MultiSelect = false;
      this.SessionListView.Name = "SessionListView";
      this.SessionListView.Size = new Size(345, 154);
      this.SessionListView.TabIndex = 1;
      this.SessionListView.UseCompatibleStateImageBehavior = false;
      this.SessionListView.View = View.Details;
      this.SessionListView.ColumnClick += new ColumnClickEventHandler(this.SessionListView_ColumnClick);
      this.idHeader1.Text = "Id";
      this.idHeader1.Width = 30;
      this.stateHeader1.Text = "State";
      this.stateHeader1.Width = 80;
      this.userHeader1.Text = "User";
      this.userHeader1.Width = 135;
      this.clientHeader1.Text = "Client";
      this.clientHeader1.Width = 96;
      this.RefreshButton.Anchor = AnchorStyles.Top | AnchorStyles.Right;
      this.RefreshButton.Location = new Point(256, 9);
      this.RefreshButton.Name = "RefreshButton";
      this.RefreshButton.Size = new Size(100, 24);
      this.RefreshButton.TabIndex = 2;
      this.RefreshButton.Text = "Refresh";
      this.RefreshButton.UseVisualStyleBackColor = true;
      this.RefreshButton.Click += new EventHandler(this.RefreshButton_Click);
      this.ClientSize = new Size(366, 206);
      this.Controls.Add((Control) this.RefreshButton);
      this.Controls.Add((Control) this.SessionListView);
      this.Controls.Add((Control) this.StatusLabel);
      this.Name = nameof (ListSessionsForm);
      this.Load += new EventHandler(this.ListSessionsForm_Load);
      this.SizeChanged += new EventHandler(this.ListSessionsForm_SizeChanged);
      this.FormClosed += new FormClosedEventHandler(this.ListSessionsForm_FormClosed);
      this.Resize += new EventHandler(this.ListSessionsForm_Resize);
      this.ResumeLayout(false);
    }

    public ListSessionsForm(ServerBase server)
    {
      this.InitializeComponent();
      this._remoteSessions = new RemoteSessions(server);
      this._areQuerying = true;
      this.Text = server.ServerName + " Sessions";
      ContextMenu contextMenu = new ContextMenu();
      contextMenu.Popup += new EventHandler(this.OnContextMenu);
      this.SessionListView.ContextMenu = contextMenu;
      this.StartPosition = FormStartPosition.Manual;
      this.Location = Program.TheForm.Location;
      this.Location.Offset(100, 100);
      this.Icon = Program.TheForm.Icon;
    }

    private ListViewItem GetSelection()
    {
      ListView sessionListView = this.SessionListView;
      if (sessionListView.SelectedItems.Count != 1)
        return (ListViewItem) null;
      IEnumerator enumerator = sessionListView.SelectedItems.GetEnumerator();
      enumerator.MoveNext();
      return enumerator.Current as ListViewItem;
    }

    private void ListSessionsForm_Load(object sender, EventArgs e) => ThreadPool.QueueUserWorkItem(new WaitCallback(ListSessionsForm.OpenThreadProc), (object) this);

    private static void OpenThreadProc(object o)
    {
      ListSessionsForm form = o as ListSessionsForm;
      if (!form._remoteSessions.OpenServer())
        form.Invoke((Delegate) (() => form.StatusLabel.Text = "Unable to access remote sessions"));
      else
        ListSessionsForm.QuerySessions((object) form);
    }

    private static void QuerySessions(object o)
    {
      ListSessionsForm form = o as ListSessionsForm;
      if (form.IsDisposed)
        return;
      form.Invoke((Delegate) (() =>
      {
        form.SessionListView.BeginUpdate();
        form.SessionListView.Items.Clear();
      }));
      IList<RemoteSessionInfo> remoteSessionInfoList = form._remoteSessions.QuerySessions();
      if (remoteSessionInfoList == null)
      {
        form.Invoke((Delegate) (() => form.StatusLabel.Text = "Unable to enumerate remote sessions"));
      }
      else
      {
        foreach (RemoteSessionInfo remoteSessionInfo in (IEnumerable<RemoteSessionInfo>) remoteSessionInfoList)
        {
          Wts.ConnectstateClass state = remoteSessionInfo.State;
          string text = remoteSessionInfo.DomainName.Length <= 0 ? remoteSessionInfo.UserName : remoteSessionInfo.DomainName + (object) '\\' + remoteSessionInfo.UserName;
          ListViewItem value = new ListViewItem()
          {
            Text = remoteSessionInfo.SessionId.ToString()
          };
          value.SubItems.Add(state.ToString());
          value.SubItems.Add(text);
          value.SubItems.Add(remoteSessionInfo.ClientName);
          form.Invoke((Delegate) (() => form.SessionListView.Items.Add(value)));
        }
        form.Invoke((Delegate) (() =>
        {
          int count = form.SessionListView.Items.Count;
          string str = count.ToString() + " session";
          if (count != 1)
            str += (string) (object) 's';
          form.StatusLabel.Text = str;
          form.SortListView();
          form.SessionListView.EndUpdate();
          form._areQuerying = false;
        }));
      }
    }

    private void ListSessionsForm_FormClosed(object sender, FormClosedEventArgs e) => this._remoteSessions.CloseServer();

    private void RefreshButton_Click(object sender, EventArgs e) => this.RefreshSessions();

    private void RefreshSessions()
    {
      lock (this._queryLock)
      {
        if (this._areQuerying)
          return;
        this._areQuerying = true;
      }
      this.StatusLabel.Text = "Refreshing...";
      ThreadPool.QueueUserWorkItem(new WaitCallback(ListSessionsForm.QuerySessions), (object) this);
    }

    private void OnContextMenu(object sender, EventArgs e)
    {
      ContextMenu contextMenu = sender as ContextMenu;
      ListViewItem selection = this.GetSelection();
      if (selection == null)
        return;
      contextMenu.MenuItems.Clear();
      Wts.ConnectstateClass connectstateClass;
      switch (selection.SubItems[1].Text)
      {
        case "Active":
        case "Connected":
          connectstateClass = Wts.ConnectstateClass.Connected;
          break;
        case "Disconnected":
          connectstateClass = Wts.ConnectstateClass.Disconnected;
          break;
        default:
          connectstateClass = Wts.ConnectstateClass.ConnectQuery;
          break;
      }
      MenuItem menuItem1 = new MenuItem("&Disconnect", new EventHandler(this.DisconnectSession))
      {
        Enabled = connectstateClass == Wts.ConnectstateClass.Connected
      };
      contextMenu.MenuItems.Add(menuItem1);
      contextMenu.MenuItems.Add("-");
      MenuItem menuItem2 = new MenuItem("&Log off", new EventHandler(this.LogOffSession));
      contextMenu.MenuItems.Add(menuItem2);
      menuItem2.Enabled = !Policies.DisableLogOff;
    }

    private void DisconnectSession(object sender, EventArgs e)
    {
      int result;
      if (!int.TryParse(this.GetSelection().SubItems[0].Text, out result))
        return;
      this._remoteSessions.DisconnectSession(result);
      this.RefreshSessions();
    }

    private void LogOffSession(object sender, EventArgs e)
    {
      int result;
      if (!int.TryParse(this.GetSelection().SubItems[0].Text, out result))
        return;
      this._remoteSessions.LogOffSession(result);
      this.RefreshSessions();
    }

    private void ListSessionsForm_Resize(object sender, EventArgs e) => this.SessionListView.Width = this.RefreshButton.Right - this.SessionListView.Left;

    private void SessionListView_ColumnClick(object sender, ColumnClickEventArgs e)
    {
      this._sortOrder = new int[1];
      int num1 = 0;
      int[] sortOrder = this._sortOrder;
      int index = num1;
      int num2 = index + 1;
      int column = e.Column;
      sortOrder[index] = column;
      this.SortListView();
    }

    private void SortListView()
    {
      ArrayList arrayList = new ArrayList(this.SessionListView.Items.Count);
      foreach (ListViewItem listViewItem in this.SessionListView.Items)
        arrayList.Add((object) listViewItem);
      arrayList.Sort((IComparer) new SessionListSortComparer(this._sortOrder));
      this.SessionListView.Items.Clear();
      foreach (ListViewItem listViewItem in arrayList)
        this.SessionListView.Items.Add(listViewItem);
    }

    private void ListSessionsForm_SizeChanged(object sender, EventArgs e) => this.SessionListView.Size = new Size(Math.Max(20, this.ClientRectangle.Width - this.SessionListView.Location.X * 2), Math.Max(20, this.ClientRectangle.Height - this.SessionListView.Location.Y - 10));
  }
}
