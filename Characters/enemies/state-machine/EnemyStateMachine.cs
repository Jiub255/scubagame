using Godot;

public partial class EnemyStateMachine : Node2D
{
	[Export]
	public EnemyStateNode IdleState { get; private set; }
	[Export]
	public EnemyStateNode ChaseState { get; private set; }
	[Export]
	public EnemyStateNode AttackState { get; private set; }

	private EnemyStateNode CurrentState { get; set; }

	private Area2D SightRange { get; set; }
	private Area2D AttackRange { get; set; }
	
	// Called by Enemy controller script (parent of state machine).
	public void InitializeStates(CharacterBody2D enemy)
	{
		SightRange = enemy.GetNode<Area2D>("SightRange");
		AttackRange = enemy.GetNode<Area2D>("AttackRange");
		
		IdleState.InitializeState(enemy);
		ChaseState.InitializeState(enemy);
		AttackState.InitializeState(enemy);

		SightRange.BodyEntered += (body) => ChangeState(body, ChaseState);
		SightRange.BodyExited += (body) => ChangeState(body, IdleState);
		
		AttackRange.BodyEntered += (body) => ChangeState(body, AttackState);
		AttackRange.BodyExited += (body) => ChangeState(body, ChaseState);

		CurrentState = IdleState;
	}

	public override void _ExitTree()
	{
		base._ExitTree();
		
		SightRange.BodyEntered -= (body) => ChangeState(body, ChaseState);
		SightRange.BodyExited -= (body) => ChangeState(body, IdleState);
		
		AttackRange.BodyEntered -= (body) => ChangeState(body, AttackState);
		AttackRange.BodyExited -= (body) => ChangeState(body, ChaseState);
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
}
