using Godot;

[GlobalClass]
public abstract partial class EnemyStateNode : Node2D
{
	public Enemy Enemy { get; set; }
	public Player Target { get; set; }
	protected AnimationPlayer AnimationPlayer { get; private set; }
	
	public virtual void InitializeState(Enemy enemy)
	{
		Enemy = enemy;
		AnimationPlayer = Enemy.GetNode<AnimationPlayer>("AnimationPlayer");
	}

	public abstract void EnterState();
	public abstract void ExitState();
	public abstract void ProcessState(double delta);
	public abstract void PhysicsProcessState(double delta);
	
	public void Animate()
	{
		if (Enemy.Velocity == Vector2.Zero)
		{
			AnimationPlayer.Play("idle");
		}
		else if (Enemy.Velocity.Y < 0)
		{
			AnimationPlayer.Play("swim-up");
		}
		else
		{
			AnimationPlayer.Play("swim-down");
		}
	}
}
