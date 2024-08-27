using Godot;

[GlobalClass]
public partial class SpriteAnimator : Sprite2D
{
	private AnimationPlayer AnimationPlayer { get; set; }

	public override void _Ready()
	{
		base._Ready();
		
		AnimationPlayer = GetNode<AnimationPlayer>("AnimationPlayer");
	}
	
	public void Play(StringName animationName)
	{
		AnimationPlayer.Play(animationName);
	}
}
