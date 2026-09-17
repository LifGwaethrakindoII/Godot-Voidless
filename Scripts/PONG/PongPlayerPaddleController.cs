using Godot;
using System;

namespace Voidless.Pong
{
	public partial class PongPlayerPaddleController : Node
	{
		[Export] public PongPaddle playerPaddle;
		
		// Called when the node enters the scene tree for the first time.
		public override void _Ready()
		{
		}

		// Called every frame. 'delta' is the elapsed time since the previous frame.
		public override void _Process(double delta)
		{
			
		}
		
		public override void _PhysicsProcess(double delta)
		{
			ProcessInput(delta);
		}
		
		private void ProcessInput(double dt)
		{
			if(playerPaddle == null) return;
			
			float d = Input.GetAxis("ui_down", "ui_up");
			playerPaddle.Move(d, (float)dt);
		}
	}
}
