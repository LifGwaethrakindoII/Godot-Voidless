using Godot;
using System;

namespace  Voidless.Pong
{
    public partial class PongGameplayUIController : CanvasLayer
    {
        [ExportCategory("UI Elements:")]
        [Export] private CheckButton debugCheckbox;
        [Export] private OptionButton AIModeDropdown;
        [Export] private OptionButton difficultyDropdown;
        [Export] private HSlider timeScalarSlider;
        [Export] private HSlider paddleSpeedSlider;
        [Export] private HSlider ballSpeedSlider;
        [Export] private Button resetGameButton;
        [Export] private Button resetParametersButton;
        [Export] private Label timeScalarLabel;
        [Export] private Label paddleSpeedLabel;
        [Export] private Label ballSpeedLabel;
        private PongGameplayManager _gameplayManager;

        public PongGameplayManager gameplayManager
        {
            get { return _gameplayManager; }
            set { _gameplayManager = value; }
        }

        public override void _Ready()
        {
            if(debugCheckbox != null) debugCheckbox.Toggled += OnDebugToggled;
            if(AIModeDropdown != null) AIModeDropdown.ItemSelected += OnAIModeSelected;
            if(difficultyDropdown != null) difficultyDropdown.ItemSelected += OnDifficultySelected;
            if(timeScalarSlider != null) timeScalarSlider.ValueChanged += OnTimeScalarValueChanged;
            if(paddleSpeedSlider != null) paddleSpeedSlider.ValueChanged += OnPaddleSpeedValueChanged;
            if(ballSpeedSlider != null) ballSpeedSlider.ValueChanged += OnBallSpeedValueChanged;
            if(resetGameButton != null) resetGameButton.Pressed += OnResetGamePressed;
            if(resetParametersButton != null) resetParametersButton.Pressed += OnResetParametersPressed;
        }

        private void SetSliderValue(HSlider slider, float value, Label label = null)
        {
            if(slider != null) slider.Value = value;
            SetSliderLabelText(label, value);
        }

        private void SetSliderLabelText(Label label, double value)
        {
            if(label != null) label.Text = value.ToString("00.00");
        }

        public void SetValues(bool debug, float ballSpeed, float paddleSpeed, float timeScalar, AIDifficulty difficulty, AIMode aiMode)
        {
            debugCheckbox.ButtonPressed = debug;
            SetSliderValue(ballSpeedSlider, ballSpeed, ballSpeedLabel);
            SetSliderValue(paddleSpeedSlider, paddleSpeed, paddleSpeedLabel);
            SetSliderValue(timeScalarSlider, timeScalar, timeScalarLabel);
        }

        private void OnDebugToggled(bool toggled)
        {
            if(gameplayManager != null) gameplayManager.OnDebugToggled(toggled);
        }

        private void OnAIModeSelected(long index)
        {
            if(gameplayManager != null) gameplayManager.OnAIModeSelected(index);
        }

        private void OnDifficultySelected(long index)
        {
            if(gameplayManager != null) gameplayManager.OnDifficultySelected(index);
        }

        private void OnTimeScalarValueChanged(double value)
        {
            if(gameplayManager != null) gameplayManager.OnTimeScalarValueChanged(value);
            SetSliderLabelText(timeScalarLabel, value);
        }

        private void OnPaddleSpeedValueChanged(double value)
        {
            if(gameplayManager != null) gameplayManager.OnPaddleSpeedValueChanged(value);
            SetSliderLabelText(paddleSpeedLabel, value);
        }

        private void OnBallSpeedValueChanged(double value)
        {
            if(gameplayManager != null) gameplayManager.OnBallSpeedValueChanged(value);
            SetSliderLabelText(ballSpeedLabel, value);
        }

        private void OnResetGamePressed()
        {
            if(gameplayManager != null) gameplayManager.OnResetGamePressed();
        }

        private void OnResetParametersPressed()
        {
            if(gameplayManager != null) gameplayManager.OnResetParametersPressed();
        }
    }
}