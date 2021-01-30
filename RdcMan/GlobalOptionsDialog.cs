// Decompiled with JetBrains decompiler
// Type: RdcMan.GlobalOptionsDialog
// Assembly: RDCMan, Version=2.7.1406.0, Culture=neutral, PublicKeyToken=null
// MVID: 984CDC7A-116B-406E-B519-0DEE541B9019
// Assembly location: C:\Program Files (x86)\Microsoft\Remote Desktop Connection Manager\RDCMan.exe

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using System.Windows.Forms.Layout;

namespace RdcMan
{
  public class GlobalOptionsDialog : TabbedSettingsDialog
  {
    private GroupBox _virtualGroupsGroup;
    private ValueComboBox<ControlVisibility> _treeVisibilityCombo;
    private ValueComboBox<DockStyle> _treeLocationCombo;
    private CheckBox _connectionBarEnabledCheckBox;
    private CheckBox _connectionBarAutoHiddenCheckBox;
    private RdcCheckBox _enablePanningCheckBox;
    private RdcNumericUpDown _panningAccelerationUpDown;
    private GroupBox _casSizeGroup;
    private RadioButton _casCustomRadio;
    private RadioButton _thumbnailPixelsRadio;
    private RadioButton _thumbnailPercentageRadio;
    private TextBox _thumbnailPercentageTextBox;
    private Button _casCustomButton;
    private Button _thumbnailPixelsButton;
    private ValueComboBox<GlobalOptionsDialog.BandwidthItem> _bandwidthComboBox;
    private CheckBox _desktopBackgroundCheckBox;
    private CheckBox _fontSmoothingCheckBox;
    private CheckBox _desktopCompositionCheckBox;
    private CheckBox _windowDragCheckBox;
    private CheckBox _menuAnimationCheckBox;
    private CheckBox _themesCheckBox;
    private bool _inHandler;
    private readonly GlobalOptionsDialog.BandwidthItem[] _bandwidthItems;

    protected GlobalOptionsDialog(Form parentForm)
      : base("Options", "OK", parentForm)
    {
      this._bandwidthItems = new GlobalOptionsDialog.BandwidthItem[5]
      {
        new GlobalOptionsDialog.BandwidthItem("Modem (28.8 Kbps)", 15),
        new GlobalOptionsDialog.BandwidthItem("Modem (56 Kbps)", 7),
        new GlobalOptionsDialog.BandwidthItem("Broadband (128 Kpbs - 1.5 Mbps)", 257),
        new GlobalOptionsDialog.BandwidthItem("LAN (10 Mbps or higher)", 384),
        new GlobalOptionsDialog.BandwidthItem("Custom", 0)
      };
      this.InitializeComponent();
    }

    public static GlobalOptionsDialog New()
    {
      GlobalOptionsDialog globalOptionsDialog = new GlobalOptionsDialog((Form) Program.TheForm);
      globalOptionsDialog.InitializeControlsFromPreferences();
      return globalOptionsDialog;
    }

    private void InitializeControlsFromPreferences()
    {
      MainForm theForm = Program.TheForm;
      foreach (CheckBox control in (ArrangedElementCollection) this._virtualGroupsGroup.Controls)
        control.Checked = (control.Tag as IBuiltInVirtualGroup).IsInTree;
      this._treeLocationCombo.SelectedValue = Program.TheForm.ServerTreeLocation;
      this._treeVisibilityCombo.SelectedValue = Program.TheForm.ServerTreeVisibility;
      this._connectionBarEnabledCheckBox.Checked = Program.Preferences.ConnectionBarState != RdpClient.ConnectionBarState.Off;
      this._connectionBarAutoHiddenCheckBox.Checked = Program.Preferences.ConnectionBarState == RdpClient.ConnectionBarState.AutoHide;
      this._connectionBarAutoHiddenCheckBox.Enabled = this._connectionBarEnabledCheckBox.Enabled;
      if (RdpClient.SupportsPanning)
        this._panningAccelerationUpDown.Enabled = Program.Preferences.EnablePanning;
      Size clientSize = theForm.GetClientSize();
      RadioButton radioButton = this._casSizeGroup.Controls.OfType<RadioButton>().Where<RadioButton>((Func<RadioButton, bool>) (r =>
      {
        Size? tag = (Size?) r.Tag;
        Size size = clientSize;
        return tag.HasValue && tag.GetValueOrDefault() == size;
      })).FirstOrDefault<RadioButton>();
      if (radioButton != null)
        radioButton.Checked = true;
      else
        this._casCustomRadio.Checked = true;
      this._casCustomButton.Text = clientSize.ToFormattedString();
      this._thumbnailPixelsButton.Text = Program.Preferences.ThumbnailSize.ToFormattedString();
      this._thumbnailPercentageTextBox.Text = Program.Preferences.ThumbnailPercentage.ToString();
      if (Program.Preferences.ThumbnailSizeIsInPixels)
        this._thumbnailPixelsRadio.Checked = true;
      else
        this._thumbnailPercentageRadio.Checked = true;
      this.SetBandwidthCheckBoxes(Program.Preferences.PerformanceFlags);
    }

