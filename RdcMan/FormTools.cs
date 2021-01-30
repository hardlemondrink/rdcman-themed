// Decompiled with JetBrains decompiler
// Type: RdcMan.FormTools
// Assembly: RDCMan, Version=2.7.1406.0, Culture=neutral, PublicKeyToken=null
// MVID: 984CDC7A-116B-406E-B519-0DEE541B9019
// Assembly location: C:\Program Files (x86)\Microsoft\Remote Desktop Connection Manager\RDCMan.exe

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using System.Windows.Forms.Layout;

namespace RdcMan
{
  public static class FormTools
  {
    public const int TabControlWidth = 520;
    public const int TabControlHeight = 350;
    public const int ControlHeight = 20;
    public const int VerticalSpace = 4;
    public const int HorizontalMargin = 8;
    public const int TopMargin = 16;
    public const int BottomMargin = 16;
    public const int Indent = 24;
    public const int TabPageWidth = 512;
    public const int TabPageHeight = 334;
    public const int TabPageInternalVerticalSpace = 8;
    public const int TabPageControlWidth = 480;
    public const int TabPageControlHeight = 302;
    public const int LabelWidth = 140;
    public const int TextBoxWidth = 340;
    public const int DropDownWidth = 340;
    public const int GroupBoxWidth = 496;

    public static int XPos(int colIndex) => 8 + 148 * colIndex;

    public static int YPos(int rowIndex) => 16 + 24 * rowIndex;

    public static int YPosNoMargin(int rowIndex) => 24 * rowIndex;

    public static Point TopLeftLocation() => new Point(4, 22);

    public static Point NewLocation(int colIndex, int rowIndex) => new Point(FormTools.XPos(colIndex), FormTools.YPos(rowIndex));

    public static Point NewUngroupedLocation(int colIndex, int rowIndex) => new Point(16 + 140 * colIndex, FormTools.YPos(rowIndex));

    public static Label NewLabel(string text, int colIndex, int rowIndex)
    {
      Label label = new Label();
      label.Location = FormTools.NewLocation(colIndex, rowIndex);
      label.Text = text;
      label.TextAlign = ContentAlignment.MiddleLeft;
      label.Size = new Size(140, 20);
      return label;
    }

    public static RdcTextBox NewTextBox(int colIndex, int rowIndex, int tabIndex)
    {
      RdcTextBox rdcTextBox = new RdcTextBox();
      rdcTextBox.Enabled = false;
      rdcTextBox.Location = FormTools.NewLocation(colIndex, rowIndex);
      rdcTextBox.Size = new Size(340, 20);
      rdcTextBox.TabIndex = tabIndex;
      return rdcTextBox;
    }

    public static RdcTextBox NewTextBox(
      int colIndex,
      int rowIndex,
      int tabIndex,
      int height)
    {
      RdcTextBox rdcTextBox = new RdcTextBox();
      rdcTextBox.Location = FormTools.NewLocation(colIndex, rowIndex);
      rdcTextBox.Size = new Size(340, 20 * height);
      rdcTextBox.TabIndex = tabIndex;
      rdcTextBox.Multiline = true;
      rdcTextBox.AcceptsReturn = true;
      rdcTextBox.ScrollBars = ScrollBars.Vertical;
      return rdcTextBox;
    }

    public static RdcTextBox AddLabeledTextBox(
      Control parent,
      string text,
      ref int rowIndex,
      ref int tabIndex)
    {
      Label label = FormTools.NewLabel(text, 0, rowIndex);
      RdcTextBox rdcTextBox = FormTools.NewTextBox(1, rowIndex++, tabIndex++);
      parent.Controls.Add((Control) label);
      parent.Controls.Add((Control) rdcTextBox);
      return rdcTextBox;
    }

    public static RdcTextBox AddLabeledTextBox(
      Control parent,
      string text,
      StringSetting setting,
      ref int rowIndex,
      ref int tabIndex)
    {
      RdcTextBox rdcTextBox = FormTools.AddLabeledTextBox(parent, text, ref rowIndex, ref tabIndex);
      rdcTextBox.Setting = setting;
      return rdcTextBox;
    }

    public static RdcCheckBox NewCheckBox(
      string text,
      int colIndex,
      int rowIndex,
      int tabIndex)
    {
      return FormTools.NewCheckBox(text, colIndex, rowIndex, tabIndex, 340);
    }

    public static RdcCheckBox NewCheckBox(
      string text,
      int colIndex,
      int rowIndex,
      int tabIndex,
      int width)
    {
      RdcCheckBox rdcCheckBox = new RdcCheckBox();
      rdcCheckBox.Location = FormTools.NewLocation(colIndex, rowIndex);
      rdcCheckBox.Size = new Size(width, 20);
      rdcCheckBox.TabIndex = tabIndex;
      rdcCheckBox.Text = text;
      return rdcCheckBox;
    }

