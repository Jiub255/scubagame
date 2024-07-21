using Godot;

[GlobalClass]
public abstract partial class EnemyStateNode : Node2D
{
	protected EnemyIdleChase Enemy { get; private set; }
	public Player Target { get; set; }
	protected SpriteAnimator SpriteAnimator { get; private set; }
	
	public virtual void InitializeState(EnemyIdleChase enemy)
	{
		Enemy = enemy;
		SpriteAnimator = enemy.GetNode<SpriteAnimator>("Sprite2D");
	}

	public virtual void ProcessState(double delta)
	{
		Animate();
		HandleSpriteFlip();
	}
	
	public virtual void Animate()
	{
		if (Enemy.Velocity == Vector2.Zero)
		{
			SpriteAnimator.Play("idle");
		}
		else if (Enemy.Velocity.Y < 0)
		{
			SpriteAnimator.Play("swim-up");
		}
		else
		{
			SpriteAnimator.Play("swim-down");
		}
	}
	
	private void HandleSpriteFlip()
	{
		if (Enemy.Velocity.X != 0)
		{
			SpriteAnimator.FlipH = Enemy.Velocity.X < 0;
		}
	}

	public abstract void EnterState();
	public abstract void ExitState();
	public abstract void PhysicsProcessState(double delta);
}
