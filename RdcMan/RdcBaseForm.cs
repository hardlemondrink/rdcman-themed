// Decompiled with JetBrains decompiler
// Type: RdcMan.RdcBaseForm
// Assembly: RDCMan, Version=2.7.1406.0, Culture=neutral, PublicKeyToken=null
// MVID: 984CDC7A-116B-406E-B519-0DEE541B9019
// Assembly location: C:\Program Files (x86)\Microsoft\Remote Desktop Connection Manager\RDCMan.exe

using System;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using System.Windows.Forms.Layout;
using Win32;

namespace RdcMan
{
  public abstract class RdcBaseForm : Form
  {
    private const bool WindowedFullScreen = false;
    private IntPtr NCButtonDownLParam;
    private Rectangle _savedBounds;
    private FormBorderStyle _savedBorderStyle;
    protected RdcMenuStrip _menuStrip;
    protected Panel _menuPanel;

    protected RdcBaseForm()
    {
      this.AutoScaleDimensions = new SizeF(96f, 96f);
      this.AutoScaleMode = AutoScaleMode.Dpi;
      Panel panel = new Panel();
      panel.Dock = DockStyle.None;
      this._menuPanel = panel;
      RdcMenuStrip rdcMenuStrip = new RdcMenuStrip();
      rdcMenuStrip.BackColor = Color.FromKnownColor(KnownColor.Control);
      rdcMenuStrip.ForeColor = Color.FromKnownColor(KnownColor.ControlText);
      rdcMenuStrip.Visible = true;
      this._menuStrip = rdcMenuStrip;
      this._menuStrip.MenuActivate += (EventHandler) ((s, e) =>
      {
        this.SetMainMenuVisibility(true);
        this.UpdateMainMenu();
      });
      this._menuPanel.Controls.Add((Control) this._menuStrip);
      this.Controls.Add((Control) this._menuPanel);
    }

    public bool IsActive => Form.ActiveForm == this;

    public abstract void SetClientSize(Size size);

    public abstract Size GetClientSize();

    public void SetMainMenuVisibility() => this.SetMainMenuVisibility(!Program.Preferences.HideMainMenu);

    public bool SetMainMenuVisibility(bool show)
    {
      int num = show ? this._menuStrip.Height : 0;
      if (this._menuPanel.Height != num)
      {
        this._menuPanel.Height = num;
        this.LayoutContent();
      }
      return show;
    }

    public virtual void GoFullScreenClient(Server server, bool isTopMostWindow)
    {
      RdpClient client = server.Client;
      Rectangle rectangle = Screen.GetBounds(client.Control);
      if (Program.Preferences.UseMultipleMonitors && (rectangle.Height < client.MsRdpClient.DesktopHeight || rectangle.Width < client.MsRdpClient.DesktopWidth))
      {
        int val1 = 0;
        int num = (int) ushort.MaxValue;
        foreach (Screen allScreen in Screen.AllScreens)
        {
          val1 += allScreen.Bounds.Width;
          num = Math.Min(allScreen.Bounds.Height, num);
        }
        rectangle = new Rectangle(0, 0, Math.Min(val1, RdpClient.MaxDesktopWidth), Math.Min(num, RdpClient.MaxDesktopHeight));
      }
      this._savedBounds = this.Bounds;
      this._savedBorderStyle = this.FormBorderStyle;
      RdcBaseForm.DrawingControl.SuspendDrawing((Control) this);
      this.SuspendLayout();
      this.FormBorderStyle = FormBorderStyle.None;
      this.SetMainMenuVisibility(false);
      this.SetBounds(rectangle.Left, rectangle.Top, rectangle.Width, rectangle.Height);
      server.SetClientSizeProperties();
      client.Control.Bounds = new Rectangle(0, 0, this.Width, this.Height);
      this.ResumeLayout();
      this.TopMost = isTopMostWindow;
      client.Control.Show();
      RdcBaseForm.DrawingControl.ResumeDrawing((Control) this);
      this.BringToFront();
      this.Activate();
    }

