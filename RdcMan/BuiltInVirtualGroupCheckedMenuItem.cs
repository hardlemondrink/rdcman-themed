// Decompiled with JetBrains decompiler
// Type: RdcMan.BuiltInVirtualGroupCheckedMenuItem
// Assembly: RDCMan, Version=2.7.1406.0, Culture=neutral, PublicKeyToken=null
// MVID: 984CDC7A-116B-406E-B519-0DEE541B9019
// Assembly location: C:\Program Files (x86)\Microsoft\Remote Desktop Connection Manager\RDCMan.exe

namespace RdcMan
{
  internal class BuiltInVirtualGroupCheckedMenuItem : CheckedMenuItem
  {
    private IBuiltInVirtualGroup _group;

    public BuiltInVirtualGroupCheckedMenuItem(IBuiltInVirtualGroup group)
      : base(group.Text)
      => this._group = group;

    protected override void CheckChanged(bool isChecked) => this._group.IsInTree = isChecked;

    public override void Update() => this.Checked = this._group.IsInTree;
  }
}
