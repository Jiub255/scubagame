using Godot;

public abstract class PlayerState3 : State3<Player3>
{
	protected AnimationNodeStateMachinePlayback animationStateMachinePlayback { get; set; }
	
	public PlayerState3(Player3 player) : base(player)
	{
		animationStateMachinePlayback = 
			(AnimationNodeStateMachinePlayback)CharacterBody2D.AnimationTree.Get("parameters/playback");
	}
}
