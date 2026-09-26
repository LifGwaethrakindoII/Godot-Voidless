using Godot;
using System;
using Voidless.UI;

namespace Voidless.PixelArtToolkit
{
    public partial class EnhancementsControl : ControlModule
    {
        [Export] private CheckButton darkOutlineCheckButton;
        [ExportGroup("Spinboxes")]
        [Export] private SpinBox contrastSpinBox;
        [Export] private SpinBox brightnessSpinBox;
        [ExportGroup("Sliders")]
        [Export] private HSlider contrastSlider;
        [Export] private HSlider brightnessSlider;

        public bool DarkOutline { get { return darkOutlineCheckButton.ButtonPressed; } }

        public float Contrast { get { return (float)contrastSlider.Value; } }
        
        public float Brightness { get { return (float)brightnessSlider.Value; } }

        public override void _Ready()
        {
            contrastSpinBox.SetupNormalized(Constants.STEP_RANGE, true, contrastSlider);
            brightnessSpinBox.SetupNormalized(Constants.STEP_RANGE, true, brightnessSlider);

            darkOutlineCheckButton.Toggled += OnDarkOutlineCheckBoxToggled;
            contrastSlider.ValueChanged += OnContrastSliderValueChanged;
            brightnessSlider.ValueChanged += OnBrightnessSliderValueChanged;
            brightnessSpinBox.ValueChanged += OnBrightnessSpinBoxValueChanged;
            contrastSpinBox.ValueChanged += OnContrastSpinBoxValueChanged;

            GD.Print(contrastSpinBox.RangeToString());
        }

        public void SetValues(bool darkOutline, float contrast, float brightness, bool sendSignal = true)
        {
            switch(sendSignal)
            {
                case true:
                    darkOutlineCheckButton.SetPressed(darkOutline);
                    contrastSpinBox.SetValue(contrast);
                    brightnessSpinBox.SetValue(brightness); // Fixed typo
                    contrastSlider.SetValue(contrast);
                    brightnessSlider.SetValue(brightness);  // Fixed typo
                break;

                case false:
                    darkOutlineCheckButton.SetPressedNoSignal(darkOutline);
                    contrastSpinBox.SetValueNoSignal(contrast);
                    brightnessSpinBox.SetValueNoSignal(brightness); // Fixed typo
                    contrastSlider.SetValueNoSignal(contrast);
                    brightnessSlider.SetValueNoSignal(brightness);  // Fixed typo
                break;
            }
        }

#region Callbacks:
        private void OnDarkOutlineCheckBoxToggled(bool toggled)
        {
            InvokeChangedSignal();
        }

        private void OnContrastSliderValueChanged(double value)
        {
            contrastSpinBox.SetValueNoSignal(value);
            InvokeChangedSignal();
        }

        private void OnBrightnessSliderValueChanged(double value)
        {
            brightnessSpinBox.SetValueNoSignal(value);
            InvokeChangedSignal();
        }

        private void OnBrightnessSpinBoxValueChanged(double value)
        {
            brightnessSlider.SetValueNoSignal(value);
            InvokeChangedSignal();
        }

        private void OnContrastSpinBoxValueChanged(double value)
        {
            contrastSlider.SetValueNoSignal(value);
            InvokeChangedSignal();
        }
#endregion
    }
}