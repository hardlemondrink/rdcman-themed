// Decompiled with JetBrains decompiler
// Type: RdcMan.SelectedNodeMenuItem`1
// Assembly: RDCMan, Version=2.7.1406.0, Culture=neutral, PublicKeyToken=null
// MVID: 984CDC7A-116B-406E-B519-0DEE541B9019
// Assembly location: C:\Program Files (x86)\Microsoft\Remote Desktop Connection Manager\RDCMan.exe

using System;

namespace RdcMan
{
  internal class SelectedNodeMenuItem<T> : RdcMenuItem where T : RdcTreeNode
  {
    private readonly Action<T> _action;

    public SelectedNodeMenuItem(string text, MenuNames name, Action<T> action)
      : base(text)
    {
      this.Name = name.ToString();
      this._action = action;
    }

    public SelectedNodeMenuItem(string text, MenuNames name, string shortcut, Action<T> action)
      : this(text, name, action)
      => this.ShortcutKeyDisplayString = shortcut;

    public override void Update() => this.Enabled = Program.TheForm.GetSelectedNode() is T;

    protected override void OnClick() => this._action(Program.TheForm.GetSelectedNode() as T);
  }
}
