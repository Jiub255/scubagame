using Godot;
using System;

[GlobalClass]
public partial class IdleWaypointState : EnemyStateNode
{
	[Export]
	public Godot.Collections.Array<Node2D> Waypoints { get; set; }
	[Export]
	public Godot.Collections.Array<Vector2> WaypointPositions { get; set; }

	private int Index { get; set; } = 0;
	
	public override void EnterState()
	{
		
	}

	public override void ExitState()
	{
		
	}

	public override void PhysicsProcessState(double delta)
	{
		if (Waypoints.Count > 0)
		{
			Vector2 direction = Waypoints[Index].Position - Enemy.Position;
			direction = direction.Normalized();
			
		}
		else
		{
			GD.PushWarning($"No waypoints exist for {Enemy.Name}");
		}
	}

	public override void ProcessState(double delta)
	{
		
	}
}
