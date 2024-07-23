using Godot;

public partial class DepthFinder : RayCast2D
{	private Player Player { get; set; }
	private float WaterLevel { get; set; }
	
	public override void _Ready()
	{
		base._Ready();

		Player = GetParent() as Player;
		
		CallDeferred(MethodName.GetWaterLevel);
	}
	
	private void GetWaterLevel()
	{
		ForceRaycastUpdate();
		if (IsColliding())
		{
			Vector2 collisionPoint = GetCollisionPoint();
			WaterLevel = collisionPoint.Y;
			this.PrintDebug($"Water level set to {WaterLevel}");
		}
		else
		{
			WaterLevel = 0;
			GD.PushWarning("No water level found, setting to 0.");
		}
	}

	public override void _PhysicsProcess(double delta)
	{
		base._PhysicsProcess(delta);

		Player.Data.Depth = Player.GlobalPosition.Y - WaterLevel;
		//this.PrintDebug($"Depth: {Player.Data.Depth}");
	}
}