    public virtual bool SwitchFullScreenClient(Server newServer) => false;

    public virtual void LeaveFullScreenClient(Server server)
    {
      RdcBaseForm.DrawingControl.SuspendDrawing((Control) this);
      this.SuspendLayout();
      this.FormBorderStyle = this._savedBorderStyle;
      this.SetMainMenuVisibility();
      this.Bounds = this._savedBounds;
      this.ResumeLayout();
      this.TopMost = false;
      RdcBaseForm.DrawingControl.ResumeDrawing((Control) this);
      server.SetClientSizeProperties();
      this.Activate();
    }

    protected override void OnLeave(EventArgs e)
    {
      if (Program.Preferences.HideMainMenu && this._menuPanel.Height > 0)
        User.SendMessage(this._menuStrip.Handle, 16U, IntPtr.Zero, IntPtr.Zero);
      base.OnLeave(e);
    }

    protected abstract void UpdateMainMenu();

    protected abstract void LayoutContent();

    protected void UpdateMenuItems(ToolStripItemCollection items)
    {
      foreach (ToolStripItem toolStripItem in (ArrangedElementCollection) items)
      {
        if (toolStripItem is RdcMenuItem)
          (toolStripItem as RdcMenuItem).Update();
        if (toolStripItem is ToolStripMenuItem toolStripMenuItem && toolStripMenuItem.DropDownItems != null)
          this.UpdateMenuItems(toolStripMenuItem.DropDownItems);
      }
    }

    protected override bool ProcessKeyPreview(ref Message m)
    {
      if (Program.Preferences.HideMainMenu && m.WParam == (IntPtr) 18L && m.Msg == 261)
        this.SetMainMenuVisibility(this._menuPanel.Height == 0);
      return base.ProcessKeyPreview(ref m);
    }

    private void SetNonClientTracking(int hoverMilliseconds)
    {
      Structs.TRACKMOUSEEVENT lpEventTrack = new Structs.TRACKMOUSEEVENT(17U, this.Handle, (uint) hoverMilliseconds);
      if (hoverMilliseconds < 0)
        lpEventTrack.dwFlags |= 2147483648U;
      User.TrackMouseEvent(ref lpEventTrack);
      Marshal.GetLastWin32Error();
    }

    protected override void OnDeactivate(EventArgs e)
    {
      base.OnDeactivate(e);
      this.SetMainMenuVisibility();
    }

    protected override void WndProc(ref Message m)
    {
      switch ((uint) m.Msg)
      {
        case 33:
          if (Program.Preferences.HideMainMenu)
          {
            this.SetMainMenuVisibility();
            break;
          }
          break;
        case 160:
          this.SetNonClientTracking(100);
          break;
        case 161:
          this.NCButtonDownLParam = m.LParam;
          this.SetNonClientTracking(100);
          break;
        case 672:
          if (this.IsActive && Program.Preferences.HideMainMenu && (m.WParam.ToInt32() == 2 && m.LParam == this.NCButtonDownLParam && ((int) User.GetAsyncKeyState(1) & 32768) == 0))
          {
            this.SetMainMenuVisibility(this._menuPanel.Height == 0);
            this.SetNonClientTracking(-1);
            this.NCButtonDownLParam = IntPtr.Zero;
            return;
          }
          break;
        case 674:
          this.SetNonClientTracking(-1);
          break;
      }
      base.WndProc(ref m);
    }

    private class DrawingControl
    {
      public static void SuspendDrawing(Control parent) => User.SendMessage(parent.Handle, 11U, (IntPtr) 0, (IntPtr) 0);

      public static void ResumeDrawing(Control parent)
      {
        User.SendMessage(parent.Handle, 11U, (IntPtr) 1, (IntPtr) 0);
        parent.Refresh();
      }
    }
  }
}
