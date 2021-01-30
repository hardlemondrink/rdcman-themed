// Decompiled with JetBrains decompiler
// Type: RdcMan.CustomClientSizeCheckedMenuItem
// Assembly: RDCMan, Version=2.7.1406.0, Culture=neutral, PublicKeyToken=null
// MVID: 984CDC7A-116B-406E-B519-0DEE541B9019
// Assembly location: C:\Program Files (x86)\Microsoft\Remote Desktop Connection Manager\RDCMan.exe

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace RdcMan
{
  internal class CustomClientSizeCheckedMenuItem : CheckedMenuItem
  {
    private RdcBaseForm _form;
    private string _baseText;

    public CustomClientSizeCheckedMenuItem(RdcBaseForm form, string text)
      : base(text)
    {
      this._baseText = this.Text;
      this._form = form;
    }

    protected override void CheckChanged(bool isChecked)
    {
      using (CustomSizeDialog customSizeDialog = new CustomSizeDialog(this._form.GetClientSize()))
      {
        if (customSizeDialog.ShowDialog() != DialogResult.OK)
          return;
        this._form.SetClientSize(SizeHelper.FromString(customSizeDialog.WidthText, customSizeDialog.HeightText));
      }
    }

    public override void Update()
    {
      Size clientSize = this._form.GetClientSize();
      bool flag = ((IEnumerable<Size>) SizeHelper.StockSizes).All<Size>((Func<Size, bool>) (size => size != clientSize));
      this.Checked = flag;
      string baseText = this._baseText;
      if (flag)
        baseText += " ({0})".InvariantFormat((object) clientSize.ToFormattedString());
      this.Text = baseText + "...";
    }
  }
}
