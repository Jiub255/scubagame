using Godot;

public partial class ScubaGearCollectable : Area2D, ICollectable
{
	[Export]
	private ScubaGearData _scubaGearData;
	public ScubaGearData ScubaGearData { get { return _scubaGearData; } }
	
	public void GetCollected(Player3 player)
	{
		player.Data.ScubaGear = ScubaGearData;
		QueueFree();
	}
}
