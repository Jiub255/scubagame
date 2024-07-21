using Godot;

[GlobalClass]
public partial class ChaseState : EnemyChaseState
{
	public override void PhysicsProcessState(double delta)
	{
		Vector2 direction = (Target.GlobalPosition - Enemy.GlobalPosition).Normalized();
		//this.PrintDebug($"{Target.Name}: {Target.GlobalPosition}, Enemy: {Enemy.GlobalPosition}, Direction: {direction}");
		Enemy.Velocity = Speed * direction;
	}

	public override void EnterState() {}
	public override void ExitState() {}
}
