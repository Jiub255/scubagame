using Godot;
using System;

public class PlayerTakeDamageState3 : PlayerActionState
{
	public float KnockbackTimer { get; set; } = 0f;
	public float KnockbackDuration { get; } = 1f;
	public float KnockbackSpeed { get; } = 75f;
	
	public PlayerTakeDamageState3(Player3 player) : base(player)
	{
	}

	public override void EnterState()
	{
		SetupAnimation();
		SetupKnockback();
	}

	private void SetupAnimation()
	{
		CharacterBody2D.Sprite.FlipH = CharacterBody2D.Data.KnockbackDirection.X > 0;
		animationStateMachinePlayback.Travel("take-damage");
	}

	private void SetupKnockback()
	{
		KnockbackTimer = KnockbackDuration;
		Vector2 knockback = CharacterBody2D.Data.KnockbackDirection * KnockbackSpeed;
		CharacterBody2D.Velocity = knockback;
	}

	public override void ExitState()
	{
		animationStateMachinePlayback.Travel("move-blend-tree");	
	}

	public override void ProcessState(double delta) 
	{
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
		CharacterBody2D.Velocity = Vector2.Zero;
		ChangeState(new PlayerMovementState3(CharacterBody2D));
	}
}
