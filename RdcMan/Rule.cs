// Decompiled with JetBrains decompiler
// Type: RdcMan.Rule
// Assembly: RDCMan, Version=2.7.1406.0, Culture=neutral, PublicKeyToken=null
// MVID: 984CDC7A-116B-406E-B519-0DEE541B9019
// Assembly location: C:\Program Files (x86)\Microsoft\Remote Desktop Connection Manager\RDCMan.exe

using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Xml;

namespace RdcMan
{
  public class Rule
  {
    public const string XmlNodeName = "rule";
    private const string PropertyXmlNodeName = "property";
    private const string OperatorXmlNodeName = "operator";
    private const string ValueXmlNodeName = "value";

    public RuleProperty Property { get; private set; }

    public RuleOperator Operator { get; private set; }

    public object Value { get; private set; }

    public Rule(RuleProperty property, RuleOperator operation, object value)
    {
      this.Property = property;
      this.Operator = operation;
      this.Value = value;
    }

    protected Rule()
    {
    }

    public static Rule Create(XmlNode xmlNode, RdcTreeNode node, ICollection<string> errors)
    {
      Rule rule = new Rule();
      rule.ReadXml(xmlNode, node, errors);
      return rule;
    }

    public bool Evaluate(Server server) => Regex.IsMatch((string) (this.Property.GetValue(server, out bool _) ?? (object) string.Empty), (string) this.Value, RegexOptions.IgnoreCase) ^ this.Operator == RuleOperator.DoesNotMatch;

    public void ReadXml(XmlNode xmlNode, RdcTreeNode node, ICollection<string> errors)
    {
      foreach (XmlNode childNode in xmlNode.ChildNodes)
      {
        switch (childNode.Name)
        {
          case "property":
            this.Property = new RuleProperty(childNode.InnerText.ParseEnum<ServerProperty>());
            continue;
          case "operator":
            this.Operator = childNode.InnerText.ParseEnum<RuleOperator>();
            continue;
          case "value":
            this.Value = (object) childNode.InnerText;
            continue;
          default:
            throw new NotImplementedException();
        }
      }
    }

    public void WriteXml(XmlTextWriter tw)
    {
      tw.WriteStartElement("rule");
      tw.WriteElementString("property", this.Property.ServerProperty.ToString());
      tw.WriteElementString("operator", this.Operator.ToString());
      tw.WriteElementString("value", this.Value.ToString());
      tw.WriteEndElement();
    }

    public override string ToString() => "{0} {1} {2}".InvariantFormat((object) this.Property.ServerProperty, (object) this.Operator, this.Value);
  }
}
