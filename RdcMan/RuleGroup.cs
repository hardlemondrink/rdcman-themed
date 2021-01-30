// Decompiled with JetBrains decompiler
// Type: RdcMan.RuleGroup
// Assembly: RDCMan, Version=2.7.1406.0, Culture=neutral, PublicKeyToken=null
// MVID: 984CDC7A-116B-406E-B519-0DEE541B9019
// Assembly location: C:\Program Files (x86)\Microsoft\Remote Desktop Connection Manager\RDCMan.exe

using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml;

namespace RdcMan
{
  internal class RuleGroup
  {
    public const string XmlNodeName = "ruleGroup";
    private const string GroupingOperatorXmlNodeName = "operator";
    private static Dictionary<string, Helpers.ReadXmlDelegate> NodeActions = new Dictionary<string, Helpers.ReadXmlDelegate>();

    public RuleGroupOperator Operator { get; private set; }

    public List<Rule> Rules { get; private set; }

    public RuleGroup(RuleGroupOperator op, IEnumerable<Rule> rules) => this.Set(op, rules);

    protected RuleGroup()
    {
    }

    public static RuleGroup Create(
      XmlNode xmlNode,
      RdcTreeNode node,
      ICollection<string> errors)
    {
      RuleGroup ruleGroup = new RuleGroup();
      ruleGroup.ReadXml(xmlNode, node, errors);
      return ruleGroup;
    }

    public void Set(RuleGroupOperator op, IEnumerable<Rule> rules)
    {
      this.Operator = op;
      this.Rules = rules.ToList<Rule>();
    }

    public bool Evaluate(Server server)
    {
      bool flag1 = false;
      bool flag2 = true;
      foreach (Rule rule in this.Rules)
      {
        if (rule.Evaluate(server))
          flag1 = true;
        else
          flag2 = false;
      }
      return this.Operator != RuleGroupOperator.Any ? flag2 : flag1;
    }

    public void ReadXml(XmlNode xmlNode, RdcTreeNode node, ICollection<string> errors)
    {
      this.Operator = xmlNode.Attributes["operator"].Value.ParseEnum<RuleGroupOperator>();
      this.Rules = new List<Rule>();
      foreach (XmlNode childNode in xmlNode.ChildNodes)
        this.Rules.Add(Rule.Create(childNode, node, errors));
    }

    public void WriteXml(XmlTextWriter tw)
    {
      tw.WriteStartElement("ruleGroup");
      tw.WriteAttributeString("operator", this.Operator.ToString());
      this.Rules.ForEach((Action<Rule>) (r => r.WriteXml(tw)));
      tw.WriteEndElement();
    }
  }
}
