using Godot;
using System;

public class AirState : PlayerLocationState, ICanEnterWater
{
	private float Gravity { get; } = 175f;
	private RandomNumberGenerator RNG { get; } 
	
	public AirState(Player3 characterBody2D) : base(characterBody2D)
	{
		RNG = new RandomNumberGenerator();
	}

	public override bool CanMove => false;

	public override void EnterState()
	{
		// 50% chance of flipping sprite when jumping. 
		CharacterBody2D.Sprite.FlipV = RNG.Randf() < 0.5f;
		animationStateMachinePlayback.Travel("take-damage");
		CharacterBody2D.Data.RefillAir();
	}

	public override void ExitState()
	{
		CharacterBody2D.Sprite.FlipV = false;
		animationStateMachinePlayback.Travel("move-blend-tree");	
	}

	public override void PhysicsProcessState(double delta)
	{
		CharacterBody2D.Velocity += Gravity * Vector2.Down * (float)delta;
	}

	public override void ProcessState(double delta)
	{
	}

    public void EnterWater()
    {
		ChangeState(new WaterState(CharacterBody2D));
    }
}
