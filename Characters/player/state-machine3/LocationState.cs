using Godot;
using System;

public abstract class LocationState<T> : State3<T> where T : CharacterBody2D
{
    protected LocationState(T characterBody2D) : base(characterBody2D)
    {
    }

    public abstract bool CanMove { get; }
	public abstract bool CanAttack { get; }
	
	public abstract override void EnterState();
	public abstract override void ExitState();
	public abstract override void ProcessState(double delta);
	public abstract override void PhysicsProcessState(double delta);
}