    public void UpdatePreferences()
    {
      this.UpdateSettings();
      MainForm theForm = Program.TheForm;
      foreach (CheckBox control in (ArrangedElementCollection) this._virtualGroupsGroup.Controls)
        (control.Tag as IBuiltInVirtualGroup).IsInTree = control.Checked;
      Program.TheForm.ServerTreeLocation = this._treeLocationCombo.SelectedValue;
      Program.TheForm.ServerTreeVisibility = this._treeVisibilityCombo.SelectedValue;
      Program.Preferences.ConnectionBarState = this._connectionBarEnabledCheckBox.Checked ? (this._connectionBarAutoHiddenCheckBox.Checked ? RdpClient.ConnectionBarState.AutoHide : RdpClient.ConnectionBarState.Pinned) : RdpClient.ConnectionBarState.Off;
      Program.Preferences.PerformanceFlags = this.ComputeFlagsFromCheckBoxes();
      string text = this._casCustomButton.Text;
      if (!this._casCustomRadio.Checked)
        text = this._casSizeGroup.Controls.OfType<RadioButton>().Where<RadioButton>((Func<RadioButton, bool>) (r => r.Checked)).First<RadioButton>().Text;
      Size size = SizeHelper.Parse(text);
      if (theForm.GetClientSize() != size)
        theForm.SetClientSize(size);
      Program.Preferences.ThumbnailSize = SizeHelper.Parse(this._thumbnailPixelsButton.Text);
      Program.Preferences.ThumbnailSizeIsInPixels = this._thumbnailPixelsRadio.Checked;
      Program.Preferences.ThumbnailPercentage = int.Parse(this._thumbnailPercentageTextBox.Text);
    }

    private void InitializeComponent()
    {
      this.CreateGeneralPage();
      this.CreateServerTreePage();
      this.CreateClientAreaPage();
      this.CreateHotKeysPage();
      this.CreateExperiencePage();
      this.CreateFullScreenPage();
      this.InitButtons();
      this.ScaleAndLayout();
    }

    private TabPage NewTabPage(string name)
    {
      SettingsTabPage settingsTabPage = new SettingsTabPage();
      settingsTabPage.Location = FormTools.TopLeftLocation();
      settingsTabPage.Size = new Size(512, 334);
      settingsTabPage.Text = name;
      TabPage page = (TabPage) settingsTabPage;
      this.AddTabPage(page);
      return page;
    }

