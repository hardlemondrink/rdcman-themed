// Decompiled with JetBrains decompiler
// Type: RdcMan.SelectServersDialogBase
// Assembly: RDCMan, Version=2.7.1406.0, Culture=neutral, PublicKeyToken=null
// MVID: 984CDC7A-116B-406E-B519-0DEE541B9019
// Assembly location: C:\Program Files (x86)\Microsoft\Remote Desktop Connection Manager\RDCMan.exe

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Threading;
using System.Windows.Forms;

namespace RdcMan
{
  internal class SelectServersDialogBase : RdcDialog
  {
    protected const int DialogWidth = 500;
    private int _suspendItemChecked;

    public SelectServersDialogBase(string dialogTitle, string acceptButtonText)
      : base(dialogTitle, acceptButtonText)
    {
    }

    protected void AddLabel(string text, ref int rowIndex, ref int tabIndex)
    {
      Label label = new Label();
      label.Location = FormTools.NewLocation(0, rowIndex++);
      label.Text = text;
      label.TextAlign = ContentAlignment.MiddleLeft;
      label.Size = new Size(500, 20);
      this.Controls.Add((Control) label);
    }

    protected void AddListView(ref int rowIndex, ref int tabIndex)
    {
      RdcListView rdcListView = new RdcListView();
      rdcListView.CheckBoxes = true;
      rdcListView.FullRowSelect = true;
      rdcListView.Location = FormTools.NewLocation(0, rowIndex++);
      rdcListView.MultiSelect = false;
      rdcListView.Size = new Size(500, 300);
      rdcListView.TabIndex = tabIndex++;
      rdcListView.View = View.Details;
      this.ListView = rdcListView;
      this.ListView.KeyDown += new KeyEventHandler(this.List_KeyDownHandler);
      this.ListView.MouseDoubleClick += new MouseEventHandler(this.List_MouseDoubleClick);
      this.ListView.ItemChecked += new ItemCheckedEventHandler(this.ListView_ItemChecked);
      this.ListView.Columns.AddRange(new ColumnHeader[3]
      {
        new ColumnHeader() { Text = string.Empty, Width = 22 },
        new ColumnHeader() { Text = "Server", Width = 130 },
        new ColumnHeader() { Text = "Group", Width = 349 }
      });
      this.Controls.Add((Control) this.ListView);
      if (!RdcListView.SupportsHeaderCheckBoxes)
        return;
      this.ListView.SetColumnHeaderToCheckBox(0);
      this.ListView.HeaderCheckBoxClick += new HeaderColumnClickEventHandler(this.ListView_HeaderCheckBoxClick);
    }

    public void SuspendItemChecked() => Interlocked.Increment(ref this._suspendItemChecked);

    public void ResumeItemChecked()
    {
      if (Interlocked.Decrement(ref this._suspendItemChecked) != 0)
        return;
      this.SetHeaderCheckFromItems();
    }

    public IEnumerable<ServerBase> SelectedServers
    {
      get
      {
        foreach (ListViewItem listViewItem in this.ListView.Items)
        {
          if (listViewItem.Checked)
            yield return (ServerBase) listViewItem.Tag;
        }
      }
    }

    protected ListViewItem CreateListViewItem(ServerBase server) => new ListViewItem(new string[3]
    {
      "",
      server.DisplayName,
      server.Parent.FullPath
    })
    {
      Tag = (object) server
    };

    public override void InitButtons()
    {
      base.InitButtons();
      if (RdcListView.SupportsHeaderCheckBoxes)
        return;
      Button button1 = new Button();
      button1.Text = "Select &all";
      button1.TabIndex = this._acceptButton.TabIndex - 1;
      Button button2 = button1;
      button2.Click += new EventHandler(this.SelectAll_Click);
      button2.Location = new Point(8, this._acceptButton.Location.Y);
      this.Controls.Add((Control) button2);
    }

    protected RdcListView ListView { get; private set; }

    private void List_MouseDoubleClick(object sender, MouseEventArgs e)
    {
      if (e.Button != MouseButtons.Left)
        return;
      this.OK();
    }

    private void List_KeyDownHandler(object sender, KeyEventArgs e)
    {
      if (e.KeyData != (Keys.A | Keys.Control))
        return;
      e.Handled = true;
      this.SelectAllItems(true);
    }

    private void SelectAll_Click(object sender, EventArgs e) => this.SelectAllItems(true);

    private void SelectAllItems(bool isChecked)
    {
      try
      {
        this.SuspendItemChecked();
        foreach (ListViewItem listViewItem in this.ListView.Items)
          listViewItem.Checked = isChecked;
      }
      finally
      {
        this.ResumeItemChecked();
      }
    }

    private void ListView_ItemChecked(object sender, ItemCheckedEventArgs e)
    {
      if (this._suspendItemChecked != 0)
        return;
      this.SetHeaderCheckFromItems();
    }

    private void SetHeaderCheckFromItems() => this.ListView.SetColumnHeaderChecked(0, this.ListView.Items.OfType<ListViewItem>().All<ListViewItem>((Func<ListViewItem, bool>) (i => i.Checked)));

    private void ListView_HeaderCheckBoxClick(object sender, HeaderColumnClickEventArgs e) => this.SelectAllItems(e.IsChecked);
  }
}
