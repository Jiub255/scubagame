using Godot;

// TODO: Should you be able to attack in recovery state?
public class PlayerRecoveryState : PlayerActionState, ICanMove
{
	private float RecoveryTimer { get; set; } = 0f;
	private float RecoveryDuration { get; } = 2f;
	
	public PlayerRecoveryState(Player player) : base(player) {}

	public override void EnterState()
	{
		RecoveryTimer = RecoveryDuration;
	}

	public override void PhysicsProcessState(double delta) 
	{
		RecoveryTimer -= (float)delta;
		if (RecoveryTimer <= 0f)
		{
			ChangeState(new PlayerMovementState(CharacterBody2D));
		}
	}

	public override void ExitState() {}
}
