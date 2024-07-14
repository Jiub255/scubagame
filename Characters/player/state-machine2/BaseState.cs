using Godot;
using System;

/// <summary>
/// Subscribe to character body collision events in the concrete states, then send
/// appropriate event up to the decider. 
/// </summary>
public abstract class BaseState<T> where T : CharacterBody2D
{
	//public event Action<BaseState<T>> OnStateChanged;
	
	protected T CharacterBody2D { get; }
	public BaseState<T> SubState { get; set; }
	
	public BaseState(T characterBody2D)
	{
		CharacterBody2D = characterBody2D;
	}
	
	public abstract void EnterState();
	public abstract void ExitState();
	public abstract void HandleInput();
	public abstract void HandleMovement();
	public abstract void ProcessState(double delta);
	public abstract void PhysicsProcessState(double delta);
	
/* 	public void ChangeState(BaseState<T> newState)
	{
		OnStateChanged?.Invoke(newState);
	} */
	public void ChangeSubState(BaseState<T> newState)
	{
		if (SubState == newState)
		{
			return;
		}
		if (SubState != null)
		{
			SubState.ExitState();
		}
		SubState = newState;
		SubState.EnterState();
	}
}
