using Godot;
using System.Linq;

[GlobalClass]
public partial class IdleWaypointState : EnemyStateNode
{
	[Export]
	private Node2D[] Waypoints { get; set; }
	[Export]
	private float _changeWaypointsRadius = 0.1f;
	
	private float ChangeWaypointsRadiusSquared 
	{
		get
		{
			return _changeWaypointsRadius * _changeWaypointsRadius;
		}
	}
	private Vector2[] Positions { get; set; }
	private int Index { get; set; } = 0;
	private int Speed { get; set; }

	public override void InitializeState(Enemy enemy)
	{
		base.InitializeState(enemy);

		Positions = Waypoints.Select(x => x.Position).ToArray();
		Speed = Enemy.IdleSpeed;
	}

	public override void EnterState()
	{
		
	}

	public override void ExitState()
	{
		
	}

	public override void PhysicsProcessState(double delta)
	{
		if (Positions.Length > 1)
		{
			Vector2 direction = Positions[Index] - Enemy.Position;
			if (direction.LengthSquared() < ChangeWaypointsRadiusSquared)
			{
				Index = (Index + 1) % Positions.Length;
				this.PrintDebug($"Index: {Index}");
			}
			
			Enemy.Velocity = direction.Normalized() * Speed;
		}
		else if (Positions.Length == 1)
		{
			GD.PushWarning($"Only one waypoint exists for {Enemy.Name}");
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
