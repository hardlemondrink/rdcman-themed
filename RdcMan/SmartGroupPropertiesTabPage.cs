// Decompiled with JetBrains decompiler
// Type: RdcMan.SmartGroupPropertiesTabPage
// Assembly: RDCMan, Version=2.7.1406.0, Culture=neutral, PublicKeyToken=null
// MVID: 984CDC7A-116B-406E-B519-0DEE541B9019
// Assembly location: C:\Program Files (x86)\Microsoft\Remote Desktop Connection Manager\RDCMan.exe

using System;
using System.Drawing;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using System.Windows.Forms.Layout;

namespace RdcMan
{
  internal class SmartGroupPropertiesTabPage : GroupBasePropertiesTabPage<SmartGroupSettings>
  {
    private const int RuleWidth = 459;
    private const int RuleHeight = 21;
    private readonly RuleGroup _ruleGroup;
    private readonly Panel _rulePanel;
    private readonly ValueComboBox<RuleGroupOperator> _anyAllCombo;
    private int _nextRuleTabIndex;

    public SmartGroupPropertiesTabPage(TabbedSettingsDialog dialog, SmartGroupSettings settings)
      : base(dialog, settings, settings.Name)
    {
      this._ruleGroup = ((dialog as SmartGroupPropertiesDialog).AssociatedNode as SmartGroup).RuleGroup;
      int rowIndex1 = 0;
      int tabIndex = 0;
      this.AddGroupName(ref rowIndex1, ref tabIndex);
      this.AddParentCombo(ref rowIndex1, ref tabIndex);
      int num1 = rowIndex1 + 1;
      Label label1 = new Label();
      Label label2 = label1;
      int rowIndex2 = num1;
      int num2 = rowIndex2 + 1;
      Point point1 = FormTools.NewLocation(0, rowIndex2);
      label2.Location = point1;
      label1.Text = "Servers which match";
      label1.TextAlign = ContentAlignment.MiddleLeft;
      label1.Size = new Size(110, 20);
      Label label3 = label1;
      ValueComboBox<RuleGroupOperator> valueComboBox1 = new ValueComboBox<RuleGroupOperator>(Helpers.EnumValues<RuleGroupOperator>(), (Func<RuleGroupOperator, string>) (v => v.ToString()));
      valueComboBox1.Location = new Point(label3.Right, label3.Top);
      valueComboBox1.Size = new Size(50, 20);
      ValueComboBox<RuleGroupOperator> valueComboBox2 = valueComboBox1;
      int num3 = tabIndex;
      int num4 = num3 + 1;
      valueComboBox2.TabIndex = num3;
      valueComboBox1.SelectedValue = RuleGroupOperator.All;
      this._anyAllCombo = valueComboBox1;
      Label label4 = new Label();
      label4.Location = new Point(this._anyAllCombo.Right + 5, label3.Top);
      label4.Text = "of the following rules";
      label4.TextAlign = ContentAlignment.MiddleLeft;
      label4.Size = new Size(140, 20);
      Label label5 = label4;
      this.Controls.Add((Control) label3, (Control) this._anyAllCombo, (Control) label5);
      GroupBox groupBox1 = new GroupBox();
      GroupBox groupBox2 = groupBox1;
      int rowIndex3 = num2;
      int num5 = rowIndex3 + 1;
      Point point2 = FormTools.NewLocation(0, rowIndex3);
      groupBox2.Location = point2;
      GroupBox groupBox3 = groupBox1;
      Panel panel = new Panel();
      panel.Location = FormTools.NewLocation(0, 0);
      panel.AutoScroll = true;
      this._rulePanel = panel;
      int height = 302 - groupBox3.Top - 40;
      this._rulePanel.Size = new Size(480, height);
      this._rulePanel.VerticalScroll.LargeChange = height;
      this._rulePanel.VerticalScroll.SmallChange = height / 20;
      groupBox3.Size = new Size(496, height + this._rulePanel.Top * 2);
      groupBox3.Controls.Add((Control) this._rulePanel);
      this._nextRuleTabIndex = num4;
      this.Controls.Add((Control) groupBox3);
    }

    protected override void UpdateControls()
    {
      base.UpdateControls();
      this._anyAllCombo.SelectedValue = this._ruleGroup.Operator;
      if (this._ruleGroup.Rules.Count > 0)
        this._ruleGroup.Rules.ForEach(new Action<Rule>(this.AddRuleControl));
      else
        this.AddRuleControl((Rule) null);
      this.LayoutRuleControls();
    }

    protected override bool IsValid()
    {
      bool flag = true;
      foreach (SmartGroupPropertiesTabPage.SmartRuleControl control in (ArrangedElementCollection) this._rulePanel.Controls)
      {
        string text = (string) null;
        try
        {
          if (control.Value is string pattern)
          {
            if (string.IsNullOrEmpty(pattern))
              text = "Please enter a pattern";
            else
              Regex.Match(string.Empty, pattern);
          }
        }
        catch (Exception ex)
        {
          text = ex.Message;
        }
        flag &= !this.Dialog.SetError(control.ValueControl, text);
      }
      return flag && base.IsValid();
    }

    protected override void UpdateSettings()
    {
      base.UpdateSettings();
      this._ruleGroup.Set(this._anyAllCombo.SelectedValue, this._rulePanel.Controls.Cast<SmartGroupPropertiesTabPage.SmartRuleControl>().ToList<SmartGroupPropertiesTabPage.SmartRuleControl>().OrderBy<SmartGroupPropertiesTabPage.SmartRuleControl, int>((Func<SmartGroupPropertiesTabPage.SmartRuleControl, int>) (r => r.Index)).Select<SmartGroupPropertiesTabPage.SmartRuleControl, Rule>((Func<SmartGroupPropertiesTabPage.SmartRuleControl, Rule>) (c => new Rule(new RuleProperty(c.Property), c.Operator, c.Value))));
    }

