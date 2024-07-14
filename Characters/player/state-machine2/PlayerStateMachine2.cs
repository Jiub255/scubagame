using Godot;
using System;

public partial class PlayerStateMachine2 : BaseStateMachine<Player>
{
	public PlayerStateMachine2(Player characterBody2D) : base(characterBody2D)
	{
		Factory = new PlayerStateFactory2(characterBody2D);
		Factory.OnStateChanged += ChangeState;
	}

	public override void ExitTree()
	{
		base.ExitTree();
		
		Factory.OnStateChanged -= ChangeState;
	}
	
}
