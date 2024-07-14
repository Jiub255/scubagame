using Godot;
using System;

public class TestMovementState : BasePlayerState
{	
	public float Acceleration { get; set; }
	public string MovementBlendPath { get; } = "parameters/move-blend-tree/move-blend-space/blend_position";
	public Vector2 Direction { get; set; }

	public TestMovementState(Player CharacterBody2D) : base(CharacterBody2D)
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
		if (Input.IsActionJustPressed("attack"))
		{
			
		}
	}
	protected void HandleAttack(float delta)
	{
		if (CharacterBody2D.Data.HarpoonGun.GunAcquired)
		{
			if (CharacterBody2D.Data.Reloading)
			{
				TickAttackTimer(delta);
			}
			else if (Input.IsActionPressed("attack"))
			{
				Attack();
			}	
		}
	}
	
	private void TickAttackTimer(float delta)
	{
		CharacterBody2D.Data.AttackTimer -= (float)delta;
		if (CharacterBody2D.Data.AttackTimer <= 0)
		{
			CharacterBody2D.Data.Reloading = false;
		}
	}
	
	private void Attack()
	{
		CharacterBody2D.HarpoonGun.Shoot(
			CharacterBody2D.Data.HarpoonGun.Damage, 
			CharacterBody2D.Data.HarpoonGun.Speed);
		CharacterBody2D.Data.Reloading = true;
		CharacterBody2D.Data.AttackTimer = CharacterBody2D.Data.HarpoonGun.AttackTimerLength;
	}
	public override void HandleMovement()
	{
		Direction = Input.GetVector("left", "right", "up", "down").Normalized();
	}

	public override void ProcessState(double delta)
	{
		// This runs UseAir() in base state class. Might do this differently eventually. 
		HandleAttack((float)delta);
	}

	public override void PhysicsProcessState(double delta)
	{
		float fdelta = (float)delta;
		
		if (Direction == Vector2.Zero)
		{
			Decelerate(fdelta);
		}
		else
		{
			Accelerate(fdelta);
			HandleSpriteFlip();
		}

		Animate();
	}

	private void Decelerate(float fdelta)
	{
		float deceleration = CharacterBody2D.Data.ScubaGear.Deceleration;
		if (CharacterBody2D.Velocity.Length() > (deceleration * fdelta))
		{
			CharacterBody2D.Velocity -= CharacterBody2D.Velocity.Normalized() * (deceleration * fdelta);
		}
		else
		{
			CharacterBody2D.Velocity = Vector2.Zero;
		}
	}
	
	private void Accelerate(float delta)
	{
		float targetAcceleration = AngleToAccel();

        Acceleration = Mathf.MoveToward(
            Acceleration, 
			targetAcceleration, 
			(float)delta * CharacterBody2D.Data.ScubaGear.Jerk);
			
		CharacterBody2D.Velocity += Direction * /* target */Acceleration * delta;
		
		CharacterBody2D.Velocity = CharacterBody2D.Velocity.LimitLength(CharacterBody2D.Data.ScubaGear.MaxSpeed);
	}
	
	/// <summary>
	/// Translates angle between direction and Velocity into a number between 
	/// AccelerationMin and AccelerationMax, with smaller angles giving higher acceleration. 
	/// </summary>
	private float AngleToAccel()
	{
        Vector2 normVel = CharacterBody2D.Velocity.Normalized();
		float dot = normVel.Dot(Direction);
		return 0.5f * 
			(CharacterBody2D.Data.ScubaGear.MaxAcceleration - CharacterBody2D.Data.ScubaGear.MinAcceleration) * 
			(1 - dot) + 
			CharacterBody2D.Data.ScubaGear.MinAcceleration;
	}

	private void HandleSpriteFlip()
	{
		if (Direction.X != 0)
		{
			CharacterBody2D.Sprite.FlipH = Direction.X < 0;
		}
	}

	private void Animate()
	{
		CharacterBody2D.AnimationTree.Set(MovementBlendPath, Direction);
	}
}
