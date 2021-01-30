// Decompiled with JetBrains decompiler
// Type: RdcMan.TabbedSettingsDialog
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
  public class TabbedSettingsDialog : RdcDialog
  {
    private TabPage _initiallyActiveTab;
    private readonly TabControl _tabControl;

    public TabbedSettingsDialog(string dialogTitle, string acceptButtonText, Form parentForm)
      : base(dialogTitle, acceptButtonText, parentForm)
    {
      TabControl tabControl = new TabControl();
      tabControl.Location = new Point(8, 8);
      tabControl.SelectedIndex = 0;
      tabControl.Size = new Size(520, 350);
      tabControl.Appearance = TabAppearance.Normal;
      tabControl.Multiline = true;
      this._tabControl = tabControl;
      this.Controls.Add((Control) this._tabControl);
    }

    public IEnumerable<TabPage> TabPages => this._tabControl.TabPages.Cast<TabPage>();

    public void AddTabPage(TabPage page) => this._tabControl.TabPages.Add(page);

    public void SetActiveTab(string name) => this._initiallyActiveTab = this.TabPages.Where<TabPage>((Func<TabPage, bool>) (p => p.Text == name)).FirstOrDefault<TabPage>();

    public void UpdateSettings()
    {
      foreach (ISettingsTabPage settingsTabPage in this.TabPages.OfType<ISettingsTabPage>())
        settingsTabPage.UpdateSettings();
    }

    public void EnableTabs(EnableTabsEventArgs args)
    {
      foreach (string tabName in args.TabNames)
      {
        string name = tabName;
        (this.TabPages.Where<TabPage>((Func<TabPage, bool>) (p => p.Text.Equals(name))).First<TabPage>() as ISettingsTabPage).InheritanceControl.Enable(args.Enabled, args.Reason);
      }
    }

    protected override void ShownCallback(object sender, EventArgs args)
    {
      foreach (TabPage tabPage in this.TabPages)
      {
        tabPage.Enabled = this._acceptButton.Enabled;
        if (tabPage is ISettingsTabPage settingsTabPage)
        {
          settingsTabPage.UpdateControls();
          if (settingsTabPage.FocusControl != null)
          {
            this._tabControl.SelectedTab = tabPage;
            settingsTabPage.FocusControl.Focus();
          }
        }
      }
      if (this._initiallyActiveTab == null)
        return;
      this._tabControl.SelectedTab = this._initiallyActiveTab;
    }

    protected override void AcceptIfValid(object sender, EventArgs e)
    {
      TabPage tabPage1 = (TabPage) null;
      foreach (TabPage tabPage2 in this.TabPages)
      {
        if (tabPage2 is ISettingsTabPage settingsTabPage && !settingsTabPage.Validate() && (tabPage1 == null || tabPage2 == this._tabControl.SelectedTab))
          tabPage1 = tabPage2;
      }
      if (tabPage1 == null)
        base.AcceptIfValid(sender, e);
      else
        this._tabControl.SelectedTab = tabPage1;
    }
  }
}