    private TabPage CreateFullScreenPage()
    {
      int num1 = 0;
      int num2 = 0;
      TabPage tabPage = this.NewTabPage("Full Screen");
      int rowIndex1 = num1;
      int num3 = rowIndex1 + 1;
      int tabIndex1 = num2;
      int num4 = tabIndex1 + 1;
      this._connectionBarEnabledCheckBox = (CheckBox) FormTools.NewCheckBox("Show full screen connection bar", 0, rowIndex1, tabIndex1);
      this._connectionBarEnabledCheckBox.CheckedChanged += new EventHandler(this.ConnectionBarEnabledCheckedChanged);
      int rowIndex2 = num3;
      int rowIndex3 = rowIndex2 + 1;
      int tabIndex2 = num4;
      int tabIndex3 = tabIndex2 + 1;
      this._connectionBarAutoHiddenCheckBox = (CheckBox) FormTools.NewCheckBox("Auto-hide connection bar", 0, rowIndex2, tabIndex2);
      this._connectionBarAutoHiddenCheckBox.Location = new Point(this._connectionBarEnabledCheckBox.Left + 24, this._connectionBarAutoHiddenCheckBox.Top);
      FormTools.AddCheckBox((Control) tabPage, "Full screen window is always on top", Program.Preferences.Settings.FullScreenWindowIsTopMost, 0, ref rowIndex3, ref tabIndex3);
      if (RdpClient.SupportsMonitorSpanning)
        FormTools.AddCheckBox((Control) tabPage, "Use multiple monitors when necessary", Program.Preferences.Settings.UseMultipleMonitors, 0, ref rowIndex3, ref tabIndex3);
      if (RdpClient.SupportsPanning)
      {
        int rowIndex4 = rowIndex3;
        int rowIndex5 = rowIndex4 + 1;
        this._enablePanningCheckBox = FormTools.NewCheckBox("Use panning instead of scroll bars", 0, rowIndex4, tabIndex3++);
        this._enablePanningCheckBox.Setting = Program.Preferences.Settings.EnablePanning;
        this._enablePanningCheckBox.CheckedChanged += new EventHandler(this.EnablePanningCheckedChanged);
        Label label = FormTools.NewLabel("Panning speed", 0, rowIndex5);
        label.Size = new Size(116, 20);
        label.Location = new Point(this._enablePanningCheckBox.Left + 24, label.Top);
        this._panningAccelerationUpDown = new RdcNumericUpDown();
        RdcNumericUpDown accelerationUpDown = this._panningAccelerationUpDown;
        int rowIndex6 = rowIndex5;
        int num5 = rowIndex6 + 1;
        Point point = FormTools.NewLocation(1, rowIndex6);
        accelerationUpDown.Location = point;
        this._panningAccelerationUpDown.Minimum = 1M;
        this._panningAccelerationUpDown.Maximum = 9M;
        this._panningAccelerationUpDown.Size = new Size(40, 20);
        this._panningAccelerationUpDown.TabIndex = tabIndex3++;
        this._panningAccelerationUpDown.Setting = Program.Preferences.Settings.PanningAcceleration;
        tabPage.Controls.Add((Control) this._enablePanningCheckBox, (Control) label, (Control) this._panningAccelerationUpDown);
      }
      tabPage.Controls.Add((Control) this._connectionBarEnabledCheckBox, (Control) this._connectionBarAutoHiddenCheckBox);
      return tabPage;
    }

