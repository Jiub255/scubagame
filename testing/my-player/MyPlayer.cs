using Godot;

public partial class MyPlayer : CharacterBody2D, ICanMove
{
	[Export]
	private float MaxSpeed { get; set; } = 2300f;
	[Export]
	private float Acceleration { get; set; } = 2000f;
	[Export]
	private float DragMultiplier { get; set; } = 0.01f;
	[Export]
	private float MinSpeed { get; set; } = 35f;
	[Export]
	private float RotationSpeed { get; set; } = 180f;

	private float Drag
	{
		get
		{
			return DragMultiplier * Velocity.LengthSquared();
		}
	}
	private bool FacingLeft { get; set; } = true;
	private AnimationPlayer AnimationPlayer { get; set; }
	private Vector2 Direction { get; set; }
	private AnimationTree AnimationTree { get; set; }

	public override void _Ready()
	{
		AnimationPlayer = GetNode<AnimationPlayer>("AnimationPlayer");
		AnimationTree = GetNode<AnimationTree>("AnimationTree");
	}

	public override void _PhysicsProcess(double delta)
	{
		GetMovementInput();
		Move((float)delta);
		Animate();
	}

	public void GetMovementInput()
	{
		Direction = Input.GetVector("left", "right", "up", "down");
	}

	public void Move(float delta)
	{
		if (Direction == Vector2.Zero)
		{
			Decelerate();
		}
		else
		{
			Accelerate(delta);
		}
		RotateDiver(delta);
		HandleFlip();
		HandleDrag(delta);
		this.PrintDebug($"Speed: {Velocity.Length()}");
		MoveAndSlide();
	}
	
	private void Animate()
	{
		AnimationTree.Set("parameters/blend_position", Mathf.Min(1, Velocity.Length() / MaxSpeed));
	}

	private void Decelerate()
	{
		if (Velocity.Length() < MinSpeed)
		{
			Velocity = Vector2.Zero;
		}
	}
	
	private void Accelerate(float delta)
	{
		Velocity += Direction * Acceleration * delta;
		Velocity = Velocity.LimitLength(MaxSpeed);
	}
	
	private void RotateDiver(float delta)
	{
		if (Velocity != Vector2.Zero)
		{
			Rotation = Mathf.LerpAngle(Rotation, Velocity.Angle() + Mathf.Pi, delta * RotationSpeed);
		}
	}

	private void HandleFlip()
	{
		if (FacingLeft && Velocity.X > 0)
		{
			FacingLeft = false;
			Scale = new Vector2(Scale.X, -Scale.Y);
		}
		else if (!FacingLeft && Velocity.X < 0)
		{
			FacingLeft = true;
			Scale = new Vector2(Scale.X, -Scale.Y);
		}
	}

	private void HandleDrag(float delta)
	{
		Velocity -= Velocity.Normalized() * Drag * delta;
	}
}
