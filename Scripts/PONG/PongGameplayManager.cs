using Godot;
using System;

namespace Voidless.Pong
{
	public enum PongGoalZone { Left, Right }
	
	public partial class PongGameplayManager : Node2D
	{
		public static readonly Vector2 CENTER;

		[ExportCategory("Game Nodes:")]
		[Export] private PongBall ball;
		[Export] private PongPaddle leftPaddle;
		[Export] private PongPaddle rightPaddle;
		[ExportCategory("Court Nodes:")]
		[Export] private Area2D leftArea;
		[Export] private Area2D rightArea;
		[ExportCategory("Controllers:")]
		[Export] private PongGameplayUIController UIController;
		[Export] private PongPlayerPaddleController playerPaddleController;
		[Export] private PongAIPaddleController AIPaddleController;
		private int p1Score;
		private int p2Score;
		private bool defaultDebug;
		private float defaultPaddleSpeed;
		private float defaultBallSpeed;
		private AIDifficulty defaultDifficulty;
		private AIMode defaultAIMode;
		
		static PongGameplayManager()
		{
			CENTER = new Vector2(-160.0f, 0.0f);
		}

		public override void _Ready()
		{
			// 2. Subscribe to the events
			// Make sure to check for null just in case you forgot to drag them in the Inspector!
			if(leftArea != null) leftArea.BodyEntered += OnLeftGoalEntered;
			if(rightArea != null) rightArea.BodyEntered += OnRightGoalEntered;
			if(UIController != null) UIController.gameplayManager = this;

			// Store default values for the UI
			defaultDebug = AIPaddleController != null ? AIPaddleController.debug : false;
			defaultBallSpeed = ball != null ? ball.Speed : 0.0f;
			defaultPaddleSpeed = rightPaddle != null && leftPaddle != null ? leftPaddle.Speed : 0.0f;
			defaultDifficulty = AIPaddleController != null ? AIPaddleController.difficulty : AIDifficulty.Easy;
			defaultAIMode = AIPaddleController != null ? AIPaddleController.AIMode : AIMode.Scripted;

			Reset();
			SetUIValues();
		}

		private void Reset()
		{
			p1Score = 0;
			p2Score = 0;
		}

		private void SetUIValues()
		{
			if(UIController != null)
			UIController.SetValues(defaultDebug, defaultBallSpeed, defaultPaddleSpeed, 1.0f, defaultDifficulty, defaultAIMode);
		}
		
		private void OnLeftGoalEntered(Node2D body)
		{
			if(body.Name == "Ball") OnGoalEntered(body, PongGoalZone.Left);
		}
		
		private void OnRightGoalEntered(Node2D body)
		{
			if(body.Name == "Ball") OnGoalEntered(body, PongGoalZone.Right);
		}
		
		// 3. The Callback Method
		private void OnGoalEntered(Node2D body, PongGoalZone side)
		{
			if(ball == null) return;
			
			ball.Reset(side == PongGoalZone.Left ? 1.0f : -1.0f);
		}

		// 4. Clean up to prevent memory leaks (Good habit!)
		public override void _ExitTree()
		{
			if (leftArea != null) leftArea.BodyEntered -= OnLeftGoalEntered;
			if (rightArea != null) rightArea.BodyEntered -= OnRightGoalEntered;
		}

#region UICallbacks:
		public void OnDebugToggled(bool toggled)
        {
			if(AIPaddleController != null) AIPaddleController.debug = toggled;
        }

        public void OnAIModeSelected(long index)
        {
			if(AIPaddleController != null) AIPaddleController.AIMode = (AIMode)index;
        }

        public void OnDifficultySelected(long index)
        {
			if(AIPaddleController != null) AIPaddleController.difficulty = (AIDifficulty)index;
        }

        public void OnTimeScalarValueChanged(double value)
        {
			Engine.TimeScale = (float)value;
        }

        public void OnPaddleSpeedValueChanged(double value)
        {
			float v = (float)value;

            if(leftPaddle != null) leftPaddle.Speed = v;
            if(rightPaddle != null) rightPaddle.Speed = v;
        }

        public void OnBallSpeedValueChanged(double value)
        {
            if(ball != null) ball.Speed = (float)value;
        }

        public void OnResetGamePressed()
        {
			p1Score = 0;
			p2Score = 0;
        }

        public void OnResetParametersPressed()
        {
			if(ball != null) ball.Speed = defaultBallSpeed;
            if(leftPaddle != null && rightPaddle != null)
			{
				leftPaddle.Speed = defaultPaddleSpeed;
				rightPaddle.Speed = defaultPaddleSpeed;
			}
			if(AIPaddleController != null)
			{
				AIPaddleController.debug = defaultDebug;
				AIPaddleController.difficulty = defaultDifficulty;
				AIPaddleController.AIMode = defaultAIMode;
			}
			Engine.TimeScale = 1.0;
			SetUIValues();
        }
#endregion
	}
}
