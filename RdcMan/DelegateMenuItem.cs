// Decompiled with JetBrains decompiler
// Type: RdcMan.DelegateMenuItem
// Assembly: RDCMan, Version=2.7.1406.0, Culture=neutral, PublicKeyToken=null
// MVID: 984CDC7A-116B-406E-B519-0DEE541B9019
// Assembly location: C:\Program Files (x86)\Microsoft\Remote Desktop Connection Manager\RDCMan.exe

using System;
using System.Windows.Forms;

namespace RdcMan
{
  public class DelegateMenuItem : ToolStripMenuItem
  {
    public DelegateMenuItem(string text, MenuNames name, Action click)
      : base(text)
    {
      this.Click += (EventHandler) ((s, e) => click());
      this.Name = name.ToString();
    }

    public DelegateMenuItem(string text, MenuNames name, string shortcut, Action click)
      : this(text, name, click)
      => this.ShortcutKeyDisplayString = shortcut;
  }
}
