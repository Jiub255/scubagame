using Godot;

// TODO: Maybe add AttackRange and AttackState? Different behavior for close range. 
// Also, Get hit state, "revenge" state (where they chase you from further out than 
// their usual sight range because you shot them), Die/drop loot state.
// TODO: Might be better to separate out the machine once it gets too complicated. 
public partial class Enemy : CharacterBody2D
{
	[Export]
	public EnemyStateNode IdleState { get; private set; }
	[Export]
	public EnemyStateNode ChaseState { get; private set; }
	// Make an enemy data resource?
	[Export]
	public int Damage { get; private set; } = 1;

	private EnemyStateNode CurrentState { get; set; }

	private Area2D SightRange { get; set; }
	//private Area2D AttackRange { get; set; }
	
	// Called by Enemy controller script (parent of state machine).
	public override void _Ready()
	{
		base._Ready();
		
		SightRange = GetNode<Area2D>("SightRange");
		
		IdleState.InitializeState(this);
		ChaseState.InitializeState(this);

		SightRange.BodyEntered += (body) => ChangeState(body, ChaseState);
		SightRange.BodyExited += (body) => ChangeState(body, IdleState);
		
		CurrentState = IdleState;
	}

	public override void _ExitTree()
	{
		base._ExitTree();
		
		SightRange.BodyEntered -= (body) => ChangeState(body, ChaseState);
		SightRange.BodyExited -= (body) => ChangeState(body, IdleState);
	}
	
	private void ChangeState(Node2D body, EnemyStateNode newState)
	{
		if (newState == CurrentState) return;
		if (body is Player player)
		{
			CurrentState?.ExitState();
			CurrentState = newState;
			CurrentState?.EnterState();
			CurrentState.Target = player;
		}
	}
	
	public override void _Process(double delta)
	{
		base._Process(delta);

		CurrentState?.ProcessState(delta);
	}

	public override void _PhysicsProcess(double delta)
	{
		base._PhysicsProcess(delta);

		CurrentState?.PhysicsProcessState(delta);

		KinematicCollision2D collision = MoveAndCollide(Velocity * (float)delta);
		HandleCollision(collision);
	}
	
	private void HandleCollision(KinematicCollision2D collision)
	{
		GodotObject collider = collision?.GetCollider();
		if (collider is Player player)
		{
			HitPlayer(player);
		}
	}

	private void HitPlayer(Player player)
	{
		Vector2 knockbackDirection = (player.Position - Position).Normalized();
		player.TakeDamage(Damage, knockbackDirection);
	}
}