    private TabPage CreateExperiencePage()
    {
      TabPage tabPage = this.NewTabPage("Experience");
      int rowIndex = 0;
      int tabIndex = 0;
      this._bandwidthComboBox = FormTools.AddLabeledValueDropDown<GlobalOptionsDialog.BandwidthItem>((Control) tabPage, "Connection &speed", ref rowIndex, ref tabIndex, (Func<GlobalOptionsDialog.BandwidthItem, string>) (v => v.Text), (IEnumerable<GlobalOptionsDialog.BandwidthItem>) this._bandwidthItems);
      this._bandwidthComboBox.SelectedIndexChanged += new EventHandler(this.BandwidthCombo_ControlChanged);
      Label label = FormTools.NewLabel("Allow the following:", 0, rowIndex);
      this._desktopBackgroundCheckBox = (CheckBox) FormTools.NewCheckBox("Desktop background", 1, rowIndex++, tabIndex++);
      this._desktopBackgroundCheckBox.CheckedChanged += new EventHandler(this.PerfCheckBox_CheckedChanged);
      this._fontSmoothingCheckBox = (CheckBox) FormTools.NewCheckBox("Font smoothing", 1, rowIndex++, tabIndex++);
      this._fontSmoothingCheckBox.CheckedChanged += new EventHandler(this.PerfCheckBox_CheckedChanged);
      this._desktopCompositionCheckBox = (CheckBox) FormTools.NewCheckBox("Desktop composition", 1, rowIndex++, tabIndex++);
      this._desktopCompositionCheckBox.CheckedChanged += new EventHandler(this.PerfCheckBox_CheckedChanged);
      this._windowDragCheckBox = (CheckBox) FormTools.NewCheckBox("Show contents of window while dragging", 1, rowIndex++, tabIndex++);
      this._windowDragCheckBox.CheckedChanged += new EventHandler(this.PerfCheckBox_CheckedChanged);
      this._menuAnimationCheckBox = (CheckBox) FormTools.NewCheckBox("Menu and window animation", 1, rowIndex++, tabIndex++);
      this._menuAnimationCheckBox.CheckedChanged += new EventHandler(this.PerfCheckBox_CheckedChanged);
      this._themesCheckBox = (CheckBox) FormTools.NewCheckBox("Themes", 1, rowIndex++, tabIndex++);
      this._themesCheckBox.CheckedChanged += new EventHandler(this.PerfCheckBox_CheckedChanged);
      tabPage.Controls.Add((Control) label, (Control) this._desktopBackgroundCheckBox, (Control) this._fontSmoothingCheckBox, (Control) this._desktopCompositionCheckBox, (Control) this._windowDragCheckBox, (Control) this._menuAnimationCheckBox, (Control) this._themesCheckBox);
      return tabPage;
    }

    private TabPage CreateHotKeysPage()
    {
      GlobalSettings settings = Program.Preferences.Settings;
      TabPage tabPage = this.NewTabPage("Hot Keys");
      GroupBox groupBox1 = new GroupBox();
      groupBox1.Text = "ALT hot keys (only effective when Windows key combos are not redirected)";
      GroupBox groupBox2 = groupBox1;
      int rowIndex1 = 0;
      int tabIndex1 = 0;
      this.AddHotKeyBox((Control) groupBox2, "ALT+TAB", "ALT+", settings.HotKeyAltTab, ref rowIndex1, ref tabIndex1);
      this.AddHotKeyBox((Control) groupBox2, "ALT+SHIFT+TAB", "ALT+", settings.HotKeyAltShiftTab, ref rowIndex1, ref tabIndex1);
      this.AddHotKeyBox((Control) groupBox2, "ALT+ESC", "ALT+", settings.HotKeyAltEsc, ref rowIndex1, ref tabIndex1);
      this.AddHotKeyBox((Control) groupBox2, "ALT+SPACE", "ALT+", settings.HotKeyAltSpace, ref rowIndex1, ref tabIndex1);
      this.AddHotKeyBox((Control) groupBox2, "CTRL+ESC", "ALT+", settings.HotKeyCtrlEsc, ref rowIndex1, ref tabIndex1);
      groupBox2.SizeAndLocate((Control) null);
      GroupBox groupBox3 = new GroupBox();
      groupBox3.Text = "CTRL+ALT hot keys (always effective)";
      GroupBox groupBox4 = groupBox3;
      int rowIndex2 = 0;
      int tabIndex2 = 0;
      this.AddHotKeyBox((Control) groupBox4, "CTRL+ALT+DEL", "CTRL+ALT+", settings.HotKeyCtrlAltDel, ref rowIndex2, ref tabIndex2);
      this.AddHotKeyBox((Control) groupBox4, "Full screen", "CTRL+ALT+", settings.HotKeyFullScreen, ref rowIndex2, ref tabIndex2);
      this.AddHotKeyBox((Control) groupBox4, "Previous session", "CTRL+ALT+", settings.HotKeyFocusReleaseLeft, ref rowIndex2, ref tabIndex2);
      this.AddHotKeyBox((Control) groupBox4, "Select session", "CTRL+ALT+", settings.HotKeyFocusReleaseRight, ref rowIndex2, ref tabIndex2);
      groupBox4.SizeAndLocate((Control) groupBox2);
      tabPage.Controls.Add((Control) groupBox2, (Control) groupBox4);
      return tabPage;
    }

