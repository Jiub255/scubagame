using Godot;
using System;

public class BaseStateFactory<T> where T : CharacterBody2D
{
	public event Action<BaseState<T>> OnStateChanged;
	
	public BaseStateDecider Decider { get; set; }
	
	public BaseStateFactory(T characterBody2D)
	{
		CreateStates(characterBody2D);
	}

	public virtual void CreateStates(T characterBody2D) { }
	
	public void ChangeState(BaseState<T> state)
	{
		OnStateChanged?.Invoke(state);
	}
}
