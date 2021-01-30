// Decompiled with JetBrains decompiler
// Type: RdcMan.CredentialsStore
// Assembly: RDCMan, Version=2.7.1406.0, Culture=neutral, PublicKeyToken=null
// MVID: 984CDC7A-116B-406E-B519-0DEE541B9019
// Assembly location: C:\Program Files (x86)\Microsoft\Remote Desktop Connection Manager\RDCMan.exe

using System;
using System.Collections.Generic;
using System.Xml;

namespace RdcMan
{
  public class CredentialsStore
  {
    public const string XmlNodeName = "credentialsProfiles";
    public const string ProfileXmlNodeName = "credentialsProfile";
    private readonly Dictionary<string, CredentialsProfile> _profiles;

    public CredentialsStore() => this._profiles = new Dictionary<string, CredentialsProfile>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);

    public void ReadXml(
      XmlNode xmlNode,
      ProfileScope scope,
      RdcTreeNode node,
      ICollection<string> errors)
    {
      foreach (XmlNode childNode in xmlNode.ChildNodes)
      {
        LogonCredentials logonCredentials1 = new LogonCredentials("", "credentialsProfile");
        logonCredentials1.ReadXml(childNode, node, errors);
        ILogonCredentials logonCredentials2 = (ILogonCredentials) logonCredentials1;
        CredentialsProfile credentialsProfile = new CredentialsProfile(logonCredentials2.ProfileName, scope, logonCredentials2.UserName, logonCredentials2.Password, logonCredentials2.Domain);
        this[logonCredentials2.ProfileName] = credentialsProfile;
        Encryption.DeferDecryption((IDeferDecryption) credentialsProfile, node, credentialsProfile.QualifiedName);
      }
    }

    public void WriteXml(XmlTextWriter tw, RdcTreeNode node)
    {
      tw.WriteStartElement("credentialsProfiles");
      foreach (ILogonCredentials profile in this.Profiles)
      {
        LogonCredentials logonCredentials1 = new LogonCredentials("", "credentialsProfile");
        logonCredentials1.InheritSettingsType.Mode = InheritanceMode.None;
        logonCredentials1.ProfileName.Value = profile.ProfileName;
        LogonCredentials logonCredentials2 = logonCredentials1;
        logonCredentials2.UserName.Value = profile.UserName;
        logonCredentials2.Password.Copy((ISetting) profile.Password);
        logonCredentials2.Domain.Value = profile.Domain;
        logonCredentials2.WriteXml(tw, node);
      }
      tw.WriteEndElement();
    }

    public int ChangeId { get; private set; }

    public CredentialsProfile this[string name]
    {
      get => this._profiles[name];
      set
      {
        this._profiles[name] = value;
        ++this.ChangeId;
      }
    }

    public IEnumerable<CredentialsProfile> Profiles => (IEnumerable<CredentialsProfile>) this._profiles.Values;

    public bool TryGetValue(string name, out CredentialsProfile profile) => this._profiles.TryGetValue(name, out profile);

    public void Remove(string name)
    {
      this._profiles.Remove(name);
      ++this.ChangeId;
    }

    public bool Contains(string name) => this._profiles.ContainsKey(name);
  }
}
