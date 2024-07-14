using Godot;
/// <summary>
/// Inheriting Recovery state since it holds all the movement code. Might be bad practice, seems sloppy. 
/// But what? Have a MovingActionState instead that this and recovery inherit?
/// Or put every method on the base class and call everything through interfaces? That might work actually.  
/// </summary>
public class PlayerMovementState : PlayerRecoveryState, IDamageable, ICanMove, ICanAttack
{
	public PlayerMovementState(Player player) : base(player)
	{
	}

	public override void EnterState() {}
	
	public override void ExitState() {}
	
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
	

	public void TakeDamage(int damage, Vector2 knockbackDirection)
	{
		CharacterBody2D.Data.Health -= damage;
		if (CharacterBody2D.Data.Health <= 0)
		{
			ChangeState(new PlayerDeathState(CharacterBody2D));
		}
		else 
		{
			// Maybe set outside of if block if you want death state to have knockback. 
			CharacterBody2D.Data.KnockbackDirection = knockbackDirection;
			ChangeState(new PlayerKnockbackState(CharacterBody2D));
		}
	}
}
