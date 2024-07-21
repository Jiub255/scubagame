using Godot;

// TODO: Maybe add AttackRange and AttackState? Different behavior for close range. 
// Also, Get hit state, "revenge" state (where they chase you from further out than 
// their usual sight range because you shot them), Die/drop loot state.
// TODO: Might be better to separate out the machine once it gets too complicated. 
public partial class EnemyIdleChase : CharacterBody2D, IDamageable
{
	[Export]
	public EnemyData Data { get; private set; }
	[Export]
	public EnemyStateNode IdleState { get; private set; }
	[Export]
	public EnemyStateNode ChaseState { get; private set; }

	private EnemyStateNode CurrentState { get; set; }

	private Area2D SightRange { get; set; }
	
	// Called by Enemy controller script (parent of state machine).
	public override void _Ready()
	{
		base._Ready();
		
		SightRange = GetNode<Area2D>("SightRange");
		
		IdleState.InitializeState(this);
		ChaseState.InitializeState(this);

		SightRange.BodyEntered += Chase; /* (body) => ChangeState(body, ChaseState); */
		SightRange.BodyExited += Idle; //(body) => ChangeState(body, IdleState);
		
		CurrentState = IdleState;
	}

	public override void _ExitTree()
	{
		SightRange.BodyEntered -= Chase; //(body) => ChangeState(body, ChaseState);
		SightRange.BodyExited -= Idle; //(body) => ChangeState(body, IdleState);
		
		base._ExitTree();
	}
	
	private void Chase(Node2D body)
	{
		ChangeState(body, ChaseState);
	}
	
	private void Idle(Node2D body)
	{
		ChangeState(body, IdleState);
	}
	
	private void ChangeState(Node2D body, EnemyStateNode newState)
	{
		if (newState == CurrentState) return;
		if (body is Player player)
		{
			this.PrintDebug($"Old state: {CurrentState.Name}");
			CurrentState?.ExitState();
			CurrentState = newState;
			CurrentState?.EnterState();
			CurrentState.Target = player;
			this.PrintDebug($"New state: {CurrentState.Name}");
		}
	}
	
	// TODO: Might not need process at all for this machine.
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
		player.TakeDamage(Data.Damage, knockbackDirection);
	}

	public void TakeDamage(int damage, Vector2 knockbackDirection)
	{
		Data.Health -= damage;
		if (Data.Health <= 0)
		{
			QueueFree();
		}
	}
}
