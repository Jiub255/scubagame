using Godot;
using System;

public partial class MyPlayer : CharacterBody2D
{
	[Export]
	private float Speed { get; set; } = 10000f;
	private AnimationPlayer AnimationPlayer { get; set; }

	public override void _Ready()
	{
		AnimationPlayer = GetNode<AnimationPlayer>("AnimationPlayer");
		AnimationPlayer.Play("swim-left");
	}

	public override void _PhysicsProcess(double delta)
	{
		//base._PhysicsProcess(delta);
		
		if (Input.IsActionPressed("left"))
		{
			this.PrintDebug($"Left pressed");
			Velocity = Vector2.Left * Speed * (float)delta;
		}
		else
		{
			Velocity = Vector2.Zero;
		}
		MoveAndSlide();
	}
}
