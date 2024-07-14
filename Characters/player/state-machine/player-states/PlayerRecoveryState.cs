using Godot;

public class PlayerRecoveryState : PlayerActionState, ICanMove
{
	public float RecoveryTimer { get; set; } = 0f;
	public float RecoveryDuration { get; } = 1f;
	
	public float Acceleration { get; set; }
	public string MovementBlendPath { get; } = "parameters/move-blend-tree/move-blend-space/blend_position";
	public Vector2 Direction { get; set; }
	
	public PlayerRecoveryState(Player player) : base(player)
	{
		
	}

	public override void EnterState()
	{
		
	}

	public override void ExitState()
	{
		
	}
	public void GetMovementInput()
	{
		Direction = Input.GetVector("left", "right", "up", "down").Normalized();
	}

	public void Move(float delta)
	{
		if (Direction == Vector2.Zero)
		{
			Decelerate(delta);
		}
		else
		{
			Accelerate(delta);
			HandleSpriteFlip();
		}

		Animate();
	}

	public override void PhysicsProcessState(double delta) {}

	private void Decelerate(float delta)
	{
		float deceleration = CharacterBody2D.Data.ScubaGear.Deceleration;
		if (CharacterBody2D.Velocity.Length() > (deceleration * delta))
		{
			CharacterBody2D.Velocity -= CharacterBody2D.Velocity.Normalized() * (deceleration * delta);
		}
		else
		{
			CharacterBody2D.Velocity = Vector2.Zero;
		}
	}
	
	private void Accelerate(float delta)
	{
		float targetAcceleration = AngleToAccel();
		
		Acceleration = Mathf.MoveToward(
			Acceleration, 
			targetAcceleration, 
			(float)delta * CharacterBody2D.Data.ScubaGear.Jerk);
			
		CharacterBody2D.Velocity += Direction * /* target */Acceleration * delta;
		
		CharacterBody2D.Velocity = CharacterBody2D.Velocity.LimitLength(CharacterBody2D.Data.ScubaGear.MaxSpeed);
	}
	
	/// <summary>
	/// Translates angle between direction and Velocity into a number between 
	/// AccelerationMin and AccelerationMax, with smaller angles giving higher acceleration. 
	/// </summary>
	private float AngleToAccel()
	{
		Vector2 normVel = CharacterBody2D.Velocity.Normalized();
		float dot = normVel.Dot(Direction);
		return 0.5f * 
			(CharacterBody2D.Data.ScubaGear.MaxAcceleration - CharacterBody2D.Data.ScubaGear.MinAcceleration) * 
			(1 - dot) + 
			CharacterBody2D.Data.ScubaGear.MinAcceleration;
	}

	private void HandleSpriteFlip()
	{
		if (Direction.X != 0)
		{
			CharacterBody2D.Sprite.FlipH = Direction.X < 0;
		}
	}

	private void Animate()
	{
		CharacterBody2D.AnimationTree.Set(MovementBlendPath, Direction);
	}

}
