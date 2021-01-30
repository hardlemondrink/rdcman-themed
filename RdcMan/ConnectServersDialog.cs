// Decompiled with JetBrains decompiler
// Type: RdcMan.ConnectServersDialog
// Assembly: RDCMan, Version=2.7.1406.0, Culture=neutral, PublicKeyToken=null
// MVID: 984CDC7A-116B-406E-B519-0DEE541B9019
// Assembly location: C:\Program Files (x86)\Microsoft\Remote Desktop Connection Manager\RDCMan.exe

using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace RdcMan
{
  internal class ConnectServersDialog : SelectServersDialogBase
  {
    public ConnectServersDialog(IEnumerable<ServerBase> servers)
      : base("Connect Servers", "&Connect")
    {
      int rowIndex = 0;
      int tabIndex = 0;
      this.AddLabel("Select servers to be connected", ref rowIndex, ref tabIndex);
      this.AddListView(ref rowIndex, ref tabIndex);
      this.InitButtons();
      this.ScaleAndLayout();
      servers.ForEach<ServerBase>((Action<ServerBase>) (server => this.ListView.Items.Add(this.CreateListViewItem(server))));
      this.ListView.ItemChecked += new ItemCheckedEventHandler(this.ListView_ItemChecked);
      this._acceptButton.Enabled = false;
    }

    private void ListView_ItemChecked(object sender, ItemCheckedEventArgs e) => this._acceptButton.Enabled = this.ListView.CheckedItems.Count > 0;
  }
}
