using Godot;

public class PlayerTakeDamageState : PlayerState
{
	public float KnockbackTimer { get; set; } = 0f;
	public float KnockbackDuration { get; } = 1f;
	public float KnockbackSpeed { get; } = 75f;
	
	public PlayerTakeDamageState(Player player) : base(player) {}

	public override void EnterState()
	{
		SetupAnimation();
		SetupKnockback();
	}

	private void SetupAnimation()
	{
		Player.Sprite.FlipH = Player.Data.KnockbackDirection.X > 0;
		animationStateMachinePlayback.Travel("take-damage");
	}

	private void SetupKnockback()
	{
		KnockbackTimer = KnockbackDuration;
		Vector2 knockback = Player.Data.KnockbackDirection * KnockbackSpeed;
		Player.Velocity = knockback;
	}

	public override void ExitState()
	{
		animationStateMachinePlayback.Travel("move-blend-tree");	
	}

	public override void ProcessState(double delta) 
	{
		TickAir((float)delta);
	}

	public override void PhysicsProcessState(double delta)
	{
		TickKnockback(delta);
	}

	private void TickKnockback(double delta)
	{
		KnockbackTimer -= (float)delta;
		if (KnockbackTimer <= 0f)
		{
			EndKnockback();
		}
	}

	private void EndKnockback()
	{
		Player.Velocity = Vector2.Zero;
		ChangeState(PlayerStateNames.MOVEMENT);
	}

    public override void MaybeTakeDamage(int damage)
    {
        // Don't take damage in this state.
    }
}
