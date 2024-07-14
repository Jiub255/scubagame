using Godot;
using System;

// Character only needs a reference to the machine, not the decider. To run processes. 
public partial class BaseStateDecider : Node
{
	public BaseStateDecider(CharacterBody2D characterBody2D)
	{
		// Subscribe to concrete state events. That is, do the deciding. 
	}
}
