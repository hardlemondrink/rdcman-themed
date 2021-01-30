// Decompiled with JetBrains decompiler
// Type: RdcMan.LongRunningActionForm
// Assembly: RDCMan, Version=2.7.1406.0, Culture=neutral, PublicKeyToken=null
// MVID: 984CDC7A-116B-406E-B519-0DEE541B9019
// Assembly location: C:\Program Files (x86)\Microsoft\Remote Desktop Connection Manager\RDCMan.exe

using System;
using System.Drawing;
using System.Threading;
using System.Windows.Forms;

namespace RdcMan
{
  internal class LongRunningActionForm : Form
  {
    private const int PopupDelayInSeconds = 2;
    private const int UpdateFrequencyInMilliseconds = 25;
    private readonly Label _statusLabel;
    private double _lastUpdateInMilliseconds;
    private DateTime _startTime;

    public bool Done { get; protected set; }

    public static LongRunningActionForm Instance { get; private set; }

    private LongRunningActionForm()
    {
      this.SuspendLayout();
      this.AutoScaleDimensions = new SizeF(96f, 96f);
      this.AutoScaleMode = AutoScaleMode.Dpi;
      ProgressBar progressBar1 = new ProgressBar();
      progressBar1.Location = FormTools.NewLocation(0, 0);
      progressBar1.Size = new Size(450, 20);
      progressBar1.Style = ProgressBarStyle.Marquee;
      ProgressBar progressBar2 = progressBar1;
      Label label = new Label();
      label.AutoEllipsis = true;
      label.Location = FormTools.NewLocation(0, 1);
      label.Size = progressBar2.Size;
      this._statusLabel = label;
      this.ClientSize = new Size(466, this._statusLabel.Bottom + 16);
      this.StartPosition = FormStartPosition.Manual;
      this.Location = new Point(Program.TheForm.Left + (Program.TheForm.Width - this.Width) / 2, Program.TheForm.Top + (Program.TheForm.Height - this.Height) / 2);
      this.ControlBox = false;
      this.FormBorderStyle = FormBorderStyle.FixedDialog;
      this.Controls.AddRange(new Control[2]
      {
        (Control) progressBar2,
        (Control) this._statusLabel
      });
      this.FormClosing += new FormClosingEventHandler(this.FormClosingHandler);
      this.Shown += new EventHandler(this.ShownHandler);
      this.ScaleAndLayout();
    }

    public static void PerformOperation(string title, bool showImmediately, Action action)
    {
      LongRunningActionForm runningActionForm = new LongRunningActionForm();
      runningActionForm.Text = title;
      LongRunningActionForm form = runningActionForm;
      try
      {
        Program.TheForm.Enabled = false;
        form._startTime = DateTime.Now;
        if (showImmediately)
          form.MakeVisible();
        LongRunningActionForm.Instance = form;
        action();
      }
      finally
      {
        if (form.Visible)
        {
          form.Done = true;
          form.Invoke((Delegate) (() => form.Close()));
        }
        LongRunningActionForm.Instance = (LongRunningActionForm) null;
        Program.TheForm.Enabled = true;
      }
    }

    public void UpdateStatus(string statusText)
    {
      TimeSpan timeSpan = DateTime.Now.Subtract(this._startTime);
      if (!this.Visible && timeSpan.TotalSeconds >= 2.0)
        this.MakeVisible();
      if (!this.Visible || timeSpan.TotalMilliseconds - this._lastUpdateInMilliseconds < 25.0)
        return;
      this._lastUpdateInMilliseconds = timeSpan.TotalMilliseconds;
      this.Invoke((Delegate) (() => this._statusLabel.Text = statusText));
    }

    private void MakeVisible()
    {
      ThreadPool.QueueUserWorkItem((WaitCallback) (o => Application.Run(new ApplicationContext((Form) this))));
      SpinWait.SpinUntil((Func<bool>) (() => this.Visible));
    }

    private void ShownHandler(object sender, EventArgs e) => this.BringToFront();

    private void FormClosingHandler(object sender, FormClosingEventArgs e)
    {
      if (this.Done || e.CloseReason != CloseReason.UserClosing)
        return;
      e.Cancel = true;
    }
  }
}
