using Godot;
using System;
using System.Text;
using Voidless.UI;

namespace Voidless.PixelArtToolkit
{
    public partial class ScalingControl : ControlModule
    {
        [Export] private Label outputLabel;
        [Export] private SpinBox scaleFactorSpinBox;
        [ExportGroup("Buttons")]
        [Export] private Button x1ScaleButton;
        [Export] private Button x2ScaleButton;
        [Export] private Button x4ScaleButton;
        [Export] private Button x8ScaleButton;
        [Export] private Button x16ScaleButton;
        [Export] private Button x32ScaleButton;
        private int scaleFactor;
        private int targetWidth;
        private int targetHeight;

        public int ScaleFactor { get { return scaleFactor; } }

        public int TargetWidth
        {
            get { return targetWidth; }
            set { targetWidth = value; }
        }

        public int TargetHeight
        {
            get { return targetHeight; }
            set { targetHeight = value; }
        }

        public Vector2I ScaledDimensions { get { return new Vector2I(targetWidth, TargetHeight) * scaleFactor; } }

        public override void _Ready()
        {
            scaleFactorSpinBox.SetupIntRange(0, 32);

            scaleFactorSpinBox.ValueChanged += OnScaleFactorSpinBoxValueChanged;
            x1ScaleButton.Pressed += ()=> OnScaleFactorButtonPressed(1);
            x2ScaleButton.Pressed += ()=> OnScaleFactorButtonPressed(2);
            x4ScaleButton.Pressed += ()=> OnScaleFactorButtonPressed(4);
            x8ScaleButton.Pressed += ()=> OnScaleFactorButtonPressed(8);
            x16ScaleButton.Pressed += ()=> OnScaleFactorButtonPressed(16);
            x32ScaleButton.Pressed += ()=> OnScaleFactorButtonPressed(32);
        }

        public void SetValues(int scale, int width, int height)
        {
            scaleFactorSpinBox.SetValue(scale);
            scaleFactor = scale;
            TargetWidth = width;
            TargetHeight = height;
            UpdateOutputLabel();
        }

        private void UpdateOutputLabel()
        {
            StringBuilder builder = new StringBuilder();
            int width = targetWidth * scaleFactor;
            int height = targetHeight * scaleFactor;    

            builder.Append(width.ToString());
            builder.Append(" x ");
            builder.Append(height.ToString());
            builder.Append("px");

            outputLabel.Text = builder.ToString();
        }

        private void OnScaleFactorButtonPressed(int scale)
        {
            scaleFactor = scale;
            scaleFactorSpinBox.SetValueNoSignal(scale);
            UpdateOutputLabel();
        }

        private void OnScaleFactorSpinBoxValueChanged(double value)
        {
            scaleFactor = (int)value;
            UpdateOutputLabel();
        }
    }
}