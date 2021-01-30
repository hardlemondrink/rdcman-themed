// Decompiled with JetBrains decompiler
// Type: RdcMan.RdcMenuItem
// Assembly: RDCMan, Version=2.7.1406.0, Culture=neutral, PublicKeyToken=null
// MVID: 984CDC7A-116B-406E-B519-0DEE541B9019
// Assembly location: C:\Program Files (x86)\Microsoft\Remote Desktop Connection Manager\RDCMan.exe

using System;
using System.Windows.Forms;

namespace RdcMan
{
  internal abstract class RdcMenuItem : ToolStripMenuItem
  {
    protected RdcMenuItem()
    {
    }

    protected RdcMenuItem(string text)
      : this()
      => this.Text = text;

    protected override void OnClick(EventArgs e)
    {
      base.OnClick(e);
      this.OnClick();
    }

    public abstract void Update();

    protected abstract void OnClick();
  }
}
