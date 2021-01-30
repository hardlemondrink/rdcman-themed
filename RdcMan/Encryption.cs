// Decompiled with JetBrains decompiler
// Type: RdcMan.Encryption
// Assembly: RDCMan, Version=2.7.1406.0, Culture=neutral, PublicKeyToken=null
// MVID: 984CDC7A-116B-406E-B519-0DEE541B9019
// Assembly location: C:\Program Files (x86)\Microsoft\Remote Desktop Connection Manager\RDCMan.exe

using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Windows.Forms;
using Win32;

namespace RdcMan
{
  public static class Encryption
  {
    private static readonly List<DeferDecryptionItem> PendingDecryption = new List<DeferDecryptionItem>();

    public static void DeferDecryption(IDeferDecryption o, RdcTreeNode node, string errorDetail) => Encryption.PendingDecryption.Add(new DeferDecryptionItem()
    {
      Object = o,
      Node = node,
      ErrorDetail = errorDetail
    });

    public static void DecryptPasswords()
    {
      StringBuilder builder = new StringBuilder("There were problems decrypting some credentials.").AppendLine().AppendLine("Click OK to copy the details to the clipboard.");
      bool credentialsProfileFail = false;
      Encryption.PendingDecryption.Where<DeferDecryptionItem>((Func<DeferDecryptionItem, bool>) (d => d.Object is CredentialsProfile)).ForEach<DeferDecryptionItem>((Action<DeferDecryptionItem>) (item => Encryption.DecryptPassword(item, builder, "Credentials profiles:", ref credentialsProfileFail)));
      bool passwordFail = false;
      Encryption.PendingDecryption.Where<DeferDecryptionItem>((Func<DeferDecryptionItem, bool>) (d => d.Object is PasswordSetting)).ForEach<DeferDecryptionItem>((Action<DeferDecryptionItem>) (item => Encryption.DecryptPassword(item, builder, "Custom passwords:", ref passwordFail)));
      Encryption.PendingDecryption.Clear();
      if (!credentialsProfileFail && !passwordFail || FormTools.ExclamationDialog(builder.ToString(), MessageBoxButtons.OKCancel) != DialogResult.OK)
        return;
      Clipboard.SetText(builder.ToString());
    }

    private static bool DecryptPassword(
      DeferDecryptionItem item,
      StringBuilder builder,
      string header,
      ref bool anyFailed)
    {
      IDeferDecryption deferDecryption = item.Object;
      RdcTreeNode node = item.Node;
      string errorDetail = item.ErrorDetail;
      bool anyInherited = false;
      node.EncryptionSettings.InheritSettings(node, ref anyInherited);
      try
      {
        deferDecryption.Decrypt(node.EncryptionSettings);
      }
      catch (Exception ex)
      {
        if (!anyFailed)
        {
          builder.AppendLine().AppendLine(header);
          anyFailed = true;
        }
        if (node is DefaultSettingsGroup)
          builder.Append("Default settings group");
        else
          builder.Append(node.FullPath);
        builder.AppendFormat(": {0}", (object) errorDetail);
        if (!string.IsNullOrEmpty(ex.Message))
          builder.AppendFormat(" [{0}]", (object) ex.Message);
        builder.AppendLine();
      }
      return anyFailed;
    }

    public static string SimpleName(this X509Certificate2 cert)
    {
      string str = cert.FriendlyName;
      if (string.IsNullOrEmpty(str))
        str = cert.GetNameInfo(X509NameType.SimpleName, false);
      return str + ", " + cert.GetNameInfo(X509NameType.SimpleName, true);
    }

    public static string EncryptionMethodToString(EncryptionMethod method)
    {
      switch (method)
      {
        case EncryptionMethod.LogonCredentials:
          return "Logged on user's credentials";
        case EncryptionMethod.Certificate:
          return "Certificate";
        default:
          throw new Exception("Unexpected EncryptionMethod");
      }
    }

    public static X509Certificate2 SelectCertificate()
    {
      X509Store x509Store = new X509Store();
      X509Certificate2Collection privateCollection = new X509Certificate2Collection();
      try
      {
        x509Store.Open(OpenFlags.OpenExistingOnly);
        X509Certificate2Collection foundCollection = x509Store.Certificates.Find(X509FindType.FindByTimeValid, (object) DateTime.Now, false);
        LongRunningActionForm.PerformOperation("Checking valid certificates", true, (Action) (() =>
        {
          foreach (X509Certificate2 x509Certificate2 in foundCollection)
          {
            try
            {
              if (Encryption.DecryptStringUsingCertificate(x509Certificate2, Encryption.EncryptStringUsingCertificate(x509Certificate2, "test")) == "test")
                privateCollection.Add(x509Certificate2);
            }
            catch
            {
            }
            LongRunningActionForm.Instance.UpdateStatus(x509Certificate2.SimpleName());
          }
        }));
      }
      finally
      {
        x509Store.Close();
      }
      X509Certificate2Collection certificate2Collection = X509Certificate2UI.SelectFromCollection(privateCollection, "Select Certificate", "Select a certificate for secure password storage", X509SelectionFlag.SingleSelection, Program.TheForm.Handle);
      return certificate2Collection.Count != 1 ? (X509Certificate2) null : certificate2Collection[0];
    }

