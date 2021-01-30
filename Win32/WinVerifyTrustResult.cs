// Decompiled with JetBrains decompiler
// Type: Win32.WinVerifyTrustResult
// Assembly: RDCMan, Version=2.7.1406.0, Culture=neutral, PublicKeyToken=null
// MVID: 984CDC7A-116B-406E-B519-0DEE541B9019
// Assembly location: C:\Program Files (x86)\Microsoft\Remote Desktop Connection Manager\RDCMan.exe

namespace Win32
{
  public enum WinVerifyTrustResult : uint
  {
    Success = 0,
    SubjectCertificateRevoked = 134262800, // 0x0800B010
    SignatureOrFileCorrupt = 2148098064, // 0x80096010
    ProviderUnknown = 2148204545, // 0x800B0001
    ActionUnknown = 2148204546, // 0x800B0002
    SubjectFormUnknown = 2148204547, // 0x800B0003
    SubjectNotTrusted = 2148204548, // 0x800B0004
    FileNotSigned = 2148204800, // 0x800B0100
    SubjectCertExpired = 2148204801, // 0x800B0101
    SubjectExplicitlyDistrusted = 2148204817, // 0x800B0111
  }
}
