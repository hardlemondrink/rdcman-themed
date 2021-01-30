// Decompiled with JetBrains decompiler
// Type: RdcMan.ClientSizeCheckedMenuItem
// Assembly: RDCMan, Version=2.7.1406.0, Culture=neutral, PublicKeyToken=null
// MVID: 984CDC7A-116B-406E-B519-0DEE541B9019
// Assembly location: C:\Program Files (x86)\Microsoft\Remote Desktop Connection Manager\RDCMan.exe

using System.Drawing;

namespace RdcMan
{
  internal class ClientSizeCheckedMenuItem : CheckedMenuItem
  {
    private RdcBaseForm _form;

    public ClientSizeCheckedMenuItem(RdcBaseForm form, Size size)
      : base(size.ToFormattedString())
    {
      this.Tag = (object) size;
      this._form = form;
    }

    protected override void CheckChanged(bool isChecked) => this._form.SetClientSize((Size) this.Tag);

    public override void Update()
    {
      Size tag = (Size) this.Tag;
      this.Checked = this._form.GetClientSize() == tag;
    }
  }
}
