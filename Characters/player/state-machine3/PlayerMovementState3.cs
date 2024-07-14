using Godot;
using System;

public class PlayerMovementState3 : PlayerActionState, IDamageable, ICanMove, ICanAttack
{
	public float Acceleration { get; set; }
	public string MovementBlendPath { get; } = "parameters/move-blend-tree/move-blend-space/blend_position";
	public Vector2 Direction { get; set; }
	
	public PlayerMovementState3(Player3 player) : base(player)
	{
	}

	public override void EnterState() {}
	
	public override void ExitState() {}
	
	public void HandleAttack()
	{
		GD.Print($"Gun acquired: {CharacterBody2D.Data.HarpoonGun.GunAcquired}");
		GD.Print($"Reloading: {CharacterBody2D.Data.Reloading}");
		GD.Print($"Attack pressed: {Input.IsActionPressed("attack")}");
		
		if (CharacterBody2D.Data.HarpoonGun.GunAcquired &&
			!CharacterBody2D.Data.Reloading &&
			Input.IsActionPressed("attack"))
		{
			Attack();
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
	
	public void HandleMovement()
	{
		Direction = Input.GetVector("left", "right", "up", "down").Normalized();
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

	public override void ProcessState(double delta)
	{
	}

	public void TakeDamage(int damage, Vector2 knockbackDirection)
	{
		CharacterBody2D.Data.Health -= damage;
		if (CharacterBody2D.Data.Health <= 0)
		{
			ChangeState(new PlayerDieState3(CharacterBody2D));
		}
		else 
		{
			// Maybe set outside of if block if you want death state to have knockback. 
			CharacterBody2D.Data.KnockbackDirection = knockbackDirection;
			ChangeState(new PlayerTakeDamageState3(CharacterBody2D));
		}
	}
}
