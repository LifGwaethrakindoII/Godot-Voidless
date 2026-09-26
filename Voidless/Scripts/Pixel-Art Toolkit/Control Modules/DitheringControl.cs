using Godot;
using System;
using Voidless.UI;

namespace Voidless.PixelArtToolkit
{
    public partial class DitheringControl : ControlModule
    {
        [Export] private MarginContainer strengthSliderContainer;
        [Export] private OptionButton ditheringTypeDropdown;
        [Export] private SpinBox strengthSpinBox;
        [Export] private HSlider strengthSlider;

        public DitheringType DitheringType { get { return (DitheringType)ditheringTypeDropdown.Selected; } }

        public float Strength { get { return (float)strengthSpinBox.Value; } }

        public override void _Ready()
        {
            strengthSpinBox.SetupNormalized(Constants.STEP_RANGE, false, strengthSlider);

            strengthSlider.ValueChanged += OnDitheringSliderValueChanged;
            strengthSpinBox.ValueChanged += OnDitheringSpinBoxValueChanged;
            ditheringTypeDropdown.ItemSelected += OnDitheringTypeOptionSelected;
        }

        public void SetValues(DitheringType ditheringType, float strength, bool sendSignal = false)
        {
            int index = (int)ditheringType;

            switch(sendSignal)
            {
                case true:
                    ditheringTypeDropdown.Selected = index;
                    strengthSpinBox.SetValue(strength);
                    strengthSlider.SetValue(strength);
                    ditheringTypeDropdown.EmitSignal(OptionButton.SignalName.ItemSelected, index);
                break;

                case false:
                    strengthSpinBox.SetValueNoSignal(strength);
                    strengthSlider.SetValueNoSignal(strength);
                break;
            }
        }

#region Callbacks:
        private void OnDitheringSliderValueChanged(double value)
        {
            InvokeChangedSignal();
        }

        private void OnDitheringSpinBoxValueChanged(double value)
        {
            InvokeChangedSignal();
        }

        private void OnDitheringTypeOptionSelected(long selection)
        {
            switch((DitheringType)selection)
            {
                case DitheringType.None:
                    strengthSliderContainer.Visible = false;
                break;

                case DitheringType.Ordered:
                case DitheringType.Floyd_S:
                    strengthSliderContainer.Visible = true;
                break;
            }

            InvokeChangedSignal();
        }
#endregion
    }
}