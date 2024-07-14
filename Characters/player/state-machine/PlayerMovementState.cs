using Godot;

// TODO: Make player rotate towards movement like a real diver. Also make its collision
// shape a circle in the center so it can fit the same regardless of orientation. Put it
// a layer above the environment so its sprite draws over. Rotate with direction or velocity?
public class PlayerMovementState : PlayerState
{
	public float Acceleration { get; set; }
	public string MovementBlendPath { get; } = "parameters/move-blend-tree/move-blend-space/blend_position";
	public Vector2 Direction { get; set; }

	public PlayerMovementState(Player player) : base(player) {}

	public override void EnterState() {}

	public override void ExitState() {}

	public override void ProcessState(double delta)
	{
		// This runs UseAir() in base state class. Might do this differently eventually. 
		TickAir((float)delta);
		HandleAttack((float)delta);
	}

	public override void PhysicsProcessState(double delta)
	{
		float fdelta = (float)delta;
		Direction = Input.GetVector("left", "right", "up", "down").Normalized();
		
		if (Direction == Vector2.Zero)
		{
			Decelerate(fdelta);
		}
		else
		{
			Accelerate(fdelta);
			HandleSpriteFlip();
		}

		Animate();
	}

	private void Decelerate(float fdelta)
	{
		float deceleration = Player.Data.ScubaGear.Deceleration;
		if (Player.Velocity.Length() > (deceleration * fdelta))
		{
			Player.Velocity -= Player.Velocity.Normalized() * (deceleration * fdelta);
		}
		else
		{
			Player.Velocity = Vector2.Zero;
		}
	}
	
	private void Accelerate(float delta)
	{
		float targetAcceleration = AngleToAccel();
		
		Acceleration = Mathf.MoveToward(
			Acceleration, 
			targetAcceleration, 
			(float)delta * Player.Data.ScubaGear.Jerk);
			
		Player.Velocity += Direction * /* target */Acceleration * delta;
		
		Player.Velocity = Player.Velocity.LimitLength(Player.Data.ScubaGear.MaxSpeed);
	}
	
	/// <summary>
	/// Translates angle between direction and Velocity into a number between 
	/// AccelerationMin and AccelerationMax, with smaller angles giving higher acceleration. 
	/// </summary>
	private float AngleToAccel()
	{
		Vector2 normVel = Player.Velocity.Normalized();
		float dot = normVel.Dot(Direction);
		return 0.5f * 
			(Player.Data.ScubaGear.MaxAcceleration - Player.Data.ScubaGear.MinAcceleration) * 
			(1 - dot) + 
			Player.Data.ScubaGear.MinAcceleration;
	}

	private void HandleSpriteFlip()
	{
		if (Direction.X != 0)
		{
			Player.Sprite.FlipH = Direction.X < 0;
		}
	}

	private void Animate()
	{
		Player.AnimationTree.Set(MovementBlendPath, Direction);
	}
	
	public override void Jump()
	{
		base.Jump();
	}

	public override void MaybeTakeDamage(int damage)
	{
		TakeDamage(damage);
	}
/* 
	public override void TakeDamage(int damage)
	{
		base.TakeDamage(damage);
	} */
}