    private void AddHotKeyBox(
      Control parent,
      string label,
      string prefix,
      EnumSetting<Keys> setting,
      ref int rowIndex,
      ref int tabIndex)
    {
      parent.Controls.Add((Control) FormTools.NewLabel(label, 0, rowIndex));
      HotKeyBox hotKeyBox1 = new HotKeyBox();
      hotKeyBox1.Prefix = prefix;
      hotKeyBox1.Location = FormTools.NewLocation(1, rowIndex++);
      hotKeyBox1.Size = new Size(340, 20);
      hotKeyBox1.TabIndex = tabIndex++;
      hotKeyBox1.Setting = setting;
      HotKeyBox hotKeyBox2 = hotKeyBox1;
      parent.Controls.Add((Control) hotKeyBox2);
    }

    private TabPage CreateClientAreaPage()
    {
      this._casCustomButton = new Button();
      this._casCustomRadio = new RadioButton();
      this._thumbnailPercentageRadio = new RadioButton();
      this._thumbnailPixelsRadio = new RadioButton();
      this._thumbnailPixelsButton = new Button();
      TabPage tabPage = this.NewTabPage("Client Area");
      GroupBox groupBox1 = new GroupBox();
      groupBox1.Text = "Client Area Size";
      this._casSizeGroup = groupBox1;
      this._casSizeGroup.Controls.AddRange(FormTools.NewSizeRadios());
      this._casCustomRadio.Size = new Size(72, 24);
      this._casCustomRadio.Text = "&Custom";
      this._casSizeGroup.Controls.Add((Control) this._casCustomRadio);
      FormTools.LayoutGroupBox(this._casSizeGroup, 2, (Control) null, 1, 1);
      RdcCheckBox rdcCheckBox = new RdcCheckBox();
      rdcCheckBox.Size = new Size(480, 24);
      rdcCheckBox.Text = "&Lock window size";
      rdcCheckBox.Location = FormTools.NewLocation(0, 0);
      rdcCheckBox.TabIndex = 0;
      rdcCheckBox.TabStop = true;
      rdcCheckBox.Setting = Program.Preferences.Settings.LockWindowSize;
      this._casSizeGroup.Controls.Add((Control) rdcCheckBox);
      this._casCustomButton.Location = new Point(this._casCustomRadio.Right + 10, this._casCustomRadio.Location.Y);
      this._casCustomButton.TabIndex = this._casCustomRadio.TabIndex + 1;
      this._casCustomButton.Click += new EventHandler(this.CustomSizeClick);
      this._casSizeGroup.Controls.Add((Control) this._casCustomButton);
      GroupBox groupBox2 = new GroupBox();
      groupBox2.Size = new Size(512, 72);
      groupBox2.Text = "Thumbnail Unit Size";
      GroupBox groupBox3 = groupBox2;
      groupBox3.Controls.Add((Control) this._thumbnailPixelsRadio, (Control) this._thumbnailPercentageRadio);
      this._thumbnailPixelsRadio.Size = new Size(80, 24);
      this._thumbnailPixelsRadio.Text = "Pi&xels";
      this._thumbnailPercentageRadio.Size = new Size(88, 24);
      this._thumbnailPercentageRadio.Text = "Pe&rcentage";
      this._thumbnailPercentageRadio.CheckedChanged += new EventHandler(this.ThumbnailPercentageRadioCheckedChanged);
      FormTools.LayoutGroupBox(groupBox3, 1, (Control) this._casSizeGroup);
      int num = Math.Max(this._thumbnailPixelsRadio.Right, this._thumbnailPercentageRadio.Right);
      this._thumbnailPixelsButton.Location = new Point(num + 10, this._thumbnailPixelsRadio.Location.Y);
      this._thumbnailPixelsButton.TabIndex = this._thumbnailPercentageRadio.TabIndex + 1;
      this._thumbnailPixelsButton.Click += new EventHandler(this.CustomSizeClick);
      this._thumbnailPercentageTextBox = (TextBox) new NumericTextBox(1, 100, "Percentage must be between 1 and 100 inclusive");
      this._thumbnailPercentageTextBox.Enabled = false;
      this._thumbnailPercentageTextBox.Location = new Point(num + 11, this._thumbnailPercentageRadio.Location.Y + 2);
      this._thumbnailPercentageTextBox.Size = new Size(72, 20);
      this._thumbnailPercentageTextBox.TabIndex = this._thumbnailPercentageRadio.TabIndex + 1;
      groupBox3.Controls.Add((Control) this._thumbnailPixelsButton, (Control) this._thumbnailPercentageTextBox);
      tabPage.Controls.Add((Control) this._casSizeGroup, (Control) groupBox3);
      return tabPage;
    }

