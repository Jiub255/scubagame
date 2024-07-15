using Godot;

public abstract partial class EnemyStateNode : Node
{
	public CharacterBody2D Enemy { get; set; }
	public Player Target { get; set; }
	
	public virtual void InitializeState(CharacterBody2D enemy)
	{
		Enemy = enemy;
	}

	public abstract void EnterState();
	public abstract void ExitState();
	public abstract void ProcessState(double delta);
	public abstract void PhysicsProcessState(double delta);
}
