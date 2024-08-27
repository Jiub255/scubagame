using Godot;

public abstract class PlayerState : State<Player>
{
	protected AnimationNodeStateMachinePlayback animationStateMachinePlayback { get; set; }
	
	public PlayerState(Player player) : base(player)
	{
		animationStateMachinePlayback = 
			(AnimationNodeStateMachinePlayback)CharacterBody2D.AnimationTree.Get("parameters/playback");
	}
}
