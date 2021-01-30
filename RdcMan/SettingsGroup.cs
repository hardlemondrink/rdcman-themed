// Decompiled with JetBrains decompiler
// Type: RdcMan.SettingsGroup
// Assembly: RDCMan, Version=2.7.1406.0, Culture=neutral, PublicKeyToken=null
// MVID: 984CDC7A-116B-406E-B519-0DEE541B9019
// Assembly location: C:\Program Files (x86)\Microsoft\Remote Desktop Connection Manager\RDCMan.exe

using System;
using System.Collections;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Xml;

namespace RdcMan
{
  public abstract class SettingsGroup
  {
    internal SettingsGroup(string name, string xmlNodeName)
    {
      this.XmlNodeName = xmlNodeName;
      this.Name = name;
      this.InheritSettingsType = new InheritSettingsType();
      foreach (SettingProperty settingProperty in this.SettingProperties.Values)
      {
        ISetting instance = (ISetting) Activator.CreateInstance(settingProperty.Property.PropertyType, settingProperty.Attribute.DefaultValue);
        settingProperty.Property.SetValue((object) this, (object) instance, (object[]) null);
      }
    }

    public string Name { get; private set; }

    public string XmlNodeName { get; private set; }

    public InheritSettingsType InheritSettingsType { get; private set; }

    protected abstract Dictionary<string, SettingProperty> SettingProperties { get; }

    public abstract TabPage CreateTabPage(TabbedSettingsDialog dialog);

    public override string ToString() => this.Name;

    internal void ReadXml(XmlNode xmlNode, RdcTreeNode node, ICollection<string> errors)
    {
      InheritanceMode result = this.InheritSettingsType.Mode;
      if (result != InheritanceMode.Disabled)
      {
        try
        {
          string str = xmlNode.Attributes["inherit"].Value;
          if (!Enum.TryParse<InheritanceMode>(str, out result))
          {
            errors.Add("Unexpected inheritance mode '{0}' in {1}".InvariantFormat((object) str, (object) xmlNode.GetFullPath()));
            result = InheritanceMode.None;
          }
        }
        catch
        {
          errors.Add("No inheritance mode specified in {0}".InvariantFormat((object) xmlNode.GetFullPath()));
          result = InheritanceMode.None;
        }
        this.InheritSettingsType.Mode = result;
      }
      switch (result)
      {
        case InheritanceMode.FromParent:
          bool anyInherited = false;
          this.InheritSettings(node, ref anyInherited);
          break;
        case InheritanceMode.None:
        case InheritanceMode.Disabled:
          IEnumerator enumerator = xmlNode.ChildNodes.GetEnumerator();
          try
          {
            while (enumerator.MoveNext())
            {
              XmlNode current = (XmlNode) enumerator.Current;
              try
              {
                ISetting setting = (ISetting) this.SettingProperties[current.Name].Property.GetValue((object) this, (object[]) null);
                try
                {
                  setting.ReadXml(current, node);
                }
                catch
                {
                  errors.Add("Error processing Xml node {0}".InvariantFormat((object) current.GetFullPath()));
                }
              }
              catch
              {
                errors.Add("Unexpected Xml node {0}".InvariantFormat((object) current.GetFullPath()));
              }
            }
            break;
          }
          finally
          {
            if (enumerator is IDisposable disposable)
              disposable.Dispose();
          }
        default:
          errors.Add("Unexpected inheritance mode '{0}' in {1}".InvariantFormat((object) result.ToString(), (object) xmlNode.GetFullPath()));
          break;
      }
    }

    internal void WriteXml(XmlTextWriter tw, RdcTreeNode node)
    {
      if (this.InheritSettingsType.Mode == InheritanceMode.FromParent)
        return;
      tw.WriteStartElement(this.XmlNodeName);
      if (this.InheritSettingsType.Mode != InheritanceMode.Disabled)
        this.InheritSettingsType.WriteXml(tw);
      this.WriteSettings(tw, node);
      tw.WriteEndElement();
    }

    protected virtual void WriteSettings(XmlTextWriter tw, RdcTreeNode node) => this.WriteSettings(tw, node, (HashSet<ISetting>) null);

    protected virtual void WriteSettings(
      XmlTextWriter tw,
      RdcTreeNode node,
      HashSet<ISetting> exclusionSet)
    {
      foreach (SettingProperty settingProperty in this.SettingProperties.Values)
      {
        ISetting setting = (ISetting) settingProperty.Property.GetValue((object) this, (object[]) null);
        if ((exclusionSet == null || !exclusionSet.Contains(setting)) && !settingProperty.Attribute.IsObsolete)
        {
          tw.WriteStartElement(settingProperty.Attribute.XmlName);
          setting.WriteXml(tw, node);
          tw.WriteEndElement();
        }
      }
    }

    internal void InheritSettings(RdcTreeNode node, ref bool anyInherited)
    {
      GroupBase inheritedSettingsNode = this.InheritSettingsType.GetInheritedSettingsNode(node);
      if (inheritedSettingsNode == null)
        return;
      inheritedSettingsNode.InheritSettings();
      this.Copy((RdcTreeNode) inheritedSettingsNode);
      anyInherited = true;
    }

    protected virtual void Copy(RdcTreeNode node) => throw new NotImplementedException();

    internal void Copy(SettingsGroup source)
    {
      foreach (SettingProperty settingProperty in this.SettingProperties.Values)
        ((ISetting) settingProperty.Property.GetValue((object) this, (object[]) null)).Copy((ISetting) settingProperty.Property.GetValue((object) source, (object[]) null));
    }
  }
}
