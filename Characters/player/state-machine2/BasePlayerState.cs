using Godot;
using System;

public abstract class BasePlayerState : BaseState<Player>
{
	public event Action<States> OnStateChanged;
	
	protected AnimationNodeStateMachinePlayback animationStateMachinePlayback { get; set; }
	
	public BasePlayerState(Player player) : base(player)
	{
		animationStateMachinePlayback = (AnimationNodeStateMachinePlayback)player.AnimationTree.Get("parameters/playback");
	}

	public abstract override void EnterState();
	public abstract override void ExitState();
	public abstract override void HandleInput();
	public abstract override void HandleMovement();
	public abstract override void PhysicsProcessState(double delta);
	public abstract override void ProcessState(double delta);
}
