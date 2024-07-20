using Godot;

public partial class DarknessOverlay : CanvasLayer
{
	private Camera2D Camera { get; set; }
	private ColorRect ColorRect { get; set; }
	[Export]
	private float WaterLevel { get; set; } = -536f;
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
	
	public override void _Ready()
	{
		base._Ready();

		CallDeferred(MethodName.GetCamera);
		ColorRect = GetNode<ColorRect>("ColorRect");
		ColorRect.Color = WaterColor;
		
		if (MinAlpha >= MaxAlpha)
		{
			MinAlpha = MaxAlpha * 0.2f;
		}
	}
	
	private void GetCamera()
	{
		Camera = GetParent() as Camera2D;
	}

	public override void _PhysicsProcess(double delta)
	{
		base._PhysicsProcess(delta);

		float depth = Camera.GlobalPosition.Y - WaterLevel;
		if (depth <= 0)
		{
			ColorRect.Color = NewColor(0);
		}
		else
		{
			float intensity = IntensityMultiplier * Mathf.Exp(depth * DepthMultiplier * ExtinctionCoefficient);
			ColorRect.Color = NewColor(Mathf.Min(intensity, MaxAlpha));
		}
		this.PrintDebug($"Depth: {depth}, Alpha: {ColorRect.Color.A}");
	}
	
	private Color NewColor(float alpha)
	{
		return new Color(
			ColorRect.Color.R,
			ColorRect.Color.G,
			ColorRect.Color.B,
			alpha
		);
	}
}
