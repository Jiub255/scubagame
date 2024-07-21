using Godot;

public partial class AmbushState : EnemyChaseState
{
	[Export]
	private float LaunchSpeedMult { get; set; } = 2f;
	[Export]
	private float TimeBetweenLaunches { get; set; } = 1.5f;
	private float Timer { get; set; }
	
	public override void EnterState()
	{
		Launch();
	}
	
	public override void PhysicsProcessState(double delta)
	{
		Timer -= (float)delta;
		if (Timer < 0)
		{
			Launch();
		}
	}
	
	private void Launch()
	{
		Vector2 direction = (Target.Position - Enemy.Position).Normalized();
		Enemy.Velocity = direction * Speed * LaunchSpeedMult;
		Timer = TimeBetweenLaunches;
	}
	
	public override void ExitState()
	{
		Enemy.Velocity = Vector2.Zero;
	}
}
