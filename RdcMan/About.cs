// Decompiled with JetBrains decompiler
// Type: RdcMan.About
// Assembly: RDCMan, Version=2.7.1406.0, Culture=neutral, PublicKeyToken=null
// MVID: 984CDC7A-116B-406E-B519-0DEE541B9019
// Assembly location: C:\Program Files (x86)\Microsoft\Remote Desktop Connection Manager\RDCMan.exe

using System;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace RdcMan
{
  internal class About : Form
  {
    private void InitializeComponent(bool isLarge)
    {
      int width1 = isLarge ? 400 : 350;
      int width2 = width1 - 26;
      Label label1 = new Label();
      label1.Location = new Point(13, 13);
      label1.Size = new Size(width2, 28);
      label1.Text = "{1}{0}{2}".InvariantFormat((object) Environment.NewLine, (object) Program.TheForm.DescriptionText, (object) "by Julian Burger");
      Label label2 = label1;
      Panel panel = new Panel();
      panel.AutoScroll = true;
      panel.Location = new Point(13, 55);
      panel.Size = new Size(width2, 110);
      StringBuilder versionText = new StringBuilder();
      versionText.AppendLine("RDCMan v{0} build {1}".InvariantFormat((object) Program.TheForm.VersionText, (object) Program.TheForm.BuildText)).AppendLine().AppendLine(Environment.OSVersion.ToString()).AppendLine(".NET v{0}".InvariantFormat((object) Environment.Version)).AppendLine("mstscax.dll v{0}".InvariantFormat((object) RdpClient.RdpControlVersion));
      bool first = true;
      Program.PluginAction((Action<IPlugin>) (p =>
      {
        if (first)
        {
          versionText.AppendLine().AppendLine("Plugins:");
          first = false;
        }
        versionText.AppendLine(p.GetType().FullName);
      }));
      TextBox textBox = new TextBox();
      textBox.ScrollBars = ScrollBars.Vertical;
      textBox.BackColor = this.BackColor;
      textBox.Enabled = true;
      textBox.ForeColor = SystemColors.WindowText;
      textBox.Location = new Point(13, 55);
      textBox.Multiline = true;
      textBox.ReadOnly = true;
      textBox.Size = new Size(width2, 110);
      textBox.Text = versionText.ToString();
      TextBox versionLabel = textBox;
      versionLabel.VisibleChanged += (EventHandler) ((s, e) => versionLabel.Select(0, 0));
      Button button1 = new Button();
      button1.TabIndex = 1;
      button1.Text = "OK";
      Button button2 = button1;
      button2.Location = new Point(width2 - button2.Width, 167);
      this.AutoScaleDimensions = new SizeF(96f, 96f);
      this.AutoScaleMode = AutoScaleMode.Dpi;
      this.FormBorderStyle = FormBorderStyle.FixedDialog;
      this.ClientSize = new Size(width1, 200);
      this.Controls.Add((Control) versionLabel);
      this.Controls.Add((Control) label2);
      this.Controls.Add((Control) button2);
      this.MaximizeBox = false;
      this.MinimizeBox = false;
      this.SizeGripStyle = SizeGripStyle.Hide;
      this.AcceptButton = (IButtonControl) button2;
      this.CancelButton = (IButtonControl) button2;
      this.Text = "About Remote Desktop Connection Manager";
      this.StartPosition = FormStartPosition.CenterParent;
      this.ScaleAndLayout();
    }

    public About(bool isLarge) => this.InitializeComponent(isLarge);
  }
}
