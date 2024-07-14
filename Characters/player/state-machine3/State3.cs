using Godot;
using System;

public abstract class State3<T> where T : CharacterBody2D
{
	public event Action<State3<T>> OnStateChanged;
	
	protected T CharacterBody2D { get; }
	
	public State3(T characterBody2D)
	{
		CharacterBody2D = characterBody2D;
	}
	
	public abstract void EnterState();
	public abstract void ExitState();
	public abstract void ProcessState(double delta);
	public abstract void PhysicsProcessState(double delta);
	protected void ChangeState(State3<T> newState)
	{
		OnStateChanged?.Invoke(newState);
	}
}
