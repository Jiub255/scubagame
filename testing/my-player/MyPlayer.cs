using System;
using Godot;

public partial class MyPlayer : CharacterBody2D, ICanMove
{
	[Export]
	private float Acceleration { get; set; } = 2000f;
	[Export]
	private float DragMultiplier { get; set; } = 0.01f;
	[Export]
	private float MinSpeed { get; set; } = 35f;
	private float Drag
	{
		get
		{
			return DragMultiplier * Velocity.LengthSquared();
		}
	}
	private AnimationPlayer AnimationPlayer { get; set; }
	private Vector2 Direction { get; set; }
	private AnimationTree AnimationTree { get; set; }

	public override void _Ready()
	{
		AnimationPlayer = GetNode<AnimationPlayer>("AnimationPlayer");
		AnimationTree = GetNode<AnimationTree>("AnimationTree");
		//AnimationPlayer.Play("swim-left");
	}

	public override void _PhysicsProcess(double delta)
	{
		GetMovementInput();
		Move((float)delta);
		Animate();
	}

	private void HandleFlip()
	{
		if (Velocity.X > 0)
		{
			Scale = new Vector2(1, -1);
		}
		else
		{
			Scale = new Vector2(1, 1);
		}
	}

	public void GetMovementInput()
	{
		Direction = Input.GetVector("left", "right", "up", "down");
	}

	public void Move(float delta)
	{
		if (Direction == Vector2.Zero)
		{
			Decelerate();
		}
		else
		{
			Accelerate(delta);
			RotateDiver();
			HandleFlip();
		}
		
		Velocity -= Velocity.Normalized() * Drag * delta;
		
		this.PrintDebug($"Speed: {Velocity.Length()}");
		MoveAndSlide();
	}
	
	private void Decelerate()
	{
		if (Velocity.Length() < MinSpeed)
		{
			Velocity = Vector2.Zero;
		}
	}
	
	private void Accelerate(float delta)
	{
		Velocity += Direction * Acceleration * delta;
	}
	
	private void Animate()
	{
		// TODO: Need max speed to set blend position. Or could just set a lower than actual max value
		// and have anything faster just be fully swimming animation. 
		float maxSpeedGuess = 240f;
		AnimationTree.Set("parameters/blend_position", Mathf.Min(1, Velocity.Length() / maxSpeedGuess));
	}
	
	private void RotateDiver()
	{
		Rotation = Velocity.Angle() + (Mathf.Pi);
	}
}
