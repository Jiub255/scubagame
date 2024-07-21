using Godot;

[GlobalClass]
public partial class IdleWanderRandomState : EnemyStateNode
{
	[Export]
	private float MinWanderTime { get; set; } = 0.1f;
	[Export]
	private float MaxWanderTime { get; set; } = 1.0f;
	private float Speed { get; set; }
	private RandomNumberGenerator RNG { get; set; }
	private Vector2 RandomDirection { get; set; }
	private float Timer { get; set; }

	public override void InitializeState(EnemyIdleChase enemy)
	{
		base.InitializeState(enemy);

		Speed = Enemy.Data.IdleSpeed;
		RNG = new RandomNumberGenerator();
	}

	public override void EnterState()
	{
		ChooseRandomDirection();
		ChooseRandomDuration();
	}

	private void ChooseRandomDirection()
	{
		RandomDirection = Vector2.Up.Rotated(RNG.RandfRange(-Mathf.Pi, Mathf.Pi));
	}
	
	private void ChooseRandomDuration()
	{
		Timer = RNG.RandfRange(MinWanderTime, MaxWanderTime);
	}

	public override void ExitState()
	{
		
	}

	public override void PhysicsProcessState(double delta)
	{
		Timer -= (float)delta;
		if (Timer <= 0)
		{
			Timer = 0;
			ChooseRandomDirection();
			ChooseRandomDuration();
		}
		else
		{
			Enemy.Velocity = RandomDirection * Speed;
		}
	}

	public override void ProcessState(double delta)
	{
		
	}
}
