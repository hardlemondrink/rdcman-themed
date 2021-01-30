// Decompiled with JetBrains decompiler
// Type: RdcMan.ServerBase
// Assembly: RDCMan, Version=2.7.1406.0, Culture=neutral, PublicKeyToken=null
// MVID: 984CDC7A-116B-406E-B519-0DEE541B9019
// Assembly location: C:\Program Files (x86)\Microsoft\Remote Desktop Connection Manager\RDCMan.exe

using System.Drawing;
using System.Windows.Forms;

namespace RdcMan
{
  public abstract class ServerBase : RdcTreeNode
  {
    public string ServerName => this.ServerNode.Properties.ServerName.Value;

    public string DisplayName => this.ServerNode.Properties.DisplayName.Value;

    public abstract string RemoveTypeDescription { get; }

    public abstract bool IsConnected { get; }

    public abstract bool IsClientDocked { get; }

    public abstract bool IsClientUndocked { get; }

    public abstract bool IsClientFullScreen { get; }

    public abstract ServerSettings Properties { get; }

    public new abstract CommonDisplaySettings DisplaySettings { get; }

    public bool IsThumbnail => this.DisplayState == ServerBase.DisplayStates.Thumbnail;

    public abstract ServerBase.DisplayStates DisplayState { get; set; }

    public abstract Size Size { get; set; }

    public abstract Point Location { get; set; }

    public override bool ConfirmRemove(bool askUser)
    {
      if (!this.CanRemove(true))
        return false;
      if (askUser)
      {
        if (FormTools.YesNoDialog((Form) this.ParentForm, "Remove '{0}' {1} from '{2}'?".InvariantFormat((object) this.Text, (object) this.RemoveTypeDescription, (object) this.Parent.Text), MessageBoxDefaultButton.Button1) != DialogResult.Yes)
          return false;
      }
      return true;
    }

    internal abstract void Focus();

    internal abstract void FocusConnectedClient();

    internal abstract void ScreenCapture();

    internal abstract void GoFullScreen();

    internal abstract void LeaveFullScreen();

    internal abstract void Undock();

    internal abstract void Dock();

    public abstract Server ServerNode { get; }

    public enum DisplayStates
    {
      Invalid,
      Normal,
      Thumbnail,
    }
  }
}
