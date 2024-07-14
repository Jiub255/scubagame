using Godot;

public partial class FishChase : EnemySightRange
{
	public override void ChasePhysicsProcess(double delta)
	{
		Vector2 direction = (Target.Position - Position).Normalized();
		Velocity = Speed * direction;
	
		HandleSpriteFlip();
	}

	public override void IdlePhysicsProcess(double delta)
	{
		Velocity = Vector2.Zero;
	}
	
	public override void Animate()
	{
		if (Velocity == Vector2.Zero)
		{
			AnimationPlayer.Play("idle");
		}
		else if (Velocity.Y < 0)
		{
			AnimationPlayer.Play("swim-up");
		}
		else
		{
			AnimationPlayer.Play("swim-down");
		}
	}

	private void HandleSpriteFlip()
	{
		if (Velocity.X != 0)
		{
			Sprite.FlipH = Velocity.X < 0;
		}
	}
}
