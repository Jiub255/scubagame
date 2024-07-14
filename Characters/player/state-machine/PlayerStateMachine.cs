using Godot;

/// <summary>
/// TODO: Pump input from InputManager directly into state machine, then feed that into current state. 
/// </summary>
public class PlayerStateMachine
{
	public PlayerState LocationState { get; private set; }
	public PlayerState CurrentState { get; private set; }
	
	private PlayerStateFactory StateFactory { get; set; }
	
	public PlayerStateMachine(Player player)
	{
		//GD.Print("State machine created");
		StateFactory = new PlayerStateFactory(player);
		StateFactory.OnStateChanged += ChangeState;
		ChangeState(PlayerStateNames.MOVEMENT);
	}
	
	public void ExitTree()
	{
		StateFactory.ExitTree();
		StateFactory.OnStateChanged -= ChangeState;
	}
	
	public void ChangeState(PlayerStateNames stateName)
	{		
		PlayerState newState = StateFactory.StateDict[stateName];
		
		if (CurrentState == newState)
		{
			return;
		}
		if (CurrentState != null)
		{
			CurrentState.ExitState();
		}
		CurrentState = newState;
		CurrentState.EnterState();
		GD.Print($"State: {CurrentState}");
	}

	// TODO: Pass input into process methods. But how? What data type? Input event? 	
	public void ProcessState(double delta)
	{
		if (CurrentState != null)
		{
			CurrentState.ProcessState(delta);
		}
	}
	
	public void PhysicsProcessState(double delta)
	{
		//GD.Print($"Current state: {CurrentState}");
		if (CurrentState != null)
		{
			CurrentState.PhysicsProcessState(delta);
		}
	}
}
