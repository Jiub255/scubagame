using Godot;

public partial class Jellyfish : EnemySightRange
{
	private RandomNumberGenerator RNG { get; set; }
	private Vector2 RandomDirection { get; set; }
	private float Timer { get; set; }
	private float RandomDuration { get; set; }
	
	public override void _Ready()
	{
		base._Ready();

		AnimationPlayer.Play("swim");
		RNG = new RandomNumberGenerator();
		SetupNextMovement();
	}

	public override void Animate()
	{
	}

	public override void ChasePhysicsProcess(double delta)
	{
		Vector2 direction = (Target.Position - Position).Normalized();
		Velocity = Speed * direction;
	}

	public override void IdlePhysicsProcess(double delta)
	{
		Timer -= (float)delta;
		if (Timer > 0)
		{
			Velocity = RandomDirection * Speed;
		}
		else
		{
			SetupNextMovement();
		}
	}
	
	private void SetupNextMovement()
	{
		RandomDirection = Vector2.Up.Rotated(RNG.RandfRange(-Mathf.Pi, Mathf.Pi));
		Timer = RNG.RandfRange(1f, 3f);
	}
}