    private TabPage CreateServerTreePage()
    {
      int rowIndex1 = 0;
      int tabIndex1 = 0;
      TabPage tabPage = this.NewTabPage("Tree");
      GroupBox groupBox1 = new GroupBox();
      groupBox1.Text = "Server Tree";
      GroupBox groupBox2 = groupBox1;
      FormTools.AddCheckBox((Control) groupBox2, "Click to select gives focus to remote client", Program.Preferences.Settings.FocusOnClick, 0, ref rowIndex1, ref tabIndex1);
      FormTools.AddCheckBox((Control) groupBox2, "Dim nodes when tree control is inactive", Program.Preferences.Settings.DimNodesWhenInactive, 0, ref rowIndex1, ref tabIndex1);
      this._treeLocationCombo = FormTools.AddLabeledValueDropDown<DockStyle>((Control) groupBox2, "Location", ref rowIndex1, ref tabIndex1, (Func<DockStyle, string>) (v => v.ToString()), (IEnumerable<DockStyle>) new DockStyle[2]
      {
        DockStyle.Left,
        DockStyle.Right
      });
      this._treeVisibilityCombo = FormTools.AddLabeledValueDropDown<ControlVisibility>((Control) groupBox2, "Visibility", ref rowIndex1, ref tabIndex1, (Func<ControlVisibility, string>) (v => v.ToString()), (IEnumerable<ControlVisibility>) new ControlVisibility[3]
      {
        ControlVisibility.Dock,
        ControlVisibility.AutoHide,
        ControlVisibility.Hide
      });
      Label label1 = FormTools.NewLabel("Pop up delay:", 0, rowIndex1++);
      label1.Left += 24;
      label1.Size = new Size(80, label1.Height);
      NumericTextBox numericTextBox = new NumericTextBox(0, 1000, "Auto hide pop-up delay must be 0 to 1000 milliseconds");
      numericTextBox.Enabled = false;
      numericTextBox.Location = new Point(label1.Right, label1.Top);
      numericTextBox.Size = new Size(40, 24);
      numericTextBox.Setting = Program.Preferences.Settings.ServerTreeAutoHidePopUpDelay;
      numericTextBox.TabStop = true;
      numericTextBox.TabIndex = tabIndex1++;
      NumericTextBox serverTreeAutoHidePopUpDelay = numericTextBox;
      this._treeVisibilityCombo.SelectedIndexChanged += (EventHandler) ((s, e) => serverTreeAutoHidePopUpDelay.Enabled = this._treeVisibilityCombo.SelectedValue == ControlVisibility.AutoHide);
      groupBox2.AddControlsAndSizeGroup((Control) label1);
      Label label2 = new Label();
      label2.Location = new Point(serverTreeAutoHidePopUpDelay.Right + 3, label1.Top);
      label2.Size = new Size(80, 24);
      label2.Text = "millisecond(s)";
      Label label3 = label2;
      groupBox2.Controls.Add((Control) serverTreeAutoHidePopUpDelay, (Control) label3);
      groupBox2.SizeAndLocate((Control) null);
      GroupBox groupBox3 = new GroupBox();
      groupBox3.Text = "Virtual Groups";
      this._virtualGroupsGroup = groupBox3;
      foreach (IBuiltInVirtualGroup builtInVirtualGroup in Program.BuiltInVirtualGroups.Where<IBuiltInVirtualGroup>((Func<IBuiltInVirtualGroup, bool>) (group => group.IsVisibilityConfigurable)))
      {
        Control.ControlCollection controls = this._virtualGroupsGroup.Controls;
        CheckBox checkBox1 = new CheckBox();
        checkBox1.Size = new Size(112, 24);
        checkBox1.Tag = (object) builtInVirtualGroup;
        checkBox1.Text = builtInVirtualGroup.Text;
        CheckBox checkBox2 = checkBox1;
        controls.Add((Control) checkBox2);
      }
      FormTools.LayoutGroupBox(this._virtualGroupsGroup, 2, (Control) groupBox2);
      int rowIndex2 = 0;
      int tabIndex2 = 0;
      GroupBox groupBox4 = new GroupBox();
      FormTools.AddLabeledValueDropDown<SortOrder>((Control) groupBox4, "Group sort order", (Setting<SortOrder>) Program.Preferences.Settings.GroupSortOrder, ref rowIndex2, ref tabIndex2, new Func<SortOrder, string>(Helpers.SortOrderToString), (IEnumerable<SortOrder>) new SortOrder[2]
      {
        SortOrder.ByName,
        SortOrder.None
      });
      FormTools.AddLabeledEnumDropDown<SortOrder>((Control) groupBox4, "Server sort order", Program.Preferences.Settings.ServerSortOrder, ref rowIndex2, ref tabIndex2, new Func<SortOrder, string>(Helpers.SortOrderToString));
      groupBox4.Text = "Sort Order";
      FormTools.LayoutGroupBox(groupBox4, 2, (Control) this._virtualGroupsGroup);
      tabPage.Controls.Add((Control) groupBox2, (Control) groupBox4, (Control) this._virtualGroupsGroup);
      return tabPage;
    }

