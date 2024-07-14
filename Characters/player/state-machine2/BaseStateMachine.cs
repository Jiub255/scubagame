using Godot;
using System;

// Not sure how to make this work with generics. 
public class BaseStateMachine<T> where T : CharacterBody2D
{
	public BaseState<T> CurrentState { get; private set; }
	public BaseStateFactory<T> Factory { get; set; }
	
	public BaseStateMachine(T characterBody2D)
	{
		/* Factory = new BaseStateFactory<T>(characterBody2D);
		Factory.OnStateChanged += ChangeState; */
	}
	
	public virtual void ExitTree()
	{
		//Factory.OnStateChanged -= ChangeState;
	}
	
	public void ChangeState(BaseState<T> newState)
	{		
		if (CurrentState == newState)
		{
			return;
		}
		BaseState<T> subState = null;
		BaseState<T> newSubState = null;
		if (CurrentState != null)
		{
			subState = CurrentState.SubState;
			CurrentState.ExitState();
		}
		CurrentState = newState;
		if (subState != CurrentState.SubState)
		{
			// Don't call ChangeSubState because we don't want to run enter/exit,
			// since the substate isn't changing. 
			CurrentState.SubState = newSubState;
		}
		CurrentState.EnterState();
		GD.Print($"State: {CurrentState}");
	}

	public void ProcessState(double delta)
	{
		if (CurrentState != null)
		{
			CurrentState.HandleInput();
			CurrentState.ProcessState(delta);
		}
	}
	
	public void PhysicsProcessState(double delta)
	{
		if (CurrentState != null)
		{
			CurrentState.PhysicsProcessState(delta);
		}
	}
}
