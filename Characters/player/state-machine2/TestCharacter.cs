using Godot;
using System;

public partial class TestCharacter : CharacterBody2D
{/* 
	private BaseStateDecider Decider { get; set; }
	private BaseStateMachine Machine { get; set; }

	public override void _Ready()
	{
		base._Ready();
		
		// TODO: OR, have the machine create and hold the decider? Then you can attach different deciders for different characters, and just reuse the basic machine. 
		Decider = new BaseStateDecider(this);
		Machine = Decider.BaseStateMachine;
	} */
}
