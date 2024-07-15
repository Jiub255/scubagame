using Godot;

public abstract class StateMachine<T> where T : CharacterBody2D
{
	public virtual State<T> CurrentState { get; protected set; }
	
	public StateMachine(T character) {}
	
	public void ProcessState(double delta)
	{
		CurrentState?.ProcessState(delta);
	}
	
	public void PhysicsProcessState(double delta)
	{
		CurrentState?.PhysicsProcessState(delta);
	}
	
	public void ChangeState(State<T> newState)
	{
		//this.PrintDebug($"Old state: {CurrentState}");
		if (CurrentState == newState)
		{
			return;
		}
		if (CurrentState != null)
		{
			CurrentState.ExitState();
			CurrentState.OnStateChanged -= ChangeState;
		}
		CurrentState = newState;
		if (CurrentState != null)
		{
			CurrentState.OnStateChanged += ChangeState;
			CurrentState.EnterState();
		}
		this.PrintDebug($"New state: {CurrentState}");
	}
}
