using Godot;
using System;

public partial class TestDecider : BaseStateDecider
{
	public TestDecider(
		CharacterBody2D characterBody2D,
		TestWaterState testState,
		TestAirState testState2
		) : base(characterBody2D)
	{
		
	}
}
