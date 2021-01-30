// Decompiled with JetBrains decompiler
// Type: RdcMan.CustomSizeDialog
// Assembly: RDCMan, Version=2.7.1406.0, Culture=neutral, PublicKeyToken=null
// MVID: 984CDC7A-116B-406E-B519-0DEE541B9019
// Assembly location: C:\Program Files (x86)\Microsoft\Remote Desktop Connection Manager\RDCMan.exe

using System;
using System.Drawing;
using System.Windows.Forms;

namespace RdcMan
{
  public class CustomSizeDialog : RdcDialog
  {
    private NumericTextBox _widthTextBox;
    private NumericTextBox _heightTextBox;
    private Button _fourThreeButton;
    private Button _sixteenNineButton;
    private Button _sixteenTenButton;

    public CustomSizeDialog(Size size)
      : base("Custom Size", "OK")
    {
      this.InitComp();
      this._widthTextBox.Text = size.Width.ToString();
      this._heightTextBox.Text = size.Height.ToString();
    }

    public string WidthText => this._widthTextBox.Text;

    public string HeightText => this._heightTextBox.Text;

    private void InitComp()
    {
      Label label1 = new Label();
      Label label2 = new Label();
      this._fourThreeButton = new Button();
      this._sixteenNineButton = new Button();
      this._sixteenTenButton = new Button();
      int num1 = 0;
      label1.Location = new Point(8, 8);
      label1.Size = new Size(50, 23);
      label1.Text = "&Width:";
      label1.TextAlign = ContentAlignment.MiddleLeft;
      label2.Location = new Point(8, 40);
      label2.Size = new Size(50, 23);
      label2.Text = "&Height:";
      label2.TextAlign = ContentAlignment.MiddleLeft;
      this._widthTextBox = new NumericTextBox(1, int.MaxValue, "Width must be at least 1 pixel");
      this._widthTextBox.Location = new Point(72, 8);
      this._widthTextBox.Size = new Size(75, 20);
      NumericTextBox widthTextBox = this._widthTextBox;
      int num2 = num1;
      int num3 = num2 + 1;
      widthTextBox.TabIndex = num2;
      this._heightTextBox = new NumericTextBox(1, int.MaxValue, "Height must be at least 1 pixel");
      this._heightTextBox.Location = new Point(72, 40);
      this._heightTextBox.Size = new Size(75, 20);
      NumericTextBox heightTextBox = this._heightTextBox;
      int num4 = num3;
      int num5 = num4 + 1;
      heightTextBox.TabIndex = num4;
      this._fourThreeButton.Location = new Point(160, 8);
      Button fourThreeButton = this._fourThreeButton;
      int num6 = num5;
      int num7 = num6 + 1;
      fourThreeButton.TabIndex = num6;
      this._fourThreeButton.Text = "&4 x 3";
      this._fourThreeButton.Click += new EventHandler(this.fourThreeButton_Click);
      this._sixteenNineButton.Location = new Point(160, 40);
      Button sixteenNineButton = this._sixteenNineButton;
      int num8 = num7;
      int num9 = num8 + 1;
      sixteenNineButton.TabIndex = num8;
      this._sixteenNineButton.Text = "1&6 x 9";
      this._sixteenNineButton.Click += new EventHandler(this.sixteenNineButton_Click);
      this._sixteenTenButton.Location = new Point(160, 72);
      Button sixteenTenButton = this._sixteenTenButton;
      int num10 = num9;
      int num11 = num10 + 1;
      sixteenTenButton.TabIndex = num10;
      this._sixteenTenButton.Text = "16 x 1&0";
      this._sixteenTenButton.Click += new EventHandler(this.sixteenTenButton_Click);
      this.ClientSize = new Size(238, 143);
      this.Controls.Add((Control) this._sixteenTenButton, (Control) this._sixteenNineButton, (Control) this._fourThreeButton, (Control) label1, (Control) this._widthTextBox, (Control) label2, (Control) this._heightTextBox);
      this.InitButtons();
      this.ScaleAndLayout();
    }

    private void fourThreeButton_Click(object sender, EventArgs e) => this._heightTextBox.Text = (int.Parse(this._widthTextBox.Text) * 3 / 4).ToString();

    private void sixteenNineButton_Click(object sender, EventArgs e) => this._heightTextBox.Text = (int.Parse(this._widthTextBox.Text) * 9 / 16).ToString();

    private void sixteenTenButton_Click(object sender, EventArgs e) => this._heightTextBox.Text = (int.Parse(this._widthTextBox.Text) * 10 / 16).ToString();
  }
}
