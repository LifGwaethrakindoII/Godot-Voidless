using Godot;
using System;

namespace Voidless.UI
{
    public partial class DisplayDialogue : ColorRect
    {
        [Export] private float _fadeOutDuration;
        [Export] private float _fadeInDuration;
        [Export] private float _scaleDownDuration;
        [Export] private float _scaleUpDuration;
        [ExportCategory("UI Components:")]
        [Export] private ColorRect _background;
        [Export] private PanelContainer _panel;
        [Export] private Label _titleLabel;
        [Export] private Label _messageLabel;
        [Export] private Button _cancelButton;
        [Export] private Button _confirmButton;
        private Color backgroundColor;

        public float fadeInDuration
        {
            get { return _fadeInDuration; }
            set { _fadeInDuration = value; }
        }

        public float fadeOutDuration
        {
            get { return _fadeOutDuration; }
            set { _fadeOutDuration = value; }
        }

        public float scaleDownDuration
        {
            get { return _scaleDownDuration; }
            set { _scaleDownDuration = value; }
        }

        public float scaleUpDuration
        {
            get { return _scaleUpDuration; }
            set { _scaleUpDuration = value; }
        }

        public ColorRect background { get { return _background; } }

        public PanelContainer panel { get { return _panel; } }

        public Label titleLabel { get { return _titleLabel; } }

        public Label messageLabel { get { return _messageLabel; } }

        public Button cancelButton { get { return _cancelButton; } }

        public Button confirmButton { get { return _confirmButton; } }

        public override void _Ready()
        {
            background.Visible = false;
            panel.Visible = false;
            backgroundColor = background != null ? background.Color : Colors.Transparent;
            panel.PivotOffset = panel.Size * 0.5f;

            //ShowDialog("Ai b0ss!", "Skipiri bop!", null, HideDialog);
        }

        public async void ShowDialog(string title = null, string message = null, Action onCancelPressed = null, Action onConfirmPressed = null, Action onPopUpScaled = null)
        {
            titleLabel.Visible = !string.IsNullOrEmpty(title);
            messageLabel.Visible = !string.IsNullOrEmpty(message);
            cancelButton.Visible = onCancelPressed != null;
            confirmButton.Visible = onConfirmPressed != null;
            titleLabel.Text = title;
            messageLabel.Text = message;

            if(background != null)
            {
                background.Color = Colors.Transparent;
                background.Visible = true;
                await background.LerpColor(Colors.Transparent, backgroundColor, fadeInDuration, VMath.EaseInOutSine);
            }

            panel.Scale = Vector2.One;
            panel.Visible = true;

            await panel.LerpScale(Vector2.Zero, Vector2.One, scaleUpDuration, VMath.EaseInOutBounce);

            cancelButton.Pressed -= onCancelPressed;
            confirmButton.Pressed -= onConfirmPressed;
            cancelButton.Pressed += onCancelPressed;
            confirmButton.Pressed += onConfirmPressed;

            if(onPopUpScaled != null) onPopUpScaled();
        }

        public async void HideDialog()
        {
            await panel.LerpScale(Vector2.One, Vector2.Zero, scaleDownDuration, VMath.EaseInOutSine);
            if(background != null) await background.LerpColor(backgroundColor, Colors.Transparent, fadeOutDuration, VMath.EaseInOutSine);

            if(background != null) background.Visible = false;
            panel.Visible = false;
        }
    }
}