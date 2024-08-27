public class PlayerMovementState : PlayerActionState, IDamageable, ICanMove, ICanAttack
{
	public PlayerMovementState(Player player) : base(player) {}

	public override void EnterState() {}
	public override void ExitState() {}
	public override void PhysicsProcessState(double delta) {}
}
