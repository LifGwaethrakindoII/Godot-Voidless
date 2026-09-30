using Godot;
using System;
using Voidless.UI;

namespace Voidless.PixelArtToolkit
{
    public partial class DimensionsControl : ControlModule
    {
        [Export] private CheckBox proportionalEditingCheckBox;
        [ExportGroup("Spinboxes")]
        [Export] private SpinBox widthSpinBox;
        [Export] private SpinBox heightSpinBox;
        [ExportGroup("Buttons")]
        [Export] private Button squareDimensionButton8;
        [Export] private Button squareDimensionButton16;
        [Export] private Button squareDimensionButton32;
        [Export] private Button squareDimensionButton64;
        [Export] private Button squareDimensionButton128;
        [Export] private Button squareDimensionButton256;
        
        public int Width { get { return (int)widthSpinBox.Value; } }

        public int Height { get { return (int)heightSpinBox.Value; } }

        public bool ProportionalEditing { get { return proportionalEditingCheckBox.ButtonPressed; } }

        public override void _Ready()
        {
            widthSpinBox.SetupIntRange(0, App.MAX_DIMENSION);
            heightSpinBox.SetupIntRange(0, App.MAX_DIMENSION);

            widthSpinBox.ValueChanged += OnWidthSpinBoxValueChanged;
            heightSpinBox.ValueChanged += OnHeightSpinBoxValueChanged;
            proportionalEditingCheckBox.Toggled += OnProportionalEditingCheckBoxToggled;
            squareDimensionButton8.Pressed += ()=> OnSquareDimensionButtonPressed(8);
            squareDimensionButton16.Pressed += ()=> OnSquareDimensionButtonPressed(16);
            squareDimensionButton32.Pressed += ()=> OnSquareDimensionButtonPressed(32);
            squareDimensionButton64.Pressed += ()=> OnSquareDimensionButtonPressed(64);
            squareDimensionButton128.Pressed += ()=> OnSquareDimensionButtonPressed(128);
            squareDimensionButton256.Pressed += ()=> OnSquareDimensionButtonPressed(256);
        }

        public void SetValues(bool proportionalEditing, int width, int height, bool sendSignal = true)
        {
            switch(sendSignal)
            {
                case true:
                    proportionalEditingCheckBox.SetPressed(proportionalEditing);
                    widthSpinBox.SetValue(width);
                    heightSpinBox.SetValue(height);
                break;

                case false:
                    proportionalEditingCheckBox.SetPressedNoSignal(proportionalEditing);
                    widthSpinBox.SetValueNoSignal(width);
                    heightSpinBox.SetValueNoSignal(height);
                break;
            }
        }

#region Callbacks:
        private void OnWidthSpinBoxValueChanged(double value)
        {
            if(ProportionalEditing) heightSpinBox.SetValue(value);

            InvokeChangedSignal();
        }

        private void OnHeightSpinBoxValueChanged(double value)
        {
            if(ProportionalEditing) widthSpinBox.SetValue(value);

            InvokeChangedSignal();
        }

        private void OnProportionalEditingCheckBoxToggled(bool toggled)
        {
            if(toggled)
            {
                int max = Mathf.Max(Width, Height);

                widthSpinBox.SetValueNoSignal(max);
                heightSpinBox.SetValueNoSignal(max);
            }

            InvokeChangedSignal();
        }

        private void OnSquareDimensionButtonPressed(int dimension)
        {
            widthSpinBox.SetValueNoSignal(dimension);
            heightSpinBox.SetValueNoSignal(dimension);

            InvokeChangedSignal();
        }
#endregion
    }
}