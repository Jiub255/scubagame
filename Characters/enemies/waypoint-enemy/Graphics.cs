using Godot;
using System;

public partial class Graphics : Node2D
{
	private AnimationPlayer AnimationPlayer { get; set; }
	private Sprite2D Sprite { get; set; }

	public override void _Ready()
	{
		base._Ready();
		
		AnimationPlayer = GetNode<AnimationPlayer>("AnimationPlayer");
		Sprite = GetNode<Sprite2D>("Sprite2D");
	}
	
	public void Play(StringName animationName)
	{
		AnimationPlayer.Play(animationName);
	}
}
