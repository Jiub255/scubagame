using Godot;
using System;

public class TestWaterState : BasePlayerState
{
	public TestWaterState(Player player) : base(player)
	{
		
	}
	
	public override void EnterState()
	{
		
	}

	public override void ExitState()
	{
		
	}

	public override void HandleInput()
	{
		SubState?.HandleInput();
	}

	public override void HandleMovement()
	{
		SubState?.HandleMovement();
	}

	public override void PhysicsProcessState(double delta)
	{
		SubState?.PhysicsProcessState(delta);
	}

	public override void ProcessState(double delta)
	{
		SubState?.ProcessState(delta);
		TickAir((float)delta);
	}
	
	protected void TickAir(float delta)
	{
		CharacterBody2D.Data.AirTimer -= (float)delta;
		if (CharacterBody2D.Data.AirTimer <= 0)
		{
			CharacterBody2D.Data.AirTimer = 1;
			CharacterBody2D.Data.Air--;
			if (CharacterBody2D.Data.Air <= 0)
			{
				//ChangeState(CharacterBody2D.Machine.Factory.DieState);
			}
		}
	}
}
