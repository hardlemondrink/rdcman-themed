// Decompiled with JetBrains decompiler
// Type: RdcMan.Policies
// Assembly: RDCMan, Version=2.7.1406.0, Culture=neutral, PublicKeyToken=null
// MVID: 984CDC7A-116B-406E-B519-0DEE541B9019
// Assembly location: C:\Program Files (x86)\Microsoft\Remote Desktop Connection Manager\RDCMan.exe

using Microsoft.Win32;

namespace RdcMan
{
  public class Policies
  {
    public const string PolicyRegKey = "RDCMan";
    public static bool DisableLogOff;

    public static void Read()
    {
      try
      {
        RegistryKey subKey = Registry.LocalMachine.OpenSubKey("Software", true).OpenSubKey(nameof (Policies), true).OpenSubKey("Microsoft", true).CreateSubKey("RDCMan", RegistryKeyPermissionCheck.ReadSubTree);
        if (subKey == null)
          return;
        Policies.DisableLogOff = (int) subKey.GetValue("DisableLogOff", (object) 0) == 1;
      }
      catch
      {
      }
    }
  }
}
