using Godot;
using System;

namespace Voidless.Pong
{
	public partial class PongBall : CharacterBody2D
	{
		[Export] private float speed;
		[ExportCategory("Bounce Attributes:")]
		[Export(PropertyHint.Range, "0.0, 1.0")] private float distanceInfluence;
		[Export(PropertyHint.Range, "0.0, 1.0")] private float velocityInfluence;
		[Export(PropertyHint.Range, "0.0, 1.0")] private float speedBoost;

		public float Speed
		{
			get { return speed; }
			set { speed = value; }
		}

		// Called when the node enters the scene tree for the first time.
		public override void _Ready()
		{
			Reset();
		}
		
		public override void _PhysicsProcess(double delta)
		{
			KinematicCollision2D collision = MoveAndCollide(Velocity * (float)delta);
			
			if(collision == null) return;
			
			PongPaddle paddle = collision.GetCollider() as PongPaddle;
			bool isPaddle = paddle != null;

			// Because our velocity is still intact, Bounce works perfectly now!
			Velocity = isPaddle ? CalculatePaddleBounce(collision, paddle) : Velocity.Bounce(collision.GetNormal());
		}
		
		public void Reset()
		{
			Reset(VRandom.Sign());
		}
		
		public void Reset(float xd)
		{
			// Position ball at center of scenario.
			Position = PongGameplayManager.CENTER;
			
			// Set new random velocity.
			Vector2 v = Vector2.Zero;
			
			v.X = xd;
			v.Y = VRandom.Range(-1f, 1f);
			
			Velocity = v * speed;
		}

		private Vector2 CalculatePaddleBounce(KinematicCollision2D collision, PongPaddle paddle)
		{
			Vector2 sn = collision.GetNormal(); 				// Paddle's surface normal
			Vector2 hp = collision.GetPosition(); 				// Hit's point
			float dy = hp.Y - paddle.Position.Y;

			dy /= paddle.halfHeight; 							// Normalize the Y-Offset.

			Vector2 n = sn;
			n.Y = (dy * distanceInfluence);
			n = n.Normalized();

			Vector2 b = Velocity.Bounce(n);
			//b.X = Mathf.Abs(b.X) * Mathf.Sign(paddle.Position.X);
			b.Y += (paddle.Velocity.Y * velocityInfluence); 	/// Add the paddle's Y-velocity.
			b *= (1.0f + speedBoost);
			b = b.ClampedMagnitude(speed * 3.0f);

			return b;
		}
	}
}
