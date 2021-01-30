// Decompiled with JetBrains decompiler
// Type: RdcMan.PasswordSetting
// Assembly: RDCMan, Version=2.7.1406.0, Culture=neutral, PublicKeyToken=null
// MVID: 984CDC7A-116B-406E-B519-0DEE541B9019
// Assembly location: C:\Program Files (x86)\Microsoft\Remote Desktop Connection Manager\RDCMan.exe

using System.Xml;

namespace RdcMan
{
  public class PasswordSetting : BaseSetting<string>, IDeferDecryption
  {
    private const string StoreAsClearTextAttribute = "storeAsClearText";

    public PasswordSetting(object o)
      : base(o)
    {
    }

    public void SetPlainText(string value)
    {
      this.Value = value;
      this.IsDecrypted = true;
    }

    public override void ReadXml(XmlNode xmlNode, RdcTreeNode node)
    {
      XmlNode firstChild = xmlNode.FirstChild;
      if (firstChild == null)
        this.Value = string.Empty;
      else
        this.Value = firstChild.InnerText;
      try
      {
        XmlNode attribute = (XmlNode) xmlNode.Attributes["storeAsClearText"];
        if (attribute != null && bool.Parse(attribute.InnerText))
        {
          node.Password.IsDecrypted = true;
        }
        else
        {
          if (!(xmlNode.ParentNode.Name != "credentialsProfile"))
            return;
          Encryption.DeferDecryption((IDeferDecryption) this, node, xmlNode.GetFullPath());
        }
      }
      catch
      {
      }
    }

    public override void WriteXml(XmlTextWriter tw, RdcTreeNode node)
    {
      string text = this.IsDecrypted ? Encryption.EncryptString(this.Value, node.EncryptionSettings) : this.Value;
      tw.WriteString(text);
    }

    public override void Copy(ISetting source)
    {
      base.Copy(source);
      this.IsDecrypted = ((PasswordSetting) source).IsDecrypted;
    }

    public bool IsDecrypted { get; set; }

    public void Decrypt(EncryptionSettings settings)
    {
      this.Value = Encryption.DecryptString(this.Value, settings);
      this.IsDecrypted = true;
    }
  }
}
