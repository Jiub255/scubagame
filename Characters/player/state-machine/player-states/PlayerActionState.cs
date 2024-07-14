public abstract class PlayerActionState : PlayerState
{
	protected PlayerActionState(Player player) : base(player)
	{
	}

	public override void ProcessState(double delta)
	{
		TickAttackTimer((float)delta);
	}
	
	// Run this in PlayerActionState in ProcessState, so changing action states doesn't
	// pause the timer. 
	private void TickAttackTimer(float delta)
	{
		if (CharacterBody2D.Data.Reloading)
		{
			CharacterBody2D.Data.AttackTimer -= (float)delta;
			if (CharacterBody2D.Data.AttackTimer <= 0)
			{
				CharacterBody2D.Data.Reloading = false;
			}
		}
	}
}
