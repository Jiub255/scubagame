using Godot;

/// <summary>
/// Currently keeping all methods for movement, attack, and taking damage here, then only
/// calling them through interfaces on the concrete states that inherit this class. 
/// Helps reuse code, not sure if there's a better way. 
/// </summary>
public abstract class PlayerActionState : PlayerState
{
	private Vector2 Direction { get; set; }
	private float Acceleration { get; set; }
	private string MovementBlendPath { get; } = "parameters/move-blend-tree/move-blend-space/blend_position";
	
	protected PlayerActionState(Player player) : base(player) {}
	
	#region MOVEMENT
	
	public void GetMovementInput()
	{
		Direction = Input.GetVector("left", "right", "up", "down").Normalized();
	}

	public void Move(float delta)
	{
		if (Direction == Vector2.Zero)
		{
			Decelerate(delta);
		}
		else
		{
			Accelerate(delta);
			HandleSpriteFlip();
		}

		Animate();
	}

	private void Decelerate(float delta)
	{
		float deceleration = CharacterBody2D.Data.ScubaGear.Deceleration;
		if (CharacterBody2D.Velocity.Length() > (deceleration * delta))
		{
			CharacterBody2D.Velocity -= CharacterBody2D.Velocity.Normalized() * (deceleration * delta);
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
	
	#endregion
	
	#region ATTACK

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
	
	public void HandleAttack()
	{		
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
	#endregion

	#region TAKEDAMAGE
	
	public void TakeDamage(int damage, Vector2 knockbackDirection)
	{
		CharacterBody2D.Data.Health -= damage;
		if (CharacterBody2D.Data.Health <= 0)
		{
			CharacterBody2D.Data.Health = 0;
			ChangeState(new PlayerDeathState(CharacterBody2D));
		}
		else 
		{
			// Maybe set outside of if block if you want death state to have knockback. 
			CharacterBody2D.Data.KnockbackDirection = knockbackDirection;
			ChangeState(new PlayerKnockbackState(CharacterBody2D));
		}
	}
	
	#endregion
}
 