    protected override void ParentGroupChangedHandler(object sender, EventArgs e)
    {
    }

    private void InsertRuleControl(
      SmartGroupPropertiesTabPage.SmartRuleControl afterRule)
    {
      int index = afterRule.Index + 1;
      foreach (SmartGroupPropertiesTabPage.SmartRuleControl control in (ArrangedElementCollection) this._rulePanel.Controls)
      {
        if (control.Index >= index)
          ++control.Index;
      }
      this._rulePanel.Controls.Add((Control) this.CreateRuleControl((Rule) null, index));
      this.LayoutRuleControls();
    }

    private void AddRuleControl(Rule rule) => this._rulePanel.Controls.Add((Control) this.CreateRuleControl(rule, this._rulePanel.Controls.Count));

    private SmartGroupPropertiesTabPage.SmartRuleControl CreateRuleControl(
      Rule rule,
      int index)
    {
      SmartGroupPropertiesTabPage.SmartRuleControl newRule = new SmartGroupPropertiesTabPage.SmartRuleControl(rule, ref this._nextRuleTabIndex)
      {
        Index = index
      };
      newRule.AddButton.Click += (EventHandler) ((s, o) => this.InsertRuleControl(newRule));
      newRule.DeleteButton.Click += (EventHandler) ((s, o) => this.DeleteRuleControl(newRule));
      return newRule;
    }

    private void DeleteRuleControl(SmartGroupPropertiesTabPage.SmartRuleControl rule)
    {
      int index = rule.Index;
      this._rulePanel.Controls.Remove((Control) rule);
      foreach (SmartGroupPropertiesTabPage.SmartRuleControl control in (ArrangedElementCollection) this._rulePanel.Controls)
      {
        if (control.Index > index)
          --control.Index;
      }
      this.LayoutRuleControls();
    }

    private void LayoutRuleControls()
    {
      int count = this._rulePanel.Controls.Count;
      int val1 = 0;
      this._rulePanel.SuspendLayout();
      int val2 = this._rulePanel.VerticalScroll.Value;
      foreach (SmartGroupPropertiesTabPage.SmartRuleControl control in (ArrangedElementCollection) this._rulePanel.Controls)
      {
        control.DeleteButton.Enabled = count > 1;
        control.Location = new Point(0, control.Index * 25 - val2);
        val1 = Math.Max(val1, control.Top);
      }
      this._rulePanel.VerticalScroll.Maximum = val1;
      this._rulePanel.VerticalScroll.Value = Math.Min(val1, val2);
      this._rulePanel.ResumeLayout();
    }

    private class SmartRuleControl : Control
    {
      private readonly ValueComboBox<ServerProperty> _propertyCombo;
      private readonly ValueComboBox<RuleOperator> _operatorCombo;
      private readonly TextBox _valueTextBox;

      public SmartRuleControl(Rule rule, ref int tabIndex)
      {
        ValueComboBox<ServerProperty> valueComboBox1 = new ValueComboBox<ServerProperty>(Helpers.EnumValues<ServerProperty>(), (Func<ServerProperty, string>) (v => v.ToString()));
        valueComboBox1.Location = new Point(0, 0);
        valueComboBox1.Width = 100;
        valueComboBox1.TabIndex = tabIndex++;
        valueComboBox1.SelectedValue = ServerProperty.DisplayName;
        this._propertyCombo = valueComboBox1;
        ValueComboBox<RuleOperator> valueComboBox2 = new ValueComboBox<RuleOperator>(Helpers.EnumValues<RuleOperator>(), (Func<RuleOperator, string>) (v => v.ToString()));
        valueComboBox2.Location = new Point(this._propertyCombo.Right + 4, 0);
        valueComboBox2.Width = 100;
        valueComboBox2.TabIndex = tabIndex++;
        valueComboBox2.SelectedValue = RuleOperator.Matches;
        this._operatorCombo = valueComboBox2;
        TextBox textBox = new TextBox();
        textBox.Enabled = true;
        textBox.Location = new Point(this._operatorCombo.Right + 4, 0);
        textBox.Width = 459 - (this._operatorCombo.Right + 4) - 48;
        textBox.TabIndex = tabIndex++;
        this._valueTextBox = textBox;
        Button button1 = new Button();
        button1.Enabled = true;
        button1.Location = new Point(this._valueTextBox.Right + 4, 0);
        button1.Size = new Size(20, 20);
        button1.TabIndex = tabIndex++;
        button1.Text = "-";
        this.DeleteButton = button1;
        Button button2 = new Button();
        button2.Enabled = true;
        button2.Location = new Point(this.DeleteButton.Right + 4, 0);
        button2.Size = new Size(20, 20);
        button2.TabIndex = tabIndex++;
        button2.Text = "+";
        this.AddButton = button2;
        this.Controls.Add((Control) this._propertyCombo, (Control) this._operatorCombo, (Control) this._valueTextBox, (Control) this.DeleteButton, (Control) this.AddButton);
        this.Size = new Size(459, 21);
        this.TabStop = false;
        if (rule == null)
          return;
        this._propertyCombo.SelectedValue = rule.Property.ServerProperty;
        this._operatorCombo.SelectedValue = rule.Operator;
        this._valueTextBox.Text = rule.Value.ToString();
      }

      public ServerProperty Property => this._propertyCombo.SelectedValue;

      public RuleOperator Operator => this._operatorCombo.SelectedValue;

      public Control ValueControl => (Control) this._valueTextBox;

      public object Value => (object) this._valueTextBox.Text;

      public Button AddButton { get; private set; }

      public Button DeleteButton { get; private set; }

      public int Index { get; set; }
    }
  }
}
