// Decompiled with JetBrains decompiler
// Type: RdcMan.SelectActiveServerForm
// Assembly: RDCMan, Version=2.7.1406.0, Culture=neutral, PublicKeyToken=null
// MVID: 984CDC7A-116B-406E-B519-0DEE541B9019
// Assembly location: C:\Program Files (x86)\Microsoft\Remote Desktop Connection Manager\RDCMan.exe

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using System.Windows.Forms.Layout;

namespace RdcMan
{
  internal class SelectActiveServerForm : RdcDialog
  {
    public SelectActiveServerForm.SelectedObject Selected { get; private set; }

    public SelectActiveServerForm(IEnumerable<ServerBase> servers)
      : base("Select server", (string) null)
    {
      this.BackColor = Color.White;
      this.ClientSize = new Size(304, FormTools.YPos(7));
      int num1 = 0;
      foreach (ServerBase server in servers.Take<ServerBase>(10))
      {
        char key = num1 == 9 ? '0' : (char) (49 + num1);
        this.AddButton(num1 / 5, num1 % 5, key, server.DisplayName, SelectActiveServerForm.Operation.SelectServer, server);
        ++num1;
      }
      int num2 = num1 + num1 % 2;
      int rowIndex = num2 < 10 ? num2 % 5 : 5;
      this.AddButton(0, rowIndex, 'T', "Server Tree", SelectActiveServerForm.Operation.SelectTree, (ServerBase) null);
      this.AddButton(1, rowIndex, 'N', "Minimize RDCMan", SelectActiveServerForm.Operation.MinimizeWindow, (ServerBase) null);
      this.KeyDown += new KeyEventHandler(this.List_KeyDownHandler);
      this.ScaleAndLayout();
    }

    private void AddButton(
      int colIndex,
      int rowIndex,
      char key,
      string text,
      SelectActiveServerForm.Operation operation,
      ServerBase server)
    {
      Button button1 = new Button();
      button1.Location = FormTools.NewLocation(colIndex, rowIndex);
      button1.FlatStyle = FlatStyle.Flat;
      button1.Text = string.Format("{0} - {1}", (object) key, (object) text);
      button1.Tag = (object) new SelectActiveServerForm.SelectedObject()
      {
        Key = key,
        Operation = operation,
        Server = server
      };
      Button button2 = button1;
      button2.Width = 140;
      button2.Click += new EventHandler(this.Button_Click);
      button2.KeyDown += new KeyEventHandler(this.Button_KeyDown);
      this.Controls.Add((Control) button2);
    }

    private void Button_KeyDown(object sender, KeyEventArgs e)
    {
      this.List_KeyDownHandler(sender, e);
      if (this.DialogResult != DialogResult.None)
        return;
      e.Handled = false;
    }

    private void Button_Click(object sender, EventArgs e) => this.SelectObject((sender as Button).Tag as SelectActiveServerForm.SelectedObject);

    private void List_KeyDownHandler(object sender, KeyEventArgs e)
    {
      char ch = (char) e.KeyData;
      if (e.KeyData >= Keys.NumPad0 && e.KeyData <= Keys.NumPad9)
        ch = (char) (e.KeyData - 96 + 48);
      if (ch >= 'a' && ch <= 'z')
        ch = (char) ((int) ch - 97 + 65);
      foreach (Control control in (ArrangedElementCollection) this.Controls)
      {
        if (control.Tag is SelectActiveServerForm.SelectedObject tag && (int) tag.Key == (int) ch)
        {
          this.SelectObject(tag);
          break;
        }
      }
      if (e.KeyData == Keys.Escape)
        this.Cancel();
      e.Handled = true;
    }

    private void SelectObject(SelectActiveServerForm.SelectedObject o)
    {
      this.Selected = o;
      this.OK();
    }

    public enum Operation
    {
      SelectServer,
      SelectTree,
      MinimizeWindow,
    }

    public class SelectedObject
    {
      public SelectActiveServerForm.Operation Operation;
      public char Key;
      public ServerBase Server;
    }
  }
}
