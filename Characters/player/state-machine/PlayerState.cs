using Godot;
using System;

public abstract class PlayerState
{
	public event Action<PlayerStateNames> OnStateChanged;
	
	private Player _player;
	protected Player Player { get { return _player; } }
	
	protected AnimationNodeStateMachinePlayback animationStateMachinePlayback { get; set; }
	
	public PlayerState(Player player)
	{
		_player = player;
		animationStateMachinePlayback = (AnimationNodeStateMachinePlayback)Player.AnimationTree.Get("parameters/playback");
	}
	
	public abstract void EnterState();
	public abstract void ExitState();

	// TODO: Figure out the best way to do shared methods between classes.
	// Trying four different ways in this script right now. 
	// Could also try using interfaces but then would have to implement them 
	// individually in each state that uses them. Might try some more inheritance. 
	// Try drawing out the states on paper and their transitions. 
	// Call base.ProcessState in classes you want to use air in. 
	public abstract void ProcessState(double delta);
	public abstract void PhysicsProcessState(double delta);
	public abstract void MaybeTakeDamage(int damage);

	/// <summary>
	/// Put this in MaybeTakeDamage in states you want to be able to take damage in. 
	/// </summary>
	protected void TakeDamage(int damage)
	{
		Player.Data.Health -= damage;
		if (Player.Data.Health <= 0)
		{
			ChangeState(PlayerStateNames.DIE);
		}
		else 
		{
			ChangeState(PlayerStateNames.TAKEDAMAGE);
		}
	}
	
	// This seems like the best way for process methods. 
	// Could do the same with one off methods, just put an abstract shell method
	// that gets called, and only fill it with these common state methods in the 
	// appropriate states. 
	/// <summary>
	/// Put this inside ProcessState in states you want to be able to attack in. 
	/// </summary>
	protected void HandleAttack(float delta)
	{
		if (Player.Data.HarpoonGun.GunAcquired)
		{
			if (Player.Data.Reloading)
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
		Player.Data.AttackTimer -= (float)delta;
		if (Player.Data.AttackTimer <= 0)
		{
			Player.Data.Reloading = false;
		}
	}
	
	private void Attack()
	{
		Player.HarpoonGun.Shoot(
			Player.Data.HarpoonGun.Damage, 
			Player.Data.HarpoonGun.Speed);
		Player.Data.Reloading = true;
		Player.Data.AttackTimer = Player.Data.HarpoonGun.AttackTimerLength;
	}
	
	// TODO: How to handle these situations better? Don't want to override a bunch of
	// methods just to not use them. 
	/// <summary>
	/// Override this in states you DON'T want to be able to jump/land in. 
	/// </summary>
	public virtual void Jump()
	{
		ChangeState(PlayerStateNames.AIR);
	}
	
	/// <summary>
	/// Override this in states you DON'T want to be able to jump/land in. 
	/// </summary>
	public virtual void Land()
	{
		ChangeState(PlayerStateNames.MOVEMENT);
	}
	
	protected void ChangeState(PlayerStateNames state)
	{
		OnStateChanged?.Invoke(state);
	}
	
	/// <summary>
	/// Put this inside ProcessState in states you want to be able to use up air in. 
	/// </summary>
	protected void TickAir(float delta)
	{
		Player.Data.AirTimer -= (float)delta;
		if (Player.Data.AirTimer <= 0)
		{
			Player.Data.AirTimer = 1;
			Player.Data.Air--;
			if (Player.Data.Air <= 0)
			{
				ChangeState(PlayerStateNames.DIE);
			}
		}
	}
}
