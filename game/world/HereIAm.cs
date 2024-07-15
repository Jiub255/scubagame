using Godot;
using System;

public partial class HereIAm : Node2D
{
	public refctest refCounted = new refctest();
	
	public override void _PhysicsProcess(double delta)
	{
		base._PhysicsProcess(delta);

		this.PrintDebug($"Position: {Position}");
		refCounted.printtest();
	}
}
