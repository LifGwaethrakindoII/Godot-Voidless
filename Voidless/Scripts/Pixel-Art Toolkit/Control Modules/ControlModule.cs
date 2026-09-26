using Godot;
using System;

namespace Voidless.PixelArtToolkit
{
    public delegate void OnControlChanged();

    public partial class ControlModule : Control
    {
        public event OnControlChanged OnChanged;

        protected virtual void InvokeChangedSignal()
        {
            if(OnChanged != null) OnChanged();
        }
    }
}