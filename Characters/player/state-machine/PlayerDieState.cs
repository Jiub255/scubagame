using Godot;

public class PlayerDieState : PlayerState
{
	private float DeathDuration { get; } = 1.5f;
	private float DeathTimer { get; set; } = 0f;
	private float DeathFloatSpeed { get; set; } = 50f;
	
	public PlayerDieState(Player player) : base(player) {}

	public override void EnterState()
	{
		SetupAnimation();
		SetupDeath();
	}

	private void SetupAnimation()
	{
		Player.Sprite.FlipV = true;
		animationStateMachinePlayback.Travel("take-damage");
	}

	private void SetupDeath()
	{
		DeathTimer = DeathDuration;
		Player.Velocity = Vector2.Up * DeathFloatSpeed;
		// TODO: Change collision layers so player doesn't hit enemies but does hit sky.
		Player.SetCollisionMaskValue(6, true);
	}

	public override void ProcessState(double delta)
	{
		TickDeath(delta);
	}

	private void TickDeath(double delta)
	{
		DeathTimer -= (float)delta;
		if (DeathTimer <= 0f)
		{
			Die();
		}
	}

    private void Die()
	{
		// Just resetting scene for now.
		// What to keep when you die? 
		Player.Data.ResetPlayerData();
		Player.Inventory.ClearInventory();
		Player.GetTree().ReloadCurrentScene();
	}

	public override void ExitState() {}

	public override void PhysicsProcessState(double delta) {}

	public override void MaybeTakeDamage(int damage)
	{
		// Don't take damage in this state.
	}
}
