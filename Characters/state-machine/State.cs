using Godot;
using System;

public abstract class State<T> where T : CharacterBody2D
{
	public event Action<State<T>> OnStateChanged;
	
	protected T CharacterBody2D { get; }
	
	public State(T characterBody2D)
	{
		CharacterBody2D = characterBody2D;
	}
	
	public abstract void EnterState();
	public abstract void ExitState();
	public abstract void ProcessState(double delta);
	public abstract void PhysicsProcessState(double delta);
	protected void ChangeState(State<T> newState)
	{
		OnStateChanged?.Invoke(newState);
	}
}
