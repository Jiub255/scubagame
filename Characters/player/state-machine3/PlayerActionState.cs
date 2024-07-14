using Godot;
using System;

public abstract class PlayerActionState : PlayerState3
{
	protected PlayerActionState(Player3 player) : base(player)
	{
	}

	public abstract override void EnterState();
	public abstract override void ExitState();
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
	public abstract override void PhysicsProcessState(double delta);
}
