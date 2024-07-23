using Godot;

public partial class WorldColorModulate : CanvasModulate
{
	[Export]
	private PlayerData PlayerData { get; set; }
	
	[Export]
	private Color AboveWater { get; set; }  = new Color(1, 1, 1, 1);
	[Export]
	private Gradient Gradient { get; set; }
	[Export]
	private float MaxDepth { get; set; } = 4000f;
	
	public override void _PhysicsProcess(double delta)
	{
		base._PhysicsProcess(delta);

		Color = DepthToColor(PlayerData.Depth);
	}
	
	private Color DepthToColor(float depth)
	{
		if (depth <= 0)
		{
			return AboveWater;
		}
		else
		{
			float scaledDepth = depth / MaxDepth;
			//this.PrintDebug($"Scaled depth: {scaledDepth}");
			return Gradient.Sample(scaledDepth);
		}
	}
}
