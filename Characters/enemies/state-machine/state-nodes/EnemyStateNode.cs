using Godot;

[GlobalClass]
public abstract partial class EnemyStateNode : Node2D
{
	protected EnemyIdleChase Enemy { get; private set; }
	public Player Target { get; set; }
	protected Graphics Graphics { get; private set; }
	
	public virtual void InitializeState(EnemyIdleChase enemy)
	{
		Enemy = enemy;
		Graphics = enemy.GetNode<Graphics>("Graphics");
	}

	public abstract void EnterState();
	public abstract void ExitState();
	public abstract void ProcessState(double delta);
	public abstract void PhysicsProcessState(double delta);
	
	public virtual void Animate()
	{
		if (Enemy.Velocity == Vector2.Zero)
		{
			Graphics.Play("idle");
		}
		else if (Enemy.Velocity.Y < 0)
		{
			Graphics.Play("swim-up");
		}
		else
		{
			Graphics.Play("swim-down");
		}
	}
}
