using Godot;

[GlobalClass]
public partial class ChaseState : EnemyStateNode
{
	private int Speed { get; set; }

	public override void InitializeState(Enemy enemy)
	{
		base.InitializeState(enemy);

		Speed = Enemy.Data.ChaseSpeed;
	}

	public override void EnterState()
	{
		
	}

	public override void ExitState()
	{
		
	}

	public override void PhysicsProcessState(double delta)
	{
		Vector2 direction = (Target.GlobalPosition - Enemy.GlobalPosition).Normalized();
		this.PrintDebug($"{Target.Name}: {Target.GlobalPosition}, Enemy: {Enemy.GlobalPosition}, Direction: {direction}");
		Enemy.Velocity = Speed * direction;
	}

	public override void ProcessState(double delta)
	{
		
	}
}
