// Decompiled with JetBrains decompiler
// Type: RdcMan.ProfileSetting
// Assembly: RDCMan, Version=2.7.1406.0, Culture=neutral, PublicKeyToken=null
// MVID: 984CDC7A-116B-406E-B519-0DEE541B9019
// Assembly location: C:\Program Files (x86)\Microsoft\Remote Desktop Connection Manager\RDCMan.exe

using System.Xml;

namespace RdcMan
{
  public class ProfileSetting : StringSetting
  {
    public const string ProfileScopeAttribute = "scope";

    public ProfileSetting(object o)
      : base(o)
      => this.Reset();

    public override void ReadXml(XmlNode xmlNode, RdcTreeNode node)
    {
      base.ReadXml(xmlNode, node);
      try
      {
        this.Scope = xmlNode.Attributes.GetNamedItem("scope").InnerText.ParseEnum<ProfileScope>();
      }
      catch
      {
        this.Scope = ProfileScope.Local;
      }
    }

    public override void WriteXml(XmlTextWriter tw, RdcTreeNode node)
    {
      tw.WriteAttributeString("scope", this.Scope.ToString());
      tw.WriteString(this.Value);
    }

    public override void Copy(ISetting source)
    {
      base.Copy(source);
      this.Scope = ((ProfileSetting) source).Scope;
    }

    public override string ToString() => "{0} ({1})".InvariantFormat((object) this.Value, (object) this.Scope);

    public void UpdateValue(string newValue, ProfileScope newScope)
    {
      this.Value = newValue;
      this.Scope = newScope;
    }

    public void Reset()
    {
      this.Scope = ProfileScope.Local;
      this.Value = "Custom";
    }

    public ProfileScope Scope { get; private set; }
  }
}
