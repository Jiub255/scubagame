using Godot;
using System;

public abstract class StateMachine3<T> where T : CharacterBody2D
{
	public virtual State3<T> CurrentState { get; protected set; }
	
	public StateMachine3(T character)
	{
		
	}
	
	public void ProcessState(double delta)
	{
		CurrentState?.ProcessState(delta);
	}
	
	public void PhysicsProcessState(double delta)
	{
		CurrentState?.PhysicsProcessState(delta);
	}
	
	public void ChangeState(State3<T> newState)
	{
		GD.Print($"Old state : {CurrentState}");
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
		GD.Print($"New state : {CurrentState}");
	}
}
