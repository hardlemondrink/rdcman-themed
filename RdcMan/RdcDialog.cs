// Decompiled with JetBrains decompiler
// Type: RdcMan.RdcDialog
// Assembly: RDCMan, Version=2.7.1406.0, Culture=neutral, PublicKeyToken=null
// MVID: 984CDC7A-116B-406E-B519-0DEE541B9019
// Assembly location: C:\Program Files (x86)\Microsoft\Remote Desktop Connection Manager\RDCMan.exe

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace RdcMan
{
  public abstract class RdcDialog : Form
  {
    protected Button _acceptButton;
    protected Button _cancelButton;
    private readonly Dictionary<Control, ErrorProvider> _errorProviders;

    protected RdcDialog(string dialogTitle, string acceptButtonText)
    {
      this._errorProviders = new Dictionary<Control, ErrorProvider>();
      this.SuspendLayout();
      this.AutoScaleDimensions = new SizeF(96f, 96f);
      this.AutoScaleMode = AutoScaleMode.Dpi;
      this.FormBorderStyle = FormBorderStyle.FixedDialog;
      this.MaximizeBox = false;
      this.MinimizeBox = false;
      this.ShowInTaskbar = false;
      this.StartPosition = FormStartPosition.CenterParent;
      this.Text = dialogTitle;
      Button button = new Button();
      button.Text = acceptButtonText;
      this._acceptButton = button;
      this._cancelButton = new Button();
      this._cancelButton.Click += new EventHandler(this.CancelButton_Click);
      this.Shown += new EventHandler(this.ShownCallback);
    }

    private void CancelButton_Click(object sender, EventArgs e) => this._errorProviders.ForEach<KeyValuePair<Control, ErrorProvider>>((Action<KeyValuePair<Control, ErrorProvider>>) (kvp => kvp.Value.Clear()));

    protected RdcDialog(string dialogTitle, string acceptButtonText, Form parentForm)
      : this(dialogTitle, acceptButtonText)
    {
      if (parentForm == null)
        return;
      this.StartPosition = FormStartPosition.Manual;
      this.Location = new Point(parentForm.Location.X + 10, parentForm.Location.Y + 20);
    }

    public bool SetError(Control c, string text)
    {
      ErrorProvider errorProvider;
      if (!this._errorProviders.TryGetValue(c, out errorProvider))
      {
        errorProvider = new ErrorProvider();
        errorProvider.SetIconAlignment(c, ErrorIconAlignment.MiddleLeft);
        this._errorProviders[c] = errorProvider;
      }
      errorProvider.SetError(c, text);
      return !string.IsNullOrEmpty(text);
    }

    public virtual void InitButtons()
    {
      this._cancelButton.TabIndex = 100;
      this._cancelButton.Text = "Cancel";
      this._cancelButton.DialogResult = DialogResult.Cancel;
      this._acceptButton.TabIndex = 99;
      this._acceptButton.Click += new EventHandler(this.AcceptIfValid);
      FormTools.AddButtonsAndSizeForm((Form) this, this._acceptButton, this._cancelButton);
    }

    protected virtual void ShownCallback(object sender, EventArgs args)
    {
    }

    protected void Close(DialogResult dr)
    {
      this.DialogResult = dr;
      this.Close();
    }

    protected void OK() => this.Close(DialogResult.OK);

    protected void Cancel() => this.Close(DialogResult.Cancel);

    protected virtual void AcceptIfValid(object sender, EventArgs e)
    {
      if (!this.ValidateControls(this.Controls.FlattenControls(), true))
        return;
      this.OK();
    }

    public bool ValidateControls(IEnumerable<Control> controls, bool isValid)
    {
      foreach (Control control in controls)
      {
        if (control is ISettingControl settingControl && control.Enabled)
        {
          string text = settingControl.Validate();
          if (this.SetError(control, text) && isValid)
          {
            control.Focus();
            isValid = false;
          }
        }
      }
      return isValid;
    }
  }
}