    public static RdcCheckBox AddCheckBox(
      Control parent,
      string text,
      BoolSetting setting,
      int colIndex,
      ref int rowIndex,
      ref int tabIndex)
    {
      RdcCheckBox rdcCheckBox1 = new RdcCheckBox();
      rdcCheckBox1.Setting = setting;
      rdcCheckBox1.Location = FormTools.NewLocation(colIndex, rowIndex++);
      rdcCheckBox1.Size = new Size(340, 20);
      rdcCheckBox1.TabIndex = tabIndex++;
      rdcCheckBox1.Text = text;
      RdcCheckBox rdcCheckBox2 = rdcCheckBox1;
      parent.Controls.Add((Control) rdcCheckBox2);
      return rdcCheckBox2;
    }

    public static ValueComboBox<TEnum> AddLabeledEnumDropDown<TEnum>(
      Control parent,
      string text,
      ref int rowIndex,
      ref int tabIndex,
      Func<TEnum, string> toString)
      where TEnum : struct
    {
      return FormTools.AddLabeledValueDropDown<TEnum>(parent, text, ref rowIndex, ref tabIndex, toString, Helpers.EnumValues<TEnum>());
    }

    public static ValueComboBox<TValue> AddLabeledValueDropDown<TValue>(
      Control parent,
      string text,
      ref int rowIndex,
      ref int tabIndex,
      Func<TValue, string> toString,
      IEnumerable<TValue> values)
    {
      Label label = FormTools.NewLabel(text, 0, rowIndex);
      ValueComboBox<TValue> valueComboBox = new ValueComboBox<TValue>(values, toString);
      FormTools.SetDropDownProperties((ComboBox) valueComboBox, 1, rowIndex++, tabIndex++);
      parent.Controls.Add((Control) label);
      parent.Controls.Add((Control) valueComboBox);
      return valueComboBox;
    }

    public static ValueComboBox<TValue> AddLabeledValueDropDown<TValue>(
      Control parent,
      string text,
      Setting<TValue> setting,
      ref int rowIndex,
      ref int tabIndex,
      Func<TValue, string> toString,
      IEnumerable<TValue> values)
    {
      Label label = FormTools.NewLabel(text, 0, rowIndex);
      ValueComboBox<TValue> valueComboBox = new ValueComboBox<TValue>(setting, values, toString);
      FormTools.SetDropDownProperties((ComboBox) valueComboBox, 1, rowIndex++, tabIndex++);
      parent.Controls.Add((Control) label);
      parent.Controls.Add((Control) valueComboBox);
      return valueComboBox;
    }

    public static ValueComboBox<TEnum> AddLabeledEnumDropDown<TEnum>(
      Control parent,
      string text,
      EnumSetting<TEnum> setting,
      ref int rowIndex,
      ref int tabIndex,
      Func<TEnum, string> toString)
      where TEnum : struct
    {
      Label label = FormTools.NewLabel(text, 0, rowIndex);
      ValueComboBox<TEnum> valueComboBox = new ValueComboBox<TEnum>((Setting<TEnum>) setting, Helpers.EnumValues<TEnum>(), toString);
      FormTools.SetDropDownProperties((ComboBox) valueComboBox, 1, rowIndex++, tabIndex++);
      parent.Controls.Add((Control) label);
      parent.Controls.Add((Control) valueComboBox);
      return valueComboBox;
    }

    private static void SetDropDownProperties(
      ComboBox comboBox,
      int colIndex,
      int rowIndex,
      int tabIndex)
    {
      comboBox.Location = FormTools.NewLocation(colIndex, rowIndex);
      comboBox.Size = new Size(340, 20);
      comboBox.TabIndex = tabIndex;
    }

    public static void LayoutGroupBox(GroupBox groupBox, int numCols, Control previousGroupBox) => FormTools.LayoutGroupBox(groupBox, numCols, previousGroupBox, 0, 0);

    public static void LayoutGroupBox(
      GroupBox groupBox,
      int numCols,
      Control previousControl,
      int rowIndex,
      int tabIndex)
    {
      int num = 0;
      foreach (Control control in (ArrangedElementCollection) groupBox.Controls)
      {
        if (num == 1 && control.Width == 340)
          control.Width -= 8;
        control.Location = FormTools.NewLocation(num++, rowIndex);
        control.TabIndex = tabIndex;
        tabIndex += 2;
        if (!(control is Label))
          control.TabStop = true;
        if (num == numCols)
        {
          num = 0;
          ++rowIndex;
        }
      }
      groupBox.SizeAndLocate(previousControl);
    }

    public static void AddControlsAndSizeGroup(this GroupBox groupBox, params Control[] controls)
    {
      groupBox.Controls.AddRange(controls);
      foreach (Control control in (ArrangedElementCollection) groupBox.Controls)
      {
        if (control.Width == 340)
          control.Width -= 8;
      }
    }