    private TabPage CreateGeneralPage()
    {
      int rowIndex = 0;
      int tabIndex1 = 0;
      TabPage tabPage = this.NewTabPage("General");
      FormTools.AddCheckBox((Control) tabPage, "Hide main menu until ALT pressed", Program.Preferences.Settings.HideMainMenu, 0, ref rowIndex, ref tabIndex1);
      RdcCheckBox autoSaveCheckBox = FormTools.AddCheckBox((Control) tabPage, "Auto save interval:", Program.Preferences.Settings.AutoSaveFiles, 0, ref rowIndex, ref tabIndex1);
      autoSaveCheckBox.Size = new Size(120, 24);
      NumericTextBox numericTextBox1 = new NumericTextBox(0, 60, "Auto save interval must 0 to 60 minutes inclusive");
      numericTextBox1.Location = new Point(autoSaveCheckBox.Right + 1, autoSaveCheckBox.Top + 2);
      numericTextBox1.Size = new Size(20, 24);
      NumericTextBox numericTextBox2 = numericTextBox1;
      int num1 = tabIndex1;
      int tabIndex2 = num1 + 1;
      numericTextBox2.TabIndex = num1;
      numericTextBox1.TabStop = true;
      numericTextBox1.Enabled = false;
      NumericTextBox autoSaveInterval = numericTextBox1;
      autoSaveInterval.Setting = Program.Preferences.Settings.AutoSaveInterval;
      autoSaveCheckBox.CheckedChanged += (EventHandler) ((s, e) => autoSaveInterval.Enabled = autoSaveCheckBox.Checked);
      Label label1 = new Label();
      label1.Location = new Point(autoSaveInterval.Right + 3, autoSaveCheckBox.Top + 4);
      label1.Size = new Size(60, 24);
      label1.Text = "minute(s)";
      Label label2 = label1;
      RdcCheckBox rdcCheckBox = FormTools.AddCheckBox((Control) tabPage, "Prompt to reconnect connected servers on startup", Program.Preferences.Settings.ReconnectOnStartup, 0, ref rowIndex, ref tabIndex2);
      Button button1 = new Button();
      button1.Location = new Point(8, rdcCheckBox.Bottom + 8);
      Button button2 = button1;
      int num2 = tabIndex2;
      int num3 = num2 + 1;
      button2.TabIndex = num2;
      button1.Text = "Default group settings...";
      button1.Width = 140;
      Button button3 = button1;
      button3.Click += (EventHandler) ((s, e) => DefaultSettingsGroup.Instance.DoPropertiesDialog());
      tabPage.Controls.Add((Control) autoSaveCheckBox, (Control) autoSaveInterval, (Control) label2, (Control) button3);
      return tabPage;
    }

