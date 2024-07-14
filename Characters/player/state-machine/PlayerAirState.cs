using Godot;

public partial class PlayerAirState : PlayerState
{
	private float Gravity { get; } = 175f;
	private RandomNumberGenerator RNG { get; } 
	
	public PlayerAirState(Player player) : base(player)
	{
		RNG = new RandomNumberGenerator();
	}

	public override void EnterState()
	{
		// TODO: 50% chance of flipping sprite when jumping. 
		Player.Sprite.FlipV = RNG.Randf() < 0.5f;
		animationStateMachinePlayback.Travel("take-damage");
		Player.Data.RefillAir();
		// TODO: Set initial launch vector based off last velocity. 
		// Or will it just stay from the last frame? I think it should. 
	}

	public override void ExitState()
	{
		Player.Sprite.FlipV = false;
		animationStateMachinePlayback.Travel("move-blend-tree");	
	}

	public override void ProcessState(double delta)
	{
		//HandleAttack((float)delta);
	}

	public override void PhysicsProcessState(double delta)
	{
		Player.Velocity += Gravity * Vector2.Down * (float)delta;
	}

	public override void Land()
	{
		base.Land();
	}

	public override void MaybeTakeDamage(int damage)
	{
		// Don't take damage in this state, for now. Gonna redo with 2 machines. 
	}
}
