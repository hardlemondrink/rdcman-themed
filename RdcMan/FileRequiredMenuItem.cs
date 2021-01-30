// Decompiled with JetBrains decompiler
// Type: RdcMan.FileRequiredMenuItem
// Assembly: RDCMan, Version=2.7.1406.0, Culture=neutral, PublicKeyToken=null
// MVID: 984CDC7A-116B-406E-B519-0DEE541B9019
// Assembly location: C:\Program Files (x86)\Microsoft\Remote Desktop Connection Manager\RDCMan.exe

using System;

namespace RdcMan
{
  internal class FileRequiredMenuItem : RdcMenuItem
  {
    private readonly Action _clickDelegate;

    public FileRequiredMenuItem(string text, MenuNames name, Action clickDelegate)
      : base(text)
    {
      this.Name = name.ToString();
      this._clickDelegate = clickDelegate;
    }

    public FileRequiredMenuItem(
      string text,
      MenuNames name,
      string shortcut,
      Action clickDelegate)
      : this(text, name, clickDelegate)
    {
      this.ShortcutKeyDisplayString = shortcut;
    }

    public override void Update() => this.Enabled = ServerTree.Instance.AnyOpenedEditableFiles();

    protected override void OnClick() => this._clickDelegate();
  }
}
