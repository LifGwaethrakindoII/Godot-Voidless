using Godot;
using System;

namespace Voidless.PixelArtToolkit
{
    public delegate void OnProcessRequested();

    public delegate void OnPreviewToggled(bool isPreviewEnabled);

    public partial class ProcessControl : ControlModule
    {
        public event OnProcessRequested OnProcessRequested;
        public event OnPreviewToggled OnPreviewToggled;

        [Export] private CheckButton previewCheckButton;
        [Export] private Button processButton;

        public override void _Ready()
        {
            processButton.Pressed += OnProcessButtonPressed;
            previewCheckButton.Toggled += OnPreviewCheckButtonToggled;
        }

        public void SetValues(bool toggled)
        {
            previewCheckButton.ButtonPressed = toggled;
        }

        private void OnProcessButtonPressed()
        {
            if(OnProcessRequested != null) OnProcessRequested();
        }

        private void OnPreviewCheckButtonToggled(bool toggledOn)
        {
            if(OnPreviewToggled != null) OnPreviewToggled(toggledOn);
        }
    }
}