    public static X509Certificate2 GetCertificate(string thumbprint)
    {
      X509Store x509Store = new X509Store();
      x509Store.Open(OpenFlags.OpenExistingOnly);
      X509Certificate2Collection certificate2Collection = x509Store.Certificates.Find(X509FindType.FindByThumbprint, (object) thumbprint, false);
      return certificate2Collection.Count != 1 ? (X509Certificate2) null : certificate2Collection[0];
    }

    public static string EncryptString(string plaintext, EncryptionSettings settings)
    {
      switch (settings.EncryptionMethod.Value)
      {
        case EncryptionMethod.LogonCredentials:
          return Encryption.EncryptStringUsingLocalUser(plaintext);
        case EncryptionMethod.Certificate:
          return Encryption.EncryptStringUsingCertificate(Encryption.GetCertificate(settings.CredentialData.Value), plaintext);
        default:
          throw new NotImplementedException("Unexpected encryption method '{0}'".InvariantFormat((object) settings.EncryptionMethod.Value.ToString()));
      }
    }

    private static unsafe string EncryptStringUsingLocalUser(string plaintext)
    {
      Crypto.DataBlob optionalEntropy = new Crypto.DataBlob();
      Crypto.CryptProtectPromptStruct promptStruct = new Crypto.CryptProtectPromptStruct();
      if (string.IsNullOrEmpty(plaintext))
        return (string) null;
      optionalEntropy.Size = 0;
      promptStruct.Size = 0;
      char[] charArray = plaintext.ToCharArray();
      Crypto.DataBlob dataIn;
      dataIn.Size = charArray.Length * 2;
      fixed (char* chPtr = charArray)
      {
        dataIn.Data = (IntPtr) (void*) chPtr;
        Crypto.DataBlob dataOut;
        if (!Crypto.CryptProtectData(ref dataIn, (string) null, ref optionalEntropy, (IntPtr) (void*) null, ref promptStruct, 0, out dataOut))
        {
          FormTools.ErrorDialog("Unable to encrypt password");
          return (string) null;
        }
        // ISSUE: __unpin statement
        __unpin(chPtr);
        byte* data = (byte*) (void*) dataOut.Data;
        byte[] inArray = new byte[dataOut.Size];
        for (int index = 0; index < inArray.Length; ++index)
          inArray[index] = data[index];
        string base64String = Convert.ToBase64String(inArray);
        Crypto.LocalFree(dataOut.Data);
        return base64String;
      }
    }

    private static string EncryptStringUsingCertificate(X509Certificate2 cert, string plaintext) => Convert.ToBase64String(((RSACryptoServiceProvider) cert.PublicKey.Key).Encrypt(Encoding.UTF8.GetBytes(plaintext), false));

    public static string DecryptString(string encryptedString, EncryptionSettings settings)
    {
      if (string.IsNullOrEmpty(encryptedString))
        return encryptedString;
      switch (settings.EncryptionMethod.Value)
      {
        case EncryptionMethod.LogonCredentials:
          return Encryption.DecryptStringUsingLocalUser(encryptedString);
        case EncryptionMethod.Certificate:
          return Encryption.DecryptStringUsingCertificate(Encryption.GetCertificate(settings.CredentialData.Value) ?? throw new Exception("Certificate '{0}' with thumbprint '{1}' not found".InvariantFormat((object) settings.CredentialName.Value, (object) settings.CredentialData.Value)), encryptedString);
        default:
          throw new NotImplementedException("Unexpected encryption method '{0}'".InvariantFormat((object) settings.EncryptionMethod.Value.ToString()));
      }
    }

    private static unsafe string DecryptStringUsingLocalUser(string encryptedString)
    {
      Crypto.DataBlob optionalEntropy = new Crypto.DataBlob();
      Crypto.CryptProtectPromptStruct promptStruct = new Crypto.CryptProtectPromptStruct();
      if (string.IsNullOrEmpty(encryptedString))
        return string.Empty;
      optionalEntropy.Size = 0;
      promptStruct.Size = 0;
      byte[] numArray = Convert.FromBase64String(encryptedString);
      Crypto.DataBlob dataIn;
      dataIn.Size = numArray.Length;
      fixed (byte* numPtr = numArray)
      {
        dataIn.Data = (IntPtr) (void*) numPtr;
        Crypto.DataBlob dataOut;
        if (!Crypto.CryptUnprotectData(ref dataIn, (string) null, ref optionalEntropy, (IntPtr) (void*) null, ref promptStruct, 0, out dataOut))
          throw new Exception("Failed to decrypt using {0} credential".InvariantFormat((object) CredentialsUI.GetLoggedInUser()));
        char* data = (char*) (void*) dataOut.Data;
        char[] chArray = new char[dataOut.Size / 2];
        for (int index = 0; index < chArray.Length; ++index)
          chArray[index] = data[index];
        string str = new string(chArray);
        // ISSUE: __unpin statement
        __unpin(numPtr);
        Crypto.LocalFree(dataOut.Data);
        return str;
      }
    }

    private static string DecryptStringUsingCertificate(
      X509Certificate2 cert,
      string encryptedString)
    {
      return string.IsNullOrEmpty(encryptedString) ? (string) null : Encoding.UTF8.GetString(((RSACryptoServiceProvider) cert.PrivateKey).Decrypt(Convert.FromBase64String(encryptedString), false));
    }
  }
}
