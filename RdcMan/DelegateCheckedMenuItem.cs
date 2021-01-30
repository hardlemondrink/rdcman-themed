// Decompiled with JetBrains decompiler
// Type: RdcMan.DelegateCheckedMenuItem
// Assembly: RDCMan, Version=2.7.1406.0, Culture=neutral, PublicKeyToken=null
// MVID: 984CDC7A-116B-406E-B519-0DEE541B9019
// Assembly location: C:\Program Files (x86)\Microsoft\Remote Desktop Connection Manager\RDCMan.exe

using System;

namespace RdcMan
{
  internal class DelegateCheckedMenuItem : CheckedMenuItem
  {
    private readonly Func<bool> _initDelegate;
    private readonly Action<bool> _changedDelegate;

    public DelegateCheckedMenuItem(
      string text,
      MenuNames name,
      Func<bool> initDelegate,
      Action<bool> changedDelegate)
      : base(text)
    {
      this.Name = name.ToString();
      this._initDelegate = initDelegate;
      this._changedDelegate = changedDelegate;
    }

    protected override sealed void CheckChanged(bool isChecked) => this._changedDelegate(isChecked);

    public override sealed void Update() => this.Checked = this._initDelegate();
  }
}
