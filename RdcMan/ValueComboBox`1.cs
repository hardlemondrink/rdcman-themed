// Decompiled with JetBrains decompiler
// Type: RdcMan.ValueComboBox`1
// Assembly: RDCMan, Version=2.7.1406.0, Culture=neutral, PublicKeyToken=null
// MVID: 984CDC7A-116B-406E-B519-0DEE541B9019
// Assembly location: C:\Program Files (x86)\Microsoft\Remote Desktop Connection Manager\RDCMan.exe

using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace RdcMan
{
  public class ValueComboBox<TValue> : ComboBox, ISettingControl
  {
    public RdcMan.Setting<TValue> Setting;

    public ValueComboBox(
      RdcMan.Setting<TValue> setting,
      IEnumerable<TValue> values,
      Func<TValue, string> toString)
      : this(values, toString)
    {
      this.Setting = setting;
    }

    public ValueComboBox(IEnumerable<TValue> values, Func<TValue, string> toString)
    {
      this.DropDownStyle = ComboBoxStyle.DropDownList;
      if (values == null)
        return;
      this.AddItems(values, toString);
    }

    public TValue SelectedValue
    {
      get => this.SelectedItem == null ? default (TValue) : (this.SelectedItem as ValueComboBox<TValue>.ComboBoxItem<TValue>).Value;
      set
      {
        int num = this.FindItem(value);
        if (num == -1)
          return;
        this.SelectedIndex = num;
      }
    }

    public void AddItems(IEnumerable<TValue> values, Func<TValue, string> toString) => values.ForEach<TValue>((Action<TValue>) (v => this.AddItem(toString(v), v)));

    public void AddItem(string name, TValue value) => this.Items.Add((object) new ValueComboBox<TValue>.ComboBoxItem<TValue>()
    {
      Name = name,
      Value = value
    });

    public void ClearItems() => this.Items.Clear();

    public void ReplaceItem(string name, TValue newValue)
    {
      int index = this.FindItem(name);
      if (index == -1)
        return;
      (this.Items[index] as ValueComboBox<TValue>.ComboBoxItem<TValue>).Value = newValue;
    }

    public int ItemCount => this.Items.Count;

    public new ComboBox.ObjectCollection Items
    {
      set => throw new InvalidOperationException();
    }

    public new string Text => throw new InvalidOperationException();

    public int FindItem(TValue value)
    {
      for (int index = 0; index < this.Items.Count; ++index)
      {
        if (object.Equals((object) (this.Items[index] as ValueComboBox<TValue>.ComboBoxItem<TValue>).Value, (object) value))
          return index;
      }
      return -1;
    }

    public int FindItem(string name)
    {
      for (int index = 0; index < this.Items.Count; ++index)
      {
        if ((this.Items[index] as ValueComboBox<TValue>.ComboBoxItem<TValue>).Name.Equals(name, StringComparison.OrdinalIgnoreCase))
          return index;
      }
      return -1;
    }

    void ISettingControl.UpdateControl()
    {
      if (this.Setting == null)
        return;
      this.SelectedValue = this.Setting.Value;
    }

    void ISettingControl.UpdateSetting()
    {
      if (this.Setting == null)
        return;
      this.Setting.Value = this.SelectedValue;
    }

    string ISettingControl.Validate() => (string) null;

    private class ComboBoxItem<T>
    {
      public string Name;
      public T Value;

      public override string ToString() => this.Name;
    }
  }
}
