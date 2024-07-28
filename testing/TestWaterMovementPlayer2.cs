using Godot;

public partial class TestWaterMovementPlayer2 : CharacterBody2D
{
	[Export]
	private PlayerData Data;
	private Vector2 Direction { get; set; }
	private float Acceleration { get; set; }
	private float InitialSpeed { get; set; }
	private float TimeElapsed { get; set; } = 0f;
	private float DecayRate { get; set; } = 0.95f;
	private bool AcceleratingLastFrame { get; set; } = false;
	private float MinSpeed { get; set; } = 20f;

	public override void _PhysicsProcess(double delta)
	{
		base._PhysicsProcess(delta);

		GetMovementInput();
		Move((float)delta);
		this.PrintDebug($"Speed: {Velocity.Length()}");
	}

	public void GetMovementInput()
	{
		Direction = Input.GetVector("left", "right", "up", "down").Normalized();
	}

	public void Move(float delta)
	{
		if (Direction == Vector2.Zero)
		{
			if (AcceleratingLastFrame)
			{
				InitialSpeed = Velocity.Length();
				TimeElapsed = 0f;
			}
			Decelerate(delta);
			AcceleratingLastFrame = false;
		}
		else
		{
			Accelerate(delta);
			AcceleratingLastFrame = true;
		}
		MoveAndSlide();
	}

	private void Decelerate(float delta)
	{
		TimeElapsed += delta;
		float speed = InitialSpeed * Mathf.Pow(1 - DecayRate, TimeElapsed);
		if (speed < MinSpeed)
		{
			speed = 0f;
		}
		Velocity = Velocity.Normalized() * speed;
	}
	
	private void Accelerate(float delta)
	{
		float targetAcceleration = AngleToAccel();

		Acceleration = Mathf.MoveToward(
			Acceleration, 
			targetAcceleration, 
			(float)delta * Data.ScubaGear.Jerk);

		Velocity += Direction * targetAcceleration * delta;
		
		Velocity = Velocity.LimitLength(Data.ScubaGear.MaxSpeed);
	}
	
	// TODO: Work on this method to make tighter movement. 
	// TODO: Compare direction vector to movement vector, NOT normalized. Use magnitude and angle in comparison,
	// So velocity doesn't jump from min to max instantly when turning around. 
	// MAYBE use dot product on non-normalized velocity and direction, since it takes magnitude and angle into account. 
	/// <summary>
	/// Translates angle between direction and Velocity into a number between 
	/// AccelerationMin and AccelerationMax, with smaller angles giving higher acceleration. 
	/// </summary>
	private float AngleToAccel()
	{
		Vector2 normVel = Velocity.Normalized();
		float dot = normVel.Dot(Direction);
		this.PrintDebug($"Non-normalized dot product: {Velocity.Dot(Direction)}, Normalized: {dot}");
		// Scales dot product to number between min and max acceleration.
		float acceleration = 0.5f * 
			(Data.ScubaGear.MaxAcceleration - Data.ScubaGear.MinAcceleration) * 
			(1 - dot) + 
			Data.ScubaGear.MinAcceleration;
		this.PrintDebug($"Angle: {Mathf.Abs(Mathf.RadToDeg(normVel.AngleTo(Direction)))}, Acceleration: {acceleration}");
		return acceleration;
	}
}
