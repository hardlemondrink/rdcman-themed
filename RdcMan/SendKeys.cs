// Decompiled with JetBrains decompiler
// Type: RdcMan.SendKeys
// Assembly: RDCMan, Version=2.7.1406.0, Culture=neutral, PublicKeyToken=null
// MVID: 984CDC7A-116B-406E-B519-0DEE541B9019
// Assembly location: C:\Program Files (x86)\Microsoft\Remote Desktop Connection Manager\RDCMan.exe

using MSTSCLib;
using System.Windows.Forms;

namespace RdcMan
{
  internal class SendKeys
  {
    public static unsafe void Send(Keys[] keyCodes, ServerBase serverBase)
    {
      IMsRdpClientNonScriptable ocx = (IMsRdpClientNonScriptable) serverBase.ServerNode.Client.GetOcx();
      int length = keyCodes.Length;
      try
      {
        SendKeys.SendKeysData sendKeysData;
        bool* keyUp = (bool*) sendKeysData.keyUp;
        int* keyData = sendKeysData.keyData;
        int numKeys = 0;
        for (int index1 = 0; index1 < length && index1 < 10; ++index1)
        {
          int num = (int) Win32.Util.MapVirtualKey((uint) keyCodes[index1], 0U);
          sendKeysData.keyData[numKeys] = num;
          sendKeysData.keyUp[numKeys++] = (short) 0;
          if (!SendKeys.IsModifier(keyCodes[index1]))
          {
            for (int index2 = numKeys - 1; index2 >= 0; --index2)
            {
              sendKeysData.keyData[numKeys] = sendKeysData.keyData[index2];
              sendKeysData.keyUp[numKeys++] = (short) 1;
            }
            ocx.SendKeys(numKeys, ref *keyUp, ref *keyData);
            numKeys = 0;
          }
        }
      }
      catch
      {
      }
    }

    private static bool IsModifier(Keys key)
    {
      switch (key)
      {
        case Keys.ShiftKey:
        case Keys.ControlKey:
        case Keys.Menu:
        case Keys.LWin:
        case Keys.RWin:
        case Keys.LControlKey:
        case Keys.RControlKey:
          return true;
        default:
          return false;
      }
    }

    private struct SendKeysData
    {
      public const int MaxKeys = 20;
      public unsafe fixed short keyUp[20];
      public unsafe fixed int keyData[20];
    }
  }
}
