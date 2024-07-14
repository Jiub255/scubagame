using Godot;
using System;

public class TestAirState : BasePlayerState
{
	private float Gravity { get; } = 175f;
	private RandomNumberGenerator RNG { get; } 
	
	public TestAirState(Player player) : base(player)
	{
		RNG = new RandomNumberGenerator();
	}
	
	public override void EnterState()
	{
		// TODO: 50% chance of flipping sprite when jumping. 
		CharacterBody2D.Sprite.FlipV = RNG.Randf() < 0.5f;
		animationStateMachinePlayback.Travel("take-damage");
		CharacterBody2D.Data.RefillAir();
	}

	public override void ExitState()
	{
		CharacterBody2D.Sprite.FlipV = false;
		animationStateMachinePlayback.Travel("move-blend-tree");	
	}

	public override void HandleInput()
	{
		SubState?.HandleInput();
	}

	public override void HandleMovement()
	{
		// No movement controls while in Air state. 
	}

	public override void PhysicsProcessState(double delta)
	{
		CharacterBody2D.Velocity += Gravity * Vector2.Down * (float)delta;
		SubState?.PhysicsProcessState(delta);
	}

	public override void ProcessState(double delta)
	{
		SubState?.ProcessState(delta);
	}
}
