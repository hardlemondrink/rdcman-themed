// Decompiled with JetBrains decompiler
// Type: RdcMan.RdcSettingsProvider
// Assembly: RDCMan, Version=2.7.1406.0, Culture=neutral, PublicKeyToken=null
// MVID: 984CDC7A-116B-406E-B519-0DEE541B9019
// Assembly location: C:\Program Files (x86)\Microsoft\Remote Desktop Connection Manager\RDCMan.exe

using System;
using System.Collections.Specialized;
using System.Configuration;
using System.IO;
using System.Windows.Forms;
using System.Xml;

namespace RdcMan
{
  public class RdcSettingsProvider : SettingsProvider
  {
    private const string SettingsRoot = "Settings";

    public override void Initialize(string name, NameValueCollection values) => base.Initialize(this.ApplicationName, values);

    public override string ApplicationName
    {
      get => "RDCMan";
      set
      {
      }
    }

    private string SettingsDirectory => new FileInfo(Application.LocalUserAppDataPath).DirectoryName;

    private string SettingsFilename => Path.Combine(this.SettingsDirectory, this.ApplicationName + ".settings");

    public override SettingsPropertyValueCollection GetPropertyValues(
      SettingsContext context,
      SettingsPropertyCollection properties)
    {
      SettingsPropertyValueCollection propertyValueCollection = new SettingsPropertyValueCollection();
      XmlDocument xmlDocument = new XmlDocument();
      XmlNode xmlNode1 = (XmlNode) null;
      try
      {
        if (!Program.ResetPreferences)
        {
          xmlDocument.Load(this.SettingsFilename);
          xmlNode1 = xmlDocument.SelectSingleNode("Settings");
        }
      }
      catch
      {
      }
      if (xmlNode1 == null)
        xmlNode1 = xmlDocument.CreateNode(XmlNodeType.Element, "root", "");
      foreach (SettingsProperty property1 in properties)
      {
        SettingsPropertyValue property2 = new SettingsPropertyValue(property1);
        XmlNode xmlNode2 = xmlNode1.SelectSingleNode(property1.Name);
        property2.SerializedValue = xmlNode2 == null ? property1.DefaultValue : (!(property1.PropertyType == typeof (XmlDocument)) ? (object) xmlNode2.InnerText : (object) xmlNode2.InnerXml);
        propertyValueCollection.Add(property2);
      }
      return propertyValueCollection;
    }

    public override void SetPropertyValues(
      SettingsContext context,
      SettingsPropertyValueCollection values)
    {
      throw new InvalidOperationException();
    }
  }
}
