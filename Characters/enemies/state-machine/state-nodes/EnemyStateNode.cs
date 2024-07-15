using Godot;

[GlobalClass]
public abstract partial class EnemyStateNode : Node2D
{
	public Enemy Enemy { get; set; }
	public Player Target { get; set; }
	
	public virtual void InitializeState(Enemy enemy)
	{
		Enemy = enemy;
	}

	public abstract void EnterState();
	public abstract void ExitState();
	public abstract void ProcessState(double delta);
	public abstract void PhysicsProcessState(double delta);
}
