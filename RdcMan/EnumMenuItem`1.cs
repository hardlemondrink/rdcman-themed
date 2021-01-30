// Decompiled with JetBrains decompiler
// Type: RdcMan.EnumMenuItem`1
// Assembly: RDCMan, Version=2.7.1406.0, Culture=neutral, PublicKeyToken=null
// MVID: 984CDC7A-116B-406E-B519-0DEE541B9019
// Assembly location: C:\Program Files (x86)\Microsoft\Remote Desktop Connection Manager\RDCMan.exe

namespace RdcMan
{
  internal abstract class EnumMenuItem<T> : RdcMenuItem
  {
    protected EnumMenuItem(string text, T value)
    {
      this.Text = text;
      this.Tag = (object) value;
    }

    protected override void OnClick()
    {
      this.Value = (T) this.Tag;
      Program.Preferences.NeedToSave = true;
    }

    public override void Update() => this.Checked = this.Tag.Equals((object) this.Value);

    protected abstract T Value { get; set; }
  }
}
