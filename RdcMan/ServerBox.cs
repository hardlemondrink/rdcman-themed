// Decompiled with JetBrains decompiler
// Type: RdcMan.ServerBox
// Assembly: RDCMan, Version=2.7.1406.0, Culture=neutral, PublicKeyToken=null
// MVID: 984CDC7A-116B-406E-B519-0DEE541B9019
// Assembly location: C:\Program Files (x86)\Microsoft\Remote Desktop Connection Manager\RDCMan.exe

using System;
using System.Drawing;
using System.Windows.Forms;

namespace RdcMan
{
  public class ServerBox : Label
  {
    private Server _server;

    public ServerBox(Server server)
    {
      this._server = server;
      this.BackColor = Color.White;
      this.BorderStyle = BorderStyle.FixedSingle;
      this.TextAlign = ContentAlignment.MiddleCenter;
      this.Hide();
    }

    public void SetText()
    {
      string str = this._server.GetConnectionStateText();
      if (this._server.IsClientUndocked)
        str = str + Environment.NewLine + "{ Undocked }";
      this.Text = str;
    }
  }
}