    public static void SizeAndLocate(this GroupBox groupBox, Control previousControl)
    {
      int y = 8;
      if (previousControl != null)
      {
        groupBox.TabIndex = previousControl.TabIndex + 1;
        y += previousControl.Bottom;
      }
      else
        groupBox.TabIndex = 1;
      groupBox.Location = new Point(8, y);
      FormTools.ResizeGroupBox(groupBox);
    }

    public static void ResizeGroupBox(GroupBox groupBox)
    {
      int heightFromChildren = FormTools.ComputeControlHeightFromChildren((Control) groupBox);
      groupBox.Size = new Size(496, heightFromChildren + 8);
    }

    public static TabPage NewTabPage(string name)
    {
      TabPage tabPage1 = new TabPage();
      tabPage1.Location = FormTools.TopLeftLocation();
      tabPage1.Size = new Size(512, 334);
      tabPage1.Text = name;
      TabPage tabPage2 = tabPage1;
      tabPage2.SuspendLayout();
      return tabPage2;
    }

    public static TabPage NewTabPage(this TabControl parent, string name)
    {
      TabPage tabPage = FormTools.NewTabPage(name);
      parent.Controls.Add((Control) tabPage);
      return tabPage;
    }

    public static Control[] NewSizeRadios()
    {
      Control[] controlArray1 = new Control[SizeHelper.StockSizes.Length];
      int num = 0;
      foreach (Size stockSiz in SizeHelper.StockSizes)
      {
        Control[] controlArray2 = controlArray1;
        int index = num++;
        RadioButton radioButton1 = new RadioButton();
        radioButton1.Tag = (object) stockSiz;
        radioButton1.Text = stockSiz.ToFormattedString();
        RadioButton radioButton2 = radioButton1;
        controlArray2[index] = (Control) radioButton2;
      }
      return controlArray1;
    }

    public static void AddButtonsAndSizeForm(Form form, Button okButton, Button cancelButton)
    {
      int val1_1 = 0;
      int val1_2 = 0;
      foreach (Control control in (ArrangedElementCollection) form.Controls)
      {
        val1_1 = Math.Max(val1_1, control.Right);
        val1_2 = Math.Max(val1_2, control.Bottom);
      }
      int width = val1_1 + 8;
      cancelButton.Location = new Point(width - cancelButton.Width - 8 - 1, val1_2 + 16);
      okButton.Location = new Point(cancelButton.Location.X - okButton.Width - 8, cancelButton.Location.Y);
      form.AcceptButton = (IButtonControl) okButton;
      form.CancelButton = (IButtonControl) cancelButton;
      form.Controls.Add((Control) cancelButton);
      form.Controls.Add((Control) okButton);
      form.ClientSize = new Size(width, okButton.Location.Y + okButton.Height + 16);
    }

    public static int ComputeControlHeightFromChildren(Control control)
    {
      int val1 = 0;
      foreach (Control control1 in (ArrangedElementCollection) control.Controls)
        val1 = Math.Max(val1, control1.Bottom);
      return val1;
    }

    public static void ErrorDialog(string text)
    {
      int num = (int) MessageBox.Show((IWin32Window) Program.TheForm, text, "RDCMan Error", MessageBoxButtons.OK, MessageBoxIcon.Hand);
    }

    public static void InformationDialog(string text)
    {
      int num = (int) MessageBox.Show((IWin32Window) Program.TheForm, text, "RDCMan", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
    }

    public static DialogResult ExclamationDialog(string text, MessageBoxButtons buttons) => MessageBox.Show((IWin32Window) Program.TheForm, text, "RDCMan", buttons, MessageBoxIcon.Exclamation);

    public static DialogResult YesNoDialog(string text) => FormTools.YesNoDialog((Form) Program.TheForm, text, MessageBoxDefaultButton.Button2);

    public static DialogResult YesNoDialog(
      string text,
      MessageBoxDefaultButton defaultButton)
    {
      return FormTools.YesNoDialog((Form) Program.TheForm, text, defaultButton);
    }

    public static DialogResult YesNoDialog(Form owner, string text) => FormTools.YesNoDialog(owner, text, MessageBoxDefaultButton.Button2);

    public static DialogResult YesNoDialog(
      Form owner,
      string text,
      MessageBoxDefaultButton defaultButton)
    {
      return MessageBox.Show((IWin32Window) owner, text, "RDCMan", MessageBoxButtons.YesNo, MessageBoxIcon.Question, defaultButton);
    }

    public static DialogResult YesNoCancelDialog(string text) => FormTools.YesNoCancelDialog(text, MessageBoxDefaultButton.Button2);

    public static DialogResult YesNoCancelDialog(
      string text,
      MessageBoxDefaultButton defaultButton)
    {
      return MessageBox.Show((IWin32Window) Program.TheForm, text, "RDCMan", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question, defaultButton);
    }

    public static void ScaleAndLayout(this Form form)
    {
      form.PerformAutoScale();
      form.ResumeLayout(false);
      form.PerformLayout();
    }
  }
}