    private void EnablePanningCheckedChanged(object sender, EventArgs e) => this._panningAccelerationUpDown.Enabled = this._enablePanningCheckBox.Checked;

    private void ConnectionBarEnabledCheckedChanged(object sender, EventArgs e) => this._connectionBarAutoHiddenCheckBox.Enabled = this._connectionBarEnabledCheckBox.Checked;

    private void CustomSizeClick(object sender, EventArgs e)
    {
      Button button = sender as Button;
      using (CustomSizeDialog customSizeDialog = new CustomSizeDialog(SizeHelper.Parse(button.Text)))
      {
        if (customSizeDialog.ShowDialog() != DialogResult.OK)
          return;
        button.Text = customSizeDialog.WidthText + SizeHelper.Separator + customSizeDialog.HeightText;
        this._thumbnailPixelsRadio.Checked = true;
      }
    }

    private void ThumbnailPercentageRadioCheckedChanged(object sender, EventArgs e) => this._thumbnailPercentageTextBox.Enabled = (sender as RadioButton).Checked;

    private void BandwidthCombo_ControlChanged(object sender, EventArgs e) => this.BandwidthSettingsChanged();

    private void BandwidthSettingsChanged()
    {
      if (this._inHandler)
        return;
      this._inHandler = true;
      this.SetBandwidthCheckBoxes(this._bandwidthComboBox.SelectedValue.Flags);
      this._inHandler = false;
    }

    private void SetBandwidthCheckBoxes(int flags)
    {
      this._desktopBackgroundCheckBox.Checked = (flags & 1) == 0;
      this._fontSmoothingCheckBox.Checked = (flags & 128) != 0;
      this._desktopCompositionCheckBox.Checked = (flags & 256) != 0;
      this._windowDragCheckBox.Checked = (flags & 2) == 0;
      this._menuAnimationCheckBox.Checked = (flags & 4) == 0;
      this._themesCheckBox.Checked = (flags & 8) == 0;
    }

    private void PerfCheckBox_CheckedChanged(object sender, EventArgs e)
    {
      if (this._inHandler)
        return;
      this._inHandler = true;
      int flags = this.ComputeFlagsFromCheckBoxes();
      this._bandwidthComboBox.SelectedValue = ((IEnumerable<GlobalOptionsDialog.BandwidthItem>) this._bandwidthItems).Where<GlobalOptionsDialog.BandwidthItem>((Func<GlobalOptionsDialog.BandwidthItem, bool>) (i => i.Flags == flags)).FirstOrDefault<GlobalOptionsDialog.BandwidthItem>() ?? ((IEnumerable<GlobalOptionsDialog.BandwidthItem>) this._bandwidthItems).First<GlobalOptionsDialog.BandwidthItem>((Func<GlobalOptionsDialog.BandwidthItem, bool>) (i => i.Text.Equals("Custom")));
      this._inHandler = false;
    }

    private int ComputeFlagsFromCheckBoxes()
    {
      int num = 0;
      if (!this._desktopBackgroundCheckBox.Checked)
        num |= 1;
      if (this._fontSmoothingCheckBox.Checked)
        num |= 128;
      if (this._desktopCompositionCheckBox.Checked)
        num |= 256;
      if (!this._windowDragCheckBox.Checked)
        num |= 2;
      if (!this._menuAnimationCheckBox.Checked)
        num |= 4;
      if (!this._themesCheckBox.Checked)
        num |= 8;
      return num;
    }

    private class BandwidthItem
    {
      public string Text;
      public int Flags;

      public BandwidthItem(string text, int flags)
      {
        this.Text = text;
        this.Flags = flags;
      }
    }
  }
}
