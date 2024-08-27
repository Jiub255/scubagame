using Godot;

public partial class TrueIdleState : EnemyIdleState
{
	public override void EnterState()
	{
		Enemy.Velocity = Vector2.Zero;
	}
	public override void ExitState() {}
	public override void PhysicsProcessState(double delta) {}
}
