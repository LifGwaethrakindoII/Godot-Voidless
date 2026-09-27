using Godot;
using System;

namespace Voidless.PixelArtToolkit
{
    public delegate void OnControlChanged();

    public delegate void OnControlError(string message);

    public partial class ControlModule : Control
    {
        public event OnControlChanged OnChanged;
        public event OnControlError OnControlError;

        protected virtual void InvokeChangedSignal()
        {
            if(OnChanged != null) OnChanged();
        }

        protected virtual void InvokeErrorSignal(string message)
        {
            if(OnControlError != null) OnControlError(message);
        }
    }
}