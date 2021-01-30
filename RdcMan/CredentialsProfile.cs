// Decompiled with JetBrains decompiler
// Type: RdcMan.CredentialsProfile
// Assembly: RDCMan, Version=2.7.1406.0, Culture=neutral, PublicKeyToken=null
// MVID: 984CDC7A-116B-406E-B519-0DEE541B9019
// Assembly location: C:\Program Files (x86)\Microsoft\Remote Desktop Connection Manager\RDCMan.exe

namespace RdcMan
{
  public class CredentialsProfile : ILogonCredentials, IDeferDecryption
  {
    public const string CustomProfileName = "Custom";
    private readonly string _profileName;
    private readonly ProfileScope _profileScope;
    private string _userName;
    private PasswordSetting _password;
    private string _domain;

    public CredentialsProfile(
      string profileName,
      ProfileScope profileScope,
      string userName,
      string password,
      string domain)
    {
      this._profileName = profileName;
      this._profileScope = profileScope;
      this._userName = userName;
      this._password = new PasswordSetting((object) password)
      {
        IsDecrypted = true
      };
      this._domain = domain;
    }

    public CredentialsProfile(
      string profileName,
      ProfileScope profileScope,
      string userName,
      PasswordSetting password,
      string domain)
    {
      this._profileName = profileName;
      this._profileScope = profileScope;
      this._userName = userName;
      this._password = password;
      this._domain = domain;
    }

    public string ProfileName => this._profileName;

    public ProfileScope ProfileScope => this._profileScope;

    public string UserName => this._userName;

    public PasswordSetting Password => this._password;

    public string Domain => this._domain;

    public bool IsDecrypted
    {
      get => this._password.IsDecrypted;
      set => this._password.IsDecrypted = value;
    }

    public void Decrypt(EncryptionSettings settings) => this._password.Decrypt(settings);

    public override string ToString() => this.ProfileName;

    public string QualifiedName => LogonCredentials.ConstructQualifiedName((ILogonCredentials) this);
  }
}
