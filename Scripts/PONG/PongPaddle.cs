using Godot;
using System;

namespace Voidless.Pong
{
	public partial class PongPaddle : CharacterBody2D
	{
		[Export] public float speed;
		private CollisionShape2D _collisionShape;

		public float Speed
		{
			get { return speed; }
			set { speed = value; }
		}
		
		public CollisionShape2D collisionShape
		{
			get
			{
				if(_collisionShape == null) _collisionShape = this.GetNode<CollisionShape2D>("CollisionShape2D");
				return _collisionShape;
			}
		}
		
		public Vector2 size { get { return collisionShape.GetShape().GetRect().Size; } }
		
		public float halfHeight { get { return size.Y * 0.5f; } }
		
		public void Move(float d, float dt)
		{
			Velocity = Vector2.Up * (d * speed);
			MoveAndSlide();
		}	
	}
}
