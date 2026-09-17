using Godot;

namespace Voidless.Pong
{
	public enum AIMode { Scripted, NeuralNetwork }
	
	public enum AIDifficulty { Easy, Normal, Hard }
	
	public partial class PongAIPaddleController : Node2D
	{
		[Export] private PongPaddle paddle;
		[Export] private CharacterBody2D ball;
		[ExportCategory("AI Parameters:")]
		[Export] private AIMode _AIMode;
		[Export] private AIDifficulty _difficulty;
		[Export(PropertyHint.Range, "0, 1")] private float reactionTime;
		[Export] private bool _debug;
		private float targetY;
		private float reactionTimer;

		public AIMode AIMode
		{
			get { return _AIMode; }
			set { _AIMode = value; }
		}

		public AIDifficulty difficulty
		{
			get { return _difficulty; }
			set { _difficulty = value; }
		}

		public bool debug
		{
			get { return _debug; }
			set { _debug = value; }
		}
		
#region GDCallbacks:	
		public override void _Draw()
		{
			if(!debug || paddle == null || ball == null) return;

			// 1. Draw a line from the paddle to the target Y position
			Vector2 paddlePos = paddle.Position;
			Vector2 targetPos = new Vector2(paddlePos.X, targetY);

			Vector2 localPaddlePosition = ToLocal(paddlePos);
			Vector2 localTargetPosition = ToLocal(targetPos);

			// DrawLine(Vector2 from, Vector2 to, Color color, float width = 1.0f)
			DrawLine(paddlePos, localPaddlePosition, Colors.Red, 3.0f);

			// 2. Draw a circle at the target position
			DrawCircle(localTargetPosition, 5.0f, Colors.Yellow);

			// 3. Draw text (e.g., the current AI mode)
			DrawString(
				ThemeDB.FallbackFont, 
				new Vector2(localPaddlePosition.X + 10, localPaddlePosition.Y - 20), 
				$"Mode: {AIMode}", 
				HorizontalAlignment.Left, 
				-1, 
				16, 
				Colors.White
			);

			QueueRedraw();
		}
		
		public override void _Ready()
		{
			targetY = ball.Position.Y;
			reactionTimer = 0.0f;
		}
		
		public override void _Process(double delta)
		{
			if(ball == null || paddle == null) return;
			
			float dt = (float)delta;
			
			switch(AIMode)
			{
				case AIMode.Scripted:
					ScriptedProcess(dt);
				break;
				
				case AIMode.NeuralNetwork:
					NeuralNetworkProcess(dt);
				break;
			}

			QueueRedraw(); 
		}
		
		public override void _PhysicsProcess(double delta)
		{
			if(paddle == null || ball == null) return;
			
			float dt = (float)delta;
			
			switch(AIMode)
			{
				case AIMode.Scripted:
					ScriptedPhysicsProcess(dt);
				break;
				
				case AIMode.NeuralNetwork:
					NeuralNetworkPhysicsProcess(dt);
				break;
			}
		}
#endregion
		
#region Methods:	
		private void ScriptedProcess(float dt)
		{
			if(reactionTimer >= reactionTime)
			{ // Update Game's information each reaction time.
				float pd = Mathf.Sign(paddle.Transform.X.X);
				float bd = Mathf.Sign(ball.Velocity.X);
				reactionTimer = 0.0f;
				
				// If the ball goes towards this paddle, the target Y is the ball's Y.
				// Else if the ball goes towards the rival's paddle, the target Y is the center.
				targetY = pd != bd ? ball.Position.Y : GetViewport().GetVisibleRect().Size.Y * 0.5f;
			}
			else reactionTimer += dt;
		}
		
		private void NeuralNetworkProcess(float dt) {}
		
		private void ScriptedPhysicsProcess(float dt)
		{
			float dy = targetY - paddle.Position.Y;
			float d = 0f;

			if (dy < -paddle.halfHeight) d = 1f;
			else if (dy > paddle.halfHeight) d = -1f;

			paddle.Move(d, dt);
		}
		
		private void NeuralNetworkPhysicsProcess(float dt) {}
#endregion
	}
}
