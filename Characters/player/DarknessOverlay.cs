using Godot;

// TODO: Maybe change color as you go deeper too? Less red, more blue, like real water?
public partial class DarknessOverlay : CanvasLayer
{
	private Player Player { get; set; }
	private ColorRect ColorRect { get; set; }
	private float WaterLevel { get; set; }
	[Export]
	private float ExtinctionCoefficient { get; set; } = 0.1f;
	[Export(PropertyHint.Range, "0.0, 1.0,")]
	private float IntensityMultiplier { get; set; } = 0.1f;
	// TODO: Derive this from water level and max depth vars instead, 
	// so that it hits MaxAlpha at the very bottom, and zero at sea level. 
	[Export]
	private float DepthMultiplier { get; set; } = 0.0059f;
	[Export(PropertyHint.Range, "0.0, 1.0,")]
	private float MinAlpha { get; set; } = 0.2f;
	[Export(PropertyHint.Range, "0.0, 1.0,")]
	private float MaxAlpha { get; set; } = 0.86f;
	[Export]
	private Color WaterColor { get; set; }
	private RayCast2D RayCast { get; set; }
	
	public override void _Ready()
	{
		base._Ready();

		GetCamera();
		
		CallDeferred(MethodName.GetWaterLevel);
		
		RayCast = GetNode<RayCast2D>("RayCast2D");
		
		ColorRect = GetNode<ColorRect>("ColorRect");
		ColorRect.Color = WaterColor;
		
		if (MinAlpha >= MaxAlpha)
		{
			MinAlpha = MaxAlpha * 0.2f;
		}
	}
	
	private void GetCamera()
	{
		Player = GetParent() as Player;
	}
	
	private void GetWaterLevel()
	{
		if (RayCast.IsColliding())
		{
			Vector2 collisionPoint = RayCast.GetCollisionPoint();
			WaterLevel = collisionPoint.Y;
			this.PrintDebug($"Water level set to {WaterLevel}");
		}
		else
		{
			WaterLevel = 0;
			GD.PushWarning("No water level found, setting to 0.");
		}
		RayCast.QueueFree();
	}

	public override void _PhysicsProcess(double delta)
	{
		base._PhysicsProcess(delta);

		float depth = Player.GlobalPosition.Y - WaterLevel;
		SetAlpha(depth);
		//this.PrintDebug($"Depth: {depth}, Alpha: {ColorRect.Color.A}");
	}
	
	private void SetAlpha(float depth)
	{
		if (depth <= 0)
		{
			ColorRect.Color = NewColorWithAlpha(0);
		}
		else
		{
			float intensity = IntensityMultiplier * Mathf.Exp(depth * DepthMultiplier * ExtinctionCoefficient);
			ColorRect.Color = NewColorWithAlpha(Mathf.Min(intensity, MaxAlpha));
		}
	}
	
	private Color NewColorWithAlpha(float alpha)
	{
		return new Color(
			ColorRect.Color.R,
			ColorRect.Color.G,
			ColorRect.Color.B,
			alpha
		);
	}
}
